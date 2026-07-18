using LagoVista.Core.Models;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public enum BillingEventType
    {
        AudioGeneration,
        VideoAvatarCreated,
        VideoGenerationStandard,
        VideoGenerationPremium,
        VideoAssembly
    }

    public interface IBillingEventRecorder
    {
        Task RecordUsageAsync(BillingEventType eventType, GuidString36 subscriptionId, double quantity, string note, EntityHeader org, EntityHeader user);
        Task RecordUsageAsync(BillingEventType eventType, double quantity, string note, EntityHeader org, EntityHeader user);
        Task RecordUsageAsync(BillingEventType eventType, double quantity, BillingUsageContext ctx);

    }

    public class BillingUsageContext
    {
        public GuidString36? subscriptionId { get; set; }
        public EntityHeader Organization { get; set; }
        public EntityHeader User { get; set; }
        public string Note { get; set; }
    }
}
