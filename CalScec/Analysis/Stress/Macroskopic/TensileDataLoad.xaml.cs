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
    /// Interaction logic for TensileDataLoad.xaml
    /// </summary>
    public partial class TensileDataLoad : Window
    {
        public TensileTest newTensileTest;

        public TensileDataLoad()
        {
            InitializeComponent();
        }

        private void LoadData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime executionDate = (DateTime)this.ExecutionDate.SelectedDate;
                double baseLength = Convert.ToDouble(this.BaseLength.Text);
                double sampleArea = Convert.ToDouble(this.SampleArea.Text);
                double offSet = Convert.ToDouble(this.Offset.Text);

                int timeCol = Convert.ToInt32(this.ColoumnSelection1.Text);
                int forceCol = Convert.ToInt32(this.ColoumnSelection2.Text);
                int extensionCol = Convert.ToInt32(this.ColoumnSelection3.Text);
                int stressCol = Convert.ToInt32(this.ColoumnSelection4.Text);
                int strainCol = Convert.ToInt32(this.ColoumnSelection5.Text);

                if (forceCol == -1 && stressCol == -1)
                {
                    MessageBox.Show("At least a coloumn for force or stress musst be added!", "No force or stress given", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    if (extensionCol == -1 && strainCol == -1)
                    {
                        MessageBox.Show("At least a coloumn for extension or strain musst be added!", "No extension or strain given", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        if (FormatSelection.SelectedIndex == 2)
                        {
                            //CSV Format
                            Microsoft.Win32.OpenFileDialog OpenSampleFile = new Microsoft.Win32.OpenFileDialog();
                            OpenSampleFile.Multiselect = false;
                            OpenSampleFile.DefaultExt = ".csv";
                            OpenSampleFile.Filter = "tensile data (.csv)|*.csv";

                            Nullable<bool> Opened = OpenSampleFile.ShowDialog();

                            if (Opened == true)
                            {
                                string testPath = OpenSampleFile.FileName;
                                bool IsFile = System.IO.File.Exists(testPath);
                                //string[] PatternFileLines = System.IO.File.ReadLines(testPath).ToArray();

                                List<double> timeData = new List<double>();

                                List<double> forceData = new List<double>();
                                List<double> stressData = new List<double>();

                                List<double> extensionData = new List<double>();
                                List<double> strainData = new List<double>();

                                using (var reader = new System.IO.StreamReader(testPath))
                                {
                                    while (!reader.EndOfStream)
                                    {
                                        var line = reader.ReadLine();

                                        try
                                        {
                                            char[] sepChars = { ';', ',' };
                                            string[] splitData = line.Split(sepChars);

                                            if (offSet == -1 && extensionCol != -1)
                                            {
                                                offSet = Convert.ToDouble(splitData[extensionCol]); ;
                                            }

                                            if (timeCol != -1)
                                            {
                                                double timeTmp = Convert.ToDouble(splitData[timeCol]);
                                                timeData.Add(timeTmp);
                                            }
                                            if (forceCol != -1)
                                            {
                                                double forceTmp = Convert.ToDouble(splitData[forceCol]);
                                                if (this.ForceUnit.SelectedIndex == 1)
                                                {
                                                    forceData.Add(forceTmp * 1000);
                                                }
                                                else
                                                {
                                                    forceData.Add(forceTmp);
                                                }
                                            }
                                            else
                                            {
                                                double stressTmp = Convert.ToDouble(splitData[stressCol]);

                                                if (this.StressUnit.SelectedIndex == 0)
                                                {
                                                    stressTmp /= 1000000.0;
                                                }
                                                else if (this.StressUnit.SelectedIndex == 1)
                                                {
                                                    stressTmp /= 1000.0;
                                                }

                                                forceData.Add(stressTmp * sampleArea);
                                            }
                                            if (extensionCol != -1)
                                            {
                                                double extensionTmp = Convert.ToDouble(splitData[extensionCol]);
                                                extensionData.Add(extensionTmp);
                                            }
                                            else
                                            {
                                                double strainTmp = Convert.ToDouble(splitData[strainCol]);
                                                double extensionTmp = strainTmp * (baseLength + offSet);
                                                extensionTmp += offSet;
                                                extensionData.Add(extensionTmp);
                                            }
                                            if (stressCol != -1)
                                            {
                                                double stressTmp = Convert.ToDouble(splitData[stressCol]);
                                                if (this.StressUnit.SelectedIndex == 0)
                                                {
                                                    stressData.Add(stressTmp / 1000000.0);
                                                }
                                                else if (this.StressUnit.SelectedIndex == 1)
                                                {
                                                    stressData.Add(stressTmp / 1000.0);
                                                }
                                                else if (this.StressUnit.SelectedIndex == 2)
                                                {
                                                    stressData.Add(stressTmp);
                                                }
                                                else
                                                {
                                                    stressData.Add(stressTmp);
                                                }

                                            }
                                            else
                                            {
                                                double forceTmp = Convert.ToDouble(splitData[forceCol]);
                                                if (this.ForceUnit.SelectedIndex == 1)
                                                {
                                                    forceTmp *= 1000;
                                                }
                                                stressData.Add(forceTmp / sampleArea);
                                            }
                                            if (strainCol != -1)
                                            {
                                                double strainTmp = Convert.ToDouble(splitData[strainCol]);
                                                strainData.Add(strainTmp);
                                            }
                                            else
                                            {
                                                double extensionTmp = Convert.ToDouble(splitData[extensionCol]);
                                                double strainTmp = (extensionTmp - offSet) / (baseLength + offSet);
                                                strainData.Add(strainTmp);
                                            }
                                        }
                                        catch
                                        {

                                        }
                                    }
                                }

                                this.newTensileTest = new TensileTest(executionDate, baseLength, sampleArea, offSet);
                                this.newTensileTest.TimeData = timeData;
                                this.newTensileTest.ForceData = forceData;
                                this.newTensileTest.ExtensionData = extensionData;
                                this.newTensileTest.StressData = stressData;
                                this.newTensileTest.StrainData = strainData;

                                this.Close();
                            }
                        }
                        else if (FormatSelection.SelectedIndex == 3)
                        {
                            //sonstige Textformate
                            Microsoft.Win32.OpenFileDialog OpenSampleFile = new Microsoft.Win32.OpenFileDialog();
                            //OpenSampleFile.Multiselect = false;
                            //OpenSampleFile.DefaultExt = ".csv";
                            //OpenSampleFile.Filter = "tensile data (.csv)|*.csv";

                            Nullable<bool> Opened = OpenSampleFile.ShowDialog();

                            if (Opened == true)
                            {
                                string testPath = OpenSampleFile.FileName;
                                bool IsFile = System.IO.File.Exists(testPath);
                                //string[] PatternFileLines = System.IO.File.ReadLines(testPath).ToArray();

                                List<double> timeData = new List<double>();

                                List<double> forceData = new List<double>();
                                List<double> stressData = new List<double>();

                                List<double> extensionData = new List<double>();
                                List<double> strainData = new List<double>();

                                using (var reader = new System.IO.StreamReader(testPath))
                                {
                                    while (!reader.EndOfStream)
                                    {
                                        var line = reader.ReadLine();

                                        try
                                        {
                                            char[] sepChars = { ';', ',' };
                                            string[] splitData = line.Split(sepChars);

                                            if (offSet == -1 && extensionCol != -1)
                                            {
                                                offSet = Convert.ToDouble(splitData[extensionCol]); ;
                                            }

                                            if (timeCol != -1)
                                            {
                                                double timeTmp = Convert.ToDouble(splitData[timeCol]);
                                                timeData.Add(timeTmp);
                                            }
                                            if (forceCol != -1)
                                            {
                                                double forceTmp = Convert.ToDouble(splitData[forceCol]);
                                                if (this.ForceUnit.SelectedIndex == 1)
                                                {
                                                    forceData.Add(forceTmp * 1000);
                                                }
                                                else
                                                {
                                                    forceData.Add(forceTmp);
                                                }
                                            }
                                            else
                                            {
                                                double stressTmp = Convert.ToDouble(splitData[stressCol]);

                                                if (this.StressUnit.SelectedIndex == 0)
                                                {
                                                    stressTmp /= 1000000.0;
                                                }
                                                else if (this.StressUnit.SelectedIndex == 1)
                                                {
                                                    stressTmp /= 1000.0;
                                                }

                                                forceData.Add(stressTmp * sampleArea);
                                            }
                                            if (extensionCol != -1)
                                            {
                                                double extensionTmp = Convert.ToDouble(splitData[extensionCol]);
                                                extensionData.Add(extensionTmp);
                                            }
                                            else
                                            {
                                                double strainTmp = Convert.ToDouble(splitData[strainCol]);
                                                double extensionTmp = strainTmp * (baseLength + offSet);
                                                extensionTmp += offSet;
                                                extensionData.Add(extensionTmp);
                                            }
                                            if (stressCol != -1)
                                            {
                                                double stressTmp = Convert.ToDouble(splitData[stressCol]);
                                                if (this.StressUnit.SelectedIndex == 0)
                                                {
                                                    stressData.Add(stressTmp / 1000000.0);
                                                }
                                                else if (this.StressUnit.SelectedIndex == 1)
                                                {
                                                    stressData.Add(stressTmp / 1000.0);
                                                }
                                                else if (this.StressUnit.SelectedIndex == 2)
                                                {
                                                    stressData.Add(stressTmp);
                                                }
                                                else
                                                {
                                                    stressData.Add(stressTmp);
                                                }

                                            }
                                            else
                                            {
                                                double forceTmp = Convert.ToDouble(splitData[forceCol]);
                                                if (this.ForceUnit.SelectedIndex == 1)
                                                {
                                                    forceTmp *= 1000;
                                                }
                                                stressData.Add(forceTmp / sampleArea);
                                            }
                                            if (strainCol != -1)
                                            {
                                                double strainTmp = Convert.ToDouble(splitData[strainCol]);
                                                strainData.Add(strainTmp);
                                            }
                                            else
                                            {
                                                double extensionTmp = Convert.ToDouble(splitData[extensionCol]);
                                                double strainTmp = (extensionTmp - offSet) / (baseLength + offSet);
                                                strainData.Add(strainTmp);
                                            }
                                        }
                                        catch
                                        {

                                        }
                                    }
                                }

                                this.newTensileTest = new TensileTest(executionDate, baseLength, sampleArea, offSet);
                                this.newTensileTest.TimeData = timeData;
                                this.newTensileTest.ForceData = forceData;
                                this.newTensileTest.ExtensionData = extensionData;
                                this.newTensileTest.StressData = stressData;
                                this.newTensileTest.StrainData = strainData;

                                this.Close();
                            }
                        }
                        else
                        {
                            //XLS, XLSX Format
                            Microsoft.Win32.OpenFileDialog OpenSampleFile = new Microsoft.Win32.OpenFileDialog();
                            OpenSampleFile.Multiselect = false;
                            OpenSampleFile.DefaultExt = ".xls";
                            OpenSampleFile.Filter = "tensile data (.xls)|*.xls";

                            Nullable<bool> Opened = OpenSampleFile.ShowDialog();

                            if (Opened == true)
                            {

                            }
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("File in use, please close programs using it!", "File in use", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
