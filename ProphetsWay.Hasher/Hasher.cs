using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ProphetsWay.Utilities
{
    public static class Hasher
	{
		private const int BUFFER_SIZE = 1024*1024*128;

		public static string GenerateHash(this Stream stream, HashTypes hashType)
		{
			var worker = new HashWorker(hashType);
			worker.GenerateHash(stream);

			return BitConverter.ToString(worker.Hash).Replace("-", "").ToLower();
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

		public async static Task<HashCollection> GenerateHashes(this Stream stream)
		{
			var buffer = new byte[BUFFER_SIZE];
			var md5Worker = new HashWorker(HashTypes.MD5);
			var sha1Worker = new HashWorker(HashTypes.SHA1);
			var sha256Worker = new HashWorker(HashTypes.SHA256);
			var sha512Worker = new HashWorker(HashTypes.SHA512);


            await Task.Run(() =>
            {
                var bRead = 0;
                do
                {
                    bRead = stream.Read(buffer, 0, buffer.Length);

                    var tasks = new List<Task>
                    {
                        Task.Run(() => md5Worker.GenerateIncrementalHash(buffer, bRead)),
                        Task.Run(() => sha1Worker.GenerateIncrementalHash(buffer, bRead)),
                        Task.Run(() => sha256Worker.GenerateIncrementalHash(buffer, bRead)),
                        Task.Run(() => sha512Worker.GenerateIncrementalHash(buffer, bRead))
                    };

                    Task.WaitAll(tasks.ToArray());
                } while (bRead > 0);
            });


			var ret = new HashCollection(md5Worker.Hash, sha1Worker.Hash, sha256Worker.Hash, sha512Worker.Hash);
			return ret;
		}

		
	}
}