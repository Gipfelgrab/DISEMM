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
    /// Interaction logic for REKAssociationCalculationWindow.xaml
    /// </summary>
    public partial class REKAssociationCalculationWindow : Window
    {
        public Sample ActSample;
        bool TextEventsActive = true;

        public OxyPlot.PlotModel MainPlotModel = new OxyPlot.PlotModel();
        OxyPlot.Axes.LinearAxis MainXAxisLin = new OxyPlot.Axes.LinearAxis();
        OxyPlot.Axes.LinearAxis MainYAxisLin = new OxyPlot.Axes.LinearAxis();

        public OxyPlot.PlotModel ResPlotModel = new OxyPlot.PlotModel();
        OxyPlot.Axes.LinearAxis ResXAxisLin = new OxyPlot.Axes.LinearAxis();
        OxyPlot.Axes.LinearAxis ResYAxisLin = new OxyPlot.Axes.LinearAxis();

        public REKAssociationCalculationWindow(Sample actSample)
        {
            InitializeComponent();

            this.ActSample = actSample;

            this.LoadData();
            this.PreparePlot();
        }

        private void LoadData()
        {
            TextEventsActive = false;
            ComboBoxItem Empty = new ComboBoxItem();
            Empty.Content = "";

            this.DiffractionPatternBox.Items.Add(Empty);

            for (int n = 0; n < this.ActSample.DiffractionPatterns.Count; n++)
            {
                ComboBoxItem PatternItem = new ComboBoxItem();
                PatternItem.Content = this.ActSample.DiffractionPatterns[n].Name;

                this.DiffractionPatternBox.Items.Add(PatternItem);
            }

            for(int n = 0; n < this.ActSample.CrystalData.Count; n++)
            {
                ComboBoxItem PhaseItem = new ComboBoxItem();
                PhaseItem.Content = this.ActSample.CrystalData[n].SymmetryGroup;

                this.PhaseSwitchBox.Items.Add(PhaseItem);
            }

            this.PhaseSwitchBox.SelectedIndex = 0;
            this.REKCalculationList.ItemsSource = this.ActSample.DiffractionConstants[0];

            //RefreshSampleElasticData();
            TextEventsActive = true;
        }

        private void PreparePlot()
        {
            MainPlotModel.LegendBorder = OxyPlot.OxyColors.Black;
            MainPlotModel.LegendItemAlignment = OxyPlot.HorizontalAlignment.Left;
            MainPlotModel.LegendTitle = "REK data";

            MainXAxisLin.Position = OxyPlot.Axes.AxisPosition.Bottom;
            MainXAxisLin.Minimum = 0;
            MainXAxisLin.Maximum = 180;
            MainXAxisLin.Title = "cos^2(Psi)";

            MainYAxisLin.Position = OxyPlot.Axes.AxisPosition.Left;
            MainYAxisLin.Minimum = 0;
            MainYAxisLin.Maximum = 10;
            MainYAxisLin.Title = "Strain / Applied stress";

            ResXAxisLin.Position = OxyPlot.Axes.AxisPosition.Bottom;
            ResXAxisLin.Minimum = 0;
            ResXAxisLin.Maximum = 180;
            ResXAxisLin.Title = "cos^2(Psi)";

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

            Res1Annotation.Y = 1;
            Res2Annotation.Y = -1;
            Res3Annotation.Y = 3;
            Res4Annotation.Y = -3;

            ResPlotModel.Annotations.Add(Res1Annotation);
            ResPlotModel.Annotations.Add(Res2Annotation);
            ResPlotModel.Annotations.Add(Res3Annotation);
            ResPlotModel.Annotations.Add(Res4Annotation);

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
                            OmegaAngleBox.Text = this.ActSample.DiffractionPatterns[n].OmegaAngle.ToString("F3");
                            ChiAngleBox.Text = this.ActSample.DiffractionPatterns[n].ChiAngle.ToString("F3");
                            PhiSampleAngleBox.Text = this.ActSample.DiffractionPatterns[n].PhiSampleAngle.ToString("F3");
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
                    PhiSampleAngleBox.Text = "";
                    AppliedStressBox.Text = "";
                }
            }
            else
            {
                DiffractionPeakList.ItemsSource = null;

                OmegaAngleBox.Text = "";
                ChiAngleBox.Text = "";
                PhiSampleAngleBox.Text = "";
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
                            if (this.ActSample.DiffractionPatterns[n].Name == PatternName)
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

        private void CreateNewList_Click(object sender, RoutedEventArgs e)
        {
            if (this.DiffractionPeakList.SelectedIndex != -1 && this.PhaseSwitchBox.SelectedIndex != -1 && this.PhaseSwitchBox.Text != "")
            {
                Peaks.DiffractionPeak SelectedPeak = (Peaks.DiffractionPeak)this.DiffractionPeakList.SelectedItem;
                REK NewREKData = null;
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
                                    Macroskopic.PeakStressAssociation NewAssociation = new Macroskopic.PeakStressAssociation(appStress, psyAngle, SelectedPeak);

                                    NewREKData = new REK(this.ActSample.CrystalData[this.PhaseSwitchBox.SelectedIndex], SelectedPeak.AssociatedHKLReflex);
                                    NewREKData.ElasticStressData.Add(NewAssociation);
                                }
                            }
                        }
                    }
                }
                catch
                {
                    Macroskopic.PeakStressAssociation NewAssociation = new Macroskopic.PeakStressAssociation(0, 0, SelectedPeak);

                    NewREKData = new REK(this.ActSample.CrystalData[this.PhaseSwitchBox.SelectedIndex], SelectedPeak.AssociatedHKLReflex);
                    NewREKData.ElasticStressData.Add(NewAssociation);
                }
                finally
                {
                    for(int n= 0; n < this.ActSample.MacroElasticData.Count; n++)
                    {
                        if (this.ActSample.MacroElasticData[n][0].HKLAssociation == NewREKData.HKLAssociation)
                        {
                            if (this.ActSample.MacroElasticData[n][0].PsiAngle == 0)
                            {
                                NewREKData.LongitudionalElasticity = this.ActSample.MacroElasticData[n];
                            }
                            else if(this.ActSample.MacroElasticData[n][0].PsiAngle == 90)
                            {
                                NewREKData.TransversalElasticity = this.ActSample.MacroElasticData[n];
                            }
                        }
                    }

                    this.ActSample.DiffractionConstants[this.PhaseSwitchBox.SelectedIndex].Add(NewREKData);
                }

                this.REKCalculationList.Items.Refresh();
                this.REKPeakList.Items.Refresh();
            }
        }

        private void AddToREKList_Click(object sender, RoutedEventArgs e)
        {
            if (this.REKCalculationList.SelectedIndex != -1)
            {
                REK UsedDataSet = (REK)this.REKCalculationList.SelectedItem;

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
                                        Macroskopic.PeakStressAssociation NewAssociation = new Macroskopic.PeakStressAssociation(appStress, psyAngle, SelectedPeak);

                                        UsedDataSet.ElasticStressData.Add(NewAssociation);
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        Macroskopic.PeakStressAssociation NewAssociation = new Macroskopic.PeakStressAssociation(0, 0, SelectedPeak);

                        UsedDataSet.ElasticStressData.Add(NewAssociation);
                    }

                    this.REKCalculationList.Items.Refresh();
                    this.REKPeakList.Items.Refresh();
                }
            }
        }

        private void REKCalculationList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(this.REKCalculationList.SelectedIndex != -1)
            {
                REK SelectedItem = (REK)this.REKCalculationList.SelectedItem;

                this.REKPeakList.ItemsSource = SelectedItem.ElasticStressData;

                PlotREKDataClassic(SelectedItem);
            }
            else
            {
                this.REKPeakList.ItemsSource = null;
            }
        }

        private void PlotREKDataClassic(REK REKData)
        {
            MainPlot.Model.Series.Clear();
            ResPlot.Model.Series.Clear();

            OxyPlot.Series.LineSeries MainPointTmp = new OxyPlot.Series.LineSeries();
            OxyPlot.Series.LineSeries MainFitTmp = new OxyPlot.Series.LineSeries();
            OxyPlot.Series.LineSeries ResTmp = new OxyPlot.Series.LineSeries();

            Pattern.Counts UsedCounts = REKData.GetClassicREKFittingData();

            if (UsedCounts.Count > 0)
            {
                MainPointTmp.Title = REKData.HKLAssociation;
                MainFitTmp.Title = "S1: " + REKData.ClassicS1.ToString("G3") + " +- " + REKData.ClassicS1Error.ToString("G3") + ", 0.5S2: " + REKData.ClassicHS2.ToString("G3") + " +- " + REKData.ClassicHS2Error.ToString("G3");

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

                double LowerXBorder = 0;
                double UpperXBorder = 1;
                double LowerYBorder = Double.MaxValue;
                double UpperYBorder = Double.MinValue;
                double LowerResYBorder = Double.MaxValue;
                double UpperResYBorder = Double.MinValue;

                for (int n = 0; n < UsedCounts.Count; n++)
                {
                    if (LowerYBorder > UsedCounts[n][1])
                    {
                        LowerYBorder = UsedCounts[n][1];
                    }
                    if (UpperYBorder < UsedCounts[n][1])
                    {
                        UpperYBorder = UsedCounts[n][1];
                    }

                    OxyPlot.DataPoint DPTmp = new OxyPlot.DataPoint(UsedCounts[n][0], UsedCounts[n][1]);
                    MainPointTmp.Points.Add(DPTmp);
                    double Deviation = UsedCounts[n][1] - REKData.ClassicFittingFunction.Y(UsedCounts[n][0]);
                    if (LowerResYBorder > Deviation)
                    {
                        LowerResYBorder = Deviation;
                    }
                    if (UpperResYBorder < Deviation)
                    {
                        UpperResYBorder = Deviation;
                    }

                    OxyPlot.DataPoint RDPTmp = new OxyPlot.DataPoint(UsedCounts[n][0], Deviation/* / ElasticDataPoints[n][2]*/);
                    ResTmp.Points.Add(RDPTmp);
                }

                if (LowerXBorder > 0)
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
                    OxyPlot.DataPoint DPTmp = new OxyPlot.DataPoint(i, REKData.ClassicFittingFunction.Y(i));
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

            Res1Annotation.Y = 1 * (REKData.ClassicS1Error + REKData.ClassicHS2Error);
            Res2Annotation.Y = -1 * (REKData.ClassicS1Error + REKData.ClassicHS2Error);
            Res3Annotation.Y = 3 * (REKData.ClassicS1Error + REKData.ClassicHS2Error);
            Res4Annotation.Y = -3 * (REKData.ClassicS1Error + REKData.ClassicHS2Error);

            ResPlotModel.Annotations.Add(Res1Annotation);
            ResPlotModel.Annotations.Add(Res2Annotation);
            ResPlotModel.Annotations.Add(Res3Annotation);
            ResPlotModel.Annotations.Add(Res4Annotation);

            this.MainPlot.Model.ResetAllAxes();
            this.MainPlot.Model.InvalidatePlot(true);
            this.ResPlot.Model.ResetAllAxes();
            this.ResPlot.Model.InvalidatePlot(true);
        }

        private void RemovePeakFromREK_Click(object sender, RoutedEventArgs e)
        {
            if (this.REKCalculationList.SelectedIndex != -1 && this.REKPeakList.SelectedIndex != -1)
            {
                REK SelectedItem = (REK)this.REKCalculationList.SelectedItem;
                Macroskopic.PeakStressAssociation SelectedPeak = (Macroskopic.PeakStressAssociation)this.REKPeakList.SelectedItem;

                SelectedItem.ElasticStressData.Remove(SelectedPeak);

                this.REKPeakList.Items.Refresh();
            }
        }

        private void AutoREK_Click(object sender, RoutedEventArgs e)
        {
            if(CalScec.Properties.Settings.Default.REKAutoAusociationType == 1)
            {
                for(int n = 0; n < this.ActSample.CrystalData.Count; n++)
                {
                    for(int i = 0; i < this.ActSample.CrystalData[n].HKLList.Count; i++)
                    {
                        REK ActualREK = new REK(this.ActSample.CrystalData[n], this.ActSample.CrystalData[n].HKLList[i]);

                        for(int j = 0; j < this.ActSample.DiffractionPatterns.Count; j++)
                        {
                            for(int k = 0; k < this.ActSample.DiffractionPatterns[j].FoundPeaks.Count; k++)
                            {
                                if(this.ActSample.DiffractionPatterns[j].FoundPeaks[k].AssociatedCrystalData.SymmetryGroupID == this.ActSample.CrystalData[n].SymmetryGroupID)
                                {
                                    if(this.ActSample.DiffractionPatterns[j].FoundPeaks[k].AssociatedHKLReflex.HKLString == this.ActSample.CrystalData[n].HKLList[i].HKLString)
                                    {
                                        Macroskopic.PeakStressAssociation NewAssociation = new Macroskopic.PeakStressAssociation(this.ActSample.DiffractionPatterns[j].Stress, this.ActSample.DiffractionPatterns[j].PsiAngle(this.ActSample.DiffractionPatterns[j].FoundPeaks[k].Angle), this.ActSample.DiffractionPatterns[j].FoundPeaks[k]);

                                        ActualREK.ElasticStressData.Add(NewAssociation);
                                    }
                                }
                            }
                        }

                        for (int j = 0; j < this.ActSample.MacroElasticData.Count; j++)
                        {
                            if (this.ActSample.MacroElasticData[j][0].HKLAssociation == ActualREK.HKLAssociation)
                            {
                                if (this.ActSample.MacroElasticData[j][0].PsiAngle == 0)
                                {
                                    ActualREK.LongitudionalElasticity = this.ActSample.MacroElasticData[j];
                                }
                                else if (this.ActSample.MacroElasticData[j][0].PsiAngle == 90)
                                {
                                    ActualREK.TransversalElasticity = this.ActSample.MacroElasticData[j];
                                }
                            }
                        }

                        this.ActSample.DiffractionConstants[n].Add(ActualREK);
                    }
                }
                
                this.REKCalculationList.Items.Refresh();
            }
        }

        private void DeleteREK_Click(object sender, RoutedEventArgs e)
        {
            if (this.REKCalculationList.SelectedIndex != -1)
            {
                REK SelectedItem = (REK)this.REKCalculationList.SelectedItem;

                this.ActSample.DiffractionConstants[this.PhaseSwitchBox.SelectedIndex].Remove(SelectedItem);

                this.REKCalculationList.Items.Refresh();
            }
        }

        private void PhaseSwitchBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.PhaseSwitchBox.SelectedIndex != -1)
            {
                this.REKCalculationList.ItemsSource = this.ActSample.DiffractionConstants[this.PhaseSwitchBox.SelectedIndex];
            }
        }

        private void RefitREK_Click(object sender, RoutedEventArgs e)
        {
            if (this.REKCalculationList.SelectedIndex != -1)
            {
                REK SelectedItem = (REK)this.REKCalculationList.SelectedItem;

                SelectedItem.FitClassicREKFunction();

                PlotREKDataClassic(SelectedItem);
                this.REKCalculationList.Items.Refresh();
            }
        }
    }
}
