using Api;
using System.Reactive.Linq;

namespace UIClient.ViewModels
{
    public class LoginVM : IView
    {
        public ParamaterlessCommand GoBackCmd { get; }
        public ParamaterlessCommand LogInCmd { get; }

        public Reactive<string> Email { get; } = new Reactive<string>();
        public Reactive<string> Password { get; } = new Reactive<string>();
        public LoginRequest Request => new LoginRequest()
        { Email = this.Email.Value, Password = Password.Value };

        public LoginVM(IMasterView<BaseVMInfo> master)
        {
            this.GoBackCmd = new ParamaterlessCommand(
                Observable.Return(true), () => master.PopView(this));

            this.LogInCmd = new ParamaterlessCommand(
                Observable.Return(true), async () =>
                {
                    var res = await master.Data.Client.Login(Request);

                    if (res.Result == null)
                    {
                        master.ShowErrorMessage(res.ErrorMsg);
                        return;
                    }

                    var newMaster = master.UpgradeData(new LoggedInVMInfo(master.Data, res.Result));
                    newMaster.PushView(new UserHomeVM(newMaster));
                });
        }
    }
}
