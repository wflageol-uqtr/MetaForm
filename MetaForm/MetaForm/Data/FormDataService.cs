using MetaForm.models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MetaForm.Data
{
    public class FormDataService
    {
        private readonly HttpClient _httpClient;
        private readonly string filePath = "data.json";
        private List<List> lists = new List<List>();
        private readonly ILogger<FormDataService> logger;

        public List<List> Lists
        {
            get => lists;
            set => lists = value ?? new List<List>();
        }

        public FormDataService(HttpClient httpClient, ILogger<FormDataService> logger)
        {
            _httpClient = httpClient;
            this.logger = logger;

            try
            {
                if (File.Exists(filePath))
                {
                    var jsonData = File.ReadAllText(filePath);
                    Lists = JsonSerializer.Deserialize<List<List>>(jsonData) ?? new List<List>();
                    logger.LogInformation("Loaded lists from file: {Lists}", jsonData);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error loading lists from file: {Error}", ex.Message);
            }
        }

        public Dictionary<string, object> GetHardcodedData()
        {
            var hardcodedData = new Dictionary<string, object>
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
            logger.LogInformation("Returning hardcoded form data: {HardcodedData}", JsonSerializer.Serialize(hardcodedData));
            return hardcodedData;
        }

        public async Task AssociateFileWithListItem(int listId, int itemId, string filePath)
        {
            logger.LogInformation("Associating file {FilePath} with list ID {ListId} and item ID {ItemId}", filePath, listId, itemId);

            try
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
            catch (Exception ex)
            {
                logger.LogError("Error associating file: {Error}", ex.Message);
            }
        }

        public void SaveChanges()
        {
            try
            {
                var jsonData = JsonSerializer.Serialize(Lists, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, jsonData);
                logger.LogInformation("Saved changes to file: {FilePath}", filePath);
            }
            catch (Exception ex)
            {
                logger.LogError("Error saving changes to file: {Error}", ex.Message);
            }
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
