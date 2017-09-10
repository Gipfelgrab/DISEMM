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

namespace CalScec.Analysis.Stress.Macroskopic
{
    /// <summary>
    /// Interaction logic for ElasticityWindow.xaml
    /// </summary>
    public partial class ElasticityWindow : Window
    {

        public Sample ActSample;
        bool TextEventsActive = true;

        public OxyPlot.PlotModel MainPlotModel = new OxyPlot.PlotModel();
        OxyPlot.Axes.LinearAxis MainXAxisLin = new OxyPlot.Axes.LinearAxis();
        OxyPlot.Axes.LinearAxis MainYAxisLin = new OxyPlot.Axes.LinearAxis();

        public OxyPlot.PlotModel ResPlotModel = new OxyPlot.PlotModel();
        OxyPlot.Axes.LinearAxis ResXAxisLin = new OxyPlot.Axes.LinearAxis();
        OxyPlot.Axes.LinearAxis ResYAxisLin = new OxyPlot.Axes.LinearAxis();

        public ElasticityWindow(Sample actSample)
        {
            InitializeComponent();

            this.ActSample = actSample;

            this.LoadData();
            this.PreparePlot();
        }

        private void LoadData()
        {
            ComboBoxItem Empty = new ComboBoxItem();
            Empty.Content = "";

            this.DiffractionPatternBox.Items.Add(Empty);

            for(int n = 0; n < this.ActSample.DiffractionPatterns.Count; n++)
            {
                ComboBoxItem PatternItem = new ComboBoxItem();
                PatternItem.Content = this.ActSample.DiffractionPatterns[n].Name;

                this.DiffractionPatternBox.Items.Add(PatternItem);
            }

            this.ElasticCalculationList.ItemsSource = this.ActSample.MacroElasticData;

            RefreshSampleElasticData();
        }

        private void RefreshSampleElasticData()
        {
            List<Tools.BulkElasticPhaseEvaluations> NewSource = new List<Tools.BulkElasticPhaseEvaluations>();
            NewSource.AddRange(this.ActSample.AveragedEModulStandard());
            NewSource.AddRange(this.ActSample.AveragedPossionNumberStandard());
            BulkElasticDataList.ItemsSource = NewSource;
        }

        private void PreparePlot()
        {
            MainPlotModel.LegendBorder = OxyPlot.OxyColors.Black;
            MainPlotModel.LegendItemAlignment = OxyPlot.HorizontalAlignment.Left;
            MainPlotModel.LegendTitle = "Elastic data";
            
            MainXAxisLin.Position = OxyPlot.Axes.AxisPosition.Bottom;
            MainXAxisLin.Minimum = 0;
            MainXAxisLin.Maximum = 180;
            MainXAxisLin.Title = "Strain";

            MainYAxisLin.Position = OxyPlot.Axes.AxisPosition.Left;
            MainYAxisLin.Minimum = 0;
            MainYAxisLin.Maximum = 10;
            MainYAxisLin.Title = "Applied stress";

            ResXAxisLin.Position = OxyPlot.Axes.AxisPosition.Bottom;
            ResXAxisLin.Minimum = 0;
            ResXAxisLin.Maximum = 180;
            ResXAxisLin.Title = "Strain";

            ResYAxisLin.Position = OxyPlot.Axes.AxisPosition.Left;
            ResYAxisLin.Minimum = -3.1;
            ResYAxisLin.Maximum = 3.1;
            ResYAxisLin.Title = "TotalDeviation";

            MainXAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dot;
            MainYAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dot;
            ResXAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dot;
            ResYAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dot;

            MainXAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dot;
            MainYAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dot;
            ResXAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dot;
            ResYAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dot;
            
            MainPlotModel.Axes.Add(MainXAxisLin);
            MainPlotModel.Axes.Add(MainYAxisLin);
            ResPlotModel.Axes.Add(ResXAxisLin);
            ResPlotModel.Axes.Add(ResYAxisLin);
            
            this.MainPlot.Model = MainPlotModel;
            this.MainPlot.Model.ResetAllAxes();
            this.MainPlot.Model.InvalidatePlot(true);

            this.ResPlot.Model = ResPlotModel;
            this.ResPlot.Model.ResetAllAxes();
            this.ResPlot.Model.InvalidatePlot(true);
        }

