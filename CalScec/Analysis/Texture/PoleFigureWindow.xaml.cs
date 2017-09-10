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

namespace CalScec.Analysis.Texture
{
    /// <summary>
    /// Interaktionslogik für PoleFigureWindow.xaml
    /// </summary>
    public partial class PoleFigureWindow : Window
    {
        public OxyPlot.PlotModel MainPlotModel = new OxyPlot.PlotModel();
        OxyPlot.Axes.LinearAxis MainXAxisLin = new OxyPlot.Axes.LinearAxis();
        OxyPlot.Axes.LinearColorAxis MainYAxisLin = new OxyPlot.Axes.LinearColorAxis();

        public OxyPlot.PlotModel SecondPlotModel = new OxyPlot.PlotModel();
        OxyPlot.Axes.LinearAxis SecondXAxisLin = new OxyPlot.Axes.LinearAxis();
        OxyPlot.Axes.LinearColorAxis SecondYAxisLin = new OxyPlot.Axes.LinearColorAxis();

        Sample InvestigatedSample = new Sample();

        public PoleFigureWindow(Sample investigatedSample)
        {
            InitializeComponent();

            this.InvestigatedSample = investigatedSample;

            SetPlot();
            SetDataBindings();
        }

        public void SetDataBindings()
        {
            for(int n = 0; n < this.InvestigatedSample.CrystalData.Count; n++)
            {
                ComboBoxItem PhaseBox1 = new ComboBoxItem();
                PhaseBox1.Content = this.InvestigatedSample.CrystalData[n].SymmetryGroup;

                this.MainPhaseSelection.Items.Add(PhaseBox1);

                ComboBoxItem PhaseBox2 = new ComboBoxItem();
                PhaseBox2.Content = this.InvestigatedSample.CrystalData[n].SymmetryGroup;

                this.SecondPhaseSelection.Items.Add(PhaseBox2);
            }

            ComboBoxItem AngleSwitch1 = new ComboBoxItem();
            AngleSwitch1.Content = "Phi";

            ComboBoxItem AngleSwitch2 = new ComboBoxItem();
            AngleSwitch2.Content = "VarPhi2";

            ComboBoxItem AngleSwitch3 = new ComboBoxItem();
            AngleSwitch3.Content = "Phi";

            ComboBoxItem AngleSwitch4 = new ComboBoxItem();
            AngleSwitch4.Content = "VarPhi2";

            this.MainAngleSelection.Items.Add(AngleSwitch1);
            this.MainAngleSelection.Items.Add(AngleSwitch2);

            this.SecondAngleSelection.Items.Add(AngleSwitch3);
            this.SecondAngleSelection.Items.Add(AngleSwitch4);
        }

        public void SetPlot()
        {
            MainPlotModel.LegendBorder = OxyPlot.OxyColors.Black;
            MainPlotModel.LegendItemAlignment = OxyPlot.HorizontalAlignment.Left;
            MainPlotModel.LegendTitle = "Diffraction pattern";

            MainXAxisLin.Position = OxyPlot.Axes.AxisPosition.Bottom;
            MainXAxisLin.Minimum = 0;
            MainXAxisLin.Maximum = 180;
            MainXAxisLin.Title = "Angle";

            MainYAxisLin.Position = OxyPlot.Axes.AxisPosition.Left;
            MainYAxisLin.Minimum = 0;
            MainYAxisLin.Maximum = 100;
            MainYAxisLin.Title = "Intensity";
            MainYAxisLin.Palette = OxyPlot.OxyPalettes.Rainbow(100);

            SecondPlotModel.LegendBorder = OxyPlot.OxyColors.Black;
            SecondPlotModel.LegendItemAlignment = OxyPlot.HorizontalAlignment.Left;
            SecondPlotModel.LegendTitle = "Diffraction pattern";

            SecondXAxisLin.Position = OxyPlot.Axes.AxisPosition.Bottom;
            SecondXAxisLin.Minimum = 0;
            SecondXAxisLin.Maximum = 180;
            SecondXAxisLin.Title = "Angle";

            SecondYAxisLin.Position = OxyPlot.Axes.AxisPosition.Left;
            SecondYAxisLin.Minimum = 0;
            SecondYAxisLin.Maximum = 100;
            SecondYAxisLin.Title = "Intensity";
            SecondYAxisLin.Palette = OxyPlot.OxyPalettes.Hue(100);

            MainXAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dot;
            MainYAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dot;
            SecondXAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dot;
            SecondYAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dot;

            MainPlotModel.Axes.Add(MainXAxisLin);
            MainPlotModel.Axes.Add(MainYAxisLin);
            SecondPlotModel.Axes.Add(SecondXAxisLin);
            SecondPlotModel.Axes.Add(SecondYAxisLin);

            this.MainPoleFigurePlot.Model = MainPlotModel;
            this.MainPoleFigurePlot.Model.ResetAllAxes();
            this.MainPoleFigurePlot.Model.InvalidatePlot(true);
            this.SecondPoleFigurePlot.Model = SecondPlotModel;
            this.SecondPoleFigurePlot.Model.ResetAllAxes();
            this.SecondPoleFigurePlot.Model.InvalidatePlot(true);
        }

