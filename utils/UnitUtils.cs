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
    };
  }

  public static List<ActiveUnit> ActivateUnits(List<Unit> units)
  {
    return units.Select(x => ActivateUnit(x)).ToList();
  }

}