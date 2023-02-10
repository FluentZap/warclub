using System.Collections.Generic;
using System.Linq;
using System;

namespace WarClub;

enum CreateUnitSize
{
  One,
  Min,
  Max,
  Random,
}

static class UnitUtils
{

  public static List<ActiveUnit> GetRoster(Simulation s, List<Models> models)
  {
    var roster = new List<ActiveUnit>();
    foreach (var model in models)
      foreach (var id in model.DataCardIds)
        roster.Add(CreateUnit(s.DataSheets[id], CreateUnitSize.One));

    // Only show units that are under 50 points to start
    roster = roster.Where(x => x.Points <= 50).ToList();
    return roster;
  }

  public static ActiveUnit CreateUnit(DataSheet dataSheet, CreateUnitSize unitSize)
  {
    var newActiveUnit = new ActiveUnit();
    var newUnit = new Unit();
    newActiveUnit.BaseUnit = newUnit;
    newUnit.DataSheet = dataSheet;

    if (unitSize == CreateUnitSize.One && dataSheet.Units.Count != 1)
    {
      var alpha = dataSheet.Units.ToArray()[1];
      newUnit.UnitLines.Add(alpha.Key, 1);
      newActiveUnit.Points += alpha.Value.Cost;
      return newActiveUnit;
    }

    int getUnitCount(UnitStats line)
    {
      if (unitSize == CreateUnitSize.One) return 1;
      if (unitSize == CreateUnitSize.Min) return line.MinModelsPerUnit;
      if (unitSize == CreateUnitSize.Max) return line.MaxModelsPerUnit;
      return RNG.Integer(line.MinModelsPerUnit, line.MaxModelsPerUnit);
    }

    foreach (var line in dataSheet.Units.Values)
    {
      var modelsPerUnit = getUnitCount(line);
      newUnit.UnitLines.Add(line.LineId, modelsPerUnit);
      newActiveUnit.Points += line.Cost * modelsPerUnit;
    }
    return newActiveUnit;

  }

  public static ActiveUnit ActivateUnit(Unit u, Models model = null)
  {
    return new ActiveUnit()
    {
      BaseUnit = u,
      DeployedCount = u.UnitLines.First().Value,
      Points = Generator.GetUnitPoints(u),
      Model = model,
      Job = ClassifyUnit(u),
    };
  }

  public static List<ActiveUnit> ActivateUnits(List<Unit> units, List<Models> models = null)
  {
    Models PickUnit(Unit unit)
    {
      var unitSize = unit.DataSheet.Units[1].Size.X;
      var extraUnit = unit.UnitLines.Count > 1 ? 1 : 0;
      var quantityModels = models.Where(x => x.Count >= unit.UnitLines[1] + extraUnit).ToList();
      var sizeModels = quantityModels.Where(x => x.Size.X * 1.25 > unitSize && x.Size.X * 0.75 < unitSize).ToList();

      var model = RNG.PickFrom(sizeModels.Count > 0 ? sizeModels : quantityModels);
      models.Remove(model);
      return model;
    }

    return units.Select(x => ActivateUnit(x, models != null ? PickUnit(x) : null)).ToList();
  }


  public static UnitArchetypeJob ClassifyUnit(Unit unit)
  {
    var stats = GenerateUnitArchetypeStats(unit);
    return GenerateArchetypeFromStats(stats);
  }

  public static UnitArchetypeStats GenerateUnitArchetypeStats(Unit unit)
  {
    var stats = new UnitArchetypeStats();

    // digit with " mark example: 10"
    // digit with - another digit and " marks example 10-20"
    // digit with + and d6 example: 6+D6
    // dash mark only Exaample: -

    var unitStats = unit.DataSheet.Units[1];

    stats.Quick = unitStats.Movement != "-" && Int32.Parse(unitStats.Movement.Split('-')[0].Split('"')[0].Split('+')[0]) >= 10;
    stats.Cheap = unitStats.Cost <= 15;
    stats.Expensive = unitStats.Cost >= 100;

    // high damage we are going to need to make calculation of all the different damage score modifyers
    // and then calculate that bassed on DPS per cost point
    // stats.HighDamage = unitStats.Movement != "-" && Int32.Parse(unitStats.Movement.Split('-')[0].Split('"')[0].Split('+')[0]) >= 10;

    var maxRangeWargear = unit.DataSheet.DefaultWargear.Count > 0 ? unit.DataSheet.DefaultWargear.Select(x =>
    {
      var range = x.WargearLine.First().Value.Range;
      if (range == "Melee") return 0;
      return Int32.Parse(range.Split('"')[0].Split('-')[0]);
    }).MaxBy(x => x) : 0;

    stats.Melee = unit.DataSheet.DefaultWargear.Find(x => x.Archetype == "Melee Weapons") != null;
    stats.LongRange = maxRangeWargear >= 30;
    // stats.Melee = unitStats.Movement != "-" && Int32.Parse(unitStats.Movement.Split('-')[0].Split('"')[0].Split('+')[0]) >= 10;
    // stats.Tough = unitStats.Movement != "-" && Int32.Parse(unitStats.Movement.Split('-')[0].Split('"')[0].Split('+')[0]) >= 10;

    return stats;
  }


  public static UnitArchetypeJob GenerateArchetypeFromStats(UnitArchetypeStats stats)
  {
    var personas = new HashSet<UnitArchetypeJob>();

    if (stats.LongRange) personas.Add(UnitArchetypeJob.Eliminator);
    if (stats.Cheap) personas.Add(UnitArchetypeJob.Attrition);
    if (stats.Melee) personas.Add(UnitArchetypeJob.Brawler);

    if (personas.Count == 0) return UnitArchetypeJob.Attrition;
    return RNG.PickFrom(personas);
  }




  public static bool CalculateHighDamage(Unit unit)
  {
    // skill, number of dice, strength, armor periecing, dmg
    var meleeScore = 0;
    int meleeSkill;
    if (Int32.TryParse(unit.DataSheet.Units[1].A.Split('+')[0], out meleeSkill))
      meleeScore = Math.Clamp(meleeSkill, 0, 10);
    return false;
  }

}