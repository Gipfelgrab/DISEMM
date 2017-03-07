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

            this.ModelSwitchBox.Items.Add(ModelItem1);
            this.ModelSwitchBox.Items.Add(ModelItem2);
            this.ModelSwitchBox.Items.Add(ModelItem3);

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

            switch(this.ModelSwitchBox.SelectedIndex)
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
                default:
                    UsedTensor = this.ActSample.VoigtTensorData[this.PhaseSwitchBox.SelectedIndex];
                    break;
            }

            #region TensorData

            if (this.StiffnessComlplianceSwitchBox.SelectedIndex == 0)
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
                this.T11.Text = UsedTensor.C11.ToString("F3");
                this.T12.Text = UsedTensor.C12.ToString("F3");
                this.T13.Text = UsedTensor.C13.ToString("F3");
                this.T14.Text = UsedTensor.C14.ToString("F3");
                this.T15.Text = UsedTensor.C15.ToString("F3");
                this.T16.Text = UsedTensor.C16.ToString("F3");

                this.T21.Text = UsedTensor.C21.ToString("F3");
                this.T22.Text = UsedTensor.C22.ToString("F3");
                this.T23.Text = UsedTensor.C23.ToString("F3");
                this.T24.Text = UsedTensor.C24.ToString("F3");
                this.T25.Text = UsedTensor.C25.ToString("F3");
                this.T26.Text = UsedTensor.C26.ToString("F3");

                this.T31.Text = UsedTensor.C31.ToString("F3");
                this.T32.Text = UsedTensor.C32.ToString("F3");
                this.T33.Text = UsedTensor.C33.ToString("F3");
                this.T34.Text = UsedTensor.C34.ToString("F3");
                this.T35.Text = UsedTensor.C35.ToString("F3");
                this.T36.Text = UsedTensor.C36.ToString("F3");

                this.T41.Text = UsedTensor.C41.ToString("F3");
                this.T42.Text = UsedTensor.C42.ToString("F3");
                this.T43.Text = UsedTensor.C43.ToString("F3");
                this.T44.Text = UsedTensor.C44.ToString("F3");
                this.T45.Text = UsedTensor.C45.ToString("F3");
                this.T46.Text = UsedTensor.C46.ToString("F3");

                this.T51.Text = UsedTensor.C51.ToString("F3");
                this.T52.Text = UsedTensor.C52.ToString("F3");
                this.T53.Text = UsedTensor.C53.ToString("F3");
                this.T54.Text = UsedTensor.C54.ToString("F3");
                this.T55.Text = UsedTensor.C55.ToString("F3");
                this.T56.Text = UsedTensor.C56.ToString("F3");

                this.T61.Text = UsedTensor.C61.ToString("F3");
                this.T62.Text = UsedTensor.C62.ToString("F3");
                this.T63.Text = UsedTensor.C63.ToString("F3");
                this.T64.Text = UsedTensor.C64.ToString("F3");
                this.T65.Text = UsedTensor.C65.ToString("F3");
                this.T66.Text = UsedTensor.C66.ToString("F3");

                this.T11E.Content = UsedTensor.C11Error.ToString("F3");
                this.T12E.Content = UsedTensor.C12Error.ToString("F3");
                this.T13E.Content = UsedTensor.C13Error.ToString("F3");
                this.T14E.Content = UsedTensor.C14Error.ToString("F3");
                this.T15E.Content = UsedTensor.C15Error.ToString("F3");
                this.T16E.Content = UsedTensor.C16Error.ToString("F3");

                this.T21E.Content = UsedTensor.C21Error.ToString("F3");
                this.T22E.Content = UsedTensor.C22Error.ToString("F3");
                this.T23E.Content = UsedTensor.C23Error.ToString("F3");
                this.T24E.Content = UsedTensor.C24Error.ToString("F3");
                this.T25E.Content = UsedTensor.C25Error.ToString("F3");
                this.T26E.Content = UsedTensor.C26Error.ToString("F3");

                this.T31E.Content = UsedTensor.C31Error.ToString("F3");
                this.T32E.Content = UsedTensor.C32Error.ToString("F3");
                this.T33E.Content = UsedTensor.C33Error.ToString("F3");
                this.T34E.Content = UsedTensor.C34Error.ToString("F3");
                this.T35E.Content = UsedTensor.C35Error.ToString("F3");
                this.T36E.Content = UsedTensor.C36Error.ToString("F3");

                this.T41E.Content = UsedTensor.C41Error.ToString("F3");
                this.T42E.Content = UsedTensor.C42Error.ToString("F3");
                this.T43E.Content = UsedTensor.C43Error.ToString("F3");
                this.T44E.Content = UsedTensor.C44Error.ToString("F3");
                this.T45E.Content = UsedTensor.C45Error.ToString("F3");
                this.T46E.Content = UsedTensor.C46Error.ToString("F3");

                this.T51E.Content = UsedTensor.C51Error.ToString("F3");
                this.T52E.Content = UsedTensor.C52Error.ToString("F3");
                this.T53E.Content = UsedTensor.C53Error.ToString("F3");
                this.T54E.Content = UsedTensor.C54Error.ToString("F3");
                this.T55E.Content = UsedTensor.C55Error.ToString("F3");
                this.T56E.Content = UsedTensor.C56Error.ToString("F3");

                this.T61E.Content = UsedTensor.C61Error.ToString("F3");
                this.T62E.Content = UsedTensor.C62Error.ToString("F3");
                this.T63E.Content = UsedTensor.C63Error.ToString("F3");
                this.T64E.Content = UsedTensor.C64Error.ToString("F3");
                this.T65E.Content = UsedTensor.C65Error.ToString("F3");
                this.T66E.Content = UsedTensor.C66Error.ToString("F3");
            }

            #endregion

            #region REKData

            this.REKClassicCalculationList.ItemsSource = UsedTensor.DiffractionConstants;
            this.REKMacroskopicCalculationList.ItemsSource = UsedTensor.DiffractionConstants;

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
                    this.ActSample.SetHillTensorData(this.PhaseSwitchBox.SelectedIndex);
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

            this.SetTensorData();

            this.TextEventsActive = true;
        }
        
        private void Tensor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(this.TextEventsActive)
            {
                Stress.Microsopic.ElasticityTensors UsedTensor = null;

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
                    default:
                        UsedTensor = this.ActSample.VoigtTensorData[this.PhaseSwitchBox.SelectedIndex];
                        break;
                }

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
        
    }
}
