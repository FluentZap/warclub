using System;
using System.Collections.Generic;

namespace WarClub
{

  class Trait
  {
    public string Name;
    public List<string> Type { get; set; }
    public Dictionary<string, int> Aspects { get; set; }
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
    public List<Unit> UnitList = new List<Unit>();
    public List<Unit> AvailableUnits = new List<Unit>();

    public Dictionary<int, DataSheet> DataSheets = new Dictionary<int, DataSheet>();

    public TraitList Traits = new TraitList();
    public Dictionary<string, TraitList> TraitLists = new Dictionary<string, TraitList>();
    public Dictionary<string, OrderEvent> OrderEvents = new Dictionary<string, OrderEvent>();
    public Dictionary<string, Dictionary<string, OrderEvent>> OrderEventLists = new Dictionary<string, Dictionary<string, OrderEvent>>();

    public Dictionary<string, ChaosEvent> ChaosEvents = new Dictionary<string, ChaosEvent>();
    public Dictionary<string, Dictionary<string, ChaosEvent>> ChaosEventLists = new Dictionary<string, Dictionary<string, ChaosEvent>>();

    public void Generate()
    {
      LoadLists();
      LoadUnits();
      LoadDataSheets();
      // GeneratePlanes();
    }

  }
}