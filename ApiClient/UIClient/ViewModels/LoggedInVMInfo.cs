using Api;

namespace UIClient.ViewModels
{
    public class LoggedInVMInfo : BaseVMInfo
    {
        public LoginResult LoginInfo { get; }

        public LoggedInVMInfo(BaseVMInfo baseVM, LoginResult loginResult)
            : base(baseVM) => LoginInfo = loginResult;

        public LoggedInVMInfo(LoggedInVMInfo other)
            : this(other, other.LoginInfo) { }
    }
}
