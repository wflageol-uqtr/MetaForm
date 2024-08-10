using MetaForm.Data;
using MetaForm.models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class FormController : ControllerBase
{
    private readonly ListService _listService;

    // Constructeur injectant le ListService pour accéder aux listes
    public FormController(ListService listService)
    {
        _listService = listService;
    }

    // Endpoint pour sauvegarder un enregistrement
    [HttpPost("saveRecord")]
    public async Task<IActionResult> SaveRecord([FromBody] FormData data)
    {
        // Afficher les données reçues pour le débogage
        Console.WriteLine("Données reçues pour saveRecord:");
        Console.WriteLine($"ListId: {data.ListId}, RecordId: {data.RecordId}");
        foreach (var kvp in data.Formdata)
        {
            Console.WriteLine($"{kvp.Key}: {kvp.Value}");
        }

        // Récupérer la liste correspondant à ListId
        var list = await _listService.GetListAsync(data.ListId);
        if (list == null)
        {
            return NotFound(new { Message = "List not found" });
        }

        // Vérifier si l'enregistrement existe déjà
        var existingItem = list.Items.FirstOrDefault(item => item.Id == data.RecordId);
        if (existingItem != null)
        {
            // Mettre à jour l'enregistrement existant
            existingItem.Values = data.Formdata.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.ToString());
            await _listService.UpdateListAsync(list);
        }
        else
        {
            // Créer un nouvel enregistrement
            var newItem = new ListItem
            {
                Id = list.Items.Any() ? list.Items.Max(item => item.Id) + 1 : 1,
                Values = data.Formdata.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.ToString())
            };
            list.Items.Add(newItem);
            await _listService.UpdateListAsync(list);
        }

        // Retourner un message de succès
        return Ok(new { Message = "Record saved successfully" });
    }

    // Endpoint pour récupérer un enregistrement
    [HttpGet("getRecord/{listId}/{recordId}")]
    public async Task<IActionResult> GetRecord(int listId, int recordId)
    {
        // Récupérer la liste correspondant à ListId
        var list = await _listService.GetListAsync(listId);
        if (list == null)
        {
            return NotFound(new { Message = "List not found" });
        }

        // Récupérer l'enregistrement correspondant à RecordId
        var existingItem = list.Items.FirstOrDefault(item => item.Id == recordId);
        if (existingItem != null)
        {
            return Ok(existingItem.Values);
        }
        return NotFound(new { Message = "Record not found" });
    }

    // Endpoint pour récupérer les options d'une liste
    [HttpGet("getOptions/{listId}")]
    public async Task<IActionResult> GetOptions(int listId)
    {
        // Récupérer la liste correspondant à ListId
        var list = await _listService.GetListAsync(listId);
        if (list == null)
        {
            return NotFound(new { Message = "List not found" });
        }

        // Sélectionner les valeurs des items de la liste
        var options = list.Items.Select(item => item.Values).ToList();
        return Ok(options);
    }
}

// Classe pour représenter les données de formulaire
public class FormData
{
    public int ListId { get; set; } // ID de la liste
    public int RecordId { get; set; } // ID de l'enregistrement
    public Dictionary<string, object> Formdata { get; set; } // Dictionnaire contenant les données du formulaire
}
