using System.Windows.Controls;
using System.Windows.Media;

namespace Player
{
    /// <summary>
    /// Interaction logic for SongInfo.xaml
    /// </summary>
    public partial class SongInfo : UserControl
    {
        private Song song;

        public SongInfo()
        {
            InitializeComponent();
        }

        public SongInfo(Song song)
        {
            InitializeComponent();
            SetSongInfo(song);
        }

        public SongInfo(string title, string artist, string album, string year, Image albumImg) 
            => SetSongInfo(title, artist, album, year, albumImg);

        public void SetSongInfo(string title, string artist, string album, 
            string year, Image albumImg)
        {
            SetTitle(title);
            SetArtist(artist);
            SetAlbum(album);
            SetYear(year);
            SetAlbumImage(albumImg);
        }

        public void SetSongInfo(Song song)
        {
            this.song = song;
            SetTitle(song.Title);
            SetArtist(song.Artist);
            SetAlbum(song.Album);
            SetYear(song.Year);
            SetAlbumImage(song.AlbumImage);
        }

        private void SetTitle(string title)
        {
            this.title.Text = title;
        }

        private void SetArtist(string artist)
        {
            this.artist.Text = artist;
        }

        private void SetAlbum(string album)
        {
            this.album.Text = album;
        }

        private void SetYear(string year)
        {
            this.year.Text = year;
        }

        private void SetAlbumImage(Image image)
        {
            if (image != null)
            {
                var imgBrush = new ImageBrush();
                imgBrush.ImageSource = image.Source;
                albumImage.Background = imgBrush;
            }
        }

        public Song GetSong()
        {
            return this.song;
        }
    }
}
