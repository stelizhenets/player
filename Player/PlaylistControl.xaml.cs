using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using WinForms = System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;

namespace Player
{
    /// <summary>
    /// Interaction logic for PlaylistControl.xaml
    /// </summary>
    public partial class PlaylistControl : UserControl
    {
        OpenFileDialog openFileDialog;
        SaveFileDialog saveFileDialog;
        Playlist currentPlaylist;
        MusicPlayer player;
        AppTop appTop;
        WinForms.Timer timer;
        System.Windows.Threading.DispatcherTimer dispatcherTimer;

        public PlaylistControl()
        {
            InitializeComponent();
            
            openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "MP3 File (*.mp3)|*.mp3";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            openFileDialog.Title = "Add songs to playlist";
            openFileDialog.Multiselect = true;

            saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Playlist (*.playlist)|*.playlist";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            saveFileDialog.Title = "Save playlist as...";
            saveFileDialog.FileName = "My playlist";

            player = new MusicPlayer();
            player.AddMediaEndedEventHandler(Player_PlayStateChange);

            timer = new WinForms.Timer();
            timer.Interval = 300;
            timer.Tick += UpdatePlayProgress;
            timer.Enabled = false;

            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
        }

        public void SetAppTop(AppTop appTop)
        {
            this.appTop = appTop;
            appTop.playPauseBtn.Click += PlayPauseBtn_Click;
            appTop.nextBtn.Click += NextBtn_Click;
            appTop.previousBtn.Click += PreviousBtn_Click;
            appTop.progress.MouseDown += Progress_MouseDown;
            appTop.volumeSlider.ValueChanged += VolumeSlider_ValueChanged;
        }

        public void SetPlaylist(Playlist newPlaylist, bool autoPlay = false)
        {
            currentPlaylist = newPlaylist;
            playlist.ItemsSource = currentPlaylist.Items;
            RefreshPlaylist();
            if (autoPlay) Play(currentPlaylist.GetCurrentSong());
        }

        private void RefreshPlaylist()
        {
            playlist.Items.Refresh();
            playlist.SelectedIndex = currentPlaylist.GetCurrentIndex();
            playlistName.Text = currentPlaylist.Name;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            if(openFileDialog.ShowDialog() == true)
            {
                foreach(string pathToSong in openFileDialog.FileNames)
                {
                    currentPlaylist.AddSong(Song.GetSong(pathToSong));
                }
                RefreshPlaylist();
            }
        }

        private void RemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (playlist.SelectedItem != null)
            {
                currentPlaylist.RemoveSong(playlist.SelectedIndex);
                RefreshPlaylist();
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if(currentPlaylist.Items.Count > 0)
            {
                if (saveFileDialog.ShowDialog() == true)
                {
                    string playlistName = saveFileDialog.SafeFileName.Substring(0, saveFileDialog.SafeFileName.LastIndexOf("."));
                    currentPlaylist.Save(playlistName, saveFileDialog.FileName);
                    RefreshPlaylist();
                }
            }
        }

        private void ShuffleBtn_Click(object sender, RoutedEventArgs e)
        {
            if (currentPlaylist.Items.Count > 0)
            {
                currentPlaylist.Shuffle();
                RefreshPlaylist();
            }
        }

        PlayMode playMode = PlayMode.RepeatOff;
        private void PlayModeBtn_Click(object sender, RoutedEventArgs e)
        {
            if(playMode == PlayMode.RepeatOff)
            {
                currentPlaylist.SetPlayMode(playMode = PlayMode.RepeatOne);
                playModeBtn.Content = FindResource("RepeatOne");
            } else if(playMode == PlayMode.RepeatOne)
            {
                currentPlaylist.SetPlayMode(playMode = PlayMode.RepeatAll);
                playModeBtn.Content = FindResource("RepeatAll");
            } else
            {
                currentPlaylist.SetPlayMode(playMode = PlayMode.RepeatOff);
                playModeBtn.Content = FindResource("RepeatOff");
            }
        }

        // Helper to search up the VisualTree
        private static T FindAnchestor<T>(DependencyObject current)
            where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }


