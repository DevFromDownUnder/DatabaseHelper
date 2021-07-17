using PropertyChanged;
using System;

namespace DatabaseHelper.Contracts
{
    [AddINotifyPropertyChangedInterface]
    public class SQLServer
    {
        public ushort Port { get; set; }
        public string Server { get; set; }

        #region "Equality logic based on Server"

        public override bool Equals(object obj)
        {
            if (obj is SQLServer other)
            {
                return string.Equals(this.Server, other.Server, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Server.GetHashCode();
        }

        public override string ToString()
        {
            return Server;
        }

        #endregion "Equality logic based on Server"
    }
}