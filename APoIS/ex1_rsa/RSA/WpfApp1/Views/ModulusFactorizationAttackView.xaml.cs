using System.Windows.Controls;

namespace RSA_Demo
{
    /// <summary>
    /// Interaction logic for ModulusFactorizationAttackView.xaml
    /// </summary>
    public partial class ModulusFactorizationAttackView : UserControl
    {
        public ModulusFactorizationAttackView()
        {
            InitializeComponent();

            DataContext = new ModulusFactorizationAttackViewModel();
        }
    }
}
