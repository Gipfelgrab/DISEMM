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

using System.ComponentModel;

namespace CalScec.Analysis.Stress.Plasticity
{
    
    /// <summary>
    /// Interaction logic for YieldSurfaceWindow.xaml
    /// </summary>
    public partial class YieldSurfaceWindow : Window
    {
        List<MatrixView> sMV = new List<MatrixView>();
        List<MatrixView> hMV = new List<MatrixView>();
        Sample ActSample;

        public OxyPlot.PlotModel YieldPlotModel = new OxyPlot.PlotModel();
        OxyPlot.Axes.LinearAxis YieldXAxisLin = new OxyPlot.Axes.LinearAxis();
        OxyPlot.Axes.LinearAxis YieldYAxisLin = new OxyPlot.Axes.LinearAxis();

        public OxyPlot.PlotModel YieldFractionPlotModel = new OxyPlot.PlotModel();
        OxyPlot.Axes.LinearAxis YieldFractionXAxisLin = new OxyPlot.Axes.LinearAxis();
        OxyPlot.Axes.LinearAxis YieldFractionYAxisLin = new OxyPlot.Axes.LinearAxis();

        Tools.PlottingWindow PW = new Tools.PlottingWindow();

        private bool AnnotationEventsActive = true;
        private bool IndexEventAktive = true;

        List<double> ComboPsiAngles = new List<double>();

        private List<SimulationTask> SimulationQueque = new List<SimulationTask>();

        private Tools.YieldSurfacePlotSettings plotSettingsWindow = new Tools.YieldSurfacePlotSettings();

        public YieldSurfaceWindow(Sample actSample)
        {
            InitializeComponent();

            this.ActSample = actSample;

            this.SetDataBindings();
        }
        
        private void SetDataBindings()
        {
            IndexEventAktive = false;
            PW.Show();

            for (int n = 0; n < this.ActSample.CrystalData.Count; n++)
            {
                ComboBoxItem PhaseItem = new ComboBoxItem();
                PhaseItem.Content = this.ActSample.CrystalData[n].SymmetryGroup;

                this.PhaseSwitchBox.Items.Add(PhaseItem);
            }
            this.PhaseSwitchBox.SelectedIndex = 0;

            if (this.ActSample.PlasticTensor.Count == 0)
            {
                for(int n = 0; n < this.ActSample.CrystalData.Count; n++ )
                {
                    this.ActSample.PlasticTensor.Add(new PlasticityTensor());
                    this.SetYieldSurface();
                }
            }

            YieldPlotModel.LegendBorder = OxyPlot.OxyColors.Black;
            YieldPlotModel.LegendItemAlignment = OxyPlot.HorizontalAlignment.Left;
            YieldPlotModel.LegendTitle = "Region";
            YieldPlotModel.LegendFontSize = 28;
            YieldPlotModel.LegendTitleFontSize = 30;

            YieldXAxisLin.Position = OxyPlot.Axes.AxisPosition.Bottom;
            YieldXAxisLin.Minimum = 0;
            YieldXAxisLin.Maximum = 180;
            YieldXAxisLin.FontSize = 28;
            YieldXAxisLin.TitleFontSize = 30;
            YieldXAxisLin.Title = "Strain";

            YieldYAxisLin.Position = OxyPlot.Axes.AxisPosition.Left;
            YieldYAxisLin.Minimum = 0;
            YieldYAxisLin.Maximum = 100;
            YieldYAxisLin.FontSize = 28;
            YieldYAxisLin.TitleFontSize = 30;
            YieldYAxisLin.Title = "Stress";
            YieldXAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dash;
            YieldYAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dash;
            YieldXAxisLin.MajorGridlineThickness = 3;

            YieldXAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dot;
            YieldYAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dot;
            YieldYAxisLin.MinorGridlineThickness = 1.5;

            this.YieldPlotModel.Axes.Add(YieldXAxisLin);
            this.YieldPlotModel.Axes.Add(YieldYAxisLin);

            this.PW.MainPlot.Model = YieldPlotModel;

            //[0]: "Sample stress";
            //[1]: "Sample strain";
            //[2]: "Sample Stress rate";
            //[3]: "Sample strain rate";
            //[0]: "Grain stress";
            //[1]: "Grain strain";
            //[2]: "Grain Stress rate";
            //[3]: "Grain strain rate";

            ComboBoxItem CBIXA1 = new ComboBoxItem();
            CBIXA1.Content = "Sample Stress";
            this.xAxesm.Items.Add(CBIXA1);
            ComboBoxItem CBIXA2 = new ComboBoxItem();
            CBIXA2.Content = "Sample Strain";
            this.xAxesm.Items.Add(CBIXA2);
            ComboBoxItem CBIXA3 = new ComboBoxItem();
            CBIXA3.Content = "Sample Stress Rate";
            this.xAxesm.Items.Add(CBIXA3);
            ComboBoxItem CBIXA4 = new ComboBoxItem();
            CBIXA4.Content = "Sample Strain Rate";
            this.xAxesm.Items.Add(CBIXA4);
            ComboBoxItem CBIXA9 = new ComboBoxItem();
            CBIXA9.Content = "Lattice Strain";
            this.xAxesm.Items.Add(CBIXA9);
            ComboBoxItem CBIXA10 = new ComboBoxItem();
            CBIXA10.Content = "Lattice Strain Rate";
            this.xAxesm.Items.Add(CBIXA10);
            ComboBoxItem CBIXA5 = new ComboBoxItem();
            CBIXA5.Content = "Grain Stress (Expert Only)";
            this.xAxesm.Items.Add(CBIXA5);
            ComboBoxItem CBIXA6 = new ComboBoxItem();
            CBIXA6.Content = "Grain Strain (Expert Only)";
            this.xAxesm.Items.Add(CBIXA6);
            ComboBoxItem CBIXA7 = new ComboBoxItem();
            CBIXA7.Content = "Grain Stress Rate (Expert Only)";
            this.xAxesm.Items.Add(CBIXA7);
            ComboBoxItem CBIXA8 = new ComboBoxItem();
            CBIXA8.Content = "Grain Strain Rate (Expert Only)";
            this.xAxesm.Items.Add(CBIXA8);

            this.xAxesm.SelectedIndex = 1;
            //CBIXA11.Content = "Slip Activity";
            //this.xAxesm.Items.Add(CBIXA11);
            //ComboBoxItem CBIXA12 = new ComboBoxItem();
            //CBIXA12.Content = "Slip Activity Grain";
            //this.xAxesm.Items.Add(CBIXA12);
            
            ComboBoxItem CBIYA1 = new ComboBoxItem();
            CBIYA1.Content = "Sample Stress";
            this.yAxesm.Items.Add(CBIYA1);
            ComboBoxItem CBIYA2 = new ComboBoxItem();
            CBIYA2.Content = "Sample Strain";
            this.yAxesm.Items.Add(CBIYA2);
            ComboBoxItem CBIYA3 = new ComboBoxItem();
            CBIYA3.Content = "Sample Stress Rate";
            this.yAxesm.Items.Add(CBIYA3);
            ComboBoxItem CBIYA4 = new ComboBoxItem();
            CBIYA4.Content = "Sample Strain Rate";
            this.yAxesm.Items.Add(CBIYA4);
            ComboBoxItem CBIYA9 = new ComboBoxItem();
            CBIYA9.Content = "Lattice Strain";
            this.yAxesm.Items.Add(CBIYA9);
            ComboBoxItem CBIYA10 = new ComboBoxItem();
            CBIYA10.Content = "Lattice Strain Rate";
            this.yAxesm.Items.Add(CBIYA10);
            ComboBoxItem CBIYA11 = new ComboBoxItem();
            CBIYA11.Content = "Slip Activity";
            this.yAxesm.Items.Add(CBIYA11);
            ComboBoxItem CBIYA12 = new ComboBoxItem();
            CBIYA12.Content = "Slip Activity Grain";
            this.yAxesm.Items.Add(CBIYA12);
            ComboBoxItem CBIYA13 = new ComboBoxItem();
            CBIYA13.Content = "FWHM (Data only)";
            this.yAxesm.Items.Add(CBIYA13);
            ComboBoxItem CBIYA14 = new ComboBoxItem();
            CBIYA14.Content = "Area (Data only)";
            this.yAxesm.Items.Add(CBIYA14);
            ComboBoxItem CBIYA5 = new ComboBoxItem();
            CBIYA5.Content = "Grain Stress (Expert Only)";
            this.yAxesm.Items.Add(CBIYA5);
            ComboBoxItem CBIYA6 = new ComboBoxItem();
            CBIYA6.Content = "Grain Strain (Expert Only)";
            this.yAxesm.Items.Add(CBIYA6);
            ComboBoxItem CBIYA7 = new ComboBoxItem();
            CBIYA7.Content = "Grain Stress Rate (Expert Only)";
            this.yAxesm.Items.Add(CBIYA7);
            ComboBoxItem CBIYA8 = new ComboBoxItem();
            CBIYA8.Content = "Grain Strain Rate (Expert Only)";
            this.yAxesm.Items.Add(CBIYA8);

            this.yAxesm.SelectedIndex = 0;

            this.SetSimulationExperiemnts();
            
            this.Phi1Start.Text = "5.0";
            this.Phi1End.Text = "85.0";
            this.Phi1Step.Text = "10.0";
            this.PsiStart.Text = "0.0";
            this.PsiEnd.Text = "90.0";
            this.PsiStep.Text = "2.5";
            this.Phi2Start.Text = "0.0";
            this.Phi2End.Text = "90.0";
            this.Phi2Step.Text = "10.0";

            this.SetGrainContentLabel();

            this.ExperimentalTicks.Text = "5";
            this.ExperimentalChiAngle.Text = "0.0";
            this.ExperimentalOmegaAngle.Text = "0.0";
            this.ExperimentalValue33.Text = "100";
            this.ExperimentalValue22.Text = "0";
            this.ExperimentalValue11.Text = "0";
            this.ExperimentalValue12.Text = "0";
            this.ExperimentalValue13.Text = "0";
            this.ExperimentalValue23.Text = "0";

            this.MacroExperimentPlotList.ItemsSource = this.ActSample.TensileTests;

            this.SimulationList.ItemsSource = this.SimulationQueque;
            this.CalculationModel.SelectedIndex = 0;
            this.SlipCriterion.SelectedIndex = 0;

            this.yAxesMacro.SelectedIndex = 1;
            this.xAxesMacro.SelectedIndex = 0;
            this.plotSettingsWindow.SimuLineType.SelectedIndex = 0;
            this.plotSettingsWindow.ExpLineType.SelectedIndex = 0;
            this.plotSettingsWindow.TensileLineType.SelectedIndex = 0;
            this.plotSettingsWindow.ExpMarkerType.SelectedIndex = 0;
            this.plotSettingsWindow.TensileMarkerType.SelectedIndex = 1;
            this.ExperimentalDataSelection.SelectedIndex = 0;
            this.ExperimentalDataSelection1.SelectedIndex = 1;

            this.SimulationLimit.Text = Properties.Settings.Default.EPSCSimulationLimit.ToString("F3");

            IndexEventAktive = true;

            this.ActSample.SetExperimentalData();
            this.ActSample.SetExperimentalStrainData();

            this.SetExperimentalReflexView();
        }

