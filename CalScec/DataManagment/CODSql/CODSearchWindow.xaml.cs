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

namespace CalScec.DataManagment.CODSql
{
    /// <summary>
    /// Interaktionslogik für CODSearchWindow.xaml
    /// </summary>
    public partial class CODSearchWindow : Window
    {
        public bool SelectionFinished = false;
        public CrystalData.CODData SelectedData = new CrystalData.CODData();
        

        public CODSearchWindow()
        {
            InitializeComponent();

            LastUpdatePicker.SelectedDate = DateTime.Today;
        }

        private void SearchCOD_Click(object sender, RoutedEventArgs e)
        {
            object[] ForWorker = { Convert.ToBoolean(HoleFormula.IsChecked), LastUpdatePicker.SelectedDate, ElementsofSearch.Text, NumberOfElementsInSearch.Text };

            System.ComponentModel.BackgroundWorker SearchForCIFData = new System.ComponentModel.BackgroundWorker();

            SearchForCIFData.DoWork += SeachCODDatabase_Work;
            SearchForCIFData.RunWorkerCompleted += SeachCODDatabase_Completed;

            SearchForCIFData.RunWorkerAsync(ForWorker);

            this.StatusProgress.IsIndeterminate = true;
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
        }

        private void SeachCODDatabase_Work(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            List<CrystalData.CODData> DatDisplay = new List<CrystalData.CODData>();

            System.ComponentModel.BackgroundWorker worker = sender as System.ComponentModel.BackgroundWorker;

            object[] SearchData = e.Argument as object[];
            string NOEIS = SearchData[3] as string;
            if (Convert.ToBoolean(SearchData[0]))
            {
                if (Convert.ToDateTime(SearchData[1]) != DateTime.Today)
                {
                    DatDisplay = GetCODData.GetListOfCrystalData(SearchData[2] as string, Convert.ToDateTime(SearchData[1]));
                }
                else
                {
                    DatDisplay = GetCODData.GetListOfCrystalData(SearchData[2] as string);
                }
            }
            else
            {
                string Tmp = SearchData[2] as string;
                string WithoutSpaced = Tmp.Replace(" ", "");
                string[] ElementsChoosen = WithoutSpaced.Split(',');
                if (NOEIS != "0")
                {
                    if (Convert.ToDateTime(SearchData[1]) != DateTime.Today)
                    {
                        DatDisplay = GetCODData.GetListOfCrystalData(ElementsChoosen, Convert.ToInt32(NOEIS), Convert.ToDateTime(SearchData[1]));
                    }
                    else
                    {
                        DatDisplay = GetCODData.GetListOfCrystalData(ElementsChoosen, Convert.ToInt32(NOEIS));
                    }
                }
                else
                {
                    if (Convert.ToDateTime(SearchData[1]) != DateTime.Today)
                    {
                        DatDisplay = GetCODData.GetListOfCrystalData(ElementsChoosen, Convert.ToDateTime(SearchData[1]));
                    }
                    else
                    {
                        DatDisplay = GetCODData.GetListOfCrystalData(ElementsChoosen);
                    }
                }
            }

            e.Result = DatDisplay;
        }

        private void SeachCODDatabase_Completed(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            System.ComponentModel.BackgroundWorker worker = sender as System.ComponentModel.BackgroundWorker;

            List<CrystalData.CODData> ResultData = e.Result as List<CrystalData.CODData>;
            CODDataList.ItemsSource = ResultData;
            this.StatusProgress.IsIndeterminate = false;
            this.StatusProgress.Value = 0;
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            if (CODDataList.SelectedItem != null)
            {
                this.SelectedData = (CrystalData.CODData)CODDataList.SelectedItem;
                this.SelectionFinished = true;
                this.Close();
            }
        }
    }
}
