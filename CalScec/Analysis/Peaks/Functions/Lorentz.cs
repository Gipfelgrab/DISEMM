using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.Analysis.Peaks.Functions
{
    [Serializable]
    public class Lorentz
    {
        #region Parameters

        private double normFactor = 1.0;

        private double _sigma;
        public double Sigma
        {
            get
            {
                return this._sigma;
            }
            set
            {
                this._sigma = value;
            }
        }
        public double FWHM
        {
            get
            {
                return this._sigma * this.normFactor;
            }
            set
            {
                this._sigma = value / this.normFactor;
            }
        }

        private double _angle;
        public double Angle
        {
            get
            {
                return this._angle;
            }
            set
            {
                this._angle = value;
            }
        }

        #endregion

        public Lorentz(double angle)
        {
            this._angle = angle;
            this.FWHM = Tools.Calculation.GetEstimatedFWHM(angle);
        }

        public Lorentz(double angle, double fWHM)
        {
            this._angle = angle;
            this.FWHM = fWHM;
        }

        #region Calculation

        public double Y(double theta)
        {
            double Ret = this._sigma;
            Ret /= 2 * Math.PI;

            double Sum1 = Math.Pow(theta - this._angle, 2);

            double Sum2 = Math.Pow(0.5 * this._sigma, 2);

            Ret /= (Sum1 + Sum2);

            return Ret;
        }

        #region FirstDerivative

        public double FirstDerivativeAngle(double theta)
        {
            double Ret = this.Y(theta);
            
            double MultiZ = 2 * (theta - this._angle);

            double MultiN = Math.Pow(0.5 * this._sigma, 2) + Math.Pow(theta - this._angle, 2);

            Ret *= MultiZ / MultiN;

            return Ret;
        }

        public double FirstDerivativeSigma(double theta)
        {
            double Ret = 1;
            Ret /= Math.Pow(0.5 * this._sigma, 2) + Math.Pow(theta - this._angle, 2);

            double Sum1 = -1;
            Sum1 *= 0.0795775 * Math.Pow(this._sigma, 2);
            Sum1 /= Math.Pow(Math.Pow(0.5 * this._sigma, 2) + Math.Pow(theta - this._angle, 2), 2);

            double Sum2 = 0.5 / Math.PI;

            Ret *= (Sum1 + Sum2);

            return Ret;
        }

        #endregion

        #region Secoond Derivative

        public double SecondDerivativeAngleAngle(double theta)
        {
            double Ret = this._sigma / Math.Pow(Math.Pow(0.5 * this._sigma, 2) + Math.Pow(theta - this._angle, 2), 2);
            Ret /= Math.PI;

            double Sum1 = 4 * Math.Pow(theta - this._angle, 2);
            Sum1 /= (Math.Pow(0.5 * this._sigma, 2) + Math.Pow(theta - this._angle, 2));

            double Sum2 = -1;

            Ret *= (Sum1 + Sum2);

            return Ret;
        }

        public double SecondDerivativeAngleSigma(double theta)
        {
            double Ret = theta - this._angle;
            Ret /= Math.Pow(Math.Pow(0.5 * this._sigma, 2) + Math.Pow(theta - this._angle, 2), 2);

            double Sum1 = -0.31831 * Math.Pow(this._sigma, 2);
            Sum1 /= Math.Pow(0.5 * this._sigma, 2) + Math.Pow(theta - this._angle, 2);

            double Sum2 = 1 / Math.PI;

            Ret *= (Sum1 + Sum2);

            return Ret;
        }

        public double SecondDerivativeSigmaSigma(double theta)
        {
            double Ret = this._sigma;
            Ret /= Math.Pow(Math.Pow(0.5 * this._sigma, 2) + Math.Pow(theta - this._angle, 2), 2);

            double Sum1 = 0.0795775 * Math.Pow(this._sigma, 2);
            Sum1 /= Math.Pow(0.5 * this._sigma, 2) + Math.Pow(theta - this._angle, 2);

            double Sum2 = -0.238732;

            Ret *= (Sum1 + Sum2);

            return Ret;
        }

        #endregion

        #endregion
    }
}
