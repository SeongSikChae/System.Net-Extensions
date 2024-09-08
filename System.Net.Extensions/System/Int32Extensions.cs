namespace System
{
	using Net;
	using Net.Sockets;
	using System.Buffers.Binary;

	public static class Int32Extensions
	{
		public static IPAddress ToSubnetMask(this int cidr, AddressFamily addressFamily)
		{
			if (cidr < 0)
				throw new InvalidOperationException("The 'mask' value must be 0 or greater.");
			if (addressFamily != AddressFamily.InterNetwork)
				throw new InvalidOperationException($"unsupported addressFamily '{Enum.GetName(addressFamily)}'");
			uint mask = ~(uint.MaxValue >> cidr);
			Span<byte> buffer = stackalloc byte[sizeof(uint)];
			BinaryPrimitives.WriteUInt32BigEndian(buffer, mask);
			return new IPAddress(buffer);
		}

		public static uint GetAddressCount(this int cidr)
		{
			return (uint)Math.Pow(2, 32 - cidr) - 2;
		}
	}
}
