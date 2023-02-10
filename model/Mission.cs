using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace WarClub;

enum MissionEventType
{
  // Show on the map until the next turn
  PCDeploymentZone,
  // Show on the map until the next turn
  AISpawn,
  // Unit action
  AIAction,
  // Shows on map until end of game
  PCEvacZone,
  // Shows on map until it's interacted with
  LootBox,
}

class MissionEvent
{
  public int Turn;
  public int Frequency;
  public bool InteractionEvent;
  public Phases Phase;
  public MissionEventType Type;
  public MissionMessage Message;
  public Icon Icon;
  public List<Rectangle> Zones = new List<Rectangle>();
  public ActiveUnit Unit;
  public int LootCredits;
  public List<int> LootUnit = new List<int>();
  public List<MissionEvent> TriggeredEvents = new List<MissionEvent>();
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
  // public Models Model;
  public ActiveUnit Unit;
}
