using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ProphetsWay.Utilities
{
	public static class Hasher
	{
		private const int BUFFER_SIZE = 1024*1024*128;

		internal static string ToLowerString(this byte[] bytes)
		{
			return BitConverter.ToString(bytes).Replace("-", "").ToLower();
		}

		public static string GenerateHash(this Stream stream, HashTypes hashType)
		{
			var worker = new HashWorker(hashType);
			worker.GenerateHash(stream);
			return worker.Hash.ToLowerString();
		}

		public static bool VerifyHash(this Stream stream, string hash, HashTypes hashType)
		{
			return string.Equals(hash, GenerateHash(stream, hashType), StringComparison.OrdinalIgnoreCase);
		}

		public static bool VerifyHash(this FileInfo fileInfo, string hash, HashTypes hashType)
		{
			return VerifyHash(fileInfo.OpenRead(), hash, hashType);
		}

		public static string GenerateHash(string fileName, HashTypes hashType)
		{
			return GenerateHash(new FileInfo(fileName), hashType);
		}

		public static string GenerateHash(this FileInfo fileInfo, HashTypes hashType)
		{
			return GenerateHash(fileInfo.OpenRead(), hashType);
		}

		[Obsolete("This method is deprecated.  Switch to using GenerateHashes(Stream, HashTypes) instead.")]
		public static HashCollection GenerateHashes(this Stream stream)
		{
			var task = Task.Run(() => { return GenerateHashesAsync(stream); });
			task.Wait();
			return task.Result;
		}

		public static Dictionary<HashTypes, string> GenerateHashes(this Stream stream, HashTypes hashtypes)
		{
			var task = Task.Run(() => { return GenerateHashesAsync(stream, hashtypes); });
			task.Wait();
			return task.Result;	
		}

		private async static Task<Dictionary<HashTypes, byte[]>> GenerateHashesAsBytes(Stream stream, HashTypes hashtypes)
		{
			var buffer = new byte[BUFFER_SIZE];
			var hashers = hashtypes.GetHashers();

			await Task.Run(() =>
			{
				var bRead = 0;
				do
				{
					bRead = stream.Read(buffer, 0, buffer.Length);

					var tasks = new List<Task>();

					foreach (var hasher in hashers)
						tasks.Add(Task.Run(() => hasher.Value.GenerateIncrementalHash(buffer, bRead)));

					Task.WaitAll(tasks.ToArray());
				} while (bRead > 0);
			});
			
			var rslt = new Dictionary<HashTypes, byte[]>();
			foreach (var hasher in hashers)
				rslt.Add(hasher.Key, hasher.Value.Hash);

			return rslt;
		}

		public async static Task<Dictionary<HashTypes, string>> GenerateHashesAsync(this Stream stream, HashTypes hashtypes)
		{
			var byteResults = await GenerateHashesAsBytes(stream, hashtypes);

			var rslt = new Dictionary<HashTypes, string>();
			foreach (var br in byteResults)
				rslt.Add(br.Key, br.Value.ToLowerString());

			return rslt;
		}

		[Obsolete("This method is deprecated.  Switch to using GenerateHashesAsync(Stream, HashTypes) instead.")]
		public async static Task<HashCollection> GenerateHashesAsync(this Stream stream)
		{
			var hashes = HashTypes.MD5 | HashTypes.SHA1 | HashTypes.SHA256 | HashTypes.SHA512;

			var results = await GenerateHashesAsBytes(stream, hashes);

			var ret = new HashCollection(results[HashTypes.MD5], results[HashTypes.SHA1], results[HashTypes.SHA256], results[HashTypes.SHA512]);
			return ret;
		}

		
	}
}