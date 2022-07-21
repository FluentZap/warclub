using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System.Collections.Generic;

namespace WarClub
{
  class Star : Entity
  {
    public Vector2 location;
    public Quaternion rotationSpeed;
    public Quaternion rotation;
    public Color color;
    public float size;

    public Star() : base(EntityType.Star) { }
  }
}