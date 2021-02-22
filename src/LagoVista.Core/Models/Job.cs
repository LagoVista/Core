namespace LagoVista.Core.Models
{
    public enum JobStatus
    {
        New,
        Queued,
        Running,
        Completed,
        Error,
    }

    public class Job
    {
        /// <summary>
        /// Unique ID for the Job, each job/process execution will have a unique job id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// This is the ID that will be used to send notifications as to job status via a push mechanism, consumers
        /// can subscribe to this, primarily through a web socket.
        /// </summary>
        public string NotificationId { get; set; }

        /// <summary>
        /// Used to identify the job type id, this is usually specific to the job that will be picked up
        /// the processor needs to know how to handle the job.
        /// 
        /// It will be specific to the processoor.
        /// </summary>
        public EntityHeader JobType { get; set; }

        /// <summary>
        /// In some cases a job/report may have versions that we want to run in parallel,
        /// this can be added in the JobVersion field if present, if not it will use the 
        /// most current or the one default one associated with the job/report
        /// 
        /// It will be specific to the processoor.
        /// </summary>
        public EntityHeader JobVersion { get; set; }

        /// <summary>
        /// Name of the organization
        /// </summary>
        public EntityHeader Organization { get; set; }

        /// <summary>
        /// User that requested the report (optional)
        /// </summary>
        public EntityHeader RequestedByUser { get; set; }

        /// <summary>
        /// Distribution list of users that should be notified when this job has been completed
        /// as well as receive any work products generated from this job.
        /// </summary>
        public EntityHeader DistroList { get; set; }

        /// <summary>
        /// True if this was initiated by a schedule false, if by a user.
        /// </summary>
        public bool Scheduled { get; set; }

        /// <summary>
        /// Procesor will be used to determine which queue will be used to process the job
        /// </summary>
        public EntityHeader Processor { get; set; }

        /// <summary>
        /// Progress Percent (optional, should mark as 100% when completed)
        /// </summary>
        public int PercentComplete { get; set; }

        /// <summary>
        // Current status of the job.
        /// </summary>
        public JobStatus Status { get; set; }

        /// <summary>
        /// Date stamp of when the job was queued, or empty if not queued.
        /// </summary>
        public string QueuedTimeStamp { get; set; }

        /// <summary>
        /// Timestamp of when the job was started.
        /// </summary>
        public string StartedTimeStamp { get; set; }

        /// <summary>
        /// Timestamp of when the job was completed.
        /// </summary>
        public string CompletedTimeStamp { get; set; }

        /// <summary>
        /// Total Execution Time for Job
        /// </summary>
        public double? ExecutionTimeSeconds { get; set; }

        /// <summary>
        /// Most recent error message.
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// Payload for the job, will be stored in blob storage.
        /// </summary>
        public string Payload { get; set; }
    }
}
