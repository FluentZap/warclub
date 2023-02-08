using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;


namespace WarClub;


static class MissionRunner
{
  // public static void AdvanceState(Simulation s)
  // {
  //   var state = s.MissionState;
  //   var m = s.ActiveMission;
  //   if (!state.DeploymentPhase)
  //   {
  //     state.Round++;
  //     state.Messages.Clear();
  //     if (state.PlayerTurn)
  //     {
  //       if (state.PCUnitsReady.Count > 0) state.PCUnitsReady.RemoveAt(0);
  //       if (state.AIUnitsReady.Count > 0)
  //       {
  //         state.ActiveUnit = RNG.PickFrom(state.AIUnitsReady);
  //         state.PlayerTurn = false;
  //         state.Messages.Add(new MissionMessage() { Text = $"Activate {state.ActiveUnit.BaseUnit.DataSheet.Name}", Color = Color.OrangeRed, Model = state.ActiveUnit.Model });
  //       }
  //       else
  //       {
  //         state.Messages.Add(new MissionMessage() { Text = "Ally Activation", Color = Color.GreenYellow });
  //       }
  //     }
  //     else
  //     {
  //       state.AIUnitsReady.Remove(state.ActiveUnit);
  //       if (state.PCUnitsReady.Count > 0)
  //       {
  //         state.PlayerTurn = true;
  //         state.Messages.Add(new MissionMessage() { Text = "Ally Activation", Color = Color.GreenYellow });
  //       }
  //       else
  //       {
  //         if (state.AIUnitsReady.Count > 0)
  //         {
  //           state.ActiveUnit = RNG.PickFrom(state.AIUnitsReady);
  //           state.Messages.Add(new MissionMessage() { Text = $"Activate {state.ActiveUnit.BaseUnit.DataSheet.Name}", Color = Color.OrangeRed, Model = state.ActiveUnit.Model });
  //         }
  //       }
  //     }
  //     if (state.AIUnitsReady.Count > 0 || state.PCUnitsReady.Count > 0) return;
  //   }

  //   state.Turn++;
  //   state.Round = 1;
  //   state.TempZones.Clear();
  //   state.Messages.Clear();
  //   var events = m.MissionEvents.Where(x => x.Turn == state.Turn).ToList();
  //   SpawnZones(state, events);
  //   // Spawn Units
  //   bool spawnedUnitRound = false;
  //   foreach (var e in events)
  //   {
  //     if (e.Type == MissionSpawnType.AISpawn)
  //       state.AIUnits.Add(e.Unit);
  //     if (e.Type == MissionSpawnType.PCDeploymentZone)
  //       state.PCUnits.AddRange(s.SelectedUnits);
  //     if (e.Type == MissionSpawnType.AISpawn || e.Type == MissionSpawnType.PCDeploymentZone) spawnedUnitRound = true;
  //   }

  //   state.DeploymentPhase = spawnedUnitRound;

  //   // reset for new turn
  //   if (!state.DeploymentPhase)
  //   {
  //     state.PlayerTurn = true;
  //     state.Messages.Add(new MissionMessage() { Text = "Ally Activation", Color = Color.GreenYellow });
  //     state.AIUnitsReady.AddRange(state.AIUnits);
  //     state.PCUnitsReady.AddRange(state.PCUnits);
  //   }
  // }

  public static void AdvanceState(Simulation s)
  {
    s.MissionState.TempZones.Clear();
    if (s.MissionState.Turn == 0) AddStartingEvents(s);

    if (s.MissionState.EventQueue.Count > 0) { ProcessMissionEvent(s); return; }
    AdvancePhase(s);
    if (s.MissionState.Phase == Phases.Command) { Command(s); return; }
    if (s.MissionState.Phase == Phases.Movement) { Movement(s); return; }
    if (s.MissionState.Phase == Phases.Psychic) { Psychic(s); return; }
    if (s.MissionState.Phase == Phases.Shooting) { Shooting(s); return; }
    if (s.MissionState.Phase == Phases.Charge) { Charge(s); return; }
    if (s.MissionState.Phase == Phases.Fight) { Fight(s); return; }
    if (s.MissionState.Phase == Phases.Morale) { Morale(s); return; }
  }

  public static void Interact(Simulation s, int id)
  {
    var state = s.MissionState;
    if (state.Phase != Phases.Morale) return;
    if (id >= state.Interactables.Count) return;
    var interacted = state.Interactables[id];
    // foreach (var unit in interacted.AIUnits)
    //   state.EventQueue.Enqueue(new MissionEvent()
    //   {
    //     Type = MissionEventType.AISpawn,
    //     Zones = interacted.Zone.Zones,
    //     Unit = units,
    //     Message = new MissionMessage() { Text = Generator.GetUnitSpawnLabel(unit), Color = Color.White }
    //   });
    foreach (var e in interacted.TriggeredEvents)
      state.EventQueue.Enqueue(e);
    state.Interactables.Remove(interacted);
    for (int i = 0; i < state.Interactables.Count; i++)
      state.Interactables[i].Zone.Message.Text = $"Objective: {i + 1}";

    AdvanceState(s);
    state.EventQueue.Enqueue(new MissionEvent() { Message = new MissionMessage() { Text = "Activate Locations", Color = Color.White }, InteractionEvent = true });
  }

