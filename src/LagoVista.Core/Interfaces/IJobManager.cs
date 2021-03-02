using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public interface IJobManager
    {
        Task<InvokeResult<string>> QueueJobAsync(Job job);
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
