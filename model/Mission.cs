using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace WarClub;




class MissionEvent
{
  public int Turn;
  public List<Point> EvacZones = new List<Point>();
  public List<MissionEventSpawn> Intractables = new List<MissionEventSpawn>();
  public List<MissionEventSpawn> Spawns = new List<MissionEventSpawn>();
}

class MissionEventSpawn
{
  public Point Location;
  public List<Unit> Units = new List<Unit>();
  public List<int> Loot = new List<int>();
}

class Mission
{
  public (MapTile, MapTile) Tiles;
  public List<Rectangle> PlayerDeploymentZones = new List<Rectangle>();
  public List<MissionEvent> MissionEvents = new List<MissionEvent>();
}
