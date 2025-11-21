// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 199843c5a0f07b8337ffa63c8ae227373b342c28faa2a058bc41c5e95e554375
// IndexVersion: 2
// --- END CODE INDEX META ---
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
namespace LagoVista.Core.Utils.Types
{
        public static class CodeDocId
        {
            // Choose a fixed namespace GUID for your deployment (change to your own!)
            // Namespace is used for UUIDv5; pick any random GUID and keep it constant.
            public static readonly Guid NamespaceCodeFiles = new Guid("6fb6d1a8-9f3e-4dd4-9b2d-4c0c2d8e0e4a");

            /// <summary>
            /// Canonicalize parts to a single lowercased key "org|repo|path".
            /// Path must use forward slashes already; this will also collapse runs of '/'.
            /// </summary>
            public static string Canonical(string repo, string path)
            {
                string norm(string s) => (s ?? string.Empty).Trim().ToLowerInvariant();
                var r = norm(repo);
                var p = norm(path).Replace('\\', '/');
                while (p.Contains("//")) p = p.Replace("//", "/");
                return (r + "|" + p);
            }

            /// <summary>
            /// Deterministic GUID (UUIDv5, SHA-1) from canonical name in a fixed namespace.
            /// </summary>
            public static Guid FileDocIdV5(string canonical)
            {
                if (string.IsNullOrWhiteSpace(canonical)) throw new ArgumentException("canonical required");
                return UuidV5(NamespaceCodeFiles, canonical);
            }

            /// <summary>
            /// SHA-256 based 128-bit (first 16 bytes) hex identifier (alternative to GUID).
            /// Returns 32 hex chars. Collision risk is negligible for this use.
            /// </summary>
            public static string FileDocIdHex128(string canonical)
            {
                if (string.IsNullOrWhiteSpace(canonical)) throw new ArgumentException("canonical required");
                using (var sha = SHA256.Create())
                {
                    var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(canonical));
                    var sb = new StringBuilder(32);
                    for (int i = 0; i < 16; i++) sb.Append(bytes[i].ToString("x2")); // first 16 bytes → 128 bits
                    return sb.ToString();
                }
            }

            // ---------- UUIDv5 (RFC 4122) helpers ----------

            private static Guid UuidV5(Guid ns, string name)
            {
                // name-based UUID v5 = SHA-1(namespace_bytes + name_bytes), with RFC 4122 adjustments
                var nsBytes = ns.ToByteArray();
                SwapGuidByteOrder(nsBytes); // to big-endian
                var nameBytes = Encoding.UTF8.GetBytes(name);

                byte[] hash;
                using (var sha1 = SHA1.Create())
                {
                    sha1.TransformBlock(nsBytes, 0, nsBytes.Length, null, 0);
                    sha1.TransformFinalBlock(nameBytes, 0, nameBytes.Length);
                    hash = sha1.Hash;
                }

                var newGuid = new byte[16];
                Array.Copy(hash, 0, newGuid, 0, 16);

                // Set version to 5 (0101)
                newGuid[6] = (byte)((newGuid[6] & 0x0F) | (5 << 4));
                // Set variant to RFC 4122 (10xx)
                newGuid[8] = (byte)((newGuid[8] & 0x3F) | 0x80);

                SwapGuidByteOrder(newGuid); // back to little-endian Guid layout
                return new Guid(newGuid);
            }

            // .NET Guid bytes are little-endian for parts; convert for RFC-compliant hashing.
            private static void SwapGuidByteOrder(byte[] guid)
            {
                void swap(int a, int b) { var t = guid[a]; guid[a] = guid[b]; guid[b] = t; }
                // time_low
                swap(0, 3); swap(1, 2);
                // time_mid
                swap(4, 5);
                // time_hi_and_version
                swap(6, 7);
                // clock_seq_hi/reserved & clock_seq_low (8,9) stay
                // node (10..15) stays
            }
        }
    }
