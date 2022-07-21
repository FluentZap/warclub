using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WarClub
{
  partial class Simulation
  {

    T getFromRange<T>(Dictionary<int, T> rollTable, int rollOverride = 0)
    {
      int roll = rollOverride != 0 ? rollOverride : RNG.Integer(1, 100);
      foreach (var type in rollTable)
        if (roll <= type.Key) return type.Value;
      return rollTable.First().Value;
    }

    enum RollTable
    {
      PlanetType,
      PlanetTech,
      PlanetTilt,
      PlanetSize,
      StarSize,
    }

    Dictionary<RollTable, Dictionary<int, Trait>> GetRollTables(Dictionary<string, TraitList> traitLists)
    {
      var wt = TraitLists["world type"];
      var tl = TraitLists["tech level"];
      var ss = TraitLists["star size"];
      var ps = TraitLists["planet size"];
      var pt = TraitLists["planet tilt"];
      var presenceTraits = TraitLists["presence"];

      return new Dictionary<RollTable, Dictionary<int, Trait>>
      {
        [RollTable.PlanetType] = new Dictionary<int, Trait>
        {
          [20] = wt["Hive World"],
          [28] = wt["Agri World"],
          [33] = wt["Forge World"],
          [41] = wt["Mining World"],
          [49] = wt["Developing World"],
          [53] = wt["Feudal World"],
          [58] = wt["Feral World"],
          [64] = wt["Shrine World"],
          [68] = wt["Cemetery World"],
          [73] = wt["Pleasure World"],
          [76] = wt["Quarantined World"],
          [82] = wt["War World"],
          [84] = wt["Dead World"],
          [87] = wt["Death World"],
          [92] = wt["Frontier World"],
          [95] = wt["Forbidden World"],
          [99] = wt["Xenos World"],
        },
        [RollTable.PlanetTech] = new Dictionary<int, Trait>
        {
          [05] = tl["Stone Age"],
          [10] = tl["Iron Age"],
          [15] = tl["Steel Age"],
          [20] = tl["Pre Industrial"],
          [25] = tl["Industrial"],
          [30] = tl["Early Space"],
          [35] = tl["Advanced Space"],
          [40] = tl["Warp Space"],
          [45] = tl["Low Imperial"],
          [50] = tl["Mid Imperial"],
          [55] = tl["High Imperial"],
          [60] = tl["Advanced"],
        },
        [RollTable.StarSize] = new Dictionary<int, Trait>
        {
          [5] = ss["Tiny"],
          [15] = ss["Small"],
          [75] = ss["Medium"],
          [85] = ss["Large"],
          [95] = ss["Huge"],
          [100] = ss["Giant"],
        },
        [RollTable.PlanetSize] = new Dictionary<int, Trait>
        {
          [10] = ps["Miniscule"],
          [20] = ps["Tiny"],
          [35] = ps["Small"],
          [75] = ps["Average"],
          [85] = ps["Large"],
          [90] = ps["Huge"],
          [95] = ps["Enormous"],
          [100] = ps["Massive"],
        },
        [RollTable.PlanetTilt] = new Dictionary<int, Trait>
        {
          [5] = pt["No Tilt"],
          [15] = pt["Slight Tilt"],
          [30] = pt["Notable Tilt"],
          [70] = pt["Moderate Tilt"],
          [85] = pt["Large Tilt"],
          [95] = pt["Severe Tilt"],
          [100] = pt["Extreme Tilt"],
        },
      };
    }



    public void GeneratePlanes()
    {

      var rollTables = GetRollTables(TraitLists);
      var wt = TraitLists["world type"];
      var ps = TraitLists["planet size"];
      var ss = TraitLists["star size"];
      var presenceTraits = TraitLists["presence"];

      var starSizeMap = new Dictionary<Trait, float>
      {
        [ss["Tiny"]] = 0.5f,
        [ss["Small"]] = 0.75f,
        [ss["Medium"]] = 1.0f,
        [ss["Large"]] = 1.25f,
        [ss["Huge"]] = 1.5f,
        [ss["Giant"]] = 1.75f,
      };

      var planetSizeMap = new Dictionary<Trait, float>
      {
        [ps["Miniscule"]] = 0.2f,
        [ps["Tiny"]] = 0.4f,
        [ps["Small"]] = 0.6f,
        [ps["Average"]] = 0.8f,
        [ps["Large"]] = 1.0f,
        [ps["Huge"]] = 1.2f,
        [ps["Enormous"]] = 1.4f,
        [ps["Massive"]] = 1.6f,
      };

      var planetDayLength = new Dictionary<int, (int, int, int)>
      {
        // dice, count, multiplier
        [5] = (1, 5, 1),
        [15] = (1, 10, 1),
        [25] = (2, 10, 1),
        [35] = (3, 10, 1),
        [45] = (4, 10, 1),
        [65] = (5, 10, 1),
        [75] = (6, 10, 1),
        [85] = (7, 10, 1),
        [90] = (8, 10, 1),
        [95] = (9, 10, 1),
        [100] = (10, 10, 1),
        [120] = (10, 10, 2),
        [150] = (10, 10, 3),
      };

      var planetDayLengthMap = new Dictionary<Trait, int>
      {
        [ps["Miniscule"]] = -30,
        [ps["Tiny"]] = -20,
        [ps["Small"]] = -10,
        [ps["Average"]] = 0,
        [ps["Large"]] = 10,
        [ps["Huge"]] = 20,
        [ps["Enormous"]] = 30,
        [ps["Massive"]] = 50,
      };

      var adeptusList = new Dictionary<Trait, (int, int)[]>
      {
        [wt["Hive World"]] = new (int, int)[7] { (3, 10), (3, 10), (1, 10), (2, 10), (4, 10), (3, 10), (3, 10) },
        [wt["Agri World"]] = new (int, int)[7] { (1, 10), (1, 10), (1, 5), (1, 10), (2, 10), (2, 10), (1, 5) },
        [wt["Forge World"]] = new (int, int)[7] { (1, 10), (1, 10), (1, 5), (5, 10), (2, 10), (1, 5), (1, 5) },
        [wt["Mining World"]] = new (int, int)[7] { (1, 10), (1, 10), (1, 5), (3, 10), (2, 10), (2, 10), (1, 5) },
        [wt["Developing World"]] = new (int, int)[7] { (0, 0), (1, 5), (0, 0), (1, 10), (1, 10), (1, 10), (1, 5) },
        [wt["Feudal World"]] = new (int, int)[7] { (0, 0), (1, 5), (0, 0), (0, 0), (1, 5), (1, 5), (1, 5) },
        [wt["Feral World"]] = new (int, int)[7] { (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0) },
        [wt["Shrine World"]] = new (int, int)[7] { (1, 10), (1, 5), (1, 5), (1, 5), (2, 10), (4, 10), (1, 5) },
        [wt["Cemetery World"]] = new (int, int)[7] { (1, 5), (1, 5), (1, 5), (1, 5), (1, 10), (3, 10), (1, 5) },
        [wt["Pleasure World"]] = new (int, int)[7] { (2, 10), (2, 10), (1, 5), (2, 10), (2, 10), (2, 10), (1, 5) },
        [wt["Death World"]] = new (int, int)[7] { (1, 5), (1, 5), (1, 5), (1, 5), (1, 10), (1, 10), (1, 5) },
        [wt["Frontier World"]] = new (int, int)[7] { (0, 0), (0, 5), (0, 0), (1, 5), (1, 5), (1, 5), (1, 5) },
        [wt["Forbidden World"]] = new (int, int)[7] { (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0) },
        [wt["Xenos World"]] = new (int, int)[7] { (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0) },
      };

      var destroyedWorlds = new HashSet<Trait> { wt["Quarantined World"], wt["War World"], wt["Dead World"] };

      void addPlanetType(Planet p)
      {
        var planetType = getFromRange(rollTables[RollTable.PlanetType]);

        if (destroyedWorlds.Contains(planetType))
        {
          var planetTypeOriginal = getFromRange(rollTables[RollTable.PlanetType]);
          while (destroyedWorlds.Contains(planetTypeOriginal))
            planetTypeOriginal = getFromRange(rollTables[RollTable.PlanetType]);
          p.AddTrait(planetTypeOriginal);
        }
        System.Console.WriteLine(planetType.Name);
        p.AddTrait(planetType);
      }

      void addTechLevel(Planet p)
      {
        int getTechRoll()
        {
          Trait t = TraitUtil.getTraitsByType(p.Traits, "world type").Keys.First();
          if (t == wt["Hive World"]) return 36 + RNG.DiceRoll(2, 10);
          if (t == wt["Agri World"]) return 15 + RNG.DiceRoll(3, 10);
          if (t == wt["Forge World"]) return 50 + RNG.DiceRoll(1, 10);
          if (t == wt["Mining World"]) return 15 + RNG.DiceRoll(4, 10);
          if (t == wt["Developing World"]) return 20 + RNG.DiceRoll(2, 10);
          if (t == wt["Feudal World"]) return 5 + RNG.DiceRoll(1, 10);
          if (t == wt["Feral World"]) return RNG.DiceRoll(1, 10);
          if (t == wt["Shrine World"]) return 20 + RNG.DiceRoll(4, 10);
          if (t == wt["Cemetery World"]) return 20 + RNG.DiceRoll(3, 10);
          if (t == wt["Pleasure World"]) return 35 + RNG.DiceRoll(2, 10);
          if (t == wt["Death World"]) return RNG.DiceRoll(4, 10);
          if (t == wt["Frontier World"]) return 35 + RNG.DiceRoll(2, 10);
          if (t == wt["Forbidden World"]) return RNG.DiceRoll(6, 10);
          if (t == wt["Xenos World"]) return RNG.DiceRoll(6, 10);
          return 0;
        }
        p.AddTrait(getFromRange(rollTables[RollTable.PlanetTech], getTechRoll()));
      }

      void addAdeptus(Planet p)
      {
        Trait t = TraitUtil.getTraitsByType(p.Traits, "world type").Keys.First();
        var rolls = adeptusList[t];
        p.AddTrait(presenceTraits["Adeptus Arbites Presence"], RNG.DiceRoll(rolls[0].Item1, rolls[0].Item2));
        p.AddTrait(presenceTraits["Adeptus Astra Telepathica Presence"], RNG.DiceRoll(rolls[1].Item1, rolls[1].Item2));
        p.AddTrait(presenceTraits["Adeptus Astronimica Presence"], RNG.DiceRoll(rolls[2].Item1, rolls[2].Item2));
        p.AddTrait(presenceTraits["Adeptus Mechanicus Presence"], RNG.DiceRoll(rolls[3].Item1, rolls[3].Item2));
        p.AddTrait(presenceTraits["Administratum Presence"], RNG.DiceRoll(rolls[4].Item1, rolls[4].Item2));
        p.AddTrait(presenceTraits["Adeptus Ministorum Presence"], RNG.DiceRoll(rolls[5].Item1, rolls[5].Item2));
        p.AddTrait(presenceTraits["Inquisition Presence"], RNG.DiceRoll(rolls[6].Item1, rolls[6].Item2));
        // ADEPTA PRESENCE
        // 01-03 None.
        // 04-06 Token. For administrative purposes only.
        // 07-09 Slight. Specific duties; not involved in wider planetary affairs.
        // 10-12 Small. Involved, but quietly and unobtrusively.
        // 13-15 Moderate. Has offices and planetary duties, and are widely known.
        // 16-18 Notable. A powerful force in its own area of expertise.
        // 19-21 Significant. Controls its field, and has a say in wider planetary matters.
        // 22-24 Major. A powerful and influential force throughout the planet.
        // 25+ Dominating: One of, if not the, most powerful and influential forces on the planet.
      }

      void addDayLength(Planet p)
      {
        Trait t = TraitUtil.getTraitsByType(p.Traits, "planet size").Keys.First();
        var (count, dice, multiplier) = getFromRange(planetDayLength);
        p.DayLength = planetDayLengthMap[t] + RNG.DiceRoll(count, dice) * multiplier;
      }

      void addYearLength(Planet p)
      {
        p.YearLength += RNG.DiceRoll(10, 10) * (RNG.Integer(11, 109) / 10);
      }


      // Create Stars
      foreach (int i in Enumerable.Range(1, 6))
      {
        var size = getFromRange(rollTables[RollTable.StarSize]);
        var s = new Star()
        {
          location = new Vector2((float)(RNG.Integer(-80, 80) / 10), (float)(RNG.Integer(-90, 90) / 10)),
          rotationSpeed = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(RNG.Integer(-5, 5) / 20f), MathHelper.ToRadians(RNG.Integer(-5, 5) / 20f), 0),
          rotation = Quaternion.Identity,
          size = starSizeMap[size],
          color = new Color(RNG.Integer(0, 255), RNG.Integer(0, 255), RNG.Integer(0, 255)),
        };
        cosmos.Stars.AutoAdd(s);
      }

      // Create Planets
      foreach (int i in Enumerable.Range(1, 32))
      {
        var p = new Planet()
        {
          star = RNG.PickFrom(cosmos.Stars.Values),
          rotationSpeed = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(RNG.Integer(-5, 5) / 20f), MathHelper.ToRadians(RNG.Integer(-5, 5) / 20f), 0),
          rotation = Quaternion.Identity,
          size = RNG.Integer(10, 25) / 10f,
          color = new Color(RNG.Integer(0, 255), RNG.Integer(0, 255), RNG.Integer(0, 255)),
        };

        addPlanetType(p);
        addTechLevel(p);
        addAdeptus(p);
        addDayLength(p);
        addYearLength(p);

        cosmos.Planets.AutoAdd(p);
      }
    }
  }
}