using System.Collections.Generic;

namespace WarClub
{

  class EventRequirements
  {
    public string Name;
    public List<string> AllowedEntityType { get; set; }
    public Dictionary<Trait, int> RequiredTraits { get; set; }
    public Dictionary<string, int> RequiredAspects { get; set; }
  }

  class OrderEvent : EventRequirements
  {
    public Psyche Psyche { get; set; }
  }

  class ChaosEvent : EventRequirements
  {
    public Dictionary<Trait, int> InfluentialTraits { get; set; }
    public Dictionary<string, int> InfluentialAspects { get; set; }
    public List<EventAction> EventActions { get; set; } = new List<EventAction>();
  }

  class EventAction
  {
    public string Name;
    public Dictionary<Trait, int> RequiredTraits { get; set; }
    public Dictionary<string, int> RequiredAspects { get; set; }
    public Psyche Psyche { get; set; }
    public Psyche PsycheWeight { get; set; }
  }
}