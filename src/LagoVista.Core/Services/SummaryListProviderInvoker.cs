using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Services
{
    public class SummaryListProviderInvoker : ISummaryListProviderInvoker
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ISummaryListProviderRegistry _registry;

        public SummaryListProviderInvoker(IServiceProvider serviceProvider, ISummaryListProviderRegistry registry)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        }

        public async Task<object> InvokeAsync(GetSummaryRecordsRequest request, EntityHeader org, EntityHeader user, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (String.IsNullOrWhiteSpace(request.ModelName))
                throw new ArgumentNullException(nameof(request.ModelName));

            if (request.ListRequest == null)
                throw new ArgumentNullException(nameof(request.ListRequest));

            var scope = String.IsNullOrWhiteSpace(request.Scope) ? SummaryListProviderScopes.Organization : request.Scope;
            var definition = _registry.GetRequired(request.ModelName, scope);
            var service = _serviceProvider.GetRequiredService(definition.ServiceType);
            var args = BindArguments(definition, request, org, user, cancellationToken);
            var result = definition.Method.Invoke(service, args);

            if (!(result is Task task))
                throw new InvalidOperationException($"Summary list provider method [{definition.ServiceType.FullName}.{definition.MethodName}] did not return a Task.");

            await task.ConfigureAwait(false);

            var resultProperty = task.GetType().GetProperty("Result", BindingFlags.Public | BindingFlags.Instance);
            if (resultProperty == null)
                throw new InvalidOperationException($"Summary list provider method [{definition.ServiceType.FullName}.{definition.MethodName}] did not return Task<ListResponse<TSummary>>.");

            return resultProperty.GetValue(task);
        }

        private static object[] BindArguments(SummaryListProviderDefinition definition, GetSummaryRecordsRequest request, EntityHeader org, EntityHeader user, CancellationToken cancellationToken)
        {
            var parameters = definition.Method.GetParameters();
            var args = new object[parameters.Length];

            for (var idx = 0; idx < parameters.Length; ++idx)
            {
                var parameter = parameters[idx];

                if (parameter.ParameterType == typeof(ListRequest))
                {
                    args[idx] = request.ListRequest;
                    continue;
                }

                if (parameter.ParameterType == typeof(CancellationToken))
                {
                    args[idx] = cancellationToken;
                    continue;
                }

                if (parameter.ParameterType == typeof(EntityHeader))
                {
                    if (IsOrgParameter(parameter.Name))
                    {
                        args[idx] = org ?? throw new ArgumentNullException(nameof(org));
                        continue;
                    }

                    if (IsUserParameter(parameter.Name))
                    {
                        args[idx] = user ?? throw new ArgumentNullException(nameof(user));
                        continue;
                    }
                }

                if (parameter.ParameterType == typeof(string))
                {
                    if (IsOrgIdParameter(parameter.Name))
                    {
                        if (org == null)
                            throw new ArgumentNullException(nameof(org));

                        args[idx] = org.Id;
                        continue;
                    }

                    if (IsCategoryParameter(parameter.Name))
                    {
                        args[idx] = request.Category;
                        continue;
                    }
                }

                throw new InvalidOperationException($"Could not bind parameter [{parameter.Name}] of type [{parameter.ParameterType.FullName}] for summary list provider [{definition.ServiceType.FullName}.{definition.MethodName}].");
            }

            return args;
        }

        private static bool IsOrgParameter(string name)
        {
            return String.Equals(name, "org", StringComparison.OrdinalIgnoreCase) || String.Equals(name, "organization", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsUserParameter(string name)
        {
            return String.Equals(name, "user", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsOrgIdParameter(string name)
        {
            return String.Equals(name, "orgId", StringComparison.OrdinalIgnoreCase) || String.Equals(name, "organizationId", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsCategoryParameter(string name)
        {
            return String.Equals(name, "category", StringComparison.OrdinalIgnoreCase) || String.Equals(name, "categoryKey", StringComparison.OrdinalIgnoreCase);
        }
    }
}
