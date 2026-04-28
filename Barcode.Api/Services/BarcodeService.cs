using Renci.SshNet;
using System.Net.Sockets;
using System.Text;

namespace BarcodeBackend.Services
{
    public class BarcodeService
    {
        private readonly IConfiguration _config;

        public BarcodeService(IConfiguration config)
        {
            _config = config;
        }

        public async Task StartStreamingFromSftp()
        {
            var sftpSett = _config.GetSection("SftpConfig").Get<Models.SftpConfig>();
            var printerIp = _config["PrinterConfig:IpAddress"];

            using var client = new SftpClient(sftpSett.Host, sftpSett.Username, sftpSett.Password);
            client.Connect();

            using var sftpStream = client.OpenRead(sftpSett.RemotePath);
            using var reader = new StreamReader(sftpStream);

            string? sku;
            while ((sku = await reader.ReadLineAsync()) != null)
            {
                if (string.IsNullOrWhiteSpace(sku)) continue;

                // Format ZPL sederhana
                string zpl = $"^XA^FO50,50^BY3^BCN,100,Y,N,N^FD{sku.Trim()}^FS^XZ";
                
                await SendToPrinter(printerIp!, zpl);
            }

            client.Disconnect();
        }

        private async Task SendToPrinter(string ip, string zpl)
        {
            try 
            {
                using var tcpClient = new TcpClient();
                await tcpClient.ConnectAsync(ip, 9100);
                using var stream = tcpClient.GetStream();
                byte[] data = Encoding.ASCII.GetBytes(zpl);
                await stream.WriteAsync(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Gagal kirim ke printer: {ex.Message}");
            }
        }
    }
}