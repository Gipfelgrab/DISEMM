﻿using System;
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

using OxyPlot;

namespace CalScec.Analysis.Stress.Microsopic
{
    /// <summary>
    /// Interaction logic for ElasticityCalculationWindow.xaml
    /// </summary>
    public partial class ElasticityCalculationWindow : Window
    {
        public Sample ActSample;
        bool TextEventsActive = true;

        int textureId = 0;

        List<Tools.TextureFitInformation> TextureFitObjects = new List<Tools.TextureFitInformation>();

        public ElasticityCalculationWindow(Sample usedSample)
        {
            InitializeComponent();

            this.ActSample = usedSample;

            this.PrepareREKS();
            //PrepareStrainFit();
            this.LoadData();

            this.PrepareAnIsoPlot();

            AnIsoPlotDirectionSelection.SelectedIndex = 0;
            AnIsoParamSelector.SelectedIndex = 0;
            AnIsoModelSelector.SelectedIndex = 2;
            AnIsoColorSelector.SelectedIndex = 0;
        }

        private void PrepareREKS()
        {
            for (int n = 0; n < this.ActSample.CrystalData.Count; n++)
            {
                this.ActSample.VoigtTensorData[n].DiffractionConstants = ActSample.DiffractionConstants[n];
                this.ActSample.ReussTensorData[n].DiffractionConstants = ActSample.DiffractionConstants[n];
                this.ActSample.HillTensorData[n].DiffractionConstants = ActSample.DiffractionConstants[n];
                this.ActSample.KroenerTensorData[n].DiffractionConstants = ActSample.DiffractionConstants[n];
                this.ActSample.DeWittTensorData[n].DiffractionConstants = ActSample.DiffractionConstants[n];
                this.ActSample.GeometricHillTensorData[n].DiffractionConstants = ActSample.DiffractionConstants[n];

                this.ActSample.VoigtTensorData[n].DiffractionConstantsTexture = ActSample.DiffractionConstantsTexture[n];
                this.ActSample.ReussTensorData[n].DiffractionConstantsTexture = ActSample.DiffractionConstantsTexture[n];
                this.ActSample.HillTensorData[n].DiffractionConstantsTexture = ActSample.DiffractionConstantsTexture[n];
                this.ActSample.KroenerTensorData[n].DiffractionConstantsTexture = ActSample.DiffractionConstantsTexture[n];
                this.ActSample.DeWittTensorData[n].DiffractionConstantsTexture = ActSample.DiffractionConstantsTexture[n];
                this.ActSample.GeometricHillTensorData[n].DiffractionConstantsTexture = ActSample.DiffractionConstantsTexture[n];

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
                    this.ActSample.VoigtTensorData[n].ODF.FitUpdated += this.TextureFitUpdated;
                    this.ActSample.VoigtTensorData[n].ODF.FitFinished += this.TextureFitFinished;
                    this.ActSample.ReussTensorData[n].ODF.FitStarted += this.TextureFitStarted;
                    this.ActSample.ReussTensorData[n].ODF.FitUpdated += this.TextureFitUpdated;
                    this.ActSample.ReussTensorData[n].ODF.FitFinished += this.TextureFitFinished;
                    this.ActSample.HillTensorData[n].ODF.FitStarted += this.TextureFitStarted;
                    this.ActSample.HillTensorData[n].ODF.FitUpdated += this.TextureFitUpdated;
                    this.ActSample.HillTensorData[n].ODF.FitFinished += this.TextureFitFinished;
                    this.ActSample.GeometricHillTensorData[n].ODF.FitStarted += this.TextureFitStarted;
                    this.ActSample.GeometricHillTensorData[n].ODF.FitUpdated += this.TextureFitUpdated;
                    this.ActSample.GeometricHillTensorData[n].ODF.FitFinished += this.TextureFitFinished;
                    this.ActSample.KroenerTensorData[n].ODF.FitStarted += this.TextureFitStarted;
                    this.ActSample.KroenerTensorData[n].ODF.FitUpdated += this.TextureFitUpdated;
                    this.ActSample.KroenerTensorData[n].ODF.FitFinished += this.TextureFitFinished;
                    this.ActSample.DeWittTensorData[n].ODF.FitStarted += this.TextureFitStarted;
                    this.ActSample.DeWittTensorData[n].ODF.FitUpdated += this.TextureFitUpdated;
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

        private void PrepareStrainFit()
        {
            for (int n = 0; n < this.ActSample.CrystalData.Count; n++)
            {
                //TODO: Achtung hier wegen des Strain Fits, aber es ist sehr wahrscheinlich, dass hier Fehler passieren
                this.ActSample.VoigtTensorData[n].SetPeakStressAssociation(this.ActSample);
                this.ActSample.VoigtTensorData[n].SetStrainDataReflexYield();
                this.ActSample.ReussTensorData[n].SetPeakStressAssociation(this.ActSample);
                this.ActSample.ReussTensorData[n].SetStrainDataReflexYield();
                this.ActSample.HillTensorData[n].SetPeakStressAssociation(this.ActSample);
                this.ActSample.HillTensorData[n].SetStrainDataReflexYield();
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
            ModelItem1.Content = "Costom Data";
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
            ComboBoxItem REKItem3 = new ComboBoxItem();
            REKItem3.Content = "Strain";
            ComboBoxItem REKItem4 = new ComboBoxItem();
            REKItem4.Content = "Textured";

            this.REKSwitchBox.Items.Add(REKItem1);
            this.REKSwitchBox.Items.Add(REKItem2);
            this.REKSwitchBox.Items.Add(REKItem3);
            this.REKSwitchBox.Items.Add(REKItem4);

            this.REKSwitchBox.SelectedIndex = 0;

            ComboBoxItem StiffnessComlplianceItem1 = new ComboBoxItem();
            StiffnessComlplianceItem1.Content = "Compliance tensor";
            ComboBoxItem StiffnessComlplianceItem2 = new ComboBoxItem();
            StiffnessComlplianceItem2.Content = "Stiffness tensor";

            this.StiffnessComlplianceSwitchBox.Items.Add(StiffnessComlplianceItem1);
            this.StiffnessComlplianceSwitchBox.Items.Add(StiffnessComlplianceItem2);

            this.StiffnessComlplianceSwitchBox.SelectedIndex = 0;

            this.SetTensorData();
            this.SetTransitionView();

            //this.TextureFittingPoolList.ItemsSource = this.TextureFitObjects;

            TextEventsActive = true;
        }

        private ElasticityTensors GetSelectedTensor()
        {
            ElasticityTensors ret = null;

            switch (this.ModelSwitchBox.SelectedIndex)
            {
                case 0:
                    ret = this.ActSample.VoigtTensorData[this.PhaseSwitchBox.SelectedIndex];
                    break;
                case 1:
                    ret = this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex];
                    break;
                case 2:
                    ret = this.ActSample.HillTensorData[this.PhaseSwitchBox.SelectedIndex];
                    break;
                case 3:
                    ret = this.ActSample.KroenerTensorData[this.PhaseSwitchBox.SelectedIndex];
                    break;
                case 4:
                    ret = this.ActSample.DeWittTensorData[this.PhaseSwitchBox.SelectedIndex];
                    break;
                case 5:
                    ret = this.ActSample.GeometricHillTensorData[this.PhaseSwitchBox.SelectedIndex];
                    break;
                default:
                    ret = this.ActSample.VoigtTensorData[this.PhaseSwitchBox.SelectedIndex];
                    break;
            }

            return ret;
        }

        private void SetTensorData()
        {
            Stress.Microsopic.ElasticityTensors UsedTensor = this.GetSelectedTensor();

            if (Convert.ToBoolean(this.FixAnisotropyCheckBox.IsChecked))
            {
                UsedTensor.FixedAnIsotropy = true;
            }
            else
            {
                UsedTensor.FixedAnIsotropy = false;
            }

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

            if (this.REKSwitchBox.SelectedIndex != 3)
            {
                UsedTensor.SetAverageParametersFit(false);
            }
            else
            {
                UsedTensor.SetAverageParametersFit(true);
            }

            this.AEModulusLabel.Content = UsedTensor.AveragedEModul.ToString("F3");
            this.ANuLabel.Content = UsedTensor.AveragedNu.ToString("F3");
            this.AShearModulusLabel.Content = UsedTensor.AveragedSchearModul.ToString("F3");
            this.ABulkModulusLabel.Content = UsedTensor.AveragedBulkModul.ToString("e3");
            this.AEModulusFitLabel.Content = UsedTensor.AveragedEModulFit.ToString("F3");
            this.ANuFitLabel.Content = UsedTensor.AveragedNuFit.ToString("F3");
            this.AShearModulusFitLabel.Content = UsedTensor.AveragedSchearModulFit.ToString("F3");
            this.ABulkModulusFitLabel.Content = UsedTensor.AveragedBulkModulFit.ToString("e3");

            this.UniversalAnisotropyLabel.Content = UsedTensor.GetUniversalAnisotropy().ToString("F3");
            this.EquivalentAnistropyLabel.Content = UsedTensor.GetZenerEquivalentAnisotropy().ToString("F3");

            this.LogEukAnisitropyLabel.Content = this.ActSample.GetLogEukAnisitropyCubic(this.PhaseSwitchBox.SelectedIndex).ToString("F3");
            this.FixedAnisotropy.Text = UsedTensor.AnIsotropy.ToString("F3");
            this.ZehnderAnisitropyLabel.Content = UsedTensor.GetZehnderAnisotropy.ToString("F3");

            double Chi2Tmp = 0.0;
            if (UsedTensor.Symmetry == "cubic")
            {
                if (this.REKSwitchBox.SelectedIndex == 3)
                {
                    Chi2Tmp = UsedTensor.GetFittingChi2Cubic(this.ModelSwitchBox.SelectedIndex, Compliance, true);
                }
                else
                {
                    Chi2Tmp = UsedTensor.GetFittingChi2Cubic(this.ModelSwitchBox.SelectedIndex, Compliance, false);
                }
            }
            else
            {
                if (this.REKSwitchBox.SelectedIndex == 3)
                {
                    Chi2Tmp = UsedTensor.GetFittingChi2Hexagonal(this.ModelSwitchBox.SelectedIndex, Compliance, true);
                }
                else
                {
                    Chi2Tmp = UsedTensor.GetFittingChi2Hexagonal(this.ModelSwitchBox.SelectedIndex, Compliance, false);
                }
            }
            this.Chi2TensorFitLabel.Content = Chi2Tmp.ToString("F3");

            #endregion

            #region REKData

            if (this.REKSwitchBox.SelectedIndex != 3)
            {
                this.REKClassicCalculationList.ItemsSource = UsedTensor.DiffractionConstants;
            }
            else
            {
                this.REKClassicCalculationList.ItemsSource = UsedTensor.DiffractionConstantsTexture;
            }

            this.REKMatrixCalculationList.ItemsSource = CalculatedREK;

            #endregion
        }

        private void SwitchBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.TextEventsActive)
            {
                this.TextEventsActive = false;
                this.SetTensorData();
                this.TextEventsActive = true;
            }
        }

