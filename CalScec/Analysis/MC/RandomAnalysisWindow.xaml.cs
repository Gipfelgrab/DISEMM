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

namespace CalScec.Analysis.MC
{
    /// <summary>
    /// Interaktionslogik für RandomAnalysisWindow.xaml
    /// </summary>
    public partial class RandomAnalysisWindow : Window
    {
        #region Parameters

        List<Stress.Microsopic.ElasticityTensors> PreSelectedAnalysisTensors = new List<Stress.Microsopic.ElasticityTensors>();
        List<Stress.Microsopic.REK> PreSelectedAnalysisREKs = new List<Stress.Microsopic.REK>();

        List<RandomAnalysis> RandomAnalisisData = new List<RandomAnalysis>();
        int SelectedAnaylsisIndex = -1;

        public OxyPlot.PlotModel MainPlotModel = new OxyPlot.PlotModel();
        OxyPlot.Axes.LinearAxis MainXAxisLin = new OxyPlot.Axes.LinearAxis();
        OxyPlot.Axes.LinearAxis MainYAxisLin = new OxyPlot.Axes.LinearAxis();

        public OxyPlot.PlotModel SecundaryPlotModel = new OxyPlot.PlotModel();
        OxyPlot.Axes.LinearAxis SecundaryXAxisLin = new OxyPlot.Axes.LinearAxis();
        OxyPlot.Axes.LinearAxis SecundaryYAxisLin = new OxyPlot.Axes.LinearAxis();

        bool NameChangeactive = true;

        #endregion

        public void AddNewAnalysisData()
        {
            if(this.SelectedAnaylsisIndex != -1)
            {
                RandomAnalysis RATmp = new RandomAnalysis(RandomAnalisisData[this.SelectedAnaylsisIndex].InvestigatedTensor);
                RandomAnalisisData.Add(RATmp);

                RefreshSources();
            }
        }

        public void AddNewAnalysisData(Stress.Microsopic.ElasticityTensors ET)
        {
            RandomAnalysis RATmp = new RandomAnalysis(ET);
            RandomAnalisisData.Add(RATmp);

            RefreshSources();
        }

        public RandomAnalysisWindow()
        {
            InitializeComponent();

            FillComboBoxes();
            AddSources();

            SetMainPlot();
            SetSecundaryPlot();
        }

        public RandomAnalysisWindow(Stress.Microsopic.ElasticityTensors ET)
        {
            InitializeComponent();

            RandomAnalysis RATmp = new RandomAnalysis(ET);
            RandomAnalisisData.Add(RATmp);
            PreSelectedAnalysisTensors.Add(ET);

            FillComboBoxes();
            AddSources();
            RefreshSources();

            SetMainPlot();
            SetSecundaryPlot();
        }

        private void FillComboBoxes()
        {
            for(int n = 0; n < 13; n++)
            {
                string Name = RandomAnalysis.GetStiffnessConstantName(n);
                
                ComboBoxItem FirstItem = new ComboBoxItem();
                ComboBoxItem SecundaryItem = new ComboBoxItem();

                FirstItem.Content = Name;
                SecundaryItem.Content = Name;

                MainParameterPlot.Items.Add(FirstItem);
                SecundaryParameterPlot.Items.Add(SecundaryItem);
            }

            MainParameterPlot.SelectedIndex = 0;
            SecundaryParameterPlot.SelectedIndex = 0; ;

            for (int n = 0; n < 13; n++)
            {
                string Name = RandomAnalysis.GetComplianceConstantName(n);

                ComboBoxItem FirstItem = new ComboBoxItem();
                ComboBoxItem SecundaryItem = new ComboBoxItem();

                FirstItem.Content = Name;
                SecundaryItem.Content = Name;

                MainParameterPlot.Items.Add(FirstItem);
                SecundaryParameterPlot.Items.Add(SecundaryItem);
            }

            ComboBoxItem simulationModelItem0 = new ComboBoxItem();
            ComboBoxItem simulationModelItem1 = new ComboBoxItem();
            ComboBoxItem simulationModelItem2 = new ComboBoxItem();
            ComboBoxItem simulationModelItem3 = new ComboBoxItem();
            ComboBoxItem simulationModelItem4 = new ComboBoxItem();

            simulationModelItem0.Content = "Voigt";
            simulationModelItem1.Content = "Reuss";
            simulationModelItem2.Content = "Hill";
            simulationModelItem3.Content = "Kroener";
            simulationModelItem4.Content = "DeWitt";

            SimulationModelComboBox.Items.Add(simulationModelItem0);
            SimulationModelComboBox.Items.Add(simulationModelItem1);
            SimulationModelComboBox.Items.Add(simulationModelItem2);
            SimulationModelComboBox.Items.Add(simulationModelItem3);
            SimulationModelComboBox.Items.Add(simulationModelItem4);

            SimulationModelComboBox.SelectedIndex = 3;

            ComboBoxItem simulationModeItem0 = new ComboBoxItem();
            ComboBoxItem simulationModeItem1 = new ComboBoxItem();

            simulationModeItem0.Content = "Equal distribution";
            simulationModeItem1.Content = "Random distribution";

            SimulationModeComboBox.Items.Add(simulationModeItem0);
            SimulationModeComboBox.Items.Add(simulationModeItem1);

            SimulationModeComboBox.SelectedIndex = 0;

            ComboBoxItem simulationConstantTypeItem0 = new ComboBoxItem();
            ComboBoxItem simulationConstantTypeItem1 = new ComboBoxItem();

            simulationConstantTypeItem0.Content = "Stiffness";
            simulationConstantTypeItem1.Content = "Compliance";

            InitialValueCalculationComboBox.Items.Add(simulationConstantTypeItem0);
            InitialValueCalculationComboBox.Items.Add(simulationConstantTypeItem1);

            InitialValueCalculationComboBox.SelectedIndex = 0;
        }

