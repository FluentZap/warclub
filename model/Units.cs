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

class ActiveUnit
{
  public int Points;
  public Models Model;
  public int DeployedCount;
  public Commander Commander;
  public Unit BaseUnit;
}

class Unit
{
  public DataSheet DataSheet;
  public Dictionary<int, int> UnitLines = new Dictionary<int, int>();
  public List<Wargear> Wargear = new List<Wargear>();
}

class UnitArchetypeStats
{
  public bool Cheap;
  public bool Expensive;
  public bool HighDamage;
  public bool LongRange;
  public bool Melee;
  public bool Quick;
  public bool Tough;
}