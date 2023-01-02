using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
    mission.PlayerDeploymentZones.Add(BuildRect(0, 0, 6, ScreenSize.Y, fortOnLeft));

    var turn0 = new MissionEvent() { Turn = 0 };
    turn0.Spawns.Add(new MissionEventSpawn()
    {
      Location = BuildPoint(14, 16, !fortOnLeft),
      Units = SelectUnits(faction.Units, pointMax, Troops: 1, HQ: 1, FastAttack: 3, HeavySupport: 10, Elites: 5),
    });
    mission.MissionEvents.Add(turn0);

    return mission;
  }

  static public Mission GenerateWarProfiteering(Simulation s)
  {
    // default to fortress forest map type
    var mission = new Mission();
    var faction = s.cosmos.Factions[RNG.PickFrom(s.SelectedWorld.Relations.Where(x => x.Source.EntityType == EntityType.Faction).ToList()).Source.Id];

    var pointMax = mission.PointCapacity = 375 + TraitUtil.getAspect(s.SelectedWorld.GetTraits(), "entrenchment") * 100;
    mission.Tiles.Item1 = RNG.PickFrom(GetTileList(s.MapTiles, MapTileTerrain.Forest, MapTileOrientation.Left));
    mission.Tiles.Item2 = RNG.PickFrom(GetTileList(s.MapTiles, MapTileTerrain.Forest, MapTileOrientation.Right));
    mission.PlayerDeploymentZones.Add(BuildRect(0, 0, 6, ScreenSize.Y));
    mission.PlayerDeploymentZones.Add(BuildRect(0, 0, 6, ScreenSize.Y, flipX: true));

    mission.PlayerDeploymentZones.Add(BuildRect(6, 0, ScreenSize.X - 12, 3));
    mission.PlayerDeploymentZones.Add(BuildRect(6, 0, ScreenSize.X - 12, 3, flipY: true));

    var turn0 = new MissionEvent() { Turn = 1 };
    turn0.Spawns.Add(new MissionEventSpawn()
    {
      Location = BuildPoint(RNG.Integer(6, ScreenSize.X - 6), RNG.Integer(6, ScreenSize.Y - 6), false),
      Units = SelectUnits(faction.Units, pointMax, Troops: 5, HQ: 1, FastAttack: 1, HeavySupport: 1, Elites: 2),
    });
    mission.MissionEvents.Add(turn0);

    return mission;
  }

}