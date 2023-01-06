﻿using Songify_Slim.Util.Settings;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Xml;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using static System.Convert;
using TwitchLib.Api.Helix.Models.Users.GetUsers;
using Application = System.Windows.Application;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using Settings = Songify_Slim.Util.Settings.Settings;
using System.Numerics;

namespace Songify_Slim
{
    /// <summary>
    ///     This class is for writing, exporting and importing the config file
    ///     The config file is XML and has a single config tag with attributes
    /// </summary>
    internal class ConfigHandler
    {
        public enum ConfigTypes
        {
            SpotifyCredentials,
            TwitchCredentials,
            BotConfig,
            AppConfig
        }

        public static void WriteConfig(ConfigTypes configType, object o)
        {
            string path = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            string yaml;

            switch (configType)
            {
                case ConfigTypes.SpotifyCredentials:
                    path += "/SpotifyCredentials.yaml";
                    yaml = serializer.Serialize(o as SpotifyCredentials);
                    break;
                case ConfigTypes.TwitchCredentials:
                    path += "/TwitchCredentials.yaml";
                    yaml = serializer.Serialize(o as TwitchCredentials);
                    break;
                case ConfigTypes.BotConfig:
                    path += "/BotConfig.yaml";
                    yaml = serializer.Serialize(o as BotConfig);
                    break;
                case ConfigTypes.AppConfig:
                    path += "/AppConfig.yaml";
                    yaml = serializer.Serialize(o as AppConfig);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(configType), configType, null);
            }
            File.WriteAllText(path, yaml);
        }

        public static void ReadConfig()
        {
            string path = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)  // see height_in_inches in sample yml 
                .Build();

            Configuration config = new Configuration();

