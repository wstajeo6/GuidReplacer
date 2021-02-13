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

        private readonly IDialogProvider dialogProvider;

        private readonly List<Configuration> configurationModels;
        private BindingList<ConfigurationVM> configurations;

        private string sourceText;

        private string regexValue;

        private string resultText;

        #endregion

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

        #endregion

        #region Public Properties

        public string SourceText
        {
            get => sourceText;
            set => SetProperty(ref sourceText, value);
        }

        public string RegexValue
        {
            get => regexValue;
            set => SetProperty(ref regexValue, value);
        }

        public string ResultText
        {
            get => resultText;
            set => SetProperty(ref resultText, value);
        }

        public ConfigurationVM CurrentConfiguration { get; }

        public Action<ConfigurationVM> EditAction { get; set; }

        public Action<MainVM> ReloadAllAction { get; set; }

        public RelayCommand EditCommand { get; }

        public RelayCommand ReloadAllCommand { get; }

        public Action<List<Configuration>> SaveAction { get; set; }

        public BindingList<ConfigurationVM> Configurations
        {
            get => configurations;
            set => SetProperty(ref configurations, value);
        }

        public RelayCommand ReplaceCommand { get; }

        #endregion

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

        #endregion

        #region Private Methods

        private void ReloadAll()
        {
            Debug.Assert(ReloadAllAction != null);
            ReloadAllAction.Invoke(this);
        }

        private void Replace()
        {
            CurrentConfiguration.ReplaceText(SourceText);
        }

        private void Edit()
        {
            Debug.Assert(EditAction != null);
            Debug.Assert(CurrentConfiguration != null);
            EditAction.Invoke(CurrentConfiguration);
        }

        private void Save()
        {
            Debug.Assert(SaveAction != null);
            SaveAction.Invoke(configurationModels);
        }

        #endregion
    }
}