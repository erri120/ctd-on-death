using NetScriptFramework.Tools;

namespace CTDDeath
{
    public class Settings
    {
        [ConfigValue(nameof(DeleteAutoSaves), "Delete Auto Saves on Death", "Auto saves for the current character will be deleted on death.")]
        public bool DeleteAutoSaves { get; set; } = false;

        [ConfigValue(nameof(DeleteQuickSaves), "Delete Quick Saves on Death", "Quick saves for the current character will be deleted on death.")]
        public bool DeleteQuickSaves { get; set; } = false;

        [ConfigValue(nameof(DeleteSaves), "Delete Save File on Death", "Regular saves for the current character will be deleted on death.")]
        public bool DeleteSaves { get; set; } = false;

        [ConfigValue(nameof(DeleteNamedSaves), "Delete Named Saves on Death", "Named saves, created using the save command, will be deleted on death. This is achieved by parsing all named saves and checking the character name in the save. This means that if you have multiple characters with the same name, all of their named saves will be deleted. This setting is also not really efficient as the save file has to be opened, read and parsed.")]
        public bool DeleteNamedSaves { get; set; } = false;

        [ConfigValue(nameof(UninstallSkyrim), "Uninstall Skyrim on Death", "Setting this option to True will make Steam uninstall your Skyrim installation on death.")]
        public bool UninstallSkyrim { get; set; } = false;

        internal void Load()
        {
            ConfigFile.LoadFrom(this, "CTD on Death", true);
        }
    }
}
