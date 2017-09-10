using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.DataManagment.Files.SCEC
{
    [Serializable]
    public class ODFInformation
    {
        /// <summary>
        /// [0] phi1
        /// [1] phi
        /// [2] phi2
        /// [3] value
        /// </summary>
        public int TDData = 0;

        private double _stepSizePhi1;

        private double _stepSizePhi;

        private double _stepSizePhi2;

        private double _maxMRD = 0;

        public ElasticityTensorInformation BaseTensor;
        public ElasticityTensorInformation TextureTensor;

        public ODFInformation(Analysis.Texture.OrientationDistributionFunction NewODF)
        {
            //this.TDData = NewODF.TDData;

            this._stepSizePhi = NewODF.StepPhi;
            this._stepSizePhi1 = NewODF.StepSizePhi1;
            this._stepSizePhi2 = NewODF.StepSizePhi2;

            this._maxMRD = NewODF.MaxMRD;

            this.BaseTensor = new ElasticityTensorInformation(NewODF.BaseTensor);
            this.TextureTensor = new ElasticityTensorInformation(NewODF.TextureTensor);
        }

        public Analysis.Texture.OrientationDistributionFunction GetODF()
        {
            Analysis.Texture.OrientationDistributionFunction Ret = new Analysis.Texture.OrientationDistributionFunction();

            //Ret.TDData = this.TDData;
            //Ret.SetStepSizes();

            Ret.BaseTensor = this.BaseTensor.GetElasticityTensor();
            Ret.TextureTensor = this.TextureTensor.GetElasticityTensor();

            return Ret;
        }
    }
}
