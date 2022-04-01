using ProtectionOfInfo.WebApp.ViewModels.PortInfoViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ProtectionOfInfo.WebApp.Infrastructure.Services.PortInfoService
{
    /// <summary>
    ///  // aggink: update summary - 02.03.2022 18:58:07
    /// </summary>
    public class PortInfoService : IPortInfoService
    {
        private IPGlobalProperties _properties;
        public PortInfoService()
        {
            _properties = IPGlobalProperties.GetIPGlobalProperties(); 
        }

        public List<PortInfoViewModel> GetActiveTcpConnections()
        {
            var connections = _properties.GetActiveTcpConnections();
            return connections.Select(p =>
            {
                return new PortInfoViewModel(
                    portNumber: p.LocalEndPoint.Port,
                    local: $"{p.LocalEndPoint.Address}:{p.LocalEndPoint.Port}",
                    remote: $"{p.RemoteEndPoint.Address}:{p.RemoteEndPoint.Port}",
                    state: p.State.ToString());
            }).ToList();
        }

        public List<EndPointViewModel> GetActiveTcpListeners()
        {
            var tcpListeners = _properties.GetActiveTcpListeners();
            return tcpListeners.Select(p => {
                return new EndPointViewModel(
                    port: p.Port.ToString(),
                    address: p.Address.ToString(),
                    addressFamily: p.AddressFamily.ToString());
            }).ToList();
        }

        public List<EndPointViewModel> GetActiveUdpListeners()
        {
            var udpListeners = _properties.GetActiveUdpListeners();
            return udpListeners.Select(p =>
            {
                return new EndPointViewModel(
                    port: p.Port.ToString(),
                    address: p.Address.ToString(),
                    addressFamily: p.AddressFamily.ToString());
            }).ToList();
        }
    }
}
