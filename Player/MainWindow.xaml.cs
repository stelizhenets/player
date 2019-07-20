using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Player
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool expanded = true;
        private bool isPlay = false;

        public MainWindow()
        {
            InitializeComponent();
            MakeEvents();
        }

        private void MakeEvents()
        {
            // appTop
            appTop.closeBtn.Click += Close_Click;
            appTop.playPauseBtn.Click += PlayPause_Click;
            appTop.expandBtn.Click += Extend_Click;

            // songInfo
            songInfo.editBtn.Click += EditBtn_Click;
            songInfo.getLyricsBtn.Click += GetLyricsBtn_Click;
        }

        private void GetLyricsBtn_Click(object sender, RoutedEventArgs e)
        {
            TabItem editTab = new TabItem();
            editTab.Header = "LYRICS";
            editTab.IsSelected = true;
            var tabContent = new LyricsControl();
            tabContent.closeTabBtn.Click += CloseTab_Click;
            tabContent.closeTabTopBtn.Click += CloseTab_Click;
            editTab.Content = tabContent;
            tabMenu.Items.Add(editTab);
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            Song song;
            if((song = songInfo.GetSong()) != null) {
                TabItem editTab = new TabItem();
                editTab.Header = "CHANGING SONG INFO";
                editTab.IsSelected = true;
                var tabContent = new SongInfoEditor(song);
                tabContent.cancelBtn.Click += CloseTab_Click;
                tabContent.saveBtn.Click += CloseTab_Click;
                editTab.Content = tabContent;
                tabMenu.Items.Add(editTab);
            }
        }

        private void CloseTab_Click(object sender, RoutedEventArgs e)
        {
            tabMenu.Items.Remove(tabMenu.SelectedValue);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void PlayPause_Click(object sender, RoutedEventArgs e)
        {
            if(isPlay)
            {
                (sender as Button).Content = FindResource("Play");
            } else
            {
                (sender as Button).Content = FindResource("Pause");
            }
            isPlay = !isPlay;
        }

        private double lastHeight = 600;
        private void Extend_Click(object sender, RoutedEventArgs e)
        {
            if (expanded)
            {
                lastHeight = mainWindow.Height;
                mainWindow.Height = mainWindow.MinHeight;
                (sender as Button).Content = FindResource("ExpandMore");
            }
            else
            {
                mainWindow.Height = lastHeight;
                (sender as Button).Content = FindResource("ExpandLess");
            }
            expanded = !expanded;
        }
    }
}
