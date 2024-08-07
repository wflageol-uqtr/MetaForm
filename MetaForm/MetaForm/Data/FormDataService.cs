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
        private readonly string associationsFilePath = "fileAssociations.json";
        private Dictionary<string, int> fileAssociations = new Dictionary<string, int>();
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

                if (File.Exists(associationsFilePath))
                {
                    var associationsData = File.ReadAllText(associationsFilePath);
                    fileAssociations = JsonSerializer.Deserialize<Dictionary<string, int>>(associationsData) ?? new Dictionary<string, int>();
                    logger.LogInformation("Loaded file associations from file: {Associations}", associationsData);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error loading data from file: {Error}", ex.Message);
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

        public async Task AssociateFileWithListItemAsync(string fileName, int listId)
        {
            fileAssociations[fileName] = listId;
            await SaveChangesAsync();
        }

        private void SaveChanges()
        {
            try
            {
                var jsonData = JsonSerializer.Serialize(Lists, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, jsonData);

                var associationsData = JsonSerializer.Serialize(fileAssociations, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(associationsFilePath, associationsData);

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

        public async Task<int?> GetListIdForFileAsync(string fileName)
        {
            return fileAssociations.TryGetValue(fileName, out var listId) ? listId : (int?)null;
        }
    }

    public class FileAssociationRequest
    {
        public int ListId { get; set; }
        public int ItemId { get; set; }
        public string FilePath { get; set; } = string.Empty;
    }
}
