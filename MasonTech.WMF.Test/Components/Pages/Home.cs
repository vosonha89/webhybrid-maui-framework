using MasonTech.WMF.Core.Components.Objects;
using Microsoft.JSInterop;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json;

namespace MasonTech.WMF.Test.Components.Pages
{
    public partial class Home
    {
        [JSInvokable]
        public void CallCSharp()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += WorkerDoWork;
            worker.RunWorkerCompleted += WorkerRunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        private void WorkerDoWork(object? sender, DoWorkEventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                if (CurrentJSRuntime != null)
                {
                    CurrentJSRuntime.InvokeVoidAsync("csharpCallJS", "JS function run in " + (i + 1) + 's');
                }
            }
        }

        private void WorkerRunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (CurrentJSRuntime != null)
            {
                CurrentJSRuntime.InvokeVoidAsync("csharpCallJS", "JS function finished from .NET");
            }
        }
    }
}
