using PropertyChanged;

namespace DatabaseHelper.Contracts
{
    [AddINotifyPropertyChangedInterface]
    public class SQLConnectionDetails
    {
        public int CommandTimeout { get; set; }
        public int ConnectTimeout { get; set; }
        public string Database { get; set; }
        public string Domain { get; set; }
        public string Password { get; set; }
        public ushort Port { get; set; }
        public string Server { get; set; }
        public bool UseActiveDirectory { get; set; }
        public bool UseIntegratedSecurity { get; set; }
        public string Username { get; set; }
        public bool UseSQLPassword { get; set; }
    }
}