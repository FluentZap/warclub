using System.Linq;
using System.Collections.Generic;

namespace WarClub
{

  class Trait
  {
    public string Name;
    public List<string> Type { get; set; }
    public Dictionary<string, int> Aspects { get; set; }
  }

  static class TraitUtil
  {
    public static Dictionary<string, int> getAspects(Dictionary<Trait, int> traits)
    {
      Dictionary<string, int> traitPower = new Dictionary<string, int>();
      foreach (var (t, count) in traits)
        if (t.Aspects != null)
          foreach (KeyValuePair<string, int> m in t.Aspects)
            if (!traitPower.TryAdd(m.Key, m.Value * count))
              traitPower[m.Key] += m.Value * count;
      return traitPower;
    }

    public static int getAspect(Dictionary<Trait, int> traits, string name)
    {
      Dictionary<string, int> aspects = TraitUtil.getAspects(traits);
      return aspects.ContainsKey(name) ? aspects[name] : 0;
    }

    public static Dictionary<Trait, int> getTraitsByType(Dictionary<Trait, int> traits, string type)
    {
      var newTraits = new Dictionary<Trait, int>();
      foreach (var (trait, strength) in traits)
        if (trait.Type.Contains(type))
          newTraits.Add(trait, strength);
      return newTraits;
    }

    public static bool hasTrait(Dictionary<Trait, int> traits, Trait t, int count = 1)
    {
      return traits.ContainsKey(t) && traits[t] >= count;
    }

    public static bool hasTrait(Dictionary<Trait, int> traits, Dictionary<Trait, int> requiredTraits)
    {
      foreach (var (trait, count) in requiredTraits)
        if (!TraitUtil.hasTrait(traits, trait, count))
          return false;
      return true;
    }

    public static int getTraitCount(Dictionary<Trait, int> traits, Trait t)
    {
      if (traits.ContainsKey(t))
        return traits[t];
      return 0;
    }

    public static int getTraitCount(Dictionary<Trait, int> traits, string t)
    {
      // return traits.Where(x => x.Key.Name == t).FirstOrDefault().Value;
      return traits.FirstOrDefault(x => x.Key.Name == t).Value;
    }

    public static bool hasAspect(Dictionary<Trait, int> traits, string name, int count)
    {
      Dictionary<string, int> aspects = TraitUtil.getAspects(traits);
      return aspects.ContainsKey(name) && aspects[name] >= count;
    }

    public static bool hasAspect(Dictionary<Trait, int> traits, Dictionary<string, int> requiredAspects)
    {
      Dictionary<string, int> aspects = TraitUtil.getAspects(traits);
      foreach (var (name, count) in requiredAspects)
        if (!(aspects.ContainsKey(name) && aspects[name] >= count))
          return false;
      return true;
    }

    public static Dictionary<Trait, int> combineTraits(List<Dictionary<Trait, int>> traitsLists)
    {
      var combinedTraits = new Dictionary<Trait, int>();
      foreach (var traits in traitsLists)
        foreach (var (trait, strength) in traits)
          combinedTraits.Add(trait, strength);
      return combinedTraits;
    }

  }

}