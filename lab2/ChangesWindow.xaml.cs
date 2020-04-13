using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace lab2
{
    /// <summary>
    /// Interaction logic for ChangesWindow.xaml
    /// </summary>
    public partial class ChangesWindow : Window
    {
        public List<string> Changes { get; set; }

        protected override void OnContentRendered(EventArgs e)
        {
            ChangesList.ItemsSource = Changes; 
            base.OnContentRendered(e);
        }

        public ChangesWindow()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); 
        }
    }
}
