using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TextReplacer.Common.Api;
using TextReplacer.Common.Extensions;
using TextReplacer.Common.Implementation;
using TextReplacer.Model.Json;
using TextReplacer.View;
using TextReplacer.ViewModel;

namespace TextReplacer
{
    public class TextReplacerCore
    {
        #region Private Fields

        private const string APP_NAME = "TextReplacer";
        private const string CONFIG_FILE_NAME = APP_NAME + "Configuration.json";
        private readonly IDialogProvider dialogProvider;
        private readonly string currentDir;
        private readonly string configFilePath;

        #endregion

        #region Private Constructors

        private TextReplacerCore()
        {
            dialogProvider = new DialogProvider();
            currentDir = Assembly.GetExecutingAssembly().GetDirectory();
            configFilePath = Path.Combine(currentDir, CONFIG_FILE_NAME);
        }

        #endregion

        #region Public Methods

        public static void Run() =>
            new TextReplacerCore().RunProtected();

        #endregion

        #region Private Methods

        private TextReplacerConfiguration LoadConfigurationFile()
        {
            try
            {
                var configText = File.ReadAllText(configFilePath);
                return JsonConvert.DeserializeObject<TextReplacerConfiguration>(configText);
            }
            catch (Exception ex)
            {
                dialogProvider.ShowMessage(ex.Message, "Error loading configuration file", DialogIcon.Error);
                throw ex;
            }
        }

        private void RunProtected()
        {
            var config = LoadConfigurationFile();
            var mainVM = new MainVM(dialogProvider, config.Configurations)
            {
                SaveAction = Save,
                EditAction = Edit,
                ReloadAllAction = ReloadAll
            };
            var mainWindow = new MainWindow();
            mainWindow.Initialize(mainVM);
            mainWindow.ShowDialog();
        }

        private void ReloadAll(MainVM mainVM)
        {
            var config = LoadConfigurationFile();
            mainVM.LoadConfigurationModels(config.Configurations);
        }

        private void Edit(ConfigurationVM configurationVM)
        {
            var editConfigurationWindow = new EditConfigurationWindow();
            configurationVM.CancelAction = editConfigurationWindow.Close;

            editConfigurationWindow.Initialize(configurationVM);
            editConfigurationWindow.ShowDialog();

            configurationVM.CancelAction = null;
        }

        private void Save(List<Configuration> configurations)
        {
            try
            {
                var textReplacerConfiguration = new TextReplacerConfiguration
                {
                    Configurations = new List<Configuration>(configurations)
                };

                JsonConvert.SerializeObject(
                    value: textReplacerConfiguration,
                    type: typeof(TextReplacerConfiguration),
                    formatting: Formatting.Indented,
                    settings: null);
            }
            catch (Exception ex)
            {
                dialogProvider.ShowMessage(ex.Message, "Error saving configuration file", DialogIcon.Error);
            }
        }

        #endregion
    }
}