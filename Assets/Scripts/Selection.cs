using System.Linq;
using System;

public static class Selection
{
    public static int[] Select(float[] FitnessArray){
        int ParentAmount, RandomAmount;
        ParentAmount = Controller.Instance.ParentAmount;
        RandomAmount = UnityEngine.Mathf.RoundToInt((1/10)*MathF.E*ParentAmount);

        // Erstelle ein Array mit den Indizes von FitnessArray
        int[] indices = Enumerable.Range(0, FitnessArray.Length).ToArray();

        // Sortiere das Array nach Fitness-Werten in aufsteigender Reihenfolge
        Array.Sort(FitnessArray, indices);

        // Nehme die höchsten ParentAmount Werte aus dem sortierten Array
        int[] parents = new int[ParentAmount];
        Array.Copy(indices, indices.Length - ParentAmount, parents, 0, ParentAmount);

        // Füge RandomAmount zufällige Indizes hinzu
        Random rnd = new Random();
        while (RandomAmount > 0)
        {
            int randomIndex = rnd.Next(0, indices.Length);
            if (!parents.Contains(randomIndex))
            {
                parents[ParentAmount - RandomAmount] = randomIndex;
                RandomAmount--;
            }
        }

        return parents;
    }
}
