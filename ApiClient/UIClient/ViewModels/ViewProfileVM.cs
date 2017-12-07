using System.Reactive.Linq;

namespace UIClient.ViewModels
{
    public class ViewProfileVM : IView
    {
        public ParamaterlessCommand GoBackCmd { get; }

        public string Pseudo => _master.Data.OtherUser.Pseudo;
        public string FirstName => _master.Data.OtherUser.FirstName;
        public string LastName => _master.Data.OtherUser.LastName;
        public string CompanyName => _master.Data.OtherUser.CompanyName;

        private readonly IMasterView<OtherUserVMInfo> _master;

        public ViewProfileVM(IMasterView<OtherUserVMInfo> master)
        {
            this.GoBackCmd = new ParamaterlessCommand(
                Observable.Return(true), () => master.PopView(this));

            _master = master;
        }
    }
}
