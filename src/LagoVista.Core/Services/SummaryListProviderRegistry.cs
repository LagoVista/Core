using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Services
{
    public class SummaryListProviderRegistry : ISummaryListProviderRegistry
    {
        private static readonly SummaryListProviderRegistry _instance = new SummaryListProviderRegistry();

        public static SummaryListProviderRegistry Instance
        {
            get { return _instance; }
        }

        private readonly Dictionary<string, SummaryListProviderDefinition> _providers = new Dictionary<string, SummaryListProviderDefinition>(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _registeredAssemblies = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public void RegisterAssembly(Assembly assembly, IEntityTypeResolver entityTypeResolver)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            if (entityTypeResolver == null)
                throw new ArgumentNullException(nameof(entityTypeResolver));

            if (!_registeredAssemblies.Add(assembly.FullName))
                return;

            foreach (var type in assembly.DefinedTypes)
            {
                foreach (var method in type.DeclaredMethods.Where(method => !method.IsSpecialName))
                {
                    var attr = method.GetCustomAttribute<SummaryListProviderAttribute>();
                    if (attr == null)
                        continue;

                    var modelType = attr.ModelType ?? throw new InvalidOperationException($"Summary list provider [{type.FullName}.{method.Name}] did not provide a model type.");

                    if (!entityTypeResolver.TryGetEntityType(modelType.Name, out var registeredEntityType))
                        throw new InvalidOperationException($"Summary list provider [{type.FullName}.{method.Name}] references model type [{modelType.FullName}], but that model has not been registered with MetaDataHelper.");

                    if (registeredEntityType != modelType)
                        throw new InvalidOperationException($"Summary list provider [{type.FullName}.{method.Name}] references model type [{modelType.FullName}], but MetaDataHelper resolves [{modelType.Name}] to [{registeredEntityType.FullName}].");


                    var methodBinding = ResolveServiceMethodBinding(type.AsType(), method, attr);
                    var summaryType = GetSummaryType(methodBinding.Method);

                    var definition = new SummaryListProviderDefinition
                    {
                        ModelName = attr.ModelName,
                        Scope = String.IsNullOrWhiteSpace(attr.Scope) ? SummaryListProviderScopes.Organization : attr.Scope,
                        Description = attr.Description,
                        EntityType = modelType,
                        SummaryType = summaryType,
                        ServiceType = methodBinding.ServiceType,
                        Method = methodBinding.Method,
                        MethodName = methodBinding.Method.Name,
                        Parameters = methodBinding.Method.GetParameters().Select(parameter => new SummaryListProviderParameterDefinition
                        {
                            Name = parameter.Name,
                            ParameterType = parameter.ParameterType
                        }).ToList()
                    };

                    ValidateBindableParameters(definition);
                    Add(definition);
                }
            }
        }

        public SummaryListProviderDefinition GetRequired(string modelName, string scope = SummaryListProviderScopes.Organization)
        {
            if (!TryGet(modelName, out var definition, scope))
                throw new KeyNotFoundException($"Could not find summary list provider for model [{modelName}] and scope [{scope}].");

            return definition;
        }

        public bool TryGet(string modelName, out SummaryListProviderDefinition definition, string scope = SummaryListProviderScopes.Organization)
        {
            definition = null;

            if (String.IsNullOrWhiteSpace(modelName))
                return false;

            if (String.IsNullOrWhiteSpace(scope))
                return false;

            return _providers.TryGetValue(CreateKey(modelName, scope), out definition);
        }

        public IReadOnlyList<SummaryListProviderDefinition> GetAll()
        {
            return _providers.Values.OrderBy(provider => provider.ModelName).ThenBy(provider => provider.Scope).ToList();
        }

        public void Add(SummaryListProviderDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            var key = CreateKey(definition.ModelName, definition.Scope);

            if (_providers.TryGetValue(key, out var existingDefinition))
                throw new InvalidOperationException($"Duplicate summary list provider for model [{definition.ModelName}] and scope [{definition.Scope}]. Existing provider: [{existingDefinition.ServiceType.FullName}.{existingDefinition.MethodName}], duplicate provider: [{definition.ServiceType.FullName}.{definition.MethodName}].");

            _providers.Add(key, definition);
        }

        private static SummaryListProviderMethodBinding ResolveServiceMethodBinding(Type declaringType, MethodInfo method, SummaryListProviderAttribute attr)
        {
            if (declaringType == null)
                throw new ArgumentNullException(nameof(declaringType));

            if (method == null)
                throw new ArgumentNullException(nameof(method));

            if (attr == null)
                throw new ArgumentNullException(nameof(attr));

            if (declaringType.IsInterface)
            {
                if (attr.ServiceType != null)
                    throw new InvalidOperationException($"Summary list provider [{declaringType.FullName}.{method.Name}] is declared on an interface and must not specify ServiceType.");

                return new SummaryListProviderMethodBinding
                {
                    ServiceType = declaringType,
                    Method = method
                };
            }

            if (attr.ServiceType == null)
                throw new InvalidOperationException($"Summary list provider [{declaringType.FullName}.{method.Name}] is declared on a concrete class and must specify ServiceType.");

            if (!attr.ServiceType.IsInterface)
                throw new InvalidOperationException($"Summary list provider [{declaringType.FullName}.{method.Name}] specifies ServiceType [{attr.ServiceType.FullName}], but ServiceType must be an interface.");

            if (!attr.ServiceType.IsAssignableFrom(declaringType))
                throw new InvalidOperationException($"Summary list provider [{declaringType.FullName}.{method.Name}] specifies ServiceType [{attr.ServiceType.FullName}], but [{declaringType.FullName}] does not implement that interface.");

            return new SummaryListProviderMethodBinding
            {
                ServiceType = attr.ServiceType,
                Method = FindMatchingInterfaceMethod(attr.ServiceType, method)
            };
        }

        private static MethodInfo FindMatchingInterfaceMethod(Type serviceType, MethodInfo implementationMethod)
        {
            var candidates = serviceType.GetMethods().Where(candidate =>
                String.Equals(candidate.Name, implementationMethod.Name, StringComparison.Ordinal) &&
                ParametersMatch(candidate, implementationMethod)).ToList();

            if (candidates.Count == 0)
                throw new InvalidOperationException($"Could not find matching interface method [{serviceType.FullName}.{implementationMethod.Name}] for implementation method [{implementationMethod.DeclaringType.FullName}.{implementationMethod.Name}].");

            if (candidates.Count > 1)
                throw new InvalidOperationException($"Found multiple matching interface methods [{serviceType.FullName}.{implementationMethod.Name}] for implementation method [{implementationMethod.DeclaringType.FullName}.{implementationMethod.Name}].");

            return candidates[0];
        }

        private static bool ParametersMatch(MethodInfo left, MethodInfo right)
        {
            var leftParameters = left.GetParameters();
            var rightParameters = right.GetParameters();

            if (leftParameters.Length != rightParameters.Length)
                return false;

            for (var idx = 0; idx < leftParameters.Length; ++idx)
            {
                if (leftParameters[idx].ParameterType != rightParameters[idx].ParameterType)
                    return false;
            }

            return true;
        }

        private static Type GetSummaryType(MethodInfo method)
        {
            var returnType = method.ReturnType;

            if (!returnType.IsGenericType || returnType.GetGenericTypeDefinition() != typeof(Task<>))
                throw new InvalidOperationException($"Summary list provider method [{method.DeclaringType.FullName}.{method.Name}] must return Task<ListResponse<TSummary>>.");

            var taskResultType = returnType.GetGenericArguments()[0];

            if (!taskResultType.IsGenericType || taskResultType.GetGenericTypeDefinition() != typeof(ListResponse<>))
                throw new InvalidOperationException($"Summary list provider method [{method.DeclaringType.FullName}.{method.Name}] must return Task<ListResponse<TSummary>>.");

            var summaryType = taskResultType.GetGenericArguments()[0];
            return summaryType;
        }

        private static void ValidateBindableParameters(SummaryListProviderDefinition definition)
        {
            foreach (var parameter in definition.Method.GetParameters())
            {
                if (parameter.ParameterType == typeof(ListRequest))
                    continue;

                if (parameter.ParameterType == typeof(CancellationToken))
                    continue;

                if (parameter.ParameterType == typeof(EntityHeader))
                {
                    if (IsOrgParameter(parameter.Name) || IsUserParameter(parameter.Name))
                        continue;

                    throw new InvalidOperationException($"Summary list provider method [{definition.ServiceType.FullName}.{definition.MethodName}] has EntityHeader parameter [{parameter.Name}], but only org/organization or user parameter names are supported.");
                }

                if (parameter.ParameterType == typeof(string))
                {
                    if (IsOrgIdParameter(parameter.Name) || IsCategoryParameter(parameter.Name))
                        continue;

                    throw new InvalidOperationException($"Summary list provider method [{definition.ServiceType.FullName}.{definition.MethodName}] has string parameter [{parameter.Name}], but only orgId/organizationId or category/categoryKey parameter names are supported.");
                }

                throw new InvalidOperationException($"Summary list provider method [{definition.ServiceType.FullName}.{definition.MethodName}] has unsupported parameter [{parameter.Name}] of type [{parameter.ParameterType.FullName}].");
            }
        }

        private static string CreateKey(string modelName, string scope)
        {
            return $"{scope}::{modelName}";
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

        private class SummaryListProviderMethodBinding
        {
            public Type ServiceType { get; set; }

            public MethodInfo Method { get; set; }
        }
    }
}