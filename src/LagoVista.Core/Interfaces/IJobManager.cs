// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: d0d37f54a6c8c0686f2f76a478e00a8faaf7cc8d6bf7d27a45bc9e459df597ab
// IndexVersion: 1
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public interface IJobManager
    {
        Task<InvokeResult<string>> EnqueueScheduledReportAsync(string scheduledReportId);
        Task<InvokeResult<string>> EnqueueJobAsync(Job job);
        Task<ListResponse<Job>> GetJobsForJobTypeAsync(string jobTypeId, ListRequest request);
        Task<Job> GetJobAsync(string jobId, string jobTypeId);
        Task<string> GetJobPayloadAsync(string jobId, string jobTypeId);
        Task<InvokeResult> MarkJobAsQueued(string jobId, string jobTypeId);
        Task<InvokeResult> MarkJobAsStarted(string jobId, string jobTypeId);
        Task<InvokeResult> UpdateJobProgress(string jobId, string jobTypeId, int percent);
        Task<InvokeResult> MarkJobAsCompletedAsync(JobOutput output);
        Task<InvokeResult> MarkJobAsFailedAsync(JobOutput output);
    }
}