        private void AddSources()
        {
            ParameterGrid.ItemsSource = this.RandomAnalisisData;
        }

        private void SetMainPlot()
        {
            MainPlotModel.LegendBorder = OxyPlot.OxyColors.Black;
            MainPlotModel.LegendItemAlignment = OxyPlot.HorizontalAlignment.Left;
            MainPlotModel.LegendTitle = "Analysis";

            MainXAxisLin.Position = OxyPlot.Axes.AxisPosition.Bottom;
            MainXAxisLin.Minimum = 0;
            MainXAxisLin.Maximum = 180;
            MainXAxisLin.Title = "Constant in MPa";

            MainYAxisLin.Position = OxyPlot.Axes.AxisPosition.Left;
            MainYAxisLin.Minimum = 0;
            MainYAxisLin.Maximum = 100;
            MainYAxisLin.Title = "Chi2";

            #region GridStyles

            MainXAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dot;
            MainYAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dot;

            MainXAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dot;
            MainYAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dot;

            #endregion

            MainPlotModel.Axes.Add(MainXAxisLin);
            MainPlotModel.Axes.Add(MainYAxisLin);

            this.AnalysisPlot.Model = MainPlotModel;
            this.AnalysisPlot.Model.ResetAllAxes();
            this.AnalysisPlot.Model.InvalidatePlot(true);
        }

        private void SetSecundaryPlot()
        {
            SecundaryPlotModel.LegendBorder = OxyPlot.OxyColors.Black;
            SecundaryPlotModel.LegendItemAlignment = OxyPlot.HorizontalAlignment.Left;
            SecundaryPlotModel.LegendTitle = "Diffraction pattern";

            SecundaryXAxisLin.Position = OxyPlot.Axes.AxisPosition.Bottom;
            SecundaryXAxisLin.Minimum = 0;
            SecundaryXAxisLin.Maximum = 180;
            SecundaryXAxisLin.Title = "Probability";

            SecundaryYAxisLin.Position = OxyPlot.Axes.AxisPosition.Left;
            SecundaryYAxisLin.Minimum = 0;
            SecundaryYAxisLin.Maximum = 100;
            SecundaryYAxisLin.Title = "Value";

            #region GridStyles

            SecundaryXAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dot;
            SecundaryYAxisLin.MajorGridlineStyle = OxyPlot.LineStyle.Dot;

            SecundaryXAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dot;
            SecundaryYAxisLin.MinorGridlineStyle = OxyPlot.LineStyle.Dot;

            #endregion

            SecundaryPlotModel.Axes.Add(SecundaryXAxisLin);
            SecundaryPlotModel.Axes.Add(SecundaryYAxisLin);

            this.SecundaryPlot.Model = SecundaryPlotModel;
            this.SecundaryPlot.Model.ResetAllAxes();
            this.SecundaryPlot.Model.InvalidatePlot(true);
        }

        private void RefreshSources()
        {
            NameChangeactive = false;
            string selectedHeader = "";

            if (SimulationComboBox.SelectedIndex != -1)
            {
                ComboBoxItem selectedItem = SimulationComboBox.SelectedItem as ComboBoxItem;
                selectedHeader = Convert.ToString(selectedItem.Content);
            }
            int NewIndex = 0;

            SimulationComboBox.Items.Clear();

            for (int n = 0; n < RandomAnalisisData.Count; n++)
            {
                if (SimulationComboBox.SelectedIndex != -1)
                {
                    if (RandomAnalisisData[n].Name == selectedHeader)
                    {
                        NewIndex = n;
                    }
                }

                ComboBoxItem randomDataNameItem = new ComboBoxItem();
                randomDataNameItem.Content = RandomAnalisisData[n].Name;
                SimulationComboBox.Items.Add(randomDataNameItem);
            }

            ParameterGrid.Items.Refresh();
            NameChangeactive = true;
            if (SimulationComboBox.Items.Count != 0)
                SimulationComboBox.SelectedIndex = NewIndex;

            
        }

