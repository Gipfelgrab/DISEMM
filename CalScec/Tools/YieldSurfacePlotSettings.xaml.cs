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
    /// Interaktionslogik für YieldSurfacePlotSettings.xaml
    /// </summary>
    public partial class YieldSurfacePlotSettings : Window
    {
        public bool PreventClosing = true;
        public YieldSurfacePlotSettings()
        {
            InitializeComponent();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (PreventClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
            else
            {
                base.OnClosed(e);
            }
        }
    }
}
