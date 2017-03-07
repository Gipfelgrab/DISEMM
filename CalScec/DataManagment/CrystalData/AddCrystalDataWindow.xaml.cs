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

namespace CalScec.DataManagment.CrystalData
{
    /// <summary>
    /// Interaktionslogik für AddCrystalDataWindow.xaml
    /// </summary>
    public partial class AddCrystalDataWindow : Window
    {
        public CODData CrystalDataList = new CODData();

        public bool ForSave = false;

        public AddCrystalDataWindow()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveAndExitButton_Click(object sender, RoutedEventArgs e)
        {
            string CrystalName = this.NameText.Text;
            string SymmetryGroupHM = this.SymetryBox.Text;
            string SymmetryGroupHall = this.SymmetryHallText.Text;
            string ChemicalFormula = this.ChemicalFormulaText.Text;
            string Comments = this.CommentText.Text;

            try
            {
                double CellA = Convert.ToDouble(CellAText.Text);
                double CellB = Convert.ToDouble(CellBText.Text);
                double CellC = Convert.ToDouble(CellCText.Text);
                double CellAlpha = Convert.ToDouble(CellAlphaText.Text);
                double CellBeta = Convert.ToDouble(CellBetaText.Text);
                double CellGamma = Convert.ToDouble(CellGammaText.Text);
                double CellVolume = Convert.ToDouble(CellVolumeText.Text);

                int SymmetryID = Convert.ToInt32(SymmetryIDText.Text);

                this.CrystalDataList = new CODData(CrystalName, CellVolume, SymmetryID, SymmetryGroupHall, Comments);
                this.CrystalDataList.A = CellA;
                this.CrystalDataList.B = CellB;
                this.CrystalDataList.C = CellC;
                this.CrystalDataList.Alpha = CellAlpha;
                this.CrystalDataList.Beta = CellBeta;
                this.CrystalDataList.Gamma = CellGamma;
                this.CrystalDataList.ChemicalFormula = ChemicalFormula;
                this.CrystalDataList.SymmetryGroup = SymmetryGroupHM;

                Tools.Calculation.AddHKLList(this.CrystalDataList);

                this.ForSave = true;
                this.Close();
            }
            catch
            {
                MessageBox.Show("Some of the given data could not be processed! Please check the cell parameters and volume! Chekc for double sepperation as '.'!", "Some input in wrong format", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
