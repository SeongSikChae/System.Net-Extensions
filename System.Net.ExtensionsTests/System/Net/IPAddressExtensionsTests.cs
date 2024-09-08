namespace System.Net.Tests
{
	using Diagnostics;
	using NetworkInformation;

	[TestClass]
	public class IPAddressExtensionsTests
	{
		[TestMethod]
		public void GetNetworkTest()
		{
			{
				IPAddress network = IPAddress.Parse("192.168.0.2");
				IPAddress netmask = IPAddress.Parse("255.255.255.128");
				Assert.AreEqual(IPAddress.Parse("192.168.0.0"), network.GetNetwork(netmask));
			}

			{
				IPAddress network = IPAddress.Parse("192.168.0.130");
				IPAddress netmask = IPAddress.Parse("255.255.255.128");
				Assert.AreEqual(IPAddress.Parse("192.168.0.128"), network.GetNetwork(netmask));
			}

			{
				IPAddress network = IPAddress.IPv6Any;
				IPAddress netmask = IPAddress.Parse("255.255.255.128");
				Assert.ThrowsException<InvalidOperationException>(() =>
				{
					network.GetNetwork(netmask);
				});
			}

			{
				IPAddress netmask = IPAddress.IPv6Any;
				IPAddress network = IPAddress.Parse("255.255.255.128");
				Assert.ThrowsException<InvalidOperationException>(() =>
				{
					network.GetNetwork(netmask);
				});
			}
		}

		[TestMethod]
		public void ValidNetworkTest()
		{
			{
				IPAddress network = IPAddress.Parse("192.168.0.0");
				IPAddress netmask = IPAddress.Parse("255.255.255.128");
				Assert.IsTrue(network.ValidNetwork(netmask));
			}

			{
				IPAddress network = IPAddress.Parse("192.168.0.128");
				IPAddress netmask = IPAddress.Parse("255.255.255.128");
				Assert.IsTrue(network.ValidNetwork(netmask));
			}

			{
				IPAddress network = IPAddress.IPv6Any;
				IPAddress netmask = IPAddress.Parse("255.255.255.128");
				Assert.ThrowsException<InvalidOperationException>(() =>
				{
					network.ValidNetwork(netmask);
				});
			}

			{
				IPAddress netmask = IPAddress.IPv6Any;
				IPAddress network = IPAddress.Parse("255.255.255.128");
				Assert.ThrowsException<InvalidOperationException>(() =>
				{
					network.ValidNetwork(netmask);
				});
			}
		}

		[TestMethod]
		public void ValidHostTest()
		{
			Assert.ThrowsException<InvalidOperationException>(() =>
			{
				IPAddress.IPv6Any.ValidHost("192.168.0.0/25");
			});

			Assert.ThrowsException<InvalidOperationException>(() =>
			{
				IPAddress.Parse("192.168.0.129").ValidHost("192.168.0.128/A");
			});

			Assert.IsTrue(IPAddress.Parse("192.168.0.129").ValidHost("192.168.0.128/25"));
		}

		[TestMethod]
		public void ToCidrTest()
		{
			Assert.ThrowsException<InvalidOperationException>(() =>
			{
				IPAddress.IPv6Any.ToCidr();
			});

			Assert.AreEqual(24, IPAddress.Parse("255.255.255.0").ToCidr());
			Assert.AreEqual(25, IPAddress.Parse("255.255.255.128").ToCidr());
		}

		[TestMethod]
		public void GetBroadcastTest()
		{
			Assert.ThrowsException<InvalidOperationException>(() =>
			{
				IPAddress.IPv6Any.GetBroadcast(IPAddress.Any);
			});

			Assert.ThrowsException<InvalidOperationException>(() =>
			{
				IPAddress.Any.GetBroadcast(IPAddress.IPv6Any);
			});

			Assert.AreEqual(IPAddress.Parse("192.168.0.255"), IPAddress.Parse("192.168.0.128").GetBroadcast(IPAddress.Parse("255.255.255.128")));
		}

		[TestMethod]
		public void GetFirstAddressTest()
		{
			Assert.ThrowsException<InvalidOperationException>(() =>
			{
				IPAddress.IPv6Any.GetFirstAddress(IPAddress.Any);
			});

			Assert.ThrowsException<InvalidOperationException>(() =>
			{
				IPAddress.Any.GetFirstAddress(IPAddress.IPv6Any);
			});

			Assert.AreEqual(IPAddress.Parse("192.168.0.129"), IPAddress.Parse("192.168.0.255").GetFirstAddress(IPAddress.Parse("192.168.0.128")));
		}

		[TestMethod]
		public void GetLastAddressTest()
		{
			Assert.ThrowsException<InvalidOperationException>(() =>
			{
				IPAddress.IPv6Any.GetLastAddress(IPAddress.Any);
			});

			Assert.ThrowsException<InvalidOperationException>(() =>
			{
				IPAddress.Any.GetLastAddress(IPAddress.IPv6Any);
			});

			Assert.AreEqual(IPAddress.Parse("192.168.0.254"), IPAddress.Parse("192.168.0.255").GetLastAddress(IPAddress.Parse("192.168.0.128")));
		}

		[TestMethod]
		public void GetAddressCountTest()
		{
			Assert.ThrowsException<InvalidOperationException>(() =>
			{
				IPAddress.IPv6Any.GetAddressCount();
			});

			Assert.AreEqual<uint>(126, IPAddress.Parse("255.255.255.128").GetAddressCount());
		}

		[TestMethod]
		public void GetLocalHostAddressesTest()
		{
			Assert.ThrowsException<InvalidOperationException>(() =>
			{
				IPAddressExtensions.GetLocalHostAddresses(Sockets.AddressFamily.InterNetworkV6).ToList();
			});

			NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
			Dictionary<string, HashSet<IPAddress>> localAddresses = IPAddressExtensions.GetLocalHostAddresses(Sockets.AddressFamily.InterNetwork);
			foreach (KeyValuePair<string, HashSet<IPAddress>> pair in localAddresses)
			{
				NetworkInterface networkInterface = interfaces.Where(item => item.Id.Equals(pair.Key)).First();
				Trace.WriteLine($"{networkInterface.Name} : {string.Join(',', pair.Value)}");
			}
		}
    }
}