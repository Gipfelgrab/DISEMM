using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.Analysis.Peaks.Functions
{
    [Serializable]
    public class BackgroundPolynomial
    {

        #region Parameters

        private double _constant;
        public double Constant
        {
            get
            {
                return this._constant;
            }
            set
            {
                this._constant = value;
            }
        }

        private double _aclivity;
        public double Aclivity
        {
            get
            {
                return this._aclivity;
            }
            set
            {
                this._aclivity = value;
            }
        }

        private double _center;
        public double Center
        {
            get
            {
                return this._center;
            }
            set
            {
                this._center = value;
            }
        }

        #endregion

        public BackgroundPolynomial(double angle)
        {
            this._constant = 100.0;
            this._aclivity = 0.0;
            //this._center = angle;
        }

        public BackgroundPolynomial(double angle, double backgroundHeight)
        {
            this._constant = backgroundHeight;
            this._aclivity = 0.0;
            //this._center = angle;
        }

        public BackgroundPolynomial(double angle, double backgroundHeight, double aclivity)
        {
            this._constant = backgroundHeight;
            this._aclivity = aclivity;
            this._center = angle;
        }

        #region Calculation

        public double Y(double theta)
        {
            //double Ret = this._constant;
            //Ret += this._aclivity * Math.Pow(theta - _center, 2);

            //return Ret;

            double Ret = this._constant;
            Ret += this._center * theta;
            Ret += this._aclivity * Math.Pow(theta, 2);

            return Ret;
        }

        #region FirstDerivative

        public double FirstDerivativeConstant(double theta)
        {
            return 1.0;
        }

        public double FirstDerivativeCenter(double theta)
        {
            //double Ret = -2.0 * this._aclivity;
            //Ret *= theta - this._center;

            //return Ret;

            return theta;
        }

        public double FirstDerivativeAclivity(double theta)
        {
            //return Math.Pow(theta - this._center, 2);
            return Math.Pow(theta, 2);
        }

        #endregion

        #region Second Derivatives

        public double SecondDerivativeConstantConstant(double theta)
        {
            return 0.0;
        }

        public double SecondDerivativeConstantAclivity(double theta)
        {
            return 0.0;
        }

        public double SecondDerivativeConstantCenter(double theta)
        {
            return 0.0;
        }

        public double SecondDerivativeCenterCenter(double theta)
        {
            double Ret = 2.0 * this._aclivity;

            return Ret;
        }

        public double SecondDerivativeCenterAclicity(double theta)
        {
            double Ret = -2.0;
            Ret *= theta - this._center;

            return Ret;
        }

        public double SecondDerivativeAclicityAclivity(double theta)
        {
            return 0.0;
        }

        #endregion

        #endregion
    }
}
