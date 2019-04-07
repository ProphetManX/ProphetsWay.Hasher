﻿using System;
using System.ComponentModel;
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

            switch (_hashType)
            {
                case HashTypes.MD5:
                    Hasher = new MD5CryptoServiceProvider();
                    break;

                case HashTypes.SHA1:
                    Hasher = new SHA1Managed();
                    break;

                case HashTypes.SHA256:
                    Hasher = new SHA256Managed();
                    break;

                case HashTypes.SHA512:
                    Hasher = new SHA512Managed();
                    break;

                default:
                    throw new InvalidEnumArgumentException("Improper value of HashType was used.");
            }
        }

        public void GenerateHash(Stream stream)
        {
            try
            {
                Hasher.ComputeHash(stream);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Error when trying to compute a [{_hashType}] hash on a Stream.");
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
