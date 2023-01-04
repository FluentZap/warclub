using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace WarClub;

enum MissionSpawnType
{
  // Show on the map until the next turn
  DeploymentZone,
  // Show on the map until the next turn
  EnemySpawn,
  // Shows on map until end of game
  EvacZone,
  // Shows on map until it's interacted with
  LootBox,
}

class MissionEvent
{
  public int Turn;
  public string Message;
  public List<MissionEventSpawn> Spawns = new List<MissionEventSpawn>();
}

class MissionEventSpawn
{
  public MissionSpawnType Type;
  public Icon ZonesIcon;
  public List<Rectangle> Zones = new List<Rectangle>();
  public List<Unit> Units = new List<Unit>();
  public int LootCredits;
  public List<int> LootWargear = new List<int>();
  public List<int> LootUnit = new List<int>();
}

class Mission
{
  public (MapTile, MapTile) Tiles;
  public List<MissionEvent> MissionEvents = new List<MissionEvent>();
  public List<ActiveUnit> PlayerUnits = new List<ActiveUnit>();
  public int PointCapacity;
}
