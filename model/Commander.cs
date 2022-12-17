using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace WarClub;

class Commander
{
  public string Name;
  public Color Color;
  public List<Unit> Units = new List<Unit>();
  public bool Active = false;
}