using System.Collections.Generic;

namespace WarClub
{

  class KeyCollection<T> : Dictionary<uint, T>
  {
    uint id = 0;

    public uint AutoAdd(T item)
    {
      if (!this.ContainsKey(id))
      {
        this.Add(id, item);
        return id++;
      }
      return id;
    }
  }

  class Cosmos
  {
    public ulong Year;
    public uint Month;

    public KeyCollection<Planet> Planets = new KeyCollection<Planet>();

    public EventLog EventLog = new EventLog();

    public void AdvanceMonth()
    {
      Month++;
      if (Month >= 12)
      {
        Month = 0;
        Year++;
      }
    }
  }
}