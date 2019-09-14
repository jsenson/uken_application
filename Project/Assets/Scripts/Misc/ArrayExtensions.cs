using System;

public static class ArrayExtensions {
    public static void Shuffle<T>(this T[] array) {
        Random rng = new Random();
        int n = array.Length;

        while(n > 1) {
            int k = rng.Next(n--);
            T tmp = array[n];
            array[n] = array[k];
            array[k] = tmp;
        }
    }
}
