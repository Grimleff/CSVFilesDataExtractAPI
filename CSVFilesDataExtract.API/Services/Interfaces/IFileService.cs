using System.IO;
using System.Threading.Tasks;

namespace CSVFilesDataExtract.API.Services.Interfaces;

public interface IFileService
{
    Task<string> DownloadFile(string url);
}