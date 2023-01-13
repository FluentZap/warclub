using System.Linq;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace WarClub;

static partial class Generator
{

  static public Mission GenerateBattleMap(Simulation s)
  {
    // if (s.SelectedMission.Name == "assault outpost") return GenerateAssaultOutpost(s);
    // return GenerateAssaultOutpost(s);
    return GenerateWarProfiteering(s);
    // return null;
  }

  static public Mission GenerateAssaultOutpost(Simulation s)
  {
    // default to fortress forest map type
    var mission = new Mission();
    var faction = s.cosmos.Factions[RNG.PickFrom(s.SelectedWorld.Relations.Where(x => x.Source.EntityType == EntityType.Faction).ToList()).Source.Id];

    var pointMax = mission.PointCapacity = 375 + TraitUtil.getAspect(s.SelectedWorld.GetTraits(), "entrenchment") * 100;
    var fortOnLeft = RNG.Boolean();
    mission.Tiles.Item1 = RNG.PickFrom(GetTileList(s.MapTiles, MapTileTerrain.Forest, MapTileOrientation.Left, fortOnLeft ? MapTileType.Fortress : MapTileType.Obstacles));
    mission.Tiles.Item2 = RNG.PickFrom(GetTileList(s.MapTiles, MapTileTerrain.Forest, MapTileOrientation.Right, fortOnLeft ? MapTileType.Obstacles : MapTileType.Fortress));
    // mission.PlayerDeploymentZones.Add(BuildRect(0, 0, 6, ScreenSize.Y, fortOnLeft));

    // var turn0 = new MissionEvent() { Turn = 0 };
    // turn0.Spawns.Add(new MissionEventSpawn()
    // {
    //   Location = BuildPoint(14, 16, !fortOnLeft),
    //   Units = SelectUnits(faction.Units, pointMax, Troops: 1, HQ: 1, FastAttack: 3, HeavySupport: 10, Elites: 5),
    // });
    // mission.MissionEvents.Add(turn0);

    return mission;
  }

  static public Mission GenerateWarProfiteering(Simulation s)
  {
    // default to fortress forest map type
    var mission = new Mission();
    var terrain = GetPlanetTerrain(s.SelectedWorld);
    var faction = s.cosmos.Factions[RNG.PickFrom(s.SelectedWorld.Relations.Where(x => x.Source.EntityType == EntityType.Faction).ToList()).Source.Id];

    var pointMax = mission.PointCapacity = 375 + TraitUtil.getAspect(s.SelectedWorld.GetTraits(), "entrenchment") * 100;
    mission.Tiles.Item1 = RNG.PickFrom(GetTileList(s.MapTiles, terrain, MapTileOrientation.Left));
    mission.Tiles.Item2 = RNG.PickFrom(GetTileList(s.MapTiles, terrain, MapTileOrientation.Right));

    mission.MissionEvents.Add(new MissionEvent()
    {
      Turn = 1,
      Type = MissionSpawnType.PCDeploymentZone,
      Message = new MissionMessage() { Text = "Deploy Player Units" },
      Zones = new List<Rectangle>() {
          BuildRect(0, 0, 6, ScreenSize.Y),
          BuildRect(0, 0, 6, ScreenSize.Y, flipX: true),
          BuildRect(6, 0, ScreenSize.X - 12, 3),
          BuildRect(6, 0, ScreenSize.X - 12, 3, flipY: true)
        }
    });

    // foreach (var rawUnit in SelectUnits(faction.Units, pointMax, Troops: 5, HQ: 1, FastAttack: 1, HeavySupport: 1, Elites: 2))
    foreach (var rawUnit in SelectUnits(faction.Units, pointMax, Troops: 5, HQ: 1, FastAttack: 1, HeavySupport: 1))
    {
      var unit = UnitUtils.ActivateUnit(rawUnit, s.UnitList.First());
      mission.MissionEvents.Add(new MissionEvent()
      {
        Turn = 2,
        Type = MissionSpawnType.AISpawn,
        Icon = Icon.Barracks,
        Zones = new List<Rectangle>() { BuildRect(RNG.Integer(6, ScreenSize.X - 6), RNG.Integer(6, ScreenSize.Y - 6), 6, 6) },
        Unit = unit,
        Message = new MissionMessage() { Text = GetUnitSpawnLabel(unit), Color = Color.White }
      });
    };

    return mission;
  }

}