        private void RefitSEC(int selectedPhase, int fittingMode, int modelIndex, int complianceStiffness)
        {
            switch (modelIndex)
            {
                case 0:
                    if (fittingMode == 0)
                    {
                        this.ActSample.VoigtTensorData[selectedPhase].FitVoigt(true);
                    }
                    else if (fittingMode == 2)
                    {
                        this.ActSample.VoigtTensorData[selectedPhase].FitVoigtStrain(true);
                    }
                    else
                    {
                        this.ActSample.VoigtTensorData[selectedPhase].FitVoigt(false);
                    }
                    break;
                case 1:
                    if (fittingMode == 0)
                    {
                        this.ActSample.ReussTensorData[selectedPhase].FitReuss(true);
                    }
                    else if (fittingMode == 1)
                    {
                        this.ActSample.ReussTensorData[selectedPhase].FitReuss(false);
                    }
                    else if (fittingMode == 2)
                    {
                        this.ActSample.ReussTensorData[selectedPhase].FitReussStrain(true);
                    }
                    else if (fittingMode == 3)
                    {
                        if (CalScec.Properties.Settings.Default.ActivateDECTextureWeighting)
                        {
                            this.ActSample.ReussTensorData[selectedPhase].ActivateDECMRDWeighting();
                        }
                        else
                        {
                            this.ActSample.ReussTensorData[selectedPhase].DeactivateDECMRDWeighting();
                        }

                        this.ActSample.ReussTensorData[selectedPhase].FitReussTextured(true);
                    }
                    else
                    {
                        this.ActSample.ReussTensorData[selectedPhase].FitReuss(true);
                    }
                    break;
                case 2:
                    if (fittingMode == 0)
                    {
                        this.ActSample.HillTensorData[selectedPhase].FitHill(true);
                    }
                    else if (fittingMode == 1)
                    {
                        this.ActSample.HillTensorData[selectedPhase].FitHill(false);
                    }
                    else if (fittingMode == 3)
                    {
                        if (CalScec.Properties.Settings.Default.ActivateDECTextureWeighting)
                        {
                            this.ActSample.HillTensorData[selectedPhase].ActivateDECMRDWeighting();
                        }
                        else
                        {
                            this.ActSample.HillTensorData[selectedPhase].DeactivateDECMRDWeighting();
                        }

                        this.ActSample.HillTensorData[selectedPhase].FitHillTextured(true);
                    }
                    else
                    {
                        this.ActSample.HillTensorData[selectedPhase].FitHill(true);
                    }
                    break;
                case 3:
                    bool SC = false;
                    if (complianceStiffness == 1)
                    {
                        SC = true;
                    }
                    if (fittingMode == 0)
                    {
                        this.ActSample.KroenerTensorData[selectedPhase].FitKroener(true, SC);
                    }
                    else if (fittingMode == 1)
                    {
                        this.ActSample.KroenerTensorData[selectedPhase].FitKroener(false, SC);
                    }
                    else if (fittingMode == 3)
                    {
                        if (CalScec.Properties.Settings.Default.ActivateDECTextureWeighting)
                        {
                            this.ActSample.KroenerTensorData[selectedPhase].ActivateDECMRDWeighting();
                        }
                        else
                        {
                            this.ActSample.KroenerTensorData[selectedPhase].DeactivateDECMRDWeighting();
                        }

                        this.ActSample.KroenerTensorData[selectedPhase].FitKroenerTextured(true, SC);
                    }
                    else
                    {
                        this.ActSample.KroenerTensorData[selectedPhase].FitKroener(true, SC);
                    }
                    break;
                case 4:
                    bool SC1 = false;
                    if (complianceStiffness == 1)
                    {
                        SC1 = true;
                    }
                    if (fittingMode == 0)
                    {
                        this.ActSample.DeWittTensorData[selectedPhase].FitDeWitt(true, SC1);
                    }
                    else if (fittingMode == 1)
                    {
                        this.ActSample.DeWittTensorData[selectedPhase].FitDeWitt(false, SC1);
                    }
                    else if (fittingMode == 3)
                    {
                        if (CalScec.Properties.Settings.Default.ActivateDECTextureWeighting)
                        {
                            this.ActSample.DeWittTensorData[selectedPhase].ActivateDECMRDWeighting();
                        }
                        else
                        {
                            this.ActSample.DeWittTensorData[selectedPhase].DeactivateDECMRDWeighting();
                        }

                        this.ActSample.DeWittTensorData[selectedPhase].FitDeWittTextured(true, SC1);
                    }
                    else
                    {
                        this.ActSample.DeWittTensorData[selectedPhase].FitDeWitt(true, SC1);
                    }
                    break;
                case 5:
                    if (fittingMode == 0)
                    {
                        this.ActSample.GeometricHillTensorData[selectedPhase].FitGeometricHill(true);
                    }
                    else if (fittingMode == 1)
                    {
                        this.ActSample.GeometricHillTensorData[selectedPhase].FitGeometricHill(false);
                    }
                    else if (fittingMode == 3)
                    {
                        if (CalScec.Properties.Settings.Default.ActivateDECTextureWeighting)
                        {
                            this.ActSample.GeometricHillTensorData[selectedPhase].ActivateDECMRDWeighting();
                        }
                        else
                        {
                            this.ActSample.GeometricHillTensorData[selectedPhase].DeactivateDECMRDWeighting();
                        }

                        this.ActSample.GeometricHillTensorData[selectedPhase].FitGeometricHillTextured(true);
                    }
                    else
                    {
                        this.ActSample.GeometricHillTensorData[selectedPhase].FitGeometricHill(true);
                    }
                    break;
                default:
                    if (fittingMode == 1)
                    {
                        this.ActSample.VoigtTensorData[selectedPhase].FitReuss(false);
                    }
                    else
                    {
                        this.ActSample.VoigtTensorData[selectedPhase].FitReuss(true);
                    }
                    break;
            }
        }

