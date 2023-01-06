using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Mutation
{
    public static float[] Mutate(float[] Gene, float MutationRate){
        MutationRate /= 2; // Damit eine gesamte MutationRate für beide Mutationsfunktionen angegeben werden kann
        int mutationAmount = Mathf.RoundToInt(Gene.Length*MutationRate);

        for(int i=0; i<Mathf.RoundToInt(Gene.Length*MutationRate); i++)
            Gene[Random.Range(0, Gene.Length)] *= Random.Range(-1.2f, 1.2f);

        for(int i=0; i<Mathf.RoundToInt(Gene.Length*MutationRate); i++)
            Gene[Random.Range(0, Gene.Length)] = Random.Range(-0.5f, 0.5f);

        return Gene;
    }
}
