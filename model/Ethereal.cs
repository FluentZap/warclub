using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace WarClub;

class EtherealCosmos
{
  public uint Day;
  public Dictionary<uint, EtherealSector> Sectors;
  public Dictionary<uint, EtherealWorld> Worlds;
  public Dictionary<uint, EtherealFaction> Factions;
  public uint PlayerFaction;
}

class EtherealSector : EtherealEntity
{
  public Vector2 Location;
}

class EtherealWorld : EtherealEntity
{
  public uint sector;
  public Vector2 location;
  public float rotationSpeed;
  public Quaternion rotation;
  public int DayLength;
  public int YearLength;
  public WorldColor color;
  public float size;
}

class EtherealFaction : EtherealEntity
{
  public Dictionary<UnitRole, List<EtherealUnit>> Units;
}

class EtherealEntity
{
  public EntityType EntityType;
  public uint Id;
  public string Name;
  public List<EtherealRelation> Relations;
  public Psyche Psyche = new Psyche();
  public Dictionary<string, int> Traits;
}


class EtherealRelation
{
  public int Strength;
  public RelationType relationType;
  public string Source;
  public string Target;
  public Dictionary<string, int> Traits;
}

class EtherealUnit
{
  public int DataSheet;
  public Dictionary<int, int> UnitLines;
  public List<int> Wargear;
}
