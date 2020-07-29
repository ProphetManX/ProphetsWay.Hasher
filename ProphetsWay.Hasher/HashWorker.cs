using System;
using System.IO;
using System.Security.Cryptography;

namespace ProphetsWay.Utilities
{
    internal class HashWorker
    {
        public byte[] Hash => Hasher.Hash;

        private HashAlgorithm Hasher { get; set; }

        private readonly HashTypes _hashType;

        public HashWorker(HashTypes hashType)
        {
            _hashType = hashType;
            Hasher = hashType.GetHasher();
        }

        public void GenerateHash(Stream stream)
        {
            try
            {
                Hasher.ComputeHash(stream);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error when trying to compute a [{_hashType}] hash on a Stream.", ex);
            }
        }

        public void GenerateIncrementalHash(byte[] inputBuffer, int bufferLength)
        {
            if (bufferLength > 0)
                Hasher.TransformBlock(inputBuffer, 0, bufferLength, inputBuffer, 0);
            else
                Hasher.TransformFinalBlock(inputBuffer, 0, bufferLength);
        }
    }

}
