using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WarClub
{

  class KeyCollection<T> : Dictionary<uint, T> where T : ID
  {
    uint id = 0;

    public uint AutoAdd(T item)
    {
      if (!this.ContainsKey(id))
      {
        this.Add(id, item);
        item.Id = id;
        return id++;
      }
      return id;
    }
  }

  class Cosmos
  {
    // public float Hour;
    public uint Day { get; set; }
    // public uint Month;
    // public ulong Year;

    // [JsonIgnore]
    public KeyCollection<Sector> Sectors { get; set; }
    // [JsonIgnore]
    public KeyCollection<World> Worlds { get; set; }
    [JsonIgnore]
    public KeyCollection<Faction> Factions { get; set; }

    // public KeyCollection<Relation> Relations = new KeyCollection<Relation>();


    [JsonIgnore]
    public EventLog EventLog = new EventLog();
    [JsonIgnore]
    public Faction PlayerFaction;

    public Cosmos()
    {
      Sectors = new KeyCollection<Sector>();
      Worlds = new KeyCollection<World>();
      Factions = new KeyCollection<Faction>();
    }


    // public void AdvanceHour()
    // {
    //   Hour++;
    //   if (Hour >= 24)
    //   {
    //     Hour = 0;
    //     Day++;
    //   }
    // }
    public void AdvanceDay()
    {
      Day++;
    }
  }
}