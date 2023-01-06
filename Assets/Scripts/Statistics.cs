using System.Collections.Generic;

public class Statistics
{
    public Dictionary<DeathCause, int> DeathCauses;

    public Statistics(){
        DeathCauses = new Dictionary<DeathCause, int>();
        DeathCauses.Add(DeathCause.Crash, 0);
        DeathCauses.Add(DeathCause.NoEnergy, 0);
    }

}
