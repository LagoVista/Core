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
