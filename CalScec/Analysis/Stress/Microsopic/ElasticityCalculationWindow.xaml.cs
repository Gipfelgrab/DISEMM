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

namespace CalScec.Analysis.Stress.Microsopic
{
    /// <summary>
    /// Interaction logic for ElasticityCalculationWindow.xaml
    /// </summary>
    public partial class ElasticityCalculationWindow : Window
    {
        public Sample ActSample;
        bool TextEventsActive = true;

        List<Tools.TextureFitInformation> TextureFitObjects = new List<Tools.TextureFitInformation>();

        public ElasticityCalculationWindow(Sample usedSample)
        {
            InitializeComponent();

            this.ActSample = usedSample;

            this.PrepareREKS();
            this.LoadData();
        }

        private void PrepareREKS()
        {
            for(int n = 0; n < this.ActSample.CrystalData.Count; n++)
            {
                this.ActSample.VoigtTensorData[n].DiffractionConstants = ActSample.DiffractionConstants[n];
                this.ActSample.ReussTensorData[n].DiffractionConstants = ActSample.DiffractionConstants[n];
                this.ActSample.HillTensorData[n].DiffractionConstants = ActSample.DiffractionConstants[n];
                this.ActSample.KroenerTensorData[n].DiffractionConstants = ActSample.DiffractionConstants[n];
                this.ActSample.DeWittTensorData[n].DiffractionConstants = ActSample.DiffractionConstants[n];
                this.ActSample.GeometricHillTensorData[n].DiffractionConstants = ActSample.DiffractionConstants[n];

                if (this.ActSample.HillTensorData[n].ODF != null)
                {
                    this.ActSample.VoigtTensorData[n].ODF.BaseTensor.DiffractionConstants = ActSample.DiffractionConstants[n];
                    this.ActSample.ReussTensorData[n].ODF.BaseTensor.DiffractionConstants = ActSample.DiffractionConstants[n];
                    this.ActSample.HillTensorData[n].ODF.BaseTensor.DiffractionConstants = ActSample.DiffractionConstants[n];
                    this.ActSample.KroenerTensorData[n].ODF.BaseTensor.DiffractionConstants = ActSample.DiffractionConstants[n];
                    this.ActSample.DeWittTensorData[n].ODF.BaseTensor.DiffractionConstants = ActSample.DiffractionConstants[n];
                    this.ActSample.GeometricHillTensorData[n].ODF.BaseTensor.DiffractionConstants = ActSample.DiffractionConstants[n];
                    this.ActSample.VoigtTensorData[n].ODF.TextureTensor.DiffractionConstants = ActSample.DiffractionConstants[n];
                    this.ActSample.ReussTensorData[n].ODF.TextureTensor.DiffractionConstants = ActSample.DiffractionConstants[n];
                    this.ActSample.HillTensorData[n].ODF.TextureTensor.DiffractionConstants = ActSample.DiffractionConstants[n];
                    this.ActSample.KroenerTensorData[n].ODF.TextureTensor.DiffractionConstants = ActSample.DiffractionConstants[n];
                    this.ActSample.DeWittTensorData[n].ODF.TextureTensor.DiffractionConstants = ActSample.DiffractionConstants[n];
                    this.ActSample.GeometricHillTensorData[n].ODF.TextureTensor.DiffractionConstants = ActSample.DiffractionConstants[n];

                    this.ActSample.VoigtTensorData[n].ODF.FitStarted += this.TextureFitStarted;
                    this.ActSample.VoigtTensorData[n].ODF.FitFinished += this.TextureFitFinished;
                    this.ActSample.ReussTensorData[n].ODF.FitStarted += this.TextureFitStarted;
                    this.ActSample.ReussTensorData[n].ODF.FitFinished += this.TextureFitFinished;
                    this.ActSample.HillTensorData[n].ODF.FitStarted += this.TextureFitStarted;
                    this.ActSample.HillTensorData[n].ODF.FitFinished += this.TextureFitFinished;
                    this.ActSample.GeometricHillTensorData[n].ODF.FitStarted += this.TextureFitStarted;
                    this.ActSample.GeometricHillTensorData[n].ODF.FitFinished += this.TextureFitFinished;
                    this.ActSample.KroenerTensorData[n].ODF.FitStarted += this.TextureFitStarted;
                    this.ActSample.KroenerTensorData[n].ODF.FitFinished += this.TextureFitFinished;
                    this.ActSample.DeWittTensorData[n].ODF.FitStarted += this.TextureFitStarted;
                    this.ActSample.DeWittTensorData[n].ODF.FitFinished += this.TextureFitFinished;

                    this.ActSample.VoigtTensorData[n].ODF.SetResetEvent(new System.Threading.ManualResetEvent(true));
                    this.ActSample.ReussTensorData[n].ODF.SetResetEvent(new System.Threading.ManualResetEvent(true));
                    this.ActSample.HillTensorData[n].ODF.SetResetEvent(new System.Threading.ManualResetEvent(true));
                    this.ActSample.GeometricHillTensorData[n].ODF.SetResetEvent(new System.Threading.ManualResetEvent(true));
                    this.ActSample.KroenerTensorData[n].ODF.SetResetEvent(new System.Threading.ManualResetEvent(true));
                    this.ActSample.DeWittTensorData[n].ODF.SetResetEvent(new System.Threading.ManualResetEvent(true));
                }
            }
        }

