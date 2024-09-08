namespace System
{
	using Net;
	using Net.Sockets;
	using System.Buffers.Binary;

	/// <summary>
	/// System.Int32 Extensions
	/// </summary>
	public static class Int32Extensions
	{
		/// <summary>
		/// Converts CIDR Mask Bits to Subnet Mask Address.
		/// </summary>
		/// <param name="cidr">CIDR Mask Bits</param>
		/// <returns>Subnet Mask Address</returns>
		/// <exception cref="InvalidOperationException">Occurs when CIDR Mask Bits are less than 0</exception>
		public static IPAddress ToSubnetMask(this int cidr)
		{
			if (cidr < 0)
				throw new InvalidOperationException("The 'mask' value must be 0 or greater.");
			uint mask = ~(uint.MaxValue >> cidr);
			Span<byte> buffer = stackalloc byte[sizeof(uint)];
			BinaryPrimitives.WriteUInt32BigEndian(buffer, mask);
			return new IPAddress(buffer);
		}

		/// <summary>
		/// Gets the number of host addresses from a given CIDR Mask Bits
		/// </summary>
		/// <param name="cidr">CIDR Mask Bits</param>
		/// <returns>Host Count</returns>
		public static uint GetAddressCount(this int cidr)
		{
			return (uint)Math.Pow(2, 32 - cidr) - 2;
		}
	}
}
