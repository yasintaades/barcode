namespace Barcode.Api.Models
{
    public class BarcodeData
    {
        public int Id { get; set; }
        public string Sku { get; set; }
        public string FileName { get; set; }
        public DateTime ProcessedAt { get; set; } = DateTime.Now;
    }
}