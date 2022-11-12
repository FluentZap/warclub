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
    var fortOnLeft = RNG.Boolean();
    mission.Tiles.Item1 = RNG.PickFrom(GetTileList(s.MapTiles, MapTileTerrain.Forest, MapTileOrientation.Left, fortOnLeft ? MapTileType.Fortress : MapTileType.Obstacles));
    mission.Tiles.Item2 = RNG.PickFrom(GetTileList(s.MapTiles, MapTileTerrain.Forest, MapTileOrientation.Right, fortOnLeft ? MapTileType.Obstacles : MapTileType.Fortress));
    mission.PlayerDeploymentZones.Add(new Rectangle(fortOnLeft ? 0 : screenSize.X - 402, 0, 402, screenSize.Y));
    return mission;
  }



}