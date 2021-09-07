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
        public AutoFillWindow()
        {
            InitializeComponent();

            ChiAngle1.Text = "-1";
            ChiAngle2.Text = "0";
            ChiAngle3.Text = "0";
            ChiAngleFileName.IsChecked = true;
            PhiAngle1.Text = "-1";
            PhiAngle2.Text = "0";
            PhiAngle3.Text = "0";
            PhiAngleFileName.IsChecked = true;
            OmegaAngle1.Text = "-1";
            OmegaAngle2.Text = "0";
            OmegaAngle3.Text = "0";
            OmegaAngleFileName.IsChecked = true;
            AppliedForce1.Text = "-1";
            AppliedForce2.Text = "0";
            AppliedForce3.Text = "0";
            AppliedForceFileName.IsChecked = true;
            AppliedStress1.Text = "-1";
            AppliedStress2.Text = "0";
            AppliedStress3.Text = "0";
            AppliedStressFileName.IsChecked = true;
            MacroStrain1.Text = "-1";
            MacroStrain2.Text = "0";
            MacroStrain3.Text = "0";
            MacroStrainFileName.IsChecked = true;
        }

        private void Autofill_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
