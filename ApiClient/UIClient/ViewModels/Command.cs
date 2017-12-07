using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UIClient.ViewModels
{
    public abstract class CommandBase : ICommand
    {
        public CommandBase(IObservable<bool> canExecute)
        {
            if (canExecute == null)
                throw new ArgumentNullException();

            canExecute.DistinctUntilChanged().Subscribe(x =>
            {
                _canExecute = x;
                CanExecuteChanged?.Invoke(this, new EventArgs());
            });
        }

        private bool _canExecute;

        public bool CanExecute(object parameter) => _canExecute;
        public event EventHandler CanExecuteChanged;
        public abstract void Execute(object parameter);
    }

    public class ParamaterlessCommand : CommandBase
    {
        public ParamaterlessCommand(IObservable<bool> canExecute, Action action)
            : base(canExecute)
        {
            if (action == null)
                throw new ArgumentNullException();

            _action = action;
        }

        private readonly Action _action;

        public override void Execute(object _) => _action?.Invoke();
    }

    public class SingleParamCommand : CommandBase
    {
        public SingleParamCommand(IObservable<bool> canExecute, Action<object> action)
            : base(canExecute)
        {
            if (action == null)
                throw new ArgumentNullException();

            _action = action;
        }

        private readonly Action<object> _action;

        public override void Execute(object arg) => _action?.Invoke(arg);
    }
}
