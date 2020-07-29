namespace ProphetsWay.Utilities
{
	public enum HashTypes
	{
		MD5,
		SHA1,
		SHA256,
		SHA384,
		SHA512,
#if NET451 || NET452 || NET46 || NET461 || NET471 || NET472 || NET48
		RIPEMD160		
#endif
	}
}



