using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace WarClub;

enum View
{
  MainMenu,
  NewGame,
  LoadGame,
  GalaxyOverview,
  MissionSelect,
  MissionBriefing,
  Battlefield,
}

class TraitList : Dictionary<string, Trait>
{
  public List<Trait> getFromArray(string[] keys)
  {
    List<Trait> traits = new List<Trait>();
    foreach (string key in keys)
    {
      traits.Add(this[key]);
    }
    return traits;
  }
}

partial class Simulation
{
  public List<Models> UnitList = new List<Models>();
  public List<Models> AvailableUnits = new List<Models>();

  public Dictionary<int, DataSheet> DataSheets = new Dictionary<int, DataSheet>();
  public Dictionary<int, Wargear> Wargear = new Dictionary<int, Wargear>();
  public Dictionary<int, Stratagem> Stratagems = new Dictionary<int, Stratagem>();

  public Cosmos cosmos = new Cosmos();
  public TraitList Traits = new TraitList();
  public Dictionary<string, TraitList> TraitLists = new Dictionary<string, TraitList>();
  public Dictionary<string, OrderEvent> OrderEvents = new Dictionary<string, OrderEvent>();
  public Dictionary<string, Dictionary<string, OrderEvent>> OrderEventLists = new Dictionary<string, Dictionary<string, OrderEvent>>();

  public Dictionary<string, ChaosEvent> ChaosEvents = new Dictionary<string, ChaosEvent>();
  public Dictionary<string, Dictionary<string, ChaosEvent>> ChaosEventLists = new Dictionary<string, Dictionary<string, ChaosEvent>>();

  public List<MapTile> MapTiles = new List<MapTile>();



  public KeyState KeyState = new KeyState();
  public Matrix ViewMatrix;
  public View SelectedView = View.MissionBriefing;
  public World SelectedWorld = null;
  public OrderEvent SelectedMission = null;
  public int SelectedUnit;
  public int Turn;

  public Mission ActiveMission;
  public List<ActiveUnit> SelectableUnits;
  public List<ActiveUnit> SelectedUnits = new List<ActiveUnit>();
  public int CurrentPage;
  public List<Commander> Commanders = new List<Commander>(){
    new Commander(){
      Color = Color.Red,
      Icon = Icon.CrossedAxes,
      Name = "Red",
    },
    new Commander(){
      Color = Color.CornflowerBlue,
      Icon = Icon.Barracks,
      Name = "Blue",
    },
    new Commander(){
      Color = Color.Green,
      Icon = Icon.HumanTarget,
      Name = "Green",
    },
    new Commander(){
      Color = Color.Orange,
      Icon = Icon.MilitaryFort,
      Name = "Orange",
    },
  };


  public void Generate()
  {
    LoadLists();
    LoadUnits();
    LoadDataSheets();
    LoadMapTiles();
    GenerateWorlds();
    GenerateFactions();

    AdvanceTime();


    foreach (var world in cosmos.Worlds.Values)
    {
      var missions = world.GetEventList(OrderEventLists["Mercenary"]);
      if (missions.Count > 0)
      {
        SelectedWorld = world;
        SelectedMission = missions[0];
        ActiveMission = Generator.GenerateBattleMap(this);
        break;
      }
    }

    SelectableUnits = cosmos.PlayerFaction.Units.SelectMany(x => x.Value).Select(x => UnitUtils.ActivateUnit(x)).ToList();
    CurrentPage = 0;

    // TimeWizard.Stasis(this);
    // PlanetColors.Add(Traits["cemetery world"], new WorldColor()
    // {
    //   col_bot = Color.FromArgb(227, 27, 160),
    //   col_mid1 = Color.FromArgb(168, 155, 109),
    //   col_mid2 = Color.FromArgb(16, 62, 86),
    //   col_mid3 = Color.FromArgb(162, 182, 201),
    //   col_top = Color.FromArgb(36, 121, 174),
    // });
    // PlanetColors.Add(Traits["agri world"], new WorldColor()
    // {
    //   col_bot = Color.FromArgb(126, 81, 207),
    //   col_mid1 = Color.FromArgb(42, 151, 10),
    //   col_mid2 = Color.FromArgb(209, 153, 197),
    //   col_mid3 = Color.FromArgb(98, 171, 205),
    //   col_top = Color.FromArgb(182, 139, 122),
    // });
    // PlanetColors.Add(Traits["forge world"], new WorldColor()
    // {
    //   col_bot = Color.FromArgb(122, 8, 13),
    //   col_mid1 = Color.FromArgb(20, 76, 133),
    //   col_mid2 = Color.FromArgb(170, 218, 56),
    //   col_mid3 = Color.FromArgb(175, 203, 146),
    //   col_top = Color.FromArgb(144, 184, 77),
    // });
  }

  public void AdvanceTime()
  {
    // Do all events for the star Sign
    cosmos.AdvanceDay();

    // foreach (World p in cosmos.Worlds.Values)
    // {
    //   p.rotation *= p.rotationSpeed;
    // }

    foreach (var (id, e) in cosmos.Factions)
    {
      switch (e.EntityType)
      {
        case EntityType.Faction: FactionEvent(id); break;
      }
    }

  }

  Dictionary<Trait, WorldColor> PlanetColors = new Dictionary<Trait, WorldColor>();

}