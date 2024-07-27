using System.IO; // Pour les opérations de fichier
using System.Text.Json; // Pour la sérialisation et la désérialisation JSON
using Microsoft.AspNetCore.Mvc; // Pour les fonctionnalités MVC
using MetaForm.Data; // Assurez-vous que l'espace de noms est correct
using MetaForm.Models; // Assurez-vous que l'espace de noms est correct

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

    // Méthode pour lire le fichier JSON et retourner les données sous forme de dictionnaire
    private Dictionary<string, Dictionary<string, object>> ReadJsonFile()
    {
        if (!System.IO.File.Exists(jsonFilePath))
        {
            return new Dictionary<string, Dictionary<string, object>>(); // Si le fichier n'existe pas, retourner un dictionnaire vide
        }

        var jsonData = System.IO.File.ReadAllText(jsonFilePath); // Lire tout le contenu du fichier JSON
        return JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, object>>>(jsonData)!; // Utilisation de l'opérateur "null-forgiving"
    }

    // Méthode pour écrire les données dans le fichier JSON
    private void WriteJsonFile(Dictionary<string, Dictionary<string, object>> data)
    {
        var jsonData = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true }); // Sérialiser les données avec une mise en forme
        System.IO.File.WriteAllText(jsonFilePath, jsonData); // Écrire les données dans le fichier
    }
}
