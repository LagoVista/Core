using System;
using System.Text;

namespace LagoVista.Core.Security
{
    public static class SignedRequestCanonicalizer
    {
        public static string Build(SignedRequestCanonicalContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (context.Headers == null) throw new ArgumentNullException(nameof(context.Headers));

            switch (context.Profile)
            {
                case SignedRequestCanonicalProfile.RuntimeInstanceV1:
                    return BuildRuntimeInstanceV1(context);

                case SignedRequestCanonicalProfile.RuntimeInstanceHttpV1:
                    return BuildRuntimeInstanceHttpV1(context);

                case SignedRequestCanonicalProfile.ServiceHttpV1:
                    return BuildServiceHttpV1(context);

                default:
                    throw new InvalidOperationException($"Unsupported signed request canonical profile '{context.Profile}'.");
            }
        }

        private static string BuildRuntimeInstanceV1(SignedRequestCanonicalContext context)
        {
            var builder = new StringBuilder();

            Append(builder, SignedRequestHeaders.GetRequestId(context.Headers, context.Profile));
            Append(builder, SignedRequestHeaders.GetRequired(context.Headers, SignedRequestHeaders.Date));
            Append(builder, SignedRequestHeaders.GetRequired(context.Headers, SignedRequestHeaders.Version));
            Append(builder, SignedRequestHeaders.GetRequired(context.Headers, SignedRequestHeaders.OrganizationId));
            Append(builder, SignedRequestHeaders.GetRequired(context.Headers, SignedRequestHeaders.UserId));
            Append(builder, SignedRequestHeaders.GetRequired(context.Headers, SignedRequestHeaders.InstanceId));

            return builder.ToString();
        }

        private static string BuildRuntimeInstanceHttpV1(SignedRequestCanonicalContext context)
        {
            var builder = new StringBuilder();

            Append(builder, SignedRequestHeaders.GetRequestId(context.Headers, context.Profile));
            Append(builder, SignedRequestHeaders.GetRequired(context.Headers, SignedRequestHeaders.Date));
            Append(builder, SignedRequestHeaders.GetRequired(context.Headers, SignedRequestHeaders.Version));
            Append(builder, SignedRequestHeaders.GetRequired(context.Headers, SignedRequestHeaders.OrganizationId));
            Append(builder, SignedRequestHeaders.GetRequired(context.Headers, SignedRequestHeaders.UserId));
            Append(builder, SignedRequestHeaders.GetRequired(context.Headers, SignedRequestHeaders.InstanceId));
            Append(builder, Require(context.Method, nameof(context.Method)));
            Append(builder, Require(context.PathAndQuery, nameof(context.PathAndQuery)));
            Append(builder, ResolveBodySha256(context));

            return builder.ToString();
        }

        private static string BuildServiceHttpV1(SignedRequestCanonicalContext context)
        {
            var builder = new StringBuilder();

            Append(builder, SignedRequestHeaders.GetRequestId(context.Headers, context.Profile));
            Append(builder, SignedRequestHeaders.GetRequired(context.Headers, SignedRequestHeaders.Date));
            Append(builder, SignedRequestHeaders.GetRequired(context.Headers, SignedRequestHeaders.Version));
            Append(builder, SignedRequestHeaders.GetRequired(context.Headers, SignedRequestHeaders.AppKey));
            Append(builder, SignedRequestHeaders.GetRequired(context.Headers, SignedRequestHeaders.SigningKeyId));
            Append(builder, SignedRequestHeaders.GetRequired(context.Headers, SignedRequestHeaders.SignatureAlgorithm));
            Append(builder, SignedRequestHeaders.GetRequired(context.Headers, SignedRequestHeaders.KeyMaterialFormat));
            Append(builder, Require(context.Method, nameof(context.Method)));
            Append(builder, Require(context.PathAndQuery, nameof(context.PathAndQuery)));
            Append(builder, ResolveBodySha256(context));

            return builder.ToString();
        }

        private static string ResolveBodySha256(SignedRequestCanonicalContext context)
        {
            if (!String.IsNullOrWhiteSpace(context.BodySha256))
            {
                return context.BodySha256;
            }

            return SignedRequestHeaders.GetRequired(context.Headers, SignedRequestHeaders.BodySha256);
        }

        private static string Require(string value, string name)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException($"{name} is required for this signed request canonical profile.");
            }

            return value;
        }

        private static void Append(StringBuilder builder, string value)
        {
            builder.Append(value);
            builder.Append("\r\n");
        }
    }
}
