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
    // Is attuned
    Attunement
  }
  // class Relation : Dictionary<RelationType, byte> { }
  class Relation
  {
    public byte Strength;
    public Relation(byte strength)
    {
      Strength = strength;
    }
  }
}