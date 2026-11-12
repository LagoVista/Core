using System;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Security
{
    public class SignedRequestValidatorService : ISignedRequestValidatorService
    {
        private readonly ISignedRequestValidationKeyResolver _validationKeyResolver;
        private readonly ISignedRequestPublicKeyRefreshService _publicKeyRefreshService;
        private readonly SignedRequestValidator _validator;
        private readonly ISignedRequestKeyDiagnostics _diagnostics;

        public SignedRequestValidatorService(ISignedRequestValidationKeyResolver validationKeyResolver, ISignedRequestPublicKeyRefreshService publicKeyRefreshService, ISignedRequestKeyDiagnostics diagnostics = null)
        {
            _validationKeyResolver = validationKeyResolver ?? throw new ArgumentNullException(nameof(validationKeyResolver));
            _publicKeyRefreshService = publicKeyRefreshService ?? throw new ArgumentNullException(nameof(publicKeyRefreshService));
            _validator = new SignedRequestValidator();
            _diagnostics = diagnostics;
        }

        public SignedRequestValidationResult ValidateRuntimeInstanceV1(SignedRequestValidationContext context)
        {
            return ValidateRuntime(context, SignedRequestCanonicalProfile.RuntimeInstanceV1);
        }

        public SignedRequestValidationResult ValidateRuntimeInstanceHttpV1(SignedRequestValidationContext context)
        {
            return ValidateRuntime(context, SignedRequestCanonicalProfile.RuntimeInstanceHttpV1);
        }

        public async Task<SignedRequestValidationResult> ValidateServiceHttpV1Async(SignedRequestValidationContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (context.Profile != SignedRequestCanonicalProfile.ServiceHttpV1) throw new InvalidOperationException("ValidateServiceHttpV1Async requires a ServiceHttpV1 validation context.");

            var serviceContext = CreateContext(context, SignedRequestCanonicalProfile.ServiceHttpV1, _validationKeyResolver);
            var result = _validator.Validate(serviceContext);
            if (result.Successful || !String.Equals(result.ErrorCode, "validation_key_not_found", StringComparison.OrdinalIgnoreCase))
            {
                return result;
            }

            _diagnostics?.Write("validation-key-miss",
                $"caller={SignedRequestHeaders.GetRequired(context.Headers, SignedRequestHeaders.AppKey)} keyId={SignedRequestHeaders.GetRequired(context.Headers, SignedRequestHeaders.SigningKeyId)} algorithm={SignedRequestHeaders.GetRequired(context.Headers, SignedRequestHeaders.SignatureAlgorithm)}");
            var refresh = await _publicKeyRefreshService.RefreshAsync(cancellationToken).ConfigureAwait(false);
            if (!refresh.Successful)
            {
                var statusMatch = System.Text.RegularExpressions.Regex.Match(refresh.ErrorMessage ?? String.Empty, @"HTTP [0-9]{3}");
                _diagnostics?.Write("key-refresh-failed",
                    $"errorCode={refresh.ErrorCode} http={(statusMatch.Success ? statusMatch.Value : "not available")} (no key material or response body logged)");
            }
            else
                _diagnostics?.Write("key-refresh-succeeded", $"version={refresh.Version}");

            serviceContext = CreateContext(context, SignedRequestCanonicalProfile.ServiceHttpV1, _validationKeyResolver);
            var retry = _validator.Validate(serviceContext);
            _diagnostics?.Write("key-validation-retry", $"success={retry.Successful} errorCode={retry.ErrorCode} matchedKeyId={retry.MatchedKeyId}");
            return retry;
        }

        private SignedRequestValidationResult ValidateRuntime(SignedRequestValidationContext context, SignedRequestCanonicalProfile profile)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (context.Profile != profile) throw new InvalidOperationException($"Runtime validation requires a {profile} validation context.");

            var runtimeContext = CreateContext(context, profile, null);
            return _validator.Validate(runtimeContext);
        }

        private static SignedRequestValidationContext CreateContext(SignedRequestValidationContext source, SignedRequestCanonicalProfile profile, ISignedRequestValidationKeyResolver validationKeyResolver)
        {
            return new SignedRequestValidationContext
            {
                Profile = profile,
                Headers = source.Headers,
                Key1 = source.Key1,
                Key2 = source.Key2,
                ValidationKeyResolver = validationKeyResolver,
                Method = source.Method,
                PathAndQuery = source.PathAndQuery,
                BodySha256 = source.BodySha256,
                MaxClockSkew = source.MaxClockSkew,
                ValidateTimestamp = source.ValidateTimestamp
            };
        }
    }
}