        private void RefitConstants_Click(object sender, RoutedEventArgs e)
        {
            this.TextEventsActive = false;

            RefitSEC(this.PhaseSwitchBox.SelectedIndex, REKSwitchBox.SelectedIndex, this.ModelSwitchBox.SelectedIndex, this.StiffnessComlplianceSwitchBox.SelectedIndex);

            this.SetTensorData();

            this.TextEventsActive = true;
        }

        private void Tensor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.TextEventsActive)
            {
                Stress.Microsopic.ElasticityTensors UsedTensor = this.GetSelectedTensor();

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

                    this.ActSample.VoigtTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.SetPeakStressAssociation(this.ActSample);
                    this.ActSample.VoigtTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.SetStrainData();
                    break;
                case 1:
                    this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.BaseTensor = this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex].Clone() as ElasticityTensors;
                    this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.BaseTensor.ODF = null;

                    this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.SetPeakStressAssociation(this.ActSample);
                    this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.SetStrainData();
                    break;
                case 2:
                    this.ActSample.HillTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.BaseTensor = this.ActSample.HillTensorData[this.PhaseSwitchBox.SelectedIndex].Clone() as ElasticityTensors;
                    this.ActSample.HillTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.BaseTensor.ODF = null;

                    this.ActSample.HillTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.SetPeakStressAssociation(this.ActSample);
                    this.ActSample.HillTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.SetStrainData();
                    break;
                case 3:
                    this.ActSample.KroenerTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.BaseTensor = this.ActSample.KroenerTensorData[this.PhaseSwitchBox.SelectedIndex].Clone() as ElasticityTensors;
                    this.ActSample.KroenerTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.BaseTensor.ODF = null;

                    this.ActSample.KroenerTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.SetPeakStressAssociation(this.ActSample);
                    this.ActSample.KroenerTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.SetStrainData();
                    break;
                case 4:
                    this.ActSample.DeWittTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.BaseTensor = this.ActSample.DeWittTensorData[this.PhaseSwitchBox.SelectedIndex].Clone() as ElasticityTensors;
                    this.ActSample.DeWittTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.BaseTensor.ODF = null;

                    this.ActSample.DeWittTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.SetPeakStressAssociation(this.ActSample);
                    this.ActSample.DeWittTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.SetStrainData();
                    break;
                case 5:
                    this.ActSample.GeometricHillTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.BaseTensor = this.ActSample.HillTensorData[this.PhaseSwitchBox.SelectedIndex].Clone() as ElasticityTensors;
                    this.ActSample.GeometricHillTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.BaseTensor.ODF = null;

                    this.ActSample.GeometricHillTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.SetPeakStressAssociation(this.ActSample);
                    this.ActSample.GeometricHillTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.SetStrainData();
                    break;
                default:
                    this.ActSample.VoigtTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.BaseTensor = this.ActSample.VoigtTensorData[this.PhaseSwitchBox.SelectedIndex].Clone() as ElasticityTensors;
                    this.ActSample.VoigtTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.BaseTensor.ODF = null;

                    this.ActSample.VoigtTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.SetPeakStressAssociation(this.ActSample);
                    this.ActSample.VoigtTensorData[this.PhaseSwitchBox.SelectedIndex].ODF.SetStrainData();
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

        private void TextureFitUpdated(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            FitTextureUpdateDelegate FittingDelegate = TextureFitUpdatedHandler;

            Dispatcher.Invoke(FittingDelegate, sender as Texture.OrientationDistributionFunction);
        }

        private void TextureFitFinished(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            FitTextureUpdateDelegate FittingDelegate = TextureFitFinishedHandler;

            Dispatcher.Invoke(FittingDelegate, sender as Texture.OrientationDistributionFunction);
        }


        private void TextureFitStartedHandler(Texture.OrientationDistributionFunction oDF)
        {
            oDF.FitDisplayInfo = new Tools.TextureFitInformation(oDF.fittingModel);
            oDF.FitDisplayInfo.iD = this.textureId;
            this.textureId++;
            TextureFitObjects.Add(oDF.FitDisplayInfo);

            //this.TextureFittingPoolList.Items.Refresh();

            //if (this.TextureProgress.IsIndeterminate)
            //{
            //    this.TextureProgress.ToolTip = "Fitting " + Convert.ToString(this.TextureFitObjects.Count) + " Tensors";
            //}
            //else
            //{
            //    this.TextureProgress.IsIndeterminate = true;
            //    this.TextureProgress.ToolTip = "Fitting " + Convert.ToString(this.TextureFitObjects.Count) + " Tensors";
            //}
        }

