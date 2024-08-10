namespace MetaForm.models
{
    public class List
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> Columns { get; set; } = new List<string>();
        public List<ListItem> Items { get; set; } = new List<ListItem>();
    }
}