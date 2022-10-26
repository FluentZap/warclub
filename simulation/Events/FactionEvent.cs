using System;
using System.Collections.Generic;
using System.Linq;

namespace WarClub;

partial class FactionEvent
{
  public static void AllOutWar(Simulation s, Entity e)
  {
    var (id, world) = RNG.PickFrom(s.cosmos.Worlds);
    e.AddRelation(RNG.PickFrom(s.cosmos.Worlds).Value, RelationType.MissionTarget, new Dictionary<Trait, int>
    {
      [s.Traits["war zone"]] = 1
    }, 10);
    e.AddTrait(s.Traits["aspiration fatigue"], 10);
  }

  public static void DoEvents(Simulation s, Entity e)
  {
    ShiftPersonality(s, e);
    CompleteMissions(s, e);
  }

  public static void CompleteMissions(Simulation s, Entity e)
  {
    foreach (var relation in e.Relations)
    {
      relation.Strength--;
      // missions can resolve here
      // if (relation.Strength <= 0)
      // {
      // }
    }

    foreach (var relation in e.Relations.Where(x => x.Strength <= 0))
      relation.Target.Relations.Remove(relation);
    e.Relations.RemoveAll(x => x.Strength <= 0);
  }

  public static void ShiftPersonality(Simulation s, Entity e)
  {
    // if the fatigue is higher then 100, switch approach
    if (TraitUtil.getAspect(e.GetTraits(), "aspiration fatigue") >= 100)
    {
      var aspirations = TraitUtil.getTraitsByType(e.GetTraits(), "aspiration");
      var newAspirations = s.TraitLists["aspiration"];

      foreach (var aspiration in aspirations)
      {
        e.RemoveTrait(aspiration.Key);
        newAspirations.Remove(aspiration.Key.Name);
      }

      e.AddTrait(RNG.PickFrom(newAspirations).Value);
      e.RemoveTrait(s.Traits["aspiration fatigue"]);
    }
  }
}

