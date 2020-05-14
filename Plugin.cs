using NetScriptFramework.SkyrimSE;

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

        protected override bool Initialize(bool loadedAny)
        {
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

                if (player.IsDead)
                    Main.Instance.QuitGame = true;
            });

            return true;
        }
    }
}
