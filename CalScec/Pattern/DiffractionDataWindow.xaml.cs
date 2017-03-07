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
                this.PatternPsiAngle.Text = SelectedPattern.PsiAngle.ToString("F3");
                this.PatternPhiAngle.Text = SelectedPattern.PhiAngle.ToString("F3");
                this.PatternAppliedStress.Text = SelectedPattern.Stress.ToString("F3");
                this.PatternAppliedForce.Text = SelectedPattern.Force.ToString("F3");
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

        private void PatternPsiAngle_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.DiffractionPatternList.SelectedIndex != -1)
            {
                Pattern.DiffractionPattern SelectedPattern = (Pattern.DiffractionPattern)this.DiffractionPatternList.SelectedItem;

                try
                {
                    double NewValue = Convert.ToDouble(this.PatternPsiAngle.Text);

                    SelectedPattern.PsiAngle = NewValue;
                    this.PatternPsiAngle.Foreground = Brushes.DarkGreen;
                }
                catch
                {
                    this.PatternPsiAngle.Foreground = Brushes.DarkRed;
                }
            }
        }

        private void PatternPhiAngle_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.DiffractionPatternList.SelectedIndex != -1)
            {
                Pattern.DiffractionPattern SelectedPattern = (Pattern.DiffractionPattern)this.DiffractionPatternList.SelectedItem;

                try
                {
                    double NewValue = Convert.ToDouble(this.PatternPhiAngle.Text);

                    SelectedPattern.PhiAngle = NewValue;
                    this.PatternPhiAngle.Foreground = Brushes.DarkGreen;
                }
                catch
                {
                    this.PatternPhiAngle.Foreground = Brushes.DarkRed;
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
    }
}
