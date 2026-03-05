using LagoVista.Core.Models;
using System;
using System.IO;
using System.Text;

namespace LagoVista.Crypto.Modern
{
    /// <summary>
    /// AAD v1 binary layout:
    /// 'aad1'(4 ASCII bytes) | OrgId(16) | RECID(16) | FieldNameLen(u16 BE) | FieldName(UTF-8) | KeyIdLen(u16 BE) | KeyId(UTF-8)
    /// </summary>
    public sealed class AadBuilderV1 : IAadBuilder
    {
        private static readonly byte[] Prefix = Encoding.ASCII.GetBytes("aad1");

        public byte[] BuildAadV1(GuidString36 orgId, GuidString36 recId, string fieldNameLower, string keyId)
        {
            if (string.IsNullOrWhiteSpace(fieldNameLower)) throw new ArgumentNullException(nameof(fieldNameLower));
            if (string.IsNullOrWhiteSpace(keyId)) throw new ArgumentNullException(nameof(keyId));

            var orgBytes = orgId.ToGuid().ToByteArray();
            var recBytes = recId.ToGuid().ToByteArray();

            var fieldBytes = Encoding.UTF8.GetBytes(fieldNameLower);
            var keyIdBytes = Encoding.UTF8.GetBytes(keyId);

            if (fieldBytes.Length > ushort.MaxValue) throw new ArgumentOutOfRangeException(nameof(fieldNameLower), "FieldName too long.");
            if (keyIdBytes.Length > ushort.MaxValue) throw new ArgumentOutOfRangeException(nameof(keyId), "KeyId too long.");

            using (var ms = new MemoryStream())
            {
                ms.Write(Prefix, 0, Prefix.Length);
                ms.Write(orgBytes, 0, 16);
                ms.Write(recBytes, 0, 16);

                WriteU16BE(ms, (ushort)fieldBytes.Length);
                ms.Write(fieldBytes, 0, fieldBytes.Length);

                WriteU16BE(ms, (ushort)keyIdBytes.Length);
                ms.Write(keyIdBytes, 0, keyIdBytes.Length);

                return ms.ToArray();
            }
        }

        private static void WriteU16BE(Stream stream, ushort value)
        {
            stream.WriteByte((byte)(value >> 8));
            stream.WriteByte((byte)(value & 0xFF));
        }
    }
}
