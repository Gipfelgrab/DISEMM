///////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////Im Gedenken an Tepi//////////////////////////////////////
//////////////////////Das Leben ist wie eine Reise in totaler Dunkelheit://////////////////////
/////Man weiß wie wo der nächste Schritt hinführt, aber jeder findet irgendwann das Licht//////
///////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.Analysis.Peaks.Functions
{
    
    public class PeakFunction : ICloneable
    {

        #region Parameters

        private int _peakId;
        public int PeakId
        {
            get
            {
                return this._peakId;
            }
        }

        /// <summary>
        /// Use as follows:
        /// 0 = Gaussian function
        /// 1 = Lorentz function
        /// 2 = Pseudo Voigt function
        /// </summary>
        public int functionType
        {
            get
            {
                return this._functionType;
            }
            set
            {
                switch (value)
                {
                    case 0:
                        this.FreeParameters[2] = false;
                        this._functionType = value;
                        break;
                    case 1:
                        goto case 0;
                    case 2:
                        goto default;
                    default:
                        this._functionType = value;
                        break;
                }
            }
        }
        private int _functionType;
        /// <summary>
        /// Use as follows:
        /// false = Off
        /// true = On
        /// </summary>
        public bool backgroundSwitch;
        public bool backgroundFit;

        private Gaussian GaussianFunction;
        private Lorentz LorentzFunction;
        private PseudoVoigt PseudoVoigtFunction;
        private BackgroundPolynomial PolynomialBackgroundFunction;

        public Gaussian GetGaussianFunction()
        {
            return new Gaussian(this.GaussianFunction.Angle, this.GaussianFunction.FWHM);
        }
        public Lorentz GetLorentzFunction()
        {
            return new Lorentz(this.LorentzFunction.Angle, this.LorentzFunction.FWHM);
        }
        public PseudoVoigt GetPseudoVoigt()
        {
            return new PseudoVoigt(this.PseudoVoigtFunction.Angle, this.PseudoVoigtFunction.FWHM, this.PseudoVoigtFunction.LorentzRatio);
        }
        public BackgroundPolynomial GetPolynomialBackground()
        {
            return new BackgroundPolynomial(this.PolynomialBackgroundFunction.Center, this.PolynomialBackgroundFunction.Constant, this.PolynomialBackgroundFunction.Aclivity);
        }

        private double _intensityGaussian;
        private double _intensityLorentz;
        private double _intensityPseudoVoigt;

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
        public double Sigma
        {
            get
            {
                switch (this.functionType)
                {
                    case 0:
                        return this.GaussianFunction.Sigma;
                    case 1:
                        return this.LorentzFunction.Sigma;
                    case 2:
                        return this.PseudoVoigtFunction.Sigma;
                    default:
                        return this.PseudoVoigtFunction.Sigma;
                }
            }
            set
            {
                double NewValue = value;
                if (NewValue < 0.001)
                {
                    NewValue = 0.001;
                }
                else if (NewValue > 3.0)
                {
                    NewValue = 3.0;
                }
                switch (this.functionType)
                {
                    case 0:
                        this.GaussianFunction.Sigma = NewValue;
                        return;
                    case 1:
                        this.LorentzFunction.Sigma = NewValue;
                        return;
                    case 2:
                        this.PseudoVoigtFunction.Sigma = NewValue;
                        return;
                    default:
                        this.PseudoVoigtFunction.Sigma = NewValue;
                        return;
                }
            }
        }
        public double SetSigmaWithConstraints
        {
            set
            {
                if (this.ParameterConstraints.SigmaConstraintActiv)
                {
                    double ForSet = this.Sigma;
                    if (this.ParameterConstraints.SigmaConstraint > Math.Abs(value))
                    {
                        ForSet += value;
                    }
                    else
                    {
                        if (value > 0)
                        {
                            ForSet += this.ParameterConstraints.SigmaConstraint;
                        }
                        else
                        {
                            ForSet -= this.ParameterConstraints.SigmaConstraint;
                        }
                    }

                    switch (this.functionType)
                    {
                        case 0:
                            this.GaussianFunction.Sigma = ForSet;
                            return;
                        case 1:
                            this.LorentzFunction.Sigma = ForSet;
                            return;
                        case 2:
                            this.PseudoVoigtFunction.Sigma = ForSet;
                            return;
                        default:
                            this.PseudoVoigtFunction.Sigma = ForSet;
                            return;

                    }
                }
                else
                {
                    this.Sigma = value;
                }
            }
        }

        public double FWHM
        {
            get
            {
                switch (this.functionType)
                {
                    case 0:
                        return this.GaussianFunction.FWHM;
                    case 1:
                        return this.LorentzFunction.FWHM;
                    case 2:
                        return this.PseudoVoigtFunction.FWHM;
                    default:
                        return this.PseudoVoigtFunction.FWHM;
                }
            }
            set
            {
                switch (this.functionType)
                {
                    case 0:
                        this.GaussianFunction.FWHM = value;
                        return;
                    case 1:
                        this.LorentzFunction.FWHM = value;
                        return;
                    case 2:
                        this.PseudoVoigtFunction.FWHM = value;
                        return;
                    default:
                        this.PseudoVoigtFunction.FWHM = value;
                        return;
                }
            }
        }

        public double Angle
        {
            get
            {
                switch (this.functionType)
                {
                    case 0:
                        return this.GaussianFunction.Angle;
                    case 1:
                        return this.LorentzFunction.Angle;
                    case 2:
                        return this.PseudoVoigtFunction.Angle;
                    default:
                        return this.PseudoVoigtFunction.Angle;
                }
            }
            set
            {
                switch (this.functionType)
                {
                    case 0:
                        this.GaussianFunction.Angle = value;
                        return;
                    case 1:
                        this.LorentzFunction.Angle = value;
                        return;
                    case 2:
                        this.PseudoVoigtFunction.Angle = value;
                        return;
                    default:
                        this.PseudoVoigtFunction.Angle = value;
                        return;
                }
            }
        }
        public double SetAngleWithConstraints
        {
            set
            {
                if (this.ParameterConstraints.AngleConstraintActiv)
                {
                    double ForSet = this.Angle;
                    if (this.ParameterConstraints.AngleConstraint > Math.Abs(value))
                    {
                        ForSet += value;
                    }
                    else
                    {
                        if (value > 0)
                        {
                            ForSet += this.ParameterConstraints.AngleConstraint;
                        }
                        else
                        {
                            ForSet -= this.ParameterConstraints.AngleConstraint;
                        }
                    }

                    switch (this.functionType)
                    {
                        case 0:
                            this.GaussianFunction.Angle = ForSet;
                            return;
                        case 1:
                            this.LorentzFunction.Angle = ForSet;
                            return;
                        case 2:
                            this.PseudoVoigtFunction.Angle = ForSet;
                            return;
                        default:
                            this.PseudoVoigtFunction.Angle = ForSet;
                            return;

                    }
                }
                else
                {
                    this.Angle = value;
                }
            }
        }

        public double LorentzRatio
        {
            get
            {
                switch (this.functionType)
                {
                    case 0:
                        return 0.0;
                    case 1:
                        return 0.0;
                    case 2:
                        return this.PseudoVoigtFunction.LorentzRatio;
                    default:
                        return this.PseudoVoigtFunction.LorentzRatio;
                }
            }
            set
            {
                switch (this.functionType)
                {
                    case 0:
                        return;
                    case 1:
                        return;
                    case 2:
                        if (value > 0.0)
                        {
                            if (value < 1.0)
                            {
                                this.PseudoVoigtFunction.LorentzRatio = value;
                            }
                            else
                            {
                                this.PseudoVoigtFunction.LorentzRatio = 1.0;
                            }
                        }
                        else
                        {
                            this.PseudoVoigtFunction.LorentzRatio = 0.0;
                        }
                        return;
                    default:
                        if (value > 0.0)
                        {
                            if (value < 1.0)
                            {
                                this.PseudoVoigtFunction.LorentzRatio = value;
                            }
                            else
                            {
                                this.PseudoVoigtFunction.LorentzRatio = 1.0;
                            }
                        }
                        else
                        {
                            this.PseudoVoigtFunction.LorentzRatio = 0.0;
                        }
                        return;
                }
            }
        }
        public double SetLorentzRatioWithConstraints
        {
            set
            {
                if (this.ParameterConstraints.LorentzRatioConstraintActiv)
                {
                    double ForSet = this.LorentzRatio;
                    if (this.ParameterConstraints.LorentzRatioConstraint > Math.Abs(value))
                    {
                        ForSet += value;
                    }
                    else
                    {
                        if (value > 0)
                        {
                            ForSet += this.ParameterConstraints.AngleConstraint;
                        }
                        else
                        {
                            ForSet -= this.ParameterConstraints.AngleConstraint;
                        }
                    }
                    switch (this.functionType)
                    {
                        case 0:
                            return;
                        case 1:
                            return;
                        case 2:
                            this.PseudoVoigtFunction.LorentzRatio = ForSet;
                            return;
                        default:
                            this.PseudoVoigtFunction.LorentzRatio = ForSet;
                            return;
                    }
                }
            }
        }

        public double Intensity
        {
            get
            {
                switch (this.functionType)
                {
                    case 0:
                        return this._intensityGaussian;
                    case 1:
                        return this._intensityLorentz;
                    case 2:
                        return this._intensityPseudoVoigt;
                    default:
                        return this._intensityPseudoVoigt;
                }
            }
            set
            {
                double NewValue = value;
                if(NewValue < 1.0)
                {
                    NewValue = 1;
                }
                switch (this.functionType)
                {
                    case 0:
                        this._intensityGaussian = NewValue;
                        return;
                    case 1:
                        this._intensityLorentz = NewValue;
                        return;
                    case 2:
                        this._intensityPseudoVoigt = NewValue;
                        return;
                    default:
                        this._intensityPseudoVoigt = NewValue;
                        return;
                }
            }
        }

        public double ConstantBackground
        {
            get
            {
                if (this.backgroundSwitch)
                {
                    return this.PolynomialBackgroundFunction.Constant;
                }
                else
                {
                    return 0.0;
                }
            }
            set
            {
                if (this.backgroundSwitch)
                {
                    this.PolynomialBackgroundFunction.Constant = value;
                }
            }
        }

        public double CenterBackground
        {
            get
            {
                if (this.backgroundSwitch)
                {
                    return this.PolynomialBackgroundFunction.Center;
                }
                else
                {
                    return 0.0;
                }
            }
            set
            {
                if (this.backgroundSwitch)
                {
                    this.PolynomialBackgroundFunction.Center = value;
                }
            }
        }

        public double AclivityBackground
        {
            get
            {
                if (this.backgroundSwitch)
                {
                    return this.PolynomialBackgroundFunction.Aclivity;
                }
                else
                {
                    return 0.0;
                }
            }
            set
            {
                if (this.backgroundSwitch)
                {
                    this.PolynomialBackgroundFunction.Aclivity = value;
                }
            }
        }

        public Constraints ParameterConstraints;

        public double Chi2Function
        {
            get
            {
                return Analysis.Fitting.Chi2.Chi2PeakFunction(this);
            }
        }

        public double ReducedChi2Function
        {
            get
            {
                return Analysis.Fitting.Chi2.ReducedChi2PeakFunction(this);
            }
        }

        public System.Threading.ManualResetEvent _doneEvent;

        /// <summary>
        /// If positive this lamdba is used in LMA as starting parameter
        /// </summary>
        public double startingLambda;

        private bool _fitConverged;
        public bool FitConverged
        {
            get
            {
                return this._fitConverged;
            }
        }

        private bool _fitCompleted;
        public bool FitCompleted
        {
            get
            {
                return this._fitCompleted;
            }
            set
            {
                this._fitCompleted = value;
            }
        }

        #endregion

        private PeakFunction(int iD)
        {
            this._peakId = iD;
            this.startingLambda = -1.0;
            this._fitConverged = false;
            this._fitCompleted = true;
        }

        public PeakFunction(double angle, Pattern.Counts fittingCounts)
        {
            this._peakId = Tools.IdManagment.ActualPeakId;

            this.GaussianFunction = new Gaussian(angle);
            this.LorentzFunction = new Lorentz(angle);
            this.PseudoVoigtFunction = new PseudoVoigt(angle);
            this.PolynomialBackgroundFunction = new BackgroundPolynomial(angle);

            this._intensityGaussian = 100.0;
            this._intensityLorentz = 100.0;
            this._intensityPseudoVoigt = 100.0;

            this.FittingCounts = fittingCounts;
            ParameterConstraints = new Constraints();

            this.backgroundFit = true;
            this.startingLambda = -1.0;
            this._fitConverged = false;
            this._fitCompleted = true;
        }

        public PeakFunction(double angle, Pattern.Counts fittingCounts, BackgroundPolynomial BP)
        {
            this._peakId = Tools.IdManagment.ActualPeakId;

            this.GaussianFunction = new Gaussian(angle);
            this.LorentzFunction = new Lorentz(angle);
            this.PseudoVoigtFunction = new PseudoVoigt(angle);
            this.PolynomialBackgroundFunction = BP;

            this._intensityGaussian = 100.0;
            this._intensityLorentz = 100.0;
            this._intensityPseudoVoigt = 100.0;

            this.FittingCounts = fittingCounts;
            ParameterConstraints = new Constraints();

            this.backgroundFit = true;
            this.startingLambda = -1.0;
            this._fitConverged = false;
            this._fitCompleted = true;
        }

        public PeakFunction(double angle, double intensity, Pattern.Counts fittingCounts)
        {
            this._peakId = Tools.IdManagment.ActualPeakId;

            this.GaussianFunction = new Gaussian(angle);
            this.LorentzFunction = new Lorentz(angle);
            this.PseudoVoigtFunction = new PseudoVoigt(angle);
            this.PolynomialBackgroundFunction = new BackgroundPolynomial(angle);

            this._intensityGaussian = intensity;
            this._intensityLorentz = intensity;
            this._intensityPseudoVoigt = intensity;

            this.FittingCounts = fittingCounts;
            ParameterConstraints = new Constraints();

            this.backgroundFit = true;
            this.startingLambda = -1.0;
            this._fitConverged = false;
            this._fitCompleted = true;
        }

        public PeakFunction(double angle, double intensity, Pattern.Counts fittingCounts, BackgroundPolynomial BP)
        {
            this._peakId = Tools.IdManagment.ActualPeakId;

            this.GaussianFunction = new Gaussian(angle);
            this.LorentzFunction = new Lorentz(angle);
            this.PseudoVoigtFunction = new PseudoVoigt(angle);
            this.PolynomialBackgroundFunction = new BackgroundPolynomial(angle);

            this._intensityGaussian = intensity;
            this._intensityLorentz = intensity;
            this._intensityPseudoVoigt = intensity;

            this.FittingCounts = fittingCounts;
            ParameterConstraints = new Constraints();

            this.backgroundFit = true;
            this.startingLambda = -1.0;
            this._fitConverged = false;
            this._fitCompleted = true;
        }

        public PeakFunction(double angle, double intensity, double constantBackground, Pattern.Counts fittingCounts)
        {
            this._peakId = Tools.IdManagment.ActualPeakId;

            this.GaussianFunction = new Gaussian(angle);
            this.LorentzFunction = new Lorentz(angle);
            this.PseudoVoigtFunction = new PseudoVoigt(angle);
            this.PolynomialBackgroundFunction = new BackgroundPolynomial(angle, constantBackground);

            this._intensityGaussian = intensity;
            this._intensityLorentz = intensity;
            this._intensityPseudoVoigt = intensity;

            this.FittingCounts = fittingCounts;
            ParameterConstraints = new Constraints();

            this.backgroundFit = true;
            this.startingLambda = -1.0;
            this._fitConverged = false;
            this._fitCompleted = true;
        }

        public PeakFunction(double angle, double intensity, double constantBackground, double fWHM, Pattern.Counts fittingCounts)
        {
            this._peakId = Tools.IdManagment.ActualPeakId;

            this.GaussianFunction = new Gaussian(angle, fWHM);
            this.LorentzFunction = new Lorentz(angle, fWHM);
            this.PseudoVoigtFunction = new PseudoVoigt(angle, fWHM);
            this.PolynomialBackgroundFunction = new BackgroundPolynomial(angle, constantBackground);

            this._intensityGaussian = intensity;
            this._intensityLorentz = intensity;
            this._intensityPseudoVoigt = intensity;

            this.FittingCounts = fittingCounts;
            ParameterConstraints = new Constraints();

            this.backgroundFit = true;
            this.startingLambda = -1.0;
            this._fitConverged = false;
            this._fitCompleted = true;
        }

        public PeakFunction(double angle, double intensity, double constantBackground, double fWHM, Constraints constrains, Pattern.Counts fittingCounts)
        {
            this._peakId = Tools.IdManagment.ActualPeakId;

            this.GaussianFunction = new Gaussian(angle, fWHM);
            this.LorentzFunction = new Lorentz(angle, fWHM);
            this.PseudoVoigtFunction = new PseudoVoigt(angle, fWHM);
            this.PolynomialBackgroundFunction = new BackgroundPolynomial(angle, constantBackground);

            this._intensityGaussian = intensity;
            this._intensityLorentz = intensity;
            this._intensityPseudoVoigt = intensity;

            this.FittingCounts = fittingCounts;
            ParameterConstraints = constrains.Clone() as Constraints;

            this.backgroundFit = true;
            this.startingLambda = -1.0;
            this._fitConverged = false;
            this._fitCompleted = true;
        }

        public PeakFunction(DataManagment.Files.SCEC.PeakFunctionInformation PFI)
        {
            this._peakId = PFI.PeakId;
            this._functionType = PFI._functionType;
            this.backgroundFit = PFI.backgroundFit;
            this.backgroundSwitch = PFI.backgroundSwitch;

            this.GaussianFunction = PFI.GaussianFunction;
            this.LorentzFunction = PFI.LorentzFunction;
            this.PseudoVoigtFunction = PFI.PseudoVoigtFunction;
            this.PolynomialBackgroundFunction = PFI.PolynomialBackgroundFunction;
            this.FittingCounts = PFI.FittingCounts;

            this.FreeParameters = PFI.FreeParameters;
            this.ParameterConstraints = PFI.ParameterConstraints;

            this.startingLambda = PFI.startingLambda;
            this._fitConverged = PFI._fitConverged;
            this._fitCompleted = PFI.FitCompleted;

            this._intensityGaussian = PFI._intensityGaussian;
            this._intensityLorentz = PFI._intensityLorentz;
            this._intensityPseudoVoigt = PFI._intensityPseudoVoigt;
        }

        #region Calculation

        public double Y(double theta)
        {
            if(backgroundSwitch)
            {
                switch (this.functionType)
                {
                    case 0:
                        return  (this._intensityGaussian * this.GaussianFunction.Y(theta)) + PolynomialBackgroundFunction.Y(theta);
                    case 1:
                        return (this._intensityLorentz * this.LorentzFunction.Y(theta)) + PolynomialBackgroundFunction.Y(theta);
                    case 2:
                        return (this._intensityPseudoVoigt * this.PseudoVoigtFunction.Y(theta)) + PolynomialBackgroundFunction.Y(theta);
                    default:
                        return (this._intensityPseudoVoigt * this.PseudoVoigtFunction.Y(theta)) + PolynomialBackgroundFunction.Y(theta);
                }
            }
            else
            {
                switch (this.functionType)
                {
                    case 0:
                        return this._intensityGaussian * this.GaussianFunction.Y(theta);
                    case 1:
                        return this._intensityLorentz * this.LorentzFunction.Y(theta);
                    case 2:
                        return this._intensityPseudoVoigt * this.PseudoVoigtFunction.Y(theta);
                    default:
                        return this._intensityPseudoVoigt * this.PseudoVoigtFunction.Y(theta);
                }
            }
        }

        public double YNoBackground(double theta)
        {
            switch (this.functionType)
            {
                case 0:
                    return this._intensityGaussian * this.GaussianFunction.Y(theta);
                case 1:
                    return this._intensityLorentz * this.LorentzFunction.Y(theta);
                case 2:
                    return this._intensityPseudoVoigt * this.PseudoVoigtFunction.Y(theta);
                default:
                    return this._intensityPseudoVoigt * this.PseudoVoigtFunction.Y(theta);
            }
        }

        #region First Derivative

        public double FirstDerivativeSigma(double theta)
        {
            switch (this.functionType)
            {
                case 0:
                    return this._intensityGaussian * this.GaussianFunction.FirstDerivativeSigma(theta);
                case 1:
                    return this._intensityLorentz * this.LorentzFunction.FirstDerivativeSigma(theta);
                case 2:
                    return this._intensityPseudoVoigt * this.PseudoVoigtFunction.FirstDerivativeSigma(theta);
                default:
                    return this._intensityPseudoVoigt * this.PseudoVoigtFunction.FirstDerivativeSigma(theta);
            }
        }

        public double FirstDerivativeAngle(double theta)
        {
            switch (this.functionType)
            {
                case 0:
                    return this._intensityGaussian * this.GaussianFunction.FirstDerivativeAngle(theta);
                case 1:
                    return this._intensityLorentz * this.LorentzFunction.FirstDerivativeAngle(theta);
                case 2:
                    return this._intensityPseudoVoigt * this.PseudoVoigtFunction.FirstDerivativeAngle(theta);
                default:
                    return this._intensityPseudoVoigt * this.PseudoVoigtFunction.FirstDerivativeAngle(theta);
            }
        }

        public double FirstDerivativeIntensity(double theta)
        {
            switch (this.functionType)
            {
                case 0:
                    return this.GaussianFunction.Y(theta);
                case 1:
                    return this.LorentzFunction.Y(theta);
                case 2:
                    return this.PseudoVoigtFunction.Y(theta);
                default:
                    return this.PseudoVoigtFunction.Y(theta);
            }
        }

        public double FirstDerivativeLorentzRatio(double theta)
        {
            switch (this.functionType)
            {
                case 0:
                    return 0.0;
                case 1:
                    return 0.0;
                case 2:
                    return this._intensityPseudoVoigt * this.PseudoVoigtFunction.FirstDerivativeLorentzRatio(theta);
                default:
                    return this._intensityPseudoVoigt * this.PseudoVoigtFunction.FirstDerivativeLorentzRatio(theta);
            }
        }

        public double FirstDerivativeConstant(double theta)
        {
            if (backgroundSwitch)
            {
                return this.PolynomialBackgroundFunction.FirstDerivativeConstant(theta);
            }
            else
            {
                return 0.0;
            }
        }

        public double FirstDerivativeCenter(double theta)
        {
            if (backgroundSwitch)
            {
                return this.PolynomialBackgroundFunction.FirstDerivativeCenter(theta);
            }
            else
            {
                return 0.0;
            }
        }

        public double FirstDerivativeAclivity(double theta)
        {
            if (backgroundSwitch)
            {
                return this.PolynomialBackgroundFunction.FirstDerivativeAclivity(theta);
            }
            else
            {
                return 0.0;
            }
        }

        #endregion

        #region Second Derivative

        public double SecondDerivativeSigmaSigma(double theta)
        {
            switch (this.functionType)
            {
                case 0:
                    return this._intensityGaussian * this.GaussianFunction.SecondDerivativeSigmaSigma(theta);
                case 1:
                    return this._intensityLorentz * this.LorentzFunction.SecondDerivativeSigmaSigma(theta);
                case 2:
                    return this._intensityPseudoVoigt * this.PseudoVoigtFunction.SecondDerivativeSigmaSigma(theta);
                default:
                    return this._intensityPseudoVoigt * this.PseudoVoigtFunction.SecondDerivativeSigmaSigma(theta);
            }
        }

        public double SecondDerivativeAngleSigma(double theta)
        {
            switch (this.functionType)
            {
                case 0:
                    return this._intensityGaussian * this.GaussianFunction.SecondDerivativeAngleSigma(theta);
                case 1:
                    return this._intensityLorentz * this.LorentzFunction.SecondDerivativeAngleSigma(theta);
                case 2:
                    return this._intensityPseudoVoigt * this.PseudoVoigtFunction.SecondDerivativeAngleSigma(theta);
                default:
                    return this._intensityPseudoVoigt * this.PseudoVoigtFunction.SecondDerivativeAngleSigma(theta);
            }
        }

        public double SecondDerivativeAngleAngle(double theta)
        {
            switch (this.functionType)
            {
                case 0:
                    return this._intensityGaussian * this.GaussianFunction.SecondDerivativeAngleAngle(theta);
                case 1:
                    return this._intensityLorentz * this.LorentzFunction.SecondDerivativeAngleAngle(theta);
                case 2:
                    return this._intensityPseudoVoigt * this.PseudoVoigtFunction.SecondDerivativeAngleAngle(theta);
                default:
                    return this._intensityPseudoVoigt * this.PseudoVoigtFunction.SecondDerivativeAngleAngle(theta);
            }
        }

        public double SecondDerivativeAngleLorentzRatio(double theta)
        {
            switch (this.functionType)
            {
                case 0:
                    return 0.0;
                case 1:
                    return 0.0;
                case 2:
                    return this._intensityPseudoVoigt * this.PseudoVoigtFunction.SecondDerivativeAngleLorentzRatio(theta);
                default:
                    return this._intensityPseudoVoigt * this.PseudoVoigtFunction.SecondDerivativeAngleLorentzRatio(theta);
            }
        }

        public double SecondDerivativeLorentzRatioLorentzRatio(double theta)
        {
            switch (this.functionType)
            {
                case 0:
                    return 0.0;
                case 1:
                    return 0.0;
                case 2:
                    return this._intensityPseudoVoigt * this.PseudoVoigtFunction.SecondDerivativeLorentzRatioLorentzRatio(theta);
                default:
                    return this._intensityPseudoVoigt * this.PseudoVoigtFunction.SecondDerivativeLorentzRatioLorentzRatio(theta);
            }
        }

        public double SecondDerivativeSigmaLorentzRatio(double theta)
        {
            switch (this.functionType)
            {
                case 0:
                    return 0.0;
                case 1:
                    return 0.0;
                case 2:
                    return this._intensityPseudoVoigt * this.PseudoVoigtFunction.SecondDerivativeSigmaLorentzRatio(theta);
                default:
                    return this._intensityPseudoVoigt * this.PseudoVoigtFunction.SecondDerivativeSigmaLorentzRatio(theta);
            }
        }

        public double SecondDerivativeConstantConstant(double theta)
        {
            return 0.0;
        }

        public double SecondDerivativeConstantSigma(double theta)
        {
            return 0.0;
        }

        public double SecondDerivativeConstantAngle(double theta)
        {
            return 0.0;
        }

        public double SecondDerivativeConstantLorentzRatio(double theta)
        {
            return 0.0;
        }

        public double SecondDerivativeCenterCenter(double theta)
        {
            if (backgroundSwitch)
            {
                return this.PolynomialBackgroundFunction.SecondDerivativeCenterCenter(theta);
            }
            else
            {
                return 0.0;
            }
        }

        public double SecondDerivativeConstantCenter(double theta)
        {
            if (backgroundSwitch)
            {
                return this.PolynomialBackgroundFunction.SecondDerivativeConstantCenter(theta);
            }
            else
            {
                return 0.0;
            }
        }

        public double SecondDerivativeAngleCenter(double theta)
        {
            return 0.0;
        }

        public double SecondDerivativeSigmaCenter(double theta)
        {
            return 0.0;
        }

        public double SecondDerivativeLorentzRatioCenter(double theta)
        {
            return 0.0;
        }

        public double SecondDerivativeAclicityAclivity(double theta)
        {
            if (backgroundSwitch)
            {
                return this.PolynomialBackgroundFunction.SecondDerivativeAclicityAclivity(theta);
            }
            else
            {
                return 0.0;
            }
        }

        public double SecondDerivativeCenterAclicity(double theta)
        {
            if (backgroundSwitch)
            {
                return this.PolynomialBackgroundFunction.SecondDerivativeCenterAclicity(theta);
            }
            else
            {
                return 0.0;
            }
        }

        public double SecondDerivativeConstantAclivity(double theta)
        {
            if (backgroundSwitch)
            {
                return this.PolynomialBackgroundFunction.SecondDerivativeConstantAclivity(theta);
            }
            else
            {
                return 0.0;
            }
        }

        public double SecondDerivativeAngleAclivity(double theta)
        {
            return 0.0;
        }

        public double SecondDerivativeSigmaAclivity(double theta)
        {
            return 0.0;
        }

        public double SecondDerivativeLorentzRatioAclivity(double theta)
        {
            return 0.0;
        }

        #endregion

        #region Reduced Hessian

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Sigma
        ///[1] Angle
        ///[2] Intensity
        ///[3] Lorentz Ratio
        ///[4] Background Constant
        ///[5] BackgroundCenter
        ///[6] Background aclivity
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorSigmaAngleIntensityLorentzRatioBackground(double Lambda)
        {
            //[0][0] Sigma
            //[1][1] Angle
            //[2][2] Intensity
            //[3][3] Lorentz Ratio
            //[4][4] Background Constant
            //[5][5] BackgroundCenter
            //[6][6] Background aclivity
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(7, 7, 0.0);

            //[0] Sigma
            //[1] Angle
            //[2] Intensity
            //[3] Lorentz Ratio
            //[4] Background Constant
            //[5] BackgroundCenter
            //[6] Background aclivity
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(7);

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeSigma(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeAngle(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeSigma(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeSigma(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[3, 3] += (this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[4, 4] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[5, 5] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 5] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[5, 0] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 5] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[5, 1] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[5, 2] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 5] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 5] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[5, 3] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[5, 4] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 5] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[6, 6] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 6] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[6, 0] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 6] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[6, 1] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[6, 2] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 6] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 6] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[6, 3] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[6, 4] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 6] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[6, 5] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[5, 6] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                #endregion
                
                #region Vector build

                SolutionVector[0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeSigma(this.FittingCounts[n][0]);
                SolutionVector[1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeAngle(this.FittingCounts[n][0]);
                SolutionVector[2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]);
                SolutionVector[3] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]);
                SolutionVector[4] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeConstant(this.FittingCounts[n][0]);
                SolutionVector[5] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeCenter(this.FittingCounts[n][0]);
                SolutionVector[6] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeAclivity(this.FittingCounts[n][0]);

                #endregion
            }

            for (int n = 0; n < 7; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(7);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Sigma
        ///[1] Angle
        ///[2] Intensity
        ///[3] Background Constant
        ///[4] BackgroundCenter
        ///[5] Background aclivity
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorSigmaAngleIntensityBackground(double Lambda)

        {
            //[0][0] Sigma
            //[1][1] Angle
            //[2][2] Intensity
            //[3][3] Background Constant
            //[4][4] BackgroundCenter
            //[5][5] Background aclivity
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(6, 6, 0.0);

            //[0] Sigma
            //[1] Angle
            //[2] Intensity
            //[3] Background Constant
            //[4] BackgroundCenter
            //[5] Background aclivity
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(6);

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeSigma(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeAngle(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeSigma(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeSigma(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[3, 3] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[4, 4] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[5, 5] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 5] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[5, 0] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 5] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[5, 1] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 5] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[5, 2] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[5, 3] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 5] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[5, 4] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 5] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                #endregion

                #region Vector build

                SolutionVector[0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeSigma(this.FittingCounts[n][0]);
                SolutionVector[1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeAngle(this.FittingCounts[n][0]);
                SolutionVector[2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]);
                SolutionVector[3] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeConstant(this.FittingCounts[n][0]);
                SolutionVector[4] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeCenter(this.FittingCounts[n][0]);
                SolutionVector[5] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeAclivity(this.FittingCounts[n][0]);

                #endregion
            }

            for (int n = 0; n < 6; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(6);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Sigma
        ///[1] Angle
        ///[2] LorentzRatio
        ///[3] Background Constant
        ///[4] Background Center
        ///[5] Background aclivity
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorSigmaAngleLorentzRatioBackground(double Lambda)
        {
            //[0][0] Sigma
            //[1][1] Angle
            //[2][2] Lorentz Ratio
            //[3][3] Background Constant
            //[4][4] BackgroundCenter
            //[5][5] Background aclivity
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(6, 6, 0.0);

            //[0] Sigma
            //[1] Angle
            //[2] Lorentz Ratio
            //[3] Background Constant
            //[4] BackgroundCenter
            //[5] Background aclivity
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(6);

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeSigma(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeAngle(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeSigma(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeSigma(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[3, 3] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[4, 4] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[5, 5] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 5] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[5, 0] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 5] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[5, 1] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 5] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[5, 2] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[5, 3] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 5] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[5, 4] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 5] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                #endregion

                #region Vector build

                SolutionVector[0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeSigma(this.FittingCounts[n][0]);
                SolutionVector[1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeAngle(this.FittingCounts[n][0]);
                SolutionVector[2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]);
                SolutionVector[3] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeConstant(this.FittingCounts[n][0]);
                SolutionVector[4] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeCenter(this.FittingCounts[n][0]);
                SolutionVector[5] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeAclivity(this.FittingCounts[n][0]);

                #endregion
            }

            for (int n = 0; n < 6; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(6);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Sigma
        ///[1] LorentzRatio
        ///[2] Intensity
        ///[3] Background Constant
        ///[4] Background Center
        ///[5] Background aclivity
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorSigmaLorentzRatioIntensityBackground(double Lambda)
        {
            //[0][0] Sigma
            //[1][1] Lorentz Ratio
            //[2][2] Intensity
            //[3][3] Background Constant
            //[4][4] BackgroundCenter
            //[5][5] Background aclivity
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(6, 6, 0.0);

            //[0] Sigma
            //[1] Lorentz Ratio
            //[2] Intensity
            //[3] Background Constant
            //[4] BackgroundCenter
            //[5] Background aclivity
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(6);

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeSigma(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[3, 3] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[4, 4] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[5, 5] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 5] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[5, 0] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 5] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[5, 1] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[5, 2] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 5] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[5, 3] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 5] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[5, 4] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 5] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                #endregion

                #region Vector build

                SolutionVector[0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeSigma(this.FittingCounts[n][0]);
                SolutionVector[1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]);
                SolutionVector[2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]);
                SolutionVector[3] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeConstant(this.FittingCounts[n][0]);
                SolutionVector[4] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeCenter(this.FittingCounts[n][0]);
                SolutionVector[5] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeAclivity(this.FittingCounts[n][0]);

                #endregion
            }

            for (int n = 0; n < 6; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(6);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Angle
        ///[1] LorentzRatio
        ///[2] Intensity
        ///[3] Background Constant
        ///[4] Background Center
        ///[5] Background aclivity
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorAngleLorentzRatioIntensityBackground(double Lambda)
        {
            //[0][0] Sigma
            //[1][1] Background Constant
            //[2][2] BackgroundCenter
            //[3][3] Background aclivity
            //[4][4] Lorentz Ratio
            //[5][5] Intensity
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(6, 6, 0.0);

            //[0] Sigma
            //[1] Background Constant
            //[2] BackgroundCenter
            //[3] Background aclivity
            //[4] Lorentz Ratio
            //[5] Intensity
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(6);

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeAngle(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[3, 3] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[4, 4] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[5, 5] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 5] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[5, 0] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 5] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[5, 1] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[5, 2] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 5] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[5, 3] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 5] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[5, 4] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 5] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                #endregion

                #region Vector build

                SolutionVector[0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeAngle(this.FittingCounts[n][0]);
                SolutionVector[1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]);
                SolutionVector[2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]);
                SolutionVector[3] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeConstant(this.FittingCounts[n][0]);
                SolutionVector[4] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeCenter(this.FittingCounts[n][0]);
                SolutionVector[5] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeAclivity(this.FittingCounts[n][0]);

                #endregion
            }

            for (int n = 0; n < 6; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(6);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Sigma
        ///[1] Intensity
        ///[2] Background Constant
        ///[3] BackgroundCenter
        ///[4] Background aclivity
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorSigmaIntensityBackground(double Lambda)

        {
            //[0][0] Sigma
            //[1][1] Intensity
            //[2][2] Background Constant
            //[3][3] BackgroundCenter
            //[4][4] Background aclivity
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(5, 5, 0.0);

            //[0] Sigma
            //[1] Intensity
            //[2] Background Constant
            //[3] BackgroundCenter
            //[4] Background aclivity
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(5);

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeSigma(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[3, 3] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[4, 4] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                #endregion

                #region Vector build

                SolutionVector[0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeSigma(this.FittingCounts[n][0]);
                SolutionVector[1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]);
                SolutionVector[2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeConstant(this.FittingCounts[n][0]);
                SolutionVector[3] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeCenter(this.FittingCounts[n][0]);
                SolutionVector[4] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeAclivity(this.FittingCounts[n][0]);

                #endregion
            }

            for (int n = 0; n < 5; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(5);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Angle
        ///[1] Intensity
        ///[2] Background Constant
        ///[3] BackgroundCenter
        ///[4] Background aclivity
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorAngleIntensityBackground(double Lambda)

        {
            //[0][0] Angle
            //[1][1] Intensity
            //[2][2] Background Constant
            //[3][3] BackgroundCenter
            //[4][4] Background aclivity
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(5, 5, 0.0);

            //[0] Angle
            //[1] Intensity
            //[2] Background Constant
            //[3] BackgroundCenter
            //[4] Background aclivity
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(5);

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeAngle(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[3, 3] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[4, 4] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                #endregion

                #region Vector build

                SolutionVector[0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeAngle(this.FittingCounts[n][0]);
                SolutionVector[1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]);
                SolutionVector[2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeConstant(this.FittingCounts[n][0]);
                SolutionVector[3] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeCenter(this.FittingCounts[n][0]);
                SolutionVector[4] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeAclivity(this.FittingCounts[n][0]);

                #endregion
            }

            for (int n = 0; n < 5; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(5);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Sigma
        ///[1] Angle
        ///[2] Background Constant
        ///[3] Background Center
        ///[4] Background aclivity
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorSigmaAngleBackground(double Lambda)
        {
            //[0][0] Sigma
            //[1][1] Angle
            //[2][2] Background Constant
            //[3][3] BackgroundCenter
            //[4][4] Background aclivity
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(5, 5, 0.0);

            //[0] Sigma
            //[1] Angle
            //[2] Background Constant
            //[3] BackgroundCenter
            //[4] Background aclivity
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(5);

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeSigma(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeAngle(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeAngle(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeAngle(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[3, 3] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[4, 4] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                #endregion

                #region Vector build

                SolutionVector[0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeSigma(this.FittingCounts[n][0]);
                SolutionVector[1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeAngle(this.FittingCounts[n][0]);
                SolutionVector[2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeConstant(this.FittingCounts[n][0]);
                SolutionVector[3] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeCenter(this.FittingCounts[n][0]);
                SolutionVector[4] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeAclivity(this.FittingCounts[n][0]);

                #endregion
            }

            for (int n = 0; n < 5; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(5);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Sigma
        ///[1] LorentzRatio
        ///[2] Background Constant
        ///[3] Background Center
        ///[4] Background aclivity
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorSigmaLorentzRatioBackground(double Lambda)
        {
            //[0][0] Sigma
            //[1][1] Lorentz Ratio
            //[2][2] Background Constant
            //[3][3] BackgroundCenter
            //[4][4] Background aclivity
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(5, 5, 0.0);

            //[0] Sigma
            //[1] Lorentz Ratio
            //[2] Background Constant
            //[3] BackgroundCenter
            //[4] Background aclivity
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(5);

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeSigma(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                
                HessianMatrix[2, 2] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[3, 3] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[4, 4] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                #endregion

                #region Vector build

                SolutionVector[0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeSigma(this.FittingCounts[n][0]);
                SolutionVector[1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]);
                SolutionVector[2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeConstant(this.FittingCounts[n][0]);
                SolutionVector[3] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeCenter(this.FittingCounts[n][0]);
                SolutionVector[4] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeAclivity(this.FittingCounts[n][0]);

                #endregion
            }

            for (int n = 0; n < 5; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(5);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Angle
        ///[1] LorentzRatio
        ///[2] Background Constant
        ///[3] Background Center
        ///[4] Background aclivity
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorAngleLorentzRatioBackground(double Lambda)
        {
            //[0][0] Angle
            //[1][1] Lorentz Ratio
            //[2][2] Background Constant
            //[3][3] BackgroundCenter
            //[4][4] Background aclivity
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(5, 5, 0.0);

            //[0] Angle
            //[1] Lorentz Ratio
            //[2] Background Constant
            //[3] BackgroundCenter
            //[4] Background aclivity
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(5);

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeAngle(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[3, 3] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[4, 4] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                #endregion

                #region Vector build

                SolutionVector[0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeAngle(this.FittingCounts[n][0]);
                SolutionVector[1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]);
                SolutionVector[2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeConstant(this.FittingCounts[n][0]);
                SolutionVector[3] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeCenter(this.FittingCounts[n][0]);
                SolutionVector[4] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeAclivity(this.FittingCounts[n][0]);

                #endregion
            }

            for (int n = 0; n < 5; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(5);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Intensity
        ///[1] LorentzRatio
        ///[2] Background Constant
        ///[3] Background Center
        ///[4] Background aclivity
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorIntensityLorentzRatioBackground(double Lambda)
        {
            //[0][0] Intensity
            //[1][1] Lorentz Ratio
            //[2][2] Background Constant
            //[3][3] BackgroundCenter
            //[4][4] Background aclivity
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(5, 5, 0.0);

            //[0] Intensity
            //[1] Lorentz Ratio
            //[2] Background Constant
            //[3] BackgroundCenter
            //[4] Background aclivity
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(5);

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[3, 3] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[4, 4] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                #endregion

                #region Vector build

                SolutionVector[0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]);
                SolutionVector[1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]);
                SolutionVector[2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeConstant(this.FittingCounts[n][0]);
                SolutionVector[3] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeCenter(this.FittingCounts[n][0]);
                SolutionVector[4] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeAclivity(this.FittingCounts[n][0]);

                #endregion
            }

            for (int n = 0; n < 5; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(5);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Intensity
        ///[1] Background Constant
        ///[2] Background Center
        ///[3] Background aclivity
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorIntensityBackground(double Lambda)
        {
            //[0][0] Intensity
            //[1][1] Background Constant
            //[2][2] BackgroundCenter
            //[3][3] Background aclivity
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(4, 4, 0.0);

            //[0] Intensity
            //[1] Background Constant
            //[2] BackgroundCenter
            //[3] Background aclivity
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(4);

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[3, 3] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                #endregion

                #region Vector build

                SolutionVector[0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]);
                SolutionVector[1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeConstant(this.FittingCounts[n][0]);
                SolutionVector[2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeCenter(this.FittingCounts[n][0]);
                SolutionVector[3] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeAclivity(this.FittingCounts[n][0]);

                #endregion
            }

            for (int n = 0; n < 4; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(4);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] LorentzRatio
        ///[1] Background Constant
        ///[2] Background Center
        ///[3] Background aclivity
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorLorentzRatioBackground(double Lambda)
        {
            //[0][0] LorentzRatio
            //[1][1] Background Constant
            //[2][2] BackgroundCenter
            //[3][3] Background aclivity
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(4, 4, 0.0);

            //[0] LorentzRatio
            //[1] Background Constant
            //[2] BackgroundCenter
            //[3] Background aclivity
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(4);

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[3, 3] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                #endregion

                #region Vector build

                SolutionVector[0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]);
                SolutionVector[1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeConstant(this.FittingCounts[n][0]);
                SolutionVector[2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeCenter(this.FittingCounts[n][0]);
                SolutionVector[3] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeAclivity(this.FittingCounts[n][0]);

                #endregion
            }

            for (int n = 0; n < 4; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(4);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Sigma
        ///[1] Background Constant
        ///[2] Background Center
        ///[3] Background aclivity
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorSigmaBackground(double Lambda)
        {
            //[0][0] Sigma
            //[2][2] Background Constant
            //[3][3] BackgroundCenter
            //[4][4] Background aclivity
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(4, 4, 0.0);

            //[0] Sigma
            //[2] Background Constant
            //[3] BackgroundCenter
            //[4] Background aclivity
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(4);

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeSigma(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[3, 3] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                #endregion

                #region Vector build

                SolutionVector[0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeSigma(this.FittingCounts[n][0]);
                SolutionVector[1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeConstant(this.FittingCounts[n][0]);
                SolutionVector[2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeCenter(this.FittingCounts[n][0]);
                SolutionVector[3] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeAclivity(this.FittingCounts[n][0]);

                #endregion
            }

            for (int n = 0; n < 4; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(4);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Angle
        ///[1] Background Constant
        ///[2] Background Center
        ///[3] Background aclivity
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorAngleBackground(double Lambda)
        {
            //[0][0] Angle
            //[1][1] Background Constant
            //[2][2] BackgroundCenter
            //[3][3] Background aclivity
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(4, 4, 0.0);

            //[0] Angle
            //[1] Background Constant
            //[2] BackgroundCenter
            //[3] Background aclivity
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(4);

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeAngle(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[3, 3] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                #endregion

                #region Vector build

                SolutionVector[0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeAngle(this.FittingCounts[n][0]);
                SolutionVector[1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeConstant(this.FittingCounts[n][0]);
                SolutionVector[2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeCenter(this.FittingCounts[n][0]);
                SolutionVector[3] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeAclivity(this.FittingCounts[n][0]);

                #endregion
            }

            for (int n = 0; n < 4; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(4);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }
        
        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Background Constant
        ///[1] Background Center
        ///[2] Background aclivity
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorBackground(double Lambda)
        {
            //[0][0] Background Constant
            //[1][1] BackgroundCenter
            //[2][2] Background aclivity
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
            
            //[0] Background Constant
            //[1] BackgroundCenter
            //[2] Background aclivity
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                #endregion

                #region Vector build
                
                SolutionVector[0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeConstant(this.FittingCounts[n][0]);
                SolutionVector[1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeCenter(this.FittingCounts[n][0]);
                SolutionVector[2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeAclivity(this.FittingCounts[n][0]);

                #endregion
            }

            for (int n = 0; n < 3; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }


        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Sigma
        ///[1] Angle
        ///[2] Intensity
        ///[3] LorentzRatio
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorSigmaAngleIntensityLorentzRatio(double Lambda)
        {
            //[0][0] Sigma
            //[1][1] Angle
            //[2][2] Intensity
            //[3][3] LorentzRatio
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(4, 4, 0.0);

            //[0] Sigma
            //[1] Angle
            //[2] Intensity
            //[3] LorentzRatio
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(4);

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeSigma(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeAngle(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeSigma(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeSigma(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[3, 3] += (this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                #endregion

                #region Vector build

                SolutionVector[0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeSigma(this.FittingCounts[n][0]);
                SolutionVector[1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeAngle(this.FittingCounts[n][0]);
                SolutionVector[2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]);
                SolutionVector[3] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]);

                #endregion
            }

            for (int n = 0; n < 4; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(4);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Sigma
        ///[1] Angle
        ///[2] LorentzRatio
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorSigmaAngleLorentzRatio(double Lambda)
        {
            //[0][0] Sigma
            //[1][1] Angle
            //[2][2] LorentzRatio
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

            //[0] Sigma
            //[1] Angle
            //[2] LorentzRatio
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeSigma(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeAngle(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeSigma(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeSigma(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                #endregion

                #region Vector build

                SolutionVector[0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeSigma(this.FittingCounts[n][0]);
                SolutionVector[1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeAngle(this.FittingCounts[n][0]);
                SolutionVector[2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]);

                #endregion
            }

            for (int n = 0; n < 3; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Sigma
        ///[1] Angle
        ///[2] Intensity
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorSigmaAngleIntensity(double Lambda)
        {
            //[0][0] Sigma
            //[1][1] Angle
            //[2][2] Intensity
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

            //[0] Sigma
            //[1] Angle
            //[2] Intensity
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeSigma(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeAngle(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeSigma(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeSigma(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                #endregion

                #region Vector build

                SolutionVector[0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeSigma(this.FittingCounts[n][0]);
                SolutionVector[1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeAngle(this.FittingCounts[n][0]);
                SolutionVector[2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]);

                #endregion
            }

            for (int n = 0; n < 3; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Sigma
        ///[1] LorentzRatio
        ///[2] Intensity
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorSigmaLorentzRatioIntensity(double Lambda)
        {
            //[0][0] Sigma
            //[1][1] LorentzRatio
            //[2][2] Intensity
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

            //[0] Sigma
            //[1] LorentzRatio
            //[2] Intensity
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeSigma(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeSigma(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeSigma(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                #endregion

                #region Vector build

                SolutionVector[0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeSigma(this.FittingCounts[n][0]);
                SolutionVector[1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]);
                SolutionVector[2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]);

                #endregion
            }

            for (int n = 0; n < 3; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Angle
        ///[1] LorentzRatio
        ///[2] Intensity
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorAngleLorentzRatioIntensity(double Lambda)
        {
            //[0][0] Angle
            //[1][1] LorentzRatio
            //[2][2] Intensity
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

            //[0] Angle
            //[1] LorentzRatio
            //[2] Intensity
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeAngle(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeAngle(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeAngle(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                #endregion

                #region Vector build

                SolutionVector[0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeAngle(this.FittingCounts[n][0]);
                SolutionVector[1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]);
                SolutionVector[2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]);

                #endregion
            }

            for (int n = 0; n < 3; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Sigma
        ///[1] Angle
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorSigmaAngle(double Lambda)
        {
            //[0][0] Sigma
            //[1][1] Angle
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(2, 2, 0.0);

            //[0] Sigma
            //[1] Angle
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(2);

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeSigma(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeAngle(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeSigma(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeSigma(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                #endregion

                #region Vector build

                SolutionVector[0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeSigma(this.FittingCounts[n][0]);
                SolutionVector[1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeAngle(this.FittingCounts[n][0]);

                #endregion
            }

            for (int n = 0; n < 2; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(2);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Sigma
        ///[1] Intensity
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorSigmaIntensity(double Lambda)
        {
            //[0][0] Sigma
            //[1][1] Intensity
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(2, 2, 0.0);

            //[0] Sigma
            //[1] Intensity
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(2);

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeSigma(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeSigma(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeSigma(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                #endregion

                #region Vector build

                SolutionVector[0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeSigma(this.FittingCounts[n][0]);
                SolutionVector[1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]);

                #endregion
            }

            for (int n = 0; n < 2; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(2);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Angle
        ///[1] Intensity
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorAngleIntensity(double Lambda)
        {
            //[0][0] Angle
            //[1][1] Intensity
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(2, 2, 0.0);

            //[0] Angle
            //[1] Intensity
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(2);

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeAngle(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeAngle(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeAngle(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                #endregion

                #region Vector build

                SolutionVector[0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeAngle(this.FittingCounts[n][0]);
                SolutionVector[1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]);

                #endregion
            }

            for (int n = 0; n < 2; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(2);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Sigma
        ///[1] LorentzRatio
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorSigmaLorentzRatio(double Lambda)
        {
            //[0][0] Sigma
            //[1][1] LorentzRatio
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(2, 2, 0.0);

            //[0] Sigma
            //[1] LorentzRatio
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(2);

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeSigma(this.FittingCounts[n][0]) * this.FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeSigma(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeSigma(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                #endregion

                #region Vector build

                SolutionVector[0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeSigma(this.FittingCounts[n][0]);
                SolutionVector[1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]);

                #endregion
            }

            for (int n = 0; n < 2; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(2);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Angle
        ///[1] LorentzRatio
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorAngleLorentzRatio(double Lambda)
        {
            //[0][0] Angle
            //[1][1] LorentzRatio
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(2, 2, 0.0);

            //[0] Angle
            //[1] LorentzRatio
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(2);

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeAngle(this.FittingCounts[n][0]) * this.FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeAngle(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeAngle(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                #endregion

                #region Vector build

                SolutionVector[0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeAngle(this.FittingCounts[n][0]);
                SolutionVector[1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]);

                #endregion
            }

            for (int n = 0; n < 2; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(2);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Intensity
        ///[1] LorentzRatio
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorIntensityLorentzRatio(double Lambda)
        {
            //[0][0] Intensity
            //[1][1] LorentzRatio
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(2, 2, 0.0);

            //[0] Intensity
            //[1] LorentzRatio
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(2);

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                #endregion

                #region Vector build

                SolutionVector[0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeIntensity(this.FittingCounts[n][0]);
                SolutionVector[1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.FirstDerivativeLorentzRatio(this.FittingCounts[n][0]);

                #endregion
            }

            for (int n = 0; n < 2; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(2);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }


        #endregion

        #endregion

        #region Fitting using Multi Threading

        public event System.ComponentModel.PropertyChangedEventHandler FitFinished;
        public event System.ComponentModel.PropertyChangedEventHandler FitStarted;

        protected void OnFitStarted()
        {
            this._fitCompleted = false;
            this._fitConverged = false;

            //FitStarted?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs("FitStarted"));
            System.ComponentModel.PropertyChangedEventHandler handler = FitStarted;
            if (handler != null)
            {
                handler(this, new System.ComponentModel.PropertyChangedEventArgs("FitStarted"));
            }
        }

        protected void OnFitFinished()
        {
            this._fitCompleted = true;

            SetFittingErrors();
            this._doneEvent.Set();

            //FitFinished?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs("FitFinished"));
            System.ComponentModel.PropertyChangedEventHandler handler = FitFinished;
            if (handler != null)
            {
                handler(this, new System.ComponentModel.PropertyChangedEventArgs("FitFinished"));
            }
        }

        // Wrapper method for use with thread pool. 
        public void FitPeakCallback(Object threadContext)
        {
            OnFitStarted();

            //Fit Zeug kommt hier hin
            if(this.startingLambda < 0.0)
            {
                this._fitConverged = Analysis.Fitting.LMA.FitPeakFunction(this);
            }
            else
            {
                this._fitConverged = Analysis.Fitting.LMA.FitPeakFunction(this, this.startingLambda);
            }

            OnFitFinished();
        }

        public void SetResetEvent(System.Threading.ManualResetEvent DoneEvent)
        {
            this._doneEvent = DoneEvent;
        }

        private void SetFittingErrors()
        {

        }

        #endregion

        #region Cloning

        public object Clone()
        {
            PeakFunction Ret = new PeakFunction(this._peakId);
            bool StateBackgroundSwitch = this.backgroundSwitch;
            int StateFunctionType = this.functionType;

            Ret._intensityGaussian = this._intensityGaussian;
            Ret._intensityLorentz = this._intensityLorentz;
            Ret._intensityPseudoVoigt = this._intensityPseudoVoigt;

            Ret.ParameterConstraints = this.ParameterConstraints.Clone() as Constraints;
            Ret.FittingCounts = this.FittingCounts.Clone() as Pattern.Counts;

            Ret.backgroundSwitch = true;
            this.backgroundSwitch = true;

            Ret.PolynomialBackgroundFunction = new BackgroundPolynomial(this.PolynomialBackgroundFunction.Center, this.PolynomialBackgroundFunction.Constant, this.PolynomialBackgroundFunction.Aclivity);

            Ret.functionType = 0;
            this.functionType = 0;

            Ret.GaussianFunction = new Gaussian(this.Angle, this.FWHM);

            Ret.functionType = 1;
            this.functionType = 1;

            Ret.LorentzFunction = new Lorentz(this.Angle, this.FWHM);

            Ret.functionType = 2;
            this.functionType = 2;

            Ret.PseudoVoigtFunction = new PseudoVoigt(this.Angle, this.FWHM, this.LorentzRatio);

            Ret.functionType = StateFunctionType;
            this.functionType = StateFunctionType;
            Ret.backgroundSwitch = StateBackgroundSwitch;
            this.backgroundSwitch = StateBackgroundSwitch;

            Ret._doneEvent = this._doneEvent;
            Ret.startingLambda = this.startingLambda;
            Ret._fitConverged = this._fitConverged;
            Ret._fitCompleted = this._fitCompleted;

            Ret.ParameterConstraints = this.ParameterConstraints.Clone() as Constraints;

            return Ret;
        }

        #endregion
    }
}
