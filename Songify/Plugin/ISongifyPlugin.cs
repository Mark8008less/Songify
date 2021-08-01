﻿using Songify.Model;

namespace Songify.Plugin
{
    public interface ISongifyPlugin
    {
        public PluginTypes PluginType { get; }
        public string Author { get; }
        public string Version { get; }
        public string PluginName { get; }
        public string SourceName { get; }
        public Song Song { get; protected set; }

        /// <summary>
        /// <para>
        /// If this plugin is of type SongFetcher, this will be called every time this plugin is selected as the song information source.
        /// </para>
        /// <para>
        /// If this plugin is of type FeatureExtension, this will be called once the plugin has been loaded.
        /// </para>
        /// </summary>
        public void Initialize();
        
        /// <summary>
        /// This method will be called when the settings for this plugin have been modified.
        /// </summary>
        public void RestartFetchProcess();
    }
}
