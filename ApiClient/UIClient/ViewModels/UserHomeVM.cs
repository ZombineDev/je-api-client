using System.Reactive.Linq;

namespace UIClient.ViewModels
{
    public class UserHomeVM : IView
    {
        public ParamaterlessCommand LogOutCmd { get; }
        public ParamaterlessCommand GoToSearchUsersCmd { get; }
        public ParamaterlessCommand GoToEditProfileCmd { get; }
        public ParamaterlessCommand DeleteProfileCmd { get; }

        public string Pseudo { get; }

        public UserHomeVM(IMasterView<LoggedInVMInfo> master)
        {
            Pseudo = master.Data.LoginInfo.Pseudo;

            this.LogOutCmd = new ParamaterlessCommand(
                Observable.Return(true), () => master.PopView(this));

            this.GoToSearchUsersCmd = new ParamaterlessCommand(
                Observable.Return(true), () => master.PushView(new SearchUsersVM(master))
            );

            this.GoToEditProfileCmd = new ParamaterlessCommand(
                Observable.Return(true), () => master.PushView(new EditProfileVM(master))
            );

            this.DeleteProfileCmd = new ParamaterlessCommand(
                Observable.Return(true), async () =>
                {
                    if (!master.AskForConfirmation(
                        "Are you sure you want to delete your account?"))
                        return;

                    var res = await master.Data.Client.DeleteUser(
                            master.Data.LoginInfo.Id, master.Data.LoginInfo.Token);

                    if (res.Result == null)
                        master.ShowErrorMessage(res.ErrorMsg);

                    master.PopView(this);
                }
            );
        }
    }
}
