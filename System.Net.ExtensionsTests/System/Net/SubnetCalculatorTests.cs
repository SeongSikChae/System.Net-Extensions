namespace System.Net.Tests
{
	using Diagnostics;

	[TestClass]
	public class SubnetCalculatorTests
	{
		[TestMethod]
		public void CalculateTest()
		{
			Assert.ThrowsException<InvalidOperationException>(() =>
			{
				SubnetCalculator.Calculate(IPAddress.IPv6Any, IPAddress.Any);
			});

			Assert.ThrowsException<InvalidOperationException>(() =>
			{
				SubnetCalculator.Calculate(IPAddress.Any, IPAddress.IPv6Any);
			});

			SubnetInfo subnetInfo = SubnetCalculator.Calculate(IPAddress.Parse("192.168.0.129"), IPAddress.Parse("255.255.255.128"));
			Assert.AreEqual(IPAddress.Parse("192.168.0.129"), subnetInfo.Address);
			Assert.AreEqual(25, subnetInfo.CidrMask);
			Assert.AreEqual(IPAddress.Parse("192.168.0.128"), subnetInfo.Network);
			Assert.AreEqual(IPAddress.Parse("255.255.255.128"), subnetInfo.Netmask);
			Assert.AreEqual(IPAddress.Parse("192.168.0.129"), subnetInfo.FirstAddress);
			Assert.AreEqual(IPAddress.Parse("192.168.0.254"), subnetInfo.LastAddress);
			Assert.AreEqual(IPAddress.Parse("192.168.0.255"), subnetInfo.Broadcast);
			Assert.AreEqual<uint>(126, subnetInfo.Count);

			Trace.WriteLine(subnetInfo);
		}
	}
}