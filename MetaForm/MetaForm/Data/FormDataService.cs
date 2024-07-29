using MetaForm.models;
using MetaForm.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

public class FormDataService
{
    private readonly string filePath = "data.json";
    private List<List> lists;

    public List<List> Lists
    {
        get => lists;
        set => lists = value ?? new List<List>();
    }

    public FormDataService()
    {
        if (File.Exists(filePath))
        {
            var jsonData = File.ReadAllText(filePath);
            Lists = JsonSerializer.Deserialize<List<List>>(jsonData) ?? new List<List>();
        }
        else
        {
            Lists = new List<List>();
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
        var list = Lists.FirstOrDefault(l => l.Id == listId);
        if (list != null)
        {
            var item = list.Items.FirstOrDefault(i => i.ContainsKey("Id") && i["Id"] == itemId.ToString());
            if (item != null)
            {
                item["FilePath"] = filePath;
                SaveChanges();
            }
        }
        return Task.CompletedTask;
    }

    private void SaveChanges()
    {
        var jsonData = JsonSerializer.Serialize(Lists, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, jsonData);
    }
}
