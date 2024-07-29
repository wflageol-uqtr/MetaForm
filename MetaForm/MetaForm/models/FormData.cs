namespace MetaForm.Models
{
    public class FormData
    {
        public string ListName { get; set; } = string.Empty; // Initialisation pour éviter les nulls
        public string RecordId { get; set; } = string.Empty; // Initialisation pour éviter les nulls
        public Dictionary<string, object> formData { get; set; } = new Dictionary<string, object>(); // Initialisation pour éviter les nulls
    }
}
