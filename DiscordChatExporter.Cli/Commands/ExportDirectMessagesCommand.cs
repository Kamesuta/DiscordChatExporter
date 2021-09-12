using System.Linq;
using System.Threading.Tasks;
using CliFx.Attributes;
using CliFx.Infrastructure;
using DiscordChatExporter.Cli.Commands.Base;
using DiscordChatExporter.Core.Discord.Data;
using DiscordChatExporter.Core.Utils.Extensions;

namespace DiscordChatExporter.Cli.Commands
{
    [Command("exportdm", Description = "Export all direct message channels.")]
    public class ExportDirectMessagesCommand : ExportCommandBase
    {
        public override async ValueTask ExecuteAsync(IConsole console)
        {
            await base.ExecuteAsync(console);

            // Get channel metadata
            await console.Output.WriteLineAsync("Fetching channels...");
            var channels = await Discord.GetGuildChannelsAsync(Guild.DirectMessages.Id);
            var textChannels = channels
                .Where(c => c.IsTextChannel)
                .Where(c => {
                    // 日付の範囲が設定されていない場合はtrue
                    if (!Before.HasValue && !After.HasValue)
                        return true;

                    // メッセージが一度も送られたことがない人はfalse
                    if (!c.LastMessageId.HasValue)
                        return false;

                    // 始端が指定されていて、メッセージが範囲外の場合はfalse
                    if (Before.HasValue && Before.Value.CompareTo(c.LastMessageId.Value) < 0)
                        return false;

                    // 終端が指定されていて、メッセージが範囲外のときはfalse
                    if (After.HasValue && After.Value.CompareTo(c.LastMessageId.Value) > 0)
                        return false;

                    return true;
                }).ToArray();

            // Export
            await ExportAsync(console, textChannels);
        }
    }
}