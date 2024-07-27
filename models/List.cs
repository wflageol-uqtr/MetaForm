namespace MetaForm.models
{
    public class List
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> Columns { get; set; } = new List<string>();
        public List<Dictionary<string, string>> Items { get; set; } = new List<Dictionary<string, string>>();
    }
}
