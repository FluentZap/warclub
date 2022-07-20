using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WarClub
{

  enum PlanetType
  {
    None,
    Hive_World,
    Agri_World,
    Forge_World,
    Mining_World,
    Developing_World,
    Feudal_World,
    Feral_World,
    Shrine_World,
    Cemetery_World,
    Pleasure_World,
    Quarantined_World,
    War_World,
    Dead_World,
    Death_World,
    Frontier_World,
    Forbidden_World,
    Xenos_World,
  }

  enum PlanetTech
  {
    Stone_Age,
    Iron_Age,
    Steel_Age,
    Pre_Industrial,
    Industrial,
    Early_Space,
    Advanced_Space,
    Warp_Space,
    Low_Imperial,
    Mid_Imperial,
    High_Imperial,
    Advanced,
  }

  partial class Simulation
  {

    Trait getFromRange(Dictionary<int, Trait> rollTable, int rollOverride = 0)
    {
      int roll = rollOverride != 0 ? rollOverride : RNG.Integer(1, 100);
      foreach (var type in rollTable)
        if (roll <= type.Key) return type.Value;
      return new Trait();
    }

    public void GeneratePlanes()
    {
      var wt = TraitLists["world type"];
      var tl = TraitLists["tech level"];

      var planetTypeRollTable = new Dictionary<int, Trait>
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
      };

      var planetTechRollTable = new Dictionary<int, Trait>
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
      };

      var destroyedWorlds = new HashSet<Trait> { wt["Quarantined World"], wt["War World"], wt["Dead World"] };

      void addPlanetType(Planet p)
      {
        var planetType = getFromRange(planetTypeRollTable);

        if (destroyedWorlds.Contains(planetType))
        {
          var planetTypeOriginal = getFromRange(planetTypeRollTable);
          while (destroyedWorlds.Contains(planetTypeOriginal))
            planetTypeOriginal = getFromRange(planetTypeRollTable);
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
        p.AddTrait(getFromRange(planetTechRollTable, getTechRoll()));
      }


      foreach (int i in Enumerable.Range(0, 10))
      {
        var p = new Planet()
        {
          location = new Vector2((float)(RNG.Integer(-80, 80) / 10), (float)(RNG.Integer(-90, 90) / 10)),
          rotationSpeed = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(RNG.Integer(-5, 5) / 20f), MathHelper.ToRadians(RNG.Integer(-5, 5) / 20f), 0),
          rotation = Quaternion.Identity,
          size = RNG.Integer(10, 25) / 10f,
          color = new Color(RNG.Integer(0, 255), RNG.Integer(0, 255), RNG.Integer(0, 255)),
        };

        addPlanetType(p);
        addTechLevel(p);

        cosmos.Planets.AutoAdd(p);
      }
    }
  }
}