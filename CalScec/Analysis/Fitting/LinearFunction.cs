using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.Analysis.Fitting
{
    [Serializable]
    public class LinearFunction : ICloneable
    {
        #region Parameters

        public double ConstantError
        {
            get
            {
                MathNet.Numerics.LinearAlgebra.Matrix<double> CovMatrix = _hessianMatrix.Inverse();
                return Math.Sqrt(CovMatrix[1, 1]);
            }
        }
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

        public double AclivityError
        {
            get
            {
                MathNet.Numerics.LinearAlgebra.Matrix<double> CovMatrix = _hessianMatrix.Inverse();
                return Math.Sqrt(CovMatrix[0, 0]);
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

        public MathNet.Numerics.LinearAlgebra.Matrix<double> _hessianMatrix;

        #endregion

        public LinearFunction()
        {
            this._constant = 1;
            this._aclivity = 0;

            this._hessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(2, 2, 0.0);
        }

        public LinearFunction(double constant)
        {
            this._constant = constant;
            this._aclivity = 0;

            this._hessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(2, 2, 0.0);
        }

        public LinearFunction(double constant, double aclivity)
        {
            this._constant = constant;
            this._aclivity = aclivity;

            this._hessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(2, 2, 0.0);
        }

        #region Calculation

        public double Y(double X)
        {
            return (this._aclivity * X) + this._constant;
        }

        #region First derivative

        public double FirstDerivativeConstant(double X)
        {
            return 1;
        }

        public double FirstDerivativeAclivity(double X)
        {
            return X;
        }

        #endregion

        #region Second derivative

        public double SecondDerivativeConstantConstant(double X)
        {
            return 0;
        }

        public double SecondDerivativeConstantAclivity(double X)
        {
            return 0;
        }

        public double SecondDerivativeAclivityAclivity(double X)
        {
            return 0;
        }

        #endregion

        #region Fitting

        #region Reduced hessian

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Aclivity
        ///[1] Constant
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorAclivityConstant(double Lambda, Pattern.Counts UsedCounts)
        {
            //[0][0] Aclivity
            //[1][1] Constant
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(2, 2, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(2);

            for (int n = 0; n < UsedCounts.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeAclivity(UsedCounts[n][0]) * this.FirstDerivativeAclivity(UsedCounts[n][0]) / Math.Pow(UsedCounts[n][2], 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeConstant(UsedCounts[n][0]) * this.FirstDerivativeConstant(UsedCounts[n][0]) / Math.Pow(UsedCounts[n][2], 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeConstant(UsedCounts[n][0]) * this.FirstDerivativeAclivity(UsedCounts[n][0]) / Math.Pow(UsedCounts[n][2], 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeConstant(UsedCounts[n][0]) * this.FirstDerivativeAclivity(UsedCounts[n][0]) / Math.Pow(UsedCounts[n][2], 2));
                
                #endregion

                #region Vector build

                SolutionVector[0] += ((UsedCounts[n][1] - this.Y(UsedCounts[n][0])) / Math.Pow(UsedCounts[n][2], 2)) * this.FirstDerivativeAclivity(UsedCounts[n][0]);
                SolutionVector[1] += ((UsedCounts[n][1] - this.Y(UsedCounts[n][0])) / Math.Pow(UsedCounts[n][2], 2)) * this.FirstDerivativeConstant(UsedCounts[n][0]);

                #endregion
            }

            this._hessianMatrix = HessianMatrix;

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
        ///[0] Aclivity
        ///[1] Constant
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorAclivityConstant(double Lambda, Pattern.Counts UsedCounts, List<double> weightings)
        {
            //[0][0] Aclivity
            //[1][1] Constant
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(2, 2, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(2);

            for (int n = 0; n < UsedCounts.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += weightings[n] * (this.FirstDerivativeAclivity(UsedCounts[n][0]) * this.FirstDerivativeAclivity(UsedCounts[n][0]) / Math.Pow(UsedCounts[n][2], 2));

                HessianMatrix[1, 1] += weightings[n] * (this.FirstDerivativeConstant(UsedCounts[n][0]) * this.FirstDerivativeConstant(UsedCounts[n][0]) / Math.Pow(UsedCounts[n][2], 2));
                HessianMatrix[0, 1] += weightings[n] * (this.FirstDerivativeConstant(UsedCounts[n][0]) * this.FirstDerivativeAclivity(UsedCounts[n][0]) / Math.Pow(UsedCounts[n][2], 2));
                HessianMatrix[1, 0] += weightings[n] * (this.FirstDerivativeConstant(UsedCounts[n][0]) * this.FirstDerivativeAclivity(UsedCounts[n][0]) / Math.Pow(UsedCounts[n][2], 2));

                #endregion

                #region Vector build

                SolutionVector[0] += weightings[n] * ((UsedCounts[n][1] - this.Y(UsedCounts[n][0])) / Math.Pow(UsedCounts[n][2], 2)) * this.FirstDerivativeAclivity(UsedCounts[n][0]);
                SolutionVector[1] += weightings[n] * ((UsedCounts[n][1] - this.Y(UsedCounts[n][0])) / Math.Pow(UsedCounts[n][2], 2)) * this.FirstDerivativeConstant(UsedCounts[n][0]);

                #endregion
            }

            this._hessianMatrix = HessianMatrix;

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

        #endregion

        #region Cloning

        public object Clone()
        {
            LinearFunction Ret = new LinearFunction(this.Constant, this.Aclivity);
            Ret._hessianMatrix = this._hessianMatrix.Clone();

            return Ret;
        }

        #endregion
    }
}
