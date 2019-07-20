using Microsoft.Win32;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Windows;
using System.Windows.Controls;

namespace Player
{
    /// <summary>
    /// Interaction logic for LyricsControl.xaml
    /// </summary>
    public partial class LyricsControl : UserControl
    {
        private string artist;
        private string title;

        public LyricsControl(Song song)
        {
            InitializeComponent();
            this.artist = song.Artist;
            this.title = song.Title;
            GetLyrics();
        }

        public LyricsControl()
        {
            InitializeComponent();
            artist = "The White Stripes";
            title = "Fell In Love With A Girl";
            GetLyrics();
        }

        private void GetLyrics()
        {
            tabTitle.Text = artist + " - " + title;
            try
            {
                new Thread(new ThreadStart(LyricsLoader)).Start();
            } catch(Exception)
            {
                lyrics.Text = "Unexpected error.";
            }
        }

        private void LyricsLoader()
        {
            try
            {
                string keyWords = artist + " - " + title + " lyrics";
                string req = HttpUtility.UrlEncode(keyWords.Trim());
                StringBuilder sb = new StringBuilder();
                byte[] ResultsBuffer = new byte[8192];
                string SearchResults = "http://google.com/search?q=" + req + "&ie=UTF-8&oe=UTF-8";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(SearchResults);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                Stream resStream = response.GetResponseStream();
                string tempString = null;
                int count = 0;
                do
                {
                    count = resStream.Read(ResultsBuffer, 0, ResultsBuffer.Length);
                    if (count != 0)
                    {
                        tempString = Encoding.UTF8.GetString(ResultsBuffer, 0, count);
                        sb.Append(tempString);
                    }
                }

                while (count > 0);
                string sbb = sb.ToString();

                HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();
                html.OptionOutputAsXml = true;
                html.LoadHtml(sbb);

                var nodes = html.DocumentNode.SelectNodes("//div");
                string hnya = nodes[7].InnerHtml;
                html.LoadHtml(hnya);
                nodes = html.DocumentNode.SelectNodes("//div[@class='BNeawe tAd8D AP7Wnd']");
                hnya = String.Empty;
                int i = 0;

                foreach (var v in nodes)
                {
                    if (++i < 3) continue;
                    hnya += v.InnerText;
                    if (i > nodes.Count / 2) break;
                    else hnya += "\n";
                }

                if (String.IsNullOrEmpty(hnya)) hnya = "Error while loading lyrics.";
                lyrics.Dispatcher.Invoke(() =>
                {
                    lyrics.Text = hnya;
                    copyright.Visibility = Visibility.Visible;
                });
            } catch(Exception)
            {
                lyrics.Dispatcher.Invoke(() =>
                {
                    lyrics.Text = "Unexpected error";
                });
            }
        }

        private void ZoomOutBtn_Click(object sender, RoutedEventArgs e)
        {
            if(lyrics.FontSize > 12)
            {
                lyrics.FontSize -= 2;
                tabTitle.FontSize -= 1;
            }
        }

        private void ZoomInBtn_Click(object sender, RoutedEventArgs e)
        {
            if (lyrics.FontSize < 34)
            {
                tabTitle.FontSize += 1;
                lyrics.FontSize += 2;
            }
        }

        private void CopyBtn_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(lyrics.Text);
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text file (*.txt)|*.txt";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveFileDialog.Title = "Save lyrics to...";
            saveFileDialog.FileName = artist + " - " + title + " lyrics";
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, lyrics.Text);
            }
        }
    }
}
