﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
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

		internal static void GenerateIncrementalHash(this HashAlgorithm hasher, byte[] inputBuffer, int bufferLength)
		{
			if (bufferLength > 0)
				hasher.TransformBlock(inputBuffer, 0, bufferLength, inputBuffer, 0);
			else
				hasher.TransformFinalBlock(inputBuffer, 0, bufferLength);
		}

		/// <summary>
		/// Reads the stream and returns a Checksum calculated with the requested HashType
		/// </summary>
		/// <param name="hashType">The Algorithm used to calculate the checksum.</param>
		/// <returns>A lowercase string representing the checksum.</returns>
		public static string GenerateHash(this Stream stream, HashTypes hashType)
		{
			var hasher = hashType.GetHasher();
			hasher.ComputeHash(stream);
			return hasher.Hash.ToLowerString();
		}

		/// <summary>
		/// Reads the stream and returns true/false if your expected checksum matches.
		/// </summary>
		/// <param name="hash">Your expected checksum.</param>
		/// <param name="hashType">The Algorithm used to calculate the checksum.</param>
		public static bool VerifyHash(this Stream stream, string hash, HashTypes hashType)
		{
			return GenerateHash(stream, hashType).Equals(hash, StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Opens the FileInfo and returns true/false if your expected checksum matches.
		/// </summary>
		/// <param name="hash">Your expected checksum.</param>
		/// <param name="hashType">The Algorithm used to calculate the checksum.</param>
		public static bool VerifyHash(this FileInfo fileInfo, string hash, HashTypes hashType)
		{
			return VerifyHash(fileInfo.OpenRead(), hash, hashType);
		}

		/// <summary>
		/// Opens the FileInfo and returns a Checksum calculated with the requested HashType
		/// </summary>
		/// <param name="hashType">The Algorithm used to calculate the checksum.</param>
		/// <returns>A lowercase string representing the checksum.</returns>
		public static string GenerateHash(this FileInfo fileInfo, HashTypes hashType)
		{
			return GenerateHash(fileInfo.OpenRead(), hashType);
		}

		/// <summary>
		/// Opens a file specified by FileName and returns a Checksum calculated with the requested HashType
		/// </summary>
		/// <param name="hashType">The Algorithm used to calculate the checksum.</param>
		/// <param name="fileName">The path of your file to be hashed.</param>
		/// <returns>A lowercase string representing the checksum.</returns>
		public static string GenerateHash(string fileName, HashTypes hashType)
		{
			return GenerateHash(new FileInfo(fileName), hashType);
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

		/// <summary>
		/// Reads a stream and returns a Dictionary with all the requested HashTypes and its associated checksum.
		/// </summary>
		/// <param name="hashtypes">The Algorithm(s) requested to generate checksums.  You can select multiple HashTypes by using bitwise "or" operations '|'</param>
		/// <returns>A dictionary with enum HashType as the key, and the lowercase string checksum as its value.</returns>
		public async static Task<Dictionary<HashTypes, string>> GenerateHashesAsync(this Stream stream, HashTypes hashtypes)
		{
			var byteResults = await GenerateHashesAsBytes(stream, hashtypes);

			var rslt = new Dictionary<HashTypes, string>();
			foreach (var br in byteResults)
				rslt.Add(br.Key, br.Value.ToLowerString());

			return rslt;
		}

		/// <summary>
		/// Reads a stream and returns a Dictionary with all the requested HashTypes and its associated checksum.
		/// </summary>
		/// <param name="hashtypes">The Algorithm(s) requested to generate checksums.  You can select multiple HashTypes by using bitwise "or" operations '|'</param>
		/// <returns>A dictionary with enum HashType as the key, and the lowercase string checksum as its value.</returns>
		public static Dictionary<HashTypes, string> GenerateHashes(this Stream stream, HashTypes hashtypes)
		{
			var task = Task.Run(() => { return GenerateHashesAsync(stream, hashtypes); });
			task.Wait();
			return task.Result;
		}

		[Obsolete("This method is deprecated.  Switch to using GenerateHashesAsync(Stream, HashTypes) instead.")]
		public async static Task<HashCollection> GenerateHashesAsync(this Stream stream)
		{
			var hashes = HashTypes.MD5 | HashTypes.SHA1 | HashTypes.SHA256 | HashTypes.SHA512;

			var results = await GenerateHashesAsBytes(stream, hashes);

			var ret = new HashCollection(results[HashTypes.MD5], results[HashTypes.SHA1], results[HashTypes.SHA256], results[HashTypes.SHA512]);
			return ret;
		}

		[Obsolete("This method is deprecated.  Switch to using GenerateHashes(Stream, HashTypes) instead.")]
		public static HashCollection GenerateHashes(this Stream stream)
		{
			var task = Task.Run(() => { return GenerateHashesAsync(stream); });
			task.Wait();
			return task.Result;
		}


	}
}