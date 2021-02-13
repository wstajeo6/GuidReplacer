using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TextReplacer.ViewModel;

namespace TextReplacer.View
{
    /// <summary>
    /// Interaction logic for EditConfiguration.xaml
    /// </summary>
    public partial class EditConfigurationWindow : Window
    {
        public EditConfigurationWindow()
        {
            InitializeComponent();
        }

        public void Initialize(ConfigurationVM vm)
        {
            DataContext = vm ?? throw new ArgumentNullException(nameof(vm));
        }
    }
}
