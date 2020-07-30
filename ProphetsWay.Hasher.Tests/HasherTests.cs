using System.IO;
using Xunit;
using ProphetsWay.Utilities;
using FluentAssertions;
using System.Collections.Generic;

namespace ProphetsWay.Hasher.Tests
{
	public class HasherTests
	{
		[Theory]
		[InlineData(TestFiles.TestFileA.Name, TestFiles.TestFileA.MD5, HashTypes.MD5)]
		[InlineData(TestFiles.TestFileA.Name, TestFiles.TestFileA.SHA1, HashTypes.SHA1)]
		[InlineData(TestFiles.TestFileA.Name, TestFiles.TestFileA.SHA256, HashTypes.SHA256)]
		[InlineData(TestFiles.TestFileA.Name, TestFiles.TestFileA.SHA384, HashTypes.SHA384)]
		[InlineData(TestFiles.TestFileA.Name, TestFiles.TestFileA.SHA512, HashTypes.SHA512)]
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.MD5, HashTypes.MD5)]
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.SHA1, HashTypes.SHA1)]
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.SHA256, HashTypes.SHA256)]
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.SHA384, HashTypes.SHA384)]
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.SHA512, HashTypes.SHA512)]
		[InlineData(TestFiles.TestFileC.Name, TestFiles.TestFileC.MD5, HashTypes.MD5)]
		[InlineData(TestFiles.TestFileC.Name, TestFiles.TestFileC.SHA1, HashTypes.SHA1)]
		[InlineData(TestFiles.TestFileC.Name, TestFiles.TestFileC.SHA256, HashTypes.SHA256)]
		[InlineData(TestFiles.TestFileC.Name, TestFiles.TestFileC.SHA384, HashTypes.SHA384)]
		[InlineData(TestFiles.TestFileC.Name, TestFiles.TestFileC.SHA512, HashTypes.SHA512)]
#if NET451 || NET452 || NET46 || NET461 || NET471 || NET472 || NET48
		[InlineData(TestFiles.TestFileA.Name, TestFiles.TestFileA.RIPEMD160, HashTypes.RIPEMD160)]
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.RIPEMD160, HashTypes.RIPEMD160)]
		[InlineData(TestFiles.TestFileC.Name, TestFiles.TestFileC.RIPEMD160, HashTypes.RIPEMD160)]
