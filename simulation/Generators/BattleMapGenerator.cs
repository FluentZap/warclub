using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WarClub;

static class Generator
{

  static public List<MapTile> GetTileList(List<MapTile> tiles, MapTileTerrain terrain, MapTileOrientation orientation, MapTileType type)
  {
    return tiles.FindAll(x => x.Type == type && x.Terrain == terrain && (x.Orientation == orientation || x.Orientation == MapTileOrientation.Both)).ToList();
  }

  static public Mission GenerateBattleMap(Simulation s)
  {
    var screenSize = new Point(3840, 2160);
    // default to fortress forest map type
    var mission = new Mission();
    var faction = RNG.PickFrom(s.SelectedWorld.Relations.Where(x => x.Source.EntityType == EntityType.Faction).ToList());
    var fortOnLeft = RNG.Boolean();
    mission.Tiles.Item1 = RNG.PickFrom(GetTileList(s.MapTiles, MapTileTerrain.Forest, MapTileOrientation.Left, fortOnLeft ? MapTileType.Fortress : MapTileType.Obstacles));
    mission.Tiles.Item2 = RNG.PickFrom(GetTileList(s.MapTiles, MapTileTerrain.Forest, MapTileOrientation.Right, fortOnLeft ? MapTileType.Obstacles : MapTileType.Fortress));
    mission.PlayerDeploymentZones.Add(new Rectangle(fortOnLeft ? screenSize.X - 402 : 0, 0, 402, screenSize.Y));
    mission.PlayerDeploymentZones.Add(new Rectangle(0, 0, 3840, 128));
    mission.PlayerDeploymentZones.Add(new Rectangle(0, 2160 - 128, 3840, 128));

    var turn0 = new MissionEvent() { Turn = 0 };
    turn0.Spawns.Add(new MissionEventSpawn()
    {
      Location = new Point(500, 1000),
      Units = SelectUnits(Troops: 10),
    });
    mission.MissionEvents.Add(turn0);



    return mission;
  }

  static public List<Unit> SelectUnits(int Troops = 0, int Elites = 0, int HeavySupport = 0, int HQ = 0, int FastAttack = 0, int DedicatedTransport = 0, int Flyers = 0, int LordsOfWar = 0, int Fortifications = 0)
  {
    var units = new List<Unit>();
    return units;
  }


}