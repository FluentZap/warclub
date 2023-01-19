using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System.Collections.Generic;

namespace WarClub;

class Sector : Entity
{
  public Vector2 Location;
  public Sector() : base(EntityType.Sector) { }
}