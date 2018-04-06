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
        Sample ActSample;

        public OxyPlot.PlotModel YieldPlotModel = new OxyPlot.PlotModel();
        OxyPlot.Axes.LinearAxis YieldXAxisLin = new OxyPlot.Axes.LinearAxis();
        OxyPlot.Axes.LinearAxis YieldYAxisLin = new OxyPlot.Axes.LinearAxis();

        private bool AnnotationEventsActive = true;

        public YieldSurfaceWindow(Sample actSample)
        {
            InitializeComponent();

            this.ActSample = actSample;
        }

        private void SetDataBindings()
        {
            for (int n = 0; n < this.ActSample.CrystalData.Count; n++)
            {
                ComboBoxItem PhaseItem = new ComboBoxItem();
                PhaseItem.Content = this.ActSample.CrystalData[n].SymmetryGroup;

                this.PhaseSwitchBox.Items.Add(PhaseItem);
            }

            this.PhaseSwitchBox.SelectedIndex = 0;

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

            this.MainYieldPlot.Model = YieldPlotModel;
        }

        private void PhaseSwitchBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(this.PhaseSwitchBox.SelectedIndex != -1)
            {
                this.ReflexList.ItemsSource = this.ActSample.YieldSurfaceData[this.PhaseSwitchBox.SelectedIndex].ReflexYieldData;
            }
        }

        private void ReflexList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(this.ReflexList.SelectedIndex != -1)
            {
                ReflexYield SelectedRY = (ReflexYield)this.ReflexList.SelectedItem;

                this.PeakList.ItemsSource = SelectedRY.PeakData;

                this.SetPlot(SelectedRY);
            }
        }

        private void SetPlot(ReflexYield RY)
        {
            #region Data points

            this.YieldPlotModel.Series.Clear();

            Pattern.Counts UsedCountsElastic = RY.MicroStrainOverMacroStrainData(true);
            Pattern.Counts UsedCountsPlastic = RY.MicroStrainOverMacroStrainData(false);

            OxyPlot.Series.LineSeries TmpElastic = new OxyPlot.Series.LineSeries();
            TmpElastic.Title = "Elastic regime";

            TmpElastic.LineStyle = OxyPlot.LineStyle.None;
            TmpElastic.MarkerSize = 2;
            TmpElastic.MarkerType = OxyPlot.MarkerType.Circle;
            TmpElastic.Color = OxyPlot.OxyColors.Black;
            TmpElastic.MarkerStroke = OxyPlot.OxyColors.Black;

            OxyPlot.Series.LineSeries TmpPlastic = new OxyPlot.Series.LineSeries();
            TmpPlastic.Title = "Plastic regime";

            TmpPlastic.LineStyle = OxyPlot.LineStyle.None;
            TmpPlastic.MarkerSize = 2;
            TmpPlastic.MarkerType = OxyPlot.MarkerType.Cross;
            TmpPlastic.Color = OxyPlot.OxyColors.DarkRed;
            TmpPlastic.MarkerStroke = OxyPlot.OxyColors.DarkRed;

            double Xmin = double.MaxValue;
            double Xmax = double.MinValue;

            double Ymin = double.MaxValue;
            double Ymax = double.MinValue;

            for (int n = 0; n < UsedCountsElastic.Count; n++)
            {
                OxyPlot.DataPoint EDP = new OxyPlot.DataPoint(UsedCountsElastic[n][0], UsedCountsElastic[n][1]);

                TmpElastic.Points.Add(EDP);

                if(Xmin > UsedCountsElastic[n][0])
                {
                    Xmin = UsedCountsElastic[n][0];
                }
                if (Ymin > UsedCountsElastic[n][1])
                {
                    Ymin = UsedCountsElastic[n][1];
                }
            }

            for (int n = 0; n < UsedCountsPlastic.Count; n++)
            {
                OxyPlot.DataPoint PDP = new OxyPlot.DataPoint(UsedCountsPlastic[n][0], UsedCountsPlastic[n][1]);

                TmpPlastic.Points.Add(PDP);

                if (Xmax > UsedCountsPlastic[n][0])
                {
                    Xmax = UsedCountsPlastic[n][0];
                }
                if (Ymax > UsedCountsPlastic[n][1])
                {
                    Ymax = UsedCountsPlastic[n][1];
                }
            }

            this.YieldXAxisLin.Minimum = Xmin;
            this.YieldXAxisLin.Maximum = Xmax;
            this.YieldYAxisLin.Minimum = Ymin;
            this.YieldYAxisLin.Maximum = Ymax;

            #endregion

            this.YieldPlotModel.Series.Add(TmpElastic);
            this.YieldPlotModel.Series.Add(TmpPlastic);

            this.MainYieldPlot.ResetAllAxes();
            this.MainYieldPlot.InvalidatePlot(true);
        }

        private void SetAnnotations(ReflexYield RY)
        {
            this.AnnotationEventsActive = false;

            this.YieldPlotModel.Annotations.Clear();

            double ELB = RY.LowerElasticBorder();
            double EHB = RY.UpperElasticBorder();
            double PLB = RY.LowerPlasticBorder();
            double PHB = RY.UpperPlasticBorder();

            this.ElasticLow.Text = ELB.ToString("e3");
            this.ElasticHigh.Text = EHB.ToString("e3");
            this.PlasticLow.Text = PLB.ToString("e3");
            this.PlasticHigh.Text = PHB.ToString("e3");

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
            for(int n = 0; n < this.ActSample.DiffractionPatterns.Count; n++)
            {
                for(int i = 0; i < this.ActSample.DiffractionPatterns[n].FoundPeaks.Count; i++)
                {
                    for( int j = 0; j < this.ActSample.CrystalData.Count; j++)
                    {
                        if(this.ActSample.DiffractionPatterns[n].FoundPeaks[i].AssociatedCrystalData.SymmetryGroup == this.ActSample.CrystalData[j].SymmetryGroup)
                        {
                            bool found = false;

                            for(int k = 0; k < this.ActSample.YieldSurfaceData[j].ReflexYieldData.Count; k++)
                            {
                                if(this.ActSample.DiffractionPatterns[n].FoundPeaks[i].AssociatedHKLReflex.HKLString == this.ActSample.YieldSurfaceData[j].ReflexYieldData[k].SlipPlane.HKLString)
                                {
                                    Stress.Macroskopic.PeakStressAssociation PSATmp = new Macroskopic.PeakStressAssociation(this.ActSample.DiffractionPatterns[n].Stress, this.ActSample.DiffractionPatterns[n].PsiAngle(this.ActSample.DiffractionPatterns[n].FoundPeaks[i].Angle), this.ActSample.DiffractionPatterns[n].PhiAngle(this.ActSample.DiffractionPatterns[n].FoundPeaks[i].Angle), this.ActSample.DiffractionPatterns[n].MacroStrain, true, this.ActSample.DiffractionPatterns[n].FoundPeaks[i]);
                                    PSATmp.MainSlipDirectionAngle = this.ActSample.DiffractionPatterns[n].SlipDirectionAngle(this.ActSample.DiffractionPatterns[n].FoundPeaks[i].Angle, this.ActSample.YieldSurfaceData[j].ReflexYieldData[k].SlipPlane, this.ActSample.YieldSurfaceData[j].ReflexYieldData[k].MainSlipDirection);
                                    PSATmp.SecondarySlipDirectionAngle = this.ActSample.DiffractionPatterns[n].SlipDirectionAngle(this.ActSample.DiffractionPatterns[n].FoundPeaks[i].Angle, this.ActSample.YieldSurfaceData[j].ReflexYieldData[k].SlipPlane, this.ActSample.YieldSurfaceData[j].ReflexYieldData[k].SecondarySlipDirection);
                                    this.ActSample.YieldSurfaceData[j].ReflexYieldData[k].PeakData.Add(PSATmp);

                                    found = true;
                                    break;
                                }
                            }

                            if(!found)
                            {
                                ReflexYield RYieldTmp = new ReflexYield(this.ActSample.DiffractionPatterns[n].FoundPeaks[i].AssociatedHKLReflex, this.ActSample.CrystalData[j]);

                                Stress.Macroskopic.PeakStressAssociation PSATmp = new Macroskopic.PeakStressAssociation(this.ActSample.DiffractionPatterns[n].Stress, this.ActSample.DiffractionPatterns[n].PsiAngle(this.ActSample.DiffractionPatterns[n].FoundPeaks[i].Angle), this.ActSample.DiffractionPatterns[n].PhiAngle(this.ActSample.DiffractionPatterns[n].FoundPeaks[i].Angle), this.ActSample.DiffractionPatterns[n].MacroStrain, true, this.ActSample.DiffractionPatterns[n].FoundPeaks[i]);
                                PSATmp.MainSlipDirectionAngle = this.ActSample.DiffractionPatterns[n].SlipDirectionAngle(this.ActSample.DiffractionPatterns[n].FoundPeaks[i].Angle, RYieldTmp.SlipPlane, RYieldTmp.MainSlipDirection);
                                PSATmp.SecondarySlipDirectionAngle = this.ActSample.DiffractionPatterns[n].SlipDirectionAngle(this.ActSample.DiffractionPatterns[n].FoundPeaks[i].Angle, RYieldTmp.SlipPlane, RYieldTmp.SecondarySlipDirection);
                                RYieldTmp.PeakData.Add(PSATmp);
                                
                                this.ActSample.YieldSurfaceData[j].ReflexYieldData.Add(RYieldTmp);
                            }
                        }
                    }
                }
            }
        }

        private void FitYieldStrenght_Click(object sender, RoutedEventArgs e)
        {
            if (this.ReflexList.SelectedIndex != -1)
            {
                ReflexYield SelectedRY = (ReflexYield)this.ReflexList.SelectedItem;

                SelectedRY.FitElasticData();
                SelectedRY.FitPlasticData();

                this.ReflexList.Items.Refresh();
            }
        }
    }
}
