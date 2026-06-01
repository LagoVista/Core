using LagoVista.Core.Attributes;
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public interface ISummaryListProviderRegistry
    {
        void Add(SummaryListProviderDefinition definition);

        SummaryListProviderDefinition GetRequired(string modelName, string scope = SummaryListProviderScopes.Organization);

        bool TryGet(string modelName, out SummaryListProviderDefinition definition, string scope = SummaryListProviderScopes.Organization);

        IReadOnlyList<SummaryListProviderDefinition> GetAll();
    }

    public interface ISummaryListProviderExtractor
    {
        void RegisterAssembly(Assembly assembly);
    }

    public interface ISummaryListProviderInvoker
    {
        Task<object> InvokeAsync(GetSummaryRecordsRequest request, EntityHeader org, EntityHeader user, CancellationToken cancellationToken = default(CancellationToken));
    }
}
