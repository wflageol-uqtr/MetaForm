namespace MetaForm.models
{
    public class ListItem
    {
        public int Id { get; set; }
        public Dictionary<string, string> Values { get; set; } = new Dictionary<string, string>();
    }
}