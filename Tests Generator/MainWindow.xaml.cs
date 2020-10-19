using Microsoft.Win32;
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
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                ListBoxItem listBoxItem =  new ListBoxItem();
                listBoxItem.Content = openFileDialog.FileName;
                lboxPathToFiles.Items.Add(listBoxItem);
            }
        }

        private void btnChooseFolder_Click(object sender, RoutedEventArgs e)
        {
            
        }
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
