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

  public static void ConquerStrongholds(Simulation s, Entity e)
  {
    var (id, world) = RNG.PickFrom(s.cosmos.Worlds);
    e.AddRelation(RNG.PickFrom(s.cosmos.Worlds).Value, RelationType.MissionTarget, new Dictionary<Trait, int>
    {
      [s.Traits["strongholds"]] = 1
    }, 10);
    e.AddTrait(s.Traits["aspiration fatigue"], 10);
  }

  public static void AssassinateHeros(Simulation s, Entity e)
  {
    var (id, world) = RNG.PickFrom(s.cosmos.Worlds);
    e.AddRelation(RNG.PickFrom(s.cosmos.Worlds).Value, RelationType.MissionTarget, new Dictionary<Trait, int>
    {
      [s.Traits["high value targets"]] = 1
    }, 10);
    e.AddTrait(s.Traits["aspiration fatigue"], 10);
  }

  public static void CompleteRitual(Simulation s, Entity e)
  {
    var (id, world) = RNG.PickFrom(s.cosmos.Worlds);
    e.AddRelation(RNG.PickFrom(s.cosmos.Worlds).Value, RelationType.MissionTarget, new Dictionary<Trait, int>
    {
      [s.Traits["enlisted gods"]] = 1
    }, 10);
    e.AddTrait(s.Traits["aspiration fatigue"], 10);
  }

  public static void RecruitInitiates(Simulation s, Entity e)
  {
    var (id, world) = RNG.PickFrom(s.cosmos.Worlds);
    e.AddRelation(RNG.PickFrom(s.cosmos.Worlds).Value, RelationType.MissionTarget, new Dictionary<Trait, int>
    {
      [s.Traits["training camps"]] = 1
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
      if (relation.relationType == RelationType.MissionTarget)
        relation.Strength -= 5;
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
      var newAspirations = s.TraitLists["aspiration"].Select(x => x.Key).ToHashSet();

      foreach (var aspiration in aspirations)
      {
        e.RemoveTrait(aspiration.Key);
        newAspirations.Remove(aspiration.Key.Name);
      }

      e.AddTrait(s.Traits[RNG.PickFrom(newAspirations)]);
      e.RemoveTrait(s.Traits["aspiration fatigue"]);
    }
  }
}

