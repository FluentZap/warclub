using System;
using System.Collections.Generic;
using System.Linq;

namespace WarClub
{

  enum RelationType
  {
    Satellite,
    // Created by
    Created,
    // Exists in the region of the other
    Region,
    // Headquarters in the area
    Headquarters,
    // Mission Target
    MissionTarget,
  }

  class Relation
  {
    public int Strength;
    public Entity Source;
    public Entity Target;
    public RelationType relationType;
    public Dictionary<Trait, int> Traits = new Dictionary<Trait, int>();
  }
}