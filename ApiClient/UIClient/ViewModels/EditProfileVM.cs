using System;
using System.Linq;
using System.Reactive.Linq;
using Api;

namespace UIClient.ViewModels
{
    public class EditProfileVM : IView
    {
        public ParamaterlessCommand GoBackCmd { get; }
        public ParamaterlessCommand SaveChangesCmd { get; }

        public Reactive<string> Pseudo { get; }
        public Reactive<string> FirstName { get; }
        public Reactive<string> LastName { get; }
        public Reactive<string> Email { get; }
        public Reactive<string> Password { get; }
        public Reactive<string> CompanyName { get; }
        public Reactive<string> Address { get; }
        public Reactive<string> PostalCode { get; }
        public Reactive<string> City { get; }
        public Reactive<string> Country { get; }
        public Reactive<string> SteemUserName { get; }
        public Reactive<string> SteemPostingKey { get; }

        public ModifyUser ToModifyUser => new ModifyUser()
        {
            Pseudo = this.Pseudo.Value,
            FirstName = this.FirstName.Value,
            LastName = this.LastName.Value,
            Email = this.Email.Value,
            Password = String.IsNullOrWhiteSpace(this.Password.Value) ?
                null : this.Password.Value,
            CompanyName = this.CompanyName.Value,
            Address = this.Address.Value,
            PostalCode = this.PostalCode.Value,
            City = this.City.Value,
            Country = this.Country.Value,
            SteemUserName = this.SteemUserName.Value,
            SteemPostingKey = this.SteemPostingKey.Value
        };

        public EditProfileVM(IMasterView<LoggedInVMInfo> master)
        {
            Reactive<T> make<T>(T value) where T : IEquatable<T> =>
                Reactive<T>.FromInitialValue<T>(value);

            this.Pseudo = make(master.Data.LoginInfo.Pseudo);
            this.FirstName = make(master.Data.LoginInfo.FirstName);
            this.LastName = make(master.Data.LoginInfo.LastName);
            this.Email = make(master.Data.LoginInfo.Email);
            this.Password = make(String.Empty);
            this.CompanyName = make(master.Data.LoginInfo.CompanyName);
            this.Address = make(master.Data.LoginInfo.Address);
            this.PostalCode = make(master.Data.LoginInfo.PostalCode);
            this.City = make(master.Data.LoginInfo.City);
            this.Country = make(master.Data.LoginInfo.Country);
            this.SteemUserName = make(master.Data.LoginInfo.SteemUserName);
            this.SteemPostingKey = make(master.Data.LoginInfo.SteemPostingKey);

            var props = new[]
            {
                Pseudo, FirstName, LastName, Email, CompanyName,
                Address, PostalCode, City, Country,
                SteemUserName, SteemPostingKey
            };

            var hasChanges = Observable.CombineLatest(
                this.Password.Differs,
                props.Select(x => x.Differs)
                    .CombineLatest()
                    .Select(x => x.Any(y => y))
            ).Select(x => x.Any(y => y));

            this.GoBackCmd = new ParamaterlessCommand(
                Observable.Return(true), () => master.PopView(this));

            this.SaveChangesCmd = new ParamaterlessCommand(
                hasChanges, async () =>
            {
                var user = this.ToModifyUser;
                var res = await master.Data.Client.ModifyUser(
                    master.Data.LoginInfo.Id,
                    master.Data.LoginInfo.Token,
                    user);

                if (res.Result == null)
                {
                    master.ShowErrorMessage(res.ErrorMsg);
                    return;
                }

                master.ShowMessage("Profile successfully updated.");

                master.PopView(this); // Back to UserHomeVM
                master.PopView(master.CurrentView.Value); // Back to LoginVM

                var login = master.CurrentView.Value as LoginVM;

                if (this.Email.Differs.Value)
                    login.Email.Value = this.Email.Value;

                if (!String.IsNullOrWhiteSpace(this.Password.Value))
                    login.Password.Value = this.Password.Value;

                login.LogInCmd.Execute(null);
            });
        }
    }
}
