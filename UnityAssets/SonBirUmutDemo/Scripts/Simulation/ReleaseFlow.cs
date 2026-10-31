using System;

namespace SonBirUmutDemo.Simulation
{
    public enum ReleaseScreen { MainMenu, Playing, Paused, HowToPlay, Settings, Ending }
    public enum ReleaseCommand { NewGame, StartGameplay, Pause, Resume, Restart, HowToPlay, Settings, Back, MainMenu }
    public enum EscapeResult { None, CloseCards, MenuChanged }

    // UI shell only. Game phases, economy, tasks and win conditions stay in GameRunState.
    public sealed class ReleaseFlow
    {
        private readonly Func<GameRunState> createRun;
        private ReleaseScreen parent = ReleaseScreen.MainMenu;
        private bool focused = true, nativePaused;
        public GameRunState Run { get; private set; }
        public ReleaseScreen Screen { get; private set; } = ReleaseScreen.MainMenu;
        public bool MenuVisible => Screen != ReleaseScreen.Playing && Screen != ReleaseScreen.Ending;
        public bool MainMenuBackground => Screen == ReleaseScreen.MainMenu || Screen == ReleaseScreen.HowToPlay ||
            (Screen == ReleaseScreen.Settings && parent == ReleaseScreen.MainMenu);

        public ReleaseFlow(Func<GameRunState> createRun = null)
        {
            this.createRun = createRun ?? (() => new GameRunState(GameMode.PlaytestDemo));
            Run = FreshRun();
        }

        private GameRunState FreshRun()
        {
            var run = createRun();
            if (run == null || (run.Mode != GameMode.PlaytestDemo && run.Mode != GameMode.FinalPacing) || run.Phase != GameRunPhase.Intro)
                throw new ArgumentException("Release flow needs a fresh PlaytestDemo or FinalPacing run.");
            run.SetFocus(focused); run.SetApplicationPaused(nativePaused);
            return run;
        }

        public bool Command(ReleaseCommand command)
        {
            switch (command)
            {
                case ReleaseCommand.NewGame:
                    if (Screen != ReleaseScreen.MainMenu) return false;
                    return BeginFresh();
                case ReleaseCommand.Restart:
                    if (Screen != ReleaseScreen.Paused && Screen != ReleaseScreen.Ending) return false;
                    return BeginFresh();
                case ReleaseCommand.StartGameplay:
                    return Screen == ReleaseScreen.Playing && Run.Start();
                case ReleaseCommand.Pause:
                    if (Screen != ReleaseScreen.Playing || Run.Phase == GameRunPhase.Intro || Run.IsEnded) return false;
                    Screen = ReleaseScreen.Paused; Run.SetUserPaused(true); return true;
                case ReleaseCommand.Resume:
                    if (Screen != ReleaseScreen.Paused) return false;
                    Screen = ReleaseScreen.Playing; Run.SetUserPaused(false); return true;
                case ReleaseCommand.HowToPlay:
                    if (Screen != ReleaseScreen.MainMenu) return false;
                    parent = Screen; Screen = ReleaseScreen.HowToPlay; return true;
                case ReleaseCommand.Settings:
                    if (Screen != ReleaseScreen.MainMenu && Screen != ReleaseScreen.Paused) return false;
                    parent = Screen; Screen = ReleaseScreen.Settings; return true;
                case ReleaseCommand.Back:
                    if (Screen != ReleaseScreen.Settings && Screen != ReleaseScreen.HowToPlay) return false;
                    Screen = parent; return true;
                case ReleaseCommand.MainMenu:
                    if (Screen != ReleaseScreen.Paused && Screen != ReleaseScreen.Ending) return false;
                    Run = FreshRun(); Screen = ReleaseScreen.MainMenu; parent = Screen; return true;
                default: return false;
            }
        }

        private bool BeginFresh()
        {
            Run = FreshRun(); Screen = ReleaseScreen.Playing; parent = ReleaseScreen.MainMenu;
            // Retain Intro for the short existing orientation, then StartGameplay runs once.
            return true;
        }

        public EscapeResult Escape(bool cardsOpen)
        {
            if (Screen == ReleaseScreen.Playing && cardsOpen) return EscapeResult.CloseCards;
            if (Screen == ReleaseScreen.Settings || Screen == ReleaseScreen.HowToPlay)
                return Command(ReleaseCommand.Back) ? EscapeResult.MenuChanged : EscapeResult.None;
            if (Screen == ReleaseScreen.Paused)
                return Command(ReleaseCommand.Resume) ? EscapeResult.MenuChanged : EscapeResult.None;
            return Command(ReleaseCommand.Pause) ? EscapeResult.MenuChanged : EscapeResult.None;
        }

        public void SetFocus(bool value) { focused = value; Run.SetFocus(value); }
        public void SetNativePaused(bool value) { nativePaused = value; Run.SetApplicationPaused(value); }
        public void Tick(double seconds, bool cardsOpen, bool held = false, bool inRange = false)
        {
            Run.Tick(seconds, cardsOpen || Screen != ReleaseScreen.Playing, held, inRange);
            if (Screen == ReleaseScreen.Playing && Run.IsEnded) Screen = ReleaseScreen.Ending;
        }
    }
}
