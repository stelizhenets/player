using System;
using System.Collections.Generic;
using System.IO;

namespace Player
{
    public enum PlayMode
    {
        RepeatOff,
        RepeatOne,
        RepeatAll
    }

    public class Playlist
    {
        public string Name { get; set; }
        public List<Song> Items { get; private set; } = new List<Song>();
        int currentIndex = 0;
        PlayMode playMode = PlayMode.RepeatOff;

        public Playlist(string name)
        {
            Name = name;
        }

        public Playlist(string name, List<Song> songs) : this(name)
        {
            Items = songs;
        }

        public Playlist(string name, Song[] songs) : this(name)
        {
            AddAllSongs(songs);
        }

        public Playlist(string name, string[] pathsToSongs) : this(name)
        {
            foreach(string pathToSong in pathsToSongs)
            {
                AddSong(Song.GetSong(pathToSong));
            }
        }

        public void SetPlayMode(PlayMode playMode)
        {
            this.playMode = playMode;
        }

        public bool AddAllSongs(List<Song> songs)
        {
            bool allAdded = true;
            foreach(Song song in songs)
            {
                allAdded = AddSong(song);
            }

            return allAdded;
        }

        public bool AddAllSongs(Song[] songs)
        {
            bool allAdded = true;
            foreach (Song song in songs)
            {
                allAdded = AddSong(song);
            }

            return allAdded;
        }

        public bool AddSong(Song song)
        {
            if(!Items.Contains(song))
            {
                Items.Add(song);
                return true;
            }

            return false;
        }

        public void RemoveSong(int songIndex)
        {
            if (songIndex < currentIndex) currentIndex--;
            Items.RemoveAt(songIndex);
        }

        public Song GetCurrentSong() => currentIndex < Items.Count ? Items[currentIndex] : null;
        public int GetCurrentIndex() => currentIndex;
        public void SetCurrentIndex(int newIndex)
        {
            if (newIndex > -1 && newIndex < Items.Count)
                currentIndex = newIndex;
        }

        public Song AutoNext()
        {
            if (playMode == PlayMode.RepeatOff)
                return Next();

            if (playMode == PlayMode.RepeatOne)
                return Items[currentIndex];

            if (playMode == PlayMode.RepeatAll && currentIndex != Items.Count - 1)
                return Next();
            else return GetSongByIndex(0);
        }

        public Song GetSongByIndex(int index)
        {
            if (index > -1 && index < Items.Count)
            {
                currentIndex = index;
                return Items[currentIndex];
            }
            else return null;
        }

        public int? GetIndexBySong(Song song)
        {
            int? result = null;

            for (int i = 0; i < Items.Count; i++)
                if (song == Items[i]) return i;

            return result;
        }

        public Song Next()
        {
            if (currentIndex < Items.Count - 1)
            {
                return Items[++currentIndex];
            }
            else return null;
        }

        public Song Previous()
        {
            if (currentIndex > 0)
            {
                return Items[--currentIndex];
            }
            else return null;
        }

        public List<Song> Shuffle()
        {
            Shuffle(Items);
            currentIndex = 0;
            return Items;
        }

        public void Replace(int fromIndex, int toIndex)
        {
            var itemToReplace = Items[fromIndex];
            Items.RemoveAt(fromIndex);
            Items.Insert(toIndex, itemToReplace);

            if (fromIndex == currentIndex)
                currentIndex = toIndex;
            else if(fromIndex < currentIndex && toIndex >= currentIndex)
            {
                currentIndex--;
            } else if(fromIndex > currentIndex && toIndex <= currentIndex)
            {
                currentIndex++;
            }
        }

        Random rand = new Random();
        void Shuffle(List<Song> list)
        {
            // replacing current song to the first place
            Replace(currentIndex, 0);

            int n = list.Count;
            while (n > 1)
            {
                int k = rand.Next(n);
                if (k == 0) continue;
                n--;
                Song song = list[k];
                list[k] = list[n];
                list[n] = song;
            }
        }

        public void Save(string playlistName, string pathToFile)
        {
            string textToWrite = playlistName;
            foreach (Song song in Items)
                textToWrite += "\n" + song.PathToFile;
            File.WriteAllText(pathToFile, textToWrite);
            Name = playlistName;
        }

        public static Playlist GetMyMusicList()
        {
            List<Song> songs = new List<Song>();
            string[] libraryPaths = ConfigSettings.ReadSetting("musicLibrary").Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            string[] songsInFolder;
            for(int i = 0; i < libraryPaths.Length; i++)
            {
                songsInFolder = Directory.GetFiles(libraryPaths[i], "*.mp3");
                for (int j = 0; j < songsInFolder.Length; j++)
                    songs.Add(Song.GetSong(songsInFolder[j]));
            }

            return new Playlist("My music", songs);
        }
    }
}