        private void LoadData()
        {
            TextEventsActive = false;

            for (int n = 0; n < this.ActSample.CrystalData.Count; n++)
            {
                ComboBoxItem PhaseItem = new ComboBoxItem();
                PhaseItem.Content = this.ActSample.CrystalData[n].SymmetryGroup;

                this.PhaseSwitchBox.Items.Add(PhaseItem);
            }

            this.PhaseSwitchBox.SelectedIndex = 0;

            ComboBoxItem ModelItem1 = new ComboBoxItem();
            ModelItem1.Content = "Voigt";
            ComboBoxItem ModelItem2 = new ComboBoxItem();
            ModelItem2.Content = "Reuss";
            ComboBoxItem ModelItem3 = new ComboBoxItem();
            ModelItem3.Content = "Hill";
            ComboBoxItem ModelItem4 = new ComboBoxItem();
            ModelItem4.Content = "Kroener";
            ComboBoxItem ModelItem5 = new ComboBoxItem();
            ModelItem5.Content = "DeWitt";
            ComboBoxItem ModelItem6 = new ComboBoxItem();
            ModelItem6.Content = "Geometric Hill";

            this.ModelSwitchBox.Items.Add(ModelItem1);
            this.ModelSwitchBox.Items.Add(ModelItem2);
            this.ModelSwitchBox.Items.Add(ModelItem3);
            this.ModelSwitchBox.Items.Add(ModelItem4);
            this.ModelSwitchBox.Items.Add(ModelItem5);
            this.ModelSwitchBox.Items.Add(ModelItem6);

            this.ModelSwitchBox.SelectedIndex = 0;

            ComboBoxItem REKItem1 = new ComboBoxItem();
            REKItem1.Content = "Classic";
            ComboBoxItem REKItem2 = new ComboBoxItem();
            REKItem2.Content = "Macroskopic";

            this.REKSwitchBox.Items.Add(REKItem1);
            this.REKSwitchBox.Items.Add(REKItem2);

            this.REKSwitchBox.SelectedIndex = 0;

            ComboBoxItem StiffnessComlplianceItem1 = new ComboBoxItem();
            StiffnessComlplianceItem1.Content = "Compliance tensor";
            ComboBoxItem StiffnessComlplianceItem2 = new ComboBoxItem();
            StiffnessComlplianceItem2.Content = "Stiffness tensor";

            this.StiffnessComlplianceSwitchBox.Items.Add(StiffnessComlplianceItem1);
            this.StiffnessComlplianceSwitchBox.Items.Add(StiffnessComlplianceItem2);

            this.StiffnessComlplianceSwitchBox.SelectedIndex = 0;

            this.SetTensorData();

            TextEventsActive = true;
        }

        private void SetTensorData()
        {
            Stress.Microsopic.ElasticityTensors UsedTensor = null;

            #region Tensor selection

            if (Convert.ToBoolean(this.ActivateTexture.IsChecked))
            {
                if (Convert.ToBoolean(this.ActivateWeightedTensor.IsChecked))
                {
                    switch (this.ModelSwitchBox.SelectedIndex)
                    {
                        case 0:
                            UsedTensor = this.ActSample.VoigtTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.TextureTensor;
                            break;
                        case 1:
                            UsedTensor = this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.TextureTensor;
                            break;
                        case 2:
                            UsedTensor = this.ActSample.HillTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.TextureTensor;
                            break;
                        case 3:
                            UsedTensor = this.ActSample.KroenerTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.TextureTensor;
                            break;
                        case 4:
                            UsedTensor = this.ActSample.DeWittTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.TextureTensor;
                            break;
                        case 5:
                            UsedTensor = this.ActSample.GeometricHillTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.TextureTensor;
                            break;
                        default:
                            UsedTensor = this.ActSample.VoigtTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.TextureTensor;
                            break;
                    }
                }
                else
                {
                    switch (this.ModelSwitchBox.SelectedIndex)
                    {
                        case 0:
                            UsedTensor = this.ActSample.VoigtTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.BaseTensor;
                            break;
                        case 1:
                            UsedTensor = this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.BaseTensor;
                            break;
                        case 2:
                            UsedTensor = this.ActSample.HillTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.BaseTensor;
                            break;
                        case 3:
                            UsedTensor = this.ActSample.KroenerTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.BaseTensor;
                            break;
                        case 4:
                            UsedTensor = this.ActSample.DeWittTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.BaseTensor;
                            break;
                        case 5:
                            UsedTensor = this.ActSample.GeometricHillTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.BaseTensor;
                            break;
                        default:
                            UsedTensor = this.ActSample.VoigtTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.BaseTensor;
                            break;
                    }
                }
            }
            else
            {
                switch (this.ModelSwitchBox.SelectedIndex)
                {
                    case 0:
                        UsedTensor = this.ActSample.VoigtTensorData[this.PhaseSwitchBox.SelectedIndex];
                        break;
                    case 1:
                        UsedTensor = this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex];
                        break;
                    case 2:
                        UsedTensor = this.ActSample.HillTensorData[this.PhaseSwitchBox.SelectedIndex];
                        break;
                    case 3:
                        UsedTensor = this.ActSample.KroenerTensorData[this.PhaseSwitchBox.SelectedIndex];
                        break;
                    case 4:
                        UsedTensor = this.ActSample.DeWittTensorData[this.PhaseSwitchBox.SelectedIndex];
                        break;
                    case 5:
                        UsedTensor = this.ActSample.GeometricHillTensorData[this.PhaseSwitchBox.SelectedIndex];
                        break;
                    default:
                        UsedTensor = this.ActSample.VoigtTensorData[this.PhaseSwitchBox.SelectedIndex];
                        break;
                }
            }


            #endregion

            #region TensorData

            bool Compliance = true;

            if (this.StiffnessComlplianceSwitchBox.SelectedIndex == 1)
            {
                Compliance = false;
            }

