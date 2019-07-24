using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Player
{
    public class Song
    {
        public string PathToFile { get; private set; }
        public string Title { get; private set; }
        public string Artist { get; private set; }
        public string Album { get; private set; }
        public uint Year { get; private set; }
        public string Comment { get; private set; }
        public Image AlbumImage { get; private set; }

        private Song(string path, string title, string artist, string album, uint year, string comment, Image albumImg)
        {   
            PathToFile = path;
            Title = title;
            Artist = artist;
            Album = album;
            AlbumImage = albumImg;
            Year = year;
            Comment = comment;
        }

        public static Song GetSong(string path)
        {
            return new SongInfoGetter().GetSong(path);
        }

        public void ChangeSongInfo(string title, string artist, string album,
            string year, Image albumImg)
        {
            TagLib.File file = TagLib.File.Create(PathToFile);
            Artist = artist;
            file.Tag.Artists = new string[] { artist };
            file.Tag.Performers = new string[] { artist };
            Title = file.Tag.Title = title;
            Album = file.Tag.Album = album;
            Year = file.Tag.Year = uint.Parse(year);
            file.Save();
        }

        public override string ToString()
        {
            return Artist + " - " + Title;
        }

        class SongInfoGetter
        {
            string path;
            string artist;
            string title;
            string album;
            uint year;
            string comment = string.Empty;
            Image albumImage = new Image();

            public Song GetSong(string path)
            {
                if (File.Exists(path))
                {
                    try
                    {
                        this.path = path;

                        TagLib.File file = TagLib.File.Create(path);
                        artist = String.IsNullOrWhiteSpace(file.Tag.Performers[0]) ? String.IsNullOrWhiteSpace(file.Tag.Artists[0]) ? "Unknown Artist" : file.Tag.Artists[0] : file.Tag.Performers[0];
                        title = String.IsNullOrWhiteSpace(file.Tag.Title) ? "Unknown title" : file.Tag.Title;
                        album = String.IsNullOrWhiteSpace(file.Tag.Album) ? "Unknown album" : file.Tag.Album;
                        year = file.Tag.Year;
                        comment = file.Tag.Comment;

                        // look for album art
                        string[] artPaths = Directory.GetFiles(Path.GetDirectoryName(path), "AlbumArt_*Large.jpg");

                        if (artPaths.Length == 0) artPaths = Directory.GetFiles(Path.GetDirectoryName(path), "AlbumArt*.jpg");
                        if (artPaths.Length == 0) artPaths = Directory.GetFiles(Path.GetDirectoryName(path), "Album*.jpg");
                        if (artPaths.Length == 0) artPaths = Directory.GetFiles(Path.GetDirectoryName(path), "*.jpg");

                        if (artPaths.Length > 0)
                        {
                            albumImage.Source = new BitmapImage(new System.Uri(artPaths[0]));
                        }
                        else
                        {
                            albumImage.Source = null;
                        }
                    }
                    catch(Exception ex) { System.Console.WriteLine(ex); }
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