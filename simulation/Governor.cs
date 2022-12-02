using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace WarClub;

static class InputGovernor
{
  static public Dictionary<Keys, int> MissionKeys = new Dictionary<Keys, int>
  {
    [Keys.D1] = 0,
    [Keys.D2] = 1,
    [Keys.D3] = 2,
    [Keys.D4] = 3,
  };




  public static void DoEvents(Simulation s)
  {
    if (s.SelectedView == View.MissionSelect) MissionSelect(s);
    if (s.SelectedView == View.MainMenu) MainMenu(s);
    if (s.SelectedView == View.NewGame) NewGame(s);
    if (s.SelectedView == View.LoadGame) LoadGame(s);
  }

  static void MissionSelect(Simulation s)
  {
    var keys = s.KeyState.GetTriggeredKeys();
    if (keys.Contains(Keys.Space))
    {
      s.SelectedWorld = null;
      s.SelectedView = View.GalaxyOverview;
      return;
    }
    var missions = s.SelectedWorld.GetEventList(s.OrderEventLists["Mercenary"]);


    foreach (var (key, i) in MissionKeys)
    {
      if (keys.Contains(key) && missions.Count > i)
      {
        s.SelectedView = View.MissionBriefing;
        s.SelectedMission = missions[i];
        s.ActiveMission = Generator.GenerateBattleMap(s);
      }

    }

  }

  static void MainMenu(Simulation s)
  {
    var keys = s.KeyState.GetTriggeredKeys();
    if (keys.Contains(Keys.D1))
    {
      s.SelectedView = View.NewGame;
      s.SelectableUnits = UnitUtils.GetRoster(s, s.UnitList);
      return;
    }
    if (keys.Contains(Keys.D2))
    {
      s.SelectedView = View.LoadGame;
      return;
    }
  }

  static void NewGame(Simulation s)
  {
    var keys = s.KeyState.GetTriggeredKeys();
    if (keys.Contains(Keys.Space))
    {
      s.SelectedWorld = null;
      s.SelectedView = View.GalaxyOverview;
      return;
    }
  }

  static void LoadGame(Simulation s)
  {
    var keys = s.KeyState.GetTriggeredKeys();
    if (keys.Contains(Keys.Space))
    {
      s.SelectedWorld = null;
      s.SelectedView = View.GalaxyOverview;
      return;
    }
  }
}
