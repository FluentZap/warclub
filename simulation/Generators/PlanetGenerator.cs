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
      PlanetGravity,
      PlanetAtmosphere,
      PlanetHydrosphere,
      PlanetTemperature,
      PlanetTerrainType,
      StarSize,
    }

    Dictionary<RollTable, Dictionary<int, Trait>> GetRollTables(Dictionary<string, TraitList> traitLists)
    {
      var wt = TraitLists["world type"];
      var tl = TraitLists["tech level"];
      var ss = TraitLists["star size"];
      var ps = TraitLists["planet size"];
      var pt = TraitLists["planet tilt"];
      var pg = TraitLists["planet gravity"];
      var pa = TraitLists["atmosphere"];
      var ph = TraitLists["hydrosphere"];
      var temp = TraitLists["temperature"];
      var terrain = TraitLists["planet terrain"];
      var presenceTraits = TraitLists["presence"];

      return new Dictionary<RollTable, Dictionary<int, Trait>>
      {
        [RollTable.PlanetType] = new Dictionary<int, Trait>
        {
          [20] = wt["hive world"],
          [28] = wt["agri world"],
          [33] = wt["forge world"],
          [41] = wt["mining world"],
          [49] = wt["developing world"],
          [53] = wt["feudal world"],
          [58] = wt["feral world"],
          [64] = wt["shrine world"],
          [68] = wt["cemetery world"],
          [73] = wt["pleasure world"],
          [76] = wt["quarantined world"],
          [82] = wt["war world"],
          [84] = wt["dead world"],
          [87] = wt["death world"],
          [92] = wt["frontier world"],
          [95] = wt["forbidden world"],
          [99] = wt["xenos world"],
        },
        [RollTable.PlanetTech] = new Dictionary<int, Trait>
        {
          [05] = tl["stone age"],
          [10] = tl["iron age"],
          [15] = tl["steel age"],
          [20] = tl["pre industrial"],
          [25] = tl["industrial"],
          [30] = tl["early space"],
          [35] = tl["advanced space"],
          [40] = tl["warp space"],
          [45] = tl["low imperial"],
          [50] = tl["mid imperial"],
          [55] = tl["high imperial"],
          [60] = tl["advanced"],
        },
        [RollTable.StarSize] = new Dictionary<int, Trait>
        {
          [5] = ss["tiny star"],
          [15] = ss["small star"],
          [75] = ss["medium star"],
          [85] = ss["large star"],
          [95] = ss["huge star"],
          [100] = ss["giant star"],
        },
        [RollTable.PlanetSize] = new Dictionary<int, Trait>
        {
          [10] = ps["miniscule size"],
          [20] = ps["tiny size"],
          [35] = ps["small size"],
          [75] = ps["average size"],
          [85] = ps["large size"],
          [90] = ps["huge size"],
          [95] = ps["enormous size"],
          [100] = ps["massive size"],
        },
        [RollTable.PlanetTilt] = new Dictionary<int, Trait>
        {
          [5] = pt["no tilt"],
          [15] = pt["slight tilt"],
          [30] = pt["notable tilt"],
          [70] = pt["moderate tilt"],
          [85] = pt["large tilt"],
          [95] = pt["severe tilt"],
          [100] = pt["extreme tilt"],
        },
        [RollTable.PlanetGravity] = new Dictionary<int, Trait>
        {
          [5] = pg["very light gravity"],
          [15] = pg["light gravity"],
          [90] = pg["standard gravity"],
          [95] = pg["heavy gravity"],
          [100] = pg["very heavy gravity"],
        },
        [RollTable.PlanetAtmosphere] = new Dictionary<int, Trait>
        {
          [70] = pa["normal atmosphere"],
          [85] = pa["bearable atmosphere"],
          [92] = pa["tainted atmosphere"],
          [97] = pa["poisonous atmosphere"],
          [100] = pa["deadly atmosphere"],
        },
        [RollTable.PlanetHydrosphere] = new Dictionary<int, Trait>
        {
          [10] = ph["waterless hydrosphere"],
          [20] = ph["parched hydrosphere"],
          [35] = ph["arid hydrosphere"],
          [55] = ph["average hydrosphere"],
          [70] = ph["damp hydrosphere"],
          [80] = ph["moist hydrosphere"],
          [90] = ph["watery hydrosphere"],
          [100] = ph["aquatic hydrosphere"],
        },
        [RollTable.PlanetTemperature] = new Dictionary<int, Trait>
        {
          [5] = temp["bitter temperature"],
          [10] = temp["cold temperature"],
          [20] = temp["chilly temperature"],
          [35] = temp["frosty temperature"],
          [60] = temp["average temperature"],
          [75] = temp["warm temperature"],
          [85] = temp["tepid temperature"],
          [90] = temp["hot temperature"],
          [95] = temp["roasting temperature"],
          [100] = temp["searing temperature"],
        },
        [RollTable.PlanetTerrainType] = new Dictionary<int, Trait>
        {
          [5] = terrain["grassland terrain"],
          [10] = terrain["savannah terrain"],
          [15] = terrain["continual forest terrain"],
          [20] = terrain["broken forest terrain"],
          [25] = terrain["hills terrain"],
          [30] = terrain["mountains terrain"],
          [35] = terrain["plateaus terrain"],
          [40] = terrain["dormant volcanoes terrain"],
          [45] = terrain["active volcanoes terrain"],
          [50] = terrain["broken rock terrain"],
          [55] = terrain["flat rock terrain"],
          [60] = terrain["columns terrain"],
          [65] = terrain["moor terrain"],
          [70] = terrain["barren terrain"],
          [75] = terrain["swamp terrain"],
          [80] = terrain["caves terrain"],
          [85] = terrain["ravines terrain"],
          [90] = terrain["sandy terrain"],
          [95] = terrain["islands terrain"],
          [100] = terrain["cliffs terrain"],
        },
      };
    }



    public void GeneratePlanes()
    {

      var rollTables = GetRollTables(TraitLists);
      var wt = TraitLists["world type"];
      var ps = TraitLists["planet size"];
      var planetTilt = TraitLists["planet tilt"];
      var ss = TraitLists["star size"];
      var presenceTraits = TraitLists["presence"];

      var starSizeMap = new Dictionary<Trait, float>
      {
        [ss["tiny star"]] = 0.5f,
        [ss["small star"]] = 0.75f,
        [ss["medium star"]] = 1.0f,
        [ss["large star"]] = 1.25f,
        [ss["huge star"]] = 1.5f,
        [ss["giant star"]] = 1.75f,
      };

      var planetTiltMap = new Dictionary<Trait, (int, int)>
      {
        [planetTilt["no tilt"]] = (0, 0),
        [planetTilt["slight tilt"]] = (1, 5),
        [planetTilt["notable tilt"]] = (6, 15),
        [planetTilt["moderate tilt"]] = (16, 25),
        [planetTilt["large tilt"]] = (26, 35),
        [planetTilt["severe tilt"]] = (36, 45),
        [planetTilt["extreme tilt"]] = (46, 70),
      };

      var planetSizeMap = new Dictionary<Trait, float>
      {
        [ps["miniscule size"]] = 0.2f,
        [ps["tiny size"]] = 0.4f,
        [ps["small size"]] = 0.6f,
        [ps["average size"]] = 0.8f,
        [ps["large size"]] = 1.0f,
        [ps["huge size"]] = 1.2f,
        [ps["enormous size"]] = 1.4f,
        [ps["massive size"]] = 1.6f,
      };

      var satelliteSizeMap = new Dictionary<Trait, int>
      {
        [ps["miniscule size"]] = -30,
        [ps["tiny size"]] = -20,
        [ps["small size"]] = -10,
        [ps["average size"]] = 0,
        [ps["large size"]] = 10,
        [ps["huge size"]] = 20,
        [ps["enormous size"]] = 30,
        [ps["massive size"]] = 50,
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
        [ps["miniscule size"]] = -30,
        [ps["tiny size"]] = -20,
        [ps["small size"]] = -10,
        [ps["average size"]] = 0,
        [ps["large size"]] = 10,
        [ps["huge size"]] = 20,
        [ps["enormous size"]] = 30,
        [ps["massive size"]] = 50,
      };

      var adeptusList = new Dictionary<Trait, (int, int)[]>
      {
        [wt["hive world"]] = new (int, int)[7] { (3, 10), (3, 10), (1, 10), (2, 10), (4, 10), (3, 10), (3, 10) },
        [wt["agri world"]] = new (int, int)[7] { (1, 10), (1, 10), (1, 5), (1, 10), (2, 10), (2, 10), (1, 5) },
        [wt["forge world"]] = new (int, int)[7] { (1, 10), (1, 10), (1, 5), (5, 10), (2, 10), (1, 5), (1, 5) },
        [wt["mining world"]] = new (int, int)[7] { (1, 10), (1, 10), (1, 5), (3, 10), (2, 10), (2, 10), (1, 5) },
        [wt["developing world"]] = new (int, int)[7] { (0, 0), (1, 5), (0, 0), (1, 10), (1, 10), (1, 10), (1, 5) },
        [wt["feudal world"]] = new (int, int)[7] { (0, 0), (1, 5), (0, 0), (0, 0), (1, 5), (1, 5), (1, 5) },
        [wt["feral world"]] = new (int, int)[7] { (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0) },
        [wt["shrine world"]] = new (int, int)[7] { (1, 10), (1, 5), (1, 5), (1, 5), (2, 10), (4, 10), (1, 5) },
        [wt["cemetery world"]] = new (int, int)[7] { (1, 5), (1, 5), (1, 5), (1, 5), (1, 10), (3, 10), (1, 5) },
        [wt["pleasure world"]] = new (int, int)[7] { (2, 10), (2, 10), (1, 5), (2, 10), (2, 10), (2, 10), (1, 5) },
        [wt["death world"]] = new (int, int)[7] { (1, 5), (1, 5), (1, 5), (1, 5), (1, 10), (1, 10), (1, 5) },
        [wt["frontier world"]] = new (int, int)[7] { (0, 0), (0, 5), (0, 0), (1, 5), (1, 5), (1, 5), (1, 5) },
        [wt["forbidden world"]] = new (int, int)[7] { (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0) },
        [wt["xenos world"]] = new (int, int)[7] { (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0) },
      };

      var satelliteMap = new Dictionary<int, (int, int)>
      {
        [20] = (0, 0),
        [40] = (1, 1),
        [70] = (1, 5),
        [80] = (1, 10),
        [90] = (2, 10),
        [100] = (3, 10),
        [110] = (4, 10),
        [130] = (5, 10),
        [150] = (6, 10),
      };

      var techLevelMap = new Dictionary<Trait, (int, int, int)>
      {
        [wt["hive world"]] = (36, 2, 10),
        [wt["agri world"]] = (15, 3, 10),
        [wt["forge world"]] = (50, 1, 10),
        [wt["mining world"]] = (15, 4, 10),
        [wt["developing world"]] = (20, 2, 10),
        [wt["feudal world"]] = (5, 1, 10),
        [wt["feral world"]] = (0, 1, 10),
        [wt["shrine world"]] = (20, 4, 10),
        [wt["cemetery world"]] = (20, 3, 10),
        [wt["pleasure world"]] = (35, 2, 10),
        [wt["death world"]] = (0, 4, 10),
        [wt["frontier world"]] = (35, 2, 10),
        [wt["forbidden world"]] = (0, 6, 10),
        [wt["xenos world"]] = (0, 6, 10),
      };

      var destroyedWorlds = new HashSet<Trait> { wt["quarantined world"], wt["war world"], wt["dead world"] };

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
        Trait t = TraitUtil.getTraitsByType(p.Traits, "world type").Keys.First();
        var (mod, count, dice) = techLevelMap[t];
        p.AddTrait(getFromRange(rollTables[RollTable.PlanetTech], mod + RNG.DiceRoll(count, dice)));
      }

      void addAdeptus(Planet p)
      {
        Trait t = TraitUtil.getTraitsByType(p.Traits, "world type").Keys.First();
        var rolls = adeptusList[t];
        p.AddTrait(presenceTraits["adeptus arbites presence"], RNG.DiceRoll(rolls[0].Item1, rolls[0].Item2));
        p.AddTrait(presenceTraits["adeptus astra telepathica presence"], RNG.DiceRoll(rolls[1].Item1, rolls[1].Item2));
        p.AddTrait(presenceTraits["adeptus astronimica presence"], RNG.DiceRoll(rolls[2].Item1, rolls[2].Item2));
        p.AddTrait(presenceTraits["adeptus mechanicus presence"], RNG.DiceRoll(rolls[3].Item1, rolls[3].Item2));
        p.AddTrait(presenceTraits["administratum presence"], RNG.DiceRoll(rolls[4].Item1, rolls[4].Item2));
        p.AddTrait(presenceTraits["adeptus ministorum presence"], RNG.DiceRoll(rolls[5].Item1, rolls[5].Item2));
        p.AddTrait(presenceTraits["inquisition presence"], RNG.DiceRoll(rolls[6].Item1, rolls[6].Item2));
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

      void addTilt(Planet p)
      {
        var tilt = getFromRange(rollTables[RollTable.PlanetTilt]);
        p.AddTrait(tilt);

        Trait t = TraitUtil.getTraitsByType(p.Traits, "planet tilt").Keys.First();
        var (min, max) = planetTiltMap[t];
        p.rotation = Quaternion.CreateFromYawPitchRoll(0, 0, MathHelper.ToRadians(RNG.Integer(min, max)));
      }

      void addDayLength(Planet p)
      {
        Trait t = TraitUtil.getTraitsByType(p.Traits, "planet size").Keys.First();
        var (count, dice, multiplier) = getFromRange(planetDayLength);
        p.DayLength = planetDayLengthMap[t] + RNG.DiceRoll(count, dice) * multiplier;
        p.rotationSpeed = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(180f / p.DayLength), 0, 0);
      }

      void addYearLength(Planet p)
      {
        p.YearLength += RNG.DiceRoll(10, 10) * (RNG.Integer(11, 109) / 10);
        p.rotationSpeed = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(RNG.Integer(-5, 5) / 20f), MathHelper.ToRadians(RNG.Integer(-5, 5) / 20f), 0);
      }

      void addSatellites(Planet p)
      {
        Trait t = TraitUtil.getTraitsByType(p.Traits, "planet size").Keys.First();
        var mod = satelliteSizeMap[t];
        var (count, dice) = getFromRange(satelliteMap, mod + RNG.Integer(100));
        foreach (int i in Enumerable.Range(0, RNG.DiceRoll(count, dice)))
        {
          Satellite satellite = new Satellite();
          p.AddRelation(satellite, RelationType.Created, 255);
        }
      }

      void addRollTableTrait(Planet p, RollTable r)
      {
        p.AddTrait(getFromRange(rollTables[r]));
      }

      void addTerrain(Planet p)
      {
        foreach (int i in Enumerable.Range(0, RNG.Integer(5)))
          p.AddTrait(getFromRange(rollTables[RollTable.PlanetTerrainType]));
      }


      // Create Sectors
      foreach (int i in Enumerable.Range(0, 8))
      {
        var s = new Sector()
        {
          location = new Vector2(0),
        };
        cosmos.Sectors.AutoAdd(s);
      }

      // Create Planets
      foreach (uint i in Enumerable.Range(0, 32))
      {
        var p = new Planet()
        {
          sector = cosmos.Sectors[i / 4],
          size = RNG.Integer(10, 25) / 10f,
          color = new Color(RNG.Integer(0, 255), RNG.Integer(0, 255), RNG.Integer(0, 255)),
        };

        addPlanetType(p);
        addTechLevel(p);
        addAdeptus(p);
        addTilt(p);

        addRollTableTrait(p, RollTable.PlanetSize);
        addRollTableTrait(p, RollTable.StarSize);

        // need size for day length
        addDayLength(p);
        addYearLength(p);


        addRollTableTrait(p, RollTable.PlanetGravity);
        addRollTableTrait(p, RollTable.PlanetAtmosphere);
        addRollTableTrait(p, RollTable.PlanetHydrosphere);
        addRollTableTrait(p, RollTable.PlanetTemperature);

        addSatellites(p);
        addTerrain(p);

        cosmos.Planets.AutoAdd(p);
      }
    }
  }
}