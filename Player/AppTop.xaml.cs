using System.Windows.Controls;

namespace Player
{
    /// <summary>
    /// Interaction logic for AppTop.xaml
    /// </summary>
    public partial class AppTop : UserControl
    {
        public AppTop()
        {
            InitializeComponent();
        }

        private void Grid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            volumeSlider.Visibility = System.Windows.Visibility.Visible;
            volumeBtn.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void Grid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            volumeSlider.Visibility = System.Windows.Visibility.Collapsed;
            volumeBtn.Visibility = System.Windows.Visibility.Visible;
        }
    }
}
