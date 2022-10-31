using System.Windows.Controls;

namespace RSA_Demo
{
    /// <summary>
    /// Interaction logic for ModulusFactorizationAttackView.xaml
    /// </summary>
    public partial class AttackView : UserControl
    {
        public AttackView()
        {
            InitializeComponent();

            DataContext = new AttackViewModel();
        }
    }
}
