using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Configuration
{
    public static class ConfigurationDiagnostics
    {
        public static string CurrentClassName { get; set; }

        private static List<ConfigurationDiagnosticContext> _diagnostics = new List<ConfigurationDiagnosticContext>();

        public static void AddRegisteredConfiguration(string path, bool keyPresent = true, bool valuePresent = true)
        {
            _diagnostics.Add(new ConfigurationDiagnosticContext()
            {
                Class = CurrentClassName,
                Path = path,
                Optional = false,
                KeyPresent = keyPresent,
                ValuePresent = valuePresent
            });
        }

        public static void AddOptionalConfiguration(string path, bool keyPresent = true, bool valuePresent = true)
        {
            _diagnostics.Add(new ConfigurationDiagnosticContext()
            {
                Class = CurrentClassName,
                Path = path,
                Optional = true,
                KeyPresent = keyPresent,
                ValuePresent = valuePresent
            });
        }

        public static void Reset()
        {
            _diagnostics.Clear();
        }

        public static IReadOnlyList<ConfigurationDiagnosticContext> GetEntries()
        {
            return _diagnostics.AsReadOnly();
        }
    }

    public class ConfigurationDiagnosticContext
    {
        public string Class { get; set; }
        public string Path { get; set; }
        public bool Optional { get; set; }

        public bool KeyPresent { get; set; }
        public bool ValuePresent { get; set; }
    }
}
