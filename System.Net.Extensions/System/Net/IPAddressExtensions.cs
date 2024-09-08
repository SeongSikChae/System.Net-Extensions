namespace System.Net
{
	using Buffers.Binary;
	using Sockets;
	using Text.RegularExpressions;

	/// <summary>
	/// System.Net.IPAddress Extensions
	/// </summary>
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

		/// <summary>
		/// Get the Network Address to which a specific host address belongs through Subnet.
		/// </summary>
		/// <param name="host">Specific Host</param>
		/// <param name="netmask">Subnet Mask Address</param>
		/// <returns>Network Address</returns>
		public static IPAddress GetNetwork(this IPAddress host, IPAddress netmask)
		{
			uint value = host.GetNetworkValue(netmask);
			Span<byte> buffer = stackalloc byte[sizeof(uint)];
			BinaryPrimitives.WriteUInt32BigEndian(buffer, value);
			return new IPAddress(buffer);
		}

		/// <summary>
		/// Verify that a specific Network Address is valid via Subnet Mask.
		/// </summary>
		/// <param name="network">Network address to be verified</param>
		/// <param name="netmask">Subnet Mask Address</param>
		/// <returns>true if valid, otherwise false</returns>
		public static bool ValidNetwork(this IPAddress network, IPAddress netmask)
		{
			IPAddress value = network.GetNetwork(netmask);
			return network.Equals(value);
		}

		private static readonly Regex CIDR_PATTERN = new Regex("(?<Network>\\S+)\\/(?<Mask>\\d+)");

		/// <summary>
		/// Verify that a specific host address is valid via CIDR Signature.
		/// </summary>
		/// <param name="host">Host address to verify</param>
		/// <param name="cidr">CIDR Signature</param>
		/// <returns>true if valid, otherwise false</returns>
		/// <exception cref="InvalidOperationException">Any AddressFamily other than InetAddress or an invalid CIDR Signature</exception>
		public static bool ValidHost(this IPAddress host, string cidr)
		{
			ValidateInterNetwork(host);
			Match match = CIDR_PATTERN.Match(cidr);
			if (!match.Success)
				throw new InvalidOperationException("invalid cidr");
			IPAddress network = IPAddress.Parse(match.Groups["Network"].Value);
			ValidateInterNetwork(network);
			int mask = int.Parse(match.Groups["Mask"].Value);
			IPAddress netmask = mask.ToSubnetMask();
			network = network.GetNetwork(netmask);
			return host.ValidHost(network, netmask);
		}

		/// <summary>
		/// Verifies that a specific host address is valid via network address and netmask address.
		/// </summary>
		/// <param name="host">Host address to verify</param>
		/// <param name="network">Network Address</param>
		/// <param name="netmask">Netmask Address</param>
		/// <returns>true if valid, otherwise false</returns>
		public static bool ValidHost(this IPAddress host, IPAddress network, IPAddress netmask)
		{
			ValidateInterNetwork(network);
			return network.Equals(host.GetNetwork(netmask));
		}

		/// <summary>
		/// Converts a subnet mask address to a CIDR Mask Bit value.
		/// </summary>
		/// <param name="mask">Subnet mask address to convert</param>
		/// <returns>CIDR Mask Bit</returns>
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

		/// <summary>
		/// Get the broadcast address for a specific network address via the subnet mask address.
		/// </summary>
		/// <param name="network">Specific Network address</param>
		/// <param name="netmask">Subnet Mask Address</param>
		/// <returns>Broadcast Address</returns>
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

		/// <summary>
		/// Get the first host address from the broadcast address using a specific network address.
		/// </summary>
		/// <param name="broadcast">Specific Broadcast Address</param>
		/// <param name="network">Network Address</param>
		/// <returns>First Address</returns>
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

		/// <summary>
		/// Get the last host address from the broadcast address using a specific network address.
		/// </summary>
		/// <param name="broadcast">Specific Broadcast Address</param>
		/// <param name="network">Network Address</param>
		/// <returns>Last Address</returns>
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

		/// <summary>
		/// Gets the number of host addresses from a given netmask.
		/// </summary>
		/// <param name="netmask">Specific Netmask Address</param>
		/// <returns>Host Count</returns>
		public static uint GetAddressCount(this IPAddress netmask)
		{
			return netmask.ToCidr().GetAddressCount();
		}
	}
}