        #region Plotting

        private Pattern.Counts GetSpecificMainPlotData(int ModelIndex, int ViewIndex, int ParameterIndex)
        {
            Pattern.Counts MainPlotData = new Pattern.Counts();

            if (ViewIndex == 0)
            {
                switch (ParameterIndex)
                {
                    case 0:
                        MainPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetTotalValuesForC11(ModelIndex);
                        break;
                    case 1:
                        MainPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetTotalValuesForC22(ModelIndex);
                        break;
                    case 2:
                        MainPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetTotalValuesForC33(ModelIndex);
                        break;
                    case 3:
                        MainPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetTotalValuesForC44(ModelIndex);
                        break;
                    case 4:
                        MainPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetTotalValuesForC55(ModelIndex);
                        break;
                    case 5:
                        MainPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetTotalValuesForC66(ModelIndex);
                        break;
                    case 6:
                        MainPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetTotalValuesForC12(ModelIndex);
                        break;
                    case 7:
                        MainPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetTotalValuesForC13(ModelIndex);
                        break;
                    case 8:
                        MainPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetTotalValuesForC23(ModelIndex);
                        break;
                    case 9:
                        MainPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetTotalValuesForC45(ModelIndex);
                        break;
                    case 10:
                        MainPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetTotalValuesForC16(ModelIndex);
                        break;
                    case 11:
                        MainPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetTotalValuesForC26(ModelIndex);
                        break;
                    case 12:
                        MainPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetTotalValuesForC36(ModelIndex);
                        break;
                }
            }
            else
            {
                switch (ParameterIndex)
                {
                    case 0:
                        MainPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetTotalValuesForS11(ModelIndex);
                        break;
                    case 1:
                        MainPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetTotalValuesForS22(ModelIndex);
                        break;
                    case 2:
                        MainPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetTotalValuesForS33(ModelIndex);
                        break;
                    case 3:
                        MainPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetTotalValuesForS44(ModelIndex);
                        break;
                    case 4:
                        MainPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetTotalValuesForS55(ModelIndex);
                        break;
                    case 5:
                        MainPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetTotalValuesForS66(ModelIndex);
                        break;
                    case 6:
                        MainPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetTotalValuesForS12(ModelIndex);
                        break;
                    case 7:
                        MainPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetTotalValuesForS13(ModelIndex);
                        break;
                    case 8:
                        MainPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetTotalValuesForS23(ModelIndex);
                        break;
                    case 9:
                        MainPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetTotalValuesForS45(ModelIndex);
                        break;
                    case 10:
                        MainPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetTotalValuesForS16(ModelIndex);
                        break;
                    case 11:
                        MainPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetTotalValuesForS26(ModelIndex);
                        break;
                    case 12:
                        MainPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetTotalValuesForS36(ModelIndex);
                        break;
                }
            }

            return MainPlotData;
        }