        private void DiffractionPatternBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.TextEventsActive = false;
            if(this.DiffractionPatternBox.SelectedIndex != -1)
            {
                ComboBoxItem ActItem = (ComboBoxItem)this.DiffractionPatternBox.SelectedItem;
                string PatternName = Convert.ToString(ActItem.Content);

                if (PatternName != "")
                {
                    for(int n = 0; n < this.ActSample.DiffractionPatterns.Count; n++)
                    {
                        if(PatternName == this.ActSample.DiffractionPatterns[n].Name)
                        {
                            OmegaAngleBox.Text = this.ActSample.DiffractionPatterns[n].OmegaAngle.ToString("F3");
                            ChiAngleBox.Text = this.ActSample.DiffractionPatterns[n].ChiAngle.ToString("F3");
                            AppliedStressBox.Text = this.ActSample.DiffractionPatterns[n].Stress.ToString("F3");

                            DiffractionPeakList.ItemsSource = this.ActSample.DiffractionPatterns[n].FoundPeaks;

                            break;
                        }
                    }
                }
                else
                {
                    DiffractionPeakList.ItemsSource = null;

                    OmegaAngleBox.Text = "";
                    ChiAngleBox.Text = "";
                    AppliedStressBox.Text = "";
                }
            }
            else
            {
                DiffractionPeakList.ItemsSource = null;

                OmegaAngleBox.Text = "";
                ChiAngleBox.Text = "";
                AppliedStressBox.Text = "";
            }

            this.TextEventsActive = true; ;
        }

