using System;

namespace LagoVista.Core.Security
{
    public interface ISignedRequestPublicKeySetStore
    {
        SignedRequestPublicKeySet Current { get; }
        void Update(SignedRequestPublicKeySet keySet);
    }
}