        private Point startPoint = new Point();
        private int startIndex = -1;
        private void Playlist_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Get current mouse position
            startPoint = e.GetPosition(null);
        }

        private void Playlist_MouseMove(object sender, MouseEventArgs e)
        {
            // Get the current mouse position
            Point mousePos = e.GetPosition(null);
            Vector diff = startPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                       Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                // Get the dragged ListViewItem
                ListView listView = sender as ListView;
                ListViewItem listViewItem = FindAnchestor<ListViewItem>((DependencyObject)e.OriginalSource);
                if (listViewItem == null) return;           // Abort
                                                            // Find the data behind the ListViewItem
                Song item = (Song)listView.ItemContainerGenerator.ItemFromContainer(listViewItem);
                if (item == null) return;                   // Abort
                                                            // Initialize the drag & drop operation
                startIndex = playlist.SelectedIndex;
                DataObject dragData = new DataObject("Song", item);
                DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Copy | DragDropEffects.Move);
            }
        }

        private void Playlist_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("Song") || sender != e.Source)
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void Playlist_Drop(object sender, DragEventArgs e)
        {
            int index = -1;
            if (e.Data.GetDataPresent("Song") && sender == e.Source)
            {
                // Get the drop ListViewItem destination
                ListView listView = sender as ListView;
                ListViewItem listViewItem = FindAnchestor<ListViewItem>((DependencyObject)e.OriginalSource);
                if (listViewItem == null)
                {
                    // Abort
                    e.Effects = DragDropEffects.None;
                    return;
                }
                // Find the data behind the ListViewItem
                Song item = (Song)listView.ItemContainerGenerator.ItemFromContainer(listViewItem);
                // Move item into observable collection 
                // (this will be automatically reflected to lstView.ItemsSource)
                e.Effects = DragDropEffects.Move;
                index = playlist.Items.IndexOf(item);
                if (startIndex >= 0 && index >= 0)
                {
                    currentPlaylist.Replace(startIndex, index);
                    RefreshPlaylist();
                }
                startIndex = -1;        // Done!
            }
        }

        private void Player_PlayStateChange(int NewState)
        {
            if (NewState == 8)
            {
                Play(currentPlaylist.AutoNext());
                
                dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
                dispatcherTimer.Start();
            }
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            player.Play();
            dispatcherTimer.Stop();
        }

        private void Play(Song song)
        {
            player.Play(song);
            if (song != null)
            {
                PreparePlay(song);
            }
            else
            {
                SetPlayPauseButtonIcon("Play");
                timer.Enabled = false;
            }
        }

        private void Play()
        {
            player.Play();
            PreparePlay(currentPlaylist.GetCurrentSong());
        }

        private void PreparePlay(Song song)
        {
            appTop.songInfo.Text = song.Artist + " - " + song.Title;
            SetPlayPauseButtonIcon("Pause");
            RefreshPlaylist();
            string durationString = player.GetDurationString();
            appTop.duration.Text = "0:00 / " + durationString;
            timer.Enabled = true;
        }

        private void UpdatePlayProgress(object sender, EventArgs e)
        {
            appTop.progress.Maximum = player.GetDuration();
            appTop.progress.Value = Convert.ToInt32(player.GetCurrentPosition());
            appTop.duration.Text = player.GetCurrentPositionString() + " / " + player.GetDurationString();
        }

        private void Progress_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                double MousePosition = e.GetPosition(appTop.progress).X;
                player.SetCurrentPosition(appTop.progress.Value = SetProgressBarValue(MousePosition));
            }
        }

        private double SetProgressBarValue(double MousePosition)
        {
            double ratio = MousePosition / appTop.progress.ActualWidth;
            double ProgressBarValue = ratio * appTop.progress.Maximum;
            return ProgressBarValue;
        }

        private void PreviousBtn_Click(object sender, RoutedEventArgs e)
        {
            if(currentPlaylist.GetCurrentIndex() > 0)
                Play(currentPlaylist.Previous());
        }

        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            if(currentPlaylist.GetCurrentIndex() < currentPlaylist.Items.Count-1)
                Play(currentPlaylist.Next());
        }

        private void PlayPauseBtn_Click(object sender, RoutedEventArgs e)
        {
            if(player.IsPlay())
            {
                player.Pause();
                SetPlayPauseButtonIcon("Play");
            } else
            {
                if(player.hasURL())
                {
                    Play();
                    SetPlayPauseButtonIcon("Pause");
                } else
                {
                    Play(currentPlaylist.GetSongByIndex(0));
                }
            }
        }

        private void SetPlayPauseButtonIcon(string iconKeyName)
        {
            appTop.playPauseBtn.Content = FindResource(iconKeyName);
        }

        private void Playlist_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int currentIndex = currentPlaylist.GetCurrentIndex();
            if (playlist.SelectedIndex != currentIndex)
            {
                currentPlaylist.SetCurrentIndex(playlist.SelectedIndex);
                Play(currentPlaylist.GetCurrentSong());
            }
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            player.SetVolume((int)e.NewValue);
        }
    }
}