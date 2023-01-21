using System.Linq;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System;

namespace WarClub;

static class TimeWizard
{
  public static void Stasis(Simulation s)
  {
    var options = new JsonSerializerOptions { WriteIndented = true, IncludeFields = true, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull };
    string jsonString = JsonSerializer.Serialize(Banish(s.cosmos), options);
    File.WriteAllText("./ChronoStasis.json", jsonString);
  }

  public static void Awaken(Simulation s)
  {
    var options = new JsonSerializerOptions { WriteIndented = true, IncludeFields = true, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull };
    string jsonString = File.ReadAllText("./ChronoStasis.json");
    var ec = JsonSerializer.Deserialize<EtherealCosmos>(jsonString, options);
    Summon(s, ec);
  }

  public static EtherealCosmos Banish(Cosmos c)
  {
    var ec = new EtherealCosmos();

    ec.Day = c.Day;
    ec.PlayerFaction = c.PlayerFaction.Id;

    ec.Sectors = c.Sectors.Values.Select(x => new EtherealSector()
    {
      Location = x.Location,
      EntityType = x.EntityType,
      Id = x.Id,
      Psyche = x.Psyche,
      Traits = GhostTraits(x.Traits),
      Name = x.Name,
      Relations = GhostRelations(x),
    }).ToDictionary(x => x.Id);

    ec.Worlds = c.Worlds.Values.Select(x => new EtherealWorld()
    {
      sector = x.sector.Id,
      location = x.location,
      rotationSpeed = x.rotationSpeed,
      rotation = x.rotation,
      DayLength = x.DayLength,
      YearLength = x.YearLength,
      color = x.color,
      size = x.size,
      EntityType = x.EntityType,
      Id = x.Id,
      Psyche = x.Psyche,
      Traits = GhostTraits(x.Traits),
      Name = x.Name,
      Relations = GhostRelations(x),
    }).ToDictionary(x => x.Id);

    ec.Factions = c.Factions.Values.Select(x => new EtherealFaction()
    {
      EntityType = x.EntityType,
      Id = x.Id,
      Psyche = x.Psyche,
      Traits = GhostTraits(x.Traits),
      Name = x.Name,
      Relations = GhostRelations(x),
      Units = GhostUnits(x.Units),
    }).ToDictionary(x => x.Id);

    return ec;
  }

  public static void Summon(Simulation s, EtherealCosmos ec)
  {
    var c = s.cosmos = new Cosmos();

    c.Day = ec.Day;

    c.Sectors = new KeyCollection<Sector>(ec.Sectors.Values.Select(x => new Sector()
    {
      Location = x.Location,
      EntityType = x.EntityType,
      Id = x.Id,
      Psyche = x.Psyche,
      Traits = ReviveTraits(s, x.Traits),
      Name = x.Name,
    }));

    c.Worlds = new KeyCollection<World>(ec.Worlds.Values.Select(x => new World()
    {
      sector = c.Sectors[x.sector],
      location = x.location,
      rotationSpeed = x.rotationSpeed,
      rotation = x.rotation,
      DayLength = x.DayLength,
      YearLength = x.YearLength,
      color = x.color,
      size = x.size,
      EntityType = x.EntityType,
      Id = x.Id,
      Psyche = x.Psyche,
      Traits = ReviveTraits(s, x.Traits),
      Name = x.Name,
    }));

    c.Factions = new KeyCollection<Faction>(ec.Factions.Values.Select(x => new Faction()
    {
      EntityType = x.EntityType,
      Id = x.Id,
      Psyche = x.Psyche,
      Traits = ReviveTraits(s, x.Traits),
      Name = x.Name,
      Units = GhostUnits(x.Units),
    }));

    // rebuild relations
    foreach (var entity in c.Sectors) entity.Value.Relations.AddRange(ReviveRelations(s, ec.Sectors[entity.Key].Relations));
    foreach (var entity in c.Worlds) entity.Value.Relations.AddRange(ReviveRelations(s, ec.Worlds[entity.Key].Relations));
    foreach (var entity in c.Factions) entity.Value.Relations.AddRange(ReviveRelations(s, ec.Factions[entity.Key].Relations));

    c.PlayerFaction = c.Factions[ec.PlayerFaction];
  }

