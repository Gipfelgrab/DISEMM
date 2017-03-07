///////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////Im Gedenken an Tepi//////////////////////////////////////
//////////////////////Das Leben ist wie eine Reise in totaler Dunkelheit://////////////////////
/////Man weiß wie wo der nächste Schritt hinführt, aber jeder findet irgendwann das Licht//////
///////////////////////////////////////////////////////////////////////////////////////////////

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

namespace CalScec.Analysis.Fitting
{
    /// <summary>
    /// Interaktionslogik für PeakFittingWindow.xaml
    /// </summary>
    public partial class PeakFittingWindow : Window
    {
        Sample ActSample = new Sample();

        List<Peaks.Functions.PeakRegionFunction> AllPeakRegions = new List<Peaks.Functions.PeakRegionFunction>();
        List<Peaks.Functions.PeakRegionFunction> StandByPeakRegions = new List<Peaks.Functions.PeakRegionFunction>();
        List<Peaks.Functions.PeakRegionFunction> FittingPeakRegions = new List<Peaks.Functions.PeakRegionFunction>();
        
        private bool EditAktive = true;
        public bool PreventClosing = true;

        private int _regionManipulationIndex = 0;
        private List<OxyPlot.Annotations.Annotation> _regionManipulationLineCount = new List<OxyPlot.Annotations.Annotation>();
        private double[] _regionManipulationValues = { 0.0, 0.0 };

        #region Plotting

        public OxyPlot.PlotModel FittingPlotModel = new OxyPlot.PlotModel();
        OxyPlot.Axes.LinearAxis FittingXAxisLin = new OxyPlot.Axes.LinearAxis();
        OxyPlot.Axes.LinearAxis FittingYAxisLin = new OxyPlot.Axes.LinearAxis();
        OxyPlot.Axes.LogarithmicAxis FittingYAxisLog = new OxyPlot.Axes.LogarithmicAxis();

        public OxyPlot.PlotModel ResidualPlotModel = new OxyPlot.PlotModel();
        OxyPlot.Axes.LinearAxis ResidualXAxisLin = new OxyPlot.Axes.LinearAxis();
        OxyPlot.Axes.LinearAxis ResidualYAxisLin = new OxyPlot.Axes.LinearAxis();

        #endregion

        int TotalPeaks = 0;
        int ActualFittingPeaks = 0;

        public PeakFittingWindow()
        {
            InitializeComponent();

            this.SetWindowProperties();

            this.SetFittingPlot();
            this.SetResidualPlot();

            SetDataBindings();
        }

        public PeakFittingWindow(Analysis.Sample S)
        {
            InitializeComponent();

            for (int n = 0; n < S.DiffractionPatterns.Count; n++ )
            {
                for(int i = 0; i < S.DiffractionPatterns[n].PeakRegions.Count; i++)
                {
                    this.AddRegion(S.DiffractionPatterns[n].PeakRegions[i]);
                }
            }

            this.ActSample = S;

            this.PreventClosing = false;

            this.SetWindowProperties();

            this.SetFittingPlot();
            this.SetResidualPlot();
            
            SetDataBindings();
        }

        #region Window positioning and saving

        bool MaxToNormal = false;

        private void SetWindowProperties()
        {
            this.MaxToNormal = true;

            this.StateChanged += FitWindow_StateChanged;
            this.LocationChanged += FitWindow_LocationChanged;
            this.SizeChanged += FitWindow_SizeChanged;


            if (CalScec.Properties.Settings.Default.FitWindowIsMaximized)
            {
                this.WindowState = System.Windows.WindowState.Maximized;
            }
            else
            {
                this.Width = CalScec.Properties.Settings.Default.FitWindowWidth;
                this.Height = CalScec.Properties.Settings.Default.FitWindowHeight;
                this.Left = CalScec.Properties.Settings.Default.FitWindowLocationX;
                this.Top = CalScec.Properties.Settings.Default.FitWindowLocationY;
            }

            this.MaxToNormal = false;
        }

        void FitWindow_StateChanged(object sender, EventArgs e)
        {
            this.MaxToNormal = true;

            if (this.WindowState == WindowState.Maximized)
            {
                CalScec.Properties.Settings.Default.FitWindowIsMaximized = true;
            }
            else if (this.WindowState == WindowState.Normal)
            {
                CalScec.Properties.Settings.Default.FitWindowIsMaximized = false;
                this.Width = CalScec.Properties.Settings.Default.FitWindowWidth;
                this.Height = CalScec.Properties.Settings.Default.FitWindowHeight;
                this.Left = CalScec.Properties.Settings.Default.FitWindowLocationX;
                this.Top = CalScec.Properties.Settings.Default.FitWindowLocationY;
            }

            this.MaxToNormal = false;
        }

        void FitWindow_LocationChanged(object sender, EventArgs e)
        {
            if (!MaxToNormal)
            {
                CalScec.Properties.Settings.Default.FitWindowLocationY = this.Top;
                CalScec.Properties.Settings.Default.FitWindowLocationX = this.Left;
            }
        }

        void FitWindow_SizeChanged(object sender, EventArgs e)
        {
            if (!MaxToNormal)
            {
                CalScec.Properties.Settings.Default.FitWindowWidth = this.Width;
                CalScec.Properties.Settings.Default.FitWindowHeight = this.Height;
            }
        }

        #endregion

        private void SetDataBindings()
        {
            this.RegionView.ItemsSource = StandByPeakRegions;

            CollectionView RegionCollection = (CollectionView)CollectionViewSource.GetDefaultView(RegionView.ItemsSource);
            PropertyGroupDescription PeakGroupDescription = new PropertyGroupDescription("AssociatedPatternName");
            RegionCollection.GroupDescriptions.Add(PeakGroupDescription);
            this.FittingRegionView.ItemsSource = FittingPeakRegions;

            ComboBoxItem PeakGauss = new ComboBoxItem();
            ComboBoxItem PeakLorentz = new ComboBoxItem();
            ComboBoxItem PeakPseudoVoigt = new ComboBoxItem();

            PeakGauss.Content = "Gaussian function";
            PeakLorentz.Content = "Lorentzian function";
            PeakPseudoVoigt.Content = "Pseudo Voigt function";

            this.PeakFunctionBox.Items.Add(PeakGauss);
            this.PeakFunctionBox.Items.Add(PeakLorentz);
            this.PeakFunctionBox.Items.Add(PeakPseudoVoigt);

            FunctionPlotResolution.Text = CalScec.Properties.Settings.Default.FittingFunctionResolution.ToString("F3");

            this.ReflexAutoCorrection.IsChecked = CalScec.Properties.Settings.Default.ReflexFitAutoCorrection;
        }

        private void ResetGroupDescriptions()
        {
            CollectionView RegionCollection = (CollectionView)CollectionViewSource.GetDefaultView(RegionView.ItemsSource);
            PropertyGroupDescription PeakGroupDescription = new PropertyGroupDescription("AssociatedPatternName");
            RegionCollection.GroupDescriptions.Clear();
            RegionCollection.GroupDescriptions.Add(PeakGroupDescription);
        }

        private void SetFittingPlot()
        {
            FittingPlotModel.LegendBorder = OxyPlot.OxyColors.Black;
            FittingPlotModel.LegendItemAlignment = OxyPlot.HorizontalAlignment.Left;
            FittingPlotModel.LegendTitle = "";

            FittingXAxisLin.Position = OxyPlot.Axes.AxisPosition.Bottom;
            FittingXAxisLin.Minimum = 0;
            FittingXAxisLin.Maximum = 180;
            FittingXAxisLin.Title = "Angle";

            FittingYAxisLin.Position = OxyPlot.Axes.AxisPosition.Left;
            FittingYAxisLin.Minimum = 0;
            FittingYAxisLin.Maximum = 100;
            FittingYAxisLin.Title = "Intensity";

            FittingYAxisLog.Position = OxyPlot.Axes.AxisPosition.Left;
            FittingYAxisLog.Minimum = 0;
            FittingYAxisLog.Maximum = 100;
            FittingYAxisLog.Title = "Intensity";

            #region GridStyles

            switch (CalScec.Properties.Settings.Default.FittingPlotMajorGridStyle)
            {
                case 0:
                    FittingXAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.None;
                    FittingYAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.None;
                    FittingYAxisLog.MajorGridlineStyle = OxyPlot.LineStyle.None;
                    this.FittingMajorGridStyleNone.IsChecked = true;
                    this.FittingMajorGridStyleDot.IsChecked = false;
                    this.FittingMajorGridStyleDash.IsChecked = false;
                    break;
                case 1:
                    FittingXAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dash;
                    FittingYAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dash;
                    FittingYAxisLog.MajorGridlineStyle = OxyPlot.LineStyle.Dash;
                    this.FittingMajorGridStyleNone.IsChecked = false;
                    this.FittingMajorGridStyleDot.IsChecked = false;
                    this.FittingMajorGridStyleDash.IsChecked = true;
                    break;
                case 2:
                    FittingXAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dot;
                    FittingYAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dot;
                    FittingYAxisLog.MajorGridlineStyle = OxyPlot.LineStyle.Dot;
                    this.FittingMajorGridStyleNone.IsChecked = false;
                    this.FittingMajorGridStyleDot.IsChecked = true;
                    this.FittingMajorGridStyleDash.IsChecked = false;
                    break;
                default:
                    FittingXAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dot;
                    FittingYAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dot;
                    FittingYAxisLog.MajorGridlineStyle = OxyPlot.LineStyle.Dot;
                    this.FittingMajorGridStyleNone.IsChecked = false;
                    this.FittingMajorGridStyleDot.IsChecked = true;
                    this.FittingMajorGridStyleDash.IsChecked = false;
                    break;
            }

            switch (CalScec.Properties.Settings.Default.FittingPlotMinorGridStyle)
            {
                case 0:
                    FittingXAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.None;
                    FittingYAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.None;
                    FittingYAxisLog.MinorGridlineStyle = OxyPlot.LineStyle.None;
                    this.FittingMinorGridStyleNone.IsChecked = true;
                    this.FittingMinorGridStyleDot.IsChecked = false;
                    this.FittingMinorGridStyleDash.IsChecked = false;
                    break;
                case 1:
                    FittingXAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dash;
                    FittingYAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dash;
                    FittingYAxisLog.MinorGridlineStyle = OxyPlot.LineStyle.Dash;
                    this.FittingMinorGridStyleNone.IsChecked = false;
                    this.FittingMinorGridStyleDot.IsChecked = false;
                    this.FittingMinorGridStyleDash.IsChecked = true;
                    break;
                case 2:
                    FittingXAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dot;
                    FittingYAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dot;
                    FittingYAxisLog.MinorGridlineStyle = OxyPlot.LineStyle.Dot;
                    this.FittingMinorGridStyleNone.IsChecked = false;
                    this.FittingMinorGridStyleDot.IsChecked = true;
                    this.FittingMinorGridStyleDash.IsChecked = false;
                    break;
                default:
                    FittingXAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.None;
                    FittingYAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.None;
                    FittingYAxisLog.MinorGridlineStyle = OxyPlot.LineStyle.None;
                    this.FittingMinorGridStyleNone.IsChecked = true;
                    this.FittingMinorGridStyleDot.IsChecked = false;
                    this.FittingMinorGridStyleDash.IsChecked = false;
                    break;
            }

            #endregion

            FittingPlotModel.Axes.Add(FittingXAxisLin);

            if (CalScec.Properties.Settings.Default.PlotYFittingAxes == 0)
            {
                FittingPlotModel.Axes.Add(FittingYAxisLin);
                this.FittingPlotAxesToLog.IsChecked = false;
                this.FittingPlotAxesToLinear.IsChecked = true;
            }
            else if (CalScec.Properties.Settings.Default.PlotYFittingAxes == 1)
            {
                FittingPlotModel.Axes.Add(FittingYAxisLog);
                this.FittingPlotAxesToLog.IsChecked = true;
                this.FittingPlotAxesToLinear.IsChecked = false;
            }

            this.MainFitPlot.Model = FittingPlotModel;
            this.MainFitPlot.Model.ResetAllAxes();
            this.MainFitPlot.Model.InvalidatePlot(true);
        }

