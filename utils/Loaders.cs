using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;

static class Loader
{
  public static List<string[]> CSVLoadFile(string path)
  {
    return File.ReadAllLines(path).Select(e => e.Split('|')).ToList();
  }

  public static T JSONLoadFile<T>(string path)
  {
    string jsonString = File.ReadAllText(path);
    T json = JsonSerializer.Deserialize<T>(jsonString);
    return json;
  }
}
