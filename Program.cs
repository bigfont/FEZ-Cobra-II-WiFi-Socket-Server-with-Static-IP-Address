using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using GHI.Networking;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Net.NetworkInformation;

namespace G120.SocketServer
{
    public class Program
    {
        private const int Port = 12000;
        private const string IpAddress = "192.168.1.115";
        private const string SubnetMask = "255.255.255.0";
        private const string GatewayAddress = "192.168.1.1";
        private const string Bigfont = "BigFont";
        private const string Nutbutter3 = "NutButter3";

        public static void Main()
        {
            var wiFiRs9110 = new WiFiRS9110(
                SPI.SPI_module.SPI2,
                GHI.Pins.G120.P1_10,
                GHI.Pins.G120.P2_11,
                GHI.Pins.G120.P1_9, 4000);

            wiFiRs9110.Open(); // what does this do?

            wiFiRs9110.EnableStaticIP(IpAddress, SubnetMask, GatewayAddress);
            
            ConnectWiFi(wiFiRs9110, Bigfont, Nutbutter3);

            // this never fires
            NetworkChange.NetworkAvailabilityChanged += (sender, args) =>
            {
                ("NetworkAvailabilityChanged. IsAvailable is now " + args.IsAvailable).Dump();

                if (!args.IsAvailable) return;

                var threadStart = new ThreadStart(() => ConnectSocket(IpAddress, Port));
                var thread = new Thread(threadStart);
                thread.Start();
            };

            // this throws
            try
            {
                var threadStart = new ThreadStart(() => ConnectSocket(IpAddress, Port));
                var thread = new Thread(threadStart);
                thread.Start();
            }
            catch (Exception ex)
            {
                ex.ToString().Dump();
            }

            // wait to connect
            var i = 0;
            while (true)
            {
                Thread.Sleep(5000);
                ("Waiting count " + i++).Dump();
             
                wiFiRs9110.Dump();
            }
        }

        private static void ConnectWiFi(WiFiRS9110 wifiRs9110, string ssid, string password)
        {
            const int millisecondsTimeout = 1000;

            var targetGateway = new WiFiRS9110.NetworkParameters[0];

            while (targetGateway.Length == 0)
            {
                targetGateway = wifiRs9110.Scan(ssid);
                Thread.Sleep(millisecondsTimeout);
            }

            wifiRs9110.Join(ssid, password);
        }

        private static void ConnectSocket(string ipAddress, int port)
        {
            const int socketBacklog = 25;

            var localEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);

            var serverSocket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);

            serverSocket.Bind(localEndPoint);

            serverSocket.Listen(socketBacklog);

            ("#####").Dump();
            ("#####").Dump();
            ("#####").Dump();

            ("Socket connection succeeded!").Dump();
            ("Navigate to " + ipAddress + ":" + port + " in your web browser.").Dump();

            ("#####").Dump();
            ("#####").Dump();
            ("#####").Dump();

            while (true)
            {
                // Wait for an incoming connection; queue it when it arrives.
                // Accept blocks until a connection arrives.
                // See also bit.ly/1S54gul
                using (var clientSocket = serverSocket.Accept())
                {
                    clientSocket.Okay("It works.");
                }
            }
        }
    }

    public static class Extensions
    {
        public static void Dump(this NetworkInterface networkInterface)
        {
            var builder = new StringBuilder();

            builder.AppendLine("NetworkInterface");

            foreach (var dns in networkInterface.DnsAddresses)
            {
                builder.AppendLine("DnsAddress: " + dns);
            }
            builder.AppendLine("GatewayAddress:" + networkInterface.GatewayAddress);
            builder.AppendLine("IPAddress:" + networkInterface.IPAddress);
            builder.AppendLine("IsDhcpEnabled:" + networkInterface.IsDhcpEnabled);
            builder.AppendLine("IsDynamicDnsEnabled:" + networkInterface.IsDynamicDnsEnabled);
            builder.AppendLine("NetworkInterfaceType:" + networkInterface.NetworkInterfaceType);
            builder.AppendLine("PhysicalAddress:" + ByteArrayToHex(networkInterface.PhysicalAddress, ":"));
            builder.AppendLine("SubnetMask:" + networkInterface.SubnetMask);

            builder.ToString().Dump();
        }

        public static void Dump(this WiFiRS9110 wiFiRs9110)
        {
            var builder = new StringBuilder();

            builder.AppendLine("WiFiRS9110");

            wiFiRs9110.NetworkInterface.Dump();

            builder.AppendLine("LinkConnected:" + wiFiRs9110.LinkConnected);
            builder.AppendLine("ActiveNetwork:" + wiFiRs9110.ActiveNetwork);
            builder.AppendLine("NetworkIsAvailable:" + wiFiRs9110.NetworkIsAvailable);
            builder.AppendLine("Opened:" + wiFiRs9110.Opened);

            builder.ToString().Dump();
        }

        public static void Dump(this string output)
        {
            Debug.Print(output);
        }

        public static string ByteArrayToHex(this byte[] byteArray, string separator)
        {
            // even though it is in the NETMF System namespace,
            // BitConverter.ToString(byteArray) does NOT work on Cobra boards.

            var macBuilder = new StringBuilder(byteArray.Length * 2);
            for (var i = 0; i < byteArray.Length; ++i)
            {
                var hex = byteArray[i].ToString("X2");
                if (i != 0)
                {
                    macBuilder.Append(separator);
                }
                macBuilder.Append(hex);
            }

            return macBuilder.ToString();
        }

        public static void Okay(this Socket clientSocket, string message)
        {
            const string crlf = "\r\n";

            var builder = new StringBuilder();

            builder.Append("HTTP/1.1 200 OK");
            builder.Append(crlf);
            builder.Append("Connection: close");
            builder.Append(crlf);
            builder.Append(crlf);
            builder.Append(message);
            builder.Append(crlf);
            builder.Append(crlf);

            var bytes = Encoding.UTF8.GetBytes(builder.ToString());

            clientSocket.Send(bytes);
        }
    }
}

// NetworkInterface class, 
// provides info about interfaces and enables apps to control them, https://msdn.microsoft.com/en-us/library/ee435203.aspx

// WiFiRS9110, 
// driver for the WiFi module that lives on the G120, https://www.ghielectronics.com/downloads/man/Library_Documentation_v4.2/Premium/html/8e2dbcc0-804e-1f70-4d0a-11fef987e01e.htm