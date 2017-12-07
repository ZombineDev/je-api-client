using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIClient.ViewModels
{
    public interface IView
    {
    }

    public interface IMasterView<T> : IView
        where T : class
    {
        ReactiveProperty<IView> CurrentView { get; }
        void PopView(IView current);
        void PushView(IView view);

        T Data { get; }
        IMasterView<U> UpgradeData<U>(U derived) where U : class, T;

        void ShowErrorMessage(string errorMessage);
        void ShowMessage(string errorMessage);
        bool AskForConfirmation(string message);
    }

    public class MasterView<T> : IMasterView<T>
        where T : class
    {
        public ReactiveProperty<IView> CurrentView { get; }
        private readonly Stack<IView> viewStack;

        public T Data { get; private set; }

        public IMasterView<U> UpgradeData<U>(U derivedData) where U : class, T
        {
            if (this.Data.GetType() == derivedData.GetType())
                throw new ArgumentException(
                    "Upgrade() expects an instance of a derived class " +
                    "distinct from the current instance");

            return new MasterView<U>(
                derivedData, this.CurrentView, this.viewStack);
        }

        public void PopView(IView current)
        {
            if (!Object.ReferenceEquals(viewStack.Peek(), current))
                throw new ArgumentException();
            viewStack.Pop();
            CurrentView.Value = viewStack.Peek();
        }

        public void PushView(IView view)
        {
            viewStack.Push(view);
            CurrentView.Value = viewStack.Peek();
        }

        public void ShowErrorMessage(string errorMessage)
        {
            System.Windows.MessageBox.Show(errorMessage, "Error",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Error);
        }

        public void ShowMessage(string message)
        {
            System.Windows.MessageBox.Show(message, "",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information);
        }

        public bool AskForConfirmation(string message)
        {
            var res = System.Windows.MessageBox.Show(message, "",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Question);

            return res == System.Windows.MessageBoxResult.Yes;
        }

        private MasterView(T commonData,
            ReactiveProperty<IView> currentViewRP,
            Stack<IView> views)
        {
            this.CurrentView = currentViewRP;
            this.viewStack = views;
            this.Data = commonData;
        }

        public MasterView(T commonData)
            : this(commonData, new ReactiveProperty<IView>(), new Stack<IView>())
        {
            PushView(this);
        }
    }
}
