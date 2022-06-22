using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFI.Helpers
{
    internal static class ArgsParser
    {
        public static Dictionary<string, string> Parse(string[] args)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Trim().StartsWith("--"))
                {
                    string name = args[i];
                    string value = null;

                    if (!args[i + 1].Trim().StartsWith("--"))
                    {
                        value = args[i + 1];
                        i++;
                    }

                    result.Add(name, value);
                }
            }

            return result;
        }
    }
}
