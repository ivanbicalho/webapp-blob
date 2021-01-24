using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAppBlob.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly List<string> _uris = new List<string>();

        public string ErrorMessage { get; private set; }
        public IEnumerable<string> Uris => _uris;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public async Task OnGet()
        {
            try
            {
                var endpoint = $"https://{Environment.GetEnvironmentVariable("STORAGE_ACCOUNT_NAME")}.blob.core.windows.net/images";
                var container = new BlobContainerClient(new Uri(endpoint), new DefaultAzureCredential());

                var resultSegment = container.GetBlobsAsync().AsPages(default, 100);

                await foreach (var blobPage in resultSegment)
                {
                    foreach (var blob in blobPage.Values)
                    {
                        var uri = $"https://{Environment.GetEnvironmentVariable("FUNCTION_NAME")}.azurewebsites.net/api/{blob.Name}";
                        _uris.Add(uri);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }
    }
}
