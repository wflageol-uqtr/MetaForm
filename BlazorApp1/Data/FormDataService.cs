public class FormDataService
{
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
}
