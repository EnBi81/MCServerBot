namespace HamachiCli
{
    /// <summary>
    /// Data class for representing Hamachi Status
    /// </summary>
    public class HamachiStatus
    { 
        /// <summary>
        /// Hamachi Username.
        /// </summary>
        public string NickName { get; init; }
        /// <summary>
        /// Ip address of the current hamachi machine.
        /// </summary>
        public string Address { get; init; }
        /// <summary>
        /// Returns if Hamachi is connected.
        /// </summary>
        public bool Online { get; init; }
    }
}
