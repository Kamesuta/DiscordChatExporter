using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DiscordChatExporter.Core.Discord.Data;
using DiscordChatExporter.Core.Exporting;
using DiscordChatExporter.Core.Utils;
using DiscordChatExporter.Core.Utils.Extensions;
using Tyrrrz.Extensions;

namespace DiscordChatExporter.Core.Custom
{
    internal class CustomMediaDownloader : MediaDownloader
    {
        private int _num = 0;
        private ExportRequest _request;

        public CustomMediaDownloader(HttpClient httpClient, string workingDirPath, bool reuseMedia, ExportRequest request)
            : base(httpClient, workingDirPath, reuseMedia)
        {
            _request = request;
        }

        public CustomMediaDownloader(string workingDirPath, bool reuseMedia, ExportRequest request)
            : base(workingDirPath, reuseMedia)
        {
            _request = request;
        }

        protected override string GetFileNameFromUrl(string url)
        {
            _num++;

            // Try to extract file name from URL
            var fileName = Regex.Match(url, @".+/([^?]*)").Groups[1].Value;

            // If it's not there, just use the URL hash as the file name
            if (string.IsNullOrWhiteSpace(fileName))
                return $"{_request.Channel.Name}_{_num}.png";

            // Otherwise, use the original file name but inject the hash in the middle
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            var fileExtension = Path.GetExtension(fileName);
            var fileExtensionOrPng = fileExtension.IsNullOrEmpty() ? ".png" : fileExtension;

            return $"{_request.Channel.Index:D2}-{_request.Channel.Name}-{_num}_{fileNameWithoutExtension}{fileExtensionOrPng}";
        }
    }
}