        private Pattern.Counts GetSpecificSecundaryPlotData(int ModelIndex, int ViewIndex, int ParameterIndex)
        {
            Pattern.Counts SecundaryPlotData = new Pattern.Counts();

            if (ViewIndex == 0)
            {
                switch (ParameterIndex)
                {
                    case 0:
                        SecundaryPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetProbabilityDistributionForC11(ModelIndex, CalScec.Properties.Settings.Default.RandomProbabilityIntervall);
                        break;
                    case 1:
                        SecundaryPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetProbabilityDistributionForC22(ModelIndex, CalScec.Properties.Settings.Default.RandomProbabilityIntervall);
                        break;
                    case 2:
                        SecundaryPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetProbabilityDistributionForC33(ModelIndex, CalScec.Properties.Settings.Default.RandomProbabilityIntervall);
                        break;
                    case 3:
                        SecundaryPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetProbabilityDistributionForC44(ModelIndex, CalScec.Properties.Settings.Default.RandomProbabilityIntervall);
                        break;
                    case 4:
                        SecundaryPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetProbabilityDistributionForC55(ModelIndex, CalScec.Properties.Settings.Default.RandomProbabilityIntervall);
                        break;
                    case 5:
                        SecundaryPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetProbabilityDistributionForC66(ModelIndex, CalScec.Properties.Settings.Default.RandomProbabilityIntervall);
                        break;
                    case 6:
                        SecundaryPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetProbabilityDistributionForC12(ModelIndex, CalScec.Properties.Settings.Default.RandomProbabilityIntervall);
                        break;
                    case 7:
                        SecundaryPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetProbabilityDistributionForC13(ModelIndex, CalScec.Properties.Settings.Default.RandomProbabilityIntervall);
                        break;
                    case 8:
                        SecundaryPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetProbabilityDistributionForC23(ModelIndex, CalScec.Properties.Settings.Default.RandomProbabilityIntervall);
                        break;
                    case 9:
                        SecundaryPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetProbabilityDistributionForC45(ModelIndex, CalScec.Properties.Settings.Default.RandomProbabilityIntervall);
                        break;
                    case 10:
                        SecundaryPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetProbabilityDistributionForC16(ModelIndex, CalScec.Properties.Settings.Default.RandomProbabilityIntervall);
                        break;
                    case 11:
                        SecundaryPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetProbabilityDistributionForC26(ModelIndex, CalScec.Properties.Settings.Default.RandomProbabilityIntervall);
                        break;
                    case 12:
                        SecundaryPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetProbabilityDistributionForC36(ModelIndex, CalScec.Properties.Settings.Default.RandomProbabilityIntervall);
                        break;
                }
            }
            else
            {
                switch (ParameterIndex)
                {
                    case 0:
                        SecundaryPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetProbabilityDistributionForS11(ModelIndex, CalScec.Properties.Settings.Default.RandomProbabilityIntervall);
                        break;
                    case 1:
                        SecundaryPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetProbabilityDistributionForS22(ModelIndex, CalScec.Properties.Settings.Default.RandomProbabilityIntervall);
                        break;
                    case 2:
                        SecundaryPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetProbabilityDistributionForS33(ModelIndex, CalScec.Properties.Settings.Default.RandomProbabilityIntervall);
                        break;
                    case 3:
                        SecundaryPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetProbabilityDistributionForS44(ModelIndex, CalScec.Properties.Settings.Default.RandomProbabilityIntervall);
                        break;
                    case 4:
                        SecundaryPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetProbabilityDistributionForS55(ModelIndex, CalScec.Properties.Settings.Default.RandomProbabilityIntervall);
                        break;
                    case 5:
                        SecundaryPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetProbabilityDistributionForS66(ModelIndex, CalScec.Properties.Settings.Default.RandomProbabilityIntervall);
                        break;
                    case 6:
                        SecundaryPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetProbabilityDistributionForS12(ModelIndex, CalScec.Properties.Settings.Default.RandomProbabilityIntervall);
                        break;
                    case 7:
                        SecundaryPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetProbabilityDistributionForS13(ModelIndex, CalScec.Properties.Settings.Default.RandomProbabilityIntervall);
                        break;
                    case 8:
                        SecundaryPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetProbabilityDistributionForS23(ModelIndex, CalScec.Properties.Settings.Default.RandomProbabilityIntervall);
                        break;
                    case 9:
                        SecundaryPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetProbabilityDistributionForS45(ModelIndex, CalScec.Properties.Settings.Default.RandomProbabilityIntervall);
                        break;
                    case 10:
                        SecundaryPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetProbabilityDistributionForS16(ModelIndex, CalScec.Properties.Settings.Default.RandomProbabilityIntervall);
                        break;
                    case 11:
                        SecundaryPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetProbabilityDistributionForS26(ModelIndex, CalScec.Properties.Settings.Default.RandomProbabilityIntervall);
                        break;
                    case 12:
                        SecundaryPlotData = RandomAnalisisData[this.SelectedAnaylsisIndex].GetProbabilityDistributionForS36(ModelIndex, CalScec.Properties.Settings.Default.RandomProbabilityIntervall);
                        break;
                }
            }

            return SecundaryPlotData;
        }

