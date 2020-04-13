using ExcelDataReader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ExcelDataReader;
using Microsoft.Win32;
using System.Threading;
//System.Windows.Forms;

namespace lab2
{

    //!!! надо запускать Release версию 
    //Путь можно поменять в LocalDataBaseController
    public partial class MainWindow : Window
    {

        LocalDataBaseController dataBaseController = new LocalDataBaseController();
        private bool skipTextChange = false; 
        
        public MainWindow()
        {
            InitializeComponent();
            UpdateButton.IsEnabled = false; 
            NextPageButton.IsEnabled = false;
            PrevPageButton.IsEnabled = false;
        }

        private void ShowThreatInformation(object sender, RoutedEventArgs e)
        {
            Threat threat = (Table.SelectedCells[0].Item as Threat);
            FullInfoWindow info = new FullInfoWindow();
            info.CurThreat = threat;
            info.ShowDialog(); 
        }

        private async void DownloadTable(object sender, RoutedEventArgs e)
        {
            try
            {
                List<Threat> firstPage; 
                DownloadTableButton.IsEnabled = false;
                StatusTextBlock.Text = "Загружаем таблицу...";
                StatusColor.Background = Brushes.Yellow; 
                
                firstPage = await Task.Run(() => dataBaseController.GetLocalDB());
                StatusTextBlock.Text = "Все отлично";
                StatusColor.Background = Brushes.Green;

                PrevPageButton.IsEnabled = false;
                PageText.Text  = $"из {dataBaseController.AmountOfPages}";
                skipTextChange = true;
                InputPage.Text = $"{dataBaseController.PageNumber}";
                skipTextChange = false;
                InputPage.BorderBrush = Brushes.DarkGray;

                NextPageButton.IsEnabled = true;
                UpdateButton.IsEnabled   = true; 

                Table.ItemsSource = firstPage;
                
                Table.Items.Refresh();
            }
            catch (Exception ex) 
            {
                DownloadTableButton.IsEnabled = true;
                StatusTextBlock.Text   = "Ошибка :(";
                StatusColor.Background = Brushes.Red;
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);               
            }
           
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (skipTextChange) return; 

            int result;
            if (InputPage.Text != "")
            {
                if (!int.TryParse(InputPage.Text, out result)) return; 
               
                if (result >= 1 && result <= dataBaseController.AmountOfPages) 
                {
                    Table.ItemsSource = dataBaseController.GetPage(result);

                    if (dataBaseController.PageNumber == dataBaseController.AmountOfPages)
                        NextPageButton.IsEnabled = false;
                    else if (!NextPageButton.IsEnabled)
                        NextPageButton.IsEnabled = true;

                    if (dataBaseController.PageNumber == 1)
                        PrevPageButton.IsEnabled = false;
                    else if (!PrevPageButton.IsEnabled)
                        PrevPageButton.IsEnabled = true; 
                }
            }
            
        }

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {

            Table.ItemsSource = dataBaseController.NextPage();
            PageText.Text  = $"из {dataBaseController.AmountOfPages}";
            
            skipTextChange = true; 
            InputPage.Text = $"{dataBaseController.PageNumber}";
            InputPage.BorderBrush = Brushes.DarkGray;
            skipTextChange = false; 

            Table.Items.Refresh(); 
            if (!PrevPageButton.IsEnabled) PrevPageButton.IsEnabled = true;
            if (dataBaseController.PageNumber == dataBaseController.AmountOfPages) NextPageButton.IsEnabled = false; 

        }

        private void PrevPageButton_Click(object sender, RoutedEventArgs e)
        {
            Table.ItemsSource = dataBaseController.PreviousPage();
            PageText.Text = $"из {dataBaseController.AmountOfPages}";
            skipTextChange = true; 
            InputPage.Text = $"{dataBaseController.PageNumber}";
            InputPage.BorderBrush = Brushes.DarkGray;
            skipTextChange = false; 

            Table.Items.Refresh();
            if (!NextPageButton.IsEnabled) NextPageButton.IsEnabled = true;
            if (dataBaseController.PageNumber == 1) PrevPageButton.IsEnabled = false;
        }

        private void SaveLocalDbButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog(); 
            fbd.ShowDialog();
            string targetPath = fbd.SelectedPath;
            if (!dataBaseController.CopyLocalDbToFile(targetPath))
                MessageBox.Show("Не получилось копировать лбд в указанный файл", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            else MessageBox.Show("Все прошло успешно."); 
        }

        private void ShowChanges_Click(object sender, RoutedEventArgs e)
        {
            ChangesWindow changes = new ChangesWindow();
            changes.Changes = dataBaseController.ChangesList;
            changes.ShowDialog(); 
        }

        private async void UpdateLocalDb_Click(object sender, RoutedEventArgs e)
        {
            StatusTextBlock.Text = "Обновляем ЛБД...";
            StatusColor.Background = Brushes.Yellow;

            try
            {
                await Task.Run(() => dataBaseController.UpdateLocalDb());
                Table.ItemsSource = dataBaseController.GetPage(dataBaseController.PageNumber); //some kind of Table.Items.Refresh, иначе не робит
                StatusTextBlock.Text = "Все отлично!";
                StatusColor.Background = Brushes.Green;
                MessageBox.Show("Успешно!"); 
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusTextBlock.Text = "Ошибка(";
                StatusColor.Background = Brushes.Red;
            }
            
        }

        
        private void DS_MouseDown(object sender, RoutedEventArgs e) 
        {
            dataBaseController.AddHiddenThreat();
            Table.Items.Refresh(); 
        }

        private void DownloadTableButton_GotFocus(object sender, RoutedEventArgs e)
        {
            DownloadTableButton.Background = Brushes.Red; 
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Данное приложение представляет из себя лр, подробнее о лр\n" +
                            "можно почитать здесь https://clck.ru/MxDeX");
        }
    }
}
