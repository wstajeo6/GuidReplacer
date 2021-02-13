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

        private ScriptRunner<string> scriptRunner;

        #endregion

        #region Public Constructors

        public ConfigurationVM(IDialogProvider dialogProvider, Configuration configurationModel)
        {
            SaveCommand = new RelayCommand(Save, Validate);
            CancelCommand = new RelayCommand(Cancel);
            this.dialogProvider = dialogProvider;
            this.configurationModel = configurationModel;
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

        public RelayCommand SaveCommand { get; }

        public RelayCommand CancelCommand { get; }

        public Action SaveAction { get; set; }

        public Action CancelAction { get; set; }

        #endregion

        #region Private Properties

        private ScriptRunner<string> ScriptRunner
        {
            get
            {
                if (scriptRunner is null)
                    CompileTextScript();
                return scriptRunner;
            }
            set => scriptRunner = value;
        }

        #endregion

        #region Public Methods

        public string ReplaceText(string sourceText)
        {
            try
            {
                var resultText = RunScript(sourceText);
                return resultText.Result;
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

        protected bool Validate() =>
            !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(TextScript);

        protected void Save()
        {
            if (!CompileTextScript())
                return;

            configurationModel.Name = Name;
            configurationModel.TextFunction = TextScript;
            Debug.Assert(SaveAction != null);
            SaveAction.Invoke();
        }

        protected void Cancel()
        {
            Debug.Assert(CancelAction != null);
            CancelAction.Invoke();
        }

        #endregion

        #region Private Methods

        private async Task<string> RunScript(string sourceText)
        {
            var globals = new TextReplaceScriptGlobals
            {
                SourceText = sourceText
            };
            return await ScriptRunner.Invoke(globals);
        }

        private bool CompileTextScript()
        {
            try
            {
                var globalsType = typeof(TextReplaceScriptGlobals);
                var options = ScriptOptions.Default
                    .AddReferences(globalsType.Assembly)
                    .WithOptimizationLevel(Microsoft.CodeAnalysis.OptimizationLevel.Release);
                var fullScript = ($"{TextScript};");
                ScriptRunner = CSharpScript.Create<string>(fullScript, options, globalsType).CreateDelegate();
                return true;
            }
            catch (Exception ex)
            {
                dialogProvider.ShowMessage(ex.Message, "Script compile error", DialogIcon.Error);
                return false;
            }
        }

        #endregion
    }
}