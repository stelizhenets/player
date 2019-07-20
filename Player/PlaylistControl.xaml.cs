using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Player
{
    /// <summary>
    /// Interaction logic for PlaylistControl.xaml
    /// </summary>
    public partial class PlaylistControl : UserControl
    {
        OpenFileDialog openFileDialog;
        SaveFileDialog saveFileDialog;

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

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            if(openFileDialog.ShowDialog() == true)
            {
                foreach(string pathToSong in openFileDialog.FileNames)
                {
                    playlist.Items.Add(Song.GetSong(pathToSong));
                }
            }
        }

        private void RemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (playlist.SelectedItem != null)
                playlist.Items.Remove(playlist.SelectedItem);
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if(playlist.Items.Count > 0)
            {
                if (saveFileDialog.ShowDialog() == true)
                {
                    string listOfSongs = saveFileDialog.SafeFileName.Substring(0, saveFileDialog.SafeFileName.LastIndexOf("."));
                    foreach (Song song in playlist.Items)
                    {
                        listOfSongs += Environment.NewLine + song.PathToFile;
                    }
                    File.WriteAllText(saveFileDialog.FileName, listOfSongs);
                }
            }
        }
    }
}
