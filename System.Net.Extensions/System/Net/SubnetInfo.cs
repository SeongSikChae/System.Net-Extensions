using System.Text;

namespace System.Net
{
	/// <summary>
	/// Subnet information calculated by SubnetCalculator
	/// </summary>
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

		/// <summary>
		/// Host Address entered in SubnetCalculator
		/// </summary>
		public IPAddress Address { get; }
		/// <summary>
		/// CIDR Mask Bit
		/// </summary>
		public int CidrMask { get; }
		/// <summary>
		/// Network Address
		/// </summary>
		public IPAddress Network { get; }
		/// <summary>
		/// Netmask Address
		/// </summary>
		public IPAddress Netmask { get; }
		/// <summary>
		/// Broad Address
		/// </summary>
		public IPAddress Broadcast { get; }
		/// <summary>
		/// First Address
		/// </summary>
		public IPAddress FirstAddress { get; }
		/// <summary>
		/// Last Address
		/// </summary>
		public IPAddress LastAddress { get; }
		/// <summary>
		/// Address Count
		/// </summary>
		public uint Count { get; }

		/// <summary>
		/// Output information about SubnetInfo
		/// </summary>
		/// <returns>String format information</returns>
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
