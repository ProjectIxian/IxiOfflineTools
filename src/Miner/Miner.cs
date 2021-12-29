using IXICore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IxiOfflineTools
{
    class Miner
    {
        [ThreadStatic] private static byte[] dummyExpandedNonce = null;
        [ThreadStatic] private static int lastNonceLength = 0;

        // Expand a provided nonce up to expand_length bytes by appending a suffix of fixed-value bytes
        private static byte[] expandNonce(byte[] nonce, int expand_length)
        {
            if (dummyExpandedNonce == null)
            {
                dummyExpandedNonce = new byte[expand_length];
                for (int i = 0; i < dummyExpandedNonce.Length; i++)
                {
                    dummyExpandedNonce[i] = 0x23;
                }
            }

            // set dummy with nonce
            for (int i = 0; i < nonce.Length; i++)
            {
                dummyExpandedNonce[i] = nonce[i];
            }

            // clear any bytes from last nonce
            for (int i = nonce.Length; i < lastNonceLength; i++)
            {
                dummyExpandedNonce[i] = 0x23;
            }

            lastNonceLength = nonce.Length;

            return dummyExpandedNonce;
        }

        // Verify nonce
        public static bool verifyNonce_v3(string nonce, byte[] block_checksum, byte[] solver_address, ulong difficulty)
        {
            if (nonce == null || nonce.Length < 1 || nonce.Length > 128)
            {
                return false;
            }

            // TODO protect against spamming with invalid nonce/block_num
            byte[] p1 = new byte[block_checksum.Length + solver_address.Length];
            System.Buffer.BlockCopy(block_checksum, 0, p1, 0, block_checksum.Length);
            System.Buffer.BlockCopy(solver_address, 0, p1, block_checksum.Length, solver_address.Length);

            byte[] nonce_bytes = Crypto.stringToHash(nonce);
            byte[] fullnonce = expandNonce(nonce_bytes, 234236);
            byte[] hash = Argon2id.getHash(p1, fullnonce, 2, 2048, 2);

            if (validateHash_v2(hash, difficulty) == true)
            {
                // Hash is valid
                return true;
            }

            return false;
        }

        public static bool validateHash_v2(byte[] hash, ulong difficulty)
        {
            return validateHashInternal_v2(hash, MiningUtils.getHashCeilFromDifficulty(difficulty));
        }

        private static bool validateHashInternal_v2(byte[] hash, byte[] hash_ceil)
        {
            if (hash == null || hash.Length < 32)
            {
                return false;
            }
            for (int i = 0; i < hash.Length; i++)
            {
                byte cb = i < hash_ceil.Length ? hash_ceil[i] : (byte)0xff;
                if (cb > hash[i]) return true;
                if (cb < hash[i]) return false;
            }
            // if we reach this point, the hash is exactly equal to the ceiling we consider this a 'passing hash'
            return true;
        }

    }
}
