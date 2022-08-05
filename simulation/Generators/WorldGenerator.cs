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
      WorldType,
      WorldTypeLimited,
      WorldTech,
      WorldTilt,
      WorldSize,
      WorldGravity,
      WorldAtmosphere,
      WorldHydrosphere,
      WorldTemperature,
      WorldTerrainType,
      WorldSocietyType,
      StarSize,
    }

    Dictionary<RollTable, Dictionary<int, Trait>> GetRollTables(Dictionary<string, TraitList> traitLists)
    {
      return new Dictionary<RollTable, Dictionary<int, Trait>>
      {
        [RollTable.WorldType] = new Dictionary<int, Trait>
        {
          [20] = Traits["hive world"],
          [28] = Traits["agri world"],
          [33] = Traits["forge world"],
          [41] = Traits["mining world"],
          [49] = Traits["developing world"],
          [53] = Traits["feudal world"],
          [58] = Traits["feral world"],
          [64] = Traits["shrine world"],
          [68] = Traits["cemetery world"],
          [73] = Traits["pleasure world"],
          [76] = Traits["quarantined world"],
          [82] = Traits["war world"],
          [84] = Traits["dead world"],
          [87] = Traits["death world"],
          [92] = Traits["frontier world"],
          [95] = Traits["forbidden world"],
          [100] = Traits["xenos world"],
        },
        [RollTable.WorldTypeLimited] = new Dictionary<int, Trait>
        {
          [20] = Traits["hive world"],
          [28] = Traits["agri world"],
          [33] = Traits["forge world"],
          [39] = Traits["mining world"],
          [48] = Traits["developing world"],
          [57] = Traits["feudal world"],
          [62] = Traits["feral world"],
          [68] = Traits["shrine world"],
          [75] = Traits["cemetery world"],
          [80] = Traits["pleasure world"],
          [86] = Traits["death world"],
          [92] = Traits["frontier world"],
          [95] = Traits["forbidden world"],
          [100] = Traits["xenos world"],
        },
        [RollTable.WorldTech] = new Dictionary<int, Trait>
        {
          [05] = Traits["stone age"],
          [10] = Traits["iron age"],
          [15] = Traits["steel age"],
          [20] = Traits["pre industrial"],
          [25] = Traits["industrial"],
          [30] = Traits["early space"],
          [35] = Traits["advanced space"],
          [40] = Traits["warp space"],
          [45] = Traits["low imperial"],
          [50] = Traits["mid imperial"],
          [55] = Traits["high imperial"],
          [100] = Traits["advanced"],
        },
        [RollTable.StarSize] = new Dictionary<int, Trait>
        {
          [5] = Traits["tiny star"],
          [15] = Traits["small star"],
          [75] = Traits["medium star"],
          [85] = Traits["large star"],
          [95] = Traits["huge star"],
          [100] = Traits["giant star"],
        },
        [RollTable.WorldSize] = new Dictionary<int, Trait>
        {
          [10] = Traits["miniscule size"],
          [20] = Traits["tiny size"],
          [35] = Traits["small size"],
          [75] = Traits["average size"],
          [85] = Traits["large size"],
          [90] = Traits["huge size"],
          [95] = Traits["enormous size"],
          [100] = Traits["massive size"],
        },
        [RollTable.WorldTilt] = new Dictionary<int, Trait>
        {
          [5] = Traits["no tilt"],
          [15] = Traits["slight tilt"],
          [30] = Traits["notable tilt"],
          [70] = Traits["moderate tilt"],
          [85] = Traits["large tilt"],
          [95] = Traits["severe tilt"],
          [100] = Traits["extreme tilt"],
        },
        [RollTable.WorldGravity] = new Dictionary<int, Trait>
        {
          [5] = Traits["very light gravity"],
          [15] = Traits["light gravity"],
          [90] = Traits["standard gravity"],
          [95] = Traits["heavy gravity"],
          [100] = Traits["very heavy gravity"],
        },
        [RollTable.WorldAtmosphere] = new Dictionary<int, Trait>
        {
          [70] = Traits["normal atmosphere"],
          [85] = Traits["bearable atmosphere"],
          [92] = Traits["tainted atmosphere"],
          [97] = Traits["poisonous atmosphere"],
          [100] = Traits["deadly atmosphere"],
        },
        [RollTable.WorldHydrosphere] = new Dictionary<int, Trait>
        {
          [10] = Traits["waterless hydrosphere"],
          [20] = Traits["parched hydrosphere"],
          [35] = Traits["arid hydrosphere"],
          [55] = Traits["average hydrosphere"],
          [70] = Traits["damp hydrosphere"],
          [80] = Traits["moist hydrosphere"],
          [90] = Traits["watery hydrosphere"],
          [100] = Traits["aquatic hydrosphere"],
        },
        [RollTable.WorldTemperature] = new Dictionary<int, Trait>
        {
          [5] = Traits["bitter temperature"],
          [10] = Traits["cold temperature"],
          [20] = Traits["chilly temperature"],
          [35] = Traits["frosty temperature"],
          [60] = Traits["average temperature"],
          [75] = Traits["warm temperature"],
          [85] = Traits["tepid temperature"],
          [90] = Traits["hot temperature"],
          [95] = Traits["roasting temperature"],
          [100] = Traits["searing temperature"],
        },
        [RollTable.WorldTerrainType] = new Dictionary<int, Trait>
        {
          [5] = Traits["grassland terrain"],
          [10] = Traits["savannah terrain"],
          [15] = Traits["continual forest terrain"],
          [20] = Traits["broken forest terrain"],
          [25] = Traits["hills terrain"],
          [30] = Traits["mountains terrain"],
          [35] = Traits["plateaus terrain"],
          [40] = Traits["dormant volcanoes terrain"],
          [45] = Traits["active volcanoes terrain"],
          [50] = Traits["broken rock terrain"],
          [55] = Traits["flat rock terrain"],
          [60] = Traits["columns terrain"],
          [65] = Traits["moor terrain"],
          [70] = Traits["barren terrain"],
          [75] = Traits["swamp terrain"],
          [80] = Traits["caves terrain"],
          [85] = Traits["ravines terrain"],
          [90] = Traits["sandy terrain"],
          [95] = Traits["islands terrain"],
          [100] = Traits["cliffs terrain"],
        },
        [RollTable.WorldSocietyType] = new Dictionary<int, Trait>
        {
          [10] = Traits["democracy"],
          [20] = Traits["elected dictator"],
          [30] = Traits["hereditary dictator"],
          [40] = Traits["tyrannical dictator"],
          [50] = Traits["elected monarchy"],
          [60] = Traits["hereditary monarchy"],
          [70] = Traits["religious local"],
          [80] = Traits["religious ministorum"],
          [90] = Traits["religious machine god"],
          [100] = Traits["oligarchy"],
        },
      };
    }



    public void GenerateWorlds()
    {

      var rollTables = GetRollTables(TraitLists);
      var starSizeMap = new Dictionary<Trait, float>
      {
        [Traits["tiny star"]] = 0.5f,
        [Traits["small star"]] = 0.75f,
        [Traits["medium star"]] = 1.0f,
        [Traits["large star"]] = 1.25f,
        [Traits["huge star"]] = 1.5f,
        [Traits["giant star"]] = 1.75f,
      };

      var worldTiltMap = new Dictionary<Trait, (int, int)>
      {
        [Traits["no tilt"]] = (0, 0),
        [Traits["slight tilt"]] = (1, 5),
        [Traits["notable tilt"]] = (6, 15),
        [Traits["moderate tilt"]] = (16, 25),
        [Traits["large tilt"]] = (26, 35),
        [Traits["severe tilt"]] = (36, 45),
        [Traits["extreme tilt"]] = (46, 70),
      };

      var worldSizeMap = new Dictionary<Trait, float>
      {
        [Traits["miniscule size"]] = 0.2f,
        [Traits["tiny size"]] = 0.4f,
        [Traits["small size"]] = 0.6f,
        [Traits["average size"]] = 0.8f,
        [Traits["large size"]] = 1.0f,
        [Traits["huge size"]] = 1.2f,
        [Traits["enormous size"]] = 1.4f,
        [Traits["massive size"]] = 1.6f,
      };

      var satelliteSizeMap = new Dictionary<Trait, int>
      {
        [Traits["miniscule size"]] = -30,
        [Traits["tiny size"]] = -20,
        [Traits["small size"]] = -10,
        [Traits["average size"]] = 0,
        [Traits["large size"]] = 10,
        [Traits["huge size"]] = 20,
        [Traits["enormous size"]] = 30,
        [Traits["massive size"]] = 50,
      };

      var worldDayLength = new Dictionary<int, (int, int, int)>
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

      var worldDayLengthMap = new Dictionary<Trait, int>
      {
        [Traits["miniscule size"]] = -30,
        [Traits["tiny size"]] = -20,
        [Traits["small size"]] = -10,
        [Traits["average size"]] = 0,
        [Traits["large size"]] = 10,
        [Traits["huge size"]] = 20,
        [Traits["enormous size"]] = 30,
        [Traits["massive size"]] = 50,
      };

      var adeptusList = new Dictionary<Trait, (int, int)[]>
      {
        [Traits["hive world"]] = new (int, int)[7] { (3, 10), (3, 10), (1, 10), (2, 10), (4, 10), (3, 10), (3, 10) },
        [Traits["agri world"]] = new (int, int)[7] { (1, 10), (1, 10), (1, 5), (1, 10), (2, 10), (2, 10), (1, 5) },
        [Traits["forge world"]] = new (int, int)[7] { (1, 10), (1, 10), (1, 5), (5, 10), (2, 10), (1, 5), (1, 5) },
        [Traits["mining world"]] = new (int, int)[7] { (1, 10), (1, 10), (1, 5), (3, 10), (2, 10), (2, 10), (1, 5) },
        [Traits["developing world"]] = new (int, int)[7] { (0, 0), (1, 5), (0, 0), (1, 10), (1, 10), (1, 10), (1, 5) },
        [Traits["feudal world"]] = new (int, int)[7] { (0, 0), (1, 5), (0, 0), (0, 0), (1, 5), (1, 5), (1, 5) },
        [Traits["feral world"]] = new (int, int)[7] { (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0) },
        [Traits["shrine world"]] = new (int, int)[7] { (1, 10), (1, 5), (1, 5), (1, 5), (2, 10), (4, 10), (1, 5) },
        [Traits["cemetery world"]] = new (int, int)[7] { (1, 5), (1, 5), (1, 5), (1, 5), (1, 10), (3, 10), (1, 5) },
        [Traits["pleasure world"]] = new (int, int)[7] { (2, 10), (2, 10), (1, 5), (2, 10), (2, 10), (2, 10), (1, 5) },
        [Traits["death world"]] = new (int, int)[7] { (1, 5), (1, 5), (1, 5), (1, 5), (1, 10), (1, 10), (1, 5) },
        [Traits["frontier world"]] = new (int, int)[7] { (0, 0), (0, 5), (0, 0), (1, 5), (1, 5), (1, 5), (1, 5) },
        [Traits["forbidden world"]] = new (int, int)[7] { (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0) },
        [Traits["xenos world"]] = new (int, int)[7] { (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0) },
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
        [Traits["hive world"]] = (36, 2, 10),
        [Traits["agri world"]] = (15, 3, 10),
        [Traits["forge world"]] = (50, 1, 10),
        [Traits["mining world"]] = (15, 4, 10),
        [Traits["developing world"]] = (20, 2, 10),
        [Traits["feudal world"]] = (5, 1, 10),
        [Traits["feral world"]] = (0, 1, 10),
        [Traits["shrine world"]] = (20, 4, 10),
        [Traits["cemetery world"]] = (20, 3, 10),
        [Traits["pleasure world"]] = (35, 2, 10),
        [Traits["death world"]] = (0, 4, 10),
        [Traits["frontier world"]] = (35, 2, 10),
        [Traits["forbidden world"]] = (0, 6, 10),
        [Traits["xenos world"]] = (0, 6, 10),
      };

      var populationTypeModMap = new Dictionary<Trait, int>
      {
        [Traits["hive world"]] = 40,
        [Traits["agri world"]] = 0,
        [Traits["forge world"]] = 20,
        [Traits["mining world"]] = 10,
        [Traits["developing world"]] = -20,
        [Traits["feudal world"]] = -20,
        [Traits["feral world"]] = 0,
        [Traits["shrine world"]] = -10,
        [Traits["cemetery world"]] = -20,
        [Traits["pleasure world"]] = 0,
        [Traits["quarantined world"]] = 0,
        [Traits["war world"]] = 0,
        [Traits["dead world"]] = -40,
        [Traits["death world"]] = -30,
        [Traits["frontier world"]] = -20,
        [Traits["forbidden world"]] = 0,
        [Traits["xenos world"]] = 0,
      };

      var populationSizeModMap = new Dictionary<Trait, int>
      {
        [Traits["miniscule size"]] = -30,
        [Traits["tiny size"]] = -20,
        [Traits["small size"]] = -10,
        [Traits["average size"]] = 0,
        [Traits["large size"]] = 10,
        [Traits["huge size"]] = 20,
        [Traits["enormous size"]] = 30,
        [Traits["massive size"]] = 40,
      };

      var populationMap = new Dictionary<int, (Trait, int, int, int)>
      {
        [5] = (Traits["population"], 10, 10, 1),
        [10] = (Traits["population"], 10, 10, 10),
        [15] = (Traits["population"], 10, 10, 100),
        [20] = (Traits["population"], 10, 10, 1000),
        [25] = (Traits["population"], 10, 10, 10000),
        [30] = (Traits["million population"], 1, 5, 1),
        [35] = (Traits["million population"], 1, 10, 1),
        [40] = (Traits["million population"], 5, 10, 1),
        [50] = (Traits["million population"], 10, 10, 1),
        [70] = (Traits["million population"], 1, 10, 100),
        [90] = (Traits["million population"], 1, 5, 1000),
        [100] = (Traits["million population"], 1, 10, 1000),
        [110] = (Traits["million population"], 2, 10, 1000),
        [1000] = (Traits["million population"], 3, 10, 1000),
      };

      var defensesMap = new Dictionary<Trait, (int, int, int)[]>
      {
        [Traits["hive world"]] = new (int, int, int)[12] { (99, 3, 2), (99, 3, 2), (99, 3, 3), (99, 3, 3), (30, 2, 2), (30, 2, 2), (99, 3, 3), (50, 2, 2), (85, 3, 2), (70, 2, 2), (30, 3, 2), (10, 3, 2) },
        [Traits["agri world"]] = new (int, int, int)[12] { (90, 1, 1), (75, 1, 1), (50, 1, 2), (5, 1, 1), (1, 1, 2), (30, 1, 2), (25, 1, 2), (5, 1, 1), (5, 1, 1), (5, 1, 1), (15, 1, 1), (30, 2, 2) },
        [Traits["forge world"]] = new (int, int, int)[12] { (50, 2, 2), (20, 1, 2), (60, 2, 2), (90, 3, 3), (70, 3, 3), (5, 1, 2), (90, 3, 3), (80, 2, 3), (90, 2, 3), (90, 3, 3), (90, 3, 3), (1, 1, 2) },
        [Traits["mining world"]] = new (int, int, int)[12] { (95, 2, 2), (60, 3, 1), (5, 2, 2), (1, 1, 1), (1, 1, 1), (20, 2, 2), (5, 1, 2), (15, 1, 2), (1, 1, 1), (1, 1, 1), (30, 1, 1), (10, 1, 1) },
        [Traits["developing world"]] = new (int, int, int)[12] { (90, 3, 2), (90, 2, 2), (90, 3, 2), (50, 2, 3), (1, 1, 1), (15, 2, 2), (15, 1, 2), (10, 1, 1), (75, 3, 2), (65, 2, 1), (40, 1, 1), (40, 3, 2) },
        [Traits["feudal world"]] = new (int, int, int)[12] { (75, 2, 2), (99, 2, 2), (99, 3, 2), (0, 0, 0), (0, 0, 0), (90, 2, 2), (0, 0, 0), (0, 0, 0), (0, 0, 0), (0, 0, 0), (0, 0, 0), (85, 2, 2) },
        [Traits["feral world"]] = new (int, int, int)[12] { (20, 1, 2), (99, 3, 2), (90, 3, 2), (0, 0, 0), (0, 0, 0), (99, 2, 2), (0, 0, 0), (0, 0, 0), (0, 0, 0), (0, 0, 0), (0, 0, 0), (0, 0, 0) },
        [Traits["shrine world"]] = new (int, int, int)[12] { (60, 2, 3), (20, 2, 2), (40, 1, 2), (1, 1, 2), (1, 1, 2), (1, 1, 2), (10, 1, 2), (1, 1, 2), (1, 1, 1), (1, 1, 1), (1, 1, 1), (0, 0, 0) },
        [Traits["cemetery world"]] = new (int, int, int)[12] { (10, 2, 2), (00, 0, 0), (10, 1, 2), (0, 0, 0), (0, 0, 0), (10, 1, 2), (0, 0, 0), (0, 0, 0), (0, 0, 0), (0, 0, 0), (0, 0, 0), (0, 0, 0) },
        [Traits["pleasure world"]] = new (int, int, int)[12] { (90, 3, 2), (10, 2, 2), (40, 2, 2), (5, 1, 2), (0, 0, 0), (30, 3, 2), (5, 2, 2), (1, 1, 2), (1, 1, 2), (1, 1, 2), (15, 2, 2), (30, 2, 2) },
        [Traits["frontier world"]] = new (int, int, int)[12] { (50, 2, 2), (30, 2, 2), (5, 1, 2), (1, 1, 1), (0, 0, 0), (20, 1, 1), (1, 1, 1), (0, 0, 0), (0, 0, 0), (0, 0, 0), (0, 0, 0), (20, 1, 2) },
      };

      var defensesQualityMap = new Dictionary<int, int>
      {
        [4] = 1,
        [9] = 2,
        [15] = 3,
        [20] = 4,
        [100] = 5,
      };

      var defensesTypeMap = new List<string>
      {
        {"enforcers lv"},
        {"militia lv"},
        {"standing army lv"},
        {"armoured force lv"},
        {"titan force lv"},
        {"private army lv"},
        {"naval force lv"},
        {"orbital station lv"},
        {"planetary missile silos lv"},
        {"orbital missile silos lv"},
        {"defense lasers lv"},
        {"mercenary force lv"},
      };

      var destroyedWorlds = new HashSet<Trait> { Traits["quarantined world"], Traits["war world"], Traits["dead world"] };

      void addPlanetType(World p, Trait typeOverride)
      {
        var t = typeOverride == null ? getFromRange(rollTables[RollTable.WorldType]) : typeOverride;
        p.AddTrait(t);
        if (destroyedWorlds.Contains(t))
          p.AddTrait(getFromRange(rollTables[RollTable.WorldTypeLimited]));
      }

      void addTechLevel(World p)
      {
        Trait t = TraitUtil.getTraitsByType(p.Traits, "world type").Keys.First();
        var (mod, count, dice) = techLevelMap[t];
        p.AddTrait(getFromRange(rollTables[RollTable.WorldTech], mod + RNG.DiceRoll(count, dice)));
      }

      void addAdeptus(World p)
      {
        Trait t = TraitUtil.getTraitsByType(p.Traits, "world type").Keys.First();
        var rolls = adeptusList[t];
        p.AddTrait(Traits["adeptus arbites presence"], RNG.DiceRoll(rolls[0].Item1, rolls[0].Item2));
        p.AddTrait(Traits["adeptus astra telepathica presence"], RNG.DiceRoll(rolls[1].Item1, rolls[1].Item2));
        p.AddTrait(Traits["adeptus astronimica presence"], RNG.DiceRoll(rolls[2].Item1, rolls[2].Item2));
        p.AddTrait(Traits["adeptus mechanicus presence"], RNG.DiceRoll(rolls[3].Item1, rolls[3].Item2));
        p.AddTrait(Traits["administratum presence"], RNG.DiceRoll(rolls[4].Item1, rolls[4].Item2));
        p.AddTrait(Traits["adeptus ministorum presence"], RNG.DiceRoll(rolls[5].Item1, rolls[5].Item2));
        p.AddTrait(Traits["inquisition presence"], RNG.DiceRoll(rolls[6].Item1, rolls[6].Item2));
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

      void addTilt(World p)
      {
        var tilt = getFromRange(rollTables[RollTable.WorldTilt]);
        p.AddTrait(tilt);

        Trait t = TraitUtil.getTraitsByType(p.Traits, "world tilt").Keys.First();
        var (min, max) = worldTiltMap[t];
        p.rotation = Quaternion.CreateFromYawPitchRoll(0, 0, MathHelper.ToRadians(RNG.Integer(min, max)));
      }

      void addDayLength(World p)
      {
        Trait t = TraitUtil.getTraitsByType(p.Traits, "world size").Keys.First();
        var (count, dice, multiplier) = getFromRange(worldDayLength);
        p.DayLength = worldDayLengthMap[t] + RNG.DiceRoll(count, dice) * multiplier;
        p.rotationSpeed = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(180f / p.DayLength), 0, 0);
      }

      void addYearLength(World p)
      {
        p.YearLength += RNG.DiceRoll(10, 10) * (RNG.Integer(11, 109) / 10);
        p.rotationSpeed = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(RNG.Integer(-5, 5) / 20f), MathHelper.ToRadians(RNG.Integer(-5, 5) / 20f), 0);
      }

      void addSatellites(World p)
      {
        Trait t = TraitUtil.getTraitsByType(p.Traits, "world size").Keys.First();
        var mod = satelliteSizeMap[t];
        var (count, dice) = getFromRange(satelliteMap, mod + RNG.Integer(100));
        foreach (int i in Enumerable.Range(0, RNG.DiceRoll(count, dice)))
        {
          Satellite satellite = new Satellite();
          // p.AddRelation(satellite, RelationType.Created, 255);
          addRollTableTrait(satellite, RollTable.WorldTerrainType);
          addRollTableTrait(satellite, RollTable.WorldAtmosphere);
          addRollTableTrait(satellite, RollTable.WorldHydrosphere);
          addRollTableTrait(satellite, RollTable.WorldTemperature);
        }
      }

      void addRollTableTrait(Entity e, RollTable r)
      {
        e.AddTrait(getFromRange(rollTables[r]));
      }

      void addTerrain(Entity e)
      {
        foreach (int i in Enumerable.Range(0, RNG.Integer(5)))
          e.AddTrait(getFromRange(rollTables[RollTable.WorldTerrainType]));
      }

      bool addPopulation(World p)
      {
        Trait type = TraitUtil.getTraitsByType(p.Traits, "world type").Keys.First();
        Trait size = TraitUtil.getTraitsByType(p.Traits, "world size").Keys.First();
        var roll = populationTypeModMap[type] + populationSizeModMap[size] + RNG.Integer(100);
        if (roll <= 0) return false;
        var (trait, count, dice, multiplier) = getFromRange(populationMap, roll);
        p.AddTrait(trait, RNG.DiceRoll(count, dice) * multiplier);
        return true;
      }

      void addDefenses(World p)
      {
        Trait t = TraitUtil.getTraitsByType(p.Traits, "world type").Keys.First();
        // No standard forces on planet
        if (!defensesMap.ContainsKey(t)) return;
        var rolls = defensesMap[t];

        foreach (int i in Enumerable.Range(0, defensesTypeMap.Count))
        {
          var (percentage, quality, size) = rolls[i];
          if (RNG.Integer(100) <= percentage)
          {
            var level = getFromRange(defensesQualityMap, RNG.DiceRoll(quality, 10));
            p.AddTrait(Traits[defensesTypeMap[i] + level], RNG.DiceRoll(size, 10));
          }
        }
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
        var p = new World()
        {
          size = RNG.Integer(10, 25) / 10f,
          color = new Color(RNG.Integer(0, 255), RNG.Integer(0, 255), RNG.Integer(0, 255)),
        };

        // if (i < TraitLists["world type"].Count)
        var worldTypes = rollTables[RollTable.WorldType].ToArray();
        var typeOverride = i < worldTypes.Length ? worldTypes[i].Value : null;

        addPlanetType(p, typeOverride);


        addTechLevel(p);
        addAdeptus(p);
        addTilt(p);

        addRollTableTrait(p, RollTable.WorldSize);
        addRollTableTrait(p, RollTable.StarSize);

        // need size for day length
        addDayLength(p);
        addYearLength(p);


        addRollTableTrait(p, RollTable.WorldGravity);
        addRollTableTrait(p, RollTable.WorldAtmosphere);
        addRollTableTrait(p, RollTable.WorldHydrosphere);
        addRollTableTrait(p, RollTable.WorldTemperature);

        addSatellites(p);
        addTerrain(p);
        var hasPopulation = addPopulation(p);

        if (hasPopulation)
        {
          addRollTableTrait(p, RollTable.WorldSocietyType);
          addDefenses(p);
        }

        cosmos.Worlds.AutoAdd(p);
      }

      // add worlds to sectors
      World[] worlds = cosmos.Worlds.Select(x => x.Value).ToArray();
      worlds = worlds.OrderBy(x => RNG.Integer(1000)).ToArray();
      foreach (uint i in Enumerable.Range(0, 32))
        worlds[i].sector = cosmos.Sectors[i / 4];

    }
  }
}