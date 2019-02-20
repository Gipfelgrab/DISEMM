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

            //for (int n = 0; n < this.ActSample.ReussTensorData.Count; n++)
            //{
            //    //this.ActSample.ReussTensorData[n].SetPeakStressAssociation(this.ActSample);
            //    if (this.ActSample.ReussTensorData[n].DiffractionConstants.Count == 0)
            //    {
            //        this.ActSample.ReussTensorData[n].DiffractionConstants = ActSample.DiffractionConstants[n];
            //    }
            //    if (this.ActSample.HillTensorData[n].DiffractionConstants.Count == 0)
            //    {
            //        this.ActSample.HillTensorData[n].DiffractionConstants = ActSample.DiffractionConstants[n];
            //    }
            //    if (this.ActSample.KroenerTensorData[n].DiffractionConstants.Count == 0)
            //    {
            //        this.ActSample.KroenerTensorData[n].DiffractionConstants = ActSample.DiffractionConstants[n];
            //    }
            //    if (this.ActSample.DeWittTensorData[n].DiffractionConstants.Count == 0)
            //    {
            //        this.ActSample.DeWittTensorData[n].DiffractionConstants = ActSample.DiffractionConstants[n];
            //    }
            //    if (this.ActSample.GeometricHillTensorData[n].DiffractionConstants.Count == 0)
            //    {
            //        this.ActSample.GeometricHillTensorData[n].DiffractionConstants = ActSample.DiffractionConstants[n];
            //    }
            //}

            for (int n = 0; n < this.ActSample.CrystalData.Count; n++)
            {
                ComboBoxItem PhaseItem = new ComboBoxItem();
                PhaseItem.Content = this.ActSample.CrystalData[n].SymmetryGroup;

                this.PhaseSwitchBox.Items.Add(PhaseItem);
            }

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

            YieldXAxisLin.Position = OxyPlot.Axes.AxisPosition.Bottom;
            YieldXAxisLin.Minimum = 0;
            YieldXAxisLin.Maximum = 180;
            YieldXAxisLin.Title = "Macroscopic strain";

            YieldYAxisLin.Position = OxyPlot.Axes.AxisPosition.Left;
            YieldYAxisLin.Minimum = 0;
            YieldYAxisLin.Maximum = 100;
            YieldYAxisLin.Title = "Micro strain";

            YieldXAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dash;
            YieldYAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dash;

            YieldXAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dot;
            YieldYAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dot;

            this.YieldPlotModel.Axes.Add(YieldXAxisLin);
            this.YieldPlotModel.Axes.Add(YieldYAxisLin);

            this.PW.MainPlot.Model = YieldPlotModel;

            YieldFractionPlotModel.LegendBorder = OxyPlot.OxyColors.Black;
            YieldFractionPlotModel.LegendItemAlignment = OxyPlot.HorizontalAlignment.Left;
            YieldFractionPlotModel.LegendTitle = "Region";

            YieldFractionXAxisLin.Position = OxyPlot.Axes.AxisPosition.Bottom;
            YieldFractionXAxisLin.Minimum = 0;
            YieldFractionXAxisLin.Maximum = 180;
            YieldFractionXAxisLin.Title = "Macroscopic strain";

            YieldFractionYAxisLin.Position = OxyPlot.Axes.AxisPosition.Left;
            YieldFractionYAxisLin.Minimum = 0;
            YieldFractionYAxisLin.Maximum = 100;
            YieldFractionYAxisLin.Title = "Micro strain";

            YieldFractionXAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dash;
            YieldFractionYAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dash;

            YieldFractionXAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dot;
            YieldFractionYAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dot;

            this.YieldFractionPlotModel.Axes.Add(YieldFractionXAxisLin);
            this.YieldFractionPlotModel.Axes.Add(YieldFractionYAxisLin);

            //this.FractionPlot.Model = YieldFractionPlotModel;

            ComboBoxItem CBIXA1 = new ComboBoxItem();
            CBIXA1.Content = "Sample strain";
            this.xAxesm.Items.Add(CBIXA1);
            ComboBoxItem CBIXA2 = new ComboBoxItem();
            CBIXA2.Content = "Grain strain";
            this.xAxesm.Items.Add(CBIXA2);
            ComboBoxItem CBIXA3 = new ComboBoxItem();
            CBIXA3.Content = "Sample Stress";
            this.xAxesm.Items.Add(CBIXA3);
            ComboBoxItem CBIXA4 = new ComboBoxItem();
            CBIXA4.Content = "Grain Stress";
            this.xAxesm.Items.Add(CBIXA4);
            ComboBoxItem CBIXA5 = new ComboBoxItem();
            CBIXA5.Content =  "Sample Stress";
            this.xAxesm.Items.Add(CBIXA5);
            ComboBoxItem CBIXA6 = new ComboBoxItem();
            CBIXA6.Content = "Grain Shear Stress";
            this.xAxesm.Items.Add(CBIXA6);
            ComboBoxItem CBIXA7 = new ComboBoxItem();
            CBIXA7.Content = "Sample strain Rate";
            this.xAxesm.Items.Add(CBIXA7);
            ComboBoxItem CBIXA8 = new ComboBoxItem();
            CBIXA8.Content = "Grain strain Rate";
            this.xAxesm.Items.Add(CBIXA8);

            ComboBoxItem CBIYA1 = new ComboBoxItem();
            CBIYA1.Content = "Sample strain";
            this.yAxesm.Items.Add(CBIYA1);
            ComboBoxItem CBIYA2 = new ComboBoxItem();
            CBIYA2.Content = "Grain strain";
            this.yAxesm.Items.Add(CBIYA2);
            ComboBoxItem CBIYA3 = new ComboBoxItem();
            CBIYA3.Content = "Sample Stress";
            this.yAxesm.Items.Add(CBIYA3);
            ComboBoxItem CBIYA4 = new ComboBoxItem();
            CBIYA4.Content = "Grain Stress";
            this.yAxesm.Items.Add(CBIYA4);
            ComboBoxItem CBIYA5 = new ComboBoxItem();
            CBIYA5.Content = "Sample Stress";
            this.yAxesm.Items.Add(CBIYA5);
            ComboBoxItem CBIYA6 = new ComboBoxItem();
            CBIYA6.Content = "Grain Shear Stress";
            this.yAxesm.Items.Add(CBIYA6);
            ComboBoxItem CBIYA7 = new ComboBoxItem();
            CBIYA7.Content = "Sample strain Rate";
            this.yAxesm.Items.Add(CBIYA7);
            ComboBoxItem CBIYA8 = new ComboBoxItem();
            CBIYA8.Content = "Grain strain Rate";
            this.yAxesm.Items.Add(CBIYA8);

            ComboBoxItem CBIFXA1 = new ComboBoxItem();
            CBIFXA1.Content = "Macro strain";
            this.xAxesf.Items.Add(CBIFXA1);
            ComboBoxItem CBIFXA2 = new ComboBoxItem();
            CBIFXA2.Content = "Micro strain";
            this.xAxesf.Items.Add(CBIFXA2);
            ComboBoxItem CBIFXA3 = new ComboBoxItem();
            CBIFXA3.Content = "Stress";
            this.xAxesf.Items.Add(CBIFXA3);
            ComboBoxItem CBIFXA5 = new ComboBoxItem();
            CBIFXA5.Content = "Load Plain Stress";
            this.xAxesf.Items.Add(CBIFXA5);
            ComboBoxItem CBIFXA4 = new ComboBoxItem();
            CBIFXA4.Content = "Slip plain orientation Stress";
            this.xAxesf.Items.Add(CBIFXA4);

            ComboBoxItem CBIFYA1 = new ComboBoxItem();
            CBIFYA1.Content = "Macro strain";
            this.yAxesf.Items.Add(CBIFYA1);
            ComboBoxItem CBIFYA2 = new ComboBoxItem();
            CBIFYA2.Content = "Micro strain";
            this.yAxesf.Items.Add(CBIFYA2);
            ComboBoxItem CBIFYA3 = new ComboBoxItem();
            CBIFYA3.Content = "Stress";
            this.yAxesf.Items.Add(CBIFYA3);
            ComboBoxItem CBIFYA4 = new ComboBoxItem();
            CBIFYA4.Content = "Load orientation Stress";
            this.yAxesf.Items.Add(CBIFYA4);
            ComboBoxItem CBIFYA5 = new ComboBoxItem();
            CBIFYA5.Content = "Slip plain stress";
            this.yAxesf.Items.Add(CBIFYA5);

            this.SetSimulationExperiemnts();

            MatrixView stressRow1 = new MatrixView();
            MatrixView stressRow2 = new MatrixView();
            MatrixView stressRow3 = new MatrixView();

            stressRow1.Value1 = 0.0;
            stressRow1.Value2 = 0.0;
            stressRow1.Value3 = 0.0;

            stressRow2.Value1 = 0.0;
            stressRow2.Value2 = 0.0;
            stressRow2.Value3 = 0.0;

            stressRow3.Value1 = 0.0;
            stressRow3.Value2 = 0.0;
            stressRow3.Value3 = 0.0;

            this.sMV.Add(stressRow1);
            this.sMV.Add(stressRow2);
            this.sMV.Add(stressRow3);

            MatrixView hardeningRow1 = new MatrixView();
            MatrixView hardeningRow2 = new MatrixView();
            MatrixView hardeningRow3 = new MatrixView();

            hardeningRow1.Value1 = 0.0;
            hardeningRow1.Value2 = 0.0;
            hardeningRow1.Value3 = 0.0;

            hardeningRow2.Value1 = 0.0;
            hardeningRow2.Value2 = 0.0;
            hardeningRow2.Value3 = 0.0;

            hardeningRow3.Value1 = 0.0;
            hardeningRow3.Value2 = 0.0;
            hardeningRow3.Value3 = 0.0;

            this.hMV.Add(hardeningRow1);
            this.hMV.Add(hardeningRow2);
            this.hMV.Add(hardeningRow3);

            StressTensorData.ItemsSource = this.sMV;
            HardenningTensorData.ItemsSource = this.hMV;

            IndexEventAktive = true;
        }

        private void PhaseSwitchBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(this.PhaseSwitchBox.SelectedIndex != -1)
            {
                HardenningTensorData.ItemsSource = null;
                this.ReflexList.ItemsSource = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData;
                this.PotenitalSlipSystemsList.ItemsSource = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.PotentialSlipSystems;
                if (PsiAnglef.SelectedIndex != -1)
                {
                    this.SetFractionPlot();
                }
                this.hMV.Clear();

                MatrixView hardeningRow1 = new MatrixView();
                MatrixView hardeningRow2 = new MatrixView();
                MatrixView hardeningRow3 = new MatrixView();

                hardeningRow1.Value1 = 0.0;
                hardeningRow1.Value2 = 0.0;
                hardeningRow1.Value3 = 0.0;

                hardeningRow2.Value1 = 0.0;
                hardeningRow2.Value2 = 0.0;
                hardeningRow2.Value3 = 0.0;

                hardeningRow3.Value1 = 0.0;
                hardeningRow3.Value2 = 0.0;
                hardeningRow3.Value3 = 0.0;

                this.hMV.Add(hardeningRow1);
                this.hMV.Add(hardeningRow2);
                this.hMV.Add(hardeningRow3);

                HardenningTensorData.ItemsSource = this.hMV;
            }
        }

        private void ReflexList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(this.ReflexList.SelectedIndex != -1)
            {
                IndexEventAktive = false;

                ReflexYield SelectedRY = (ReflexYield)this.ReflexList.SelectedItem;

                this.PsiAnglem.Items.Clear();
                this.PsiAnglef.Items.Clear();
                this.ComboPsiAngles.Clear();
                for (int n = 0; n < SelectedRY.PeakData.Count; n++)
                {
                    this.ComboPsiAngles.Add(SelectedRY.PeakData[n][0].PsiAngle);
                    ComboBoxItem PsiAngleCBI = new ComboBoxItem();
                    PsiAngleCBI.Content = SelectedRY.PeakData[n][0].PsiAngle.ToString("F3");
                    ComboBoxItem PsiAngleCBI1 = new ComboBoxItem();
                    PsiAngleCBI1.Content = SelectedRY.PeakData[n][0].PsiAngle.ToString("F3");

                    this.PsiAnglem.Items.Add(PsiAngleCBI);
                    this.PsiAnglef.Items.Add(PsiAngleCBI1);
                }

                this.PsiAnglem.SelectedIndex = 0;
                this.PsiAnglef.SelectedIndex = 0;

                this.ReflexYieldSlipDirection.Text = SelectedRY.YieldMainStrength.ToString("F3");
                this.ReflexYieldSlipDirectionSecondary.Text = SelectedRY.YieldSecondaryStrength.ToString("F3");

                for (int n = 0; n < SelectedRY.PeakData.Count; n++)
                {
                    if(Math.Abs(Convert.ToDouble(this.PsiAnglem.Text) - SelectedRY.PeakData[n][0].PsiAngle) < CalScec.Properties.Settings.Default.PsyAcceptanceAngle)
                    {
                        this.PeakList.ItemsSource = SelectedRY.PeakData[n];
                        double lowElBorder = 100000;
                        double highElBorder = 0;
                        double lowPlBorder = 100000;
                        double highPlBorder = 0;

                        for(int i = 0; i < SelectedRY.PeakData[n].Count; i++)
                        {
                            if(SelectedRY.PeakData[n][i].ElasticRegime)
                            {
                                if(lowElBorder > SelectedRY.PeakData[n][i].Stress)
                                {
                                    lowElBorder = SelectedRY.PeakData[n][i].Stress;
                                }
                                
                                if(highElBorder < SelectedRY.PeakData[n][i].Stress)
                                {
                                    highElBorder = SelectedRY.PeakData[n][i].Stress;
                                }
                            }
                            else
                            {
                                if (lowPlBorder > SelectedRY.PeakData[n][i].Stress)
                                {
                                    lowPlBorder = SelectedRY.PeakData[n][i].Stress;
                                }

                                if (highPlBorder < SelectedRY.PeakData[n][i].Stress)
                                {
                                    highPlBorder = SelectedRY.PeakData[n][i].Stress;
                                }
                            }
                        }

                        //this.ElasticLow.Text = lowElBorder.ToString("F3");
                        //this.ElasticHigh.Text = highElBorder.ToString("F3");
                        //this.PlasticLow.Text = lowElBorder.ToString("F3");
                        //this.PlasticHigh.Text = highElBorder.ToString("F3");

                        break;
                    }
                }

                this.SetPlot(SelectedRY);

                IndexEventAktive = true;
            }
        }

        private void SetPlot(ReflexYield RY)
        {
            #region Data points

            //this.YieldPlotModel.Series.Clear();
            double psiAngletmp = this.ComboPsiAngles[PsiAnglem.SelectedIndex];
            Pattern.Counts UsedCountsMeasured = RY.MicroStrainOverMacroStrainData(true, psiAngletmp);
            Pattern.Counts UsedCountsSimulated = new Pattern.Counts();

            #region Axis selection
            //[0]: MacroStrain
            //[1]: MicroStrain
            //[2]: Stress
            //[3]: Plain adjusted Stress
            //[4]: Psi adjusted Stress
            if (this.xAxesm.SelectedIndex == 0)
            {
                this.YieldYAxisLin.Title = "Macro strain";
                if (this.yAxesm.SelectedIndex == 0)
                {
                    this.YieldXAxisLin.Title = "Macro strain";
                    UsedCountsMeasured = RY.MacroStrainOverMacroStrainData(true, psiAngletmp);
                    if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                    {
                        UsedCountsSimulated = this.ActSample.MacroStrainOverMacroStrainData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex]);
                    }
                }
                else if (this.yAxesm.SelectedIndex == 1)
                {
                    this.YieldXAxisLin.Title = "Micro strain";
                    UsedCountsMeasured = RY.MacroStrainOverMicroStrainData(true, psiAngletmp);
                    if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                    {
                        UsedCountsSimulated = this.ActSample.MacroStrainOverMicroStrainData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], RY);
                    }
                }
                else if (this.yAxesm.SelectedIndex == 3)
                {
                    this.YieldXAxisLin.Title = "Load direction stress";
                    UsedCountsMeasured = RY.MacroStrainDataOverPsiAdjustedStress(true, psiAngletmp);
                    //if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                    //{
                    //    UsedCountsSimulated = this.ActSample.MacroStrainDataOverStressRD(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex]);
                    //}
                }
                else if (this.yAxesm.SelectedIndex == 4)
                {
                    this.YieldXAxisLin.Title = "Slip plain direction stress";
                    UsedCountsMeasured = RY.MacroStrainDataOverPlainAdjustedStress(true, psiAngletmp);
                    if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                    {
                        UsedCountsSimulated = this.ActSample.MacroStrainDataOverPlainAdjustedStress(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], RY);
                    }
                }
                else
                {
                    this.YieldXAxisLin.Title = "Total stress";
                    UsedCountsMeasured = RY.MacroStrainDataOverStress(true, psiAngletmp);
                    if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                    {
                        UsedCountsSimulated = this.ActSample.MacroStrainDataOverStressRD(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex]);
                    }
                }
            }
            else if(this.xAxesm.SelectedIndex == 1)
            {
                this.YieldYAxisLin.Title = "Micro strain";
                if (this.yAxesm.SelectedIndex == 0)
                {
                    this.YieldXAxisLin.Title = "Macro strain";
                    UsedCountsMeasured = RY.MicroStrainOverMacroStrainData(true, psiAngletmp);
                    if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                    {
                        UsedCountsSimulated = this.ActSample.MicroStrainOverMacroStrainData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], RY);
                    }
                }
                else if (this.yAxesm.SelectedIndex == 1)
                {
                    this.YieldXAxisLin.Title = "Micro strain";
                    UsedCountsMeasured = RY.MicroStrainOverMicroStrainData(true, psiAngletmp);
                    if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                    {
                        UsedCountsSimulated = this.ActSample.MicroStrainOverMicroStrainData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], RY);
                    }
                }
                else if (this.yAxesm.SelectedIndex == 3)
                {
                    this.YieldXAxisLin.Title = "Load direction stress";
                    UsedCountsMeasured = RY.MicroStrainDataOverPsiAdjustedStress(true, psiAngletmp);
                    //if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                    //{
                    //    UsedCountsSimulated = this.ActSample.MicroStrainDataOverPsiAdjustedStress(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], RY);
                    //}
                }
                else if (this.yAxesm.SelectedIndex == 4)
                {
                    this.YieldXAxisLin.Title = "Slip plain direction stress";
                    UsedCountsMeasured = RY.MicroStrainDataOverPlainAdjustedStress(true, psiAngletmp);
                    if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                    {
                        UsedCountsSimulated = this.ActSample.MicroStrainDataOverPlainAdjustedStress(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], RY);
                    }
                }
                else
                {
                    this.YieldXAxisLin.Title = "Total stress";
                    UsedCountsMeasured = RY.MicroStrainDataOverStress(true, psiAngletmp);
                    if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                    {
                        UsedCountsSimulated = this.ActSample.MicroStrainDataOverStressRD(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], RY);
                    }
                }
            }
            else if(this.xAxesm.SelectedIndex == 2)
            {
                this.YieldYAxisLin.Title = "Total stress";
                if (this.yAxesm.SelectedIndex == 0)
                {
                    this.YieldXAxisLin.Title = "Macro strain";
                    UsedCountsMeasured = RY.StressOverMacroStrainData(true, psiAngletmp);
                    if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                    {
                        UsedCountsSimulated = this.ActSample.StressRDOverMacroStrainData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex]);
                    }
                }
                else if (this.yAxesm.SelectedIndex == 1)
                {
                    this.YieldXAxisLin.Title = "Micro strain";
                    UsedCountsMeasured = RY.StressOverMicroStrainData(true, psiAngletmp);
                    if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                    {
                        UsedCountsSimulated = this.ActSample.StressRDOverMicroStrainData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], RY);
                    }
                }
                else if (this.yAxesm.SelectedIndex == 2)
                {
                    this.YieldXAxisLin.Title = "Total stress";
                    UsedCountsMeasured = RY.StressOverStressData(true, psiAngletmp);
                    if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                    {
                        UsedCountsSimulated = this.ActSample.StressRDOverStressData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], RY);
                    }
                }
                else if (this.yAxesm.SelectedIndex == 3)
                {
                    this.YieldXAxisLin.Title = "Load direction stress";
                    UsedCountsMeasured = RY.StressOverPsiAdjustedStressData(true, psiAngletmp);
                    //if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                    //{
                    //    UsedCountsSimulated = this.ActSample.StressOverPsiAdjustedStressData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], RY);
                    //}
                }
                else
                {
                    this.YieldXAxisLin.Title = "Slip plain direction stress";
                    UsedCountsMeasured = RY.StressOverPlainAdjustedStressData(true, psiAngletmp);
                    if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                    {
                        UsedCountsSimulated = this.ActSample.StressRDOverPlainAdjustedStressData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], RY);
                    }
                }
            }
            else if (this.xAxesm.SelectedIndex == 3)
            {
                this.YieldYAxisLin.Title = "Load direction stress";
                if (this.yAxesm.SelectedIndex == 0)
                {
                    this.YieldXAxisLin.Title = "Macro strain";
                    UsedCountsMeasured = RY.PsiAdjustedStressOverMacroStrainData(true, psiAngletmp);
                    if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                    {
                        UsedCountsSimulated = this.ActSample.MicroStrainOverMacroStrainData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], RY);
                    }
                }
                else if (this.yAxesm.SelectedIndex == 1)
                {
                    this.YieldXAxisLin.Title = "Micro strain";
                    UsedCountsMeasured = RY.PsiAdjustedStressOverMicroStrainData(true, psiAngletmp);
                    //if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                    //{
                    //    UsedCountsSimulated = this.ActSample.PsiAdjustedStressOverMacroStrainData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], RY);
                    //}
                }
                else if (this.yAxesm.SelectedIndex == 2)
                {
                    this.YieldXAxisLin.Title = "Total stress";
                    UsedCountsMeasured = RY.PsiAdjustedStressOverStressData(true, psiAngletmp);
                    //if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                    //{
                    //    UsedCountsSimulated = this.ActSample.PsiAdjustedStressOverStressData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], RY);
                    //}
                }
                else if (this.yAxesm.SelectedIndex == 3)
                {
                    this.YieldXAxisLin.Title = "Load direction stress";
                    UsedCountsMeasured = RY.PsiAdjustedStressOverPsiAdjustedStressData(true, psiAngletmp);
                    //if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                    //{
                    //    UsedCountsSimulated = this.ActSample.PsiAdjustedStressOverPsiAdjustedStressData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], RY);
                    //}
                }
                else if (this.yAxesm.SelectedIndex == 3)
                {
                    this.YieldXAxisLin.Title = "Slip plain direction stress";
                    UsedCountsMeasured = RY.PsiAdjustedStressOverPlainAdjustedStressData(true, psiAngletmp);
                    //if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                    //{
                    //    UsedCountsSimulated = this.ActSample.PsiAdjustedStressOverPlainAdjustedStressData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], RY);
                    //}
                }
            }
            else
            {
                this.YieldYAxisLin.Title = "Slip plain direction stress";
                if (this.yAxesm.SelectedIndex == 0)
                {
                    this.YieldXAxisLin.Title = "Macro strain";
                    UsedCountsMeasured = RY.PlainAdjustedStressOverMacroStrainData(true, psiAngletmp);
                    if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                    {
                        UsedCountsSimulated = this.ActSample.PlainAdjustedStressOverMacroStrainData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], RY);
                    }
                }
                else if (this.yAxesm.SelectedIndex == 1)
                {
                    this.YieldXAxisLin.Title = "Micro strain";
                    UsedCountsMeasured = RY.PlainAdjustedStressOverMicroStrainData(true, psiAngletmp);
                    if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                    {
                        UsedCountsSimulated = this.ActSample.PlainAdjustedStressOverMicroStrainData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], RY);
                    }
                }
                else if (this.yAxesm.SelectedIndex == 2)
                {
                    this.YieldXAxisLin.Title = "Total stress";
                    UsedCountsMeasured = RY.PlainAdjustedStressOverStressData(true, psiAngletmp);
                    if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                    {
                        UsedCountsSimulated = this.ActSample.PlainAdjustedStressOverStressRDData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], RY);
                    }
                }
                else if (this.yAxesm.SelectedIndex == 3)
                {
                    this.YieldXAxisLin.Title = "Load direction stress";
                    UsedCountsMeasured = RY.PlainAdjustedStressOverPsiAdjustedStressData(true, psiAngletmp);
                    //if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                    //{
                    //    UsedCountsSimulated = this.ActSample.PlainAdjustedStressOverPsiAdjustedStressData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], RY);
                    //}
                }
                else
                {
                    this.YieldXAxisLin.Title = "Slip plain direction stress";
                    UsedCountsMeasured = RY.PlainAdjustedStressOverPlainAdjustedStressData(true, psiAngletmp);
                    if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                    {
                        UsedCountsSimulated = this.ActSample.PlainAdjustedStressOverPlainAdjustedStressData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], RY);
                    }
                }
            }

            #endregion

            OxyPlot.Series.LineSeries TmpMeasured = new OxyPlot.Series.LineSeries();
            TmpMeasured.Title = "Measured data";

            TmpMeasured.LineStyle = OxyPlot.LineStyle.Solid;
            TmpMeasured.StrokeThickness = 0;
            TmpMeasured.MarkerSize = 2.5;
            TmpMeasured.MarkerType = OxyPlot.MarkerType.Circle;
            TmpMeasured.Color = OxyPlot.OxyColors.Black;
            TmpMeasured.MarkerFill = OxyPlot.OxyColors.Black;
            TmpMeasured.MarkerStroke = OxyPlot.OxyColors.Black;

            OxyPlot.Series.LineSeries TmpSimulated = new OxyPlot.Series.LineSeries();
            TmpSimulated.Title = "Simulated measurement";

            TmpSimulated.LineStyle = OxyPlot.LineStyle.Dot;
            TmpSimulated.StrokeThickness = 2;
            TmpSimulated.MarkerSize = 2;
            TmpSimulated.MarkerType = OxyPlot.MarkerType.Circle;
            TmpSimulated.MarkerFill = OxyPlot.OxyColors.DarkRed;
            TmpSimulated.Color = OxyPlot.OxyColors.DarkRed;
            TmpSimulated.MarkerStroke = OxyPlot.OxyColors.DarkRed;

            double Xmin = double.MaxValue;
            double Xmax = double.MinValue;

            double Ymin = double.MaxValue;
            double Ymax = double.MinValue;

            for (int n = 0; n < UsedCountsMeasured.Count; n++)
            {
                OxyPlot.DataPoint EDP = new OxyPlot.DataPoint(UsedCountsMeasured[n][0], UsedCountsMeasured[n][1]);

                TmpMeasured.Points.Add(EDP);

                if(Xmin > UsedCountsMeasured[n][0])
                {
                    Xmin = UsedCountsMeasured[n][0];
                }
                if (Ymin > UsedCountsMeasured[n][1])
                {
                    Ymin = UsedCountsMeasured[n][1];
                }

                if (Xmax < UsedCountsMeasured[n][0])
                {
                    Xmax = UsedCountsMeasured[n][0];
                }
                if (Ymax < UsedCountsMeasured[n][1])
                {
                    Ymax = UsedCountsMeasured[n][1];
                }
            }

            for (int n = 0; n < UsedCountsSimulated.Count; n++)
            {
                OxyPlot.DataPoint PDP = new OxyPlot.DataPoint(UsedCountsSimulated[n][0], UsedCountsSimulated[n][1]);

                TmpSimulated.Points.Add(PDP);

                if (Xmin > UsedCountsSimulated[n][0])
                {
                    Xmin = UsedCountsSimulated[n][0];
                }
                if (Ymin > UsedCountsSimulated[n][1])
                {
                    Ymin = UsedCountsSimulated[n][1];
                }

                if (Xmax < UsedCountsSimulated[n][0])
                {
                    Xmax = UsedCountsSimulated[n][0];
                }
                if (Ymax < UsedCountsSimulated[n][1])
                {
                    Ymax = UsedCountsSimulated[n][1];
                }
            }

            this.YieldXAxisLin.Minimum = Xmin;
            this.YieldXAxisLin.Maximum = Xmax;
            this.YieldYAxisLin.Minimum = Ymin;
            this.YieldYAxisLin.Maximum = Ymax;

            #endregion

            this.YieldPlotModel.Series.Add(TmpMeasured);
            if (TmpSimulated.Points.Count > 0)
            {
                this.YieldPlotModel.Series.Add(TmpSimulated);
            }

            this.PW.MainPlot.ResetAllAxes();
            this.PW.MainPlot.InvalidatePlot(true);
        }

        private void SetPlotAllPsi(ReflexYield RY)
        {
            #region Data points

            this.YieldPlotModel.Series.Clear();
            List<Pattern.Counts> UsedCountsElastic = new List<Pattern.Counts>();
            List<Pattern.Counts> UsedCountsPlastic = new List<Pattern.Counts>();

            #region Axis selection
            //[0]: MacroStrain
            //[1]: MicroStrain
            //[2]: Stress
            //[3]: Plain adjusted Stress
            //[4]: Psi adjusted Stress
            if (this.xAxesm.SelectedIndex == 0)
            {
                this.YieldYAxisLin.Title = "Macro strain";
                if (this.yAxesm.SelectedIndex == 0)
                {
                    this.YieldXAxisLin.Title = "Macro strain";
                    for (int n = 0; n < RY.PeakData.Count; n++)
                    {
                        UsedCountsElastic.Add(RY.MacroStrainOverMacroStrainData(true, RY.PeakData[n][0].PsiAngle));
                        UsedCountsPlastic.Add(RY.MacroStrainOverMacroStrainData(false, RY.PeakData[n][0].PsiAngle));
                    }
                }
                else if (this.yAxesm.SelectedIndex == 1)
                {
                    this.YieldXAxisLin.Title = "Micro strain";
                    for (int n = 0; n < RY.PeakData.Count; n++)
                    {
                        UsedCountsElastic.Add(RY.MacroStrainOverMicroStrainData(true, RY.PeakData[n][0].PsiAngle));
                        UsedCountsPlastic.Add(RY.MacroStrainOverMicroStrainData(false, RY.PeakData[n][0].PsiAngle));
                    }
                }
                else if (this.yAxesm.SelectedIndex == 3)
                {
                    this.YieldXAxisLin.Title = "Load direction stress";
                    for (int n = 0; n < RY.PeakData.Count; n++)
                    {
                        UsedCountsElastic.Add(RY.MacroStrainDataOverPsiAdjustedStress(true, RY.PeakData[n][0].PsiAngle));
                        UsedCountsPlastic.Add(RY.MacroStrainDataOverPsiAdjustedStress(false, RY.PeakData[n][0].PsiAngle));
                    }
                }
                else if (this.yAxesm.SelectedIndex == 4)
                {
                    this.YieldXAxisLin.Title = "Slip plain direction stress";
                    for (int n = 0; n < RY.PeakData.Count; n++)
                    {
                        UsedCountsElastic.Add(RY.MacroStrainDataOverPlainAdjustedStress(true, RY.PeakData[n][0].PsiAngle));
                        UsedCountsPlastic.Add(RY.MacroStrainDataOverPlainAdjustedStress(false, RY.PeakData[n][0].PsiAngle));
                    }
                }
                else
                {
                    this.YieldXAxisLin.Title = "Total stress";
                    for (int n = 0; n < RY.PeakData.Count; n++)
                    {
                        UsedCountsElastic.Add(RY.MacroStrainDataOverStress(true, RY.PeakData[n][0].PsiAngle));
                        UsedCountsPlastic.Add(RY.MacroStrainDataOverStress(false, RY.PeakData[n][0].PsiAngle));
                    }
                }
            }
            else if (this.xAxesm.SelectedIndex == 1)
            {
                this.YieldYAxisLin.Title = "Micro strain";
                if (this.yAxesm.SelectedIndex == 0)
                {
                    this.YieldXAxisLin.Title = "Macro strain";
                    for (int n = 0; n < RY.PeakData.Count; n++)
                    {
                        UsedCountsElastic.Add(RY.MicroStrainOverMacroStrainData(true, RY.PeakData[n][0].PsiAngle));
                        UsedCountsPlastic.Add(RY.MicroStrainOverMacroStrainData(false, RY.PeakData[n][0].PsiAngle));
                    }
                }
                else if (this.yAxesm.SelectedIndex == 1)
                {
                    this.YieldXAxisLin.Title = "Micro strain";
                    for (int n = 0; n < RY.PeakData.Count; n++)
                    {
                        UsedCountsElastic.Add(RY.MicroStrainOverMicroStrainData(true, RY.PeakData[n][0].PsiAngle));
                        UsedCountsPlastic.Add(RY.MicroStrainOverMicroStrainData(false, RY.PeakData[n][0].PsiAngle));
                    }
                }
                else if (this.yAxesm.SelectedIndex == 3)
                {
                    this.YieldXAxisLin.Title = "Load direction stress";
                    for (int n = 0; n < RY.PeakData.Count; n++)
                    {
                        UsedCountsElastic.Add(RY.MicroStrainDataOverPsiAdjustedStress(true, RY.PeakData[n][0].PsiAngle));
                        UsedCountsPlastic.Add(RY.MicroStrainDataOverPsiAdjustedStress(false, RY.PeakData[n][0].PsiAngle));
                    }
                }
                else if (this.yAxesm.SelectedIndex == 4)
                {
                    this.YieldXAxisLin.Title = "Slip plain direction stress";
                    for (int n = 0; n < RY.PeakData.Count; n++)
                    {
                        UsedCountsElastic.Add(RY.MicroStrainDataOverPlainAdjustedStress(true, RY.PeakData[n][0].PsiAngle));
                        UsedCountsPlastic.Add(RY.MicroStrainDataOverPlainAdjustedStress(false, RY.PeakData[n][0].PsiAngle));
                    }
                }
                else
                {
                    this.YieldXAxisLin.Title = "Total stress";
                    for (int n = 0; n < RY.PeakData.Count; n++)
                    {
                        UsedCountsElastic.Add(RY.MicroStrainDataOverStress(true, RY.PeakData[n][0].PsiAngle));
                        UsedCountsPlastic.Add(RY.MicroStrainDataOverStress(false, RY.PeakData[n][0].PsiAngle));
                    }
                }
            }
            else if (this.xAxesm.SelectedIndex == 2)
            {
                this.YieldYAxisLin.Title = "Total stress";
                if (this.yAxesm.SelectedIndex == 0)
                {
                    this.YieldXAxisLin.Title = "Macro strain";
                    for (int n = 0; n < RY.PeakData.Count; n++)
                    {
                        UsedCountsElastic.Add(RY.StressOverMacroStrainData(true, RY.PeakData[n][0].PsiAngle));
                        UsedCountsPlastic.Add(RY.StressOverMacroStrainData(false, RY.PeakData[n][0].PsiAngle));
                    }
                }
                else if (this.yAxesm.SelectedIndex == 1)
                {
                    this.YieldXAxisLin.Title = "Micro strain";
                    for (int n = 0; n < RY.PeakData.Count; n++)
                    {
                        UsedCountsElastic.Add(RY.StressOverMicroStrainData(true, RY.PeakData[n][0].PsiAngle));
                        UsedCountsPlastic.Add(RY.StressOverMicroStrainData(false, RY.PeakData[n][0].PsiAngle));
                    }
                }
                else if (this.yAxesm.SelectedIndex == 2)
                {
                    this.YieldXAxisLin.Title = "Total stress";
                    for (int n = 0; n < RY.PeakData.Count; n++)
                    {
                        UsedCountsElastic.Add(RY.StressOverStressData(true, RY.PeakData[n][0].PsiAngle));
                        UsedCountsPlastic.Add(RY.StressOverStressData(false, RY.PeakData[n][0].PsiAngle));
                    }
                }
                else if (this.yAxesm.SelectedIndex == 3)
                {
                    this.YieldXAxisLin.Title = "Load direction stress";
                    for (int n = 0; n < RY.PeakData.Count; n++)
                    {
                        UsedCountsElastic.Add(RY.StressOverPsiAdjustedStressData(true, RY.PeakData[n][0].PsiAngle));
                        UsedCountsPlastic.Add(RY.StressOverPsiAdjustedStressData(false, RY.PeakData[n][0].PsiAngle));
                    }
                }
                else
                {
                    this.YieldXAxisLin.Title = "Slip plain direction stress";
                    for (int n = 0; n < RY.PeakData.Count; n++)
                    {
                        UsedCountsElastic.Add(RY.StressOverPlainAdjustedStressData(true, RY.PeakData[n][0].PsiAngle));
                        UsedCountsPlastic.Add(RY.StressOverPlainAdjustedStressData(false, RY.PeakData[n][0].PsiAngle));
                    }
                }
            }
            else if (this.xAxesm.SelectedIndex == 3)
            {
                this.YieldYAxisLin.Title = "Load direction stress";
                if (this.yAxesm.SelectedIndex == 0)
                {
                    this.YieldXAxisLin.Title = "Macro strain";
                    for (int n = 0; n < RY.PeakData.Count; n++)
                    {
                        UsedCountsElastic.Add(RY.PsiAdjustedStressOverMacroStrainData(true, RY.PeakData[n][0].PsiAngle));
                        UsedCountsPlastic.Add(RY.PsiAdjustedStressOverMacroStrainData(false, RY.PeakData[n][0].PsiAngle));
                    }
                }
                else if (this.yAxesm.SelectedIndex == 1)
                {
                    this.YieldXAxisLin.Title = "Micro strain";
                    for (int n = 0; n < RY.PeakData.Count; n++)
                    {
                        UsedCountsElastic.Add(RY.PsiAdjustedStressOverMicroStrainData(true, RY.PeakData[n][0].PsiAngle));
                        UsedCountsPlastic.Add(RY.PsiAdjustedStressOverMicroStrainData(false, RY.PeakData[n][0].PsiAngle));
                    }
                }
                else if (this.yAxesm.SelectedIndex == 2)
                {
                    this.YieldXAxisLin.Title = "Total stress";
                    for (int n = 0; n < RY.PeakData.Count; n++)
                    {
                        UsedCountsElastic.Add(RY.PsiAdjustedStressOverStressData(true, RY.PeakData[n][0].PsiAngle));
                        UsedCountsPlastic.Add(RY.PsiAdjustedStressOverStressData(false, RY.PeakData[n][0].PsiAngle));
                    }
                }
                else if (this.yAxesm.SelectedIndex == 3)
                {
                    this.YieldXAxisLin.Title = "Load direction stress";
                    for (int n = 0; n < RY.PeakData.Count; n++)
                    {
                        UsedCountsElastic.Add(RY.PsiAdjustedStressOverPsiAdjustedStressData(true, RY.PeakData[n][0].PsiAngle));
                        UsedCountsPlastic.Add(RY.PsiAdjustedStressOverPsiAdjustedStressData(false, RY.PeakData[n][0].PsiAngle));
                    }
                }
                else if (this.yAxesm.SelectedIndex == 3)
                {
                    this.YieldXAxisLin.Title = "Slip plain direction stress";
                    for (int n = 0; n < RY.PeakData.Count; n++)
                    {
                        UsedCountsElastic.Add(RY.PsiAdjustedStressOverPlainAdjustedStressData(true, RY.PeakData[n][0].PsiAngle));
                        UsedCountsPlastic.Add(RY.PsiAdjustedStressOverPlainAdjustedStressData(false, RY.PeakData[n][0].PsiAngle));
                    }
                }
            }
            else
            {
                this.YieldYAxisLin.Title = "Slip plain direction stress";
                if (this.yAxesm.SelectedIndex == 0)
                {
                    this.YieldXAxisLin.Title = "Macro strain";
                    for (int n = 0; n < RY.PeakData.Count; n++)
                    {
                        UsedCountsElastic.Add(RY.PlainAdjustedStressOverMacroStrainData(true, RY.PeakData[n][0].PsiAngle));
                        UsedCountsPlastic.Add(RY.PlainAdjustedStressOverMacroStrainData(false, RY.PeakData[n][0].PsiAngle));
                    }
                }
                else if (this.yAxesm.SelectedIndex == 1)
                {
                    this.YieldXAxisLin.Title = "Micro strain";
                    for (int n = 0; n < RY.PeakData.Count; n++)
                    {
                        UsedCountsElastic.Add(RY.PlainAdjustedStressOverMicroStrainData(true, RY.PeakData[n][0].PsiAngle));
                        UsedCountsPlastic.Add(RY.PlainAdjustedStressOverMicroStrainData(false, RY.PeakData[n][0].PsiAngle));
                    }
                }
                else if (this.yAxesm.SelectedIndex == 2)
                {
                    this.YieldXAxisLin.Title = "Total stress";
                    for (int n = 0; n < RY.PeakData.Count; n++)
                    {
                        UsedCountsElastic.Add(RY.PlainAdjustedStressOverStressData(true, RY.PeakData[n][0].PsiAngle));
                        UsedCountsPlastic.Add(RY.PlainAdjustedStressOverStressData(false, RY.PeakData[n][0].PsiAngle));
                    }
                }
                else if (this.yAxesm.SelectedIndex == 3)
                {
                    this.YieldXAxisLin.Title = "Load direction stress";
                    for (int n = 0; n < RY.PeakData.Count; n++)
                    {
                        UsedCountsElastic.Add(RY.PlainAdjustedStressOverPsiAdjustedStressData(true, RY.PeakData[n][0].PsiAngle));
                        UsedCountsPlastic.Add(RY.PlainAdjustedStressOverPsiAdjustedStressData(false, RY.PeakData[n][0].PsiAngle));
                    }
                }
                else
                {
                    this.YieldXAxisLin.Title = "Slip plain direction stress";
                    for (int n = 0; n < RY.PeakData.Count; n++)
                    {
                        UsedCountsElastic.Add(RY.PlainAdjustedStressOverPlainAdjustedStressData(true, RY.PeakData[n][0].PsiAngle));
                        UsedCountsPlastic.Add(RY.PlainAdjustedStressOverPlainAdjustedStressData(false, RY.PeakData[n][0].PsiAngle));
                    }
                }
            }

            #endregion
            
            double Xmin = double.MaxValue;
            double Xmax = double.MinValue;

            double Ymin = double.MaxValue;
            double Ymax = double.MinValue;

            int[] Blue = { 0, 255, 255, 0, 0, 255, 255 };
            int[] Red = { 255, 0, 255, 0, 255, 0, 255 };
            int[] Green = { 255, 255, 0, 255, 0, 0, 255 };

            for (int n = 0; n < UsedCountsElastic.Count; n++)
            {
                double Round = Math.Floor(n / 7.0);

                double PrefactorZ = 1.0;
                double PreFactorN = 1.0;
                if (Round < 3)
                {
                    PreFactorN = Math.Pow(2, Round);
                    PrefactorZ = 1.0;
                }
                else
                {
                    for (double i = 2; i <= 2368767; i++)
                    {
                        if (Round < Math.Pow(2, i))
                        {
                            PreFactorN = Math.Pow(2, i);
                            PrefactorZ = (2 * (Round - Math.Pow(2, i - 1))) + 1;
                            break;
                        }
                    }
                }

                double PreFactor = PrefactorZ / PreFactorN;
                int[] ActColor = { 255 - Convert.ToInt32(Blue[n % 7] * PreFactor), 255 - Convert.ToInt32(Red[n % 7] * PreFactor), 255 - Convert.ToInt32(Green[n % 7] * PreFactor) };

                OxyPlot.OxyColor LineColor = OxyPlot.OxyColor.FromRgb(Convert.ToByte(ActColor[1]), Convert.ToByte(ActColor[2]), Convert.ToByte(ActColor[0]));

                OxyPlot.Series.LineSeries TmpElastic = new OxyPlot.Series.LineSeries();
                TmpElastic.Title = "Elastic regime";

                TmpElastic.LineStyle = OxyPlot.LineStyle.Solid;
                TmpElastic.StrokeThickness = 2;
                TmpElastic.MarkerSize = 2;
                TmpElastic.MarkerType = OxyPlot.MarkerType.Circle;
                TmpElastic.Color = LineColor;
                TmpElastic.MarkerStroke = OxyPlot.OxyColors.Black;

                OxyPlot.Series.LineSeries TmpPlastic = new OxyPlot.Series.LineSeries();
                TmpPlastic.Title = "Plastic regime";

                TmpPlastic.LineStyle = OxyPlot.LineStyle.Dash;
                TmpPlastic.StrokeThickness = 2;
                TmpPlastic.MarkerSize = 2;
                TmpPlastic.MarkerType = OxyPlot.MarkerType.Cross;
                TmpPlastic.Color = LineColor;
                TmpPlastic.MarkerStroke = OxyPlot.OxyColors.DarkRed;
                
                for (int i = 0; i < UsedCountsElastic[n].Count; i++)
                {
                    OxyPlot.DataPoint EDP = new OxyPlot.DataPoint(UsedCountsElastic[n][i][0], UsedCountsElastic[n][i][1]);

                    TmpElastic.Points.Add(EDP);

                    if (Xmin > UsedCountsElastic[n][i][0])
                    {
                        Xmin = UsedCountsElastic[n][i][0];
                    }
                    if (Ymin > UsedCountsElastic[n][i][1])
                    {
                        Ymin = UsedCountsElastic[n][i][1];
                    }

                    if (Xmax < UsedCountsElastic[n][i][0])
                    {
                        Xmax = UsedCountsElastic[n][i][0];
                    }
                    if (Ymax < UsedCountsElastic[n][i][1])
                    {
                        Ymax = UsedCountsElastic[n][i][1];
                    }
                }

                for (int i = 0; i < UsedCountsPlastic[n].Count; i++)
                {
                    OxyPlot.DataPoint PDP = new OxyPlot.DataPoint(UsedCountsPlastic[n][i][0], UsedCountsPlastic[n][i][1]);

                    TmpPlastic.Points.Add(PDP);

                    if (Xmin > UsedCountsPlastic[n][i][0])
                    {
                        Xmin = UsedCountsPlastic[n][i][0];
                    }
                    if (Ymin > UsedCountsPlastic[n][i][1])
                    {
                        Ymin = UsedCountsPlastic[n][i][1];
                    }

                    if (Xmax < UsedCountsPlastic[n][i][0])
                    {
                        Xmax = UsedCountsPlastic[n][i][0];
                    }
                    if (Ymax < UsedCountsPlastic[n][i][1])
                    {
                        Ymax = UsedCountsPlastic[n][i][1];
                    }
                }

                this.YieldPlotModel.Series.Add(TmpElastic);
                this.YieldPlotModel.Series.Add(TmpPlastic);
            }
            this.YieldXAxisLin.Minimum = Xmin;
            this.YieldXAxisLin.Maximum = Xmax;
            this.YieldYAxisLin.Minimum = Ymin;
            this.YieldYAxisLin.Maximum = Ymax;

            #endregion
            

            this.PW.MainPlot.ResetAllAxes();
            this.PW.MainPlot.InvalidatePlot(true);
        }

        private void SetFractionPlot()
        {
            #region Data points

            this.YieldFractionPlotModel.Series.Clear();
            double psiAngletmp = this.ComboPsiAngles[PsiAnglef.SelectedIndex];
            List<Pattern.Counts> UsedCountsElastic = new List<Pattern.Counts>();
            List<Pattern.Counts> UsedCountsPlastic = new List<Pattern.Counts>();
            Pattern.Counts totalCounts = new Pattern.Counts();

            #region Axis selection
            
            //[0]: MacroStrain
            //[1]: MicroStrain
            //[2]: Stress
            //[3]: PsiStress
            //[4]: PlainStress
            bool index1Cor = false;
            bool index2Cor = false;
            if (this.ReflexList.SelectedItems.Count == 1)
            {
                for (int n = 0; n < this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData.Count; n++)
                {
                    if (this.xAxesf.SelectedIndex == 0)
                    {
                        this.YieldFractionYAxisLin.Title = "Macro strain";
                        if (this.yAxesf.SelectedIndex == 0)
                        {
                            this.YieldFractionXAxisLin.Title = "Macro strain";

                            UsedCountsElastic.Add(this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n].MacroStrainOverMacroStrainData(true, psiAngletmp));
                            if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            {
                                UsedCountsPlastic.Add(this.ActSample.MacroStrainOverMacroStrainData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex]));
                            }
                            //if (totalCounts.Count == 0)
                            //{
                            //    totalCounts = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.MacroStrainOverMacroStrainData(psiAngletmp);
                            //    index2Cor = true;
                            //}
                        }
                        else if (this.yAxesf.SelectedIndex == 1)
                        {
                            this.YieldFractionXAxisLin.Title = "Micro strain";
                            UsedCountsElastic.Add(this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n].MacroStrainOverMicroStrainData(true, psiAngletmp));
                            if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            {
                                UsedCountsPlastic.Add(this.ActSample.MacroStrainOverMicroStrainData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n]));
                            }
                            //if (totalCounts.Count == 0)
                            //{
                            //    totalCounts = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.MacroStrainOverMacroStrainData(psiAngletmp);
                            //    index1Cor = true;
                            //}
                        }
                        else if (this.yAxesf.SelectedIndex == 2)
                        {
                            this.YieldFractionXAxisLin.Title = "Total stress";
                            UsedCountsElastic.Add(this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n].MacroStrainDataOverStress(true, psiAngletmp));
                            if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            {
                                UsedCountsPlastic.Add(this.ActSample.MacroStrainDataOverStressRD(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex]));
                            }
                        }
                        else if (this.yAxesf.SelectedIndex == 3)
                        {
                            this.YieldFractionXAxisLin.Title = "Load direction stress";
                            UsedCountsElastic.Add(this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n].MacroStrainDataOverPsiAdjustedStress(true, psiAngletmp));
                            //if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            //{
                            //    UsedCountsPlastic.Add(this.ActSample.MacroStrainDataOverPsiAdjustedStress(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n]));
                            //}
                        }
                        else
                        {
                            this.YieldFractionXAxisLin.Title = "Slip plane direction stress";
                            UsedCountsElastic.Add(this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n].MacroStrainDataOverPlainAdjustedStress(true, psiAngletmp));
                            if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            {
                                UsedCountsPlastic.Add(this.ActSample.MacroStrainDataOverPlainAdjustedStress(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n]));
                            }
                        }
                    }
                    else if (this.xAxesf.SelectedIndex == 1)
                    {
                        this.YieldFractionYAxisLin.Title = "Micro strain";
                        if (this.yAxesf.SelectedIndex == 0)
                        {
                            this.YieldFractionXAxisLin.Title = "Macro strain";
                            UsedCountsElastic.Add(this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n].MicroStrainOverMacroStrainData(true, psiAngletmp));
                            if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            {
                                UsedCountsPlastic.Add(this.ActSample.MicroStrainOverMacroStrainData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n]));
                            }
                            index2Cor = true;
                        }
                        else if (this.yAxesf.SelectedIndex == 1)
                        {
                            this.YieldFractionXAxisLin.Title = "Micro strain";
                            UsedCountsElastic.Add(this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n].MicroStrainOverMicroStrainData(true, psiAngletmp));
                            if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            {
                                UsedCountsPlastic.Add(this.ActSample.MacroStrainOverMicroStrainData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n]));
                            }
                            //if (totalCounts.Count == 0)
                            //{
                            //    totalCounts = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.MacroStrainOverMacroStrainData(psiAngletmp);
                            //    index1Cor = true;
                            //    index2Cor = true;
                            //}
                        }
                        else if (this.yAxesf.SelectedIndex == 2)
                        {
                            this.YieldFractionXAxisLin.Title = "Total stress";
                            UsedCountsElastic.Add(this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n].MicroStrainDataOverStress(true, psiAngletmp));
                            if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            {
                                UsedCountsPlastic.Add(this.ActSample.MicroStrainDataOverStressRD(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n]));
                            }
                            //if (totalCounts.Count == 0)
                            //{
                            //    totalCounts = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.MacroStrainDataOverStress(psiAngletmp);
                            //    index2Cor = true;
                            //}
                        }
                        else if (this.yAxesf.SelectedIndex == 3)
                        {
                            this.YieldFractionXAxisLin.Title = "Load direction stress";
                            UsedCountsElastic.Add(this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n].MicroStrainDataOverPsiAdjustedStress(true, psiAngletmp));
                            //if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            //{
                            //    UsedCountsPlastic.Add(this.ActSample.MicroStrainDataOverPsiAdjustedStress(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n]));
                            //}
                            //if (totalCounts.Count == 0)
                            //{
                            //    totalCounts = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.MacroStrainDataOverPsiAdjustedStress(psiAngletmp);
                            //    index2Cor = true;
                            //}
                        }
                        else
                        {
                            this.YieldFractionXAxisLin.Title = "Slip plane direction stress";
                            UsedCountsElastic.Add(this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n].MicroStrainDataOverPlainAdjustedStress(true, psiAngletmp));
                            if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            {
                                UsedCountsPlastic.Add(this.ActSample.MicroStrainDataOverPlainAdjustedStress(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n]));
                            }
                        }
                    }
                    else if (this.xAxesf.SelectedIndex == 2)
                    {
                        this.YieldFractionYAxisLin.Title = "Total stress";
                        if (this.yAxesf.SelectedIndex == 0)
                        {
                            this.YieldFractionXAxisLin.Title = "Macro strain";
                            UsedCountsElastic.Add(this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n].StressOverMacroStrainData(true, psiAngletmp));
                            if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            {
                                UsedCountsPlastic.Add(this.ActSample.StressRDOverMacroStrainData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex]));
                            }
                        }
                        else if (this.yAxesf.SelectedIndex == 1)
                        {
                            this.YieldFractionXAxisLin.Title = "Micro strain";
                            UsedCountsElastic.Add(this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n].StressOverMicroStrainData(true, psiAngletmp));
                            if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            {
                                UsedCountsPlastic.Add(this.ActSample.StressRDOverMicroStrainData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n]));
                            }
                        }
                        else if (this.yAxesf.SelectedIndex == 2)
                        {
                            this.YieldFractionXAxisLin.Title = "Total stress";
                            UsedCountsElastic.Add(this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n].StressOverStressData(true, psiAngletmp));
                            if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            {
                                UsedCountsPlastic.Add(this.ActSample.StressRDOverStressData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n]));
                            }
                        }
                        else if (this.yAxesf.SelectedIndex == 3)
                        {
                            this.YieldFractionXAxisLin.Title = "Load direction stress";
                            UsedCountsElastic.Add(this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n].StressOverPsiAdjustedStressData(true, psiAngletmp));
                            //if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            //{
                            //    UsedCountsPlastic.Add(this.ActSample.MacroStrainOverMicroStrainData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n]));
                            //}
                        }
                    }
                    else if (this.xAxesf.SelectedIndex == 3)
                    {
                        this.YieldFractionYAxisLin.Title = "Load direction stress";
                        if (this.yAxesf.SelectedIndex == 0)
                        {
                            this.YieldFractionXAxisLin.Title = "Macro strain";
                            UsedCountsElastic.Add(this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n].PsiAdjustedStressOverMacroStrainData(true, psiAngletmp));
                            //if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            //{
                            //    UsedCountsPlastic.Add(this.ActSample.MacroStrainOverMicroStrainData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n]));
                            //}
                        }
                        else if (this.yAxesf.SelectedIndex == 1)
                        {
                            this.YieldFractionXAxisLin.Title = "Micro strain";
                            UsedCountsElastic.Add(this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n].PsiAdjustedStressOverMicroStrainData(true, psiAngletmp));
                            //if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            //{
                            //    UsedCountsPlastic.Add(this.ActSample.MacroStrainOverMicroStrainData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n]));
                            //}
                        }
                        else if (this.yAxesf.SelectedIndex == 2)
                        {
                            this.YieldFractionXAxisLin.Title = "Total stress";
                            UsedCountsElastic.Add(this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n].PsiAdjustedStressOverStressData(true, psiAngletmp));
                            //if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            //{
                            //    UsedCountsPlastic.Add(this.ActSample.MacroStrainOverMicroStrainData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n]));
                            //}
                        }
                        else if (this.yAxesf.SelectedIndex == 3)
                        {
                            this.YieldFractionXAxisLin.Title = "Load direction stress";
                            UsedCountsElastic.Add(this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n].PsiAdjustedStressOverPsiAdjustedStressData(true, psiAngletmp));
                            //if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            //{
                            //    UsedCountsPlastic.Add(this.ActSample.MacroStrainOverMicroStrainData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n]));
                            //}
                        }
                        else
                        {
                            this.YieldFractionXAxisLin.Title = "Slip plain direction stress";
                            UsedCountsElastic.Add(this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n].PsiAdjustedStressOverPlainAdjustedStressData(true, psiAngletmp));
                            //if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            //{
                            //    UsedCountsPlastic.Add(this.ActSample.PsiAdjustedStressOverPlainAdjustedStressData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n]));
                            //}
                        }
                    }
                    else
                    {
                        this.YieldFractionYAxisLin.Title = "Slip plain direction stress";
                        if (this.yAxesf.SelectedIndex == 0)
                        {
                            this.YieldFractionXAxisLin.Title = "Macro stress";
                            UsedCountsElastic.Add(this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n].PlainAdjustedStressOverMacroStrainData(true, psiAngletmp));
                            if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            {
                                UsedCountsPlastic.Add(this.ActSample.PlainAdjustedStressOverMacroStrainData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n]));
                            }
                        }
                        else if (this.yAxesf.SelectedIndex == 1)
                        {
                            this.YieldFractionXAxisLin.Title = "Micro stress";
                            UsedCountsElastic.Add(this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n].PlainAdjustedStressOverMicroStrainData(true, psiAngletmp));
                            if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            {
                                UsedCountsPlastic.Add(this.ActSample.PlainAdjustedStressOverMicroStrainData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n]));
                            }
                        }
                        else if (this.yAxesf.SelectedIndex == 2)
                        {
                            this.YieldFractionXAxisLin.Title = "Total stress";
                            UsedCountsElastic.Add(this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n].PlainAdjustedStressOverMicroStrainData(true, psiAngletmp));
                            if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            {
                                UsedCountsPlastic.Add(this.ActSample.PlainAdjustedStressOverMicroStrainData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n]));
                            }
                        }
                        else if (this.yAxesf.SelectedIndex == 3)
                        {
                            this.YieldFractionXAxisLin.Title = "Load direction stress";
                            UsedCountsElastic.Add(this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n].PlainAdjustedStressOverMicroStrainData(true, psiAngletmp));
                            if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            {
                                UsedCountsPlastic.Add(this.ActSample.PlainAdjustedStressOverMicroStrainData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n]));
                            }
                        }
                        else if (this.yAxesf.SelectedIndex == 3)
                        {
                            this.YieldFractionXAxisLin.Title = "Load direction stress";
                            UsedCountsElastic.Add(this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n].PlainAdjustedStressOverMicroStrainData(true, psiAngletmp));
                            if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            {
                                UsedCountsPlastic.Add(this.ActSample.PlainAdjustedStressOverMicroStrainData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n]));
                            }
                        }
                    }
                }
            }
            else
            {
                List<ReflexYield> SelectedReflexes = new List<ReflexYield>();
                for(int n = 0; n < ReflexList.SelectedItems.Count; n++)
                {
                    ReflexYield selectedRY = (ReflexYield)this.ReflexList.SelectedItems[n];

                    if (this.xAxesf.SelectedIndex == 0)
                    {
                        this.YieldFractionYAxisLin.Title = "Macro strain";
                        if (this.yAxesf.SelectedIndex == 0)
                        {
                            this.YieldFractionXAxisLin.Title = "Macro strain";

                            UsedCountsElastic.Add(selectedRY.MacroStrainOverMacroStrainData(true, psiAngletmp));
                            if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            {
                                UsedCountsPlastic.Add(this.ActSample.MacroStrainOverMacroStrainData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex]));
                            }
                        }
                        else if (this.yAxesf.SelectedIndex == 1)
                        {
                            this.YieldFractionXAxisLin.Title = "Micro strain";
                            UsedCountsElastic.Add(selectedRY.MacroStrainOverMicroStrainData(true, psiAngletmp));
                            if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            {
                                UsedCountsPlastic.Add(this.ActSample.MacroStrainOverMicroStrainData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n]));
                            }
                        }
                        else if (this.yAxesf.SelectedIndex == 2)
                        {
                            this.YieldFractionXAxisLin.Title = "Total stress";
                            UsedCountsElastic.Add(selectedRY.MacroStrainDataOverStress(true, psiAngletmp));
                            if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            {
                                UsedCountsPlastic.Add(this.ActSample.MacroStrainDataOverStressRD(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex]));
                            }
                        }
                        else if (this.yAxesf.SelectedIndex == 3)
                        {
                            this.YieldFractionXAxisLin.Title = "Load direction stress";
                            UsedCountsElastic.Add(selectedRY.MacroStrainDataOverPsiAdjustedStress(true, psiAngletmp));
                            //if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            //{
                            //    UsedCountsPlastic.Add(this.ActSample.MacroStrainDataOverPsiAdjustedStress(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n]));
                            //}
                        }
                        else
                        {
                            this.YieldFractionXAxisLin.Title = "Slip plane direction stress";
                            UsedCountsElastic.Add(selectedRY.MacroStrainDataOverPlainAdjustedStress(true, psiAngletmp));
                            if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            {
                                UsedCountsPlastic.Add(this.ActSample.MacroStrainDataOverPlainAdjustedStress(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n]));
                            }
                        }
                    }
                    else if (this.xAxesf.SelectedIndex == 1)
                    {
                        this.YieldFractionYAxisLin.Title = "Micro strain";
                        if (this.yAxesf.SelectedIndex == 0)
                        {
                            this.YieldFractionXAxisLin.Title = "Macro strain";
                            UsedCountsElastic.Add(selectedRY.MicroStrainOverMacroStrainData(true, psiAngletmp));
                            if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            {
                                UsedCountsPlastic.Add(this.ActSample.MicroStrainOverMacroStrainData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n]));
                            }
                        }
                        else if (this.yAxesf.SelectedIndex == 1)
                        {
                            this.YieldFractionXAxisLin.Title = "Micro strain";
                            UsedCountsElastic.Add(selectedRY.MicroStrainOverMicroStrainData(true, psiAngletmp));
                            if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            {
                                UsedCountsPlastic.Add(this.ActSample.MicroStrainOverMicroStrainData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n]));
                            }
                        }
                        else if (this.yAxesf.SelectedIndex == 2)
                        {
                            this.YieldFractionXAxisLin.Title = "Total stress";
                            UsedCountsElastic.Add(selectedRY.MicroStrainDataOverStress(true, psiAngletmp));
                            if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            {
                                UsedCountsPlastic.Add(this.ActSample.MicroStrainDataOverStressRD(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n]));
                            }
                        }
                        else if (this.yAxesf.SelectedIndex == 3)
                        {
                            this.YieldFractionXAxisLin.Title = "Load direction stress";
                            UsedCountsElastic.Add(selectedRY.MicroStrainDataOverPsiAdjustedStress(true, psiAngletmp));
                            //if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            //{
                            //    UsedCountsPlastic.Add(this.ActSample.MicroStrainDataOverPsiAdjustedStress(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n]));
                            //}
                        }
                        else
                        {
                            this.YieldFractionXAxisLin.Title = "Slip plane direction stress";
                            UsedCountsElastic.Add(selectedRY.MicroStrainDataOverPlainAdjustedStress(true, psiAngletmp));
                            if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            {
                                UsedCountsPlastic.Add(this.ActSample.MicroStrainDataOverPlainAdjustedStress(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n]));
                            }
                        }
                    }
                    else if (this.xAxesf.SelectedIndex == 2)
                    {
                        this.YieldFractionYAxisLin.Title = "Total stress";
                        if (this.yAxesf.SelectedIndex == 0)
                        {
                            this.YieldFractionXAxisLin.Title = "Macro strain";
                            UsedCountsElastic.Add(selectedRY.StressOverMacroStrainData(true, psiAngletmp));
                            if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            {
                                UsedCountsPlastic.Add(this.ActSample.StressRDOverMacroStrainData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex]));
                            }
                        }
                        else if (this.yAxesf.SelectedIndex == 1)
                        {
                            this.YieldFractionXAxisLin.Title = "Micro strain";
                            UsedCountsElastic.Add(selectedRY.StressOverMicroStrainData(true, psiAngletmp));
                            if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            {
                                UsedCountsPlastic.Add(this.ActSample.StressRDOverMicroStrainData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n]));
                            }
                        }
                        else if (this.yAxesf.SelectedIndex == 2)
                        {
                            this.YieldFractionXAxisLin.Title = "Total stress";
                            UsedCountsElastic.Add(selectedRY.StressOverStressData(true, psiAngletmp));
                            if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            {
                                UsedCountsPlastic.Add(this.ActSample.StressRDOverStressData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n]));
                            }
                        }
                        else if (this.yAxesf.SelectedIndex == 3)
                        {
                            this.YieldFractionXAxisLin.Title = "Load direction stress";
                            UsedCountsElastic.Add(selectedRY.StressOverPsiAdjustedStressData(true, psiAngletmp));
                            //if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            //{
                            //    UsedCountsPlastic.Add(this.ActSample.StressRDOverPsiAdjustedStressData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n]));
                            //}
                        }
                    }
                    else if (this.xAxesf.SelectedIndex == 3)
                    {
                        this.YieldFractionYAxisLin.Title = "Load direction stress";
                        if (this.yAxesf.SelectedIndex == 0)
                        {
                            this.YieldFractionXAxisLin.Title = "Macro strain";
                            UsedCountsElastic.Add(selectedRY.PsiAdjustedStressOverMacroStrainData(true, psiAngletmp));
                            //if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            //{
                            //    UsedCountsPlastic.Add(this.ActSample.MacroStrainOverMicroStrainData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n]));
                            //}
                        }
                        else if (this.yAxesf.SelectedIndex == 1)
                        {
                            this.YieldFractionXAxisLin.Title = "Micro strain";
                            UsedCountsElastic.Add(selectedRY.PsiAdjustedStressOverMicroStrainData(true, psiAngletmp));
                            //if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            //{
                            //    UsedCountsPlastic.Add(this.ActSample.MacroStrainOverMicroStrainData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n]));
                            //}
                        }
                        else if (this.yAxesf.SelectedIndex == 2)
                        {
                            this.YieldFractionXAxisLin.Title = "Total stress";
                            UsedCountsElastic.Add(selectedRY.PsiAdjustedStressOverStressData(true, psiAngletmp));
                            //if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            //{
                            //    UsedCountsPlastic.Add(this.ActSample.MacroStrainOverMicroStrainData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n]));
                            //}
                        }
                        else if (this.yAxesf.SelectedIndex == 3)
                        {
                            this.YieldFractionXAxisLin.Title = "Load direction stress";
                            UsedCountsElastic.Add(selectedRY.PsiAdjustedStressOverPsiAdjustedStressData(true, psiAngletmp));
                            //if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            //{
                            //    UsedCountsPlastic.Add(this.ActSample.MacroStrainOverMicroStrainData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n]));
                            //}
                        }
                        else
                        {
                            this.YieldFractionXAxisLin.Title = "Slip plain direction stress";
                            UsedCountsElastic.Add(selectedRY.PsiAdjustedStressOverPlainAdjustedStressData(true, psiAngletmp));
                            //if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            //{
                            //    UsedCountsPlastic.Add(this.ActSample.MacroStrainOverMicroStrainData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n]));
                            //}
                        }
                    }
                    else
                    {
                        this.YieldFractionYAxisLin.Title = "Slip plain direction stress";
                        if (this.yAxesf.SelectedIndex == 0)
                        {
                            this.YieldFractionXAxisLin.Title = "Macro stress";
                            UsedCountsElastic.Add(selectedRY.PlainAdjustedStressOverMacroStrainData(true, psiAngletmp));
                            if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            {
                                UsedCountsPlastic.Add(this.ActSample.PlainAdjustedStressOverMacroStrainData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n]));
                            }
                        }
                        else if (this.yAxesf.SelectedIndex == 1)
                        {
                            this.YieldFractionXAxisLin.Title = "Micro stress";
                            UsedCountsElastic.Add(selectedRY.PlainAdjustedStressOverMicroStrainData(true, psiAngletmp));
                            if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            {
                                UsedCountsPlastic.Add(this.ActSample.PlainAdjustedStressOverMicroStrainData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n]));
                            }
                        }
                        else if (this.yAxesf.SelectedIndex == 2)
                        {
                            this.YieldFractionXAxisLin.Title = "Total stress";
                            UsedCountsElastic.Add(selectedRY.PlainAdjustedStressOverStressData(true, psiAngletmp));
                            if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            {
                                UsedCountsPlastic.Add(this.ActSample.PlainAdjustedStressOverStressRDData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n]));
                            }
                        }
                        else if (this.yAxesf.SelectedIndex == 3)
                        {
                            this.YieldFractionXAxisLin.Title = "Load direction stress";
                            UsedCountsElastic.Add(selectedRY.PlainAdjustedStressOverPsiAdjustedStressData(true, psiAngletmp));
                            //if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            //{
                            //    UsedCountsPlastic.Add(this.ActSample.PlainAdjustedStressOverMicroStrainData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n]));
                            //}
                        }
                        else if (this.yAxesf.SelectedIndex == 3)
                        {
                            this.YieldFractionXAxisLin.Title = "Load direction stress";
                            UsedCountsElastic.Add(selectedRY.PlainAdjustedStressOverMicroStrainData(true, psiAngletmp));
                            //if (Convert.ToBoolean(this.PlotSimulatedData.IsChecked))
                            //{
                            //    UsedCountsPlastic.Add(this.ActSample.PlainAdjustedStressOverMicroStrainData(this.ActSample.ReussTensorData[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n]));
                            //}
                        }
                    }
                }
            }

            #endregion

            int[] Blue = { 0, 255, 255, 0, 0, 255, 255 };
            int[] Red = { 255, 0, 255, 0, 255, 0, 255 };
            int[] Green = { 255, 255, 0, 255, 0, 0, 255 };

            double Xmin = double.MaxValue;
            double Xmax = double.MinValue;

            double Ymin = double.MaxValue;
            double Ymax = double.MinValue;

            List<OxyPlot.Series.LineSeries> TmpElasticA = new List<OxyPlot.Series.LineSeries>();
            List<OxyPlot.Series.LineSeries> TmpPlasticA = new List<OxyPlot.Series.LineSeries>();

            for (int n = 0; n < UsedCountsElastic.Count; n++)
            {
                OxyPlot.Series.LineSeries TmpElastic = new OxyPlot.Series.LineSeries();
                TmpElastic.Title = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n].SlipPlane.HKLString + " measured data";

                TmpElastic.LineStyle = OxyPlot.LineStyle.None;
                TmpElastic.StrokeThickness = 2;
                TmpElastic.MarkerSize = 2;
                TmpElastic.MarkerType = OxyPlot.MarkerType.Circle;

                double Round = Math.Floor(n / 7.0);

                double PrefactorZ = 1.0;
                double PreFactorN = 1.0;
                if (Round < 3)
                {
                    PreFactorN = Math.Pow(2, Round);
                    PrefactorZ = 1.0;
                }
                else
                {
                    for (double i = 2; i <= 2368767; i++)
                    {
                        if (Round < Math.Pow(2, i))
                        {
                            PreFactorN = Math.Pow(2, i);
                            PrefactorZ = (2 * (Round - Math.Pow(2, i - 1))) + 1;
                            break;
                        }
                    }
                }

                double PreFactor = PrefactorZ / PreFactorN;
                int[] ActColor = { 255 - Convert.ToInt32(Blue[n % 7] * PreFactor), 255 - Convert.ToInt32(Red[n % 7] * PreFactor), 255 - Convert.ToInt32(Green[n % 7] * PreFactor) };

                OxyPlot.OxyColor LineColor = OxyPlot.OxyColor.FromRgb(Convert.ToByte(ActColor[1]), Convert.ToByte(ActColor[2]), Convert.ToByte(ActColor[0]));

                TmpElastic.Color = LineColor;
                TmpElastic.MarkerStroke = LineColor;

                OxyPlot.Series.LineSeries TmpPlastic = new OxyPlot.Series.LineSeries();
                TmpPlastic.Title = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData[n].SlipPlane.HKLString + " simulated data";

                TmpPlastic.LineStyle = OxyPlot.LineStyle.Dot;
                TmpPlastic.StrokeThickness = 2;
                TmpPlastic.MarkerSize = 0;
                TmpPlastic.MarkerType = OxyPlot.MarkerType.Circle;

                TmpPlastic.Color = LineColor;
                TmpPlastic.MarkerStroke = LineColor;

                for (int i = 0; i < UsedCountsElastic[n].Count; i++)
                {
                    double dPXValue = UsedCountsElastic[n][i][0];
                    double dPYValue = UsedCountsElastic[n][i][1];
                    //if (totalCounts.Count != 0 && i < totalCounts.Count)
                    //{
                    //    if (index1Cor == true)
                    //    {
                    //        dPXValue /= totalCounts[i][0];
                    //    }

                    //    if (index2Cor == true)
                    //    {
                    //        dPYValue /= totalCounts[i][1];
                    //    }
                    //}

                    OxyPlot.DataPoint EDP = new OxyPlot.DataPoint(dPXValue, dPYValue);

                    TmpElastic.Points.Add(EDP);

                    if (Xmin > UsedCountsElastic[n][i][0])
                    {
                        Xmin = UsedCountsElastic[n][i][0];
                    }
                    if (Ymin > UsedCountsElastic[n][i][1])
                    {
                        Ymin = UsedCountsElastic[n][i][1];
                    }

                    if (Xmax < UsedCountsElastic[n][i][0])
                    {
                        Xmax = UsedCountsElastic[n][i][0];
                    }
                    if (Ymax < UsedCountsElastic[n][i][1])
                    {
                        Ymax = UsedCountsElastic[n][i][1];
                    }
                }

                for (int i = 0; i < UsedCountsPlastic[n].Count; i++)
                {
                    OxyPlot.DataPoint PDP = new OxyPlot.DataPoint(UsedCountsPlastic[n][i][0], UsedCountsPlastic[n][i][1]);

                    TmpPlastic.Points.Add(PDP);

                    if (Xmin > UsedCountsPlastic[n][i][0])
                    {
                        Xmin = UsedCountsPlastic[n][i][0];
                    }
                    if (Ymin > UsedCountsPlastic[n][i][1])
                    {
                        Ymin = UsedCountsPlastic[n][i][1];
                    }

                    if (Xmax < UsedCountsPlastic[n][i][0])
                    {
                        Xmax = UsedCountsPlastic[n][i][0];
                    }
                    if (Ymax < UsedCountsPlastic[n][i][1])
                    {
                        Ymax = UsedCountsPlastic[n][i][1];
                    }
                }

                this.YieldFractionPlotModel.Series.Add(TmpElastic);
                this.YieldFractionPlotModel.Series.Add(TmpPlastic);
            }

            this.YieldFractionXAxisLin.Minimum = Xmin;
            this.YieldFractionXAxisLin.Maximum = Xmax;
            this.YieldFractionYAxisLin.Minimum = Ymin;
            this.YieldFractionYAxisLin.Maximum = Ymax;

            #endregion
            
            //this.FractionPlot.ResetAllAxes();
            //this.FractionPlot.InvalidatePlot(true);
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
            this.SetYieldSurface();

            //ReflexList.ItemsSource = this.ActSample.YieldSurfaceData[0].ReflexYieldData;

            for(int n = 0; n < this.ActSample.PlasticTensor.Count; n++)
            {
                for(int i = 0; i < this.ActSample.PlasticTensor[n].YieldSurfaceData.ReflexYieldData.Count; i++)
                {
                    if (this.ActSample.CrystalData[n].SymmetryGroupID == 194)
                    {
                        this.ActSample.PlasticTensor[n].YieldSurfaceData.ReflexYieldData[i].SetSlipDirectionAngles(this.ActSample, 1);
                    }
                    else
                    {
                        this.ActSample.PlasticTensor[n].YieldSurfaceData.ReflexYieldData[i].SetSlipDirectionAngles(this.ActSample, 0);
                    }
                }
            }

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

        private void SetYieldSurface()
        {
            for (int n = 0; n < this.ActSample.CrystalData.Count; n++)
            {
                //YieldSurface ySTmp = new YieldSurface(this.ActSample.CrystalData[n]);

                ////List<ReflexYield> rYieldList = new List<ReflexYield>();
                //this.ActSample.ReussTensorData[n].SetPeakStressAssociation(this.ActSample);
                List<List<List<Stress.Macroskopic.PeakStressAssociation>>> allPAPhase = this.ActSample.ReussTensorData[n].SetStrainDataReflexYield();

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
            if(PsiAnglem.SelectedIndex != -1 && this.IndexEventAktive)
            {
                if (this.ReflexList.SelectedIndex != -1)
                {
                    ReflexYield SelectedRY = (ReflexYield)this.ReflexList.SelectedItem;

                    for (int n = 0; n < SelectedRY.PeakData.Count; n++)
                    {
                        if (Math.Abs(Convert.ToDouble(this.PsiAnglem.Text) - SelectedRY.PeakData[n][0].PsiAngle) < CalScec.Properties.Settings.Default.PsyAcceptanceAngle)
                        {
                            this.PeakList.ItemsSource = SelectedRY.PeakData[n];
                        }
                    }

                    if (Convert.ToBoolean(PlotAllPsim.IsChecked))
                    {
                        this.SetPlotAllPsi(SelectedRY);
                    }
                    else
                    {
                        this.SetPlot(SelectedRY);
                    }
                    //this.SetFractionPlot();
                }
            }
        }

        private void PsiAnglef_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PsiAnglef.SelectedIndex != -1 && this.IndexEventAktive)
            {
                this.SetFractionPlot();
            }
        }

        private void xAxesm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (xAxesm.SelectedIndex != -1 && this.IndexEventAktive)
            {
                if (this.ReflexList.SelectedIndex != -1)
                {
                    ReflexYield SelectedRY = (ReflexYield)this.ReflexList.SelectedItem;
                    if (Convert.ToBoolean(PlotAllPsim.IsChecked))
                    {
                        this.SetPlotAllPsi(SelectedRY);
                    }
                    else
                    {
                        this.SetPlot(SelectedRY);
                    }
                }
            }
        }

        private void yAxesm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (yAxesm.SelectedIndex != -1 && this.IndexEventAktive)
            {
                if (this.ReflexList.SelectedIndex != -1)
                {
                    ReflexYield SelectedRY = (ReflexYield)this.ReflexList.SelectedItem;
                    if (Convert.ToBoolean(PlotAllPsim.IsChecked))
                    {
                        this.SetPlotAllPsi(SelectedRY);
                    }
                    else
                    {
                        this.SetPlot(SelectedRY);
                    }
                }
            }
        }

        private void yAxesf_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (yAxesf.SelectedIndex != -1 && this.IndexEventAktive)
            {
                this.SetFractionPlot();
            }
        }

        private void xAxesf_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (yAxesf.SelectedIndex != -1 && this.IndexEventAktive)
            {
                this.SetFractionPlot();
            }
        }

        private void ReflexList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if(this.ReflexList.SelectedIndex != -1)
                {
                    ReflexYield SelectedRY = (ReflexYield)this.ReflexList.SelectedItem;

                    if(this.PhaseSwitchBox.SelectedIndex != -1)
                    {
                        this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData.Remove(SelectedRY);

                        this.ReflexList.ItemsSource = this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].YieldSurfaceData.ReflexYieldData;
                        if (PsiAnglef.SelectedIndex != -1)
                        {
                            this.SetFractionPlot();
                        }
                    }
                }
            }
        }

        private void PlotAllPsim_Checked(object sender, RoutedEventArgs e)
        {
            if (this.ReflexList.SelectedIndex != -1)
            {
                ReflexYield SelectedRY = (ReflexYield)this.ReflexList.SelectedItem;
                this.SetPlotAllPsi(SelectedRY);
            }
        }

        private void PlotAllPsim_Unchecked(object sender, RoutedEventArgs e)
        {
            if (this.ReflexList.SelectedIndex != -1)
            {
                ReflexYield SelectedRY = (ReflexYield)this.ReflexList.SelectedItem;
                this.SetPlot(SelectedRY);
            }
        }

        private void ReflexYieldSlipDirectionSecondary_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.ReflexList.SelectedIndex != -1 && IndexEventAktive)
            {
                ReflexYield SelectedRY = (ReflexYield)this.ReflexList.SelectedItem;

                try
                {
                    double newValue = Convert.ToDouble(ReflexYieldSlipDirection.Text);

                    SelectedRY.YieldMainStrength = newValue;
                }
                catch
                {

                }
            }
        }

        private void ReflexYieldSlipDirection_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.PotenitalSlipSystemsList.SelectedIndex != -1 && IndexEventAktive)
            {
                ReflexYield SelectedRY = (ReflexYield)this.PotenitalSlipSystemsList.SelectedItem;

                try
                {
                    double newValue = Convert.ToDouble(ReflexYieldSlipDirection.Text);

                    SelectedRY.YieldMainStrength = newValue;
                }
                catch
                {

                }
            }
        }

        private void ShowTestData_Click(object sender, RoutedEventArgs e)
        {
            if (this.ReflexList.SelectedIndex != -1)
            {
                ReflexYield SelectedRY = (ReflexYield)this.ReflexList.SelectedItem;

                #region Data points

                //this.YieldPlotModel.Series.Clear();
                int phase = this.PhaseSwitchBox.SelectedIndex;
                Pattern.Counts UsedCountsElastic = SelectedRY.CalculatedCrystalStrainElastic(this.ActSample.HillTensorData[phase]);
                switch(this.CalculationModel.SelectedIndex)
                {
                    case 0:
                        UsedCountsElastic = SelectedRY.CalculatedCrystalStrainElastic(this.ActSample.ReussTensorData[phase]);
                        break;
                    case 1:
                        UsedCountsElastic = SelectedRY.CalculatedCrystalStrainElastic(this.ActSample.HillTensorData[phase]);
                        break;
                    case 2:
                        UsedCountsElastic = SelectedRY.CalculatedCrystalStrainElastic(this.ActSample.KroenerTensorData[phase]);
                        break;
                    case 3:
                        UsedCountsElastic = SelectedRY.CalculatedCrystalStrainElastic(this.ActSample.DeWittTensorData[phase]);
                        break;
                    case 4:
                        UsedCountsElastic = SelectedRY.CalculatedCrystalStrainElastic(this.ActSample.GeometricHillTensorData[phase]);
                        break;
                    default:
                        UsedCountsElastic = SelectedRY.CalculatedCrystalStrainElastic(this.ActSample.HillTensorData[phase]);
                        break;
                }
                this.YieldYAxisLin.Title = "Calculated Strain";
                this.YieldXAxisLin.Title = "Stress";
                

                OxyPlot.Series.LineSeries TmpElastic = new OxyPlot.Series.LineSeries();
                TmpElastic.Title = "Elastic regime";

                TmpElastic.LineStyle = OxyPlot.LineStyle.Dash;
                TmpElastic.StrokeThickness = 2;
                TmpElastic.MarkerSize = 2;
                TmpElastic.MarkerType = OxyPlot.MarkerType.Circle;
                TmpElastic.Color = OxyPlot.OxyColors.Black;
                TmpElastic.MarkerStroke = OxyPlot.OxyColors.Black;

                double Xmin = double.MaxValue;
                double Xmax = double.MinValue;

                double Ymin = double.MaxValue;
                double Ymax = double.MinValue;

                for (int n = 0; n < UsedCountsElastic.Count; n++)
                {
                    OxyPlot.DataPoint EDP = new OxyPlot.DataPoint(UsedCountsElastic[n][0], UsedCountsElastic[n][1]);

                    TmpElastic.Points.Add(EDP);

                    if (Xmin > UsedCountsElastic[n][0])
                    {
                        Xmin = UsedCountsElastic[n][0];
                    }
                    if (Ymin > UsedCountsElastic[n][1])
                    {
                        Ymin = UsedCountsElastic[n][1];
                    }

                    if (Xmax < UsedCountsElastic[n][0])
                    {
                        Xmax = UsedCountsElastic[n][0];
                    }
                    if (Ymax < UsedCountsElastic[n][1])
                    {
                        Ymax = UsedCountsElastic[n][1];
                    }
                }

                this.YieldXAxisLin.Minimum = Xmin;
                this.YieldXAxisLin.Maximum = Xmax;
                this.YieldYAxisLin.Minimum = Ymin;
                this.YieldYAxisLin.Maximum = Ymax;

                #endregion

                this.YieldPlotModel.Series.Add(TmpElastic);

                this.PW.MainPlot.ResetAllAxes();
                this.PW.MainPlot.InvalidatePlot(true);
            }
        }

        private void PlotPlasticData_Click(object sender, RoutedEventArgs e)
        {
            if (this.ReflexList.SelectedIndex != -1)
            {
                ReflexYield SelectedRY = (ReflexYield)this.ReflexList.SelectedItem;

                #region Data points

                //this.YieldPlotModel.Series.Clear();
                int phase = this.PhaseSwitchBox.SelectedIndex;

                Pattern.Counts UsedCountsElastic = SelectedRY.CalculatedCrystalStrainElastic(this.ActSample.HillTensorData[phase]);

                double psiAngletmp = this.ComboPsiAngles[PsiAnglem.SelectedIndex];
                double sampleYield = Convert.ToDouble(SampleYieldStrength.Text);
                switch (this.CalculationModel.SelectedIndex)
                {
                    case 0:
                        UsedCountsElastic = ActSample.GetMacroStrainLatticeStrainCurveLD(1, phase, sampleYield, SelectedRY.GetPeakData(psiAngletmp));
                        break;
                    case 1:
                        UsedCountsElastic = ActSample.GetMacroStrainLatticeStrainCurveLD(2, phase, sampleYield, SelectedRY.GetPeakData(psiAngletmp));
                        break;
                    case 2:
                        UsedCountsElastic = ActSample.GetMacroStressStrainCurveLD(3, phase, sampleYield, SelectedRY.GetPeakData(psiAngletmp));
                        break;
                    case 3:
                        UsedCountsElastic = ActSample.GetMacroStressStrainCurveLD(4, phase, sampleYield, SelectedRY.GetPeakData(psiAngletmp));
                        break;
                    case 4:
                        UsedCountsElastic = ActSample.GetMacroStressStrainCurveLD(5, phase, sampleYield, SelectedRY.GetPeakData(psiAngletmp));
                        break;
                    default:
                        UsedCountsElastic = ActSample.GetMacroStressStrainCurveLD(1, phase, sampleYield, SelectedRY.GetPeakData(psiAngletmp));
                        break;
                }
                this.YieldYAxisLin.Title = "Calculated Strain";
                this.YieldXAxisLin.Title = "Stress";


                OxyPlot.Series.LineSeries TmpElastic = new OxyPlot.Series.LineSeries();
                TmpElastic.Title = "Calculated data";

                TmpElastic.LineStyle = OxyPlot.LineStyle.Dash;
                TmpElastic.StrokeThickness = 2;
                TmpElastic.MarkerSize = 2;
                TmpElastic.MarkerType = OxyPlot.MarkerType.Circle;
                TmpElastic.Color = OxyPlot.OxyColors.DarkRed;
                TmpElastic.MarkerStroke = OxyPlot.OxyColors.DarkRed;

                double Xmin = double.MaxValue;
                double Xmax = double.MinValue;

                double Ymin = double.MaxValue;
                double Ymax = double.MinValue;

                for (int n = 0; n < UsedCountsElastic.Count; n++)
                {
                    OxyPlot.DataPoint EDP = new OxyPlot.DataPoint(UsedCountsElastic[n][0], UsedCountsElastic[n][1]);

                    TmpElastic.Points.Add(EDP);

                    if (Xmin > UsedCountsElastic[n][0])
                    {
                        Xmin = UsedCountsElastic[n][0];
                    }
                    if (Ymin > UsedCountsElastic[n][1])
                    {
                        Ymin = UsedCountsElastic[n][1];
                    }

                    if (Xmax < UsedCountsElastic[n][0])
                    {
                        Xmax = UsedCountsElastic[n][0];
                    }
                    if (Ymax < UsedCountsElastic[n][1])
                    {
                        Ymax = UsedCountsElastic[n][1];
                    }
                }

                this.YieldXAxisLin.Minimum = Xmin;
                this.YieldXAxisLin.Maximum = Xmax;
                this.YieldYAxisLin.Minimum = Ymin;
                this.YieldYAxisLin.Maximum = Ymax;

                #endregion

                this.YieldPlotModel.Series.Add(TmpElastic);

                this.PW.MainPlot.ResetAllAxes();
                this.PW.MainPlot.InvalidatePlot(true);
            }
        }

        private void HardenningTensorData_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (this.PhaseSwitchBox.SelectedIndex != -1 && IndexEventAktive)
            {
                this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex]._hardenningTensor[0, 0] = hMV[0].Value1;
                this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex]._hardenningTensor[0, 1] = hMV[0].Value2;
                this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex]._hardenningTensor[0, 2] = hMV[0].Value3;

                this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex]._hardenningTensor[1, 0] = hMV[1].Value1;
                this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex]._hardenningTensor[1, 1] = hMV[1].Value2;
                this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex]._hardenningTensor[1, 2] = hMV[1].Value3;

                this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex]._hardenningTensor[2, 0] = hMV[2].Value1;
                this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex]._hardenningTensor[2, 1] = hMV[2].Value2;
                this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex]._hardenningTensor[2, 2] = hMV[2].Value3;
            }
        }

        private void StressTensorData_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (this.ReflexList.SelectedIndex != -1 && IndexEventAktive)
            {
                ReflexYield SelectedRY = (ReflexYield)this.ReflexList.SelectedItem;
            }
        }

        private void SampleYieldStrength_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.PhaseSwitchBox.SelectedIndex != -1 && IndexEventAktive)
            {
                try
                {
                    double newYield = Convert.ToDouble(SampleYieldStrength.Text);
                    this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].PhaseYieldStrength = newYield;
                }
                catch
                {

                }
            }
        }

        private void SampleStrainRate_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.PhaseSwitchBox.SelectedIndex != -1 && IndexEventAktive)
            {
                try
                {
                    double newYield = Convert.ToDouble(SampleStrainRate.Text);
                    this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].PhaseStrainRate = newYield;
                }
                catch
                {

                }
            }
        }
        private void SampleHardenningRate_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.PhaseSwitchBox.SelectedIndex != -1 && IndexEventAktive)
            {
                try
                {
                    double newYield = Convert.ToDouble(SampleHardenningRate.Text);
                    this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].PhaseHardeningRate = newYield;
                    this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex]._hardenningTensor[0, 0] = newYield;
                    this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex]._hardenningTensor[1, 1] = newYield;
                    this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex]._hardenningTensor[2, 2] = newYield;
                }
                catch
                {

                }
            }
        }
        

        private void Plastic11_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                double newVal = Convert.ToDouble(Plastic11.Text);
                switch (SelectedStiffnessTensor.SelectedIndex)
                {
                    case 0:
                        this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].EffectiveS11 = newVal;
                        break;
                    case 1:
                        this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].ConstraintS11 = newVal;
                        break;
                    case 2:
                        this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].GrainS11 = newVal;
                        break;
                    default:
                        break;
                }
            }
            catch
            {

            }
        }

        private void Plastic12_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                double newVal = Convert.ToDouble(Plastic11.Text);
                switch (SelectedStiffnessTensor.SelectedIndex)
                {
                    case 0:
                        this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].EffectiveS12 = newVal;
                        break;
                    case 1:
                        this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].ConstraintS12 = newVal;
                        break;
                    case 2:
                        this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].GrainS12 = newVal;
                        break;
                    default:
                        break;
                }
            }
            catch
            {

            }
        }

        private void Plastic44_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                double newVal = Convert.ToDouble(Plastic11.Text);
                switch (SelectedStiffnessTensor.SelectedIndex)
                {
                    case 0:
                        this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].EffectiveS44 = newVal;
                        break;
                    case 1:
                        this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].ConstraintS44 = newVal;
                        break;
                    case 2:
                        this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].GrainS44 = newVal;
                        break;
                    default:
                        break;
                }
            }
            catch
            {

            }
        }

        private void Plastic13_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                double newVal = Convert.ToDouble(Plastic11.Text);
                switch (SelectedStiffnessTensor.SelectedIndex)
                {
                    case 0:
                        this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].EffectiveS13 = newVal;
                        break;
                    case 1:
                        this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].ConstraintS13 = newVal;
                        break;
                    case 2:
                        this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].GrainS13 = newVal;
                        break;
                    default:
                        break;
                }
            }
            catch
            {

            }
        }

        private void Plastic33_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                double newVal = Convert.ToDouble(Plastic11.Text);
                switch (SelectedStiffnessTensor.SelectedIndex)
                {
                    case 0:
                        this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].EffectiveS33 = newVal;
                        break;
                    case 1:
                        this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].ConstraintS33 = newVal;
                        break;
                    case 2:
                        this.ActSample.PlasticTensor[this.PhaseSwitchBox.SelectedIndex].GrainS33 = newVal;
                        break;
                    default:
                        break;
                }
            }
            catch
            {

            }
        }

        #region Simulation Data

        private void DriveToStress_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double ticks = Convert.ToInt32(this.ExperimentalTicks.Text);

                MathNet.Numerics.LinearAlgebra.Matrix<double> target = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

                target[0, 0] = Convert.ToDouble(ExperimentalValue11.Text);
                target[0, 1] = Convert.ToDouble(ExperimentalValue12.Text);
                target[0, 2] = Convert.ToDouble(ExperimentalValue13.Text);
                target[1, 0] = Convert.ToDouble(ExperimentalValue12.Text);
                target[2, 0] = Convert.ToDouble(ExperimentalValue13.Text);
                target[1, 1] = Convert.ToDouble(ExperimentalValue22.Text);
                target[1, 2] = Convert.ToDouble(ExperimentalValue23.Text);
                target[2, 1] = Convert.ToDouble(ExperimentalValue23.Text);
                target[2, 2] = Convert.ToDouble(ExperimentalValue33.Text);

                MathNet.Numerics.LinearAlgebra.Matrix<double> step = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

                MathNet.Numerics.LinearAlgebra.Matrix<double> last = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
                switch (ExperimentalDataSelection.SelectedIndex)
                {
                    case 0:
                        if (this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StressCFHistory[this.PhaseSwitchBox.SelectedIndex].Count != 0)
                        {
                            last = this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StressCFHistory[this.PhaseSwitchBox.SelectedIndex][this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StressCFHistory[this.PhaseSwitchBox.SelectedIndex].Count - 1];
                            break;
                        }
                        goto default;
                    case 1:
                        if (this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StrainCFHistory[this.PhaseSwitchBox.SelectedIndex].Count != 0)
                        {
                            last = this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StrainCFHistory[this.PhaseSwitchBox.SelectedIndex][this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StrainCFHistory[this.PhaseSwitchBox.SelectedIndex].Count - 1];
                            break;
                        }
                        goto default;
                    case 2:
                        if (this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StressRateCFHistory[this.PhaseSwitchBox.SelectedIndex].Count != 0)
                        {
                            last = this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StressRateCFHistory[this.PhaseSwitchBox.SelectedIndex][this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StressRateCFHistory[this.PhaseSwitchBox.SelectedIndex].Count - 1];
                            break;
                        }
                        goto default;
                    case 3:
                        if (this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StrainRateCFHistory[this.PhaseSwitchBox.SelectedIndex].Count != 0)
                        {
                            last = this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StrainRateCFHistory[this.PhaseSwitchBox.SelectedIndex][this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StrainRateCFHistory[this.PhaseSwitchBox.SelectedIndex].Count - 1];
                            break;
                        }
                        goto default;
                    case 4:
                        if (this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].HardeningCFHistory[this.PhaseSwitchBox.SelectedIndex].Count != 0)
                        {
                            last = this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].HardeningCFHistory[this.PhaseSwitchBox.SelectedIndex][this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StrainRateCFHistory[this.PhaseSwitchBox.SelectedIndex].Count - 1];
                            break;
                        }
                        goto default;
                    case 5:
                        if (this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StressSFHistory.Count != 0)
                        {
                            last = this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StressSFHistory[this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StressSFHistory.Count - 1];
                            break;
                        }
                        goto default;
                    case 6:
                        if (this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StrainSFHistory.Count != 0)
                        {
                            last = this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StrainSFHistory[this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StrainSFHistory.Count - 1];
                            break;
                        }
                        goto default;
                    case 7:
                        if (this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StressRateSFHistory.Count != 0)
                        {
                            last = this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StressRateSFHistory[this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StressRateSFHistory.Count - 1];
                            break;
                        }
                        goto default;
                    case 8:
                        if (this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StrainRateSFHistory.Count != 0)
                        {
                            last = this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StrainRateSFHistory[this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StrainRateSFHistory.Count - 1];
                            break;
                        }
                        goto default;
                    case 9:
                        if (this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].HardeningSFHistory.Count != 0)
                        {
                            last = this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].HardeningSFHistory[this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StrainRateSFHistory.Count - 1];
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
                            this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StressCFHistory[this.PhaseSwitchBox.SelectedIndex].Add(hist);
                            goto default;
                        case 1:
                            this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StrainCFHistory[this.PhaseSwitchBox.SelectedIndex].Add(hist);
                            goto default;
                        case 2:
                            this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StressRateCFHistory[this.PhaseSwitchBox.SelectedIndex].Add(hist);
                            goto default;
                        case 3:
                            this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StrainRateCFHistory[this.PhaseSwitchBox.SelectedIndex].Add(hist);
                            goto default;
                        case 4:
                            this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].HardeningCFHistory[this.PhaseSwitchBox.SelectedIndex].Add(hist);
                            goto default;
                        case 5:
                            this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StressSFHistory.Add(hist);
                            goto default;
                        case 6:
                            this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StrainSFHistory.Add(hist);
                            goto default;
                        case 7:
                            this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StressRateSFHistory.Add(hist);
                            goto default;
                        case 8:
                            this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].StrainRateSFHistory.Add(hist);
                            goto default;
                        case 9:
                            this.ActSample.SimulationData[ExperimentSelection.SelectedIndex].HardeningSFHistory.Add(hist);
                            goto default;
                        default:
                            break;
                    }
                }

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

        private void SimulateExperiment_Click(object sender, RoutedEventArgs e)
        {
            if (this.ExperimentSelection.SelectedIndex != -1 && PhaseSwitchBox.SelectedIndex != -1)
            {
                switch (CalculationModel.SelectedIndex)
                {
                    case 0:
                        EPModeling.PerformStandardExperimentGrain(this.ActSample.ReussTensorData[PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[ExperimentSelection.SelectedIndex], PhaseSwitchBox.SelectedIndex);
                        break;
                    case 1:
                        EPModeling.PerformStandardExperimentGrain(this.ActSample.HillTensorData[PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[PhaseSwitchBox.SelectedIndex], PhaseSwitchBox.SelectedIndex);
                        break;
                    case 2:
                        EPModeling.PerformStandardExperimentGrain(this.ActSample.KroenerTensorData[PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[PhaseSwitchBox.SelectedIndex], PhaseSwitchBox.SelectedIndex);
                        break;
                    case 3:
                        EPModeling.PerformStandardExperimentGrain(this.ActSample.DeWittTensorData[PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[PhaseSwitchBox.SelectedIndex], PhaseSwitchBox.SelectedIndex);
                        break;
                    case 4:
                        EPModeling.PerformStandardExperimentGrain(this.ActSample.GeometricHillTensorData[PhaseSwitchBox.SelectedIndex], this.ActSample.PlasticTensor[PhaseSwitchBox.SelectedIndex], this.ActSample.SimulationData[PhaseSwitchBox.SelectedIndex], PhaseSwitchBox.SelectedIndex);
                        break;
                    default:
                        break;
                }
                
            }
            this.SetExpData();
        }

        private void ShowExperiment_Click(object sender, RoutedEventArgs e)
        {
            Pattern.Counts usedCountsSimulated = new Pattern.Counts();

            #region Axis selection

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


            OxyPlot.Series.LineSeries TmpSimulated = new OxyPlot.Series.LineSeries();
            TmpSimulated.Title = "Simulation: ";

            //[0]: "Sample strain";
            //[1]: "Grain strain";
            //[2]: "Sample Stress";
            //[3]: "Grain Stress";
            //[4]: "Sample Shear Stress";
            //[5]: "Grain Shear Stress";
            //[6]: "Sample strain Rate";
            //[7]: "Grain strain Rate";
            if (this.ExperimentSelection.SelectedIndex != -1 && this.PhaseSwitchBox.SelectedIndex != -1)
            {
                if (this.xAxesm.SelectedIndex == 0)
                {
                    this.YieldYAxisLin.Title = "Sample strain";
                    TmpSimulated.Title += mainDir.HKLString;
                    if (this.yAxesm.SelectedIndex == 0)
                    {
                        this.YieldXAxisLin.Title = "Sample strain";
                        usedCountsSimulated = this.ActSample.SampleStrainOverSampleStrainData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 1)
                    {
                        this.YieldXAxisLin.Title = "Grain strain";
                        usedCountsSimulated = this.ActSample.SampleStrainOverGrainStrainData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 2)
                    {
                        this.YieldXAxisLin.Title = "Sample stress";
                        usedCountsSimulated = this.ActSample.SampleStrainOverSampleStressData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 3)
                    {
                        this.YieldXAxisLin.Title = "Grain stress";
                        usedCountsSimulated = this.ActSample.SampleStrainOverGrainStrainData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 4)
                    {
                        this.YieldXAxisLin.Title = "Sample shear stress";
                        usedCountsSimulated = this.ActSample.SampleStrainOverSampleShearStresData(mainDir, this.ExperimentSelection.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 5)
                    {
                        this.YieldXAxisLin.Title = "Grain shear stress";
                        usedCountsSimulated = this.ActSample.SampleStrainOverGrainShearStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 6)
                    {
                        this.YieldXAxisLin.Title = "Sample strain rate";
                        usedCountsSimulated = this.ActSample.SampleStrainOverSampleStrainRateData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex);
                    }
                    else
                    {
                        this.YieldXAxisLin.Title = "Grain strain rate";
                        usedCountsSimulated = this.ActSample.SampleStrainOverGrainStrainRateData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                }
                else if (this.xAxesm.SelectedIndex == 1)
                {
                    this.YieldYAxisLin.Title = "Grain strain";
                    if (this.yAxesm.SelectedIndex == 0)
                    {
                        this.YieldXAxisLin.Title = "Sample strain";
                        usedCountsSimulated = this.ActSample.GrainStrainOverSampleStrainData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 1)
                    {
                        this.YieldXAxisLin.Title = "Grain strain";
                        usedCountsSimulated = this.ActSample.GrainStrainOverGrainStrainData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 2)
                    {
                        this.YieldXAxisLin.Title = "Sample stress";
                        usedCountsSimulated = this.ActSample.GrainStrainOverSampleStressData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 3)
                    {
                        this.YieldXAxisLin.Title = "Grain stress";
                        usedCountsSimulated = this.ActSample.GrainStrainOverGrainStressData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 4)
                    {
                        this.YieldXAxisLin.Title = "Sample shear stress";
                        usedCountsSimulated = this.ActSample.GrainStrainOverSampleShearStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 5)
                    {
                        this.YieldXAxisLin.Title = "Grain shear stress";
                        usedCountsSimulated = this.ActSample.GrainStrainOverGrainShearStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 6)
                    {
                        this.YieldXAxisLin.Title = "Sample strain rate";
                        usedCountsSimulated = this.ActSample.GrainStrainOverSampleStrainRateData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                    else
                    {
                        this.YieldXAxisLin.Title = "Grain strain rate";
                        usedCountsSimulated = this.ActSample.GrainStrainOverGrainStrainRateData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                }
                else if (this.xAxesm.SelectedIndex == 2)
                {
                    this.YieldYAxisLin.Title = "Sample stress";
                    if (this.yAxesm.SelectedIndex == 0)
                    {
                        this.YieldXAxisLin.Title = "Sample strain";
                        usedCountsSimulated = this.ActSample.SampleStressOverSampleStrainData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 1)
                    {
                        this.YieldXAxisLin.Title = "Grain strain";
                        usedCountsSimulated = this.ActSample.SampleStressOverGrainStrainData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 2)
                    {
                        this.YieldXAxisLin.Title = "Sample stress";
                        usedCountsSimulated = this.ActSample.SampleStressOverSampleStressData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 3)
                    {
                        this.YieldXAxisLin.Title = "Grain stress";
                        usedCountsSimulated = this.ActSample.SampleStressOverGrainStressData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 4)
                    {
                        this.YieldXAxisLin.Title = "Sample shear stress";
                        usedCountsSimulated = this.ActSample.SampleStressOverSampleShearStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 5)
                    {
                        this.YieldXAxisLin.Title = "Grain shear stress";
                        usedCountsSimulated = this.ActSample.SampleStressOverGrainShearStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 6)
                    {
                        this.YieldXAxisLin.Title = "Sample strain rate";
                        usedCountsSimulated = this.ActSample.SampleStressOverSampleStrainRateData(mainDir, this.ExperimentSelection.SelectedIndex);
                    }
                    else
                    {
                        this.YieldXAxisLin.Title = "Grain strain rate";
                        usedCountsSimulated = this.ActSample.SampleStressOverGrainStrainRateData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                }
                else if (this.xAxesm.SelectedIndex == 3)
                {
                    this.YieldYAxisLin.Title = "Grain stress";
                    if (this.yAxesm.SelectedIndex == 0)
                    {
                        this.YieldXAxisLin.Title = "Sample strain";
                        usedCountsSimulated = this.ActSample.GrainStressOverSampleStrainData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 1)
                    {
                        this.YieldXAxisLin.Title = "Grain strain";
                        usedCountsSimulated = this.ActSample.GrainStressOverGrainStrainData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 2)
                    {
                        this.YieldXAxisLin.Title = "Sample stress";
                        usedCountsSimulated = this.ActSample.GrainStressOverSampleStressData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 3)
                    {
                        this.YieldXAxisLin.Title = "Grain stress";
                        usedCountsSimulated = this.ActSample.GrainStressOverGrainStressData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 4)
                    {
                        this.YieldXAxisLin.Title = "Sample shear stress";
                        usedCountsSimulated = this.ActSample.GrainStressOverSampleShearStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 5)
                    {
                        this.YieldXAxisLin.Title = "Grain shear stress";
                        usedCountsSimulated = this.ActSample.GrainStressOverGrainShearStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 6)
                    {
                        this.YieldXAxisLin.Title = "Sample strain rate";
                        usedCountsSimulated = this.ActSample.GrainStressOverSampleStrainRateData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                    else
                    {
                        this.YieldXAxisLin.Title = "Grain strain rate";
                        usedCountsSimulated = this.ActSample.GrainStressOverGrainStrainRateData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                }
                else if (this.xAxesm.SelectedIndex == 4)
                {
                    this.YieldYAxisLin.Title = "Sample Shear Stress";
                    TmpSimulated.Title += mainDir.HKLString;
                    if (this.yAxesm.SelectedIndex == 0)
                    {
                        this.YieldXAxisLin.Title = "Sample strain";
                        usedCountsSimulated = this.ActSample.SampleShearStressOverSampleStrainData(mainDir, this.ExperimentSelection.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 1)
                    {
                        this.YieldXAxisLin.Title = "Grain strain";
                        usedCountsSimulated = this.ActSample.SampleShearStressOverGrainStrainData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 2)
                    {
                        this.YieldXAxisLin.Title = "Sample stress";
                        usedCountsSimulated = this.ActSample.SampleShearStressOverSampleStressData(mainDir, this.ExperimentSelection.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 3)
                    {
                        this.YieldXAxisLin.Title = "Grain stress";
                        usedCountsSimulated = this.ActSample.SampleShearStressOverGrainStrainData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 4)
                    {
                        this.YieldXAxisLin.Title = "Sample shear stress";
                        usedCountsSimulated = this.ActSample.SampleShearStressOverSampleShearStressData(this.ExperimentSelection.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 5)
                    {
                        this.YieldXAxisLin.Title = "Grain shear stress";
                        usedCountsSimulated = this.ActSample.SampleShearStressOverGrainShearStressData(this.ExperimentSelection.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 6)
                    {
                        this.YieldXAxisLin.Title = "Sample strain rate";
                        usedCountsSimulated = this.ActSample.SampleShearStressOverSampleStrainRateData(mainDir, this.ExperimentSelection.SelectedIndex);
                    }
                    else
                    {
                        this.YieldXAxisLin.Title = "Grain strain rate";
                        usedCountsSimulated = this.ActSample.SampleShearStressOverGrainStrainRateData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                }
                else if (this.xAxesm.SelectedIndex == 5)
                {
                    this.YieldYAxisLin.Title = "Grain Shear Stress";
                    TmpSimulated.Title += mainDir.HKLString;
                    if (this.yAxesm.SelectedIndex == 0)
                    {
                        this.YieldXAxisLin.Title = "Sample strain";
                        usedCountsSimulated = this.ActSample.GrainShearStressOverSampleStrainData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 1)
                    {
                        this.YieldXAxisLin.Title = "Grain strain";
                        usedCountsSimulated = this.ActSample.GrainShearStressOverGrainStrainData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 2)
                    {
                        this.YieldXAxisLin.Title = "Sample stress";
                        usedCountsSimulated = this.ActSample.GrainShearStressOverSampleStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 3)
                    {
                        this.YieldXAxisLin.Title = "Grain stress";
                        usedCountsSimulated = this.ActSample.GrainShearStressOverGrainStrainData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 4)
                    {
                        this.YieldXAxisLin.Title = "Sample shear stress";
                        usedCountsSimulated = this.ActSample.GrainShearStressOverSampleShearStressData(this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 5)
                    {
                        this.YieldXAxisLin.Title = "Grain shear stress";
                        usedCountsSimulated = this.ActSample.GrainShearStressOverGrainShearStressData(this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 6)
                    {
                        this.YieldXAxisLin.Title = "Sample strain rate";
                        usedCountsSimulated = this.ActSample.GrainShearStressOverSampleStrainRateData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                    else
                    {
                        this.YieldXAxisLin.Title = "Grain strain rate";
                        usedCountsSimulated = this.ActSample.GrainStrainRateOverGrainStrainRateData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                }
                else if (this.xAxesm.SelectedIndex == 6)
                {
                    this.YieldYAxisLin.Title = "Sample Strain rate";
                    TmpSimulated.Title += mainDir.HKLString;
                    if (this.yAxesm.SelectedIndex == 0)
                    {
                        this.YieldXAxisLin.Title = "Sample strain";
                        usedCountsSimulated = this.ActSample.SampleStrainRateOverSampleStrainData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 1)
                    {
                        this.YieldXAxisLin.Title = "Grain strain";
                        usedCountsSimulated = this.ActSample.SampleStrainRateOverGrainStrainData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 2)
                    {
                        this.YieldXAxisLin.Title = "Sample stress";
                        usedCountsSimulated = this.ActSample.SampleStrainRateOverSampleStressData(mainDir, this.ExperimentSelection.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 3)
                    {
                        this.YieldXAxisLin.Title = "Grain stress";
                        usedCountsSimulated = this.ActSample.SampleStrainRateOverGrainStrainData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 4)
                    {
                        this.YieldXAxisLin.Title = "Sample shear stress";
                        usedCountsSimulated = this.ActSample.SampleStrainRateOverSampleShearStressData(mainDir, this.ExperimentSelection.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 5)
                    {
                        this.YieldXAxisLin.Title = "Grain shear stress";
                        usedCountsSimulated = this.ActSample.SampleStrainRateOverGrainShearStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 6)
                    {
                        this.YieldXAxisLin.Title = "Sample strain rate";
                        usedCountsSimulated = this.ActSample.SampleStrainRateOverSampleStrainRateData(mainDir, mainDir, this.ExperimentSelection.SelectedIndex);
                    }
                    else
                    {
                        this.YieldXAxisLin.Title = "Grain strain rate";
                        usedCountsSimulated = this.ActSample.SampleStrainRateOverGrainStrainRateData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                }
                else if (this.xAxesm.SelectedIndex == 7)
                {
                    this.YieldYAxisLin.Title = "Grain Strain rate";
                    TmpSimulated.Title += mainDir.HKLString;
                    if (this.yAxesm.SelectedIndex == 0)
                    {
                        this.YieldXAxisLin.Title = "Sample strain";
                        usedCountsSimulated = this.ActSample.GrainStrainRateOverSampleStrainData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 1)
                    {
                        this.YieldXAxisLin.Title = "Grain strain";
                        usedCountsSimulated = this.ActSample.GrainStrainRateOverGrainStrainData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 2)
                    {
                        this.YieldXAxisLin.Title = "Sample stress";
                        usedCountsSimulated = this.ActSample.GrainStrainRateOverSampleStressData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 3)
                    {
                        this.YieldXAxisLin.Title = "Grain stress";
                        usedCountsSimulated = this.ActSample.GrainStrainRateOverGrainStrainData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 4)
                    {
                        this.YieldXAxisLin.Title = "Sample shear stress";
                        usedCountsSimulated = this.ActSample.GrainStrainRateOverSampleShearStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 5)
                    {
                        this.YieldXAxisLin.Title = "Grain shear stress";
                        usedCountsSimulated = this.ActSample.GrainStrainRateOverGrainShearStressData(mainDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                    else if (this.yAxesm.SelectedIndex == 6)
                    {
                        this.YieldXAxisLin.Title = "Sample strain rate";
                        usedCountsSimulated = this.ActSample.GrainStrainRateOverSampleStrainRateData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                    else
                    {
                        this.YieldXAxisLin.Title = "Grain strain rate";
                        usedCountsSimulated = this.ActSample.GrainStrainRateOverGrainStrainRateData(mainDir, secondDir, this.ExperimentSelection.SelectedIndex, this.PhaseSwitchBox.SelectedIndex);
                    }
                }
            }

            #endregion
            
            TmpSimulated.LineStyle = OxyPlot.LineStyle.Dot;
            TmpSimulated.StrokeThickness = 2;
            TmpSimulated.MarkerSize = 1;
            TmpSimulated.MarkerType = OxyPlot.MarkerType.Circle;
            if(this.YieldPlotModel.Series.Count == 0)
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
            else if (this.YieldPlotModel.Series.Count == 1)
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
                OxyPlot.DataPoint PDP = new OxyPlot.DataPoint(usedCountsSimulated[n][0], usedCountsSimulated[n][1]);

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

            this.YieldXAxisLin.Minimum = Xmin;
            this.YieldXAxisLin.Maximum = Xmax;
            this.YieldYAxisLin.Minimum = Ymin;
            this.YieldYAxisLin.Maximum = Ymax;
            
            this.YieldPlotModel.Series.Add(TmpSimulated);

            this.PW.MainPlot.ResetAllAxes();
            this.PW.MainPlot.InvalidatePlot(true);
        }

        private void SampleFrameSimulation_Checked(object sender, RoutedEventArgs e)
        {
            this.SetExpData();
        }

        private void SampleFrameSimulation_Unchecked(object sender, RoutedEventArgs e)
        {
            this.SetExpData();
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
            SetExpData();
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
            if (this.ExperimentSelection.SelectedIndex != -1 && PhaseSwitchBox.SelectedIndex != -1)
            {
                List<MathNet.Numerics.LinearAlgebra.Matrix<double>> viewData = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
                switch (ExperimentalDataSelection.SelectedIndex)
                {
                    case 0:
                        viewData = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressCFHistory[PhaseSwitchBox.SelectedIndex];
                        break;
                    case 1:
                        viewData = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainCFHistory[PhaseSwitchBox.SelectedIndex];
                        break;
                    case 2:
                        viewData = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateCFHistory[PhaseSwitchBox.SelectedIndex];
                        break;
                    case 3:
                        viewData = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateCFHistory[PhaseSwitchBox.SelectedIndex];
                        break;
                    case 4:
                        viewData = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].HardeningCFHistory[PhaseSwitchBox.SelectedIndex];
                        break;
                    case 5:
                        viewData = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressSFHistory;
                        break;
                    case 6:
                        viewData = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainSFHistory;
                        break;
                    case 7:
                        viewData = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StressRateSFHistory;
                        break;
                    case 8:
                        viewData = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].StrainRateSFHistory;
                        break;
                    case 9:
                        viewData = this.ActSample.SimulationData[this.ExperimentSelection.SelectedIndex].HardeningSFHistory;
                        break;
                    default:
                        break;
                }

                for (int n = 0; n < viewData.Count; n++)
                {
                    MatrixView tmp = new MatrixView();
                    
                    tmp.Value1 = viewData[n][0, 0];
                    tmp.Value2 = viewData[n][1, 1];
                    tmp.Value3 = viewData[n][2, 2];
                    tmp.Value4 = viewData[n][0, 1];
                    tmp.Value5 = viewData[n][0, 2];
                    tmp.Value6 = viewData[n][1, 2];

                    source.Add(tmp);
                }

                this.StressTensorData.ItemsSource = source;
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
                ComboBoxItem cBI = new ComboBoxItem();
                cBI.Content = "Experiment " + n.ToString();

                this.ExperimentSelection.Items.Add(cBI);
            }
        }

        #endregion
    }

    public struct MatrixView
    {
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
}
