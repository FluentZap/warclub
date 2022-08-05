using System;
using System.Collections.Generic;
using System.Linq;

namespace WarClub
{
  partial class FactionEvent
  {
    public static void AllOutWar(Simulation s, Entity e)
    {
      var (id, world) = RNG.PickFrom(s.cosmos.Worlds);
      // e.AddRelation(world, RelationType.MissionTarget, 255);
      // e.TraitsOut.Clear();
      // e.TraitsOut.Add(RelationType.MissionTarget, new Dictionary<Trait, int>
      // {
      //   [s.Traits["war zone"]] = 1
      // });
    }
  }
}