        private void TextureFitUpdatedHandler(Texture.OrientationDistributionFunction oDF)
        {
            //for(int n = 0; n < TextureFitObjects.Count; n++)
            //{
            //    if(TextureFitObjects[n].iD == oDF.FitDisplayInfo.iD)
            //    {
            //        TextureFitObjects[n].LMATrial++;
            //    }
            //}
            //this.TextureFittingPoolList.Items.Refresh();
            //this.SetTensorData();
        }

        private void TextureFitFinishedHandler(Texture.OrientationDistributionFunction oDF)
        {
            for (int n = 0; n < TextureFitObjects.Count; n++)
            {
                if (oDF.FitDisplayInfo.iD == TextureFitObjects[n].iD)
                {
                    TextureFitObjects.RemoveAt(n);
                    break;
                }
            }

            //this.TextureFittingPoolList.Items.Refresh();

            //this.TextureProgress.ToolTip = "Fitting " + Convert.ToString(this.TextureFitObjects.Count) + " Tensors";

            //if (this.TextureFitObjects.Count == 0)
            //{
            //    this.TextureProgress.IsIndeterminate = false;
            //}
        }

        #endregion

        #endregion

        #region Anisotropy

        private Tools.PlottingWindow AniIsoPlotWindow = new Tools.PlottingWindow();
        public OxyPlot.PlotModel AnIsoPlotModel = new OxyPlot.PlotModel();
        OxyPlot.Axes.AngleAxis AnIsoAngleAxis = new OxyPlot.Axes.AngleAxis();
        OxyPlot.Axes.MagnitudeAxis AnIsoValueAxis = new OxyPlot.Axes.MagnitudeAxis();

        private void PrepareAnIsoPlot()
        {
            AnIsoPlotModel.PlotType = OxyPlot.PlotType.Polar;

            AnIsoPlotModel.LegendBorder = OxyPlot.OxyColors.Black;
            AnIsoPlotModel.LegendItemAlignment = OxyPlot.HorizontalAlignment.Left;
            AnIsoPlotModel.LegendTitle = "Used modeling";
            AnIsoPlotModel.LegendFontSize = 28;
            AnIsoPlotModel.LegendTitleFontSize = 30;

            AnIsoAngleAxis.MajorGridlineStyle = OxyPlot.LineStyle.Dash;
            AnIsoAngleAxis.MinorGridlineStyle = OxyPlot.LineStyle.Dot;
            AnIsoAngleAxis.FractionUnit = 2 * Math.PI;
            AnIsoAngleAxis.FractionUnitSymbol = "π";
            AnIsoAngleAxis.FontSize = 28;
            AnIsoAngleAxis.TitleFontSize = 30;
            AnIsoAngleAxis.IsAxisVisible = false;

            AnIsoValueAxis.MajorGridlineStyle = OxyPlot.LineStyle.Dash;
            AnIsoValueAxis.MinorGridlineStyle = OxyPlot.LineStyle.Dot;
            //AnIsoValueAxis.FractionUnitSymbol = "GPa";
            //AnIsoAngleAxis.Minimum = 0.0;
            AnIsoValueAxis.FractionUnit = 1000;
            AnIsoValueAxis.FormatAsFractions = true;
            AnIsoValueAxis.Title = "GPa";
            AnIsoValueAxis.FontSize = 28;
            AnIsoValueAxis.TitleFontSize = 30;
            AnIsoValueAxis.MinimumMajorStep = 5000;


            AnIsoPlotModel.Axes.Add(AnIsoAngleAxis);
            AnIsoPlotModel.Axes.Add(AnIsoValueAxis);

            AniIsoPlotWindow.MainPlot.Model = AnIsoPlotModel;
        }

        private void SetAnIsoData(ElasticityTensors usedTensor, int h, int k, int l, OxyPlot.Series.LineSeries parameterSeries, MathNet.Numerics.LinearAlgebra.Vector<double> angleDirection, double angleCorrection)
        {
            MathNet.Numerics.LinearAlgebra.Vector<double> angleVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense(3, 0.0);

            angleVector[0] = h;
            angleVector[1] = k;
            angleVector[2] = l;



            double angle = Math.Acos((angleDirection * angleVector) / (angleDirection.L2Norm() * angleVector.L2Norm()));
            angle += angleCorrection;

            double s1 = 0.0;
            double s2 = 0.0;

            DataManagment.CrystalData.HKLReflex reflex = new DataManagment.CrystalData.HKLReflex(h, k, l, 1.0);

            if (usedTensor.Symmetry == "hexagonal")
            {
                switch (AnIsoModelSelector.SelectedIndex)
                {
                    case 0:
                        s1 = usedTensor.S1VoigtCubic();
                        s2 = usedTensor.HS2VoigtCubic();
                        break;
                    case 1:
                        s1 = usedTensor.S1ReussHexagonal(reflex);
                        s2 = usedTensor.HS2ReussHexagonal(reflex);
                        break;
                    case 2:
                        s1 = usedTensor.S1HillHexagonal(reflex);
                        s2 = usedTensor.HS2HillHexagonal(reflex);
                        break;
                    case 3:
                        s1 = usedTensor.S1ReussHexagonal(reflex);
                        s2 = usedTensor.HS2ReussHexagonal(reflex);
                        break;
                    case 5:
                        s1 = usedTensor.S1GeometricHillHexagonal(reflex);
                        s2 = usedTensor.HS2GeometricHillHexagonal(reflex);
                        break;
                    default:
                        s1 = usedTensor.S1ReussHexagonal(reflex);
                        s2 = usedTensor.HS2ReussHexagonal(reflex);
                        break;
                }
                //double orientationParameter = usedTensor.OrientationParameterH(h, k, 0);

                //s1 = usedTensor.DirectionalS1ReussHexagonal(orientationParameter);
                //s2 = usedTensor.DirectionalS2ReussHexagonal(orientationParameter);
            }
            else
            {
                switch (AnIsoModelSelector.SelectedIndex)
                {
                    case 0:
                        s1 = usedTensor.S1VoigtCubic();
                        s2 = usedTensor.HS2VoigtCubic();
                        break;
                    case 1:
                        s1 = usedTensor.S1ReussCubic(reflex);
                        s2 = usedTensor.HS2ReussCubic(reflex);
                        break;
                    case 2:
                        s1 = usedTensor.S1HillCubic(reflex);
                        s2 = usedTensor.HS2HillCubic(reflex);
                        break;
                    case 3:
                        s1 = usedTensor.S1KroenerCubicComplianceAnIso();
                        s2 = usedTensor.HS2KroenerCubicComplianceAnIso();
                        break;
                    case 4:
                        s1 = usedTensor.S1DeWittCubicComplianceAnIso(reflex);
                        s2 = usedTensor.HS2DeWittCubicComplianceAnIso(reflex);
                        break;
                    case 5:
                        s1 = usedTensor.S1GeometricHillCubic(reflex);
                        s2 = usedTensor.HS2GeometricHillCubic(reflex);
                        break;
                    default:
                        s1 = usedTensor.S1ReussCubic(reflex);
                        s2 = usedTensor.HS2ReussCubic(reflex);
                        break;
                }
            }

            REK rekTmp = new REK(usedTensor.GetPhaseInformation, reflex);

            rekTmp.ClassicFittingFunction.Constant = s1;
            rekTmp.ClassicFittingFunction.Aclivity = s2;

            OxyPlot.DataPoint plotPoint = new OxyPlot.DataPoint(0, 0);

            switch (this.AnIsoParamSelector.SelectedIndex)
            {
                case 0:
                    plotPoint = new OxyPlot.DataPoint(rekTmp.ClassicEModulus, angle);
                    break;
                case 1:
                    plotPoint = new OxyPlot.DataPoint(rekTmp.ClassicShearModulus, angle);
                    break;
                case 2:
                    plotPoint = new OxyPlot.DataPoint(rekTmp.ClassicTransverseContraction, angle);
                    break;
                case 3:
                    plotPoint = new OxyPlot.DataPoint(rekTmp.ClassicBulkModulus, angle);
                    break;
                default:
                    plotPoint = new OxyPlot.DataPoint(rekTmp.ClassicEModulus, angle);
                    break;
            }

            parameterSeries.Points.Add(plotPoint);
        }

