using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Player
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool expanded = true;
        private bool isPlay = false;
        private const int MIN_WIDTH = 500;
        private const int MIN_HEIGHT = 620;
        private Playlist playlist;

        public MainWindow(string[] args)
        {
            InitializeComponent();
            ConnectEventHandlers();

            if (args.Length > 0)
                playlist = new Playlist("Custom", args);
            else playlist = Playlist.GetMyMusicList();
            SetPlaylist();

            mainWindow.MinWidth = MIN_WIDTH;
            mainWindow.MinHeight = MIN_HEIGHT;
        }

        private void SetPlaylist()
        {
            if(playlist != null)
            {
                currentPlaylistControl.SetPlaylist(playlist);
            }
        }

        private void ConnectEventHandlers()
        {
            // appTop
            appTop.closeBtn.Click += Close_Click;
            appTop.playPauseBtn.Click += PlayPause_Click;
            appTop.expandBtn.Click += Expand_Click;
            appTop.settingsBtn.Click += SettingsBtn_Click;

            // songInfo
            songInfo.editBtn.Click += EditBtn_Click;
            songInfo.getLyricsBtn.Click += GetLyricsBtn_Click;

            // playlist
            currentPlaylistControl.playlist.SelectionChanged += Playlist_SelectionChanged;
        }

        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!expanded)
                Expand_Click(appTop.expandBtn, null);

            if (!IsTabOpen("SETTINGS"))
            {
                TabItem settingsTab = new TabItem();
                settingsTab.Header = "SETTINGS";
                settingsTab.IsSelected = true;
                var tabContent = new SettingsControl();
                tabContent.cancelBtn.Click += CloseTab_Click;
                tabContent.saveBtn.Click += CloseTab_Click;
                settingsTab.Content = tabContent;
                tabMenu.Items.Add(settingsTab);
            }
        }

        private bool IsTabOpen(string tabHeader)
        {
            foreach(TabItem tab in tabMenu.Items)
            {
                if (tab.Header == tabHeader) return true;
            }
            return false;
        }

        private void Playlist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Song current = currentPlaylistControl.playlist.SelectedItem as Song;
            if(current != null) songInfo.ShowSongInfo(current);
        }

        private void GetLyricsBtn_Click(object sender, RoutedEventArgs e)
        {
            Song song;
            if ((song = songInfo.GetSong()) != null)
            {
                TabItem editTab = new TabItem();
                editTab.Header = song.Title.ToUpper() + " LYRICS";
                editTab.IsSelected = true;
                var tabContent = new LyricsControl(song);
                tabContent.closeTabBtn.Click += CloseTab_Click;
                tabContent.closeTabTopBtn.Click += CloseTab_Click;
                editTab.Content = tabContent;
                tabMenu.Items.Add(editTab);
            }
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            Song song;
            if((song = songInfo.GetSong()) != null) {
                TabItem editTab = new TabItem();
                editTab.Header = "CHANGING SONG INFO";
                editTab.IsSelected = true;
                var tabContent = new SongInfoEditor(song);
                tabContent.cancelBtn.Click += CloseTab_Click;
                tabContent.saveBtn.Click += SaveSongInfo_Click;
                editTab.Content = tabContent;
                tabMenu.Items.Add(editTab);
            }
        }

        private void CloseTab_Click(object sender, RoutedEventArgs e)
        {
            tabMenu.Items.Remove(tabMenu.SelectedValue);
        }

        private void SaveSongInfo_Click(object sender, RoutedEventArgs e)
        {
            songInfo.ShowSongInfo(songInfo.GetSong());
            currentPlaylistControl.playlist.Items.Refresh();
            CloseTab_Click(null, null);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mainWindow.WindowState = WindowState.Normal;
            DragMove();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void PlayPause_Click(object sender, RoutedEventArgs e)
        {
            if(isPlay)
            {
                (sender as Button).Content = FindResource("Play");
            } else
            {
                (sender as Button).Content = FindResource("Pause");
            }
            isPlay = !isPlay;
        }

        private double lastHeight = MIN_HEIGHT;
        private void Expand_Click(object sender, RoutedEventArgs e)
        {
            var beginState = mainWindow.WindowState;
            mainWindow.WindowState = WindowState.Normal;
            if (expanded)
            {
                lastHeight = mainWindow.Height;
                mainWindow.MinHeight = mainWindow.MaxHeight = mainWindow.Height = 60;
                (sender as Button).Content = FindResource("ExpandMore");
            }
            else
            {
                mainWindow.MinHeight = MIN_HEIGHT;
                mainWindow.MinWidth = MIN_WIDTH;
                mainWindow.MaxHeight = int.MaxValue;
                mainWindow.MaxWidth = int.MaxValue;

                mainWindow.Height = lastHeight;
                (sender as Button).Content = FindResource("ExpandLess");
            }
            expanded = !expanded;
            mainWindow.WindowState = beginState;
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            lastHeight = e.NewSize.Height;
            if (e.NewSize.Width - MIN_WIDTH < 200)
                songInfo.Visibility = Visibility.Collapsed;
            else songInfo.Visibility = Visibility.Visible;
        }
    }
}
