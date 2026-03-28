using System;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace GHelper.CompanionApp
{
    class SocketServer : CompanionService
    {
        public static readonly int Port = 8080;

        private CancellationTokenSource BroadcastCancellationToken;
        private Task BroadcastTask;
        private CancellationTokenSource CancellationToken;
        private Task Task;

        private TcpListener? server;
        private Socket? client;

        public class IpNotFoundException : Exception { }

        //private UdpClient? client = null;
        //private string? IpToSend = null;

        public SocketServer() : base()
        {

        }


        //private string GetCurrentWifiIPAddress()
        //{
        //    // Iterate through all network interfaces
        //    foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
        //    {
        //        // Filter for operational Wireless80211 or Ethernet interfaces
        //        if ((ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211) && // || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet) &&
        //            ni.OperationalStatus == OperationalStatus.Up)
        //        {
        //            // Get the IP properties of the interface
        //            IPInterfaceProperties properties = ni.GetIPProperties();
        //            // Look for Unicast IP addresses associated with this interface
        //            foreach (UnicastIPAddressInformation ip in properties.UnicastAddresses)
        //            {
        //                // Return the first valid IPv4 address found
        //                if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
        //                {
        //                    Debug.WriteLine($"IP {ip.Address.ToString()} ");
        //                }
        //            }
        //        }
        //    }
        //    return "No Wi-Fi IP address found";
        //}

        public string? GetLocalIPAddress()
        {
            //GetCurrentWifiIPAddress();

            var host = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = host.AddressList
                .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork); // Filters for IPv4 addresses

            if (ipAddress == null)
            {
                return null;
            }

            return ipAddress.ToString();
        }

        private void BroadcastIp()
        {

            BroadcastCancellationToken?.Cancel();
            BroadcastCancellationToken = new CancellationTokenSource();
            BroadcastTask = new Task(() =>
            {
                int broadcastPort = 11000;



                // Create an IPEndPoint for the broadcast address and port
                // IPAddress.Broadcast (255.255.255.255) sends to all hosts on the local network segment.
                // You can also use a directed broadcast address like 192.168.1.255 for a specific subnet.
                IPEndPoint broadcastEndpoint = new IPEndPoint(IPAddress.Broadcast, broadcastPort);

                UdpClient udpClient = new UdpClient();
                // Enable broadcast for the UdpClient
                udpClient.EnableBroadcast = true;

                udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 2000);


                //udpClient.Client.Bind(broadcastEndpoint);

                string Ip = GetLocalIPAddress() ?? "";

                byte[] sendBytes = Encoding.ASCII.GetBytes("G-Helper:IP:" + Ip + ":" + Port);



                while (!BroadcastCancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        udpClient.Send(sendBytes, sendBytes.Length, broadcastEndpoint);

                        Debug.WriteLine($"Socket: Broadcast {Ip}:{Port}  message sent to {broadcastEndpoint}");

                        byte[] recBytes = udpClient.Receive(ref broadcastEndpoint);

                        string data = Encoding.ASCII.GetString(recBytes, 0, recBytes.Length);

                        string[] msg = data.Split(":");
                        if (msg.Length == 3 && msg[0] == "G-Helper-Client" && msg[1] == "IP")
                        {
                            //IpToSend = msg[2];
                        }

                        Debug.WriteLine($"Socket: Broadcast received from {broadcastEndpoint} msg = {data}");


                        break;

                    }
                    catch (SocketException e)
                    {
                        if (e.SocketErrorCode == SocketError.TimedOut)
                        {
                            Debug.WriteLine("Socket: Waiting for connection...");
                        }
                        else
                        {
                            Debug.WriteLine($"Socket: SocketException: {e.Message}");
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine($"Socket: Error: {e.Message}");
                        break;
                    }
                }
                udpClient.Close();

            }, BroadcastCancellationToken.Token);

            BroadcastTask.Start();
        }

        public override void Start()
        {
            string IpAddress = GetLocalIPAddress();

            //if (IpAddress == null)
            //{
            //    SetStatusChanged(EStatus.Stopped, new IpNotFoundException());
            //    return;
            //}

            // BroadcastIp();

            CancellationToken = new CancellationTokenSource();
            Task = new Task(() =>
            {
                try
                {
                    server = new TcpListener(Port);
                    server.Start();

                    SetStatusChanged(EStatus.Started);


                accept:
                    client = server.AcceptSocket();

                    // No need for broadcast Ip. we connected to client
                    BroadcastCancellationToken?.Cancel();

                    while (!CancellationToken.IsCancellationRequested && client.Connected)
                    {
                        try
                        {
                            CancellationToken.Token.ThrowIfCancellationRequested();

                            byte[] buffer = new byte[1024];
                            int r = client.Receive(buffer);

                            Type Code = (Type)buffer[0];

                            switch (Code)
                            {
                                case Type.INFO:
                                    {
                                        byte[] b = PrepareInfoBuffer();

                                        client.Send(b);

                                        break;
                                    }
                                case Type.MODES:
                                    {
                                        byte[] b = PrepareModesBuffer();

                                        client.Send(b);

                                        break;
                                    }
                                case Type.SENSOR:
                                    {
                                        byte[] b = PrepareSensorBuffer();

                                        client.Send(b);

                                        break;
                                    }
                                case Type.CMD:
                                    {
                                        OnRead(buffer);
                                    }
                                    break;

                                default:
                                    break;
                            }
                        }
                        catch (Exception e) { }
                    }

                    if (!client.Connected)
                    {
                        // client might stop app. broadcsast server ip again
                        BroadcastIp();
                        goto accept;
                    }
                }
                catch (Exception e)
                {
                    server?.Stop();
                    SetStatusChanged(EStatus.Stopped, e);
                }

            }, CancellationToken.Token);

            Task.Start();

        }

        public override void Stop()
        {
            base.Stop();

            try
            {
                client?.Disconnect(false);
                server?.Stop();
            }
            catch
            {

            }
            //client?.Close();

            BroadcastCancellationToken?.Cancel();
            CancellationToken?.Cancel();



        }

    }
}
