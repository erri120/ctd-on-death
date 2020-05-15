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

        internal void Load()
        {
            ConfigFile.LoadFrom(this, "CTD on Death", true);
        }
    }
}
