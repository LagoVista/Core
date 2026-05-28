namespace LagoVista.Core.Interfaces
{
    public interface ISignedServiceHttpClientSettings
    {
        string AppKey { get; }
        string ServiceSigningKeyId { get; }
        string ServiceSigningAlgorithm { get; }
        string ServiceSigningKeyMaterialFormat { get; }
        string ServiceSigningPrivateKeyMaterial { get; }
        string Version { get; }
        int TimeoutSeconds { get; }
    }
}
