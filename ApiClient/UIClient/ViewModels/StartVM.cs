using System.Reactive.Linq;

namespace UIClient.ViewModels
{
    public class StartVM : MasterView<BaseVMInfo>
    {
        public ParamaterlessCommand GoToLoginCmd { get; }
        public ParamaterlessCommand GoToRegisterCmd { get; }
        public ParamaterlessCommand GoToResetPasswordCmd { get; }

        public StartVM() : base(new BaseVMInfo())
        {
            this.GoToLoginCmd = new ParamaterlessCommand(
                Observable.Return(true), () => PushView(new LoginVM(this)));

            this.GoToRegisterCmd = new ParamaterlessCommand(
                Observable.Return(true), () => PushView(new RegisterUserVM(this)));

            this.GoToResetPasswordCmd = new ParamaterlessCommand(
                Observable.Return(true), () => PushView(new ResetPasswordVM(this)));
        }
    }
}
