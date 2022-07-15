using System;

namespace WarClub
{
  public static class Program
  {
    [STAThread]
    static void Main()
    {
      using (var game = new WarClub())
        game.Run();
    }
  }
}
