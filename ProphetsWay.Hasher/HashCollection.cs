using System;

namespace ProphetsWay.Utilities
{
	public class HashCollection
	{
		public HashCollection(byte[] md5, byte[] sha1, byte[] sha256, byte[] sha512)
		{
			MD5 = BitConverter.ToString(md5).Replace("-", "").ToLower();
			SHA1 = BitConverter.ToString(sha1).Replace("-", "").ToLower();
			SHA256 = BitConverter.ToString(sha256).Replace("-", "").ToLower();
			SHA512 = BitConverter.ToString(sha512).Replace("-", "").ToLower();
		}

		public string SHA1 { get; private set; }
		public string SHA256 { get; private set; }
		public string SHA512 { get; private set; }
		public string MD5 { get; private set; }
	}
}
