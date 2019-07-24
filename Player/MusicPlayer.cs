using WMPLib;

namespace Player
{
    class MusicPlayer
    {
        WindowsMediaPlayer player;

        public MusicPlayer()
        {
            player = new WindowsMediaPlayer();
            player.URL = null;
        }

        public void AddMediaEndedEventHandler(WMPLib._WMPOCXEvents_PlayStateChangeEventHandler handler) =>
            player.PlayStateChange += handler;

        public void Play()
        {
            if (!string.IsNullOrEmpty(player.URL.Trim()))
                player.controls.play();
        }
        public void Play(Song song)
        {
            if (song != null)
            {
                player.URL = song.PathToFile;
                Play();
            }
            else
            {
                player.URL = null;
                player.controls.stop();
            }
        }
        public void Stop() => player.controls.stop();
        public void Pause() => player.controls.pause();
        public bool hasURL() => !string.IsNullOrWhiteSpace(player.URL);

        public bool IsPlay() => player.playState == WMPPlayState.wmppsPlaying;

        public string GetDurationString() => player.currentMedia.durationString;
        public double GetDuration() => player.currentMedia.duration;
        public string GetCurrentPositionString() => player.controls.currentPositionString;
        public double GetCurrentPosition() => player.controls.currentPosition;
        public void SetCurrentPosition(double newValue) => player.controls.currentPosition = newValue;
        public void SetVolume(int newValue) => player.settings.volume = newValue;
    }
}