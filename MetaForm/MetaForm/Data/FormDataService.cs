using MetaForm.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MetaForm.Data
{
    public class FormDataService
    {
        private readonly HttpClient _httpClient;
        private readonly string filePath = "data.json";
        private List<List> lists = new List<List>();

        public List<List> Lists
        {
            get => lists;
            set => lists = value ?? new List<List>();
        }

        public FormDataService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            if (File.Exists(filePath))
            {
                var jsonData = File.ReadAllText(filePath);
                Lists = JsonSerializer.Deserialize<List<List>>(jsonData) ?? new List<List>();
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

        public async Task AssociateFileWithListItem(int listId, int itemId, string filePath)
        {
            var request = new FileAssociationRequest
            {
                ListId = listId,
                ItemId = itemId,
                FilePath = filePath
            };

            var jsonRequest = JsonSerializer.Serialize(request);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/form/associateFile", content);
            response.EnsureSuccessStatusCode();
        }

        public void SaveChanges()
        {
            var jsonData = JsonSerializer.Serialize(Lists, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonData);
        }

        public async Task SaveChangesAsync()
        {
            await Task.Run(() => SaveChanges());
        }
    }

    public class FileAssociationRequest
    {
        public int ListId { get; set; }
        public int ItemId { get; set; }
        public string FilePath { get; set; } = string.Empty;
    }
}
