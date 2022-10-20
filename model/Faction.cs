using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System.Collections.Generic;

namespace WarClub;

class Faction : Entity
{
  public Dictionary<string, List<Unit>> Units = new Dictionary<string, List<Unit>>();
  public Faction() : base(EntityType.Faction) { }
}