        private void OmegaAngleBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.DiffractionPatternBox.SelectedIndex != -1 && this.DiffractionPatternBox.Text != "")
            {
                if (this.TextEventsActive)
                {
                    try
                    {
                        double NewValue = Convert.ToDouble(OmegaAngleBox.Text);

                        string PatternName = this.DiffractionPatternBox.Text;
                        Pattern.DiffractionPattern SelectedPattern = null;
                        for (int n = 0; n < this.ActSample.DiffractionPatterns.Count; n++)
                        {
                            if (this.ActSample.DiffractionPatterns[n].Name == PatternName)
                            {
                                SelectedPattern = this.ActSample.DiffractionPatterns[n];
                                break;
                            }
                        }

                        SelectedPattern.OmegaAngle = NewValue;

                        OmegaAngleBox.Foreground = Brushes.DarkGreen;
                    }
                    catch
                    {
                        OmegaAngleBox.Foreground = Brushes.DarkRed;
                    }
                }
            }
        }

        private void ChiAngleBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.DiffractionPatternBox.SelectedIndex != -1 && this.DiffractionPatternBox.Text != "")
            {
                if (this.TextEventsActive)
                {
                    try
                    {
                        double NewValue = Convert.ToDouble(ChiAngleBox.Text);

                        string PatternName = this.DiffractionPatternBox.Text;
                        Pattern.DiffractionPattern SelectedPattern = null;
                        for (int n = 0; n < this.ActSample.DiffractionPatterns.Count; n++)
                        {
                            if(this.ActSample.DiffractionPatterns[n].Name == PatternName)
                            {
                                SelectedPattern = this.ActSample.DiffractionPatterns[n];
                                break;
                            }
                        }

                        SelectedPattern.ChiAngle = NewValue;

                        ChiAngleBox.Foreground = Brushes.DarkGreen;
                    }
                    catch
                    {
                        ChiAngleBox.Foreground = Brushes.DarkRed;
                    }
                }
            }
        }

        private void PhiSampleAngleBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.DiffractionPatternBox.SelectedIndex != -1 && this.DiffractionPatternBox.Text != "")
            {
                if (this.TextEventsActive)
                {
                    try
                    {
                        double NewValue = Convert.ToDouble(PhiSampleAngleBox.Text);

                        string PatternName = this.DiffractionPatternBox.Text;
                        Pattern.DiffractionPattern SelectedPattern = null;
                        for (int n = 0; n < this.ActSample.DiffractionPatterns.Count; n++)
                        {
                            if (this.ActSample.DiffractionPatterns[n].Name == PatternName)
                            {
                                SelectedPattern = this.ActSample.DiffractionPatterns[n];
                                break;
                            }
                        }

                        SelectedPattern.PhiSampleAngle = NewValue;

                        PhiSampleAngleBox.Foreground = Brushes.DarkGreen;
                    }
                    catch
                    {
                        PhiSampleAngleBox.Foreground = Brushes.DarkRed;
                    }
                }
            }
        }

        private void AppliedStressBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.DiffractionPatternBox.SelectedIndex != -1 && this.DiffractionPatternBox.Text != "")
            {
                if (this.TextEventsActive)
                {
                    try
                    {
                        double NewValue = Convert.ToDouble(AppliedStressBox.Text);

                        string PatternName = this.DiffractionPatternBox.Text;
                        Pattern.DiffractionPattern SelectedPattern = null;
                        for (int n = 0; n < this.ActSample.DiffractionPatterns.Count; n++)
                        {
                            if (this.ActSample.DiffractionPatterns[n].Name == PatternName)
                            {
                                SelectedPattern = this.ActSample.DiffractionPatterns[n];
                                break;
                            }
                        }

                        SelectedPattern.Stress = NewValue;

                        AppliedStressBox.Foreground = Brushes.DarkGreen;
                    }
                    catch
                    {
                        AppliedStressBox.Foreground = Brushes.DarkRed;
                    }
                }
            }
        }

        private void AddToElasticList_Click(object sender, RoutedEventArgs e)
        {
            if(this.ElasticCalculationList.SelectedIndex != -1)
            {
                Elasticity UsedDataSet = (Elasticity)this.ElasticCalculationList.SelectedItem;

                if(this.DiffractionPeakList.SelectedIndex != -1)
                {
                    Peaks.DiffractionPeak SelectedPeak = (Peaks.DiffractionPeak)this.DiffractionPeakList.SelectedItem;

                    try
                    {
                        if (this.DiffractionPatternBox.SelectedIndex != -1)
                        {
                            ComboBoxItem ActItem = (ComboBoxItem)this.DiffractionPatternBox.SelectedItem;
                            string PatternName = Convert.ToString(ActItem.Content);

                            if (PatternName != "")
                            {
                                for (int n = 0; n < this.ActSample.DiffractionPatterns.Count; n++)
                                {
                                    if (PatternName == this.ActSample.DiffractionPatterns[n].Name)
                                    {
                                        double appStress = this.ActSample.DiffractionPatterns[n].Stress;
                                        double psyAngle = this.ActSample.DiffractionPatterns[n].PsiAngle(SelectedPeak.Angle);
                                        PeakStressAssociation NewAssociation = new PeakStressAssociation(appStress, psyAngle, SelectedPeak);

                                        UsedDataSet.Add(NewAssociation);
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        PeakStressAssociation NewAssociation = new PeakStressAssociation(0, 0, SelectedPeak);

                        UsedDataSet.Add(NewAssociation);
                    }

                    this.ElasticCalculationList.Items.Refresh();
                    this.ElasticPeakList.Items.Refresh();
                }
            }
        }

        private void CreateNewList_Click(object sender, RoutedEventArgs e)
        {
            if (this.DiffractionPeakList.SelectedIndex != -1)
            {
                Peaks.DiffractionPeak SelectedPeak = (Peaks.DiffractionPeak)this.DiffractionPeakList.SelectedItem;

                try
                {
                    if (this.DiffractionPatternBox.SelectedIndex != -1)
                    {
                        ComboBoxItem ActItem = (ComboBoxItem)this.DiffractionPatternBox.SelectedItem;
                        string PatternName = Convert.ToString(ActItem.Content);

                        if (PatternName != "")
                        {
                            for (int n = 0; n < this.ActSample.DiffractionPatterns.Count; n++)
                            {
                                if (PatternName == this.ActSample.DiffractionPatterns[n].Name)
                                {
                                    double appStress = this.ActSample.DiffractionPatterns[n].Stress;
                                    double psyAngle = this.ActSample.DiffractionPatterns[n].PsiAngle(SelectedPeak.Angle);
                                    PeakStressAssociation NewAssociation = new PeakStressAssociation(appStress, psyAngle, SelectedPeak);

                                    Elasticity NewDataSet = new Elasticity();
                                    NewDataSet.Add(NewAssociation);

                                    this.ActSample.MacroElasticData.Add(NewDataSet);
                                }
                            }
                        }
                    }
                }
                catch
                {
                    PeakStressAssociation NewAssociation = new PeakStressAssociation(0, 0, SelectedPeak);

                    Elasticity NewDataSet = new Elasticity();
                    NewDataSet.Add(NewAssociation);

                    this.ActSample.MacroElasticData.Add(NewDataSet);
                }

                this.ElasticCalculationList.Items.Refresh();
                this.ElasticPeakList.Items.Refresh();
            }
        }

        private void ElasticCalculationList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.TextEventsActive = false;

            if (this.ElasticCalculationList.SelectedIndex != -1)
            {
                Elasticity ActElasticCalc = (Elasticity)this.ElasticCalculationList.SelectedItem;
                this.ElasticPeakList.ItemsSource = ActElasticCalc;

                PlotElasticData(ActElasticCalc);
            }
            else
            {
                this.ElasticPeakList.ItemsSource = null;
                this.ElasticPsyAngleBox.Text = "";
                this.ElasticAppliedStressBox.Text = "";

                MainPlot.Model.Series.Clear();
                ResPlot.Model.Series.Clear();

                this.MainPlot.Model.ResetAllAxes();
                this.MainPlot.Model.InvalidatePlot(true);
                this.ResPlot.Model.ResetAllAxes();
                this.ResPlot.Model.InvalidatePlot(true);
            }

            this.TextEventsActive = true;
        }

        private void PlotElasticData(Elasticity ElasticData)
        {
            ElasticData.Sort((x, y) => x.DPeak.LatticeDistance.CompareTo(y.DPeak.LatticeDistance));
            MainPlot.Model.Series.Clear();
            ResPlot.Model.Series.Clear();

            OxyPlot.Series.LineSeries MainPointTmp = new OxyPlot.Series.LineSeries();
            OxyPlot.Series.LineSeries MainFitTmp = new OxyPlot.Series.LineSeries();
            OxyPlot.Series.LineSeries ResTmp = new OxyPlot.Series.LineSeries();

            if (ElasticData.Count > 0 && ElasticData.CalculatedCounts.Count > 0)
            {
                MainPointTmp.Title = "Psi angle: " + ElasticData[0].PsiAngle.ToString("F3") + ", Applied stress: " + ElasticData[0].Stress.ToString("F3");
                MainFitTmp.Title = "E-Modul: " + ElasticData.EModul.ToString("F3") + " +- " + ElasticData.EModulError.ToString("F3") + ", Constant: " + ElasticData.FittingFunction.Constant.ToString("F3");

                MainPointTmp.LineStyle = OxyPlot.LineStyle.None;
                ResTmp.LineStyle = OxyPlot.LineStyle.None;
                MainFitTmp.LineStyle = OxyPlot.LineStyle.Solid;

                MainPointTmp.StrokeThickness = 0;
                MainPointTmp.MarkerSize = 3;
                MainFitTmp.StrokeThickness = 2;
                MainFitTmp.MarkerSize = 0;
                ResTmp.StrokeThickness = 0;
                ResTmp.MarkerSize = 3;

                MainPointTmp.MarkerType = OxyPlot.MarkerType.Circle;
                MainFitTmp.MarkerType = OxyPlot.MarkerType.None;
                ResTmp.MarkerType = OxyPlot.MarkerType.Circle;

                MainPointTmp.Color = OxyPlot.OxyColors.DarkBlue;
                MainFitTmp.Color = OxyPlot.OxyColors.Black;
                ResTmp.Color = OxyPlot.OxyColors.Black;

                MainPointTmp.MarkerFill = OxyPlot.OxyColors.DarkBlue;
                ResTmp.MarkerFill = OxyPlot.OxyColors.Black;

                Pattern.Counts ElasticDataPoints = ElasticData.CalculatedCounts;

                double LowerXBorder = ElasticDataPoints[0][0];
                double UpperXBorder = ElasticDataPoints[ElasticDataPoints.Count - 1][0];
                double LowerYBorder = Double.MaxValue;
                double UpperYBorder = Double.MinValue;
                double LowerResYBorder = Double.MaxValue;
                double UpperResYBorder = Double.MinValue;

                for (int n = 0; n < ElasticData.Count; n++)
                {
                    if(LowerYBorder > ElasticDataPoints[n][1])
                    {
                        LowerYBorder = ElasticDataPoints[n][1];
                    }
                    if (UpperYBorder < ElasticDataPoints[n][1])
                    {
                        UpperYBorder = ElasticDataPoints[n][1];
                    }

                    OxyPlot.DataPoint DPTmp = new OxyPlot.DataPoint(ElasticDataPoints[n][0], ElasticDataPoints[n][1]);
                    MainPointTmp.Points.Add(DPTmp);
                    double Deviation = ElasticDataPoints[n][1] - ElasticData.FittingFunction.Y(ElasticDataPoints[n][0]);
                    if (LowerResYBorder > Deviation)
                    {
                        LowerResYBorder = Deviation;
                    }
                    if (UpperResYBorder < Deviation)
                    {
                        UpperResYBorder = Deviation;
                    }

                    OxyPlot.DataPoint RDPTmp = new OxyPlot.DataPoint(ElasticDataPoints[n][0], Deviation/* / ElasticDataPoints[n][2]*/);
                    ResTmp.Points.Add(RDPTmp);
                }

                if(LowerXBorder > 0)
                {
                    LowerXBorder *= 0.9;
                }
                else
                {
                    LowerXBorder *= 1.1;
                }
                if (UpperXBorder > 0)
                {
                    UpperXBorder *= 1.1;
                }
                else
                {
                    UpperXBorder *= 0.9;
                }
                if (LowerYBorder > 0)
                {
                    LowerYBorder *= 0.9;
                }
                else
                {
                    LowerYBorder *= 1.1;
                }
                if (UpperYBorder > 0)
                {
                    UpperYBorder *= 1.1;
                }
                else
                {
                    UpperYBorder *= 0.9;
                }

                for (double i = LowerXBorder; i < UpperXBorder; i += ((UpperXBorder) - (LowerXBorder)) / 100)
                {
                    OxyPlot.DataPoint DPTmp = new OxyPlot.DataPoint(i, ElasticData.FittingFunction.Y(i));
                    MainFitTmp.Points.Add(DPTmp);
                }

                MainXAxisLin.Minimum = LowerXBorder;
                MainXAxisLin.Maximum = UpperXBorder;

                ResXAxisLin.Minimum = LowerXBorder;
                ResXAxisLin.Maximum = UpperXBorder;

                MainYAxisLin.Minimum = LowerYBorder;
                MainYAxisLin.Maximum = UpperYBorder;

                ResYAxisLin.Minimum = LowerResYBorder * 1.1;
                ResYAxisLin.Maximum = UpperResYBorder * 1.1;

                MainPlotModel.Series.Add(MainPointTmp);
                MainPlotModel.Series.Add(MainFitTmp);

                ResPlotModel.Series.Add(ResTmp);
            }

            ResPlotModel.Annotations.Clear();

            OxyPlot.Annotations.LineAnnotation Res1Annotation = new OxyPlot.Annotations.LineAnnotation();
            OxyPlot.Annotations.LineAnnotation Res2Annotation = new OxyPlot.Annotations.LineAnnotation();
            OxyPlot.Annotations.LineAnnotation Res3Annotation = new OxyPlot.Annotations.LineAnnotation();
            OxyPlot.Annotations.LineAnnotation Res4Annotation = new OxyPlot.Annotations.LineAnnotation();

            Res1Annotation.LineStyle = OxyPlot.LineStyle.Dash;
            Res2Annotation.LineStyle = OxyPlot.LineStyle.Dash;
            Res3Annotation.LineStyle = OxyPlot.LineStyle.Dash;
            Res4Annotation.LineStyle = OxyPlot.LineStyle.Dash;

            Res1Annotation.Color = OxyPlot.OxyColors.OrangeRed;
            Res2Annotation.Color = OxyPlot.OxyColors.OrangeRed;
            Res3Annotation.Color = OxyPlot.OxyColors.DarkRed;
            Res4Annotation.Color = OxyPlot.OxyColors.DarkRed;

            Res1Annotation.ClipByXAxis = true;
            Res2Annotation.ClipByXAxis = true;
            Res3Annotation.ClipByXAxis = true;
            Res4Annotation.ClipByXAxis = true;

            Res1Annotation.Type = OxyPlot.Annotations.LineAnnotationType.Horizontal;
            Res2Annotation.Type = OxyPlot.Annotations.LineAnnotationType.Horizontal;
            Res3Annotation.Type = OxyPlot.Annotations.LineAnnotationType.Horizontal;
            Res4Annotation.Type = OxyPlot.Annotations.LineAnnotationType.Horizontal;

            Res1Annotation.Y = 1 * ElasticData.EModulError;
            Res2Annotation.Y = -1 * ElasticData.EModulError;
            Res3Annotation.Y = 3 * ElasticData.EModulError;
            Res4Annotation.Y = -3 * ElasticData.EModulError;

            ResPlotModel.Annotations.Add(Res1Annotation);
            ResPlotModel.Annotations.Add(Res2Annotation);
            ResPlotModel.Annotations.Add(Res3Annotation);
            ResPlotModel.Annotations.Add(Res4Annotation);

            this.MainPlot.Model.ResetAllAxes();
            this.MainPlot.Model.InvalidatePlot(true);
            this.ResPlot.Model.ResetAllAxes();
            this.ResPlot.Model.InvalidatePlot(true);
        }

        private void ElasticPeakList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.TextEventsActive = false;

            if (ElasticPeakList.SelectedIndex != -1)
            {
                PeakStressAssociation PSA = (PeakStressAssociation)ElasticPeakList.SelectedItem;
                ElasticPsyAngleBox.Text = PSA.PsiAngle.ToString("F3");
                ElasticAppliedStressBox.Text = PSA.Stress.ToString("F3");
            }
            else
            {
                ElasticPsyAngleBox.Text = "";
                ElasticAppliedStressBox.Text = "";
            }

            this.TextEventsActive = true;
        }

        private void ElasticPsyAngleBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(ElasticPeakList.SelectedIndex != -1)
            {
                PeakStressAssociation PSA = (PeakStressAssociation)ElasticPeakList.SelectedItem;

                if (this.TextEventsActive)
                {
                    try
                    {
                        double NewValue = Convert.ToDouble(ElasticPsyAngleBox.Text);

                        PSA.PsiAngle = NewValue;

                        ElasticPsyAngleBox.Foreground = Brushes.DarkGreen;
                    }
                    catch
                    {
                        ElasticPsyAngleBox.Foreground = Brushes.DarkRed;
                    }
                }
            }
        }

        private void ElasticAppliedStressBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ElasticPeakList.SelectedIndex != -1)
            {
                PeakStressAssociation PSA = (PeakStressAssociation)ElasticPeakList.SelectedItem;

                if (this.TextEventsActive)
                {
                    try
                    {
                        double NewValue = Convert.ToDouble(ElasticAppliedStressBox.Text);

                        PSA.PsiAngle = NewValue;

                        ElasticAppliedStressBox.Foreground = Brushes.DarkGreen;
                    }
                    catch
                    {
                        ElasticAppliedStressBox.Foreground = Brushes.DarkRed;
                    }
                }
            }
        }

        private void RemoveFromElasticList_Click(object sender, RoutedEventArgs e)
        {
            if (ElasticCalculationList.SelectedIndex != -1)
            {
                if (ElasticPeakList.SelectedIndex != -1)
                {
                    Elasticity ElasticData = (Elasticity)ElasticCalculationList.SelectedItem;

                    PeakStressAssociation PSA = (PeakStressAssociation)ElasticPeakList.SelectedItem;

                    ElasticData.Remove(PSA);

                    ElasticCalculationList.Items.Refresh();
                    ElasticPeakList.Items.Refresh();
                }
            }
        }

        private void RemoveElasticList_Click(object sender, RoutedEventArgs e)
        {
            if (ElasticCalculationList.SelectedIndex != -1)
            {
                Elasticity ElasticData = (Elasticity)ElasticCalculationList.SelectedItem;

                this.ActSample.MacroElasticData.Remove(ElasticData);

                ElasticCalculationList.Items.Refresh();
                ElasticPeakList.Items.Refresh();

                RefreshSampleElasticData();
            }
        }

        private void RefitElasticity_Click(object sender, RoutedEventArgs e)
        {
            if (ElasticCalculationList.SelectedIndex != -1)
            {
                Elasticity ElasticData = (Elasticity)ElasticCalculationList.SelectedItem;

                ElasticData.FitToCounts();

                ElasticCalculationList.Items.Refresh();
                ElasticPeakList.Items.Refresh();

                RefreshSampleElasticData();
                //SampleEModule.Content = this.ActSample.AveragedEModulStandard().ToString("F3");
                //SamplePoissonNumber.Content = this.ActSample.AveragedPossionNumberStandard().ToString("F3");
            }
        }

        private void AutoAssociation_Click(object sender, RoutedEventArgs e)
        {
            for (int n = 0; n < this.ActSample.DiffractionPatterns.Count; n++)
            {
                for (int i = 0; i < this.ActSample.DiffractionPatterns[n].FoundPeaks.Count; i++)
                {
                    bool PeakAdded = false;

                    for (int j = 0; j < ActSample.MacroElasticData.Count; j++)
                    {
                        if (this.ActSample.DiffractionPatterns[n].PsiAngle(this.ActSample.DiffractionPatterns[n].FoundPeaks[i].Angle) < CalScec.Properties.Settings.Default.PsyAcceptanceAngle || Math.Abs(this.ActSample.DiffractionPatterns[n].PsiAngle(this.ActSample.DiffractionPatterns[n].FoundPeaks[i].Angle) - 90.0) < CalScec.Properties.Settings.Default.PsyAcceptanceAngle)
                        {
                            if (ActSample.MacroElasticData[j].Count != 0)
                            {
                                if (Math.Abs(ActSample.MacroElasticData[j].PsiAngle - this.ActSample.DiffractionPatterns[n].PsiAngle(this.ActSample.DiffractionPatterns[n].FoundPeaks[i].Angle)) < CalScec.Properties.Settings.Default.PsyAcceptanceAngle)
                                {
                                    if (ActSample.MacroElasticData[j][0].DPeak.HKLAssociation == this.ActSample.DiffractionPatterns[n].FoundPeaks[i].HKLAssociation)
                                    {
                                        PeakStressAssociation PSATmp = new PeakStressAssociation(this.ActSample.DiffractionPatterns[n].Stress, this.ActSample.DiffractionPatterns[n].PsiAngle(this.ActSample.DiffractionPatterns[n].FoundPeaks[i].Angle), this.ActSample.DiffractionPatterns[n].FoundPeaks[i]);
                                        ActSample.MacroElasticData[j].Add(PSATmp);

                                        PeakAdded = true;
                                    }
                                }
                            }
                        }
                    }

                    if (!PeakAdded)
                    {
                        PeakStressAssociation PSATmp = new PeakStressAssociation(this.ActSample.DiffractionPatterns[n].Stress, this.ActSample.DiffractionPatterns[n].PsiAngle(this.ActSample.DiffractionPatterns[n].FoundPeaks[i].Angle), this.ActSample.DiffractionPatterns[n].FoundPeaks[i]);
                        Elasticity NewDataSet = new Elasticity();
                        NewDataSet.Add(PSATmp);

                        this.ActSample.MacroElasticData.Add(NewDataSet);
                    }
                }
            }

            for(int n = 0; n < this.ActSample.MacroElasticData.Count; n++)
            {
                this.ActSample.MacroElasticData[n].FitToCounts();
            }

            ElasticCalculationList.Items.Refresh();
            ElasticPeakList.Items.Refresh();

            RefreshSampleElasticData();

            //SampleEModule.Content = this.ActSample.AveragedEModulStandard().ToString("F3");
            //SamplePoissonNumber.Content = this.ActSample.AveragedPossionNumberStandard().ToString("F3");
        }
    }
}