        private void SetAnIsoPlot()
        {
            MathNet.Numerics.LinearAlgebra.Vector<double> angleDirection = MathNet.Numerics.LinearAlgebra.CreateVector.Dense(3, 0.0);
            MathNet.Numerics.LinearAlgebra.Vector<double> plainDirection = MathNet.Numerics.LinearAlgebra.CreateVector.Dense(3, 0.0);

            ElasticityTensors usedTensor = this.GetSelectedTensor();

            switch (AnIsoPlotDirectionSelection.SelectedIndex)
            {
                case 0:
                    angleDirection[0] = 1;
                    angleDirection[1] = 0;
                    angleDirection[2] = 0;

                    plainDirection[0] = 0;
                    plainDirection[1] = 1;
                    plainDirection[2] = 0;
                    break;
                case 1:
                    angleDirection[0] = 1;
                    angleDirection[1] = 0;
                    angleDirection[2] = 0;

                    plainDirection[0] = 0;
                    plainDirection[1] = 0;
                    plainDirection[2] = 1;
                    break;
                case 2:
                    angleDirection[0] = 0;
                    angleDirection[1] = 1;
                    angleDirection[2] = 0;

                    plainDirection[0] = 0;
                    plainDirection[1] = 0;
                    plainDirection[2] = 1;
                    break;
                case 4:
                    angleDirection[0] = 1;
                    angleDirection[1] = 0;
                    angleDirection[2] = 0;

                    plainDirection[0] = 0;
                    plainDirection[1] = 1;
                    plainDirection[2] = 1;
                    break;
                case 5:
                    angleDirection[0] = 0;
                    angleDirection[1] = 1;
                    angleDirection[2] = 0;

                    plainDirection[0] = 1;
                    plainDirection[1] = 0;
                    plainDirection[2] = 1;
                    break;
                case 6:
                    angleDirection[0] = 0;
                    angleDirection[1] = 0;
                    angleDirection[2] = 1;

                    plainDirection[0] = 1;
                    plainDirection[1] = 1;
                    plainDirection[2] = 0;
                    break;
                case 8:
                    angleDirection[0] = 1;
                    angleDirection[1] = 1;
                    angleDirection[2] = 0;

                    plainDirection[0] = 1;
                    plainDirection[1] = -1;
                    plainDirection[2] = 0;
                    break;
                case 9:
                    angleDirection[0] = 1;
                    angleDirection[1] = 0;
                    angleDirection[2] = 1;

                    plainDirection[0] = 1;
                    plainDirection[1] = 0;
                    plainDirection[2] = -1;
                    break;
                case 10:
                    angleDirection[0] = 0;
                    angleDirection[1] = 1;
                    angleDirection[2] = 1;

                    plainDirection[0] = 0;
                    plainDirection[1] = 1;
                    plainDirection[2] = -1;
                    break;
                case 12:
                    angleDirection[0] = 1;
                    angleDirection[1] = 1;
                    angleDirection[2] = 1;

                    plainDirection[0] = 1;
                    plainDirection[1] = -1;
                    plainDirection[2] = 0;
                    break;
                case 13:
                    angleDirection[0] = 1;
                    angleDirection[1] = 1;
                    angleDirection[2] = 1;

                    plainDirection[0] = 1;
                    plainDirection[1] = 0;
                    plainDirection[2] = -1;
                    break;
                case 14:
                    angleDirection[0] = 1;
                    angleDirection[1] = 1;
                    angleDirection[2] = 1;

                    plainDirection[0] = 0;
                    plainDirection[1] = 1;
                    plainDirection[2] = -1;
                    break;
                default:
                    angleDirection[0] = 1;
                    angleDirection[1] = 0;
                    angleDirection[2] = 0;

                    plainDirection[0] = 0;
                    plainDirection[1] = 1;
                    plainDirection[2] = 0;
                    break;
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> normalDirection = MathNet.Numerics.LinearAlgebra.CreateVector.Dense(3, 0.0);
            normalDirection[0] = angleDirection[1] * plainDirection[2] - angleDirection[2] * plainDirection[1];
            normalDirection[1] = -angleDirection[0] * plainDirection[2] + angleDirection[2] * plainDirection[0];
            normalDirection[2] = angleDirection[0] * plainDirection[1] - angleDirection[1] * plainDirection[0];

            double viewCheck = angleDirection * plainDirection;

            if (usedTensor != null && viewCheck == 0)
            {
                OxyPlot.Series.LineSeries parameterSeries = new OxyPlot.Series.LineSeries();
                parameterSeries.LineStyle = OxyPlot.LineStyle.None;
                parameterSeries.MarkerType = OxyPlot.MarkerType.Circle;
                parameterSeries.MarkerSize = 2;

                #region Title

                parameterSeries.Title = "Constants: ";
                switch (ModelSwitchBox.SelectedIndex)
                {
                    case 0:
                        parameterSeries.Title += "Voigt";
                        break;
                    case 1:
                        parameterSeries.Title += "Reuss";
                        break;
                    case 2:
                        parameterSeries.Title += "Hill";
                        break;
                    case 3:
                        parameterSeries.Title += "Kroener";
                        break;
                    case 4:
                        parameterSeries.Title += "De Wit";
                        break;
                    case 5:
                        parameterSeries.Title += "Matthies";
                        break;
                    default:
                        parameterSeries.Title += "Reuss";
                        break;
                }
                //switch (ModelSwitchBox.SelectedIndex)
                //{
                //    case 0:
                //        parameterSeries.Title += "Voigt; Modeling: ";
                //        break;
                //    case 1:
                //        parameterSeries.Title += "Reuss; Modeling: ";
                //        break;
                //    case 2:
                //        parameterSeries.Title += "Hill; Modeling: ";
                //        break;
                //    case 3:
                //        parameterSeries.Title += "Kroener; Modeling: ";
                //        break;
                //    case 4:
                //        parameterSeries.Title += "De Wit; Modeling: ";
                //        break;
                //    case 5:
                //        parameterSeries.Title += "Matthies; Modeling: ";
                //        break;
                //    default:
                //        parameterSeries.Title += "Reuss; Modeling: ";
                //        break;
                //}

                //switch (AnIsoModelSelector.SelectedIndex)
                //{
                //    case 0:
                //        parameterSeries.Title += "Voigt";
                //        break;
                //    case 1:
                //        parameterSeries.Title += "Reuss";
                //        break;
                //    case 2:
                //        parameterSeries.Title += "Hill";
                //        break;
                //    case 3:
                //        parameterSeries.Title += "Kroener";
                //        break;
                //    case 4:
                //        parameterSeries.Title += "De Wit";
                //        break;
                //    case 5:
                //        parameterSeries.Title += "Matthies";
                //        break;
                //    default:
                //        parameterSeries.Title += "Reuss";
                //        break;
                //}

                #endregion

                //parameterSeries.Title = "Zener Anisotropy: " + usedTensor.GetZehnderAnisotropy.ToString("F3");

                #region Color

                switch (AnIsoColorSelector.SelectedIndex)
                {
                    case 0:
                        parameterSeries.Color = OxyPlot.OxyColors.Black;
                        parameterSeries.MarkerFill = OxyPlot.OxyColors.Black;
                        break;
                    case 1:
                        parameterSeries.Color = OxyPlot.OxyColors.DarkGray;
                        parameterSeries.MarkerFill = OxyPlot.OxyColors.DarkGray;
                        break;
                    case 2:
                        parameterSeries.Color = OxyPlot.OxyColors.DarkBlue;
                        parameterSeries.MarkerFill = OxyPlot.OxyColors.DarkBlue;
                        break;
                    case 3:
                        parameterSeries.Color = OxyPlot.OxyColors.Blue;
                        parameterSeries.MarkerFill = OxyPlot.OxyColors.Blue;
                        break;
                    case 4:
                        parameterSeries.Color = OxyPlot.OxyColors.DarkGreen;
                        parameterSeries.MarkerFill = OxyPlot.OxyColors.DarkGreen;
                        break;
                    case 5:
                        parameterSeries.Color = OxyPlot.OxyColors.Green;
                        parameterSeries.MarkerFill = OxyPlot.OxyColors.Green;
                        break;
                    case 6:
                        parameterSeries.Color = OxyPlot.OxyColors.DarkRed;
                        parameterSeries.MarkerFill = OxyPlot.OxyColors.DarkRed;
                        break;
                    case 7:
                        parameterSeries.Color = OxyPlot.OxyColors.Red;
                        parameterSeries.MarkerFill = OxyPlot.OxyColors.Red;
                        break;
                    case 8:
                        parameterSeries.Color = OxyPlot.OxyColors.Orange;
                        parameterSeries.MarkerFill = OxyPlot.OxyColors.Orange;
                        break;
                    case 9:
                        parameterSeries.Color = OxyPlot.OxyColors.Yellow;
                        parameterSeries.MarkerFill = OxyPlot.OxyColors.Yellow;
                        break;
                    default:
                        parameterSeries.Color = OxyPlot.OxyColors.Black;
                        parameterSeries.MarkerFill = OxyPlot.OxyColors.Black;
                        break;
                }

                #endregion

                if (this.AnIsoPlotModel.Annotations.Count == 0)
                {
                    OxyPlot.Annotations.LineAnnotation mainViewAxes = new OxyPlot.Annotations.LineAnnotation();
                    mainViewAxes.LineStyle = OxyPlot.LineStyle.Solid;
                    mainViewAxes.Color = OxyPlot.OxyColors.Black;
                    mainViewAxes.MinimumX = 0.0;
                    mainViewAxes.MaximumX = 250000;
                    mainViewAxes.Type = OxyPlot.Annotations.LineAnnotationType.Horizontal;
                    mainViewAxes.Y = 0.0;
                    mainViewAxes.StrokeThickness = 2;
                    mainViewAxes.Text = "[" + angleDirection[0].ToString() + " " + angleDirection[1].ToString() + " " + angleDirection[2].ToString() + "]";
                    mainViewAxes.FontSize = 25;

                    OxyPlot.Annotations.LineAnnotation secondaryViewAxes = new OxyPlot.Annotations.LineAnnotation();
                    secondaryViewAxes.LineStyle = OxyPlot.LineStyle.Solid;
                    secondaryViewAxes.Color = OxyPlot.OxyColors.Black;
                    secondaryViewAxes.MinimumX = 0.0;
                    secondaryViewAxes.MaximumX = 250000;
                    secondaryViewAxes.Type = OxyPlot.Annotations.LineAnnotationType.Horizontal;
                    secondaryViewAxes.Y = Math.PI / 2.0;
                    secondaryViewAxes.StrokeThickness = 2;
                    secondaryViewAxes.Text = "[" + plainDirection[0].ToString() + " " + plainDirection[1].ToString() + " " + plainDirection[2].ToString() + "]";
                    secondaryViewAxes.FontSize = 25;

                    AnIsoPlotModel.Annotations.Add(mainViewAxes);
                    AnIsoPlotModel.Annotations.Add(secondaryViewAxes);
                }

                #region

                double param1Count = 0;
                double param2Count = 0;
                for (int n = 0; n < 3; n++)
                {
                    if (angleDirection[n] != 0)
                    {
                        param1Count++;
                    }
                    if (plainDirection[n] != 0)
                    {
                        param2Count++;
                    }
                }

                for (int h = -50; h < 50; h++)
                {
                    for (int k = -50; k < 50; k++)
                    {
                        for (int l = -50; l < 50; l++)
                        {
                            MathNet.Numerics.LinearAlgebra.Vector<double> calcDirection = MathNet.Numerics.LinearAlgebra.CreateVector.Dense(3, 0.0);
                            calcDirection[0] = h;
                            calcDirection[1] = k;
                            calcDirection[2] = l;

                            double normalTest = normalDirection * calcDirection;

                            if (Math.Abs(normalTest) < 0.001)
                            {
                                double angleCorrection = 0.0;

                                switch (AnIsoPlotDirectionSelection.SelectedIndex)
                                {
                                    case 0:
                                        goto case 2;
                                    case 1:
                                        goto case 2;
                                    case 2:
                                        if (h < 0 && k < 0)
                                        {

                                        }
                                        else if (h < 0 && l < 0)
                                        {

                                        }
                                        else if (k < 0 && l < 0)
                                        {

                                        }
                                        else if (h < 0)
                                        {
                                            angleCorrection += Math.PI;
                                        }
                                        else if (k < 0)
                                        {
                                            angleCorrection += Math.PI;
                                        }
                                        else if (l < 0)
                                        {
                                            angleCorrection += Math.PI;
                                        }
                                        break;
                                    case 4:
                                        goto case 6;
                                    case 5:
                                        goto case 6;
                                    case 6:
                                        if (h < 0 && k < 0 && l < 0)
                                        {

                                        }
                                        else if (h < 0)
                                        {
                                            angleCorrection += Math.PI;
                                        }
                                        else if (k < 0)
                                        {
                                            angleCorrection += Math.PI;
                                        }
                                        else if (l < 0)
                                        {
                                            angleCorrection += Math.PI;
                                        }
                                        break;
                                    case 8:
                                        if ((h <= 0 && k > 0) && Math.Abs(h) < k)
                                        {
                                            angleCorrection -= (1.0 / 2.0) * Math.PI;
                                        }
                                        else if ((h > 0 && k > 0) && k > h)
                                        {
                                            angleCorrection -= (1.0 / 2.0) * Math.PI;
                                        }
                                        else if ((h < 0 && k < 0) && h < k)
                                        {
                                            angleCorrection += (1.0 / 2.0) * Math.PI;
                                        }
                                        else if ((k > 0 && h <= 0) && Math.Abs(h) > k)
                                        {
                                            angleCorrection += (1.0 / 2.0) * Math.PI;
                                        }
                                        break;
                                    case 9:
                                        if ((h <= 0 && l > 0) && Math.Abs(h) < l)
                                        {
                                            angleCorrection -= (1.0 / 2.0) * Math.PI;
                                        }
                                        else if ((h > 0 && l > 0) && l > h)
                                        {
                                            angleCorrection -= (1.0 / 2.0) * Math.PI;
                                        }
                                        else if ((h < 0 && l < 0) && h < l)
                                        {
                                            angleCorrection += (1.0 / 2.0) * Math.PI;
                                        }
                                        else if ((l > 0 && h <= 0) && Math.Abs(h) > l)
                                        {
                                            angleCorrection += (1.0 / 2.0) * Math.PI;
                                        }
                                        break;
                                    case 10:
                                        if ((k <= 0 && l > 0) && Math.Abs(k) < l)
                                        {
                                            angleCorrection -= (1.0 / 2.0) * Math.PI;
                                        }
                                        else if ((k > 0 && l > 0) && l > k)
                                        {
                                            angleCorrection -= (1.0 / 2.0) * Math.PI;
                                        }
                                        else if ((k < 0 && l < 0) && k < l)
                                        {
                                            angleCorrection += (1.0 / 2.0) * Math.PI;
                                        }
                                        else if ((l > 0 && k <= 0) && Math.Abs(k) > l)
                                        {
                                            angleCorrection += (1.0 / 2.0) * Math.PI;
                                        }
                                        break;
                                    case 12:

                                        break;
                                    case 13:
                                        if ((k <= 0 && l > 0) && Math.Abs(k) < l)
                                        {
                                            angleCorrection += (1.0 / 1.0) * Math.PI;
                                        }
                                        else if ((k > 0 && l > 0) && l > k)
                                        {
                                            angleCorrection += (1.0 / 1.0) * Math.PI;
                                        }
                                        //else if ((k < 0 && l < 0) && k < l)
                                        //{
                                        //    angleCorrection += (1.0 / 2.0) * Math.PI;
                                        //}
                                        //else if ((l > 0 && k <= 0) && Math.Abs(k) > l)
                                        //{
                                        //    angleCorrection += (1.0 / 2.0) * Math.PI;
                                        //}
                                        break;
                                    case 14:

                                        break;
                                    default:
                                        goto case 2;
                                }

                                SetAnIsoData(usedTensor, h, k, l, parameterSeries, angleDirection, angleCorrection);
                            }
                        }
                    }
                }

                #endregion

                //AnIsoValueAxis.Minimum = double.MaxValue;
                //AnIsoValueAxis.Maximum = 0;

                //for (int n = 0; n < parameterSeries.Points.Count; n++)
                //{
                //    if(parameterSeries.Points[n].X < AnIsoValueAxis.Minimum)
                //    {
                //        AnIsoValueAxis.Minimum = parameterSeries.Points[n].X;
                //    }
                //    if (parameterSeries.Points[n].X > AnIsoValueAxis.Minimum)
                //    {
                //        AnIsoValueAxis.Maximum = parameterSeries.Points[n].X;
                //    }
                //}

                AnIsoAngleAxis.Minimum = 0.0;
                AnIsoAngleAxis.Maximum = 2 * Math.PI;
                //AnIsoValueAxis.Maximum = 250000;

                AnIsoPlotModel.ResetAllAxes();
                AnIsoPlotModel.Series.Add(parameterSeries);
                AnIsoPlotModel.InvalidatePlot(true);
            }
        }

        private void FixAnisotropyCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.TextEventsActive = false;
            this.SetTensorData();
            this.TextEventsActive = true;
        }

