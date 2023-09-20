namespace key_vault.Helpers
{
    public static class Helper
    {
        public static long GetCurrentTimestamp(this DateTime dateTime)
        {
            return (long) (dateTime - new DateTime(1970, 1, 1)).TotalSeconds;
        }
    }
}
