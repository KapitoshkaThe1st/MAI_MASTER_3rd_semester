using System.Windows.Controls;

namespace RSA_Demo
{
    public partial class KeyGenerationView : UserControl
    {
        public KeyGenerationView()
        {
            InitializeComponent();

            DataContext = new KeyGenerationViewModel();
        }
    }
}