        private void SetDataMainPlot(Pattern.Counts PC, string legendName)
        {
            MainPlotModel.Series.Clear();

            OxyPlot.Series.LineSeries Tmp = new OxyPlot.Series.LineSeries();
            Tmp.Title = legendName;

            Tmp.LineStyle = OxyPlot.LineStyle.None;
            Tmp.StrokeThickness = 1;
            Tmp.MarkerSize = 3;

            Tmp.MarkerType = OxyPlot.MarkerType.Circle;
            OxyPlot.OxyColor LineColor = OxyPlot.OxyColors.Black;
            Tmp.Color = LineColor;
            Tmp.MarkerStroke = LineColor;


            foreach (double[] DPP in PC)
            {
                OxyPlot.DataPoint PlotPoint = new OxyPlot.DataPoint(DPP[0], DPP[1]);
                Tmp.Points.Add(PlotPoint);

                if (this.MainYAxisLin.Maximum < DPP[1])
                {
                    this.MainYAxisLin.Maximum = DPP[1];
                }
                if (this.MainYAxisLin.Minimum > DPP[1])
                {
                    this.MainYAxisLin.Minimum = DPP[1];
                }
                if (this.MainXAxisLin.Maximum < DPP[0])
                {
                    this.MainXAxisLin.Maximum = DPP[0];
                }
                if (this.MainXAxisLin.Minimum > DPP[0])
                {
                    this.MainXAxisLin.Minimum = DPP[0];
                }
            }

            MainPlotModel.Series.Add(Tmp);
            AnalysisPlot.Model = MainPlotModel;
            AnalysisPlot.Model.ResetAllAxes();
            AnalysisPlot.InvalidatePlot(true);
        }

        private void SetDataSecundaryPlot(Pattern.Counts PC, string legendName)
        {
            SecundaryPlotModel.Series.Clear();

            OxyPlot.Series.LineSeries Tmp = new OxyPlot.Series.LineSeries();
            Tmp.Title = legendName;

            Tmp.LineStyle = OxyPlot.LineStyle.None;
            Tmp.StrokeThickness = 1;
            Tmp.MarkerSize = 3;

            Tmp.MarkerType = OxyPlot.MarkerType.Circle;
            OxyPlot.OxyColor LineColor = OxyPlot.OxyColors.Black;
            Tmp.Color = LineColor;
            Tmp.MarkerStroke = LineColor;


            foreach (double[] DPP in PC)
            {
                OxyPlot.DataPoint PlotPoint = new OxyPlot.DataPoint(DPP[0], DPP[1]);
                Tmp.Points.Add(PlotPoint);

                if (this.SecundaryYAxisLin.Maximum < DPP[1])
                {
                    this.SecundaryYAxisLin.Maximum = DPP[1];
                }
                if (this.SecundaryYAxisLin.Minimum > DPP[1])
                {
                    this.SecundaryYAxisLin.Minimum = DPP[1];
                }
                if (this.SecundaryXAxisLin.Maximum < DPP[0])
                {
                    this.SecundaryXAxisLin.Maximum = DPP[0];
                }
                if (this.SecundaryXAxisLin.Minimum > DPP[0])
                {
                    this.SecundaryXAxisLin.Minimum = DPP[0];
                }
            }

            SecundaryPlotModel.Series.Add(Tmp);
            SecundaryPlot.Model = SecundaryPlotModel;
            SecundaryPlot.Model.ResetAllAxes();
            SecundaryPlot.InvalidatePlot(true);
        }

        private void SetEstimatedNumberOfSimulations()
        {
            if (this.SelectedAnaylsisIndex != -1)
            {
                bool EqualDistributionS = true;
                int ModeIndex = SimulationModeComboBox.SelectedIndex;

                if (ModeIndex == 1)
                {
                    EqualDistributionS = false;
                }

                NumberSimulationsLabel.Content = RandomAnalisisData[this.SelectedAnaylsisIndex].EstimatedSimulationNumber(EqualDistributionS);
            }
        }

