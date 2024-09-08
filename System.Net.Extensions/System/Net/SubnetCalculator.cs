namespace System.Net
{
	using Text.RegularExpressions;

	public static class SubnetCalculator
	{
		private static readonly Regex CIDR_PATTERN = new Regex("(?<Network>\\S+)\\/(?<Mask>\\d+)");

		public static SubnetInfo Calculate(IPAddress host, string cidr)
		{
			IPAddressExtensions.ValidateInterNetwork(host);
			Match match = CIDR_PATTERN.Match(cidr);
			if (!match.Success)
				throw new InvalidOperationException("invalid cidr");
			IPAddress network = IPAddress.Parse(match.Groups["Network"].Value);
			IPAddressExtensions.ValidateInterNetwork(network);
			int mask = int.Parse(match.Groups["Mask"].Value);
			IPAddress netmask = mask.ToSubnetMask();
			network = network.GetNetwork(netmask);
			return Calculate(host, network, netmask);
		}

		public static SubnetInfo Calculate(IPAddress host, IPAddress netmask)
		{
			IPAddress network = host.GetNetwork(netmask);
			return Calculate(host, network, netmask);
		}

		private static SubnetInfo Calculate(IPAddress host, IPAddress network, IPAddress netmask)
		{
			IPAddress broadcast = network.GetBroadcast(netmask);
			IPAddress firstAddress = broadcast.GetFirstAddress(network);
			IPAddress lastAddress = broadcast.GetLastAddress(network);
			return new SubnetInfo(host, netmask.ToCidr(), network, netmask, broadcast, firstAddress, lastAddress, netmask.GetAddressCount());
		}
	}
}
