using System.Reactive.Linq;

namespace UIClient.ViewModels
{
    public class ResetPasswordVM : IView
    {
        public ParamaterlessCommand GoBackCmd { get; }
        public ParamaterlessCommand ResetPasswordCmd { get; }

        public Reactive<string> Email { get; } = new Reactive<string>();

        public ResetPasswordVM(IMasterView<BaseVMInfo> master)
        {
            this.GoBackCmd = new ParamaterlessCommand(
                Observable.Return(true), () => master.PopView(this));

            this.ResetPasswordCmd = new ParamaterlessCommand(
                Observable.Return(true), async () =>
                {
                    var res = await master.Data.Client.RequestNewPassword(Email.Value);

                    if (res.Result == null)
                        master.ShowErrorMessage(res.ErrorMsg);
                    else
                        master.ShowMessage("Request for password reset accepted.");

                    master.PopView(this);
                });
        }
    }
}
