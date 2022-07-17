using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace WarClub
{
  class Unit
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

    public string Role;
    public string Movement;
    public string WS;
    public string BS;
    public string S;
    public string T;
    public string W;
    public string A;
    public string Ld;
    public string Sv;
    public string Cost;
    public int MinModelsPerUnit;
    public int MaxModelsPerUnit;
    public Point Size = new Point();
  }

}
