namespace System.Tests
{
	using Net;

	[TestClass]
	public class Int32ExtensionsTests
	{
		[TestMethod]
		public void ToSubnetMaskTest()
		{
			Assert.ThrowsException<InvalidOperationException>(() =>
			{
				_ = (-1 * 1).ToSubnetMask();
			});

			Assert.ThrowsException<InvalidOperationException>(() =>
			{
				_ = 0.ToSubnetMask();
			});

			Assert.AreEqual(IPAddress.Parse("255.255.255.0"), 24.ToSubnetMask());
			Assert.AreEqual(IPAddress.Parse("255.255.255.128"), 25.ToSubnetMask());
		}

		[TestMethod]
		public void GetAddressCountTest()
		{
			Assert.AreEqual<uint>(126, 25.GetAddressCount());
		}
	}
}