        private void FixAnisotropyCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.TextEventsActive = false;
            this.SetTensorData();
            this.TextEventsActive = true;
        }

        private void ZehnderAnisotropy_TextChanged(object sender, TextChangedEventArgs e)
        {
            Stress.Microsopic.ElasticityTensors UsedTensor = this.GetSelectedTensor();

            try
            {
                UsedTensor.AnIsotropy = 1.0 / Convert.ToDouble(this.FixedAnisotropy.Text);
                this.TextEventsActive = false;
                this.SetTensorData();
                this.TextEventsActive = true;
            }
            catch
            {

            }
        }

        private void StartAnalysis_Click(object sender, RoutedEventArgs e)
        {
            Tools.ValueSelection VWindow = new Tools.ValueSelection();

            VWindow.ShowDialog();

            Microsoft.Win32.SaveFileDialog XlsxSaveFile = new Microsoft.Win32.SaveFileDialog();
            XlsxSaveFile.FileName = VWindow.fileName;
            XlsxSaveFile.DefaultExt = "";
            XlsxSaveFile.Filter = "Excel data (.xlsx)|*.xlsx";

            Nullable<bool> Opened = XlsxSaveFile.ShowDialog();

            if (Opened == true)
            {
                string filename = XlsxSaveFile.FileName;
                string PathName = filename.Replace(XlsxSaveFile.SafeFileName, "");
                System.IO.Directory.CreateDirectory(PathName);

                Stress.Microsopic.ElasticityTensors UsedTensor = this.GetSelectedTensor();
                int model = 0;

                if (this.REKSwitchBox.SelectedIndex != 3)
                {
                    UsedTensor.AutoAnisotropyFit(VWindow.lborder, VWindow.uborder, VWindow.step, VWindow.SC, XlsxSaveFile.FileName, model, true);
                }
                else
                {
                    UsedTensor.AutoAnisotropyFit(VWindow.lborder, VWindow.uborder, VWindow.step, VWindow.SC, XlsxSaveFile.FileName, model, false);
                }
            }
        }

        private void SetTransitionView()
        {
            if (this.ActSample.CrystalData.Count > 1 && this.ActSample.StressTransitionFactors.Count > 0)
            {
                this.StressDistributionGroup.Visibility = Visibility.Visible;

                PlotModel distributionPieModel = new PlotModel();

                OxyPlot.Series.PieSeries distributionPieSeries = new OxyPlot.Series.PieSeries();
                distributionPieSeries.AngleSpan = 360;
                distributionPieSeries.StartAngle = 0;
                distributionPieSeries.InsideLabelPosition = 0.8;
                distributionPieSeries.StrokeThickness = 2;

                for (int n = 0; n < this.ActSample.CrystalData.Count; n++)
                {
                    OxyPlot.Series.PieSlice phaseSlice = new OxyPlot.Series.PieSlice(this.ActSample.CrystalData[n].SymmetryGroup, this.ActSample.StressTransitionFactors[n][2, 2]);

                    if (n == 0)
                    {
                        phaseSlice.Fill = OxyColors.DarkBlue;
                    }
                    else if (n == 1)
                    {
                        phaseSlice.Fill = OxyColors.DarkGreen;
                    }
                    else if (n == 2)
                    {
                        phaseSlice.Fill = OxyColors.DarkRed;
                    }

                    distributionPieSeries.Slices.Add(phaseSlice);
                }

                distributionPieModel.Series.Add(distributionPieSeries);
                this.StressDistributionPlot.Model = distributionPieModel;
                this.StressDistributionPlot.InvalidatePlot(true);
            }
            else
            {
                this.StressDistributionGroup.Visibility = Visibility.Collapsed;
            }

            #region old code

            //if(this.PhaseSwitchBox.SelectedIndex != -1)
            //{
            //    List<StressFactorView> stressFactorView = new List<StressFactorView>();

            //    for(int n = 0; n < this.ActSample.StressTransitionFactors[this.PhaseSwitchBox.SelectedIndex].RowCount; n++)
            //    {
            //        StressFactorView tmp = new StressFactorView();

            //        tmp.f1 = this.ActSample.StressTransitionFactors[this.PhaseSwitchBox.SelectedIndex][n, 0];
            //        tmp.f2 = this.ActSample.StressTransitionFactors[this.PhaseSwitchBox.SelectedIndex][n, 1];
            //        tmp.f3 = this.ActSample.StressTransitionFactors[this.PhaseSwitchBox.SelectedIndex][n, 2];
            //        tmp.f4 = this.ActSample.StressTransitionFactors[this.PhaseSwitchBox.SelectedIndex][n, 3];
            //        tmp.f5 = this.ActSample.StressTransitionFactors[this.PhaseSwitchBox.SelectedIndex][n, 4];
            //        tmp.f6 = this.ActSample.StressTransitionFactors[this.PhaseSwitchBox.SelectedIndex][n, 5];

            //        stressFactorView.Add(tmp);
            //    }

            //    this.StressTransitionFactorView.ItemsSource = stressFactorView;
            //}

            #endregion
        }

        private void SetTransitionFactors_Click(object sender, RoutedEventArgs e)
        {
            if (this.ActSample.CrystalData.Count > 1 && this.ModelSwitchBox.SelectedIndex != -1)
            {
                int matrixPhase = 0;
                int inclusionPhase = 1;

                if (this.ActSample.CrystalData[1].Matrix)
                {
                    matrixPhase = 1;
                    inclusionPhase = 0;
                }

                bool incType = false;
                if (this.ActSample.CrystalData[inclusionPhase].InclusionType == 0)
                {
                    incType = true;
                }

                this.ActSample.SetStressTransitionFactors(matrixPhase, inclusionPhase, incType, this.ModelSwitchBox.SelectedIndex);

                SetTransitionView();
            }
            else
            {
                MessageBox.Show("Only one phase found in the crystallographic data! \n at least two destinct phases are requried.", "Only one phase detected", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FitConstantsSelfConsistent_Click(object sender, RoutedEventArgs e)
        {

            this.TextEventsActive = false;

            if (this.ActSample.CrystalData.Count > 1 && this.ModelSwitchBox.SelectedIndex != -1)
            {
                bool modelCheck = false;
                if (this.ModelSwitchBox.SelectedIndex != 1)
                {
                    MessageBoxResult mBR = MessageBox.Show("To calculate the stress distribution it is recommendet to use Reuss Model. This is due to a conflict of assumptions in the physical theory. Proceed anyway?", "Model Conflict!", MessageBoxButton.YesNo, MessageBoxImage.Information);

                    if (mBR == MessageBoxResult.Yes)
                    {
                        modelCheck = true;
                    }
                }
                else
                {
                    modelCheck = true;
                }

                if (modelCheck)
                {
                    for (int n = 0; n < 10; n++)
                    {
                        #region Setting of transition Factors

                        int matrixPhase = 0;
                        int inclusionPhase = 1;

                        if (this.ActSample.CrystalData[1].Matrix)
                        {
                            matrixPhase = 1;
                            inclusionPhase = 0;
                        }

                        bool incType = false;
                        if (this.ActSample.CrystalData[inclusionPhase].InclusionType == 0)
                        {
                            incType = true;
                        }

                        this.ActSample.SetStressTransitionFactors(matrixPhase, inclusionPhase, incType, this.ModelSwitchBox.SelectedIndex);

                        #endregion

                        this.ActSample.SetPhaseStresses();
                        this.ActSample.RefitAllDECStressCorrected();

                        for (int i = 0; i < this.ActSample.CrystalData.Count; i++)
                        {
                            this.RefitSEC(i, REKSwitchBox.SelectedIndex, this.ModelSwitchBox.SelectedIndex, this.StiffnessComlplianceSwitchBox.SelectedIndex);
                        }


                    }
                }

                this.SetTransitionView();
                this.SetTensorData();
                this.TextEventsActive = true;
            }
            else
            {
                MessageBox.Show("Only one phase found in the crystallographic data! \n at least two destinct phases are requried.", "Only one phase detected", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowAnIsoPlot_Click(object sender, RoutedEventArgs e)
        {
            this.AniIsoPlotWindow.Show();
        }

        private void SetAnIsoPlot_Click(object sender, RoutedEventArgs e)
        {
            this.SetAnIsoPlot();
        }
        private void SaveAnIsoPlotClipboard_Click(object sender, RoutedEventArgs e)
        {
            OxyPlot.Wpf.PngExporter pngExporter = new OxyPlot.Wpf.PngExporter { Width = 1200, Height = 1200, Background = OxyPlot.OxyColors.White };
            var bitmap = pngExporter.ExportToBitmap(this.AnIsoPlotModel);
            Clipboard.SetImage(bitmap);
        }
        private void ReSetAnIsoPlot_Click(object sender, RoutedEventArgs e)
        {
            this.AnIsoPlotModel.Annotations.Clear();
            this.AnIsoPlotModel.Series.Clear();

            AnIsoPlotModel.ResetAllAxes();
            AnIsoPlotModel.InvalidatePlot(true);
        }

        #endregion

        private void REKSwitchBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.TextEventsActive = false;

            this.SetTensorData();

            this.TextEventsActive = true;
        }

        protected override void OnClosed(EventArgs e)
        {
            this.AniIsoPlotWindow.PreventClosing = false;
            this.AniIsoPlotWindow.Close();

            base.OnClosed(e);
        }
    }
    public struct StressFactorView
    {
        public double f1
        {
            get;
            set;
        }
        public double f2
        {
            get;
            set;
        }
        public double f3
        {
            get;
            set;
        }
        public double f4
        {
            get;
            set;
        }
        public double f5
        {
            get;
            set;
        }
        public double f6
        {
            get;
            set;
        }
    }
}
