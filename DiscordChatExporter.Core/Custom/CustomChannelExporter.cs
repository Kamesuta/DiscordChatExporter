using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DiscordChatExporter.Core.Discord;
using DiscordChatExporter.Core.Discord.Data;
using DiscordChatExporter.Core.Discord.Data.Common;
using DiscordChatExporter.Core.Exceptions;
using DiscordChatExporter.Core.Exporting;
using DiscordChatExporter.Core.Utils.Extensions;

namespace DiscordChatExporter.Core.Custom
{
    public class CustomChannelExporter : ChannelExporter
    {
        public CustomChannelExporter(DiscordClient discord) : base(discord) { }

        public CustomChannelExporter(AuthToken token) : base(token) { }

        public override async ValueTask ExportChannelAsync(ExportRequest request, IProgress<double>? progress = null)
        {
            // リクエストを上書き
            request.OutputBaseFilePath = Path.Combine(request.OutputPath, $"{request.Channel.Index:D2}-{request.Channel.Name}.{request.Format.GetFileExtension()}");

            // Build context
            var contextMembers = new HashSet<Member>(IdBasedEqualityComparer.Instance);
            var contextChannels = await _discord.GetGuildChannelsAsync(request.Guild.Id);
            var contextRoles = await _discord.GetGuildRolesAsync(request.Guild.Id);

            var context = new CustomExportContext(
                request,
                contextMembers,
                contextChannels,
                contextRoles
            );

            // Export messages
            await using var messageExporter = new MessageExporter(context);

            var exportedAnything = false;
            var encounteredUsers = new HashSet<User>(IdBasedEqualityComparer.Instance);
            await foreach (var message in _discord.GetMessagesAsync(request.Channel.Id, request.After, request.Before, progress))
            {
                // Skips any messages that fail to pass the supplied filter
                if (!request.MessageFilter.IsMatch(message))
                    continue;

                // Resolve members for referenced users
                foreach (var referencedUser in message.MentionedUsers.Prepend(message.Author))
                {
                    if (!encounteredUsers.Add(referencedUser))
                        continue;

                    var member =
                        await _discord.TryGetGuildMemberAsync(request.Guild.Id, referencedUser) ??
                        Member.CreateForUser(referencedUser);

                    contextMembers.Add(member);
                }

                // Export message
                await messageExporter.ExportMessageAsync(message);
                exportedAnything = true;
            }

            // Throw if no messages were exported
            if (!exportedAnything)
                throw DiscordChatExporterException.ChannelIsEmpty();
        }
    }
}
