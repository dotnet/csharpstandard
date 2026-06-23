class Sample
{
    public void Test()
    {
        // Rank 1, element type int
        int[] array1 = null;
        int element1 = array1[0];

        // Rank 2, element type int
        int[,] array2 = null;
        int element2 = array2[0, 1];

        // Rank 1, element type int? (Nullable<int>)
        int?[] array3 = null;
        int? element3 = array3[0];

        // Rank 1, element type string? (nullable string)
        string?[] array4 = null;
        string? element4 = array4[0];

        // Rank 1, element type string[,,][,]
        string[][,,][,] array5 = null;
        string[,,][,] element5 = array5[0];

        // Rank 1, element type string; the array itself is nullable
        string[]? array6 = null;
        string element6 = array6?[0] ?? "";

        // Rank 1, element type string[,]?
        string[,]?[] array7 = null;
        string[,]? element7 = array7[0];

        // Rank 3, element type int[]?[,]
        int[]?[,,][,] array8 = null;
        int[]?[,] element8 = array8[0, 1, 2];

        // Rank 1, element type string[,]?[]?[,,]
        string[,]?[]?[][,,] array9 = null;
        string[,]?[]?[,,] element9 = array9[0];

        // Rank 2, element type string[][][,,]
        // Note that this appears the same as the array9 example above other
        // than for the use of ? but the rank and element type are significantly different.
        string[,][][][,,] array10 = null;
        string[][][,,] element10 = array10[0, 1];
    }
}