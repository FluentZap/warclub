using System.Text.Json;
using System.IO;

namespace WarClub;

static class TimeWizard
{
  public static void Stasis(Cosmos c)
  {
    var options = new JsonSerializerOptions { WriteIndented = true, IncludeFields = true };
    string jsonString = JsonSerializer.Serialize(c, options);
    File.WriteAllText("./ChronoStasis.json", jsonString);

  }
}