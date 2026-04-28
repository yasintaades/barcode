using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Barcode.Api.Services
{
    public interface ISftpService
    {
        List<string> ListFiles();
        Task ProcessAndSaveToDb(string fileName);
    }
}