        private void SetConstantCheckBoxes()
        {
            for(int n = 0; n < RandomAnalisisData[SelectedAnaylsisIndex].UsedConstants.Count(); n++)
            {
                switch (n)
                {
                    case 0:
                        if(RandomAnalisisData[SelectedAnaylsisIndex].UsedConstants[n])
                        {
                            this.C11CheckBox.IsChecked = true;
                        }
                        else
                        {
                            this.C11CheckBox.IsChecked = false;
                        }
                        break;
                    case 1:
                        if (RandomAnalisisData[SelectedAnaylsisIndex].UsedConstants[n])
                        {
                            this.C22CheckBox.IsChecked = true;
                        }
                        else
                        {
                            this.C22CheckBox.IsChecked = false;
                        }
                        break;
                    case 2:
                        if (RandomAnalisisData[SelectedAnaylsisIndex].UsedConstants[n])
                        {
                            this.C33CheckBox.IsChecked = true;
                        }
                        else
                        {
                            this.C33CheckBox.IsChecked = false;
                        }
                        break;
                    case 3:
                        if (RandomAnalisisData[SelectedAnaylsisIndex].UsedConstants[n])
                        {
                            this.C44CheckBox.IsChecked = true;
                        }
                        else
                        {
                            this.C44CheckBox.IsChecked = false;
                        }
                        break;
                    case 4:
                        if (RandomAnalisisData[SelectedAnaylsisIndex].UsedConstants[n])
                        {
                            this.C55CheckBox.IsChecked = true;
                        }
                        else
                        {
                            this.C55CheckBox.IsChecked = false;
                        }
                        break;
                    case 5:
                        if (RandomAnalisisData[SelectedAnaylsisIndex].UsedConstants[n])
                        {
                            this.C66CheckBox.IsChecked = true;
                        }
                        else
                        {
                            this.C66CheckBox.IsChecked = false;
                        }
                        break;
                    case 6:
                        if (RandomAnalisisData[SelectedAnaylsisIndex].UsedConstants[n])
                        {
                            this.C12CheckBox.IsChecked = true;
                        }
                        else
                        {
                            this.C12CheckBox.IsChecked = false;
                        }
                        break;
                    case 7:
                        if (RandomAnalisisData[SelectedAnaylsisIndex].UsedConstants[n])
                        {
                            this.C13CheckBox.IsChecked = true;
                        }
                        else
                        {
                            this.C13CheckBox.IsChecked = false;
                        }
                        break;
                    case 8:
                        if (RandomAnalisisData[SelectedAnaylsisIndex].UsedConstants[n])
                        {
                            this.C23CheckBox.IsChecked = true;
                        }
                        else
                        {
                            this.C23CheckBox.IsChecked = false;
                        }
                        break;
                    case 9:
                        if (RandomAnalisisData[SelectedAnaylsisIndex].UsedConstants[n])
                        {
                            this.C45CheckBox.IsChecked = true;
                        }
                        else
                        {
                            this.C45CheckBox.IsChecked = false;
                        }
                        break;
                    case 10:
                        if (RandomAnalisisData[SelectedAnaylsisIndex].UsedConstants[n])
                        {
                            this.C16CheckBox.IsChecked = true;
                        }
                        else
                        {
                            this.C16CheckBox.IsChecked = false;
                        }
                        break;
                    case 11:
                        if (RandomAnalisisData[SelectedAnaylsisIndex].UsedConstants[n])
                        {
                            this.C26CheckBox.IsChecked = true;
                        }
                        else
                        {
                            this.C26CheckBox.IsChecked = false;
                        }
                        break;
                    case 12:
                        if (RandomAnalisisData[SelectedAnaylsisIndex].UsedConstants[n])
                        {
                            this.C36CheckBox.IsChecked = true;
                        }
                        else
                        {
                            this.C36CheckBox.IsChecked = false;
                        }
                        break;
                }
            }
        }

        #endregion

        #region SelectionChangedEvents

