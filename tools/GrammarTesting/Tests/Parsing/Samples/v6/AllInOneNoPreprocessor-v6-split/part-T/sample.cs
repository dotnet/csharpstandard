namespace Comments.XmlComments.UndocumentedKeywords
{
    // From here:https://github.com/dotnet/roslyn/wiki/New-Language-Features-in-C%23-6
    class CSharp6Features
    {
        async void Test()
        {
            // Index initializers
            var numbers = new Dictionary<int, string> {
                [7] = "seven",
                [9] = "nine",
                [13] = "thirteen"
            };
            
            // Exception filters
            try {}
            catch (MyException e) when (myfilter(e))
            { }
            
            // Await in catch and finally blocks
            Resource res = null;
            try
            {
                res = await Resource.OpenAsync();       // You could do this.
            } 
            catch(ResourceException e)
            {
                await Resource.LogAsync(res, e);         // Now you can do this …
            }
            finally
            {
                if (res != null)
                    await res.CloseAsync(); // … and this.
            }
        }
    }
}