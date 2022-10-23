using System.Collections.Generic;

namespace WarClub;

class EventLog
{
  public Queue<(string, ID)> Log = new Queue<(string, ID)>();

  public void AddEvent(string message, ID id)
  {
    Log.Enqueue((message, id));
  }
}