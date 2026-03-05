using LagoVista.Core.Models;

namespace LagoVista.Crypto.Modern
{
    public interface IAadBuilder
    {
        byte[] BuildAadV1(GuidString36 orgId, GuidString36 recId, string fieldNameLower, string keyId);
    }
}
