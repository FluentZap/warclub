namespace WarClub;

public enum MapTileOrientation
{
  Left,
  Right,
  Both,
}

public enum MapTileTerrain
{
  Dessert,
  Forest,
  Wasteland,
  Snow,
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