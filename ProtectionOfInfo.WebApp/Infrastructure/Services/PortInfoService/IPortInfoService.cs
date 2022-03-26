using ProtectionOfInfo.WebApp.ViewModels.PortInfoViewModels;
using System.Collections.Generic;

namespace ProtectionOfInfo.WebApp.Infrastructure.Services.PortInfoService
{
    /// <summary>
    ///  // aggink: update summary - 02.03.2022 22:26:24
    /// </summary>
    public interface IPortInfoService
    {
        public List<EndPointViewModel> GetActiveTcpListeners();
        public List<PortInfoViewModel> GetActiveTcpConnections();
        public List<EndPointViewModel> GetActiveUdpListeners();
    }
}
