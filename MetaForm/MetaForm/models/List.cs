using System.Collections.Generic;

namespace MetaForm.Models
{
    public class List
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> Columns { get; set; } = new List<string> { "ID" }; // Colonne ID par défaut
        public List<Dictionary<string, string>> Items { get; set; } = new List<Dictionary<string, string>>();

        // Constructeur pour initialiser la colonne ID si nécessaire
        public List()
        {
            if (!Columns.Contains("ID"))
            {
                Columns.Insert(0, "ID");
            }
        }
    }
}
