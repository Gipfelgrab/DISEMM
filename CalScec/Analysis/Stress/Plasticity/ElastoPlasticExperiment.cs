using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.Analysis.Stress.Plasticity
{
    [Serializable]
    public class ElastoPlasticExperiment
    {

        private double _chiAngle;
        public double ChiAngle
        {
            get
            {
                return this._chiAngle;
            }
            set
            {
                this._chiAngle = value;
            }
        }
        private double _omegaAngle;
        public double OmegaAngle
        {
            get
            {
                return this._omegaAngle;
            }
            set
            {
                this._omegaAngle = value;
            }
        }

        public ElastoPlasticExperiment(Sample actSample)
        {
            for(int n = 0; n < actSample.CrystalData.Count; n++)
            {
                List<MathNet.Numerics.LinearAlgebra.Matrix<double>> stressHis = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
                List<MathNet.Numerics.LinearAlgebra.Matrix<double>> strainHis = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
                List<MathNet.Numerics.LinearAlgebra.Matrix<double>> stressRateHis = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
                List<MathNet.Numerics.LinearAlgebra.Matrix<double>> strainRateHis = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
                List<MathNet.Numerics.LinearAlgebra.Matrix<double>> hardHis = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
                List<double> shearHist = new List<double>();
                List<double> aSystemsHist = new List<double>();

                this.StressCFHistory.Add(stressHis);
                this.StrainCFHistory.Add(strainHis);
                this.StressRateCFHistory.Add(stressRateHis);
                this.StrainRateCFHistory.Add(strainRateHis);
                this.HardeningCFHistory.Add(hardHis);
                this.ShearRateCFHistory.Add(shearHist);
                this.ActiveSystemsCFHistory.Add(aSystemsHist);
            }
        }
        
        public List<MathNet.Numerics.LinearAlgebra.Matrix<double>> StressSFHistory = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        public List<MathNet.Numerics.LinearAlgebra.Matrix<double>> StrainSFHistory = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

        public List<MathNet.Numerics.LinearAlgebra.Matrix<double>> StressRateSFHistory = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        public List<MathNet.Numerics.LinearAlgebra.Matrix<double>> StrainRateSFHistory = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

        public List<MathNet.Numerics.LinearAlgebra.Matrix<double>> HardeningSFHistory = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

        public List<double> ShearRateSFHistory = new List<double>();
        public List<double> ActiveSystemsSFHistory = new List<double>();

        public List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>> StressCFHistory = new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>();
        public List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>> StrainCFHistory = new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>();

        public List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>> StressRateCFHistory = new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>();
        public List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>> StrainRateCFHistory = new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>();

        public List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>> HardeningCFHistory = new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>();
        public List<List<double>> ShearRateCFHistory = new List<List<double>>();
        public List<List<double>> ActiveSystemsCFHistory = new List<List<double>>();
    }
}
