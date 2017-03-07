using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.Analysis.Peaks.Functions
{
    [Serializable]
    public class Gaussian
    {
        #region Parameters

        public double normFactor = 2.356820045;

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

        public Gaussian(double angle)
        {
            this._angle = angle;
            this.FWHM = Tools.Calculation.GetEstimatedFWHM(angle);
        }

        public Gaussian(double angle, double fWHM)
        {
            this._angle = angle;
            this.FWHM = fWHM;
        }

        #region Calculation

        public double Y(double theta)
        {
            double Ret = 1;
            Ret /= (this._sigma * Math.Sqrt(2 * Math.PI));

            double Exp = Math.Pow((theta - this._angle), 2);
            Exp /= (2.0 * Math.Pow(this._sigma, 2));

            Ret *= Math.Exp(-1 * Exp);

            return Ret;
        }

        #region FirstDerivative

        public double FirstDerivativeAngle(double theta)
        {
            double Ret = this.Y(theta);
            Ret /= Math.Pow(this._sigma, 2);
            Ret *= (theta - this._angle);

            return Ret;
        }

        public double FirstDerivativeSigma(double theta)
        {
            double Ret = this.Y(theta);

            double Sum1 = -1.0;
            Sum1 /= this._sigma;

            double Sum2 = Math.Pow(theta - this._angle, 2);
            Sum2 /= Math.Pow(this._sigma, 3);

            Ret *= (Sum1 + Sum2);

            return Ret;
        }

        #endregion

        #region Secoond Derivative

        public double SecondDerivativeAngleAngle(double theta)
        {
            double Ret = this.Y(theta);
            
            double Sum1 = -1;
            Sum1 /= Math.Pow(this._sigma, 2);

            double Sum2 = Math.Pow(theta - this._angle, 2);
            Sum2 /= Math.Pow(this._sigma, 4);

            Ret *= (Sum1 + Sum2);

            return Ret;
        }

        public double SecondDerivativeAngleSigma(double theta)
        {
            double Ret = this.Y(theta);
            
            double Sum1 = -3 * ( theta - this._angle );
            Sum1 /= Math.Pow(this._sigma, 3);

            double Sum2 = Math.Pow(theta - this._angle, 3);
            Sum2 /= Math.Pow(this._sigma, 5);

            Ret *= (Sum1 + Sum2);

            return Ret;
        }

        public double SecondDerivativeSigmaSigma(double theta)
        {
            double Ret = this.Y(theta);
            
            double Sum1 = 2;
            Sum1 /= Math.Pow(this._sigma, 2);

            double Sum2 = -4 * Math.Pow(theta - this._angle, 2);
            Sum2 /= Math.Pow(this._sigma, 4);

            double Sum3 = -1 * Math.Pow(theta - this._angle, 2);
            Sum3 /= Math.Pow(this._sigma, 4);

            double Sum4 = Math.Pow(theta - this._angle, 4);
            Sum4 /= Math.Pow(this._sigma, 6);

            Ret *= (Sum1 + Sum2 + Sum3 + Sum4);

            return Ret;
        }

        #endregion

        #endregion

    }
}
