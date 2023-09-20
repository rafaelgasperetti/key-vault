namespace key_vault.Models.Interfaces
{
    public interface IEnvironment
    {
        public string DatabaseHost { get; set; }
        public string DatabasePort { get; set; }
        public string DatabaseUser { get; set; }
        public string DatabasePassword { get; set; }
    }
}
