using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;


namespace WarClub;


static class MissionRunner
{
  public static void AdvanceState(Simulation s)
  {
    var state = s.MissionState;
    var m = s.ActiveMission;
    if (!state.DeploymentPhase)
    {
      state.Round++;
      state.Messages.Clear();
      if (state.PlayerTurn)
      {
        if (state.PCUnitsReady.Count > 0) state.PCUnitsReady.RemoveAt(0);
        if (state.AIUnitsReady.Count > 0)
        {
          state.ActiveUnit = RNG.PickFrom(state.AIUnitsReady);
          state.PlayerTurn = false;
          state.Messages.Add(new MissionMessage() { Text = $"Activate {state.ActiveUnit.BaseUnit.DataSheet.Name}", Color = Color.OrangeRed, Model = state.ActiveUnit.Model });
        }
        else
        {
          state.Messages.Add(new MissionMessage() { Text = "Ally Activation", Color = Color.GreenYellow });
        }
      }
      else
      {
        state.AIUnitsReady.Remove(state.ActiveUnit);
        if (state.PCUnitsReady.Count > 0)
        {
          state.PlayerTurn = true;
          state.Messages.Add(new MissionMessage() { Text = "Ally Activation", Color = Color.GreenYellow });
        }
        else
        {
          if (state.AIUnitsReady.Count > 0)
          {
            state.ActiveUnit = RNG.PickFrom(state.AIUnitsReady);
            state.Messages.Add(new MissionMessage() { Text = $"Activate {state.ActiveUnit.BaseUnit.DataSheet.Name}", Color = Color.OrangeRed, Model = state.ActiveUnit.Model });
          }
        }
      }
      if (state.AIUnitsReady.Count > 0 || state.PCUnitsReady.Count > 0) return;
    }

    state.Turn++;
    state.Round = 1;
    state.TempZones.Clear();
    state.Messages.Clear();
    var events = m.MissionEvents.Where(x => x.Turn == state.Turn).ToList();
    SpawnZones(state, events);
    // Spawn Units
    bool spawnedUnitRound = false;
    foreach (var e in events)
    {
      if (e.Type == MissionSpawnType.AISpawn)
        state.AIUnits.Add(e.Unit);
      if (e.Type == MissionSpawnType.PCDeploymentZone)
        state.PCUnits.AddRange(s.SelectedUnits);
      if (e.Type == MissionSpawnType.AISpawn || e.Type == MissionSpawnType.PCDeploymentZone) spawnedUnitRound = true;
    }

    state.DeploymentPhase = spawnedUnitRound;

    // reset for new turn
    if (!state.DeploymentPhase)
    {
      state.PlayerTurn = true;
      state.Messages.Add(new MissionMessage() { Text = "Ally Activation", Color = Color.GreenYellow });
      state.AIUnitsReady.AddRange(state.AIUnits);
      state.PCUnitsReady.AddRange(state.PCUnits);
    }
  }


  public static void SpawnZones(MissionState state, List<MissionEvent> events)
  {
    foreach (var e in events)
    {
      if (e.Type == MissionSpawnType.PCDeploymentZone)
        state.TempZones.Add(new MissionZone()
        {
          Color = new Color(119, 221, 119),
          Zones = e.Zones,
          Icon = e.Icon,
        });
      if (e.Type == MissionSpawnType.AISpawn)
        state.TempZones.Add(new MissionZone()
        {
          Color = new Color(255, 80, 80),
          Zones = e.Zones,
          Message = e.Message,
          Model = e.Unit.Model,
        });
      if (e.Type == MissionSpawnType.PCEvacZone)
        state.PermZones.Add(new MissionZone()
        {
          Color = new Color(255, 153, 0),
          Zones = e.Zones,
          Icon = e.Icon,
        });
    }
  }
}