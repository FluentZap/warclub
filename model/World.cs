using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System.Collections.Generic;

namespace WarClub;

class WorldColor
{
  public Color top;
  public Color bot;
  public Color mid1;
  public Color mid2;
  public Color mid3;
}

class World : Entity
{
  public Sector sector;
  public Vector2 location;
  // public Quaternion rotationSpeed;
  public float rotationSpeed;
  public Quaternion rotation = Quaternion.Identity;
  public int DayLength;
  public int YearLength;
  public WorldColor color;
  public float size;

  public World() : base(EntityType.World) { }
}