using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Authentication.ExtendedProtection;
using System.Threading;
using System.Threading.Tasks;

namespace LBSScreen.Bot
{
    internal class DiscordBot
    {
        public event Action UpdatedImages;

        private DiscordSocketClient _client;
        private TaskCompletionSource<object> _delayTask;
        private string token;
        private ulong guildId;
        private ulong adminRoleId;

        public DiscordBot()
        {
            Logger.Log("Starting bot");

            token = Settings.GetData<string>("token");
            guildId = Settings.GetData<ulong>("guildId");
            adminRoleId = Settings.GetData<ulong>("roleId");

            _delayTask = new TaskCompletionSource<object>();

            Thread thread = new Thread(() =>
            {
                StartBot().Wait();
            });

            thread.Start();

            Logger.Log("Bot has started");
        }

        private async Task StartBot()
        {
            DiscordSocketConfig config = new DiscordSocketConfig()
            {
                GatewayIntents = GatewayIntents.MessageContent | GatewayIntents.Guilds | GatewayIntents.GuildMembers,
                AlwaysDownloadUsers = true
            };

            _client = new DiscordSocketClient(config);
            
            _client.Log += Log;
            _client.Ready += ClientReady;
            _client.SlashCommandExecuted += SlashCommandHandler;

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await _delayTask.Task;

            await _client.LogoutAsync();
            await _client.StopAsync();
        }

        private async Task SlashCommandHandler(SocketSlashCommand command)
        {
            ITextChannel channel = await _client.GetChannelAsync((ulong)command.ChannelId) as ITextChannel;
            IEnumerable<IMessage> messages = await channel.GetMessagesAsync(10).FlattenAsync();

            await command.RespondAsync("Updaterar bilder...", ephemeral: true);


            foreach (IMessage message in messages)
            {
                if (message.Attachments.Count == 0 && message.Embeds.Count == 0) continue;

                if (message.Reactions.Count == 0) continue;

                IEnumerable<IUser> users = await message.GetReactionUsersAsync(new Emoji("\U0001F44D"), 100).FlattenAsync();

                bool hasAdminReacted = false;

                foreach (IUser user in users)
                {
                    if (user.IsBot) continue;

                    IGuildUser socketGuildUser = await channel.Guild.GetUserAsync(user.Id);
                    
                    foreach (ulong role in socketGuildUser.RoleIds)
                    {
                        if (role == adminRoleId)
                        {
                            hasAdminReacted = true;
                            break;
                        }
                    }

                    if (hasAdminReacted) break;
                    //if (socketGuildUser.Roles.Contains(adminRole))
                    //{
                    //    hasAdminReacted = true;
                    //    break;
                    //}
                }

                if (hasAdminReacted == false)
                    continue;

                foreach (Attachment attachment in message.Attachments)
                {
                    Downloader downloader = new Downloader();
                    string extension = "";
                    string name = "";

                    switch (attachment.ContentType)
                    {
                        case "image/png":
                            name = Hash.CalculateHash(attachment.Url).ToString();
                            extension = ".png";
                            break;
                        case "image/jpeg":
                            name = Hash.CalculateHash(attachment.Url).ToString();
                            extension = ".jpg";
                            break;
                        default:
                            downloader = null;
                            break;
                    }

                    await downloader?.Download(attachment.ProxyUrl, name, extension);
                }

                foreach (Embed embed in message.Embeds)
                {
                    Downloader downloader = new Downloader();
                    string extension = "";
                    string name = "";
                    string url = "";

                    switch (embed.Type)
                    {
                        case EmbedType.Gifv:
                            name = Hash.CalculateHash(embed.Url).ToString();
                            extension = ".mp4";
                            url = embed.Video?.Url ?? "";
                            break;
                        case EmbedType.Image:
                            name = Hash.CalculateHash(embed.Url).ToString();
                            extension = ".gif";
                            url = embed.Url;
                            break;
                        default:
                            downloader = null;
                            break;
                    }

                    await downloader?.Download(url, name, extension);
                }
            }

            UpdatedImages?.Invoke();

            await command.ModifyOriginalResponseAsync((MessageProperties a) => a.Content = "Klart!");
        }

        private async Task ClientReady()
        {
            SocketGuild guild = _client.GetGuild(guildId);
            SlashCommandBuilder guildCommand = new SlashCommandBuilder()
                .WithName("updatera")
                .WithDescription("Hämtar de senaste bilderna och lägger upp dem på skärmen")
                .WithDefaultMemberPermissions(GuildPermission.Administrator);

            try
            {
                await guild.CreateApplicationCommandAsync(guildCommand.Build());
            }

            catch (HttpException exception)
            {
                string json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
                Logger.Error(json);
            }
        }

        private Task Log(LogMessage message)
        {
            Logger.Log(message.ToString(prependTimestamp: false));
            return Task.CompletedTask;
        }

        public void Stop() 
        {
            _delayTask.SetResult(null);
        }
    }
}
