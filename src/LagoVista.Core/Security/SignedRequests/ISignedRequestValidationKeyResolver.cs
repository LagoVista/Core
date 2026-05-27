namespace LagoVista.Core.Security
{
    public interface ISignedRequestValidationKeyResolver
    {
        SignedRequestValidationKey Resolve(string appKey, string keyId, string algorithm);
    }
}
