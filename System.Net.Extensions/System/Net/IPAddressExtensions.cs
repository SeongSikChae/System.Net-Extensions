namespace System.Net
{
	using Buffers.Binary;
	using Sockets;
	using Text.RegularExpressions;

	public static class IPAddressExtensions
	{
		internal static void ValidateInterNetwork(IPAddress address)
		{
			if (address.AddressFamily != AddressFamily.InterNetwork)
				throw new InvalidOperationException($"unsupported addressFamily '{Enum.GetName(address.AddressFamily)}'");
		}

		private static uint GetNetworkValue(this IPAddress network, IPAddress netmask)
		{
			ValidateInterNetwork(network);
			ValidateInterNetwork(netmask);
			uint networkValue = BinaryPrimitives.ReadUInt32BigEndian(network.GetAddressBytes());
			uint netmaskValue = BinaryPrimitives.ReadUInt32BigEndian(netmask.GetAddressBytes());
			return networkValue & netmaskValue;
		}

		public static IPAddress GetNetwork(this IPAddress network, IPAddress netmask)
		{
			uint value = network.GetNetworkValue(netmask);
			Span<byte> buffer = stackalloc byte[sizeof(uint)];
			BinaryPrimitives.WriteUInt32BigEndian(buffer, value);
			return new IPAddress(buffer);
		}

		public static bool ValidNetwork(this IPAddress network, IPAddress netmask)
		{
			IPAddress value = network.GetNetwork(netmask);
			return network.Equals(value);
		}

		private static readonly Regex CIDR_PATTERN = new Regex("(?<Network>\\S+)\\/(?<Mask>\\d+)");

		public static bool ValidHost(this IPAddress host, string cidr)
		{
			ValidateInterNetwork(host);
			Match match = CIDR_PATTERN.Match(cidr);
			if (!match.Success)
				throw new InvalidOperationException("invalid cidr");
			IPAddress network = IPAddress.Parse(match.Groups["Network"].Value);
			ValidateInterNetwork(network);
			int mask = int.Parse(match.Groups["Mask"].Value);
			IPAddress netmask = mask.ToSubnetMask(network.AddressFamily);
			network = network.GetNetwork(netmask);
			return host.ValidHost(network, netmask);
		}

		public static bool ValidHost(this IPAddress host, IPAddress network, IPAddress netmask)
		{
			ValidateInterNetwork(network);
			return network.Equals(host.GetNetwork(netmask));
		}

		public static int ToCidr(this IPAddress mask)
		{
			ValidateInterNetwork(mask);
			int cidr = 0;
			foreach (byte value in mask.GetAddressBytes())
				cidr += CountOnBits(value);
			return cidr;
		}

		private static int CountOnBits(int value)
		{
			int count = 0;
			while (value > 0)
			{
				count += (value & 1);
				value >>= 1;
			}
			return count;
		}

		public static IPAddress GetBroadcast(this IPAddress network, IPAddress netmask)
		{
			ValidateInterNetwork(network);
			ValidateInterNetwork(netmask);
			byte[] networkBytes = network.GetAddressBytes();
			byte[] netmaskBytes = netmask.GetAddressBytes();
			byte[] brocastBytes = new byte[networkBytes.Length];
			for (int index = 0; index < networkBytes.Length; index++)
				brocastBytes[index] = (byte)(networkBytes[index] | ~netmaskBytes[index]);
			return new IPAddress(brocastBytes);
		}

		public static IPAddress GetFirstAddress(this IPAddress broadcast, IPAddress network)
		{
			ValidateInterNetwork(broadcast);
			ValidateInterNetwork(network);
			uint broadcastValue = BinaryPrimitives.ReadUInt32BigEndian(broadcast.GetAddressBytes());
			uint networkValue = BinaryPrimitives.ReadUInt32BigEndian(network.GetAddressBytes());
			uint firstValue = broadcastValue - networkValue;
			if (firstValue > 1)
				firstValue = networkValue + 1;
			else
				firstValue = 0;
			Span<byte> buffer = stackalloc byte[sizeof(uint)];
			BinaryPrimitives.WriteUInt32BigEndian(buffer, firstValue);
			return new IPAddress(buffer);
		}

		public static IPAddress GetLastAddress(this IPAddress broadcast, IPAddress network)
		{
			ValidateInterNetwork(broadcast);
			ValidateInterNetwork(network);
			uint broadcastValue = BinaryPrimitives.ReadUInt32BigEndian(broadcast.GetAddressBytes());
			uint networkValue = BinaryPrimitives.ReadUInt32BigEndian(network.GetAddressBytes());
			uint lastValue = broadcastValue - networkValue;
			if (lastValue > 1)
				lastValue = broadcastValue - 1;
			else
				lastValue = 0;
			Span<byte> buffer = stackalloc byte[sizeof(uint)];
			BinaryPrimitives.WriteUInt32BigEndian(buffer, lastValue);
			return new IPAddress(buffer);
		}

		public static uint GetAddressCount(this IPAddress netmask)
		{
			return netmask.ToCidr().GetAddressCount();
		}
	}
}
