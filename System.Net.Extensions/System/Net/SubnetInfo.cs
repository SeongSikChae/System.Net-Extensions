using System.Text;

namespace System.Net
{
	public sealed class SubnetInfo
	{
		internal SubnetInfo(IPAddress address, int cidrMask, IPAddress network, IPAddress netmask, IPAddress broadcast, IPAddress firstAddress, IPAddress lastAddress, uint count)
		{
			Address = address;
			CidrMask = cidrMask;
			Network = network;
			Netmask = netmask;
			FirstAddress = firstAddress;
			LastAddress = lastAddress;
			Count = count;
			Broadcast = broadcast;
		}

		public IPAddress Address { get; }
		public int CidrMask { get; }
		public IPAddress Network { get; }
		public IPAddress Netmask { get; }
		public IPAddress Broadcast { get; }
		public IPAddress FirstAddress { get; }
		public IPAddress LastAddress { get; }
		public uint Count { get; }

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder("CIDR Signature: [")
				.Append(Address).Append('/').Append(CidrMask).Append("] ")
				.Append("Netmask: [").Append(Netmask).AppendLine("]")
				.Append("Network: [").Append(Network).AppendLine("]")
				.Append("Broadcast: [").Append(Broadcast).AppendLine("]")
				.Append("First Address: [").Append(FirstAddress).AppendLine("]")
				.Append("Last Address: [").Append(LastAddress).AppendLine("]")
				.Append("# Addresses: [").Append(Count).AppendLine("]");
			return builder.ToString();
		}
	}
}
