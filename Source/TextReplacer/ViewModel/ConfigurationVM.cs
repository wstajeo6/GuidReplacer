using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using TextReplacer.Commands;
using TextReplacer.Common.Api;
using TextReplacer.Model;
using TextReplacer.Model.Json;

namespace TextReplacer.ViewModel
{
    public class ConfigurationVM : BaseVM
    {
        #region Private Fields

        private readonly IDialogProvider dialogProvider;

        private readonly Configuration configurationModel;
        private string name;
        private string textScript;

        

        #endregion

        #region Public Constructors

        public ConfigurationVM(IDialogProvider dialogProvider, Configuration configurationModel)
        {
            TestCompileCommand = new RelayCommand(TestCompile, ValidateTextScript);
            SaveCommand = new RelayCommand(Save, () => ValidateName() && ValidateTextScript());
            CancelCommand = new RelayCommand(Cancel);
            this.dialogProvider = dialogProvider;
            this.configurationModel = configurationModel;
            name = configurationModel.Name;
            textScript = configurationModel.TextScript; 
        }

        #endregion

        #region Public Properties

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public string TextScript
        {
            get => textScript;
            set => SetProperty(ref textScript, value);
        }

        public RelayCommand TestCompileCommand { get; set; }

        public RelayCommand SaveCommand { get; }

        public RelayCommand CancelCommand { get; }

        public Action SaveAction { get; set; }

        public Action CloseAction { get; set; }

        private ScriptProvider<TextReplaceScriptGlobals, string> scriptProvider;

        #endregion

        #region Public Methods

        public string ReplaceText(string sourceText)
        {
            try
            {
                var globals = new TextReplaceScriptGlobals(sourceText);
                var runResponse = scriptProvider.Run(globals).Result;

                if (runResponse.Exception != null)
                    throw runResponse.Exception;

                return runResponse.Result;
            }
            catch (Exception ex)
            {
                dialogProvider.ShowMessage(ex.Message, "Script run error", DialogIcon.Error);
                return null;
            }
        }

        #endregion

        #region Protected Methods

        protected override void OnPropertyChanged(string name)
        {
            switch (name)
            {
                default:
                    break;
            }
            base.OnPropertyChanged(name);
        }

        protected bool ValidateName() => 
            !string.IsNullOrWhiteSpace(Name);

        protected bool ValidateTextScript() => 
            !string.IsNullOrWhiteSpace(TextScript);

        protected void TestCompile()
        {
            var scriptProvider = new ScriptProvider<TextReplaceScriptGlobals, string>(TextScript);
            var compileResponse = scriptProvider.Compile().Result;

            if(compileResponse.Exception != null)
                dialogProvider.ShowMessage(compileResponse.Exception.Message, "Script compile error", DialogIcon.Error);
            else
                dialogProvider.ShowMessage("Script compile successful", "Script compile successful", DialogIcon.Information);

        }
        protected bool WasTextScriptChanged => configurationModel.TextScript != TextScript;

        protected void Save()
        {
            if (WasTextScriptChanged)
                scriptProvider = new ScriptProvider<TextReplaceScriptGlobals, string>(TextScript);

            configurationModel.Name = Name;
            configurationModel.TextScript = TextScript;
            Debug.Assert(SaveAction != null);
            SaveAction.Invoke();
            Close();
        }

        // TODO Test Compile should test run the function to see if the text script is a proper function with string result (contains return value of type string).
        // Test global = "";

        protected void Close()
        {
            Debug.Assert(CloseAction != null);
            CloseAction.Invoke();
        }

        protected void Cancel()
        {
            Name = configurationModel.Name;
            TextScript = configurationModel.TextScript;
            Close();
        }

        #endregion
    }
}