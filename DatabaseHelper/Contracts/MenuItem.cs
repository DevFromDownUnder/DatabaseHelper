using PropertyChanged;

namespace DatabaseHelper.Contracts
{
    [AddINotifyPropertyChangedInterface]
    public class MenuItem
    {
        public string PagePath { get; set; }
        public string Icon { get; set; }
        public string Title { get; set; }
        public bool Selected { get; set; }
    }
}