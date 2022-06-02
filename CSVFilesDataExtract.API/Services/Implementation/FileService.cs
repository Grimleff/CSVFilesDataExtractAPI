using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using CSVFilesDataExtract.API.Services.Interfaces;

namespace CSVFilesDataExtract.API.Services.Implementation;

public class FileService : IFileService
{
    private readonly HttpClient _httpClient;

    public FileService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> DownloadFile(string url)
    {
        using var file = await _httpClient.GetAsync(url);
        var fileContent = await file.Content.ReadAsStringAsync();
        return fileContent;
    }
}