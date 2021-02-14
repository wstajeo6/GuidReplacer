using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using TextReplacer.Commands;
using TextReplacer.Common.Api;
using TextReplacer.Model.Json;

namespace TextReplacer.ViewModel
{
    public class MainVM : BaseVM
    {
        #region Private Fields

        private readonly List<Configuration> configurationModels;
        private readonly IDialogProvider dialogProvider;
        private BindingList<ConfigurationVM> configurations;

        private string regexValue;
        private string resultText;
        private string sourceText;

        #endregion Private Fields

        #region Public Constructors

        public MainVM(IDialogProvider dialogProvider, List<Configuration> configurationModels)
        {
            this.dialogProvider = dialogProvider;
            this.configurationModels = configurationModels;

            EditCommand = new RelayCommand(Edit, () => CurrentConfiguration != null);
            ReloadAllCommand = new RelayCommand(ReloadAll);
            ReplaceCommand = new RelayCommand(Replace, () => !string.IsNullOrEmpty(sourceText) && CurrentConfiguration != null);

            Configurations = new BindingList<ConfigurationVM>();
            LoadConfigurationModels(configurationModels);
        }

        #endregion Public Constructors

        #region Public Properties

        private ConfigurationVM currentConfiguration;

        public BindingList<ConfigurationVM> Configurations
        {
            get => configurations;
            set => SetProperty(ref configurations, value);
        }

        public ConfigurationVM CurrentConfiguration
        {
            get => currentConfiguration;
            set => SetProperty(ref currentConfiguration, value);
        }

        public Action<ConfigurationVM> EditAction { get; set; }

        public RelayCommand EditCommand { get; }
   
        public Action<MainVM> ReloadAllAction { get; set; }

        public RelayCommand ReloadAllCommand { get; }

        public RelayCommand ReplaceCommand { get; }

        public string ResultText
        {
            get => resultText;
            set => SetProperty(ref resultText, value);
        }

        public string RegexValue
        {
            get => regexValue;
            set => SetProperty(ref regexValue, value);
        }


        public Action<List<Configuration>> SaveAction { get; set; }

        public string SourceText
        {
            get => sourceText;
            set => SetProperty(ref sourceText, value);
        }

        #endregion Public Properties

        #region Public Methods

        public void LoadConfigurationModels(List<Configuration> configurationModels)
        {
            Configurations.Clear();
            foreach (var configurationModel in configurationModels)
            {
                var configurationVM = new ConfigurationVM(dialogProvider, configurationModel);
                configurationVM.SaveAction = Save;
                Configurations.Add(configurationVM);
            }
        }

        #endregion Public Methods

        #region Private Methods

        private void Edit()
        {
            Debug.Assert(EditAction != null);
            Debug.Assert(CurrentConfiguration != null);
            EditAction.Invoke(CurrentConfiguration);
        }

        private void ReloadAll()
        {
            Debug.Assert(ReloadAllAction != null);
            ReloadAllAction.Invoke(this);
        }

        private void Replace()
        {
            CurrentConfiguration.ReplaceText(SourceText);
        }

        private void Save()
        {
            Debug.Assert(SaveAction != null);
            SaveAction.Invoke(configurationModels);
        }

        #endregion Private Methods
    }
}