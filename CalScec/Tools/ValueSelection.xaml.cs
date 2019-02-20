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
    /// Interaction logic for ValueSelection.xaml
    /// </summary>
    public partial class ValueSelection : Window
    {
        public double lborder = -1;
        public double uborder = -1;
        public double step = -1;
        public bool SC = false;
        public string fileName = "";

        public ValueSelection()
        {
            InitializeComponent();
        }

        private void LowerBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                lborder = Convert.ToDouble(LowerBox.Text);
            }
            catch
            {

            }
        }

        private void UpperBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                uborder = Convert.ToDouble(UpperBox.Text);
            }
            catch
            {

            }
        }

        private void StepBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                step = Convert.ToDouble(StepBox.Text);
            }
            catch
            {

            }
        }

        private void StiffnessCheck_Checked(object sender, RoutedEventArgs e)
        {
            SC = true;
        }

        private void StiffnessCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            SC = false;
        }

        private void FileNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.fileName = FileNameBox.Text;
        }
    }
}
