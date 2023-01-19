using System.Collections.Generic;
using System.Linq;

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
      var (key, value) = dataSheet.Units.ToArray()[1];
      newUnit.UnitLines.Add(key, new UnitLine()
      {
        Count = 1,
        UnitStats = value,
      });
      newActiveUnit.Points += value.Cost;
      return newActiveUnit;
    }

    int getUnitCount(UnitStats line)
    {
      if (unitSize == CreateUnitSize.One) return 1;
      if (unitSize == CreateUnitSize.Min) return line.MinModelsPerUnit;
      if (unitSize == CreateUnitSize.Max) return line.MaxModelsPerUnit;
      return RNG.Integer(line.MinModelsPerUnit, line.MaxModelsPerUnit);
    }

    foreach (var line in dataSheet.Units)
    {
      var modelsPerUnit = getUnitCount(line.Value);
      newUnit.UnitLines.Add(line.Key, new UnitLine()
      {
        Count = modelsPerUnit,
        UnitStats = line.Value,
      });
      newActiveUnit.Points += line.Value.Cost * modelsPerUnit;
    }
    return newActiveUnit;

  }

  public static ActiveUnit ActivateUnit(Unit u, Models model = null)
  {
    return new ActiveUnit()
    {
      BaseUnit = u,
      DeployedCount = u.UnitLines.First().Value.Count,
      Points = Generator.GetUnitPoints(u),
      Model = model,
    };
  }

}