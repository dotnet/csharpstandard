namespace AccessWithCreation
{
    internal class AccessWithCreationSample
    {
        static void ElementAccess()
        {
            var x0 = new string[4][2];        // semantic error
            var x = (new string[4])[2];

            var y = new string[4] { "ant", "bat", "cat", "dog" }[2];
            var z = new string[] { "ant", "bat", "cat", "dog" }[3];

            var s = stackalloc int[4];        // syntactically valid but only valid in unsafe context
            Span<int> s = stackalloc int[4];  // valid in safe context
            int t = s[2];
            int u = s?[1];                    // syntactically valid but semantically invalid
                                              // as Span<T> is not a nullable

            var p0 = stackalloc int[4][2];    // semantic error
            var p = (stackalloc int[4])[2];
            var q = stackalloc int[4] { 0, 1, 2, 3 }[3];
        }

        static void NullConditionalElementAccess()
        {
            var x0 = new string[4]?[2];       // semantic error
            var x = (new string[4])?[2];

            var y = new string[4] { "ant", "bat", "cat", "dog" }?[2];
            var z = new string[] { "ant", "bat", "cat", "dog" }?[3];

            // safe context, stackalloc is Span<T>
            var p0 = stackalloc int[4]?[2];   // semantic error
            var p = (stackalloc int[4])?[2];  // type error?
            var q = stackalloc int[4] { 0, 1, 2, 3 }?[3];   // type error?
        }

        static void UnsafeElementAccess()
        {
            unsafe
            {
                // Memory uninitialized
                int* p = stackalloc int[3];        // stackalloc type is int*
                int n = p[2];

                var x0 = stackalloc int[3][2];     // semantic error
                var x = (stackalloc int[3])[2];    // stackalloc type is Span<int>

                // Memory initialized
                int *q = stackalloc int[3] { -10, -15, -30 };     // stackalloc type is int*

                var y = stackalloc int[3] { -10, -15, -30 }[2];   // stackalloc type is Span<int>
            }
        }
    }
}