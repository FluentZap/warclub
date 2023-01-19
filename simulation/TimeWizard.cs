using System;
using System.Linq;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace WarClub;

static class TimeWizard
{
  public static void Stasis(Simulation s)
  {
    var options = new JsonSerializerOptions { WriteIndented = true, IncludeFields = true };

    string jsonString = JsonSerializer.Serialize(SonicScrewDrive(s.cosmos), options);
    File.WriteAllText("./ChronoStasis.json", jsonString);

  }



  public static EtherealCosmos SonicScrewDrive(Cosmos c)
  {
    var ec = new EtherealCosmos()
    {
      Day = c.Day,
    };

    ec.Sectors = c.Sectors.Values.Select(x => new EtherealSector()
    {
      Location = x.Location,
      EntityType = x.EntityType,
      Id = x.Id,
      Psyche = x.Psyche,
      Traits = EncodeTraits(x.Traits),
      Name = x.Name,
      Relations = Ghost(x.Relations),
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
      Traits = EncodeTraits(x.Traits),
      Name = x.Name,
      Relations = Ghost(x.Relations),
    }).ToDictionary(x => x.Id);

    ec.Factions = c.Factions.Values.Select(x => new EtherealFaction()
    {
      EntityType = x.EntityType,
      Id = x.Id,
      Psyche = x.Psyche,
      Traits = EncodeTraits(x.Traits),
      Name = x.Name,
      Relations = Ghost(x.Relations),
    }).ToDictionary(x => x.Id);

    return ec;
  }


  public static List<EtherealRelation> Ghost(List<Relation> r)
  {
    return r.Select(x => new EtherealRelation()
    {
      Strength = x.Strength,
      relationType = x.relationType,
      Source = x.Source.Id,
      Target = x.Target.Id,
      Traits = EncodeTraits(x.Traits),
    }).ToList();
  }

  public static Dictionary<string, int> EncodeTraits(Dictionary<Trait, int> t)
  {
    return t.ToDictionary(x => x.Key.Name, x => x.Value);
  }

}
