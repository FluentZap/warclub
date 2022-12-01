using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace WarClub;

class Models
{
  public string Name;
  public HashSet<int> DataCardIds = new HashSet<int>();
  public string Image;
  public Texture2D Texture;
  public int Count;
  public Point Size = new Point();
  public HashSet<string> Types = new HashSet<string>();
  public bool Reserved = false;
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