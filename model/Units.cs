using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace WarClub
{
  class Models
  {
    public int Id;
    public string Name;
    public string Image;
    public int Count;
    public Point Size = new Point();
    public HashSet<string> Types = new HashSet<string>();
  }

  class DataSheet
  {
    public int Id;
    public string Name;
    public string Faction;
    // Troops
    // Elites
    // Heavy Support
    // HQ
    // Fast Attack
    // Dedicated Transport
    // Flyers
    // Lords of War
    // Fortifications
    public string Role;
    public Dictionary<string, UnitStats> Units = new Dictionary<string, UnitStats>();
  }

  class UnitStats
  {
    public string Movement;
    public string WS;
    public string BS;
    public string S;
    public string T;
    public string W;
    public string A;
    public string Ld;
    public string Sv;
    public int Cost;
    public int MinModelsPerUnit;
    public int MaxModelsPerUnit;
    public Point Size = new Point();
  }

  class Unit
  {
    public DataSheet DataSheet;
    public Dictionary<string, int> UnitModels = new Dictionary<string, int>();
  }

}
