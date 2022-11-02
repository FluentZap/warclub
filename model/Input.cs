using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace WarClub;

class KeyState
{
  HashSet<Keys> exhausted = new HashSet<Keys>();
  HashSet<Keys> triggered = new HashSet<Keys>();

  public void SetKeys(Keys[] pressed)
  {
    foreach (var key in pressed)
    {
      if (!exhausted.Contains(key))
      {
        triggered.Add(key);
        exhausted.Add(key);
      }
    }

    var pressedHash = new HashSet<Keys>(pressed);
    foreach (var key in exhausted.ToList())
    {
      if (!pressedHash.Contains(key))
      {
        exhausted.Remove(key);
      }
    }
  }

  public HashSet<Keys> GetTriggeredKeys()
  {
    var triggeredKeys = triggered.ToHashSet();
    triggered.Clear();
    return triggeredKeys;
  }
}