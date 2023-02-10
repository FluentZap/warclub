using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace WarClub;

class KeyState
{
  HashSet<Keys> persist = new HashSet<Keys>()
  {
    Keys.LeftShift,
    Keys.RightShift,
  };

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

  public HashSet<Keys> GetTriggeredKeys(bool persist = false)
  {
    var triggeredKeys = triggered.ToHashSet();
    if (!persist) triggered.Clear();

    return triggeredKeys;
  }
}