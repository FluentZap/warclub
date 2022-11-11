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

  static public (MapTile, MapTile) GenerateBattleMap(Simulation s)
  {
    // default to fortress forest map type
    var fortOnLeft = RNG.Boolean();
    var leftTile = RNG.PickFrom(GetTileList(s.MapTiles, MapTileTerrain.Forest, MapTileOrientation.Left, fortOnLeft ? MapTileType.Fortress : MapTileType.Obstacles));
    var rightTile = RNG.PickFrom(GetTileList(s.MapTiles, MapTileTerrain.Forest, MapTileOrientation.Right, fortOnLeft ? MapTileType.Obstacles : MapTileType.Fortress));
    return (leftTile, rightTile);
  }

}