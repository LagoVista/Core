using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Security
{
    public interface ISignedRequestValidatorService
    {
        SignedRequestValidationResult ValidateRuntimeInstanceV1(SignedRequestValidationContext context);
        SignedRequestValidationResult ValidateRuntimeInstanceHttpV1(SignedRequestValidationContext context);
        Task<SignedRequestValidationResult> ValidateServiceHttpV1Async(SignedRequestValidationContext context, CancellationToken cancellationToken = default(CancellationToken));
    }
}
