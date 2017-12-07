using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;

namespace UIClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            PresentationTraceSources.Refresh();
            PresentationTraceSources.DataBindingSource.Listeners.Add(new BindingErrorListener());
            PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Warning;

            base.OnStartup(e);
        }
    }

    public class BindingErrorListener : TraceListener
    {
        public override void Write(string message) { }
        public override void WriteLine(string message)
        {
            Debugger.Break();
        }
    }
}
