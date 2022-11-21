using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace WarClub;

static class FileLoading
{
  public static Dictionary<string, int> ParseStrengthList(string list, int baseValue = 1)
  {
    Dictionary<string, int> sl = new Dictionary<string, int>();
    foreach (string item in list.Trim().Split(','))
    {
      if (item == "") continue;
      if (item.Contains('_'))
      {
        string[] values = item.Split('_');
        string name = values[1].Trim();
        int strength = int.Parse(values[0]);
        if (!sl.ContainsKey(values[1]))
        {
          sl.Add(name, strength);
        }
        else
        {
          sl[name] += strength;
        }
      }
      else
      {
        var returnItem = item.Trim();
        var value = baseValue;
        if (returnItem.Contains('-'))
        {
          returnItem = returnItem.Substring(1);
          value = -value;
        }
        sl.Add(returnItem, value);
      }
    }
    return sl;
  }
}

partial class Simulation
{
  void LoadLists()
  {
    var traitList = Loader.CSVLoadFile(Path.Combine("./", "Traits.csv"));
    traitList.RemoveAt(0);
    foreach (string[] r in traitList)
    {
      if (r.Length < 3 || r[0][0] == '*')
        continue;

      var types = r[1].Split(',').Select(x => x.Trim()).ToList();
      types.ForEach(x => TraitLists.TryAdd(x, new TraitList()));

      Dictionary<string, int> modifiers = FileLoading.ParseStrengthList(r[2]);

      Trait newTrait = new Trait()
      {
        Name = r[0].Trim(),
        Type = types,
        Aspects = modifiers,
      };

      Traits.Add(r[0].Trim(), newTrait);
      types.ForEach(x => TraitLists[x].Add(newTrait.Name, newTrait));
    }

    // add default trait lists
    TraitLists.Add("Species", new TraitList());

    // Add Order Events
    var orderEventList = Loader.CSVLoadFile(Path.Combine("./", "OrderEvents.csv"));
    orderEventList.RemoveAt(0);
    foreach (string[] r in orderEventList)
    {
      if (r.Length < 8 || r[0][0] == '*')
        continue;

      var allowedEntityType = r[1].Split(',').Select(x => x.Trim()).ToList();
      allowedEntityType.ForEach(x => { if (x != "") OrderEventLists.TryAdd(x, new Dictionary<string, OrderEvent>()); });

      var newEvent = new OrderEvent()
      {
        Name = r[0].Trim(),
        AllowedEntityType = allowedEntityType,
        RequiredTraits = FileLoading.ParseStrengthList(r[2]).ToDictionary(x => Traits[x.Key], x => x.Value),
        RequiredAspects = FileLoading.ParseStrengthList(r[3]),
        Psyche = new Psyche()
        {
          Openness = byte.Parse(r[4]),
          Conscientiousness = byte.Parse(r[5]),
          Extroversion = byte.Parse(r[6]),
          Agreeableness = byte.Parse(r[7]),
          Neuroticism = byte.Parse(r[8]),
        }
      };
      OrderEvents.Add(newEvent.Name, newEvent);
      allowedEntityType.ForEach(x => OrderEventLists[x].Add(newEvent.Name, newEvent));
    }


    var chaosEventList = Loader.CSVLoadFile(Path.Combine("./", "ChaosEvents.csv"));
    chaosEventList.RemoveAt(0);

    ChaosEvent lastEvent = null;

    foreach (string[] r in chaosEventList)
    {
      if (r.Length < 11 || r[0][0] == '*')
        continue;

      var allowedEntityType = r[1].Split(',').Select(x => x.Trim()).ToList();
      allowedEntityType.ForEach(x => { if (x != "") ChaosEventLists.TryAdd(x, new Dictionary<string, ChaosEvent>()); });
      if (r[0][0] == '_')
      {
        var newAction = new EventAction()
        {
          Name = r[0].Trim().Substring(1),
          RequiredTraits = FileLoading.ParseStrengthList(r[2]).ToDictionary(x => Traits[x.Key], x => x.Value),
          RequiredAspects = FileLoading.ParseStrengthList(r[3]),
        };

        byte.TryParse(r[6], out byte Openness);
        byte.TryParse(r[7], out byte Conscientiousness);
        byte.TryParse(r[8], out byte Extroversion);
        byte.TryParse(r[9], out byte Agreeableness);
        byte.TryParse(r[10], out byte Neuroticism);

        newAction.Psyche = new Psyche()
        {
          Openness = byte.Parse(r[6]),
          Conscientiousness = byte.Parse(r[7]),
          Extroversion = byte.Parse(r[8]),
          Agreeableness = byte.Parse(r[9]),
          Neuroticism = byte.Parse(r[10]),
        };

        if (lastEvent != null) lastEvent.EventActions.Add(newAction);
      }
      else
      {
        var newEvent = new ChaosEvent()
        {
          Name = r[0].Trim(),
          AllowedEntityType = allowedEntityType,
          RequiredTraits = FileLoading.ParseStrengthList(r[2]).ToDictionary(x => Traits[x.Key], x => x.Value),
          RequiredAspects = FileLoading.ParseStrengthList(r[3]),
          InfluentialTraits = FileLoading.ParseStrengthList(r[4]).ToDictionary(x => Traits[x.Key], x => x.Value),
          InfluentialAspects = FileLoading.ParseStrengthList(r[5]),
        };
        ChaosEvents.Add(newEvent.Name, newEvent);
        allowedEntityType.ForEach(x => ChaosEventLists[x].Add(newEvent.Name, newEvent));
        lastEvent = newEvent;
      }
    }
  }

