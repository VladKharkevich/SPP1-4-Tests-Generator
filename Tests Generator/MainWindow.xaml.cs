using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tests_Generator
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Cs files (*.cs)|*.cs",
            };
            if (openFileDialog.ShowDialog() == true)
            {
                ListBoxItem listBoxItem =  new ListBoxItem();
                listBoxItem.Content = openFileDialog.FileName;
                lboxPathToFiles.Items.Add(listBoxItem);
            }
        }

        private void btnChooseFolder_Click(object sender, RoutedEventArgs e)
        {
            var ookiiDialog = new VistaFolderBrowserDialog();
            if (ookiiDialog.ShowDialog() == true)
            {
                lblPathToFolder.Content = ookiiDialog.SelectedPath;
            }
        }
        
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            int number;
            int.TryParse(tbLimit.Text, out number);
            if ((number <= 0) || (number > 100))
                lblResult.Content = "Ограничение по размеру секции конвейера должно быть от 1 до 100!";
            else if (lblPathToFolder.Content.ToString() == "")
                lblResult.Content = "Нужно выбрать конечную директорию для создания тестовых файлов!";
            else
            {
                List<string> filenames = new List<string>();
                foreach (ListBoxItem listBoxItem in lboxPathToFiles.Items)
                {
                    filenames.Add(listBoxItem.Content.ToString());
                }
                Conveyor conveyor = new Conveyor(filenames, lblPathToFolder.Content.ToString(), number);
                conveyor.Start().Wait();
                lblResult.Content = "OK";
            }
        }
    }
}
