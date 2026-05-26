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

        public SignedRequestValidatorService(ISignedRequestValidationKeyResolver validationKeyResolver, ISignedRequestPublicKeyRefreshService publicKeyRefreshService)
        {
            _validationKeyResolver = validationKeyResolver ?? throw new ArgumentNullException(nameof(validationKeyResolver));
            _publicKeyRefreshService = publicKeyRefreshService ?? throw new ArgumentNullException(nameof(publicKeyRefreshService));
            _validator = new SignedRequestValidator();
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

            await _publicKeyRefreshService.RefreshAsync(cancellationToken).ConfigureAwait(false);

            serviceContext = CreateContext(context, SignedRequestCanonicalProfile.ServiceHttpV1, _validationKeyResolver);
            return _validator.Validate(serviceContext);
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
