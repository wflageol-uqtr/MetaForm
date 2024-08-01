using System.IO; // Pour les opérations de fichier
using System.Text.Json; // Pour la sérialisation et la désérialisation JSON
using MetaForm.Data;
using Microsoft.AspNetCore.Mvc; // Pour les fonctionnalités MVC

namespace MetaForm.Controllers
{
    [ApiController] // Indique que cette classe gère les requêtes HTTP en tant que contrôleur API
    [Route("api/[controller]")] // Définit la route pour accéder à ce contrôleur
    public class FormController : ControllerBase
    {
        private readonly FormDataService _formDataService; // Service pour manipuler les données de formulaire
        private readonly string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "formData.json"); // Chemin du fichier JSON

        // Constructeur du contrôleur, initialise le service de données de formulaire
        public FormController(FormDataService formDataService)
        {
            _formDataService = formDataService;
        }

        // Méthode HTTP POST pour sauvegarder un enregistrement
        [HttpPost("saveRecord")]
        public IActionResult SaveRecord([FromBody] FormData data)
        {
            var existingData = ReadJsonFile(); // Lire les données existantes à partir du fichier JSON
            existingData[data.RecordId] = data.formData; // Mettre à jour ou ajouter les nouvelles données
            WriteJsonFile(existingData); // Écrire les données mises à jour dans le fichier JSON

            return Ok(new { Message = "Record saved successfully" }); // Retourner un message de succès
        }

        // Méthode HTTP GET pour obtenir un enregistrement spécifique
        [HttpGet("getRecord/{recordId}")]
        public IActionResult GetRecord(string recordId)
        {
            var existingData = ReadJsonFile(); // Lire les données existantes à partir du fichier JSON
            if (existingData.ContainsKey(recordId))
            {
                return Ok(existingData[recordId]); // Si l'enregistrement existe, le retourner
            }
            return NotFound(new { Message = "Record not found" }); // Sinon, retourner un message d'erreur
        }

        // Méthode HTTP POST pour associer un fichier à un élément
        [HttpPost("associateFile")]
        public IActionResult AssociateFile([FromBody] FileAssociationRequest request)
        {
            var existingData = ReadJsonFile();
            if (existingData.ContainsKey(request.ListId.ToString()))
            {
                var list = existingData[request.ListId.ToString()];
                if (list.ContainsKey(request.ItemId.ToString()))
                {
                    var item = list[request.ItemId.ToString()] as Dictionary<string, object>;
                    if (item != null)
                    {
                        item["FilePath"] = request.FilePath;
                        WriteJsonFile(existingData);
                        return Ok(new { Message = "File associated successfully" });
                    }
                }
            }
            return NotFound(new { Message = "List or Item not found" });
        }

        // Méthode pour lire le fichier JSON et retourner les données sous forme de dictionnaire
        private Dictionary<string, Dictionary<string, object>> ReadJsonFile()
        {
            if (!System.IO.File.Exists(jsonFilePath))
            {
                return new Dictionary<string, Dictionary<string, object>>(); // Si le fichier n'existe pas, retourner un dictionnaire vide
            }

            var jsonData = System.IO.File.ReadAllText(jsonFilePath); // Lire tout le contenu du fichier JSON
            return JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, object>>>(jsonData); // Désérialiser le JSON en dictionnaire
        }

        // Méthode pour écrire les données dans le fichier JSON
        private void WriteJsonFile(Dictionary<string, Dictionary<string, object>> data)
        {
            var jsonData = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true }); // Sérialiser les données avec une mise en forme
            System.IO.File.WriteAllText(jsonFilePath, jsonData); // Écrire les données dans le fichier
        }
    }

    // Classe pour représenter les données de formulaire
    public class FormData
    {
        public string ListName { get; set; } = string.Empty; // Nom de la liste
        public string RecordId { get; set; } = string.Empty; // ID de l'enregistrement
        public Dictionary<string, object> formData { get; set; } = new Dictionary<string, object>(); // Dictionnaire contenant les données du formulaire
    }

    // Classe pour la requête d'association de fichier
    public class FileAssociationRequest
    {
        public int ListId { get; set; }
        public int ItemId { get; set; }
        public string FilePath { get; set; } = string.Empty;
    }
}
