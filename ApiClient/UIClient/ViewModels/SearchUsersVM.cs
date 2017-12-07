using Api;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using UIClient.Controls;

namespace UIClient.ViewModels
{
    public static class RXExtensions
    {
        public static IObservable<T> ObserveOnUIThread<T>(
            this IObservable<T> stream)
        {
            return stream.ObserveOn(
                System.Windows.Application.Current.Dispatcher);
        }
    }

    public class SearchUsersVM : IView
    {
        public ParamaterlessCommand GoBackCmd { get; }
        public ParamaterlessCommand ViewUserProfileCmd { get; }

        public Reactive<string> SearchQuery { get; } =
            new Reactive<string>();

        public ReadOnlyReactiveProperty<User[]> Results { get; }

        public Reactive<User> SelectedUser { get; } =
            new Reactive<User>();

        public SearchUsersVM(IMasterView<LoggedInVMInfo> master)
        {
            this.GoBackCmd = new ParamaterlessCommand(
                Observable.Return(true), () => master.PopView(this));

            this.ViewUserProfileCmd = new ParamaterlessCommand(
                SelectedUser.Select(x => x != null), async () =>
                {
                    var res = await master.Data.Client.GetUser(
                        SelectedUser.Value.Id, master.Data.LoginInfo.Token);

                    if (res.Result == null)
                    {
                        master.ShowErrorMessage(res.Error.ToString());
                        return;
                    }

                    var newMaster = master.UpgradeData(
                        new OtherUserVMInfo(master.Data, res.Result));
                    newMaster.PushView(new ViewProfileVM(newMaster));
                });

            this.Results = SearchQuery
                .Select(query => master.Data.Client.SearchUsers(
                    master.Data.LoginInfo.Token, query))
                .Throttle(TimeSpan.FromMilliseconds(200))
                .Switch()
                .Select(x => x.Result)
                .ObserveOnUIThread()
                .ToReadOnlyReactiveProperty();
        }
    }
}
