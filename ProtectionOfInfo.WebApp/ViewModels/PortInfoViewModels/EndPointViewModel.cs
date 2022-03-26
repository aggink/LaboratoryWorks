namespace ProtectionOfInfo.WebApp.ViewModels.PortInfoViewModels
{
    public class EndPointViewModel
    {
        public EndPointViewModel(string port, string  addressFamily, string address)
        {
            AddressFamily = addressFamily;
            Address = address;
            Port = port;
        }

        public string Address { get; set; }
        public string Port { get; set; }
        public string AddressFamily { get; set; }
    }
}
