using MetaForm.models;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;
using System.Threading.Tasks;

namespace MetaForm.Data
{
    public class FormDataService
    {
        private readonly string filePath = "data.json";
        private List<List> lists;

        public List<List> Lists { get => lists; set => lists = value; }

        public FormDataService()
        {
            if (File.Exists(filePath))
            {
                var jsonData = File.ReadAllText(filePath);
                lists = JsonSerializer.Deserialize<List<List>>(jsonData) ?? new List<List>();
            }
            else
            {
                lists = new List<List>();
            }
        }

        public Dictionary<string, object> GetHardcodedData()
        {
            return new Dictionary<string, object>
            {
                { "name", "RXX" },
                { "region", "north" },
                { "ug", "UG001" },
                { "ua", "UA001" },
                { "anneeReference", "2024" },
                { "intervenant", "John Doe" },
                { "representantIntervenant", "Jane Smith" },
                { "noEntenteContratPermis", "12345" },
                { "detecteLors", "Inspection" },
                { "detectePar", "Agent X" },
                { "detecteLe", "2024-01-01" }
            };
        }

        public Task AssociateFileWithListItem(int listId, int itemId, string filePath)
        {
            var list = lists.FirstOrDefault(l => l.Id == listId);
            if (list != null)
            {
                var item = list.Items.FirstOrDefault(i => i["Id"] == itemId.ToString());
                if (item != null)
                {
                    item["FilePath"] = filePath;
                    SaveChanges();
                    Console.Error.WriteLine($"File path {filePath} associated with item ID {itemId} in list ID {listId}.");
                }
                else
                {
                    Console.Error.WriteLine($"Item with ID {itemId} not found in list ID {listId}.");
                }
            }
            else
            {
                Console.Error.WriteLine($"List with ID {listId} not found.");
            }
            return Task.CompletedTask;
        }

        private void SaveChanges()
        {
            var jsonData = JsonSerializer.Serialize(lists, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonData);
            Console.Error.WriteLine($"Data saved to {filePath}.");
        }
    }
}
