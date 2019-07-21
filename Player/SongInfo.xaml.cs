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
            ShowSongInfo(song);
        }

        public SongInfo(string title, string artist, string album, uint year, Image albumImg) 
            => ShowSongInfo(title, artist, album, year, albumImg);

        public void ShowSongInfo(string title, string artist, string album, 
            uint year, Image albumImg)
        {
            SetTitle(title);
            SetArtist(artist);
            SetAlbum(album);
            SetYear(year);
            SetAlbumImage(albumImg);
        }

        public void ShowSongInfo(Song song)
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

        private void SetYear(uint year)
        {
            this.year.Text = year.ToString();
        }

        private void SetAlbumImage(Image image)
        {
            if (image == null || image.Source == null)
            {
                image = new Image();
                image.Source = (ImageSource)FindResource("AlbumDefaultImage");
            }

            var imgBrush = new ImageBrush();
            imgBrush.ImageSource = image.Source;
            albumImage.Background = imgBrush;
        }

        public Song GetSong()
        {
            return this.song;
        }
    }
}
