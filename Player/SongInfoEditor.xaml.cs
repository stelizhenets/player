using System.Windows.Controls;

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
            year.Text = song.Year; 
        }

        private void SaveBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if(!string.IsNullOrEmpty(artist.Text) && !string.IsNullOrEmpty(title.Text) &&
                !string.IsNullOrEmpty(album.Text) && !string.IsNullOrEmpty(year.Text))
            {
                song.ChangeSongInfo(title.Text, artist.Text, album.Text, year.Text, song.AlbumImage);
            } else
            {
                
            }
        }
    }
}
