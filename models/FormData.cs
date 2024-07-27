namespace MetaForm.Models
{
    public class FormData
    {
        public string ListName { get; set; } // Nom de la liste
        public string RecordId { get; set; } // ID de l'enregistrement
        public Dictionary<string, object> formData { get; set; } // Dictionnaire contenant les données du formulaire
    }
}
