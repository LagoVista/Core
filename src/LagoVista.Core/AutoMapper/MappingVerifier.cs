using LagoVista.Core.Attributes;
using LagoVista.Core.AutoMapper.Converters;
using LagoVista.Core.Interfaces.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LagoVista.Core.AutoMapper
{
    public static class MappingVerifier
    {
        public sealed class MappingVerificationException : Exception
        {
            public MappingVerificationException(string message) : base(message) { }
        }

        public static void Verify<TSource, TTarget>(bool writePlan = false)
           where TSource : class
           where TTarget : class
        {

            var builder = new ReflectionAtomicPlanBuilder(ConvertersRegistration.DefaultConverterRegistery);
            var rootSteps = builder.BuildAtomicSteps(typeof(TSource), typeof(TTarget));
            var failures = new List<string>();

            if (writePlan)
            {
                Console.WriteLine($"{typeof(TSource).Name} ({typeof(TSource).GetProperties().Count()}) => {typeof(TTarget).Name} ({typeof(TTarget).GetProperties().Count()})");
                Console.WriteLine("----------------------------");
                foreach (var step in rootSteps.Result)
                {
                    Console.WriteLine(step);
                }
                Console.WriteLine();
            }

            var rootMissing = GetMissingTargetProperties(typeof(TTarget), rootSteps.Result);
            if (rootMissing.Count > 0)
            {
                failures.Add(FormatMissing("[root]", typeof(TSource), typeof(TTarget), rootMissing));
            }

            var graph = new ReflectionMappingPlanGraphValidator(builder);

            foreach (var pair in graph.EnumeratePairs(typeof(TSource), typeof(TTarget)))
            {
                if (pair.Depth == 0) continue; // root already printed above

                var indent = new string(' ', pair.Depth * 2);
                var steps = builder.BuildAtomicSteps(pair.SourceType, pair.TargetType).Result;

                Console.WriteLine($"{indent}{pair.Path}");
                Console.WriteLine($"{indent}{pair.SourceType.Name} ({pair.SourceType.GetProperties().Count()}) => {pair.TargetType.Name} ({pair.TargetType.GetProperties().Count()})");
                Console.WriteLine($"{indent}----------------------------");
                foreach (var step in steps)
                    Console.WriteLine($"{indent}{step}");
                Console.WriteLine();
            }

            if (failures.Count > 0)
            {
                throw new MappingVerificationException(
                    "Mapping verification failed. Every writable target property must be mapped or ignored.\\n\\n" +
                    string.Join("\\n\\n", failures));
            }
        }

        private static string FormatMissing(string path, Type sourceType, Type targetType, IReadOnlyList<PropertyInfo> missing)
        {
            // include type info so it’s immediately actionable
            var items = missing.Select(p => $"  - {p.Name} : {p.PropertyType.Name}");
            return $"{path} {sourceType.Name} => {targetType.Name}\\nMissing target properties:\\n{string.Join("\\n", items)}";
        }

        private static IReadOnlyList<PropertyInfo> GetRequiredTargetProperties(Type targetType)
        {
            return targetType
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.GetIndexParameters().Length == 0) // no indexers
                .Where(p => p.CanWrite)                         // writable only
                .ToList();
        }

        private static IReadOnlyList<PropertyInfo> GetMissingTargetProperties(
            Type targetType,
            IReadOnlyList<AtomicMapStep> steps)
        {
            var required = GetRequiredTargetProperties(targetType);
            var covered = new HashSet<string>(StringComparer.Ordinal);

            foreach (var step in steps)
            {
                if (step.TargetProperty != null)
                {
                    covered.Add(step.TargetProperty.Name);
                }
            }
            ;

            var missing = required
                .Where(p => !covered.Contains(p.Name))
                .OrderBy(p => p.Name, StringComparer.Ordinal)
                .ToList();

            return missing;
        }


        public class MappingPair
        {
            public MappingPair(Type sourceType, Type targetType, string path, int depth)
            {
                SourceType = sourceType;
                TargetType = targetType;
                Path = path;
                Depth = depth;
            }

            public Type SourceType { get; }
            public Type TargetType { get; }

            public string Path { get;  }
            public int Depth { get; }  

        } 


    }
}
