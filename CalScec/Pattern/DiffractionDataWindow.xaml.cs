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

namespace CalScec.Pattern
{
    /// <summary>
    /// Interaktionslogik für DiffractionDataWindow.xaml
    /// </summary>
    public partial class DiffractionDataWindow : Window
    {
        Analysis.Sample _sample;

        bool EditAktive = true;

        public DiffractionDataWindow(Analysis.Sample ActSample)
        {
            InitializeComponent();

            this._sample = ActSample;

            FillData();
        }

        private void FillData()
        {
            this.SampleName.Text = this._sample.Name;
            this.SampleArea.Text = this._sample.Area.ToString("F3");

            this.DiffractionPatternList.ItemsSource = this._sample.DiffractionPatterns;
        }

        private void DiffractionPatternList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EditAktive = false;
            if (this.DiffractionPatternList.SelectedIndex != -1)
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
            EditAktive = true;
        }

        private void SampleName_TextChanged(object sender, TextChangedEventArgs e)
        {
            this._sample.Name = SampleName.Text;
        }

        private void SampleArea_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                double NewArea = Convert.ToDouble(this.SampleArea.Text);

                this._sample.Area = NewArea;
                this.SampleArea.Foreground = Brushes.DarkGreen;
            }
            catch
            {
                this.SampleArea.Foreground = Brushes.DarkRed;
            }
        }

        private void PatternName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.DiffractionPatternList.SelectedIndex != -1)
            {
                Pattern.DiffractionPattern SelectedPattern = (Pattern.DiffractionPattern)this.DiffractionPatternList.SelectedItem;

                SelectedPattern.Name = this.PatternName.Text;
            }
        }

        private void PatternChiAngle_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.DiffractionPatternList.SelectedIndex != -1)
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
            if (this.DiffractionPatternList.SelectedIndex != -1)
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
            if (this.DiffractionPatternList.SelectedIndex != -1)
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
            if (EditAktive)
            {
                if (this.DiffractionPatternList.SelectedIndex != -1)
                {
                    Pattern.DiffractionPattern SelectedPattern = (Pattern.DiffractionPattern)this.DiffractionPatternList.SelectedItem;

                    try
                    {
                        double NewValue = Convert.ToDouble(this.PatternAppliedForce.Text);

                        SelectedPattern.Force = NewValue;
                        SelectedPattern.Stress = SelectedPattern.Force / this._sample.Area;
                        EditAktive = false;
                        this.PatternAppliedStress.Text = SelectedPattern.Stress.ToString("F3");
                        EditAktive = true;
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
            if (EditAktive)
            {
                if (this.DiffractionPatternList.SelectedIndex != -1)
                {
                    Pattern.DiffractionPattern SelectedPattern = (Pattern.DiffractionPattern)this.DiffractionPatternList.SelectedItem;

                    try
                    {
                        double NewValue = Convert.ToDouble(this.PatternAppliedStress.Text);

                        SelectedPattern.Stress = NewValue;
                        SelectedPattern.Force = SelectedPattern.Stress * this._sample.Area;
                        EditAktive = false;
                        this.PatternAppliedForce.Text = SelectedPattern.Force.ToString("F3");
                        EditAktive = true;
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
            if (EditAktive)
            {
                if (this.DiffractionPatternList.SelectedIndex != -1)
                {
                    Pattern.DiffractionPattern SelectedPattern = (Pattern.DiffractionPattern)this.DiffractionPatternList.SelectedItem;

                    try
                    {
                        double NewValue = Convert.ToDouble(this.PatternMacroStrain.Text);

                        SelectedPattern.MacroStrain = NewValue;
                        EditAktive = false;
                        this.PatternMacroStrain.Text = SelectedPattern.MacroStrain.ToString();
                        EditAktive = true;
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

        private void Autofill_Click(object sender, RoutedEventArgs e)
        {
            for(int n = 0; n < this._sample.DiffractionPatterns.Count; n++)
            {
                char[] SepChars = { '.', '-', '_' };

                string[] sepPatternName = this._sample.DiffractionPatterns[n].Name.Split(SepChars);

                int ActChiAngle = Convert.ToInt32(sepPatternName[2]);
                int patternIndex = Convert.ToInt32(sepPatternName[1]);

                this._sample.DiffractionPatterns[n].OmegaAngle = 0.0;
                this._sample.DiffractionPatterns[n].PhiSampleAngle = 0.0;
                this._sample.DiffractionPatterns[n].ChiAngle = (10.0 * ActChiAngle) + 95;
                //this._sample.DiffractionPatterns[n].Force = ((patternIndex - 20.0) * 45.0) + 200;

                //if(this._sample.Area != 0.0)
                //{
                //    this._sample.DiffractionPatterns[n].Stress = this._sample.DiffractionPatterns[n].Force / this._sample.Area;
                //}
            }
        }
    }
}
