using System;
using System.Security.Cryptography;
using System.Xml;

namespace LagoVista.Core.Security
{
    public static class SignedRequestRsaXmlSerializer
    {
        public static RSAParameters FromXmlString(string xml)
        {
            if (String.IsNullOrWhiteSpace(xml)) throw new ArgumentNullException(nameof(xml));

            var doc = new XmlDocument();
            doc.PreserveWhitespace = false;
            doc.LoadXml(xml);

            var parameters = new RSAParameters
            {
                Modulus = ReadRequired(doc, "Modulus"),
                Exponent = ReadRequired(doc, "Exponent"),
                P = ReadOptional(doc, "P"),
                Q = ReadOptional(doc, "Q"),
                DP = ReadOptional(doc, "DP"),
                DQ = ReadOptional(doc, "DQ"),
                InverseQ = ReadOptional(doc, "InverseQ"),
                D = ReadOptional(doc, "D")
            };

            return parameters;
        }

        public static string ToXmlString(RSAParameters parameters, bool includePrivateParameters)
        {
            if (parameters.Modulus == null || parameters.Modulus.Length == 0) throw new ArgumentNullException(nameof(parameters.Modulus));
            if (parameters.Exponent == null || parameters.Exponent.Length == 0) throw new ArgumentNullException(nameof(parameters.Exponent));

            var doc = new XmlDocument();
            var root = doc.CreateElement("RSAKeyValue");
            doc.AppendChild(root);

            Append(doc, root, "Modulus", parameters.Modulus);
            Append(doc, root, "Exponent", parameters.Exponent);

            if (includePrivateParameters)
            {
                AppendRequiredPrivate(doc, root, "P", parameters.P);
                AppendRequiredPrivate(doc, root, "Q", parameters.Q);
                AppendRequiredPrivate(doc, root, "DP", parameters.DP);
                AppendRequiredPrivate(doc, root, "DQ", parameters.DQ);
                AppendRequiredPrivate(doc, root, "InverseQ", parameters.InverseQ);
                AppendRequiredPrivate(doc, root, "D", parameters.D);
            }

            return doc.OuterXml;
        }

        private static byte[] ReadRequired(XmlDocument doc, string name)
        {
            var node = doc.SelectSingleNode("/RSAKeyValue/" + name);
            if (node == null || String.IsNullOrWhiteSpace(node.InnerText))
            {
                throw new InvalidOperationException($"RSA XML key is missing required element '{name}'.");
            }

            return Convert.FromBase64String(node.InnerText);
        }

        private static byte[] ReadOptional(XmlDocument doc, string name)
        {
            var node = doc.SelectSingleNode("/RSAKeyValue/" + name);
            if (node == null || String.IsNullOrWhiteSpace(node.InnerText))
            {
                return null;
            }

            return Convert.FromBase64String(node.InnerText);
        }

        private static void AppendRequiredPrivate(XmlDocument doc, XmlElement root, string name, byte[] value)
        {
            if (value == null || value.Length == 0)
            {
                throw new InvalidOperationException($"RSA private key material is missing required parameter '{name}'.");
            }

            Append(doc, root, name, value);
        }

        private static void Append(XmlDocument doc, XmlElement root, string name, byte[] value)
        {
            var element = doc.CreateElement(name);
            element.InnerText = Convert.ToBase64String(value);
            root.AppendChild(element);
        }
    }
}
