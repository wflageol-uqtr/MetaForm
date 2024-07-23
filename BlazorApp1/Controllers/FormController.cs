using System.IO; // Pour les opérations de fichier
using System.Text.Json; // Pour la sérialisation et la désérialisation JSON
using Microsoft.AspNetCore.Mvc;

[ApiController] 
[Route("api/[controller]")] 
public class FormController : ControllerBase
{
    private readonly FormDataService _formDataService; 
    private readonly string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "formData.json"); 

    // Constructeur du contrôleur, initialise le service de données de formulaire
    public FormController(FormDataService formDataService)
    {
        _formDataService = formDataService;
    }

    // Méthode HTTP POST pour sauvegarder un enregistrement
    [HttpPost("saveRecord")]
    public IActionResult SaveRecord([FromBody] FormData data)
    {
        var existingData = ReadJsonFile(); 
        existingData[data.RecordId] = data.formData; // Mettre à jour ou ajouter les nouvelles données
        WriteJsonFile(existingData); 

        return Ok(new { Message = "Record saved successfully" }); // Retourner un message de succès
    }

    // Méthode HTTP GET pour obtenir un enregistrement spécifique
    [HttpGet("getRecord/{recordId}")]
    public IActionResult GetRecord(string recordId)
    {
        var existingData = ReadJsonFile(); 
        if (existingData.ContainsKey(recordId))
        {
            return Ok(existingData[recordId]); 
        }
        return NotFound(new { Message = "Record not found" }); // Sinon, retourner un message d'erreur
    }

    // Méthode pour lire le fichier JSON et retourner les données sous forme de dictionnaire
    private Dictionary<string, Dictionary<string, object>> ReadJsonFile()
    {
        if (!System.IO.File.Exists(jsonFilePath)) // Si le fichier n'existe pas, retourner un dictionnaire vide
        {
            return new Dictionary<string, Dictionary<string, object>>();
        }

        var jsonData = System.IO.File.ReadAllText(jsonFilePath); 
        return JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, object>>>(jsonData); 
    }

    // Méthode pour écrire les données dans le fichier JSON
    private void WriteJsonFile(Dictionary<string, Dictionary<string, object>> data)
    {
        var jsonData = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true }); 
        System.IO.File.WriteAllText(jsonFilePath, jsonData); // Écrire les données dans le fichier
    }
}

// Classe pour représenter les données de formulaire
public class FormData
{
    public string ListName { get; set; } 
    public string RecordId { get; set; } 
    public Dictionary<string, object> formData { get; set; } 
}
