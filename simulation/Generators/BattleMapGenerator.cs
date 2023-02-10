using System;
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
    // return GenerateWarProfiteering(s);
    if (s.SelectedMission.Name == "take kill contract") return GenerateKillContract(s);
    // return GenerateKillContract(s);
    return null;
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

  static public Mission GenerateKillContract(Simulation s)
  {
    var mission = new Mission();
    var terrain = GetPlanetTerrain(s.SelectedWorld);
    var faction = s.cosmos.Factions[RNG.PickFrom(s.SelectedWorld.Relations.Where(x => x.Source.EntityType == EntityType.Faction).ToList()).Source.Id];
    var sus = new SelectUnitSystem(faction.Units);
    mission.Tiles.Item1 = RNG.PickFrom(GetTileList(s.MapTiles, terrain, MapTileOrientation.Left));
    mission.Tiles.Item2 = RNG.PickFrom(GetTileList(s.MapTiles, terrain, MapTileOrientation.Right));
    var pointMax = mission.PointCapacity = 150 + TraitUtil.getAspect(s.SelectedWorld.GetTraits(), "entrenchment") * 25;

    var used = s.cosmos.PlayerFaction.Units.SelectMany(x => x.Value.Select(x => x.DataSheet.Id)).ToHashSet();
    var models = s.ModelList.Where(x => !x.DataCardIds.Overlaps(used)).ToList();

    for (int i = 0; i < 3; i++)
      mission.MissionEvents.Add(new MissionEvent()
      {
        Turn = 0,
        Type = MissionEventType.PCDeploymentZone,
        Message = new MissionMessage() { Text = $"{i + 1}: Deploy 1/3 of player units", Color = Color.White },
        Zones = new List<Rectangle>() { BuildRect(RNG.Integer(6, ScreenSize.X - 6), RNG.Integer(6, ScreenSize.Y - 6), 6, 6) }
      });

    var tempEvents = new List<MissionEvent>();
    Point spawnPoint;
    for (int i = 0; i < RNG.DiceRoll(2, 4); i++)
    {
      spawnPoint = new Point(RNG.Integer(2, ScreenSize.X - 2), RNG.Integer(2, ScreenSize.Y - 2));
      tempEvents.Add(new MissionEvent()
      {
        Turn = 0,
        Type = MissionEventType.LootBox,
        Zones = new List<Rectangle>() { BuildRect(spawnPoint.X, spawnPoint.Y, 2, 2) },
        TriggeredEvents = UnitUtils.ActivateUnits(sus.SelectUnits(pointMax / 2, Troops: 1, HQ: 1, FastAttack: 1), models).Select(x => new MissionEvent()
        {
          Type = MissionEventType.AISpawn,
          Unit = x,
          Message = new MissionMessage() { Text = GetUnitSpawnLabel(x), Color = Color.White },
          Zones = new List<Rectangle>() { BuildRect(
            Math.Clamp(RNG.Integer(spawnPoint.X - 10, spawnPoint.X + 10), 8, ScreenSize.X - 8),
            Math.Clamp(RNG.Integer(spawnPoint.Y - 10, spawnPoint.Y + 10), 8, ScreenSize.Y - 8),
            8, 8) },
        }).ToList(),
      });
    }

    spawnPoint = new Point(RNG.Integer(2, ScreenSize.X - 2), RNG.Integer(2, ScreenSize.Y - 2));
    tempEvents.Add(new MissionEvent()
    {
      Turn = 0,
      Type = MissionEventType.LootBox,
      Zones = new List<Rectangle>() { BuildRect(spawnPoint.X, spawnPoint.Y, 2, 2) },
      TriggeredEvents = UnitUtils.ActivateUnits(sus.SelectUnits(pointMax, Troops: 1, HQ: 1, FastAttack: 1), models).Select(x => new MissionEvent()
      {
        Type = MissionEventType.AISpawn,
        Unit = x,
        Message = new MissionMessage() { Text = GetUnitSpawnLabel(x), Color = Color.White },
        Zones = new List<Rectangle>() { BuildRect(
            Math.Clamp(RNG.Integer(spawnPoint.X - 10, spawnPoint.X + 10), 8, ScreenSize.X - 8),
            Math.Clamp(RNG.Integer(spawnPoint.Y - 10, spawnPoint.Y + 10), 8, ScreenSize.Y - 8),
            8, 8) },
      }).ToList(),
    });

    RNG.Shuffle(tempEvents);

    for (int i = 0; i < tempEvents.Count; i++)
      tempEvents[i].Message = new MissionMessage() { Text = $"Objective: {i + 1}", Color = Color.White };

    mission.MissionEvents.AddRange(tempEvents);

    mission.MissionEvents.Add(new MissionEvent()
    {
      Turn = 1,
      Frequency = 1,
      Phase = Phases.Morale,
      Message = new MissionMessage() { Text = "Activate Locations", Color = Color.White },
      InteractionEvent = true,
    });

    return mission;
  }

  // static public Mission GenerateWarProfiteering(Simulation s)
  // {
  //   var mission = new Mission();
  //   var terrain = GetPlanetTerrain(s.SelectedWorld);
  //   var faction = s.cosmos.Factions[RNG.PickFrom(s.SelectedWorld.Relations.Where(x => x.Source.EntityType == EntityType.Faction).ToList()).Source.Id];

  //   var pointMax = mission.PointCapacity = 150 + TraitUtil.getAspect(s.SelectedWorld.GetTraits(), "entrenchment") * 25;
  //   mission.Tiles.Item1 = RNG.PickFrom(GetTileList(s.MapTiles, terrain, MapTileOrientation.Left));
  //   mission.Tiles.Item2 = RNG.PickFrom(GetTileList(s.MapTiles, terrain, MapTileOrientation.Right));

  //   mission.MissionEvents.Add(new MissionEvent()
  //   {
  //     Turn = 0,
  //     Type = MissionEventType.PCDeploymentZone,
  //     Message = new MissionMessage() { Text = "Deploy Player Units" },
  //     Zones = new List<Rectangle>() {
  //         BuildRect(0, 0, 6, ScreenSize.Y),
  //         // BuildRect(0, 0, 6, ScreenSize.Y, flipX: true),
  //         BuildRect(6, 0, (ScreenSize.X - 6) / 3, 3),
  //         BuildRect(6, 0, (ScreenSize.X - 6) / 3, 3, flipY: true)
  //       }
  //   });

  // foreach (var rawUnit in SelectUnits(faction.Units, pointMax, Troops: 5, HQ: 1, FastAttack: 1, HeavySupport: 1, Elites: 2))

  // var wave1SpawnPoint = new Point(RNG.Integer(30, ScreenSize.X), RNG.Integer(8, ScreenSize.Y));
  // foreach (var rawUnit in SelectUnits(faction.Units, pointMax, Troops: 5, HQ: 1, FastAttack: 1))
  // {
  // var models = s.AvailableUnits.Where(x => x.Count >= rawUnit.UnitLines.First().Value && x.Size.X >= rawUnit.DataSheet.Units[rawUnit.UnitLines.First().Key].Size.X).ToList();
  // var model = models.Count > 0 ? RNG.PickFrom(models) : null;
  // var unit = UnitUtils.ActivateUnit(rawUnit, null);
  // s.AvailableUnits.Remove(model);

  //   var distance = 0;
  //   if (unit.BaseUnit.DataSheet.Role == UnitRole.Troops) distance = -5;
  //   if (unit.BaseUnit.DataSheet.Role == UnitRole.HeavySupport) distance = 1;
  //   if (unit.BaseUnit.DataSheet.Role == UnitRole.HQ) distance = -3;
  //   if (unit.BaseUnit.DataSheet.Role == UnitRole.FastAttack) distance = -2;
  //   mission.MissionEvents.Add(new MissionEvent()
  //   {
  //     Turn = 0,
  //     Type = MissionEventType.AISpawn,
  //     Icon = Icon.Barracks,
  //     Zones = new List<Rectangle>() { BuildRect(wave1SpawnPoint.X + distance, RNG.Integer(6, ScreenSize.Y - 6), 4, 4) },
  //     Unit = unit,
  //     Message = new MissionMessage() { Text = GetUnitSpawnLabel(unit), Color = Color.White }
  //   });
  // };


  // foreach (var rawUnit in SelectUnits(faction.Units, 50, Troops: 1))
  // {
  //   var unit = UnitUtils.ActivateUnit(rawUnit, null);
  //   mission.MissionEvents.Add(new MissionEvent()
  //   {
  //     Turn = 1,
  //     Phase = Phases.Movement,
  //     Type = MissionEventType.AISpawn,
  //     Zones = new List<Rectangle>() { BuildRect(RNG.Integer(6, ScreenSize.X - 6), RNG.Integer(6, ScreenSize.Y - 6), 4, 4) },
  //     // Unit = unit,
  //     Message = new MissionMessage() { Text = GetUnitSpawnLabel(unit), Color = Color.White }
  //   });
  // };

  //   return mission;
  // }

}