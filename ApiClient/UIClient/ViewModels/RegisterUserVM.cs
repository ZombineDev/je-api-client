using System.Reactive.Linq;
using Api;

namespace UIClient.ViewModels
{
    public class RegisterUserVM : IView
    {
        public ParamaterlessCommand GoBackCmd { get; }
        public ParamaterlessCommand RegisterCmd { get; }

        public Reactive<string> Pseudo { get; } = new Reactive<string>();
        public Reactive<string> FirstName { get; } = new Reactive<string>();
        public Reactive<string> LastName { get; } = new Reactive<string>();
        public Reactive<string> Email { get; } = new Reactive<string>();
        public Reactive<string> Password { get; } = new Reactive<string>();

        public RegisterUserVM(IMasterView<BaseVMInfo> master)
        {
            this.GoBackCmd = new ParamaterlessCommand(
                Observable.Return(true), () => master.PopView(this));

            this.RegisterCmd = new ParamaterlessCommand(
                Observable.Return(true), async () =>
            {
                var request = new RegisterUserRequest()
                {
                    Pseudo = this.Pseudo.Value,
                    FirstName = this.FirstName.Value,
                    LastName = this.LastName.Value,
                    Email = this.Email.Value,
                    Password = this.Password.Value
                };
                var res = await master.Data.Client.Register(request);

                if (res.Result == null)
                {
                    master.ShowErrorMessage(res.ErrorMsg);
                    return;
                }

                master.PopView(this);
            });
        }
    }
}
