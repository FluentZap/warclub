using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WarClub;

partial class Simulation
{

  public void GenerateFactions()
  {

    var factionAbvMap = new Dictionary<string, string>
    {
      ["adeptus custodes"] = "AC",
      ["adeptus mechanicus"] = "AdM",
      ["aeldari"] = "AE",
      ["astra militarum"] = "AM",
      ["adepta sororitas"] = "AS",
      ["astra cartographica"] = "CA",
      ["chaos daemons"] = "CD",
      ["chaos space marines"] = "CSM",
      ["death guard"] = "DG",
      ["drukhari"] = "DRU",
      ["genestealer cults"] = "GC",
      ["grey knights"] = "GK",
      ["heretic titan legions"] = "HTL",
      ["inquisition"] = "INQ",
      ["necrons"] = "NEC",
      ["officio assassinorum"] = "OA",
      ["orks"] = "ORK",
      ["imperial knights"] = "QI",
      ["chaos knights"] = "QT",
      ["renegades and heretics"] = "RaH",
      ["rogue traders"] = "RT",
      ["space marines"] = "SM",
      ["t’au empire"] = "TAU",
      ["titan legions"] = "TL",
      ["thousand sons"] = "TS",
      ["tyranids"] = "TYR",
      ["unaligned"] = "UN",
    };

    // var troopTypes = new string[] {
    //     "Troops",
    //     "Elites",
    //     "Heavy Support",
    //     "HQ",
    //     "Fast Attack",
    //     "Dedicated Transport",
    //     "Flyers",
    //     "Lords of War",
    //     "Fortifications",
    //   };

    var allUnitsByType = new Dictionary<UnitRole, Dictionary<int, DataSheet>>();
    foreach (var unitType in (UnitRole[])Enum.GetValues(typeof(UnitRole)))
      allUnitsByType.Add(unitType, DataSheets.Where(x => x.Value.Role == unitType).ToDictionary(x => x.Key, x => x.Value));


    var factionUnits = new Dictionary<Trait, (int, int)[]>
    {
      [Traits["adeptus custodes"]] = new (int, int)[9] { (10, 10), (10, 10), (5, 10), (5, 10), (5, 10), (0, 0), (0, 0), (0, 0), (0, 0) },
      [Traits["adeptus mechanicus"]] = new (int, int)[9] { (10, 10), (10, 10), (10, 10), (5, 10), (10, 10), (0, 0), (0, 0), (0, 0), (0, 0) },
      [Traits["aeldari"]] = new (int, int)[9] { (10, 10), (10, 10), (10, 10), (5, 10), (10, 10), (0, 0), (0, 0), (0, 0), (0, 0) },
      [Traits["astra militarum"]] = new (int, int)[9] { (30, 10), (5, 10), (20, 10), (2, 10), (5, 10), (0, 0), (0, 0), (0, 0), (0, 0) },
      [Traits["adepta sororitas"]] = new (int, int)[9] { (10, 10), (10, 10), (10, 10), (5, 10), (10, 10), (0, 0), (0, 0), (0, 0), (0, 0) },
      // [Traits["astra cartographica"]] = new (int, int)[9] { (10, 10), (10, 10), (10, 10), (5, 10), (10, 10), (0, 0), (0, 0), (0, 0), (0, 0) },
      [Traits["chaos daemons"]] = new (int, int)[9] { (10, 10), (10, 10), (10, 10), (5, 10), (10, 10), (0, 0), (0, 0), (0, 0), (0, 0) },
      [Traits["chaos space marines"]] = new (int, int)[9] { (10, 10), (10, 10), (10, 10), (5, 10), (10, 10), (0, 0), (0, 0), (0, 0), (0, 0) },
      [Traits["death guard"]] = new (int, int)[9] { (10, 10), (10, 10), (10, 10), (5, 10), (10, 10), (0, 0), (0, 0), (0, 0), (0, 0) },
      [Traits["drukhari"]] = new (int, int)[9] { (10, 10), (10, 10), (10, 10), (5, 10), (10, 10), (0, 0), (0, 0), (0, 0), (0, 0) },
      [Traits["genestealer cults"]] = new (int, int)[9] { (10, 10), (10, 10), (10, 10), (5, 10), (10, 10), (0, 0), (0, 0), (0, 0), (0, 0) },
      [Traits["grey knights"]] = new (int, int)[9] { (10, 10), (10, 10), (10, 10), (5, 10), (10, 10), (0, 0), (0, 0), (0, 0), (0, 0) },
      // [Traits["heretic titan legions"]] = new (int, int)[9] { (10, 10), (10, 10), (10, 10), (5, 10), (10, 10), (0, 0), (0, 0), (0, 0), (0, 0) },
      // [Traits["inquisition"]] = new (int, int)[9] { (10, 10), (10, 10), (10, 10), (5, 10), (10, 10), (0, 0), (0, 0), (0, 0), (0, 0) },
      [Traits["necrons"]] = new (int, int)[9] { (10, 10), (10, 10), (10, 10), (5, 10), (10, 10), (0, 0), (0, 0), (0, 0), (0, 0) },
      // [Traits["officio assassinorum"]] = new (int, int)[9] { (10, 10), (10, 10), (10, 10), (5, 10), (10, 10), (0, 0), (0, 0), (0, 0), (0, 0) },
      [Traits["orks"]] = new (int, int)[9] { (50, 10), (3, 10), (10, 10), (2, 10), (20, 10), (0, 0), (0, 0), (0, 0), (0, 0) },
      // [Traits["imperial knights"]] = new (int, int)[9] { (10, 10), (10, 10), (10, 10), (5, 10), (10, 10), (0, 0), (0, 0), (0, 0), (0, 0) },
      // [Traits["chaos knights"]] = new (int, int)[9] { (10, 10), (10, 10), (10, 10), (5, 10), (10, 10), (0, 0), (0, 0), (0, 0), (0, 0) },
      [Traits["renegades and heretics"]] = new (int, int)[9] { (10, 10), (10, 10), (10, 10), (5, 10), (10, 10), (0, 0), (0, 0), (0, 0), (0, 0) },
      // [Traits["rogue traders"]] = new (int, int)[9] { (10, 10), (10, 10), (10, 10), (5, 10), (10, 10), (0, 0), (0, 0), (0, 0), (0, 0) },
      [Traits["space marines"]] = new (int, int)[9] { (10, 10), (10, 10), (10, 10), (5, 10), (10, 10), (0, 0), (0, 0), (0, 0), (0, 0) },
      [Traits["t’au empire"]] = new (int, int)[9] { (10, 10), (10, 10), (10, 10), (5, 10), (10, 10), (0, 0), (0, 0), (0, 0), (0, 0) },
      // [Traits["titan legions"]] = new (int, int)[9] { (10, 10), (10, 10), (10, 10), (5, 10), (10, 10), (0, 0), (0, 0), (0, 0), (0, 0) },
      [Traits["thousand sons"]] = new (int, int)[9] { (10, 10), (10, 10), (10, 10), (5, 10), (10, 10), (0, 0), (0, 0), (0, 0), (0, 0) },
      [Traits["tyranids"]] = new (int, int)[9] { (10, 10), (10, 10), (10, 10), (5, 10), (10, 10), (0, 0), (0, 0), (0, 0), (0, 0) },
      [Traits["unaligned"]] = new (int, int)[9] { (10, 10), (10, 10), (10, 10), (5, 10), (10, 10), (0, 0), (0, 0), (0, 0), (0, 0) },
    };

    void AddUnits(Faction f, Dictionary<int, DataSheet> unitsByType, int count, int pointMax = -1)
    {
      int points = 0;
      foreach (int e in Enumerable.Range(0, count))
      {
        var unitTemplate = RNG.PickFrom(unitsByType);

        var newUnit = UnitUtils.CreateUnit(unitTemplate.Value, CreateUnitSize.Random);
        points += newUnit.Points;
        f.Units[unitTemplate.Value.Role].Add(newUnit.BaseUnit);

        if (pointMax != -1 && points > pointMax) return;
      }
    }

    // Generate random troops to add to the faction
    void GenerateTroops(Faction f)
    {
      var factionTrait = TraitUtil.getTraitsByType(f.GetTraits(), "faction").First().Key;

      var unitRoles = (UnitRole[])Enum.GetValues(typeof(UnitRole));
      // foreach (var role in (UnitRole[])Enum.GetValues(typeof(UnitRole)))
      foreach (int i in Enumerable.Range(0, 9))
      {
        var role = unitRoles[i];
        var unitsByType = allUnitsByType[role].Where(x => x.Value.FactionId == factionAbvMap[factionTrait.Name]).ToDictionary(x => x.Key, x => x.Value);
        var (count, die) = factionUnits[factionTrait][i];
        if (unitsByType.Count > 0)
        {
          // AddUnits(f, unitsByType, RNG.DiceRoll(count, die), 100000);
          AddUnits(f, unitsByType, RNG.DiceRoll(count, die), -1);
        }
      }
    }

    void AddUnitToFaction(Faction f, DataSheet unitTemplate)
    {
      var newUnit = new Unit();
      newUnit.DataSheet = unitTemplate;

      foreach (var (lineId, line) in unitTemplate.Units)
      {
        newUnit.UnitLines.Add(lineId, line.MinModelsPerUnit);
        // adding new unit
      }
      if (!f.Units.ContainsKey(unitTemplate.Role))
        f.Units.Add(unitTemplate.Role, new List<Unit>());
      f.Units[unitTemplate.Role].Add(newUnit);
    }

    // add one of each faction
    var factions = TraitLists["faction"];

    foreach (var faction in factions)
    {
      var f = new Faction();
      f.AddTrait(faction.Value);
      f.AddTrait(RNG.PickFrom(TraitLists["aspiration"]).Value);
      f.AddRelation(RNG.PickFrom(cosmos.Worlds).Value, RelationType.Headquarters, new Dictionary<Trait, int> { }, 100);
      GenerateTroops(f);
      cosmos.Factions.AutoAdd(f);
    }

    // add random factions
    foreach (int i in Enumerable.Range(0, 13))
    {
      var f = new Faction();
      f.AddTrait(RNG.PickFrom(TraitLists["faction"]).Value);
      f.AddTrait(RNG.PickFrom(TraitLists["aspiration"]).Value);
      f.AddRelation(RNG.PickFrom(cosmos.Worlds).Value, RelationType.Headquarters, new Dictionary<Trait, int> { }, 100);
      GenerateTroops(f);
      cosmos.Factions.AutoAdd(f);
    }

    // add new faction for Player

    var playerFaction = new Faction();
    playerFaction.AddRelation(RNG.PickFrom(cosmos.Worlds).Value, RelationType.Headquarters, new Dictionary<Trait, int> { }, 100);
    // AddUnitToFaction(playerFaction, DataSheets[16]);
    // AddUnitToFaction(playerFaction, DataSheets[854]);
    // AddUnitToFaction(playerFaction, DataSheets[840]);
    // for (int i = 0; i < 23; i++)
    // {
    //   if (DataSheets.ContainsKey(850 + i)) AddUnitToFaction(playerFaction, DataSheets[850 + i]);
    // }


    cosmos.Factions.AutoAdd(playerFaction);
    cosmos.PlayerFaction = playerFaction;
  }
}