        private void SimulationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NameChangeactive)
            {
                NameChangeactive = false;

                ComboBoxItem selectedItem = SimulationComboBox.SelectedItem as ComboBoxItem;
                string selectedHeader = Convert.ToString(selectedItem.Content);

                for (int n = 0; n < RandomAnalisisData.Count; n++)
                {
                    if (RandomAnalisisData[n].Name == selectedHeader)
                    {
                        this.SelectedAnaylsisIndex = n;
                    }
                }

                SimulationHeaderTextBox.Text = RandomAnalisisData[this.SelectedAnaylsisIndex].Name;

                this.UsedREKsList.ItemsSource = RandomAnalisisData[this.SelectedAnaylsisIndex].InvestigatedTensor.DiffractionConstants;

                int ModelIndex = SimulationModelComboBox.SelectedIndex;
                int ViewIndex = InitialValueCalculationComboBox.SelectedIndex;
                int ModeIndex = SimulationModeComboBox.SelectedIndex;

                ComboBoxItem selectedParameterItem = MainParameterPlot.SelectedItem as ComboBoxItem;
                int ParameterIndex = RandomAnalysis.GetConstantNumber(Convert.ToString(selectedParameterItem.Content));

                Pattern.Counts MainPlotData = GetSpecificMainPlotData(ModelIndex, ViewIndex, ParameterIndex);
                Pattern.Counts SecundaryPlotData = GetSpecificSecundaryPlotData(ModelIndex, ViewIndex, ParameterIndex);

                SetDataMainPlot(MainPlotData, Convert.ToString(selectedParameterItem.Content));
                SetDataSecundaryPlot(SecundaryPlotData, Convert.ToString(selectedParameterItem.Content));

                SetEstimatedNumberOfSimulations();
                this.UsedREKsList.ItemsSource = RandomAnalisisData[this.SelectedAnaylsisIndex].InvestigatedTensor.DiffractionConstants;
                this.ResultGrid.ItemsSource = RandomAnalisisData[this.SelectedAnaylsisIndex].TensorResults;

                NameChangeactive = true;
            }
        }

        private void SimulationModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetEstimatedNumberOfSimulations();
        }

        #endregion

        #region ClickEvents

        private void CalculateSimulation_Click(object sender, RoutedEventArgs e)
        {
            int ModelIndex = SimulationModelComboBox.SelectedIndex;
            int ViewIndex = InitialValueCalculationComboBox.SelectedIndex;

            bool StiffnesCalc = true;

            if(ViewIndex == 1)
            {
                StiffnesCalc = false;
            }

            RandomAnalisisData[this.SelectedAnaylsisIndex].SimulationStarted += this.SimulationStarted;
            RandomAnalisisData[this.SelectedAnaylsisIndex].SimulationUpdated += this.SimulationUpdated;
            RandomAnalisisData[this.SelectedAnaylsisIndex].SimulationFinished += this.SimulationFinished;
            RandomAnalisisData[this.SelectedAnaylsisIndex]._multiThreadingStiffness = StiffnesCalc;
            RandomAnalisisData[this.SelectedAnaylsisIndex]._multiThreadingModel = ModelIndex;
            RandomAnalisisData[this.SelectedAnaylsisIndex]._multiThreadingEqual = true;
            //RandomAnalisisData[this.SelectedAnaylsisIndex].CalculateResultsEqualDestribution(ModelIndex, StiffnesCalc);
            System.Threading.ThreadPool.QueueUserWorkItem(RandomAnalisisData[this.SelectedAnaylsisIndex].SimulationCallback);

        }

        private void CreateNewSimulation_Click(object sender, RoutedEventArgs e)
        {
            AddNewAnalysisData();
            RefreshSources();
        }

        #region Data management

        #region SCECS

        private void SaveSimulationToSCECS_Click(object sender, RoutedEventArgs e)
        {
            if (this.SelectedAnaylsisIndex != -1)
            {
                DataManagment.Files.SCECS.RandomAnalysisInformation ForSave = new DataManagment.Files.SCECS.RandomAnalysisInformation(RandomAnalisisData[this.SelectedAnaylsisIndex]);

                Microsoft.Win32.SaveFileDialog OpenSampleFile = new Microsoft.Win32.SaveFileDialog();
                OpenSampleFile.FileName = ForSave._name;
                OpenSampleFile.DefaultExt = ".scecs";
                OpenSampleFile.Filter = "simulation data (.scecs)|*.scecs";

                Nullable<bool> Opened = OpenSampleFile.ShowDialog();

                if (Opened == true)
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
        }

        private void LoadSimulationToSCECS_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog OpenSampleFile = new Microsoft.Win32.OpenFileDialog();
            OpenSampleFile.Multiselect = false;
            OpenSampleFile.DefaultExt = ".scecs";
            OpenSampleFile.Filter = "simulation data (.scecs)|*.scecs";

            Nullable<bool> Opened = OpenSampleFile.ShowDialog();

            if (Opened == true)
            {
                string filename = OpenSampleFile.FileName;

                using (System.IO.Stream fileStream = System.IO.File.OpenRead(filename))
                {
                    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                    object DataObj = bf.Deserialize(fileStream);

                    DataManagment.Files.SCECS.RandomAnalysisInformation Loaded = DataObj as DataManagment.Files.SCECS.RandomAnalysisInformation;

                    this.RandomAnalisisData.Add(Loaded.GetRandomAnalysis());

                    RefreshSources();
                }
            }
        }

        #endregion

        #endregion

        #endregion

        private void SimulationHeaderTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                RandomAnalisisData[this.SelectedAnaylsisIndex].Name = SimulationHeaderTextBox.Text;
                RefreshSources();
            }
        }

        private void ParameterGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                RefreshSources();
            }
        }

        private void ConstantCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox SenderCheckBox = sender as CheckBox;
            switch(SenderCheckBox.Name)
            {
                case "C11CheckBox":
                    RandomAnalisisData[SelectedAnaylsisIndex].UsedConstants[0] = true;
                    break;
                case "C22CheckBox":
                    RandomAnalisisData[SelectedAnaylsisIndex].UsedConstants[1] = true;
                    break;
                case "C33CheckBox":
                    RandomAnalisisData[SelectedAnaylsisIndex].UsedConstants[2] = true;
                    break;
                case "C44CheckBox":
                    RandomAnalisisData[SelectedAnaylsisIndex].UsedConstants[3] = true;
                    break;
                case "C55CheckBox":
                    RandomAnalisisData[SelectedAnaylsisIndex].UsedConstants[4] = true;
                    break;
                case "C66CheckBox":
                    RandomAnalisisData[SelectedAnaylsisIndex].UsedConstants[5] = true;
                    break;
                case "C12CheckBox":
                    RandomAnalisisData[SelectedAnaylsisIndex].UsedConstants[6] = true;
                    break;
                case "C13CheckBox":
                    RandomAnalisisData[SelectedAnaylsisIndex].UsedConstants[7] = true;
                    break;
                case "C23CheckBox":
                    RandomAnalisisData[SelectedAnaylsisIndex].UsedConstants[8] = true;
                    break;
                case "C45CheckBox":
                    RandomAnalisisData[SelectedAnaylsisIndex].UsedConstants[9] = true;
                    break;
                case "C16CheckBox":
                    RandomAnalisisData[SelectedAnaylsisIndex].UsedConstants[10] = true;
                    break;
                case "C26CheckBox":
                    RandomAnalisisData[SelectedAnaylsisIndex].UsedConstants[11] = true;
                    break;
                case "C36CheckBox":
                    RandomAnalisisData[SelectedAnaylsisIndex].UsedConstants[12] = true;
                    break;
            }
            SetEstimatedNumberOfSimulations();
        }

        private void ConstantCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox SenderCheckBox = sender as CheckBox;
            switch (SenderCheckBox.Name)
            {
                case "C11CheckBox":
                    RandomAnalisisData[SelectedAnaylsisIndex].UsedConstants[0] = false;
                    break;
                case "C22CheckBox":
                    RandomAnalisisData[SelectedAnaylsisIndex].UsedConstants[1] = false;
                    break;
                case "C33CheckBox":
                    RandomAnalisisData[SelectedAnaylsisIndex].UsedConstants[2] = false;
                    break;
                case "C44CheckBox":
                    RandomAnalisisData[SelectedAnaylsisIndex].UsedConstants[3] = false;
                    break;
                case "C55CheckBox":
                    RandomAnalisisData[SelectedAnaylsisIndex].UsedConstants[4] = false;
                    break;
                case "C66CheckBox":
                    RandomAnalisisData[SelectedAnaylsisIndex].UsedConstants[5] = false;
                    break;
                case "C12CheckBox":
                    RandomAnalisisData[SelectedAnaylsisIndex].UsedConstants[6] = false;
                    break;
                case "C13CheckBox":
                    RandomAnalisisData[SelectedAnaylsisIndex].UsedConstants[7] = false;
                    break;
                case "C23CheckBox":
                    RandomAnalisisData[SelectedAnaylsisIndex].UsedConstants[8] = false;
                    break;
                case "C45CheckBox":
                    RandomAnalisisData[SelectedAnaylsisIndex].UsedConstants[9] = false;
                    break;
                case "C16CheckBox":
                    RandomAnalisisData[SelectedAnaylsisIndex].UsedConstants[10] = false;
                    break;
                case "C26CheckBox":
                    RandomAnalisisData[SelectedAnaylsisIndex].UsedConstants[11] = false;
                    break;
                case "C36CheckBox":
                    RandomAnalisisData[SelectedAnaylsisIndex].UsedConstants[12] = false;
                    break;
            }
            SetEstimatedNumberOfSimulations();
        }

        private void RefreshView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshSources();
        }

        #region Calculation Multi threading

        public delegate void SimulationUpdateDelegate(RandomAnalysis RA);

        private void SimulationStarted(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SimulationUpdateDelegate SimulationDelegate = SimulationStartedHandler;

            Dispatcher.Invoke(SimulationDelegate, sender as RandomAnalysis);
        }

        private void SimulationUpdated(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SimulationUpdateDelegate SimulationDelegate = SimulationUpdatedHandler;

            Dispatcher.Invoke(SimulationDelegate, sender as RandomAnalysis);
        }

        private void SimulationFinished(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SimulationUpdateDelegate SimulationDelegate = SimulationFinishedHandler;

            Dispatcher.Invoke(SimulationDelegate, sender as RandomAnalysis);
        }

        private void SimulationStartedHandler(RandomAnalysis RA)
        {
            StringBuilder NewLog = new StringBuilder("Simulation started!");

            this.StatusLog1.Content = NewLog.ToString();
        }

        private void SimulationUpdatedHandler(RandomAnalysis RA)
        {
            StringBuilder NewLog = new StringBuilder("Simulated ");
            NewLog.Append(RA._ActualThreadingSimulation);
            NewLog.Append(" of ");
            NewLog.Append(RA._TotalThreadingSimulation);
            this.StatusLog2.Content = NewLog.ToString();

            StatusProgress.Value = (Convert.ToDouble(RA._ActualThreadingSimulation) / Convert.ToDouble(RA._TotalThreadingSimulation)) * 100.0;
        }

        private void SimulationFinishedHandler(RandomAnalysis RA)
        {
            StringBuilder NewLog = new StringBuilder("Simulation completed!");

            this.StatusLog1.Content = NewLog.ToString();
            RefreshSources();
        }

        #endregion
    }
}
