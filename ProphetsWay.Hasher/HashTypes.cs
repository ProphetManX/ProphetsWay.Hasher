using System;

namespace ProphetsWay.Hasher
{
	[Flags]
	public enum HashTypes
	{
		MD5 = 1,
		SHA1 = 2,
		SHA256 = 4,
		SHA384 = 8,
		SHA512 = 16,
#if NET451 || NET452 || NET46 || NET461 || NET471 || NET472 || NET48
		RIPEMD160 = 32		
#endif
	}
}



