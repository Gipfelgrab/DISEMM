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
    /// Interaktionslogik für TextureRawDataView.xaml
    /// </summary>
    public partial class TextureRawDataView : Window
    {
        List<Analysis.Stress.Plasticity.GrainOrientationParameter> Texture = new List<Stress.Plasticity.GrainOrientationParameter>();
        public TextureRawDataView(List<double[]> textureRawData)
        {
            InitializeComponent();

            for(int n = 0; n < textureRawData.Count; n++)
            {
                Analysis.Stress.Plasticity.GrainOrientationParameter gOP = new Stress.Plasticity.GrainOrientationParameter(textureRawData[n][0], textureRawData[n][1], textureRawData[n][2], textureRawData[n][3]);
                this.Texture.Add(gOP);
            }

            this.GrainorientationList.ItemsSource = Texture;
        }
    }
}
