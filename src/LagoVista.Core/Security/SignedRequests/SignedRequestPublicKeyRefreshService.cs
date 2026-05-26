using System;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Security
{
    public class SignedRequestPublicKeyRefreshService : ISignedRequestPublicKeyRefreshService
    {
        private readonly ISignedRequestPublicKeySetClient _client;
        private readonly ISignedRequestPublicKeySetStore _store;
        private readonly SemaphoreSlim _refreshLock = new SemaphoreSlim(1, 1);

        public SignedRequestPublicKeyRefreshService(ISignedRequestPublicKeySetClient client, ISignedRequestPublicKeySetStore store)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public async Task<SignedRequestPublicKeyRefreshResult> RefreshAsync(CancellationToken cancellationToken)
        {
            await _refreshLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                var keySet = await _client.FetchAsync(cancellationToken).ConfigureAwait(false);
                _store.Update(keySet);
                return SignedRequestPublicKeyRefreshResult.Success(keySet.Version);
            }
            catch (Exception ex)
            {
                return SignedRequestPublicKeyRefreshResult.FromError("public_key_refresh_failed", ex.Message);
            }
            finally
            {
                _refreshLock.Release();
            }
        }
    }
}
