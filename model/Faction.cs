using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System.Collections.Generic;

namespace WarClub;

class Faction : Entity
{
  public Dictionary<UnitRole, List<Unit>> Units = new Dictionary<UnitRole, List<Unit>>();
  public Faction() : base(EntityType.Faction) { }
}