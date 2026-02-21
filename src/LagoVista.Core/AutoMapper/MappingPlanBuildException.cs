using System;
using System.Collections.Generic;
using System.Linq;

namespace LagoVista.Core.AutoMapper
{
    public sealed class MappingPlanBuildException : Exception
    {
        public IReadOnlyList<string> Errors { get; }

        public MappingPlanBuildException(IEnumerable<string> errors)
            : base(BuildMessage(errors))
        {
            Errors = (errors ?? throw new ArgumentNullException(nameof(errors)))
                .Where(e => !String.IsNullOrWhiteSpace(e))
                .ToArray();
        }

        private static string BuildMessage(IEnumerable<string> errors)
        {
            if (errors == null) return "Mapping plan build failed.";

            var list = errors.Where(e => !String.IsNullOrWhiteSpace(e)).ToList();
            if (list.Count == 0) return "Mapping plan build failed.";

            return "Mapping plan build failed:\n" + String.Join("\n", list.Select(e => "- " + e));
        }
    }
}
