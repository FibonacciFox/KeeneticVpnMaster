namespace KeeneticVpnMaster.Models
{
    public class AuthConfig
    {
        public string IpAddress { get; set; } = "192.168.1.1";
        public string Username { get; set; } = "admin";
        public string Password { get; set; } = "admin";
        public bool SaveAuth { get; set; } = false;
    }
}