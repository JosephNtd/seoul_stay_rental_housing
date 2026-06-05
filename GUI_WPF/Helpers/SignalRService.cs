using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.AspNet.SignalR.Client;

namespace GUI_WPF.Helpers
{
    /// <summary>
    /// Singleton service quản lý kết nối SignalR tới InvoiceHub.
    /// Tự động detect IP LAN để điện thoại cùng mạng truy cập payment URL.
    /// </summary>
    public class SignalRService
    {
        // =============================================
        // SINGLETON
        // =============================================

        private static readonly Lazy<SignalRService> _lazy =
            new Lazy<SignalRService>(() => new SignalRService());

        public static SignalRService Instance => _lazy.Value;

        private SignalRService()
        {
            // Tự detect IP LAN khi khởi tạo
            string lanIp = GetLocalIPAddress();
            PaymentBaseUrl = $"http://{lanIp}:{PaymentPort}";
        }

        // =============================================
        // CONFIG
        // =============================================

        /// <summary>
        /// Port của GUI_Web_Payment server (IIS Express mặc định).
        /// </summary>
        public int PaymentPort { get; set; } = 5000;

        /// <summary>
        /// Base URL của GUI_Web_Payment server.
        /// Tự động set thành http://{LAN_IP}:5000 khi khởi tạo.
        /// Có thể ghi đè thủ công nếu cần.
        /// </summary>
        public string PaymentBaseUrl { get; set; }

        /// <summary>
        /// SignalR Hub URL, derive từ PaymentBaseUrl.
        /// </summary>
        public string HubUrl => PaymentBaseUrl.TrimEnd('/') + "/signalr";

        // =============================================
        // CONNECTION
        // =============================================

        private HubConnection _connection;
        private IHubProxy _hubProxy;

        public bool IsConnected =>
            _connection != null && _connection.State == ConnectionState.Connected;

        // =============================================
        // EVENT
        // =============================================

        /// <summary>
        /// Fired khi Web server broadcast paymentResult.
        /// Parameters: (long invoiceId, string status)
        /// status = "Paid" | "Cancelled"
        /// </summary>
        public event Action<long, string> OnPaymentResult;

        // =============================================
        // LAN IP AUTO-DETECT
        // =============================================

        /// <summary>
        /// Lấy địa chỉ IPv4 LAN của máy tính (Wi-Fi hoặc Ethernet).
        /// Ưu tiên adapter đang hoạt động và có gateway (= đang kết nối mạng thật).
        /// Fallback: dùng UDP socket trick nếu không tìm được.
        /// </summary>
        public static string GetLocalIPAddress()
        {
            try
            {
                // Cách 1: Tìm adapter đang Up, có gateway, lấy IPv4
                var interfaces = NetworkInterface.GetAllNetworkInterfaces()
                    .Where(ni => ni.OperationalStatus == OperationalStatus.Up
                              && ni.NetworkInterfaceType != NetworkInterfaceType.Loopback
                              && ni.NetworkInterfaceType != NetworkInterfaceType.Tunnel);

                foreach (var ni in interfaces)
                {
                    var ipProps = ni.GetIPProperties();

                    // Phải có gateway (= kết nối mạng thật, không phải virtual adapter)
                    if (ipProps.GatewayAddresses.Count == 0)
                        continue;

                    var ipv4 = ipProps.UnicastAddresses
                        .FirstOrDefault(addr =>
                            addr.Address.AddressFamily == AddressFamily.InterNetwork
                            && !IPAddress.IsLoopback(addr.Address));

                    if (ipv4 != null)
                        return ipv4.Address.ToString();
                }

                // Cách 2 (fallback): UDP socket trick — hỏi OS route tới 8.8.8.8
                using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                {
                    socket.Connect("8.8.8.8", 65530); // không thực sự gửi gì
                    var endPoint = socket.LocalEndPoint as IPEndPoint;
                    return endPoint.Address.ToString();
                }
            }
            catch
            {
                // Worst case: fallback về localhost
                return "localhost";
            }
        }

        // =============================================
        // CONNECT / DISCONNECT
        // =============================================

        /// <summary>
        /// Kết nối tới InvoiceHub. Nếu đã có connection cũ sẽ disconnect trước.
        /// </summary>
        public async Task ConnectAsync()
        {
            if (_connection != null)
            {
                try
                {
                    _connection.Stop();
                    _connection.Dispose();
                }
                catch { }

                _connection = null;
                _hubProxy = null;
            }

            _connection = new HubConnection(HubUrl);
            _hubProxy = _connection.CreateHubProxy("InvoiceHub");

            // Lắng nghe event "paymentResult" từ server (camelCase — SignalR 2.x convention)
            _hubProxy.On<long, string>("paymentResult", (invoiceId, status) =>
            {
                Application.Current?.Dispatcher?.Invoke(() =>
                {
                    OnPaymentResult?.Invoke(invoiceId, status);
                });
            });

            // Tự reconnect khi mất kết nối
            _connection.Closed += () =>
            {
                Task.Delay(3000).ContinueWith(async _ =>
                {
                    try
                    {
                        await _connection.Start();
                    }
                    catch { }
                });
            };

            await _connection.Start();
        }

        /// <summary>
        /// Ngắt kết nối SignalR.
        /// </summary>
        public Task DisconnectAsync()
        {
            if (_connection != null)
            {
                try
                {
                    _connection.Stop();
                    _connection.Dispose();
                }
                catch { }

                _connection = null;
                _hubProxy = null;
            }

            return Task.CompletedTask;
        }
    }
}