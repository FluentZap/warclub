using System.Collections.Generic;
using System.Linq;

namespace WarClub;

enum CreateUnitSize
{
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
        roster.Add(CreateUnit(s.DataSheets[id], CreateUnitSize.Min));

    return roster;
  }


  public static ActiveUnit CreateUnit(DataSheet dataSheet, CreateUnitSize unitSize)
  {
    var newActiveUnit = new ActiveUnit();
    var newUnit = new Unit();
    newActiveUnit.BaseUnit = newUnit;
    newUnit.DataSheet = dataSheet;

    foreach (var line in dataSheet.Units)
    {
      var modelsPerUnit = unitSize == CreateUnitSize.Min ? line.Value.MinModelsPerUnit : unitSize == CreateUnitSize.Max ? line.Value.MaxModelsPerUnit : RNG.Integer(line.Value.MinModelsPerUnit, line.Value.MaxModelsPerUnit);
      var unitLine = new UnitLine()
      {
        Count = modelsPerUnit,
        UnitStats = line.Value,
      };
      newUnit.UnitLines.Add(line.Key, unitLine);
      newActiveUnit.Points += line.Value.Cost * modelsPerUnit;
    }
    return newActiveUnit;
  }



  public static ActiveUnit ActivateUnit(Unit u)
  {
    return new ActiveUnit()
    {
      BaseUnit = u,
      DeployedCount = u.UnitLines.First().Value.Count,
      Points = Generator.GetUnitPoints(u),
    };

  }


}