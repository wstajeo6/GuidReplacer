using System.Windows;

namespace TextReplacer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Protected Methods

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            TextReplacerCore.Run();
        }

        #endregion
    }
}