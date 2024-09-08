using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;

namespace System.Tests
{
	[TestClass]
	public class Int32ExtensionsTests
	{
		[TestMethod]
		public void ToSubnetMaskTest()
		{
			Assert.ThrowsException<InvalidOperationException>(() =>
			{
				_ = (-1 * 1).ToSubnetMask(Net.Sockets.AddressFamily.InterNetwork);
			});

			Assert.ThrowsException<InvalidOperationException>(() =>
			{
				_ = 0.ToSubnetMask(Net.Sockets.AddressFamily.InterNetworkV6);
			});

			Assert.AreEqual(IPAddress.Parse("255.255.255.0"), 24.ToSubnetMask(Net.Sockets.AddressFamily.InterNetwork));
			Assert.AreEqual(IPAddress.Parse("255.255.255.128"), 25.ToSubnetMask(Net.Sockets.AddressFamily.InterNetwork));
		}

		[TestMethod()]
		public void GetAddressCountTest()
		{
			Assert.AreEqual<uint>(126, 25.GetAddressCount());
		}
	}
}