        private void SetResidualPlot()
        {
            ResidualXAxisLin.Position = OxyPlot.Axes.AxisPosition.Bottom;
            ResidualXAxisLin.Minimum = 0;
            ResidualXAxisLin.Maximum = 180;
            ResidualXAxisLin.Title = "Angle";

            ResidualYAxisLin.Position = OxyPlot.Axes.AxisPosition.Left;
            ResidualYAxisLin.Minimum = -3;
            ResidualYAxisLin.Maximum = 3;
            ResidualYAxisLin.Title = "Deviation";

            #region GridStyles

            switch (CalScec.Properties.Settings.Default.ResidualPlotMajorGridStyle)
            {
                case 0:
                    ResidualXAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.None;
                    ResidualYAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.None;
                    this.ResidualMajorGridStyleNone.IsChecked = true;
                    this.ResidualMajorGridStyleDot.IsChecked = false;
                    this.ResidualMajorGridStyleDash.IsChecked = false;
                    break;
                case 1:
                    ResidualXAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dash;
                    ResidualYAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dash;
                    this.ResidualMajorGridStyleNone.IsChecked = false;
                    this.ResidualMajorGridStyleDot.IsChecked = false;
                    this.ResidualMajorGridStyleDash.IsChecked = true;
                    break;
                case 2:
                    ResidualXAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dot;
                    ResidualYAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dot;
                    this.ResidualMajorGridStyleNone.IsChecked = false;
                    this.ResidualMajorGridStyleDot.IsChecked = true;
                    this.ResidualMajorGridStyleDash.IsChecked = false;
                    break;
                default:
                    ResidualXAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dot;
                    ResidualYAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dot;
                    this.ResidualMajorGridStyleNone.IsChecked = false;
                    this.ResidualMajorGridStyleDot.IsChecked = true;
                    this.ResidualMajorGridStyleDash.IsChecked = false;
                    break;
            }

            switch (CalScec.Properties.Settings.Default.ResidualPlotMinorGridStyle)
            {
                case 0:
                    ResidualXAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.None;
                    ResidualYAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.None;
                    this.ResidualMinorGridStyleNone.IsChecked = true;
                    this.ResidualMinorGridStyleDot.IsChecked = false;
                    this.ResidualMinorGridStyleDash.IsChecked = false;
                    break;
                case 1:
                    ResidualXAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dash;
                    ResidualYAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dash;
                    this.ResidualMinorGridStyleNone.IsChecked = false;
                    this.ResidualMinorGridStyleDot.IsChecked = false;
                    this.ResidualMinorGridStyleDash.IsChecked = true;
                    break;
                case 2:
                    ResidualXAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dot;
                    ResidualYAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dot;
                    this.ResidualMinorGridStyleNone.IsChecked = false;
                    this.ResidualMinorGridStyleDot.IsChecked = true;
                    this.ResidualMinorGridStyleDash.IsChecked = false;
                    break;
                default:
                    ResidualXAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.None;
                    ResidualYAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.None;
                    this.ResidualMinorGridStyleNone.IsChecked = true;
                    this.ResidualMinorGridStyleDot.IsChecked = false;
                    this.ResidualMinorGridStyleDash.IsChecked = false;
                    break;
            }

            #endregion

            ResidualPlotModel.Axes.Add(ResidualXAxisLin);
            ResidualPlotModel.Axes.Add(ResidualYAxisLin);

            if(CalScec.Properties.Settings.Default.PlotFittingOneSigmaLine)
            {
                this.OneSigmaLine.IsChecked = true;

                var PositiveLine = new OxyPlot.Annotations.LineAnnotation();
                PositiveLine.Color = OxyPlot.OxyColors.DarkGreen;
                PositiveLine.ClipByXAxis = true;
                PositiveLine.Y = 1;
                PositiveLine.Type = OxyPlot.Annotations.LineAnnotationType.Horizontal;
                PositiveLine.StrokeThickness = 1.5;
                PositiveLine.Text = "1 Sigma";
                PositiveLine.ClipText = true;
                PositiveLine.LineStyle = OxyPlot.LineStyle.Solid;
                var NegativeLine = new OxyPlot.Annotations.LineAnnotation();
                NegativeLine.Color = OxyPlot.OxyColors.DarkGreen;
                NegativeLine.ClipByXAxis = true;
                NegativeLine.Y = -1;
                NegativeLine.Type = OxyPlot.Annotations.LineAnnotationType.Horizontal;
                NegativeLine.StrokeThickness = 1.5;
                NegativeLine.Text = "1 Sigma";
                NegativeLine.ClipText = true;
                NegativeLine.LineStyle = OxyPlot.LineStyle.Solid;

                this.ResidualPlotModel.Annotations.Add(PositiveLine);
                this.ResidualPlotModel.Annotations.Add(NegativeLine);
            }
            else
            {
                this.OneSigmaLine.IsChecked = false;
            }
            if (CalScec.Properties.Settings.Default.PlotFittingThreeSigmaLine)
            {
                this.ThreeSigmaLine.IsChecked = true;

                var PositiveLine = new OxyPlot.Annotations.LineAnnotation();
                PositiveLine.Color = OxyPlot.OxyColors.OrangeRed;
                PositiveLine.ClipByXAxis = true;
                PositiveLine.Y = 3;
                PositiveLine.Type = OxyPlot.Annotations.LineAnnotationType.Horizontal;
                PositiveLine.StrokeThickness = 1.5;
                PositiveLine.Text = "3 Sigma";
                PositiveLine.ClipText = true;
                PositiveLine.LineStyle = OxyPlot.LineStyle.Solid;
                var NegativeLine = new OxyPlot.Annotations.LineAnnotation();
                NegativeLine.Color = OxyPlot.OxyColors.OrangeRed;
                NegativeLine.ClipByXAxis = true;
                NegativeLine.Y = -3;
                NegativeLine.Type = OxyPlot.Annotations.LineAnnotationType.Horizontal;
                NegativeLine.StrokeThickness = 1.5;
                NegativeLine.Text = "3 Sigma";
                NegativeLine.ClipText = true;
                NegativeLine.LineStyle = OxyPlot.LineStyle.Solid;

                this.ResidualPlotModel.Annotations.Add(PositiveLine);
                this.ResidualPlotModel.Annotations.Add(NegativeLine);
            }
            else
            {
                this.ThreeSigmaLine.IsChecked = false;
            }

            this.MainResPlot.Model = ResidualPlotModel;
            this.MainResPlot.Model.ResetAllAxes();
            this.MainResPlot.Model.InvalidatePlot(true);
        }

        #region Multithreading Events

        public delegate void FitRegionUpdateDelegate(Peaks.Functions.PeakRegionFunction PRF);
        public delegate void FitPeakUpdateDelegate(Peaks.Functions.PeakFunction PRF);

        private void RegionFitStarted(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            FitRegionUpdateDelegate FittingDelegate = RegionFitStartedHandler;

            Dispatcher.Invoke(FittingDelegate, sender as Peaks.Functions.PeakRegionFunction);
        }

        private void RegionFitFinished(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            FitRegionUpdateDelegate FittingDelegate = RegionFitFinishedHandler;

            Dispatcher.Invoke(FittingDelegate, sender as Peaks.Functions.PeakRegionFunction);
        }

        private void PeakFitStarted(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            FitPeakUpdateDelegate FittingDelegate = PeakFitStartedHandler;

            Dispatcher.Invoke(FittingDelegate, sender as Peaks.Functions.PeakFunction);
        }

        private void PeakFitFinished(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            FitPeakUpdateDelegate FittingDelegate = PeakFitFinishedHandler;

            Dispatcher.Invoke(FittingDelegate, sender as Peaks.Functions.PeakFunction);
        }

        private void RegionFitStartedHandler(Peaks.Functions.PeakRegionFunction PRF)
        {
            this.StandByPeakRegions.Remove(PRF);
            this.FittingPeakRegions.Add(PRF);

            this.RegionView.Items.Refresh();
            //this.RefreshRegionView();
            this.FittingRegionView.Items.Refresh();

            if(this.FittingProgrssBar.IsIndeterminate)
            {
                FittingProgressText.Text = "Fitting " + Convert.ToString(this.FittingPeakRegions.Count) + " of " + Convert.ToString(this.AllPeakRegions.Count) + " regions and " + Convert.ToString(this.TotalPeaks) + " of " + Convert.ToString(this.ActualFittingPeaks) + " peaks";
            }
            else
            {
                this.FittingProgrssBar.IsIndeterminate = true;

                FittingProgressText.Text = "Fitting " + Convert.ToString(this.FittingPeakRegions.Count) + " of " + Convert.ToString(this.AllPeakRegions.Count) + " regions and " + Convert.ToString(this.TotalPeaks) + " of " + Convert.ToString(this.ActualFittingPeaks) + " peaks";
            }
        }

        private void RegionFitFinishedHandler(Peaks.Functions.PeakRegionFunction PRF)
        {
            this.FittingPeakRegions.Remove(PRF);
            this.StandByPeakRegions.Add(PRF);

            this.RegionView.Items.Refresh();
            //this.RefreshRegionView();
            this.FittingRegionView.Items.Refresh();

            FittingProgressText.Text = "Fitting " + Convert.ToString(this.FittingPeakRegions.Count) + " of " + Convert.ToString(this.AllPeakRegions.Count) + " regions and " + Convert.ToString(this.ActualFittingPeaks) + " of " + Convert.ToString(this.TotalPeaks) + " peaks";

            if (this.FittingPeakRegions.Count == 0 && this.ActualFittingPeaks == 0)
            {
                this.FittingProgrssBar.IsIndeterminate = false;
            }
        }

        private void PeakFitStartedHandler(Peaks.Functions.PeakFunction PF)
        {
            this.RegionView.Items.Refresh();
            //this.RefreshRegionView();
            this.FittingRegionView.Items.Refresh();

            if (this.FittingProgrssBar.IsIndeterminate)
            {
                FittingProgressText.Text = "Fitting " + Convert.ToString(this.FittingPeakRegions.Count) + " of " + Convert.ToString(this.AllPeakRegions.Count) + " regions and " + Convert.ToString(this.TotalPeaks) + " of " + Convert.ToString(this.ActualFittingPeaks) + " peaks";
            }
            else
            {
                this.FittingProgrssBar.IsIndeterminate = true;

                FittingProgressText.Text = "Fitting " + Convert.ToString(this.FittingPeakRegions.Count) + " of " + Convert.ToString(this.AllPeakRegions.Count) + " regions and " + Convert.ToString(this.TotalPeaks) + " of " + Convert.ToString(this.ActualFittingPeaks) + " peaks";
            }
        }

        private void PeakFitFinishedHandler(Peaks.Functions.PeakFunction PF)
        {
            this.RegionView.Items.Refresh();
            //this.RefreshRegionView();
            this.FittingRegionView.Items.Refresh();

            FittingProgressText.Text = "Fitting " + Convert.ToString(this.FittingPeakRegions.Count) + " of " + Convert.ToString(this.AllPeakRegions.Count) + " regions and " + Convert.ToString(this.ActualFittingPeaks) + " of " + Convert.ToString(this.TotalPeaks) + " peaks";

            if (this.FittingPeakRegions.Count == 0 && this.ActualFittingPeaks == 0)
            {
                this.FittingProgrssBar.IsIndeterminate = false;
            }
        }

        #endregion

        private void RegionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EditAktive = false;

