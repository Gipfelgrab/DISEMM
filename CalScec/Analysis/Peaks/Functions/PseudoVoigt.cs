using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.Analysis.Peaks.Functions
{
    [Serializable]
    public class PseudoVoigt
    {

        #region Parameters

        private Gaussian GaussianPart;
        private Lorentz LorentzPart;

        public double Angle
        {
            get
            {
                return this.GaussianPart.Angle;
            }
            set
            {
                this.GaussianPart.Angle = value;
                this.LorentzPart.Angle = value;
            }
        }

        public double Sigma
        {
            get
            {
                return this.GaussianPart.Sigma;
            }
            set
            {
                this.GaussianPart.Sigma = value;
                this.LorentzPart.Sigma = value * this.GaussianPart.normFactor;
            }
        }
        public double FWHM
        {
            get
            {
                return this.GaussianPart.FWHM;
            }
            set
            {
                this.GaussianPart.FWHM = value;
                this.LorentzPart.Sigma = value;
            }
        }

        private double _lorentzRatio;
        public double LorentzRatio
        {
            get
            {
                return this._lorentzRatio;
            }
            set
            {
                this._lorentzRatio = value;
            }
        }

        #endregion

        public PseudoVoigt(double angle)
        {
            Gaussian GaussianTmp = new Gaussian(angle);
            Lorentz LorentzTmp = new Lorentz(angle);
            this.GaussianPart = GaussianTmp;
            this.LorentzPart = LorentzTmp;

            this._lorentzRatio = CalScec.Properties.Settings.Default.PseudoVoigtRatio;
        }

        public PseudoVoigt(double angle, double fWHM)
        {
            Gaussian GaussianTmp = new Gaussian(angle, fWHM);
            Lorentz LorentzTmp = new Lorentz(angle, fWHM);
            this.GaussianPart = GaussianTmp;
            this.LorentzPart = LorentzTmp;
            this._lorentzRatio = CalScec.Properties.Settings.Default.PseudoVoigtRatio;
        }

        public PseudoVoigt(double angle, double fWHM, double lorentzRatio)
        {
            Gaussian GaussianTmp = new Gaussian(angle, fWHM);
            Lorentz LorentzTmp = new Lorentz(angle, fWHM);
            this.GaussianPart = GaussianTmp;
            this.LorentzPart = LorentzTmp;
            this._lorentzRatio = lorentzRatio;
        }

        #region Calculation

        public double Y(double theta)
        {
            double Ret = this._lorentzRatio * this.GaussianPart.Y(theta);
            Ret += (1 - this._lorentzRatio) * this.LorentzPart.Y(theta);

            return Ret;
        }

        #region FirstDerivative

        public double FirstDerivativeAngle(double theta)
        {
            double Ret = this._lorentzRatio * this.GaussianPart.FirstDerivativeAngle(theta);
            Ret += (1 - this._lorentzRatio) * this.LorentzPart.FirstDerivativeAngle(theta);

            return Ret;
        }

        public double FirstDerivativeSigma(double theta)
        {
            double Ret = this._lorentzRatio * this.GaussianPart.FirstDerivativeSigma(theta);
            Ret += (1 - this._lorentzRatio) * this.LorentzPart.FirstDerivativeSigma(theta);

            return Ret;
        }

        public double FirstDerivativeLorentzRatio(double theta)
        {
            double Ret = this.GaussianPart.FirstDerivativeSigma(theta);
            Ret += this.LorentzPart.FirstDerivativeSigma(theta);

            return Ret;
        }

        #endregion

        #region Secoond Derivative

        public double SecondDerivativeAngleAngle(double theta)
        {
            double Ret = this._lorentzRatio * this.GaussianPart.SecondDerivativeAngleAngle(theta);
            Ret += (1 - this._lorentzRatio) * this.LorentzPart.SecondDerivativeAngleAngle(theta);

            return Ret;
        }

        public double SecondDerivativeAngleLorentzRatio(double theta)
        {
            double Ret = this.GaussianPart.FirstDerivativeAngle(theta);
            Ret += this.LorentzPart.FirstDerivativeAngle(theta);

            return Ret;
        }

        public double SecondDerivativeAngleSigma(double theta)
        {
            double Ret = this._lorentzRatio * this.GaussianPart.SecondDerivativeAngleSigma(theta);
            Ret += (1 - this._lorentzRatio) * this.LorentzPart.SecondDerivativeAngleSigma(theta);

            return Ret;
        }

        public double SecondDerivativeSigmaLorentzRatio(double theta)
        {
            double Ret = this.GaussianPart.FirstDerivativeSigma(theta);
            Ret += this.LorentzPart.FirstDerivativeSigma(theta);

            return Ret;
        }

        public double SecondDerivativeSigmaSigma(double theta)
        {
            double Ret = this._lorentzRatio * this.GaussianPart.SecondDerivativeSigmaSigma(theta);
            Ret += (1 - this._lorentzRatio) * this.LorentzPart.SecondDerivativeSigmaSigma(theta);

            return Ret;
        }

        public double SecondDerivativeLorentzRatioLorentzRatio(double theta)
        {
            double Ret = 0.0;

            return Ret;
        }

        #endregion

        #endregion
    }
}
