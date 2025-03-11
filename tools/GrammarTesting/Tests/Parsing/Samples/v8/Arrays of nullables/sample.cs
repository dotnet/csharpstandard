class ArraysOfNullable
{
   class A { }

   void ArraysOfNullable()
   {
      A[] p_1 = new A[5];    // Array of non-nullable A all elements inited to null (oops)
      A[]? q_1 = null;       // Nullable array of non-nullable A, array inited to null
      A?[] r_1 = new A[5];   // Array of nullable A all elements inited to null
      A?[]? s_1 = null;      // Nullable array of nullable A, array inited to null

      string[] p_2 = new string[5];   // Array of non-nullable string all elements inited to null (oops)
      string[]? q_2 = null;           // Nullable array of non-nullable string, array inited to null
      string?[] r_2 = new string[5];  // Array of nullable string all elements inited to null
      string?[]? s_2 = null;          // Nullable array of nullable string, array inited to null

      int[] p_3 = new int[5];   // Array of non-nullable int all elements inited to 0
      int[]? q_3 = null;        // Nullable array of non-nullable int, array inited to null
      int?[] r_3 = new int[5];  // Array of nullable int all elements inited to null
      int?[]? s_3 = null;       // Nullable array of nullable int, array inited to null

      dynamic[] p_3 = new dynamic[5];  // Array of non-nullable dynamic all elements inited to 0
      dynamic[]? q_3 = null;           // Nullable array of non-nullable dynamic, array inited to null
      dynamic?[] r_3 = new dynamic[5]; // Array of nullable dynamic all elements inited to null
      dynamic?[]? s_3 = null;          // Nullable array of nullable dynamic, array inited to null
   }
}