            if(this.RegionView.SelectedIndex != -1)
            {
                Peaks.Functions.PeakRegionFunction PRF = (Peaks.Functions.PeakRegionFunction)this.RegionView.SelectedItem;
                this.PeakView.ItemsSource = PRF;

                this.AngleFitActive.IsChecked = PRF.FreeParameters[1];
                this.FWHMFitActive.IsChecked = PRF.FreeParameters[0];
                this.LorentzRatioFitActive.IsChecked = PRF.FreeParameters[2];
                this.IntensityFitActive.IsChecked = PRF.FreeParameters[3];
                this.ConstantBackgroundFitActive.IsChecked = PRF.FreeParameters[4];
                this.CenterBackgroundFitActive.IsChecked = PRF.FreeParameters[5];
                this.AclivityBackgroundFitActive.IsChecked = PRF.FreeParameters[6];

                this.ConstantBackgroundTextBox.Text = PRF.PolynomialBackgroundFunction.Constant.ToString("F3");
                this.CenterBackgroundTextBox.Text = PRF.PolynomialBackgroundFunction.Center.ToString("F3");
                this.AclivityBackgroundTextBox.Text = PRF.PolynomialBackgroundFunction.Aclivity.ToString("F3");

                System.ComponentModel.BackgroundWorker worker = new System.ComponentModel.BackgroundWorker();

                worker.DoWork += PlotFittingPatterns_Work;
                worker.RunWorkerCompleted += PlotFittingPatterns_Completed;

                worker.RunWorkerAsync(PRF);
            }
            else
            {
                this.PeakView.ItemsSource = null;
                this.FittingPlotModel.Series.Clear();
                this.ResidualPlotModel.Series.Clear();
            }

            EditAktive = true;
        }

        private void PlotFittingPatterns_Work(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            this.FittingPlotModel.Series.Clear();
            this.ResidualPlotModel.Series.Clear();

            Peaks.Functions.PeakRegionFunction PRF = e.Argument as Peaks.Functions.PeakRegionFunction;

            OxyPlot.Series.LineSeries PointSeries = new OxyPlot.Series.LineSeries();
            OxyPlot.Series.LineSeries ResidualSeries = new OxyPlot.Series.LineSeries();
            OxyPlot.Series.LineSeries FunctionSeries = new OxyPlot.Series.LineSeries();

            PointSeries.Title = "Data";
            ResidualSeries.Title = "Deviation";
            FunctionSeries.Title = "Fitted function";

            PointSeries.LineStyle = OxyPlot.LineStyle.None;
            ResidualSeries.LineStyle = OxyPlot.LineStyle.None;
            FunctionSeries.LineStyle = OxyPlot.LineStyle.Solid;

            PointSeries.StrokeThickness = 0.0;
            ResidualSeries.StrokeThickness = 0.0;
            FunctionSeries.StrokeThickness = 2.0;

            PointSeries.MarkerSize = 2.0;
            ResidualSeries.MarkerSize = 2.0;
            FunctionSeries.MarkerSize = 0.0;

            PointSeries.MarkerType = OxyPlot.MarkerType.Circle;
            ResidualSeries.MarkerType = OxyPlot.MarkerType.Circle;
            FunctionSeries.MarkerType = OxyPlot.MarkerType.None;

            PointSeries.Color = OxyPlot.OxyColors.Black;
            ResidualSeries.Color = OxyPlot.OxyColors.Black;
            FunctionSeries.Color = OxyPlot.OxyColors.DarkBlue;
            PointSeries.MarkerStroke = OxyPlot.OxyColors.Black;
            ResidualSeries.MarkerStroke = OxyPlot.OxyColors.Black;
            FunctionSeries.MarkerStroke = OxyPlot.OxyColors.DarkBlue;

            double FittingYAxisMaximum = 0;
            double ResidualYAxisMaximum = 3.2;
            double ResidualYAxisMinimum = -3.2;
            for(int n = 0; n < PRF.FittingCounts.Count; n++)
            {
                PointSeries.Points.Add(new OxyPlot.DataPoint(PRF.FittingCounts[n][0], PRF.FittingCounts[n][1]));
                double DiffTmp = (PRF.FittingCounts[n][1] - PRF.Y(PRF.FittingCounts[n][0])) / PRF.FittingCounts[n][2];
                ResidualSeries.Points.Add(new OxyPlot.DataPoint(PRF.FittingCounts[n][0], DiffTmp));

                if(DiffTmp > ResidualYAxisMaximum)
                {
                    ResidualYAxisMaximum = DiffTmp;
                }
                if (DiffTmp < ResidualYAxisMinimum)
                {
                    ResidualYAxisMinimum = DiffTmp;
                }
            }

            for(double n = PRF.FittingCounts[0][0]; n < PRF.FittingCounts[PRF.FittingCounts.Count - 1][0]; n += CalScec.Properties.Settings.Default.FittingFunctionResolution)
            {
                double ValueTmp = PRF.Y(n);
                FunctionSeries.Points.Add(new OxyPlot.DataPoint(n, ValueTmp));

                if(ValueTmp > FittingYAxisMaximum)
                {
                    FittingYAxisMaximum = ValueTmp;
                }
            }

            this.FittingPlotModel.Series.Add(PointSeries);
            this.FittingPlotModel.Series.Add(FunctionSeries);
            this.ResidualPlotModel.Series.Add(ResidualSeries);

            this.FittingXAxisLin.Minimum = PRF.FittingCounts[0][0];
            this.ResidualXAxisLin.Minimum = PRF.FittingCounts[0][0];
            this.FittingXAxisLin.Maximum = PRF.FittingCounts[PRF.FittingCounts.Count - 1][0];
            this.ResidualXAxisLin.Maximum = PRF.FittingCounts[PRF.FittingCounts.Count - 1][0];
            
            this.FittingYAxisLin.Minimum = PRF.FittingCounts.GetMinimum();
            this.FittingYAxisLog.Minimum = PRF.FittingCounts.GetMinimum();
            this.ResidualYAxisLin.Minimum = ResidualYAxisMinimum;
            this.ResidualYAxisLin.Maximum = ResidualYAxisMaximum;

            double CountMaximum = PRF.FittingCounts.GetMaximum();
            if (CountMaximum > FittingYAxisMaximum)
            {
                this.FittingYAxisLin.Maximum = CountMaximum;
                this.FittingYAxisLog.Maximum = CountMaximum;
            }
            else
            {
                this.FittingYAxisLin.Maximum = FittingYAxisMaximum;
                this.FittingYAxisLog.Maximum = FittingYAxisMaximum;
            }
        }

        private void PlotFittingPatterns_Completed(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            this.MainFitPlot.Model.ResetAllAxes();
            this.MainFitPlot.Model.InvalidatePlot(true);
            this.MainResPlot.Model.ResetAllAxes();
            this.MainResPlot.Model.InvalidatePlot(true);
        }