  // Ghost Utils
  public static List<EtherealRelation> GhostRelations(Entity e)
  {
    return e.Relations.Where(x => x.Source == e).Select(x => new EtherealRelation()
    {
      Strength = x.Strength,
      relationType = x.relationType,
      Source = $"{x.Source.EntityType.ToString()}_{x.Source.Id}",
      Target = $"{x.Target.EntityType.ToString()}_{x.Target.Id}",
      Traits = GhostTraits(x.Traits),
    }).ToList();
  }


  public static Dictionary<string, int> GhostTraits(Dictionary<Trait, int> t)
  {
    return t.ToDictionary(x => x.Key.Name, x => x.Value);
  }

  public static Dictionary<UnitRole, List<EtherealUnit>> GhostUnits(Dictionary<UnitRole, List<Unit>> u)
  {
    var newUnits = new Dictionary<UnitRole, List<EtherealUnit>>();
    foreach (var (role, units) in u)
    {
      if (units.Count == 0) continue;
      if (!newUnits.ContainsKey(role)) newUnits.Add(role, new List<EtherealUnit>());
      newUnits[role] = units.Select(x => new EtherealUnit()
      {
        DataSheet = x.DataSheet.Id,
        UnitLines = x.UnitLines.Select(x =>
        {
          var newLine = new EtherealUnitLine() { Count = x.Value.Count, LineId = x.Value.UnitStats.LineId };
          var warGear = x.Value.Wargear.Select(x => x.Id).ToList();
          if (warGear.Count > 0) newLine.Wargear = warGear;
          return newLine;
        }).ToList()
      }).ToList();
    }
    return newUnits;
  }


  // Revive Utils
  public static Dictionary<Trait, int> ReviveTraits(Simulation s, Dictionary<string, int> t)
  {
    return t.ToDictionary(x => s.Traits[x.Key], x => x.Value);
  }

  public static List<Relation> ReviveRelations(Simulation s, List<EtherealRelation> er)
  {
    var relations = er.Select(x => new Relation()
    {
      Strength = x.Strength,
      relationType = x.relationType,
      Source = GetRelation(s, x.Source),
      Target = GetRelation(s, x.Target),
      Traits = ReviveTraits(s, x.Traits),
    }).ToList();

    // add all relations to targets
    foreach (var relation in relations) relation.Target.Relations.Add(relation);

    return relations;
  }

  static Entity GetRelation(Simulation s, string relationKey)
  {
    var keyArray = relationKey.Split("_");
    var type = keyArray[0];
    var id = UInt32.Parse(keyArray[1]);

    switch (type)
    {
      default:
      case "Sector": return s.cosmos.Sectors[id];
      case "World": return s.cosmos.Worlds[id];
      case "Faction": return s.cosmos.Factions[id];
    }
  }

  public static Dictionary<UnitRole, List<Unit>> ReviveUnits(Simulation s, Dictionary<UnitRole, List<EtherealUnit>> u)
  {
    var newUnits = new Dictionary<UnitRole, List<Unit>>();
    foreach (var (role, units) in u)
    {
      if (!newUnits.ContainsKey(role)) newUnits.Add(role, new List<Unit>());
      newUnits[role] = units.Select(x => new Unit()
      {
        DataSheet = s.DataSheets[x.DataSheet],
        UnitLines = x.UnitLines.Select(line =>
        {
          var newLine = new UnitLine() { Count = line.Count, UnitStats = s.DataSheets[x.DataSheet].Units[line.LineId - 1] };
          var warGear = line.Value.Wargear.Select(x => x.Id).ToList();
          if (warGear.Count > 0) newLine.Wargear = warGear;
          return newLine;
        }).ToList()
      }).ToList();
    }
    return newUnits;
  }

}
