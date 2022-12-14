namespace WarClub;

public enum MapTileOrientation
{
  Left,
  Right,
  Both,
}

public enum MapTileTerrain
{
  Desert,
  Forest,
  Wasteland,
  Winter,
  Castle,
  Town,
}

public enum MapTileType
{
  Open,
  Obstacles,
  City,
  Fortress,
  Ritual,
}

class MapTile
{
  public MapTileOrientation Orientation;
  public MapTileTerrain Terrain;
  public MapTileType Type;
  public string Texture;
}