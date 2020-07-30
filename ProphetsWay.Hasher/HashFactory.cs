using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography;

namespace ProphetsWay.Utilities
{
	public static class HashFactory
	{
		public static HashAlgorithm GetHasher(this HashTypes hashType)
		{
			switch (hashType)
			{
				case HashTypes.MD5:
					return new MD5CryptoServiceProvider();

				case HashTypes.SHA1:
					return new SHA1Managed();

				case HashTypes.SHA256:
					return new SHA256Managed();

				case HashTypes.SHA384:
					return new SHA384Managed();

				case HashTypes.SHA512:
					return new SHA512Managed();

#if NET451 || NET452 || NET46 || NET461 || NET471 || NET472 || NET48
				case HashTypes.RIPEMD160:
					return new RIPEMD160Managed();
#endif
				default:
					throw new InvalidEnumArgumentException("Improper/unknown value of HashType was used.");
			}
		}

		public static Dictionary<HashTypes, HashAlgorithm> GetHashers(this HashTypes hashTypes)
		{
			var hashers = new Dictionary<HashTypes, HashAlgorithm>();

			var types = Enum.GetValues(hashTypes.GetType());
			foreach(HashTypes type in types)
				if((hashTypes & type) == type)
					hashers.Add(type, type.GetHasher());

			return hashers;
		}
	}
}




