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

namespace CalScec.Analysis.Texture
{
    /// <summary>
    /// Interaction logic for PhaseSelectionWindow.xaml
    /// </summary>
    public partial class PhaseSelectionWindow : Window
    {
        public List<DataManagment.CrystalData.CODData> CrystalData;
        private bool _canceled = true;
        public bool Canceled
        {
            get
            {
                if(this._canceled && this.PhaseSelectionBox.SelectedIndex == -1)
                {
                    return true;
                }

                return false;
            }
        }

        public PhaseSelectionWindow(List<DataManagment.CrystalData.CODData> crystalData)
        {
            InitializeComponent();

            this.CrystalData = crystalData;
            AddContent();
        }

        private void AddContent()
        {
            for (int n = 0; n < this.CrystalData.Count; n++)
            {
                ComboBoxItem Tmp = new ComboBoxItem();
                Tmp.Content = this.CrystalData[n].SymmetryGroup;

                this.PhaseSelectionBox.Items.Add(Tmp);
            }
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            this._canceled = false;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this._canceled = true;
            this.Close();
        }
    }
}
