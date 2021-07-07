using PropertyChanged;
using System;

namespace DatabaseHelper.Contracts
{
    [AddINotifyPropertyChangedInterface]
    public class SQLServer
    {
        public string Server { get; set; }
        public ushort Port { get; set; }

        #region "Equality logic based on Server"

        public override string ToString()
        {
            return Server;
        }

        public override bool Equals(object obj)
        {
            if (obj is SQLServer other)
            {
                return String.Equals(this.Server, other.Server, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Server.GetHashCode();
        }

        #endregion "Equality logic based on Server"
    }
}