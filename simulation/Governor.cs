using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

using WarClub;



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
    var keys = s.KeyState.GetTriggeredKeys();
    // s.KeyState
    if (s.SelectedView == View.MissionSelect)
    {
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
        }

      }

    }

  }

}