            if (Compliance)
            {
                this.T11.Text = UsedTensor.S11.ToString("G3");
                this.T12.Text = UsedTensor.S12.ToString("G3");
                this.T13.Text = UsedTensor.S13.ToString("G3");
                this.T14.Text = UsedTensor.S14.ToString("G3");
                this.T15.Text = UsedTensor.S15.ToString("G3");
                this.T16.Text = UsedTensor.S16.ToString("G3");

                this.T21.Text = UsedTensor.S21.ToString("G3");
                this.T22.Text = UsedTensor.S22.ToString("G3");
                this.T23.Text = UsedTensor.S23.ToString("G3");
                this.T24.Text = UsedTensor.S24.ToString("G3");
                this.T25.Text = UsedTensor.S25.ToString("G3");
                this.T26.Text = UsedTensor.S26.ToString("G3");

                this.T31.Text = UsedTensor.S31.ToString("G3");
                this.T32.Text = UsedTensor.S32.ToString("G3");
                this.T33.Text = UsedTensor.S33.ToString("G3");
                this.T34.Text = UsedTensor.S34.ToString("G3");
                this.T35.Text = UsedTensor.S35.ToString("G3");
                this.T36.Text = UsedTensor.S36.ToString("G3");
            
                this.T41.Text = UsedTensor.S41.ToString("G3");
                this.T42.Text = UsedTensor.S42.ToString("G3");
                this.T43.Text = UsedTensor.S43.ToString("G3");
                this.T44.Text = UsedTensor.S44.ToString("G3");
                this.T45.Text = UsedTensor.S45.ToString("G3");
                this.T46.Text = UsedTensor.S46.ToString("G3");

                this.T51.Text = UsedTensor.S51.ToString("G3");
                this.T52.Text = UsedTensor.S52.ToString("G3");
                this.T53.Text = UsedTensor.S53.ToString("G3");
                this.T54.Text = UsedTensor.S54.ToString("G3");
                this.T55.Text = UsedTensor.S55.ToString("G3");
                this.T56.Text = UsedTensor.S56.ToString("G3");

                this.T61.Text = UsedTensor.S61.ToString("G3");
                this.T62.Text = UsedTensor.S62.ToString("G3");
                this.T63.Text = UsedTensor.S63.ToString("G3");
                this.T64.Text = UsedTensor.S64.ToString("G3");
                this.T65.Text = UsedTensor.S65.ToString("G3");
                this.T66.Text = UsedTensor.S66.ToString("G3");

                this.T11E.Content = UsedTensor.S11Error.ToString("G3");
                this.T12E.Content = UsedTensor.S12Error.ToString("G3");
                this.T13E.Content = UsedTensor.S13Error.ToString("G3");
                this.T14E.Content = UsedTensor.S14Error.ToString("G3");
                this.T15E.Content = UsedTensor.S15Error.ToString("G3");
                this.T16E.Content = UsedTensor.S16Error.ToString("G3");

                this.T21E.Content = UsedTensor.S21Error.ToString("G3");
                this.T22E.Content = UsedTensor.S22Error.ToString("G3");
                this.T23E.Content = UsedTensor.S23Error.ToString("G3");
                this.T24E.Content = UsedTensor.S24Error.ToString("G3");
                this.T25E.Content = UsedTensor.S25Error.ToString("G3");
                this.T26E.Content = UsedTensor.S26Error.ToString("G3");

                this.T31E.Content = UsedTensor.S31Error.ToString("G3");
                this.T32E.Content = UsedTensor.S32Error.ToString("G3");
                this.T33E.Content = UsedTensor.S33Error.ToString("G3");
                this.T34E.Content = UsedTensor.S34Error.ToString("G3");
                this.T35E.Content = UsedTensor.S35Error.ToString("G3");
                this.T36E.Content = UsedTensor.S36Error.ToString("G3");
            
                this.T41E.Content = UsedTensor.S41Error.ToString("G3");
                this.T42E.Content = UsedTensor.S42Error.ToString("G3");
                this.T43E.Content = UsedTensor.S43Error.ToString("G3");
                this.T44E.Content = UsedTensor.S44Error.ToString("G3");
                this.T45E.Content = UsedTensor.S45Error.ToString("G3");
                this.T46E.Content = UsedTensor.S46Error.ToString("G3");
            
                this.T51E.Content = UsedTensor.S51Error.ToString("G3");
                this.T52E.Content = UsedTensor.S52Error.ToString("G3");
                this.T53E.Content = UsedTensor.S53Error.ToString("G3");
                this.T54E.Content = UsedTensor.S54Error.ToString("G3");
                this.T55E.Content = UsedTensor.S55Error.ToString("G3");
                this.T56E.Content = UsedTensor.S56Error.ToString("G3");

                this.T61E.Content = UsedTensor.S61Error.ToString("G3");
                this.T62E.Content = UsedTensor.S62Error.ToString("G3");
                this.T63E.Content = UsedTensor.S63Error.ToString("G3");
                this.T64E.Content = UsedTensor.S64Error.ToString("G3");
                this.T65E.Content = UsedTensor.S65Error.ToString("G3");
                this.T66E.Content = UsedTensor.S66Error.ToString("G3");
            }
            else
            {
                this.T11.Text = UsedTensor.C11.ToString("F0");
                this.T12.Text = UsedTensor.C12.ToString("F0");
                this.T13.Text = UsedTensor.C13.ToString("F0");
                this.T14.Text = UsedTensor.C14.ToString("F0");
                this.T15.Text = UsedTensor.C15.ToString("F0");
                this.T16.Text = UsedTensor.C16.ToString("F0");

                this.T21.Text = UsedTensor.C21.ToString("F0");
                this.T22.Text = UsedTensor.C22.ToString("F0");
                this.T23.Text = UsedTensor.C23.ToString("F0");
                this.T24.Text = UsedTensor.C24.ToString("F0");
                this.T25.Text = UsedTensor.C25.ToString("F0");
                this.T26.Text = UsedTensor.C26.ToString("F0");

                this.T31.Text = UsedTensor.C31.ToString("F0");
                this.T32.Text = UsedTensor.C32.ToString("F0");
                this.T33.Text = UsedTensor.C33.ToString("F0");
                this.T34.Text = UsedTensor.C34.ToString("F0");
                this.T35.Text = UsedTensor.C35.ToString("F0");
                this.T36.Text = UsedTensor.C36.ToString("F0");

                this.T41.Text = UsedTensor.C41.ToString("F0");
                this.T42.Text = UsedTensor.C42.ToString("F0");
                this.T43.Text = UsedTensor.C43.ToString("F0");
                this.T44.Text = UsedTensor.C44.ToString("F0");
                this.T45.Text = UsedTensor.C45.ToString("F0");
                this.T46.Text = UsedTensor.C46.ToString("F0");

                this.T51.Text = UsedTensor.C51.ToString("F0");
                this.T52.Text = UsedTensor.C52.ToString("F0");
                this.T53.Text = UsedTensor.C53.ToString("F0");
                this.T54.Text = UsedTensor.C54.ToString("F0");
                this.T55.Text = UsedTensor.C55.ToString("F0");
                this.T56.Text = UsedTensor.C56.ToString("F0");

                this.T61.Text = UsedTensor.C61.ToString("F0");
                this.T62.Text = UsedTensor.C62.ToString("F0");
                this.T63.Text = UsedTensor.C63.ToString("F0");
                this.T64.Text = UsedTensor.C64.ToString("F0");
                this.T65.Text = UsedTensor.C65.ToString("F0");
                this.T66.Text = UsedTensor.C66.ToString("F0");

                this.T11E.Content = UsedTensor.C11Error.ToString("F0");
                this.T12E.Content = UsedTensor.C12Error.ToString("F0");
                this.T13E.Content = UsedTensor.C13Error.ToString("F0");
                this.T14E.Content = UsedTensor.C14Error.ToString("F0");
                this.T15E.Content = UsedTensor.C15Error.ToString("F0");
                this.T16E.Content = UsedTensor.C16Error.ToString("F0");

                this.T21E.Content = UsedTensor.C21Error.ToString("F0");
                this.T22E.Content = UsedTensor.C22Error.ToString("F0");
                this.T23E.Content = UsedTensor.C23Error.ToString("F0");
                this.T24E.Content = UsedTensor.C24Error.ToString("F0");
                this.T25E.Content = UsedTensor.C25Error.ToString("F0");
                this.T26E.Content = UsedTensor.C26Error.ToString("F0");

                this.T31E.Content = UsedTensor.C31Error.ToString("F0");
                this.T32E.Content = UsedTensor.C32Error.ToString("F0");
                this.T33E.Content = UsedTensor.C33Error.ToString("F0");
                this.T34E.Content = UsedTensor.C34Error.ToString("F0");
                this.T35E.Content = UsedTensor.C35Error.ToString("F0");
                this.T36E.Content = UsedTensor.C36Error.ToString("F0");

                this.T41E.Content = UsedTensor.C41Error.ToString("F0");
                this.T42E.Content = UsedTensor.C42Error.ToString("F0");
                this.T43E.Content = UsedTensor.C43Error.ToString("F0");
                this.T44E.Content = UsedTensor.C44Error.ToString("F0");
                this.T45E.Content = UsedTensor.C45Error.ToString("F0");
                this.T46E.Content = UsedTensor.C46Error.ToString("F0");

                this.T51E.Content = UsedTensor.C51Error.ToString("F0");
                this.T52E.Content = UsedTensor.C52Error.ToString("F0");
                this.T53E.Content = UsedTensor.C53Error.ToString("F0");
                this.T54E.Content = UsedTensor.C54Error.ToString("F0");
                this.T55E.Content = UsedTensor.C55Error.ToString("F0");
                this.T56E.Content = UsedTensor.C56Error.ToString("F0");

                this.T61E.Content = UsedTensor.C61Error.ToString("F0");
                this.T62E.Content = UsedTensor.C62Error.ToString("F0");
                this.T63E.Content = UsedTensor.C63Error.ToString("F0");
                this.T64E.Content = UsedTensor.C64Error.ToString("F0");
                this.T65E.Content = UsedTensor.C65Error.ToString("F0");
                this.T66E.Content = UsedTensor.C66Error.ToString("F3");
            }

