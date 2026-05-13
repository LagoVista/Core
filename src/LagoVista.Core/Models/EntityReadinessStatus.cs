using LagoVista.Core.Attributes;
using LagoVista.Core.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Models
{
    public enum EntityReadinessLevel
    {
        [EnumLabel(EntityReadinessStatus.ReadinessLevel_NotReviewed, LagoVistaCommonStrings.Names.EntityReadinessLevel_NotReviewed, typeof(LagoVistaCommonStrings))]
        NotReviewed,

        [EnumLabel(EntityReadinessStatus.ReadinessLevel_NeedsWork, LagoVistaCommonStrings.Names.EntityReadinessLevel_NeedsWork, typeof(LagoVistaCommonStrings))]
        NeedsWork,

        [EnumLabel(EntityReadinessStatus.ReadinessLevel_MostlyReady, LagoVistaCommonStrings.Names.EntityReadinessLevel_MostlyReady, typeof(LagoVistaCommonStrings))]
        MostlyReady,

        [EnumLabel(EntityReadinessStatus.ReadinessLevel_Ready, LagoVistaCommonStrings.Names.EntityReadinessLevel_Ready, typeof(LagoVistaCommonStrings))]
        Ready
    }

    public class EntityReadinessStatus
    {
        public const string ReadinessLevel_NotReviewed = "notreviewed";
        public const string ReadinessLevel_NeedsWork = "needswork";
        public const string ReadinessLevel_MostlyReady = "mostlyready";
        public const string ReadinessLevel_Ready = "ready";

        public double Score { get; set; }
        public double Confidence { get; set; }
        public EntityHeader<EntityReadinessLevel> Level { get; set; }
        public string Summary { get; set; }
        public int BlockingIssueCount { get; set; }
        public int WarningCount { get; set; }
        public string LastReviewedUtc { get; set; }
        public EntityHeader LastReviewedBy { get; set; }
    }

    public class EntityReadinessRecommendation
    {
        public string Priority { get; set; }

        public string Message { get; set; }

        public List<string> RelatedFields { get; set; } = new List<string>();
    }

    public class EntityReadinessCriterionResult
    {
        public const string ReadinessFindingSeverity_Blocking = "blocking";
        public const string ReadinessFindingSeverity_Warning = "warning";
        public const string ReadinessFindingSeverity_Info = "info";

        public const string ReadinessRecommendationPriority_High = "high";
        public const string ReadinessRecommendationPriority_Medium = "medium";
        public const string ReadinessRecommendationPriority_Low = "low";

        public const string ReadinessCriterionStatus_Pass = "pass";
        public const string ReadinessCriterionStatus_Warning = "warning";
        public const string ReadinessCriterionStatus_Fail = "fail";
        public const string ReadinessCriterionStatus_NotApplicable = "notApplicable";

        public string CriterionKey { get; set; }

        public string Name { get; set; }

        public double Score { get; set; }

        public double Confidence { get; set; }

        public string Status { get; set; }

        public string Finding { get; set; }

        public string Recommendation { get; set; }

        public List<string> RelatedFields { get; set; } = new List<string>();

        public bool IsBlocking { get; set; }
    }

    public class EntityReadinessFinding
    {
        public string Severity { get; set; }

        public string Message { get; set; }

        public List<string> RelatedFields { get; set; } = new List<string>();
    }

    public class EntityReadinessReport
    {
        public double OverallScore { get; set; }

        public double Confidence { get; set; }

        public EntityHeader<EntityReadinessLevel> Level { get; set; }

        public string Summary { get; set; }

        public List<EntityReadinessCriterionResult> CriteriaResults { get; set; } = new List<EntityReadinessCriterionResult>();

        public List<EntityReadinessFinding> Findings { get; set; } = new List<EntityReadinessFinding>();

        public List<EntityReadinessRecommendation> Recommendations { get; set; } = new List<EntityReadinessRecommendation>();
    }
}