#endif
		/// <summary>
		/// This is meant to test all possible combinations of HashTypes,
		/// all other functionality relies on calling this, and thus won't need
		/// to be explicitly tested
		/// </summary>
		public void TestGenerateHashFromStream(string filename, string expectedHash, HashTypes hashType)
		{
			var fs = GetFileStream(filename);

			var hashResult = fs.GenerateHash(hashType);

			hashResult.Should().Be(expectedHash);
		}

		[Theory]
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.MD5, HashTypes.MD5, true)]
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.SHA1, HashTypes.SHA1, true)]
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.SHA256, HashTypes.SHA256, true)]
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.SHA384, HashTypes.SHA384, true)]
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.SHA512, HashTypes.SHA512, true)]
#if NET451 || NET452 || NET46 || NET461 || NET471 || NET472 || NET48
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.RIPEMD160, HashTypes.RIPEMD160, true)]
#endif
		public void TestVerifyHashFromStream(string filename, string expectedHash, HashTypes hashType, bool expectedOutcome)
		{
			var fs = GetFileStream(filename);

			var hashResult = fs.VerifyHash(expectedHash, hashType);

			hashResult.Should().Be(expectedOutcome);
		}

		[Theory]
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.MD5, HashTypes.MD5, true)]
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.SHA1, HashTypes.SHA1, true)]
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.SHA256, HashTypes.SHA256, true)]
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.SHA384, HashTypes.SHA384, true)]
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.SHA512, HashTypes.SHA512, true)]
#if NET451 || NET452 || NET46 || NET461 || NET471 || NET472 || NET48
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.RIPEMD160, HashTypes.RIPEMD160, true)]
#endif
		[InlineData(TestFiles.TestFileC.Name, TestFiles.TestFileA.SHA512, HashTypes.SHA512, false)]
		public void TestVerifyWithUpperCaseHashFromStream(string filename, string expectedHash, HashTypes hashType, bool expectedOutcome)
		{
			var fs = GetFileStream(filename);

			var hashResult = fs.VerifyHash(expectedHash.ToUpper(), hashType);

			hashResult.Should().Be(expectedOutcome);
		}

		[Theory]
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.MD5, HashTypes.MD5)]
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.SHA1, HashTypes.SHA1)]
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.SHA256, HashTypes.SHA256)]
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.SHA384, HashTypes.SHA384)]
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.SHA512, HashTypes.SHA512)]
#if NET451 || NET452 || NET46 || NET461 || NET471 || NET472 || NET48
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.RIPEMD160, HashTypes.RIPEMD160)]
#endif
		public void TestGenerateHashFromFileInfo(string filename, string expectedHash, HashTypes hashType)
		{
			var fi = new FileInfo(filename);

			var hashResult = fi.GenerateHash(hashType);

			hashResult.Should().Be(expectedHash);
		}

		[Theory]
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.MD5, HashTypes.MD5, true)]
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.SHA1, HashTypes.SHA1, true)]
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.SHA256, HashTypes.SHA256, true)]
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.SHA384, HashTypes.SHA384, true)]
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.SHA512, HashTypes.SHA512, true)]
#if NET451 || NET452 || NET46 || NET461 || NET471 || NET472 || NET48
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.RIPEMD160, HashTypes.RIPEMD160, true)]
#endif
		[InlineData(TestFiles.TestFileC.Name, TestFiles.TestFileA.SHA512, HashTypes.SHA512, false)]
		public void TestVerifyHashFromFileInfo(string filename, string expectedHash, HashTypes hashType, bool expectedOutcome)
		{
			var fi = new FileInfo(filename);

			var hashResult = fi.VerifyHash(expectedHash, hashType);

			hashResult.Should().Be(expectedOutcome);
		}

		[Theory]
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.MD5, HashTypes.MD5, true)]
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.SHA1, HashTypes.SHA1, true)]
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.SHA256, HashTypes.SHA256, true)]
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.SHA384, HashTypes.SHA384, true)]
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.SHA512, HashTypes.SHA512, true)]
#if NET451 || NET452 || NET46 || NET461 || NET471 || NET472 || NET48
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.RIPEMD160, HashTypes.RIPEMD160, true)]
#endif
		[InlineData(TestFiles.TestFileC.Name, TestFiles.TestFileA.SHA512, HashTypes.SHA512, false)]
		public void TestVerifyWithUpperCaseHashFromFileInfo(string filename, string expectedHash, HashTypes hashType, bool expectedOutcome)
		{
			var fi = new FileInfo(filename);

			var hashResult = fi.VerifyHash(expectedHash.ToUpper(), hashType);

			hashResult.Should().Be(expectedOutcome);
		}

		[Theory]
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.MD5, HashTypes.MD5)]
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.SHA1, HashTypes.SHA1)]
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.SHA256, HashTypes.SHA256)]
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.SHA384, HashTypes.SHA384)]
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.SHA512, HashTypes.SHA512)]
#if NET451 || NET452 || NET46 || NET461 || NET471 || NET472 || NET48
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.RIPEMD160, HashTypes.RIPEMD160)]
#endif
		public void TestGenerateHashFromFileName(string filename, string expectedHash, HashTypes hashType)
		{
			var hashResult = Utilities.Hasher.GenerateHash(filename, hashType);

			hashResult.Should().Be(expectedHash);
		}

		private Stream GetFileStream(string filename)
		{
			var fi = new FileInfo(filename);
			var fs = fi.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
			return fs;
		}

		private HashTypes GetRolledUpHashTypes(HashTypes[] hashTypes)
		{
			HashTypes requestedHashes = 0;
			foreach (var ht in hashTypes)
				requestedHashes = requestedHashes | ht;

			return requestedHashes;
		}

		private void VerifyArraysMatch(Dictionary<HashTypes, string> results, HashTypes[] hashTypes, string[] expectedHashes)
		{
			for (var i = 0; i < expectedHashes.Length; i++)
				results[hashTypes[i]].Should().Be(expectedHashes[i]);
		}

		[Theory]
		[InlineData(TestFiles.TestFileA.Name, new HashTypes[2] { HashTypes.MD5, HashTypes.SHA1 }, new string[] { TestFiles.TestFileA.MD5, TestFiles.TestFileA.SHA1 })]
		[InlineData(TestFiles.TestFileA.Name, new HashTypes[3] { HashTypes.MD5, HashTypes.SHA1, HashTypes.SHA256 }, new string[] { TestFiles.TestFileA.MD5, TestFiles.TestFileA.SHA1, TestFiles.TestFileA.SHA256 })]
		[InlineData(TestFiles.TestFileA.Name, new HashTypes[4] { HashTypes.MD5, HashTypes.SHA1, HashTypes.SHA256, HashTypes.SHA384 }, new string[] { TestFiles.TestFileA.MD5, TestFiles.TestFileA.SHA1, TestFiles.TestFileA.SHA256, TestFiles.TestFileA.SHA384 })]
		[InlineData(TestFiles.TestFileA.Name, new HashTypes[5] { HashTypes.MD5, HashTypes.SHA1, HashTypes.SHA256, HashTypes.SHA384, HashTypes.SHA512 }, new string[] { TestFiles.TestFileA.MD5, TestFiles.TestFileA.SHA1, TestFiles.TestFileA.SHA256, TestFiles.TestFileA.SHA384, TestFiles.TestFileA.SHA512 })]
		public async void TestDynamicGenerateHashesAsyncFromStream(string filename, HashTypes[] hashTypes, string[] expectedHashes)
		{
			var fs = GetFileStream(filename);
			var requestedHashes = GetRolledUpHashTypes(hashTypes);

			var hashes = await fs.GenerateHashesAsync(requestedHashes);

			VerifyArraysMatch(hashes, hashTypes, expectedHashes);
		}

		[Theory]
		[InlineData(TestFiles.TestFileA.Name, new HashTypes[2] { HashTypes.MD5, HashTypes.SHA1 }, new string[] { TestFiles.TestFileA.MD5, TestFiles.TestFileA.SHA1 })]
		[InlineData(TestFiles.TestFileA.Name, new HashTypes[3] { HashTypes.MD5, HashTypes.SHA1, HashTypes.SHA256 }, new string[] { TestFiles.TestFileA.MD5, TestFiles.TestFileA.SHA1, TestFiles.TestFileA.SHA256 })]
		[InlineData(TestFiles.TestFileA.Name, new HashTypes[4] { HashTypes.MD5, HashTypes.SHA1, HashTypes.SHA256, HashTypes.SHA384 }, new string[] { TestFiles.TestFileA.MD5, TestFiles.TestFileA.SHA1, TestFiles.TestFileA.SHA256, TestFiles.TestFileA.SHA384 })]
		[InlineData(TestFiles.TestFileA.Name, new HashTypes[5] { HashTypes.MD5, HashTypes.SHA1, HashTypes.SHA256, HashTypes.SHA384, HashTypes.SHA512 }, new string[] { TestFiles.TestFileA.MD5, TestFiles.TestFileA.SHA1, TestFiles.TestFileA.SHA256, TestFiles.TestFileA.SHA384, TestFiles.TestFileA.SHA512 })]
		public void TestDynamicGenerateHashesFromStream(string filename, HashTypes[] hashTypes, string[] expectedHashes)
		{
			var fs = GetFileStream(filename);
			var requestedHashes = GetRolledUpHashTypes(hashTypes);

			var hashes = fs.GenerateHashes(requestedHashes);

			VerifyArraysMatch(hashes, hashTypes, expectedHashes);
		}

		[Theory]
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.MD5, true)]
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.SHA1, true)]
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.SHA256, true)]
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.SHA384, true)]
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.SHA512, true)]
#if NET451 || NET452 || NET46 || NET461 || NET471 || NET472 || NET48
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileB.RIPEMD160, true)]
#endif
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileA.MD5, false)]
		[InlineData(TestFiles.TestFileB.Name, TestFiles.TestFileA.SHA1, false)]
		public void TestVerifyHashByLengthFromStream(string filename, string expectedHash, bool expectedOutcome)
		{
			var fs = GetFileStream(filename);

			var hashResult = fs.VerifyHash(expectedHash.ToUpper());

			hashResult.Should().Be(expectedOutcome);
		}
	}
}
