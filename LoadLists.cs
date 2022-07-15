using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace WarClub
{

  static class FileLoading
  {
    public static Dictionary<string, int> ParseStrengthList(string list, int baseValue = 1)
    {
      Dictionary<string, int> sl = new Dictionary<string, int>();
      foreach (string item in list.Trim().Split(','))
      {
        if (item == "") continue;
        if (item.Contains('_'))
        {
          string[] values = item.Split('_');
          string name = values[1].Trim();
          int strength = int.Parse(values[0]);
          if (!sl.ContainsKey(values[1]))
          {
            sl.Add(name, strength);
          }
          else
          {
            sl[name] += strength;
          }
        }
        else
        {
          var returnItem = item.Trim();
          var value = baseValue;
          if (returnItem.Contains('-'))
          {
            returnItem = returnItem.Substring(1);
            value = -value;
          }
          sl.Add(returnItem, value);
        }
      }
      return sl;
    }
  }

  partial class Simulation
  {
    void LoadLists()
    {
      var traitList = Loader.CSVLoadFile(Path.Combine("./", "Traits.csv"));
      traitList.RemoveAt(0);
      foreach (string[] r in traitList)
      {
        if (r.Length < 3 || r[0][0] == '*')
          continue;

        var types = r[1].Split(',').Select(x => x.Trim()).ToList();
        types.ForEach(x => TraitLists.TryAdd(x, new TraitList()));

        Dictionary<string, int> modifiers = FileLoading.ParseStrengthList(r[2]);

        Trait newTrait = new Trait()
        {
          Name = r[0].Trim(),
          Type = types,
          Aspects = modifiers,
        };

        Traits.Add(r[0].Trim(), newTrait);
        types.ForEach(x => TraitLists[x].Add(newTrait.Name, newTrait));
      }

      // add default trait lists
      TraitLists.Add("Species", new TraitList());

      // Add Order Events
      var orderEventList = Loader.CSVLoadFile(Path.Combine("./", "OrderEvents.csv"));
      orderEventList.RemoveAt(0);
      foreach (string[] r in orderEventList)
      {
        if (r.Length < 8 || r[0][0] == '*')
          continue;

        var allowedEntityType = r[1].Split(',').Select(x => x.Trim()).ToList();
        allowedEntityType.ForEach(x => { if (x != "") OrderEventLists.TryAdd(x, new Dictionary<string, OrderEvent>()); });

        var newEvent = new OrderEvent()
        {
          Name = r[0].Trim(),
          AllowedEntityType = allowedEntityType,
          RequiredTraits = FileLoading.ParseStrengthList(r[2]).ToDictionary(x => Traits[x.Key], x => x.Value),
          RequiredAspects = FileLoading.ParseStrengthList(r[3]),
          Psyche = new Psyche()
          {
            Openness = byte.Parse(r[4]),
            Conscientiousness = byte.Parse(r[5]),
            Extroversion = byte.Parse(r[6]),
            Agreeableness = byte.Parse(r[7]),
            Neuroticism = byte.Parse(r[8]),
          }
        };
        OrderEvents.Add(newEvent.Name, newEvent);
        allowedEntityType.ForEach(x => OrderEventLists[x].Add(newEvent.Name, newEvent));
      }


      var chaosEventList = Loader.CSVLoadFile(Path.Combine("./", "ChaosEvents.csv"));
      chaosEventList.RemoveAt(0);

      ChaosEvent lastEvent = null;

      foreach (string[] r in chaosEventList)
      {
        if (r.Length < 11 || r[0][0] == '*')
          continue;

        var allowedEntityType = r[1].Split(',').Select(x => x.Trim()).ToList();
        allowedEntityType.ForEach(x => { if (x != "") ChaosEventLists.TryAdd(x, new Dictionary<string, ChaosEvent>()); });
        if (r[0][0] == '_')
        {
          var newAction = new EventAction()
          {
            Name = r[0].Trim().Substring(1),
            RequiredTraits = FileLoading.ParseStrengthList(r[2]).ToDictionary(x => Traits[x.Key], x => x.Value),
            RequiredAspects = FileLoading.ParseStrengthList(r[3]),
          };

          byte.TryParse(r[6], out byte Openness);
          byte.TryParse(r[7], out byte Conscientiousness);
          byte.TryParse(r[8], out byte Extroversion);
          byte.TryParse(r[9], out byte Agreeableness);
          byte.TryParse(r[10], out byte Neuroticism);

          newAction.Psyche = new Psyche()
          {
            Openness = byte.Parse(r[6]),
            Conscientiousness = byte.Parse(r[7]),
            Extroversion = byte.Parse(r[8]),
            Agreeableness = byte.Parse(r[9]),
            Neuroticism = byte.Parse(r[10]),
          };

          if (lastEvent != null) lastEvent.EventActions.Add(newAction);
        }
        else
        {
          var newEvent = new ChaosEvent()
          {
            Name = r[0].Trim(),
            AllowedEntityType = allowedEntityType,
            RequiredTraits = FileLoading.ParseStrengthList(r[2]).ToDictionary(x => Traits[x.Key], x => x.Value),
            RequiredAspects = FileLoading.ParseStrengthList(r[3]),
            InfluentialTraits = FileLoading.ParseStrengthList(r[4]).ToDictionary(x => Traits[x.Key], x => x.Value),
            InfluentialAspects = FileLoading.ParseStrengthList(r[5]),
          };
          ChaosEvents.Add(newEvent.Name, newEvent);
          allowedEntityType.ForEach(x => ChaosEventLists[x].Add(newEvent.Name, newEvent));
          lastEvent = newEvent;
        }
      }
    }
  }
}
