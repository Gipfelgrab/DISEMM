using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.DataManagment.Files.SCEC
{
    [Serializable]
    public class PeakFunctionInformation
    {

        #region Peak function

        /// <summary>
        /// 0 if info is enough for Diffracftion Peak
        /// 1 if info is enough for Peakfunction
        /// </summary>
        public int InfoType;

        public int PeakId;
        /// <summary>
        /// Use as follows:
        /// 0 = Gaussian function
        /// 1 = Lorentz function
        /// 2 = Pseudo Voigt function
        /// </summary>
        public int _functionType;
        /// <summary>
        /// Use as follows:
        /// false = Off
        /// true = On
        /// </summary>
        public bool backgroundSwitch;
        public bool backgroundFit;

        public Analysis.Peaks.Functions.Gaussian GaussianFunction;
        public Analysis.Peaks.Functions.Lorentz LorentzFunction;
        public Analysis.Peaks.Functions.PseudoVoigt PseudoVoigtFunction;
        public Analysis.Peaks.Functions.BackgroundPolynomial PolynomialBackgroundFunction;

        public double _intensityGaussian;
        public double _intensityLorentz;
        public double _intensityPseudoVoigt;

        public Pattern.Counts FittingCounts;

        /// <summary>
        /// [0] = Sigma
        /// [1] = Angle
        /// [2] = LorentzRatio
        /// [3] = Intensity
        /// [4] = ConstantBackground
        /// [5] = CenterBackground
        /// [6] = AclivityBackground
        /// </summary>
        public bool[] FreeParameters = { true, true, true, true, true, true, true };


        public Analysis.Peaks.Functions.Constraints ParameterConstraints;


        /// <summary>
        /// If positive this lamdba is used in LMA as starting parameter
        /// </summary>
        public double startingLambda;

        public bool _fitConverged;

        public bool FitCompleted;

        #endregion

        #region Diffraction peak information

        #region Detection parameters

        public int _detectedChannel;

        public double _detectedAngle;

        public double _detectedBackground;

        public double _detectedHeight;
        #endregion

        #region HKL Stuff

        public bool _toHKLAssociated = false;

        public DataManagment.CrystalData.CODData AssociatedCrystalData;
        public DataManagment.CrystalData.HKLReflex AssociatedHKLReflex;

        #endregion

        #endregion

        public PeakFunctionInformation(Analysis.Peaks.DiffractionPeak Dp)
        {
            this.InfoType = 0;

            this.PeakId = Dp.PeakId;
            this._functionType = Dp.PFunction.functionType;

            this._detectedChannel = Dp.DetectedChannel;
            this._detectedAngle = Dp.DetectedAngle;
            this._detectedBackground = Dp.DetectedBackground;

            this._toHKLAssociated = Dp.ToHKLAssociated;

            this.AssociatedCrystalData = new CrystalData.CODData(Dp.AssociatedCrystalData);
            this.AssociatedHKLReflex = new CrystalData.HKLReflex(Dp.AssociatedHKLReflex.H, Dp.AssociatedHKLReflex.K, Dp.AssociatedHKLReflex.L, Dp.AssociatedHKLReflex.Distance);

            this._functionType = Dp.PFunction.functionType;
            this.startingLambda = Dp.PFunction.startingLambda;
            this.FitCompleted = Dp.PFunction.FitCompleted;
            this._fitConverged = Dp.PFunction.FitConverged;

            this.ParameterConstraints = Dp.PFunction.ParameterConstraints.Clone() as Analysis.Peaks.Functions.Constraints;

            this.FreeParameters[0] = Dp.PFunction.FreeParameters[0];
            this.FreeParameters[1] = Dp.PFunction.FreeParameters[1];
            this.FreeParameters[2] = Dp.PFunction.FreeParameters[2];
            this.FreeParameters[3] = Dp.PFunction.FreeParameters[3];
            this.FreeParameters[4] = Dp.PFunction.FreeParameters[4];
            this.FreeParameters[5] = Dp.PFunction.FreeParameters[5];
            this.FreeParameters[6] = Dp.PFunction.FreeParameters[6];

            this.FittingCounts = Dp.PFunction.FittingCounts.Clone() as Pattern.Counts;

            this.backgroundSwitch = Dp.PFunction.backgroundSwitch;
            this.backgroundFit = Dp.PFunction.backgroundFit;

            this.GaussianFunction = Dp.PFunction.GetGaussianFunction();
            this.LorentzFunction = Dp.PFunction.GetLorentzFunction();
            this.PseudoVoigtFunction = Dp.PFunction.GetPseudoVoigt();
            this.PolynomialBackgroundFunction = Dp.PFunction.GetPolynomialBackground();

            Dp.PFunction.functionType = 0;
            this._intensityGaussian = Dp.PFunction.Intensity;
            Dp.PFunction.functionType = 1;
            this._intensityLorentz = Dp.PFunction.Intensity;
            Dp.PFunction.functionType = 2;
            this._intensityPseudoVoigt = Dp.PFunction.Intensity;

            Dp.PFunction.functionType = this._functionType;
        }
    }
}
