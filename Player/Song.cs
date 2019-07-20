using System.Windows.Controls;

namespace Player
{
    public class Song
    {
        private string file = null;
        public string Title { get; private set; }
        public string Artist { get; private set; }
        public string Album { get; private set; }
        public string Year { get; private set; }
        public Image AlbumImage { get; private set; }

        public Song(string file)
        {
            this.file = file;
            SetSongInfo();
        }

        private void SetSongInfo()
        {

        }

        public void ChangeSongInfo(string title, string artist, string album, 
            string year, Image albumImg)
        {
            
        }

        public override string ToString()
        {
            return Artist + " - " + Title;
        }
    }
}