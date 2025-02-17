﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DiscordChatExporter.Core.Discord.Data;
using DiscordChatExporter.Core.Exporting;
using Tyrrrz.Extensions;

namespace DiscordChatExporter.Core.Custom
{
    internal class CustomExportContext : ExportContext
    {
        public CustomExportContext(
            ExportRequest request,
            IReadOnlyCollection<Member> members,
            IReadOnlyCollection<Channel> channels,
            IReadOnlyCollection<Role> roles)
            : base(request, members, channels, roles)
        {
            _mediaDownloader = new CustomMediaDownloader(request.OutputBaseDirPath, request.ShouldReuseMedia, request);
        }

        public override async ValueTask<string> ResolveMediaUrlAsync(string url, MediaType type)
        {
            if (!Request.ShouldDownloadMedia)
                return url;

            // 添付ファイルと埋め込み画像以外は無視
            if (type != MediaType.Attachment && type != MediaType.Embed)
                return url;

            try
            {
                var filePath = await _mediaDownloader.DownloadAsync(url);

                // We want relative path so that the output files can be copied around without breaking.
                // Base directory path may be null if the file is stored at the root or relative to working directory.
                var relativeFilePath = !string.IsNullOrWhiteSpace(Request.OutputBaseDirPath)
                    ? Path.GetRelativePath(Request.OutputBaseDirPath, filePath)
                    : filePath;

                // HACK: for HTML, we need to format the URL properly
                if (Request.Format is ExportFormat.HtmlDark or ExportFormat.HtmlLight)
                {
                    // Need to escape each path segment while keeping the directory separators intact
                    return relativeFilePath
                        .Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                        .Select(Uri.EscapeDataString)
                        .JoinToString(Path.AltDirectorySeparatorChar.ToString());
                }

                return relativeFilePath;
            }
            // Try to catch only exceptions related to failed HTTP requests
            // https://github.com/Tyrrrz/DiscordChatExporter/issues/332
            // https://github.com/Tyrrrz/DiscordChatExporter/issues/372
            catch (Exception ex) when (ex is HttpRequestException or OperationCanceledException)
            {
                // TODO: add logging so we can be more liberal with catching exceptions
                // We don't want this to crash the exporting process in case of failure
                return url;
            }
        }
    }
}