  void LoadUnits()
  {
    var rows = Loader.CSVLoadFile(Path.Combine("./", "Units.csv"));
    rows.RemoveAt(0);
    foreach (string[] r in rows)
    {
      if (r.Length < 5 || r[0][0] == '*')
        continue;


      string size = r[3];
      Point unitSize = new Point();
      if (size.Contains("x"))
      {
        string[] sizeString = size.Split('x');
        unitSize.X = Int32.Parse(sizeString[0]);
        unitSize.Y = Int32.Parse(sizeString[1]);
      }
      else
      {
        unitSize.X = unitSize.Y = Int32.Parse(size);
      }

      UnitList.Add(new Models()
      {
        Name = r[0].Trim(),
        Image = r[1].Trim(),
        Count = Int32.Parse(r[2]),
        Size = unitSize,
        Types = r[4].Split(',').Select(x => x.Trim()).ToHashSet(),
      });
    }
  }

  void LoadDataSheets()
  {
    // Load Datasheet csv
    var rows = Loader.CSVLoadFile(Path.Combine("./WarhammerData", "Datasheets.csv"));
    rows.RemoveAt(0);
    foreach (string[] r in rows)
    {
      // if (r.Length < 5 || r[0][0] == '*')
      //   continue;
      DataSheets.Add(Int32.Parse(r[0]), new DataSheet()
      {
        Id = Int32.Parse(r[0]),
        Name = r[1].Trim(),
        FactionId = r[3].Trim(),
        Role = Enum.Parse<UnitRole>(r[5].Trim().Replace(" ", ""), true),
        UnitComposition = r[6].Trim(),
      });
    }

    // Load model data and add to Datasheet
    rows = Loader.CSVLoadFile(Path.Combine("./WarhammerData", "Datasheets_models.csv"));
    rows.RemoveAt(0);
    foreach (string[] r in rows)
    {
      var id = Int32.Parse(r[0]);
      var units = DataSheets[id].Units;

      var name = r[2].Trim();
      units.Remove(name);

      var u = new UnitStats();

      u.Movement = r[3].Trim();
      u.WS = r[4].Trim();
      u.BS = r[5].Trim();
      u.S = r[6].Trim();
      u.T = r[7].Trim();
      u.W = r[8].Trim();
      u.A = r[9].Trim();
      u.Ld = r[10].Trim();
      u.Sv = r[11].Trim();
      u.Cost = r[12].Trim() != "" ? Int32.Parse(r[12]) : -1;

      if (u.Cost <= 0) continue;

      DataSheets[id].Units.Add(name, u);
      // Set unit sizes higher or lower
      string[] unitSizes = r[14].Split('-');
      if (unitSizes[0].Trim() != "")
      {
        u.MinModelsPerUnit = Int32.Parse(unitSizes[0]);
        u.MaxModelsPerUnit = unitSizes.Length > 1 ? Int32.Parse(unitSizes[1]) : u.MinModelsPerUnit;
      }

      string size = r[16].Trim();
      // Is there a number in the base?
      if (size.Any(char.IsDigit))
      {
        // Has rectangle base
        if (size.Contains("x"))
        {
          string[] sizeString = size.Split('x');
          u.Size.X = Int32.Parse(sizeString[0]);
          u.Size.Y = Int32.Parse(sizeString[1].Split('m')[0]);
        }
        else
        {
          u.Size.X = u.Size.Y = Int32.Parse(size.Split('m')[0]);
        }
      }
    }

    // Load weapons and add to Wargear
    rows = Loader.CSVLoadFile(Path.Combine("./WarhammerData", "Wargear.csv"));
    rows.RemoveAt(0);
    foreach (string[] r in rows)
    {
      if (r[0].Contains('s'))
        continue;

      Wargear.Add(Int32.Parse(r[0]), new Wargear()
      {
        Id = Int32.Parse(r[0]),
        Name = r[1].Trim(),
        Archetype = r[2].Trim(),
        Description = r[3].Trim(),
        Relic = r[5].Trim() == "true",
        FactionId = r[6].Trim(),
        Legend = r[8].Trim(),
      });
    }

    // Load WarGear Lines and add to wargear
    rows = Loader.CSVLoadFile(Path.Combine("./WarhammerData", "Wargear_list.csv"));
    rows.RemoveAt(0);
    foreach (string[] r in rows)
    {
      var wargear = Wargear[Int32.Parse(r[0])];
      wargear.WargearLine.Add(r[2].Trim(), new WargearLine()
      {
        Name = r[2].Trim(),
        Range = r[3].Trim(),
        Type = r[4].Trim(),
        S = r[5].Trim(),
        AP = r[6].Trim(),
        D = r[7].Trim(),
        Abilities = r[8].Trim(),
      });
    }

    // adds wargear to units
    rows = Loader.CSVLoadFile(Path.Combine("./WarhammerData", "Datasheets_wargear.csv"));
    rows.RemoveAt(0);
    foreach (string[] r in rows)
    {
      if (r[2].Contains('s'))
        continue;

      var dataSheet = DataSheets[Int32.Parse(r[0])];
      var wargear = Wargear[Int32.Parse(r[2])];
      dataSheet.Wargear.Add(wargear);
    }
    string pattern = @"\(.*\)";

    // build default gear with best guess
    foreach (DataSheet dataSheet in DataSheets.Values)
    {
      var normalizedUnitComposition = dataSheet.UnitComposition.ToLower();
      foreach (var warGear in dataSheet.Wargear)
      {
        var name = warGear.Name.ToLower();
        name = Regex.Replace(name, pattern, String.Empty).Trim();

        if (normalizedUnitComposition.Contains(name))
          dataSheet.DefaultWargear.Add(warGear);
      }

      if (dataSheet.DefaultWargear.Count == 0)
        dataSheet.DefaultWargear.AddRange(dataSheet.Wargear);
    }

    // add wargear options
    rows = Loader.CSVLoadFile(Path.Combine("./WarhammerData", "Datasheets_options.csv"));
    rows.RemoveAt(0);
    foreach (string[] r in rows)
      DataSheets[Int32.Parse(r[0])].WargearOptions.Add(r[3].Trim());

    // add keywords and Faction Key words
    rows = Loader.CSVLoadFile(Path.Combine("./WarhammerData", "Datasheets_keywords.csv"));
    rows.RemoveAt(0);
    foreach (string[] r in rows)
      if (r[3] == "true")
        DataSheets[Int32.Parse(r[0])].FactionKeywords.Add(r[1].Trim());
      else
        DataSheets[Int32.Parse(r[0])].Keywords.Add(r[1].Trim());

    // add stratagems
    rows = Loader.CSVLoadFile(Path.Combine("./WarhammerData", "Stratagems.csv"));
    rows.RemoveAt(0);
    foreach (string[] r in rows)
    {
      Stratagems.Add(Int32.Parse(r[7]), new Stratagem()
      {
        Id = Int32.Parse(r[7]),
        Name = r[1].Trim(),
        Type = r[2].Trim(),
        Cost = r[3].Trim(),
        Legend = r[4].Trim(),
        Description = r[6].Trim(),
      });
    }

    // add stratagems to datasheet
    rows = Loader.CSVLoadFile(Path.Combine("./WarhammerData", "Datasheets_stratagems.csv"));
    rows.RemoveAt(0);
    foreach (string[] r in rows)
    {
      var dataSheet = DataSheets[Int32.Parse(r[0])];
      var stratagem = Stratagems[Int32.Parse(r[1])];
      dataSheet.Stratagems.Add(stratagem);
    }
    DataSheets = DataSheets.Where(x => x.Value.Units.Count != 0).ToDictionary(x => x.Key, x => x.Value);
  }

  void LoadMapTiles()
  {
    var rows = Loader.CSVLoadFile(Path.Combine("./", "MapTiles.csv"));
    rows.RemoveAt(0);
    foreach (string[] r in rows)
    {

      MapTiles.Add(new MapTile()
      {
        Texture = r[0].Trim(),
        Terrain = Enum.Parse<MapTileTerrain>(r[1].Trim(), true),
        Orientation = Enum.Parse<MapTileOrientation>(r[2].Trim(), true),
        Type = Enum.Parse<MapTileType>(r[3].Trim(), true),
      });
    }
  }
}
