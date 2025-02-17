using System.Text.Json;
using JsonExtensions.Reading;

namespace DiscordChatExporter.Core.Discord.Data.Embeds
{
    // https://discord.com/developers/docs/resources/channel#embed-object-embed-image-structure
    public partial class EmbedImage
    {
        public string? Url { get; }

        public string? ProxyUrl { get; }

        public int? Width { get; }

        public int? Height { get; }

        public EmbedImage(string? url, string? proxyUrl, int? width, int? height)
        {
            Url = url;
            ProxyUrl = proxyUrl;
            Height = height;
            Width = width;
        }
    }

    public partial class EmbedImage
    {
        public static EmbedImage Parse(JsonElement json)
        {
            var url = json.GetPropertyOrNull("url")?.GetString();
            var proxyUrl = json.GetPropertyOrNull("proxy_url")?.GetString();
            var width = json.GetPropertyOrNull("width")?.GetInt32();
            var height = json.GetPropertyOrNull("height")?.GetInt32();

            return new EmbedImage(url, proxyUrl, width, height);
        }
    }
}