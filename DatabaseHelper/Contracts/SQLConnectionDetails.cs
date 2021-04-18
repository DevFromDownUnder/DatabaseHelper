using PropertyChanged;

namespace DatabaseHelper.Contracts
{
    [AddINotifyPropertyChangedInterface]
    public class SQLConnectionDetails
    {
        public string Server { get; set; }
        public string Database { get; set; }
        public string Domain { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool UseSQLPassword { get; set; }
        public bool UseActiveDirectory { get; set; }
        public bool UseIntegratedSecurity { get; set; }
    }
}