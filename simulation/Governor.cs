using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.Xna.Framework;


namespace WarClub;

static class InputGovernor
{
  static public Dictionary<Keys, int> SelectionKeys = new Dictionary<Keys, int>
  {
    [Keys.D1] = 0,
    [Keys.D2] = 1,
    [Keys.D3] = 2,
    [Keys.D4] = 3,
    [Keys.D5] = 4,
    [Keys.D6] = 5,
    [Keys.D7] = 6,
    [Keys.D8] = 7,
    [Keys.D9] = 8,
  };




  public static void DoEvents(Simulation s)
  {
    if (s.SelectedView == View.GalaxyOverview) GalaxyOverview(s);
    if (s.SelectedView == View.MissionBriefing) MissionBriefing(s);
    if (s.SelectedView == View.MissionSelect) MissionSelect(s);
    if (s.SelectedView == View.MainMenu) MainMenu(s);
    if (s.SelectedView == View.NewGame) NewGame(s);
    if (s.SelectedView == View.LoadGame) LoadGame(s);
  }

  static void MissionBriefing(Simulation s)
  {

    var keys = s.KeyState.GetTriggeredKeys();
    int pageCount = s.SelectableUnits.Count / 9;

    if (keys.Contains(Keys.F1))
      s.Commanders[0].Active = !s.Commanders[0].Active;
    if (keys.Contains(Keys.F2))
      s.Commanders[1].Active = !s.Commanders[1].Active;
    if (keys.Contains(Keys.F3))
      s.Commanders[2].Active = !s.Commanders[2].Active;
    if (keys.Contains(Keys.F4))
      s.Commanders[3].Active = !s.Commanders[3].Active;

    if (keys.Contains(Keys.Right))
    {
      if (s.CurrentPage < pageCount) s.CurrentPage++;
      return;
    }

    if (keys.Contains(Keys.Left))
    {
      if (s.CurrentPage > 0) s.CurrentPage--;
      return;
    }

  }



  static void GalaxyOverview(Simulation s)
  {
    var size = new Point(480, 570);

    if (Mouse.GetState().LeftButton == ButtonState.Pressed)
    {

      var pos = Vector2.Transform(Mouse.GetState().Position.ToVector2(), Matrix.Invert(s.ViewMatrix));
      foreach (var world in s.cosmos.Worlds)
        if (new Rectangle(world.Value.location.ToPoint() - size / new Point(2), size).Contains(pos))
        {
          s.SelectedView = View.MissionSelect;
          s.SelectedWorld = world.Value;
        }
    }
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


    foreach (var (key, i) in SelectionKeys)
    {
      if (keys.Contains(key) && missions.Count > i)
      {
        s.SelectableUnits = s.cosmos.PlayerFaction.Units.SelectMany(x => x.Value).Select(x => UnitUtils.ActivateUnit(x)).ToList();
        s.CurrentPage = 0;
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
    int pageCount = s.SelectableUnits.Count / 9;

    if (keys.Contains(Keys.Right))
    {
      if (s.CurrentPage < pageCount) s.CurrentPage++;
      return;
    }

    if (keys.Contains(Keys.Left))
    {
      if (s.CurrentPage > 0) s.CurrentPage--;
      return;
    }
    // select units
    foreach (var (key, i) in SelectionKeys)
    {
      var unitIndex = (s.CurrentPage * 9) + i;
      if (keys.Contains(key) && unitIndex < s.SelectableUnits.Count)
      {
        if (s.SelectedUnits.Contains(s.SelectableUnits[unitIndex]))
        {
          s.SelectedUnits.Remove(s.SelectableUnits[unitIndex]);
        }
        else
        {
          s.SelectedUnits.Add(s.SelectableUnits[unitIndex]);
        }
      }
    }

    if (keys.Contains(Keys.Enter) && s.SelectedUnits.Count > 0)
    {
      foreach (var unit in s.SelectedUnits)
        s.cosmos.PlayerFaction.Units[unit.BaseUnit.DataSheet.Role].Add(unit.BaseUnit);
      s.SelectedView = View.GalaxyOverview;
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
