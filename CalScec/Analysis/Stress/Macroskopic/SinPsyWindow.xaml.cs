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
    /// Interaction logic for SinPsyWindow.xaml
    /// </summary>
    public partial class SinPsyWindow : Window
    {
        public OxyPlot.PlotModel SinPlotModel = new OxyPlot.PlotModel();
        OxyPlot.Axes.LinearAxis SinXAxisLin = new OxyPlot.Axes.LinearAxis();
        OxyPlot.Axes.LinearAxis SinYAxisLin = new OxyPlot.Axes.LinearAxis();

        List<PeakStressAssociation> AllStressData;

        public SinPsyWindow(Analysis.Sample S)
        {
            InitializeComponent();

            SetDataBinding(S);
            SetPlot();
        }

        private void SetDataBinding(Analysis.Sample S)
        {
            AllStressData = S.GetSinPsyData();
            this.ReflexView.ItemsSource = AllStressData;

            CollectionView ReflexCollection = (CollectionView)CollectionViewSource.GetDefaultView(this.ReflexView.ItemsSource);
            PropertyGroupDescription ReflexGroupDescription = new PropertyGroupDescription("HKLAssociation");
            PropertyGroupDescription StressGroupDescription = new PropertyGroupDescription("stress");
            ReflexCollection.GroupDescriptions.Add(ReflexGroupDescription);
            ReflexCollection.GroupDescriptions.Add(StressGroupDescription);

            this.SinItem.IsChecked = true;
            this.CosItem.IsChecked = false;
            this.LatticeDistanceItem.IsChecked = true;
            this.ExtensionItem.IsChecked = false;
        }

        private void SetPlot()
        {
            SinPlotModel.LegendBorder = OxyPlot.OxyColors.Black;
            SinPlotModel.LegendItemAlignment = OxyPlot.HorizontalAlignment.Left;
            SinPlotModel.LegendTitle = "";

            SinXAxisLin.Position = OxyPlot.Axes.AxisPosition.Bottom;
            SinXAxisLin.Minimum = 0;
            SinXAxisLin.Maximum = 1;
            SinXAxisLin.Title = "Sin²(Psi)";

            SinYAxisLin.Position = OxyPlot.Axes.AxisPosition.Left;
            SinYAxisLin.Minimum = 1;
            SinYAxisLin.Maximum = 5;
            SinYAxisLin.Title = "Lattice distance";

            #region GridStyles

            SinXAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dash;
            SinYAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dash;

            SinXAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dot;
            SinYAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dot;

            #endregion

            SinPlotModel.Axes.Add(SinXAxisLin);
            SinPlotModel.Axes.Add(SinYAxisLin);

            this.SinPsyPlot.Model = SinPlotModel;
            this.SinPsyPlot.Model.ResetAllAxes();
            this.SinPsyPlot.Model.InvalidatePlot(true);
        }

        private void ReflexView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(this.ReflexView.SelectedIndex != -1)
            {
                this.SinPsyPlot.Model.Series.Clear();
                System.ComponentModel.BackgroundWorker worker = new System.ComponentModel.BackgroundWorker();
                object ForPlot = this.ReflexView.SelectedItems;

                if (this.SinItem.IsChecked)
                {
                    if(this.LatticeDistanceItem.IsChecked)
                    {
                        worker.DoWork += PlotSinLatticeDistance_Work;
                    }
                    else
                    {
                        worker.DoWork += PlotSinExtension_Work;
                    }
                }
                else
                {
                    if (this.LatticeDistanceItem.IsChecked)
                    {
                        worker.DoWork += PlotCosLatticeDistance_Work;
                    }
                    else
                    {
                        worker.DoWork += PlotCosExtension_Work;
                    }
                }

                worker.RunWorkerCompleted += Plot_Completed;
                worker.RunWorkerAsync(ForPlot);
            }
        }

        private void PlotSinLatticeDistance_Work(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            System.ComponentModel.BackgroundWorker worker = sender as System.ComponentModel.BackgroundWorker;
            
            System.Collections.IList Data = (System.Collections.IList)e.Argument;

            List<PeakStressAssociation> PlottedData = new List<PeakStressAssociation>();

            this.SinYAxisLin.Maximum = 0.0;
            this.SinYAxisLin.Minimum = 10;

            for (int n = 0; n < Data.Count; n++)
            {
                PeakStressAssociation ActDataSet = (PeakStressAssociation)Data[n];

                bool AlreadyPlotted = false;

                for(int i = 0; i < PlottedData.Count; i++)
                {
                    if(PlottedData[i].Stress == ActDataSet.Stress && PlottedData[i].HKLAssociation == ActDataSet.HKLAssociation)
                    {
                        AlreadyPlotted = true;
                        break;
                    }
                }

                if(!AlreadyPlotted)
                {
                    PlottedData.Add(ActDataSet);

                    List<PeakStressAssociation> PlotingData = new List<PeakStressAssociation>();

                    for(int i = 0; i < this.AllStressData.Count; i++)
                    {
                        if(this.AllStressData[i].Stress == ActDataSet.Stress && this.AllStressData[i].HKLAssociation == ActDataSet.HKLAssociation)
                        {
                            PlotingData.Add(this.AllStressData[i]);
                        }
                    }

                    int[] Blue = { 0, 255, 255, 0, 0, 255, 255 };
                    int[] Red = { 255, 0, 255, 0, 255, 0, 255 };
                    int[] Green = { 255, 255, 0, 255, 0, 0, 255 };

                    OxyPlot.Series.LineSeries Tmp = new OxyPlot.Series.LineSeries();
                    Tmp.Title = "Applied stress: " + PlotingData[0].Stress.ToString("F3") + "; " + PlotingData[0].HKLAssociation;

                    Tmp.LineStyle = OxyPlot.LineStyle.Solid;

                    Tmp.StrokeThickness = CalScec.Properties.Settings.Default.MainPlotLineThickness;
                    Tmp.MarkerSize = CalScec.Properties.Settings.Default.MainPlotDotThickness;

                    switch (n % 6)
                    {
                        case 0:
                            Tmp.MarkerType = OxyPlot.MarkerType.Circle;
                            break;
                        case 1:
                            Tmp.MarkerType = OxyPlot.MarkerType.Cross;
                            break;
                        case 2:
                            Tmp.MarkerType = OxyPlot.MarkerType.Diamond;
                            break;
                        case 3:
                            Tmp.MarkerType = OxyPlot.MarkerType.Plus;
                            break;
                        case 4:
                            Tmp.MarkerType = OxyPlot.MarkerType.Square;
                            break;
                        case 5:
                            Tmp.MarkerType = OxyPlot.MarkerType.Star;
                            break;
                        default:
                            Tmp.MarkerType = OxyPlot.MarkerType.Triangle;
                            break;
                    }

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
                    Tmp.Color = LineColor;
                    Tmp.MarkerStroke = LineColor;
                    
                    for (int i = 0; i < PlotingData.Count; i++)
                    {
                        OxyPlot.DataPoint PlotPoint = new OxyPlot.DataPoint(Math.Pow(Math.Sin(PlotingData[i].PsiAngle * (Math.PI / 180.0)), 2), PlotingData[i].DPeak.LatticeDistance);
                        Tmp.Points.Add(PlotPoint);

                        if(PlotingData[i].DPeak.LatticeDistance > this.SinYAxisLin.Maximum)
                        {
                            this.SinYAxisLin.Maximum = PlotingData[i].DPeak.LatticeDistance;
                        }
                        if (PlotingData[i].DPeak.LatticeDistance < this.SinYAxisLin.Minimum)
                        {
                            this.SinYAxisLin.Minimum = PlotingData[i].DPeak.LatticeDistance;
                        }
                    }

                    SinPlotModel.Series.Add(Tmp);
                }
            }

            this.SinXAxisLin.Title = "Sin²(Psi)";
            this.SinYAxisLin.Title = "Lattice distance";
        }

        private void PlotCosLatticeDistance_Work(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            System.ComponentModel.BackgroundWorker worker = sender as System.ComponentModel.BackgroundWorker;
            
            System.Collections.IList Data = (System.Collections.IList)e.Argument;

            List<PeakStressAssociation> PlottedData = new List<PeakStressAssociation>();

            this.SinYAxisLin.Maximum = 0.0;
            this.SinYAxisLin.Minimum = 10;

            for (int n = 0; n < Data.Count; n++)
            {
                PeakStressAssociation ActDataSet = (PeakStressAssociation)Data[n];

                bool AlreadyPlotted = false;

                for (int i = 0; i < PlottedData.Count; i++)
                {
                    if (PlottedData[i].Stress == ActDataSet.Stress && PlottedData[i].HKLAssociation == ActDataSet.HKLAssociation)
                    {
                        AlreadyPlotted = true;
                        break;
                    }
                }

                if (!AlreadyPlotted)
                {
                    PlottedData.Add(ActDataSet);

                    List<PeakStressAssociation> PlotingData = new List<PeakStressAssociation>();

                    for (int i = 0; i < this.AllStressData.Count; i++)
                    {
                        if (this.AllStressData[i].Stress == ActDataSet.Stress && this.AllStressData[i].HKLAssociation == ActDataSet.HKLAssociation)
                        {
                            PlotingData.Add(this.AllStressData[i]);
                        }
                    }

                    int[] Blue = { 0, 255, 255, 0, 0, 255, 255 };
                    int[] Red = { 255, 0, 255, 0, 255, 0, 255 };
                    int[] Green = { 255, 255, 0, 255, 0, 0, 255 };

                    OxyPlot.Series.LineSeries Tmp = new OxyPlot.Series.LineSeries();
                    Tmp.Title = "Applied stress: " + PlotingData[0].Stress.ToString("F3") + "; " + PlotingData[0].HKLAssociation;

                    Tmp.LineStyle = OxyPlot.LineStyle.Solid;

                    Tmp.StrokeThickness = CalScec.Properties.Settings.Default.MainPlotLineThickness;
                    Tmp.MarkerSize = CalScec.Properties.Settings.Default.MainPlotDotThickness;

                    switch (n % 6)
                    {
                        case 0:
                            Tmp.MarkerType = OxyPlot.MarkerType.Circle;
                            break;
                        case 1:
                            Tmp.MarkerType = OxyPlot.MarkerType.Cross;
                            break;
                        case 2:
                            Tmp.MarkerType = OxyPlot.MarkerType.Diamond;
                            break;
                        case 3:
                            Tmp.MarkerType = OxyPlot.MarkerType.Plus;
                            break;
                        case 4:
                            Tmp.MarkerType = OxyPlot.MarkerType.Square;
                            break;
                        case 5:
                            Tmp.MarkerType = OxyPlot.MarkerType.Star;
                            break;
                        default:
                            Tmp.MarkerType = OxyPlot.MarkerType.Triangle;
                            break;
                    }

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
                    Tmp.Color = LineColor;
                    Tmp.MarkerStroke = LineColor;

                    for (int i = 0; i < PlotingData.Count; i++)
                    {
                        OxyPlot.DataPoint PlotPoint = new OxyPlot.DataPoint(Math.Pow(Math.Cos(PlotingData[i].PsiAngle * (Math.PI / 180.0)), 2), PlotingData[i].DPeak.LatticeDistance);
                        Tmp.Points.Add(PlotPoint);
                        
                        if (PlotingData[i].DPeak.LatticeDistance > this.SinYAxisLin.Maximum)
                        {
                            this.SinYAxisLin.Maximum = PlotingData[i].DPeak.LatticeDistance;
                        }
                        if (PlotingData[i].DPeak.LatticeDistance < this.SinYAxisLin.Minimum)
                        {
                            this.SinYAxisLin.Minimum = PlotingData[i].DPeak.LatticeDistance;
                        }
                    }

                    SinPlotModel.Series.Add(Tmp);
                }
            }

            this.SinXAxisLin.Title = "Cos²(Psi)";
            this.SinYAxisLin.Title = "Lattice distance";
        }

        private void PlotSinExtension_Work(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            System.ComponentModel.BackgroundWorker worker = sender as System.ComponentModel.BackgroundWorker;
            
            System.Collections.IList Data = (System.Collections.IList)e.Argument;

            List<PeakStressAssociation> PlottedData = new List<PeakStressAssociation>();

            this.SinYAxisLin.Maximum = 0.0;
            this.SinYAxisLin.Minimum = 10;

            for (int n = 0; n < Data.Count; n++)
            {
                PeakStressAssociation ActDataSet = (PeakStressAssociation)Data[n];

                bool AlreadyPlotted = false;

                for (int i = 0; i < PlottedData.Count; i++)
                {
                    if (PlottedData[i].Stress == ActDataSet.Stress && PlottedData[i].HKLAssociation == ActDataSet.HKLAssociation)
                    {
                        AlreadyPlotted = true;
                        break;
                    }
                }

                if (!AlreadyPlotted)
                {
                    PlottedData.Add(ActDataSet);

                    List<PeakStressAssociation> PlotingData = new List<PeakStressAssociation>();

                    for (int i = 0; i < this.AllStressData.Count; i++)
                    {
                        if (this.AllStressData[i].Stress == ActDataSet.Stress && this.AllStressData[i].HKLAssociation == ActDataSet.HKLAssociation)
                        {
                            PlotingData.Add(this.AllStressData[i]);
                        }
                    }

                    int[] Blue = { 0, 255, 255, 0, 0, 255, 255 };
                    int[] Red = { 255, 0, 255, 0, 255, 0, 255 };
                    int[] Green = { 255, 255, 0, 255, 0, 0, 255 };

                    OxyPlot.Series.LineSeries Tmp = new OxyPlot.Series.LineSeries();
                    Tmp.Title = "Applied stress: " + PlotingData[0].Stress.ToString("F3") + "; " + PlotingData[0].HKLAssociation;

                    Tmp.LineStyle = OxyPlot.LineStyle.Solid;

                    Tmp.StrokeThickness = CalScec.Properties.Settings.Default.MainPlotLineThickness;
                    Tmp.MarkerSize = CalScec.Properties.Settings.Default.MainPlotDotThickness;

                    switch (n % 6)
                    {
                        case 0:
                            Tmp.MarkerType = OxyPlot.MarkerType.Circle;
                            break;
                        case 1:
                            Tmp.MarkerType = OxyPlot.MarkerType.Cross;
                            break;
                        case 2:
                            Tmp.MarkerType = OxyPlot.MarkerType.Diamond;
                            break;
                        case 3:
                            Tmp.MarkerType = OxyPlot.MarkerType.Plus;
                            break;
                        case 4:
                            Tmp.MarkerType = OxyPlot.MarkerType.Square;
                            break;
                        case 5:
                            Tmp.MarkerType = OxyPlot.MarkerType.Star;
                            break;
                        default:
                            Tmp.MarkerType = OxyPlot.MarkerType.Triangle;
                            break;
                    }

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
                    Tmp.Color = LineColor;
                    Tmp.MarkerStroke = LineColor;

                    for (int i = 0; i < PlotingData.Count; i++)
                    {
                        double SmallestStress = Double.MaxValue;
                        double D0 = 100.0;
                        for (int j = 0; j < AllStressData.Count; j++)
                        {
                            if(AllStressData[j].HKLAssociation == PlotingData[i].HKLAssociation && Math.Abs(AllStressData[j].PsiAngle - PlotingData[i].PsiAngle) < CalScec.Properties.Settings.Default.PsyAcceptanceAngle && AllStressData[j].Stress < SmallestStress)
                            {
                                SmallestStress = AllStressData[j].Stress;
                                D0 = AllStressData[j].DPeak.LatticeDistance;
                            }
                        }

                        OxyPlot.DataPoint PlotPoint = new OxyPlot.DataPoint(Math.Pow(Math.Sin(PlotingData[i].PsiAngle * (Math.PI / 180.0)), 2), (PlotingData[i].DPeak.LatticeDistance - D0) / D0);
                        Tmp.Points.Add(PlotPoint);
                        
                        if ((PlotingData[i].DPeak.LatticeDistance - D0) / D0 > this.SinYAxisLin.Maximum)
                        {
                            this.SinYAxisLin.Maximum = (PlotingData[i].DPeak.LatticeDistance - D0) / D0;
                        }
                        if ((PlotingData[i].DPeak.LatticeDistance - D0) / D0 < this.SinYAxisLin.Minimum)
                        {
                            this.SinYAxisLin.Minimum = (PlotingData[i].DPeak.LatticeDistance - D0) / D0;
                        }
                    }

                    SinPlotModel.Series.Add(Tmp);
                }
            }

            this.SinXAxisLin.Title = "Sin²(Psi)";
            this.SinYAxisLin.Title = "Extension";
        }

        private void PlotCosExtension_Work(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            System.ComponentModel.BackgroundWorker worker = sender as System.ComponentModel.BackgroundWorker;
            
            System.Collections.IList Data = (System.Collections.IList)e.Argument;

            List<PeakStressAssociation> PlottedData = new List<PeakStressAssociation>();

            this.SinYAxisLin.Maximum = 0.0;
            this.SinYAxisLin.Minimum = 10;

            for (int n = 0; n < Data.Count; n++)
            {
                PeakStressAssociation ActDataSet = (PeakStressAssociation)Data[n];

                bool AlreadyPlotted = false;

                for (int i = 0; i < PlottedData.Count; i++)
                {
                    if (PlottedData[i].Stress == ActDataSet.Stress && PlottedData[i].HKLAssociation == ActDataSet.HKLAssociation)
                    {
                        AlreadyPlotted = true;
                        break;
                    }
                }

                if (!AlreadyPlotted)
                {
                    PlottedData.Add(ActDataSet);

                    List<PeakStressAssociation> PlotingData = new List<PeakStressAssociation>();

                    for (int i = 0; i < this.AllStressData.Count; i++)
                    {
                        if (this.AllStressData[i].Stress == ActDataSet.Stress && this.AllStressData[i].HKLAssociation == ActDataSet.HKLAssociation)
                        {
                            PlotingData.Add(this.AllStressData[i]);
                        }
                    }

                    int[] Blue = { 0, 255, 255, 0, 0, 255, 255 };
                    int[] Red = { 255, 0, 255, 0, 255, 0, 255 };
                    int[] Green = { 255, 255, 0, 255, 0, 0, 255 };

                    OxyPlot.Series.LineSeries Tmp = new OxyPlot.Series.LineSeries();
                    Tmp.Title = "Applied stress: " + PlotingData[0].Stress.ToString("F3") + "; " + PlotingData[0].HKLAssociation;

                    Tmp.LineStyle = OxyPlot.LineStyle.Solid;

                    Tmp.StrokeThickness = CalScec.Properties.Settings.Default.MainPlotLineThickness;
                    Tmp.MarkerSize = CalScec.Properties.Settings.Default.MainPlotDotThickness;

                    switch (n % 6)
                    {
                        case 0:
                            Tmp.MarkerType = OxyPlot.MarkerType.Circle;
                            break;
                        case 1:
                            Tmp.MarkerType = OxyPlot.MarkerType.Cross;
                            break;
                        case 2:
                            Tmp.MarkerType = OxyPlot.MarkerType.Diamond;
                            break;
                        case 3:
                            Tmp.MarkerType = OxyPlot.MarkerType.Plus;
                            break;
                        case 4:
                            Tmp.MarkerType = OxyPlot.MarkerType.Square;
                            break;
                        case 5:
                            Tmp.MarkerType = OxyPlot.MarkerType.Star;
                            break;
                        default:
                            Tmp.MarkerType = OxyPlot.MarkerType.Triangle;
                            break;
                    }

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
                    Tmp.Color = LineColor;
                    Tmp.MarkerStroke = LineColor;

                    for (int i = 0; i < PlotingData.Count; i++)
                    {
                        double SmallestStress = Double.MaxValue;
                        double D0 = 100.0;
                        for (int j = 0; j < AllStressData.Count; j++)
                        {
                            if (AllStressData[j].HKLAssociation == PlotingData[i].HKLAssociation && Math.Abs(AllStressData[j].PsiAngle - PlotingData[i].PsiAngle) < CalScec.Properties.Settings.Default.PsyAcceptanceAngle && AllStressData[j].Stress < SmallestStress)
                            {
                                SmallestStress = AllStressData[j].Stress;
                                D0 = AllStressData[j].DPeak.LatticeDistance;
                            }
                        }

                        OxyPlot.DataPoint PlotPoint = new OxyPlot.DataPoint(Math.Pow(Math.Cos(PlotingData[i].PsiAngle * (Math.PI / 180.0)), 2), (PlotingData[i].DPeak.LatticeDistance - D0) / D0);
                        Tmp.Points.Add(PlotPoint);

                        if ((PlotingData[i].DPeak.LatticeDistance - D0) / D0 > this.SinYAxisLin.Maximum)
                        {
                            this.SinYAxisLin.Maximum = (PlotingData[i].DPeak.LatticeDistance - D0) / D0;
                        }
                        if ((PlotingData[i].DPeak.LatticeDistance - D0) / D0 < this.SinYAxisLin.Minimum)
                        {
                            this.SinYAxisLin.Minimum = (PlotingData[i].DPeak.LatticeDistance - D0) / D0;
                        }
                    }

                    SinPlotModel.Series.Add(Tmp);
                }
            }

            this.SinXAxisLin.Title = "Sin²(Psi)";
            this.SinYAxisLin.Title = "Extension";
        }
        
        private void Plot_Completed(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            this.SinPsyPlot.Model.ResetAllAxes();
            this.SinPsyPlot.Model.InvalidatePlot(true);
        }

        #region Menu

        private void SinItem_Click(object sender, RoutedEventArgs e)
        {
            if (this.ReflexView.SelectedIndex != -1)
            {
                this.SinPsyPlot.Model.Series.Clear();
                System.ComponentModel.BackgroundWorker worker = new System.ComponentModel.BackgroundWorker();
                object ForPlot = this.ReflexView.SelectedItems;

                if (!this.SinItem.IsChecked)
                {
                    this.SinItem.IsChecked = false;
                    this.CosItem.IsChecked = true;

                    if(this.LatticeDistanceItem.IsChecked)
                    {
                        worker.DoWork += PlotCosLatticeDistance_Work;
                    }
                    else
                    {
                        worker.DoWork += PlotCosExtension_Work;
                    }
                }
                else
                {
                    this.SinItem.IsChecked = true;
                    this.CosItem.IsChecked = false;

                    if (this.LatticeDistanceItem.IsChecked)
                    {
                        worker.DoWork += PlotSinLatticeDistance_Work;
                    }
                    else
                    {
                        worker.DoWork += PlotSinExtension_Work;
                    }
                }

                worker.RunWorkerCompleted += Plot_Completed;
                worker.RunWorkerAsync(ForPlot);
            }
            else
            {
                if(this.SinItem.IsChecked)
                {
                    this.SinItem.IsChecked = false;
                    this.CosItem.IsChecked = true;
                }
                else
                {
                    this.SinItem.IsChecked = true;
                    this.CosItem.IsChecked = false;
                }
            }
        }

        private void CosItem_Click(object sender, RoutedEventArgs e)
        {
            if (this.ReflexView.SelectedIndex != -1)
            {
                this.SinPsyPlot.Model.Series.Clear();
                System.ComponentModel.BackgroundWorker worker = new System.ComponentModel.BackgroundWorker();
                object ForPlot = this.ReflexView.SelectedItems;

                if (!this.CosItem.IsChecked)
                {
                    this.SinItem.IsChecked = true;
                    this.CosItem.IsChecked = false;

                    if (this.LatticeDistanceItem.IsChecked)
                    {
                        worker.DoWork += PlotSinLatticeDistance_Work;
                    }
                    else
                    {
                        worker.DoWork += PlotSinExtension_Work;
                    }
                }
                else
                {
                    this.SinItem.IsChecked = false;
                    this.CosItem.IsChecked = true;

                    if (this.LatticeDistanceItem.IsChecked)
                    {
                        worker.DoWork += PlotCosLatticeDistance_Work;
                    }
                    else
                    {
                        worker.DoWork += PlotCosExtension_Work;
                    }
                }

                worker.RunWorkerCompleted += Plot_Completed;
                worker.RunWorkerAsync(ForPlot);
            }
            else
            {
                if (this.CosItem.IsChecked)
                {
                    this.SinItem.IsChecked = true;
                    this.CosItem.IsChecked = false;
                }
                else
                {
                    this.SinItem.IsChecked = false;
                    this.CosItem.IsChecked = true;
                }
            }
        }

        private void LatticeDistanceItem_Click(object sender, RoutedEventArgs e)
        {
            if (this.ReflexView.SelectedIndex != -1)
            {
                this.SinPsyPlot.Model.Series.Clear();
                System.ComponentModel.BackgroundWorker worker = new System.ComponentModel.BackgroundWorker();
                object ForPlot = this.ReflexView.SelectedItems;

                if (!this.LatticeDistanceItem.IsChecked)
                {
                    this.ExtensionItem.IsChecked = true;
                    this.LatticeDistanceItem.IsChecked = false;

                    if (this.SinItem.IsChecked)
                    {
                        worker.DoWork += PlotSinExtension_Work;
                    }
                    else
                    {
                        worker.DoWork += PlotCosExtension_Work;
                    }
                }
                else
                {
                    this.ExtensionItem.IsChecked = false;
                    this.LatticeDistanceItem.IsChecked = true;

                    if (this.SinItem.IsChecked)
                    {
                        worker.DoWork += PlotSinLatticeDistance_Work;
                    }
                    else
                    {
                        worker.DoWork += PlotCosLatticeDistance_Work;
                    }
                }

                worker.RunWorkerCompleted += Plot_Completed;
                worker.RunWorkerAsync(ForPlot);
            }
            else
            {
                if (this.LatticeDistanceItem.IsChecked)
                {
                    this.ExtensionItem.IsChecked = true;
                    this.LatticeDistanceItem.IsChecked = false;
                }
                else
                {
                    this.ExtensionItem.IsChecked = false;
                    this.LatticeDistanceItem.IsChecked = true;
                }
            }
        }

        private void ExtensionItem_Click(object sender, RoutedEventArgs e)
        {
            if (this.ReflexView.SelectedIndex != -1)
            {
                this.SinPsyPlot.Model.Series.Clear();
                System.ComponentModel.BackgroundWorker worker = new System.ComponentModel.BackgroundWorker();
                object ForPlot = this.ReflexView.SelectedItems;

                if (!this.ExtensionItem.IsChecked)
                {
                    this.ExtensionItem.IsChecked = false;
                    this.LatticeDistanceItem.IsChecked = true;

                    if (this.SinItem.IsChecked)
                    {
                        worker.DoWork += PlotSinLatticeDistance_Work;
                    }
                    else
                    {
                        worker.DoWork += PlotCosLatticeDistance_Work;
                    }
                }
                else
                {
                    this.ExtensionItem.IsChecked = true;
                    this.LatticeDistanceItem.IsChecked = false;

                    if (this.SinItem.IsChecked)
                    {
                        worker.DoWork += PlotSinExtension_Work;
                    }
                    else
                    {
                        worker.DoWork += PlotCosExtension_Work;
                    }
                }

                worker.RunWorkerCompleted += Plot_Completed;
                worker.RunWorkerAsync(ForPlot);
            }
            else
            {
                if (!this.ExtensionItem.IsChecked)
                {
                    this.ExtensionItem.IsChecked = false;
                    this.LatticeDistanceItem.IsChecked = true;
                }
                else
                {
                    this.ExtensionItem.IsChecked = true;
                    this.LatticeDistanceItem.IsChecked = false;
                }
            }
        }

        #endregion
    }
}