  public static void AddStartingEvents(Simulation s)
  {
    var state = s.MissionState;
    var events = s.ActiveMission.MissionEvents.Where(x => x.Turn == 0).ToList();
    foreach (var e in events)
      state.EventQueue.Enqueue(e);

    state.Turn = 1;
  }

  public static void AdvancePhase(Simulation s)
  {
    var state = s.MissionState;
    state.Phase++;

    if ((int)state.Phase >= Enum.GetValues(typeof(Phases)).Length)
    {
      state.Phase = Phases.Command;
      if (state.PlayerTurn)
      {
        state.PlayerTurn = false;
      }
      else
      {
        state.PlayerTurn = true;
        state.Turn++;
      }
    }
    var events = s.ActiveMission.MissionEvents.Where(x =>
    {
      if (x.Turn == state.Turn && x.Phase == state.Phase) return true;
      if (state.PlayerTurn == true && x.Frequency > 0 && state.Turn > x.Turn && x.Phase == state.Phase && (state.Turn - x.Turn) % x.Frequency == 0) return true;
      return false;
    }).ToList();
    foreach (var e in events)
      state.EventQueue.Enqueue(e);
  }

  public static void ProcessMissionEvent(Simulation s)
  {
    var state = s.MissionState;
    state.Messages.Clear();
    var e = s.MissionState.EventQueue.Dequeue();

    state.CanInteract = e.InteractionEvent;
    SpawnZones(s.MissionState, e);

    if (e.Type == MissionEventType.AISpawn)
      state.AIUnits.Add(e.Unit);
    if (e.Type == MissionEventType.PCDeploymentZone && state.PCUnits.Count == 0)
      state.PCUnits.AddRange(s.SelectedUnits);

    if (e.Type == MissionEventType.LootBox)
      state.Interactables.Add(new MissionInteractable()
      {
        Zone = new MissionZone()
        {
          Color = Color.White,
          Zones = e.Zones,
          Icon = Icon.HumanTarget,
          Message = e.Message,
        },
        TriggeredEvents = e.TriggeredEvents,
      });


    state.Messages.Add(e.Message);
  }

  public static void Command(Simulation s)
  {
    var state = s.MissionState;
    state.Messages.Clear();
    state.Messages.Add(new MissionMessage() { Text = "Command Phase", Color = Color.GreenYellow });

  }

  public static void Movement(Simulation s)
  {
    var state = s.MissionState;
    state.Messages.Clear();
    state.Messages.Add(new MissionMessage() { Text = "Movement Phase", Color = Color.GreenYellow });

  }

  public static void Psychic(Simulation s)
  {
    var state = s.MissionState;
    state.Messages.Clear();
    state.Messages.Add(new MissionMessage() { Text = "Psychic Phase", Color = Color.GreenYellow });

  }

  public static void Shooting(Simulation s)
  {
    var state = s.MissionState;
    state.Messages.Clear();
    state.Messages.Add(new MissionMessage() { Text = "Shooting Phase", Color = Color.GreenYellow });

  }

  public static void Charge(Simulation s)
  {
    var state = s.MissionState;
    state.Messages.Clear();
    state.Messages.Add(new MissionMessage() { Text = "Charge Phase", Color = Color.GreenYellow });

  }

  public static void Fight(Simulation s)
  {
    var state = s.MissionState;
    state.Messages.Clear();
    state.Messages.Add(new MissionMessage() { Text = "Fight Phase", Color = Color.GreenYellow });

  }

  public static void Morale(Simulation s)
  {
    var state = s.MissionState;
    state.Messages.Clear();
    state.Messages.Add(new MissionMessage() { Text = "Morale Phase", Color = Color.GreenYellow });

  }

  public static void SpawnZones(MissionState state, MissionEvent e)
  {
    if (e.Type == MissionEventType.PCDeploymentZone)
      state.TempZones.Add(new MissionZone()
      {
        Color = new Color(119, 221, 119),
        Zones = e.Zones,
        Icon = e.Icon,
      });
    if (e.Type == MissionEventType.AISpawn)
      state.TempZones.Add(new MissionZone()
      {
        Color = new Color(255, 80, 80),
        Zones = e.Zones,
        Message = e.Message,
        Model = e.Unit.Model,
      });
    if (e.Type == MissionEventType.PCEvacZone)
      state.PermZones.Add(new MissionZone()
      {
        Color = new Color(255, 153, 0),
        Zones = e.Zones,
        Icon = e.Icon,
      });
  }

}