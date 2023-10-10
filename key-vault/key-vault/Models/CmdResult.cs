namespace key_vault.Models
{
    public class CmdResult
    {
        public bool Success { get; set; }
        public int ExitCode { get; set; }
        public DateTime ExitTime { get; set; }
        public string Message { get; set; }
    }
}
