using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace WarClub;

class MissionState
{
  public int Turn;
  public int Round;
  public bool ActivationPhase;
  public bool DeploymentPhase = true;
  public bool PlayerTurn;
  public ActiveUnit ActiveUnit;
  public List<MissionMessage> Messages = new List<MissionMessage>();
  public List<MissionZone> TempZones = new List<MissionZone>();
  public List<MissionZone> PermZones = new List<MissionZone>();
  public List<ActiveUnit> AIUnits = new List<ActiveUnit>();
  public List<ActiveUnit> AIUnitsReady = new List<ActiveUnit>();
  public List<ActiveUnit> PCUnits = new List<ActiveUnit>();
  public List<ActiveUnit> PCUnitsReady = new List<ActiveUnit>();
  public List<MissionInteractable> Interactables = new List<MissionInteractable>();
}

class MissionZone
{
  public List<Rectangle> Zones = new List<Rectangle>();
  public Models Model;
  public Icon Icon;
  public Color Color;
  public MissionMessage Message;
}

class MissionInteractable
{
  public int Credits;
  public List<int> Units = new List<int>();
  public List<Unit> AIUnits = new List<Unit>();
  public MissionZone Zone;
}
