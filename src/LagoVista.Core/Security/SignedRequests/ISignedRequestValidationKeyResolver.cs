namespace LagoVista.Core.Security
{
    public interface ISignedRequestValidationKeyResolver
    {
        SignedRequestValidationKey Resolve(string callerId, string keyId, string algorithm);
    }
}
