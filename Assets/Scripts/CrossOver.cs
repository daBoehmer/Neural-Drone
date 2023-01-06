using System.Linq;

public static class CrossOver
{
    public static float[] CrossItOver(float[] GeneA, float[] GeneB){

        float[] GeneC = new float[GeneA.Length];
        int n_A = (int)System.Math.Round((double)(GeneA.Length * UnityEngine.Random.Range(1/3, 2/3))); // Wieviele Elemente aus GeneA genommen werden, (1/3)*Length<=n_A<=(2/3)*Length
        int[] indicesFromA = new int[n_A];

        for(int i=0; i<n_A; i++){
            int index = UnityEngine.Random.Range(0, GeneA.Length);
            while(indicesFromA.Contains(index)){
                index = UnityEngine.Random.Range(0, GeneA.Length);
            }
            indicesFromA[i] = index;
        }

        for(int i=0; i<GeneA.Length; i++)
            if(indicesFromA.Contains(i))
                GeneC[i] = GeneA[i];
            else
                GeneC[i] = GeneB[i];

        return GeneC;
    }
}
