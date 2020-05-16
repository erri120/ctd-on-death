using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NetScriptFramework.SkyrimSE;
// ReSharper disable InconsistentlySynchronizedField

namespace CTDDeath
{
    public class Plugin : NetScriptFramework.Plugin
    {
        public override string Key => "erri120.crash-on-death";
        public override string Name => "Crash on Death";
        public override int Version => 1;

        public override string Author => "erri120";
        public override string Website => "https://github.com/erri120/ctd-on-death";

        public override int RequiredFrameworkVersion => 9;
        public override int RequiredLibraryVersion => 13;

        private bool _inMainMenu;
        private Settings _settings;
        private readonly object _lockObject = new object();

        private static string DocumentsFolder => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static string SavesFolder => Path.Combine(DocumentsFolder, "My Games", "Skyrim Special Edition", "Saves");
        private static bool SavesFolderExists => Directory.Exists(SavesFolder);

        protected override bool Initialize(bool loadedAny)
        {
            _settings = new Settings();
            _settings.Load();

            Events.OnMainMenu.Register(e =>
            {
                _inMainMenu = e.Entering;
            });

            Events.OnFrame.Register(e =>
            {
                if (Main.Instance == null)
                    return;

                if (_inMainMenu || Main.Instance.IsGamePaused)
                    return;

                var player = PlayerCharacter.Instance;
                if (player == null)
                    return;

                if (!player.IsDead)
                    return;

                lock (_lockObject)
                {
                    var killer = player.KilledBy;

                    Utils.Log(killer == null
                        ? $"{player.BaseActor.Name} died."
                        : $"{player.BaseActor.Name} was killed by {killer.BaseActor.Name}.");

                    if (!SavesFolderExists)
                    {
                        Main.Instance.QuitGame = true;
                        return;
                    }

                    if(_settings.DeleteSaves || _settings.DeleteQuickSaves || _settings.DeleteAutoSaves || _settings.DeleteNamedSaves)
                        DeleteSaves(player);

                    if(_settings.UninstallSkyrim)
                        Process.Start("explorer.exe", "steam://uninstall/489830");

                    Main.Instance.QuitGame = true;
                }
            });

            return true;
        }

        public void DeleteSaves(PlayerCharacter player)
        {
            List<string> files = Directory.EnumerateFiles(SavesFolder)
                .Where(File.Exists)
                .Where(x => x.EndsWith(".ess") || x.EndsWith(".skse"))
                .OrderByDescending(File.GetLastWriteTime)
                .Select(x => x.Replace(SavesFolder, ""))
                .Select(x =>
                {
                    if (x.StartsWith("\\"))
                        x = x.Substring(1, x.Length - 1);
                    return x;
                })
                .ToList();

            if (files.Count == 0)
                return;

            var first = files.First();
            var regex = new Regex(@"^Save\d+_");
            var match = regex.Match(first);

            if (!match.Success)
                return;

            var substring = first.Replace(match.Value, "");
            string[] split = substring.Split('_');

            if (split.Length == 0)
                return;

            var id = split[0];

            var filesToDelete = new List<string>();

            if (_settings.DeleteSaves)
            {
                var regularSavesRegex = new Regex($@"^Save\d+_({id})+[^.]+.(ess|skse)$");
                filesToDelete.AddRange(files
                    .Where(x => regularSavesRegex.Match(x).Success)
                    .Select(x => Path.Combine(SavesFolder, x)));
            }

            if (_settings.DeleteNamedSaves)
            {
                var currentPlayerName = string.IsNullOrWhiteSpace(player.Name) ? player.BaseActor.Name : player.Name;
                var namedSaves = files
                    .Where(x => !x.StartsWith("Save") && !x.StartsWith("Autosave") && !x.StartsWith("Quicksave"))
                    .Where(x => x.EndsWith(".ess"))
                    .Select(x => Path.Combine(SavesFolder, x))
                    .ToList();

                namedSaves.Do(x =>
                {
                    if (!File.Exists(x))
                        return;

                    try
                    {
                        using (var stream = File.OpenRead(x))
                        using (var br = new BinaryReader(stream))
                        {
                            char[] magic = br.ReadChars(13);
                            var magicString = new string(magic);
                            if (magicString != "TESV_SAVEGAME")
                                return;

                            br.ReadUInt32(); //header size
                            br.ReadUInt32(); //version
                            br.ReadUInt32(); //save number
                            var playerName = br.ReadWString();
                            if (playerName != currentPlayerName)
                                return;

                            Utils.Log(
                                $"Player name in save {x} is \"{playerName}\" and matches current name \"{currentPlayerName}\"");
                            filesToDelete.Add(x);
                        }
                    }
                    catch (Exception e)
                    {
                        Utils.Log($"Unable to open, read or parse save file {x}: {e}");
                    }
                });
            }

            if (_settings.DeleteAutoSaves)
            {
                var autoSavesRegex = new Regex($@"^Autosave\d+_({id})+[^.]+.(ess|skse)$");
                filesToDelete.AddRange(files
                    .Where(x => autoSavesRegex.Match(x).Success)
                    .Select(x => Path.Combine(SavesFolder, x)));
            }

            if (_settings.DeleteQuickSaves)
            {
                var quickSaveRegex = new Regex($@"^Quicksave\d+_({id})+[^.]+.(ess|skse)$");
                filesToDelete.AddRange(files
                    .Where(x => quickSaveRegex.Match(x).Success)
                    .Select(x => Path.Combine(SavesFolder, x)));
            }

            Utils.Log($"Found {filesToDelete.Count} files to delete");
            var total = 0;
            filesToDelete.Do(x =>
            {
                try
                {
                    File.Delete(x);
                    Utils.Log($"Deleted file {x}");
                    total += 1;
                }
                catch (Exception e)
                {
                    Utils.Log($"Could not delete file {x}: {e}");
                }
            });

            Utils.Log($"Deleted {total}/{filesToDelete.Count} files");
        }
    }
}
