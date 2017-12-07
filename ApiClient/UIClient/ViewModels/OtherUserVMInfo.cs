using Api;

namespace UIClient.ViewModels
{
    public class OtherUserVMInfo : LoggedInVMInfo
    {
        public User OtherUser { get; }

        public OtherUserVMInfo(LoggedInVMInfo loggedInVMInfo, User otherUser)
            : base(loggedInVMInfo) => OtherUser = otherUser;
    }
}
