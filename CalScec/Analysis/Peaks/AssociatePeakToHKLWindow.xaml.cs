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

namespace CalScec.Analysis.Peaks
{
    /// <summary>
    /// Interaktionslogik für AssociatePeakToHKLWindow.xaml
    /// </summary>
    public partial class AssociatePeakToHKLWindow : Window
    {
        public int[] SelectedIndices = { -1, -1 };
        List<DataManagment.CrystalData.CODData> CrystalData;

        public AssociatePeakToHKLWindow(List<DataManagment.CrystalData.CODData> crystalData)
        {
            InitializeComponent();

            CrystalData = crystalData;
            CrystalDataListView.ItemsSource = CrystalData;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveAndExitButton_Click(object sender, RoutedEventArgs e)
        {
            if (HKLListView.SelectedIndex != -1)
            {
                if (CrystalDataListView.SelectedIndex != -1)
                {
                    SelectedIndices[0] = CrystalDataListView.SelectedIndex;
                    SelectedIndices[1] = HKLListView.SelectedIndex;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("I am not so good in mindreading please tell me what reflex you want to associate!", "No information given", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("You need to select a HKL reflex or how should I now what you want to associate??", "No HKL reflex selected", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CrystalDataListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(CrystalDataListView.SelectedIndex != -1)
            {
                DataManagment.CrystalData.CODData SelectedData = (DataManagment.CrystalData.CODData)CrystalDataListView.SelectedItem;
                HKLListView.ItemsSource = SelectedData.HKLList;
            }
            else
            {
                HKLListView.ItemsSource = null;
            }
        }
    }
}
