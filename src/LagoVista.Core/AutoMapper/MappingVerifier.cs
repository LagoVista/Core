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
        public static void Verify<TSource, TTarget>(bool writePlan = false)
           where TSource : class
           where TTarget : class
        {

            var builder = new ReflectionAtomicPlanBuilder(ConvertersRegistration.DefaultConverterRegistery);
            var toResult = builder.BuildAtomicSteps(typeof(TSource), typeof(TTarget));
            var fromResult = builder.BuildAtomicSteps(typeof(TTarget), typeof(TSource));

            if (writePlan)
            {
                Console.WriteLine($"{typeof(TSource).Name} ({typeof(TSource).GetProperties().Count()}) => {typeof(TTarget).Name} ({typeof(TTarget).GetProperties().Count()})");
                Console.WriteLine("----------------------------");
                foreach (var step in toResult.Result)
                {
                    Console.WriteLine(step);
                }
                Console.WriteLine();

                Console.WriteLine($"{typeof(TTarget).Name} ({typeof(TTarget).GetProperties().Count()}) => {typeof(TSource).Name} ({typeof(TSource).GetProperties().Count()})");
                Console.WriteLine("----------------------------");
                foreach (var step in fromResult.Result)
                {
                    Console.WriteLine(step);
                }

                Console.WriteLine();
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
