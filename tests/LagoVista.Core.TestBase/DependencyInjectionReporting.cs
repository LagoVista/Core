using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LagoVista.Core.TestBase
{
    public static class DependencyErrorFormatter
    {
        public static string Format(Exception ex)
        {
            var lines = new List<string>();
            Flatten(ex, lines);

            var friendlyLines = lines
                .Select(TrySimplifyLine)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Distinct()
                .OrderBy(line => line)
                .ToList();

            var sb = new StringBuilder();
            sb.AppendLine("Dependency injection validation failed.");

            if (friendlyLines.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine($"Resolved issues found ({friendlyLines.Count}):");
                foreach (var line in friendlyLines)
                {
                    sb.AppendLine($" - {line}");
                }
            }
            else
            {
                sb.AppendLine();
                sb.AppendLine(ex.ToString());
            }

            return sb.ToString();
        }

        private static void Flatten(Exception ex, List<string> lines)
        {
            if (ex == null)
            {
                return;
            }

            lines.Add(ex.Message);

            if (ex is AggregateException aggregateException)
            {
                foreach (var inner in aggregateException.Flatten().InnerExceptions)
                {
                    Flatten(inner, lines);
                }
            }
            else if (ex.InnerException != null)
            {
                Flatten(ex.InnerException, lines);
            }
        }

        private static string TrySimplifyLine(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return null;
            }

            var descriptorMatch = Regex.Match(
                message,
                @"ImplementationType:\s(?<impl>[^:]+):\sUnable to resolve service for type\s'(?<missing>[^']+)'\swhile attempting to activate\s'(?<activator>[^']+)'");

            if (descriptorMatch.Success)
            {
                var impl = ShortName(descriptorMatch.Groups["impl"].Value);
                var missing = ShortName(descriptorMatch.Groups["missing"].Value);
                var activator = ShortName(descriptorMatch.Groups["activator"].Value);

                return $"{impl} cannot be created because {missing} is not registered (while activating {activator}).";
            }

            var activationMatch = Regex.Match(
                message,
                @"Unable to resolve service for type\s'(?<missing>[^']+)'\swhile attempting to activate\s'(?<activator>[^']+)'");

            if (activationMatch.Success)
            {
                var missing = ShortName(activationMatch.Groups["missing"].Value);
                var activator = ShortName(activationMatch.Groups["activator"].Value);

                return $"{activator} is missing dependency {missing}.";
            }

            if (message.StartsWith("Some services are not able to be constructed", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            if (message.StartsWith("Error while validating the service descriptor", StringComparison.OrdinalIgnoreCase))
            {
                return message;
            }

            return message;
        }

        private static string ShortName(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
            {
                return typeName;
            }

            var tickIndex = typeName.IndexOf('`');
            if (tickIndex > 0)
            {
                typeName = typeName.Substring(0, tickIndex);
            }

            var lastDot = typeName.LastIndexOf('.');
            return lastDot >= 0 ? typeName.Substring(lastDot + 1) : typeName;
        }
    }
}