            #endregion

            #region Results

            List<REK> CalculatedREK = UsedTensor.GetCalculatedDiffractionConstants(this.ModelSwitchBox.SelectedIndex, this.ActSample.CrystalData[this.PhaseSwitchBox.SelectedIndex], this.StiffnessComlplianceSwitchBox.SelectedIndex);

            UsedTensor.SetAverageParameters(CalculatedREK);
            UsedTensor.SetAverageParametersFit();

            this.AEModulusLabel.Content = UsedTensor.AveragedEModul.ToString("F3");
            this.ANuLabel.Content = UsedTensor.AveragedNu.ToString("F3");
            this.AShearModulusLabel.Content = UsedTensor.AveragedSchearModul.ToString("F3");
            this.ABulkModulusLabel.Content = UsedTensor.AveragedBulkModul.ToString("e3");
            this.AEModulusFitLabel.Content = UsedTensor.AveragedEModulFit.ToString("F3");
            this.ANuFitLabel.Content = UsedTensor.AveragedNuFit.ToString("F3");
            this.AShearModulusFitLabel.Content = UsedTensor.AveragedSchearModulFit.ToString("F3");
            this.ABulkModulusFitLabel.Content = UsedTensor.AveragedBulkModulFit.ToString("e3");

            double Chi2Tmp = 0.0;
            if (UsedTensor.Symmetry == "cubic")
            {
                Chi2Tmp = UsedTensor.GetFittingChi2Cubic(this.ModelSwitchBox.SelectedIndex, Compliance);
            }
            else
            {
                Chi2Tmp = UsedTensor.GetFittingChi2Hexagonal(this.ModelSwitchBox.SelectedIndex, Compliance);
            }
            this.Chi2TensorFitLabel.Content = Chi2Tmp.ToString("F3");

            #endregion

            #region REKData

            this.REKClassicCalculationList.ItemsSource = UsedTensor.DiffractionConstants;
            this.REKMacroskopicCalculationList.ItemsSource = UsedTensor.DiffractionConstants;
            this.REKMatrixCalculationList.ItemsSource = CalculatedREK;

            #endregion
        }

