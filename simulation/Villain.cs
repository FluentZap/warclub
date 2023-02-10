using System;

namespace WarClub;

static class Villain
{

  public static string GetMovement(Simulation s, ActiveUnit u)
  {
    var attack = RNG.PickFrom(s.MissionState.PCUnits);
    switch (u.Job)
    {
      case UnitArchetypeJob.Brawler:
        return "Moves to nearest enemy!";
      case UnitArchetypeJob.Eliminator:
        return $"Get into position to attack {attack.BaseUnit.DataSheet.Name}!";
      case UnitArchetypeJob.Attrition:
      default:
        return "Move closer to nearest ally unit!";
    }
  }

  public static string GetShooting(Simulation s, ActiveUnit u)
  {
    var shoot = RNG.PickFrom(s.MissionState.PCUnits);
    switch (u.Job)
    {
      case UnitArchetypeJob.Brawler:
        return "Shoot closest!";
      case UnitArchetypeJob.Eliminator:
        return $"Shoot closest or {shoot.BaseUnit.DataSheet.Name}!";
      case UnitArchetypeJob.Attrition:
      default:
        return "Shoot closest unit to ally!";
    }
  }

  public static string GetCharge(Simulation s, ActiveUnit u)
  {
    var charge = RNG.PickFrom(s.MissionState.PCUnits);
    switch (u.Job)
    {
      case UnitArchetypeJob.Brawler:
        return $"Charge closest or {charge.BaseUnit.DataSheet.Name}!";
      case UnitArchetypeJob.Eliminator:
        return $"If unit is at half strength/wounds Charge!";
      case UnitArchetypeJob.Attrition:
      default:
        return "support or charge in with team mate!";
    }
  }

}
