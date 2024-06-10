using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
namespace Fileshard.Frontend
{
    /// <summary>
    /// Interaction logic for NewCollectionDialog.xaml
    /// </summary>
    public partial class NewCollectionDialog : Window
    {
        public NewCollectionDialog()
        {
            InitializeComponent();
        }

        public string ResponseText
        {
            get { return txtResponse.Text; }
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void TxtResponse_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("^[a-zA-Z0-9_]*$"); // Only letters, numbers, and underscores
            return regex.IsMatch(text);
        }

        private void TxtResponse_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
    }
}
