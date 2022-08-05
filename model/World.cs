using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System.Collections.Generic;

namespace WarClub
{
  class World : Entity
  {
    public Sector sector;
    // public Vector2 location;
    // public Quaternion rotationSpeed;
    public float rotationSpeed;
    public Quaternion rotation = Quaternion.Identity;
    public int DayLength;
    public int YearLength;
    public Color color_top;
    public Color color_bot;
    public Color color_mid1;
    public Color color_mid2;
    public Color color_mid3;
    public float size;

    public World() : base(EntityType.World) { }
  }
}