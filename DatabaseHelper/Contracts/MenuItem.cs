using PropertyChanged;
using System.Windows.Controls;

namespace DatabaseHelper.Contracts
{
    [AddINotifyPropertyChangedInterface]
    public class MenuItem
    {
        public Page Content { get; set; }
        public string Icon { get; set; }
        public string PageKey { get; set; }
        public bool Selected { get; set; }
        public string Title { get; set; }
    }
}