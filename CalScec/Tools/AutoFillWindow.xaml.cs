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

namespace CalScec.Tools
{
    /// <summary>
    /// Interaktionslogik für AutoFillWindow.xaml
    /// </summary>
    public partial class AutoFillWindow : Window
    {
        List<Pattern.DiffractionPattern> modifiedPattern;
        public AutoFillWindow(double area, List<Pattern.DiffractionPattern> dP)
        {
            InitializeComponent();

            modifiedPattern = dP;
            this.PatternDataGrid.ItemsSource = modifiedPattern;

            this.PasteSelection.SelectedIndex = 0;

        }

        private void Autofill_Click(object sender, RoutedEventArgs e)
        {
            char[] seperators = { ' ', '_', '-', ';' };

            if(this.PasteSelection.SelectedIndex == 1)
            {
                try
                {
                    int nameIndex = Convert.ToInt32(Index1.Text);
                    int chiIndex = Convert.ToInt32(Index2.Text);
                    int omegaIndex = Convert.ToInt32(Index3.Text);
                    int phiIndex = Convert.ToInt32(Index4.Text);
                    int forceIndex = Convert.ToInt32(Index5.Text);
                    int stressIndex = Convert.ToInt32(Index6.Text);
                    int strainIndex = Convert.ToInt32(Index7.Text);

                    try
                    {
                        for(int n = 0; n < this.modifiedPattern.Count; n++)
                        {
                            string[] patternNameParts = modifiedPattern[n].Name.Split(seperators);

                            if (chiIndex > -1)
                            {
                                modifiedPattern[n].ChiAngle = Convert.ToDouble(patternNameParts[chiIndex]);
                            }
                            if (omegaIndex > -1)
                            {
                                modifiedPattern[n].OmegaAngle = Convert.ToDouble(patternNameParts[omegaIndex]);
                            }
                            if (phiIndex > -1)
                            {
                                modifiedPattern[n].PhiSampleAngle = Convert.ToDouble(patternNameParts[phiIndex]);
                            }
                            if (forceIndex > -1)
                            {
                                modifiedPattern[n].Force = Convert.ToDouble(patternNameParts[forceIndex]);
                            }
                            if (stressIndex > -1)
                            {
                                modifiedPattern[n].Stress = Convert.ToDouble(patternNameParts[stressIndex]);
                            }
                            if (strainIndex > -1)
                            {
                                modifiedPattern[n].MacroStrain = Convert.ToDouble(patternNameParts[strainIndex]);
                            }
                        }
                    }
                    catch
                    {
                        MessageBox.Show("An Error occured during the reading of the pattern names! Are they correctly formatted?", "Loading data failed!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch
                {
                    MessageBox.Show("The indices musst be of type int!", "Index Conversion Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if(this.PasteSelection.SelectedIndex == 2)
            {
                Microsoft.Win32.OpenFileDialog OpenDiffractionFile = new Microsoft.Win32.OpenFileDialog();
                OpenDiffractionFile.Multiselect = false;
                OpenDiffractionFile.DefaultExt = ".txt";
                Nullable<bool> Opened = OpenDiffractionFile.ShowDialog();

                if (Opened == true)
                {
                    string FilePath = OpenDiffractionFile.FileName;

                    string FileExt = System.IO.Path.GetExtension(FilePath);
                    if (FileExt.ToLower() == ".txt")
                    {
                        string[] PatternFileLines = System.IO.File.ReadLines(FilePath).ToArray();
                        try
                        {
                            int nameIndex = Convert.ToInt32(Index1.Text);
                            int chiIndex = Convert.ToInt32(Index2.Text);
                            int omegaIndex = Convert.ToInt32(Index3.Text);
                            int phiIndex = Convert.ToInt32(Index4.Text);
                            int forceIndex = Convert.ToInt32(Index5.Text);
                            int stressIndex = Convert.ToInt32(Index6.Text);
                            int strainIndex = Convert.ToInt32(Index7.Text);
                            
                            try
                            {
                                for (int n = 0; n < PatternFileLines.Count(); n++)
                                {
                                    string[] patternNameParts = PatternFileLines[n].Split(seperators);

                                    if (nameIndex > -1)
                                    {
                                        modifiedPattern[n].Name = patternNameParts[nameIndex];
                                    }
                                    if (chiIndex > -1)
                                    {
                                        modifiedPattern[n].ChiAngle = Convert.ToDouble(patternNameParts[chiIndex]);
                                    }
                                    if (omegaIndex > -1)
                                    {
                                        modifiedPattern[n].OmegaAngle = Convert.ToDouble(patternNameParts[omegaIndex]);
                                    }
                                    if (phiIndex > -1)
                                    {
                                        modifiedPattern[n].PhiSampleAngle = Convert.ToDouble(patternNameParts[phiIndex]);
                                    }
                                    if (forceIndex > -1)
                                    {
                                        modifiedPattern[n].Force = Convert.ToDouble(patternNameParts[forceIndex]);
                                    }
                                    if (stressIndex > -1)
                                    {
                                        modifiedPattern[n].Stress = Convert.ToDouble(patternNameParts[stressIndex]);
                                    }
                                    if (strainIndex > -1)
                                    {
                                        modifiedPattern[n].MacroStrain = Convert.ToDouble(patternNameParts[strainIndex]);
                                    }
                                }
                            }
                            catch (IndexOutOfRangeException)
                            {
                                MessageBox.Show("There are more lines in the file than added pattern. ", "Too many data sets", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                            }
                            catch
                            {
                                MessageBox.Show("An Error occured during the reading of file lines! Are they correctly formatted?", "Loading data failed!", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        catch
                        {
                            MessageBox.Show("The indices musst be of type int!", "Index Conversion Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }

            this.Close();
        }
    }
}
