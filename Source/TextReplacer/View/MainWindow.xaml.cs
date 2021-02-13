using System;
using System.Windows;
using TextReplacer.ViewModel;

namespace TextReplacer.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Public Constructors

        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Public Methods

        public void Initialize(MainVM vm)
        {
            DataContext = vm ?? throw new ArgumentNullException(nameof(vm));
        }

        #endregion
    }
}