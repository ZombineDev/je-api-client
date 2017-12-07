using Api;

namespace UIClient.ViewModels
{
    public class BaseVMInfo
    {
        public ApiClient Client { get; }

        public BaseVMInfo() : this(new ApiClient()) { }
        public BaseVMInfo(BaseVMInfo other) : this(other.Client) { }
        public BaseVMInfo(ApiClient client) => this.Client = client;
    }
}
