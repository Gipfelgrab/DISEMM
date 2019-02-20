using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.DataManagment.Files.SCEC
{
    [Serializable]
    public class PlasticityTensorInformation
    {
        public MathNet.Numerics.LinearAlgebra.Matrix<double> _hardenningTensor = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
        public MathNet.Numerics.LinearAlgebra.Matrix<double> _isotropicHardenningTensor = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
        public MathNet.Numerics.LinearAlgebra.Matrix<double> _independentHardenningTensor = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
        public MathNet.Numerics.LinearAlgebra.Matrix<double> _kinematicHardenningTensor = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

        public MathNet.Numerics.LinearAlgebra.Matrix<double> _plasticStrainRate = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
        public List<MathNet.Numerics.LinearAlgebra.Matrix<double>> appliedCrystalStress = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

        private MathNet.Numerics.LinearAlgebra.Matrix<double> _phaseStrainRate = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
        
        private double _phaseHardeningRate;
        private double _phaseYieldStrength;
        public double PhaseActYieldStrength = 0.0;

        private int _symmetry;

        public YieldSurfaceInformation YieldSurfaceData;

        public PlasticityTensorInformation(Analysis.Stress.Plasticity.PlasticityTensor pT)
        {
            this._hardenningTensor = pT._hardenningTensor;
            this._isotropicHardenningTensor = pT._isotropicHardenningTensor;
            this._independentHardenningTensor = pT._independentHardenningTensor;
            this._kinematicHardenningTensor = pT._kinematicHardenningTensor;

            this._plasticStrainRate = pT._plasticStrainRate;
            this.appliedCrystalStress = pT.appliedCrystalStress;

            this._phaseStrainRate = pT._phaseStrainRate;


            this._phaseHardeningRate = pT.PhaseHardeningRate;
            this._phaseYieldStrength = pT.PhaseYieldStrength;
            this.PhaseActYieldStrength = pT.PhaseActYieldStrength;

            this._symmetry = pT._symmetry;

            this.YieldSurfaceData = new YieldSurfaceInformation(pT.YieldSurfaceData);
        }

        public Analysis.Stress.Plasticity.PlasticityTensor GetPlastticityTensor()
        {
            Analysis.Stress.Plasticity.PlasticityTensor ret = new Analysis.Stress.Plasticity.PlasticityTensor();

            ret._hardenningTensor = this._hardenningTensor;
            ret._isotropicHardenningTensor = this._isotropicHardenningTensor;
            ret._independentHardenningTensor = this._independentHardenningTensor;
            ret._kinematicHardenningTensor = this._kinematicHardenningTensor;

            ret._plasticStrainRate = this._plasticStrainRate;
            ret.appliedCrystalStress = this.appliedCrystalStress;

            ret._phaseStrainRate = this._phaseStrainRate;


            ret.PhaseHardeningRate = this._phaseHardeningRate;
            ret.PhaseYieldStrength = this._phaseYieldStrength;
            ret.PhaseActYieldStrength = this.PhaseActYieldStrength;

            ret._symmetry = this._symmetry;

            ret.YieldSurfaceData = this.YieldSurfaceData.GetYieldSurface();

            return ret;
        }
    }
}
