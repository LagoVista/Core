// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 861739a7d54b5aa72f2dff0236789a0312025f673b73f06c14457791fccf27d4
// IndexVersion: 2
// --- END CODE INDEX META ---
using System;
using System.Net;

namespace LagoVista.Core.Utils.Types
{
    /// <summary>
    /// Exception that carries HTTP status and response body for Qdrant calls.
    /// </summary>
    public sealed class QdrantHttpException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public string ResponseBody { get; }

        public QdrantHttpException(string message, HttpStatusCode statusCode, string responseBody)
            : base($"{message} (HTTP {(int)statusCode}): {Truncate(responseBody, 512)}")
        {
            StatusCode = statusCode;
            ResponseBody = responseBody;
        }

        private static string Truncate(string s, int max)
        {
            if (string.IsNullOrEmpty(s) || s.Length <= max) return s ?? string.Empty;
            return s.Substring(0, max) + "...";
        }
    }
}