        private void PhaseSwitchBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.IndexEventAktive = false;
            if (this.PhaseSwitchBox.SelectedIndex != -1 && this.ExperimentSelection.SelectedIndex != -1)
            {
                this.SetExperimentalReflexView();
                this.PotenitalSlipSystemsList.ItemsSource = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems;

                List<ReflexYield> slipFamilySystems = new List<ReflexYield>();
                for(int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems.Count; n++)
                {
                    if(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].PlainMainMultiplizity != 1)
                    {
                        slipFamilySystems.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n]);
                    }
                }
                SlipFamilyList.ItemsSource = slipFamilySystems;
                
                this.GrainorientationList.ItemsSource = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GrainOrientations[this.PhaseSwitchBox.SelectedIndex];
                //Set Slip Families
                List<ReflexYield> SlipFamlilyList = new List<ReflexYield>();
                if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].CrystalData.SymmetryGroupID == 225)
                {
                    //FCC
                    SlipFamlilyList.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[0]);
                    SlipFamilyModificationYieldStrength.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[0].YieldMainStrength.ToString("F3");
                    SlipFamilyModificationHardenning.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[0].YieldHardenning.ToString("F3");
                }
                else if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].CrystalData.SymmetryGroupID == 229)
                {
                    //BCC
                    SlipFamlilyList.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[0]);
                    SlipFamlilyList.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[12]);
                    SlipFamlilyList.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[25]);
                    SlipFamilyModificationYieldStrength.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[0].YieldMainStrength.ToString("F3");
                    SlipFamilyModificationHardenning.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[0].YieldHardenning.ToString("F3");
                }
                else if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].CrystalData.SymmetryGroupID == 194)
                {
                    //HCP
                    SlipFamlilyList.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[0]);
                    SlipFamlilyList.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[3]);
                    SlipFamlilyList.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[6]);
                    SlipFamlilyList.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[9]);
                    SlipFamlilyList.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[12]);
                    SlipFamlilyList.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[15]);
                    SlipFamlilyList.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[18]);
                    SlipFamlilyList.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[21]);
                    SlipFamlilyList.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[24]);
                    SlipFamlilyList.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[39]);
                    SlipFamlilyList.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[42]);
                    SlipFamlilyList.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[57]);
                    SlipFamlilyList.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[63]);
                    SlipFamlilyList.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[66]);
                }

                this.SlipFamilyList.ItemsSource = SlipFamlilyList;
                this.SlipFamilyList.SelectedIndex = 0;

            }
            this.IndexEventAktive = true;
        }

        private void SlipFamilyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(this.IndexEventAktive && this.SlipFamilyList.SelectedIndex != -1 && this.PhaseSwitchBox.SelectedIndex != -1 && this.ExperimentSelection.SelectedIndex != -1)
            {
                this.IndexEventAktive = false;

                if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].CrystalData.SymmetryGroupID == 225)
                {
                    //FCC
                    SlipFamilyModificationYieldStrength.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[0].YieldMainStrength.ToString("F3");
                    SlipFamilyModificationHardenning.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[0].YieldHardenning.ToString("F3");
                    SlipFamilyModificationYieldLimit.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[0].YieldLimit.ToString("F3");
                }
                else if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].CrystalData.SymmetryGroupID == 229)
                {
                    //BCC
                    switch(this.SlipFamilyList.SelectedIndex)
                    {
                        case 0:
                            SlipFamilyModificationYieldStrength.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[0].YieldMainStrength.ToString("F3");
                            SlipFamilyModificationHardenning.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[0].YieldHardenning.ToString("F3");
                            SlipFamilyModificationYieldLimit.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[0].YieldLimit.ToString("F3");
                            break;
                        case 1:
                            SlipFamilyModificationYieldStrength.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[12].YieldMainStrength.ToString("F3");
                            SlipFamilyModificationHardenning.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[12].YieldHardenning.ToString("F3");
                            SlipFamilyModificationYieldLimit.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[0].YieldLimit.ToString("F3");
                            break;
                        case 2:
                            SlipFamilyModificationYieldStrength.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[25].YieldMainStrength.ToString("F3");
                            SlipFamilyModificationHardenning.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[25].YieldHardenning.ToString("F3");
                            SlipFamilyModificationYieldLimit.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[0].YieldLimit.ToString("F3");
                            break;
                        default:
                            SlipFamilyModificationYieldStrength.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[0].YieldMainStrength.ToString("F3");
                            SlipFamilyModificationHardenning.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[0].YieldHardenning.ToString("F3");
                            SlipFamilyModificationYieldLimit.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[0].YieldLimit.ToString("F3");
                            break;
                    }
                }
                else if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].CrystalData.SymmetryGroupID == 194)
                {
                    //HCP
                    switch (this.SlipFamilyList.SelectedIndex)
                    {
                        case 0:
                            //Basal low burgers length
                            SlipFamilyModificationYieldStrength.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[0].YieldMainStrength.ToString("F3");
                            SlipFamilyModificationHardenning.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[0].YieldHardenning.ToString("F3");
                            SlipFamilyModificationYieldLimit.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[0].YieldLimit.ToString("F3");
                            SlipFamilyActive.IsChecked = Convert.ToBoolean(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[0].ActiveSystem);
                            break;
                        case 1:
                            //Basal high burgers length
                            SlipFamilyModificationYieldStrength.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[3].YieldMainStrength.ToString("F3");
                            SlipFamilyModificationHardenning.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[3].YieldHardenning.ToString("F3");
                            SlipFamilyModificationYieldLimit.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[3].YieldLimit.ToString("F3");
                            SlipFamilyActive.IsChecked = Convert.ToBoolean(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[3].ActiveSystem);
                            break;
                        case 2:
                            //Prismatic I 1st order 1. Direction
                            SlipFamilyModificationYieldStrength.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[6].YieldMainStrength.ToString("F3");
                            SlipFamilyModificationHardenning.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[6].YieldHardenning.ToString("F3");
                            SlipFamilyModificationYieldLimit.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[6].YieldLimit.ToString("F3");
                            SlipFamilyActive.IsChecked = Convert.ToBoolean(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[6].ActiveSystem);
                            break;
                        case 3:
                            //Prismatic I 1st order 2. Direction
                            SlipFamilyModificationYieldStrength.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[9].YieldMainStrength.ToString("F3");
                            SlipFamilyModificationHardenning.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[9].YieldHardenning.ToString("F3");
                            SlipFamilyModificationYieldLimit.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[9].YieldLimit.ToString("F3");
                            SlipFamilyActive.IsChecked = Convert.ToBoolean(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[9].ActiveSystem);
                            break;
                        case 4:
                            //Prismatic II 1st order 1. Direction
                            SlipFamilyModificationYieldStrength.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[12].YieldMainStrength.ToString("F3");
                            SlipFamilyModificationHardenning.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[12].YieldHardenning.ToString("F3");
                            SlipFamilyModificationYieldLimit.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[12].YieldLimit.ToString("F3");
                            SlipFamilyActive.IsChecked = Convert.ToBoolean(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[12].ActiveSystem);
                            break;
                        case 5:
                            //Prismatic II 1st order 2. Direction
                            SlipFamilyModificationYieldStrength.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[15].YieldMainStrength.ToString("F3");
                            SlipFamilyModificationHardenning.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[15].YieldHardenning.ToString("F3");
                            SlipFamilyModificationYieldLimit.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[15].YieldLimit.ToString("F3");
                            SlipFamilyActive.IsChecked = Convert.ToBoolean(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[15].ActiveSystem);
                            break;
                        case 6:
                            //Prismatic II 1st order 3. Direction
                            SlipFamilyModificationYieldStrength.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[18].YieldMainStrength.ToString("F3");
                            SlipFamilyModificationHardenning.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[18].YieldHardenning.ToString("F3");
                            SlipFamilyModificationYieldLimit.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[18].YieldLimit.ToString("F3");
                            SlipFamilyActive.IsChecked = Convert.ToBoolean(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[18].ActiveSystem);
                            break;
                        case 7:
                            //Pyramid I 1st order 1. Direction
                            SlipFamilyModificationYieldStrength.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[21].YieldMainStrength.ToString("F3");
                            SlipFamilyModificationHardenning.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[21].YieldHardenning.ToString("F3");
                            SlipFamilyModificationYieldLimit.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[21].YieldLimit.ToString("F3");
                            SlipFamilyActive.IsChecked = Convert.ToBoolean(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[21].ActiveSystem);
                            break;
                        case 8:
                            //Pyramid I 1st order 2. Direction
                            SlipFamilyModificationYieldStrength.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[24].YieldMainStrength.ToString("F3");
                            SlipFamilyModificationHardenning.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[24].YieldHardenning.ToString("F3");
                            SlipFamilyModificationYieldLimit.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[24].YieldLimit.ToString("F3");
                            SlipFamilyActive.IsChecked = Convert.ToBoolean(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[24].ActiveSystem);
                            break;
                        case 9:
                            //Pyramid II 1st order 1. Direction
                            SlipFamilyModificationYieldStrength.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[39].YieldMainStrength.ToString("F3");
                            SlipFamilyModificationHardenning.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[39].YieldHardenning.ToString("F3");
                            SlipFamilyModificationYieldLimit.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[39].YieldLimit.ToString("F3");
                            SlipFamilyActive.IsChecked = Convert.ToBoolean(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[39].ActiveSystem);
                            break;
                        case 10:
                            //Pyramid II 1st order 2. Direction
                            SlipFamilyModificationYieldStrength.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[42].YieldMainStrength.ToString("F3");
                            SlipFamilyModificationHardenning.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[42].YieldHardenning.ToString("F3");
                            SlipFamilyModificationYieldLimit.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[42].YieldLimit.ToString("F3");
                            SlipFamilyActive.IsChecked = Convert.ToBoolean(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[42].ActiveSystem);
                            break;
                        case 11:
                            //Pyramid I 2nd order 1. Direction
                            SlipFamilyModificationYieldStrength.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[57].YieldMainStrength.ToString("F3");
                            SlipFamilyModificationHardenning.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[57].YieldHardenning.ToString("F3");
                            SlipFamilyModificationYieldLimit.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[57].YieldLimit.ToString("F3");
                            SlipFamilyActive.IsChecked = Convert.ToBoolean(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[57].ActiveSystem);
                            break;
                        case 12:
                            //Pyramid I 2nd order 2. Direction
                            SlipFamilyModificationYieldStrength.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[63].YieldMainStrength.ToString("F3");
                            SlipFamilyModificationHardenning.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[63].YieldHardenning.ToString("F3");
                            SlipFamilyModificationYieldLimit.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[63].YieldLimit.ToString("F3");
                            SlipFamilyActive.IsChecked = Convert.ToBoolean(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[63].ActiveSystem);
                            break;
                        case 13:
                            //Pyramid II 2nd order 1. Direction
                            SlipFamilyModificationYieldStrength.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[66].YieldMainStrength.ToString("F3");
                            SlipFamilyModificationHardenning.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[66].YieldHardenning.ToString("F3");
                            SlipFamilyModificationYieldLimit.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[66].YieldLimit.ToString("F3");
                            SlipFamilyActive.IsChecked = Convert.ToBoolean(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[66].ActiveSystem);
                            break;
                        default:
                            SlipFamilyModificationYieldStrength.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[0].YieldMainStrength.ToString("F3");
                            SlipFamilyModificationHardenning.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[0].YieldHardenning.ToString("F3");
                            SlipFamilyModificationYieldLimit.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[0].YieldLimit.ToString("F3");
                            SlipFamilyActive.IsChecked = Convert.ToBoolean(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[0].ActiveSystem);
                            break;
                    }
                }
                this.IndexEventAktive = true;
            }
        }

        private void SlipFamilyModification_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (this.IndexEventAktive && this.SlipFamilyList.SelectedIndex != -1 && this.PhaseSwitchBox.SelectedIndex != -1 && this.ExperimentSelection.SelectedIndex != -1)
                {
                    if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].CrystalData.SymmetryGroupID == 225)
                    {
                        //FCC
                        for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems.Count; n++)
                        {
                            this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldMainStrength = Convert.ToDouble(SlipFamilyModificationYieldStrength.Text);
                            this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldHardenning = Convert.ToDouble(SlipFamilyModificationHardenning.Text);
                            this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldLimit = Convert.ToDouble(SlipFamilyModificationYieldLimit.Text);
                        }
                    }
                    if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].CrystalData.SymmetryGroupID == 227)
                    {
                        //FCC
                        for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems.Count; n++)
                        {
                            this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldMainStrength = Convert.ToDouble(SlipFamilyModificationYieldStrength.Text);
                            this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldHardenning = Convert.ToDouble(SlipFamilyModificationHardenning.Text);
                            this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldLimit = Convert.ToDouble(SlipFamilyModificationYieldLimit.Text);
                        }
                    }
                    else if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].CrystalData.SymmetryGroupID == 229)
                    {
                        //BCC
                        switch (this.SlipFamilyList.SelectedIndex)
                        {
                            case 0:
                                for(int n = 0; n < 12; n++)
                                {
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldMainStrength = Convert.ToDouble(SlipFamilyModificationYieldStrength.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldHardenning = Convert.ToDouble(SlipFamilyModificationHardenning.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldLimit = Convert.ToDouble(SlipFamilyModificationYieldLimit.Text);
                                }
                                break;
                            case 1:
                                for (int n = 12; n < 25; n++)
                                {
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldMainStrength = Convert.ToDouble(SlipFamilyModificationYieldStrength.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldHardenning = Convert.ToDouble(SlipFamilyModificationHardenning.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldLimit = Convert.ToDouble(SlipFamilyModificationYieldLimit.Text);
                                }
                                break;
                            case 2:
                                for (int n = 25; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems.Count; n++)
                                {
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldMainStrength = Convert.ToDouble(SlipFamilyModificationYieldStrength.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldHardenning = Convert.ToDouble(SlipFamilyModificationHardenning.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldLimit = Convert.ToDouble(SlipFamilyModificationYieldLimit.Text);
                                }
                                break;
                            default:
                                for (int n = 0; n < 12; n++)
                                {
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldMainStrength = Convert.ToDouble(SlipFamilyModificationYieldStrength.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldHardenning = Convert.ToDouble(SlipFamilyModificationHardenning.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldLimit = Convert.ToDouble(SlipFamilyModificationYieldLimit.Text);
                                }
                                break;
                        }
                    }
                    else if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].CrystalData.SymmetryGroupID == 194)
                    {
                        //HCP
                        switch (this.SlipFamilyList.SelectedIndex)
                        {
                            case 0:
                                for (int n = 0; n < 3; n++)
                                {
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldMainStrength = Convert.ToDouble(SlipFamilyModificationYieldStrength.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldHardenning = Convert.ToDouble(SlipFamilyModificationHardenning.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldLimit = Convert.ToDouble(SlipFamilyModificationYieldLimit.Text);
                                }
                                break;
                            case 1:
                                for (int n = 3; n < 6; n++)
                                {
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldMainStrength = Convert.ToDouble(SlipFamilyModificationYieldStrength.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldHardenning = Convert.ToDouble(SlipFamilyModificationHardenning.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldLimit = Convert.ToDouble(SlipFamilyModificationYieldLimit.Text);
                                }
                                break;
                            case 2:
                                for (int n = 6; n < 9; n++)
                                {
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldMainStrength = Convert.ToDouble(SlipFamilyModificationYieldStrength.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldHardenning = Convert.ToDouble(SlipFamilyModificationHardenning.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldLimit = Convert.ToDouble(SlipFamilyModificationYieldLimit.Text);
                                }
                                break;
                            case 3:
                                for (int n = 9; n < 12; n++)
                                {
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldMainStrength = Convert.ToDouble(SlipFamilyModificationYieldStrength.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldHardenning = Convert.ToDouble(SlipFamilyModificationHardenning.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldLimit = Convert.ToDouble(SlipFamilyModificationYieldLimit.Text);
                                }
                                break;
                            case 4:
                                for (int n = 12; n < 15; n++)
                                {
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldMainStrength = Convert.ToDouble(SlipFamilyModificationYieldStrength.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldHardenning = Convert.ToDouble(SlipFamilyModificationHardenning.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldLimit = Convert.ToDouble(SlipFamilyModificationYieldLimit.Text);
                                }
                                break;
                            case 5:
                                for (int n = 15; n < 18; n++)
                                {
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldMainStrength = Convert.ToDouble(SlipFamilyModificationYieldStrength.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldHardenning = Convert.ToDouble(SlipFamilyModificationHardenning.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldLimit = Convert.ToDouble(SlipFamilyModificationYieldLimit.Text);
                                }
                                break;
                            case 6:
                                for (int n = 18; n < 21; n++)
                                {
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldMainStrength = Convert.ToDouble(SlipFamilyModificationYieldStrength.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldHardenning = Convert.ToDouble(SlipFamilyModificationHardenning.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldLimit = Convert.ToDouble(SlipFamilyModificationYieldLimit.Text);
                                }
                                break;
                            case 7:
                                for (int n = 21; n < 24; n++)
                                {
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldMainStrength = Convert.ToDouble(SlipFamilyModificationYieldStrength.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldHardenning = Convert.ToDouble(SlipFamilyModificationHardenning.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldLimit = Convert.ToDouble(SlipFamilyModificationYieldLimit.Text);
                                }
                                for (int n = 30; n < 33; n++)
                                {
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldMainStrength = Convert.ToDouble(SlipFamilyModificationYieldStrength.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldHardenning = Convert.ToDouble(SlipFamilyModificationHardenning.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldLimit = Convert.ToDouble(SlipFamilyModificationYieldLimit.Text);
                                }
                                break;
                            case 8:
                                for (int n = 24; n < 30; n++)
                                {
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldMainStrength = Convert.ToDouble(SlipFamilyModificationYieldStrength.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldHardenning = Convert.ToDouble(SlipFamilyModificationHardenning.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldLimit = Convert.ToDouble(SlipFamilyModificationYieldLimit.Text);
                                }
                                for (int n = 33; n < 39; n++)
                                {
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldMainStrength = Convert.ToDouble(SlipFamilyModificationYieldStrength.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldHardenning = Convert.ToDouble(SlipFamilyModificationHardenning.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldLimit = Convert.ToDouble(SlipFamilyModificationYieldLimit.Text);
                                }
                                break;
                            case 9:
                                for (int n = 39; n < 42; n++)
                                {
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldMainStrength = Convert.ToDouble(SlipFamilyModificationYieldStrength.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldHardenning = Convert.ToDouble(SlipFamilyModificationHardenning.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldLimit = Convert.ToDouble(SlipFamilyModificationYieldLimit.Text);
                                }
                                for (int n = 48; n < 51; n++)
                                {
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldMainStrength = Convert.ToDouble(SlipFamilyModificationYieldStrength.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldHardenning = Convert.ToDouble(SlipFamilyModificationHardenning.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldLimit = Convert.ToDouble(SlipFamilyModificationYieldLimit.Text);
                                }
                                break;
                            case 10:
                                for (int n = 42; n < 48; n++)
                                {
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldMainStrength = Convert.ToDouble(SlipFamilyModificationYieldStrength.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldHardenning = Convert.ToDouble(SlipFamilyModificationHardenning.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldLimit = Convert.ToDouble(SlipFamilyModificationYieldLimit.Text);
                                }
                                for (int n = 51; n < 57; n++)
                                {
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldMainStrength = Convert.ToDouble(SlipFamilyModificationYieldStrength.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldHardenning = Convert.ToDouble(SlipFamilyModificationHardenning.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldLimit = Convert.ToDouble(SlipFamilyModificationYieldLimit.Text);
                                }
                                break;
                            case 11:
                                for (int n = 57; n < 63; n++)
                                {
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldMainStrength = Convert.ToDouble(SlipFamilyModificationYieldStrength.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldHardenning = Convert.ToDouble(SlipFamilyModificationHardenning.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldLimit = Convert.ToDouble(SlipFamilyModificationYieldLimit.Text);
                                }
                                break;
                            case 12:
                                for (int n = 63; n < 66; n++)
                                {
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldMainStrength = Convert.ToDouble(SlipFamilyModificationYieldStrength.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldHardenning = Convert.ToDouble(SlipFamilyModificationHardenning.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldLimit = Convert.ToDouble(SlipFamilyModificationYieldLimit.Text);
                                }
                                for (int n = 69; n < 72; n++)
                                {
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldMainStrength = Convert.ToDouble(SlipFamilyModificationYieldStrength.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldHardenning = Convert.ToDouble(SlipFamilyModificationHardenning.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldLimit = Convert.ToDouble(SlipFamilyModificationYieldLimit.Text);
                                }
                                break;
                            case 13:
                                for (int n = 66; n < 69; n++)
                                {
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldMainStrength = Convert.ToDouble(SlipFamilyModificationYieldStrength.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldHardenning = Convert.ToDouble(SlipFamilyModificationHardenning.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldLimit = Convert.ToDouble(SlipFamilyModificationYieldLimit.Text);
                                }
                                for (int n = 72; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems.Count; n++)
                                {
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldMainStrength = Convert.ToDouble(SlipFamilyModificationYieldStrength.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldHardenning = Convert.ToDouble(SlipFamilyModificationHardenning.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldLimit = Convert.ToDouble(SlipFamilyModificationYieldLimit.Text);
                                }
                                break;
                            default:
                                for (int n = 0; n < 3; n++)
                                {
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldMainStrength = Convert.ToDouble(SlipFamilyModificationYieldStrength.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldHardenning = Convert.ToDouble(SlipFamilyModificationHardenning.Text);
                                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].YieldLimit = Convert.ToDouble(SlipFamilyModificationYieldLimit.Text);
                                }
                                break;
                        }
                    }

                    this.PotenitalSlipSystemsList.Items.Refresh();
                }
            }
            catch
            {

            }
        }

        private void SlipFamilyActive_Changed(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.IndexEventAktive && this.SlipFamilyList.SelectedIndex != -1 && this.PhaseSwitchBox.SelectedIndex != -1 && this.ExperimentSelection.SelectedIndex != -1)
                {
                    if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].CrystalData.SymmetryGroupID == 225)
                    {
                        //FCC
                        for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems.Count; n++)
                        {
                            if (Convert.ToBoolean(this.SlipFamilyActive.IsChecked))
                            {
                                this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 1;
                            }
                            else
                            {
                                this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 0;
                            }
                        }
                    }
                    else if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].CrystalData.SymmetryGroupID == 227)
                    {
                        //FCC
                        for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems.Count; n++)
                        {
                            if (Convert.ToBoolean(this.SlipFamilyActive.IsChecked))
                            {
                                this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 1;
                            }
                            else
                            {
                                this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 0;
                            }
                        }
                    }
                    else if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].CrystalData.SymmetryGroupID == 229)
                    {
                        //BCC
                        switch (this.SlipFamilyList.SelectedIndex)
                        {
                            case 0:
                                for (int n = 0; n < 12; n++)
                                {
                                    if (Convert.ToBoolean(this.SlipFamilyActive.IsChecked))
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 1;
                                    }
                                    else
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 0;
                                    }
                                }
                                break;
                            case 1:
                                for (int n = 12; n < 25; n++)
                                {
                                    if (Convert.ToBoolean(this.SlipFamilyActive.IsChecked))
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 1;
                                    }
                                    else
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 0;
                                    }
                                }
                                break;
                            case 2:
                                for (int n = 25; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems.Count; n++)
                                {
                                    if (Convert.ToBoolean(this.SlipFamilyActive.IsChecked))
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 1;
                                    }
                                    else
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 0;
                                    }
                                }
                                break;
                            default:
                                for (int n = 0; n < 12; n++)
                                {
                                    if (Convert.ToBoolean(this.SlipFamilyActive.IsChecked))
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 1;
                                    }
                                    else
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 0;
                                    }
                                }
                                break;
                        }
                    }
                    else if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].CrystalData.SymmetryGroupID == 194)
                    {
                        //HCP
                        switch (this.SlipFamilyList.SelectedIndex)
                        {
                            case 0:
                                for (int n = 0; n < 3; n++)
                                {
                                    if (Convert.ToBoolean(this.SlipFamilyActive.IsChecked))
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 1;
                                    }
                                    else
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 0;
                                    }
                                }
                                break;
                            case 1:
                                for (int n = 3; n < 6; n++)
                                {
                                    if (Convert.ToBoolean(this.SlipFamilyActive.IsChecked))
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 1;
                                    }
                                    else
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 0;
                                    }
                                }
                                break;
                            case 2:
                                for (int n = 6; n < 9; n++)
                                {
                                    if (Convert.ToBoolean(this.SlipFamilyActive.IsChecked))
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 1;
                                    }
                                    else
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 0;
                                    }
                                }
                                break;
                            case 3:
                                for (int n = 9; n < 12; n++)
                                {
                                    if (Convert.ToBoolean(this.SlipFamilyActive.IsChecked))
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 1;
                                    }
                                    else
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 0;
                                    }
                                }
                                break;
                            case 4:
                                for (int n = 12; n < 15; n++)
                                {
                                    if (Convert.ToBoolean(this.SlipFamilyActive.IsChecked))
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 1;
                                    }
                                    else
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 0;
                                    }
                                }
                                break;
                            case 5:
                                for (int n = 15; n < 18; n++)
                                {
                                    if (Convert.ToBoolean(this.SlipFamilyActive.IsChecked))
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 1;
                                    }
                                    else
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 0;
                                    }
                                }
                                break;
                            case 6:
                                for (int n = 18; n < 21; n++)
                                {
                                    if (Convert.ToBoolean(this.SlipFamilyActive.IsChecked))
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 1;
                                    }
                                    else
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 0;
                                    }
                                }
                                break;
                            case 7:
                                for (int n = 21; n < 24; n++)
                                {
                                    if (Convert.ToBoolean(this.SlipFamilyActive.IsChecked))
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 1;
                                    }
                                    else
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 0;
                                    }
                                }
                                for (int n = 30; n < 33; n++)
                                {
                                    if (Convert.ToBoolean(this.SlipFamilyActive.IsChecked))
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 1;
                                    }
                                    else
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 0;
                                    }
                                }
                                break;
                            case 8:
                                for (int n = 24; n < 30; n++)
                                {
                                    if (Convert.ToBoolean(this.SlipFamilyActive.IsChecked))
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 1;
                                    }
                                    else
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 0;
                                    }
                                }
                                for (int n = 33; n < 39; n++)
                                {
                                    if (Convert.ToBoolean(this.SlipFamilyActive.IsChecked))
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 1;
                                    }
                                    else
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 0;
                                    }
                                }
                                break;
                            case 9:
                                for (int n = 39; n < 42; n++)
                                {
                                    if (Convert.ToBoolean(this.SlipFamilyActive.IsChecked))
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 1;
                                    }
                                    else
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 0;
                                    }
                                }
                                for (int n = 48; n < 51; n++)
                                {
                                    if (Convert.ToBoolean(this.SlipFamilyActive.IsChecked))
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 1;
                                    }
                                    else
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 0;
                                    }
                                }
                                break;
                            case 10:
                                for (int n = 42; n < 48; n++)
                                {
                                    if (Convert.ToBoolean(this.SlipFamilyActive.IsChecked))
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 1;
                                    }
                                    else
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 0;
                                    }
                                }
                                for (int n = 51; n < 57; n++)
                                {
                                    if (Convert.ToBoolean(this.SlipFamilyActive.IsChecked))
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 1;
                                    }
                                    else
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 0;
                                    }
                                }
                                break;
                            case 11:
                                for (int n = 57; n < 63; n++)
                                {
                                    if (Convert.ToBoolean(this.SlipFamilyActive.IsChecked))
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 1;
                                    }
                                    else
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 0;
                                    }
                                }
                                break;
                            case 12:
                                for (int n = 63; n < 66; n++)
                                {
                                    if (Convert.ToBoolean(this.SlipFamilyActive.IsChecked))
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 1;
                                    }
                                    else
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 0;
                                    }
                                }
                                for (int n = 69; n < 72; n++)
                                {
                                    if (Convert.ToBoolean(this.SlipFamilyActive.IsChecked))
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 1;
                                    }
                                    else
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 0;
                                    }
                                }
                                break;
                            case 13:
                                for (int n = 66; n < 69; n++)
                                {
                                    if (Convert.ToBoolean(this.SlipFamilyActive.IsChecked))
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 1;
                                    }
                                    else
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 0;
                                    }
                                }
                                for (int n = 72; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems.Count; n++)
                                {
                                    if (Convert.ToBoolean(this.SlipFamilyActive.IsChecked))
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 1;
                                    }
                                    else
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 0;
                                    }
                                }
                                break;
                            default:
                                for (int n = 0; n < 3; n++)
                                {
                                    if (Convert.ToBoolean(this.SlipFamilyActive.IsChecked))
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 1;
                                    }
                                    else
                                    {
                                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[n].ActiveSystem = 0;
                                    }
                                }
                                break;
                        }
                    }

                    this.PotenitalSlipSystemsList.Items.Refresh();
                }
            }
            catch
            {

            }
        }

        private void ReflexList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(this.ReflexList.SelectedIndex != -1)
            {
                IndexEventAktive = false;

                SetExperimentalReflexViewPsy();
                
                IndexEventAktive = true;
            }
        }
        
        private void SetAnnotations(ReflexYield RY)
        {
            this.AnnotationEventsActive = false;

            this.YieldPlotModel.Annotations.Clear();
            double psiAngletmp = Convert.ToDouble(PsiAnglem.Text);

            double ELB = RY.LowerElasticBorder(psiAngletmp);
            double EHB = RY.UpperElasticBorder(psiAngletmp);
            double PLB = RY.LowerPlasticBorder(psiAngletmp);
            double PHB = RY.UpperPlasticBorder(psiAngletmp);

            //this.ElasticLow.Text = ELB.ToString("e3");
            //this.ElasticHigh.Text = EHB.ToString("e3");
            //this.PlasticLow.Text = PLB.ToString("e3");
            //this.PlasticHigh.Text = PHB.ToString("e3");

            var MarkerLineEL = new OxyPlot.Annotations.LineAnnotation();
            MarkerLineEL.Color = OxyPlot.OxyColors.DarkGreen;
            MarkerLineEL.ClipByYAxis = true;
            MarkerLineEL.X = ELB;
            MarkerLineEL.Type = OxyPlot.Annotations.LineAnnotationType.Vertical;
            MarkerLineEL.StrokeThickness = CalScec.Properties.Settings.Default.PeakMarkingThickness;
            MarkerLineEL.Text = "Lower elastic border";
            MarkerLineEL.ClipText = true;
            MarkerLineEL.LineStyle = OxyPlot.LineStyle.Dash;

            var MarkerLineEH = new OxyPlot.Annotations.LineAnnotation();
            MarkerLineEH.Color = OxyPlot.OxyColors.DarkGreen;
            MarkerLineEH.ClipByYAxis = true;
            MarkerLineEH.X = EHB;
            MarkerLineEH.Type = OxyPlot.Annotations.LineAnnotationType.Vertical;
            MarkerLineEH.StrokeThickness = CalScec.Properties.Settings.Default.PeakMarkingThickness;
            MarkerLineEH.Text = "Upper elastic border";
            MarkerLineEH.ClipText = true;
            MarkerLineEH.LineStyle = OxyPlot.LineStyle.Dash;

            var MarkerLinePL = new OxyPlot.Annotations.LineAnnotation();
            MarkerLinePL.Color = OxyPlot.OxyColors.DarkRed;
            MarkerLinePL.ClipByYAxis = true;
            MarkerLinePL.X = EHB;
            MarkerLinePL.Type = OxyPlot.Annotations.LineAnnotationType.Vertical;
            MarkerLinePL.StrokeThickness = CalScec.Properties.Settings.Default.PeakMarkingThickness;
            MarkerLinePL.Text = "Lower plastic border";
            MarkerLinePL.ClipText = true;
            MarkerLinePL.LineStyle = OxyPlot.LineStyle.Dash;

            var MarkerLinePH = new OxyPlot.Annotations.LineAnnotation();
            MarkerLinePH.Color = OxyPlot.OxyColors.DarkRed;
            MarkerLinePH.ClipByYAxis = true;
            MarkerLinePH.X = EHB;
            MarkerLinePH.Type = OxyPlot.Annotations.LineAnnotationType.Vertical;
            MarkerLinePH.StrokeThickness = CalScec.Properties.Settings.Default.PeakMarkingThickness;
            MarkerLinePH.Text = "Upper plastic border";
            MarkerLinePH.ClipText = true;
            MarkerLinePH.LineStyle = OxyPlot.LineStyle.Dash;

            this.YieldPlotModel.Annotations.Add(MarkerLineEL);
            this.YieldPlotModel.Annotations.Add(MarkerLineEH);
            this.YieldPlotModel.Annotations.Add(MarkerLinePL);
            this.YieldPlotModel.Annotations.Add(MarkerLinePH);

            this.AnnotationEventsActive = true;
        }

        private void ReflexGroup_Click(object sender, RoutedEventArgs e)
        {
            this.ActSample.SetExperimentalData();
            this.ActSample.SetExperimentalStrainData();

            this.SetExperimentalReflexView();
            //this.SetYieldSurface();

            ////ReflexList.ItemsSource = this.ActSample.YieldSurfaceData[0].ReflexYieldData;

            //for(int n = 0; n < this.ActSample.PlasticTensor.Count; n++)
            //{
            //    for(int i = 0; i < this.ActSample.PlasticTensor[n].YieldSurfaceData.ReflexYieldData.Count; i++)
            //    {
            //        if (this.ActSample.CrystalData[n].SymmetryGroupID == 194)
            //        {
            //            this.ActSample.PlasticTensor[n].YieldSurfaceData.ReflexYieldData[i].SetSlipDirectionAngles(this.ActSample, 1);
            //        }
            //        else
            //        {
            //            this.ActSample.PlasticTensor[n].YieldSurfaceData.ReflexYieldData[i].SetSlipDirectionAngles(this.ActSample, 0);
            //        }
            //    }
            //}

            #region oldstuff

            //for(int n = 0; n < this.ActSample.DiffractionPatterns.Count; n++)
            //{
            //    for(int i = 0; i < this.ActSample.DiffractionPatterns[n].FoundPeaks.Count; i++)
            //    {
            //        for( int j = 0; j < this.ActSample.CrystalData.Count; j++)
            //        {
            //            if(this.ActSample.DiffractionPatterns[n].FoundPeaks[i].AssociatedCrystalData.SymmetryGroup == this.ActSample.CrystalData[j].SymmetryGroup)
            //            {
            //                bool found = false;

            //                for(int k = 0; k < this.ActSample.YieldSurfaceData[j].ReflexYieldData.Count; k++)
            //                {
            //                    if(this.ActSample.DiffractionPatterns[n].FoundPeaks[i].AssociatedHKLReflex.HKLString == this.ActSample.YieldSurfaceData[j].ReflexYieldData[k].SlipPlane.HKLString)
            //                    {
            //                        Stress.Macroskopic.PeakStressAssociation PSATmp = new Macroskopic.PeakStressAssociation(this.ActSample.DiffractionPatterns[n].Stress, this.ActSample.DiffractionPatterns[n].PsiAngle(this.ActSample.DiffractionPatterns[n].FoundPeaks[i].Angle), this.ActSample.DiffractionPatterns[n].PhiAngle(this.ActSample.DiffractionPatterns[n].FoundPeaks[i].Angle), this.ActSample.DiffractionPatterns[n].MacroStrain, true, this.ActSample.DiffractionPatterns[n].FoundPeaks[i]);
            //                        PSATmp.MainSlipDirectionAngle = this.ActSample.DiffractionPatterns[n].SlipDirectionAngle(this.ActSample.DiffractionPatterns[n].FoundPeaks[i].Angle, this.ActSample.YieldSurfaceData[j].ReflexYieldData[k].SlipPlane, this.ActSample.YieldSurfaceData[j].ReflexYieldData[k].MainSlipDirection);
            //                        PSATmp.SecondarySlipDirectionAngle = this.ActSample.DiffractionPatterns[n].SlipDirectionAngle(this.ActSample.DiffractionPatterns[n].FoundPeaks[i].Angle, this.ActSample.YieldSurfaceData[j].ReflexYieldData[k].SlipPlane, this.ActSample.YieldSurfaceData[j].ReflexYieldData[k].SecondarySlipDirection);
            //                        this.ActSample.YieldSurfaceData[j].ReflexYieldData[k].PeakData.Add(PSATmp);

            //                        found = true;
            //                        break;
            //                    }
            //                }

            //                if(!found)
            //                {
            //                    ReflexYield RYieldTmp = new ReflexYield(this.ActSample.DiffractionPatterns[n].FoundPeaks[i].AssociatedHKLReflex, this.ActSample.CrystalData[j]);

            //                    Stress.Macroskopic.PeakStressAssociation PSATmp = new Macroskopic.PeakStressAssociation(this.ActSample.DiffractionPatterns[n].Stress, this.ActSample.DiffractionPatterns[n].PsiAngle(this.ActSample.DiffractionPatterns[n].FoundPeaks[i].Angle), this.ActSample.DiffractionPatterns[n].PhiAngle(this.ActSample.DiffractionPatterns[n].FoundPeaks[i].Angle), this.ActSample.DiffractionPatterns[n].MacroStrain, true, this.ActSample.DiffractionPatterns[n].FoundPeaks[i]);
            //                    PSATmp.MainSlipDirectionAngle = this.ActSample.DiffractionPatterns[n].SlipDirectionAngle(this.ActSample.DiffractionPatterns[n].FoundPeaks[i].Angle, RYieldTmp.SlipPlane, RYieldTmp.MainSlipDirection);
            //                    PSATmp.SecondarySlipDirectionAngle = this.ActSample.DiffractionPatterns[n].SlipDirectionAngle(this.ActSample.DiffractionPatterns[n].FoundPeaks[i].Angle, RYieldTmp.SlipPlane, RYieldTmp.SecondarySlipDirection);
            //                    RYieldTmp.PeakData.Add(PSATmp);

            //                    this.ActSample.YieldSurfaceData[j].ReflexYieldData.Add(RYieldTmp);
            //                }
            //            }
            //        }
            //    }
            //}

            #endregion
        }

        private void SetExperimentalReflexView()
        {
            if (this.PhaseSwitchBox.SelectedIndex != -1 && this.ActSample.SortedExperimentalPeakData.Count != 0)
            {
                List<Macroskopic.PeakStressAssociation> tmp = new List<Macroskopic.PeakStressAssociation>();

                for (int n = 0; n < this.ActSample.SortedExperimentalPeakData[this.PhaseSwitchBox.SelectedIndex].Count; n++)
                {
                    tmp.Add(this.ActSample.SortedExperimentalPeakData[this.PhaseSwitchBox.SelectedIndex][n][0][0]);
                }

                ReflexList.ItemsSource = tmp;
                ReflexList.Items.Refresh();
            }
        }

        private void SetExperimentalReflexViewPsy()
        {
            if (this.PhaseSwitchBox.SelectedIndex != -1)
            {

                if (this.ReflexList.SelectedIndex != -1)
                {
                    int actIndex = PsiAnglem.SelectedIndex;
                    this.PsiAnglem.Items.Clear();

                    for (int n = 0; n < this.ActSample.SortedExperimentalPeakData[this.PhaseSwitchBox.SelectedIndex][this.ReflexList.SelectedIndex].Count; n++)
                    {
                        double tmp = this.ActSample.SortedExperimentalPeakData[this.PhaseSwitchBox.SelectedIndex][this.ReflexList.SelectedIndex][n][0].PsiAngle;

                        ComboBoxItem PsiAngleCBI = new ComboBoxItem();
                        PsiAngleCBI.Content = tmp.ToString("F3");

                        this.PsiAnglem.Items.Add(PsiAngleCBI);
                    }

                    if (actIndex != -1)
                    {
                        PsiAnglem.SelectedIndex = actIndex;
                        ReflexListPsi.ItemsSource = this.ActSample.SortedExperimentalPeakData[this.PhaseSwitchBox.SelectedIndex][this.ReflexList.SelectedIndex][actIndex];
                    }
                }
            }
        }

        private void SetYieldSurface()
        {
            for (int n = 0; n < this.ActSample.CrystalData.Count; n++)
            {
                //YieldSurface ySTmp = new YieldSurface(this.ActSample.CrystalData[n]);

                ////List<ReflexYield> rYieldList = new List<ReflexYield>();
                //this.ActSample.ReussTensorData[n].SetPeakStressAssociation(this.ActSample);
                List<List<List<Macroskopic.PeakStressAssociation>>> allPAPhase = this.ActSample.ReussTensorData[n].SetStrainDataReflexYield();

                for (int i = 0; i < allPAPhase.Count; i++)
                {
                    for (int j = 0; j < this.ActSample.PlasticTensor[n].YieldSurfaceData.ReflexYieldData.Count; j++)
                    {
                        if (allPAPhase[i][0][0].DPeak.AssociatedHKLReflex.HKLString == this.ActSample.PlasticTensor[n].YieldSurfaceData.ReflexYieldData[j].SlipPlane.HKLString)
                        {
                            this.ActSample.PlasticTensor[n].YieldSurfaceData.ReflexYieldData[j].PeakData = allPAPhase[i];
                            break;
                        }
                    }
                }

                for (int i = 0; i < this.ActSample.PlasticTensor[n].YieldSurfaceData.ReflexYieldData.Count; i++)
                {
                    if (this.ActSample.PlasticTensor[n].YieldSurfaceData.ReflexYieldData[i].PeakData.Count == 0)
                    {
                        this.ActSample.PlasticTensor[n].YieldSurfaceData.ReflexYieldData.RemoveAt(i);
                        i--;
                    }
                }

                //this.ActSample.PlasticTensor[n].YieldSurfaceData = this.ActSample.PlasticTensor[i].YieldSurfaceData;
            }
        }

        private void FitYieldStrenght_Click(object sender, RoutedEventArgs e)
        {
            if (this.ReflexList.SelectedIndex != -1)
            {
                ReflexYield SelectedRY = (ReflexYield)this.ReflexList.SelectedItem;

                double psiAngletmp = Convert.ToDouble(PsiAnglem.Text);

                SelectedRY.FitElasticData(psiAngletmp);
                SelectedRY.FitPlasticData(psiAngletmp);

                this.ReflexList.Items.Refresh();
            }
        }

        private void PsiAnglem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(PsiAnglem.SelectedIndex != -1 && this.PhaseSwitchBox.SelectedIndex != -1 && this.ReflexList.SelectedIndex != -1)
            {
                ReflexListPsi.ItemsSource = this.ActSample.SortedExperimentalPeakData[this.PhaseSwitchBox.SelectedIndex][this.ReflexList.SelectedIndex][PsiAnglem.SelectedIndex];
            }
        }
        
        private void ShowTestData_Click(object sender, RoutedEventArgs e)
        {
            if(this.PhaseSwitchBox.SelectedIndex != -1)
            {
                if (this.ReflexList.SelectedIndex != -1)
                {
                    if (PsiAnglem.SelectedIndex != -1)
                    {
                        Stress.Macroskopic.PeakStressAssociation selectedReflex = this.ReflexList.SelectedItem as Stress.Macroskopic.PeakStressAssociation;
                        OxyPlot.Series.LineSeries tmpData = new OxyPlot.Series.LineSeries();
                        tmpData.Title = selectedReflex.HKLAssociation + " " + PsiAnglem.Text;
                        
                    
                        try
                        {
                            tmpData.StrokeThickness = Convert.ToDouble(this.plotSettingsWindow.ExpLineThickness.Text);
                            tmpData.MarkerSize = Convert.ToDouble(this.plotSettingsWindow.ExpMarkerSize.Text);
                        }
                        catch
                        {
                            tmpData.StrokeThickness = 2;
                            tmpData.MarkerSize = 10;
                        }
                        switch(this.plotSettingsWindow.ExpLineType.SelectedIndex)
                        {
                            case 0:
                                tmpData.LineStyle = OxyPlot.LineStyle.None;
                                break;
                            case 1:
                                tmpData.LineStyle = OxyPlot.LineStyle.Solid;
                                break;
                            case 2:
                                tmpData.LineStyle = OxyPlot.LineStyle.Dash;
                                break;
                            case 3:
                                tmpData.LineStyle = OxyPlot.LineStyle.Dot;
                                break;
                            default:
                                tmpData.LineStyle = OxyPlot.LineStyle.None;
                                break;
                        }
                        switch (this.plotSettingsWindow.ExpMarkerType.SelectedIndex)
                        {
                            case 0:
                                tmpData.MarkerType = OxyPlot.MarkerType.Circle;
                                break;
                            case 1:
                                tmpData.MarkerType = OxyPlot.MarkerType.Cross;
                                break;
                            case 2:
                                tmpData.MarkerType = OxyPlot.MarkerType.Plus;
                                break;
                            default:
                                tmpData.MarkerType = OxyPlot.MarkerType.Circle;
                                break;
                        }
                        //tmpData.Color = OxyPlot.OxyColors.Black;
                        //tmpData.MarkerStroke = OxyPlot.OxyColors.Black;

                        double Xmin = double.MaxValue;
                        double Xmax = double.MinValue;

                        double Ymin = double.MaxValue;
                        double Ymax = double.MinValue;

                        double xOffset = 0.0;
                        double yOffset = 0.0;

                        try
                        {
                            xOffset = Convert.ToDouble(this.DiffractionXOffset.Text);
                            yOffset = Convert.ToDouble(this.DiffractionYOffset.Text);
                        }
                        catch
                        {

                        }

                        double baseValue = 0.0;

                        for (int n = 0; n < this.ActSample.SortedExperimentalPeakData[this.PhaseSwitchBox.SelectedIndex][this.ReflexList.SelectedIndex][PsiAnglem.SelectedIndex].Count; n++)
                        {
                            double xValueTmp = 0.0;
                            double yValueTmp = 0.0;
                            if(yAxesm.SelectedIndex == 4)
                            {
                                yValueTmp = this.ActSample.SortedExperimentalPeakData[this.PhaseSwitchBox.SelectedIndex][this.ReflexList.SelectedIndex][PsiAnglem.SelectedIndex][n].Strain;
                            }
                            else if (yAxesm.SelectedIndex == 1)
                            {
                                yValueTmp = this.ActSample.SortedExperimentalPeakData[this.PhaseSwitchBox.SelectedIndex][this.ReflexList.SelectedIndex][PsiAnglem.SelectedIndex][n].MacroskopicStrain;
                            }
                            else if (yAxesm.SelectedIndex == 8)
                            {
                                if(n == 0)
                                {
                                    baseValue = this.ActSample.SortedExperimentalPeakData[this.PhaseSwitchBox.SelectedIndex][this.ReflexList.SelectedIndex][PsiAnglem.SelectedIndex][n].DPeak.PFunction.FWHM;
                                }
                                double valTmp = this.ActSample.SortedExperimentalPeakData[this.PhaseSwitchBox.SelectedIndex][this.ReflexList.SelectedIndex][PsiAnglem.SelectedIndex][n].DPeak.PFunction.FWHM;
                                yValueTmp = valTmp - baseValue;
                                yValueTmp /= baseValue;
                            }
                            else if (yAxesm.SelectedIndex == 9)
                            {
                                if (n == 0)
                                {
                                    baseValue = this.ActSample.SortedExperimentalPeakData[this.PhaseSwitchBox.SelectedIndex][this.ReflexList.SelectedIndex][PsiAnglem.SelectedIndex][n].DPeak.PFunction.Intensity;
                                }
                                double valTmp = this.ActSample.SortedExperimentalPeakData[this.PhaseSwitchBox.SelectedIndex][this.ReflexList.SelectedIndex][PsiAnglem.SelectedIndex][n].DPeak.PFunction.Intensity;
                                yValueTmp = valTmp - baseValue;
                                yValueTmp /= baseValue;
                            }
                            else
                            {
                                yValueTmp = this.ActSample.SortedExperimentalPeakData[this.PhaseSwitchBox.SelectedIndex][this.ReflexList.SelectedIndex][PsiAnglem.SelectedIndex][n].Stress;

                                if(Convert.ToBoolean(this.DiffractionAreaCorrectionActive.IsChecked))
                                {
                                    yValueTmp *= 1 + this.ActSample.SortedExperimentalPeakData[this.PhaseSwitchBox.SelectedIndex][this.ReflexList.SelectedIndex][PsiAnglem.SelectedIndex][n].MacroskopicStrain;
                                    //yValueTmp /= Math.Pow(1 - this.ActSample.SortedExperimentalPeakData[this.PhaseSwitchBox.SelectedIndex][this.ReflexList.SelectedIndex][PsiAnglem.SelectedIndex][n].MacroskopicStrain, 2);
                                }
                            }

                            if (xAxesm.SelectedIndex == 0)
                            {
                                xValueTmp = this.ActSample.SortedExperimentalPeakData[this.PhaseSwitchBox.SelectedIndex][this.ReflexList.SelectedIndex][PsiAnglem.SelectedIndex][n].Stress;

                                if (Convert.ToBoolean(this.DiffractionAreaCorrectionActive.IsChecked))
                                {
                                    yValueTmp *= 1 + this.ActSample.SortedExperimentalPeakData[this.PhaseSwitchBox.SelectedIndex][this.ReflexList.SelectedIndex][PsiAnglem.SelectedIndex][n].MacroskopicStrain;
                                    //xValueTmp /= Math.Pow(1 - this.ActSample.SortedExperimentalPeakData[this.PhaseSwitchBox.SelectedIndex][this.ReflexList.SelectedIndex][PsiAnglem.SelectedIndex][n].MacroskopicStrain, 2);
                                }
                            }
                            else if (xAxesm.SelectedIndex == 1)
                            {
                                xValueTmp = this.ActSample.SortedExperimentalPeakData[this.PhaseSwitchBox.SelectedIndex][this.ReflexList.SelectedIndex][PsiAnglem.SelectedIndex][n].MacroskopicStrain;
                            }
                            else
                            {
                                xValueTmp = this.ActSample.SortedExperimentalPeakData[this.PhaseSwitchBox.SelectedIndex][this.ReflexList.SelectedIndex][PsiAnglem.SelectedIndex][n].Strain;
                            }

                            OxyPlot.DataPoint eDP = new OxyPlot.DataPoint(xValueTmp- xOffset, yValueTmp - yOffset);

                            tmpData.Points.Add(eDP);

                            if (Xmin > xValueTmp)
                            {
                                Xmin = xValueTmp;
                            }
                            if (Ymin > yValueTmp)
                            {
                                Ymin = yValueTmp;
                            }

                            if (Xmax < xValueTmp)
                            {
                                Xmax = xValueTmp;
                            }
                            if (Ymax < yValueTmp)
                            {
                                Ymax = yValueTmp;
                            }
                        }

                        this.YieldXAxisLin.Minimum = Xmin;
                        this.YieldXAxisLin.Maximum = Xmax;
                        this.YieldYAxisLin.Minimum = Ymin;
                        this.YieldYAxisLin.Maximum = Ymax;

                        this.YieldPlotModel.Series.Add(tmpData);

                        this.PW.MainPlot.ResetAllAxes();
                        this.PW.MainPlot.InvalidatePlot(true);
                    }
                }
            }
        }

        private void PlotTensileTest_Click(object sender, RoutedEventArgs e)
        {
            if (this.MacroExperimentPlotList.SelectedIndex != -1)
            {
                double offSetX = 0.0;
                double offSetY = 0.0;
                try
                {
                    offSetX = Convert.ToDouble(this.MarcorPlotOffSetX.Text);
                    offSetY = Convert.ToDouble(this.MarcorPlotOffSetY.Text);
                }
                catch
                {

                }

                OxyPlot.Series.LineSeries tmpData = new OxyPlot.Series.LineSeries();
                tmpData.Title = "Tensile Test, executed: " + this.ActSample.TensileTests[this.MacroExperimentPlotList.SelectedIndex].ExecutedDisplay;

                tmpData.LineStyle = OxyPlot.LineStyle.None;
                tmpData.StrokeThickness = 0;
                tmpData.MarkerSize = 5;
                tmpData.MarkerType = OxyPlot.MarkerType.Cross;
                tmpData.Color = OxyPlot.OxyColors.Black;
                tmpData.MarkerStroke = OxyPlot.OxyColors.Black;

                try
                {
                    tmpData.StrokeThickness = Convert.ToDouble(this.plotSettingsWindow.TensileLineThickness.Text);
                    tmpData.MarkerSize = Convert.ToDouble(this.plotSettingsWindow.TensileMarkerSize.Text);
                }
                catch
                {
                    tmpData.StrokeThickness = 0;
                    tmpData.MarkerSize = 10;
                }
                switch (this.plotSettingsWindow.TensileLineType.SelectedIndex)
                {
                    case 0:
                        tmpData.LineStyle = OxyPlot.LineStyle.None;
                        break;
                    case 1:
                        tmpData.LineStyle = OxyPlot.LineStyle.Solid;
                        break;
                    case 2:
                        tmpData.LineStyle = OxyPlot.LineStyle.Dash;
                        break;
                    case 3:
                        tmpData.LineStyle = OxyPlot.LineStyle.Dot;
                        break;
                    default:
                        tmpData.LineStyle = OxyPlot.LineStyle.None;
                        break;
                }
                switch (this.plotSettingsWindow.TensileMarkerType.SelectedIndex)
                {
                    case 0:
                        tmpData.MarkerType = OxyPlot.MarkerType.Circle;
                        break;
                    case 1:
                        tmpData.MarkerType = OxyPlot.MarkerType.Cross;
                        break;
                    case 2:
                        tmpData.MarkerType = OxyPlot.MarkerType.Plus;
                        break;
                    default:
                        tmpData.MarkerType = OxyPlot.MarkerType.Cross;
                        break;
                }

                double Xmin = double.MaxValue;
                double Xmax = double.MinValue;

                double Ymin = double.MaxValue;
                double Ymax = double.MinValue;

                List<double> xValues = new List<double>();
                List<double> yValues = new List<double>();

                switch (xAxesMacro.SelectedIndex)
                {
                    case 0:
                        xValues = this.ActSample.TensileTests[this.MacroExperimentPlotList.SelectedIndex].TimeData;
                        this.YieldXAxisLin.Title = "Time (s)";
                        break;
                    case 1:
                        xValues = this.ActSample.TensileTests[this.MacroExperimentPlotList.SelectedIndex].StressData;
                        this.YieldXAxisLin.Title = "Stress (MPa)";
                        break;
                    case 2:
                        xValues = this.ActSample.TensileTests[this.MacroExperimentPlotList.SelectedIndex].StrainData;
                        this.YieldXAxisLin.Title = "Strain";
                        break;
                    case 3:
                        xValues = this.ActSample.TensileTests[this.MacroExperimentPlotList.SelectedIndex].ForceData;
                        this.YieldXAxisLin.Title = "Force (N)";
                        break;
                    case 4:
                        xValues = this.ActSample.TensileTests[this.MacroExperimentPlotList.SelectedIndex].ExtensionData;
                        this.YieldXAxisLin.Title = "Extension (mm)";
                        break;
                    case 5:
                        for(int n = 0; n < this.ActSample.TensileTests[this.MacroExperimentPlotList.SelectedIndex].StressData.Count - 1; n++)
                        {
                            double tmp = this.ActSample.TensileTests[this.MacroExperimentPlotList.SelectedIndex].StressData[n + 1] - this.ActSample.TensileTests[this.MacroExperimentPlotList.SelectedIndex].StressData[n];
                            xValues.Add(tmp);
                        }
                        this.YieldXAxisLin.Title = "Stress Rate (MPa)";
                        break;
                    case 6:
                        for (int n = 0; n < this.ActSample.TensileTests[this.MacroExperimentPlotList.SelectedIndex].StrainData.Count - 1; n++)
                        {
                            double tmp = this.ActSample.TensileTests[this.MacroExperimentPlotList.SelectedIndex].StrainData[n + 1] - this.ActSample.TensileTests[this.MacroExperimentPlotList.SelectedIndex].StrainData[n];
                            xValues.Add(tmp);
                        }
                        this.YieldXAxisLin.Title = "Strain Rate";
                        break;
                    default:
                        xValues = this.ActSample.TensileTests[this.MacroExperimentPlotList.SelectedIndex].StressData;
                        this.YieldXAxisLin.Title = "Stress (MPa)";
                        break;
                }
                switch (yAxesMacro.SelectedIndex)
                {
                    case 0:
                        yValues = this.ActSample.TensileTests[this.MacroExperimentPlotList.SelectedIndex].TimeData;
                        this.YieldYAxisLin.Title = "Time (s)";
                        break;
                    case 1:
                        yValues = this.ActSample.TensileTests[this.MacroExperimentPlotList.SelectedIndex].StressData;
                        this.YieldYAxisLin.Title = "Stress (MPa)";
                        break;
                    case 2:
                        yValues = this.ActSample.TensileTests[this.MacroExperimentPlotList.SelectedIndex].StrainData;
                        this.YieldYAxisLin.Title = "Strain";
                        break;
                    case 3:
                        yValues = this.ActSample.TensileTests[this.MacroExperimentPlotList.SelectedIndex].ForceData;
                        this.YieldYAxisLin.Title = "Force (N)";
                        break;
                    case 4:
                        yValues = this.ActSample.TensileTests[this.MacroExperimentPlotList.SelectedIndex].ExtensionData;
                        this.YieldYAxisLin.Title = "Extension (mm)";
                        break;
                    case 5:
                        for (int n = 0; n < this.ActSample.TensileTests[this.MacroExperimentPlotList.SelectedIndex].StressData.Count - 1; n++)
                        {
                            double tmp = this.ActSample.TensileTests[this.MacroExperimentPlotList.SelectedIndex].StressData[n + 1] - this.ActSample.TensileTests[this.MacroExperimentPlotList.SelectedIndex].StressData[n];
                            yValues.Add(tmp);
                        }
                        this.YieldYAxisLin.Title = "Stress Rate (MPa)";
                        break;
                    case 6:
                        for (int n = 0; n < this.ActSample.TensileTests[this.MacroExperimentPlotList.SelectedIndex].StrainData.Count - 1; n++)
                        {
                            double tmp = this.ActSample.TensileTests[this.MacroExperimentPlotList.SelectedIndex].StrainData[n + 1] - this.ActSample.TensileTests[this.MacroExperimentPlotList.SelectedIndex].StrainData[n];
                            yValues.Add(tmp);
                        }
                        this.YieldYAxisLin.Title = "Strain Rate";
                        break;
                    default:
                        yValues = this.ActSample.TensileTests[this.MacroExperimentPlotList.SelectedIndex].StressData;
                        this.YieldYAxisLin.Title = "Stress (MPa)";
                        break;
                }

                int maxIndex = xValues.Count;
                if(maxIndex > yValues.Count)
                {
                    maxIndex = yValues.Count;
                }

                for(int n = 0; n < maxIndex; n++)
                {
                    double elasticStrainX = 0.0;
                    double elasticStrainY = 0.0;

                    if(Convert.ToBoolean(this.TensilePlasticOnlyActive.IsChecked))
                    {
                        if(xAxesMacro.SelectedIndex == 2)
                        {
                            elasticStrainX = this.ActSample.TensileTests[this.MacroExperimentPlotList.SelectedIndex].StressData[n] / this.ActSample.TensileTests[this.MacroExperimentPlotList.SelectedIndex].EModul;
                        }
                        if (yAxesMacro.SelectedIndex == 2)
                        {
                            elasticStrainY = this.ActSample.TensileTests[this.MacroExperimentPlotList.SelectedIndex].StressData[n] / this.ActSample.TensileTests[this.MacroExperimentPlotList.SelectedIndex].EModul;
                        }
                    }

                    OxyPlot.DataPoint eDP = new OxyPlot.DataPoint(xValues[n] - offSetX - elasticStrainX, yValues[n] - offSetY - elasticStrainY);

                    tmpData.Points.Add(eDP);

                    if (Xmin > xValues[n])
                    {
                        Xmin = xValues[n];
                    }
                    if (Ymin > yValues[n])
                    {
                        Ymin = yValues[n];
                    }

                    if (Xmax < xValues[n])
                    {
                        Xmax = xValues[n];
                    }
                    if (Ymax < yValues[n])
                    {
                        Ymax = yValues[n];
                    }
                }

                this.YieldXAxisLin.Minimum = Xmin;
                this.YieldXAxisLin.Maximum = Xmax;
                this.YieldYAxisLin.Minimum = Ymin;
                this.YieldYAxisLin.Maximum = Ymax;

                this.YieldPlotModel.Series.Add(tmpData);

                this.PW.MainPlot.ResetAllAxes();
                this.PW.MainPlot.InvalidatePlot(true);
            }
        }
        
        #region Simulation Data

        private void GrainOrientation_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetGrainContentLabel();
        }

        private void SetGrainContentLabel()
        {
            if (this.IndexEventAktive && this.ExperimentSelection.SelectedIndex != -1 && this.PhaseSwitchBox.SelectedIndex != -1)
            {
                try
                {
                    double phi1Start = Convert.ToDouble(this.Phi1Start.Text);
                    double phi1End = Convert.ToDouble(this.Phi1End.Text);
                    double phi1Step = Convert.ToDouble(this.Phi1Step.Text);
                    double psiStart = Convert.ToDouble(this.PsiStart.Text);
                    double psiEnd = Convert.ToDouble(this.PsiEnd.Text);
                    double psiStep = Convert.ToDouble(this.PsiStep.Text);
                    double phi2Start = Convert.ToDouble(this.Phi2Start.Text);
                    double phi2End = Convert.ToDouble(this.Phi2End.Text);
                    double phi2Step = Convert.ToDouble(this.Phi2Step.Text);

                    double phi1Count = (phi1End - phi1Start) / phi1Step;
                    double psiCount = (psiEnd - psiStart) / psiStep;
                    double phi2Count = (phi2End - phi2Start) / phi2Step;

                    int totalCount = Convert.ToInt32(phi1Count + 1) * Convert.ToInt32(psiCount + 1) * Convert.ToInt32(phi2Count + 1);

                    this.GrainCountLabel.Content = totalCount.ToString();
                }
                catch
                {
                    this.GrainCountLabel.Content = "XXX";
                }
            }
        }

        private void SetGrainsForExperiment_Click(object sender, RoutedEventArgs e)
        {
            if (this.ExperimentSelection.SelectedIndex != -1 && PhaseSwitchBox.SelectedIndex != -1)
                try
                {
                    double phi1Start = Convert.ToDouble(this.Phi1Start.Text);
                    double phi1End = Convert.ToDouble(this.Phi1End.Text);
                    double phi1Step = Convert.ToDouble(this.Phi1Step.Text);
                    double psiStart = Convert.ToDouble(this.PsiStart.Text);
                    double psiEnd = Convert.ToDouble(this.PsiEnd.Text);
                    double psiStep = Convert.ToDouble(this.PsiStep.Text);
                    double phi2Start = Convert.ToDouble(this.Phi2Start.Text);
                    double phi2End = Convert.ToDouble(this.Phi2End.Text);
                    double phi2Step = Convert.ToDouble(this.Phi2Step.Text);

                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GrainOrientations[PhaseSwitchBox.SelectedIndex].Clear();

                    for (double psi = psiStart; psi <= psiEnd; psi += psiStep)
                    {
                        for (double phi1 = phi1Start; phi1 <= phi1End; phi1 += phi1Step)
                        {
                            for (double phi2 = phi2Start; phi2 <= phi2End; phi2 += phi2Step)
                            {
                                this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GrainOrientations[PhaseSwitchBox.SelectedIndex].Add(new GrainOrientationParameter(phi1, psi, phi2));
                            }
                        }
                    }

                    this.GrainorientationList.ItemsSource = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GrainOrientations[this.PhaseSwitchBox.SelectedIndex];
                }
                catch
                {

                }
        }

        private void DriveToStress_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double ticks = Convert.ToInt32(this.ExperimentalTicks.Text);

                MathNet.Numerics.LinearAlgebra.Matrix<double> target = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

                if (ExperimentalDataSelection.SelectedIndex == 6)
                {
                    //Längsdehnung
                    target[2, 2] = Convert.ToDouble(ExperimentalValue33.Text);
                    //Volumen unberührt
                    target[0, 1] = Convert.ToDouble(ExperimentalValue12.Text);
                    target[0, 2] = Convert.ToDouble(ExperimentalValue13.Text);
                    target[1, 0] = Convert.ToDouble(ExperimentalValue12.Text);
                    target[2, 0] = Convert.ToDouble(ExperimentalValue13.Text);
                    target[1, 2] = Convert.ToDouble(ExperimentalValue23.Text);
                    target[2, 1] = Convert.ToDouble(ExperimentalValue23.Text);
                    //Volumen bleibt konstant step wird berechnet
                    double isoTarget = -1.0 * (target[2, 2] * 0.378);
                    //double isoTarget = -1.0;
                    //isoTarget += Math.Sqrt(1 / (1 + target[2, 2]));
                    target[0, 0] = isoTarget;
                    target[1, 1] = isoTarget;


                }
                else
                {
                    target[0, 0] = Convert.ToDouble(ExperimentalValue11.Text);
                    target[0, 1] = Convert.ToDouble(ExperimentalValue12.Text);
                    target[0, 2] = Convert.ToDouble(ExperimentalValue13.Text);
                    target[1, 0] = Convert.ToDouble(ExperimentalValue12.Text);
                    target[2, 0] = Convert.ToDouble(ExperimentalValue13.Text);
                    target[1, 1] = Convert.ToDouble(ExperimentalValue22.Text);
                    target[1, 2] = Convert.ToDouble(ExperimentalValue23.Text);
                    target[2, 1] = Convert.ToDouble(ExperimentalValue23.Text);
                    target[2, 2] = Convert.ToDouble(ExperimentalValue33.Text);
                }
                

                MathNet.Numerics.LinearAlgebra.Matrix<double> step = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

                MathNet.Numerics.LinearAlgebra.Matrix<double> last = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
                switch (ExperimentalDataSelection.SelectedIndex)
                {
                    case 0:
                        if (this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StressSFHistory.Count != 0)
                        {
                            last = this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StressSFHistory[this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StressSFHistory.Count - 1];
                            break;
                        }
                        goto default;
                    case 1:
                        if (this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StrainSFHistory.Count != 0)
                        {
                            last = this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StrainSFHistory[this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StrainSFHistory.Count - 1];
                            break;
                        }
                        goto default;
                    case 2:
                        if (this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StressRateSFHistory.Count != 0)
                        {
                            last = this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StressRateSFHistory[this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StressRateSFHistory.Count - 1];
                            break;
                        }
                        goto default;
                    case 3:
                        if (this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StrainRateSFHistory.Count != 0)
                        {
                            last = this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StrainRateSFHistory[this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StrainRateSFHistory.Count - 1];
                            break;
                        }
                        goto default;
                    default:
                        break;
                }

                step[0, 0] = target[0, 0] - last[0, 0];
                step[0, 1] = target[0, 1] - last[0, 1];
                step[0, 2] = target[0, 2] - last[0, 2];
                step[1, 0] = target[1, 0] - last[1, 0];
                step[1, 1] = target[1, 1] - last[1, 1];
                step[1, 2] = target[1, 2] - last[1, 2];
                step[2, 0] = target[2, 0] - last[2, 0];
                step[2, 1] = target[2, 1] - last[2, 1];
                step[2, 2] = target[2, 2] - last[2, 2];

                step /= ticks;

                for (int n = 0; n < ticks; n++)
                {
                    MathNet.Numerics.LinearAlgebra.Matrix<double> hist = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
                    
                    hist[0, 0] = last[0, 0] + ((n + 1) * step[0, 0]);
                    hist[0, 1] = last[0, 1] + ((n + 1) * step[0, 1]);
                    hist[0, 2] = last[0, 2] + ((n + 1) * step[0, 2]);
                    hist[1, 0] = last[1, 0] + ((n + 1) * step[1, 0]);
                    hist[1, 1] = last[1, 1] + ((n + 1) * step[1, 1]);
                    hist[1, 2] = last[1, 2] + ((n + 1) * step[1, 2]);
                    hist[2, 0] = last[2, 0] + ((n + 1) * step[2, 0]);
                    hist[2, 1] = last[2, 1] + ((n + 1) * step[2, 1]);
                    hist[2, 2] = last[2, 2] + ((n + 1) * step[2, 2]);


                    switch (ExperimentalDataSelection.SelectedIndex)
                    {
                        case 0:
                            this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StressSFHistory.Add(hist);
                            goto default;
                        case 1:
                            this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StrainSFHistory.Add(hist);
                            goto default;
                        case 2:
                            this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StressRateSFHistory.Add(hist);
                            goto default;
                        case 3:
                            this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StrainRateSFHistory.Add(hist);
                            goto default;
                        default:
                            break;
                    }
                }

                SetExpData();
            }
            catch
            {

            }
        }

        private void ExperimentalChiAngle_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].ChiAngle = Convert.ToDouble(ExperimentalChiAngle.Text);
            }
            catch
            {

            }
        }

        private void ExperimentalOmegaAngle_TextChanged(object sender, TextChangedEventArgs e)
        {

            try
            {
                this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].OmegaAngle = Convert.ToDouble(ExperimentalOmegaAngle.Text);
            }
            catch
            {

            }
        }

        private Pattern.Counts GetSelectedSimulationCounts(OxyPlot.Series.LineSeries TmpSimulated)
        {
            TmpSimulated.Title = "Simulated Experiment No: " + ExperimentSelection.SelectedIndex;
            Pattern.Counts usedCountsSimulated = new Pattern.Counts();

            DataManagment.CrystalData.HKLReflex mainDir = new DataManagment.CrystalData.HKLReflex(1, 0, 0, 1);
            DataManagment.CrystalData.HKLReflex secondDir = new DataManagment.CrystalData.HKLReflex(1, 0, 0, 1);

            try
            {
                mainDir = new DataManagment.CrystalData.HKLReflex(Convert.ToInt32(this.EViewDirectionC1.Text), Convert.ToInt32(this.EViewDirectionC2.Text), Convert.ToInt32(this.EViewDirectionC3.Text), 1);
                secondDir = new DataManagment.CrystalData.HKLReflex(Convert.ToInt32(this.EViewDirection2C1.Text), Convert.ToInt32(this.EViewDirection2C2.Text), Convert.ToInt32(this.EViewDirection2C3.Text), 1);
            }
            catch
            {
                mainDir = new DataManagment.CrystalData.HKLReflex(1, 0, 0, 1);
                secondDir = new DataManagment.CrystalData.HKLReflex(1, 0, 0, 1);
            }
            
            if (this.ExperimentSelection.SelectedIndex != -1 && this.PhaseSwitchBox.SelectedIndex != -1)
            {
                List<MathNet.Numerics.LinearAlgebra.Matrix<double>> xValues = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
                List<MathNet.Numerics.LinearAlgebra.Matrix<double>> yValues = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

                bool xSummation = false;
                bool ySummation = false;

                #region Axis selection

                //X-Axis
                //[0]: "Sample Stress";
                //[1]: "Sample Strain";
                //[2]: "Sample Stress Rate";
                //[3]: "Sample Strain Rate";
                //[4]: "Grain Stress";
                //[5]: "Grain Strain";
                //[6]: "Grain Stress Rate";
                //[7]: "Grain Strain Rate";
                //[8]: "Lattice Strain";
                //[9]: "Lattice Strain Rate";

                switch (this.xAxesm.SelectedIndex)
                {
                    case 0:
                        this.YieldXAxisLin.Title = "Sample Stress [MPa]";
                        xValues = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory;
                        break;
                    case 1:
                        this.YieldXAxisLin.Title = "Sample Strain";
                        xValues = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory;
                        break;
                    case 2:
                        this.YieldXAxisLin.Title = "Sample Stress Rate [MPa / Step]";
                        xValues = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory;
                        break;
                    case 3:
                        this.YieldXAxisLin.Title = "Sample Strain [1 / Step]";
                        xValues = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory;
                        break;
                    case 4:
                        if (this.GrainorientationList.SelectedIndex != -1)
                        {
                            this.YieldXAxisLin.Title = "Lattice Strain";
                            Tools.FourthRankTensor usedCompliances = new Tools.FourthRankTensor();
                            switch (this.CalculationModel.SelectedIndex)
                            {
                                case 0:
                                    usedCompliances = this.ActSample.ReussTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances();
                                    break;
                                case 1:
                                    usedCompliances = this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances();
                                    break;
                                case 2:
                                    usedCompliances = this.ActSample.KroenerTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances();
                                    break;
                                case 3:
                                    usedCompliances = this.ActSample.DeWittTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances();
                                    break;
                                case 4:
                                    usedCompliances = this.ActSample.GeometricHillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances();
                                    break;
                                default:
                                    usedCompliances = this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances();
                                    break;
                            }
                            xValues = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, this.GrainorientationList.SelectedIndex, usedCompliances);
                            xSummation = true;
                        }
                        break;
                    case 5:
                        if (this.GrainorientationList.SelectedIndex != -1)
                        {
                            this.YieldXAxisLin.Title = "Lattice Strain Rate [1 / Step]";
                            Tools.FourthRankTensor usedCompliances = new Tools.FourthRankTensor();
                            switch (this.CalculationModel.SelectedIndex)
                            {
                                case 0:
                                    usedCompliances = this.ActSample.ReussTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances();
                                    break;
                                case 1:
                                    usedCompliances = this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances();
                                    break;
                                case 2:
                                    usedCompliances = this.ActSample.KroenerTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances();
                                    break;
                                case 3:
                                    usedCompliances = this.ActSample.DeWittTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances();
                                    break;
                                case 4:
                                    usedCompliances = this.ActSample.GeometricHillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances();
                                    break;
                                default:
                                    usedCompliances = this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances();
                                    break;
                            }
                            xValues = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, this.GrainorientationList.SelectedIndex, usedCompliances);
                        }
                        break;
                    case 6:
                        if (this.GrainorientationList.SelectedIndex != -1)
                        {
                            this.YieldXAxisLin.Title = "Grain Stress [MPa / Step]";
                            for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
                            {
                                xValues.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
                            }
                            xSummation = true;
                        }
                        break;
                    case 7:
                        if (this.GrainorientationList.SelectedIndex != -1)
                        {
                            this.YieldXAxisLin.Title = "Grain Strain [1 / Step]";
                            for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
                            {
                                xValues.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
                            }
                            xSummation = true;
                        }
                        break;
                    case 8:
                        if (this.GrainorientationList.SelectedIndex != -1)
                        {
                            this.YieldXAxisLin.Title = "Grain Stress Rate [MPa / Step]";
                            for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
                            {
                                xValues.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
                            }
                        }
                        break;
                    case 9:
                        if (this.GrainorientationList.SelectedIndex != -1)
                        {
                            this.YieldXAxisLin.Title = "Grain Strain [1 / Step]";
                            for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
                            {
                                xValues.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
                            }
                        }
                        break;
                    default:
                        this.YieldXAxisLin.Title = "Sample Stress [MPa]";
                        xValues = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory;
                        break;
                }

                //Y-Axis
                //[0]: "Sample Stress";
                //[1]: "Sample Strain";
                //[2]: "Sample Stress Rate";
                //[3]: "Sample Strain Rate";
                //[4]: "Grain Stress";
                //[5]: "Grain Strain";
                //[6]: "Grain Stress Rate";
                //[7]: "Grain Strain Rate";
                //[8]: "Lattice Strain";
                //[9]: "Lattice Strain Rate";
                //[10]: "Slip Activity";
                //[11]: "Slip Activity Grain";
                switch (this.yAxesm.SelectedIndex)
                {
                    case 0:
                        this.YieldYAxisLin.Title = "Sample Stress [MPa]";
                        yValues = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory;
                        break;
                    case 1:
                        this.YieldYAxisLin.Title = "Sample Strain";
                        yValues = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory;
                        break;
                    case 2:
                        this.YieldYAxisLin.Title = "Sample Stress Rate [MPa / Step]";
                        yValues = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory;
                        break;
                    case 3:
                        this.YieldYAxisLin.Title = "Sample Strain Rate [1 / Step]";
                        yValues = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory;
                        break;
                    case 4:
                        if (this.GrainorientationList.SelectedIndex != -1)
                        {
                            this.YieldYAxisLin.Title = "Lattice Strain";
                            Tools.FourthRankTensor usedCompliances = new Tools.FourthRankTensor();
                            switch (this.CalculationModel.SelectedIndex)
                            {
                                case 0:
                                    usedCompliances = this.ActSample.ReussTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances();
                                    break;
                                case 1:
                                    usedCompliances = this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances();
                                    break;
                                case 2:
                                    usedCompliances = this.ActSample.KroenerTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances();
                                    break;
                                case 3:
                                    usedCompliances = this.ActSample.DeWittTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances();
                                    break;
                                case 4:
                                    usedCompliances = this.ActSample.GeometricHillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances();
                                    break;
                                default:
                                    usedCompliances = this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances();
                                    break;
                            }
                            yValues = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, this.GrainorientationList.SelectedIndex, usedCompliances);
                            ySummation = true;
                        }
                        break;
                    case 5:
                        if (this.GrainorientationList.SelectedIndex != -1)
                        {
                            this.YieldYAxisLin.Title = "Lattice Strain Rate [1 / Step]";
                            Tools.FourthRankTensor usedCompliances = new Tools.FourthRankTensor();
                            switch (this.CalculationModel.SelectedIndex)
                            {
                                case 0:
                                    usedCompliances = this.ActSample.ReussTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances();
                                    break;
                                case 1:
                                    usedCompliances = this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances();
                                    break;
                                case 2:
                                    usedCompliances = this.ActSample.KroenerTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances();
                                    break;
                                case 3:
                                    usedCompliances = this.ActSample.DeWittTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances();
                                    break;
                                case 4:
                                    usedCompliances = this.ActSample.GeometricHillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances();
                                    break;
                                default:
                                    usedCompliances = this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances();
                                    break;
                            }
                            yValues = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, this.GrainorientationList.SelectedIndex, usedCompliances);
                            
                        }
                        break;
                    case 6:
                        this.YieldYAxisLin.Title = "Slip System Activity [%]";

                        if ((bool)GroupSlipReflexesPlot.IsChecked)
                        {
                            //Selection check
                            if (SlipFamilyList.SelectedIndex != -1)
                            {
                                List<ReflexYield> slipFamilySystems = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.GetSlipSystemsFamily((ReflexYield)SlipFamilyList.SelectedItem); TmpSimulated.Title += slipFamilySystems[0].HKLString;
                                for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex].Count; n++)
                                {
                                    double totalActive = 0.0;
                                    double familyActive = 0.0;

                                    for (int i = 0; i < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n].Count; i++)
                                    {
                                        if ((bool)this.RelativeActivityPlot.IsChecked)
                                        {
                                            totalActive += slipFamilySystems.Count;
                                        }
                                        else
                                        {
                                            totalActive += this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.PotentialSlipSystems.Count;
                                        }

                                        for (int j = 0; j < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i].Count; j++)
                                        {
                                            for (int k = 0; k < slipFamilySystems.Count; k++)
                                            {
                                                if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i][j].SlipPlane.HKLString == slipFamilySystems[k].SlipPlane.HKLString)
                                                {
                                                    familyActive++;
                                                }
                                            }
                                        }
                                    }

                                    MathNet.Numerics.LinearAlgebra.Matrix<double> tmp = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense<double>(3, 3, 0);
                                    tmp[2, 2] = familyActive / totalActive;

                                    yValues.Add(tmp);
                                }
                            }
                        }
                        else
                        {
                            if (PotenitalSlipSystemsList.SelectedIndex != -1)
                            {
                                ReflexYield selectedStatReflex = PotenitalSlipSystemsList.SelectedItem as ReflexYield;
                                List<ReflexYield> slipFamilySystems = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.GetSlipSystemsFamily((ReflexYield)SlipFamilyList.SelectedItem);
                                TmpSimulated.Title += slipFamilySystems[0].HKLString;
                                for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex].Count; n++)
                                {
                                    double totalActive = 0.0;
                                    double familyActive = 0.0;

                                    for (int i = 0; i < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n].Count; i++)
                                    {
                                        if ((bool)this.RelativeActivityPlot.IsChecked)
                                        {
                                            totalActive += slipFamilySystems.Count;
                                        }
                                        else
                                        {
                                            totalActive += this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.PotentialSlipSystems.Count;
                                        }

                                        for (int j = 0; j < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i].Count; j++)
                                        {
                                            if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i][j].SlipPlane.HKLString == selectedStatReflex.SlipPlane.HKLString)
                                            {
                                                familyActive++;
                                            }
                                        }
                                    }

                                    MathNet.Numerics.LinearAlgebra.Matrix<double> tmp = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense<double>(3, 3, 0);
                                    tmp[2, 2] = familyActive / totalActive;

                                    yValues.Add(tmp);
                                }

                            }
                        }
                        break;
                    case 7:
                        this.YieldYAxisLin.Title = "Slip System Activity [%]";

                        if ((bool)GroupSlipReflexesPlot.IsChecked)
                        {
                            //Selection check
                            if (SlipFamilyList.SelectedIndex != -1 && this.GrainorientationList.SelectedIndex != -1)
                            {
                                List<ReflexYield> slipFamilySystems = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.GetSlipSystemsFamily((ReflexYield)SlipFamilyList.SelectedItem); TmpSimulated.Title += slipFamilySystems[0].HKLString;
                                for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex].Count; n++)
                                {
                                    double totalActive = 0.0;
                                    double familyActive = 0.0;

                                    if ((bool)this.RelativeActivityPlot.IsChecked)
                                    {
                                        totalActive += slipFamilySystems.Count;
                                    }
                                    else
                                    {
                                        totalActive += this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.PotentialSlipSystems.Count;
                                    }

                                    for (int j = 0; j < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex].Count; j++)
                                    {
                                        for (int k = 0; k < slipFamilySystems.Count; k++)
                                        {
                                            if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex][j].SlipPlane.HKLString == slipFamilySystems[k].SlipPlane.HKLString)
                                            {
                                                familyActive++;
                                            }
                                        }
                                    }

                                    MathNet.Numerics.LinearAlgebra.Matrix<double> tmp = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense<double>(3, 3, 0);
                                    tmp[2, 2] = familyActive / totalActive;

                                    yValues.Add(tmp);
                                }
                            }
                        }
                        else
                        {
                            if (PotenitalSlipSystemsList.SelectedIndex != -1 && this.GrainorientationList.SelectedIndex != -1)
                            {
                                ReflexYield selectedStatReflex = PotenitalSlipSystemsList.SelectedItem as ReflexYield;
                                List<ReflexYield> slipFamilySystems = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.GetSlipSystemsFamily((ReflexYield)SlipFamilyList.SelectedItem);
                                TmpSimulated.Title += slipFamilySystems[0].HKLString;
                                for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex].Count; n++)
                                {
                                    double totalActive = 0.0;
                                    double familyActive = 0.0;

                                    if ((bool)this.RelativeActivityPlot.IsChecked)
                                    {
                                        totalActive += slipFamilySystems.Count;
                                    }
                                    else
                                    {
                                        totalActive += this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.PotentialSlipSystems.Count;
                                    }

                                    for (int j = 0; j < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex].Count; j++)
                                    {
                                        if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex][j].SlipPlane.HKLString == selectedStatReflex.SlipPlane.HKLString)
                                        {
                                            familyActive++;
                                        }
                                    }

                                    MathNet.Numerics.LinearAlgebra.Matrix<double> tmp = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense<double>(3, 3, 0);
                                    tmp[2, 2] = familyActive / totalActive;

                                    yValues.Add(tmp);
                                }

                            }
                        }
                        break;
                    case 8:
                        if (this.GrainorientationList.SelectedIndex != -1)
                        {
                            this.YieldYAxisLin.Title = "Grain Stress [MPa]";
                            for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
                            {
                                yValues.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
                            }
                            ySummation = true;
                        }
                        break;
                    case 9:
                        if (this.GrainorientationList.SelectedIndex != -1)
                        {
                            this.YieldYAxisLin.Title = "Grain Strain";
                            for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
                            {
                                yValues.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
                            }
                            ySummation = true;
                        }
                        break;
                    case 10:
                        if (this.GrainorientationList.SelectedIndex != -1)
                        {
                            this.YieldYAxisLin.Title = "Grain Stress Rate [MPa / Step]";
                            for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
                            {
                                yValues.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
                            }
                        }
                        break;
                    case 11:
                        if (this.GrainorientationList.SelectedIndex != -1)
                        {
                            this.YieldYAxisLin.Title = "Grain Strain Rate [1 / Step]";
                            for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
                            {
                                yValues.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
                            }
                            ySummation = true;
                        }
                        break;
                    default:
                        break;
                }

                #endregion

                if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
                {
                    usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, xValues, yValues, xSummation, ySummation);
                }
                else
                {
                    int[] xIndex = { 2, 2 };
                    int[] yIndex = { 2, 2 };
                    usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, xValues, yValues, xSummation, ySummation);
                }
            }

            return usedCountsSimulated;

        }
        
        private Pattern.Counts GetGrainSimulationCountsY(int grainIndex, int paramIndex)
        {
            Pattern.Counts usedCountsSimulated = new Pattern.Counts();

            DataManagment.CrystalData.HKLReflex mainDir = new DataManagment.CrystalData.HKLReflex(1, 0, 0, 1);
            DataManagment.CrystalData.HKLReflex secondDir = new DataManagment.CrystalData.HKLReflex(1, 0, 0, 1);

            try
            {
                mainDir = new DataManagment.CrystalData.HKLReflex(Convert.ToInt32(this.EViewDirectionC1.Text), Convert.ToInt32(this.EViewDirectionC2.Text), Convert.ToInt32(this.EViewDirectionC3.Text), 1);
                secondDir = new DataManagment.CrystalData.HKLReflex(Convert.ToInt32(this.EViewDirection2C1.Text), Convert.ToInt32(this.EViewDirection2C2.Text), Convert.ToInt32(this.EViewDirection2C3.Text), 1);
            }
            catch
            {
                mainDir = new DataManagment.CrystalData.HKLReflex(1, 0, 0, 1);
                secondDir = new DataManagment.CrystalData.HKLReflex(1, 0, 0, 1);
            }

            
            switch (paramIndex)
            {
                case 4:
                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
                    {
                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][grainIndex]);
                    }

                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
                    {
                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, orientedTensor, false, true);
                    }
                    else
                    {
                        int[] xIndex = { 2, 2 };
                        int[] yIndex = { 2, 2 };
                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, orientedTensor, false, true);
                    }
                    break;
                case 5:
                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor1 = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
                    {
                        orientedTensor1.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][grainIndex]);
                    }

                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
                    {
                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, orientedTensor1, false, true);
                    }
                    else
                    {
                        int[] xIndex = { 2, 2 };
                        int[] yIndex = { 2, 2 };
                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, orientedTensor1, false, true);
                    }
                    break;
                case 6:
                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor2 = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
                    {
                        orientedTensor2.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][grainIndex]);
                    }

                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
                    {
                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, orientedTensor2);
                    }
                    else
                    {
                        int[] xIndex = { 2, 2 };
                        int[] yIndex = { 2, 2 };
                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, orientedTensor2);
                    }
                    break;
                case 7:
                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor3 = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
                    {
                        orientedTensor3.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][grainIndex]);
                    }

                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
                    {
                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, orientedTensor3);
                    }
                    else
                    {
                        int[] xIndex = { 2, 2 };
                        int[] yIndex = { 2, 2 };
                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, orientedTensor3);
                    }
                    break;
                case 8:
                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor4 = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, grainIndex, this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances());
                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
                    {
                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, orientedTensor4, false, true);
                    }
                    else
                    {
                        int[] xIndex = { 2, 2 };
                        int[] yIndex = { 2, 2 };
                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, orientedTensor4, false, true);
                    }
                    break;
                case 9:
                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor5 = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, grainIndex, this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances());
                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
                    {
                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, orientedTensor5, false, false);
                    }
                    else
                    {
                        int[] xIndex = { 2, 2 };
                        int[] yIndex = { 2, 2 };
                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, orientedTensor5, false, false);
                    }
                    break;
                default:
                    break;
                    
            }

            return usedCountsSimulated;
        }

        private void ShowExperiment_Click(object sender, RoutedEventArgs e)
        {
            OxyPlot.Series.LineSeries TmpSimulated = new OxyPlot.Series.LineSeries();

            Pattern.Counts usedCountsSimulated = this.GetSelectedSimulationCounts(TmpSimulated);

            bool averageXActive = Convert.ToBoolean(this.SetXAverageGrainPlot.IsChecked);
            bool averageYActive = Convert.ToBoolean(this.SetYAverageGrainPlot.IsChecked);
            bool xAxesValid = (this.xAxesm.SelectedIndex == 4 || this.xAxesm.SelectedIndex == 5 || this.xAxesm.SelectedIndex == 6 || this.xAxesm.SelectedIndex == 7 || this.xAxesm.SelectedIndex == 8 || this.xAxesm.SelectedIndex == 9);
            bool yAxesValid = (this.yAxesm.SelectedIndex == 4 || this.yAxesm.SelectedIndex == 5 || this.yAxesm.SelectedIndex == 6 || this.yAxesm.SelectedIndex == 7 || this.yAxesm.SelectedIndex == 8 || this.yAxesm.SelectedIndex == 9);

            if (this.GrainorientationList.SelectedIndex != -1 && this.ExperimentSelection.SelectedIndex != -1)
            {
                //VERY IMPORTANT call for automatization
                //List<int> avergageOrientationIndices = Tools.Calculation.IntegrateIndices(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GrainOrientations[this.PhaseSwitchBox.SelectedIndex][this.GrainorientationList.SelectedIndex].Psi, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GrainOrientations[this.PhaseSwitchBox.SelectedIndex]);
                List<int> avergageOrientationIndices = new List<int>();

                foreach (var item in this.GrainorientationList.SelectedItems)
                {
                    int index = this.GrainorientationList.Items.IndexOf(item);
                   avergageOrientationIndices.Add(index);
                }
                if (averageXActive && xAxesValid)
                {
                    for(int n = 0; n < usedCountsSimulated.Count; n++)
                    {
                        usedCountsSimulated[n][0] = 0.0;
                    }

                    for(int n = 0; n < avergageOrientationIndices.Count; n++)
                    {
                        Pattern.Counts avgCounts = this.GetGrainSimulationCountsY(avergageOrientationIndices[n], xAxesm.SelectedIndex);
                        for (int i = 0; i < usedCountsSimulated.Count; i++)
                        {
                            usedCountsSimulated[i][0] += avgCounts[i][1];
                        }
                    }

                    for (int n = 0; n < usedCountsSimulated.Count; n++)
                    {
                        usedCountsSimulated[n][0] /= avergageOrientationIndices.Count;
                    }
                }
                else if (averageYActive && yAxesValid)
                {
                    for (int n = 0; n < usedCountsSimulated.Count; n++)
                    {
                        usedCountsSimulated[n][1] = 0.0;
                    }

                    for (int n = 0; n < avergageOrientationIndices.Count; n++)
                    {
                        Pattern.Counts avgCounts = this.GetGrainSimulationCountsY(avergageOrientationIndices[n], yAxesm.SelectedIndex);
                        for (int i = 0; i < usedCountsSimulated.Count; i++)
                        {
                            usedCountsSimulated[i][1] += avgCounts[i][1];
                        }
                    }

                    for (int n = 0; n < usedCountsSimulated.Count; n++)
                    {
                        usedCountsSimulated[n][1] /= avergageOrientationIndices.Count;
                    }
                }
            }
            switch(this.plotSettingsWindow.SimuLineType.SelectedIndex)
            {
                case 0:
                    TmpSimulated.LineStyle = OxyPlot.LineStyle.Solid;
                    break;
                case 1:
                    TmpSimulated.LineStyle = OxyPlot.LineStyle.Dash;
                    break;
                case 2:
                    TmpSimulated.LineStyle = OxyPlot.LineStyle.Dot;
                    break;
                default:
                    TmpSimulated.LineStyle = OxyPlot.LineStyle.Solid;
                    break;
            }
            try
            {
                TmpSimulated.StrokeThickness = Convert.ToDouble(this.plotSettingsWindow.SimuLineThickness.Text);
            }
            catch
            {
                TmpSimulated.StrokeThickness = 7;
            }
            TmpSimulated.MarkerSize = 2;
            TmpSimulated.MarkerType = OxyPlot.MarkerType.Circle;
            if (this.YieldPlotModel.Series.Count == 0)
            {
                TmpSimulated.MarkerFill = OxyPlot.OxyColors.Black;
                TmpSimulated.Color = OxyPlot.OxyColors.Black;
                TmpSimulated.MarkerStroke = OxyPlot.OxyColors.Black;
            }
            else if (this.YieldPlotModel.Series.Count == 1)
            {
                TmpSimulated.MarkerFill = OxyPlot.OxyColors.DarkBlue;
                TmpSimulated.Color = OxyPlot.OxyColors.DarkBlue;
                TmpSimulated.MarkerStroke = OxyPlot.OxyColors.DarkBlue;
            }
            else if (this.YieldPlotModel.Series.Count == 2)
            {
                TmpSimulated.MarkerFill = OxyPlot.OxyColors.DarkRed;
                TmpSimulated.Color = OxyPlot.OxyColors.DarkRed;
                TmpSimulated.MarkerStroke = OxyPlot.OxyColors.DarkRed;
            }

            double Xmin = double.MaxValue;
            double Xmax = double.MinValue;

            double Ymin = double.MaxValue;
            double Ymax = double.MinValue;

            for (int n = 0; n < usedCountsSimulated.Count; n++)
            {
                double elasticStrainX = 0.0;
                double elasticStrainY = 0.0;

                if (Convert.ToBoolean(this.TensilePlasticOnlyActive.IsChecked))
                {
                    if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory[0][2, 2] != 0.0)
                    {
                        double elasticStrainRate = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory[0][2, 2] / this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory[0][2, 2];
                        if (xAxesm.SelectedIndex == 1)
                        {
                            elasticStrainX = elasticStrainRate * this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory[n][2, 2];
                        }
                        if (yAxesm.SelectedIndex == 1)
                        {
                            elasticStrainY = elasticStrainRate * this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory[n][2, 2]; ;
                        }
                    }
                }


                OxyPlot.DataPoint PDP = new OxyPlot.DataPoint(usedCountsSimulated[n][0] - elasticStrainX, usedCountsSimulated[n][1] - elasticStrainY);

                TmpSimulated.Points.Add(PDP);

                if (Xmin > usedCountsSimulated[n][0])
                {
                    Xmin = usedCountsSimulated[n][0];
                }
                if (Ymin > usedCountsSimulated[n][1])
                {
                    Ymin = usedCountsSimulated[n][1];
                }

                if (Xmax < usedCountsSimulated[n][0])
                {
                    Xmax = usedCountsSimulated[n][0];
                }
                if (Ymax < usedCountsSimulated[n][1])
                {
                    Ymax = usedCountsSimulated[n][1];
                }
            }

            if (TmpSimulated.Points.Count != 0)
            {
                this.YieldXAxisLin.Minimum = Xmin;
                this.YieldXAxisLin.Maximum = Xmax;
                this.YieldYAxisLin.Minimum = Ymin;
                this.YieldYAxisLin.Maximum = Ymax;

                this.YieldPlotModel.Series.Add(TmpSimulated);

                this.PW.MainPlot.ResetAllAxes();
                this.PW.MainPlot.InvalidatePlot(true);
            }
        }

        private void CrystalFrameSimulation_Checked(object sender, RoutedEventArgs e)
        {
            this.SetExpData();
        }

        private void CrystalFrameSimulation_Unchecked(object sender, RoutedEventArgs e)
        {
            this.SetExpData();
        }

        private void ExperimentSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.IndexEventAktive = false;

            if (this.ExperimentSelection.SelectedIndex != -1)
            {
                this.ExperimentSampleArea.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].SampleArea.ToString("G3");
                this.SetGrainContentLabel();
                if (this.PhaseSwitchBox.SelectedIndex != -1)
                {
                    this.PotenitalSlipSystemsList.ItemsSource = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems;
                    this.GrainorientationList.ItemsSource = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GrainOrientations[this.PhaseSwitchBox.SelectedIndex];

                    bool en = true;
                    if(this.SimulationQueque.Count != 0)
                    {
                        for(int n = 0; n < this.SimulationQueque.Count; n++)
                        {
                            if(this.SimulationQueque[n].Index == this.ExperimentSelection.SelectedIndex)
                            {
                                this.DriveToStress.IsEnabled = false;
                                en = false;
                                this.DriveToStress.Foreground = Brushes.DarkRed;
                            }
                        }
                    }
                    
                    if(en)
                    {
                        this.DriveToStress.IsEnabled = true;
                        this.DriveToStress.Foreground = Brushes.Black;
                    }

                    //Set Slip Families
                    List<ReflexYield> SlipFamlilyList = new List<ReflexYield>();
                    if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].CrystalData.SymmetryGroupID == 225)
                    {
                        //FCC
                        SlipFamlilyList.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[0]);
                        SlipFamilyModificationYieldStrength.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[0].YieldMainStrength.ToString("F3");
                        SlipFamilyModificationHardenning.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[0].YieldHardenning.ToString("F3");
                    }
                    else if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].CrystalData.SymmetryGroupID == 229)
                    {
                        //BCC
                        SlipFamlilyList.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[0]);
                        SlipFamlilyList.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[12]);
                        SlipFamlilyList.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[25]);
                        SlipFamilyModificationYieldStrength.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[0].YieldMainStrength.ToString("F3");
                        SlipFamilyModificationHardenning.Text = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[0].YieldHardenning.ToString("F3");
                    }
                    else if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].CrystalData.SymmetryGroupID == 194)
                    {
                        //HCP

                        SlipFamlilyList.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[0]);
                        SlipFamlilyList.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[3]);
                        SlipFamlilyList.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[6]);
                        SlipFamlilyList.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[9]);
                        SlipFamlilyList.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[12]);
                        SlipFamlilyList.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[15]);
                        SlipFamlilyList.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[18]);
                        SlipFamlilyList.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[21]);
                        SlipFamlilyList.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[24]);
                        SlipFamlilyList.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[39]);
                        SlipFamlilyList.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[42]);
                        SlipFamlilyList.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[57]);
                        SlipFamlilyList.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[63]);
                        SlipFamlilyList.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].YieldInformation[this.PhaseSwitchBox.SelectedIndex].PotentialSlipSystems[66]);

                    }
                    this.SlipFamilyList.ItemsSource = SlipFamlilyList;
                    this.SlipFamilyList.SelectedIndex = 0;
                }
            }

            SetExpData();

            this.IndexEventAktive = true;
        }

        private void ExperimentalDataSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetExpData();
        }

        private void ClearPlot_Click(object sender, RoutedEventArgs e)
        {
            this.YieldPlotModel.Series.Clear();
            this.PW.MainPlot.InvalidatePlot(true);
        }

        private void SetExpData()
        {
            List<MatrixView> source = new List<MatrixView>();
            List<MatrixView> source1 = new List<MatrixView>();
            if (this.ExperimentSelection.SelectedIndex != -1 && PhaseSwitchBox.SelectedIndex != -1)
            {
                List<MathNet.Numerics.LinearAlgebra.Matrix<double>> viewData = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
                switch (ExperimentalDataSelection.SelectedIndex)
                {
                    case 0:
                        viewData = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory;
                        break;
                    case 1:
                        viewData = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory;
                        break;
                    case 2:
                        viewData = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory;
                        break;
                    case 3:
                        viewData = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory;
                        break;
                    default:
                        break;
                    #region Old Code for Debug
                        //case 0:
                        //    if (GrainorientationList.SelectedIndex != -1)
                        //    {
                        //        for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
                        //        {
                        //            if (n == 0)
                        //            {
                        //                viewData.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][GrainorientationList.SelectedIndex]);
                        //            }
                        //            else
                        //            {
                        //                viewData.Add(viewData[n - 1] + this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][GrainorientationList.SelectedIndex]);
                        //            }
                        //        }
                        //        //viewData = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][GrainorientationList.SelectedIndex];
                        //    }
                        //    else
                        //    {
                        //        viewData = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex];
                        //    }
                        //    break;
                        //case 1:
                        //    if (GrainorientationList.SelectedIndex != -1)
                        //    {
                        //        for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
                        //        {
                        //            if (n == 0)
                        //            {
                        //                viewData.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][GrainorientationList.SelectedIndex]);
                        //            }
                        //            else
                        //            {
                        //                viewData.Add(viewData[n - 1] + this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][GrainorientationList.SelectedIndex]);
                        //            }
                        //        }
                        //        //viewData = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][GrainorientationList.SelectedIndex];
                        //    }
                        //    else
                        //    {
                        //        viewData = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex];
                        //    }
                        //    break;
                        //case 2:
                        //    if (GrainorientationList.SelectedIndex != -1)
                        //    {
                        //        for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
                        //        {
                        //            viewData.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][GrainorientationList.SelectedIndex]);
                        //        }
                        //        //viewData = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][GrainorientationList.SelectedIndex];
                        //    }
                        //    else
                        //    {
                        //        viewData = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFHistory[PhaseSwitchBox.SelectedIndex];
                        //    }
                        //    break;
                        //case 3:
                        //    if (GrainorientationList.SelectedIndex != -1)
                        //    {
                        //        for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
                        //        {
                        //            viewData.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][GrainorientationList.SelectedIndex]);
                        //        }
                        //        //viewData = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][GrainorientationList.SelectedIndex];
                        //    }
                        //    else
                        //    {
                        //        viewData = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFHistory[PhaseSwitchBox.SelectedIndex];
                        //    }
                        //    break;
                        //case 4:
                        //    viewData = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].HardeningCFHistory[PhaseSwitchBox.SelectedIndex];
                        //    break;
                        //case 9:
                        //    viewData = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrain(this.PhaseSwitchBox.SelectedIndex, GrainorientationList.SelectedIndex, this.ActSample.HillTensorData[this.PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances());
                        //    break;
                        #endregion
                }

                double[] actParamValue = { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };

                for (int n = 0; n < viewData.Count; n++)
                {
                    MatrixView tmp = new MatrixView();
                    tmp.Index = n;
                    tmp.Value1 = viewData[n][0, 0];
                    tmp.Value2 = viewData[n][1, 1];
                    tmp.Value3 = viewData[n][2, 2];
                    tmp.Value4 = viewData[n][0, 1];
                    tmp.Value5 = viewData[n][0, 2];
                    tmp.Value6 = viewData[n][1, 2];
                    source.Add(tmp);
                }

                this.StressTensorData.ItemsSource = source;

                List<MathNet.Numerics.LinearAlgebra.Matrix<double>> viewData1 = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
                switch (ExperimentalDataSelection1.SelectedIndex)
                {
                    case 0:
                        viewData1 = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory;
                        break;
                    case 1:
                        viewData1 = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory;
                        break;
                    case 2:
                        viewData1 = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory;
                        break;
                    case 3:
                        viewData1 = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory;
                        break;
                    default:
                        break;
                    #region Old Code for Debug
                        //case 0:
                        //    if (GrainorientationList.SelectedIndex != -1)
                        //    {
                        //        for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
                        //        {
                        //            if (n == 0)
                        //            {
                        //                viewData1.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][GrainorientationList.SelectedIndex]);
                        //            }
                        //            else
                        //            {
                        //                viewData1.Add(viewData[n - 1] + this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][GrainorientationList.SelectedIndex]);
                        //            }
                        //        }
                        //        //viewData = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][GrainorientationList.SelectedIndex];
                        //    }
                        //    else
                        //    {
                        //        viewData1 = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex];
                        //    }
                        //    break;
                        //case 1:
                        //    if (GrainorientationList.SelectedIndex != -1)
                        //    {
                        //        for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
                        //        {
                        //            if (n == 0)
                        //            {
                        //                viewData1.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][GrainorientationList.SelectedIndex]);
                        //            }
                        //            else
                        //            {
                        //                viewData1.Add(viewData[n - 1] + this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][GrainorientationList.SelectedIndex]);
                        //            }
                        //        }
                        //        //viewData = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][GrainorientationList.SelectedIndex];
                        //    }
                        //    else
                        //    {
                        //        viewData1 = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex];
                        //    }
                        //    break;
                        //case 2:
                        //    if (GrainorientationList.SelectedIndex != -1)
                        //    {
                        //        for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
                        //        {
                        //            viewData1.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][GrainorientationList.SelectedIndex]);
                        //        }
                        //        //viewData = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][GrainorientationList.SelectedIndex];
                        //    }
                        //    else
                        //    {
                        //        viewData1 = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFHistory[PhaseSwitchBox.SelectedIndex];
                        //    }
                        //    break;
                        //case 3:
                        //    if (GrainorientationList.SelectedIndex != -1)
                        //    {
                        //        for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
                        //        {
                        //            viewData1.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][GrainorientationList.SelectedIndex]);
                        //        }
                        //        //viewData = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][GrainorientationList.SelectedIndex];
                        //    }
                        //    else
                        //    {
                        //        viewData1 = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFHistory[PhaseSwitchBox.SelectedIndex];
                        //    }
                        //    break;
                        //case 4:
                        //    viewData1 = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].HardeningCFHistory[PhaseSwitchBox.SelectedIndex];
                        //    break;
                        //case 9:
                        //    viewData1 = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrain(this.PhaseSwitchBox.SelectedIndex, GrainorientationList.SelectedIndex, this.ActSample.HillTensorData[this.PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances());
                        //    break;
                        #endregion
                }

                actParamValue[0] = 0;
                actParamValue[1] = 0;
                actParamValue[2] = 0;
                actParamValue[3] = 0;
                actParamValue[4] = 0;
                actParamValue[5] = 0;

                for (int n = 0; n < viewData1.Count; n++)
                {
                    MatrixView tmp1 = new MatrixView();
                    tmp1.Index = n;
                    tmp1.Value1 = viewData1[n][0, 0];
                    tmp1.Value2 = viewData1[n][1, 1];
                    tmp1.Value3 = viewData1[n][2, 2];
                    tmp1.Value4 = viewData1[n][0, 1];
                    tmp1.Value5 = viewData1[n][0, 2];
                    tmp1.Value6 = viewData1[n][1, 2];
                    source1.Add(tmp1);
                }

                this.StressTensorData1.ItemsSource = source1;
                this.GrainorientationList.ItemsSource = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GrainOrientations[this.PhaseSwitchBox.SelectedIndex];
            }
        }

        private void AddNewExperiment_Click(object sender, RoutedEventArgs e)
        {
            Plasticity.ElastoPlasticExperiment ePE = new ElastoPlasticExperiment(this.ActSample);

            this.ActSample.SimulationData.Add(ePE);

            this.SetSimulationExperiemnts();
        }

        private void SetSimulationExperiemnts()
        {
            this.ExperimentSelection.Items.Clear();
            if(this.ActSample.SimulationData == null)
            {
                this.ActSample.SimulationData = new List<ElastoPlasticExperiment>();
            }
            for (int n = 0; n < this.ActSample.SimulationData.Count; n++)
            {
                //ComboBoxItem cBI = new ComboBoxItem();
                //cBI.Content = "Experiment " + n.ToString();

                SimulationTask cBI = new SimulationTask();
                cBI.Name = "Experiment " + n.ToString();

                this.ExperimentSelection.Items.Add(cBI);
            }
        }

        #region Simulation
        
        private void SimulateExperiment_Click(object sender, RoutedEventArgs e)
        {
            if (this.ExperimentSelection.SelectedIndex != -1 && PhaseSwitchBox.SelectedIndex != -1 && CalculationModel.SelectedIndex != -1 && SlipCriterion.SelectedIndex != -1)
            {
                if (this.SimulationQueque.Count == 0)
                {
                    #region Display and Quequing
                    
                    SimulationTask sT = new SimulationTask();
                    sT.Name = "Model: " + CalculationModel.SelectedIndex;
                    sT.Index = this.ExperimentSelection.SelectedIndex;

                    this.ActSimulationIndexLabel.Content = 0;

                    if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory.Count != 0)
                    {
                        if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory.Count > this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory.Count)
                        {
                            this.AllSimulationStepsLabel.Content = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory.Count.ToString();
                            sT.NumberOfCycles = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory.Count;
                        }
                        else
                        {
                            this.AllSimulationStepsLabel.Content = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory.Count.ToString();
                            sT.NumberOfCycles = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory.Count;
                        }
                    }
                    else
                    {
                        this.AllSimulationStepsLabel.Content = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory.Count.ToString();
                        sT.NumberOfCycles = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory.Count;
                    }

                    this.SimulationQueque.Add(sT);
                    this.SimulationList.Items.Refresh();

                    this.DriveToStress.IsEnabled = false;
                    this.DriveToStress.Foreground = Brushes.DarkRed;

                    this.LastFailedGrainsLabel.Content = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].LastFailedGrains;

                    #endregion

                    #region simulation settings

                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].useHardeningMatrix = false;
                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].useAreaCorrection = Convert.ToBoolean(this.AreaCorrectionActive.IsChecked);
                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].useMultiThreading = Convert.ToBoolean(this.MultithreadingActive.IsChecked);
                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].useYieldLimit = Convert.ToBoolean(this.YieldLimitActive.IsChecked);
                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].singleCrystalTracking = Convert.ToBoolean(this.SingleCrystalTrackingActive.IsChecked);
                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ElasticModel = CalculationModel.SelectedIndex;
                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].slipCritereon = SlipCriterion.SelectedIndex;

                    #endregion

                    BackgroundWorker worker = new BackgroundWorker();

                    worker.DoWork += SimulateElastoPlasticExperiment_Work;
                    worker.WorkerReportsProgress = true;
                    worker.ProgressChanged += SimulateElastoPlasticExperiment_ProgressChanged;
                    worker.RunWorkerCompleted += SimulateElastoPlasticExperiment_Completed;

                    worker.RunWorkerAsync(this.ExperimentSelection.SelectedIndex);


                    //worker.RunWorkerAsync(ForPlot);

                    //EPModeling.PerformStandardExperiment(this.ActSample, false, this.ExperimentSelection.SelectedIndex);
                }
                else
                {
                    #region Display and Quequing

                    SimulationTask sT = new SimulationTask();
                    sT.Name = "Model: " + CalculationModel.SelectedIndex;
                    sT.Index = this.ExperimentSelection.SelectedIndex;

                    if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory.Count != 0)
                    {
                        if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory.Count > this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory.Count)
                        {
                            sT.NumberOfCycles = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory.Count;
                        }
                        else
                        {
                            sT.NumberOfCycles = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory.Count;
                        }
                    }
                    else
                    {
                        sT.NumberOfCycles = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory.Count;
                    }

                    this.SimulationQueque.Add(sT);
                    this.SimulationList.Items.Refresh();

                    #endregion

                    #region simulation settings

                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].useHardeningMatrix = false;
                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].useAreaCorrection = Convert.ToBoolean(this.AreaCorrectionActive.IsChecked);
                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].useMultiThreading = Convert.ToBoolean(this.MultithreadingActive.IsChecked);
                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].useYieldLimit = Convert.ToBoolean(this.YieldLimitActive.IsChecked);
                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ElasticModel = CalculationModel.SelectedIndex;
                    this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].slipCritereon = SlipCriterion.SelectedIndex;

                    #endregion
                }
            }
            //this.SetExpData();
        }

        private void SimulateElastoPlasticExperiment_Work(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            
            int experimentIndex = (int)e.Argument;

            // Microsopic.ElasticityTensors eT, PlasticityTensor pT, ElastoPlasticExperiment ePE
            // Zurücksetzen der Härtung vom letzten Experiment
            for (int n = 0; n < this.ActSample.PlasticTensor.Count; n++)
            {
                for (int i = 0; i < this.ActSample.PlasticTensor[n].YieldSurfaceData.PotentialSlipSystems.Count; i++)
                {
                    this.ActSample.PlasticTensor[n].YieldSurfaceData.PotentialSlipSystems[i].YieldMainHardennedStrength = this.ActSample.PlasticTensor[n].YieldSurfaceData.PotentialSlipSystems[i].YieldMainStrength;
                }
            }

            int actIndex = 0;

            if (this.ActSample.SimulationData[experimentIndex].StressSFHistory.Count != 0)
            {
                if (this.ActSample.SimulationData[experimentIndex].StressSFHistory.Count > this.ActSample.SimulationData[experimentIndex].StrainSFHistory.Count)
                {
                    actIndex = this.ActSample.SimulationData[experimentIndex].StrainSFHistory.Count;
                    for (int n = actIndex; n < this.ActSample.SimulationData[experimentIndex].StressSFHistory.Count; n++)
                    {
                        if (this.ActSample.SimulationData[experimentIndex].useMultiThreading)
                        {
                            EPModeling.PerformStressExperimentMultithreading(this.ActSample, experimentIndex, n, 10, false, this.ActSample.SimulationData[experimentIndex].singleCrystalTracking, this.ActSample.SimulationData[experimentIndex].slipCritereon, this.ActSample.SimulationData[experimentIndex].ElasticModel);
                        }
                        else
                        {
                            EPModeling.PerformStressExperiment(this.ActSample, experimentIndex, n, 10, false, this.ActSample.SimulationData[experimentIndex].singleCrystalTracking, this.ActSample.SimulationData[experimentIndex].slipCritereon, this.ActSample.SimulationData[experimentIndex].ElasticModel);
                        }
                        worker.ReportProgress(Convert.ToInt32((Convert.ToDouble(n) * 100.0) / Convert.ToDouble(this.ActSample.SimulationData[experimentIndex].StressSFHistory.Count)), n);
                        if(this.ActSample.SimulationData[experimentIndex].cancel)
                        {
                            this.ActSample.SimulationData[experimentIndex].cancel = false;
                            break;
                        }
                    }
                }
                else
                {
                    actIndex = this.ActSample.SimulationData[experimentIndex].StressSFHistory.Count;
                    for (int n = actIndex; n < this.ActSample.SimulationData[experimentIndex].StrainSFHistory.Count; n++)
                    {
                        //EPModeling.PerfomStandardExperimentStepAllGRain(this.ActSample, false, experimentIndex, n, actStrainS, actStrainG, actStressG, this.ActSample.SimulationData[experimentIndex].useHardeningMatrix, this.ActSample.SimulationData[experimentIndex].useAreaCorrection);
                        //EPModeling.PerformStrainExperimentSt(this.ActSample, experimentIndex, n, 10, false, this.ActSample.SimulationData[experimentIndex].useHardeningMatrix, this.ActSample.SimulationData[experimentIndex].invertSlipDirections);
                        worker.ReportProgress(Convert.ToInt32((Convert.ToDouble(n) * 100.0) / Convert.ToDouble(this.ActSample.SimulationData[experimentIndex].StrainSFHistory.Count)), n);
                        if (this.ActSample.SimulationData[experimentIndex].cancel)
                        {
                            this.ActSample.SimulationData[experimentIndex].cancel = false;
                            break;
                        }
                    }
                }
            }
            else
            {
                for (int n = actIndex; n < this.ActSample.SimulationData[experimentIndex].StrainSFHistory.Count; n++)
                {
                    //EPModeling.PerfomStandardExperimentStepAllGRain(this.ActSample, false, experimentIndex, n, actStrainS, actStrainG, actStressG, this.ActSample.SimulationData[experimentIndex].useHardeningMatrix, this.ActSample.SimulationData[experimentIndex].useAreaCorrection);
                    //EPModeling.PerformStrainExperimentSt(this.ActSample, experimentIndex, n, 10, false, this.ActSample.SimulationData[experimentIndex].useHardeningMatrix, this.ActSample.SimulationData[experimentIndex].invertSlipDirections);
                    worker.ReportProgress(Convert.ToInt32((Convert.ToDouble(n) * 100.0) / Convert.ToDouble(this.ActSample.SimulationData[experimentIndex].StrainSFHistory.Count)), n);
                    if (this.ActSample.SimulationData[experimentIndex].cancel)
                    {
                        this.ActSample.SimulationData[experimentIndex].cancel = false;
                        break;
                    }
                }
            }
        }

        private void SimulateElastoPlasticExperiment_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.SimulationProgress.Value = e.ProgressPercentage;
            int simStep = (int)e.UserState;
            simStep++;
            this.ActSimulationIndexLabel.Content = simStep.ToString();

            this.SetExpData();

            this.PotenitalSlipSystemsList.Items.Refresh();
            if (this.SimulationQueque.Count != 0)
            {
                this.LastActiveSlipSystems.Content = this.ActSample.SimulationData[this.SimulationQueque[0].Index].LastActiveSystems.ToString();
                this.LastFailedGrainsLabel.Content = this.ActSample.SimulationData[this.SimulationQueque[0].Index].LastFailedGrains;

                double numberOfGrains = 0.0;
                for(int n = 0; n < this.ActSample.SimulationData[this.SimulationQueque[0].Index].GrainOrientations.Count; n++)
                {
                    numberOfGrains += this.ActSample.SimulationData[this.SimulationQueque[0].Index].GrainOrientations[n].Count;
                }

                double activeRatio = this.ActSample.SimulationData[this.SimulationQueque[0].Index].LastActiveSystems / (numberOfGrains * 5.0);
                activeRatio *= 100;
                this.LastActiveSlipSystemsRatio.Content = activeRatio.ToString("F3");
            }

            //if (StateString[1] == "L")
            //{
            //    this.StatusLog1.Content = "Loading File";
            //}
            //else if (StateString[1] == "C1")
            //{
            //    this.ErrorLog1.Content = "Loading not possible";
            //    this.ErrorLog2.Content = "Not a .dat file";
            //}
            //else if (StateString[1] == "C2")
            //{
            //    this.ErrorLog1.Content = "Loading not possible";
            //    this.ErrorLog2.Content = "Wrong File Format";
            //}
            //else if (StateString[1] == "PF")
            //{
            //    this.StatusLog1.Content = "Searching peaks";
            //}
            //else if (StateString[1] == "PFF")
            //{
            //    this.StatusLog1.Content = "Search completted";
            //}
            //else if (StateString[1] == "PFE")
            //{
            //    this.ErrorLog1.Content = "Peak search not possible";
            //    this.ErrorLog2.Content = "Could not load diffraction pattern";
            //}

            //this.StatusLog2.Content = StateString[0];
        }

        private void SimulateElastoPlasticExperiment_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            
            this.SimulationProgress.Value = 0;
            this.SetExpData();

            this.SimulationQueque.RemoveAt(0);
            this.SimulationList.Items.Refresh();

            this.DriveToStress.IsEnabled = true;
            this.DriveToStress.Foreground = Brushes.Black;

            if (this.SimulationQueque.Count != 0)
            {
                this.ActSimulationIndexLabel.Content = 0;
                this.LastFailedGrainsLabel.Content = this.ActSample.SimulationData[this.SimulationQueque[0].Index].LastFailedGrains;

                if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory.Count != 0)
                {
                    if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory.Count > this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory.Count)
                    {
                        this.AllSimulationStepsLabel.Content = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory.Count.ToString();
                    }
                    else
                    {
                        this.AllSimulationStepsLabel.Content = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory.Count.ToString();
                    }
                }
                else
                {
                    this.AllSimulationStepsLabel.Content = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory.Count.ToString();
                }

                for (int n = 0; n < this.SimulationQueque.Count; n++)
                {
                    if (this.SimulationQueque[n].Index == this.ExperimentSelection.SelectedIndex)
                    {
                        this.DriveToStress.IsEnabled = false;
                        this.DriveToStress.Foreground = Brushes.DarkRed;
                    }
                }

                BackgroundWorker worker = new BackgroundWorker();

                worker.DoWork += SimulateElastoPlasticExperiment_Work;
                worker.WorkerReportsProgress = true;
                worker.ProgressChanged += SimulateElastoPlasticExperiment_ProgressChanged;
                worker.RunWorkerCompleted += SimulateElastoPlasticExperiment_Completed;

                worker.RunWorkerAsync(this.SimulationQueque[0].Index);
            }
        }

        #endregion

        #endregion

        private void StressTensorData_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                DataGridRow dGR = e.Row;
                MatrixView dataTmp = (MatrixView)dGR.DataContext;
                int selectedIndex = dGR.GetIndex();
                switch (ExperimentalDataSelection.SelectedIndex)
                {
                    case 5:
                        if (this.ExperimentSelection.SelectedIndex != -1)
                        {
                            this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory[selectedIndex][0, 0] = dataTmp.Value1;
                            this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory[selectedIndex][1, 1] = dataTmp.Value2;
                            this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory[selectedIndex][2, 2] = dataTmp.Value3;
                            this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory[selectedIndex][0, 1] = dataTmp.Value4;
                            this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory[selectedIndex][0, 2] = dataTmp.Value5;
                            this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory[selectedIndex][1, 0] = dataTmp.Value4;
                            this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory[selectedIndex][2, 0] = dataTmp.Value5;
                            this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory[selectedIndex][2, 1] = dataTmp.Value6;
                            this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory[selectedIndex][1, 2] = dataTmp.Value6;
                        }
                        break;
                    case 6:
                        if (this.ExperimentSelection.SelectedIndex != -1)
                        {
                            this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory[selectedIndex][0, 0] = dataTmp.Value1;
                            this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory[selectedIndex][1, 1] = dataTmp.Value2;
                            this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory[selectedIndex][2, 2] = dataTmp.Value3;
                            this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory[selectedIndex][0, 1] = dataTmp.Value4;
                            this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory[selectedIndex][0, 2] = dataTmp.Value5;
                            this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory[selectedIndex][1, 0] = dataTmp.Value4;
                            this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory[selectedIndex][2, 0] = dataTmp.Value5;
                            this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory[selectedIndex][2, 1] = dataTmp.Value6;
                            this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory[selectedIndex][1, 2] = dataTmp.Value6;
                        }
                        break;
                    case 7:
                        if (this.ExperimentSelection.SelectedIndex != -1)
                        {
                            this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory[selectedIndex][0, 0] = dataTmp.Value1;
                            this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory[selectedIndex][1, 1] = dataTmp.Value2;
                            this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory[selectedIndex][2, 2] = dataTmp.Value3;
                            this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory[selectedIndex][0, 1] = dataTmp.Value4;
                            this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory[selectedIndex][0, 2] = dataTmp.Value5;
                            this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory[selectedIndex][1, 0] = dataTmp.Value4;
                            this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory[selectedIndex][2, 0] = dataTmp.Value5;
                            this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory[selectedIndex][2, 1] = dataTmp.Value6;
                            this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory[selectedIndex][1, 2] = dataTmp.Value6;
                        }
                        break;
                    case 8:
                        if (this.ExperimentSelection.SelectedIndex != -1)
                        {
                            this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory[selectedIndex][0, 0] = dataTmp.Value1;
                            this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory[selectedIndex][1, 1] = dataTmp.Value2;
                            this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory[selectedIndex][2, 2] = dataTmp.Value3;
                            this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory[selectedIndex][0, 1] = dataTmp.Value4;
                            this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory[selectedIndex][0, 2] = dataTmp.Value5;
                            this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory[selectedIndex][1, 0] = dataTmp.Value4;
                            this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory[selectedIndex][2, 0] = dataTmp.Value5;
                            this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory[selectedIndex][2, 1] = dataTmp.Value6;
                            this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory[selectedIndex][1, 2] = dataTmp.Value6;
                        }
                        break;
                    default:
                        break;
                }
            }
            catch
            {

            }
        }

        private void ExperimentSampleArea_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.ExperimentSelection.SelectedIndex != -1 && IndexEventAktive)
            {
                try
                {
                    this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].SampleArea = Convert.ToDouble(this.ExperimentSampleArea.Text);
                }
                catch
                {

                }
            }
        }

        private string savePathTmp = "";

        private void SaveExperiment_Click(object sender, RoutedEventArgs e)
        {
            if (this.ExperimentSelection.SelectedIndex != -1)
            {
                Microsoft.Win32.SaveFileDialog OpenSampleFile = new Microsoft.Win32.SaveFileDialog();
                OpenSampleFile.FileName = this.ActSample.Name + "-Experiment";
                OpenSampleFile.DefaultExt = ".sim";
                OpenSampleFile.Filter = "simulation data (.sim)|*.sim";

                Nullable<bool> Opened = OpenSampleFile.ShowDialog();

                if (Opened == true)
                {
                    string filename = OpenSampleFile.FileName;
                    savePathTmp = filename;
                    string PathName = filename.Replace(OpenSampleFile.SafeFileName, "");
                    System.IO.Directory.CreateDirectory(PathName);

                    BackgroundWorker worker = new BackgroundWorker();

                    if ((bool)this.OldDataScheme.IsChecked)
                    {
                        DataManagment.Files.SCEC.SimulatedExperimentInformation ForSave = new DataManagment.Files.SCEC.SimulatedExperimentInformation(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex]);
                        ForSave.savePath = savePathTmp;

                        worker.DoWork += SaveElastoPlasticExperimentBV_Work;
                        worker.RunWorkerCompleted += SaveElastoPlasticExperimentBV_Completed;
                        
                        worker.RunWorkerAsync(ForSave);
                    }
                    else
                    {
                        bool containsSim = OpenSampleFile.SafeFileName.Contains(".sim");
                        if(containsSim)
                        {
                            savePathTmp = OpenSampleFile.SafeFileName.Substring(0, OpenSampleFile.SafeFileName.Length - 4);
                        }
                        DataManagment.Files.Simulation.SimHeader simulationHeader = new DataManagment.Files.Simulation.SimHeader(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex], PathName, savePathTmp);
                        worker.DoWork += SaveElastoPlasticExperiment_Work;
                        worker.ProgressChanged += SaveElastoPlasticExperiment_ProgressChanged;
                        worker.RunWorkerCompleted += SaveElastoPlasticSystems_Completed;
                        worker.WorkerReportsProgress = true;

                        worker.RunWorkerAsync(simulationHeader);
                    }


                    this.LoadExperiment.IsEnabled = false;
                    this.LoadExperiment.Foreground = Brushes.DarkRed;
                    this.SaveExperiment.IsEnabled = false;
                    this.SaveExperiment.Foreground = Brushes.DarkRed;

                }
            }
        }

        private void LoadExperiment_Click(object sender, RoutedEventArgs e)
        {
            if (this.SimulationQueque.Count == 0)
            {
                Microsoft.Win32.OpenFileDialog OpenSampleFile = new Microsoft.Win32.OpenFileDialog();
                OpenSampleFile.Multiselect = false;
                OpenSampleFile.DefaultExt = ".sim";
                OpenSampleFile.Filter = "simulation data (.sim)|*.sim";

                Nullable<bool> Opened = OpenSampleFile.ShowDialog();

                if (Opened == true)
                {
                    string filename = OpenSampleFile.FileName;

                    BackgroundWorker worker = new BackgroundWorker();

                    if ((bool)this.OldDataScheme.IsChecked)
                    {
                        worker.DoWork += LoadElastoPlasticExperimentBV_Work;
                        worker.RunWorkerCompleted += LoadElastoPlasticExperimentBV_Completed;
                    }
                    else
                    {
                        worker.DoWork += LoadElastoPlasticExperiment_Work;
                        worker.ProgressChanged += LoadElastoPlasticExperiment_ProgressChanged;
                        worker.RunWorkerCompleted += LoadElastoPlasticExperiment_Completed;
                        worker.WorkerReportsProgress = true;
                    }

                    worker.RunWorkerAsync(filename);

                    this.LoadExperiment.IsEnabled = false;
                    this.LoadExperiment.Foreground = Brushes.DarkRed;
                    this.SaveExperiment.IsEnabled = false;
                    this.SaveExperiment.Foreground = Brushes.DarkRed;
                }
            }
        }

        private void SaveElastoPlasticExperiment_Work(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            DataManagment.Files.Simulation.SimHeader simulationHeader = e.Argument as DataManagment.Files.Simulation.SimHeader;

            for(int n = 0; n < simulationHeader.totalBodies; n++)
            {
                double progressPercentage = Convert.ToDouble(n + 1) / Convert.ToDouble(simulationHeader.totalBodies);
                progressPercentage *= 94;
                int progressPercentageTmp = Convert.ToInt32(progressPercentage + 5);
                worker.ReportProgress(progressPercentageTmp);

                simulationHeader.SaveSimulationData(n);
            }

            simulationHeader.SimulationData.Clear();

            using (System.IO.Stream fileSaveStream = System.IO.File.Create(simulationHeader.SavePath + simulationHeader.SaveName + ".sim"))
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    bf.Serialize(ms, simulationHeader);

                    ms.WriteTo(fileSaveStream);
                }
            }
        }

        private void SaveElastoPlasticExperiment_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.FileHandleProgress.Value = Convert.ToDouble(e.ProgressPercentage);
        }

        private void SaveElastoPlasticExperiment_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            this.FileHandleProgress.Value = 100;

            this.SetSimulationExperiemnts();


            this.LoadExperiment.IsEnabled = true;
            this.LoadExperiment.Foreground = Brushes.Black;
            this.SaveExperiment.IsEnabled = true;
            this.SaveExperiment.Foreground = Brushes.Black;
        }

        private void LoadElastoPlasticExperiment_Work(object sender, DoWorkEventArgs e)
        {
            string filename = e.Argument as string;
            BackgroundWorker worker = sender as BackgroundWorker;

            DataManagment.Files.Simulation.SimHeader simulationHeader = new DataManagment.Files.Simulation.SimHeader();

            using (System.IO.Stream fileStream = System.IO.File.OpenRead(filename))
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                object DataObj = bf.Deserialize(fileStream);

                simulationHeader = DataObj as DataManagment.Files.Simulation.SimHeader;
            }

            worker.ReportProgress(5);
            if (simulationHeader.totalBodies != 0)
            {
                for (int n = 0; n < simulationHeader.totalBodies; n++)
                {
                    double progressPercentage = Convert.ToDouble(n + 1) / Convert.ToDouble(simulationHeader.totalBodies);
                    progressPercentage *= 94;
                    int progressPercentageTmp = Convert.ToInt32(progressPercentage + 5);
                    worker.ReportProgress(progressPercentageTmp);

                    simulationHeader.LoadSimulationData(n);
                }
            }
            
            this.ActSample.SimulationData.Add(simulationHeader.GetExperiment());
        }

        private void LoadElastoPlasticExperiment_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.FileHandleProgress.Value = Convert.ToDouble(e.ProgressPercentage);
        }

        private void LoadElastoPlasticExperiment_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            this.SetSimulationExperiemnts();


            this.LoadExperiment.IsEnabled = true;
            this.LoadExperiment.Foreground = Brushes.Black;
            this.SaveExperiment.IsEnabled = true;
            this.SaveExperiment.Foreground = Brushes.Black;
        }

        #region Loading and Saving Backwards compatibility code
        private void SaveElastoPlasticExperimentBV_Work(object sender, DoWorkEventArgs e)
        {
            DataManagment.Files.SCEC.SimulatedExperimentInformation ForSave = e.Argument as DataManagment.Files.SCEC.SimulatedExperimentInformation;
            string filename = ForSave.savePath;
            string PathName = filename.Replace(filename, "");

            try
            {
                using (System.IO.Stream fileSaveStream = System.IO.File.Create(filename))
                {
                    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                    {
                        bf.Serialize(ms, ForSave);

                        ms.WriteTo(fileSaveStream);
                    }
                }
            }
            catch
            {

            }
        }

        private void SaveElastoPlasticSystems_Work(object sender, DoWorkEventArgs e)
        {
            DataManagment.Files.SCEC.ActiveSystemInformation ForSaveSystems = e.Argument as DataManagment.Files.SCEC.ActiveSystemInformation;
            string filename = ForSaveSystems.savePath;
            string PathName = filename.Replace(filename, "");
            try
            {
                using (System.IO.Stream fileSaveStream = System.IO.File.Create(filename + "as"))
                {
                    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                    {
                        bf.Serialize(ms, ForSaveSystems);

                        ms.WriteTo(fileSaveStream);
                    }
                }
            }
            catch
            {

            }
        }

        private void SaveElastoPlasticExperimentBV_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            DataManagment.Files.SCEC.ActiveSystemInformation ForSaveSystems = new DataManagment.Files.SCEC.ActiveSystemInformation(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex]);
            ForSaveSystems.savePath = savePathTmp;

            BackgroundWorker worker = new BackgroundWorker();

            worker.DoWork += SaveElastoPlasticSystems_Work;
            worker.RunWorkerCompleted += SaveElastoPlasticSystems_Completed;

            worker.RunWorkerAsync(ForSaveSystems);
        }

        private void SaveElastoPlasticSystems_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            this.SetSimulationExperiemnts();
            
            this.LoadExperiment.IsEnabled = true;
            this.LoadExperiment.Foreground = Brushes.Black;
            this.SaveExperiment.IsEnabled = true;
            this.SaveExperiment.Foreground = Brushes.Black;
        }
        
        private void LoadElastoPlasticExperimentBV_Work(object sender, DoWorkEventArgs e)
        {
            string filename = e.Argument as string;

            using (System.IO.Stream fileStream = System.IO.File.OpenRead(filename))
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                object DataObj = bf.Deserialize(fileStream);

                DataManagment.Files.SCEC.SimulatedExperimentInformation Loaded = DataObj as DataManagment.Files.SCEC.SimulatedExperimentInformation;

                this.ActSample.SimulationData.Add(Loaded.GetElastoPlasticExperiment());

                //this.SetSimulationExperiemnts();
            }
            try
            {
                using (System.IO.Stream fileStream = System.IO.File.OpenRead(filename + "as"))
                {
                    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                    object DataObj = bf.Deserialize(fileStream);

                    DataManagment.Files.SCEC.ActiveSystemInformation Loaded = DataObj as DataManagment.Files.SCEC.ActiveSystemInformation;

                    this.ActSample.SimulationData[this.ActSample.SimulationData.Count - 1].ActiveSystemsCFOrientedHistory = Loaded.GetActiveSystems(this.ActSample.SimulationData[this.ActSample.SimulationData.Count - 1].YieldInformation);

                }
            }
            catch
            {

            }
        }

        private void LoadElastoPlasticExperimentBV_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            this.SetSimulationExperiemnts();


            this.LoadExperiment.IsEnabled = true;
            this.LoadExperiment.Foreground = Brushes.Black;
            this.SaveExperiment.IsEnabled = true;
            this.SaveExperiment.Foreground = Brushes.Black;
        }

        #endregion

        private void ExportExperiment_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog XlsxSaveFile = new Microsoft.Win32.SaveFileDialog();
            XlsxSaveFile.FileName = this.ActSample.Name;
            XlsxSaveFile.DefaultExt = "";
            XlsxSaveFile.Filter = "Excel data (.xlsx)|*.xlsx";

            Nullable<bool> Opened = XlsxSaveFile.ShowDialog();

            if (Opened == true)
            {
                Microsoft.Office.Interop.Excel.Application ExcelLogApp;
                Microsoft.Office.Interop.Excel.Workbook ExcelLogBook;
                Microsoft.Office.Interop.Excel.Worksheet ExcelLogSheet;

                ExcelLogApp = null;

                string filename = XlsxSaveFile.FileName;
                string PathName = filename.Replace(XlsxSaveFile.SafeFileName, "");
                System.IO.Directory.CreateDirectory(PathName);

                try
                {
                    ExcelLogApp = new Microsoft.Office.Interop.Excel.Application();

                    ExcelLogBook = (Microsoft.Office.Interop.Excel.Workbook)ExcelLogApp.Workbooks.Add(System.Reflection.Missing.Value);

                    if (ExcelLogBook.Sheets.Count < 3)
                    {
                        for (int n = ExcelLogBook.Sheets.Count; n < 4; n++)
                        {
                            ExcelLogBook.Sheets.Add(System.Reflection.Missing.Value);
                        }
                    }

                    ExcelLogBook.Sheets[1].Select();

                    ExcelLogSheet = (Microsoft.Office.Interop.Excel.Worksheet)ExcelLogBook.ActiveSheet;

                    ExcelLogSheet.Name = "General information";

                    ExcelLogSheet.Cells[1, 1] = "Sample name:";
                    ExcelLogSheet.Cells[1, 2] = this.ActSample.Name;

                    ExcelLogSheet.Cells[2, 1] = "Sample cross section (mm):";
                    ExcelLogSheet.Cells[2, 2] = this.ActSample.Area;

                    ExcelLogSheet.Cells[2, 3] = "Used Neutron wavelength (Angstrom):";
                    ExcelLogSheet.Cells[2, 4] = CalScec.Properties.Settings.Default.UsedWaveLength.ToString();

                    OxyPlot.Series.LineSeries TmpSimulated = new OxyPlot.Series.LineSeries();

                    for(int n = 0; n < this.YieldPlotModel.Series.Count; n++)
                    {
                        ExcelLogSheet.Cells[3, (2 * n) + 1] = "Series Name:";
                        ExcelLogSheet.Cells[3, (2 * n) + 2] = this.YieldPlotModel.Series[n].Title;

                        ExcelLogSheet.Cells[4, (2 * n) + 1] = "X Values:";
                        ExcelLogSheet.Cells[4, (2 * n) + 2] = "Y Values:";
                        OxyPlot.Series.LineSeries lSTmp = this.YieldPlotModel.Series[n] as OxyPlot.Series.LineSeries;

                        for (int i = 0; i < lSTmp.Points.Count; i++)
                        {
                            ExcelLogSheet.Cells[5 + i, (2 * n) + 1] = lSTmp.Points[i].X;
                            ExcelLogSheet.Cells[5 + i, (2 * n) + 2] = lSTmp.Points[i].Y;
                        }

                    }

                    //Pattern.Counts usedCountsSimulated = this.GetSelectedSimulationCounts(TmpSimulated);

                    //ExcelLogSheet.Cells[4, 1] = "X Values";
                    //ExcelLogSheet.Cells[4, 2] = "Y Values";
                    //for (int n = 0; n < usedCountsSimulated.Count; n++)
                    //{
                    //    ExcelLogSheet.Cells[5 + n, 1] = usedCountsSimulated[n][0];
                    //    ExcelLogSheet.Cells[5 + n, 2] = usedCountsSimulated[n][1];
                    //}
                }
                catch (Exception ex)
                {
                    String myErrorString = ex.Message;
                    MessageBox.Show(myErrorString);
                }
                finally
                {
                    if (ExcelLogApp != null)
                    {
                        ExcelLogApp.Quit();
                    }
                }
            }
        }

        private void GrainorientationList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetExpData();
        }

        private void CancelExperiment_Click(object sender, RoutedEventArgs e)
        {
            if (this.SimulationQueque.Count != 0)
            {
                if (this.SimulationQueque.Count == 1)
                {
                    if (this.ExperimentSelection.SelectedIndex != -1)
                    {
                        this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].cancel = true;
                    }
                }
                else
                {
                    if(this.SimulationList.SelectedIndex != -1)
                    {
                        if (this.SimulationList.SelectedIndex == 0)
                        {
                            if (this.ExperimentSelection.SelectedIndex != -1)
                            {
                                this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].cancel = true;
                            }
                        }
                        else
                        {
                            this.SimulationQueque.RemoveAt(this.SimulationList.SelectedIndex);
                            this.SimulationList.Items.Refresh();
                        }
                    }
                }
            }
        }

        private void SetTensileParameter_Click(object sender, RoutedEventArgs e)
        {
            if(this.MacroExperimentPlotList.SelectedIndex != -1)
            {
                int startIndex = 5;
                try
                {
                    startIndex = Convert.ToInt32(TensileAutoStartIndex.Text);
                }
                catch
                {

                }
                this.ActSample.TensileTests[this.MacroExperimentPlotList.SelectedIndex].SetMechanicalProperties(startIndex);

                this.IndexEventAktive = false;
                this.TensileEModul.Text = this.ActSample.TensileTests[this.MacroExperimentPlotList.SelectedIndex].EModul.ToString("F3");
                this.TensileElasticLimit.Text = this.ActSample.TensileTests[this.MacroExperimentPlotList.SelectedIndex].ElasticLimit.ToString("F3");
                this.IndexEventAktive = true;
            }
        }

        private void TensileMechChanged_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(this.IndexEventAktive && this.MacroExperimentPlotList.SelectedIndex != -1)
            {
                try
                {
                    this.ActSample.TensileTests[this.MacroExperimentPlotList.SelectedIndex].EModul = Convert.ToDouble(this.TensileEModul.Text);
                    this.ActSample.TensileTests[this.MacroExperimentPlotList.SelectedIndex].ElasticLimit = Convert.ToDouble(this.TensileElasticLimit.Text);
                }
                catch
                {

                }
            }
        }

        private void MacroExperimentPlotList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(this.IndexEventAktive && this.MacroExperimentPlotList.SelectedIndex != -1)
            {
                this.IndexEventAktive = false;

                this.TensileEModul.Text = this.ActSample.TensileTests[this.MacroExperimentPlotList.SelectedIndex].EModul.ToString("F3");
                this.TensileElasticLimit.Text = this.ActSample.TensileTests[this.MacroExperimentPlotList.SelectedIndex].ElasticLimit.ToString("F3");

                this.IndexEventAktive = true;
            }
        }

        private void ApplyPlotSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Convert.ToDouble(this.plotSettingsWindow.LegendTitleFontSize.Text) != 0)
                {
                    YieldPlotModel.IsLegendVisible = true;
                    YieldPlotModel.LegendFontSize = Convert.ToDouble(this.plotSettingsWindow.LegendFontSize.Text);
                    YieldPlotModel.LegendTitleFontSize = Convert.ToDouble(this.plotSettingsWindow.LegendTitleFontSize.Text);
                }
                else
                {
                    YieldPlotModel.IsLegendVisible = false;
                }
                
                YieldXAxisLin.FontSize = Convert.ToDouble(this.plotSettingsWindow.YAxisFontSize.Text);
                YieldYAxisLin.FontSize = Convert.ToDouble(this.plotSettingsWindow.YAxisFontSize.Text);
                if (Convert.ToDouble(this.plotSettingsWindow.XAxisMajorTickIntervall.Text) != -1)
                {
                    YieldXAxisLin.MinimumMajorStep = Convert.ToDouble(this.plotSettingsWindow.XAxisMajorTickIntervall.Text);
                }
                if (Convert.ToDouble(this.plotSettingsWindow.YAxisMajorTickIntervall.Text) != -1)
                {
                    YieldYAxisLin.MinimumMajorStep = Convert.ToDouble(this.plotSettingsWindow.YAxisMajorTickIntervall.Text);
                }

                this.PW.MainPlot.ResetAllAxes();
                this.PW.MainPlot.InvalidatePlot(true);
            }
            catch
            {

            }
        }

        private void SimulationLimit_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(this.IndexEventAktive)
            {
                try
                {
                    double newLimit = Convert.ToDouble(this.SimulationLimit.Text);
                    Properties.Settings.Default.EPSCSimulationLimit = newLimit;
                }
                catch
                {

                }
            }
        }

        private void ShowPlotSettings_Click(object sender, RoutedEventArgs e)
        {
            plotSettingsWindow.ShowDialog();
        }

        protected override void OnClosed(EventArgs e)
        {
            this.plotSettingsWindow.PreventClosing = false;
            this.plotSettingsWindow.Close();

            this.PW.PreventClosing = false;
            this.PW.Close();
            
            base.OnClosed(e);
        }

        private void ShowPlotWindow_Click(object sender, RoutedEventArgs e)
        {
            PW.Show();
        }


        #region Spagetthi Plot (Old Code)
        //private Pattern.Counts GetSelectedSimulationCounts(OxyPlot.Series.LineSeries TmpSimulated)
        //{
        //    Pattern.Counts usedCountsSimulated = new Pattern.Counts();

        //    #region Axis selection

        //    DataManagment.CrystalData.HKLReflex mainDir = new DataManagment.CrystalData.HKLReflex(1, 0, 0, 1);
        //    DataManagment.CrystalData.HKLReflex secondDir = new DataManagment.CrystalData.HKLReflex(1, 0, 0, 1);

        //    try
        //    {
        //        mainDir = new DataManagment.CrystalData.HKLReflex(Convert.ToInt32(this.EViewDirectionC1.Text), Convert.ToInt32(this.EViewDirectionC2.Text), Convert.ToInt32(this.EViewDirectionC3.Text), 1);
        //        secondDir = new DataManagment.CrystalData.HKLReflex(Convert.ToInt32(this.EViewDirection2C1.Text), Convert.ToInt32(this.EViewDirection2C2.Text), Convert.ToInt32(this.EViewDirection2C3.Text), 1);
        //    }
        //    catch
        //    {
        //        mainDir = new DataManagment.CrystalData.HKLReflex(1, 0, 0, 1);
        //        secondDir = new DataManagment.CrystalData.HKLReflex(1, 0, 0, 1);
        //    }


        //    //OxyPlot.Series.LineSeries TmpSimulated = new OxyPlot.Series.LineSeries();
        //    TmpSimulated.Title = "Simulated Experiment No: " + ExperimentSelection.SelectedIndex;

        //    //X-Axis
        //    //[0]: "Sample Stress";
        //    //[1]: "Sample Strain";
        //    //[2]: "Sample Stress Rate";
        //    //[3]: "Sample Strain Rate";
        //    //[4]: "Grain Stress";
        //    //[5]: "Grain Strain";
        //    //[6]: "Grain Stress Rate";
        //    //[7]: "Grain Strain Rate";
        //    //[8]: "Lattice Strain";
        //    //[9]: "Lattice Strain Rate";
        //    //Y-Axis
        //    //[0]: "Sample Stress";
        //    //[1]: "Sample Strain";
        //    //[2]: "Sample Stress Rate";
        //    //[3]: "Sample Strain Rate";
        //    //[4]: "Grain Stress";
        //    //[5]: "Grain Strain";
        //    //[6]: "Grain Stress Rate";
        //    //[7]: "Grain Strain Rate";
        //    //[8]: "Lattice Strain";
        //    //[9]: "Lattice Strain Rate";
        //    //[10]: "Slip Activity";
        //    //[11]: "Slip Activity Grain";
        //    if (this.ExperimentSelection.SelectedIndex != -1 && this.PhaseSwitchBox.SelectedIndex != -1)
        //    {
        //        if (this.xAxesm.SelectedIndex == 0)
        //        {
        //            this.YieldXAxisLin.Title = "Sample Stress [MPa]";
        //            //TmpSimulated.Title += mainDir.HKLString;
        //            if (this.yAxesm.SelectedIndex == 0)
        //            {
        //                this.YieldYAxisLin.Title = "Sample Stress [MPa]";
        //                if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                {
        //                    usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory);
        //                }
        //                else
        //                {
        //                    int[] xIndex = { 2, 2 };
        //                    int[] yIndex = { 2, 2 };
        //                    usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory);
        //                }
        //                //usedCountsSimulated = this.ActSample.SampleStrainOverSampleStrainData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 1)
        //            {
        //                this.YieldYAxisLin.Title = "Sample strain";
        //                if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                {
        //                    usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory);
        //                }
        //                else
        //                {
        //                    int[] xIndex = { 2, 2 };
        //                    int[] yIndex = { 2, 2 };
        //                    usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory);
        //                }
        //                //if (!Convert.ToBoolean(PlotAllPsim.IsChecked))
        //                //{
        //                //    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                //    {
        //                //        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory);
        //                //    }
        //                //    else
        //                //    {
        //                //        int[] xIndex = { 2, 2 };
        //                //        int[] yIndex = { 2, 2 };
        //                //        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory);
        //                //    }
        //                //}
        //                //else
        //                //{
        //                //    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetSampleStrainWeighted();
        //                //    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                //    {
        //                //        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, orientedTensor, false, true);
        //                //    }
        //                //    else
        //                //    {
        //                //        int[] xIndex = { 2, 2 };
        //                //        int[] yIndex = { 2, 2 };
        //                //        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, orientedTensor, false, true);
        //                //    }
        //                //}
        //                //usedCountsSimulated = this.ActSample.SampleStrainOverGrainStrainData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 2)
        //            {
        //                this.YieldYAxisLin.Title = "Sample stress rate [MPa / Step]";
        //                if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                {
        //                    usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory);
        //                }
        //                else
        //                {
        //                    int[] xIndex = { 2, 2 };
        //                    int[] yIndex = { 2, 2 };
        //                    usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory);
        //                }
        //                //usedCountsSimulated = this.ActSample.SampleStrainOverSampleStressData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 3)
        //            {
        //                this.YieldYAxisLin.Title = "Sample strain rate [1 / Step]";
        //                if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                {
        //                    usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory);
        //                }
        //                else
        //                {
        //                    int[] xIndex = { 2, 2 };
        //                    int[] yIndex = { 2, 2 };
        //                    usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory);
        //                }
        //                //usedCountsSimulated = this.ActSample.SampleStrainOverGrainStrainData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 4)
        //            {
        //                this.YieldYAxisLin.Title = "Grain stress [MPa]";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, orientedTensor, false, true);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, orientedTensor, false, true);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.SampleStrainOverSampleShearStresData(mainDir, this.ExperimentSelection.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 5)
        //            {
        //                this.YieldYAxisLin.Title = "Grain strain";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, orientedTensor, false, true);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, orientedTensor, false, true);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.SampleStrainOverGrainShearStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 6)
        //            {
        //                this.YieldYAxisLin.Title = "Grain stress rate [ MPa / Step]";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, orientedTensor);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, orientedTensor);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.SampleStrainOverSampleStrainRateData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 7)
        //            { 
        //                this.YieldYAxisLin.Title = "Grain strain rate [1 / Step]";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, orientedTensor);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, orientedTensor);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.SampleStrainOverGrainStrainRateData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 8)
        //            {
        //                this.YieldYAxisLin.Title = "Lattice strain";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, this.GrainorientationList.SelectedIndex, this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances());
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, orientedTensor, false, true);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, orientedTensor, false, true);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.SampleStrainOverSampleShearStresData(mainDir, this.ExperimentSelection.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 9)
        //            {
        //                this.YieldYAxisLin.Title = "Lattice strain rate [1 / Step]";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, this.GrainorientationList.SelectedIndex, this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances());
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, orientedTensor, false, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, orientedTensor, false, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.SampleStrainOverSampleShearStresData(mainDir, this.ExperimentSelection.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 10)
        //            {
        //                this.YieldYAxisLin.Title = "Slip System Activity [%]";

        //                if ((bool)GroupSlipReflexesPlot.IsChecked)
        //                {
        //                    if (SlipFamilyList.SelectedIndex != -1)
        //                    {
        //                        if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                        {
        //                            List<ReflexYield> slipFamilySystems = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.GetSlipSystemsFamily((ReflexYield)SlipFamilyList.SelectedItem);
        //                            TmpSimulated.Title += slipFamilySystems[0].HKLString;
        //                            for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex].Count; n++)
        //                            {
        //                                double[] dataPoint = { 0.0, 0.0 };
        //                                dataPoint[0] = mainDir.Direction * (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory[n] * mainDir.Direction);

        //                                double totalActive = 0.0;
        //                                double familyActive = 0.0;

        //                                for(int i = 0; i < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n].Count; i++)
        //                                {
        //                                    if ((bool)this.RelativeActivityPlot.IsChecked)
        //                                    {
        //                                        totalActive += slipFamilySystems.Count;
        //                                    }
        //                                    else
        //                                    {
        //                                        totalActive += this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.PotentialSlipSystems.Count;
        //                                    }

        //                                    for(int j = 0; j < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i].Count; j++)
        //                                    {
        //                                        for (int k = 0; k < slipFamilySystems.Count; k++)
        //                                        {
        //                                            if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i][j].SlipPlane.HKLString == slipFamilySystems[k].SlipPlane.HKLString)
        //                                            {
        //                                                familyActive++;
        //                                            }
        //                                        }
        //                                    }
        //                                }

        //                                dataPoint[1] = familyActive / totalActive;

        //                                usedCountsSimulated.Add(dataPoint);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            List<ReflexYield> slipFamilySystems = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.GetSlipSystemsFamily((ReflexYield)SlipFamilyList.SelectedItem);
        //                            TmpSimulated.Title += slipFamilySystems[0].HKLString;
        //                            for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex].Count; n++)
        //                            {
        //                                double[] dataPoint = { 0.0, 0.0 };
        //                                dataPoint[0] = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory[n][2, 2];

        //                                double totalActive = 0.0;
        //                                double familyActive = 0.0;

        //                                for (int i = 0; i < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n].Count; i++)
        //                                {
        //                                    if ((bool)this.RelativeActivityPlot.IsChecked)
        //                                    {
        //                                        totalActive += slipFamilySystems.Count;
        //                                    }
        //                                    else
        //                                    {
        //                                        //totalActive += this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.PotentialSlipSystems.Count;
        //                                        totalActive += 5.0;
        //                                    }

        //                                    for (int j = 0; j < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i].Count; j++)
        //                                    {
        //                                        for (int k = 0; k < slipFamilySystems.Count; k++)
        //                                        {
        //                                            if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i][j].SlipPlane.HKLString == slipFamilySystems[k].SlipPlane.HKLString)
        //                                            {
        //                                                familyActive++;
        //                                                break;
        //                                            }
        //                                        }
        //                                    }
        //                                }

        //                                dataPoint[1] = familyActive / totalActive;

        //                                usedCountsSimulated.Add(dataPoint);
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {

        //                    if(PotenitalSlipSystemsList.SelectedIndex != -1)
        //                    {
        //                        if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                        {
        //                            ReflexYield selectedStatReflex = PotenitalSlipSystemsList.SelectedItem as ReflexYield;
        //                            List<ReflexYield> slipFamilySystems = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.GetSlipSystemsFamily((ReflexYield)SlipFamilyList.SelectedItem);
        //                            TmpSimulated.Title += slipFamilySystems[0].HKLString;
        //                            for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex].Count; n++)
        //                            {
        //                                double[] dataPoint = { 0.0, 0.0 };
        //                                dataPoint[0] = mainDir.Direction * (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory[n] * mainDir.Direction);

        //                                double totalActive = 0.0;
        //                                double familyActive = 0.0;

        //                                for (int i = 0; i < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n].Count; i++)
        //                                {
        //                                    if ((bool)this.RelativeActivityPlot.IsChecked)
        //                                    {
        //                                        totalActive += slipFamilySystems.Count;
        //                                    }
        //                                    else
        //                                    {
        //                                        totalActive += this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.PotentialSlipSystems.Count;
        //                                    }

        //                                    for (int j = 0; j < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i].Count; j++)
        //                                    {
        //                                        if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i][j].SlipPlane.HKLString == selectedStatReflex.SlipPlane.HKLString)
        //                                        {
        //                                            familyActive++;
        //                                        }
        //                                    }
        //                                }

        //                                dataPoint[1] = familyActive / totalActive;

        //                                usedCountsSimulated.Add(dataPoint);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            ReflexYield selectedStatReflex = PotenitalSlipSystemsList.SelectedItem as ReflexYield;
        //                            List<ReflexYield> slipFamilySystems = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.GetSlipSystemsFamily((ReflexYield)SlipFamilyList.SelectedItem);
        //                            TmpSimulated.Title += slipFamilySystems[0].HKLString;
        //                            for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex].Count; n++)
        //                            {
        //                                double[] dataPoint = { 0.0, 0.0 };
        //                                dataPoint[0] = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory[n][2, 2];

        //                                double totalActive = 0.0;
        //                                double familyActive = 0.0;

        //                                for (int i = 0; i < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n].Count; i++)
        //                                {
        //                                    if ((bool)this.RelativeActivityPlot.IsChecked)
        //                                    {
        //                                        totalActive += slipFamilySystems.Count;
        //                                    }
        //                                    else
        //                                    {
        //                                        totalActive += this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.PotentialSlipSystems.Count;
        //                                    }

        //                                    for (int j = 0; j < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i].Count; j++)
        //                                    {
        //                                        if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i][j].SlipPlane.HKLString == selectedStatReflex.SlipPlane.HKLString)
        //                                        {
        //                                            familyActive++;
        //                                        }
        //                                    }
        //                                }

        //                                dataPoint[1] = familyActive / totalActive;

        //                                usedCountsSimulated.Add(dataPoint);
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            else if (this.yAxesm.SelectedIndex == 11)
        //            {
        //                this.YieldYAxisLin.Title = "Slip System Activity";

        //                if ((bool)GroupSlipReflexesPlot.IsChecked)
        //                {
        //                    if (SlipFamilyList.SelectedIndex != -1)
        //                    {
        //                        if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                        {
        //                            List<ReflexYield> slipFamilySystems = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.GetSlipSystemsFamily((ReflexYield)SlipFamilyList.SelectedItem);
        //                            for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex].Count; n++)
        //                            {
        //                                double[] dataPoint = { 0.0, 0.0 };
        //                                dataPoint[0] = mainDir.Direction * (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory[n] * mainDir.Direction);

        //                                double totalActive = 0.0;
        //                                double familyActive = 0.0;

        //                                if ((bool)this.RelativeActivityPlot.IsChecked)
        //                                {
        //                                    totalActive += slipFamilySystems.Count;
        //                                }
        //                                else
        //                                {
        //                                    totalActive += this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.PotentialSlipSystems.Count;
        //                                }

        //                                for (int j = 0; j < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex].Count; j++)
        //                                {
        //                                    for (int k = 0; k < slipFamilySystems.Count; k++)
        //                                    {
        //                                        if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex][j].SlipPlane.HKLString == slipFamilySystems[k].SlipPlane.HKLString)
        //                                        {
        //                                            familyActive++;
        //                                        }
        //                                    }
        //                                }

        //                                dataPoint[1] = familyActive / totalActive;

        //                                usedCountsSimulated.Add(dataPoint);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            List<ReflexYield> slipFamilySystems = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.GetSlipSystemsFamily((ReflexYield)SlipFamilyList.SelectedItem);
        //                            for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex].Count; n++)
        //                            {
        //                                double[] dataPoint = { 0.0, 0.0 };
        //                                dataPoint[0] = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory[n][2, 2];

        //                                double totalActive = 0.0;
        //                                double familyActive = 0.0;

        //                                if ((bool)this.RelativeActivityPlot.IsChecked)
        //                                {
        //                                    totalActive += slipFamilySystems.Count;
        //                                }
        //                                else
        //                                {
        //                                    totalActive += this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.PotentialSlipSystems.Count;
        //                                }

        //                                for (int j = 0; j < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex].Count; j++)
        //                                {
        //                                    for (int k = 0; k < slipFamilySystems.Count; k++)
        //                                    {
        //                                        if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex][j].SlipPlane.HKLString == slipFamilySystems[k].SlipPlane.HKLString)
        //                                        {
        //                                            familyActive++;
        //                                        }
        //                                    }
        //                                }

        //                                dataPoint[1] = familyActive / totalActive;

        //                                usedCountsSimulated.Add(dataPoint);
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    if (PotenitalSlipSystemsList.SelectedIndex != -1)
        //                    {
        //                        if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                        {
        //                            ReflexYield selectedStatReflex = PotenitalSlipSystemsList.SelectedItem as ReflexYield;
        //                            List<ReflexYield> slipFamilySystems = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.GetSlipSystemsFamily((ReflexYield)SlipFamilyList.SelectedItem);
        //                            for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex].Count; n++)
        //                            {
        //                                double[] dataPoint = { 0.0, 0.0 };
        //                                dataPoint[0] = mainDir.Direction * (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory[n] * mainDir.Direction);

        //                                double totalActive = 0.0;
        //                                double familyActive = 0.0;

        //                                for (int i = 0; i < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n].Count; i++)
        //                                {
        //                                    if ((bool)this.RelativeActivityPlot.IsChecked)
        //                                    {
        //                                        totalActive += slipFamilySystems.Count;
        //                                    }
        //                                    else
        //                                    {
        //                                        totalActive += this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.PotentialSlipSystems.Count;
        //                                    }

        //                                    for (int j = 0; j < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i].Count; j++)
        //                                    {
        //                                        if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i][j].SlipPlane.HKLString == selectedStatReflex.SlipPlane.HKLString)
        //                                        {
        //                                            familyActive++;
        //                                        }
        //                                    }
        //                                }

        //                                dataPoint[1] = familyActive / totalActive;

        //                                usedCountsSimulated.Add(dataPoint);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            ReflexYield selectedStatReflex = PotenitalSlipSystemsList.SelectedItem as ReflexYield;
        //                            List<ReflexYield> slipFamilySystems = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.GetSlipSystemsFamily((ReflexYield)SlipFamilyList.SelectedItem);
        //                            for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex].Count; n++)
        //                            {
        //                                double[] dataPoint = { 0.0, 0.0 };
        //                                dataPoint[0] = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory[n][2, 2];

        //                                double totalActive = 0.0;
        //                                double familyActive = 0.0;

        //                                for (int i = 0; i < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n].Count; i++)
        //                                {
        //                                    if ((bool)this.RelativeActivityPlot.IsChecked)
        //                                    {
        //                                        totalActive += slipFamilySystems.Count;
        //                                    }
        //                                    else
        //                                    {
        //                                        totalActive += this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.PotentialSlipSystems.Count;
        //                                    }

        //                                    for (int j = 0; j < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i].Count; j++)
        //                                    {
        //                                        if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i][j].SlipPlane.HKLString == selectedStatReflex.SlipPlane.HKLString)
        //                                        {
        //                                            familyActive++;
        //                                        }
        //                                    }
        //                                }

        //                                dataPoint[1] = familyActive / totalActive;

        //                                usedCountsSimulated.Add(dataPoint);
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        else if (this.xAxesm.SelectedIndex == 1)
        //        {
        //            this.YieldXAxisLin.Title = "Sample strain";
        //            if (this.yAxesm.SelectedIndex == 0)
        //            {
        //                this.YieldYAxisLin.Title = "Sample stress [MPa]";
        //                if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                {
        //                    usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory);
        //                }
        //                else
        //                {
        //                    int[] xIndex = { 2, 2 };
        //                    int[] yIndex = { 2, 2 };
        //                    usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory);
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverSampleStrainData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 1)
        //            {
        //                this.YieldYAxisLin.Title = "Sample strain";
        //                if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                {
        //                    usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory);
        //                }
        //                else
        //                {
        //                    int[] xIndex = { 2, 2 };
        //                    int[] yIndex = { 2, 2 };
        //                    usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory);
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainStrainData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 2)
        //            {
        //                this.YieldYAxisLin.Title = "Sample stress rate [MPa / Step]";
        //                if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                {
        //                    usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory);
        //                }
        //                else
        //                {
        //                    int[] xIndex = { 2, 2 };
        //                    int[] yIndex = { 2, 2 };
        //                    usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory);
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverSampleStressData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 3)
        //            {
        //                this.YieldYAxisLin.Title = "Sample strain rate [1 / Step]";
        //                if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                {
        //                    usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory);
        //                }
        //                else
        //                {
        //                    int[] xIndex = { 2, 2 };
        //                    int[] yIndex = { 2, 2 };
        //                    usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory);
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainStressData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 4)
        //            {
        //                this.YieldYAxisLin.Title = "Grain stress [MPa]";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory, orientedTensor, false, true);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory, orientedTensor, false, true);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverSampleShearStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 5)
        //            {
        //                this.YieldYAxisLin.Title = "Grain strain";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory, orientedTensor, false, true);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory, orientedTensor, false, true);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainShearStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 6)
        //            {
        //                this.YieldYAxisLin.Title = "Grain stress rate [MPa / Step]";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory, orientedTensor);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory, orientedTensor);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverSampleStrainRateData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 7)
        //            {
        //                this.YieldYAxisLin.Title = "Grain strain rate [1 / Step]";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory, orientedTensor);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory, orientedTensor);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainStrainRateData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 8)
        //            {
        //                this.YieldYAxisLin.Title = "Lattice Strain";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, this.GrainorientationList.SelectedIndex, this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances());

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory, orientedTensor, false, true);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory, orientedTensor, false, true);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverSampleStrainRateData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 9)
        //            {
        //                this.YieldYAxisLin.Title = "Lattice strain rate [1 / Step]";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, this.GrainorientationList.SelectedIndex, this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances());

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory, orientedTensor, false, true);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory, orientedTensor, false, true);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverSampleStrainRateData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);

        //            }
        //            else if (this.yAxesm.SelectedIndex == 10)
        //            {
        //                this.YieldYAxisLin.Title = "Slip System Activity";

        //                if ((bool)GroupSlipReflexesPlot.IsChecked)
        //                {
        //                    if (SlipFamilyList.SelectedIndex != -1)
        //                    {
        //                        if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                        {
        //                            List<ReflexYield> slipFamilySystems = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.GetSlipSystemsFamily((ReflexYield)SlipFamilyList.SelectedItem);
        //                            for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex].Count; n++)
        //                            {
        //                                double[] dataPoint = { 0.0, 0.0 };
        //                                dataPoint[0] = mainDir.Direction * (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory[n] * mainDir.Direction);

        //                                double totalActive = 0.0;
        //                                double familyActive = 0.0;

        //                                for (int i = 0; i < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n].Count; i++)
        //                                {
        //                                    if ((bool)this.RelativeActivityPlot.IsChecked)
        //                                    {
        //                                        totalActive += slipFamilySystems.Count;
        //                                    }
        //                                    else
        //                                    {
        //                                        totalActive += this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.PotentialSlipSystems.Count;
        //                                    }

        //                                    for (int j = 0; j < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i].Count; j++)
        //                                    {
        //                                        for (int k = 0; k < slipFamilySystems.Count; k++)
        //                                        {
        //                                            if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i][j].SlipPlane.HKLString == slipFamilySystems[k].SlipPlane.HKLString)
        //                                            {
        //                                                familyActive++;
        //                                            }
        //                                        }
        //                                    }
        //                                }

        //                                dataPoint[1] = familyActive / totalActive;

        //                                usedCountsSimulated.Add(dataPoint);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            List<ReflexYield> slipFamilySystems = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.GetSlipSystemsFamily((ReflexYield)SlipFamilyList.SelectedItem);
        //                            for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex].Count; n++)
        //                            {
        //                                double[] dataPoint = { 0.0, 0.0 };
        //                                dataPoint[0] = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory[n][2, 2];

        //                                double totalActive = 0.0;
        //                                double familyActive = 0.0;

        //                                for (int i = 0; i < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n].Count; i++)
        //                                {
        //                                    if ((bool)this.RelativeActivityPlot.IsChecked)
        //                                    {
        //                                        totalActive += slipFamilySystems.Count;
        //                                    }
        //                                    else
        //                                    {
        //                                        totalActive += this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.PotentialSlipSystems.Count;
        //                                    }

        //                                    for (int j = 0; j < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i].Count; j++)
        //                                    {
        //                                        for (int k = 0; k < slipFamilySystems.Count; k++)
        //                                        {
        //                                            if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i][j].SlipPlane.HKLString == slipFamilySystems[k].SlipPlane.HKLString)
        //                                            {
        //                                                familyActive++;
        //                                            }
        //                                        }
        //                                    }
        //                                }

        //                                dataPoint[1] = familyActive / totalActive;

        //                                usedCountsSimulated.Add(dataPoint);
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    if (PotenitalSlipSystemsList.SelectedIndex != -1)
        //                    {
        //                        if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                        {
        //                            ReflexYield selectedStatReflex = PotenitalSlipSystemsList.SelectedItem as ReflexYield;
        //                            List<ReflexYield> slipFamilySystems = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.GetSlipSystemsFamily((ReflexYield)SlipFamilyList.SelectedItem);
        //                            for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex].Count; n++)
        //                            {
        //                                double[] dataPoint = { 0.0, 0.0 };
        //                                dataPoint[0] = mainDir.Direction * (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory[n] * mainDir.Direction);

        //                                double totalActive = 0.0;
        //                                double familyActive = 0.0;

        //                                for (int i = 0; i < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n].Count; i++)
        //                                {
        //                                    if ((bool)this.RelativeActivityPlot.IsChecked)
        //                                    {
        //                                        totalActive += slipFamilySystems.Count;
        //                                    }
        //                                    else
        //                                    {
        //                                        totalActive += this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.PotentialSlipSystems.Count;
        //                                    }

        //                                    for (int j = 0; j < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i].Count; j++)
        //                                    {
        //                                        if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i][j].SlipPlane.HKLString == selectedStatReflex.SlipPlane.HKLString)
        //                                        {
        //                                            familyActive++;
        //                                        }
        //                                    }
        //                                }

        //                                dataPoint[1] = familyActive / totalActive;

        //                                usedCountsSimulated.Add(dataPoint);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            ReflexYield selectedStatReflex = PotenitalSlipSystemsList.SelectedItem as ReflexYield;
        //                            List<ReflexYield> slipFamilySystems = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.GetSlipSystemsFamily((ReflexYield)SlipFamilyList.SelectedItem);
        //                            for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex].Count; n++)
        //                            {
        //                                double[] dataPoint = { 0.0, 0.0 };
        //                                dataPoint[0] = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory[n][2, 2];

        //                                double totalActive = 0.0;
        //                                double familyActive = 0.0;

        //                                for (int i = 0; i < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n].Count; i++)
        //                                {
        //                                    if ((bool)this.RelativeActivityPlot.IsChecked)
        //                                    {
        //                                        totalActive += slipFamilySystems.Count;
        //                                    }
        //                                    else
        //                                    {
        //                                        totalActive += this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.PotentialSlipSystems.Count;
        //                                    }

        //                                    for (int j = 0; j < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i].Count; j++)
        //                                    {
        //                                        if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i][j].SlipPlane.HKLString == selectedStatReflex.SlipPlane.HKLString)
        //                                        {
        //                                            familyActive++;
        //                                        }
        //                                    }
        //                                }

        //                                dataPoint[1] = familyActive / totalActive;

        //                                usedCountsSimulated.Add(dataPoint);
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            else if (this.yAxesm.SelectedIndex == 11)
        //            {
        //                this.YieldYAxisLin.Title = "Slip System Activity";

        //                if ((bool)GroupSlipReflexesPlot.IsChecked)
        //                {
        //                    if (SlipFamilyList.SelectedIndex != -1)
        //                    {
        //                        if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                        {
        //                            List<ReflexYield> slipFamilySystems = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.GetSlipSystemsFamily((ReflexYield)SlipFamilyList.SelectedItem);
        //                            for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex].Count; n++)
        //                            {
        //                                double[] dataPoint = { 0.0, 0.0 };
        //                                dataPoint[0] = mainDir.Direction * (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory[n] * mainDir.Direction);

        //                                double totalActive = 0.0;
        //                                double familyActive = 0.0;

        //                                if ((bool)this.RelativeActivityPlot.IsChecked)
        //                                {
        //                                    totalActive += slipFamilySystems.Count;
        //                                }
        //                                else
        //                                {
        //                                    totalActive += this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.PotentialSlipSystems.Count;
        //                                }

        //                                for (int j = 0; j < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex].Count; j++)
        //                                {
        //                                    for (int k = 0; k < slipFamilySystems.Count; k++)
        //                                    {
        //                                        if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex][j].SlipPlane.HKLString == slipFamilySystems[k].SlipPlane.HKLString)
        //                                        {
        //                                            familyActive++;
        //                                        }
        //                                    }
        //                                }

        //                                dataPoint[1] = familyActive / totalActive;

        //                                usedCountsSimulated.Add(dataPoint);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            List<ReflexYield> slipFamilySystems = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.GetSlipSystemsFamily((ReflexYield)SlipFamilyList.SelectedItem);
        //                            for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex].Count; n++)
        //                            {
        //                                double[] dataPoint = { 0.0, 0.0 };
        //                                dataPoint[0] = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory[n][2, 2];

        //                                double totalActive = 0.0;
        //                                double familyActive = 0.0;

        //                                if ((bool)this.RelativeActivityPlot.IsChecked)
        //                                {
        //                                    totalActive += slipFamilySystems.Count;
        //                                }
        //                                else
        //                                {
        //                                    totalActive += this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.PotentialSlipSystems.Count;
        //                                }

        //                                for (int j = 0; j < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex].Count; j++)
        //                                {
        //                                    for (int k = 0; k < slipFamilySystems.Count; k++)
        //                                    {
        //                                        if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex][j].SlipPlane.HKLString == slipFamilySystems[k].SlipPlane.HKLString)
        //                                        {
        //                                            familyActive++;
        //                                        }
        //                                    }
        //                                }

        //                                dataPoint[1] = familyActive / totalActive;

        //                                usedCountsSimulated.Add(dataPoint);
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    if (PotenitalSlipSystemsList.SelectedIndex != -1)
        //                    {
        //                        if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                        {
        //                            ReflexYield selectedStatReflex = PotenitalSlipSystemsList.SelectedItem as ReflexYield;
        //                            List<ReflexYield> slipFamilySystems = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.GetSlipSystemsFamily((ReflexYield)SlipFamilyList.SelectedItem);
        //                            for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex].Count; n++)
        //                            {
        //                                double[] dataPoint = { 0.0, 0.0 };
        //                                dataPoint[0] = mainDir.Direction * (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory[n] * mainDir.Direction);

        //                                double totalActive = 0.0;
        //                                double familyActive = 0.0;

        //                                for (int i = 0; i < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n].Count; i++)
        //                                {
        //                                    if ((bool)this.RelativeActivityPlot.IsChecked)
        //                                    {
        //                                        totalActive += slipFamilySystems.Count;
        //                                    }
        //                                    else
        //                                    {
        //                                        totalActive += this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.PotentialSlipSystems.Count;
        //                                    }

        //                                    for (int j = 0; j < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i].Count; j++)
        //                                    {
        //                                        if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i][j].SlipPlane.HKLString == selectedStatReflex.SlipPlane.HKLString)
        //                                        {
        //                                            familyActive++;
        //                                        }
        //                                    }
        //                                }

        //                                dataPoint[1] = familyActive / totalActive;

        //                                usedCountsSimulated.Add(dataPoint);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            ReflexYield selectedStatReflex = PotenitalSlipSystemsList.SelectedItem as ReflexYield;
        //                            List<ReflexYield> slipFamilySystems = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.GetSlipSystemsFamily((ReflexYield)SlipFamilyList.SelectedItem);
        //                            for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex].Count; n++)
        //                            {
        //                                double[] dataPoint = { 0.0, 0.0 };
        //                                dataPoint[0] = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory[n][2, 2];

        //                                double totalActive = 0.0;
        //                                double familyActive = 0.0;

        //                                for (int i = 0; i < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n].Count; i++)
        //                                {
        //                                    if ((bool)this.RelativeActivityPlot.IsChecked)
        //                                    {
        //                                        totalActive += slipFamilySystems.Count;
        //                                    }
        //                                    else
        //                                    {
        //                                        totalActive += this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.PotentialSlipSystems.Count;
        //                                    }

        //                                    for (int j = 0; j < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i].Count; j++)
        //                                    {
        //                                        if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i][j].SlipPlane.HKLString == selectedStatReflex.SlipPlane.HKLString)
        //                                        {
        //                                            familyActive++;
        //                                        }
        //                                    }
        //                                }

        //                                dataPoint[1] = familyActive / totalActive;

        //                                usedCountsSimulated.Add(dataPoint);
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        else if (this.xAxesm.SelectedIndex == 2)
        //        {
        //            this.YieldXAxisLin.Title = "Sample stress rate [MPa / Step]";
        //            if (this.yAxesm.SelectedIndex == 0)
        //            {
        //                this.YieldYAxisLin.Title = "Sample stress [MPa]";
        //                if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                {
        //                    usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory);
        //                }
        //                else
        //                {
        //                    int[] xIndex = { 2, 2 };
        //                    int[] yIndex = { 2, 2 };
        //                    usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory);
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverSampleStrainData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 1)
        //            {
        //                this.YieldYAxisLin.Title = "Sample strain";
        //                if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                {
        //                    usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory);
        //                }
        //                else
        //                {
        //                    int[] xIndex = { 2, 2 };
        //                    int[] yIndex = { 2, 2 };
        //                    usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory);
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainStrainData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 2)
        //            {
        //                this.YieldYAxisLin.Title = "Sample stress rate [MPa / Step]";
        //                if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                {
        //                    usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory);
        //                }
        //                else
        //                {
        //                    int[] xIndex = { 2, 2 };
        //                    int[] yIndex = { 2, 2 };
        //                    usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory);
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverSampleStressData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 3)
        //            {
        //                this.YieldYAxisLin.Title = "Sample strain rate [1 / Step]";
        //                if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                {
        //                    usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory);
        //                }
        //                else
        //                {
        //                    int[] xIndex = { 2, 2 };
        //                    int[] yIndex = { 2, 2 };
        //                    usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory);
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainStressData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 4)
        //            {
        //                this.YieldYAxisLin.Title = "Grain stress [MPa]";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory, orientedTensor, false, true);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory, orientedTensor, false, true);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverSampleShearStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 5)
        //            {
        //                this.YieldYAxisLin.Title = "Grain strain";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory, orientedTensor, false, true);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory, orientedTensor, false, true);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainShearStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 6)
        //            {
        //                this.YieldYAxisLin.Title = "Grain stress rate [1 / Step]";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory, orientedTensor, false, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory, orientedTensor, false, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverSampleStrainRateData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 7)
        //            {
        //                this.YieldYAxisLin.Title = "Grain strain rate [1 / Step]";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory, orientedTensor, false, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory, orientedTensor, false, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainStrainRateData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 8)
        //            {
        //                this.YieldYAxisLin.Title = "Lattice strain";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, this.GrainorientationList.SelectedIndex, this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances());

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory, orientedTensor, false, true);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory, orientedTensor, false, true);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainShearStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 9)
        //            {
        //                this.YieldYAxisLin.Title = "Lattice strain rate [1 / Step]";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, this.GrainorientationList.SelectedIndex, this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances());

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory, orientedTensor, false, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory, orientedTensor, false, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainShearStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 10)
        //            {
        //                this.YieldYAxisLin.Title = "Slip System Activity";

        //                if ((bool)GroupSlipReflexesPlot.IsChecked)
        //                {
        //                    if (SlipFamilyList.SelectedIndex != -1)
        //                    {
        //                        if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                        {
        //                            List<ReflexYield> slipFamilySystems = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.GetSlipSystemsFamily((ReflexYield)SlipFamilyList.SelectedItem);
        //                            for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex].Count; n++)
        //                            {
        //                                double[] dataPoint = { 0.0, 0.0 };
        //                                dataPoint[0] = mainDir.Direction * (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory[n] * mainDir.Direction);

        //                                double totalActive = 0.0;
        //                                double familyActive = 0.0;

        //                                for (int i = 0; i < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n].Count; i++)
        //                                {
        //                                    if ((bool)this.RelativeActivityPlot.IsChecked)
        //                                    {
        //                                        totalActive += slipFamilySystems.Count;
        //                                    }
        //                                    else
        //                                    {
        //                                        totalActive += this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.PotentialSlipSystems.Count;
        //                                    }

        //                                    for (int j = 0; j < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i].Count; j++)
        //                                    {
        //                                        for (int k = 0; k < slipFamilySystems.Count; k++)
        //                                        {
        //                                            if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i][j].SlipPlane.HKLString == slipFamilySystems[k].SlipPlane.HKLString)
        //                                            {
        //                                                familyActive++;
        //                                            }
        //                                        }
        //                                    }
        //                                }

        //                                dataPoint[1] = familyActive / totalActive;

        //                                usedCountsSimulated.Add(dataPoint);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            List<ReflexYield> slipFamilySystems = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.GetSlipSystemsFamily((ReflexYield)SlipFamilyList.SelectedItem);
        //                            for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex].Count; n++)
        //                            {
        //                                double[] dataPoint = { 0.0, 0.0 };
        //                                dataPoint[0] = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory[n][2, 2];

        //                                double totalActive = 0.0;
        //                                double familyActive = 0.0;

        //                                for (int i = 0; i < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n].Count; i++)
        //                                {
        //                                    if ((bool)this.RelativeActivityPlot.IsChecked)
        //                                    {
        //                                        totalActive += slipFamilySystems.Count;
        //                                    }
        //                                    else
        //                                    {
        //                                        totalActive += this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.PotentialSlipSystems.Count;
        //                                    }

        //                                    for (int j = 0; j < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i].Count; j++)
        //                                    {
        //                                        for (int k = 0; k < slipFamilySystems.Count; k++)
        //                                        {
        //                                            if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i][j].SlipPlane.HKLString == slipFamilySystems[k].SlipPlane.HKLString)
        //                                            {
        //                                                familyActive++;
        //                                            }
        //                                        }
        //                                    }
        //                                }

        //                                dataPoint[1] = familyActive / totalActive;

        //                                usedCountsSimulated.Add(dataPoint);
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    if (PotenitalSlipSystemsList.SelectedIndex != -1)
        //                    {
        //                        if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                        {
        //                            ReflexYield selectedStatReflex = PotenitalSlipSystemsList.SelectedItem as ReflexYield;
        //                            List<ReflexYield> slipFamilySystems = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.GetSlipSystemsFamily((ReflexYield)SlipFamilyList.SelectedItem);
        //                            for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex].Count; n++)
        //                            {
        //                                double[] dataPoint = { 0.0, 0.0 };
        //                                dataPoint[0] = mainDir.Direction * (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory[n] * mainDir.Direction);

        //                                double totalActive = 0.0;
        //                                double familyActive = 0.0;

        //                                for (int i = 0; i < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n].Count; i++)
        //                                {
        //                                    if ((bool)this.RelativeActivityPlot.IsChecked)
        //                                    {
        //                                        totalActive += slipFamilySystems.Count;
        //                                    }
        //                                    else
        //                                    {
        //                                        totalActive += this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.PotentialSlipSystems.Count;
        //                                    }

        //                                    for (int j = 0; j < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i].Count; j++)
        //                                    {
        //                                        if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i][j].SlipPlane.HKLString == selectedStatReflex.SlipPlane.HKLString)
        //                                        {
        //                                            familyActive++;
        //                                        }
        //                                    }
        //                                }

        //                                dataPoint[1] = familyActive / totalActive;

        //                                usedCountsSimulated.Add(dataPoint);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            ReflexYield selectedStatReflex = PotenitalSlipSystemsList.SelectedItem as ReflexYield;
        //                            List<ReflexYield> slipFamilySystems = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.GetSlipSystemsFamily((ReflexYield)SlipFamilyList.SelectedItem);
        //                            for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex].Count; n++)
        //                            {
        //                                double[] dataPoint = { 0.0, 0.0 };
        //                                dataPoint[0] = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory[n][2, 2];

        //                                double totalActive = 0.0;
        //                                double familyActive = 0.0;

        //                                for (int i = 0; i < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n].Count; i++)
        //                                {
        //                                    if ((bool)this.RelativeActivityPlot.IsChecked)
        //                                    {
        //                                        totalActive += slipFamilySystems.Count;
        //                                    }
        //                                    else
        //                                    {
        //                                        totalActive += this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.PotentialSlipSystems.Count;
        //                                    }

        //                                    for (int j = 0; j < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i].Count; j++)
        //                                    {
        //                                        if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i][j].SlipPlane.HKLString == selectedStatReflex.SlipPlane.HKLString)
        //                                        {
        //                                            familyActive++;
        //                                        }
        //                                    }
        //                                }

        //                                dataPoint[1] = familyActive / totalActive;

        //                                usedCountsSimulated.Add(dataPoint);
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            else if (this.yAxesm.SelectedIndex == 11)
        //            {
        //                this.YieldYAxisLin.Title = "Slip System Activity";

        //                if ((bool)GroupSlipReflexesPlot.IsChecked)
        //                {
        //                    if (SlipFamilyList.SelectedIndex != -1)
        //                    {
        //                        if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                        {
        //                            List<ReflexYield> slipFamilySystems = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.GetSlipSystemsFamily((ReflexYield)SlipFamilyList.SelectedItem);
        //                            for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex].Count; n++)
        //                            {
        //                                double[] dataPoint = { 0.0, 0.0 };
        //                                dataPoint[0] = mainDir.Direction * (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory[n] * mainDir.Direction);

        //                                double totalActive = 0.0;
        //                                double familyActive = 0.0;

        //                                if ((bool)this.RelativeActivityPlot.IsChecked)
        //                                {
        //                                    totalActive += slipFamilySystems.Count;
        //                                }
        //                                else
        //                                {
        //                                    totalActive += this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.PotentialSlipSystems.Count;
        //                                }

        //                                for (int j = 0; j < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex].Count; j++)
        //                                {
        //                                    for (int k = 0; k < slipFamilySystems.Count; k++)
        //                                    {
        //                                        if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex][j].SlipPlane.HKLString == slipFamilySystems[k].SlipPlane.HKLString)
        //                                        {
        //                                            familyActive++;
        //                                        }
        //                                    }
        //                                }

        //                                dataPoint[1] = familyActive / totalActive;

        //                                usedCountsSimulated.Add(dataPoint);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            List<ReflexYield> slipFamilySystems = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.GetSlipSystemsFamily((ReflexYield)SlipFamilyList.SelectedItem);
        //                            for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex].Count; n++)
        //                            {
        //                                double[] dataPoint = { 0.0, 0.0 };
        //                                dataPoint[0] = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory[n][2, 2];

        //                                double totalActive = 0.0;
        //                                double familyActive = 0.0;

        //                                if ((bool)this.RelativeActivityPlot.IsChecked)
        //                                {
        //                                    totalActive += slipFamilySystems.Count;
        //                                }
        //                                else
        //                                {
        //                                    totalActive += this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.PotentialSlipSystems.Count;
        //                                }

        //                                for (int j = 0; j < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex].Count; j++)
        //                                {
        //                                    for (int k = 0; k < slipFamilySystems.Count; k++)
        //                                    {
        //                                        if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex][j].SlipPlane.HKLString == slipFamilySystems[k].SlipPlane.HKLString)
        //                                        {
        //                                            familyActive++;
        //                                        }
        //                                    }
        //                                }

        //                                dataPoint[1] = familyActive / totalActive;

        //                                usedCountsSimulated.Add(dataPoint);
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    if (PotenitalSlipSystemsList.SelectedIndex != -1)
        //                    {
        //                        if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                        {
        //                            ReflexYield selectedStatReflex = PotenitalSlipSystemsList.SelectedItem as ReflexYield;
        //                            List<ReflexYield> slipFamilySystems = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.GetSlipSystemsFamily((ReflexYield)SlipFamilyList.SelectedItem);
        //                            for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex].Count; n++)
        //                            {
        //                                double[] dataPoint = { 0.0, 0.0 };
        //                                dataPoint[0] = mainDir.Direction * (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory[n] * mainDir.Direction);

        //                                double totalActive = 0.0;
        //                                double familyActive = 0.0;

        //                                for (int i = 0; i < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n].Count; i++)
        //                                {
        //                                    if ((bool)this.RelativeActivityPlot.IsChecked)
        //                                    {
        //                                        totalActive += slipFamilySystems.Count;
        //                                    }
        //                                    else
        //                                    {
        //                                        totalActive += this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.PotentialSlipSystems.Count;
        //                                    }

        //                                    for (int j = 0; j < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i].Count; j++)
        //                                    {
        //                                        if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i][j].SlipPlane.HKLString == selectedStatReflex.SlipPlane.HKLString)
        //                                        {
        //                                            familyActive++;
        //                                        }
        //                                    }
        //                                }

        //                                dataPoint[1] = familyActive / totalActive;

        //                                usedCountsSimulated.Add(dataPoint);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            ReflexYield selectedStatReflex = PotenitalSlipSystemsList.SelectedItem as ReflexYield;
        //                            List<ReflexYield> slipFamilySystems = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.GetSlipSystemsFamily((ReflexYield)SlipFamilyList.SelectedItem);
        //                            for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex].Count; n++)
        //                            {
        //                                double[] dataPoint = { 0.0, 0.0 };
        //                                dataPoint[0] = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory[n][2, 2];

        //                                double totalActive = 0.0;
        //                                double familyActive = 0.0;

        //                                for (int i = 0; i < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n].Count; i++)
        //                                {
        //                                    if ((bool)this.RelativeActivityPlot.IsChecked)
        //                                    {
        //                                        totalActive += slipFamilySystems.Count;
        //                                    }
        //                                    else
        //                                    {
        //                                        totalActive += this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.PotentialSlipSystems.Count;
        //                                    }

        //                                    for (int j = 0; j < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i].Count; j++)
        //                                    {
        //                                        if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i][j].SlipPlane.HKLString == selectedStatReflex.SlipPlane.HKLString)
        //                                        {
        //                                            familyActive++;
        //                                        }
        //                                    }
        //                                }

        //                                dataPoint[1] = familyActive / totalActive;

        //                                usedCountsSimulated.Add(dataPoint);
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        else if (this.xAxesm.SelectedIndex == 3)
        //        {
        //            this.YieldXAxisLin.Title = "Sample strain rate [1 / Step]";
        //            if (this.yAxesm.SelectedIndex == 0)
        //            {
        //                this.YieldYAxisLin.Title = "Sample stress [MPa]";
        //                if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                {
        //                    usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory);
        //                }
        //                else
        //                {
        //                    int[] xIndex = { 2, 2 };
        //                    int[] yIndex = { 2, 2 };
        //                    usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory);
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverSampleStrainData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 1)
        //            {
        //                this.YieldYAxisLin.Title = "Sample strain";
        //                if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                {
        //                    usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory);
        //                }
        //                else
        //                {
        //                    int[] xIndex = { 2, 2 };
        //                    int[] yIndex = { 2, 2 };
        //                    usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory);
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainStrainData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 2)
        //            {
        //                this.YieldYAxisLin.Title = "Sample stress rate [MPa / Step]";
        //                if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                {
        //                    usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory);
        //                }
        //                else
        //                {
        //                    int[] xIndex = { 2, 2 };
        //                    int[] yIndex = { 2, 2 };
        //                    usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory);
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverSampleStressData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 3)
        //            {
        //                this.YieldYAxisLin.Title = "Sample strain rate [1 / Step]";
        //                if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                {
        //                    usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory);
        //                }
        //                else
        //                {
        //                    int[] xIndex = { 2, 2 };
        //                    int[] yIndex = { 2, 2 };
        //                    usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory);
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainStressData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 4)
        //            {
        //                this.YieldYAxisLin.Title = "Grain stress [MPa]";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory, orientedTensor, false, true);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory, orientedTensor, false, true);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverSampleShearStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 5)
        //            {
        //                this.YieldYAxisLin.Title = "Grain strain";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory, orientedTensor, false, true);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory, orientedTensor, false, true);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainShearStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 6)
        //            {
        //                this.YieldYAxisLin.Title = "Grain stress rate [MPa / Step]";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory, orientedTensor, false, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory, orientedTensor, false, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverSampleStrainRateData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 7)
        //            {
        //                this.YieldYAxisLin.Title = "Grain strain rate [1 / Step]";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory, orientedTensor, false, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory, orientedTensor, false, true);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainStrainRateData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 8)
        //            {
        //                this.YieldYAxisLin.Title = "Lattice strain";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, this.GrainorientationList.SelectedIndex, this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances());

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory, orientedTensor, false, true);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory, orientedTensor, false, true);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainShearStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 9)
        //            {
        //                this.YieldYAxisLin.Title = "Lattice strain rate [1 / Step]";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, this.GrainorientationList.SelectedIndex, this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances());

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory, orientedTensor, false, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory, orientedTensor, false, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainShearStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 10)
        //            {
        //                this.YieldYAxisLin.Title = "Slip System Activity";

        //                if ((bool)GroupSlipReflexesPlot.IsChecked)
        //                {
        //                    if (SlipFamilyList.SelectedIndex != -1)
        //                    {
        //                        if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                        {
        //                            List<ReflexYield> slipFamilySystems = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.GetSlipSystemsFamily((ReflexYield)SlipFamilyList.SelectedItem);
        //                            for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex].Count; n++)
        //                            {
        //                                double[] dataPoint = { 0.0, 0.0 };
        //                                dataPoint[0] = mainDir.Direction * (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory[n] * mainDir.Direction);

        //                                double totalActive = 0.0;
        //                                double familyActive = 0.0;

        //                                for (int i = 0; i < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n].Count; i++)
        //                                {
        //                                    if ((bool)this.RelativeActivityPlot.IsChecked)
        //                                    {
        //                                        totalActive += slipFamilySystems.Count;
        //                                    }
        //                                    else
        //                                    {
        //                                        totalActive += this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.PotentialSlipSystems.Count;
        //                                    }

        //                                    for (int j = 0; j < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i].Count; j++)
        //                                    {
        //                                        for (int k = 0; k < slipFamilySystems.Count; k++)
        //                                        {
        //                                            if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i][j].SlipPlane.HKLString == slipFamilySystems[k].SlipPlane.HKLString)
        //                                            {
        //                                                familyActive++;
        //                                            }
        //                                        }
        //                                    }
        //                                }

        //                                dataPoint[1] = familyActive / totalActive;

        //                                usedCountsSimulated.Add(dataPoint);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            List<ReflexYield> slipFamilySystems = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.GetSlipSystemsFamily((ReflexYield)SlipFamilyList.SelectedItem);
        //                            for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex].Count; n++)
        //                            {
        //                                double[] dataPoint = { 0.0, 0.0 };
        //                                dataPoint[0] = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory[n][2, 2];

        //                                double totalActive = 0.0;
        //                                double familyActive = 0.0;

        //                                for (int i = 0; i < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n].Count; i++)
        //                                {
        //                                    if ((bool)this.RelativeActivityPlot.IsChecked)
        //                                    {
        //                                        totalActive += slipFamilySystems.Count;
        //                                    }
        //                                    else
        //                                    {
        //                                        totalActive += this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.PotentialSlipSystems.Count;
        //                                    }

        //                                    for (int j = 0; j < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i].Count; j++)
        //                                    {
        //                                        for (int k = 0; k < slipFamilySystems.Count; k++)
        //                                        {
        //                                            if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i][j].SlipPlane.HKLString == slipFamilySystems[k].SlipPlane.HKLString)
        //                                            {
        //                                                familyActive++;
        //                                            }
        //                                        }
        //                                    }
        //                                }

        //                                dataPoint[1] = familyActive / totalActive;

        //                                usedCountsSimulated.Add(dataPoint);
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    if (PotenitalSlipSystemsList.SelectedIndex != -1)
        //                    {
        //                        if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                        {
        //                            ReflexYield selectedStatReflex = PotenitalSlipSystemsList.SelectedItem as ReflexYield;
        //                            List<ReflexYield> slipFamilySystems = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.GetSlipSystemsFamily((ReflexYield)SlipFamilyList.SelectedItem);
        //                            for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex].Count; n++)
        //                            {
        //                                double[] dataPoint = { 0.0, 0.0 };
        //                                dataPoint[0] = mainDir.Direction * (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory[n] * mainDir.Direction);

        //                                double totalActive = 0.0;
        //                                double familyActive = 0.0;

        //                                for (int i = 0; i < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n].Count; i++)
        //                                {
        //                                    if ((bool)this.RelativeActivityPlot.IsChecked)
        //                                    {
        //                                        totalActive += slipFamilySystems.Count;
        //                                    }
        //                                    else
        //                                    {
        //                                        totalActive += this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.PotentialSlipSystems.Count;
        //                                    }

        //                                    for (int j = 0; j < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i].Count; j++)
        //                                    {
        //                                        if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i][j].SlipPlane.HKLString == selectedStatReflex.SlipPlane.HKLString)
        //                                        {
        //                                            familyActive++;
        //                                        }
        //                                    }
        //                                }

        //                                dataPoint[1] = familyActive / totalActive;

        //                                usedCountsSimulated.Add(dataPoint);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            ReflexYield selectedStatReflex = PotenitalSlipSystemsList.SelectedItem as ReflexYield;
        //                            List<ReflexYield> slipFamilySystems = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.GetSlipSystemsFamily((ReflexYield)SlipFamilyList.SelectedItem);
        //                            for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex].Count; n++)
        //                            {
        //                                double[] dataPoint = { 0.0, 0.0 };
        //                                dataPoint[0] = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory[n][2, 2];

        //                                double totalActive = 0.0;
        //                                double familyActive = 0.0;

        //                                for (int i = 0; i < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n].Count; i++)
        //                                {
        //                                    if ((bool)this.RelativeActivityPlot.IsChecked)
        //                                    {
        //                                        totalActive += slipFamilySystems.Count;
        //                                    }
        //                                    else
        //                                    {
        //                                        totalActive += this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.PotentialSlipSystems.Count;
        //                                    }

        //                                    for (int j = 0; j < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i].Count; j++)
        //                                    {
        //                                        if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i][j].SlipPlane.HKLString == selectedStatReflex.SlipPlane.HKLString)
        //                                        {
        //                                            familyActive++;
        //                                        }
        //                                    }
        //                                }

        //                                dataPoint[1] = familyActive / totalActive;

        //                                usedCountsSimulated.Add(dataPoint);
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            else if (this.yAxesm.SelectedIndex == 11)
        //            {
        //                this.YieldYAxisLin.Title = "Slip System Activity";

        //                if ((bool)GroupSlipReflexesPlot.IsChecked)
        //                {
        //                    if (SlipFamilyList.SelectedIndex != -1)
        //                    {
        //                        if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                        {
        //                            List<ReflexYield> slipFamilySystems = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.GetSlipSystemsFamily((ReflexYield)SlipFamilyList.SelectedItem);
        //                            for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex].Count; n++)
        //                            {
        //                                double[] dataPoint = { 0.0, 0.0 };
        //                                dataPoint[0] = mainDir.Direction * (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory[n] * mainDir.Direction);

        //                                double totalActive = 0.0;
        //                                double familyActive = 0.0;

        //                                if ((bool)this.RelativeActivityPlot.IsChecked)
        //                                {
        //                                    totalActive += slipFamilySystems.Count;
        //                                }
        //                                else
        //                                {
        //                                    totalActive += this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.PotentialSlipSystems.Count;
        //                                }

        //                                for (int j = 0; j < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex].Count; j++)
        //                                {
        //                                    for (int k = 0; k < slipFamilySystems.Count; k++)
        //                                    {
        //                                        if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex][j].SlipPlane.HKLString == slipFamilySystems[k].SlipPlane.HKLString)
        //                                        {
        //                                            familyActive++;
        //                                        }
        //                                    }
        //                                }

        //                                dataPoint[1] = familyActive / totalActive;

        //                                usedCountsSimulated.Add(dataPoint);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            List<ReflexYield> slipFamilySystems = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.GetSlipSystemsFamily((ReflexYield)SlipFamilyList.SelectedItem);
        //                            for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex].Count; n++)
        //                            {
        //                                double[] dataPoint = { 0.0, 0.0 };
        //                                dataPoint[0] = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory[n][2, 2];

        //                                double totalActive = 0.0;
        //                                double familyActive = 0.0;

        //                                if ((bool)this.RelativeActivityPlot.IsChecked)
        //                                {
        //                                    totalActive += slipFamilySystems.Count;
        //                                }
        //                                else
        //                                {
        //                                    totalActive += this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.PotentialSlipSystems.Count;
        //                                }

        //                                for (int j = 0; j < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex].Count; j++)
        //                                {
        //                                    for (int k = 0; k < slipFamilySystems.Count; k++)
        //                                    {
        //                                        if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex][j].SlipPlane.HKLString == slipFamilySystems[k].SlipPlane.HKLString)
        //                                        {
        //                                            familyActive++;
        //                                        }
        //                                    }
        //                                }

        //                                dataPoint[1] = familyActive / totalActive;

        //                                usedCountsSimulated.Add(dataPoint);
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    if (PotenitalSlipSystemsList.SelectedIndex != -1)
        //                    {
        //                        if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                        {
        //                            ReflexYield selectedStatReflex = PotenitalSlipSystemsList.SelectedItem as ReflexYield;
        //                            List<ReflexYield> slipFamilySystems = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.GetSlipSystemsFamily((ReflexYield)SlipFamilyList.SelectedItem);
        //                            for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex].Count; n++)
        //                            {
        //                                double[] dataPoint = { 0.0, 0.0 };
        //                                dataPoint[0] = mainDir.Direction * (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory[n] * mainDir.Direction);

        //                                double totalActive = 0.0;
        //                                double familyActive = 0.0;

        //                                for (int i = 0; i < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n].Count; i++)
        //                                {
        //                                    if ((bool)this.RelativeActivityPlot.IsChecked)
        //                                    {
        //                                        totalActive += slipFamilySystems.Count;
        //                                    }
        //                                    else
        //                                    {
        //                                        totalActive += this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.PotentialSlipSystems.Count;
        //                                    }

        //                                    for (int j = 0; j < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i].Count; j++)
        //                                    {
        //                                        if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i][j].SlipPlane.HKLString == selectedStatReflex.SlipPlane.HKLString)
        //                                        {
        //                                            familyActive++;
        //                                        }
        //                                    }
        //                                }

        //                                dataPoint[1] = familyActive / totalActive;

        //                                usedCountsSimulated.Add(dataPoint);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            ReflexYield selectedStatReflex = PotenitalSlipSystemsList.SelectedItem as ReflexYield;
        //                            List<ReflexYield> slipFamilySystems = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.GetSlipSystemsFamily((ReflexYield)SlipFamilyList.SelectedItem);
        //                            for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex].Count; n++)
        //                            {
        //                                double[] dataPoint = { 0.0, 0.0 };
        //                                dataPoint[0] = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory[n][2, 2];

        //                                double totalActive = 0.0;
        //                                double familyActive = 0.0;

        //                                for (int i = 0; i < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n].Count; i++)
        //                                {
        //                                    if ((bool)this.RelativeActivityPlot.IsChecked)
        //                                    {
        //                                        totalActive += slipFamilySystems.Count;
        //                                    }
        //                                    else
        //                                    {
        //                                        totalActive += this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.PotentialSlipSystems.Count;
        //                                    }

        //                                    for (int j = 0; j < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i].Count; j++)
        //                                    {
        //                                        if (this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].ActiveSystemsCFOrientedHistory[this.PhaseSwitchBox.SelectedIndex][n][i][j].SlipPlane.HKLString == selectedStatReflex.SlipPlane.HKLString)
        //                                        {
        //                                            familyActive++;
        //                                        }
        //                                    }
        //                                }

        //                                dataPoint[1] = familyActive / totalActive;

        //                                usedCountsSimulated.Add(dataPoint);
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        else if (this.xAxesm.SelectedIndex == 4)
        //        {
        //            this.YieldXAxisLin.Title = "Grain Stress [MPa]";
        //            if (this.yAxesm.SelectedIndex == 0)
        //            {
        //                this.YieldYAxisLin.Title = "Sample stress [MPa]";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, true, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, true, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverSampleStrainData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 1)
        //            {
        //                this.YieldYAxisLin.Title = "Sample strain";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory, true, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory, true, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainStrainData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 2)
        //            {
        //                this.YieldYAxisLin.Title = "Sample stress rate [MPa / Step]";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory, true, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory, true, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverSampleStressData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 3)
        //            {
        //                this.YieldYAxisLin.Title = "Sample strain rate [1 / Step]";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory, true, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory, true, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainStressData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 4)
        //            {
        //                this.YieldYAxisLin.Title = "Grain stress [MPa]";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensor, orientedTensor, true, true);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensor, orientedTensor, true, true);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverSampleShearStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 5)
        //            {
        //                this.YieldYAxisLin.Title = "Grain strain";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensorX = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensorY = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensorX.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                        orientedTensorY.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensorX, orientedTensorY, true, true);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensorX, orientedTensorY, true, true);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainShearStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 6)
        //            {
        //                this.YieldYAxisLin.Title = "Grain stress rate [MPa / Step]";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensor, orientedTensor, true, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensor, orientedTensor, true, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverSampleStrainRateData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 7)
        //            {
        //                this.YieldYAxisLin.Title = "Grain strain rate [1 / Step]";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensorX = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensorY = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensorX.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                        orientedTensorY.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensorX, orientedTensorY, true, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensorX, orientedTensorY, true, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainStrainRateData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 8)
        //            {
        //                this.YieldYAxisLin.Title = "Lattice strain";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {

        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensorX = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensorY = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, this.GrainorientationList.SelectedIndex, this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances());
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensorX.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensorX, orientedTensorY, true, true);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensorX, orientedTensorY, true, true);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainShearStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 9)
        //            {
        //                this.YieldYAxisLin.Title = "Lattice strain rate [1 / Step]";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensorX = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensorY = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, this.GrainorientationList.SelectedIndex, this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances());
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensorX.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensorX, orientedTensorY, true, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensorX, orientedTensorY, true, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainShearStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //        }
        //        else if (this.xAxesm.SelectedIndex == 5)
        //        {
        //            this.YieldXAxisLin.Title = "Grain strain";
        //            if (this.yAxesm.SelectedIndex == 0)
        //            {
        //                this.YieldYAxisLin.Title = "Sample stress [MPa]";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, true, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, true, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverSampleStrainData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 1)
        //            {
        //                this.YieldYAxisLin.Title = "Sample strain";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory, true, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory, true, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainStrainData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 2)
        //            {
        //                this.YieldYAxisLin.Title = "Sample stress rate [MPa / Step]";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory, true, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory, true, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverSampleStressData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 3)
        //            {
        //                this.YieldYAxisLin.Title = "Sample strain rate [1 / Step]";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory, true, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory, true, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainStressData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 4)
        //            {
        //                this.YieldYAxisLin.Title = "Grain stress [MPa]";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensorX = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensorY = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensorX.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                        orientedTensorY.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensorX, orientedTensorY, true, true);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensorX, orientedTensorY, true, true);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverSampleShearStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 5)
        //            {
        //                this.YieldYAxisLin.Title = "Grain strain";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensor, orientedTensor, true, true);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensor, orientedTensor, true, true);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainShearStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 6)
        //            {
        //                this.YieldYAxisLin.Title = "Grain stress rate [MPa / Step]";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensorX = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensorY = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensorX.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                        orientedTensorY.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensorX, orientedTensorY, true, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensorX, orientedTensorY, true, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverSampleStrainRateData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 7)
        //            {
        //                this.YieldYAxisLin.Title = "Grain strain rate [1 / Step]";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensor, orientedTensor, true, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensor, orientedTensor, true, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainStrainRateData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 8)
        //            {
        //                this.YieldYAxisLin.Title = "Lattice strain";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensorY = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, this.GrainorientationList.SelectedIndex, this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances());
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensor, orientedTensorY, true, true);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensor, orientedTensorY, true, true);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainShearStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 9)
        //            {
        //                this.YieldYAxisLin.Title = "Lattice strain rate [1 / Step]";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensorY = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, this.GrainorientationList.SelectedIndex, this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances());
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensor, orientedTensorY, true, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensor, orientedTensorY, true, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainShearStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //        }
        //        else if (this.xAxesm.SelectedIndex == 6)
        //        {
        //            this.YieldXAxisLin.Title = "Grain stress rate ";
        //            if (this.yAxesm.SelectedIndex == 0)
        //            {
        //                this.YieldYAxisLin.Title = "Sample stress";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {

        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, false, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, false, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverSampleStrainData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 1)
        //            {
        //                this.YieldYAxisLin.Title = "Sample strain";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory, false, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory, false, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainStrainData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 2)
        //            {
        //                this.YieldYAxisLin.Title = "Sample stress rate";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory, false, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory, false, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverSampleStressData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 3)
        //            {
        //                this.YieldYAxisLin.Title = "Sample strain rate";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory, false, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory, false, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainStressData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 4)
        //            {
        //                this.YieldYAxisLin.Title = "Grain stress";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensor, orientedTensor, false, true);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensor, orientedTensor, false, true);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverSampleShearStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 5)
        //            {
        //                this.YieldYAxisLin.Title = "Grain strain";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensorX = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensorY = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensorX.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                        orientedTensorY.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensorX, orientedTensorY, false, true);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensorX, orientedTensorY, false, true);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainShearStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 6)
        //            {
        //                this.YieldYAxisLin.Title = "Grain stress rate";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensor, orientedTensor, false, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensor, orientedTensor, false, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverSampleStrainRateData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 7)
        //            {
        //                this.YieldYAxisLin.Title = "Grain strain rate";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensorX = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensorY = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensorX.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                        orientedTensorY.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensorX, orientedTensorY, false, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensorX, orientedTensorY, false, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainStrainRateData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 8)
        //            {
        //                this.YieldYAxisLin.Title = "Lattice strain";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensorX = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensorY = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, this.GrainorientationList.SelectedIndex, this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances());
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensorX.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensorX, orientedTensorY, false, true);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensorX, orientedTensorY, false, true);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainShearStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 9)
        //            {
        //                this.YieldYAxisLin.Title = "Lattice strain rate";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensorX = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensorY = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, this.GrainorientationList.SelectedIndex, this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances());
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensorX.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensorX, orientedTensorY, false, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensorX, orientedTensorY, false, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainShearStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //        }
        //        else if (this.xAxesm.SelectedIndex == 7)
        //        {
        //            this.YieldXAxisLin.Title = "Grain Strain rate";
        //            if (this.yAxesm.SelectedIndex == 0)
        //            {
        //                this.YieldYAxisLin.Title = "Sample stress";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, false, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, false, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverSampleStrainData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 1)
        //            {
        //                this.YieldYAxisLin.Title = "Sample strain";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory, false, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory, false, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainStrainData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 2)
        //            {
        //                this.YieldYAxisLin.Title = "Sample stress rate";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory, false, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory, false, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverSampleStressData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 3)
        //            {
        //                this.YieldYAxisLin.Title = "Sample strain rate";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory, false, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory, false, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainStressData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 4)
        //            {
        //                this.YieldYAxisLin.Title = "Grain stress";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensorX = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensorY = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensorX.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                        orientedTensorY.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensorX, orientedTensorY, false, true);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensorX, orientedTensorY, false, true);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverSampleShearStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 5)
        //            {
        //                this.YieldYAxisLin.Title = "Grain strain";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensor, orientedTensor, false, true);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensor, orientedTensor, false, true);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainShearStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 6)
        //            {
        //                this.YieldYAxisLin.Title = "Grain stress rate";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensorX = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensorY = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensorX.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                        orientedTensorY.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensorX, orientedTensorY, false, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensorX, orientedTensorY, false, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverSampleStrainRateData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 7)
        //            {
        //                this.YieldYAxisLin.Title = "Grain strain rate";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensor, orientedTensor, false, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensor, orientedTensor, false, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainStrainRateData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 8)
        //            {
        //                this.YieldYAxisLin.Title = "Lattice strain";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensorY = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, this.GrainorientationList.SelectedIndex, this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances());
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensor, orientedTensorY, false, true);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensor, orientedTensorY, false, true);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainShearStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 9)
        //            {
        //                this.YieldYAxisLin.Title = "Lattice strain rate";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensorY = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, this.GrainorientationList.SelectedIndex, this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances());
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensor, orientedTensorY, false, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensor, orientedTensorY, false, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainShearStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //        }
        //        else if (this.xAxesm.SelectedIndex == 8)
        //        {
        //            this.YieldXAxisLin.Title = "Lattice strain";
        //            if (this.yAxesm.SelectedIndex == 0)
        //            {
        //                this.YieldYAxisLin.Title = "Sample stress [MPa]";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, this.GrainorientationList.SelectedIndex, this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances());

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, true, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, true, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverSampleStrainData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 1)
        //            {
        //                this.YieldYAxisLin.Title = "Sample strain";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, this.GrainorientationList.SelectedIndex, this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances());

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory, true, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory, true, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainStrainData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 2)
        //            {
        //                this.YieldYAxisLin.Title = "Sample stress rate [MPa / Step]";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, this.GrainorientationList.SelectedIndex, this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances());

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory, true, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory, true, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverSampleStressData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 3)
        //            {
        //                this.YieldYAxisLin.Title = "Sample strain rate [1 / Step]";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, this.GrainorientationList.SelectedIndex, this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances());

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory, true, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory, true, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainStressData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 4)
        //            {
        //                this.YieldYAxisLin.Title = "Grain stress [MPa]";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensorX = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, this.GrainorientationList.SelectedIndex, this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances());
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensorY = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensorY.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensorX, orientedTensorY, true, true);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensorX, orientedTensorY, true, true);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverSampleShearStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 5)
        //            {
        //                this.YieldYAxisLin.Title = "Grain strain";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensorX = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, this.GrainorientationList.SelectedIndex, this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances());
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensorX, orientedTensor, true, true);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensorX, orientedTensor, true, true);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainShearStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 6)
        //            {
        //                this.YieldYAxisLin.Title = "Grain stress rate [MPa / Step]";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensorX = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, this.GrainorientationList.SelectedIndex, this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances());
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensorY = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensorY.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensorX, orientedTensorY, true, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensorX, orientedTensorY, true, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverSampleStrainRateData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 7)
        //            {
        //                this.YieldYAxisLin.Title = "Grain strain rate [1 / Step]";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensorX = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, this.GrainorientationList.SelectedIndex, this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances());
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensorX, orientedTensor, true, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensorX, orientedTensor, true, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainStrainRateData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 8)
        //            {
        //                this.YieldYAxisLin.Title = "Lattice strain";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, this.GrainorientationList.SelectedIndex, this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances());
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensorY = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, this.GrainorientationList.SelectedIndex, this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances());

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensor, orientedTensorY, true, true);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensor, orientedTensorY, true, true);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainShearStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 9)
        //            {
        //                this.YieldYAxisLin.Title = "Lattice strain rate [1 / Step]";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, this.GrainorientationList.SelectedIndex, this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances());
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensorY = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, this.GrainorientationList.SelectedIndex, this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances());

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensor, orientedTensorY, true, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensor, orientedTensorY, true, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainShearStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //        }
        //        else if (this.xAxesm.SelectedIndex == 9)
        //        {
        //            this.YieldXAxisLin.Title = "Lattice strain rate [1 / Step]";
        //            if (this.yAxesm.SelectedIndex == 0)
        //            {
        //                this.YieldYAxisLin.Title = "Sample stress [MPa]";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, this.GrainorientationList.SelectedIndex, this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances());

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, false, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory, false, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverSampleStrainData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 1)
        //            {
        //                this.YieldYAxisLin.Title = "Sample strain";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, this.GrainorientationList.SelectedIndex, this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances());

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory, false, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory, false, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainStrainData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 2)
        //            {
        //                this.YieldYAxisLin.Title = "Sample stress rate [MPa / Step]";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, this.GrainorientationList.SelectedIndex, this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances());

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory, false, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory, false, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverSampleStressData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 3)
        //            {
        //                this.YieldYAxisLin.Title = "Sample strain rate [1 / Step]";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, this.GrainorientationList.SelectedIndex, this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances());

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory, false, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensor, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory, false, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainStressData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 4)
        //            {
        //                this.YieldYAxisLin.Title = "Grain stress [MPa]";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensorX = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, this.GrainorientationList.SelectedIndex, this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances());
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensorY = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensorY.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensorX, orientedTensorY, false, true);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensorX, orientedTensorY, false, true);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverSampleShearStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 5)
        //            {
        //                this.YieldYAxisLin.Title = "Grain strain";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensorX = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, this.GrainorientationList.SelectedIndex, this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances());
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensorX, orientedTensor, false, true);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensorX, orientedTensor, false, true);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainShearStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 6)
        //            {
        //                this.YieldYAxisLin.Title = "Grain stress rate [MPa / Step]";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensorX = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, this.GrainorientationList.SelectedIndex, this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances());
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensorY = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensorY.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensorX, orientedTensorY, false, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensorX, orientedTensorY, false, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverSampleStrainRateData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 7)
        //            {
        //                this.YieldYAxisLin.Title = "Grain strain rate [1 / Step]";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensorX = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, this.GrainorientationList.SelectedIndex, this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances());
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        //                    for (int n = 0; n < this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex].Count; n++)
        //                    {
        //                        orientedTensor.Add(this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFOrientedHistory[PhaseSwitchBox.SelectedIndex][n][this.GrainorientationList.SelectedIndex]);
        //                    }

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensorX, orientedTensor, false, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensorX, orientedTensor, false, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainStrainRateData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 8)
        //            {
        //                this.YieldYAxisLin.Title = "Lattice strain";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, this.GrainorientationList.SelectedIndex, this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances());
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensorY = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, this.GrainorientationList.SelectedIndex, this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances());

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensor, orientedTensorY, false, true);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensor, orientedTensorY, false, true);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainShearStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //            else if (this.yAxesm.SelectedIndex == 9)
        //            {
        //                this.YieldYAxisLin.Title = "Lattice strain rate [1 / Step]";
        //                if (this.GrainorientationList.SelectedIndex != -1)
        //                {
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensor = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, this.GrainorientationList.SelectedIndex, this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances());
        //                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> orientedTensorY = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].GetLatticeStrainRate(PhaseSwitchBox.SelectedIndex, this.GrainorientationList.SelectedIndex, this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex].GetFourtRankCompliances());

        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, orientedTensor, orientedTensorY, false, false);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, orientedTensor, orientedTensorY, false, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Convert.ToBoolean(this.DirectionalPlot.IsChecked))
        //                    {
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(mainDir, secondDir, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                    else
        //                    {
        //                        int[] xIndex = { 2, 2 };
        //                        int[] yIndex = { 2, 2 };
        //                        usedCountsSimulated = this.ActSample.SimulationDataDisplay(xIndex, yIndex, this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex]);
        //                    }
        //                }
        //                //usedCountsSimulated = this.ActSample.GrainStrainOverGrainShearStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
        //            }
        //        }
        //    }

        //    #endregion

        //    return usedCountsSimulated;
        //}

        #endregion
    }

    public struct MatrixView
    {
        public double Index
        { get; set; }
        public double Value1
        { get; set; }
        public double Value2
        { get; set; }
        public double Value3
        { get; set; }
        public double Value4
        { get; set; }
        public double Value5
        { get; set; }
        public double Value6
        { get; set; }
    }

    public struct SimulationTask
    {
        public string Name
        { get; set; }
        public int Index
        { get; set; }
        public int NumberOfCycles
        { get; set; }
    }
}
