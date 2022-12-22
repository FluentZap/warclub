using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace WarClub;

class Commander
{
  public string Name;
  public Color Color;
  public List<ActiveUnit> Units = new List<ActiveUnit>();
  public Icon Icon;
}