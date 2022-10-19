using System.Text.Json;
using System.IO;

namespace WarClub;

static class TimeWizard
{
  public static void Stasis(Cosmos c)
  {
    var options = new JsonSerializerOptions { WriteIndented = true, IncludeFields = true };
    string jsonString = JsonSerializer.Serialize(c, options);
    // System.Console.WriteLine(jsonString);
    File.WriteAllText("./ChronoStasis.json", jsonString);

  }
}