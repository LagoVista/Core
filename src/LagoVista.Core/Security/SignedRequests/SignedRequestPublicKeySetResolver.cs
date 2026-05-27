using System;

namespace LagoVista.Core.Security
{
    public class SignedRequestPublicKeySetResolver : ISignedRequestValidationKeyResolver, ISignedRequestPublicKeySetStore
    {
        private readonly object _syncRoot = new object();
        private SignedRequestPublicKeySet _keySet;

        public SignedRequestPublicKeySetResolver(SignedRequestPublicKeySet keySet)
        {
            Update(keySet);
        }

        public SignedRequestPublicKeySet Current
        {
            get
            {
                lock (_syncRoot)
                {
                    return _keySet;
                }
            }
        }

        public void Update(SignedRequestPublicKeySet keySet)
        {
            if (keySet == null) throw new ArgumentNullException(nameof(keySet));
            if (keySet.Keys == null) throw new ArgumentNullException(nameof(keySet.Keys));

            lock (_syncRoot)
            {
                _keySet = keySet;
            }
        }

        public SignedRequestValidationKey Resolve(string callerId, string keyId, string algorithm)
        {
            if (String.IsNullOrWhiteSpace(callerId)) return null;
            if (String.IsNullOrWhiteSpace(keyId)) return null;
            if (String.IsNullOrWhiteSpace(algorithm)) return null;

            SignedRequestPublicKeySet keySet;
            lock (_syncRoot)
            {
                keySet = _keySet;
            }

            var normalizedAlgorithm = SignedRequestSignatureAlgorithms.Normalize(algorithm);

            foreach (var key in keySet.Keys)
            {
                if (key == null)
                {
                    continue;
                }

                if (!String.Equals(key.AppKey, callerId, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (!String.Equals(key.KeyId, keyId, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (!String.Equals(SignedRequestSignatureAlgorithms.Normalize(key.Algorithm), normalizedAlgorithm, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                return key.ToValidationKey();
            }

            return null;
        }
    }
}
