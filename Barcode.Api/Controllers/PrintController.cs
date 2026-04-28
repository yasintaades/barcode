using Microsoft.AspNetCore.Mvc;
using Barcode.Api.Services; // Pastikan namespace service benar
using Barcode.Api.Models;   // FIX ERROR: Agar 'BarcodeFile' ditemukan
using Dapper;
using Npgsql;

namespace Barcode.Api.Controllers
{
    [ApiController]
    [Route("barcode")]
    public class BarcodeController : ControllerBase
    {
        private readonly ISftpService _sftpService;
        private readonly IConfiguration _config; // FIX ERROR CS0103: Deklarasi _config

        // Tambahkan IConfiguration ke constructor
        public BarcodeController(ISftpService sftpService, IConfiguration config)
        {
            _sftpService = sftpService;
            _config = config; // Inisialisasi _config
        }

        [HttpGet("available-files")]
public async Task<IActionResult> GetFiles()
{
    try 
    {
        // Cek SFTP
        var sftpFiles = _sftpService.ListFiles();

        // Cek DB
        using var conn = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
        
        // Gunakan 'ToList()' agar data langsung dieksekusi
        var dbFiles = (await conn.QueryAsync<BarcodeFile>("SELECT * FROM barcode_files")).ToList();

        var response = sftpFiles.Select(name => {
            var dbEntry = dbFiles.FirstOrDefault(x => x.FileName == name);
            return new {
                id = dbEntry?.Id ?? 0,
                fileName = name,
                sourceType = "SFTP (Cegid)",
                statusPrint = dbEntry?.StatusPrint ?? "new",
                syncDate = dbEntry?.SyncDate ?? DateTime.Now,
                poNumber = dbEntry?.PoNumber ?? "-",
                totalArticle = dbEntry?.TotalArticle ?? 0,
                totalQty = dbEntry?.TotalQty ?? 0
            };
        });

        return Ok(response);
    }
    catch (Exception ex)
    {
        // Ini akan membantu Anda melihat error asli di Tab Network Browser
        return StatusCode(500, new { 
            message = "Terjadi kesalahan di Backend", 
            detail = ex.Message,
            stack = ex.StackTrace 
        });
    }
}
    }
}