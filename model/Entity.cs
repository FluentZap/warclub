using System;
using System.Collections.Generic;
using System.Linq;

namespace WarClub
{
  public enum EntityType
  {
    Planet,
    Faction,
    Agent,
  }

  class ID
  {
    public EntityType EntityType;
    public uint Id;

    public ID() { }

    public ID(EntityType type, uint id)
    {
      this.EntityType = type;
      this.Id = id;
    }
  }

  class Entity : ID
  {
    public string Name;

    // my relations mean i am the source of the relation
    // their relations mean i am the target of a relation
    public Dictionary<RelationType, Dictionary<Entity, Relation>> RelationsIn = new Dictionary<RelationType, Dictionary<Entity, Relation>>();
    public Dictionary<RelationType, Dictionary<Entity, Relation>> RelationsOut = new Dictionary<RelationType, Dictionary<Entity, Relation>>();

    // my traits are attached to anything with the corresponding relation
    // their traits are attached to me
    public Dictionary<RelationType, Dictionary<Trait, int>> TraitsIn = new Dictionary<RelationType, Dictionary<Trait, int>>();
    public Dictionary<RelationType, Dictionary<Trait, int>> TraitsOut = new Dictionary<RelationType, Dictionary<Trait, int>>();

    public Psyche Psyche = new Psyche();
    public Dictionary<Trait, int> Traits = new Dictionary<Trait, int>();

    public Entity(EntityType type)
    {
      this.EntityType = type;
    }

    // ****************
    // *****TRAITS*****
    // ****************

    public void AddTrait(Trait trait, int count = 1)
    {
      if (Traits.ContainsKey(trait))
      {
        // Trait hard limit at 1 bill
        var change = Math.Min(Traits[trait] + count, 1000000000);
        if (change > 0)
          Traits[trait] = change;
        else
          Traits.Remove(trait);
      }
      else
      {
        Traits.Add(trait, count);
      }
    }

    public Dictionary<Trait, int> GetTraits()
    {
      var traitsList = new List<Dictionary<Trait, int>>();
      traitsList.Add(Traits);

      foreach (var (e, traits) in TraitsIn)
        traitsList.Add(traits);
      return TraitUtil.combineTraits(traitsList); ;
    }

    // ****************
    // ****RELATIONS***
    // ****************

    // strength is calculated from 0 to 200, 255 is an unchangeable relation
    public void AddRelation(Entity e, RelationType relationType, byte strength)
    {
      Relation relation = new Relation(strength);
      if (!e.RelationsIn.ContainsKey(relationType))
        e.RelationsIn.Add(relationType, new Dictionary<Entity, Relation>());
      if (!RelationsOut.ContainsKey(relationType))
        RelationsOut.Add(relationType, new Dictionary<Entity, Relation>());

      if (!e.RelationsIn[relationType].TryAdd(this, relation))
        e.RelationsIn[relationType][this].Strength += strength;
      if (!RelationsOut[relationType].TryAdd(e, relation))
        RelationsOut[relationType][e].Strength += strength;
    }

    // public Dictionary<RelationType, Relation> GetRelations(Entity e)
    // {
    //   return Relations.GetValueOrDefault(e, new Dictionary<RelationType, Relation>());
    // }

    public Dictionary<Entity, Relation> GetRelationsOut(EntityType entityType, RelationType relationType)
    {
      Dictionary<Entity, Relation> list = new Dictionary<Entity, Relation>();
      if (RelationsOut.ContainsKey(relationType))
        foreach (var item in RelationsOut[relationType])
          if (item.Key.EntityType == entityType)
            list.Add(item.Key, item.Value);
      return list;
    }

    // ****************
    // **ORDER_EVENTS**
    // ****************

    public List<T> GetEventList<T>(Dictionary<string, T> orderEvents) where T : EventRequirements
    {
      List<T> events = new List<T>();
      foreach (var (name, orderEvent) in orderEvents)
      {
        if (this.GetEventAvailable<T>(orderEvent))
        {
          events.Add(orderEvent);
        }
      }
      return events;
    }

    public bool GetEventAvailable<T>(T orderEvent) where T : EventRequirements
    {
      if (!TraitUtil.hasTrait(this.Traits, orderEvent.RequiredTraits)) return false;
      if (!TraitUtil.hasAspect(this.Traits, orderEvent.RequiredAspects)) return false;
      return true;
    }

    public List<(OrderEvent, int)> GetEventByPsycheDivergence(List<OrderEvent> orderEvents, Dictionary<string, int> modifiers)
    {
      Psyche psyche = this.GetCalculatedPsyche();
      List<(OrderEvent, int)> sortedEvents = orderEvents.Select(x =>
      {
        int divergence = psyche.GetPsycheDivergence(x.Psyche);
        if (modifiers.ContainsKey(x.Name))
        {
          divergence += modifiers[x.Name];
        }
        return (x, divergence);
      }).ToList();
      sortedEvents.Sort((x, y) => x.Item2 - y.Item2);
      return sortedEvents;
    }

    public Psyche GetCalculatedPsyche()
    {
      Dictionary<string, int> aspects = TraitUtil.getAspects(this.Traits);
      return new Psyche()
      {
        Openness = (byte)Math.Clamp(this.Psyche.Openness + aspects.GetValueOrDefault("openness") / 10, 0, 100),
        Conscientiousness = (byte)Math.Clamp(this.Psyche.Conscientiousness + aspects.GetValueOrDefault("conscientiousness") / 10, 0, 100),
        Extroversion = (byte)Math.Clamp(this.Psyche.Extroversion + aspects.GetValueOrDefault("extroversion") / 10, 0, 100),
        Agreeableness = (byte)Math.Clamp(this.Psyche.Agreeableness + aspects.GetValueOrDefault("agreeableness") / 10, 0, 100),
        Neuroticism = (byte)Math.Clamp(this.Psyche.Neuroticism + aspects.GetValueOrDefault("neuroticism") / 10, 0, 100),
      };
    }

    // ****************
    // **CHAOS_EVENTS**
    // ****************

    public (List<(ChaosEvent, int)>, int) GetEventByInfluence(List<ChaosEvent> chaosEvents)
    {
      var events = new List<(ChaosEvent, int)>();
      int totalInfluence = 0;
      foreach (var chaosEvent in chaosEvents)
      {
        int influence = 1;
        foreach (var (trait, count) in chaosEvent.InfluentialTraits)
        {
          influence += TraitUtil.getTraitCount(this.Traits, trait) * count;
        }
        var aspects = TraitUtil.getAspects(this.Traits);
        foreach (var (aspect, count) in chaosEvent.InfluentialAspects)
        {
          influence += aspects.GetValueOrDefault(aspect, 0) * count;
        }
        if (influence > 0)
        {
          events.Add((chaosEvent, influence));
          totalInfluence += influence;
        }
      }
      events.Sort((x, y) => y.Item2 - x.Item2);
      return (events, totalInfluence);
    }

    public List<(EventAction, int)> GetActionByPsycheDivergence(List<EventAction> eventActions)
    {
      Psyche psyche = this.GetCalculatedPsyche();
      var sortedEvents = eventActions.Select(x => (x, psyche.GetPsycheDivergence(x.Psyche))).ToList();
      sortedEvents.Sort((x, y) => x.Item2 - y.Item2);
      return sortedEvents;
    }

  }

}