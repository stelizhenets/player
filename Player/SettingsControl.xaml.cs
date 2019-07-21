using System;
using System.Windows;
using System.Windows.Controls;
using WinForms = System.Windows.Forms;

namespace Player
{
    /// <summary>
    /// Interaction logic for SettingsControl.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {
        WinForms.FolderBrowserDialog folderBrowserDialog;
        public SettingsControl()
        {
            InitializeComponent();
            folderBrowserDialog = new WinForms.FolderBrowserDialog();
            username.Text = ConfigSettings.ReadSetting("username");
            string[] library = ConfigSettings.ReadSetting("musicLibrary").Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < library.Length; i++)
                musicLibraryFolders.Items.Add(library[i]);
        }

        private void AddFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            if(folderBrowserDialog.ShowDialog() == WinForms.DialogResult.OK)
            {
                if (!musicLibraryFolders.Items.Contains(folderBrowserDialog.SelectedPath))
                    musicLibraryFolders.Items.Add(folderBrowserDialog.SelectedPath);
            }
        }

        private void RemoveFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            if (musicLibraryFolders.SelectedItem != null)
                musicLibraryFolders.Items.Remove(musicLibraryFolders.SelectedItem);
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrEmpty(username.Text))
            {
                ConfigSettings.WriteSetting("username", username.Text);

                string library = string.Empty;
                foreach(string dir in musicLibraryFolders.Items)
                {
                    library += dir + ";";
                }
                ConfigSettings.WriteSetting("musicLibrary", library);
            }
        }
    }
}