            foreach (ConfigTypes ConfigType in (ConfigTypes[])Enum.GetValues(typeof(ConfigTypes)))
            {
                switch (ConfigType)
                {
                    case ConfigTypes.SpotifyCredentials:
                        if (File.Exists($@"{path}\SpotifyCredentials.yaml"))
                        {
                            var p = deserializer.Deserialize<SpotifyCredentials>(File.ReadAllText($@"{path}\SpotifyCredentials.yaml"));
                            config.SpotifyCredentials = p;
                        }
                        else
                        {
                            config.SpotifyCredentials = new SpotifyCredentials
                            {
                                AccessToken = Settings.SpotifyAccessToken,
                                RefreshToken = Settings.SpotifyRefreshToken,
                                DeviceId = Settings.SpotifyDeviceId,
                                ClientId = Settings.SpotifyDeviceId,
                                ClientSecret = Settings.ClientSecret
                            };

                        }
                        break;
                    case ConfigTypes.TwitchCredentials:
                        if (File.Exists($@"{path}\TwitchCredentials.yaml"))
                        {
                            var p = deserializer.Deserialize<TwitchCredentials>(File.ReadAllText($@"{path}\TwitchCredentials.yaml"));
                            config.TwitchCredentials = p;
                        }
                        else
                        {
                            config.TwitchCredentials = new TwitchCredentials
                            {
                                AccessToken = "",
                                ChannelName = Settings.TwChannel,
                                ChannelId = "",
                                BotAccountName = Settings.TwAcc,
                                BotOAuthToken = Settings.TwOAuth
                            };
                        }
                        break;
                    case ConfigTypes.BotConfig:
                        if (File.Exists($@"{path}\BotConfig.yaml"))
                        {
                            var p = deserializer.Deserialize<BotConfig>(File.ReadAllText($@"{path}\BotConfig.yaml"));
                            config.BotConfig = p;
                        }
                        else
                        {
                            config.BotConfig = new BotConfig
                            {
                                BotCmdNext = Settings.BotCmdNext,
                                BotCmdPos = Settings.BotCmdPos,
                                BotCmdSkip = Settings.BotCmdSkip,
                                BotCmdSkipVote = Settings.BotCmdSkipVote,
                                BotCmdSong = Settings.BotCmdSong,
                                BotCmdSkipVoteCount = Settings.BotCmdSkipVoteCount,
                                BotRespBlacklist = Settings.BotRespBlacklist,
                                BotRespError = Settings.BotRespError,
                                BotRespIsInQueue = Settings.BotRespIsInQueue,
                                BotRespLength = Settings.BotRespLength,
                                BotRespMaxReq = Settings.BotRespMaxReq,
                                BotRespModSkip = Settings.BotRespModSkip,
                                BotRespNoSong = Settings.BotRespNoSong,
                                BotRespSuccess = Settings.BotRespSuccess,
                                BotRespVoteSkip = Settings.BotRespVoteSkip,
                                OnlyWorkWhenLive = Settings.BotOnlyWorkWhenLive,
                            };
                        }
                        break;
                    case ConfigTypes.AppConfig:
                        if (File.Exists($@"{path}\AppConfig.yaml"))
                        {
                            var p = deserializer.Deserialize<AppConfig>(File.ReadAllText($@"{path}\AppConfig.yaml"));
                            config.AppConfig = p;
                        }
                        else
                        {
                            config.AppConfig = new AppConfig
                            {
                                AnnounceInChat = Settings.AnnounceInChat,
                                AppendSpaces = Settings.AppendSpaces,
                                AutoClearQueue = Settings.AutoClearQueue,
                                Autostart = Settings.Autostart,
                                CustomPauseTextEnabled = Settings.CustomPauseTextEnabled,
                                DownloadCover = Settings.DownloadCover,
                                MsgLoggingEnabled = Settings.MsgLoggingEnabled,
                                OpenQueueOnStartup = Settings.OpenQueueOnStartup,
                                SaveHistory = Settings.SaveHistory,
                                SplitOutput = Settings.SplitOutput,
                                Systray = Settings.Systray,
                                Telemetry = Settings.Telemetry,
                                TwAutoConnect = Settings.TwAutoConnect,
                                TwSrCommand = Settings.TwSrCommand,
                                TwSrReward = Settings.TwSrReward,
                                Upload = Settings.Upload,
                                UploadHistory = Settings.UploadHistory,
                                UseOwnApp = Settings.UseOwnApp,
                                MaxSongLength = Settings.MaxSongLength,
                                PosX = (int)Settings.PosX,
                                PosY = (int)Settings.PosY,
                                SpaceCount = Settings.SpaceCount,
                                TwRewardId = Settings.TwRewardId,
                                TwSrCooldown = Settings.TwSrCooldown,
                                TwSrMaxReq = Settings.TwSrMaxReq,
                                TwSrMaxReqBroadcaster = Settings.TwSrMaxReqBroadcaster,
                                TwSrMaxReqEveryone = Settings.TwSrMaxReqEveryone,
                                TwSrMaxReqModerator = Settings.TwSrMaxReqModerator,
                                TwSrMaxReqSubscriber = Settings.TwSrMaxReqSubscriber,
                                TwSrMaxReqVip = Settings.TwSrMaxReqVip,
                                TwSrUserLevel = Settings.TwSrUserLevel,
                                ArtistBlacklist = Settings.ArtistBlacklist,
                                Color = Settings.Color,
                                CustomPauseText = Settings.CustomPauseText,
                                Directory = Settings.Directory,
                                Language = Settings.Language,
                                OutputString = Settings.OutputString,
                                OutputString2 = Settings.OutputString2,
                                Theme = Settings.Theme,
                                UserBlacklist = Settings.UserBlacklist,
                                Uuid = Settings.Uuid,
                                WebServerPort = Settings.WebServerPort,
                                AutoStartWebServer = Settings.AutoStartWebServer,
                                BetaUpdates = Settings.BetaUpdates,
                                ChromeFetchRate = Settings.ChromeFetchRate,
                                Player = Settings.Player,
                                WebUserAgent = Settings.WebUserAgent,
                                BotOnlyWorkWhenLive = Settings.BotOnlyWorkWhenLive
                            };
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            Settings.Import(config);
        }

        public static void ReadXml(string path)
        {
            try
            {
                if (new FileInfo(path).Length == 0)
                {
                    //WriteXml(path);
                    return;
                }

                List<string> fileList = new List<string> { "SpotifyCredentials.yaml", "TwitchCredentials.yaml", "BotConfig.yaml", "AppConfig.yaml" };
                if (File.Exists(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) +
                                @"\SpotifyCredentials.yaml"))
                {
                    fileList.Remove("SpotifyCredentials.yaml");
                }
                if (File.Exists(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) +
                                @"\TwitchCredentials.yaml"))
                {
                    fileList.Remove("TwitchCredentials.yaml");
                }
                if (File.Exists(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) +
                                @"\BotConfig.yaml"))
                {
                    fileList.Remove("BotConfig.yaml");
                }
                if (File.Exists(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) +
                                @"\AppConfig.yaml"))
                {
                    fileList.Remove("AppConfig.yaml");
                }


