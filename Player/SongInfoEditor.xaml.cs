using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Player
{
    /// <summary>
    /// Interaction logic for SongInfoEditor.xaml
    /// </summary>
    public partial class SongInfoEditor : UserControl
    {
        private Song song;
        public SongInfoEditor(Song song)
        {
            InitializeComponent();

            this.song = song;
            artist.Text = song.Artist;
            title.Text = song.Title;
            album.Text = song.Album;
            year.Text = song.Year.ToString();

            Image image = song.AlbumImage;
            if (image == null || image.Source == null)
            {
                image = new Image();
                image.Source = (ImageSource)FindResource("AlbumDefaultImage");
            }

            var imgBrush = new ImageBrush();
            imgBrush.ImageSource = image.Source;
            albumArt.Background = imgBrush;
        }

        private void SaveBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if(!string.IsNullOrEmpty(artist.Text) && !string.IsNullOrEmpty(title.Text) &&
                !string.IsNullOrEmpty(album.Text) && !string.IsNullOrEmpty(year.Text))
            {
                try
                {
                    song.ChangeSongInfo(title.Text, artist.Text, album.Text, year.Text, song.AlbumImage);
                } catch(IOException)
                {
                    MessageBox.Show("Unable to change the song information.\nPossible reasons:\n" +
                        "1.The song is currently playing or opened in another program.\n" +
                        "2.This file no longer exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            } else
            {
                MessageBox.Show("Unable to change the song information.\n" +
                    "All fields must be filled in before proceeding.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
