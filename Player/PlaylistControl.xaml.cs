using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
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
        }

        public void SetPlaylist(Playlist newPlaylist)
        {
            currentPlaylist = newPlaylist;
            playlist.ItemsSource = currentPlaylist.Items;
            RefreshPlaylist();
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
            currentPlaylist.Shuffle();
            RefreshPlaylist();
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
    }
}
