using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FFI.Helpers
{
    internal static class JsonFile
    {
        public static async Task<T> LoadAsync<T>(string file)
        {
            using (Stream utf8Json = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                return await JsonSerializer.DeserializeAsync<T>(utf8Json);
            }
        }

        public static async Task SaveAsync(object value, string file)
        {
            using (Stream utf8Json = new FileStream(file, FileMode.Create, FileAccess.Write))
            {
                JsonSerializerOptions options = new JsonSerializerOptions()
                {
                    WriteIndented = true
                };
                await JsonSerializer.SerializeAsync(utf8Json, value, options);
            }
        }
    }
}
