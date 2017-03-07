using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.DataManagment.Files.SCEC
{
    [Serializable]
    public class ElasticityTensorInformation
    {
        public MathNet.Numerics.LinearAlgebra.Matrix<double> _stiffnessTensor = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(6, 6, 0.0);
        public MathNet.Numerics.LinearAlgebra.Matrix<double> _stiffnessTensorError = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(6, 6, 0.0);

        public MathNet.Numerics.LinearAlgebra.Matrix<double> _complianceTensor = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(6, 6, 0.0);
        public MathNet.Numerics.LinearAlgebra.Matrix<double> _complianceTensorError = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(6, 6, 0.0);

        public string _symmetry;

        public bool IsIsotropic = false;
        public bool FitConverged = false;

        public ElasticityTensorInformation(Analysis.Stress.Microsopic.ElasticityTensors ET)
        {
            this._stiffnessTensor = ET._stiffnessTensor.Clone();
            this._stiffnessTensorError = ET._stiffnessTensorError.Clone();
            this._complianceTensor = ET._complianceTensor.Clone();
            this._complianceTensorError = ET._complianceTensorError.Clone();

            this.IsIsotropic = ET.IsIsotropic;
            this.FitConverged = ET.FitConverged;

            this._symmetry = ET.Symmetry;
        }

        public Analysis.Stress.Microsopic.ElasticityTensors GetElasticityTensor()
        {
            Analysis.Stress.Microsopic.ElasticityTensors Ret = new Analysis.Stress.Microsopic.ElasticityTensors();

            Ret._stiffnessTensor = this._stiffnessTensor.Clone();
            Ret._stiffnessTensorError = this._stiffnessTensorError.Clone();
            Ret._complianceTensor = this._complianceTensor.Clone();
            Ret._complianceTensorError = this._complianceTensorError.Clone();

            Ret.IsIsotropic = this.IsIsotropic;
            Ret.FitConverged = this.FitConverged;

            Ret.Symmetry = this._symmetry;

            return Ret;
        }
    }
}
