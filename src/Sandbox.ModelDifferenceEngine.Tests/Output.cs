using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Sandbox.ModelDifferenceEngine.Tests
{
    public static class Output
    {
     public  static void WriteLine(IEnumerable<ModelChange> results)
        {
            foreach (var result in results)
            {
                WriteLine(result);
            }
        }

     public static void WriteLine(ModelChange result)
        {
            var settings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };

            Console.WriteLine("{0}: {1}=>{2}", result.Path,
                              JsonConvert.SerializeObject(result.OldValue, settings),
                              JsonConvert.SerializeObject(result.Value, settings)
                );
        }
    }
}