        private void MainPlotSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainPhaseSelection.SelectedIndex != -1 && MainAngleSelection.SelectedIndex != -1 && MainHKLSelection.SelectedIndex != -1)
            {
                MainPlotModel.Series.Clear();

                bool found = false;
                DataManagment.CrystalData.HKLReflex SelectedReflex = new DataManagment.CrystalData.HKLReflex();
                ComboBoxItem HKLComboItemTmp = (ComboBoxItem)MainHKLSelection.Items[MainHKLSelection.SelectedIndex];
                string HKLTmp = Convert.ToString(HKLComboItemTmp.Content);
                for (int n = 0; n < this.InvestigatedSample.CrystalData[MainPhaseSelection.SelectedIndex].HKLList.Count; n++)
                {
                    if (this.InvestigatedSample.CrystalData[MainPhaseSelection.SelectedIndex].HKLList[n].HKLString == HKLTmp)
                    {
                        SelectedReflex = this.InvestigatedSample.CrystalData[MainPhaseSelection.SelectedIndex].HKLList[n];
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    OxyPlot.Series.HeatMapSeries NewPolePlot = GetTexturePlot(MainPhaseSelection.SelectedIndex, MainAngleSelection.SelectedIndex, SelectedReflex);
                    MainPlotModel.Series.Add(NewPolePlot);

                    this.MainYAxisLin.Maximum = this.InvestigatedSample.ReussTensorData[MainPhaseSelection.SelectedIndex].ODF.MaxMRD;

                    this.MainPlotModel.ResetAllAxes();
                    this.MainPlotModel.InvalidatePlot(true);
                }
            }
        }

        private void MainPhaseSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MainHKLSelection.Items.Clear();
            for (int n = 0; n < this.InvestigatedSample.CrystalData[MainPhaseSelection.SelectedIndex].HKLList.Count; n++)
            {
                ComboBoxItem HKLItem = new ComboBoxItem();
                HKLItem.Content = this.InvestigatedSample.CrystalData[MainPhaseSelection.SelectedIndex].HKLList[n].HKLString;

                MainHKLSelection.Items.Add(HKLItem);
            }
        }

        private void SecondPlotSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SecondPhaseSelection.SelectedIndex != -1 && SecondAngleSelection.SelectedIndex != -1)
            {
                SecondPlotModel.Series.Clear();

                bool found = false;
                DataManagment.CrystalData.HKLReflex SelectedReflex = new DataManagment.CrystalData.HKLReflex();

                for (int n = 0; n < this.InvestigatedSample.CrystalData[SecondPhaseSelection.SelectedIndex].HKLList.Count; n++)
                {
                    if (this.InvestigatedSample.CrystalData[SecondPhaseSelection.SelectedIndex].HKLList[n].HKLString == SecondHKLSelection.Text)
                    {
                        SelectedReflex = this.InvestigatedSample.CrystalData[SecondPhaseSelection.SelectedIndex].HKLList[n];
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    OxyPlot.Series.HeatMapSeries NewPolePlot = GetTexturePlot(SecondPhaseSelection.SelectedIndex, SecondAngleSelection.SelectedIndex, SelectedReflex);
                    SecondPlotModel.Series.Add(NewPolePlot);

                    this.SecondYAxisLin.Maximum = this.InvestigatedSample.ReussTensorData[SecondPhaseSelection.SelectedIndex].ODF.MaxMRD;

                    this.SecondPlotModel.ResetAllAxes();
                    this.SecondPlotModel.InvalidatePlot(true);
                }
            }
        }

        private void SecondPhaseSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SecondHKLSelection.Items.Clear();
            for (int n = 0; n < this.InvestigatedSample.CrystalData[SecondPhaseSelection.SelectedIndex].HKLList.Count; n++)
            {
                ComboBoxItem HKLItem = new ComboBoxItem();
                HKLItem.Content = this.InvestigatedSample.CrystalData[SecondPhaseSelection.SelectedIndex].HKLList[n].HKLString;

                SecondHKLSelection.Items.Add(HKLItem);
            }
        }

        private OxyPlot.Series.HeatMapSeries GetTexturePlot(int phase, int angle, DataManagment.CrystalData.HKLReflex eReflex)
        {
            List<double[]> TData = new List<double[]>();
            OxyPlot.Series.HeatMapSeries MainPoleFigure = new OxyPlot.Series.HeatMapSeries();
            double YCount = 0;
            if (angle == 0)
            {
                TData = this.InvestigatedSample.ReussTensorData[phase].ODF.GetPoleFigurePhi(eReflex);
                MainPoleFigure.Y1 = TData[TData.Count - 1][1];
                YCount = 360 / this.InvestigatedSample.ReussTensorData[phase].ODF.StepSizePhi2;
            }
            else if (angle == 1)
            {
                TData = this.InvestigatedSample.ReussTensorData[phase].ODF.GetPoleFigureVarPhi2(eReflex);
                MainPoleFigure.Y1 = 90.0;
                YCount = 90 / this.InvestigatedSample.ReussTensorData[phase].ODF.StepSizePhi2;
            }

            MainPoleFigure.X0 = 0.0;
            MainPoleFigure.X1 = TData[TData.Count - 1][0];
            MainPoleFigure.Y0 = 0.0;
            
            double maxStepsVarPhi1 = 360.0 / this.InvestigatedSample.ReussTensorData[phase].ODF.StepSizePhi1;
            var PlotData = new double[Convert.ToInt32(maxStepsVarPhi1), Convert.ToInt32(YCount)];

            for (int n = 0; n < maxStepsVarPhi1; n++)
            {
                for (int i = 0; i < YCount; i++)
                {
                    if ((n * maxStepsVarPhi1) + i < TData.Count)
                    {
                        PlotData[n, i] = TData[Convert.ToInt32(n * maxStepsVarPhi1) + i][2];
                    }
                }
            }

            //MainPoleFigure.CoordinateDefinition = OxyPlot.Series.HeatMapCoordinateDefinition.Edge;
            MainPoleFigure.RenderMethod = OxyPlot.Series.HeatMapRenderMethod.Bitmap;
            MainPoleFigure.Interpolate = true;
            MainPoleFigure.Data = PlotData;

            return MainPoleFigure;


        }

        
    }
}
