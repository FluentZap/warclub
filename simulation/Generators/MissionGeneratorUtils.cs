using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WarClub;

static partial class Generator
{

  static int Ratio = 67;
  static Point ScreenSize = new Point(57, 32);


  //build a static rectangle builder for adjusted screen size
  public static Rectangle BuildRect(int x, int y, int width, int height, bool flipX = false, bool flipY = false)
  {
    return new Rectangle(flipX ? (ScreenSize.X - x - width) * Ratio : x * Ratio, flipY ? (ScreenSize.Y - y - height) * Ratio : y * Ratio, width * Ratio, height * Ratio);
  }

  static Point BuildPoint(int x, int y, bool flipX = false)
  {
    return new Point(flipX ? (ScreenSize.X - x) * Ratio : x * Ratio, y * Ratio);
  }

  //build a static point builder for adjusted screen size

  static public List<MapTile> GetTileList(List<MapTile> tiles, MapTileTerrain terrain, MapTileOrientation orientation, MapTileType type)
  {
    return tiles.FindAll(x => x.Type == type && x.Terrain == terrain && (x.Orientation == orientation || x.Orientation == MapTileOrientation.Both)).ToList();
  }

  static public List<MapTile> GetTileList(List<MapTile> tiles, MapTileTerrain terrain, MapTileOrientation orientation)
  {
    return tiles.FindAll(x => x.Terrain == terrain && (x.Orientation == orientation || x.Orientation == MapTileOrientation.Both)).ToList();
  }



  static public List<Unit> FilterSelected(List<Unit> units, List<Unit> excluded)
  {
    return units.Where(x => !excluded.Contains(x)).ToList();
  }

  static public int FillRanks(List<Unit> rawUnits, List<Unit> ignore, List<Unit> chosen, int points)
  {
    int unused = points;
    var units = FilterSelected(rawUnits, ignore);
    List<Unit> filteredUnits;
    do
    {
      filteredUnits = FilterByPoints(FilterSelected(units, chosen), unused);
      if (filteredUnits.Count > 0)
      {
        var selectedUnit = RNG.PickFrom(filteredUnits);
        unused -= GetUnitPoints(selectedUnit);
        chosen.Add(selectedUnit);
        ignore.Add(selectedUnit);
      }
    } while (filteredUnits.Count > 0);
    return unused;
  }

  static public int GetUnitPoints(Unit unit)
  {
    int total = 0;
    foreach (var (lineId, count) in unit.UnitLines)
    {
      total += count * unit.DataSheet.Units[lineId].Cost;
    }
    return total;
  }

  static public List<Unit> FilterByPoints(List<Unit> units, int max)
  {
    return units.Where(x => GetUnitPoints(x) <= max).ToList();
  }

  static public MapTileTerrain GetPlanetTerrain(World w)
  {
    var terrain = RNG.PickFrom(TraitUtil.getTraitsByType(w.GetTraits(), "world terrain"));
    switch (terrain.Key.Name)
    {
      case "grassland terrain":
      case "savannah terrain":
      case "continual forest terrain":
      case "broken forest terrain":
        return MapTileTerrain.Forest;
      case "hills terrain":
      case "mountains terrain":
      case "plateaus terrain":
        return MapTileTerrain.Desert;

      case "dormant volcanoes terrain":
      case "active volcanoes terrain":
      case "broken rock terrain":
        return MapTileTerrain.Town;
      case "flat rock terrain":
      case "columns terrain":
      case "moor terrain":
      case "barren terrain":
        return MapTileTerrain.Wasteland;
      case "swamp terrain":
      case "caves terrain":
      case "ravines terrain":


      case "sandy terrain":
      case "islands terrain":
      case "cliffs terrain":
        return MapTileTerrain.Castle;
      default:
        return MapTileTerrain.Winter;
    }
  }

  static public string GetUnitSpawnLabel(ActiveUnit unit)
  {
    var name = unit.BaseUnit.DataSheet.Name;
    var hasLeader = unit.BaseUnit.DataSheet.Units.Count > 1;
    var totalCount = hasLeader ? unit.DeployedCount + 1 : unit.DeployedCount;
    var size = unit.BaseUnit.DataSheet.Units[unit.BaseUnit.UnitLines.First().Key].Size.X;
    return $"{totalCount} {name} - {size}mm -" + (hasLeader ? " w/leader" : "");
  }

}

class SelectUnitSystem
{

  Dictionary<UnitRole, List<Unit>> Units;
  List<Unit> Ignore = new List<Unit>();

  public SelectUnitSystem(Dictionary<UnitRole, List<Unit>> units)
  {

    Units = units;

  }

  public List<Unit> SelectUnits(int points, float Troops = 0, float Elites = 0, float HeavySupport = 0, float HQ = 0, float FastAttack = 0, float DedicatedTransport = 0, float Flyers = 0, float LordsOfWar = 0, float Fortifications = 0)
  {
    float ratio = points / (Troops + Elites + HeavySupport + HQ + FastAttack + DedicatedTransport + Flyers + LordsOfWar + Fortifications);
    int HQP = (int)(HQ * ratio);
    int EP = (int)(Elites * ratio);
    int HSP = (int)(HeavySupport * ratio);
    int FAP = (int)(FastAttack * ratio);
    int TP = (int)(Troops * ratio);

    var ranks = new List<Unit>();
    int rollOver = 0;

    rollOver = Generator.FillRanks(Units[UnitRole.HQ], Ignore, ranks, HQP + rollOver);
    rollOver = Generator.FillRanks(Units[UnitRole.Elites], Ignore, ranks, EP + rollOver);
    rollOver = Generator.FillRanks(Units[UnitRole.HeavySupport], Ignore, ranks, HSP + rollOver);
    rollOver = Generator.FillRanks(Units[UnitRole.FastAttack], Ignore, ranks, FAP + rollOver);
    rollOver = Generator.FillRanks(Units[UnitRole.Troops], Ignore, ranks, TP + rollOver);
    rollOver = Generator.FillRanks(Units[UnitRole.FastAttack], Ignore, ranks, rollOver);
    rollOver = Generator.FillRanks(Units[UnitRole.HeavySupport], Ignore, ranks, rollOver);
    rollOver = Generator.FillRanks(Units[UnitRole.Elites], Ignore, ranks, rollOver);
    rollOver = Generator.FillRanks(Units[UnitRole.HQ], Ignore, ranks, rollOver);

    return ranks;
  }
}
