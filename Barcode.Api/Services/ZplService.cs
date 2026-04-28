public class ZplService
{
    public string GenerateZpl(string sku)
    {
        // ^XA = Start, ^FO = Position, ^BY = Barcode Width
        // ^BC = Code 128, ^FD = Data, ^XZ = End
        return $"^XA^FO50,50^BY3^BCN,100,Y,N,N^FD{sku}^FS^XZ";
    }
}