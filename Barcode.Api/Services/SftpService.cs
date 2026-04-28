using Renci.SshNet;
using Npgsql;
using Dapper;
using Barcode.Api.Models;

namespace Barcode.Api.Services
{
    public class SftpService : ISftpService
    {
        private readonly IConfiguration _config;
        private readonly string _privateKeyPath;
        private readonly string _connectionString;

        public SftpService(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("DefaultConnection");
            _privateKeyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Keys", "id_rsa");
        }

        public List<string> ListFiles()
{
    var keyFile = new PrivateKeyFile(_privateKeyPath);
    using var client = new SftpClient("sftpnew.delamibrands.com", 22, "cegid-ftp", keyFile);
    client.Connect();
    
    // Gunakan path relatif tanpa slash depan
    var files = client.ListDirectory("STR-FFO-TESTING")
        .Where(f => !f.IsDirectory && !f.Name.StartsWith("."))
        .Select(f => f.Name)
        .ToList();

    client.Disconnect();
    return files;
}

        public async Task ProcessAndSaveToDb(string fileName)
        {
            var keyFile = new PrivateKeyFile(_privateKeyPath);
            using var client = new SftpClient("sftpnew.delamibrands.com", 22, "cegid-ftp", keyFile);
            client.Connect();

            var sourcePath = $"/STR-FFO-TESTING/{fileName}";
            var archivePath = $"/STR-FFO-TESTING/Archived/{fileName}";

            if (!client.Exists(sourcePath)) throw new Exception("File tidak ditemukan di SFTP.");

            // 1. STREAMING: Baca langsung dari SFTP
            using (var stream = client.OpenRead(sourcePath))
            using (var reader = new StreamReader(stream))
            {
                var batchList = new List<BarcodeData>();
                string? line;

                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    // Asumsi: File berisi SKU murni per baris
                    batchList.Add(new BarcodeData { 
                        Sku = line.Trim(), 
                        FileName = fileName 
                    });

                    // 2. BATCH INSERT: Masukkan ke DB tiap 1000 baris agar efisien
                    if (batchList.Count >= 1000)
                    {
                        await BulkInsertToDb(batchList);
                        batchList.Clear();
                    }
                }

                if (batchList.Any()) await BulkInsertToDb(batchList);
            }

            // 3. ARCHIVE: Pindahkan file setelah sukses diproses
            if (!client.Exists("/STR-FFO-TESTING/Archived")) client.CreateDirectory("/STR-FFO-TESTING/Archived");
            client.RenameFile(sourcePath, archivePath);

            client.Disconnect();
        }

        private async Task BulkInsertToDb(List<BarcodeData> data)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            const string sql = "INSERT INTO barcode_logs (sku, file_name, processed_at) VALUES (@Sku, @FileName, @ProcessedAt)";
            await conn.ExecuteAsync(sql, data);
        }
    }
}