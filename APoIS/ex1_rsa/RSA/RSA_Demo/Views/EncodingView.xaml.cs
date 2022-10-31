using System.Windows.Controls;

namespace RSA_Demo
{
    /// <summary>
    /// Interaction logic for EncodingView.xaml
    /// </summary>
    public partial class EncodingView : UserControl
    {
        public EncodingView()
        {
            InitializeComponent();

            DataContext = new EncodingViewModel();
        }
    }
}
