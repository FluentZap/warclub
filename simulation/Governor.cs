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

  static public Dictionary<Keys, int> CommanderKeys = new Dictionary<Keys, int>
  {
    [Keys.F1] = 0,
    [Keys.F2] = 1,
    [Keys.F3] = 2,
    [Keys.F4] = 3,
  };



  public static void DoEvents(Simulation s)
  {
    if (s.SelectedView == View.GalaxyOverview) GalaxyOverview(s);
    if (s.SelectedView == View.MissionBriefing) MissionBriefing(s);
    if (s.SelectedView == View.MissionSelect) MissionSelect(s);
    if (s.SelectedView == View.Battlefield) Battlefield(s);
    if (s.SelectedView == View.MainMenu) MainMenu(s);
    if (s.SelectedView == View.NewGame) NewGame(s);
    if (s.SelectedView == View.LoadGame) LoadGame(s);
  }

  static void Battlefield(Simulation s)
  {
    var keys = s.KeyState.GetTriggeredKeys();
    if (keys.Contains(Keys.Space))
      MissionRunner.AdvanceState(s);
  }

  static void MissionBriefing(Simulation s)
  {

    var keys = s.KeyState.GetTriggeredKeys();
    int pageCount = s.SelectableUnits.Count / 9;
    var Selected = s.Commanders[0];

    if (keys.Contains(Keys.Right) && s.CurrentPage < pageCount)
    {
      s.CurrentPage++;
      s.SelectedUnit += 9;
      if (s.SelectedUnit >= s.SelectableUnits.Count) s.SelectedUnit = s.SelectableUnits.Count - 1;
    }
    if (keys.Contains(Keys.Left) && s.CurrentPage > 0)
    {
      s.CurrentPage--;
      s.SelectedUnit -= 9;
    }


    // Select Units
    foreach (var (key, i) in SelectionKeys)
    {
      var unitIndex = (s.CurrentPage * 9) + i;
      if (keys.Contains(key) && unitIndex < s.SelectableUnits.Count)
      {
        s.SelectedUnit = unitIndex;
      }
    }

    if (keys.Contains(Keys.Up) && s.SelectedUnit > 0)
    {
      s.SelectedUnit--;
      s.CurrentPage = s.SelectedUnit / 9;
    }

    if (keys.Contains(Keys.Down) && s.SelectedUnit < s.SelectableUnits.Count - 1)
    {
      s.SelectedUnit++;
      s.CurrentPage = s.SelectedUnit / 9;
    }

    var unit = s.SelectableUnits[s.SelectedUnit];

    foreach (var (key, i) in CommanderKeys)
      if (keys.Contains(key))
      {
        if (s.Commanders[i].Units.Contains(unit))
          s.Commanders[i].Units.Remove(unit);
        else
        {
          foreach (var commander in s.Commanders)
            commander.Units.Remove((unit));
          s.Commanders[i].Units.Add(unit);
        }
      }

    if (keys.Contains(Keys.Enter))
    {
      s.SelectedUnits.Clear();
      foreach (var commander in s.Commanders)
        s.SelectedUnits.AddRange(commander.Units);
      s.SelectedView = View.Battlefield;
      s.MissionState = new MissionState();
      MissionRunner.AdvanceState(s);
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
