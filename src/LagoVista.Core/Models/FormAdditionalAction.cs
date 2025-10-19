// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 9563f9f5ed78c7e304e4491d0dfa35d652711b1bb276da982330e07c9b0d6711
// IndexVersion: 0
// --- END CODE INDEX META ---
namespace LagoVista.Core.Models
{
    public class FormAdditionalAction
    {
        public string Title { get; set; }
        public string Icon { get; set; }
        public string Help { get; set; }
        public string Key { get; set; }
        public bool ForCreate { get; set; } = true;
        public bool ForEdit { get; set; } = true;
    }
}
