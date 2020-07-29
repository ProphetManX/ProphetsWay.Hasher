using Xunit;
using ProphetsWay.Utilities;
using FluentAssertions;
using System.Security.Cryptography;
using System;

namespace ProphetsWay.Hasher.Tests
{
	public class HashFactoryTests
	{
		[Theory]
		[InlineData(HashTypes.MD5, typeof(MD5CryptoServiceProvider))]
		[InlineData(HashTypes.SHA1, typeof(SHA1Managed))]
		[InlineData(HashTypes.SHA256, typeof(SHA256Managed))]
		[InlineData(HashTypes.SHA384, typeof(SHA384Managed))]
		[InlineData(HashTypes.SHA512, typeof(SHA512Managed))]
#if NET451 || NET452 || NET46 || NET461 || NET471 || NET472 || NET48
		[InlineData(HashTypes.RIPEMD160, typeof(RIPEMD160Managed))]
#endif
		public void TestGetHasher(HashTypes hashType, Type type)
		{
			var hasher = hashType.GetHasher();
			hasher.Should().BeOfType(type);
		}

		[Theory]
		[InlineData(HashTypes.MD5, 1)]
		[InlineData(HashTypes.MD5 | HashTypes.SHA1, 2)]
		[InlineData(HashTypes.MD5 | HashTypes.SHA1 | HashTypes.SHA256, 3)]
		[InlineData(HashTypes.MD5 | HashTypes.SHA1 | HashTypes.SHA256 | HashTypes.SHA384, 4)]
		[InlineData(HashTypes.MD5 | HashTypes.SHA1 | HashTypes.SHA256 | HashTypes.SHA384 | HashTypes.SHA512, 5)]
#if NET451 || NET452 || NET46 || NET461 || NET471 || NET472 || NET48
		[InlineData(HashTypes.MD5 | HashTypes.SHA1 | HashTypes.SHA256 | HashTypes.SHA384 | HashTypes.SHA512 | HashTypes.RIPEMD160, 6)]
#endif
		public void TestGetHashers(HashTypes hashTypes, int numberOfHashers)
		{
			var hashers = hashTypes.GetHashers();
			hashers.Count.Should().Be(numberOfHashers);
		}
	}
}

