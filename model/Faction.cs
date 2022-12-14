using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System.Collections.Generic;

namespace WarClub;

class Faction : Entity
{
  public Dictionary<UnitRole, List<Unit>> Units = new Dictionary<UnitRole, List<Unit>>()
  {
    [UnitRole.Troops] = new List<Unit>(),
    [UnitRole.Elites] = new List<Unit>(),
    [UnitRole.HeavySupport] = new List<Unit>(),
    [UnitRole.HQ] = new List<Unit>(),
    [UnitRole.FastAttack] = new List<Unit>(),
    [UnitRole.DedicatedTransport] = new List<Unit>(),
    [UnitRole.Flyers] = new List<Unit>(),
    [UnitRole.LordsOfWar] = new List<Unit>(),
    [UnitRole.Fortifications] = new List<Unit>(),
  };
  public Faction() : base(EntityType.Faction) { }
}