                foreach (string s in fileList.Where(s => File.Exists(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) + $@"\{s}")))
                {
                    fileList.Remove(s);
                }

                if (fileList.Count == 0)
                {
                    ReadConfig();
                    return;
                }

                Config config = new Config();
                // reading the XML file, attributes get saved in Settings
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                if (doc.DocumentElement == null) return;
                foreach (XmlNode node in doc.DocumentElement.ChildNodes)
                {
                    if (node.Name != "Config") continue;
                    //Create a new Config object and set the attributes

                    if (node.Attributes == null) continue;
                    config.AccessToken = node.Attributes["accesstoken"] != null ? node.Attributes["accesstoken"].Value : "";
                    config.AnnounceInChat = node.Attributes["announceinchat"] != null && ToBoolean(node.Attributes["announceinchat"].Value);
                    config.AppendSpaces = node.Attributes["spacesenabled"] != null && ToBoolean(node.Attributes["spacesenabled"].Value);
                    config.ArtistBlacklist = node.Attributes["artistblacklist"] != null ? node.Attributes["artistblacklist"].Value.Split(new[] { "|||" }, StringSplitOptions.None).ToList() : new List<string>();
                    config.AutoClearQueue = node.Attributes["autoclearqueue"] != null && ToBoolean(node.Attributes["autoclearqueue"].Value);
                    config.Autostart = node.Attributes["autostart"] != null && ToBoolean(node.Attributes["autostart"].Value);
                    config.BotCmdNext = node.Attributes["botcmdnext"] != null && ToBoolean(node.Attributes["botcmdnext"].Value);
                    config.BotCmdPos = node.Attributes["botcmdpos"] != null && ToBoolean(node.Attributes["botcmdpos"].Value);
                    config.BotCmdSkip = node.Attributes["botcmdskip"] != null && ToBoolean(node.Attributes["botcmdskip"].Value);
                    config.BotCmdSkipVote = node.Attributes["botcmdskipvote"] != null && ToBoolean(node.Attributes["botcmdskipvote"].Value);
                    config.BotCmdSkipVoteCount = node.Attributes["botcmdskipvotecount"] != null ? ToInt32(node.Attributes["botcmdskipvotecount"].Value) : 5;
                    config.BotCmdSong = node.Attributes["botcmdsong"] != null && ToBoolean(node.Attributes["botcmdsong"].Value);
                    config.BotRespBlacklist = node.Attributes["botrespblacklist"] != null ? node.Attributes["botrespblacklist"].Value : "@{user} the Artist: {artist} has been blacklisted by the broadcaster.";
                    config.BotRespError = node.Attributes["botresperror"] != null ? node.Attributes["botresperror"].Value : "@{user} there was an error adding your Song to the queue. Error message: {errormsg}";
                    config.BotRespIsInQueue = node.Attributes["botrespisinqueue"] != null ? node.Attributes["botrespisinqueue"].Value : "@{user} this song is already in the queue.";
                    config.BotRespLength = node.Attributes["botresplength"] != null ? node.Attributes["botresplength"].Value : "@{user} the song you requested exceeded the maximum song length ({maxlength}).";
                    config.BotRespMaxReq = node.Attributes["botrespmaxreq"] != null ? node.Attributes["botrespmaxreq"].Value : "@{user} maximum number of songs in queue reached ({maxreq}).";
                    config.BotRespModSkip = node.Attributes["botrespmodskip"] != null ? node.Attributes["botrespmodskip"].Value : "@{user} skipped the current song.";
                    config.BotRespNoSong = node.Attributes["botrespnosong"] != null ? node.Attributes["botrespnosong"].Value : "@{user} please specify a song to add to the queue.";
                    config.BotRespSuccess = node.Attributes["botrespsuccess"] != null ? node.Attributes["botrespsuccess"].Value : "{artist} - {title} requested by @{user} has been added to the queue.";
                    config.BotRespVoteSkip = node.Attributes["botrespvoteskip"] != null ? node.Attributes["botrespvoteskip"].Value : "@{user} voted to skip the current song. ({votes})";
                    config.ClientId = node.Attributes["clientid"] != null ? node.Attributes["clientid"].Value : "";
                    config.ClientSecret = node.Attributes["clientsecret"] != null ? node.Attributes["clientsecret"].Value : "";
                    config.Color = node.Attributes["color"] != null ? node.Attributes["color"].Value : "Blue";
                    config.CustomPauseText = node.Attributes["customPauseText"] != null ? node.Attributes["customPauseText"].Value : "";
                    config.CustomPauseTextEnabled = node.Attributes["customPause"] != null && ToBoolean(node.Attributes["customPause"].Value);
                    config.Directory = node.Attributes["directory"] != null ? node.Attributes["directory"].Value : Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
                    config.DownloadCover = node.Attributes["downloadcover"] != null && ToBoolean(node.Attributes["downloadcover"].Value);
                    config.Language = node.Attributes["lang"] != null ? node.Attributes["lang"].Value : "en";
                    config.MaxSongLength = node.Attributes["maxsonglength"] != null ? ToInt32(node.Attributes["maxsonglength"].Value) : 10;
                    config.MsgLoggingEnabled = node.Attributes["msglogging"] != null && ToBoolean(node.Attributes["msglogging"].Value);
                    config.OpenQueueOnStartup = node.Attributes["openqueueonstartup"] != null && ToBoolean(node.Attributes["openqueueonstartup"].Value);
                    config.OutputString = node.Attributes["outputString"] != null ? node.Attributes["outputString"].Value : "{artist} - {title} {extra}";
                    config.OutputString2 = node.Attributes["outputString2"] != null ? node.Attributes["outputString2"].Value : "{artist} - {title} {extra}";
                    config.PosX = node.Attributes["posx"] != null ? ToInt32(node.Attributes["posx"].Value) : 100;
                    config.PosY = node.Attributes["posy"] != null ? ToInt32(node.Attributes["posy"].Value) : 100;
                    config.RefreshToken = node.Attributes["refreshtoken"] != null ? node.Attributes["refreshtoken"].Value : "";
                    config.SaveHistory = node.Attributes["savehistory"] != null && ToBoolean(node.Attributes["savehistory"].Value);
                    config.SpaceCount = node.Attributes["Spacecount"] != null ? ToInt32(node.Attributes["Spacecount"].Value) : 10;
                    config.SplitOutput = node.Attributes["splitoutput"] != null && ToBoolean(node.Attributes["splitoutput"].Value);
                    config.SpotifyDeviceId = node.Attributes["spotifydeviceid"] != null ? node.Attributes["spotifydeviceid"].Value : "";
                    config.Systray = node.Attributes["systray"] != null && ToBoolean(node.Attributes["systray"].Value);
                    config.Telemetry = node.Attributes["telemetry"] != null && ToBoolean(node.Attributes["telemetry"].Value);
                    config.Theme = node.Attributes["theme"] != null ? node.Attributes["theme"].Value : "Dark";
                    config.TwAcc = node.Attributes["twacc"] != null ? node.Attributes["twacc"].Value : "";
                    config.TwAutoConnect = node.Attributes["twautoconnect"] != null && ToBoolean(node.Attributes["twautoconnect"].Value);
                    config.TwChannel = node.Attributes["twchannel"] != null ? node.Attributes["twchannel"].Value : "";
                    config.TwOAuth = node.Attributes["twoauth"] != null ? node.Attributes["twoauth"].Value : "";
                    config.TwRewardId = node.Attributes["twrewardid"] != null ? node.Attributes["twrewardid"].Value : "";
                    config.TwSrCommand = node.Attributes["twsrcommand"] != null && ToBoolean(node.Attributes["twsrcommand"].Value);
                    config.TwSrCooldown = node.Attributes["twsrcooldown"] != null ? ToInt32(node.Attributes["twsrcooldown"].Value) : 5;
                    config.TwSrMaxReq = node.Attributes["twsrmaxreq"] != null ? ToInt32(node.Attributes["twsrmaxreq"].Value) : 1;
                    config.TwSrMaxReqBroadcaster = node.Attributes["twsrmaxreqbroadcaster"] != null ? ToInt32(node.Attributes["twsrmaxreqbroadcaster"].Value) : 1;
                    config.TwSrMaxReqEveryone = node.Attributes["twsrmaxreqeveryone"] != null ? ToInt32(node.Attributes["twsrmaxreqeveryone"].Value) : 1;
                    config.TwSrMaxReqModerator = node.Attributes["twsrmaxreqmoderator"] != null ? ToInt32(node.Attributes["twsrmaxreqmoderator"].Value) : 1;
                    config.TwSrMaxReqSubscriber = node.Attributes["twsrmaxreqsubscriber"] != null ? ToInt32(node.Attributes["twsrmaxreqsubscriber"].Value) : 1;
                    config.TwSrMaxReqVip = node.Attributes["twsrmaxreqvip"] != null ? ToInt32(node.Attributes["twsrmaxreqvip"].Value) : 1;
                    config.TwSrReward = node.Attributes["twsrreward"] != null && ToBoolean(node.Attributes["twsrreward"].Value);
                    config.TwSrUserLevel = node.Attributes["twsruserlevel"] != null ? ToInt32(node.Attributes["twsruserlevel"].Value) : 1;
                    config.Upload = node.Attributes["uploadSonginfo"] != null && ToBoolean(node.Attributes["uploadSonginfo"].Value);
                    config.UploadHistory = node.Attributes["uploadhistory"] != null && ToBoolean(node.Attributes["uploadhistory"].Value);
                    config.UseOwnApp = node.Attributes["useownapp"] != null && ToBoolean(node.Attributes["useownapp"].Value);
                    config.UserBlacklist = node.Attributes["userblacklist"] != null ? node.Attributes["userblacklist"].Value.Split(new[] { "|||" }, StringSplitOptions.None).ToList() : new List<string>();
                    config.Uuid = node.Attributes["uuid"] != null ? node.Attributes["uuid"].Value : "";
                }

                ConvertConfig(config);

            }
            catch (Exception ex)
            {
                Logger.LogExc(ex);
            }
        }

        public static void LoadConfig(string path = "")
        {
            if (path != "")
            {
                ReadXml(path);
            }
            else
            {
                // OpenfileDialog with settings initialdirectory is the path were the exe is located
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    InitialDirectory = AppDomain.CurrentDomain.BaseDirectory,
                    Filter = @"XML files (*.xml)|*.xml|All files (*.*)|*.*"
                };

                // Opening the dialog and when the user hits "OK" the following code gets executed
                if (openFileDialog.ShowDialog() == DialogResult.OK) ReadXml(openFileDialog.FileName);

                // This will iterate through all windows of the software, if the window is typeof 
                // Settingswindow (from there this class is called) it calls the method SetControls
                Application.Current.Dispatcher.Invoke(() =>
                {
                    foreach (Window window in Application.Current.Windows)
                        if (window.GetType() == typeof(Window_Settings))
                            ((Window_Settings)window).SetControls();
                });
            }
        }

        public static void ConvertConfig(Config cfg)
        {
            SpotifyCredentials sp = new SpotifyCredentials
            {
                AccessToken = cfg.AccessToken,
                RefreshToken = cfg.RefreshToken,
                DeviceId = cfg.SpotifyDeviceId,
                ClientId = cfg.ClientId,
                ClientSecret = cfg.ClientSecret,
            };
            WriteConfig(ConfigTypes.SpotifyCredentials, sp);

            TwitchCredentials twitchCredentials = new TwitchCredentials
            {
                AccessToken = "",
                ChannelName = cfg.TwChannel,
                ChannelId = "",
                BotAccountName = cfg.TwAcc,
                BotOAuthToken = cfg.TwOAuth
            };
            WriteConfig(ConfigTypes.TwitchCredentials, twitchCredentials);

            BotConfig botConfig = new BotConfig
            {
                BotCmdNext = cfg.BotCmdNext,
                BotCmdPos = cfg.BotCmdPos,
                BotCmdSkip = cfg.BotCmdSkip,
                BotCmdSkipVote = cfg.BotCmdSkipVote,
                BotCmdSong = cfg.BotCmdSong,
                BotCmdSkipVoteCount = cfg.BotCmdSkipVoteCount,
                BotRespBlacklist = cfg.BotRespBlacklist,
                BotRespError = cfg.BotRespError,
                BotRespIsInQueue = cfg.BotRespIsInQueue,
                BotRespLength = cfg.BotRespLength,
                BotRespMaxReq = cfg.BotRespMaxReq,
                BotRespModSkip = cfg.BotRespModSkip,
                BotRespNoSong = cfg.BotRespNoSong,
                BotRespSuccess = cfg.BotRespSuccess,
                BotRespVoteSkip = cfg.BotRespVoteSkip,
            };
            WriteConfig(ConfigTypes.BotConfig, botConfig);

            AppConfig appConfig = new AppConfig
            {
                AnnounceInChat = cfg.AnnounceInChat,
                AppendSpaces = cfg.AppendSpaces,
                AutoClearQueue = cfg.AutoClearQueue,
                Autostart = cfg.Autostart,
                CustomPauseTextEnabled = cfg.CustomPauseTextEnabled,
                DownloadCover = cfg.DownloadCover,
                MsgLoggingEnabled = cfg.MsgLoggingEnabled,
                OpenQueueOnStartup = cfg.OpenQueueOnStartup,
                SaveHistory = cfg.SaveHistory,
                SplitOutput = cfg.SplitOutput,
                Systray = cfg.Systray,
                Telemetry = cfg.Telemetry,
                TwAutoConnect = cfg.TwAutoConnect,
                TwSrCommand = cfg.TwSrCommand,
                TwSrReward = cfg.TwSrReward,
                Upload = cfg.Upload,
                UploadHistory = cfg.UploadHistory,
                UseOwnApp = cfg.UseOwnApp,
                MaxSongLength = cfg.MaxSongLength,
                PosX = cfg.PosX,
                PosY = cfg.PosY,
                SpaceCount = cfg.SpaceCount,
                TwSrCooldown = cfg.TwSrCooldown,
                TwSrMaxReq = cfg.TwSrMaxReq,
                TwSrMaxReqBroadcaster = cfg.TwSrMaxReqBroadcaster,
                TwSrMaxReqEveryone = cfg.TwSrMaxReqEveryone,
                TwSrMaxReqModerator = cfg.TwSrMaxReqModerator,
                TwSrMaxReqSubscriber = cfg.TwSrMaxReqSubscriber,
                TwSrMaxReqVip = cfg.TwSrMaxReqVip,
                TwSrUserLevel = cfg.TwSrUserLevel,
                ArtistBlacklist = cfg.ArtistBlacklist,
                Color = cfg.Color,
                CustomPauseText = cfg.CustomPauseText,
                Directory = cfg.Directory,
                Language = cfg.Language,
                OutputString = cfg.OutputString,
                OutputString2 = cfg.OutputString2,
                Theme = cfg.Theme,
                UserBlacklist = cfg.UserBlacklist,
                Uuid = cfg.Uuid,
            };
            WriteConfig(ConfigTypes.AppConfig, appConfig);
        }

        public static void WriteAllConfig(Configuration config)
        {
            WriteConfig(ConfigTypes.AppConfig, config.AppConfig);
            WriteConfig(ConfigTypes.BotConfig, config.BotConfig);
            WriteConfig(ConfigTypes.SpotifyCredentials, config.SpotifyCredentials);
            WriteConfig(ConfigTypes.TwitchCredentials, config.TwitchCredentials);
        }
    }

    public class Configuration
    {
        public AppConfig AppConfig { get; set; }
        public SpotifyCredentials SpotifyCredentials { get; set; }
        public TwitchCredentials TwitchCredentials { get; set; }
        public BotConfig BotConfig { get; set; }
    }

    public class SpotifyCredentials
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string DeviceId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }

    public class TwitchCredentials
    {
        public string AccessToken { get; set; }
        public string ChannelName { get; set; }
        public string ChannelId { get; set; }
        public string BotAccountName { get; set; }
        public string BotOAuthToken { get; set; }
        public User TwitchUser { get; set; }
    }

    public class BotConfig
    {
        public bool BotCmdNext { get; set; }
        public bool BotCmdPos { get; set; }
        public bool BotCmdSkip { get; set; }
        public bool BotCmdSkipVote { get; set; }
        public bool BotCmdSong { get; set; }
        public int BotCmdSkipVoteCount { get; set; }
        public string BotRespBlacklist { get; set; }
        public string BotRespError { get; set; }
        public string BotRespIsInQueue { get; set; }
        public string BotRespLength { get; set; }
        public string BotRespMaxReq { get; set; }
        public string BotRespModSkip { get; set; }
        public string BotRespNoSong { get; set; }
        public string BotRespSuccess { get; set; }
        public string BotRespVoteSkip { get; set; }
        public string BotRespPos { get; set; }
        public string BotRespNext { get; set; }
        public bool OnlyWorkWhenLive { get; set; }
    }

    public class AppConfig
    {
        public bool AnnounceInChat { get; set; }
        public bool AppendSpaces { get; set; }
        public bool AutoClearQueue { get; set; }
        public bool Autostart { get; set; }
        public bool CustomPauseTextEnabled { get; set; }
        public bool DownloadCover { get; set; }
        public bool MsgLoggingEnabled { get; set; }
        public bool OpenQueueOnStartup { get; set; }
        public bool SaveHistory { get; set; }
        public bool SplitOutput { get; set; }
        public bool Systray { get; set; }
        public bool Telemetry { get; set; }
        public bool TwAutoConnect { get; set; }
        public bool TwSrCommand { get; set; }
        public bool TwSrReward { get; set; }
        public bool Upload { get; set; }
        public bool UploadHistory { get; set; }
        public bool UseOwnApp { get; set; }
        public int MaxSongLength { get; set; }
        public int PosX { get; set; }
        public int PosY { get; set; }
        public int SpaceCount { get; set; }
        public int TwSrCooldown { get; set; }
        public int TwSrMaxReq { get; set; }
        public int TwSrMaxReqBroadcaster { get; set; }
        public int TwSrMaxReqEveryone { get; set; }
        public int TwSrMaxReqModerator { get; set; }
        public int TwSrMaxReqSubscriber { get; set; }
        public int TwSrMaxReqVip { get; set; }
        public int TwSrUserLevel { get; set; }
        public string TwRewardId { get; set; }
        public int[] RefundConditons { get; set; }
        public List<string> ArtistBlacklist { get; set; }
        public string Color { get; set; }
        public string CustomPauseText { get; set; }
        public string Directory { get; set; }
        public string Language { get; set; }
        public string OutputString { get; set; }
        public string OutputString2 { get; set; }
        public string Theme { get; set; }
        public List<string> UserBlacklist { get; set; }
        public string Uuid { get; set; }
        public int WebServerPort { get; set; }
        public bool AutoStartWebServer { get; set; }
        public bool BetaUpdates { get; set; }
        public int ChromeFetchRate { get; set; }
        public int Player { get; internal set; }
        public string WebUserAgent { get; set; }
        public bool UpdateRequired { get; set; }
        public bool BotOnlyWorkWhenLive { get; set; }
    }

    public class Config
    {
        // Create fields for each setting in the config file
        public bool AnnounceInChat { get; set; }
        public bool AppendSpaces { get; set; }
        public bool AutoClearQueue { get; set; }
        public bool Autostart { get; set; }
        public bool BotCmdNext { get; set; }
        public bool BotCmdPos { get; set; }
        public bool BotCmdSkip { get; set; }
        public bool BotCmdSkipVote { get; set; }
        public bool BotCmdSong { get; set; }
        public bool CustomPauseTextEnabled { get; set; }
        public bool DownloadCover { get; set; }
        public bool MsgLoggingEnabled { get; set; }
        public bool OpenQueueOnStartup { get; set; }
        public bool SaveHistory { get; set; }
        public bool SplitOutput { get; set; }
        public bool Systray { get; set; }
        public bool Telemetry { get; set; }
        public bool TwAutoConnect { get; set; }
        public bool TwSrCommand { get; set; }
        public bool TwSrReward { get; set; }

        public bool Upload { get; set; }
        public bool UploadHistory { get; set; }
        public bool UseOwnApp { get; set; }
        public int BotCmdSkipVoteCount { get; set; }
        public int MaxSongLength { get; set; }
        public int PosX { get; set; }
        public int PosY { get; set; }
        public int SpaceCount { get; set; }
        public int TwSrCooldown { get; set; }
        public int TwSrMaxReq { get; set; }
        public int TwSrMaxReqBroadcaster { get; set; }
        public int TwSrMaxReqEveryone { get; set; }
        public int TwSrMaxReqModerator { get; set; }
        public int TwSrMaxReqSubscriber { get; set; }
        public int TwSrMaxReqVip { get; set; }
        public int TwSrUserLevel { get; set; }
        public string AccessToken { get; set; }
        public List<string> ArtistBlacklist { get; set; }

        public string BotRespBlacklist { get; set; }
        public string BotRespError { get; set; }
        public string BotRespIsInQueue { get; set; }
        public string BotRespLength { get; set; }
        public string BotRespMaxReq { get; set; }
        public string BotRespModSkip { get; set; }
        public string BotRespNoSong { get; set; }
        public string BotRespSuccess { get; set; }
        public string BotRespVoteSkip { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Color { get; set; }
        public string CustomPauseText { get; set; }
        public string Directory { get; set; }
        public string Language { get; set; }
        public string NbUser { get; set; }
        public string NbUserId { get; set; }
        public string OutputString { get; set; }
        public string OutputString2 { get; set; }
        public string RefreshToken { get; set; }
        public string SpotifyDeviceId { get; set; }
        public string Theme { get; set; }
        public string TwAcc { get; set; }
        public string TwChannel { get; set; }
        public string TwOAuth { get; set; }
        public string TwRewardId { get; set; }
        public List<string> UserBlacklist { get; set; }
        public string Uuid { get; set; }
    }
}