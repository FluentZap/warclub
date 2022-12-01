using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WarClub;

static class Generator
{

  static int Ratio = 67;
  static Point ScreenSize = new Point(57, 32);


  //build a static rectangle builder for adjusted screen size
  static Rectangle BuildRect(int x, int y, int width, int hight, bool flipX = false)
  {
    return new Rectangle(flipX ? (ScreenSize.X - x - width) * Ratio : x * Ratio, y * Ratio, width * Ratio, hight * Ratio);
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

  static public Mission GenerateBattleMap(Simulation s)
  {
    // if (s.SelectedMission.Name == "war profiteering") return GenerateWarProfiteering(s);
    return GenerateWarProfiteering(s);
    // return null;
  }

  static public List<Unit> SelectUnits(Dictionary<UnitRole, List<Unit>> units, int points, float Troops = 0, float Elites = 0, float HeavySupport = 0, float HQ = 0, float FastAttack = 0, float DedicatedTransport = 0, float Flyers = 0, float LordsOfWar = 0, float Fortifications = 0)
  {
    float ratio = points / (Troops + Elites + HeavySupport + HQ + FastAttack + DedicatedTransport + Flyers + LordsOfWar + Fortifications);
    int HQP = (int)(HQ * ratio);
    int EP = (int)(Elites * ratio);
    int HSP = (int)(HeavySupport * ratio);
    int FAP = (int)(FastAttack * ratio);
    int TP = (int)(Troops * ratio);

    var ranks = new List<Unit>();
    List<Unit> temp;
    int rollOver = 0;

    (temp, rollOver) = FillRanks(units[UnitRole.HQ], HQP + rollOver);
    ranks.AddRange(temp);
    (temp, rollOver) = FillRanks(units[UnitRole.Elites], EP + rollOver);
    ranks.AddRange(temp);
    (temp, rollOver) = FillRanks(units[UnitRole.HeavySupport], HSP + rollOver);
    ranks.AddRange(temp);
    (temp, rollOver) = FillRanks(units[UnitRole.FastAttack], FAP + rollOver);
    ranks.AddRange(temp);
    (temp, rollOver) = FillRanks(units[UnitRole.Troops], TP + rollOver);
    ranks.AddRange(temp);
    (temp, rollOver) = FillRanks(FilterSelected(units[UnitRole.FastAttack], ranks), rollOver);
    ranks.AddRange(temp);
    (temp, rollOver) = FillRanks(FilterSelected(units[UnitRole.HeavySupport], ranks), rollOver);
    ranks.AddRange(temp);
    (temp, rollOver) = FillRanks(FilterSelected(units[UnitRole.Elites], ranks), rollOver);
    ranks.AddRange(temp);
    (temp, rollOver) = FillRanks(FilterSelected(units[UnitRole.HQ], ranks), rollOver);
    ranks.AddRange(temp);

    return ranks;
  }

  static public List<Unit> FilterSelected(List<Unit> units, List<Unit> excluded)
  {
    return units.Where(x => !excluded.Contains(x)).ToList();
  }

  static public (List<Unit>, int) FillRanks(List<Unit> units, int points)
  {
    int unused = points;
    var rank = new List<Unit>();
    List<Unit> filteredUnits;
    do
    {
      filteredUnits = FilterByPoints(FilterSelected(units, rank), unused);
      if (filteredUnits.Count > 0)
      {
        var selectedUnit = RNG.PickFrom(filteredUnits);
        unused -= GetUnitPoints(selectedUnit);
        rank.Add(selectedUnit);
      }
    } while (filteredUnits.Count > 0);
    return (rank, unused);
  }

  static public int GetUnitPoints(Unit unit)
  {
    int total = 0;
    foreach (var line in unit.UnitLines.Values)
    {
      total += line.Count * line.UnitStats.Cost;
    }
    return total;
  }

  static public List<Unit> FilterByPoints(List<Unit> units, int max)
  {
    return units.Where(x => GetUnitPoints(x) <= max).ToList();
  }


  static public Mission GenerateWarProfiteering(Simulation s)
  {
    // default to fortress forest map type
    var mission = new Mission();
    var faction = s.cosmos.Factions[RNG.PickFrom(s.SelectedWorld.Relations.Where(x => x.Source.EntityType == EntityType.Faction).ToList()).Source.Id];

    var fortOnLeft = RNG.Boolean();
    mission.Tiles.Item1 = RNG.PickFrom(GetTileList(s.MapTiles, MapTileTerrain.Forest, MapTileOrientation.Left, fortOnLeft ? MapTileType.Fortress : MapTileType.Obstacles));
    mission.Tiles.Item2 = RNG.PickFrom(GetTileList(s.MapTiles, MapTileTerrain.Forest, MapTileOrientation.Right, fortOnLeft ? MapTileType.Obstacles : MapTileType.Fortress));
    mission.PlayerDeploymentZones.Add(BuildRect(0, 0, 6, ScreenSize.Y, fortOnLeft));
    // mission.PlayerDeploymentZones.Add(new Rectangle(0, 0, 3840, 128));
    // mission.PlayerDeploymentZones.Add(new Rectangle(0, 2160 - 128, 3840, 128));

    var turn0 = new MissionEvent() { Turn = 0 };
    turn0.Spawns.Add(new MissionEventSpawn()
    {
      Location = BuildPoint(14, 16, !fortOnLeft),
      Units = SelectUnits(faction.Units, 250, Troops: 10, HQ: 1, FastAttack: 3, HeavySupport: 5, Elites: 3),
    });
    mission.MissionEvents.Add(turn0);

    return mission;
  }

}