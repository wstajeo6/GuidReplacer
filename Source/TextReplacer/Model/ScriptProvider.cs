using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Threading.Tasks;

namespace TextReplacer.Model
{
    public class CompileResponse<TResult> : BaseGenericResponse<TResult>
    {
        public CompileResponse(TResult result, string message = null, Exception exception = null) :
            base(result, message, exception)
        {
        }
    }

    public class RunResponse<TResult> : BaseGenericResponse<TResult>
    {
        public RunResponse(TResult result, string message = null, Exception exception = null) :
            base(result, message, exception)
        {
        }
    }

    public class ScriptProvider<TGlobals, TResult>
    {
        private ScriptRunner<TResult> scriptRunner;

        public ScriptProvider(string textScript)
        {
            TextScript = textScript;
        }

        public bool IsCompiled => scriptRunner != null;

        public string TextScript { get; }

        public async Task<RunResponse<TResult>> Run(TGlobals globals)
        {
            try
            {
                if (scriptRunner is null)
                {
                    var compileResponse = await Compile();
                    if (compileResponse.Exception != null)
                        return new RunResponse<TResult>(default(TResult), compileResponse.Message, compileResponse.Exception);
                }

                var result = await scriptRunner.Invoke(globals).ConfigureAwait(false);
                return new RunResponse<TResult>(result, "Script was run successful");
            }
            catch (Exception ex)
            {
                return new RunResponse<TResult>(default(TResult), "Script failed", ex);
            }
        }

        public async Task<CompileResponse<ScriptRunner<TResult>>> Compile()
        {
            if (IsCompiled)
                return new CompileResponse<ScriptRunner<TResult>>(scriptRunner, "Script already compiled");

            try
            {
                var globalsType = typeof(TGlobals);
                var options = ScriptOptions.Default
                    .AddReferences(globalsType.Assembly)
                    .WithOptimizationLevel(Microsoft.CodeAnalysis.OptimizationLevel.Release);
                var fullScript = $"{TextScript};";
                scriptRunner = await Task.Run(() => CSharpScript.Create<TResult>(fullScript, options, globalsType).CreateDelegate()).ConfigureAwait(false);
                return new CompileResponse<ScriptRunner<TResult>>(scriptRunner, "Script compile successful");
            }
            catch (Exception ex)
            {
                return new CompileResponse<ScriptRunner<TResult>>(default(ScriptRunner<TResult>), "Script compile failed", ex);
            }
        }
    }
}