namespace Barcode.Api.Models
{
    public class BarcodeFile
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string? StatusPrint { get; set; }
        public DateTime? SyncDate { get; set; }
        public string? PoNumber { get; set; }
        public int TotalArticle { get; set; }
        public int TotalQty { get; set; }
    }
}