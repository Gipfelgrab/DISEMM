using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.Analysis.Fitting
{
    public static class ShearModulusPolynome
    {
        #region Parameters

        #endregion

        #region Calculation

        #region algorythm

        public static double HalleyIteration(double stepValue, double alpha, double beta, double gamma)
        {
            double Ret = stepValue;

            double HZ = ShearModulusPolynome.Y(alpha, beta, gamma, stepValue) * ShearModulusPolynome.FirstDerivativeShearModulus(alpha, beta, stepValue);
            double HN = Math.Pow(ShearModulusPolynome.FirstDerivativeShearModulus(alpha, beta, stepValue), 2);
            HN += -0.5 * ShearModulusPolynome.Y(alpha, beta, gamma, stepValue) * ShearModulusPolynome.SecondDerivativeShearModulus(alpha, stepValue);

            Ret -= HZ / HN;

            return Ret;
        }

        public static double[] GetLastZerosPolyDevision(double alpha, double beta, double FirstZero)
        {
            double[] Ret = { 0.0, 0.0 };

            double B1 = GetB1(alpha, FirstZero);
            double B0 = GetB0(beta, B1, FirstZero);

            double Disk = Math.Pow(B1 / 2.0, 2);
            Disk -= B0;

            if(Math.Abs(Disk) < 0.00001)
            {
                Ret[0] = -0.5 * B1;
                Ret[1] = -0.5 * B1;
            }
            else if(Disk > 0.0)
            {
                Ret[0] = -0.5 * B1;
                Ret[1] = -0.5 * B1;

                Ret[0] += Math.Sqrt(Disk);
                Ret[1] -= Math.Sqrt(Disk);
            }

            return Ret;
        }

        private static double GetB1(double alpha, double firstZero)
        {
            double Ret = firstZero - alpha;

            return Ret;
        }

        private static double GetB0(double beta, double b1, double firstZero)
        {
            double Ret = b1 * firstZero;
            Ret -= beta;

            return Ret;
        }

        #endregion

        public static double Y(double alpha, double beta, double gamma, double shearModulus)
        {
            double Ret = Math.Pow(shearModulus, 3);
            Ret -= alpha * Math.Pow(shearModulus, 2);
            Ret -= beta * shearModulus;
            Ret -= gamma;

            return Ret;
        }

        #region FirstDerivatives

        public static double FirstDerivativeShearModulus(double alpha, double beta, double shearModulus)
        {
            double Ret = 3.0 * Math.Pow(shearModulus, 2);
            Ret -= 2.0 * alpha * shearModulus;
            Ret -= beta;

            return Ret;
        }

        public static double FirstDerivativeAlpha(double shearModulus)
        {
            double Ret = -1.0 * Math.Pow(shearModulus, 2);

            return Ret;
        }

        public static double FirstDerivativeBeta(double shearModulus)
        {
            double Ret = -1.0 * shearModulus;

            return Ret;
        }

        public static double FirstDerivativeGamma(double shearModulus)
        {
            double Ret = -1.0;

            return Ret;
        }

        #endregion

        #region SecondDerivatives

        public static double SecondDerivativeShearModulus(double alpha, double shearModulus)
        {
            double Ret = 6.0 * shearModulus;
            Ret -= 2.0 * alpha;

            return Ret;
        }

        #endregion

        #endregion
    }
}
