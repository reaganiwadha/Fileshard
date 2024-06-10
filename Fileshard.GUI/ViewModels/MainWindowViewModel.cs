using Prism.Commands;
using Prism.Mvvm;

namespace Fileshard.GUI.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        public DelegateCommand DisplayCommand { get; private set; }

        public MainWindowViewModel()
        {
            DisplayCommand = new DelegateCommand(Display, CanDisplay);
        }

        private string _updateText;
        public string UpdateText
        {
            get { return _updateText; }
            set { SetProperty(ref _updateText, value); }
        }

        public void Display()
        {
            UpdateText = "Hello, World!";
            /*Console.WriteLine("Hello, World!");*/
        }

        bool CanDisplay()
        {
            return true;
        }
    }
}
