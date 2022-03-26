namespace ProtectionOfInfo.WebApp.ViewModels.PortInfoViewModels
{
    public class PortInfoViewModel
    {
        public PortInfoViewModel(int portNumber, string local, string remote, string state)
        {
            PortNumber = portNumber;
            Local = local;
            Remote = remote;
            State = state;
        }

        public int PortNumber { get; set; }
        public string Local { get; set; }
        public string Remote { get; set; }
        public string State { get; set; }
    }
}
