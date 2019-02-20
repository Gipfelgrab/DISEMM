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

namespace CalScec.DataManagment.CrystalData
{
    /// <summary>
    /// Interaktionslogik für CompositionWindow.xaml
    /// </summary>
    public partial class CompositionWindow : Window
    {
        public Analysis.Sample ActSample;

        private bool eventsActive = true;

        public CompositionWindow(Analysis.Sample actSample)
        {
            InitializeComponent();

            this.ActSample = actSample;
            this.PrepareView();
        }

        private void PrepareView()
        {
            this.CrystalDataList.ItemsSource = this.ActSample.CrystalData;
        }

        private void CrystalDataList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.eventsActive = false;
            if(this.CrystalDataList.SelectedIndex != -1)
            {
                CODData crystalData = (CODData)CrystalDataList.SelectedItem;

                if(crystalData.Matrix)
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

                if (this.InclusionTypeSelection.Items.Count > crystalData.InclusionType)
                {
                    this.InclusionTypeSelection.SelectedIndex = crystalData.InclusionType;
                }

                this.PhaseFraction.Text = crystalData.PhaseFraction.ToString();

                if (!crystalData.Matrix)
                {
                    if(crystalData.InclusionType == 0)
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
            this.eventsActive = true;
        }

        private void PhaseTypeSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.CrystalDataList.SelectedIndex != -1)
            {
                CODData crystalData = (CODData)CrystalDataList.SelectedItem;

                if (this.eventsActive)
                {
                    if (PhaseTypeSelection.SelectedIndex == 0)
                    {
                        crystalData.Matrix = true;
                    }
                    else
                    {
                        crystalData.Matrix = false;
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

                    if (crystalData.InclusionType == 0)
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
            if (this.CrystalDataList.SelectedIndex != -1)
            {
                CODData crystalData = (CODData)CrystalDataList.SelectedItem;

                if (this.eventsActive)
                {
                    crystalData.InclusionType = InclusionTypeSelection.SelectedIndex;
                }

                if (!crystalData.Matrix)
                {
                    if (crystalData.InclusionType == 0)
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
            if (this.CrystalDataList.SelectedIndex != -1 && eventsActive)
            {
                CODData crystalData = (CODData)CrystalDataList.SelectedItem;

                try
                {
                    crystalData.PhaseFraction = Convert.ToDouble(PhaseFraction.Text);
                }
                catch
                {

                }
            }
        }

        private void PhaseParameter1Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.CrystalDataList.SelectedIndex != -1 && eventsActive)
            {
                CODData crystalData = (CODData)CrystalDataList.SelectedItem;

                try
                {
                    crystalData.InclusionA = Convert.ToDouble(PhaseParameter1Text.Text);
                }
                catch
                {

                }
            }
        }

        private void PhaseParameter2Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.CrystalDataList.SelectedIndex != -1 && eventsActive)
            {
                CODData crystalData = (CODData)CrystalDataList.SelectedItem;

                try
                {
                    crystalData.InclusionB = Convert.ToDouble(PhaseParameter2Text.Text);
                }
                catch
                {

                }
            }
        }

        private void PhaseParameter3Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.CrystalDataList.SelectedIndex != -1 && eventsActive)
            {
                CODData crystalData = (CODData)CrystalDataList.SelectedItem;

                try
                {
                    crystalData.InclusionC = Convert.ToDouble(PhaseParameter3Text.Text);
                }
                catch
                {

                }
            }
        }
    }
}
