using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace DeviceControlPanel.Classes
{
    public class NetworkClass
    {
        private readonly object pingLock = new object();

        public async Task<bool> PingHostAsync(string hostNameOrAddress)
        {
            try
            {
                using (Ping ping = new Ping())
                {
                    PingReply reply = await ping.SendPingAsync(hostNameOrAddress);
                    return reply.Status == IPStatus.Success;
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occurred during the ping
                Console.WriteLine($"An error occurred while pinging the host: {ex.Message}");
                return false;
            }
        }

        public async Task<List<string>> ScanConnectedDevicesAsync()
        {
            List<string> connectedDevices = new List<string>();

            List<IPAddress> ipAddresses = GetConnectedIPAddresses();

            ConcurrentBag<string> bag = new ConcurrentBag<string>();
            await Task.WhenAll(ipAddresses.Select(ipAddress => ScanSubnetAsync(ipAddress, bag)));

            connectedDevices.AddRange(bag);

            return connectedDevices;
        }

        public async Task<List<string>> CheckPortOpenAsync(List<string> ipAddresses, int port)
        {
            List<string> devicesWithOpenPort = new List<string>();

            ConcurrentBag<string> bag = new ConcurrentBag<string>();
            await Task.WhenAll(ipAddresses.Select(ipAddress => CheckPortAsync(ipAddress, port, bag)));

            devicesWithOpenPort.AddRange(bag);

            return devicesWithOpenPort;
        }

        public bool IsConnectedToRouter()
        {
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                if (networkInterface.OperationalStatus == OperationalStatus.Up &&
                    networkInterface.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<List<string>> GetAllIPv4Addresses()
        {
            List<string> ipAddresses = new List<string>();

            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();
                foreach (UnicastIPAddressInformation ipAddressInfo in ipProperties.UnicastAddresses)
                {
                    if (ipAddressInfo.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        ipAddresses.Add(ipAddressInfo.Address.ToString());
                    }
                }
            }

            return ipAddresses;
        }

        public async Task<List<string>> GetAllMACAddresses()
        {
            List<string> macAddresses = new List<string>();

            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                macAddresses.Add(networkInterface.GetPhysicalAddress().ToString());
            }

            return macAddresses;
        }

        public async Task<List<string>> GetAllGateways()
        {
            List<string> gateways = new List<string>();

            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();
                foreach (GatewayIPAddressInformation gatewayInfo in ipProperties.GatewayAddresses)
                {
                    gateways.Add(gatewayInfo.Address.ToString());
                }
            }

            return gateways;
        }

        public async Task<List<string>> GetAllDNS()
        {
            List<string> dnsServers = new List<string>();

            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();
                foreach (IPAddress dnsAddress in ipProperties.DnsAddresses)
                {
                    dnsServers.Add(dnsAddress.ToString());
                }
            }

            return dnsServers;
        }

        public bool IsConnectedToProxy()
        {
            // Add your logic to determine if the network is connected to a proxy
            return false;
        }

        public bool IsConnectedToVPN()
        {
            // Add your logic to determine if the network is connected to a VPN
            return false;
        }

        public bool IsOnline()
        {
            // Add your logic to determine if the network is online
            return false;
        }

        public bool IsStaticIP(string ipAddress)
        {
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();
                foreach (UnicastIPAddressInformation ipAddressInfo in ipProperties.UnicastAddresses)
                {
                    if (ipAddressInfo.Address.ToString() == ipAddress)
                    {
                        if (!ipAddressInfo.PrefixOrigin.Equals(PrefixOrigin.Dhcp))
                        {
                            return true;
                        }
                    }
                }
            }

            throw new Exception("IP address not found.");
        }


        private async Task ScanSubnetAsync(IPAddress ipAddress, ConcurrentBag<string> bag)
        {
            string subnet = GetSubnet(ipAddress);
            int[] range = GetSubnetRange(subnet);

            await Task.Run(() =>
            {
                Parallel.For(range[0], range[1] + 1, async i =>
                {
                    string targetIP = $"{subnet}.{i}";

                    if (await IsDeviceOnline(targetIP))
                    {
                        bag.Add(targetIP);
                    }
                });
            });
        }

        private async Task CheckPortAsync(string ipAddress, int port, ConcurrentBag<string> bag)
        {
            if (await IsPortOpen(ipAddress, port))
            {
                bag.Add(ipAddress);
            }
        }

        private async Task<bool> IsDeviceOnline(string ipAddress)
        {
            using (Ping ping = new Ping())
            {
                try
                {
                    PingReply reply = await ping.SendPingAsync(ipAddress);
                    return reply.Status == IPStatus.Success;
                }
                catch (PingException)
                {
                    return false;
                }
            }
        }

        private async Task<bool> IsPortOpen(string ipAddress, int port)
        {
            using (TcpClient tcpClient = new TcpClient())
            {
                try
                {
                    await tcpClient.ConnectAsync(ipAddress, port);
                    return true;
                }
                catch (SocketException)
                {
                    return false;
                }
            }
        }

        private List<IPAddress> GetConnectedIPAddresses()
        {
            List<IPAddress> ipAddresses = new List<IPAddress>();

            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                if (networkInterface.OperationalStatus == OperationalStatus.Up &&
                    (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                     networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet))
                {
                    IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();
                    foreach (UnicastIPAddressInformation ipAddressInfo in ipProperties.UnicastAddresses)
                    {
                        if (ipAddressInfo.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            ipAddresses.Add(ipAddressInfo.Address);
                        }
                    }
                }
            }

            return ipAddresses;
        }

        private string GetSubnet(IPAddress ipAddress)
        {
            string subnetMask = GetSubnetMask(ipAddress);
            string[] ipParts = ipAddress.ToString().Split('.');
            string[] maskParts = subnetMask.Split('.');
            int[] resultParts = new int[4];

            for (int i = 0; i < 4; i++)
            {
                resultParts[i] = int.Parse(ipParts[i]) & int.Parse(maskParts[i]);
            }

            return string.Join(".", resultParts);
        }

        private string GetSubnetMask(IPAddress ipAddress)
        {
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();
                foreach (UnicastIPAddressInformation ipAddressInfo in ipProperties.UnicastAddresses)
                {
                    if (ipAddressInfo.Address.Equals(ipAddress))
                    {
                        return ipAddressInfo.IPv4Mask.ToString();
                    }
                }
            }

            throw new Exception("Subnet mask not found for the given IP address.");
        }

        private int[] GetSubnetRange(string subnet)
        {
            int[] range = new int[2];
            string[] subnetParts = subnet.Split('.');

            // Exclude network and broadcast addresses
            range[0] = int.Parse(subnetParts[0]) == 192 ? 1 : 0;
            range[1] = int.Parse(subnetParts[0]) == 192 ? 254 : 255;

            return range;
        }
    }
}