        private void SwitchBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(this.TextEventsActive)
            {
                this.TextEventsActive = false;
                this.SetTensorData();
                this.TextEventsActive = true;
            }
        }

        private void RefitConstants_Click(object sender, RoutedEventArgs e)
        {
            this.TextEventsActive = false;

            if (Convert.ToBoolean(this.ActivateTexture.IsChecked))
            {
                switch (this.ModelSwitchBox.SelectedIndex)
                {
                    case 0:
                        if (!this.ActSample.VoigtTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.fitActive)
                        {
                            this.ActSample.VoigtTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.FittingModel = 0;
                            if (REKSwitchBox.SelectedIndex == 0)
                            {
                                this.ActSample.VoigtTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.ClassicalCalculation = true;
                            }
                            else
                            {
                                this.ActSample.VoigtTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.ClassicalCalculation = false;
                            }
                            System.Threading.ThreadPool.QueueUserWorkItem(this.ActSample.VoigtTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.FitTensorCallback);
                        }
                        break;
                    case 1:
                        if (!this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.fitActive)
                        {
                            this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.FittingModel = 1;
                            if (REKSwitchBox.SelectedIndex == 0)
                            {
                                this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.ClassicalCalculation = true;
                            }
                            else
                            {
                                this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.ClassicalCalculation = false;
                            }
                            System.Threading.ThreadPool.QueueUserWorkItem(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.FitTensorCallback);
                        }
                        break;
                    case 2:
                        if (!this.ActSample.HillTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.fitActive)
                        {
                            this.ActSample.HillTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.FittingModel = 2;
                            if (REKSwitchBox.SelectedIndex == 0)
                            {
                                this.ActSample.HillTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.ClassicalCalculation = true;
                            }
                            else
                            {
                                this.ActSample.HillTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.ClassicalCalculation = false; ;
                            }
                            System.Threading.ThreadPool.QueueUserWorkItem(this.ActSample.HillTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.FitTensorCallback);
                        }
                        break;
                    case 3:
                        if (!this.ActSample.KroenerTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.fitActive)
                        {
                            this.ActSample.KroenerTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.UseStifnessCalculation = false;
                            if (this.StiffnessComlplianceSwitchBox.SelectedIndex == 1)
                            {
                                this.ActSample.KroenerTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.UseStifnessCalculation = true;
                            }
                            this.ActSample.KroenerTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.FittingModel = 4;
                            if (REKSwitchBox.SelectedIndex == 0)
                            {
                                this.ActSample.KroenerTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.ClassicalCalculation = true;
                            }
                            else
                            {
                                this.ActSample.KroenerTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.ClassicalCalculation = false;
                            }
                            System.Threading.ThreadPool.QueueUserWorkItem(this.ActSample.KroenerTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.FitTensorCallback);
                        }
                        break;
                    case 4:
                        if (!this.ActSample.DeWittTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.fitActive)
                        {
                            this.ActSample.DeWittTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.UseStifnessCalculation = false;
                            if (this.StiffnessComlplianceSwitchBox.SelectedIndex == 1)
                            {
                                this.ActSample.DeWittTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.UseStifnessCalculation = true;
                            }
                            this.ActSample.DeWittTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.FittingModel = 5;
                            if (REKSwitchBox.SelectedIndex == 0)
                            {
                                this.ActSample.DeWittTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.ClassicalCalculation = true;
                            }
                            else
                            {
                                this.ActSample.DeWittTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.ClassicalCalculation = false;
                            }
                            System.Threading.ThreadPool.QueueUserWorkItem(this.ActSample.DeWittTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.FitTensorCallback);
                        }
                        break;
                    case 5:
                        if (!this.ActSample.GeometricHillTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.fitActive)
                        {
                            this.ActSample.GeometricHillTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.FittingModel = 3;
                            if (REKSwitchBox.SelectedIndex == 0)
                            {
                                this.ActSample.GeometricHillTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.ClassicalCalculation = true;
                            }
                            else
                            {
                                this.ActSample.GeometricHillTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.ClassicalCalculation = false;
                            }
                            System.Threading.ThreadPool.QueueUserWorkItem(this.ActSample.GeometricHillTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.FitTensorCallback);
                        }
                        break;
                    default:
                        if (!this.ActSample.VoigtTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.fitActive)
                        {
                            this.ActSample.VoigtTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.FittingModel = 0;
                            if (REKSwitchBox.SelectedIndex == 0)
                            {
                                this.ActSample.VoigtTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.ClassicalCalculation = true;
                            }
                            else
                            {
                                this.ActSample.VoigtTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.ClassicalCalculation = false;
                            }
                            System.Threading.ThreadPool.QueueUserWorkItem(this.ActSample.VoigtTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.FitTensorCallback);
                        }
                        break;
                }
            }
            else
            {
                switch (this.ModelSwitchBox.SelectedIndex)
                {
                    case 0:
                        if (REKSwitchBox.SelectedIndex == 0)
                        {
                            this.ActSample.VoigtTensorData[this.PhaseSwitchBox.SelectedIndex].FitVoigt(true);
                        }
                        else
                        {
                            this.ActSample.VoigtTensorData[this.PhaseSwitchBox.SelectedIndex].FitVoigt(false);
                        }
                        break;
                    case 1:
                        if (REKSwitchBox.SelectedIndex == 0)
                        {
                            this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex].FitReuss(true);
                        }
                        else
                        {
                            this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex].FitReuss(false);
                        }
                        break;
                    case 2:
                        if (REKSwitchBox.SelectedIndex == 0)
                        {
                            this.ActSample.HillTensorData[this.PhaseSwitchBox.SelectedIndex].FitHill(true);
                        }
                        else
                        {
                            this.ActSample.HillTensorData[this.PhaseSwitchBox.SelectedIndex].FitHill(false);
                        }
                        break;
                    case 3:
                        bool SC = false;
                        if (this.StiffnessComlplianceSwitchBox.SelectedIndex == 1)
                        {
                            SC = true;
                        }
                        if (REKSwitchBox.SelectedIndex == 0)
                        {
                            this.ActSample.KroenerTensorData[this.PhaseSwitchBox.SelectedIndex].FitKroener(true, SC);
                        }
                        else
                        {
                            this.ActSample.KroenerTensorData[this.PhaseSwitchBox.SelectedIndex].FitKroener(false, SC);
                        }
                        break;
                    case 4:
                        bool SC1 = false;
                        if (this.StiffnessComlplianceSwitchBox.SelectedIndex == 1)
                        {
                            SC1 = true;
                        }
                        if (REKSwitchBox.SelectedIndex == 0)
                        {
                            this.ActSample.DeWittTensorData[this.PhaseSwitchBox.SelectedIndex].FitDeWitt(true, SC1);
                        }
                        else
                        {
                            this.ActSample.DeWittTensorData[this.PhaseSwitchBox.SelectedIndex].FitDeWitt(false, SC1);
                        }
                        break;
                    case 5:
                        if (REKSwitchBox.SelectedIndex == 0)
                        {
                            this.ActSample.GeometricHillTensorData[this.PhaseSwitchBox.SelectedIndex].FitGeometricHill(true);
                        }
                        else
                        {
                            this.ActSample.GeometricHillTensorData[this.PhaseSwitchBox.SelectedIndex].FitGeometricHill(false);
                        }
                        break;
                    default:
                        if (REKSwitchBox.SelectedIndex == 0)
                        {
                            this.ActSample.VoigtTensorData[this.PhaseSwitchBox.SelectedIndex].FitVoigt(true);
                        }
                        else
                        {
                            this.ActSample.VoigtTensorData[this.PhaseSwitchBox.SelectedIndex].FitVoigt(false);
                        }
                        break;
                }
            }

            this.SetTensorData();

            this.TextEventsActive = true;
        }
        
        private void Tensor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(this.TextEventsActive)
            {
                Stress.Microsopic.ElasticityTensors UsedTensor = null;

                #region Tensor selection

                if (Convert.ToBoolean(this.ActivateTexture.IsChecked))
                {
                    if (Convert.ToBoolean(this.ActivateWeightedTensor.IsChecked))
                    {
                        switch (this.ModelSwitchBox.SelectedIndex)
                        {
                            case 0:
                                UsedTensor = this.ActSample.VoigtTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.TextureTensor;
                                break;
                            case 1:
                                UsedTensor = this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.TextureTensor;
                                break;
                            case 2:
                                UsedTensor = this.ActSample.HillTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.TextureTensor;
                                break;
                            case 3:
                                UsedTensor = this.ActSample.KroenerTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.TextureTensor;
                                break;
                            case 4:
                                UsedTensor = this.ActSample.DeWittTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.TextureTensor;
                                break;
                            case 5:
                                UsedTensor = this.ActSample.GeometricHillTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.TextureTensor;
                                break;
                            default:
                                UsedTensor = this.ActSample.VoigtTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.TextureTensor;
                                break;
                        }
                    }
                    else
                    {
                        switch (this.ModelSwitchBox.SelectedIndex)
                        {
                            case 0:
                                UsedTensor = this.ActSample.VoigtTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.BaseTensor;
                                break;
                            case 1:
                                UsedTensor = this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.BaseTensor;
                                break;
                            case 2:
                                UsedTensor = this.ActSample.HillTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.BaseTensor;
                                break;
                            case 3:
                                UsedTensor = this.ActSample.KroenerTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.BaseTensor;
                                break;
                            case 4:
                                UsedTensor = this.ActSample.DeWittTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.BaseTensor;
                                break;
                            case 5:
                                UsedTensor = this.ActSample.GeometricHillTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.BaseTensor;
                                break;
                            default:
                                UsedTensor = this.ActSample.VoigtTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.BaseTensor;
                                break;
                        }
                    }
                }
                else
                {
                    switch (this.ModelSwitchBox.SelectedIndex)
                    {
                        case 0:
                            UsedTensor = this.ActSample.VoigtTensorData[this.PhaseSwitchBox.SelectedIndex];
                            break;
                        case 1:
                            UsedTensor = this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex];
                            break;
                        case 2:
                            UsedTensor = this.ActSample.HillTensorData[this.PhaseSwitchBox.SelectedIndex];
                            break;
                        case 3:
                            UsedTensor = this.ActSample.KroenerTensorData[this.PhaseSwitchBox.SelectedIndex];
                            break;
                        case 4:
                            UsedTensor = this.ActSample.DeWittTensorData[this.PhaseSwitchBox.SelectedIndex];
                            break;
                        case 5:
                            UsedTensor = this.ActSample.GeometricHillTensorData[this.PhaseSwitchBox.SelectedIndex];
                            break;
                        default:
                            UsedTensor = this.ActSample.VoigtTensorData[this.PhaseSwitchBox.SelectedIndex];
                            break;
                    }
                }


                #endregion

                TextBox UsedTextBox = sender as TextBox;

                try
                {
                    double NewValue = Convert.ToDouble(UsedTextBox.Text);
                    switch (UsedTextBox.Name)
                    {
                        case "T11":
                            if ((this.StiffnessComlplianceSwitchBox.SelectedIndex == 0))
                            {
                                UsedTensor.S11 = NewValue;
                            }
                            else
                            {
                                UsedTensor.C11 = NewValue;
                            }
                            break;
                        case "T12":
                            if ((this.StiffnessComlplianceSwitchBox.SelectedIndex == 0))
                            {
                                UsedTensor.S12 = NewValue;
                            }
                            else
                            {
                                UsedTensor.C12 = NewValue;
                            }
                            break;
                        case "T13":
                            if ((this.StiffnessComlplianceSwitchBox.SelectedIndex == 0))
                            {
                                UsedTensor.S13 = NewValue;
                            }
                            else
                            {
                                UsedTensor.C13 = NewValue;
                            }
                            break;
                        case "T14":
                            if ((this.StiffnessComlplianceSwitchBox.SelectedIndex == 0))
                            {
                                UsedTensor.S14 = NewValue;
                            }
                            else
                            {
                                UsedTensor.C14 = NewValue;
                            }
                            break;
                        case "T15":
                            if ((this.StiffnessComlplianceSwitchBox.SelectedIndex == 0))
                            {
                                UsedTensor.S15 = NewValue;
                            }
                            else
                            {
                                UsedTensor.C15 = NewValue;
                            }
                            break;
                        case "T16":
                            if ((this.StiffnessComlplianceSwitchBox.SelectedIndex == 0))
                            {
                                UsedTensor.S16 = NewValue;
                            }
                            else
                            {
                                UsedTensor.C16 = NewValue;
                            }
                            break;
                        case "T21":
                            if ((this.StiffnessComlplianceSwitchBox.SelectedIndex == 0))
                            {
                                UsedTensor.S21 = NewValue;
                            }
                            else
                            {
                                UsedTensor.C21 = NewValue;
                            }
                            break;
                        case "T22":
                            if ((this.StiffnessComlplianceSwitchBox.SelectedIndex == 0))
                            {
                                UsedTensor.S22 = NewValue;
                            }
                            else
                            {
                                UsedTensor.C22 = NewValue;
                            }
                            break;
                        case "T23":
                            if ((this.StiffnessComlplianceSwitchBox.SelectedIndex == 0))
                            {
                                UsedTensor.S23 = NewValue;
                            }
                            else
                            {
                                UsedTensor.C23 = NewValue;
                            }
                            break;
                        case "T24":
                            if ((this.StiffnessComlplianceSwitchBox.SelectedIndex == 0))
                            {
                                UsedTensor.S24 = NewValue;
                            }
                            else
                            {
                                UsedTensor.C24 = NewValue;
                            }
                            break;
                        case "T25":
                            if ((this.StiffnessComlplianceSwitchBox.SelectedIndex == 0))
                            {
                                UsedTensor.S25 = NewValue;
                            }
                            else
                            {
                                UsedTensor.C25 = NewValue;
                            }
                            break;
                        case "T26":
                            if ((this.StiffnessComlplianceSwitchBox.SelectedIndex == 0))
                            {
                                UsedTensor.S26 = NewValue;
                            }
                            else
                            {
                                UsedTensor.C26 = NewValue;
                            }
                            break;
                        case "T31":
                            if ((this.StiffnessComlplianceSwitchBox.SelectedIndex == 0))
                            {
                                UsedTensor.S31 = NewValue;
                            }
                            else
                            {
                                UsedTensor.C31 = NewValue;
                            }
                            break;
                        case "T32":
                            if ((this.StiffnessComlplianceSwitchBox.SelectedIndex == 0))
                            {
                                UsedTensor.S32 = NewValue;
                            }
                            else
                            {
                                UsedTensor.C32 = NewValue;
                            }
                            break;
                        case "T33":
                            if ((this.StiffnessComlplianceSwitchBox.SelectedIndex == 0))
                            {
                                UsedTensor.S33 = NewValue;
                            }
                            else
                            {
                                UsedTensor.C33 = NewValue;
                            }
                            break;
                        case "T34":
                            if ((this.StiffnessComlplianceSwitchBox.SelectedIndex == 0))
                            {
                                UsedTensor.S34 = NewValue;
                            }
                            else
                            {
                                UsedTensor.C34 = NewValue;
                            }
                            break;
                        case "T35":
                            if ((this.StiffnessComlplianceSwitchBox.SelectedIndex == 0))
                            {
                                UsedTensor.S35 = NewValue;
                            }
                            else
                            {
                                UsedTensor.C35 = NewValue;
                            }
                            break;
                        case "T36":
                            if ((this.StiffnessComlplianceSwitchBox.SelectedIndex == 0))
                            {
                                UsedTensor.S36 = NewValue;
                            }
                            else
                            {
                                UsedTensor.C36 = NewValue;
                            }
                            break;
                        case "T41":
                            if ((this.StiffnessComlplianceSwitchBox.SelectedIndex == 0))
                            {
                                UsedTensor.S41 = NewValue;
                            }
                            else
                            {
                                UsedTensor.C41 = NewValue;
                            }
                            break;
                        case "T42":
                            if ((this.StiffnessComlplianceSwitchBox.SelectedIndex == 0))
                            {
                                UsedTensor.S42 = NewValue;
                            }
                            else
                            {
                                UsedTensor.C42 = NewValue;
                            }
                            break;
                        case "T43":
                            if ((this.StiffnessComlplianceSwitchBox.SelectedIndex == 0))
                            {
                                UsedTensor.S43 = NewValue;
                            }
                            else
                            {
                                UsedTensor.C43 = NewValue;
                            }
                            break;
                        case "T44":
                            if ((this.StiffnessComlplianceSwitchBox.SelectedIndex == 0))
                            {
                                UsedTensor.S44 = NewValue;
                            }
                            else
                            {
                                UsedTensor.C44 = NewValue;
                            }
                            break;
                        case "T45":
                            if ((this.StiffnessComlplianceSwitchBox.SelectedIndex == 0))
                            {
                                UsedTensor.S45 = NewValue;
                            }
                            else
                            {
                                UsedTensor.C45 = NewValue;
                            }
                            break;
                        case "T46":
                            if ((this.StiffnessComlplianceSwitchBox.SelectedIndex == 0))
                            {
                                UsedTensor.S46 = NewValue;
                            }
                            else
                            {
                                UsedTensor.C46 = NewValue;
                            }
                            break;
                        case "T51":
                            if ((this.StiffnessComlplianceSwitchBox.SelectedIndex == 0))
                            {
                                UsedTensor.S51 = NewValue;
                            }
                            else
                            {
                                UsedTensor.C51 = NewValue;
                            }
                            break;
                        case "T52":
                            if ((this.StiffnessComlplianceSwitchBox.SelectedIndex == 0))
                            {
                                UsedTensor.S52 = NewValue;
                            }
                            else
                            {
                                UsedTensor.C52 = NewValue;
                            }
                            break;
                        case "T53":
                            if ((this.StiffnessComlplianceSwitchBox.SelectedIndex == 0))
                            {
                                UsedTensor.S53 = NewValue;
                            }
                            else
                            {
                                UsedTensor.C53 = NewValue;
                            }
                            break;
                        case "T54":
                            if ((this.StiffnessComlplianceSwitchBox.SelectedIndex == 0))
                            {
                                UsedTensor.S54 = NewValue;
                            }
                            else
                            {
                                UsedTensor.C54 = NewValue;
                            }
                            break;
                        case "T55":
                            if ((this.StiffnessComlplianceSwitchBox.SelectedIndex == 0))
                            {
                                UsedTensor.S55 = NewValue;
                            }
                            else
                            {
                                UsedTensor.C55 = NewValue;
                            }
                            break;
                        case "T56":
                            if ((this.StiffnessComlplianceSwitchBox.SelectedIndex == 0))
                            {
                                UsedTensor.S56 = NewValue;
                            }
                            else
                            {
                                UsedTensor.C56 = NewValue;
                            }
                            break;
                        case "T61":
                            if ((this.StiffnessComlplianceSwitchBox.SelectedIndex == 0))
                            {
                                UsedTensor.S61 = NewValue;
                            }
                            else
                            {
                                UsedTensor.C61 = NewValue;
                            }
                            break;
                        case "T62":
                            if ((this.StiffnessComlplianceSwitchBox.SelectedIndex == 0))
                            {
                                UsedTensor.S62 = NewValue;
                            }
                            else
                            {
                                UsedTensor.C62 = NewValue;
                            }
                            break;
                        case "T63":
                            if ((this.StiffnessComlplianceSwitchBox.SelectedIndex == 0))
                            {
                                UsedTensor.S63 = NewValue;
                            }
                            else
                            {
                                UsedTensor.C63 = NewValue;
                            }
                            break;
                        case "T64":
                            if ((this.StiffnessComlplianceSwitchBox.SelectedIndex == 0))
                            {
                                UsedTensor.S64 = NewValue;
                            }
                            else
                            {
                                UsedTensor.C64 = NewValue;
                            }
                            break;
                        case "T65":
                            if ((this.StiffnessComlplianceSwitchBox.SelectedIndex == 0))
                            {
                                UsedTensor.S65 = NewValue;
                            }
                            else
                            {
                                UsedTensor.C65 = NewValue;
                            }
                            break;
                        case "T66":
                            if ((this.StiffnessComlplianceSwitchBox.SelectedIndex == 0))
                            {
                                UsedTensor.S66 = NewValue;
                            }
                            else
                            {
                                UsedTensor.C66 = NewValue;
                            }
                            break;
                    }

                    if ((this.StiffnessComlplianceSwitchBox.SelectedIndex == 0))
                    {
                        UsedTensor.CalculateStiffnesses();
                    }
                    else
                    {
                        UsedTensor.CalculateCompliances();
                    }

                    UsedTensor.SetErrors(0.01);
                    this.SetTensorData();
                }
                catch
                {

                }
            }
        }

        #region Texture

        private void ActivateTexture_Checked(object sender, RoutedEventArgs e)
        {
            this.TextEventsActive = false;
            this.SetTensorData();
            this.TextEventsActive = true;
        }

        private void ActivateTexture_Unchecked(object sender, RoutedEventArgs e)
        {
            this.TextEventsActive = false;
            this.SetTensorData();
            this.TextEventsActive = true;
        }

        private void SetTextureTensor_Click(object sender, RoutedEventArgs e)
        {
            this.TextEventsActive = false;
            switch (this.ModelSwitchBox.SelectedIndex)
            {
                case 0:
                    this.ActSample.VoigtTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.BaseTensor = this.ActSample.VoigtTensorData[this.PhaseSwitchBox.SelectedIndex].Clone() as ElasticityTensors;
                    this.ActSample.VoigtTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.BaseTensor.ODF = null;
                    break;
                case 1:
                    this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.BaseTensor = this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex].Clone() as ElasticityTensors;
                    this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.BaseTensor.ODF = null;
                    break;
                case 2:
                    this.ActSample.HillTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.BaseTensor = this.ActSample.HillTensorData[this.PhaseSwitchBox.SelectedIndex].Clone() as ElasticityTensors;
                    this.ActSample.HillTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.BaseTensor.ODF = null;
                    break;
                case 3:
                    this.ActSample.KroenerTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.BaseTensor = this.ActSample.KroenerTensorData[this.PhaseSwitchBox.SelectedIndex].Clone() as ElasticityTensors;
                    this.ActSample.KroenerTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.BaseTensor.ODF = null;
                    break;
                case 4:
                    this.ActSample.DeWittTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.BaseTensor = this.ActSample.DeWittTensorData[this.PhaseSwitchBox.SelectedIndex].Clone() as ElasticityTensors;
                    this.ActSample.DeWittTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.BaseTensor.ODF = null;
                    break;
                case 5:
                    this.ActSample.GeometricHillTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.BaseTensor = this.ActSample.HillTensorData[this.PhaseSwitchBox.SelectedIndex].Clone() as ElasticityTensors;
                    this.ActSample.GeometricHillTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.BaseTensor.ODF = null;
                    break;
                default:
                    this.ActSample.VoigtTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.BaseTensor = this.ActSample.VoigtTensorData[this.PhaseSwitchBox.SelectedIndex].Clone() as ElasticityTensors;
                    this.ActSample.VoigtTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.BaseTensor.ODF = null;
                    break;
            }

            this.SetTensorData();
            this.TextEventsActive = true;
        }

        private void SetWeightedTensor_Click(object sender, RoutedEventArgs e)
        {
            this.TextEventsActive = false;
            switch (this.ModelSwitchBox.SelectedIndex)
            {
                case 0:
                    this.ActSample.VoigtTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.SetTextureTensor();
                    break;
                case 1:
                    this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.SetTextureTensor();
                    break;
                case 2:
                    this.ActSample.HillTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.SetTextureTensor();
                    break;
                case 3:
                    this.ActSample.KroenerTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.SetTextureTensor();
                    break;
                case 4:
                    this.ActSample.DeWittTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.SetTextureTensor();
                    break;
                case 5:
                    this.ActSample.GeometricHillTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.SetTextureTensor();
                    break;
                default:
                    this.ActSample.VoigtTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.SetTextureTensor();
                    break;
            }

            this.SetTensorData();
            this.TextEventsActive = true;
        }

        private void ActivateWeightedTensor_Checked(object sender, RoutedEventArgs e)
        {
            this.TextEventsActive = false;
            
            this.SetTensorData();
            this.TextEventsActive = true;
        }

        private void ActivateWeightedTensor_Unchecked(object sender, RoutedEventArgs e)
        {
            this.TextEventsActive = false;
            this.SetTensorData();
            this.TextEventsActive = true;
        }

        #region Multi threading und thread-pooling

        public delegate void FitTextureUpdateDelegate(Texture.OrientationDistributionFunction oDF);

        private void TextureFitStarted(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            FitTextureUpdateDelegate FittingDelegate = TextureFitStartedHandler;

            Dispatcher.Invoke(FittingDelegate, sender as Texture.OrientationDistributionFunction);
        }

        private void TextureFitFinished(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            FitTextureUpdateDelegate FittingDelegate = TextureFitFinishedHandler;

            Dispatcher.Invoke(FittingDelegate, sender as Texture.OrientationDistributionFunction);
        }
        

        private void TextureFitStartedHandler(Texture.OrientationDistributionFunction oDF)
        {
            Tools.TextureFitInformation newTextureFitObject = new Tools.TextureFitInformation(oDF.fittingModel);
            this.TextureFitObjects.Add(newTextureFitObject);

            this.TextureFittingPoolList.Items.Refresh();

            if (this.TextureProgress.IsIndeterminate)
            {
                this.TextureProgress.ToolTip = "Fitting " + Convert.ToString(this.TextureFitObjects.Count) + " Tensors";
            }
            else
            {
                this.TextureProgress.IsIndeterminate = true;
                this.TextureProgress.ToolTip = "Fitting " + Convert.ToString(this.TextureFitObjects.Count) + " Tensors";
            }
        }

        private void TextureFitFinishedHandler(Texture.OrientationDistributionFunction oDF)
        {
            for(int n = 0; n < TextureFitObjects.Count; n++)
            {
                if(oDF.fittingModel == TextureFitObjects[n].ModelName)
                {
                    TextureFitObjects.RemoveAt(n);
                    break;
                }
            }

            this.TextureFittingPoolList.Items.Refresh();

            this.TextureProgress.ToolTip = "Fitting " + Convert.ToString(this.TextureFitObjects.Count) + " Tensors";

            if (this.TextureFitObjects.Count == 0)
            {
                this.TextureProgress.IsIndeterminate = false;
            }
        }

        #endregion

        #endregion
    }
}
