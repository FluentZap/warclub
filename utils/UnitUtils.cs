using System.Collections.Generic;

namespace WarClub;

enum CreateUnitSize
{
  Min,
  Max,
  Random,
}

static class UnitUtils
{
  public static List<CalculatedUnit> GetRoster(Simulation s, List<Models> models)
  {
    var roster = new List<CalculatedUnit>();
    foreach (var model in models)
      foreach (var id in model.DataCardIds)
        roster.Add(CreateUnit(s.DataSheets[id], CreateUnitSize.Min));

    return roster;
  }

  public static CalculatedUnit CreateUnit(DataSheet datasheet, CreateUnitSize unitSize)
  {
    var newUnit = new CalculatedUnit();
    newUnit.DataSheet = datasheet;

    foreach (var line in datasheet.Units)
    {
      var modelsPerUnit = unitSize == CreateUnitSize.Min ? line.Value.MinModelsPerUnit : unitSize == CreateUnitSize.Max ? line.Value.MaxModelsPerUnit : RNG.Integer(line.Value.MinModelsPerUnit, line.Value.MaxModelsPerUnit);
      var unitLine = new UnitLine()
      {
        Count = modelsPerUnit,
        UnitStats = line.Value,
      };
      newUnit.UnitLines.Add(line.Key, unitLine);
      newUnit.Points += line.Value.Cost * modelsPerUnit;
    }
    return newUnit;
  }
}