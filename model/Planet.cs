using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System.Collections.Generic;

namespace WarClub
{
  class Planet : Entity
  {
    public Sector sector;
    // public Vector2 location;
    public Quaternion rotationSpeed;
    public Quaternion rotation = Quaternion.Identity;
    public int DayLength;
    public int YearLength;
    public Color color;
    public float size;

    public Planet() : base(EntityType.Planet) { }
  }
}