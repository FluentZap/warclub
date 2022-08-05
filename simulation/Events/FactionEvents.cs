using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace WarClub
{
  partial class Simulation
  {

    enum EventTypes
    {
      FactionEvent
    }

    void FactionEvent(uint id)
    {
      Entity e = this.cosmos.Factions[id];

      Dictionary<string, int> modifierList = new Dictionary<string, int>();
      // modifierList.Add("create creature", (int)(e.GetRelationsOut(EntityType.CreatureGroup, RelationType.Created).Count * 0.5f));
      // modifierList.Add("gain power", (int)(TraitUtil.getAspects(e.Traits).GetValueOrDefault("power") - 200));
      Trait t = TraitUtil.getTraitsByType(e.Traits, "aspiration").Keys.First();

      if (!OrderEventLists.ContainsKey("Faction"))
        return;

      List<(OrderEvent, int)> events = e.GetEventByPsycheDivergence(e.GetEventList(OrderEventLists["Faction"]), modifierList);
      if (events.Count < 1) return;
      var currentEvent = events[0].Item1;

      // shift the psyche because of the action just taken
      e.Psyche.ShiftByPsyche(currentEvent.Psyche);

      var methodName = String.Join("", currentEvent.Name.Split(" ").Select(x => char.ToUpper(x[0]) + x.Substring(1)));

      InvokeEvent(EventTypes.FactionEvent, methodName, new object[] { this, e });
    }

    static void InvokeEvent(EventTypes type, string name, object[] args)
    {
      // TODO: make a map to load all of the functions
      var eventFunctions = Type.GetType("WarClub." + Enum.GetName(typeof(EventTypes), type));
      var a = eventFunctions.GetMember(name);
      if (eventFunctions.GetMember(name).Length > 0)
        eventFunctions.InvokeMember(name, BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, args);
    }
  }
}