using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace WarClub;

class Models
{
  public int Id;
  public string Name;
  public string Image;
  public int Count;
  public Point Size = new Point();
  public HashSet<string> Types = new HashSet<string>();
}

class Unit
{
  public DataSheet DataSheet;
  public Dictionary<string, UnitLine> UnitLines = new Dictionary<string, UnitLine>();
}

class UnitLine
{
  public UnitStats UnitStats;
  public int Count;
  public List<Wargear> Wargear = new List<Wargear>();
}