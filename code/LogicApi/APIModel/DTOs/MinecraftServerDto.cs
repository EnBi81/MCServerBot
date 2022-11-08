namespace APIModel.DTOs
{
    public class MinecraftServerDTO
    {
        public const int NAME_MAX_LENGTH = 35;
        public const int NAME_MIN_LENGTH = 4;



        public long Id { get; set; }
        public string ServerName { get; set; }
        public int Status { get; set; }
        public IEnumerable<LogMessageDto> LogMessages { get; set; }
        public DateTime? OnlineFrom { get; set; }
        public int Port { get; set; }
        public IEnumerable<MinecraftPlayerDTO> Players { get; set; }
        public long StorageBytes { get; set; }
        
    }
}
