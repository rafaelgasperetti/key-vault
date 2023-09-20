using System.Text;

namespace key_vault.Helpers
{
    public static class Helper
    {
        public static long GetCurrentTimestamp(this DateTime dateTime)
        {
            return (long) (dateTime - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static string RandomString(int size = 12, bool lowerCase = true)
        {
            StringBuilder builder = new();
            Random random = new();

            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
            {
                return builder.ToString().ToLower();
            }

            return builder.ToString();
        }

        public static int RandomInt(int min = 0, int max = int.MaxValue)
        {
            Random random = new();
            return random.Next(min, max);
        }
    }
}
