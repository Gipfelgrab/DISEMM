﻿///////////////////////////////////////////////////////////////////////////////////////////////
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
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.ComponentModel;

namespace CalScec
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Analysis.Sample InvestigatedSample = new Analysis.Sample();

        public OxyPlot.PlotModel DiffractionPlotModel = new OxyPlot.PlotModel();
        OxyPlot.Axes.LinearAxis DiffractionXAxisLin = new OxyPlot.Axes.LinearAxis();
        OxyPlot.Axes.LinearAxis DiffractionYAxisLin = new OxyPlot.Axes.LinearAxis();
        OxyPlot.Axes.LogarithmicAxis DiffractionYAxisLog = new OxyPlot.Axes.LogarithmicAxis();

        List<OxyPlot.Annotations.LineAnnotation> HKLAnnotationList = new List<OxyPlot.Annotations.LineAnnotation>();
        List<OxyPlot.Annotations.LineAnnotation> PeakAnnotationList = new List<OxyPlot.Annotations.LineAnnotation>();

        Analysis.Fitting.PeakFittingWindow PeakFitWindow;

        bool _peakAddingActive = false;
        private List<OxyPlot.Annotations.Annotation> PeakManipulationLine = new List<OxyPlot.Annotations.Annotation>();
        double[] _newPeakCharacteristic = { 0.0, 0.0};

        bool _textEventactive = true;

        public MainWindow()
        {
            InitializeComponent();

            #region StartIdSettings

            CalScec.Properties.Settings.Default.CODId = 0;
            CalScec.Properties.Settings.Default.PeakId = 0;

            #endregion

            SetWindowProperties();
            SetDataBindings();
            SetPlot();
        }

        #region Window positioning and save changes

        bool MaxToNormal = false;

        private void SetWindowProperties()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");

            this.MaxToNormal = true;

            this.StateChanged += MainWindow_StateChanged;
            this.LocationChanged += MainWindow_LocationChanged;
            this.SizeChanged += MainWindow_SizeChanged;
            
            
            if(CalScec.Properties.Settings.Default.MainWindowIsMaximized)
            {
                this.WindowState = System.Windows.WindowState.Maximized;
            }
            else
            {
                this.Width = CalScec.Properties.Settings.Default.MainWindowWidth;
                this.Height = CalScec.Properties.Settings.Default.MainWindowHeight;
                this.Left = CalScec.Properties.Settings.Default.MainWindowLocationX;
                this.Top = CalScec.Properties.Settings.Default.MainWindowLocationY;
            }

            this.MaxToNormal = false;

            this.PeakFitWindow = new Analysis.Fitting.PeakFittingWindow();

            this.SetProgress();
        }

        void MainWindow_StateChanged(object sender, EventArgs e)
        {
            this.MaxToNormal = true;

            if (this.WindowState == WindowState.Maximized)
            {
                CalScec.Properties.Settings.Default.MainWindowIsMaximized = true;
            }
            else if (this.WindowState == WindowState.Normal)
            {
                CalScec.Properties.Settings.Default.MainWindowIsMaximized = false;
                this.Width = CalScec.Properties.Settings.Default.MainWindowWidth;
                this.Height = CalScec.Properties.Settings.Default.MainWindowHeight;
                this.Left = CalScec.Properties.Settings.Default.MainWindowLocationX;
                this.Top = CalScec.Properties.Settings.Default.MainWindowLocationY;
            }

            this.MaxToNormal = false;
        }

        void MainWindow_LocationChanged(object sender, EventArgs e)
        {
            if (!MaxToNormal)
            {
                CalScec.Properties.Settings.Default.MainWindowLocationY = this.Top;
                CalScec.Properties.Settings.Default.MainWindowLocationX = this.Left; 
            }
        }

        void MainWindow_SizeChanged(object sender, EventArgs e)
        {
            if (!MaxToNormal)
            {
                CalScec.Properties.Settings.Default.MainWindowWidth = this.Width;
                CalScec.Properties.Settings.Default.MainWindowHeight = this.Height;
            }
        }

        #endregion

        private void SetDataBindings()
        {
            DiffractionPatternList.ItemsSource = this.InvestigatedSample.DiffractionPatterns;

            this.HyperResolution.Text = Convert.ToString(CalScec.Properties.Settings.Default.HyperSensitivity);
            this.AcceptedPeakBackgroundRation.Text = Convert.ToString(CalScec.Properties.Settings.Default.AcceptedPeakBackgroundRatio);
            this.AcceptedDifferenceHKLAssociation.Text = Convert.ToString(CalScec.Properties.Settings.Default.HKLAssociationRangeDeg);
            this.PatternLowerLimitTextBox.Text = Convert.ToString(CalScec.Properties.Settings.Default.PatternLowerLimit);
            this.PatternUpperLimitTextBox.Text = Convert.ToString(CalScec.Properties.Settings.Default.PatternUpperLimit);
            this.WavelengthTextBox.Text = Convert.ToString(CalScec.Properties.Settings.Default.UsedWaveLength);
            if (CalScec.Properties.Settings.Default.PlotHKLOverview)
            {
                PlotHKLOverviewMenuItem.IsChecked = true;
            }
            else
            {
                PlotHKLOverviewMenuItem.IsChecked = false;
            }
            
            if(CalScec.Properties.Settings.Default.AutomaticPeakFit)
            {
                this.AutomaticPeakFitMenuItem.IsChecked = true;
            }
            else
            {
                this.AutomaticPeakFitMenuItem.IsChecked = false;
            }

            if(CalScec.Properties.Settings.Default.UsedPeakDetectionId == 0)
            {
                PeakDetectionRoutineHyper.IsChecked = true;
            }
            else
            {
                PeakDetectionRoutineCIF.IsChecked = true;
            }
        }

        private void SetPlot()
        {
            DiffractionPlotModel.LegendBorder = OxyPlot.OxyColors.Black;
            DiffractionPlotModel.LegendItemAlignment = OxyPlot.HorizontalAlignment.Left;
            DiffractionPlotModel.LegendTitle = "Diffraction pattern";

            DiffractionXAxisLin.Position = OxyPlot.Axes.AxisPosition.Bottom;
            DiffractionXAxisLin.Minimum = 0;
            DiffractionXAxisLin.Maximum = 180;
            DiffractionXAxisLin.Title = "";
            //DiffractionXAxisLin.Title = "Angle";

            DiffractionYAxisLin.Position = OxyPlot.Axes.AxisPosition.Left;
            DiffractionYAxisLin.Minimum = 0;
            DiffractionYAxisLin.Maximum = 100;
            DiffractionYAxisLin.Title = "Intensity";

            DiffractionYAxisLog.Position = OxyPlot.Axes.AxisPosition.Left;
            DiffractionYAxisLog.Minimum = 0;
            DiffractionYAxisLog.Maximum = 100;
            DiffractionYAxisLog.Title = "Intensity";

            XAxisMinToolText.Text = "0";
            XAxisMaxToolText.Text = "180";
            YAxisMinToolText.Text = "0";
            YAxisMaxToolText.Text = "100";

            #region GridStyles

            switch (CalScec.Properties.Settings.Default.MainPlotMajorGridStyle)
            {
                case 0:
                    DiffractionXAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.None;
                    DiffractionYAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.None;
                    DiffractionYAxisLog.MajorGridlineStyle = OxyPlot.LineStyle.None;
                    this.MajorGridStyleNone.IsChecked = true;
                    this.MajorGridStyleDot.IsChecked = false;
                    this.MajorGridStyleDash.IsChecked = false;
                    break;
                case 1:
                    DiffractionXAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dash;
                    DiffractionYAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dash;
                    DiffractionYAxisLog.MajorGridlineStyle = OxyPlot.LineStyle.Dash;
                    this.MajorGridStyleNone.IsChecked = false;
                    this.MajorGridStyleDot.IsChecked = false;
                    this.MajorGridStyleDash.IsChecked = true;
                    break;
                case 2:
                    DiffractionXAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dot;
                    DiffractionYAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dot;
                    DiffractionYAxisLog.MajorGridlineStyle = OxyPlot.LineStyle.Dot;
                    this.MajorGridStyleNone.IsChecked = false;
                    this.MajorGridStyleDot.IsChecked = true;
                    this.MajorGridStyleDash.IsChecked = false;
                    break;
                default:
                    DiffractionXAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dot;
                    DiffractionYAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dot;
                    DiffractionYAxisLog.MajorGridlineStyle = OxyPlot.LineStyle.Dot;
                    this.MajorGridStyleNone.IsChecked = false;
                    this.MajorGridStyleDot.IsChecked = true;
                    this.MajorGridStyleDash.IsChecked = false;
                    break;
            }

            switch (CalScec.Properties.Settings.Default.MainPlotMinorGridStyle)
            {
                case 0:
                    DiffractionXAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.None;
                    DiffractionYAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.None;
                    DiffractionYAxisLog.MinorGridlineStyle = OxyPlot.LineStyle.None;
                    this.MinorGridStyleNone.IsChecked = true;
                    this.MinorGridStyleDot.IsChecked = false;
                    this.MinorGridStyleDash.IsChecked = false;
                    break;
                case 1:
                    DiffractionXAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dash;
                    DiffractionYAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dash;
                    DiffractionYAxisLog.MinorGridlineStyle = OxyPlot.LineStyle.Dash;
                    this.MinorGridStyleNone.IsChecked = false;
                    this.MinorGridStyleDot.IsChecked = false;
                    this.MinorGridStyleDash.IsChecked = true;
                    break;
                case 2:
                    DiffractionXAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dot;
                    DiffractionYAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dot;
                    DiffractionYAxisLog.MinorGridlineStyle = OxyPlot.LineStyle.Dot;
                    this.MinorGridStyleNone.IsChecked = false;
                    this.MinorGridStyleDot.IsChecked = true;
                    this.MinorGridStyleDash.IsChecked = false;
                    break;
                default:
                    DiffractionXAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.None;
                    DiffractionYAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.None;
                    DiffractionYAxisLog.MinorGridlineStyle = OxyPlot.LineStyle.None;
                    this.MinorGridStyleNone.IsChecked = true;
                    this.MinorGridStyleDot.IsChecked = false;
                    this.MinorGridStyleDash.IsChecked = false;
                    break;
            }

            #endregion

            DiffractionPlotModel.Axes.Add(DiffractionXAxisLin);

            if(CalScec.Properties.Settings.Default.PlotYAxes == 0)
            {
                DiffractionPlotModel.Axes.Add(DiffractionYAxisLin);
                this.DiffractionPlotAxesToLog.IsChecked = false;
                this.DiffractionPlotAxesToLinear.IsChecked = true;

                this.ChangeAxisTool.ToolTip = "Set axis to logarythmic";
                this.ChangeAxisTool.Click += DiffractionPlotAxesToLog_Click;
            }
            else if(CalScec.Properties.Settings.Default.PlotYAxes == 1)
            {
                DiffractionPlotModel.Axes.Add(DiffractionYAxisLog);
                this.DiffractionPlotAxesToLog.IsChecked = true;
                this.DiffractionPlotAxesToLinear.IsChecked = false;

                this.ChangeAxisTool.ToolTip = "Set axis to linear";
                this.ChangeAxisTool.Click += DiffractionPlotAxesToLinear_Click;
            }

            #region Plot Settings

            switch (CalScec.Properties.Settings.Default.MainPlotLineStyle)
            {
                case 0:
                    this.ChangeLineStyleNone.IsChecked = true;
                    this.ChangeLineStyleDot.IsChecked = false;
                    this.ChangeLineStyleDash.IsChecked = false;
                    break;
                case 1:
                    this.ChangeLineStyleNone.IsChecked = false;
                    this.ChangeLineStyleDot.IsChecked = true;
                    this.ChangeLineStyleDash.IsChecked = false;
                    break;
                case 2:
                    this.ChangeLineStyleNone.IsChecked = false;
                    this.ChangeLineStyleDot.IsChecked = false;
                    this.ChangeLineStyleDash.IsChecked = true;
                    break;
                default:
                    this.ChangeLineStyleNone.IsChecked = true;
                    this.ChangeLineStyleDot.IsChecked = false;
                    this.ChangeLineStyleDash.IsChecked = false;
                    break;
            }

            switch (CalScec.Properties.Settings.Default.MainPlotLineThickness)
            {
                case 1:
                    this.ChangeLineThickness1.IsChecked = true;
                    this.ChangeLineThickness2.IsChecked = false;
                    this.ChangeLineThickness3.IsChecked = false;
                    break;
                case 2:
                    this.ChangeLineThickness1.IsChecked = false;
                    this.ChangeLineThickness2.IsChecked = true;
                    this.ChangeLineThickness3.IsChecked = false;
                    break;
                case 3:
                    this.ChangeLineThickness1.IsChecked = false;
                    this.ChangeLineThickness2.IsChecked = false;
                    this.ChangeLineThickness3.IsChecked = true;
                    break;
                default:
                    this.ChangeLineThickness1.IsChecked = true;
                    this.ChangeLineThickness2.IsChecked = false;
                    this.ChangeLineThickness3.IsChecked = false;
                    break;
            }

            switch (Convert.ToString(CalScec.Properties.Settings.Default.MainPlotDotThickness))
            {
                case "0":
                    this.ChangeMarkerThickness0.IsChecked = true;
                    this.ChangeMarkerThickness1.IsChecked = false;
                    this.ChangeMarkerThickness15.IsChecked = false;
                    this.ChangeMarkerThickness2.IsChecked = false;
                    this.ChangeMarkerThickness25.IsChecked = false;
                    this.ChangeMarkerThickness3.IsChecked = false;
                    this.ChangeMarkerThickness35.IsChecked = false;
                    break;
                case "1":
                    this.ChangeMarkerThickness0.IsChecked = false;
                    this.ChangeMarkerThickness1.IsChecked = true;
                    this.ChangeMarkerThickness15.IsChecked = false;
                    this.ChangeMarkerThickness2.IsChecked = false;
                    this.ChangeMarkerThickness25.IsChecked = false;
                    this.ChangeMarkerThickness3.IsChecked = false;
                    this.ChangeMarkerThickness35.IsChecked = false;
                    break;
                case "1.5":
                    this.ChangeMarkerThickness0.IsChecked = false;
                    this.ChangeMarkerThickness1.IsChecked = false;
                    this.ChangeMarkerThickness15.IsChecked = true;
                    this.ChangeMarkerThickness2.IsChecked = false;
                    this.ChangeMarkerThickness25.IsChecked = false;
                    this.ChangeMarkerThickness3.IsChecked = false;
                    this.ChangeMarkerThickness35.IsChecked = false;
                    break;
                case "2":
                    this.ChangeMarkerThickness0.IsChecked = false;
                    this.ChangeMarkerThickness1.IsChecked = false;
                    this.ChangeMarkerThickness15.IsChecked = false;
                    this.ChangeMarkerThickness2.IsChecked = true;
                    this.ChangeMarkerThickness25.IsChecked = false;
                    this.ChangeMarkerThickness3.IsChecked = false;
                    this.ChangeMarkerThickness35.IsChecked = false;
                    break;
                case "2.5":
                    this.ChangeMarkerThickness0.IsChecked = false;
                    this.ChangeMarkerThickness1.IsChecked = false;
                    this.ChangeMarkerThickness15.IsChecked = false;
                    this.ChangeMarkerThickness2.IsChecked = false;
                    this.ChangeMarkerThickness25.IsChecked = true;
                    this.ChangeMarkerThickness3.IsChecked = false;
                    this.ChangeMarkerThickness35.IsChecked = false;
                    break;
                case "3":
                    this.ChangeMarkerThickness0.IsChecked = false;
                    this.ChangeMarkerThickness1.IsChecked = false;
                    this.ChangeMarkerThickness15.IsChecked = false;
                    this.ChangeMarkerThickness2.IsChecked = false;
                    this.ChangeMarkerThickness25.IsChecked = false;
                    this.ChangeMarkerThickness3.IsChecked = true;
                    this.ChangeMarkerThickness35.IsChecked = false;
                    break;
                case "3.5":
                    this.ChangeMarkerThickness0.IsChecked = false;
                    this.ChangeMarkerThickness1.IsChecked = false;
                    this.ChangeMarkerThickness15.IsChecked = false;
                    this.ChangeMarkerThickness2.IsChecked = false;
                    this.ChangeMarkerThickness25.IsChecked = false;
                    this.ChangeMarkerThickness3.IsChecked = false;
                    this.ChangeMarkerThickness35.IsChecked = true;
                    break;
                default:
                    this.ChangeMarkerThickness0.IsChecked = false;
                    this.ChangeMarkerThickness1.IsChecked = false;
                    this.ChangeMarkerThickness15.IsChecked = false;
                    this.ChangeMarkerThickness2.IsChecked = false;
                    this.ChangeMarkerThickness25.IsChecked = false;
                    this.ChangeMarkerThickness3.IsChecked = true;
                    this.ChangeMarkerThickness35.IsChecked = false;
                    break;
            }

            switch (Convert.ToString(CalScec.Properties.Settings.Default.PeakMarkingThickness))
            {
                case "1":
                    this.ChangePeakLineThickness1.IsChecked = true;
                    this.ChangePeakLineThickness2.IsChecked = false;
                    this.ChangePeakLineThickness3.IsChecked = false;
                    break;
                case "2":
                    this.ChangePeakLineThickness1.IsChecked = false;
                    this.ChangePeakLineThickness2.IsChecked = true;
                    this.ChangePeakLineThickness3.IsChecked = false;
                    break;
                case "3":
                    this.ChangePeakLineThickness1.IsChecked = false;
                    this.ChangePeakLineThickness2.IsChecked = false;
                    this.ChangePeakLineThickness3.IsChecked = true;
                    break;
                default:
                    this.ChangePeakLineThickness1.IsChecked = true;
                    this.ChangePeakLineThickness2.IsChecked = false;
                    this.ChangePeakLineThickness3.IsChecked = false;
                    break;
            }

            switch (Convert.ToString(CalScec.Properties.Settings.Default.PeakMarkingStyle))
            {
                case "0":
                    this.ChangePeakLineStyleDot.IsChecked = true;
                    this.ChangePeakLineStyleDash.IsChecked = false;
                    break;
                case "1":
                    this.ChangePeakLineStyleDot.IsChecked = false;
                    this.ChangePeakLineStyleDash.IsChecked = true;
                    break;
                default:
                    this.ChangePeakLineStyleDot.IsChecked = true;
                    this.ChangePeakLineStyleDash.IsChecked = false;
                    break;
            }

            #endregion

            this.MainDiffPlot.Model = DiffractionPlotModel;
            this.MainDiffPlot.Model.ResetAllAxes();
            this.MainDiffPlot.Model.InvalidatePlot(true);
        }

        private void SetProgress()
        {
            if(this.InvestigatedSample.CrystalData.Count > 0)
            {
                this.StructureEllipse.Fill = Brushes.DarkGreen;

                this.CompositionTutorial.Visibility = Visibility.Collapsed;

                bool dECPassed = true;

                if(this.InvestigatedSample.DiffractionPatterns.Count > 0)
                {
                    this.PatternPeakEllipse.Fill = Brushes.DarkGreen;
                    this.SampleToPatternPath.Fill = Brushes.DarkGreen;
                    this.PatternPeakTutorial.Visibility = Visibility.Collapsed;

                    this.PatternToDECPath.Fill = Brushes.DarkGreen;

                    List<bool> dECPassedList = new List<bool>();

                    for (int n = 0; n < this.InvestigatedSample.CrystalData.Count; n++)
                    {
                        if (this.InvestigatedSample.DiffractionConstants[n].Count > 0 | this.InvestigatedSample.DiffractionConstantsTexture[n].Count > 0)
                        {
                            dECPassedList.Add(true);
                        }
                        else
                        {
                            dECPassedList.Add(false);
                        }
                    }
                    
                    for (int n = 0; n < dECPassedList.Count; n++)
                    {
                        if (!dECPassedList[n])
                        {
                            dECPassed = false;
                        }
                    }

                    if(dECPassed)
                    {
                        this.DECEllipse.Fill = Brushes.DarkGreen;

                        this.DECToSECPath.Fill = Brushes.DarkGreen;

                        this.DECTutorial.Visibility = Visibility.Visible;

                        if(this.PhaseSelection.SelectedIndex != -1)
                        {
                            this.REKClassicCalculationList.ItemsSource = this.InvestigatedSample.DiffractionConstants[this.PhaseSelection.SelectedIndex];
                            this.DECList.Visibility = Visibility.Visible;
                        }
                    }
                    else
                    {
                        this.DECEllipse.Fill = Brushes.DarkRed;

                        this.DECToSECPath.Fill = Brushes.DarkRed;

                        this.DECTutorial.Visibility = Visibility.Visible;
                    }

                }
                else
                {
                    this.PatternPeakEllipse.Fill = Brushes.DarkRed;
                    this.DECEllipse.Fill = Brushes.DarkRed;

                    this.SampleToPatternPath.Fill = Brushes.DarkGreen;
                    this.PatternToDECPath.Fill = Brushes.DarkRed;
                    this.DECToSECPath.Fill = Brushes.DarkRed;

                    this.PatternPeakTutorial.Visibility = Visibility.Visible;
                    this.DECTutorial.Visibility = Visibility.Collapsed;
                }
                
                List<bool> passedList = new List<bool>();

                for(int n = 0; n < this.InvestigatedSample.CrystalData.Count; n++)
                {
                    if(this.InvestigatedSample.VoigtTensorData[n].C11 > 0 | this.InvestigatedSample.ReussTensorData[n].C11 > 0 | this.InvestigatedSample.HillTensorData[n].C11 > 0 | this.InvestigatedSample.KroenerTensorData[n].C11 > 0 | this.InvestigatedSample.DeWittTensorData[n].C11 > 0 | this.InvestigatedSample.GeometricHillTensorData[n].C11 > 0)
                    {
                        passedList.Add(true);
                    }
                    else
                    {
                        passedList.Add(false);
                    }
                }

                bool sECPassed = true;

                for(int n = 0; n < passedList.Count; n++)
                {
                    if(!passedList[n])
                    {
                        sECPassed = false;
                    }
                }

                if(sECPassed)
                {
                    this.SECEllipse.Fill = Brushes.DarkGreen;

                    this.SECTutorial.Visibility = Visibility.Visible;
                }
                else
                {
                    this.SECEllipse.Fill = Brushes.DarkRed;

                    this.SECTutorial.Visibility = Visibility.Visible;
                }
            }
            else
            {
                this.StructureEllipse.Fill = Brushes.DarkRed;
                this.PatternPeakEllipse.Fill = Brushes.DarkRed;
                this.DECEllipse.Fill = Brushes.DarkRed;
                this.SECEllipse.Fill = Brushes.DarkRed;

                this.SampleToPatternPath.Fill = Brushes.DarkRed;
                this.PatternToDECPath.Fill = Brushes.DarkRed;
                this.DECToSECPath.Fill = Brushes.DarkRed;

                this.CompositionTutorial.Visibility = Visibility.Visible;
                this.PatternPeakTutorial.Visibility = Visibility.Collapsed;
                this.DECTutorial.Visibility = Visibility.Collapsed;
                this.SECTutorial.Visibility = Visibility.Collapsed;
            }
        }

        #region Menu

        private void RestartCalScec_Click(object sender, RoutedEventArgs e)
        {
            MainWindow NewOne = new MainWindow();
            NewOne.Show();
            this.Close();
        }

        private void QuitCalScec_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #region Saving and loading data

        #region scec format

        private void SaveToSCEC_Click(object sender, RoutedEventArgs e)
        {
            DataManagment.Files.SCEC.Header ForSave = new DataManagment.Files.SCEC.Header(this.InvestigatedSample);

            Microsoft.Win32.SaveFileDialog OpenSampleFile = new Microsoft.Win32.SaveFileDialog();
            OpenSampleFile.FileName = this.InvestigatedSample.Name;
            OpenSampleFile.DefaultExt = ".scec";
            OpenSampleFile.Filter = "sample data (.scec)|*.scec";

            Nullable<bool> Opened = OpenSampleFile.ShowDialog();

            if(Opened == true)
            {
                string filename = OpenSampleFile.FileName;
                string PathName = filename.Replace(OpenSampleFile.SafeFileName, "");
                System.IO.Directory.CreateDirectory(PathName);

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
        }

        private void LoadFromSCEC_Click(object sender, RoutedEventArgs e)
        {
           this._textEventactive = false;

            Microsoft.Win32.OpenFileDialog OpenSampleFile = new Microsoft.Win32.OpenFileDialog();
            OpenSampleFile.Multiselect = false;
            OpenSampleFile.DefaultExt = ".scec";
            OpenSampleFile.Filter = "sample data (.scec)|*.scec";

            Nullable<bool> Opened = OpenSampleFile.ShowDialog();

            if (Opened == true)
            {
                string filename = OpenSampleFile.FileName;

                using (System.IO.Stream fileStream = System.IO.File.OpenRead(filename))
                {
                    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                    object DataObj = bf.Deserialize(fileStream);

                    DataManagment.Files.SCEC.Header Loaded = DataObj as DataManagment.Files.SCEC.Header;

                    this.InvestigatedSample = Loaded.GetSample();

                    CalScec.Properties.Settings.Default.UsedWaveLength = Loaded.UsedWaveLength;
                }

                PlotHKLToPlot();

                DiffractionPatternList.ItemsSource = this.InvestigatedSample.DiffractionPatterns;

                for(int n = 0; n < this.InvestigatedSample.DiffractionPatterns.Count; n++)
                {
                    for (int i = 0; i < this.InvestigatedSample.DiffractionPatterns[n].PeakRegions.Count; i++)
                    {
                        this.PeakFitWindow.AddPresettedRegion(this.InvestigatedSample.DiffractionPatterns[n].PeakRegions[i]);
                    }
                }

                this.InvestigatedSample.SetYieldExperimentalData();

                this.SampleName.Text = this.InvestigatedSample.Name;
                this.SampleArea.Text = this.InvestigatedSample.Area.ToString("F3");

                for (int n = 0; n < this.InvestigatedSample.CrystalData.Count; n++)
                {
                    ComboBoxItem presentPhase = new ComboBoxItem();
                    presentPhase.Content = this.InvestigatedSample.CrystalData[n].Name;
                    this.PhaseSelection.Items.Add(presentPhase);

                    this.InvestigatedSample.VoigtTensorData[n].SetAverageParameters(this.InvestigatedSample.VoigtTensorData[n].GetCalculatedDiffractionConstantsVoigt(this.InvestigatedSample.CrystalData[n]));
                    this.InvestigatedSample.ReussTensorData[n].SetAverageParameters(this.InvestigatedSample.ReussTensorData[n].GetCalculatedDiffractionConstantsReuss(this.InvestigatedSample.CrystalData[n]));
                    this.InvestigatedSample.HillTensorData[n].SetAverageParameters(this.InvestigatedSample.HillTensorData[n].GetCalculatedDiffractionConstantsHill(this.InvestigatedSample.CrystalData[n]));
                    this.InvestigatedSample.KroenerTensorData[n].SetAverageParameters(this.InvestigatedSample.KroenerTensorData[n].GetCalculatedDiffractionConstantsKroenerStiffness(this.InvestigatedSample.CrystalData[n]));
                    this.InvestigatedSample.DeWittTensorData[n].SetAverageParameters(this.InvestigatedSample.DeWittTensorData[n].GetCalculatedDiffractionConstantsDeWittStiffness(this.InvestigatedSample.CrystalData[n]));
                    this.InvestigatedSample.GeometricHillTensorData[n].SetAverageParameters(this.InvestigatedSample.GeometricHillTensorData[n].GetCalculatedDiffractionConstantsGeometricHill(this.InvestigatedSample.CrystalData[n]));

                    //this.ActSample.ReussTensorData[n].SetPeakStressAssociation(this.ActSample);
                    this.InvestigatedSample.ReussTensorData[n].DiffractionConstants = InvestigatedSample.DiffractionConstants[n];
                    this.InvestigatedSample.HillTensorData[n].DiffractionConstants = InvestigatedSample.DiffractionConstants[n];
                    this.InvestigatedSample.KroenerTensorData[n].DiffractionConstants = InvestigatedSample.DiffractionConstants[n];
                    this.InvestigatedSample.DeWittTensorData[n].DiffractionConstants = InvestigatedSample.DiffractionConstants[n];
                    this.InvestigatedSample.GeometricHillTensorData[n].DiffractionConstants = InvestigatedSample.DiffractionConstants[n];
                
                    this.InvestigatedSample.VoigtTensorData[n].DiffractionConstantsTexture = InvestigatedSample.DiffractionConstantsTexture[n];
                    this.InvestigatedSample.ReussTensorData[n].DiffractionConstantsTexture = InvestigatedSample.DiffractionConstantsTexture[n];
                    this.InvestigatedSample.HillTensorData[n].DiffractionConstantsTexture = InvestigatedSample.DiffractionConstantsTexture[n];
                    this.InvestigatedSample.KroenerTensorData[n].DiffractionConstantsTexture = InvestigatedSample.DiffractionConstantsTexture[n];
                    this.InvestigatedSample.DeWittTensorData[n].DiffractionConstantsTexture = InvestigatedSample.DiffractionConstantsTexture[n];
                    this.InvestigatedSample.GeometricHillTensorData[n].DiffractionConstantsTexture = InvestigatedSample.DiffractionConstantsTexture[n];

                    //this.InvestigatedSample.VoigtTensorData[n].SetPeakStressAssociation(this.InvestigatedSample);
                    this.InvestigatedSample.ReussTensorData[n].SetPeakStressAssociation(this.InvestigatedSample);
                    this.InvestigatedSample.HillTensorData[n].SetPeakStressAssociation(this.InvestigatedSample);
                    this.InvestigatedSample.KroenerTensorData[n].SetPeakStressAssociation(this.InvestigatedSample);
                    this.InvestigatedSample.DeWittTensorData[n].SetPeakStressAssociation(this.InvestigatedSample);
                    this.InvestigatedSample.GeometricHillTensorData[n].SetPeakStressAssociation(this.InvestigatedSample);

                    //this.InvestigatedSample.ReussTensorData[n].SetStrainDataReflexYield();
                    //this.InvestigatedSample.HillTensorData[n].SetStrainDataReflexYield();
                    //this.InvestigatedSample.KroenerTensorData[n].SetStrainDataReflexYield();
                    //this.InvestigatedSample.DeWittTensorData[n].SetStrainDataReflexYield();
                    //this.InvestigatedSample.GeometricHillTensorData[n].SetStrainDataReflexYield();

                    for(int i = 0; i < this.InvestigatedSample.PlasticTensor[n].YieldSurfaceData.ReflexYieldData.Count; i++)
                    {
                        if (this.InvestigatedSample.CrystalData[n].SymmetryGroupID == 194)
                        {
                            this.InvestigatedSample.PlasticTensor[n].YieldSurfaceData.ReflexYieldData[i].SetCrystalStressAndStrain(1);
                        }
                        else
                        {
                            this.InvestigatedSample.PlasticTensor[n].YieldSurfaceData.ReflexYieldData[i].SetCrystalStressAndStrain(0);
                        }
                    }

                }
            }

            _textEventactive = true;

            this.PhaseSelection.SelectedIndex = 0;

            this.SetProgress();
        }

        private void ExportToTxt_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog XlsxSaveFile = new Microsoft.Win32.SaveFileDialog();
            XlsxSaveFile.FileName = this.InvestigatedSample.Name;
            XlsxSaveFile.DefaultExt = "";
            XlsxSaveFile.Filter = "Text Files (.txt)|*.txt";

            Nullable<bool> Opened = XlsxSaveFile.ShowDialog();

            if (Opened == true)
            {
                BackgroundWorker SaveWorker = new BackgroundWorker();
                SaveWorker.DoWork += SaveSampleTxt_Work;
                SaveWorker.ProgressChanged += SaveSampleXLS_ProgressChanged;
                SaveWorker.RunWorkerCompleted += SaveSampleTxt_WorkCompleted;
                SaveWorker.WorkerReportsProgress = true;

                SaveWorker.RunWorkerAsync(XlsxSaveFile);
            }
        }

        private void SaveSampleTxt_Work(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            worker.ReportProgress(3);

            List<string> fileLines = new List<string>();

            Microsoft.Win32.SaveFileDialog XlsxSaveFile = e.Argument as Microsoft.Win32.SaveFileDialog;

            string filename = XlsxSaveFile.FileName;
            string PathName = filename.Replace(XlsxSaveFile.SafeFileName, "");
            System.IO.Directory.CreateDirectory(PathName);

            fileLines.Add("# Sample Name: " + this.InvestigatedSample.Name);
            fileLines.Add("# Sample cross section (mm): " + this.InvestigatedSample.Area);
            fileLines.Add("# Used Neutron wavelength (Angstrom): " + CalScec.Properties.Settings.Default.UsedWaveLength.ToString("F3"));


            worker.ReportProgress(10);

            int PeakProgress = 10;

            //Peak File structure
            //Trennzeichen (;|,| |\t)
            //[0]HKL; [1]Position; [2]FWHM; [3]Area; [4]Chi; [5]Omega; [6]Phi; [7]Load as Force; [8] Macroextension;

            fileLines.Add("#HKL;Position;FWHM;Area;Chi;Omega;Phi;Load as Force; Macroextension");

            for (int n = 0; n < this.InvestigatedSample.DiffractionPatterns.Count; n++)
            {
                for (int i = 0; i < this.InvestigatedSample.DiffractionPatterns[n].FoundPeaks.Count; i++)
                {
                    fileLines.Add(this.InvestigatedSample.DiffractionPatterns[n].FoundPeaks[i].HKLAssociation + ";" + this.InvestigatedSample.DiffractionPatterns[n].FoundPeaks[i].Angle.ToString() + ";" + this.InvestigatedSample.DiffractionPatterns[n].FoundPeaks[i].PFunction.FWHM.ToString() + ";" + this.InvestigatedSample.DiffractionPatterns[n].FoundPeaks[i].PFunction.Intensity.ToString() + ";" + this.InvestigatedSample.DiffractionPatterns[n].ChiAngle.ToString() + ";" + this.InvestigatedSample.DiffractionPatterns[n].OmegaAngle.ToString() + ";" + this.InvestigatedSample.DiffractionPatterns[n].PhiSampleAngle.ToString() + ";" + this.InvestigatedSample.DiffractionPatterns[n].Force.ToString() + ";" + this.InvestigatedSample.DiffractionPatterns[n].MacroExtension.ToString());
                    
                }

                PeakProgress += Convert.ToInt32((70 / this.InvestigatedSample.DiffractionPatterns.Count) * n);

                worker.ReportProgress(PeakProgress);
            }

            worker.ReportProgress(80);

            System.IO.File.WriteAllLines(XlsxSaveFile.FileName, fileLines);
            worker.ReportProgress(95);
        }

        private void SaveSampleTxt_WorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            StatusProgress.Value = 0;
            StatusLog1.Content = "Save succesfull";
            Cursor = Cursors.Arrow;
        }


        private void ExportToxlsx_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog XlsxSaveFile = new Microsoft.Win32.SaveFileDialog();
            XlsxSaveFile.FileName = this.InvestigatedSample.Name;
            XlsxSaveFile.DefaultExt = "";
            XlsxSaveFile.Filter = "Excel data (.xlsx)|*.xlsx";

            Nullable<bool> Opened = XlsxSaveFile.ShowDialog();

            if (Opened == true)
            {
                BackgroundWorker SaveWorker = new BackgroundWorker();
                SaveWorker.DoWork += SaveSampleXLS_Work;
                SaveWorker.ProgressChanged += SaveSampleXLS_ProgressChanged;
                SaveWorker.RunWorkerCompleted += SaveSampleXLS_WorkCompleted;
                SaveWorker.WorkerReportsProgress = true;

                SaveWorker.RunWorkerAsync(XlsxSaveFile);
            }
        }

        private void SaveSampleXLS_Work(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            worker.ReportProgress(3);

            Microsoft.Win32.SaveFileDialog XlsxSaveFile = e.Argument as Microsoft.Win32.SaveFileDialog;

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

                worker.ReportProgress(5);

                ExcelLogSheet.Name = "General information";

                ExcelLogSheet.Cells[1, 1] = "Sample name:";
                ExcelLogSheet.Cells[1, 2] = this.InvestigatedSample.Name;

                ExcelLogSheet.Cells[2, 1] = "Sample cross section (mm):";
                ExcelLogSheet.Cells[2, 2] = this.InvestigatedSample.Area;

                ExcelLogSheet.Cells[3, 1] = "Used Neutron wavelength (Angstrom):";
                ExcelLogSheet.Cells[3, 2] = CalScec.Properties.Settings.Default.UsedWaveLength.ToString("F3");

                worker.ReportProgress(10);

                ExcelLogBook.Sheets[2].Select();

                ExcelLogSheet = (Microsoft.Office.Interop.Excel.Worksheet)ExcelLogBook.ActiveSheet;

                ExcelLogSheet.Name = "Peak list";

                int PeakProgress = 10;

                for (int n = 0; n < this.InvestigatedSample.DiffractionPatterns.Count; n++)
                {
                    ExcelLogSheet.Cells[(n * 7) + 1, 1] = "Diffraction pattern:";
                    ExcelLogSheet.Cells[(n * 7) + 2, 1] = "Name:";
                    ExcelLogSheet.Cells[(n * 7) + 3, 1] = "Psi angle:";
                    ExcelLogSheet.Cells[(n * 7) + 4, 1] = "Phi angle:";
                    ExcelLogSheet.Cells[(n * 7) + 5, 1] = "Applied force (N):";
                    ExcelLogSheet.Cells[(n * 7) + 6, 1] = "Applied stress (MPa):";
                    
                    ExcelLogSheet.Cells[(n * 7) + 2, 2] = this.InvestigatedSample.DiffractionPatterns[n].Name;
                    ExcelLogSheet.Cells[(n * 7) + 3, 2] = this.InvestigatedSample.DiffractionPatterns[n].OmegaAngle;
                    ExcelLogSheet.Cells[(n * 7) + 4, 2] = this.InvestigatedSample.DiffractionPatterns[n].ChiAngle;
                    ExcelLogSheet.Cells[(n * 7) + 5, 2] = this.InvestigatedSample.DiffractionPatterns[n].Force;
                    ExcelLogSheet.Cells[(n * 7) + 6, 2] = this.InvestigatedSample.DiffractionPatterns[n].Stress;

                    ExcelLogSheet.Cells[(n * 7) + 1, 4] = "Peak list";

                    ExcelLogSheet.Cells[(n * 7) + 1, 5] = "Position:";
                    ExcelLogSheet.Cells[(n * 7) + 2, 5] = "Intensity:";
                    ExcelLogSheet.Cells[(n * 7) + 3, 5] = "Symmetry and HKL:";
                    ExcelLogSheet.Cells[(n * 7) + 4, 5] = "Lattice distance";
                    ExcelLogSheet.Cells[(n * 7) + 5, 5] = "Peak id";

                    for(int i = 0; i < this.InvestigatedSample.DiffractionPatterns[n].FoundPeaks.Count; i++)
                    {
                        ExcelLogSheet.Cells[(n * 7) + 1, 6 + i] = this.InvestigatedSample.DiffractionPatterns[n].FoundPeaks[i].Angle;
                        ExcelLogSheet.Cells[(n * 7) + 2, 6 + i] = this.InvestigatedSample.DiffractionPatterns[n].FoundPeaks[i].PFunction.Intensity;
                        ExcelLogSheet.Cells[(n * 7) + 3, 6 + i] = this.InvestigatedSample.DiffractionPatterns[n].FoundPeaks[i].HKLAssociation;
                        ExcelLogSheet.Cells[(n * 7) + 4, 6 + i] = this.InvestigatedSample.DiffractionPatterns[n].FoundPeaks[i].LatticeDistance;
                        ExcelLogSheet.Cells[(n * 7) + 5, 6 + i] = this.InvestigatedSample.DiffractionPatterns[n].FoundPeaks[i].PFunction.PeakId;
                    }

                    PeakProgress += Convert.ToInt32((70 / this.InvestigatedSample.DiffractionPatterns.Count) * n);

                    worker.ReportProgress(PeakProgress);
                }

                worker.ReportProgress(80);

                ExcelLogBook.Sheets[3].Select();

                ExcelLogSheet = (Microsoft.Office.Interop.Excel.Worksheet)ExcelLogBook.ActiveSheet;

                ExcelLogSheet.Name = "Bulk Elastic data";

                List<Tools.BulkElasticPhaseEvaluations> EModuleEvaluations = this.InvestigatedSample.AveragedEModulStandard();
                List<Tools.BulkElasticPhaseEvaluations> PoissonEvaluations = this.InvestigatedSample.AveragedPossionNumberStandard();
                for (int n = 0; n < EModuleEvaluations.Count; n++)
                {
                    ExcelLogSheet.Cells[(n * 6) + 1, 1] = "Averaged bulk E-Module:";
                    ExcelLogSheet.Cells[(n * 6) + 2, 1] = "Averaged bulk E-Module error:";
                    ExcelLogSheet.Cells[(n * 6) + 3, 1] = "Phase";
                    ExcelLogSheet.Cells[(n * 6) + 4, 1] = "Averaged bulk Poisson number :";
                    ExcelLogSheet.Cells[(n * 6) + 5, 1] = "Averaged bulk Poisson number error:";
                    ExcelLogSheet.Cells[(n * 6) + 6, 1] = "Phase:";

                    ExcelLogSheet.Cells[(n * 6) + 1, 2] = EModuleEvaluations[n].BulkElasticity;
                    ExcelLogSheet.Cells[(n * 6) + 2, 2] = EModuleEvaluations[n].BulkElasticityError;
                    ExcelLogSheet.Cells[(n * 6) + 3, 2] = EModuleEvaluations[n].HKLPase;
                    ExcelLogSheet.Cells[(n * 6) + 4, 2] = PoissonEvaluations[n].BulkElasticity;
                    ExcelLogSheet.Cells[(n * 6) + 5, 2] = PoissonEvaluations[n].BulkElasticityError;
                    ExcelLogSheet.Cells[(n * 6) + 6, 2] = PoissonEvaluations[n].HKLPase;
                }

                int ActRow = 4;
                PeakProgress = 80;

                for (int n = 0; n < this.InvestigatedSample.MacroElasticData.Count; n++)
                {
                    ExcelLogSheet.Cells[1, ActRow] = "Reflex calculation";
                    ExcelLogSheet.Cells[2, ActRow] = "E-Module:";
                    ExcelLogSheet.Cells[3, ActRow] = this.InvestigatedSample.MacroElasticData[n].EModul;
                    ExcelLogSheet.Cells[4, ActRow] = "Psi angle:";
                    ExcelLogSheet.Cells[5, ActRow] = this.InvestigatedSample.MacroElasticData[n].PsiAngle;

                    ActRow++;

                    ExcelLogSheet.Cells[1, ActRow] = "Peak id";
                    ExcelLogSheet.Cells[2, ActRow] = "Symmetry and HKL";
                    ExcelLogSheet.Cells[3, ActRow] = "Lattice distance";
                    ExcelLogSheet.Cells[4, ActRow] = "Applied stress";

                    ActRow++;

                    for (int i = 0; i < this.InvestigatedSample.MacroElasticData[n].Count; i++)
                    {
                        ExcelLogSheet.Cells[1, ActRow] = this.InvestigatedSample.MacroElasticData[n][i].DPeak.PeakId;
                        ExcelLogSheet.Cells[2, ActRow] = this.InvestigatedSample.MacroElasticData[n][i].DPeak.HKLAssociation;
                        ExcelLogSheet.Cells[3, ActRow] = this.InvestigatedSample.MacroElasticData[n][i].DPeak.LatticeDistance;
                        ExcelLogSheet.Cells[4, ActRow] = this.InvestigatedSample.MacroElasticData[n][i].Stress;

                        ActRow++;
                    }

                    ActRow++;

                    PeakProgress += Convert.ToInt32((15 / this.InvestigatedSample.MacroElasticData.Count) * n);

                    worker.ReportProgress(PeakProgress);
                }

                worker.ReportProgress(95);

                ExcelLogApp.DisplayAlerts = false;
                ExcelLogBook.Close(true, XlsxSaveFile.FileName, System.Reflection.Missing.Value);
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

        private void SaveSampleXLS_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            StatusProgress.Value = e.ProgressPercentage;

            if (e.ProgressPercentage < 5)
            {
                StatusLog1.Content = "Generating the file structure!";
            }
            else if (e.ProgressPercentage < 10)
            {
                StatusLog1.Content = "Files generated!";
            }
            else if (e.ProgressPercentage < 80)
            {
                StatusLog1.Content = "Peak information is written to file!";
            }
            else if (e.ProgressPercentage < 95)
            {
                StatusLog1.Content = "Elastic data is written to file!";
            }
        }

        private void SaveSampleXLS_WorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            StatusProgress.Value = 0;
            StatusLog1.Content = "Save succesfull";
            Cursor = Cursors.Arrow;
        }

        #endregion

        #endregion

        #region Diffraction patterns

        private void OpenDiffractionPatternFile_Click(object sender, RoutedEventArgs e)
        {
            if (this.InvestigatedSample.CrystalData.Count != 0)
            {
                Microsoft.Win32.OpenFileDialog OpenDiffractionFile = new Microsoft.Win32.OpenFileDialog();
                OpenDiffractionFile.Multiselect = true;
                Nullable<bool> Opened = OpenDiffractionFile.ShowDialog();

                if (Opened == true)
                {
                    if (OpenDiffractionFile.FileNames.Length == 1)
                    {
                        BackgroundWorker OpenDiffractionPattern = new BackgroundWorker();

                        OpenDiffractionPattern.DoWork += OpenDiffractionPatternFile_Work;
                        OpenDiffractionPattern.ProgressChanged += OpenDiffractionPatternFile_ProgressChanged;
                        OpenDiffractionPattern.RunWorkerCompleted += OpenDiffractionPatternFile_Completed;
                        OpenDiffractionPattern.WorkerReportsProgress = true;

                        OpenDiffractionPattern.RunWorkerAsync(OpenDiffractionFile.FileName);
                    }
                    else
                    {
                        BackgroundWorker OpenMultipleDiffractionPattern = new BackgroundWorker();

                        OpenMultipleDiffractionPattern.DoWork += OpenMultipleDiffractionPatternFile_Work;
                        OpenMultipleDiffractionPattern.ProgressChanged += OpenMultipleDiffractionPatternFile_ProgressChanged;
                        OpenMultipleDiffractionPattern.RunWorkerCompleted += OpenMultipleDiffractionPatternFile_Completed;
                        OpenMultipleDiffractionPattern.WorkerReportsProgress = true;

                        OpenMultipleDiffractionPattern.RunWorkerAsync(OpenDiffractionFile.FileNames);
                    }
                }
            }
            else
            {
                MessageBox.Show("Since I am somehow smart I really would like to help you with finding and assigning peaks! But for that I need the crystallographic data first.\n Please be so kind and load it first", "No crystallographic data", MessageBoxButton.OK, MessageBoxImage.Stop);
            }

        }

        private void OpenDiffractionPatternFolder_Click(object sender, RoutedEventArgs e)
        {
            if (this.InvestigatedSample.CrystalData.Count != 0)
            {
                System.Windows.Forms.FolderBrowserDialog DiffractionFolder = new System.Windows.Forms.FolderBrowserDialog();
                System.Windows.Forms.DialogResult OpenFolderResult = DiffractionFolder.ShowDialog();

                if (System.Windows.Forms.DialogResult.OK == OpenFolderResult)
                {
                    string[] ContainedFiles = System.IO.Directory.GetFiles(DiffractionFolder.SelectedPath, "*", System.IO.SearchOption.AllDirectories);

                    if (ContainedFiles.Length == 1)
                    {
                        BackgroundWorker OpenDiffractionPattern = new BackgroundWorker();

                        OpenDiffractionPattern.DoWork += OpenDiffractionPatternFile_Work;
                        OpenDiffractionPattern.ProgressChanged += OpenDiffractionPatternFile_ProgressChanged;
                        OpenDiffractionPattern.RunWorkerCompleted += OpenDiffractionPatternFile_Completed;
                        OpenDiffractionPattern.WorkerReportsProgress = true;

                        OpenDiffractionPattern.RunWorkerAsync(ContainedFiles[0]);
                    }
                    else
                    {
                        BackgroundWorker OpenMultipleDiffractionPattern = new BackgroundWorker();

                        OpenMultipleDiffractionPattern.DoWork += OpenMultipleDiffractionPatternFile_Work;
                        OpenMultipleDiffractionPattern.ProgressChanged += OpenMultipleDiffractionPatternFile_ProgressChanged;
                        OpenMultipleDiffractionPattern.RunWorkerCompleted += OpenMultipleDiffractionPatternFile_Completed;
                        OpenMultipleDiffractionPattern.WorkerReportsProgress = true;

                        OpenMultipleDiffractionPattern.RunWorkerAsync(ContainedFiles);
                    }
                }
            }
            else
            {
                MessageBox.Show("Since I am somehow smart I really would like to help you with finding and assigning peaks! But for that I need the crystallographic data first.\n Please be so kind and load it first", "No crystallographic data", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void OpenDiffractionPatternFile_Work(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            Pattern.DiffractionPattern NewDiffractionPattern = null;

            string PatternPath = (string)e.Argument;

            string[] State = { System.IO.Path.GetFileName(PatternPath), "L"};
            double Progress = 5.0;

            worker.ReportProgress(Convert.ToInt32(Progress), State);

            string FileExt = System.IO.Path.GetExtension(PatternPath);
            if(FileExt.ToLower() == ".dat" || FileExt.ToLower() == ".eth" || FileExt.ToLower() == ".chi")
            {
                try
                {
                    NewDiffractionPattern = new Pattern.DiffractionPattern(PatternPath, this.InvestigatedSample.ActualPatterId);

                    this.InvestigatedSample.DiffractionPatterns.Add(NewDiffractionPattern);
                }
                catch
                {
                    Progress = 0.0;
                    State[0] = "Finished";
                    State[1] = "C2";
                }
                finally
                {
                    worker.ReportProgress(Convert.ToInt32(Progress), State);
                }

            }
            else
            {
                Progress = 0.0;
                State[0] = "Finished";
                State[1] = "C1";

                worker.ReportProgress(Convert.ToInt32(Progress), State);
            }

            Progress = 50.0;
            State[0] = "Starting......";
            State[1] = "PF";

            worker.ReportProgress(Convert.ToInt32(Progress), State);

            if (NewDiffractionPattern != null)
            {
                if (CalScec.Properties.Settings.Default.UsedPeakDetectionId == 0)
                {
                    Analysis.Peaks.Detection.HyperDetection(NewDiffractionPattern);
                    Analysis.Peaks.Detection.AssociateFoundPeaksToHKL(NewDiffractionPattern, this.InvestigatedSample.CrystalData);
                    NewDiffractionPattern.SetRegions();
                }
                else if (CalScec.Properties.Settings.Default.UsedPeakDetectionId == 1)
                {
                    Analysis.Peaks.Detection.CIFDetection(NewDiffractionPattern, this.InvestigatedSample.CrystalData);
                    NewDiffractionPattern.SetRegions();
                }
                else
                {
                    char[] SepChars = { '.', '-', '_' };
                    string[] SplitedPatternName = NewDiffractionPattern.Name.Split(SepChars);

                    int PrevPatterIndex = -1;

                    for(int n = 0; n < this.InvestigatedSample.DiffractionPatterns.Count - 1; n++)
                    {
                        string[] SplitedPrevPatternName = this.InvestigatedSample.DiffractionPatterns[n].Name.Split(SepChars);

                        if(SplitedPatternName[2] == SplitedPrevPatternName[2])
                        {
                            PrevPatterIndex = n;
                        }
                    }

                    if(PrevPatterIndex != -1)
                    {
                        Analysis.Peaks.Detection.PrevPatternDetection(NewDiffractionPattern, this.InvestigatedSample.DiffractionPatterns[PrevPatterIndex]);
                        NewDiffractionPattern.SetRegionsFromPrevious(this.InvestigatedSample.DiffractionPatterns[PrevPatterIndex]);
                    }
                    else
                    {
                        if(this.InvestigatedSample.DiffractionPatterns.Count - 1 != 0)
                        {
                            Analysis.Peaks.Detection.PrevPatternDetection(NewDiffractionPattern, this.InvestigatedSample.DiffractionPatterns[0]);
                            NewDiffractionPattern.SetRegionsFromPrevious(this.InvestigatedSample.DiffractionPatterns[PrevPatterIndex]);
                        }
                        else
                        {
                            Analysis.Peaks.Detection.CIFDetection(NewDiffractionPattern, this.InvestigatedSample.CrystalData);
                            NewDiffractionPattern.SetRegions();
                        }
                    }
                }

                //NewDiffractionPattern.SetRegions();

                if(CalScec.Properties.Settings.Default.AutomaticPeakFit)
                {
                    for(int n = 0; n < NewDiffractionPattern.PeakRegions.Count; n++)
                    {
                        this.PeakFitWindow.AddRegionWithFit(NewDiffractionPattern.PeakRegions[n]);
                    }
                }
                else
                {
                    for (int n = 0; n < NewDiffractionPattern.PeakRegions.Count; n++)
                    {
                        this.PeakFitWindow.AddRegion(NewDiffractionPattern.PeakRegions[n]);
                    }
                }

                Progress = 50.0;
                State[0] = Convert.ToString(NewDiffractionPattern.FoundPeaks.Count) + " Peaks found!";
                State[1] = "PFF";

                worker.ReportProgress(Convert.ToInt32(Progress), State);
            }
            else
            {
                Progress = 50.0;
                State[0] = "Starting......";
                State[1] = "PFE";

                worker.ReportProgress(Convert.ToInt32(Progress), State);
            }
        }

        private void OpenDiffractionPatternFile_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.StatusProgress.Value = e.ProgressPercentage;
            string[] StateString = (string[])e.UserState;

            if(StateString[1] == "L")
            {
                this.StatusLog1.Content = "Loading File";
            }
            else if (StateString[1] == "C1")
            {
                this.ErrorLog1.Content = "Loading not possible";
                this.ErrorLog2.Content = "Not a .dat file";
            }
            else if (StateString[1] == "C2")
            {
                this.ErrorLog1.Content = "Loading not possible";
                this.ErrorLog2.Content = "Wrong File Format";
            }
            else if (StateString[1] == "PF")
            {
                this.StatusLog1.Content = "Searching peaks";
            }
            else if (StateString[1] == "PFF")
            {
                this.StatusLog1.Content = "Search completted";
            }
            else if (StateString[1] == "PFE")
            {
                this.ErrorLog1.Content = "Peak search not possible";
                this.ErrorLog2.Content = "Could not load diffraction pattern";
            }

            this.StatusLog2.Content = StateString[0];
        }

        private void OpenDiffractionPatternFile_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            this.InvestigatedSample.DiffractionPatterns.Sort((a, b) => (1) * a.Name.CompareTo(b.Name));
            DiffractionPatternList.Items.Refresh();
            this.StatusLog1.Foreground = System.Windows.Media.Brushes.DarkGreen;
            this.StatusLog1.Content = "Loading completed";
            this.StatusProgress.Value = 100;
            this.SetProgress();
        }

        private void OpenMultipleDiffractionPatternFile_Work(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            List<Pattern.DiffractionPattern> AddedPatterns = new List<Pattern.DiffractionPattern>();

            string[] PatternPaths = (string[])e.Argument;

            string[] State = { "Starting........", "L", "", Convert.ToString(PatternPaths.Length) };
            double Progress = 5.0;

            int LoadingCompletedNumber = 0;
            int LoadingFailedNumber = 0;
            foreach(string PP in PatternPaths )
            {
                string FileExt = System.IO.Path.GetExtension(PP);
                if (FileExt.ToLower() == ".dat" || FileExt.ToLower() == ".eth" || FileExt.ToLower() == ".chi")
                {
                    try
                    {
                        Pattern.DiffractionPattern NewDiffractionPattern = new Pattern.DiffractionPattern(PP, this.InvestigatedSample.ActualPatterId);

                        this.InvestigatedSample.DiffractionPatterns.Add(NewDiffractionPattern);
                        AddedPatterns.Add(NewDiffractionPattern);
                        LoadingCompletedNumber++;

                        State[0] = System.IO.Path.GetFileName(PP) + " Completed";
                        State[1] = "LN";
                        State[2] = Convert.ToString(LoadingCompletedNumber);
                    }
                    catch
                    {
                        LoadingFailedNumber++;
                        State[0] = System.IO.Path.GetFileName(PP) + " FAILED";
                        State[1] = "C2";
                        State[2] = Convert.ToString(LoadingFailedNumber);
                    }
                    finally
                    {
                        Progress += ((LoadingCompletedNumber + LoadingFailedNumber) / PatternPaths.Length) * 45;
                        
                        worker.ReportProgress(Convert.ToInt32(Progress), State);
                    }
                }
                else
                {
                    LoadingFailedNumber++;
                    Progress += ((LoadingCompletedNumber + LoadingFailedNumber) / PatternPaths.Length) * 45;
                    State[0] = System.IO.Path.GetFileName(PP) + " FAILED";
                    State[1] = "C1";
                    State[2] = Convert.ToString(LoadingFailedNumber);

                    worker.ReportProgress(Convert.ToInt32(Progress), State);
                }
            }

            State[0] = Convert.ToString(LoadingCompletedNumber);
            State[1] = "LF";
            State[2] = Convert.ToString(LoadingFailedNumber);
            worker.ReportProgress(Convert.ToInt32(Progress), State);

            if(AddedPatterns.Count != 0)
            {
                Parallel.For(0, AddedPatterns.Count, i =>
                    {
                        if (CalScec.Properties.Settings.Default.UsedPeakDetectionId == 0)
                        {
                            Analysis.Peaks.Detection.HyperDetection(AddedPatterns[i]);
                            Analysis.Peaks.Detection.AssociateFoundPeaksToHKL(AddedPatterns[i], this.InvestigatedSample.CrystalData);
                            AddedPatterns[i].SetRegions();
                        }
                        else if (CalScec.Properties.Settings.Default.UsedPeakDetectionId == 1)
                        {
                            Analysis.Peaks.Detection.CIFDetection(AddedPatterns[i], this.InvestigatedSample.CrystalData);
                            AddedPatterns[i].SetRegions();
                        }
                        else
                        {
                            char[] SepChars = { '.', '-', '_' };
                            string[] SplitedPatternName = AddedPatterns[i].Name.Split(SepChars);

                            int PrevPatterIndex = -1;

                            for (int n = 0; n < this.InvestigatedSample.DiffractionPatterns.Count - AddedPatterns.Count; n++)
                            {
                                string[] SplitedPrevPatternName = this.InvestigatedSample.DiffractionPatterns[n].Name.Split(SepChars);

                                if (SplitedPatternName[2] == SplitedPrevPatternName[2])
                                {
                                    PrevPatterIndex = n;
                                }
                            }

                            if (PrevPatterIndex != -1)
                            {
                                Analysis.Peaks.Detection.PrevPatternDetection(AddedPatterns[i], this.InvestigatedSample.DiffractionPatterns[PrevPatterIndex]);

                                AddedPatterns[i].SetRegionsFromPrevious(this.InvestigatedSample.DiffractionPatterns[PrevPatterIndex]);
                            }
                            else
                            {
                                if (this.InvestigatedSample.DiffractionPatterns.Count - AddedPatterns.Count != 0)
                                {
                                    Analysis.Peaks.Detection.PrevPatternDetection(AddedPatterns[i], this.InvestigatedSample.DiffractionPatterns[0]);

                                    AddedPatterns[i].SetRegionsFromPrevious(this.InvestigatedSample.DiffractionPatterns[PrevPatterIndex]);
                                }
                                else
                                {
                                    Analysis.Peaks.Detection.CIFDetection(AddedPatterns[i], this.InvestigatedSample.CrystalData);
                                    AddedPatterns[i].SetRegions();
                                }
                            }
                        }

                        //AddedPatterns[i].SetRegions();

                        if (CalScec.Properties.Settings.Default.AutomaticPeakFit)
                        {
                            for (int n = 0; n < AddedPatterns[i].PeakRegions.Count; n++)
                            {
                                this.PeakFitWindow.AddRegionWithFit(AddedPatterns[i].PeakRegions[n]);
                            }
                        }
                        else
                        {
                            for (int n = 0; n < AddedPatterns[i].PeakRegions.Count; n++)
                            {
                                this.PeakFitWindow.AddRegion(AddedPatterns[i].PeakRegions[n]);
                            }
                        }

                        Progress += (1 / AddedPatterns.Count) * 45;

                        State[0] = "Pattern " + Convert.ToString(i) + " Completed";
                        State[1] = "PF";
                        State[2] = Convert.ToString(AddedPatterns[i].FoundPeaks.Count);
                        worker.ReportProgress(Convert.ToInt32(Progress), State);
                    });
            }
        }

        private void OpenMultipleDiffractionPatternFile_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.StatusProgress.Value = e.ProgressPercentage;
            string[] StateString = (string[])e.UserState;

            this.StatusLog2.Content = StateString[0];
            if (StateString[1] == "L")
            {
                this.StatusLog1.Content = "Loading File 0 / " + StateString[3];
            }
            else if (StateString[1] == "C1")
            {
                this.ErrorLog1.Content = "Loading not possible";
                this.ErrorLog2.Content = "Files  " + StateString[2] + " / " + StateString[3];
            }
            else if (StateString[1] == "LN")
            {
                this.StatusLog1.Content = "Loading File " + StateString[2] + " / " + StateString[3];
            }
            else if (StateString[1] == "LF")
            {
                this.StatusLog2.Content = StateString[0] + " / " + StateString[3] + "  Succesful";
                this.StatusLog1.Content = "Starting the peaks search";
            }
            else if (StateString[1] == "PF")
            {
                this.StatusLog1.Content = StateString[2] + "  Peaks found";
            }
            else if (StateString[1] == "C2")
            {
                this.ErrorLog1.Content = "Wrong file format";
                this.ErrorLog2.Content = "Files  " + StateString[2] + " / " + StateString[3];
            }

            
        }

        private void OpenMultipleDiffractionPatternFile_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            this.InvestigatedSample.DiffractionPatterns.Sort((a, b) => (1) * a.Name.CompareTo(b.Name));
            DiffractionPatternList.Items.Refresh();
            this.StatusLog1.Foreground = System.Windows.Media.Brushes.DarkGreen;
            this.StatusLog1.Content = "Loading completed";
            int PeaksInTotal = 0;
            foreach (Pattern.DiffractionPattern DP in this.InvestigatedSample.DiffractionPatterns)
            {
                PeaksInTotal += DP.FoundPeaks.Count;
            }

            this.StatusLog2.Content = Convert.ToString(PeaksInTotal) + " Peaks were found!";

            this.StatusProgress.Value = 100;
            this.SetProgress();
        }

        private void ChangeSamplePatternData_Click(object sender, RoutedEventArgs e)
        {
            Pattern.DiffractionDataWindow DDW = new Pattern.DiffractionDataWindow(this.InvestigatedSample);

            DDW.ShowDialog();
        }

        private void OpenReflexInformationFile_Click(object sender, RoutedEventArgs e)
        {
            if (this.InvestigatedSample.CrystalData.Count != 0)
            {
                if (this.InvestigatedSample.Area != 0)
                {
                    Microsoft.Win32.OpenFileDialog OpenReflexInformationFile = new Microsoft.Win32.OpenFileDialog();
                    OpenReflexInformationFile.Multiselect = false;
                    Nullable<bool> Opened = OpenReflexInformationFile.ShowDialog();

                    if (Opened == true)
                    {
                        BackgroundWorker OpenReflexInformation = new BackgroundWorker();

                        OpenReflexInformation.DoWork += OpenReflexInformationFile_Work;
                        OpenReflexInformation.ProgressChanged += OpenReflexInformationFile_ProgressChanged;
                        OpenReflexInformation.RunWorkerCompleted += OpenReflexInformationFile_Completed;
                        OpenReflexInformation.WorkerReportsProgress = true;

                        OpenReflexInformation.RunWorkerAsync(OpenReflexInformationFile.FileName);
                    }
                }
                else
                {
                    MessageBox.Show("The sample area is not defined! Please give the correct sample area in order to proceed.", "Sample Area Unknown", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            }
            else
            {
                MessageBox.Show("Since I am somehow smart I really would like to help you with finding and assigning peaks! But for that I need the crystallographic data first.\n Please be so kind and load it first", "No crystallographic data", MessageBoxButton.OK, MessageBoxImage.Stop);
            }

        }

        private void OpenReflexInformationFile_Work(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            string PatternPath = (string)e.Argument;
            string fileName = System.IO.Path.GetFileName(PatternPath);
            string[] State = { fileName, "L" };
            double Progress = 5.0;

            worker.ReportProgress(Convert.ToInt32(Progress), State);

            string FileExt = System.IO.Path.GetExtension(PatternPath);
            if (FileExt.ToLower() == ".csv" || FileExt.ToLower() == ".txt")
            {
                try
                {
                    string[] PatternFileLines = System.IO.File.ReadLines(PatternPath).ToArray();
                    int lineIndex = 0;

                    Progress = 5.0;
                    State[0] = Convert.ToString(PatternFileLines.Count()) + " Peaks found!";
                    State[1] = "LP";

                    for (int n = 0; n < PatternFileLines.Count(); n++)
                    {
                        if (PatternFileLines[n][0] != '#' && PatternFileLines[n][0] != '%')
                        {
                            //Peak File structure
                            //Trennzeichen (;|,| |\t)
                            //[0]HKL; [1]Position; [2]FWHM; [3]Area; [4]Chi; [5]Omega; [6]Phi; [7]Load as Force; [8] Macroextension;
                            char[] sepChars = { ';', '\t' };

                            string[] lineData = PatternFileLines[n].Split(sepChars);
                            for ( int i = 0; i < this.InvestigatedSample.CrystalData.Count; i++)
                            {
                                for(int j= 0; j < this.InvestigatedSample.CrystalData[i].HKLList.Count; j++)
                                {
                                    string compString = this.InvestigatedSample.CrystalData[i].SymmetryGroup + " ( " + this.InvestigatedSample.CrystalData[i].HKLList[j].H + ", " + this.InvestigatedSample.CrystalData[i].HKLList[j].K + ", " + this.InvestigatedSample.CrystalData[i].HKLList[j].L + " )";

                                    if(compString == lineData[0])
                                    {
                                        double areaE = this.InvestigatedSample.Area;
                                        if (areaE != 0.0)
                                        {
                                            Pattern.DiffractionPattern NewDiffractionPattern = new Pattern.DiffractionPattern(this.InvestigatedSample.ActualPatterId);
                                            //string indexString = lineData[0];
                                            //int hIndex = Convert.ToInt32(indexString[0].ToString());
                                            //int kIndex = Convert.ToInt32(indexString[1].ToString());
                                            //int lIndex = Convert.ToInt32(indexString[2].ToString());

                                            NewDiffractionPattern.ChiAngle = Convert.ToDouble(lineData[4]);
                                            NewDiffractionPattern.OmegaAngle = Convert.ToDouble(lineData[5]);
                                            NewDiffractionPattern.PhiSampleAngle = Convert.ToDouble(lineData[6]);
                                            NewDiffractionPattern.Force = Convert.ToDouble(lineData[7]);
                                            NewDiffractionPattern.Stress = NewDiffractionPattern.Force / this.InvestigatedSample.Area;
                                            NewDiffractionPattern.MacroExtension = Convert.ToDouble(lineData[8]);

                                            Pattern.Counts FittingCounts = new Pattern.Counts();

                                            double position = Convert.ToDouble(lineData[1]);
                                            double fWHM = Convert.ToDouble(lineData[2]);
                                            double area = Convert.ToDouble(lineData[3]);

                                            Analysis.Peaks.DiffractionPeak DPeak = new Analysis.Peaks.DiffractionPeak(0, position, area, 0, FittingCounts);

                                            DPeak.PFunction.Angle = position;
                                            DPeak.PFunction.FWHM = fWHM;
                                            DPeak.PFunction.Intensity = area;

                                            

                                            NewDiffractionPattern.Name = lineData[0] + " Ext=" + NewDiffractionPattern.MacroExtension.ToString("F3");
                                            DPeak.AddHKLAssociation(this.InvestigatedSample.CrystalData[i].HKLList[j], this.InvestigatedSample.CrystalData[i]);
                                            NewDiffractionPattern.FoundPeaks.Add(DPeak);
                                            this.InvestigatedSample.DiffractionPatterns.Add(NewDiffractionPattern);

                                            Progress = (0.95 * Convert.ToDouble(lineIndex / PatternFileLines.Count())) + 0.05;
                                            State[0] = "Peak " + lineIndex.ToString() + " of " + PatternFileLines.Count().ToString();
                                            State[1] = "LP";

                                            worker.ReportProgress(Convert.ToInt32(Progress), State);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {
                    Progress = 0.0;
                    State[0] = "Finished";
                    State[1] = "C2";
                }
                finally
                {
                    //this.InvestigatedSample.DiffractionPatterns.Sort((a, b) => a.MacroExtension.CompareTo(b.MacroExtension));
                    for (int n = 0; n < this.InvestigatedSample.DiffractionPatterns.Count; n++)
                    {
                        this.InvestigatedSample.DiffractionPatterns[n].MacroStrain = (this.InvestigatedSample.DiffractionPatterns[n].MacroExtension - this.InvestigatedSample.DiffractionPatterns[0].MacroExtension) / CalScec.Properties.Settings.Default.ExstensionBase;
                    }
                    worker.ReportProgress(Convert.ToInt32(Progress), State);
                }
            }

            #region Old Stuff
            //if (this.PhaseSelection.SelectedIndex != -1)
            //{
            //    int selectedPhase = this.PhaseSelection.SelectedIndex;

            //    //string PatternPath = (string)e.Argument;
            //    //string fileName = System.IO.Path.GetFileName(PatternPath);
            //    //string[] State = { fileName, "L" };
            //    //double Progress = 5.0;

            //    //worker.ReportProgress(Convert.ToInt32(Progress), State);

            //    //string FileExt = System.IO.Path.GetExtension(PatternPath);
            //    if (FileExt.ToLower() == ".csv" || FileExt.ToLower() == ".txt")
            //    {
            //        try
            //        {
            //            //this.PatternCounts = new Counts();

            //            string[] PatternFileLines = System.IO.File.ReadLines(PatternPath).ToArray();
            //            int lineIndex = 0;

            //            Progress = 5.0;
            //            State[0] = Convert.ToString(PatternFileLines.Count()) + " Peaks found!";
            //            State[1] = "LP";

            //            //int hIndex = Convert.ToInt32(fileName[0].ToString());
            //            //int kIndex = Convert.ToInt32(fileName[1].ToString());
            //            //int lIndex = Convert.ToInt32(fileName[2].ToString());

            //            foreach (string line in PatternFileLines)
            //            {
            //                if (line[0] != '#' && line[0] != '%')
            //                {
            //                    //Peak File structure
            //                    //Trennzeichen (;|,| |\t)
            //                    //[0]HKL; [1]Position; [2]FWHM; [3]Area; [4]Chi; [5]Omega; [6]Phi; [7]Load as Force; [8] Macroextension;
            //                    char[] sepChars = { ';', ',', '\t' };

            //                    string[] lineData = line.Split(sepChars);

            //                    try
            //                    {
            //                        double areaE = Convert.ToDouble(lineData[5]);
            //                        if (areaE != 0.0)
            //                        {
            //                            Pattern.DiffractionPattern NewDiffractionPattern = new Pattern.DiffractionPattern(this.InvestigatedSample.ActualPatterId);
            //                            string indexString = lineData[0];
            //                            int hIndex = Convert.ToInt32(indexString[0].ToString());
            //                            int kIndex = Convert.ToInt32(indexString[1].ToString());
            //                            int lIndex = Convert.ToInt32(indexString[2].ToString());

            //                            NewDiffractionPattern.ChiAngle = Convert.ToDouble(lineData[4]);
            //                            NewDiffractionPattern.OmegaAngle = Convert.ToDouble(lineData[5]);
            //                            NewDiffractionPattern.PhiSampleAngle = Convert.ToDouble(lineData[6]);
            //                            NewDiffractionPattern.Force = Convert.ToDouble(lineData[7]);
            //                            NewDiffractionPattern.Stress = NewDiffractionPattern.Force / this.InvestigatedSample.Area;
            //                            NewDiffractionPattern.MacroExtension = Convert.ToDouble(lineData[8]);

            //                            Pattern.Counts FittingCounts = new Pattern.Counts();

            //                            double position = Convert.ToDouble(lineData[1]);
            //                            double fWHM = Convert.ToDouble(lineData[2]);
            //                            double area = Convert.ToDouble(lineData[3]);

            //                            Analysis.Peaks.DiffractionPeak DPeak = new Analysis.Peaks.DiffractionPeak(0, position, area, 0, FittingCounts);

            //                            DPeak.PFunction.Angle = position;
            //                            DPeak.PFunction.FWHM = fWHM;
            //                            DPeak.PFunction.Intensity = area;

            //                            string hKLAssociation = "( " + Convert.ToString(hIndex) + " " + Convert.ToString(kIndex) + " " + Convert.ToString(lIndex) + " )"; ;

            //                            NewDiffractionPattern.Name = hKLAssociation + " Ext=" + NewDiffractionPattern.MacroExtension.ToString("F3");
            //                            for (int n = 0; n < this.InvestigatedSample.CrystalData[selectedPhase].HKLList.Count; n++)
            //                            {
            //                                if (hKLAssociation == this.InvestigatedSample.CrystalData[selectedPhase].HKLList[n].HKLString)
            //                                {
            //                                    DPeak.AddHKLAssociation(this.InvestigatedSample.CrystalData[selectedPhase].HKLList[n], this.InvestigatedSample.CrystalData[selectedPhase]);
            //                                    break;
            //                                }
            //                            }
            //                            NewDiffractionPattern.FoundPeaks.Add(DPeak);
            //                            this.InvestigatedSample.DiffractionPatterns.Add(NewDiffractionPattern);

            //                            Progress = (0.95 * Convert.ToDouble(lineIndex / PatternFileLines.Count())) + 0.05;
            //                            State[0] = "Peak " + lineIndex.ToString() + " of " + PatternFileLines.Count().ToString();
            //                            State[1] = "LP";

            //                            worker.ReportProgress(Convert.ToInt32(Progress), State);
            //                        }
            //                    }
            //                    catch
            //                    {
            //                        Progress = (0.95 * Convert.ToDouble(lineIndex / PatternFileLines.Count())) + 0.05;
            //                        State[0] = "Continue";
            //                        State[1] = "PF";

            //                        worker.ReportProgress(Convert.ToInt32(Progress), State);
            //                    }
            //                }

            //            }
            //        }
            //        catch
            //        {
            //            Progress = 0.0;
            //            State[0] = "Finished";
            //            State[1] = "C2";
            //        }
            //        finally
            //        {
            //            //this.InvestigatedSample.DiffractionPatterns.Sort((a, b) => a.MacroExtension.CompareTo(b.MacroExtension));
            //            for (int n = 0; n < this.InvestigatedSample.DiffractionPatterns.Count; n++)
            //            {
            //                this.InvestigatedSample.DiffractionPatterns[n].MacroStrain = (this.InvestigatedSample.DiffractionPatterns[n].MacroExtension - this.InvestigatedSample.DiffractionPatterns[0].MacroExtension) / CalScec.Properties.Settings.Default.ExstensionBase;
            //            }
            //            worker.ReportProgress(Convert.ToInt32(Progress), State);
            //        }

            //    }
            //    else
            //    {
            //        Progress = 0.0;
            //        State[0] = "Finished";
            //        State[1] = "C1";

            //        worker.ReportProgress(Convert.ToInt32(Progress), State);
            //    }

            //    Progress = 50.0;
            //    State[0] = "All Peaks loaded!";
            //    State[1] = "PFF";

            //    worker.ReportProgress(Convert.ToInt32(Progress), State);
            //}

            #endregion
        }

        private void OpenReflexInformationFile_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.StatusProgress.Value = e.ProgressPercentage;
            string[] StateString = (string[])e.UserState;

            if (StateString[1] == "L")
            {
                this.StatusLog1.Content = "Loading File";
            }
            else if (StateString[1] == "C1")
            {
                this.ErrorLog1.Content = "Loading not possible";
                this.ErrorLog2.Content = "Not a .dat file";
            }
            else if (StateString[1] == "C2")
            {
                this.ErrorLog1.Content = "Loading not possible";
                this.ErrorLog2.Content = "Wrong File Format";
            }
            else if (StateString[1] == "LP")
            {
                this.StatusLog1.Content = "Loading peaks";
            }
            else if (StateString[1] == "PF")
            {
                this.ErrorLog1.Content = "Peak loading failed";
                this.ErrorLog2.Content = "Wrong data format detected";
            }
            else if (StateString[1] == "PFF")
            {
                this.StatusLog1.Content = "Loading completted";
            }
            else if (StateString[1] == "PFE")
            {
                this.ErrorLog1.Content = "Peak search not possible";
                this.ErrorLog2.Content = "Could not load diffraction pattern";
            }

            this.StatusLog2.Content = StateString[0];
        }

        private void OpenReflexInformationFile_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            //this.InvestigatedSample.DiffractionPatterns.Sort((a, b) => (1) * a.Name.CompareTo(b.Name));
            DiffractionPatternList.Items.Refresh();
            this.StatusLog1.Foreground = System.Windows.Media.Brushes.DarkGreen;
            this.StatusLog1.Content = "Loading completed";
            this.StatusProgress.Value = 100;
            this.SetProgress();
        }

        #region Peak editing

        private void MainPlot_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this._peakAddingActive)
            {
                Point TMP = e.GetPosition(MainDiffPlot);
                OxyPlot.ScreenPoint ScPoint = new OxyPlot.ScreenPoint(TMP.X, TMP.Y);

                OxyPlot.TrackerHitResult NearestHitResult = this.MainDiffPlot.Model.Series[0].GetNearestPoint(ScPoint, false);
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

                if(this._peakAddingActive)
                {
                    _newPeakCharacteristic[0] = SelectedXValue;
                    _newPeakCharacteristic[1] = NearestDataPoint.Y;
                    NewLA.Text = "New peak position";
                    if (PeakManipulationLine.Count == 0)
                    {
                        PeakManipulationLine.Add(NewLA);
                    }
                    else
                    {
                        PeakManipulationLine.Clear();
                        PeakManipulationLine.Add(NewLA);
                    }
                }

                RefreshAnnotationToPlot();
            }
        }

        private void SelectNewPeak_Click(object sender, RoutedEventArgs e)
        {
            if(_peakAddingActive)
            {
                _peakAddingActive = false;
            }
            else
            {
                _peakAddingActive = true;
            }
        }

        private void AddNewPeak_Click(object sender, RoutedEventArgs e)
        {
            if (_peakAddingActive && this.DiffractionPatternList.SelectedItems.Count == 1 && PeakManipulationLine.Count == 1)
            {
                double FWHMD = Tools.Calculation.GetEstimatedFWHM(_newPeakCharacteristic[0]);

                Pattern.DiffractionPattern ActPattern = this.DiffractionPatternList.SelectedItem as Pattern.DiffractionPattern;
                
                Pattern.Counts NewFittingCounts = ActPattern.PatternCounts.GetRange(_newPeakCharacteristic[0] - (FWHMD * CalScec.Properties.Settings.Default.PeakWidthFWHM), _newPeakCharacteristic[0] + (FWHMD * CalScec.Properties.Settings.Default.PeakWidthFWHM));
                

                double AngleToCount = ActPattern.PatternCounts[1][0] - ActPattern.PatternCounts[0][0];

                Analysis.Peaks.DiffractionPeak NewDifPeak = new Analysis.Peaks.DiffractionPeak(Convert.ToInt32(_newPeakCharacteristic[0] / AngleToCount), _newPeakCharacteristic[0], _newPeakCharacteristic[0], _newPeakCharacteristic[1] - NewFittingCounts[0][1], NewFittingCounts);
                NewDifPeak.PFunction.functionType = 2;
                NewDifPeak.AssociatedPatternName = ActPattern.Name;
                ActPattern.FoundPeaks.Add(NewDifPeak);

                Analysis.Peaks.Functions.PeakRegionFunction NewPRF = new Analysis.Peaks.Functions.PeakRegionFunction(NewDifPeak.PFunction.ConstantBackground, NewFittingCounts, NewDifPeak.PFunction);
                NewPRF.AssociatedPatternName = ActPattern.Name;

                bool regionMerged = false;
                for(int n = 0; n < ActPattern.PeakRegions.Count; n++)
                {
                    bool FirstCheck = NewPRF.FittingCounts[0][0] > ActPattern.PeakRegions[n].FittingCounts[0][0] && NewPRF.FittingCounts[0][0] < ActPattern.PeakRegions[n].FittingCounts[ActPattern.PeakRegions[n].FittingCounts.Count - 1][0];
                    bool SecondCheck = NewPRF.FittingCounts[NewPRF.FittingCounts.Count - 1][0] < ActPattern.PeakRegions[n].FittingCounts[ActPattern.PeakRegions[n].FittingCounts.Count - 1][0] && NewPRF.FittingCounts[NewPRF.FittingCounts.Count - 1][0] > ActPattern.PeakRegions[n].FittingCounts[0][0];
                    bool ThirdCheck = NewPRF.FittingCounts[0][0] < ActPattern.PeakRegions[n].FittingCounts[0][0] && NewPRF.FittingCounts[NewPRF.FittingCounts.Count - 1][0] > ActPattern.PeakRegions[n].FittingCounts[ActPattern.PeakRegions[n].FittingCounts.Count - 1][0];
                    if (FirstCheck || SecondCheck || ThirdCheck)
                    {
                        ActPattern.PeakRegions[n].MergeRegions(NewPRF);
                        regionMerged = true;
                        break;
                    }
                }

                if(!regionMerged)
                {
                    ActPattern.PeakRegions.Add(NewPRF);
                }

                PeakManipulationLine.Clear();
                RefreshAnnotationToPlot();
            }
        }

        #endregion

        #endregion

        #region Crystallographic data

        private void OpenCrystallographyDataFile_Click(object sender, RoutedEventArgs e)
        {
            if (InvestigatedSample.DiffractionPatterns.Count == 0)
            {
                if (this.InvestigatedSample.CrystalData.Count == 0)
                {
                    Microsoft.Win32.OpenFileDialog OpenDiffractionFile = new Microsoft.Win32.OpenFileDialog();
                    OpenDiffractionFile.Multiselect = false;
                    OpenDiffractionFile.DefaultExt = ".cif";
                    Nullable<bool> Opened = OpenDiffractionFile.ShowDialog();

                    if (Opened == true)
                    {
                        string FilePath = OpenDiffractionFile.FileName;

                        string FileExt = System.IO.Path.GetExtension(FilePath);
                        if (FileExt.ToLower() == ".cif")
                        {
                            this.InvestigatedSample.CrystalData.Add(new DataManagment.CrystalData.CODData(FilePath));

                            this.InvestigatedSample.DiffractionConstants.Add(new List<Analysis.Stress.Microsopic.REK>());
                            this.InvestigatedSample.DiffractionConstantsTexture.Add(new List<Analysis.Stress.Microsopic.REK>());

                            this.InvestigatedSample.StressTransitionFactors.Add(MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense<double>(6, 6, 0.0));

                            this.InvestigatedSample.VoigtTensorData.Add(new Analysis.Stress.Microsopic.ElasticityTensors());
                            this.InvestigatedSample.ReussTensorData.Add(new Analysis.Stress.Microsopic.ElasticityTensors());
                            this.InvestigatedSample.HillTensorData.Add(new Analysis.Stress.Microsopic.ElasticityTensors());
                            this.InvestigatedSample.KroenerTensorData.Add(new Analysis.Stress.Microsopic.ElasticityTensors());
                            this.InvestigatedSample.DeWittTensorData.Add(new Analysis.Stress.Microsopic.ElasticityTensors());
                            this.InvestigatedSample.GeometricHillTensorData.Add(new Analysis.Stress.Microsopic.ElasticityTensors());

                            switch (this.InvestigatedSample.CrystalData[this.InvestigatedSample.CrystalData.Count - 1].SymmetryGroup)
                            {
                                case "F m -3 m":
                                    this.InvestigatedSample.VoigtTensorData[this.InvestigatedSample.VoigtTensorData.Count - 1].Symmetry = "cubic";
                                    this.InvestigatedSample.ReussTensorData[this.InvestigatedSample.ReussTensorData.Count - 1].Symmetry = "cubic";
                                    this.InvestigatedSample.HillTensorData[this.InvestigatedSample.HillTensorData.Count - 1].Symmetry = "cubic";
                                    this.InvestigatedSample.KroenerTensorData[this.InvestigatedSample.KroenerTensorData.Count - 1].Symmetry = "cubic";
                                    this.InvestigatedSample.DeWittTensorData[this.InvestigatedSample.DeWittTensorData.Count - 1].Symmetry = "cubic";
                                    this.InvestigatedSample.GeometricHillTensorData[this.InvestigatedSample.GeometricHillTensorData.Count - 1].Symmetry = "cubic";
                                    break;
                                case "F d -3 m":
                                    this.InvestigatedSample.VoigtTensorData[this.InvestigatedSample.VoigtTensorData.Count - 1].Symmetry = "cubic";
                                    this.InvestigatedSample.ReussTensorData[this.InvestigatedSample.ReussTensorData.Count - 1].Symmetry = "cubic";
                                    this.InvestigatedSample.HillTensorData[this.InvestigatedSample.HillTensorData.Count - 1].Symmetry = "cubic";
                                    this.InvestigatedSample.KroenerTensorData[this.InvestigatedSample.KroenerTensorData.Count - 1].Symmetry = "cubic";
                                    this.InvestigatedSample.DeWittTensorData[this.InvestigatedSample.DeWittTensorData.Count - 1].Symmetry = "cubic";
                                    this.InvestigatedSample.GeometricHillTensorData[this.InvestigatedSample.GeometricHillTensorData.Count - 1].Symmetry = "cubic";
                                    break;
                                case "I m -3 m":
                                    this.InvestigatedSample.VoigtTensorData[this.InvestigatedSample.VoigtTensorData.Count - 1].Symmetry = "cubic";
                                    this.InvestigatedSample.ReussTensorData[this.InvestigatedSample.ReussTensorData.Count - 1].Symmetry = "cubic";
                                    this.InvestigatedSample.HillTensorData[this.InvestigatedSample.HillTensorData.Count - 1].Symmetry = "cubic";
                                    this.InvestigatedSample.KroenerTensorData[this.InvestigatedSample.KroenerTensorData.Count - 1].Symmetry = "cubic";
                                    this.InvestigatedSample.DeWittTensorData[this.InvestigatedSample.DeWittTensorData.Count - 1].Symmetry = "cubic";
                                    this.InvestigatedSample.GeometricHillTensorData[this.InvestigatedSample.GeometricHillTensorData.Count - 1].Symmetry = "cubic";
                                    break;
                                case "P 63/m m c":
                                    this.InvestigatedSample.VoigtTensorData[this.InvestigatedSample.VoigtTensorData.Count - 1].Symmetry = "hexagonal";
                                    this.InvestigatedSample.ReussTensorData[this.InvestigatedSample.ReussTensorData.Count - 1].Symmetry = "hexagonal";
                                    this.InvestigatedSample.HillTensorData[this.InvestigatedSample.HillTensorData.Count - 1].Symmetry = "hexagonal";
                                    this.InvestigatedSample.KroenerTensorData[this.InvestigatedSample.KroenerTensorData.Count - 1].Symmetry = "hexagonal";
                                    this.InvestigatedSample.DeWittTensorData[this.InvestigatedSample.DeWittTensorData.Count - 1].Symmetry = "hexagonal";
                                    this.InvestigatedSample.GeometricHillTensorData[this.InvestigatedSample.GeometricHillTensorData.Count - 1].Symmetry = "hexagonal";
                                    break;
                            }


                            ComboBoxItem presentPhase = new ComboBoxItem();
                            presentPhase.Content = this.InvestigatedSample.CrystalData[0].Name;
                            this.PhaseSelection.Items.Add(presentPhase);
                            this.InvestigatedSample.CrystalData[0].PhaseFraction = 1;
                            this.PhaseSelection.SelectedIndex = 0;
                            this.InclusionTypeSelection.SelectedIndex = 0;
                        }
                    }
                }
                else
                {

                    MessageBoxResult Result = MessageBox.Show("You already have loaded some crystallographic data! Do you want to add more data?", "Need more crystallographic data???", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                    if(Result == MessageBoxResult.Yes)
                    {
                        Microsoft.Win32.OpenFileDialog OpenDiffractionFile = new Microsoft.Win32.OpenFileDialog();
                        OpenDiffractionFile.Multiselect = false;
                        OpenDiffractionFile.DefaultExt = ".cif";
                        Nullable<bool> Opened = OpenDiffractionFile.ShowDialog();

                        if (Opened == true)
                        {
                            string FilePath = OpenDiffractionFile.FileName;

                            string FileExt = System.IO.Path.GetExtension(FilePath);
                            if (FileExt.ToLower() == ".cif")
                            {
                                this.InvestigatedSample.CrystalData.Add(new DataManagment.CrystalData.CODData(FilePath));

                                this.InvestigatedSample.DiffractionConstants.Add(new List<Analysis.Stress.Microsopic.REK>());
                                this.InvestigatedSample.DiffractionConstantsTexture.Add(new List<Analysis.Stress.Microsopic.REK>());

                                this.InvestigatedSample.VoigtTensorData.Add(new Analysis.Stress.Microsopic.ElasticityTensors());
                                this.InvestigatedSample.ReussTensorData.Add(new Analysis.Stress.Microsopic.ElasticityTensors());
                                this.InvestigatedSample.HillTensorData.Add(new Analysis.Stress.Microsopic.ElasticityTensors());
                                this.InvestigatedSample.KroenerTensorData.Add(new Analysis.Stress.Microsopic.ElasticityTensors());
                                this.InvestigatedSample.DeWittTensorData.Add(new Analysis.Stress.Microsopic.ElasticityTensors());
                                this.InvestigatedSample.GeometricHillTensorData.Add(new Analysis.Stress.Microsopic.ElasticityTensors());

                                switch (this.InvestigatedSample.CrystalData[this.InvestigatedSample.CrystalData.Count - 1].SymmetryGroup)
                                {
                                    case "F m -3 m":
                                        this.InvestigatedSample.VoigtTensorData[this.InvestigatedSample.VoigtTensorData.Count - 1].Symmetry = "cubic";
                                        this.InvestigatedSample.ReussTensorData[this.InvestigatedSample.ReussTensorData.Count - 1].Symmetry = "cubic";
                                        this.InvestigatedSample.HillTensorData[this.InvestigatedSample.HillTensorData.Count - 1].Symmetry = "cubic";
                                        this.InvestigatedSample.KroenerTensorData[this.InvestigatedSample.KroenerTensorData.Count - 1].Symmetry = "cubic";
                                        this.InvestigatedSample.DeWittTensorData[this.InvestigatedSample.DeWittTensorData.Count - 1].Symmetry = "cubic";
                                        this.InvestigatedSample.GeometricHillTensorData[this.InvestigatedSample.GeometricHillTensorData.Count - 1].Symmetry = "cubic";
                                        break;
                                    case "F d -3 m":
                                        this.InvestigatedSample.VoigtTensorData[this.InvestigatedSample.VoigtTensorData.Count - 1].Symmetry = "cubic";
                                        this.InvestigatedSample.ReussTensorData[this.InvestigatedSample.ReussTensorData.Count - 1].Symmetry = "cubic";
                                        this.InvestigatedSample.HillTensorData[this.InvestigatedSample.HillTensorData.Count - 1].Symmetry = "cubic";
                                        this.InvestigatedSample.KroenerTensorData[this.InvestigatedSample.KroenerTensorData.Count - 1].Symmetry = "cubic";
                                        this.InvestigatedSample.DeWittTensorData[this.InvestigatedSample.DeWittTensorData.Count - 1].Symmetry = "cubic";
                                        this.InvestigatedSample.GeometricHillTensorData[this.InvestigatedSample.GeometricHillTensorData.Count - 1].Symmetry = "cubic";
                                        break;
                                    case "I m -3 m":
                                        this.InvestigatedSample.VoigtTensorData[this.InvestigatedSample.VoigtTensorData.Count - 1].Symmetry = "cubic";
                                        this.InvestigatedSample.ReussTensorData[this.InvestigatedSample.ReussTensorData.Count - 1].Symmetry = "cubic";
                                        this.InvestigatedSample.HillTensorData[this.InvestigatedSample.HillTensorData.Count - 1].Symmetry = "cubic";
                                        this.InvestigatedSample.KroenerTensorData[this.InvestigatedSample.KroenerTensorData.Count - 1].Symmetry = "cubic";
                                        this.InvestigatedSample.DeWittTensorData[this.InvestigatedSample.DeWittTensorData.Count - 1].Symmetry = "cubic";
                                        this.InvestigatedSample.GeometricHillTensorData[this.InvestigatedSample.GeometricHillTensorData.Count - 1].Symmetry = "cubic";
                                        break;
                                    case "P 63/m m c":
                                        this.InvestigatedSample.VoigtTensorData[this.InvestigatedSample.VoigtTensorData.Count - 1].Symmetry = "hexagonal";
                                        this.InvestigatedSample.ReussTensorData[this.InvestigatedSample.ReussTensorData.Count - 1].Symmetry = "hexagonal";
                                        this.InvestigatedSample.HillTensorData[this.InvestigatedSample.HillTensorData.Count - 1].Symmetry = "hexagonal";
                                        this.InvestigatedSample.KroenerTensorData[this.InvestigatedSample.KroenerTensorData.Count - 1].Symmetry = "hexagonal";
                                        this.InvestigatedSample.DeWittTensorData[this.InvestigatedSample.DeWittTensorData.Count - 1].Symmetry = "hexagonal";
                                        this.InvestigatedSample.GeometricHillTensorData[this.InvestigatedSample.GeometricHillTensorData.Count - 1].Symmetry = "hexagonal";
                                        break;
                                }


                                ComboBoxItem presentPhase = new ComboBoxItem();
                                presentPhase.Content = this.InvestigatedSample.CrystalData[this.InvestigatedSample.CrystalData.Count - 1].Name;
                                this.PhaseSelection.Items.Add(presentPhase);
                            }
                        }
                    }
                    else if (Result == MessageBoxResult.No)
                    {
                        MessageBoxResult NextResult = MessageBox.Show("Do you wish to delete your selected Data?", "Wrong crystallographic data???", MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if (MessageBoxResult.Yes == NextResult)
                        {
                            this.InvestigatedSample.CrystalData.Clear();

                            this.InvestigatedSample.DiffractionConstants.Clear();

                            this.InvestigatedSample.VoigtTensorData.Clear();
                            this.InvestigatedSample.ReussTensorData.Clear();
                            this.InvestigatedSample.HillTensorData.Clear();
                            this.InvestigatedSample.KroenerTensorData.Clear();
                            this.InvestigatedSample.DeWittTensorData.Clear();
                            this.InvestigatedSample.GeometricHillTensorData.Clear();

                            this.PhaseSelection.Items.Clear();
                        }
                    }
                }

                PlotHKLToPlot();
                this.SetProgress();
            }
            else
            {
                MessageBox.Show("You have already loaded some diffraction patterns!\n You will get into trouble if you change your crystallographic data!\n Therefore I wont allow such crazy things.", "Patterns already loaded!", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void OpenCrystallographyDataDatabase_Click(object sender, RoutedEventArgs e)
        {
            if (InvestigatedSample.DiffractionPatterns.Count == 0)
            {
                if (this.InvestigatedSample.CrystalData.Count == 0)
                {
                    DataManagment.CODSql.CODSearchWindow CrystalSelection = new DataManagment.CODSql.CODSearchWindow();

                    CrystalSelection.ShowDialog();

                    if (CrystalSelection.SelectionFinished)
                    {
                        Tools.Calculation.AddHKLList(CrystalSelection.SelectedData);

                        this.InvestigatedSample.CrystalData.Add(CrystalSelection.SelectedData);

                        this.InvestigatedSample.DiffractionConstants.Add(new List<Analysis.Stress.Microsopic.REK>());
                        this.InvestigatedSample.DiffractionConstantsTexture.Add(new List<Analysis.Stress.Microsopic.REK>());

                        this.InvestigatedSample.VoigtTensorData.Add(new Analysis.Stress.Microsopic.ElasticityTensors());
                        this.InvestigatedSample.ReussTensorData.Add(new Analysis.Stress.Microsopic.ElasticityTensors());
                        this.InvestigatedSample.HillTensorData.Add(new Analysis.Stress.Microsopic.ElasticityTensors());
                        this.InvestigatedSample.KroenerTensorData.Add(new Analysis.Stress.Microsopic.ElasticityTensors());
                        this.InvestigatedSample.DeWittTensorData.Add(new Analysis.Stress.Microsopic.ElasticityTensors());
                        this.InvestigatedSample.GeometricHillTensorData.Add(new Analysis.Stress.Microsopic.ElasticityTensors());
                    }
                }
                else
                {
                    MessageBoxResult Result = MessageBox.Show("You already have loaded some crystallographic data! Do you want to add more data?", "Need more crystallographic data???", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                    if (Result == MessageBoxResult.Yes)
                    {
                        DataManagment.CODSql.CODSearchWindow CrystalSelection = new DataManagment.CODSql.CODSearchWindow();

                        CrystalSelection.ShowDialog();

                        if (CrystalSelection.SelectionFinished)
                        {
                            Tools.Calculation.AddHKLList(CrystalSelection.SelectedData);
                            CrystalSelection.SelectedData.HKLList.Sort((A, B) => (1) * (A.H + A.K + A.L).CompareTo(B.H + B.K + B.L));

                            this.InvestigatedSample.CrystalData.Add(CrystalSelection.SelectedData);

                            this.InvestigatedSample.DiffractionConstants.Add(new List<Analysis.Stress.Microsopic.REK>());
                            this.InvestigatedSample.DiffractionConstantsTexture.Add(new List<Analysis.Stress.Microsopic.REK>());

                            this.InvestigatedSample.VoigtTensorData.Add(new Analysis.Stress.Microsopic.ElasticityTensors());
                            this.InvestigatedSample.ReussTensorData.Add(new Analysis.Stress.Microsopic.ElasticityTensors());
                            this.InvestigatedSample.HillTensorData.Add(new Analysis.Stress.Microsopic.ElasticityTensors());
                            this.InvestigatedSample.KroenerTensorData.Add(new Analysis.Stress.Microsopic.ElasticityTensors());
                            this.InvestigatedSample.DeWittTensorData.Add(new Analysis.Stress.Microsopic.ElasticityTensors());
                            this.InvestigatedSample.GeometricHillTensorData.Add(new Analysis.Stress.Microsopic.ElasticityTensors());
                        }
                    }
                    else if(Result == MessageBoxResult.No)
                    {
                        MessageBoxResult NextResult = MessageBox.Show("Do you wish to delete your selected Data?", "Wrong crystallographic data???", MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if(MessageBoxResult.Yes == NextResult)
                        {
                            this.InvestigatedSample.CrystalData.Clear();

                            this.InvestigatedSample.DiffractionConstants.Clear();

                            this.InvestigatedSample.VoigtTensorData.Clear();
                            this.InvestigatedSample.ReussTensorData.Clear();
                            this.InvestigatedSample.HillTensorData.Clear();
                            this.InvestigatedSample.KroenerTensorData.Clear();
                            this.InvestigatedSample.DeWittTensorData.Clear();
                            this.InvestigatedSample.GeometricHillTensorData.Clear();
                        }
                    }
                }

                PlotHKLToPlot();
            }
            else
            {
                MessageBox.Show("You have already loaded some diffraction patterns!\n You will get into trouble if you change your crystallographic data!\n Therefore I wont allow such crazy things.", "Patterns already loaded!", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void ModifyCrystallographicData_Click(object sender, RoutedEventArgs e)
        {
            if (InvestigatedSample.DiffractionPatterns.Count == 0)
            {
                List<DataManagment.CrystalData.CODData> TmpBackUp = new List<DataManagment.CrystalData.CODData>();

                foreach(DataManagment.CrystalData.CODData CD in this.InvestigatedSample.CrystalData)
                {
                    TmpBackUp.Add(new DataManagment.CrystalData.CODData(CD));
                }

                DataManagment.CrystalData.ShowCrystalDataWindow EditCrystalData = new DataManagment.CrystalData.ShowCrystalDataWindow(this.InvestigatedSample.CrystalData);

                EditCrystalData.ShowDialog();

                if(!EditCrystalData.ForSave)
                {
                    this.InvestigatedSample.CrystalData = TmpBackUp;
                }

                PlotHKLToPlot();
            }
            else
            {
                MessageBox.Show("You have already loaded some diffraction patterns!\n You will get into trouble if you change your crystallographic data!\n Therefore I wont allow such crazy things.", "Patterns already loaded!", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void ManualInputCrystallographicData_Click(object sender, RoutedEventArgs e)
        {
            if (InvestigatedSample.DiffractionPatterns.Count == 0)
            {
                if (this.InvestigatedSample.CrystalData.Count == 0)
                {
                    DataManagment.CrystalData.AddCrystalDataWindow ManualAddCrystalData = new DataManagment.CrystalData.AddCrystalDataWindow();
                    ManualAddCrystalData.ShowDialog();

                    if(ManualAddCrystalData.ForSave)
                    {
                        this.InvestigatedSample.CrystalData.Add(ManualAddCrystalData.CrystalDataList);

                        this.InvestigatedSample.DiffractionConstants.Add(new List<Analysis.Stress.Microsopic.REK>());
                        this.InvestigatedSample.DiffractionConstantsTexture.Add(new List<Analysis.Stress.Microsopic.REK>());

                        this.InvestigatedSample.VoigtTensorData.Add(new Analysis.Stress.Microsopic.ElasticityTensors());
                        this.InvestigatedSample.ReussTensorData.Add(new Analysis.Stress.Microsopic.ElasticityTensors());
                        this.InvestigatedSample.HillTensorData.Add(new Analysis.Stress.Microsopic.ElasticityTensors());
                        this.InvestigatedSample.KroenerTensorData.Add(new Analysis.Stress.Microsopic.ElasticityTensors());
                        this.InvestigatedSample.DeWittTensorData.Add(new Analysis.Stress.Microsopic.ElasticityTensors());
                        this.InvestigatedSample.GeometricHillTensorData.Add(new Analysis.Stress.Microsopic.ElasticityTensors());

                        ComboBoxItem presentPhase = new ComboBoxItem();
                        presentPhase.Content = ManualAddCrystalData.CrystalDataList.Name;
                        this.PhaseSelection.Items.Add(presentPhase);

                        this.PhaseSelection.SelectedIndex = 0;
                    }
                }
                else
                {
                    MessageBoxResult Result = MessageBox.Show("You already have loaded some crystallographic data! Do you want to add more data?", "Need more crystallographic data???", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                    if(Result == MessageBoxResult.Yes)
                    {
                        DataManagment.CrystalData.AddCrystalDataWindow ManualAddCrystalData = new DataManagment.CrystalData.AddCrystalDataWindow();
                        ManualAddCrystalData.ShowDialog();

                        if (ManualAddCrystalData.ForSave)
                        {
                            this.InvestigatedSample.CrystalData.Add(ManualAddCrystalData.CrystalDataList);

                            this.InvestigatedSample.DiffractionConstants.Add(new List<Analysis.Stress.Microsopic.REK>());
                            this.InvestigatedSample.DiffractionConstantsTexture.Add(new List<Analysis.Stress.Microsopic.REK>());

                            this.InvestigatedSample.VoigtTensorData.Add(new Analysis.Stress.Microsopic.ElasticityTensors());
                            this.InvestigatedSample.ReussTensorData.Add(new Analysis.Stress.Microsopic.ElasticityTensors());
                            this.InvestigatedSample.HillTensorData.Add(new Analysis.Stress.Microsopic.ElasticityTensors());
                            this.InvestigatedSample.KroenerTensorData.Add(new Analysis.Stress.Microsopic.ElasticityTensors());
                            this.InvestigatedSample.DeWittTensorData.Add(new Analysis.Stress.Microsopic.ElasticityTensors());
                            this.InvestigatedSample.GeometricHillTensorData.Add(new Analysis.Stress.Microsopic.ElasticityTensors());

                            ComboBoxItem presentPhase = new ComboBoxItem();
                            presentPhase.Content = ManualAddCrystalData.CrystalDataList.Name;
                            this.PhaseSelection.Items.Add(presentPhase);

                            this.PhaseSelection.SelectedIndex = 0;
                        }
                    }
                    else
                    {
                        MessageBoxResult NextResult = MessageBox.Show("Do you wish to delete your selected Data?", "Wrong crystallographic data???", MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if (MessageBoxResult.Yes == NextResult)
                        {
                            this.InvestigatedSample.CrystalData.Clear();

                            this.InvestigatedSample.DiffractionConstants.Clear();

                            this.InvestigatedSample.VoigtTensorData.Clear();
                            this.InvestigatedSample.ReussTensorData.Clear();
                            this.InvestigatedSample.HillTensorData.Clear();
                            this.InvestigatedSample.KroenerTensorData.Clear();
                            this.InvestigatedSample.DeWittTensorData.Clear();
                            this.InvestigatedSample.GeometricHillTensorData.Clear();

                            this.PhaseSelection.Items.Clear();
                        }
                    }
                }

                PlotHKLToPlot();
            }
            else
            {
                MessageBox.Show("You have already loaded some diffraction patterns!\n You will get into trouble if you change your crystallographic data!\n Therefore I wont allow such crazy things.", "Patterns already loaded!", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void PlotHKLToPlot()
        {
            this.HKLAnnotationList.Clear();

            if(CalScec.Properties.Settings.Default.PlotHKLOverview)
            {
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
                
                for(int n = 0; n < this.InvestigatedSample.CrystalData.Count; n++)
                {
                    for (int i = 0; i < this.InvestigatedSample.CrystalData[n].HKLList.Count; i++)
                    {
                        OxyPlot.Annotations.LineAnnotation NewLA = new OxyPlot.Annotations.LineAnnotation();

                        NewLA.Type = OxyPlot.Annotations.LineAnnotationType.Vertical;
                        NewLA.ClipByYAxis = true;
                        NewLA.ClipText = true;
                        NewLA.Color = OxyPlot.OxyColor.FromRgb(Convert.ToByte(AnnotationColors[n % 8][0]), Convert.ToByte(AnnotationColors[n % 8][1]), Convert.ToByte(AnnotationColors[n % 8][2]));
                        NewLA.Text = this.InvestigatedSample.CrystalData[n].SymmetryGroup + " (" + this.InvestigatedSample.CrystalData[n].HKLList[i].H + ", " + this.InvestigatedSample.CrystalData[n].HKLList[i].K + ", " + this.InvestigatedSample.CrystalData[n].HKLList[i].L + " )";
                        NewLA.X = this.InvestigatedSample.CrystalData[n].HKLList[i].EstimatedAngle;
                        NewLA.StrokeThickness = CalScec.Properties.Settings.Default.PeakMarkingThickness;

                        if (CalScec.Properties.Settings.Default.PeakMarkingStyle == 0)
                        {
                            NewLA.LineStyle = OxyPlot.LineStyle.Dot;
                        }
                        else if (CalScec.Properties.Settings.Default.PeakMarkingStyle == 1)
                        {
                            NewLA.LineStyle = OxyPlot.LineStyle.Dash;
                        }

                        this.HKLAnnotationList.Add(NewLA);
                    }
                }
            }

            RefreshAnnotationToPlot();
        }

        private void PlotHKLOverview_Click(object sender, RoutedEventArgs e)
        {
            if (CalScec.Properties.Settings.Default.PlotHKLOverview)
            {
                PlotHKLOverviewMenuItem.IsChecked = false;
                CalScec.Properties.Settings.Default.PlotHKLOverview = false;
            }
            else
            {
                PlotHKLOverviewMenuItem.IsChecked = true;
                CalScec.Properties.Settings.Default.PlotHKLOverview = true;
            }

            PlotHKLToPlot();
        }

        private void EditPhaseCompositionData_Click(object sender, RoutedEventArgs e)
        {
            DataManagment.CrystalData.CompositionWindow cW = new DataManagment.CrystalData.CompositionWindow(this.InvestigatedSample);

            cW.ShowDialog();
        }

        #endregion

        #region Texture

        private void LoadTexture_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog OpenDiffractionFile = new Microsoft.Win32.OpenFileDialog();
            OpenDiffractionFile.Multiselect = false;
            OpenDiffractionFile.DefaultExt = ".cif";
            Nullable<bool> Opened = OpenDiffractionFile.ShowDialog();

            if (Opened == true)
            {
                string FilePath = OpenDiffractionFile.FileName;

                if (this.InvestigatedSample.CrystalData.Count == 1)
                {
                    Analysis.Texture.OrientationDistributionFunction NewODF = new Analysis.Texture.OrientationDistributionFunction(FilePath);
                    this.InvestigatedSample.ODFList.Clear();
                    this.InvestigatedSample.ODFList.Add(NewODF);
                    this.InvestigatedSample.VoigtTensorData[0].ODF = NewODF;
                    this.InvestigatedSample.ReussTensorData[0].ODF = NewODF;
                    this.InvestigatedSample.HillTensorData[0].ODF = NewODF;
                    this.InvestigatedSample.KroenerTensorData[0].ODF = NewODF;
                    this.InvestigatedSample.DeWittTensorData[0].ODF = NewODF;
                    this.InvestigatedSample.GeometricHillTensorData[0].ODF = NewODF;
                }
                else if (this.InvestigatedSample.CrystalData.Count > 1)
                {
                    Analysis.Texture.PhaseSelectionWindow PSW = new Analysis.Texture.PhaseSelectionWindow(this.InvestigatedSample.CrystalData);
                    PSW.ShowDialog();

                    if(!PSW.Canceled)
                    {
                        int SelectedPhase = PSW.PhaseSelectionBox.SelectedIndex;

                        Analysis.Texture.OrientationDistributionFunction NewODF = new Analysis.Texture.OrientationDistributionFunction(FilePath);

                        this.InvestigatedSample.ODFList.Add(NewODF);
                        this.InvestigatedSample.VoigtTensorData[SelectedPhase].ODF = NewODF;
                        this.InvestigatedSample.ReussTensorData[SelectedPhase].ODF = NewODF;
                        this.InvestigatedSample.HillTensorData[SelectedPhase].ODF = NewODF;
                        this.InvestigatedSample.KroenerTensorData[SelectedPhase].ODF = NewODF;
                        this.InvestigatedSample.DeWittTensorData[SelectedPhase].ODF = NewODF;
                        this.InvestigatedSample.GeometricHillTensorData[SelectedPhase].ODF = NewODF;

                        //this.InvestigatedSample.VoigtTensorData[SelectedPhase].ODF.BaseTensor = this.InvestigatedSample.VoigtTensorData[SelectedPhase].Clone() as Analysis.Stress.Microsopic.ElasticityTensors;
                        //this.InvestigatedSample.ReussTensorData[SelectedPhase].ODF.BaseTensor = this.InvestigatedSample.ReussTensorData[SelectedPhase].Clone() as Analysis.Stress.Microsopic.ElasticityTensors;
                        //this.InvestigatedSample.HillTensorData[SelectedPhase].ODF.BaseTensor = this.InvestigatedSample.HillTensorData[SelectedPhase].Clone() as Analysis.Stress.Microsopic.ElasticityTensors;
                        //this.InvestigatedSample.KroenerTensorData[SelectedPhase].ODF.BaseTensor = this.InvestigatedSample.KroenerTensorData[SelectedPhase].Clone() as Analysis.Stress.Microsopic.ElasticityTensors;
                        //this.InvestigatedSample.DeWittTensorData[SelectedPhase].ODF.BaseTensor = this.InvestigatedSample.DeWittTensorData[SelectedPhase].Clone() as Analysis.Stress.Microsopic.ElasticityTensors;
                        //this.InvestigatedSample.GeometricHillTensorData[SelectedPhase].ODF.BaseTensor = this.InvestigatedSample.GeometricHillTensorData[SelectedPhase].Clone() as Analysis.Stress.Microsopic.ElasticityTensors;
                        //this.InvestigatedSample.VoigtTensorData[SelectedPhase].ODF.TextureTensor = this.InvestigatedSample.VoigtTensorData[SelectedPhase].Clone() as Analysis.Stress.Microsopic.ElasticityTensors;
                        //this.InvestigatedSample.ReussTensorData[SelectedPhase].ODF.TextureTensor = this.InvestigatedSample.ReussTensorData[SelectedPhase].Clone() as Analysis.Stress.Microsopic.ElasticityTensors;
                        //this.InvestigatedSample.HillTensorData[SelectedPhase].ODF.TextureTensor = this.InvestigatedSample.HillTensorData[SelectedPhase].Clone() as Analysis.Stress.Microsopic.ElasticityTensors;
                        //this.InvestigatedSample.KroenerTensorData[SelectedPhase].ODF.TextureTensor = this.InvestigatedSample.KroenerTensorData[SelectedPhase].Clone() as Analysis.Stress.Microsopic.ElasticityTensors;
                        //this.InvestigatedSample.DeWittTensorData[SelectedPhase].ODF.TextureTensor = this.InvestigatedSample.DeWittTensorData[SelectedPhase].Clone() as Analysis.Stress.Microsopic.ElasticityTensors;
                        //this.InvestigatedSample.GeometricHillTensorData[SelectedPhase].ODF.TextureTensor = this.InvestigatedSample.GeometricHillTensorData[SelectedPhase].Clone() as Analysis.Stress.Microsopic.ElasticityTensors;

                        //this.InvestigatedSample.VoigtTensorData[SelectedPhase].ODF.BaseTensor.ODF = null;
                        //this.InvestigatedSample.ReussTensorData[SelectedPhase].ODF.BaseTensor.ODF = null;
                        //this.InvestigatedSample.HillTensorData[SelectedPhase].ODF.BaseTensor.ODF = null;
                        //this.InvestigatedSample.KroenerTensorData[SelectedPhase].ODF.BaseTensor.ODF = null;
                        //this.InvestigatedSample.DeWittTensorData[SelectedPhase].ODF.BaseTensor.ODF = null;
                        //this.InvestigatedSample.GeometricHillTensorData[SelectedPhase].ODF.BaseTensor.ODF = null;
                        //this.InvestigatedSample.VoigtTensorData[SelectedPhase].ODF.TextureTensor.ODF = null;
                        //this.InvestigatedSample.ReussTensorData[SelectedPhase].ODF.TextureTensor.ODF = null;
                        //this.InvestigatedSample.HillTensorData[SelectedPhase].ODF.TextureTensor.ODF = null;
                        //this.InvestigatedSample.KroenerTensorData[SelectedPhase].ODF.TextureTensor.ODF = null;
                        //this.InvestigatedSample.DeWittTensorData[SelectedPhase].ODF.TextureTensor.ODF = null;
                        //this.InvestigatedSample.GeometricHillTensorData[SelectedPhase].ODF.TextureTensor.ODF = null;
                    }
                }
                else
                {
                    MessageBox.Show("Please load the crystallographic data first!", "No crystal data loaded!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ShowPoleFigures_Click(object sender, RoutedEventArgs e)
        {
            Analysis.Texture.PoleFigureWindow PoleWindow = new Analysis.Texture.PoleFigureWindow(this.InvestigatedSample);

            PoleWindow.Show();
        }
        private void ShowRawTextureData_Click(object sender, RoutedEventArgs e)
        {
            if (this.InvestigatedSample.ODFList != null)
            {
                Analysis.Texture.PhaseSelectionWindow PSW = new Analysis.Texture.PhaseSelectionWindow(this.InvestigatedSample.CrystalData);
                PSW.ShowDialog();

                if (!PSW.Canceled)
                {
                    int SelectedPhase = PSW.PhaseSelectionBox.SelectedIndex;

                    try
                    {
                        Analysis.Texture.TextureRawDataView tRDV = new Analysis.Texture.TextureRawDataView(this.InvestigatedSample.ODFList[SelectedPhase].TDData);
                        tRDV.Show();
                    }
                    catch
                    {
                        MessageBox.Show("No texture data found. Please load data first!.", "No ODF added!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                this.InvestigatedSample.ODFList = new List<Analysis.Texture.OrientationDistributionFunction>();
                MessageBox.Show("No texture data found. Please load data first!.", "No ODF added!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Settings

        #region Plot

        private void DiffractionPlotAxesToLinear_Click(object sender, RoutedEventArgs e)
        {
            this.DiffractionXAxisLin.Minimum = this.DiffractionXAxisLin.ActualMinimum;
            this.DiffractionXAxisLin.Maximum = this.DiffractionXAxisLin.ActualMaximum;
            XAxisMinToolText.Text = Convert.ToString(Convert.ToInt32(this.DiffractionXAxisLin.Minimum));
            XAxisMaxToolText.Text = Convert.ToString(Convert.ToInt32(this.DiffractionXAxisLin.Maximum));

            if (CalScec.Properties.Settings.Default.PlotYAxes != 0)
            {
                CalScec.Properties.Settings.Default.PlotYAxes = 0;
                this.DiffractionPlotModel.Axes.Remove(this.DiffractionYAxisLog);
                this.DiffractionPlotModel.Axes.Add(this.DiffractionYAxisLin);
                this.DiffractionYAxisLin.Minimum = this.DiffractionYAxisLog.ActualMinimum;
                this.DiffractionYAxisLin.Maximum = this.DiffractionYAxisLog.ActualMaximum;
                this.MainDiffPlot.Model.ResetAllAxes();
                this.MainDiffPlot.Model.InvalidatePlot(true);

                this.DiffractionPlotAxesToLog.IsChecked = false;
                this.DiffractionPlotAxesToLinear.IsChecked = true;

                YAxisMinToolText.Text = Convert.ToString(Convert.ToInt32(this.DiffractionYAxisLin.Minimum));
                YAxisMaxToolText.Text = Convert.ToString(Convert.ToInt32(this.DiffractionYAxisLin.Maximum));

                this.ChangeAxisTool.ToolTip = "Set axis to logarythmic";
                this.ChangeAxisTool.Click += DiffractionPlotAxesToLog_Click;
                this.ChangeAxisTool.Click -= DiffractionPlotAxesToLinear_Click;
            }
        }

        private void DiffractionPlotAxesToLog_Click(object sender, RoutedEventArgs e)
        {
            this.DiffractionXAxisLin.Minimum = this.DiffractionXAxisLin.ActualMinimum;
            this.DiffractionXAxisLin.Maximum = this.DiffractionXAxisLin.ActualMaximum;

            if (CalScec.Properties.Settings.Default.PlotYAxes != 1)
            {
                CalScec.Properties.Settings.Default.PlotYAxes = 1;
                this.DiffractionPlotModel.Axes.Remove(this.DiffractionYAxisLin);
                this.DiffractionPlotModel.Axes.Add(this.DiffractionYAxisLog);
                this.DiffractionYAxisLog.Minimum = this.DiffractionYAxisLin.Minimum;
                this.DiffractionYAxisLog.Maximum = this.DiffractionYAxisLin.Maximum;
                this.MainDiffPlot.Model.ResetAllAxes();
                this.MainDiffPlot.Model.InvalidatePlot(true);

                this.DiffractionPlotAxesToLog.IsChecked = true;
                this.DiffractionPlotAxesToLinear.IsChecked = false;

                YAxisMinToolText.Text = Convert.ToString(Convert.ToInt32(this.DiffractionYAxisLog.Minimum));
                YAxisMaxToolText.Text = Convert.ToString(Convert.ToInt32(this.DiffractionYAxisLog.Maximum));

                this.ChangeAxisTool.ToolTip = "Set axis to linear";
                this.ChangeAxisTool.Click += DiffractionPlotAxesToLinear_Click;
                this.ChangeAxisTool.Click -= DiffractionPlotAxesToLog_Click;
            }
        }

        private void SetAxesTool_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DiffractionXAxisLin.Minimum = Convert.ToDouble(XAxisMinToolText.Text);
                DiffractionXAxisLin.Maximum = Convert.ToDouble(XAxisMaxToolText.Text);
                DiffractionYAxisLin.Minimum = Convert.ToDouble(YAxisMinToolText.Text);
                DiffractionYAxisLin.Maximum = Convert.ToDouble(YAxisMaxToolText.Text);
                DiffractionYAxisLog.Minimum = Convert.ToDouble(YAxisMinToolText.Text);
                DiffractionYAxisLog.Maximum = Convert.ToDouble(YAxisMaxToolText.Text);
            }
            catch
            {
                MessageBox.Show("You should use at least a double for modification! \n I dont like ',' only '.'", "Wrong Format", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.MainDiffPlot.Model.ResetAllAxes();
                this.MainDiffPlot.Model.InvalidatePlot(true);
            }
        }

        private void ChangeMajorGridLineStyle_Click(object sender, RoutedEventArgs e)
        {
            MenuItem This = (MenuItem)sender;
            string Header = This.Header.ToString();

            switch (Header)
            {
                case "None":
                    CalScec.Properties.Settings.Default.MainPlotMajorGridStyle = 0;
                    DiffractionXAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.None;
                    DiffractionYAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.None;
                    DiffractionYAxisLog.MajorGridlineStyle = OxyPlot.LineStyle.None;
                    this.MajorGridStyleNone.IsChecked = true;
                    this.MajorGridStyleDot.IsChecked = false;
                    this.MajorGridStyleDash.IsChecked = false;
                    break;
                case "Dash":
                    CalScec.Properties.Settings.Default.MainPlotMajorGridStyle = 1;
                    DiffractionXAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dash;
                    DiffractionYAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dash;
                    DiffractionYAxisLog.MajorGridlineStyle = OxyPlot.LineStyle.Dash;
                    this.MajorGridStyleNone.IsChecked = false;
                    this.MajorGridStyleDot.IsChecked = false;
                    this.MajorGridStyleDash.IsChecked = true;
                    break;
                case "Dot":
                    CalScec.Properties.Settings.Default.MainPlotMajorGridStyle = 2;
                    DiffractionXAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dot;
                    DiffractionYAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dot;
                    DiffractionYAxisLog.MajorGridlineStyle = OxyPlot.LineStyle.Dot;
                    this.MajorGridStyleNone.IsChecked = false;
                    this.MajorGridStyleDot.IsChecked = true;
                    this.MajorGridStyleDash.IsChecked = false;
                    break;
                default:
                    CalScec.Properties.Settings.Default.MainPlotMajorGridStyle = 2;
                    DiffractionXAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dot;
                    DiffractionYAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dot;
                    DiffractionYAxisLog.MajorGridlineStyle = OxyPlot.LineStyle.Dot;
                    this.MajorGridStyleNone.IsChecked = false;
                    this.MajorGridStyleDot.IsChecked = true;
                    this.MajorGridStyleDash.IsChecked = false;
                    break;
            }

            this.MainDiffPlot.Model.ResetAllAxes();
            this.MainDiffPlot.Model.InvalidatePlot(true);
        }

        private void ChangeMinorGridLineStyle_Click(object sender, RoutedEventArgs e)
        {
            MenuItem This = (MenuItem)sender;
            string Header = This.Header.ToString();

            switch (Header)
            {
                case "None":
                    CalScec.Properties.Settings.Default.MainPlotMinorGridStyle = 0;
                    DiffractionXAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.None;
                    DiffractionYAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.None;
                    DiffractionYAxisLog.MinorGridlineStyle = OxyPlot.LineStyle.None;
                    this.MinorGridStyleNone.IsChecked = true;
                    this.MinorGridStyleDot.IsChecked = false;
                    this.MinorGridStyleDash.IsChecked = false;
                    break;
                case "Dash":
                    CalScec.Properties.Settings.Default.MainPlotMinorGridStyle = 1;
                    DiffractionXAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dash;
                    DiffractionYAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dash;
                    DiffractionYAxisLog.MinorGridlineStyle = OxyPlot.LineStyle.Dash;
                    this.MinorGridStyleNone.IsChecked = false;
                    this.MinorGridStyleDot.IsChecked = false;
                    this.MinorGridStyleDash.IsChecked = true;
                    break;
                case "Dot":
                    CalScec.Properties.Settings.Default.MainPlotMinorGridStyle = 2;
                    DiffractionXAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dot;
                    DiffractionYAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dot;
                    DiffractionYAxisLog.MinorGridlineStyle = OxyPlot.LineStyle.Dot;
                    this.MinorGridStyleNone.IsChecked = false;
                    this.MinorGridStyleDot.IsChecked = true;
                    this.MinorGridStyleDash.IsChecked = false;
                    break;
                default:
                    CalScec.Properties.Settings.Default.MainPlotMinorGridStyle = 2;
                    DiffractionXAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dot;
                    DiffractionYAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dot;
                    DiffractionYAxisLog.MinorGridlineStyle = OxyPlot.LineStyle.Dot;
                    this.MinorGridStyleNone.IsChecked = false;
                    this.MinorGridStyleDot.IsChecked = true;
                    this.MinorGridStyleDash.IsChecked = false;
                    break;
            }

            this.MainDiffPlot.Model.ResetAllAxes();
            this.MainDiffPlot.Model.InvalidatePlot(true);
        }

        private void ChangeLineStyle_Click(object sender, RoutedEventArgs e)
        {
            MenuItem This = (MenuItem)sender;
            string Header = This.Header.ToString();
            switch(Header)
            {
                case "None":
                    CalScec.Properties.Settings.Default.MainPlotLineStyle = 0;
                    this.ChangeLineStyleNone.IsChecked = true;
                    this.ChangeLineStyleDot.IsChecked = false;
                    this.ChangeLineStyleDash.IsChecked = false;
                    break;
                case "Dot":
                    CalScec.Properties.Settings.Default.MainPlotLineStyle = 1;
                    this.ChangeLineStyleNone.IsChecked = false;
                    this.ChangeLineStyleDot.IsChecked = true;
                    this.ChangeLineStyleDash.IsChecked = false;
                    break;
                case "Dash":
                    CalScec.Properties.Settings.Default.MainPlotLineStyle = 2;
                    this.ChangeLineStyleNone.IsChecked = false;
                    this.ChangeLineStyleDot.IsChecked = false;
                    this.ChangeLineStyleDash.IsChecked = true;
                    break;
                default:
                    CalScec.Properties.Settings.Default.MainPlotLineStyle = 0;
                    this.ChangeLineStyleNone.IsChecked = true;
                    this.ChangeLineStyleDot.IsChecked = false;
                    this.ChangeLineStyleDash.IsChecked = false;
                    break;
            }

            BackgroundWorker worker = new BackgroundWorker();

            worker.DoWork += ChangeLineMarkerStyle_Work;
            worker.RunWorkerCompleted += ChangeLineMarkerStyle_Completed;

            worker.RunWorkerAsync();
        }

        private void ChangeLineThickness_Click(object sender, RoutedEventArgs e)
        {
            MenuItem This = (MenuItem)sender;
            string Header = This.Header.ToString();
            switch (Header)
            {
                case "1":
                    CalScec.Properties.Settings.Default.MainPlotLineThickness = 1;
                    this.ChangeLineThickness1.IsChecked = true;
                    this.ChangeLineThickness2.IsChecked = false;
                    this.ChangeLineThickness3.IsChecked = false;
                    break;
                case "2":
                    CalScec.Properties.Settings.Default.MainPlotLineThickness = 2;
                    this.ChangeLineThickness1.IsChecked = false;
                    this.ChangeLineThickness2.IsChecked = true;
                    this.ChangeLineThickness3.IsChecked = false;
                    break;
                case "3":
                    CalScec.Properties.Settings.Default.MainPlotLineThickness = 3;
                    this.ChangeLineThickness1.IsChecked = false;
                    this.ChangeLineThickness2.IsChecked = false;
                    this.ChangeLineThickness3.IsChecked = true;
                    break;
                default:
                    CalScec.Properties.Settings.Default.MainPlotLineThickness = 1;
                    this.ChangeLineThickness1.IsChecked = true;
                    this.ChangeLineThickness2.IsChecked = false;
                    this.ChangeLineThickness3.IsChecked = false;
                    break;
            }

            BackgroundWorker worker = new BackgroundWorker();

            worker.DoWork += ChangeLineMarkerStyle_Work;
            worker.RunWorkerCompleted += ChangeLineMarkerStyle_Completed;

            worker.RunWorkerAsync();
        }

        private void ChangeMarkerSize_Click(object sender, RoutedEventArgs e)
        {
            MenuItem This = (MenuItem)sender;
            string Header = This.Header.ToString();
            switch (Header)
            {
                case "0":
                    CalScec.Properties.Settings.Default.MainPlotDotThickness = 0;
                    this.ChangeMarkerThickness0.IsChecked = true;
                    this.ChangeMarkerThickness1.IsChecked = false;
                    this.ChangeMarkerThickness15.IsChecked = false;
                    this.ChangeMarkerThickness2.IsChecked = false;
                    this.ChangeMarkerThickness25.IsChecked = false;
                    this.ChangeMarkerThickness3.IsChecked = false;
                    this.ChangeMarkerThickness35.IsChecked = false;
                    break;
                case "1":
                    CalScec.Properties.Settings.Default.MainPlotDotThickness = 1;
                    this.ChangeMarkerThickness0.IsChecked = false;
                    this.ChangeMarkerThickness1.IsChecked = true;
                    this.ChangeMarkerThickness15.IsChecked = false;
                    this.ChangeMarkerThickness2.IsChecked = false;
                    this.ChangeMarkerThickness25.IsChecked = false;
                    this.ChangeMarkerThickness3.IsChecked = false;
                    this.ChangeMarkerThickness35.IsChecked = false;
                    break;
                case "15":
                    CalScec.Properties.Settings.Default.MainPlotDotThickness = 1.5;
                    this.ChangeMarkerThickness0.IsChecked = false;
                    this.ChangeMarkerThickness1.IsChecked = false;
                    this.ChangeMarkerThickness15.IsChecked = true;
                    this.ChangeMarkerThickness2.IsChecked = false;
                    this.ChangeMarkerThickness25.IsChecked = false;
                    this.ChangeMarkerThickness3.IsChecked = false;
                    this.ChangeMarkerThickness35.IsChecked = false;
                    break;
                case "2":
                    CalScec.Properties.Settings.Default.MainPlotDotThickness = 2;
                    this.ChangeMarkerThickness0.IsChecked = false;
                    this.ChangeMarkerThickness1.IsChecked = false;
                    this.ChangeMarkerThickness15.IsChecked = false;
                    this.ChangeMarkerThickness2.IsChecked = true;
                    this.ChangeMarkerThickness25.IsChecked = false;
                    this.ChangeMarkerThickness3.IsChecked = false;
                    this.ChangeMarkerThickness35.IsChecked = false;
                    break;
                case "25":
                    CalScec.Properties.Settings.Default.MainPlotDotThickness = 2.5;
                    this.ChangeMarkerThickness0.IsChecked = false;
                    this.ChangeMarkerThickness1.IsChecked = false;
                    this.ChangeMarkerThickness15.IsChecked = false;
                    this.ChangeMarkerThickness2.IsChecked = false;
                    this.ChangeMarkerThickness25.IsChecked = true;
                    this.ChangeMarkerThickness3.IsChecked = false;
                    this.ChangeMarkerThickness35.IsChecked = false;
                    break;
                case "3":
                    CalScec.Properties.Settings.Default.MainPlotDotThickness = 3;
                    this.ChangeMarkerThickness0.IsChecked = false;
                    this.ChangeMarkerThickness1.IsChecked = false;
                    this.ChangeMarkerThickness15.IsChecked = false;
                    this.ChangeMarkerThickness2.IsChecked = false;
                    this.ChangeMarkerThickness25.IsChecked = false;
                    this.ChangeMarkerThickness3.IsChecked = true;
                    this.ChangeMarkerThickness35.IsChecked = false;
                    break;
                case "35":
                    CalScec.Properties.Settings.Default.MainPlotDotThickness = 3.5;
                    this.ChangeMarkerThickness0.IsChecked = false;
                    this.ChangeMarkerThickness1.IsChecked = false;
                    this.ChangeMarkerThickness15.IsChecked = false;
                    this.ChangeMarkerThickness2.IsChecked = false;
                    this.ChangeMarkerThickness25.IsChecked = false;
                    this.ChangeMarkerThickness3.IsChecked = false;
                    this.ChangeMarkerThickness35.IsChecked = true;
                    break;
                default:
                    CalScec.Properties.Settings.Default.MainPlotDotThickness = 3;
                    this.ChangeMarkerThickness0.IsChecked = false;
                    this.ChangeMarkerThickness1.IsChecked = false;
                    this.ChangeMarkerThickness15.IsChecked = false;
                    this.ChangeMarkerThickness2.IsChecked = false;
                    this.ChangeMarkerThickness25.IsChecked = false;
                    this.ChangeMarkerThickness3.IsChecked = true;
                    this.ChangeMarkerThickness35.IsChecked = false;
                    break;
            }

            BackgroundWorker worker = new BackgroundWorker();

            worker.DoWork += ChangeLineMarkerStyle_Work;
            worker.RunWorkerCompleted += ChangeLineMarkerStyle_Completed;

            worker.RunWorkerAsync();
        }

        private void ChangeLineMarkerStyle_Work(object sender, DoWorkEventArgs e)
        {
            if (!CalScec.Properties.Settings.Default.MainPlotWithErrors)
            {
                foreach (OxyPlot.Series.LineSeries S in this.DiffractionPlotModel.Series)
                {
                    if (CalScec.Properties.Settings.Default.MainPlotLineStyle == 0)
                    {
                        S.LineStyle = OxyPlot.LineStyle.None;
                    }
                    else if (CalScec.Properties.Settings.Default.MainPlotLineStyle == 1)
                    {
                        S.LineStyle = OxyPlot.LineStyle.Dot;
                    }
                    else
                    {
                        S.LineStyle = OxyPlot.LineStyle.Dash;
                    }
                    S.StrokeThickness = CalScec.Properties.Settings.Default.MainPlotLineThickness;
                    S.MarkerSize = CalScec.Properties.Settings.Default.MainPlotDotThickness;
                }
            }
            else
            {

            }
        }

        private void ChangeLineMarkerStyle_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            this.MainDiffPlot.Model.InvalidatePlot(true);
        }

        private void FitAxisToPattern_Click(object sender, RoutedEventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker();

            worker.DoWork += FitAxisToPattern_Work;
            worker.RunWorkerCompleted += FitAxisToPattern_Completed;

            worker.RunWorkerAsync();
        }

        private void FitAxisToPattern_Work(object sender, DoWorkEventArgs e)
        {
            if(DiffractionPlotModel.Series.Count != 0)
            {
                DiffractionXAxisLin.Minimum = Double.MaxValue;
                DiffractionXAxisLin.Maximum = Double.MinValue;
                DiffractionYAxisLin.Minimum = Double.MaxValue;
                DiffractionYAxisLin.Maximum = Double.MinValue;
                foreach(OxyPlot.Series.LineSeries LS in DiffractionPlotModel.Series)
                {
                    if(LS.Points[0].X < DiffractionXAxisLin.Minimum)
                    {
                        DiffractionXAxisLin.Minimum = LS.Points[0].X;
                    }
                    if (LS.Points[LS.Points.Count - 1].X > DiffractionXAxisLin.Maximum)
                    {
                        DiffractionXAxisLin.Maximum = LS.Points[LS.Points.Count - 1].X;
                    }

                    foreach(OxyPlot.DataPoint DP in LS.Points)
                    {
                        if (DP.Y < DiffractionYAxisLin.Minimum)
                        {
                            DiffractionYAxisLin.Minimum = DP.Y;
                        }
                        if (DP.Y > DiffractionYAxisLin.Maximum)
                        {
                            DiffractionYAxisLin.Maximum = DP.Y;
                        }
                    }
                }

                DiffractionYAxisLog.Minimum = DiffractionYAxisLin.Minimum;
                DiffractionYAxisLog.Maximum = DiffractionYAxisLin.Maximum;
            }
        }

        private void FitAxisToPattern_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            this.MainDiffPlot.Model.ResetAllAxes();
            this.MainDiffPlot.Model.InvalidatePlot(true);
        }

        private void ChangePeakLineStyle_Click(object sender, RoutedEventArgs e)
        {
            MenuItem This = (MenuItem)sender;
            string Header = This.Header.ToString();
            switch (Header)
            {
                case "Dot":
                    CalScec.Properties.Settings.Default.PeakMarkingStyle = 0;
                    this.ChangePeakLineStyleDot.IsChecked = true;
                    this.ChangePeakLineStyleDash.IsChecked = false;
                    break;
                case "Dash":
                    CalScec.Properties.Settings.Default.PeakMarkingStyle = 1;
                    this.ChangePeakLineStyleDot.IsChecked = false;
                    this.ChangePeakLineStyleDash.IsChecked = true;
                    break;
                default:
                    CalScec.Properties.Settings.Default.MainPlotLineStyle = 0;
                    this.ChangePeakLineStyleDot.IsChecked = true;
                    this.ChangePeakLineStyleDash.IsChecked = false;
                    break;
            }
            
            foreach(OxyPlot.Annotations.LineAnnotation LA in DiffractionPlotModel.Annotations)
            {
                if(CalScec.Properties.Settings.Default.PeakMarkingStyle == 0)
                {
                    LA.LineStyle = OxyPlot.LineStyle.Dot;
                }
                else if(CalScec.Properties.Settings.Default.PeakMarkingStyle == 1)
                {
                    LA.LineStyle = OxyPlot.LineStyle.Dash;
                }
            }

            this.MainDiffPlot.Model.InvalidatePlot(true);
        }

        private void ChangePeakLineThickness_Click(object sender, RoutedEventArgs e)
        {
            MenuItem This = (MenuItem)sender;
            string Header = This.Header.ToString();
            switch (Header)
            {
                case "1":
                    CalScec.Properties.Settings.Default.PeakMarkingThickness = 1;
                    this.ChangePeakLineThickness1.IsChecked = true;
                    this.ChangePeakLineThickness2.IsChecked = false;
                    this.ChangePeakLineThickness3.IsChecked = false;
                    break;
                case "2":
                    CalScec.Properties.Settings.Default.PeakMarkingThickness = 2;
                    this.ChangePeakLineThickness1.IsChecked = false;
                    this.ChangePeakLineThickness2.IsChecked = true;
                    this.ChangePeakLineThickness3.IsChecked = false;
                    break;
                case "3":
                    CalScec.Properties.Settings.Default.PeakMarkingThickness = 3;
                    this.ChangePeakLineThickness1.IsChecked = false;
                    this.ChangePeakLineThickness2.IsChecked = false;
                    this.ChangePeakLineThickness3.IsChecked = true;
                    break;
                default:
                    CalScec.Properties.Settings.Default.PeakMarkingThickness = 1;
                    this.ChangePeakLineThickness1.IsChecked = true;
                    this.ChangePeakLineThickness2.IsChecked = false;
                    this.ChangePeakLineThickness3.IsChecked = false;
                    break;
            }

            foreach (OxyPlot.Annotations.LineAnnotation LA in DiffractionPlotModel.Annotations)
            {
                LA.StrokeThickness = CalScec.Properties.Settings.Default.PeakMarkingThickness;
            }

            this.MainDiffPlot.Model.InvalidatePlot(true);
        }

        private void RefreshAnnotationToPlot()
        {
            DiffractionPlotModel.Annotations.Clear();

            foreach(OxyPlot.Annotations.LineAnnotation LA in this.HKLAnnotationList)
            {
                DiffractionPlotModel.Annotations.Add(LA);
            }
            foreach (OxyPlot.Annotations.LineAnnotation LA in this.PeakAnnotationList)
            {
                DiffractionPlotModel.Annotations.Add(LA);
            }
            foreach (OxyPlot.Annotations.LineAnnotation LA in this.PeakManipulationLine)
            {
                DiffractionPlotModel.Annotations.Add(LA);
            }

            this.MainDiffPlot.Model.InvalidatePlot(true);
        }

        #endregion

        #region Peak detection

        private void HyperResolution_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool DontChange = false;
            try
            {
                double NewRes = Convert.ToDouble(this.HyperResolution.Text);

                CalScec.Properties.Settings.Default.HyperSensitivity = NewRes;
            }
            catch
            {
                DontChange = true; ;
            }
            finally
            {
                if (!DontChange)
                {
                    this.HyperResolution.Text = Convert.ToString(CalScec.Properties.Settings.Default.HyperSensitivity);
                }
            }
        }

        private void AcceptedPeakBackgroundRation_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool DontChange = false;
            try
            {
                double NewRes = Convert.ToDouble(this.AcceptedPeakBackgroundRation.Text);

                CalScec.Properties.Settings.Default.AcceptedPeakBackgroundRatio = NewRes;
            }
            catch
            {
                DontChange = true;
            }
            finally
            {
                if (!DontChange)
                {
                    this.AcceptedPeakBackgroundRation.Text = Convert.ToString(CalScec.Properties.Settings.Default.AcceptedPeakBackgroundRatio);
                }
            }
        }

        private void HyperResolution_LostFocus(object sender, RoutedEventArgs e)
        {
            HyperResolution.Text = Convert.ToString(CalScec.Properties.Settings.Default.HyperSensitivity);
        }

        private void AcceptedPeakBackgroundRation_LostFocus(object sender, RoutedEventArgs e)
        {
            AcceptedPeakBackgroundRation.Text = Convert.ToString(CalScec.Properties.Settings.Default.AcceptedPeakBackgroundRatio);
        }

        private void AcceptedHKLDifference_LostFocus(object sender, RoutedEventArgs e)
        {
            AcceptedDifferenceHKLAssociation.Text = Convert.ToString(CalScec.Properties.Settings.Default.HKLAssociationRangeDeg);
        }

        private void WavelengthTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                CalScec.Properties.Settings.Default.UsedWaveLength = Convert.ToDouble(WavelengthTextBox.Text);
            }
            catch
            {

            }
        }

        private void AcceptedHKLDifference_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool DontChange = false;
            try
            {
                double NewRes = Convert.ToDouble(this.AcceptedDifferenceHKLAssociation.Text);

                CalScec.Properties.Settings.Default.HKLAssociationRangeDeg = NewRes;
            }
            catch
            {
                DontChange = true; ;
            }
            finally
            {
                if (!DontChange)
                {
                    this.AcceptedDifferenceHKLAssociation.Text = Convert.ToString(CalScec.Properties.Settings.Default.HKLAssociationRangeDeg);
                }
            }
        }

        private void PatternLowerLimitTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                double NewRes = Convert.ToDouble(this.PatternLowerLimitTextBox.Text);

                CalScec.Properties.Settings.Default.PatternLowerLimit = NewRes;
            }
            catch
            {

            }
        }

        private void PatternUpperLimitTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                double NewRes = Convert.ToDouble(this.PatternUpperLimitTextBox.Text);

                CalScec.Properties.Settings.Default.PatternUpperLimit = NewRes;
            }
            catch
            {

            }
        }

        bool RoutineChangeAvtive = true;

        private void PeakDetectionRoutineCIF_Checked(object sender, RoutedEventArgs e)
        {
            if (RoutineChangeAvtive)
            {
                RoutineChangeAvtive = false;
                PeakDetectionRoutineHyper.IsChecked = false;
                PeakDetectionRoutineCIF.IsChecked = true;
                PeakDetectionRoutinePattern.IsChecked = false;
                CalScec.Properties.Settings.Default.UsedPeakDetectionId = 1;
                RoutineChangeAvtive = true;
            }
        }

        private void PeakDetectionRoutineCIF_Unchecked(object sender, RoutedEventArgs e)
        {
            if (RoutineChangeAvtive)
            {
                RoutineChangeAvtive = false;
                PeakDetectionRoutineHyper.IsChecked = true;
                PeakDetectionRoutineCIF.IsChecked = false;
                PeakDetectionRoutinePattern.IsChecked = false;
                CalScec.Properties.Settings.Default.UsedPeakDetectionId = 0;
                RoutineChangeAvtive = true;
            }
        }

        private void PeakDetectionRoutineHyper_Checked(object sender, RoutedEventArgs e)
        {
            if (RoutineChangeAvtive)
            {
                RoutineChangeAvtive = false;
                PeakDetectionRoutineHyper.IsChecked = true;
                PeakDetectionRoutineCIF.IsChecked = false;
                PeakDetectionRoutinePattern.IsChecked = false;
                CalScec.Properties.Settings.Default.UsedPeakDetectionId = 0;
                RoutineChangeAvtive = true;
            }
        }

        private void PeakDetectionRoutineHyper_Unchecked(object sender, RoutedEventArgs e)
        {
            if (RoutineChangeAvtive)
            {
                RoutineChangeAvtive = false;
                PeakDetectionRoutineHyper.IsChecked = false;
                PeakDetectionRoutineCIF.IsChecked = true;
                PeakDetectionRoutinePattern.IsChecked = false;
                CalScec.Properties.Settings.Default.UsedPeakDetectionId = 1;
                RoutineChangeAvtive = true;
            }
        }

        private void PeakDetectionRoutinePattern_Checked(object sender, RoutedEventArgs e)
        {
            if (RoutineChangeAvtive)
            {
                RoutineChangeAvtive = false;
                PeakDetectionRoutineHyper.IsChecked = false;
                PeakDetectionRoutineCIF.IsChecked = false;
                PeakDetectionRoutinePattern.IsChecked = true;
                CalScec.Properties.Settings.Default.UsedPeakDetectionId = 2;
                RoutineChangeAvtive = true;
            }
        }

        private void PeakDetectionRoutinePattern_Unchecked(object sender, RoutedEventArgs e)
        {
            if (RoutineChangeAvtive)
            {
                RoutineChangeAvtive = false;
                PeakDetectionRoutineHyper.IsChecked = false;
                PeakDetectionRoutineCIF.IsChecked = true;
                PeakDetectionRoutinePattern.IsChecked = false;
                CalScec.Properties.Settings.Default.UsedPeakDetectionId = 1;
                RoutineChangeAvtive = true;
            }
        }

        #endregion

        private void AutomaticPeakFitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CalScec.Properties.Settings.Default.AutomaticPeakFit = this.AutomaticPeakFitMenuItem.IsChecked;
        }
        
        #endregion

        #region Program components

        private void OpenPeakFittingWindow_Click(object sender, RoutedEventArgs e)
        {
            this.PeakFitWindow.Show();
        }

        private void OpenPeakFittingWindowNew_Click(object sender, RoutedEventArgs e)
        {
            if (this.InvestigatedSample.DiffractionPatterns.Count != 0)
            {
                try
                {
                    if (this.InvestigatedSample.DiffractionPatterns[0].PatternCounts.Count != 0)
                    {
                        Analysis.Fitting.PeakFittingWindow PFW = new Analysis.Fitting.PeakFittingWindow(this.InvestigatedSample);
                        PFW.Show();
                    }
                    else
                    {
                        MessageBox.Show("Only Peak Data Found. No Fitting without data", "No Pattern Data Added", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    }
                }
                catch
                {
                    MessageBox.Show("No Pattern data found. The peak data was loaded directly!", "No Pattern Data Added", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
            }
            else
            {
                MessageBox.Show("Only Peak Data Found. Cannot Plot the Pattern", "No Pattern Data Added", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
        }

        private void REKCalculation_Click(object sender, RoutedEventArgs e)
        {
            Analysis.Stress.Microsopic.REKAssociationCalculationWindow REKACW = new Analysis.Stress.Microsopic.REKAssociationCalculationWindow(this.InvestigatedSample);

            REKACW.ShowDialog();

            this.SetProgress();
        }

        private void ElasticityTensorCalculation_Click(object sender, RoutedEventArgs e)
        {
            Analysis.Stress.Microsopic.ElasticityCalculationWindow ElCW = new Analysis.Stress.Microsopic.ElasticityCalculationWindow(this.InvestigatedSample);

            ElCW.ShowDialog();

            this.SetProgress();
        }

        private void OpenModelSimulationWindow_Click(object sender, RoutedEventArgs e)
        {
            if(this.InvestigatedSample.ReussTensorData[0].DiffractionConstants.Count == 0)
            {
                this.InvestigatedSample.ReussTensorData[0].DiffractionConstants = this.InvestigatedSample.DiffractionConstants[0];
            }
            Analysis.MC.RandomAnalysisWindow NewWindow = new Analysis.MC.RandomAnalysisWindow(this.InvestigatedSample.ReussTensorData[0]);
            NewWindow.Show();
        }

        private void YieldSurfaceWindow_Click(object sender, RoutedEventArgs e)
        {
            if (this.SECEllipse.Fill == Brushes.DarkGreen)
            {
                Analysis.Stress.Plasticity.YieldSurfaceWindow YSW = new Analysis.Stress.Plasticity.YieldSurfaceWindow(this.InvestigatedSample);
                YSW.Show();
            }
            else
            {
                MessageBox.Show("The elasto plastic modeling requires single-crystal elastic constants!", "No SEC found!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadTensileTest_Click(object sender, RoutedEventArgs e)
        {
            Analysis.Stress.Macroskopic.TensileDataLoad loadingWindow = new Analysis.Stress.Macroskopic.TensileDataLoad();
            loadingWindow.ShowDialog();
            if (loadingWindow.newTensileTest != null)
            {
                this.InvestigatedSample.TensileTests.Add(loadingWindow.newTensileTest);
            }
        }

        #region Plots

        private void ExtensionStressCalculation_Click(object sender, RoutedEventArgs e)
        {
            Analysis.Stress.Macroskopic.ElasticityWindow ElWindow = new Analysis.Stress.Macroskopic.ElasticityWindow(this.InvestigatedSample);

            ElWindow.ShowDialog();
        }

        private void Sin2PsiPlots_Click(object sender, RoutedEventArgs e)
        {
            Analysis.Stress.Macroskopic.SinPsyWindow SinPsiDistanceWindow = new Analysis.Stress.Macroskopic.SinPsyWindow(this.InvestigatedSample);

            SinPsiDistanceWindow.ShowDialog();
        }

        #endregion

        #endregion

        #endregion

        #region Diffraction Patterns

        #region DragAndDrop

        private void DiffractionPatternList_Drop(object sender, DragEventArgs e)
        {
            if (this.InvestigatedSample.CrystalData.Count != 0)
            {
                string[] droppedFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
                List<string> DiffractionFileList = new List<string>();

                foreach (string s in droppedFiles)
                {
                    bool IsFile = System.IO.File.Exists(s);

                    if (IsFile)
                    {
                        DiffractionFileList.Add(s);
                    }
                    else
                    {
                        bool IsDirectory = System.IO.Directory.Exists(s);
                        if (IsDirectory)
                        {
                            string[] ContainedFiles = System.IO.Directory.GetFiles(s);

                            foreach (string ss in ContainedFiles)
                            {
                                DiffractionFileList.Add(ss);
                            }
                        }
                    }
                }

                string[] ForOpen = DiffractionFileList.ToArray();

                if (ForOpen.Length == 1)
                {
                    BackgroundWorker OpenDiffractionPattern = new BackgroundWorker();

                    OpenDiffractionPattern.DoWork += OpenDiffractionPatternFile_Work;
                    OpenDiffractionPattern.ProgressChanged += OpenDiffractionPatternFile_ProgressChanged;
                    OpenDiffractionPattern.RunWorkerCompleted += OpenDiffractionPatternFile_Completed;
                    OpenDiffractionPattern.WorkerReportsProgress = true;

                    OpenDiffractionPattern.RunWorkerAsync(ForOpen[0]);
                }
                else
                {
                    BackgroundWorker OpenMultipleDiffractionPattern = new BackgroundWorker();

                    OpenMultipleDiffractionPattern.DoWork += OpenMultipleDiffractionPatternFile_Work;
                    OpenMultipleDiffractionPattern.ProgressChanged += OpenMultipleDiffractionPatternFile_ProgressChanged;
                    OpenMultipleDiffractionPattern.RunWorkerCompleted += OpenMultipleDiffractionPatternFile_Completed;
                    OpenMultipleDiffractionPattern.WorkerReportsProgress = true;

                    OpenMultipleDiffractionPattern.RunWorkerAsync(ForOpen);
                }
            }
            else
            {
                MessageBox.Show("Since I am somehow smart I really would like to help you with finding and assigning peaks! But for that I need the crystallographic data first.\n Please be so kind and load it first", "No crystallographic data", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        #endregion

        private void DiffractionPatternList_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Delete)
            {
                int DeletedElements = 0;
                foreach (object DP in this.DiffractionPatternList.SelectedItems)
                {
                    Pattern.DiffractionPattern DPTmp = (Pattern.DiffractionPattern)DP;
                    for(int n = 0; n < DPTmp.PeakRegions.Count; n++)
                    {
                        PeakFitWindow.RemoveRegion(DPTmp.PeakRegions[n]);
                    }
                    this.InvestigatedSample.DiffractionPatterns.Remove(DPTmp);
                    DeletedElements++;
                }

                this.StatusLog1.Foreground = System.Windows.Media.Brushes.DarkGreen;
                this.StatusLog1.Content = "Removing Complete";
                this.StatusLog2.Content = DeletedElements + " / " + DeletedElements;
                this.StatusProgress.Value = 100;
                DiffractionPatternList.Items.Refresh();

                if(this.InvestigatedSample.DiffractionPatterns.Count == 0)
                {
                    DiffractionPlotModel.Series.Clear();
                    this.MainDiffPlot.Model.ResetAllAxes();
                    this.MainDiffPlot.Model.InvalidatePlot(true);
                }
            }
            else if(e.Key == Key.Enter)
            {
                List<Pattern.DiffractionPattern> ForPlot = new List<Pattern.DiffractionPattern>();

                foreach (object DP in this.DiffractionPatternList.SelectedItems)
                {
                    ForPlot.Add((Pattern.DiffractionPattern)DP);
                }

                if (ForPlot.Count != 0)
                {
                    if (ForPlot[0].PatternCounts.Count != 0)
                    {
                        this.DiffractionXAxisLin.Minimum = 0;
                        this.DiffractionYAxisLin.Minimum = 0;
                        this.DiffractionYAxisLog.Minimum = 0;
                        this.DiffractionXAxisLin.Maximum = Double.MinValue;
                        this.DiffractionYAxisLin.Maximum = Double.MinValue;
                        this.DiffractionYAxisLog.Maximum = Double.MinValue;

                        BackgroundWorker worker = new BackgroundWorker();

                        worker.DoWork += PlotMainDiffractionPatterns_Work;
                        worker.WorkerReportsProgress = true;
                        worker.ProgressChanged += PlotMainDiffractionPatterns_ProgressChanged;
                        worker.RunWorkerCompleted += PlotMainDiffractionPatterns_Completed;

                        worker.RunWorkerAsync(ForPlot);
                    }
                    else
                    {
                        MessageBox.Show("Only Peak Data Found. Cannot Plot the Pattern", "No Pattern Data Added", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    }
                }

            }
        }

        private void PlotMainDiffractionPatterns_Work(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            DiffractionPlotModel.Series.Clear();

            List<Pattern.DiffractionPattern> ForPlot = (List<Pattern.DiffractionPattern>)e.Argument;

            string[] State = { "Starting....", "S", Convert.ToString(ForPlot.Count) };
            double Progress = 5.0;

            worker.ReportProgress(Convert.ToInt32(Progress), State);

            int[] Blue  = { 0  , 255, 255, 0  , 0  , 255, 255 };
            int[] Red   = { 255, 0  , 255, 0  , 255, 0  , 255 };
            int[] Green = { 255, 255, 0  , 255, 0  , 0  , 255 };

            Parallel.For(0, ForPlot.Count, i =>
                {
                    if(CalScec.Properties.Settings.Default.MainPlotWithErrors)
                    {

                    }
                    else
                    {
                        OxyPlot.Series.LineSeries Tmp = new OxyPlot.Series.LineSeries();
                        Tmp.Title = ForPlot[Convert.ToInt32(i)].Name;

                        if(CalScec.Properties.Settings.Default.MainPlotLineStyle == 0)
                        {
                            Tmp.LineStyle = OxyPlot.LineStyle.None;
                        }
                        else if (CalScec.Properties.Settings.Default.MainPlotLineStyle == 1)
                        {
                            Tmp.LineStyle = OxyPlot.LineStyle.Dot;
                        }
                        else
                        {
                            Tmp.LineStyle = OxyPlot.LineStyle.Dash;
                        }
                        Tmp.StrokeThickness = CalScec.Properties.Settings.Default.MainPlotLineThickness;
                        Tmp.MarkerSize = CalScec.Properties.Settings.Default.MainPlotDotThickness;
                        
                        switch(i % 6)
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

                        double Round = Math.Floor(i / 7.0);
                        
                        double PrefactorZ = 1.0;
                        double PreFactorN = 1.0;
                        if (Round < 3)
                        {
                            PreFactorN = Math.Pow(2, Round);
                            PrefactorZ = 1.0;
                        }
                        else
                        {
                            for (double n = 2; n <= 2368767; n++)
                            {
                                if (Round < Math.Pow(2, n))
                                {
                                    PreFactorN = Math.Pow(2, n);
                                    PrefactorZ = (2 * (Round - Math.Pow(2, n - 1))) + 1;
                                    break;
                                }
                            }
                        }

                        double PreFactor = PrefactorZ / PreFactorN;
                        int[] ActColor = { 255 - Convert.ToInt32(Blue[i % 7] * PreFactor), 255 - Convert.ToInt32(Red[i % 7] * PreFactor), 255 - Convert.ToInt32(Green[i % 7] * PreFactor) };

                        OxyPlot.OxyColor LineColor = OxyPlot.OxyColor.FromRgb(Convert.ToByte(ActColor[1]), Convert.ToByte(ActColor[2]), Convert.ToByte(ActColor[0]));
                        Tmp.Color = LineColor;
                        Tmp.MarkerStroke = LineColor;
                        

                        foreach(double[] DPP in ForPlot[Convert.ToInt32(i)].PatternCounts)
                        {
                            OxyPlot.DataPoint PlotPoint = new OxyPlot.DataPoint(DPP[0], DPP[1]);
                            Tmp.Points.Add(PlotPoint);

                            if(CalScec.Properties.Settings.Default.PlotYAxes == 0)
                            {
                                if(this.DiffractionYAxisLin.Maximum < DPP[1])
                                {
                                    this.DiffractionYAxisLin.Maximum = DPP[1];
                                    this.DiffractionYAxisLog.Maximum = DPP[1];
                                }
                            }
                            else if (CalScec.Properties.Settings.Default.PlotYAxes == 0)
                            {
                                if (this.DiffractionYAxisLog.Maximum < DPP[1])
                                {
                                    this.DiffractionYAxisLin.Maximum = DPP[1];
                                    this.DiffractionYAxisLog.Maximum = DPP[1];
                                }
                            }
                        }

                        if (this.DiffractionXAxisLin.Maximum < ForPlot[Convert.ToInt32(i)].PatternCounts[ForPlot[Convert.ToInt32(i)].PatternCounts.Count - 1][0])
                        {
                            this.DiffractionXAxisLin.Maximum = ForPlot[Convert.ToInt32(i)].PatternCounts[ForPlot[Convert.ToInt32(i)].PatternCounts.Count - 1][0];
                        }

                        DiffractionPlotModel.Series.Add(Tmp);

                        State[0] = "Pattern " + i + " Finished";
                        State[1] = "P";
                        State[2] = Convert.ToString(ForPlot.Count);
                        Progress += 90.0 / (i + 1);

                        worker.ReportProgress(Convert.ToInt32(Progress), State);
                    }
                });
        }

        private void PlotMainDiffractionPatterns_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.StatusProgress.Value = e.ProgressPercentage;
            string[] StateString = (string[])e.UserState;

            if (StateString[1] == "S")
            {
                this.StatusLog1.Content = "Plotting " + StateString[2] + " patterns";
            }
            if (StateString[1] == "P")
            {
                this.StatusLog1.Content = "Plotting " + StateString[2] + " patterns";
            }

            this.StatusLog2.Content = StateString[0];
        }

        private void PlotMainDiffractionPatterns_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            XAxisMinToolText.Text = "0";
            XAxisMaxToolText.Text = Convert.ToString(Convert.ToInt32(this.DiffractionXAxisLin.Maximum));
            YAxisMinToolText.Text = "0";
            YAxisMaxToolText.Text = Convert.ToString(Convert.ToInt32(this.DiffractionYAxisLin.Maximum));

            this.MainDiffPlot.Model.ResetAllAxes();
            this.MainDiffPlot.Model.InvalidatePlot(true);

            this.StatusProgress.Value = 100;
            this.StatusLog2.Content = "All patterns plotted!";
            this.ErrorLog1.Content = "";
            this.ErrorLog2.Content = "";
        }

        private void DiffractionPatternList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePeakView();

            if (this._textEventactive && this.DiffractionPatternList.SelectedIndex != -1)
            {
                Pattern.DiffractionPattern SelectedPattern = (Pattern.DiffractionPattern)this.DiffractionPatternList.SelectedItem;
                this.PatternName.Text = SelectedPattern.Name;
                this.PatternChiAngle.Text = SelectedPattern.ChiAngle.ToString("F3");
                this.PatternOmegaAngle.Text = SelectedPattern.OmegaAngle.ToString("F3");
                this.PatternAppliedStress.Text = SelectedPattern.Stress.ToString("F3");
                this.PatternAppliedForce.Text = SelectedPattern.Force.ToString("F3");
                this.PatternPhiSampleAngle.Text = SelectedPattern.PhiSampleAngle.ToString("F3");
                this.PatternMacroStrain.Text = SelectedPattern.MacroStrain.ToString();
            }
        }

        private void UpdatePeakView()
        {
            if(this.DiffractionPatternList.SelectedIndex != -1)
            {
                List<Analysis.Peaks.DiffractionPeak> ForDisplay = new List<Analysis.Peaks.DiffractionPeak>();

                foreach (object DP in this.DiffractionPatternList.SelectedItems)
                {
                    Pattern.DiffractionPattern ActDP = (Pattern.DiffractionPattern)DP;
                    ForDisplay.AddRange(ActDP.FoundPeaks);
                }

                PeakList.ItemsSource = ForDisplay;

                CollectionView PeakCollection = (CollectionView)CollectionViewSource.GetDefaultView(PeakList.ItemsSource);
                PropertyGroupDescription PeakGroupDescription = new PropertyGroupDescription("AssociatedPatternName");
                PeakCollection.GroupDescriptions.Add(PeakGroupDescription);
            }
        }


        private void PatternName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.DiffractionPatternList.SelectedIndex != -1 && this._textEventactive)
            {
                Pattern.DiffractionPattern SelectedPattern = (Pattern.DiffractionPattern)this.DiffractionPatternList.SelectedItem;

                SelectedPattern.Name = this.PatternName.Text;
            }
        }

        private void PatternChiAngle_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.DiffractionPatternList.SelectedIndex != -1 && this._textEventactive)
            {
                Pattern.DiffractionPattern SelectedPattern = (Pattern.DiffractionPattern)this.DiffractionPatternList.SelectedItem;

                try
                {
                    double NewValue = Convert.ToDouble(this.PatternChiAngle.Text);

                    SelectedPattern.ChiAngle = NewValue;
                    this.PatternChiAngle.Foreground = Brushes.DarkGreen;
                }
                catch
                {
                    this.PatternChiAngle.Foreground = Brushes.DarkRed;
                }
            }
        }

        private void PatternOmegaAngle_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.DiffractionPatternList.SelectedIndex != -1 && this._textEventactive)
            {
                Pattern.DiffractionPattern SelectedPattern = (Pattern.DiffractionPattern)this.DiffractionPatternList.SelectedItem;

                try
                {
                    double NewValue = Convert.ToDouble(this.PatternOmegaAngle.Text);

                    SelectedPattern.OmegaAngle = NewValue;
                    this.PatternOmegaAngle.Foreground = Brushes.DarkGreen;
                }
                catch
                {
                    this.PatternOmegaAngle.Foreground = Brushes.DarkRed;
                }
            }
        }

        private void PatternPhiSampleAngle_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.DiffractionPatternList.SelectedIndex != -1 && this._textEventactive)
            {
                Pattern.DiffractionPattern SelectedPattern = (Pattern.DiffractionPattern)this.DiffractionPatternList.SelectedItem;

                try
                {
                    double NewValue = Convert.ToDouble(this.PatternPhiSampleAngle.Text);

                    SelectedPattern.PhiSampleAngle = NewValue;
                    this.PatternPhiSampleAngle.Foreground = Brushes.DarkGreen;
                }
                catch
                {
                    this.PatternPhiSampleAngle.Foreground = Brushes.DarkRed;
                }
            }
        }

        private void PatternAppliedForce_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this._textEventactive)
            {
                if (this.DiffractionPatternList.SelectedIndex != -1)
                {
                    Pattern.DiffractionPattern SelectedPattern = (Pattern.DiffractionPattern)this.DiffractionPatternList.SelectedItem;

                    try
                    {
                        double NewValue = Convert.ToDouble(this.PatternAppliedForce.Text);

                        SelectedPattern.Force = NewValue;
                        SelectedPattern.Stress = SelectedPattern.Force / this.InvestigatedSample.Area;
                        this._textEventactive = false;
                        this.PatternAppliedStress.Text = SelectedPattern.Stress.ToString("F3");
                        this._textEventactive = true;
                        this.PatternAppliedForce.Foreground = Brushes.DarkGreen;
                        this.PatternAppliedStress.Foreground = Brushes.DarkGreen;
                    }
                    catch
                    {
                        this.PatternAppliedStress.Foreground = Brushes.DarkRed;
                        this.PatternAppliedForce.Foreground = Brushes.DarkRed;
                    }
                }
            }
        }

        private void PatternAppliedStress_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this._textEventactive)
            {
                if (this.DiffractionPatternList.SelectedIndex != -1)
                {
                    Pattern.DiffractionPattern SelectedPattern = (Pattern.DiffractionPattern)this.DiffractionPatternList.SelectedItem;

                    try
                    {
                        double NewValue = Convert.ToDouble(this.PatternAppliedStress.Text);

                        SelectedPattern.Stress = NewValue;
                        SelectedPattern.Force = SelectedPattern.Stress * this.InvestigatedSample.Area;
                        this._textEventactive = false;
                        this.PatternAppliedForce.Text = SelectedPattern.Force.ToString("F3");
                        this._textEventactive = true;
                        this.PatternAppliedStress.Foreground = Brushes.DarkGreen;
                        this.PatternAppliedForce.Foreground = Brushes.DarkGreen;
                    }
                    catch
                    {
                        this.PatternAppliedStress.Foreground = Brushes.DarkRed;
                        this.PatternAppliedForce.Foreground = Brushes.DarkRed;
                    }
                }
            }
        }

        private void PatternMacroStrain_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this._textEventactive)
            {
                if (this.DiffractionPatternList.SelectedIndex != -1)
                {
                    Pattern.DiffractionPattern SelectedPattern = (Pattern.DiffractionPattern)this.DiffractionPatternList.SelectedItem;

                    try
                    {
                        double NewValue = Convert.ToDouble(this.PatternMacroStrain.Text);

                        SelectedPattern.MacroStrain = NewValue;
                        //this._textEventactive = false;
                        ////this.PatternMacroStrain.Text = SelectedPattern.MacroStrain.ToString();
                        //this._textEventactive = true;
                        this.PatternAppliedStress.Foreground = Brushes.DarkGreen;
                        this.PatternAppliedForce.Foreground = Brushes.DarkGreen;
                    }
                    catch
                    {
                        this.PatternAppliedStress.Foreground = Brushes.DarkRed;
                        this.PatternAppliedForce.Foreground = Brushes.DarkRed;
                    }
                }
            }
        }

        private void AutofillPattern_Click(object sender, RoutedEventArgs e)
        {
            Tools.AutoFillWindow aFW = new Tools.AutoFillWindow(this.InvestigatedSample.Area, this.InvestigatedSample.DiffractionPatterns);

            aFW.ShowDialog();
        }

        #endregion

        #region Peaks

        #region ContextMenu

        private void RemoveHKLAssociationPeak_Click(object sender, RoutedEventArgs e)
        {
            if(this.PeakList.SelectedIndex != -1)
            {
                MessageBoxResult MR = MessageBox.Show("Should I remove the association from all diffraction patterns or just from the selected ones?", "All or just selected patterns", MessageBoxButton.YesNo, MessageBoxImage.Question);
                foreach (object DPP in this.PeakList.SelectedItems)
                {
                    Analysis.Peaks.DiffractionPeak ActualPeak = (Analysis.Peaks.DiffractionPeak)DPP;
                    if (MR == MessageBoxResult.Yes)
                    {
                        foreach (Pattern.DiffractionPattern DP in this.InvestigatedSample.DiffractionPatterns)
                        {
                            Analysis.Peaks.Detection.RemoveAssociationFromSimilarPeak(DP, ActualPeak);
                        }
                    }
                    else
                    {
                        foreach (object DP in this.DiffractionPatternList.SelectedItems)
                        {
                            Analysis.Peaks.Detection.RemoveAssociationFromSimilarPeak((Pattern.DiffractionPattern)DP, ActualPeak);
                        }
                    }
                }

                UpdatePeakView();
            }
        }

        private void ChangeHKLAssociationPeak_Click(object sender, RoutedEventArgs e)
        {
            if (this.PeakList.SelectedIndex != -1)
            {
                Analysis.Peaks.AssociatePeakToHKLWindow AssociatePeakHKLW = new Analysis.Peaks.AssociatePeakToHKLWindow(this.InvestigatedSample.CrystalData);
                AssociatePeakHKLW.ShowDialog();

                if (AssociatePeakHKLW.SelectedIndices[0] != -1 && AssociatePeakHKLW.SelectedIndices[1] != -1)
                {
                    MessageBoxResult MR = MessageBox.Show("Should I change the association from all diffraction patterns or just from the selected ones?", "All or just selected patterns", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    foreach (object DPP in this.PeakList.SelectedItems)
                    {
                        Analysis.Peaks.DiffractionPeak ActualPeak = (Analysis.Peaks.DiffractionPeak)DPP;
                        if (MR == MessageBoxResult.Yes)
                        {
                            foreach (Pattern.DiffractionPattern DP in this.InvestigatedSample.DiffractionPatterns)
                            {
                                Analysis.Peaks.Detection.ManualAssociationFromSimilarPeak(DP, ActualPeak, this.InvestigatedSample.CrystalData[AssociatePeakHKLW.SelectedIndices[0]], this.InvestigatedSample.CrystalData[AssociatePeakHKLW.SelectedIndices[0]].HKLList[AssociatePeakHKLW.SelectedIndices[1]]);
                            }
                        }
                        else
                        {
                            foreach (object DP in this.DiffractionPatternList.SelectedItems)
                            {
                                Analysis.Peaks.Detection.ManualAssociationFromSimilarPeak((Pattern.DiffractionPattern)DP, ActualPeak, this.InvestigatedSample.CrystalData[AssociatePeakHKLW.SelectedIndices[0]], this.InvestigatedSample.CrystalData[AssociatePeakHKLW.SelectedIndices[0]].HKLList[AssociatePeakHKLW.SelectedIndices[1]]);
                            }
                        }
                    }

                    UpdatePeakView();
                }
            }
        }

        #endregion

        private void PeakList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.PeakAnnotationList.Clear();

            foreach (object Peak in this.PeakList.SelectedItems)
            {
                Analysis.Peaks.DiffractionPeak DP = (Analysis.Peaks.DiffractionPeak)Peak;

                var MarkerLine = new OxyPlot.Annotations.LineAnnotation();
                MarkerLine.Color = OxyPlot.OxyColor.FromRgb(Convert.ToByte(39), Convert.ToByte(39), Convert.ToByte(39));
                MarkerLine.ClipByYAxis = true;
                MarkerLine.X = DP.Angle;
                MarkerLine.Type = OxyPlot.Annotations.LineAnnotationType.Vertical;
                MarkerLine.StrokeThickness = CalScec.Properties.Settings.Default.PeakMarkingThickness;
                MarkerLine.Text = DP.HKLAssociation;
                MarkerLine.ClipText = true;

                if (CalScec.Properties.Settings.Default.PeakMarkingStyle == 0)
                {
                    MarkerLine.LineStyle = OxyPlot.LineStyle.Dot;
                }
                else if (CalScec.Properties.Settings.Default.PeakMarkingStyle == 1)
                {
                    MarkerLine.LineStyle = OxyPlot.LineStyle.Dash;
                }


                this.PeakAnnotationList.Add(MarkerLine);
            }

            RefreshAnnotationToPlot();
        }

        private void PeakList_KeyDown(object sender, KeyEventArgs e)
        {
            if(this.DiffractionPatternList.SelectedIndex != -1)
            {
                if (e.Key == Key.Delete)
                {
                    int DeletedElements = 0;
                    foreach (object DP in this.DiffractionPatternList.SelectedItems)
                    {
                        Pattern.DiffractionPattern TryDelete = (Pattern.DiffractionPattern)DP;
                        foreach (object DPP in this.PeakList.SelectedItems)
                        {
                            Analysis.Peaks.DiffractionPeak DPDelete = (Analysis.Peaks.DiffractionPeak)DPP;
                            bool Suc = TryDelete.FoundPeaks.Remove(DPDelete);
                            if (Suc)
                            {
                                for(int n = 0; n < TryDelete.PeakRegions.Count; n++)
                                {
                                    bool RegSuc = TryDelete.PeakRegions[n].Remove(DPDelete.PFunction);
                                    if(RegSuc)
                                    {
                                        if(TryDelete.PeakRegions[n].Count == 0)
                                        {
                                            PeakFitWindow.RemoveRegion(TryDelete.PeakRegions[n]);
                                            TryDelete.PeakRegions.Remove(TryDelete.PeakRegions[n]);
                                        }
                                        break;
                                    }
                                }
                                DeletedElements++;
                            }
                        }

                        
                    }

                    this.StatusLog1.Foreground = System.Windows.Media.Brushes.DarkGreen;
                    this.StatusLog1.Content = "Removing Complete";
                    this.StatusLog2.Content = DeletedElements + " / " + DeletedElements;
                    this.StatusProgress.Value = 100;

                    this.DiffractionPatternList.Items.Refresh();
                    UpdatePeakView();
                    
                }
            }
        }

        #endregion

        protected override void OnClosed(EventArgs e)
        {
            this.PeakFitWindow.PreventClosing = false;
            this.PeakFitWindow.Close();
            base.OnClosed(e);
        }

        private void SampleName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this._textEventactive)
            {
                this.InvestigatedSample.Name = this.SampleName.Text;
            }
        }

        private void SampleArea_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this._textEventactive)
            {
                try
                {
                    double newValue = Convert.ToDouble(this.SampleArea.Text);
                    this.InvestigatedSample.Area = newValue;
                    this.SampleArea.Background = Brushes.White;
                }
                catch
                {
                    this.SampleArea.Background = Brushes.Red;
                }
            }
        }

        #region Sample Composition Display

        private void PhaseSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PhaseSelection.SelectedIndex != -1)
            {
                this._textEventactive = false;

                #region Composition and Phase information

                if (this.InvestigatedSample.CrystalData[PhaseSelection.SelectedIndex].Matrix)
                {
                    this.PhaseTypeSelection.SelectedIndex = 0;

                    this.PhaseParameter1Label.Content = "";
                    this.PhaseParameter2Label.Content = "";
                    this.PhaseParameter3Label.Content = "";

                    this.PhaseParameter1Text.Visibility = Visibility.Hidden;
                    this.PhaseParameter2Text.Visibility = Visibility.Hidden;
                    this.PhaseParameter3Text.Visibility = Visibility.Hidden;
                }
                else
                {
                    this.PhaseTypeSelection.SelectedIndex = 1;
                }

                if (this.InclusionTypeSelection.Items.Count > this.InvestigatedSample.CrystalData[PhaseSelection.SelectedIndex].InclusionType)
                {
                    this.InclusionTypeSelection.SelectedIndex = this.InvestigatedSample.CrystalData[PhaseSelection.SelectedIndex].InclusionType;
                }

                this.PhaseFraction.Text = this.InvestigatedSample.CrystalData[PhaseSelection.SelectedIndex].PhaseFraction.ToString();

                if (!this.InvestigatedSample.CrystalData[PhaseSelection.SelectedIndex].Matrix)
                {
                    if (this.InvestigatedSample.CrystalData[PhaseSelection.SelectedIndex].InclusionType == 0)
                    {
                        this.PhaseParameter1Label.Content = "";
                        this.PhaseParameter2Label.Content = "";
                        this.PhaseParameter3Label.Content = "";

                        this.PhaseParameter1Text.Visibility = Visibility.Hidden;
                        this.PhaseParameter2Text.Visibility = Visibility.Hidden;
                        this.PhaseParameter3Text.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        this.PhaseParameter1Label.Content = "Elliptic A";
                        this.PhaseParameter2Label.Content = "Elliptic B";
                        this.PhaseParameter3Label.Content = "Elliptic C";

                        this.PhaseParameter1Text.Visibility = Visibility.Visible;
                        this.PhaseParameter2Text.Visibility = Visibility.Visible;
                        this.PhaseParameter3Text.Visibility = Visibility.Visible;
                    }
                }

                #endregion

                this.CrystalNameText.Text = this.InvestigatedSample.CrystalData[PhaseSelection.SelectedIndex].Name;
                this.CellAText.Text = this.InvestigatedSample.CrystalData[PhaseSelection.SelectedIndex].A.ToString();
                this.CellBText.Text = this.InvestigatedSample.CrystalData[PhaseSelection.SelectedIndex].B.ToString();
                this.CellCText.Text = this.InvestigatedSample.CrystalData[PhaseSelection.SelectedIndex].C.ToString();
                this.CellAlphaText.Text = this.InvestigatedSample.CrystalData[PhaseSelection.SelectedIndex].Alpha.ToString();
                this.CellBetaText.Text = this.InvestigatedSample.CrystalData[PhaseSelection.SelectedIndex].Beta.ToString();
                this.CellGammaText.Text = this.InvestigatedSample.CrystalData[PhaseSelection.SelectedIndex].Gamma.ToString();
                this.ElementalCompositionText.Text = this.InvestigatedSample.CrystalData[PhaseSelection.SelectedIndex].ChemicalFormula;
                this.SymetryBox.Text = this.InvestigatedSample.CrystalData[PhaseSelection.SelectedIndex].SymmetryGroup;
                
                if(this.DECEllipse.Fill == Brushes.DarkGreen)
                {
                    this.REKClassicCalculationList.ItemsSource = this.InvestigatedSample.DiffractionConstants[this.PhaseSelection.SelectedIndex];
                }

                this._textEventactive = true;
            }
        }

        private void PhaseTypeSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.PhaseTypeSelection.SelectedIndex != -1 && PhaseSelection.SelectedIndex != -1)
            {
                if (this._textEventactive)
                {
                    if (PhaseTypeSelection.SelectedIndex == 0)
                    {
                        this.InvestigatedSample.CrystalData[PhaseSelection.SelectedIndex].Matrix = true;
                    }
                    else
                    {
                        this.InvestigatedSample.CrystalData[PhaseSelection.SelectedIndex].Matrix = false;
                    }
                }

                this.InclusionTypeSelection.Items.Clear();

                if (this.PhaseTypeSelection.SelectedIndex == 0)
                {
                    ComboBoxItem cTmp1 = new ComboBoxItem();
                    cTmp1.Content = "Equal rank";
                    this.InclusionTypeSelection.Items.Add(cTmp1);

                    this.PhaseParameter1Label.Content = "";
                    this.PhaseParameter2Label.Content = "";
                    this.PhaseParameter3Label.Content = "";

                    this.PhaseParameter1Text.Visibility = Visibility.Hidden;
                    this.PhaseParameter2Text.Visibility = Visibility.Hidden;
                    this.PhaseParameter3Text.Visibility = Visibility.Hidden;
                }
                else
                {
                    ComboBoxItem cTmp1 = new ComboBoxItem();
                    cTmp1.Content = "Sphere";
                    this.InclusionTypeSelection.Items.Add(cTmp1);

                    ComboBoxItem cTmp2 = new ComboBoxItem();
                    cTmp2.Content = "Ellipsoidal";
                    this.InclusionTypeSelection.Items.Add(cTmp2);

                    if (this.InvestigatedSample.CrystalData[PhaseSelection.SelectedIndex].InclusionType == 0)
                    {
                        this.PhaseParameter1Label.Content = "";
                        this.PhaseParameter2Label.Content = "";
                        this.PhaseParameter3Label.Content = "";

                        this.PhaseParameter1Text.Visibility = Visibility.Hidden;
                        this.PhaseParameter2Text.Visibility = Visibility.Hidden;
                        this.PhaseParameter3Text.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        this.PhaseParameter1Label.Content = "Elliptic A";
                        this.PhaseParameter2Label.Content = "Elliptic B";
                        this.PhaseParameter3Label.Content = "Elliptic C";

                        this.PhaseParameter1Text.Visibility = Visibility.Visible;
                        this.PhaseParameter2Text.Visibility = Visibility.Visible;
                        this.PhaseParameter3Text.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private void InclusionTypeSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.PhaseSelection.SelectedIndex != -1)
            {
                if (this._textEventactive)
                {
                    this.InvestigatedSample.CrystalData[PhaseSelection.SelectedIndex].InclusionType = InclusionTypeSelection.SelectedIndex;
                }

                if (!this.InvestigatedSample.CrystalData[PhaseSelection.SelectedIndex].Matrix)
                {
                    if (this.InvestigatedSample.CrystalData[PhaseSelection.SelectedIndex].InclusionType == 0)
                    {
                        this.PhaseParameter1Label.Content = "";
                        this.PhaseParameter2Label.Content = "";
                        this.PhaseParameter3Label.Content = "";

                        this.PhaseParameter1Text.Visibility = Visibility.Hidden;
                        this.PhaseParameter2Text.Visibility = Visibility.Hidden;
                        this.PhaseParameter3Text.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        this.PhaseParameter1Label.Content = "Elliptic A";
                        this.PhaseParameter2Label.Content = "Elliptic B";
                        this.PhaseParameter3Label.Content = "Elliptic C";

                        this.PhaseParameter1Text.Visibility = Visibility.Visible;
                        this.PhaseParameter2Text.Visibility = Visibility.Visible;
                        this.PhaseParameter3Text.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    this.PhaseParameter1Label.Content = "";
                    this.PhaseParameter2Label.Content = "";
                    this.PhaseParameter3Label.Content = "";

                    this.PhaseParameter1Text.Visibility = Visibility.Hidden;
                    this.PhaseParameter2Text.Visibility = Visibility.Hidden;
                    this.PhaseParameter3Text.Visibility = Visibility.Hidden;
                }
            }
        }

        private void PhaseFraction_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.PhaseSelection.SelectedIndex != -1 && this._textEventactive)
            {
                try
                {
                    this.InvestigatedSample.CrystalData[PhaseSelection.SelectedIndex].PhaseFraction = Convert.ToDouble(PhaseFraction.Text);
                }
                catch
                {

                }
            }
        }

        private void PhaseParameter1Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.PhaseSelection.SelectedIndex != -1 && this._textEventactive)
            {
                try
                {
                    this.InvestigatedSample.CrystalData[PhaseSelection.SelectedIndex].InclusionA = Convert.ToDouble(PhaseParameter1Text.Text);
                }
                catch
                {

                }
            }
        }

        private void PhaseParameter2Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.PhaseSelection.SelectedIndex != -1 && this._textEventactive)
            {
                try
                {
                    this.InvestigatedSample.CrystalData[PhaseSelection.SelectedIndex].InclusionB = Convert.ToDouble(PhaseParameter2Text.Text);
                }
                catch
                {

                }
            }
        }

        private void PhaseParameter3Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.PhaseSelection.SelectedIndex != -1 && this._textEventactive)
            {
                try
                {
                    this.InvestigatedSample.CrystalData[PhaseSelection.SelectedIndex].InclusionC = Convert.ToDouble(PhaseParameter3Text.Text);
                }
                catch
                {

                }
            }
        }

        private void CrystalSettingsChanged_Click(object sender, RoutedEventArgs e)
        {
            if(this.PhaseSelection.SelectedIndex != -1)
            {
                this.InvestigatedSample.CrystalData[this.PhaseSelection.SelectedIndex].Name = this.CrystalNameText.Text;
                try
                {
                    this.InvestigatedSample.CrystalData[this.PhaseSelection.SelectedIndex].C = Convert.ToDouble(this.CellCText.Text);
                    this.InvestigatedSample.CrystalData[this.PhaseSelection.SelectedIndex].B = Convert.ToDouble(this.CellBText.Text);
                    this.InvestigatedSample.CrystalData[this.PhaseSelection.SelectedIndex].A = Convert.ToDouble(this.CellAText.Text);

                    this.InvestigatedSample.CrystalData[this.PhaseSelection.SelectedIndex].Gamma = Convert.ToDouble(this.CellGammaText.Text);
                    this.InvestigatedSample.CrystalData[this.PhaseSelection.SelectedIndex].Beta = Convert.ToDouble(this.CellBetaText.Text);
                    this.InvestigatedSample.CrystalData[this.PhaseSelection.SelectedIndex].Alpha = Convert.ToDouble(this.CellAlphaText.Text);

                    this.InvestigatedSample.CrystalData[this.PhaseSelection.SelectedIndex].RefreshHKLList();
                    this.PlotHKLToPlot();
                }
                catch
                {
                    this.CellCText.Text = this.InvestigatedSample.CrystalData[this.PhaseSelection.SelectedIndex].C.ToString("F3");
                    this.CellBText.Text = this.InvestigatedSample.CrystalData[this.PhaseSelection.SelectedIndex].B.ToString("F3");
                    this.CellAText.Text = this.InvestigatedSample.CrystalData[this.PhaseSelection.SelectedIndex].A.ToString("F3");

                    this.CellGammaText.Text = this.InvestigatedSample.CrystalData[this.PhaseSelection.SelectedIndex].Gamma.ToString("F3");
                    this.CellBetaText.Text = this.InvestigatedSample.CrystalData[this.PhaseSelection.SelectedIndex].Beta.ToString("F3");
                    this.CellAlphaText.Text = this.InvestigatedSample.CrystalData[this.PhaseSelection.SelectedIndex].Alpha.ToString("F3");
                }
            }
        }
        #endregion

    }
}
