using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WarClub
{
  partial class Simulation
  {

    public void GenerateFactions()
    {
      // add one of each faction
      var factions = TraitLists["faction"];

      foreach (var faction in factions)
      {
        var f = new Faction();
        f.AddTrait(faction.Value);
        f.AddTrait(RNG.PickFrom(TraitLists["aspiration"]).Value);
        f.AddRelation(RNG.PickFrom(cosmos.Worlds).Value, RelationType.Headquarters, new Dictionary<Trait, int> { }, 100);
        cosmos.Factions.AutoAdd(f);
      }

      // add random factions
      foreach (int i in Enumerable.Range(0, 13))
      {
        var f = new Faction();
        f.AddTrait(RNG.PickFrom(TraitLists["faction"]).Value);
        f.AddTrait(RNG.PickFrom(TraitLists["aspiration"]).Value);
        f.AddRelation(RNG.PickFrom(cosmos.Worlds).Value, RelationType.Headquarters, new Dictionary<Trait, int> { }, 100);
        cosmos.Factions.AutoAdd(f);
      }

    }
  }
}