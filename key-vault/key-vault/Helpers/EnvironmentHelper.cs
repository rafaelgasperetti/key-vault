using Newtonsoft.Json;

namespace key_vault.Helpers
{
    public class EnvironmentHelper
    {
        public static string ReadResource(string name)
        {
            var dir = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            var file = Path.Combine(dir, name);
            using StreamReader reader = new(file);
            return reader.ReadToEnd();
        }

        public static T ReadJsonResourceAs<T>(string name) where T : class
        {
            string value = ReadResource(name);
            return JsonConvert.DeserializeObject<T>(value);
        }
    }
}
