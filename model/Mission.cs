using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace WarClub;

enum MissionSpawnType
{
  // Show on the map until the next turn
  PCDeploymentZone,
  // Show on the map until the next turn
  AISpawn,
  // Shows on map until end of game
  PCEvacZone,
  // Shows on map until it's interacted with
  LootBox,
}

class MissionEvent
{
  public int Turn;
  public MissionSpawnType Type;
  public MissionMessage Message;
  public Icon Icon;
  public List<Rectangle> Zones = new List<Rectangle>();
  public ActiveUnit Unit;
  public int LootCredits;
  public List<int> LootUnit = new List<int>();
}

class Mission
{
  public (MapTile, MapTile) Tiles;
  public List<MissionEvent> MissionEvents = new List<MissionEvent>();
  public int PointCapacity;
}

class MissionMessage
{
  public string Text;
  public Color Color;
  // public Icon Icon;
  public Models Model;
}
