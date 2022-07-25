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
    public float Hour;
    public uint Day;
    // public uint Month;
    // public ulong Year;

    public KeyCollection<Sector> Sectors = new KeyCollection<Sector>();
    public KeyCollection<Planet> Planets = new KeyCollection<Planet>();

    public EventLog EventLog = new EventLog();

    public void AdvanceHour(float hour)
    {
      Hour++;
      if (Hour >= 24)
      {
        Hour = 0;
        Day++;
      }
    }
  }
}