        private void PeakView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(PeakView.SelectedIndex != -1)
            {
                EditAktive = false;

                Analysis.Peaks.Functions.PeakFunction PF = (Analysis.Peaks.Functions.PeakFunction)PeakView.SelectedItem;

                this.AngleTextBox.Text = PF.Angle.ToString("F3");
                this.FWHMTextBox.Text = PF.FWHM.ToString("F3");
                this.LorentzRatioTextBox.Text = PF.LorentzRatio.ToString("F3");
                this.IntensityTextBox.Text = PF.Intensity.ToString("F3");
                this.ConstantBackgroundTextBox.Text = PF.ConstantBackground.ToString("F3");
                this.CenterBackgroundTextBox.Text = PF.CenterBackground.ToString("F3");
                this.AclivityBackgroundTextBox.Text = PF.AclivityBackground.ToString("F3");

                this.AngleConstraintTextBox.Text = PF.ParameterConstraints.AngleConstraint.ToString("F3");
                this.FWHMConstraintTextBox.Text = PF.ParameterConstraints.SigmaConstraint.ToString("F3");
                this.LorentzRatioConstraintTextBox.Text = PF.ParameterConstraints.LorentzRatioConstraint.ToString("F3");

                this.AngleConstraintActive.IsChecked = PF.ParameterConstraints.AngleConstraintActiv;
                this.FWHMConstraintActive.IsChecked = PF.ParameterConstraints.SigmaConstraintActiv;
                this.LorentzRatioConstraintActive.IsChecked = PF.ParameterConstraints.LorentzRatioConstraintActiv;

                this.AngleFitActive.IsChecked = PF.FreeParameters[1];
                this.FWHMFitActive.IsChecked = PF.FreeParameters[0];
                this.LorentzRatioFitActive.IsChecked = PF.FreeParameters[2];
                this.IntensityFitActive.IsChecked = PF.FreeParameters[3];
                this.ConstantBackgroundFitActive.IsChecked = PF.FreeParameters[4];
                this.CenterBackgroundFitActive.IsChecked = PF.FreeParameters[5];
                this.AclivityBackgroundFitActive.IsChecked = PF.FreeParameters[6];

                this.PeakFunctionBox.SelectedIndex = PF.functionType;

                EditAktive = true;
            }
            else
            {
                this.AngleTextBox.Text = "";
                this.FWHMTextBox.Text = "";
                this.LorentzRatioTextBox.Text = "";
                this.IntensityTextBox.Text = "";
                this.ConstantBackgroundTextBox.Text = "";
                this.CenterBackgroundTextBox.Text = "";
                this.AclivityBackgroundTextBox.Text = "";

                this.AngleConstraintTextBox.Text = "";
                this.FWHMConstraintTextBox.Text = "";
                this.LorentzRatioConstraintTextBox.Text = "";

                this.AngleConstraintActive.IsChecked = false;
                this.FWHMConstraintActive.IsChecked = false;
                this.LorentzRatioConstraintActive.IsChecked = false;

                this.AngleFitActive.IsChecked = false;
                this.FWHMFitActive.IsChecked = false;
                this.LorentzRatioFitActive.IsChecked = false;
                this.IntensityFitActive.IsChecked = false;
                this.ConstantBackgroundFitActive.IsChecked = false;
                this.CenterBackgroundFitActive.IsChecked = false;
                this.AclivityBackgroundFitActive.IsChecked = false;

                this.PeakFunctionBox.SelectedIndex = 0;
            }
        }

        #region Peak Fitting Settings

        private void AngleTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (PeakView.SelectedIndex != -1)
            {
                if (EditAktive)
                {
                    try
                    {
                        double NewAngle = Convert.ToDouble(AngleTextBox.Text);

                        Analysis.Peaks.Functions.PeakFunction PF = (Analysis.Peaks.Functions.PeakFunction)PeakView.SelectedItem;

                        PF.Angle = NewAngle;

                        PeakView.Items.Refresh();
                    }
                    catch
                    {

                    }

                    if (RegionView.SelectedIndex != -1)
                    {
                        Analysis.Peaks.Functions.PeakRegionFunction PRF = (Analysis.Peaks.Functions.PeakRegionFunction)RegionView.SelectedItem;

                        System.ComponentModel.BackgroundWorker worker = new System.ComponentModel.BackgroundWorker();

                        worker.DoWork += PlotFittingPatterns_Work;
                        worker.RunWorkerCompleted += PlotFittingPatterns_Completed;

                        worker.RunWorkerAsync(PRF);
                    }
                }
            }
        }

        private void FWHMTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (PeakView.SelectedIndex != -1)
            {
                if (EditAktive)
                {
                    try
                    {
                        double NewFWHM = Convert.ToDouble(FWHMTextBox.Text);

                        Analysis.Peaks.Functions.PeakFunction PF = (Analysis.Peaks.Functions.PeakFunction)PeakView.SelectedItem;

                        PF.FWHM = NewFWHM;

                        PeakView.Items.Refresh();
                    }
                    catch
                    {

                    }

                    if (RegionView.SelectedIndex != -1)
                    {
                        Analysis.Peaks.Functions.PeakRegionFunction PRF = (Analysis.Peaks.Functions.PeakRegionFunction)RegionView.SelectedItem;

                        System.ComponentModel.BackgroundWorker worker = new System.ComponentModel.BackgroundWorker();

                        worker.DoWork += PlotFittingPatterns_Work;
                        worker.RunWorkerCompleted += PlotFittingPatterns_Completed;

                        worker.RunWorkerAsync(PRF);
                    }
                }
            }
        }

        private void LorentzRatioTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (PeakView.SelectedIndex != -1)
            {
                if (EditAktive)
                {
                    try
                    {
                        double NewLorentzRatio = Convert.ToDouble(LorentzRatioTextBox.Text);

                        Analysis.Peaks.Functions.PeakFunction PF = (Analysis.Peaks.Functions.PeakFunction)PeakView.SelectedItem;

                        PF.LorentzRatio = NewLorentzRatio;

                        PeakView.Items.Refresh();
                    }
                    catch
                    {

                    }

                    if (RegionView.SelectedIndex != -1)
                    {
                        Analysis.Peaks.Functions.PeakRegionFunction PRF = (Analysis.Peaks.Functions.PeakRegionFunction)RegionView.SelectedItem;

                        System.ComponentModel.BackgroundWorker worker = new System.ComponentModel.BackgroundWorker();

                        worker.DoWork += PlotFittingPatterns_Work;
                        worker.RunWorkerCompleted += PlotFittingPatterns_Completed;

                        worker.RunWorkerAsync(PRF);
                    }
                }
            }
        }

        private void AngleConstraintTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (PeakView.SelectedIndex != -1)
            {
                if (EditAktive)
                {
                    try
                    {
                        double NewAngleConstraint = Convert.ToDouble(AngleConstraintTextBox.Text);

                        Analysis.Peaks.Functions.PeakFunction PF = (Analysis.Peaks.Functions.PeakFunction)PeakView.SelectedItem;

                        PF.ParameterConstraints.AngleConstraint = NewAngleConstraint;

                        PeakView.Items.Refresh();
                    }
                    catch
                    {

                    }

                    if (RegionView.SelectedIndex != -1)
                    {
                        Analysis.Peaks.Functions.PeakRegionFunction PRF = (Analysis.Peaks.Functions.PeakRegionFunction)RegionView.SelectedItem;

                        System.ComponentModel.BackgroundWorker worker = new System.ComponentModel.BackgroundWorker();

                        worker.DoWork += PlotFittingPatterns_Work;
                        worker.RunWorkerCompleted += PlotFittingPatterns_Completed;

                        worker.RunWorkerAsync(PRF);
                    }
                }
            }
        }

        private void FWHMConstraintTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (PeakView.SelectedIndex != -1)
            {
                if (EditAktive)
                {
                    try
                    {
                        double NewFWHMConstraint = Convert.ToDouble(FWHMConstraintTextBox.Text);

                        Analysis.Peaks.Functions.PeakFunction PF = (Analysis.Peaks.Functions.PeakFunction)PeakView.SelectedItem;

                        PF.ParameterConstraints.SigmaConstraint = NewFWHMConstraint;

                        PeakView.Items.Refresh();
                    }
                    catch
                    {

                    }

                    if (RegionView.SelectedIndex != -1)
                    {
                        Analysis.Peaks.Functions.PeakRegionFunction PRF = (Analysis.Peaks.Functions.PeakRegionFunction)RegionView.SelectedItem;

                        System.ComponentModel.BackgroundWorker worker = new System.ComponentModel.BackgroundWorker();

                        worker.DoWork += PlotFittingPatterns_Work;
                        worker.RunWorkerCompleted += PlotFittingPatterns_Completed;

                        worker.RunWorkerAsync(PRF);
                    }
                }
            }
        }

        private void LorentzRatioConstraintTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (PeakView.SelectedIndex != -1)
            {
                if (EditAktive)
                {
                    try
                    {
                        double NewLorentzRatioConstraint = Convert.ToDouble(LorentzRatioConstraintTextBox.Text);

                        Analysis.Peaks.Functions.PeakFunction PF = (Analysis.Peaks.Functions.PeakFunction)PeakView.SelectedItem;

                        PF.ParameterConstraints.LorentzRatioConstraint = NewLorentzRatioConstraint;

                        PeakView.Items.Refresh();
                    }
                    catch
                    {

                    }

                    if (RegionView.SelectedIndex != -1)
                    {
                        Analysis.Peaks.Functions.PeakRegionFunction PRF = (Analysis.Peaks.Functions.PeakRegionFunction)RegionView.SelectedItem;

                        System.ComponentModel.BackgroundWorker worker = new System.ComponentModel.BackgroundWorker();

                        worker.DoWork += PlotFittingPatterns_Work;
                        worker.RunWorkerCompleted += PlotFittingPatterns_Completed;

                        worker.RunWorkerAsync(PRF);
                    }
                }
            }
        }

        private void AngleConstraintActive_Checked(object sender, RoutedEventArgs e)
        {
            if (PeakView.SelectedIndex != -1)
            {
                if (EditAktive)
                {
                    Analysis.Peaks.Functions.PeakFunction PF = (Analysis.Peaks.Functions.PeakFunction)PeakView.SelectedItem;

                    PF.ParameterConstraints.AngleConstraintActiv = true;
                }
            }
        }

        private void AngleConstraintActive_Unchecked(object sender, RoutedEventArgs e)
        {
            if (PeakView.SelectedIndex != -1)
            {
                if (EditAktive)
                {
                    Analysis.Peaks.Functions.PeakFunction PF = (Analysis.Peaks.Functions.PeakFunction)PeakView.SelectedItem;

                    PF.ParameterConstraints.AngleConstraintActiv = false;
                }
            }
        }

        private void FWHMConstraintActive_Checked(object sender, RoutedEventArgs e)
        {
            if (PeakView.SelectedIndex != -1)
            {
                if (EditAktive)
                {
                    Analysis.Peaks.Functions.PeakFunction PF = (Analysis.Peaks.Functions.PeakFunction)PeakView.SelectedItem;

                    PF.ParameterConstraints.SigmaConstraintActiv = true;
                }
            }
        }

        private void FWHMConstraintActive_Unchecked(object sender, RoutedEventArgs e)
        {
            if (PeakView.SelectedIndex != -1)
            {
                if (EditAktive)
                {
                    Analysis.Peaks.Functions.PeakFunction PF = (Analysis.Peaks.Functions.PeakFunction)PeakView.SelectedItem;

                    PF.ParameterConstraints.SigmaConstraintActiv = false;
                }
            }
        }

        private void LorentzRatioConstraintActive_Checked(object sender, RoutedEventArgs e)
        {
            if (PeakView.SelectedIndex != -1)
            {
                if (EditAktive)
                {
                    Analysis.Peaks.Functions.PeakFunction PF = (Analysis.Peaks.Functions.PeakFunction)PeakView.SelectedItem;

                    PF.ParameterConstraints.LorentzRatioConstraintActiv = true;
                }
            }
        }

        private void LorentzRatioConstraintActive_Unchecked(object sender, RoutedEventArgs e)
        {
            if (PeakView.SelectedIndex != -1)
            {
                if (EditAktive)
                {
                    Analysis.Peaks.Functions.PeakFunction PF = (Analysis.Peaks.Functions.PeakFunction)PeakView.SelectedItem;

                    PF.ParameterConstraints.LorentzRatioConstraintActiv = false;
                }
            }
        }

        private void AngleFitActive_Checked(object sender, RoutedEventArgs e)
        {
            if (PeakView.SelectedIndex != -1)
            {
                if (EditAktive)
                {
                    Analysis.Peaks.Functions.PeakFunction PF = (Analysis.Peaks.Functions.PeakFunction)PeakView.SelectedItem;

                    PF.FreeParameters[1] = true;
                }
            }
            else
            {
                if (RegionView.SelectedIndex != -1)
                {
                    if (EditAktive)
                    {
                        Analysis.Peaks.Functions.PeakRegionFunction PRF = (Analysis.Peaks.Functions.PeakRegionFunction)RegionView.SelectedItem;

                        PRF.FreeParameters[1] = true;
                    }
                }
            }
        }

        private void AngleFitActive_Unchecked(object sender, RoutedEventArgs e)
        {
            if (PeakView.SelectedIndex != -1)
            {
                if (EditAktive)
                {
                    Analysis.Peaks.Functions.PeakFunction PF = (Analysis.Peaks.Functions.PeakFunction)PeakView.SelectedItem;

                    PF.FreeParameters[1] = false;
                }
            }
            else
            {
                if (RegionView.SelectedIndex != -1)
                {
                    if (EditAktive)
                    {
                        Analysis.Peaks.Functions.PeakRegionFunction PRF = (Analysis.Peaks.Functions.PeakRegionFunction)RegionView.SelectedItem;

                        PRF.FreeParameters[1] = false;
                    }
                }
            }
        }

        private void FWHMFitActive_Checked(object sender, RoutedEventArgs e)
        {
            if (PeakView.SelectedIndex != -1)
            {
                if (EditAktive)
                {
                    Analysis.Peaks.Functions.PeakFunction PF = (Analysis.Peaks.Functions.PeakFunction)PeakView.SelectedItem;

                    PF.FreeParameters[0] = true;
                }
            }
            else
            {
                if (RegionView.SelectedIndex != -1)
                {
                    if (EditAktive)
                    {
                        Analysis.Peaks.Functions.PeakRegionFunction PRF = (Analysis.Peaks.Functions.PeakRegionFunction)RegionView.SelectedItem;

                        PRF.FreeParameters[0] = true;
                    }
                }
            }
        }

        private void FWHMFitActive_Unchecked(object sender, RoutedEventArgs e)
        {
            if (PeakView.SelectedIndex != -1)
            {
                if (EditAktive)
                {
                    Analysis.Peaks.Functions.PeakFunction PF = (Analysis.Peaks.Functions.PeakFunction)PeakView.SelectedItem;

                    PF.FreeParameters[0] = false;
                }
            }
            else
            {
                if (RegionView.SelectedIndex != -1)
                {
                    if (EditAktive)
                    {
                        Analysis.Peaks.Functions.PeakRegionFunction PRF = (Analysis.Peaks.Functions.PeakRegionFunction)RegionView.SelectedItem;

                        PRF.FreeParameters[0] = false;
                    }
                }
            }
        }

        private void LorentzRatioFitActive_Checked(object sender, RoutedEventArgs e)
        {
            if (PeakView.SelectedIndex != -1)
            {
                if (EditAktive)
                {
                    Analysis.Peaks.Functions.PeakFunction PF = (Analysis.Peaks.Functions.PeakFunction)PeakView.SelectedItem;

                    PF.FreeParameters[2] = true;
                }
            }
            else
            {
                if (RegionView.SelectedIndex != -1)
                {
                    if (EditAktive)
                    {
                        Analysis.Peaks.Functions.PeakRegionFunction PRF = (Analysis.Peaks.Functions.PeakRegionFunction)RegionView.SelectedItem;

                        PRF.FreeParameters[2] = true;
                    }
                }
            }
        }

        private void LorentzRatioFitActive_Unchecked(object sender, RoutedEventArgs e)
        {
            if (PeakView.SelectedIndex != -1)
            {
                if (EditAktive)
                {
                    Analysis.Peaks.Functions.PeakFunction PF = (Analysis.Peaks.Functions.PeakFunction)PeakView.SelectedItem;

                    PF.FreeParameters[2] = false;
                }
            }
            else
            {
                if (RegionView.SelectedIndex != -1)
                {
                    if (EditAktive)
                    {
                        Analysis.Peaks.Functions.PeakRegionFunction PRF = (Analysis.Peaks.Functions.PeakRegionFunction)RegionView.SelectedItem;

                        PRF.FreeParameters[2] = false;
                    }
                }
            }
        }

        private void PeakFunctionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PeakView.SelectedIndex != -1)
            {
                if (EditAktive)
                {
                    Analysis.Peaks.Functions.PeakFunction PF = (Analysis.Peaks.Functions.PeakFunction)PeakView.SelectedItem;

                    PF.functionType = PeakFunctionBox.SelectedIndex;
                }
            }
        }

        private void IntensityTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (PeakView.SelectedIndex != -1)
            {
                if (EditAktive)
                {
                    try
                    {
                        double NewIntensity = Convert.ToDouble(IntensityTextBox.Text);

                        Analysis.Peaks.Functions.PeakFunction PF = (Analysis.Peaks.Functions.PeakFunction)PeakView.SelectedItem;

                        PF.Intensity = NewIntensity;

                        PeakView.Items.Refresh();
                    }
                    catch
                    {

                    }

                    if (RegionView.SelectedIndex != -1)
                    {
                        Analysis.Peaks.Functions.PeakRegionFunction PRF = (Analysis.Peaks.Functions.PeakRegionFunction)RegionView.SelectedItem;

                        System.ComponentModel.BackgroundWorker worker = new System.ComponentModel.BackgroundWorker();

                        worker.DoWork += PlotFittingPatterns_Work;
                        worker.RunWorkerCompleted += PlotFittingPatterns_Completed;

                        worker.RunWorkerAsync(PRF);
                    }
                }
            }
        }

        private void IntensityFitActive_Checked(object sender, RoutedEventArgs e)
        {
            if (PeakView.SelectedIndex != -1)
            {
                if (EditAktive)
                {
                    Analysis.Peaks.Functions.PeakFunction PF = (Analysis.Peaks.Functions.PeakFunction)PeakView.SelectedItem;

                    PF.FreeParameters[3] = true;
                }
            }
            else
            {
                if (RegionView.SelectedIndex != -1)
                {
                    if (EditAktive)
                    {
                        Analysis.Peaks.Functions.PeakRegionFunction PRF = (Analysis.Peaks.Functions.PeakRegionFunction)RegionView.SelectedItem;

                        PRF.FreeParameters[3] = true;
                    }
                }
            }
        }

        private void IntensityFitActive_Unchecked(object sender, RoutedEventArgs e)
        {
            if (PeakView.SelectedIndex != -1)
            {
                if (EditAktive)
                {
                    Analysis.Peaks.Functions.PeakFunction PF = (Analysis.Peaks.Functions.PeakFunction)PeakView.SelectedItem;

                    PF.FreeParameters[3] = false;
                }
            }
            else
            {
                if (RegionView.SelectedIndex != -1)
                {
                    if (EditAktive)
                    {
                        Analysis.Peaks.Functions.PeakRegionFunction PRF = (Analysis.Peaks.Functions.PeakRegionFunction)RegionView.SelectedItem;

                        PRF.FreeParameters[3] = false;
                    }
                }
            }
        }

        private void ConstantBackgroundTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (EditAktive)
            {
                if (PeakView.SelectedIndex != -1)
                {
                    try
                    {
                        double NewConstantBackground = Convert.ToDouble(ConstantBackgroundTextBox.Text);

                        Analysis.Peaks.Functions.PeakFunction PF = (Analysis.Peaks.Functions.PeakFunction)PeakView.SelectedItem;

                        PF.ConstantBackground = NewConstantBackground;

                        PeakView.Items.Refresh();
                    }
                    catch
                    {

                    }

                }
                else
                {
                    if (RegionView.SelectedIndex != -1)
                    {
                        try
                        {
                            double NewConstantBackground = Convert.ToDouble(ConstantBackgroundTextBox.Text);

                            Analysis.Peaks.Functions.PeakRegionFunction PRF = (Analysis.Peaks.Functions.PeakRegionFunction)RegionView.SelectedItem;

                            PRF.PolynomialBackgroundFunction.Constant = NewConstantBackground;

                            PeakView.Items.Refresh();
                        }
                        catch
                        {

                        }
                    }
                }

                if (RegionView.SelectedIndex != -1)
                {
                    Analysis.Peaks.Functions.PeakRegionFunction PRF = (Analysis.Peaks.Functions.PeakRegionFunction)RegionView.SelectedItem;

                    System.ComponentModel.BackgroundWorker worker = new System.ComponentModel.BackgroundWorker();

                    worker.DoWork += PlotFittingPatterns_Work;
                    worker.RunWorkerCompleted += PlotFittingPatterns_Completed;

                    worker.RunWorkerAsync(PRF);
                }
            }
        }

        private void CenterBackgroundTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (EditAktive)
            {
                if (PeakView.SelectedIndex != -1)
                {


                    try
                    {
                        double NewCenterBackground = Convert.ToDouble(CenterBackgroundTextBox.Text);

                        Analysis.Peaks.Functions.PeakFunction PF = (Analysis.Peaks.Functions.PeakFunction)PeakView.SelectedItem;

                        PF.CenterBackground = NewCenterBackground;

                        PeakView.Items.Refresh();
                    }
                    catch
                    {

                    }

                }
                else
                {
                    if (RegionView.SelectedIndex != -1)
                    {
                        try
                        {
                            double NewCenterBackground = Convert.ToDouble(CenterBackgroundTextBox.Text);

                            Analysis.Peaks.Functions.PeakRegionFunction PRF = (Analysis.Peaks.Functions.PeakRegionFunction)RegionView.SelectedItem;

                            PRF.PolynomialBackgroundFunction.Center = NewCenterBackground;

                            PeakView.Items.Refresh();
                        }
                        catch
                        {

                        }
                    }
                }

                if (RegionView.SelectedIndex != -1)
                {
                    Analysis.Peaks.Functions.PeakRegionFunction PRF = (Analysis.Peaks.Functions.PeakRegionFunction)RegionView.SelectedItem;

                    System.ComponentModel.BackgroundWorker worker = new System.ComponentModel.BackgroundWorker();

                    worker.DoWork += PlotFittingPatterns_Work;
                    worker.RunWorkerCompleted += PlotFittingPatterns_Completed;

                    worker.RunWorkerAsync(PRF);
                }
            }
        }

        private void AclivityBackgroundTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (EditAktive)
            {
                if (PeakView.SelectedIndex != -1)
                {


                    try
                    {
                        double NewAclivityBackground = Convert.ToDouble(AclivityBackgroundTextBox.Text);

                        Analysis.Peaks.Functions.PeakFunction PF = (Analysis.Peaks.Functions.PeakFunction)PeakView.SelectedItem;

                        PF.AclivityBackground = NewAclivityBackground;

                        PeakView.Items.Refresh();
                    }
                    catch
                    {

                    }

                }
                else
                {
                    if (RegionView.SelectedIndex != -1)
                    {
                        try
                        {
                            double NewAclivityBackground = Convert.ToDouble(AclivityBackgroundTextBox.Text);

                            Analysis.Peaks.Functions.PeakRegionFunction PRF = (Analysis.Peaks.Functions.PeakRegionFunction)RegionView.SelectedItem;

                            PRF.PolynomialBackgroundFunction.Aclivity = NewAclivityBackground;

                            PeakView.Items.Refresh();
                        }
                        catch
                        {

                        }
                    }
                }

                if (RegionView.SelectedIndex != -1)
                {
                    Analysis.Peaks.Functions.PeakRegionFunction PRF = (Analysis.Peaks.Functions.PeakRegionFunction)RegionView.SelectedItem;

                    System.ComponentModel.BackgroundWorker worker = new System.ComponentModel.BackgroundWorker();

                    worker.DoWork += PlotFittingPatterns_Work;
                    worker.RunWorkerCompleted += PlotFittingPatterns_Completed;

                    worker.RunWorkerAsync(PRF);
                }
            }
        }

        private void ConstantBackgroundFitActive_Checked(object sender, RoutedEventArgs e)
        {
            if (PeakView.SelectedIndex != -1)
            {
                if (EditAktive)
                {
                    Analysis.Peaks.Functions.PeakFunction PF = (Analysis.Peaks.Functions.PeakFunction)PeakView.SelectedItem;

                    PF.FreeParameters[4] = true;
                    PF.backgroundFit = true;
                }
            }
            else
            {
                if (RegionView.SelectedIndex != -1)
                {
                    if (EditAktive)
                    {
                        Analysis.Peaks.Functions.PeakRegionFunction PRF = (Analysis.Peaks.Functions.PeakRegionFunction)RegionView.SelectedItem;

                        PRF.FreeParameters[4] = true;
                        PRF.backgroundFit = true;
                    }
                }
            }
        }

        private void ConstantBackgroundFitActive_Unchecked(object sender, RoutedEventArgs e)
        {
            if (PeakView.SelectedIndex != -1)
            {
                if (EditAktive)
                {
                    Analysis.Peaks.Functions.PeakFunction PF = (Analysis.Peaks.Functions.PeakFunction)PeakView.SelectedItem;

                    PF.FreeParameters[4] = false;
                    if(!PF.FreeParameters[5] && !PF.FreeParameters[6])
                    {

                        PF.backgroundFit = false;
                    }
                }
            }
            else
            {
                if (RegionView.SelectedIndex != -1)
                {
                    if (EditAktive)
                    {
                        Analysis.Peaks.Functions.PeakRegionFunction PRF = (Analysis.Peaks.Functions.PeakRegionFunction)RegionView.SelectedItem;

                        PRF.FreeParameters[4] = false;
                        if (!PRF.FreeParameters[5] && !PRF.FreeParameters[6])
                        {

                            PRF.backgroundFit = false;
                        }
                    }
                }
            }
        }

        private void CenterBackgroundFitActive_Checked(object sender, RoutedEventArgs e)
        {
            if (PeakView.SelectedIndex != -1)
            {
                if (EditAktive)
                {
                    Analysis.Peaks.Functions.PeakFunction PF = (Analysis.Peaks.Functions.PeakFunction)PeakView.SelectedItem;

                    PF.FreeParameters[5] = true;
                    PF.backgroundFit = true;
                }
            }
            else
            {
                if (RegionView.SelectedIndex != -1)
                {
                    if (EditAktive)
                    {
                        Analysis.Peaks.Functions.PeakRegionFunction PRF = (Analysis.Peaks.Functions.PeakRegionFunction)RegionView.SelectedItem;

                        PRF.FreeParameters[5] = true;
                        PRF.backgroundFit = true;
                    }
                }
            }
        }

        private void CenterBackgroundFitActive_Unchecked(object sender, RoutedEventArgs e)
        {
            if (PeakView.SelectedIndex != -1)
            {
                if (EditAktive)
                {
                    Analysis.Peaks.Functions.PeakFunction PF = (Analysis.Peaks.Functions.PeakFunction)PeakView.SelectedItem;

                    PF.FreeParameters[5] = false;
                    if (!PF.FreeParameters[4] && !PF.FreeParameters[6])
                    {

                        PF.backgroundFit = false;
                    }
                }
            }
            else
            {
                if (RegionView.SelectedIndex != -1)
                {
                    if (EditAktive)
                    {
                        Analysis.Peaks.Functions.PeakRegionFunction PRF = (Analysis.Peaks.Functions.PeakRegionFunction)RegionView.SelectedItem;

                        PRF.FreeParameters[5] = false;
                        if (!PRF.FreeParameters[4] && !PRF.FreeParameters[6])
                        {

                            PRF.backgroundFit = false;
                        }
                    }
                }
            }
        }

        private void AclivityBackgroundFitActive_Checked(object sender, RoutedEventArgs e)
        {
            if (PeakView.SelectedIndex != -1)
            {
                if (EditAktive)
                {
                    Analysis.Peaks.Functions.PeakFunction PF = (Analysis.Peaks.Functions.PeakFunction)PeakView.SelectedItem;

                    PF.FreeParameters[6] = true;
                    PF.backgroundFit = true;
                }
            }
            else
            {
                if (RegionView.SelectedIndex != -1)
                {
                    if (EditAktive)
                    {
                        Analysis.Peaks.Functions.PeakRegionFunction PRF = (Analysis.Peaks.Functions.PeakRegionFunction)RegionView.SelectedItem;

                        PRF.FreeParameters[6] = true;
                        PRF.backgroundFit = true;
                    }
                }
            }
        }

        private void AclivityBackgroundFitActive_Unchecked(object sender, RoutedEventArgs e)
        {
            if (PeakView.SelectedIndex != -1)
            {
                if (EditAktive)
                {
                    Analysis.Peaks.Functions.PeakFunction PF = (Analysis.Peaks.Functions.PeakFunction)PeakView.SelectedItem;

                    PF.FreeParameters[6] = false;
                    if (!PF.FreeParameters[4] && !PF.FreeParameters[5])
                    {

                        PF.backgroundFit = false;
                    }
                }
            }
            else
            {
                if (RegionView.SelectedIndex != -1)
                {
                    if (EditAktive)
                    {
                        Analysis.Peaks.Functions.PeakRegionFunction PRF = (Analysis.Peaks.Functions.PeakRegionFunction)RegionView.SelectedItem;

                        PRF.FreeParameters[6] = false;
                        if (!PRF.FreeParameters[4] && !PRF.FreeParameters[5])
                        {

                            PRF.backgroundFit = false;
                        }
                    }
                }
            }
        }

        #endregion

        #region Menu

        private void RegionManipulation_Click(object sender, RoutedEventArgs e)
        {
            MenuItem SelectedMenuItem = sender as MenuItem;

            string Header = Convert.ToString(SelectedMenuItem.Header);
            _regionManipulationLineCount.Clear();

            switch (Header)
            {
                case "Split region":
                    if(_regionManipulationIndex == 1)
                    {
                        _regionManipulationIndex = 0;
                    }
                    else
                    {
                        _regionManipulationIndex = 1;
                        this.ReselectFittingDataMenu.IsChecked = false;
                        this.InsertPeakMenu.IsChecked = false;
                    }
                    break;
                case "Reselect fitting data":
                    if (_regionManipulationIndex == 2)
                    {
                        _regionManipulationIndex = 0;
                    }
                    else
                    {
                        _regionManipulationIndex = 2;
                        this.SplitRegionMenu.IsChecked = false;
                        this.InsertPeakMenu.IsChecked = false;
                    }
                    break;
                case "Insert peak":
                    if (_regionManipulationIndex == 3)
                    {
                        _regionManipulationIndex = 0;
                    }
                    else
                    {
                        _regionManipulationIndex = 3;
                        this.SplitRegionMenu.IsChecked = false;
                        this.ReselectFittingDataMenu.IsChecked = false;
                    }
                    break;
                case "Show cif data":
                    if (this.ShowCifDataRegion.IsChecked == true)
                    {
                        this.MainFitPlot.Model.Annotations.Clear();

                        for(int n = 0; n < this._regionManipulationLineCount.Count; n++)
                        {
                            this.MainFitPlot.Model.Annotations.Add(this._regionManipulationLineCount[n]);
                        }

                        this.MainFitPlot.Model.InvalidatePlot(true);
                    }
                    else
                    {
                        AddCifLines();
                    }
                    break;
                default:
                    _regionManipulationIndex = 0;
                    this.SplitRegionMenu.IsChecked = false;
                    this.ReselectFittingDataMenu.IsChecked = false;
                    this.InsertPeakMenu.IsChecked = false;
                    break;
            }
        }

        private void MainFitPlot_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(_regionManipulationIndex != 0 && this.MainFitPlot.Model.Series.Count != 0)
            {
                Point TMP = e.GetPosition(MainFitPlot);
                OxyPlot.ScreenPoint ScPoint = new OxyPlot.ScreenPoint(TMP.X, TMP.Y);

                OxyPlot.TrackerHitResult NearestHitResult = this.MainFitPlot.Model.Series[0].GetNearestPoint(ScPoint, false);
                OxyPlot.DataPoint NearestDataPoint = NearestHitResult.DataPoint;
                //x = (OxyPlot.Series.LineSeries).InverseTransform(e.Position).X;
                double SelectedXValue = NearestDataPoint.X;

                OxyPlot.Annotations.LineAnnotation NewLA = new OxyPlot.Annotations.LineAnnotation();

                NewLA.Type = OxyPlot.Annotations.LineAnnotationType.Vertical;
                NewLA.ClipByYAxis = true;
                NewLA.ClipText = true;
                NewLA.Color = OxyPlot.OxyColors.DarkBlue;
                
                NewLA.X = SelectedXValue;
                NewLA.StrokeThickness = CalScec.Properties.Settings.Default.PeakMarkingThickness;
                NewLA.LineStyle = OxyPlot.LineStyle.Dot;

                switch (_regionManipulationIndex)
                {
                    case 1:
                        _regionManipulationValues[0] = SelectedXValue;
                        NewLA.Text = "Region splitt line";
                        if (_regionManipulationLineCount.Count == 0)
                        {
                            _regionManipulationLineCount.Add(NewLA);
                            this.MainFitPlot.Model.Annotations.Add(NewLA);
                        }
                        else
                        {
                            _regionManipulationLineCount.Clear();
                            _regionManipulationLineCount.Add(NewLA);
                            this.MainFitPlot.Model.Annotations.Clear();
                            this.MainFitPlot.Model.Annotations.Add(NewLA);
                        }
                        break;
                    case 2:
                        if (_regionManipulationLineCount.Count == 0)
                        {
                            NewLA.Text = "New lower border";
                            _regionManipulationValues[0] = SelectedXValue;
                            _regionManipulationLineCount.Add(NewLA);
                            this.MainFitPlot.Model.Annotations.Add(NewLA);
                        }
                        else if(_regionManipulationLineCount.Count == 1)
                        {
                            NewLA.Text = "New upper border";
                            _regionManipulationValues[1] = SelectedXValue;
                            _regionManipulationLineCount.Add(NewLA);
                            this.MainFitPlot.Model.Annotations.Add(NewLA);
                        }
                        else
                        {
                            NewLA.Text = "New lower border";
                            _regionManipulationLineCount.Clear();
                            _regionManipulationValues[0] = SelectedXValue;
                            _regionManipulationLineCount.Add(NewLA);
                            this.MainFitPlot.Model.Annotations.Clear();
                            this.MainFitPlot.Model.Annotations.Add(NewLA);
                        }
                        break;
                    case 3:

                        _regionManipulationValues[0] = SelectedXValue;
                        _regionManipulationValues[1] = NearestDataPoint.Y;
                        NewLA.Text = "New peak position";
                        if (_regionManipulationLineCount.Count == 0)
                        {
                            _regionManipulationLineCount.Add(NewLA);
                            this.MainFitPlot.Model.Annotations.Add(NewLA);
                        }
                        else
                        {
                            _regionManipulationLineCount.Clear();
                            _regionManipulationLineCount.Add(NewLA);
                            this.MainFitPlot.Model.Annotations.Clear();
                            this.MainFitPlot.Model.Annotations.Add(NewLA);
                        }
                        break;
                    default:
                        break;
                }

                if(this.ShowCifDataRegion.IsChecked)
                {
                    AddCifLines();
                }

                this.MainFitPlot.Model.InvalidatePlot(true);
            }
        }

        private void AddCifLines()
        {
            if(this.RegionView.SelectedIndex != -1)
            {
                Peaks.Functions.PeakRegionFunction PRF = (Peaks.Functions.PeakRegionFunction)this.RegionView.SelectedItem;

                List<int[]> AnnotationColors = new List<int[]>();
                int[] AnnotationColor1 = { 18, 104, 18 };
                int[] AnnotationColor2 = { 14, 81, 14 };
                int[] AnnotationColor3 = { 22, 44, 87 };
                int[] AnnotationColor4 = { 41, 24, 89 };
                int[] AnnotationColor5 = { 62, 19, 87 };
                int[] AnnotationColor6 = { 104, 18, 67 };
                int[] AnnotationColor7 = { 130, 23, 23 };
                int[] AnnotationColor8 = { 130, 76, 9 };
                AnnotationColors.Add(AnnotationColor1);
                AnnotationColors.Add(AnnotationColor8);
                AnnotationColors.Add(AnnotationColor3);
                AnnotationColors.Add(AnnotationColor6);
                AnnotationColors.Add(AnnotationColor5);
                AnnotationColors.Add(AnnotationColor4);
                AnnotationColors.Add(AnnotationColor7);
                AnnotationColors.Add(AnnotationColor2);

                for (int n = 0; n < this.ActSample.CrystalData.Count; n++)
                {
                    for(int i = 0; i < this.ActSample.CrystalData[n].HKLList.Count; i++)
                    {
                        OxyPlot.Annotations.LineAnnotation NewLA = new OxyPlot.Annotations.LineAnnotation();

                        NewLA.Type = OxyPlot.Annotations.LineAnnotationType.Vertical;
                        NewLA.ClipByYAxis = true;
                        NewLA.ClipText = true;
                        NewLA.Color = OxyPlot.OxyColor.FromRgb(Convert.ToByte(AnnotationColors[n % 8][0]), Convert.ToByte(AnnotationColors[n % 8][1]), Convert.ToByte(AnnotationColors[n % 8][2]));
                        NewLA.Text = this.ActSample.CrystalData[n].SymmetryGroup + " (" + this.ActSample.CrystalData[n].HKLList[i].H + ", " + this.ActSample.CrystalData[n].HKLList[i].K + ", " + this.ActSample.CrystalData[n].HKLList[i].L + " )";
                        NewLA.X = this.ActSample.CrystalData[n].HKLList[i].EstimatedAngle;
                        NewLA.StrokeThickness = CalScec.Properties.Settings.Default.PeakMarkingThickness;

                        if (CalScec.Properties.Settings.Default.PeakMarkingStyle == 0)
                        {
                            NewLA.LineStyle = OxyPlot.LineStyle.Dot;
                        }
                        else if (CalScec.Properties.Settings.Default.PeakMarkingStyle == 1)
                        {
                            NewLA.LineStyle = OxyPlot.LineStyle.Dash;
                        }

                        if(this.ActSample.CrystalData[n].HKLList[i].EstimatedAngle < PRF.FittingCounts[PRF.FittingCounts.Count - 1][0] && this.ActSample.CrystalData[n].HKLList[i].EstimatedAngle > PRF.FittingCounts[0][0])
                        {
                            this.MainFitPlot.Model.Annotations.Add(NewLA);
                        }
                    }
                }

                this.MainFitPlot.Model.InvalidatePlot(true);
            }
        }

        private void DoRegionManipulationAction_Click(object sender, RoutedEventArgs e)
        {
            if (this.RegionView.SelectedIndex != -1)
            {
                Peaks.Functions.PeakRegionFunction PRF = (Peaks.Functions.PeakRegionFunction)this.RegionView.SelectedItem;

                for (int n = 0; n < this.ActSample.DiffractionPatterns.Count; n++)
                {
                    if(this.ActSample.DiffractionPatterns[n].Name == PRF.AssociatedPatternName)
                    {
                        switch (_regionManipulationIndex)
                        {
                            case 1:
                                if (_regionManipulationLineCount.Count == 1)
                                {
                                    Peaks.Functions.PeakRegionFunction FirstNewRegion = PRF.Clone() as Peaks.Functions.PeakRegionFunction;
                                    Peaks.Functions.PeakRegionFunction SecondNewRegion = PRF.Clone() as Peaks.Functions.PeakRegionFunction;
                                    FirstNewRegion.Clear();
                                    SecondNewRegion.Clear();
                                    for (int i = 0; i < PRF.Count; i++)
                                    {
                                        if (PRF[i].Angle < _regionManipulationValues[0])
                                        {
                                            FirstNewRegion.Add(PRF[i]);
                                        }
                                        else
                                        {
                                            SecondNewRegion.Add(PRF[i]);
                                        }
                                    }

                                    FirstNewRegion.FittingCounts.Clear();
                                    SecondNewRegion.FittingCounts.Clear();
                                    for (int i = 0; i < PRF.FittingCounts.Count; i++)
                                    {
                                        if (PRF.FittingCounts[i][0] < _regionManipulationValues[0])
                                        {
                                            FirstNewRegion.FittingCounts.Add(PRF.FittingCounts[i]);
                                        }
                                        else
                                        {
                                            SecondNewRegion.FittingCounts.Add(PRF.FittingCounts[i]);
                                        }
                                    }

                                    this.ActSample.DiffractionPatterns[n].PeakRegions.Remove(PRF);
                                    this.ActSample.DiffractionPatterns[n].PeakRegions.Add(FirstNewRegion);
                                    this.ActSample.DiffractionPatterns[n].PeakRegions.Add(SecondNewRegion);

                                    this.RemoveRegion(PRF);
                                    this.AddPresettedRegion(FirstNewRegion);
                                    this.AddPresettedRegion(SecondNewRegion);
                                }
                                break;
                            case 2:
                                if(_regionManipulationLineCount.Count == 2)
                                {
                                    Pattern.Counts NewFittingCounts = new Pattern.Counts();
                                    if(_regionManipulationValues[0] > _regionManipulationValues[1])
                                    {
                                        double Tmp = _regionManipulationValues[0];
                                        _regionManipulationValues[0] = _regionManipulationValues[1];
                                        _regionManipulationValues[1] = Tmp;
                                    }
                                    for(int i = 0; i < PRF.FittingCounts.Count; i++)
                                    {
                                        if(PRF.FittingCounts[i][0] > _regionManipulationValues[0] && PRF.FittingCounts[i][0] < _regionManipulationValues[1])
                                        {
                                            NewFittingCounts.Add(PRF.FittingCounts[i]);
                                        }
                                    }

                                    PRF.FittingCounts = NewFittingCounts.Clone() as Pattern.Counts;
                                }
                                break;
                            case 3:
                                if (_regionManipulationLineCount.Count == 1)
                                {
                                    double estimatedFWHM = Tools.Calculation.GetEstimatedFWHM(_regionManipulationValues[0]);

                                    Pattern.Counts NewFittingCounts = new Pattern.Counts();
                                    for (int i = 0; i < PRF.FittingCounts.Count; i++)
                                    {
                                        if (PRF.FittingCounts[i][0] > _regionManipulationValues[0] - estimatedFWHM && PRF.FittingCounts[i][0] < _regionManipulationValues[0] + estimatedFWHM)
                                        {
                                            NewFittingCounts.Add(PRF.FittingCounts[i]);
                                        }
                                    }

                                    Peaks.Functions.PeakFunction NewPeak = new Peaks.Functions.PeakFunction(_regionManipulationValues[0], _regionManipulationValues[1] - PRF.PolynomialBackgroundFunction.Y(_regionManipulationValues[0]), NewFittingCounts, new Peaks.Functions.BackgroundPolynomial(PRF.PolynomialBackgroundFunction.Center, PRF.PolynomialBackgroundFunction.Constant, PRF.PolynomialBackgroundFunction.Aclivity));
                                    NewPeak.functionType = 2;
                                    PRF.Add(NewPeak);

                                    PRF.Sort((a, b) => a.Angle.CompareTo(b.Angle));

                                    for(int i = 0; i < this.ActSample.DiffractionPatterns.Count; i++)
                                    {
                                        if(this.ActSample.DiffractionPatterns[i].Name == PRF.AssociatedPatternName)
                                        {
                                            Peaks.DiffractionPeak NewDP = new Peaks.DiffractionPeak(1, NewPeak.Angle, _regionManipulationValues[1] - PRF.PolynomialBackgroundFunction.Y(_regionManipulationValues[0]), PRF.PolynomialBackgroundFunction.Y(_regionManipulationValues[0]));
                                            NewDP.PFunction = NewPeak;
                                            NewDP.AssociatedPatternName = PRF.AssociatedPatternName;
                                            this.ActSample.DiffractionPatterns[i].FoundPeaks.Add(NewDP);
                                            this.ActSample.DiffractionPatterns[i].FoundPeaks.Sort((a, b) => a.Angle.CompareTo(b.Angle));
                                        }
                                    }
                                }
                                break;
                            default:
                                break;
                        }

                        _regionManipulationLineCount.Clear();

                        _regionManipulationIndex = 0;
                        this.SplitRegionMenu.IsChecked = false;
                        this.ReselectFittingDataMenu.IsChecked = false;
                        this.InsertPeakMenu.IsChecked = false;

                        this.MainFitPlot.Model.Annotations.Clear();

                        if (this.ShowCifDataRegion.IsChecked)
                        {
                            AddCifLines();
                        }

                        this.RegionView.Items.Refresh();

                        ResetGroupDescriptions();

                        System.ComponentModel.BackgroundWorker worker = new System.ComponentModel.BackgroundWorker();

                        worker.DoWork += PlotFittingPatterns_Work;
                        worker.RunWorkerCompleted += PlotFittingPatterns_Completed;

                        worker.RunWorkerAsync(PRF);
                    }
                }
            }
        }

        #region Settings

        #region Fitting Plot

        private void FittingPlotAxesToLinear_Click(object sender, RoutedEventArgs e)
        {
            this.FittingXAxisLin.Minimum = this.FittingXAxisLin.ActualMinimum;
            this.FittingXAxisLin.Maximum = this.FittingXAxisLin.ActualMaximum;

            if (CalScec.Properties.Settings.Default.PlotYFittingAxes != 0)
            {
                CalScec.Properties.Settings.Default.PlotYFittingAxes = 0;
                this.FittingPlotModel.Axes.Remove(this.FittingYAxisLog);
                this.FittingPlotModel.Axes.Add(this.FittingYAxisLin);
                this.FittingYAxisLin.Minimum = this.FittingYAxisLog.ActualMinimum;
                this.FittingYAxisLin.Maximum = this.FittingYAxisLog.ActualMaximum;
                this.MainFitPlot.Model.ResetAllAxes();
                this.MainFitPlot.Model.InvalidatePlot(true);

                this.FittingPlotAxesToLog.IsChecked = false;
                this.FittingPlotAxesToLinear.IsChecked = true;
            }
        }

        private void FittingPlotAxesToLog_Click(object sender, RoutedEventArgs e)
        {
            this.FittingXAxisLin.Minimum = this.FittingXAxisLin.ActualMinimum;
            this.FittingXAxisLin.Maximum = this.FittingXAxisLin.ActualMaximum;

            if (CalScec.Properties.Settings.Default.PlotYFittingAxes != 1)
            {
                CalScec.Properties.Settings.Default.PlotYFittingAxes = 1;
                this.FittingPlotModel.Axes.Remove(this.FittingYAxisLin);
                this.FittingPlotModel.Axes.Add(this.FittingYAxisLog);
                this.FittingYAxisLog.Minimum = this.FittingYAxisLin.Minimum;
                this.FittingYAxisLog.Maximum = this.FittingYAxisLin.Maximum;
                this.MainFitPlot.Model.ResetAllAxes();
                this.MainFitPlot.Model.InvalidatePlot(true);

                this.FittingPlotAxesToLog.IsChecked = true;
                this.FittingPlotAxesToLinear.IsChecked = false;
            }
        }

        private void ChangeFittingMajorGridLineStyle_Click(object sender, RoutedEventArgs e)
        {
            MenuItem This = (MenuItem)sender;
            string Header = This.Header.ToString();

            switch (Header)
            {
                case "None":
                    CalScec.Properties.Settings.Default.FittingPlotMajorGridStyle = 0;
                    FittingXAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.None;
                    FittingYAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.None;
                    FittingYAxisLog.MajorGridlineStyle = OxyPlot.LineStyle.None;
                    this.FittingMajorGridStyleNone.IsChecked = true;
                    this.FittingMajorGridStyleDot.IsChecked = false;
                    this.FittingMajorGridStyleDash.IsChecked = false;
                    break;
                case "Dash":
                    CalScec.Properties.Settings.Default.FittingPlotMajorGridStyle = 1;
                    FittingXAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dash;
                    FittingYAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dash;
                    FittingYAxisLog.MajorGridlineStyle = OxyPlot.LineStyle.Dash;
                    this.FittingMajorGridStyleNone.IsChecked = false;
                    this.FittingMajorGridStyleDot.IsChecked = false;
                    this.FittingMajorGridStyleDash.IsChecked = true;
                    break;
                case "Dot":
                    CalScec.Properties.Settings.Default.FittingPlotMajorGridStyle = 2;
                    FittingXAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dot;
                    FittingYAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dot;
                    FittingYAxisLog.MajorGridlineStyle = OxyPlot.LineStyle.Dot;
                    this.FittingMajorGridStyleNone.IsChecked = false;
                    this.FittingMajorGridStyleDot.IsChecked = true;
                    this.FittingMajorGridStyleDash.IsChecked = false;
                    break;
                default:
                    CalScec.Properties.Settings.Default.FittingPlotMajorGridStyle = 2;
                    FittingXAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dot;
                    FittingYAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dot;
                    FittingYAxisLog.MajorGridlineStyle = OxyPlot.LineStyle.Dot;
                    this.FittingMajorGridStyleNone.IsChecked = false;
                    this.FittingMajorGridStyleDot.IsChecked = true;
                    this.FittingMajorGridStyleDash.IsChecked = false;
                    break;
            }

            this.MainFitPlot.Model.ResetAllAxes();
            this.MainFitPlot.Model.InvalidatePlot(true);
        }

        private void ChangeFittingMinorGridLineStyle_Click(object sender, RoutedEventArgs e)
        {
            MenuItem This = (MenuItem)sender;
            string Header = This.Header.ToString();

            switch (Header)
            {
                case "None":
                    CalScec.Properties.Settings.Default.FittingPlotMinorGridStyle = 0;
                    FittingXAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.None;
                    FittingYAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.None;
                    FittingYAxisLog.MinorGridlineStyle = OxyPlot.LineStyle.None;
                    this.FittingMinorGridStyleNone.IsChecked = true;
                    this.FittingMinorGridStyleDot.IsChecked = false;
                    this.FittingMinorGridStyleDash.IsChecked = false;
                    break;
                case "Dash":
                    CalScec.Properties.Settings.Default.FittingPlotMinorGridStyle = 1;
                    FittingXAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dash;
                    FittingYAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dash;
                    FittingYAxisLog.MinorGridlineStyle = OxyPlot.LineStyle.Dash;
                    this.FittingMinorGridStyleNone.IsChecked = false;
                    this.FittingMinorGridStyleDot.IsChecked = false;
                    this.FittingMinorGridStyleDash.IsChecked = true;
                    break;
                case "Dot":
                    CalScec.Properties.Settings.Default.FittingPlotMinorGridStyle = 2;
                    FittingXAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dot;
                    FittingYAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dot;
                    FittingYAxisLog.MinorGridlineStyle = OxyPlot.LineStyle.Dot;
                    this.FittingMinorGridStyleNone.IsChecked = false;
                    this.FittingMinorGridStyleDot.IsChecked = true;
                    this.FittingMinorGridStyleDash.IsChecked = false;
                    break;
                default:
                    CalScec.Properties.Settings.Default.FittingPlotMinorGridStyle = 2;
                    FittingXAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dot;
                    FittingYAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dot;
                    FittingYAxisLog.MinorGridlineStyle = OxyPlot.LineStyle.Dot;
                    this.FittingMinorGridStyleNone.IsChecked = false;
                    this.FittingMinorGridStyleDot.IsChecked = true;
                    this.FittingMinorGridStyleDash.IsChecked = false;
                    break;
            }

            this.MainFitPlot.Model.ResetAllAxes();
            this.MainFitPlot.Model.InvalidatePlot(true);
        }

        private void FunctionPlotResolution_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                double NewValue = Convert.ToDouble(FunctionPlotResolution.Text);
                CalScec.Properties.Settings.Default.FittingFunctionResolution = NewValue;
            }
            catch
            {

            }
        }

        #endregion

        #region Residual Plot

        private void ChangeResidualMajorGridLineStyle_Click(object sender, RoutedEventArgs e)
        {
            MenuItem This = (MenuItem)sender;
            string Header = This.Header.ToString();

            switch (Header)
            {
                case "None":
                    CalScec.Properties.Settings.Default.ResidualPlotMajorGridStyle = 0;
                    ResidualXAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.None;
                    ResidualYAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.None;
                    this.ResidualMajorGridStyleNone.IsChecked = true;
                    this.ResidualMajorGridStyleDot.IsChecked = false;
                    this.ResidualMajorGridStyleDash.IsChecked = false;
                    break;
                case "Dash":
                    CalScec.Properties.Settings.Default.ResidualPlotMajorGridStyle = 1;
                    ResidualXAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dash;
                    ResidualYAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dash;
                    this.ResidualMajorGridStyleNone.IsChecked = false;
                    this.ResidualMajorGridStyleDot.IsChecked = false;
                    this.ResidualMajorGridStyleDash.IsChecked = true;
                    break;
                case "Dot":
                    CalScec.Properties.Settings.Default.ResidualPlotMajorGridStyle = 2;
                    ResidualXAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dot;
                    ResidualYAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dot;
                    this.ResidualMajorGridStyleNone.IsChecked = false;
                    this.ResidualMajorGridStyleDot.IsChecked = true;
                    this.ResidualMajorGridStyleDash.IsChecked = false;
                    break;
                default:
                    CalScec.Properties.Settings.Default.ResidualPlotMajorGridStyle = 2;
                    ResidualXAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dot;
                    ResidualYAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dot;
                    this.ResidualMajorGridStyleNone.IsChecked = false;
                    this.ResidualMajorGridStyleDot.IsChecked = true;
                    this.ResidualMajorGridStyleDash.IsChecked = false;
                    break;
            }

            this.MainResPlot.Model.ResetAllAxes();
            this.MainResPlot.Model.InvalidatePlot(true);
        }

        private void ChangeResidualMinorGridLineStyle_Click(object sender, RoutedEventArgs e)
        {
            MenuItem This = (MenuItem)sender;
            string Header = This.Header.ToString();

            switch (Header)
            {
                case "None":
                    CalScec.Properties.Settings.Default.ResidualPlotMinorGridStyle = 0;
                    ResidualXAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.None;
                    ResidualYAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.None;
                    this.ResidualMinorGridStyleNone.IsChecked = true;
                    this.ResidualMinorGridStyleDot.IsChecked = false;
                    this.ResidualMinorGridStyleDash.IsChecked = false;
                    break;
                case "Dash":
                    CalScec.Properties.Settings.Default.ResidualPlotMinorGridStyle = 1;
                    ResidualXAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dash;
                    ResidualYAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dash;
                    this.ResidualMinorGridStyleNone.IsChecked = false;
                    this.ResidualMinorGridStyleDot.IsChecked = false;
                    this.ResidualMinorGridStyleDash.IsChecked = true;
                    break;
                case "Dot":
                    CalScec.Properties.Settings.Default.ResidualPlotMinorGridStyle = 2;
                    ResidualXAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dot;
                    ResidualYAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dot;
                    this.ResidualMinorGridStyleNone.IsChecked = false;
                    this.ResidualMinorGridStyleDot.IsChecked = true;
                    this.ResidualMinorGridStyleDash.IsChecked = false;
                    break;
                default:
                    CalScec.Properties.Settings.Default.ResidualPlotMinorGridStyle = 2;
                    ResidualXAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dot;
                    ResidualYAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dot;
                    this.ResidualMinorGridStyleNone.IsChecked = false;
                    this.ResidualMinorGridStyleDot.IsChecked = true;
                    this.ResidualMinorGridStyleDash.IsChecked = false;
                    break;
            }

            this.MainResPlot.Model.ResetAllAxes();
            this.MainResPlot.Model.InvalidatePlot(true);
        }

        private void OneSigmaLine_Click(object sender, RoutedEventArgs e)
        {
            if (!CalScec.Properties.Settings.Default.PlotFittingOneSigmaLine)
            {
                this.OneSigmaLine.IsChecked = true;
                CalScec.Properties.Settings.Default.PlotFittingOneSigmaLine = true;

                var PositiveLine = new OxyPlot.Annotations.LineAnnotation();
                PositiveLine.Color = OxyPlot.OxyColors.DarkGreen;
                PositiveLine.ClipByXAxis = true;
                PositiveLine.Y = 1;
                PositiveLine.Type = OxyPlot.Annotations.LineAnnotationType.Horizontal;
                PositiveLine.StrokeThickness = 1.5;
                PositiveLine.Text = "1 Sigma";
                PositiveLine.ClipText = true;
                PositiveLine.LineStyle = OxyPlot.LineStyle.Solid;
                var NegativeLine = new OxyPlot.Annotations.LineAnnotation();
                NegativeLine.Color = OxyPlot.OxyColors.DarkGreen;
                NegativeLine.ClipByXAxis = true;
                NegativeLine.Y = -1;
                NegativeLine.Type = OxyPlot.Annotations.LineAnnotationType.Horizontal;
                NegativeLine.StrokeThickness = 1.5;
                NegativeLine.Text = "1 Sigma";
                NegativeLine.ClipText = true;
                NegativeLine.LineStyle = OxyPlot.LineStyle.Solid;

                this.ResidualPlotModel.Annotations.Add(PositiveLine);
                this.ResidualPlotModel.Annotations.Add(NegativeLine);
            }
            else
            {
                this.OneSigmaLine.IsChecked = false;
                CalScec.Properties.Settings.Default.PlotFittingOneSigmaLine = false;

                for(int n = 0; n < this.ResidualPlotModel.Annotations.Count; n++)
                {
                    OxyPlot.Annotations.LineAnnotation ActAn = this.ResidualPlotModel.Annotations[n] as OxyPlot.Annotations.LineAnnotation;

                    if(Math.Abs(ActAn.Y) == 1)
                    {
                        this.ResidualPlotModel.Annotations.Remove(ActAn);
                    }
                }
            }
            
            this.MainResPlot.Model.InvalidatePlot(true);
        }

        private void ThreeSigmaLine_Click(object sender, RoutedEventArgs e)
        {
            if (!CalScec.Properties.Settings.Default.PlotFittingThreeSigmaLine)
            {
                this.ThreeSigmaLine.IsChecked = true;
                CalScec.Properties.Settings.Default.PlotFittingThreeSigmaLine = true;

                var PositiveLine = new OxyPlot.Annotations.LineAnnotation();
                PositiveLine.Color = OxyPlot.OxyColors.OrangeRed;
                PositiveLine.ClipByXAxis = true;
                PositiveLine.Y = 3;
                PositiveLine.Type = OxyPlot.Annotations.LineAnnotationType.Horizontal;
                PositiveLine.StrokeThickness = 1.5;
                PositiveLine.Text = "3 Sigma";
                PositiveLine.ClipText = true;
                PositiveLine.LineStyle = OxyPlot.LineStyle.Solid;
                var NegativeLine = new OxyPlot.Annotations.LineAnnotation();
                NegativeLine.Color = OxyPlot.OxyColors.OrangeRed;
                NegativeLine.ClipByXAxis = true;
                NegativeLine.Y = -3;
                NegativeLine.Type = OxyPlot.Annotations.LineAnnotationType.Horizontal;
                NegativeLine.StrokeThickness = 1.5;
                NegativeLine.Text = "3 Sigma";
                NegativeLine.ClipText = true;
                NegativeLine.LineStyle = OxyPlot.LineStyle.Solid;

                this.ResidualPlotModel.Annotations.Add(PositiveLine);
                this.ResidualPlotModel.Annotations.Add(NegativeLine);
            }
            else
            {
                this.ThreeSigmaLine.IsChecked = false;
                CalScec.Properties.Settings.Default.PlotFittingThreeSigmaLine = false;

                for (int n = 0; n < this.ResidualPlotModel.Annotations.Count; n++)
                {
                    OxyPlot.Annotations.LineAnnotation ActAn = this.ResidualPlotModel.Annotations[n] as OxyPlot.Annotations.LineAnnotation;

                    if (Math.Abs(ActAn.Y) == 3)
                    {
                        this.ResidualPlotModel.Annotations.Remove(ActAn);
                    }
                }
            }
            
            this.MainResPlot.Model.InvalidatePlot(true);
        }

        #endregion

        private void ReflexAutoCorrection_Click(object sender, RoutedEventArgs e)
        {
            if (ReflexAutoCorrection.IsChecked)
            {
                CalScec.Properties.Settings.Default.ReflexFitAutoCorrection = true;
            }
            else
            {
                CalScec.Properties.Settings.Default.ReflexFitAutoCorrection = false;
            }
        }

        #endregion

        #endregion

        private void RefreshRegionView()
        {
            RegionView.Items.Refresh();

            CollectionView RegionCollection = (CollectionView)CollectionViewSource.GetDefaultView(RegionView.ItemsSource);
            PropertyGroupDescription PeakGroupDescription = new PropertyGroupDescription("AssociatedPatternName");
            RegionCollection.GroupDescriptions.Add(PeakGroupDescription);
        }

        private void StartRegionFit_Click(object sender, RoutedEventArgs e)
        {
            if(RegionView.SelectedIndex != -1)
            {
                Peaks.Functions.PeakRegionFunction PRF = (Peaks.Functions.PeakRegionFunction)this.RegionView.SelectedItem;

                System.Threading.ThreadPool.QueueUserWorkItem(PRF.FitRegionCallback);
            }
        }

        private void StartPeakFit_Click(object sender, RoutedEventArgs e)
        {
            if (PeakView.SelectedIndex != -1)
            {
                Peaks.Functions.PeakFunction PF = (Peaks.Functions.PeakFunction)this.PeakView.SelectedItem;

                System.Threading.ThreadPool.QueueUserWorkItem(PF.FitPeakCallback);
            }
        }

        public void AddRegion(Peaks.Functions.PeakRegionFunction PRF)
        {
            PRF.FreeParameters[0] = true;
            PRF.FreeParameters[1] = true;
            PRF.FreeParameters[2] = true;
            PRF.FreeParameters[3] = true;
            PRF.FreeParameters[4] = true;
            PRF.FreeParameters[5] = true;
            PRF.FreeParameters[6] = true;

            PRF.backgroundFit = true;
            PRF.backgroundSwitch = true;

            PRF.functionType = 2;

            PRF.SetResetEvent(new System.Threading.ManualResetEvent(true));
            PRF.startingLambda = -1;

            PRF.FitStarted += this.RegionFitStarted;
            PRF.FitFinished += this.RegionFitFinished;

            for(int n = 0; n < PRF.Count; n++)
            {
                PRF[n].functionType = 2;
                PRF[n].backgroundSwitch = true;
                PRF[n].backgroundFit = true;

                PRF[n].FreeParameters[0] = true;
                PRF[n].FreeParameters[1] = true;
                PRF[n].FreeParameters[2] = true;
                PRF[n].FreeParameters[3] = true;
                PRF[n].FreeParameters[4] = true;
                PRF[n].FreeParameters[5] = true;
                PRF[n].FreeParameters[6] = true;

                PRF[n].ParameterConstraints.AngleConstraintActiv = false;
                PRF[n].ParameterConstraints.SigmaConstraintActiv = false;
                PRF[n].ParameterConstraints.LorentzRatioConstraintActiv = false;

                PRF[n].SetResetEvent(new System.Threading.ManualResetEvent(true));

                PRF[n].startingLambda = -1;

                PRF[n].FitStarted += this.PeakFitStarted;
                PRF[n].FitFinished += this.PeakFitFinished;
            }

            AllPeakRegions.Add(PRF);
            StandByPeakRegions.Add(PRF);
            TotalPeaks += PRF.Count;

            //RegionView.Items.Refresh();
        }

        public void AddRegionWithFit(Peaks.Functions.PeakRegionFunction PRF)
        {
            this.AddRegion(PRF);

            System.Threading.ThreadPool.QueueUserWorkItem(PRF.FitRegionCallback);
        }

        public void AddPresettedRegion(Peaks.Functions.PeakRegionFunction PRF)
        {
            PRF.FitStarted += this.RegionFitStarted;
            PRF.FitFinished += this.RegionFitFinished;

            PRF.SetResetEvent(new System.Threading.ManualResetEvent(true));

            for (int n = 0; n < PRF.Count; n++)
            {
                PRF[n].FitStarted += this.PeakFitStarted;
                PRF[n].FitFinished += this.PeakFitFinished;

                PRF[n].SetResetEvent(new System.Threading.ManualResetEvent(true));
            }

            AllPeakRegions.Add(PRF);
            StandByPeakRegions.Add(PRF);
            TotalPeaks += PRF.Count;

            //RegionView.Items.Refresh();
        }

        public void AddPresettedRegionWithFit(Peaks.Functions.PeakRegionFunction PRF)
        {
            this.AddPresettedRegion(PRF);

            System.Threading.ThreadPool.QueueUserWorkItem(PRF.FitRegionCallback);
        }

        public void RemoveRegion(Peaks.Functions.PeakRegionFunction PRF)
        {
            AllPeakRegions.Remove(PRF);
            StandByPeakRegions.Remove(PRF);
            TotalPeaks -= PRF.Count;

            //RegionView.Items.Refresh();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (PreventClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
            //base.OnClosing(e);
        }
    }
}
