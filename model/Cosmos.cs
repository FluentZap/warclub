using System.Collections.Generic;

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
    public uint Day;
    // public uint Month;
    // public ulong Year;

    public KeyCollection<Sector> Sectors = new KeyCollection<Sector>();
    public KeyCollection<World> Worlds = new KeyCollection<World>();
    public KeyCollection<Faction> Factions = new KeyCollection<Faction>();

    // public KeyCollection<Relation> Relations = new KeyCollection<Relation>();

    public EventLog EventLog = new EventLog();
    public Faction PlayerFaction;



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