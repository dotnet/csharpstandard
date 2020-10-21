using System.Collections.Generic;

namespace MarkdownConverter.Spec
{
    public class StringLengthComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x.Length > y.Length)
            {
                return -1;
            }

            if (x.Length < y.Length)
            {
                return 1;
            }

            return string.Compare(x, y);
        }
    }
}
