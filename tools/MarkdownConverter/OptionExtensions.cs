using Microsoft.FSharp.Core;

namespace MarkdownConverter
{
    internal static class OptionExtensions
    {
        public static T Option<T>(this FSharpOption<T> o) where T : class
        {
            if (FSharpOption<T>.GetTag(o) == FSharpOption<T>.Tags.None)
            {
                return null;
            }

            return o.Value;
        }
    }
}
