using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Controls;

namespace Player
{
    public class Song
    {
        public string PathToFile { get; private set; }
        public string Title { get; private set; }
        public string Artist { get; private set; }
        public string Album { get; private set; }
        public string Year { get; private set; }
        public string Comment { get; private set; }
        public System.Drawing.Image AlbumImage { get; private set; }

        private Song(string path, string title, string artist, string album, string year, string comment, System.Drawing.Image albumImg)
        {
            PathToFile = path;
            Title = title;
            Artist = artist;
            Album = album;
            AlbumImage = albumImg;
            Comment = comment;
        }

        public Song GetSong(string path)
        {
            return new SongInfoGetter().GetSong(path);
        }

        public void ChangeSongInfo(string title, string artist, string album, 
            string year, System.Drawing.Image albumImg)
        {
            
        }

        public override string ToString()
        {
            return Artist + " - " + Title;
        }

        class SongInfoGetter
        {
            string path;
            string artist = "Unknown Artist";
            string title = "Unknown Title";
            string album = "Unknown Album";
            string year = "Unknown Year";
            string comment = string.Empty;
            System.Drawing.Image albumImage;
            private System.Drawing.Image defaultAlbumArt;

            public Song GetSong(string path)
            {
                if (File.Exists(path))
                {
                    try
                    {
                        this.path = path;
                        FileStream fs;
                        fs = new FileStream(path, FileMode.Open);

                        // read the tag
                        byte[] buffer = new byte[128];
                        fs.Seek(-128, SeekOrigin.End);
                        fs.Read(buffer, 0, 128);
                        fs.Close();

                        // convert the tag buffer into a string
                        UTF8Encoding encoder = new UTF8Encoding();
                        string tag = encoder.GetString(buffer);

                        // if there is a ID3 v1 tag, then read it (we can know by looking at the first 3 bytes for the word TAG)
                        if (tag.Substring(0, 3) == "TAG")
                        {
                            title = RemoveWhiteSpace(tag.Substring(3, 30));
                            artist = RemoveWhiteSpace(tag.Substring(33, 30));
                            album = RemoveWhiteSpace(tag.Substring(63, 30));
                            year = RemoveWhiteSpace(tag.Substring(93, 4));
                            comment = RemoveWhiteSpace(tag.Substring(97, 28));

                            int track;
                            if (tag[125] == 0)
                                track = (int)buffer[126];
                            else
                                track = 0;
                            System.Console.WriteLine("Track: " + track);
                        }

                        // look for album art
                        string[] artPaths = Directory.GetFiles(Path.GetDirectoryName(path), "AlbumArt_*Large.jpg");

                        if (artPaths.Length == 0) artPaths = Directory.GetFiles(Path.GetDirectoryName(path), "AlbumArt*.jpg");
                        if (artPaths.Length == 0) artPaths = Directory.GetFiles(Path.GetDirectoryName(path), "Album*.jpg");
                        if (artPaths.Length == 0) artPaths = Directory.GetFiles(Path.GetDirectoryName(path), "*.jpg");

                        if (artPaths.Length > 0)
                        {
                            albumImage = Bitmap.FromFile(artPaths[0]);
                        }
                        else
                        {
                            albumImage = defaultAlbumArt;
                        }
                    }
                    catch { }
                    return new Song(path, title, artist, album, year, comment, albumImage);
                }
                else return null;
            }

            private string RemoveWhiteSpace(string s)
            {
                string newstring = "";

                foreach (char c in s)
                {
                    if (char.IsLetterOrDigit(c) || char.IsPunctuation(c) || char.IsSeparator(c))
                    {
                        newstring += c;
                    }
                }

                return newstring.Trim();
            }
        }
    }
}