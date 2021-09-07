using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.Tools
{
    public class FourthRankTensor : ICloneable
    {
        private List<List<List<List<double>>>> _components = new List<List<List<List<double>>>>();

        public double this[int i, int j, int k, int l]
        {
            get
            {
                return this._components[i][j][k][l];
            }
            set
            {
                this._components[i][j][k][l] = value;
            }
        }

        private void _setComponents()
        {
            this._components.Clear();

            for (int m = 0; m < 3; m++)
            {
                List<List<List<double>>> Tmp1 = new List<List<List<double>>>();
                for (int n = 0; n < 3; n++)
                {
                    List<List<double>> Tmp2 = new List<List<double>>();
                    for (int o = 0; o < 3; o++)
                    {
                        List<double> Tmp3 = new List<double>();
                        for (int p = 0; p < 3; p++)
                        {
                            Tmp3.Add(0.0);
                        }
                        Tmp2.Add(Tmp3);
                    }
                    Tmp1.Add(Tmp2);
                }
                this._components.Add(Tmp1);
            }
        }

        public FourthRankTensor(MathNet.Numerics.LinearAlgebra.Matrix<double> VoigtTensor)
        {
            this._setComponents();

            if (VoigtTensor.RowCount == VoigtTensor.ColumnCount && VoigtTensor.RowCount == 6)
            {
                this[0, 0, 0, 0] = VoigtTensor[0, 0];

                this[1, 0, 0, 0] = VoigtTensor[5, 0];
                this[0, 1, 0, 0] = VoigtTensor[5, 0];
                this[0, 0, 1, 0] = VoigtTensor[0, 5];
                this[0, 0, 0, 1] = VoigtTensor[0, 5];

                this[1, 1, 0, 0] = VoigtTensor[1, 0];
                this[1, 0, 1, 0] = VoigtTensor[5, 5];
                this[1, 0, 0, 1] = VoigtTensor[5, 5];
                this[0, 1, 1, 0] = VoigtTensor[5, 5];
                this[0, 1, 0, 1] = VoigtTensor[5, 5];
                this[0, 0, 1, 1] = VoigtTensor[0, 1];

                this[1, 1, 1, 0] = VoigtTensor[1, 5];
                this[1, 1, 0, 1] = VoigtTensor[1, 5];
                this[1, 0, 1, 1] = VoigtTensor[5, 1];
                this[0, 1, 1, 1] = VoigtTensor[5, 1];

                this[1, 1, 1, 1] = VoigtTensor[1, 1];

                this[2, 0, 0, 0] = VoigtTensor[4, 0];
                this[0, 2, 0, 0] = VoigtTensor[4, 0];
                this[0, 0, 2, 0] = VoigtTensor[0, 4];
                this[0, 0, 0, 2] = VoigtTensor[0, 4];

                this[2, 2, 0, 0] = VoigtTensor[2, 0];
                this[2, 0, 2, 0] = VoigtTensor[4, 4];
                this[2, 0, 0, 2] = VoigtTensor[4, 4];
                this[0, 2, 2, 0] = VoigtTensor[4, 4];
                this[0, 2, 0, 2] = VoigtTensor[4, 4];
                this[0, 0, 2, 2] = VoigtTensor[0, 2];

                this[2, 2, 2, 0] = VoigtTensor[2, 4];
                this[2, 2, 0, 2] = VoigtTensor[2, 4];
                this[2, 0, 2, 2] = VoigtTensor[4, 2];
                this[0, 2, 2, 2] = VoigtTensor[4, 2];

                this[2, 1, 1, 1] = VoigtTensor[3, 1];
                this[1, 2, 1, 1] = VoigtTensor[3, 1];
                this[1, 1, 2, 1] = VoigtTensor[1, 3];
                this[1, 1, 1, 2] = VoigtTensor[1, 3];

                this[2, 2, 1, 1] = VoigtTensor[2, 1];
                this[2, 1, 2, 1] = VoigtTensor[3, 3];
                this[2, 1, 1, 2] = VoigtTensor[3, 3];
                this[1, 2, 2, 1] = VoigtTensor[3, 3];
                this[1, 2, 1, 2] = VoigtTensor[3, 3];
                this[1, 1, 2, 2] = VoigtTensor[1, 2];

                this[2, 2, 2, 1] = VoigtTensor[2, 3];
                this[2, 2, 1, 2] = VoigtTensor[2, 3];
                this[2, 1, 2, 2] = VoigtTensor[3, 2];
                this[1, 2, 2, 2] = VoigtTensor[3, 2];

                this[2, 2, 2, 2] = VoigtTensor[2, 2];

                this[0, 1, 2, 0] = VoigtTensor[5, 4];
                this[0, 2, 1, 0] = VoigtTensor[4, 5];
                this[1, 0, 2, 0] = VoigtTensor[5, 4];
                this[2, 0, 1, 0] = VoigtTensor[4, 5];
                this[1, 2, 0, 0] = VoigtTensor[3, 0];
                this[2, 1, 0, 0] = VoigtTensor[3, 0];

                this[0, 1, 0, 2] = VoigtTensor[5, 4];
                this[0, 2, 0, 1] = VoigtTensor[4, 5];
                this[1, 0, 0, 2] = VoigtTensor[5, 4];
                this[2, 0, 0, 1] = VoigtTensor[4, 5];
                this[0, 0, 1, 2] = VoigtTensor[0, 3];
                this[0, 0, 2, 1] = VoigtTensor[0, 3];

                this[1, 0, 2, 1] = VoigtTensor[5, 3];
                this[1, 2, 0, 1] = VoigtTensor[3, 5];
                this[0, 1, 2, 1] = VoigtTensor[5, 3];
                this[2, 1, 0, 1] = VoigtTensor[3, 5];
                this[0, 2, 1, 1] = VoigtTensor[4, 1];
                this[2, 0, 1, 1] = VoigtTensor[4, 1];

                this[1, 0, 1, 2] = VoigtTensor[5, 3];
                this[1, 2, 1, 0] = VoigtTensor[3, 5];
                this[0, 1, 1, 2] = VoigtTensor[5, 3];
                this[2, 1, 1, 0] = VoigtTensor[3, 5];
                this[1, 1, 0, 2] = VoigtTensor[1, 4];
                this[1, 1, 2, 0] = VoigtTensor[1, 4];

                this[2, 1, 0, 2] = VoigtTensor[3, 4];
                this[2, 0, 1, 2] = VoigtTensor[4, 3];
                this[1, 2, 0, 2] = VoigtTensor[3, 4];
                this[0, 2, 1, 2] = VoigtTensor[4, 3];
                this[1, 0, 2, 2] = VoigtTensor[5, 2];
                this[0, 1, 2, 2] = VoigtTensor[5, 2];

                this[2, 1, 2, 0] = VoigtTensor[3, 4];
                this[2, 0, 2, 1] = VoigtTensor[4, 3];
                this[1, 2, 2, 0] = VoigtTensor[3, 4];
                this[0, 2, 2, 1] = VoigtTensor[4, 3];
                this[2, 2, 1, 0] = VoigtTensor[2, 5];
                this[2, 2, 0, 1] = VoigtTensor[2, 5];
            }
        }

        public FourthRankTensor(MathNet.Numerics.LinearAlgebra.Matrix<double> VoigtTensor, bool compliance)
        {
            this._setComponents();

            if (VoigtTensor.RowCount == VoigtTensor.ColumnCount && VoigtTensor.RowCount == 6)
            {
                this[0, 0, 0, 0] = VoigtTensor[0, 0];

                this[1, 0, 0, 0] = 0.5 * VoigtTensor[5, 0];
                this[0, 1, 0, 0] = 0.5 * VoigtTensor[5, 0];
                this[0, 0, 1, 0] = 0.5 * VoigtTensor[0, 5];
                this[0, 0, 0, 1] = 0.5 * VoigtTensor[0, 5];

                this[1, 1, 0, 0] = VoigtTensor[1, 0];
                this[1, 0, 1, 0] = 0.25 * VoigtTensor[5, 5];
                this[1, 0, 0, 1] = 0.25 * VoigtTensor[5, 5];
                this[0, 1, 1, 0] = 0.25 * VoigtTensor[5, 5];
                this[0, 1, 0, 1] = 0.25 * VoigtTensor[5, 5];
                this[0, 0, 1, 1] = VoigtTensor[0, 1];

                this[1, 1, 1, 0] = 0.5 * VoigtTensor[1, 5];
                this[1, 1, 0, 1] = 0.5 * VoigtTensor[1, 5];
                this[1, 0, 1, 1] = 0.5 * VoigtTensor[5, 1];
                this[0, 1, 1, 1] = 0.5 * VoigtTensor[5, 1];

                this[1, 1, 1, 1] = VoigtTensor[1, 1];

                this[2, 0, 0, 0] = 0.5 * VoigtTensor[4, 0];
                this[0, 2, 0, 0] = 0.5 * VoigtTensor[4, 0];
                this[0, 0, 2, 0] = 0.5 * VoigtTensor[0, 4];
                this[0, 0, 0, 2] = 0.5 * VoigtTensor[0, 4];

                this[2, 2, 0, 0] = VoigtTensor[2, 0];
                this[2, 0, 2, 0] = 0.25 * VoigtTensor[4, 4];
                this[2, 0, 0, 2] = 0.25 * VoigtTensor[4, 4];
                this[0, 2, 2, 0] = 0.25 * VoigtTensor[4, 4];
                this[0, 2, 0, 2] = 0.25 * VoigtTensor[4, 4];
                this[0, 0, 2, 2] = VoigtTensor[0, 2];

                this[2, 2, 2, 0] = 0.5 * VoigtTensor[2, 4];
                this[2, 2, 0, 2] = 0.5 * VoigtTensor[2, 4];
                this[2, 0, 2, 2] = 0.5 * VoigtTensor[4, 2];
                this[0, 2, 2, 2] = 0.5 * VoigtTensor[4, 2];

                this[2, 1, 1, 1] = 0.5 * VoigtTensor[3, 1];
                this[1, 2, 1, 1] = 0.5 * VoigtTensor[3, 1];
                this[1, 1, 2, 1] = 0.5 * VoigtTensor[1, 3];
                this[1, 1, 1, 2] = 0.5 * VoigtTensor[1, 3];

                this[2, 2, 1, 1] = VoigtTensor[2, 1];
                this[2, 1, 2, 1] = 0.25 * VoigtTensor[3, 3];
                this[2, 1, 1, 2] = 0.25 * VoigtTensor[3, 3];
                this[1, 2, 2, 1] = 0.25 * VoigtTensor[3, 3];
                this[1, 2, 1, 2] = 0.25 * VoigtTensor[3, 3];
                this[1, 1, 2, 2] = VoigtTensor[1, 2];

                this[2, 2, 2, 1] = 0.5 * VoigtTensor[2, 3];
                this[2, 2, 1, 2] = 0.5 * VoigtTensor[2, 3];
                this[2, 1, 2, 2] = 0.5 * VoigtTensor[3, 2];
                this[1, 2, 2, 2] = 0.5 * VoigtTensor[3, 2];

                this[2, 2, 2, 2] = VoigtTensor[2, 2];

                this[0, 1, 2, 0] = 0.25 * VoigtTensor[5, 4];
                this[0, 2, 1, 0] = 0.25 * VoigtTensor[4, 5];
                this[1, 0, 2, 0] = 0.25 * VoigtTensor[5, 4];
                this[2, 0, 1, 0] = 0.25 * VoigtTensor[4, 5];
                this[1, 2, 0, 0] = 0.5 * VoigtTensor[3, 0];
                this[2, 1, 0, 0] = 0.5 * VoigtTensor[3, 0];

                this[0, 1, 0, 2] = 0.25 * VoigtTensor[5, 4];
                this[0, 2, 0, 1] = 0.25 * VoigtTensor[4, 5];
                this[1, 0, 0, 2] = 0.25 * VoigtTensor[5, 4];
                this[2, 0, 0, 1] = 0.25 * VoigtTensor[4, 5];
                this[0, 0, 1, 2] = 0.5 * VoigtTensor[0, 3];
                this[0, 0, 2, 1] = 0.5 * VoigtTensor[0, 3];

                this[1, 0, 2, 1] = 0.25 * VoigtTensor[5, 3];
                this[1, 2, 0, 1] = 0.25 * VoigtTensor[3, 5];
                this[0, 1, 2, 1] = 0.25 * VoigtTensor[5, 3];
                this[2, 1, 0, 1] = 0.25 * VoigtTensor[3, 5];
                this[0, 2, 1, 1] = 0.5 * VoigtTensor[4, 1];
                this[2, 0, 1, 1] = 0.5 * VoigtTensor[4, 1];

                this[1, 0, 1, 2] = 0.25 * VoigtTensor[5, 3];
                this[1, 2, 1, 0] = 0.25 * VoigtTensor[3, 5];
                this[0, 1, 1, 2] = 0.25 * VoigtTensor[5, 3];
                this[2, 1, 1, 0] = 0.25 * VoigtTensor[3, 5];
                this[1, 1, 0, 2] = 0.5 * VoigtTensor[1, 4];
                this[1, 1, 2, 0] = 0.5 * VoigtTensor[1, 4];

                this[2, 1, 0, 2] = 0.25 * VoigtTensor[3, 4];
                this[2, 0, 1, 2] = 0.25 * VoigtTensor[4, 3];
                this[1, 2, 0, 2] = 0.25 * VoigtTensor[3, 4];
                this[0, 2, 1, 2] = 0.25 * VoigtTensor[4, 3];
                this[1, 0, 2, 2] = 0.5 * VoigtTensor[5, 2];
                this[0, 1, 2, 2] = 0.5 * VoigtTensor[5, 2];

                this[2, 1, 2, 0] = 0.25 * VoigtTensor[3, 4];
                this[2, 0, 2, 1] = 0.25 * VoigtTensor[4, 3];
                this[1, 2, 2, 0] = 0.25 * VoigtTensor[3, 4];
                this[0, 2, 2, 1] = 0.25 * VoigtTensor[4, 3];
                this[2, 2, 1, 0] = 0.5 * VoigtTensor[2, 5];
                this[2, 2, 0, 1] = 0.5 * VoigtTensor[2, 5];
            }
        }

        public FourthRankTensor()
        {
            this._setComponents();
        }

        public static FourthRankTensor GetUnityTensor()
        {
            FourthRankTensor ret = new FourthRankTensor();

            for (int m = 0; m < 3; m++)
            {
                for (int n = 0; n < 3; n++)
                {
                    for (int o = 0; o < 3; o++)
                    {
                        for (int p = 0; p < 3; p++)
                        {
                            double mo = 0.0;
                            double np = 0.0;
                            double mp = 0.0;
                            double no = 0.0;

                            if(m == o)
                            {
                                mo = 1.0;
                            }
                            if (n == p)
                            {
                                np = 1.0;
                            }
                            if (m == p)
                            {
                                mp = 1.0;
                            }
                            if (n == o)
                            {
                                no = 1.0;
                            }

                            ret[m, n, o, p] = 0.5 * ((mo * np) + (mp * no));
                        }
                    }
                }
            }

            return ret;
        }

        public MathNet.Numerics.LinearAlgebra.Matrix<double> GetVoigtTensor()
        {
            MathNet.Numerics.LinearAlgebra.Matrix<double> ret = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense<double>(6, 6, 0);

            ret[0, 0] = this[0, 0, 0, 0];
            ret[1, 0] = this[1, 1, 0, 0];
            ret[2, 0] = this[2, 2, 0, 0];
            ret[3, 0] = this[1, 2, 0, 0];
            ret[4, 0] = this[0, 2, 0, 0];
            ret[5, 0] = this[0, 1, 0, 0];

            ret[0, 1] = this[0, 0, 1, 1];
            ret[1, 1] = this[1, 1, 1, 1];
            ret[2, 1] = this[2, 2, 1, 1];
            ret[3, 1] = this[1, 2, 1, 1];
            ret[4, 1] = this[0, 2, 1, 1];
            ret[5, 1] = this[0, 1, 1, 1];

            ret[0, 2] = this[0, 0, 2, 2];
            ret[1, 2] = this[1, 1, 2, 2];
            ret[2, 2] = this[2, 2, 2, 2];
            ret[3, 2] = this[1, 2, 2, 2];
            ret[4, 2] = this[0, 2, 2, 2];
            ret[5, 2] = this[0, 1, 2, 2];

            ret[0, 3] = this[0, 0, 1, 2];
            ret[1, 3] = this[1, 1, 1, 2];
            ret[2, 3] = this[2, 2, 1, 2];
            ret[3, 3] = this[1, 2, 1, 2];
            ret[4, 3] = this[0, 2, 1, 2];
            ret[5, 3] = this[0, 1, 1, 2];

            ret[0, 4] = this[0, 0, 0, 2];
            ret[1, 4] = this[1, 1, 0, 2];
            ret[2, 4] = this[2, 2, 0, 2];
            ret[3, 4] = this[1, 2, 0, 2];
            ret[4, 4] = this[0, 2, 0, 2];
            ret[5, 4] = this[0, 1, 0, 2];

            ret[0, 5] = this[0, 0, 0, 1];
            ret[1, 5] = this[1, 1, 0, 1];
            ret[2, 5] = this[2, 2, 0, 1];
            ret[3, 5] = this[1, 2, 0, 1];
            ret[4, 5] = this[0, 2, 0, 1];
            ret[5, 5] = this[0, 1, 0, 1];

            return ret;
        }

        public double InnerTransormation(MathNet.Numerics.LinearAlgebra.Matrix<double> t1, MathNet.Numerics.LinearAlgebra.Matrix<double> t2)
        {
            double ret = 0.0;

            for(int n = 0; n < 3; n++ )
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            ret += t1[n, i] * this[n, i, j, k] * t2[j, k];
                        }
                    }
                }
            }

            return ret;
        }

        public FourthRankTensor Inverse()
        {
            //MathNet.Numerics.LinearAlgebra.Matrix<double> linearMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense<double>(81, 81, 0);
            //MathNet.Numerics.LinearAlgebra.Vector<double> solutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(81, 0);

            //int vektorIndex = 0;
            //for(int i = 0; i < 3; i++)
            //{
            //    for (int j = 0; j < 3; j++)
            //    {
            //        for (int k = 0; k < 3; k++)
            //        {
            //            for (int l = 0; l < 3; l++)
            //            {
            //                double ik = 0.0;
            //                double jl = 0.0;
            //                double il = 0.0;
            //                double jk = 0.0;

            //                if (i == k)
            //                {
            //                    ik = 1.0;
            //                }
            //                if (j == l)
            //                {
            //                    jl = 1.0;
            //                }
            //                if (i == l)
            //                {
            //                    il = 1.0;
            //                }
            //                if (j == k)
            //                {
            //                    jk = 1.0;
            //                }

            //                solutionVector[vektorIndex] = 0.5 * ((ik * jl) + (il * jk));

            //                int matrixIndex = 0;
            //                for(int m = 0; m < 3; m++)
            //                {
            //                    for(int n = 0; n < 3; n++)
            //                    {
            //                        linearMatrix[vektorIndex, matrixIndex + (m * 27) + (n * 9)] = this[i, j, m, n];
            //                        matrixIndex++;
            //                    }
            //                }

            //                vektorIndex++;
            //            }
            //        }
            //    }
            //}

            //MathNet.Numerics.LinearAlgebra.Vector<double> solution = linearMatrix.Solve(solutionVector);

            //FourthRankTensor ret = new FourthRankTensor();
            //vektorIndex = 0;
            //for (int i = 0; i < 3; i++)
            //{
            //    for (int j = 0; j < 3; j++)
            //    {
            //        for (int k = 0; k < 3; k++)
            //        {
            //            for (int l = 0; l < 3; l++)
            //            {
            //                ret[i, j, k, l] = solution[vektorIndex];
            //                vektorIndex++;
            //            }
            //        }
            //    }
            //}

            //FourthRankTensor check = this * ret;
            //FourthRankTensor ui = GetUnityTensor();

            //double dif = check.GetDifference(ui);

            MathNet.Numerics.LinearAlgebra.Matrix<double> inv = this.GetVoigtTensor();
            FourthRankTensor ret = new FourthRankTensor(inv.Inverse());

            return ret;
        }

        //inverse from Stiffness to Compliance
        public FourthRankTensor InverseSC()
        {
            MathNet.Numerics.LinearAlgebra.Matrix<double> inv = this.GetVoigtTensor();
            FourthRankTensor ret = new FourthRankTensor(inv.Inverse(), true);

            FourthRankTensor check = this * ret;
            FourthRankTensor ui = GetUnityTensor();

            double dif = check.GetDifference(ui);

            return ret;
        }

        /// <summary>
        /// Transforms the tensor from one orientation frame into another
        /// </summary>
        /// <param name="phi1">First rotation angle in rad</param>
        /// <param name="psi">second rotation angle in rad</param>
        /// <returns>The transformed tensor</returns>
        public FourthRankTensor FrameTransformation(double phi1, double psi)
        {
            MathNet.Numerics.LinearAlgebra.Matrix<double> transformationMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense<double>(3, 3, 0);

            transformationMatrix[0, 0] = Math.Cos(phi1) * Math.Cos(psi);
            transformationMatrix[0, 1] = Math.Sin(phi1) * Math.Cos(psi);
            transformationMatrix[0, 2] = -1.0 * Math.Sin(psi);

            transformationMatrix[1, 0] = -1.0 * Math.Sin(phi1);
            transformationMatrix[1, 1] = Math.Cos(phi1);

            transformationMatrix[2, 0] = Math.Cos(phi1) * Math.Sin(psi);
            transformationMatrix[2, 1] = Math.Sin(phi1) * Math.Sin(psi);
            transformationMatrix[2, 2] = Math.Cos(psi);

            FourthRankTensor ret = new FourthRankTensor();

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        for (int l = 0; l < 3; l++)
                        {
                            //-----------------------
                            for (int m = 0; m < 3; m++)
                            {
                                for (int n = 0; n < 3; n++)
                                {
                                    for (int o = 0; o < 3; o++)
                                    {
                                        for (int p = 0; p < 3; p++)
                                        {
                                            ret[i, j, k, l] += transformationMatrix[i, m] * transformationMatrix[j, n] * transformationMatrix[k, o] * transformationMatrix[l, p] * this[m, n, o, p];
                                        }
                                    }
                                }
                            }
                            //-----------------------
                        }
                    }
                }
            }

            return ret;
        }
        public FourthRankTensor FrameTransformation(double phi1, double psi, double phi2)
        {
            MathNet.Numerics.LinearAlgebra.Matrix<double> transformationMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense<double>(3, 3, 0);
            transformationMatrix[0, 0] = -1 * Math.Cos(phi1) * Math.Cos(psi) * Math.Sin(phi2);
            transformationMatrix[0, 0] -= Math.Sin(phi1) * Math.Cos(phi2);
            transformationMatrix[0, 1] = -1 * Math.Cos(psi) * Math.Sin(phi1) * Math.Sin(phi2);
            transformationMatrix[0, 1] -= Math.Cos(phi1) * Math.Cos(phi2);
            transformationMatrix[0, 2] = Math.Sin(psi) * Math.Sin(phi2);
            transformationMatrix[1, 0] = -1 * Math.Cos(psi) * Math.Cos(phi1) * Math.Cos(phi2);
            transformationMatrix[1, 0] -= Math.Sin(phi1) * Math.Sin(phi2);
            transformationMatrix[1, 1] = -1 * Math.Sin(phi1) * Math.Cos(psi) * Math.Cos(phi2);
            transformationMatrix[1, 1] -= Math.Cos(phi1) * Math.Sin(phi2);
            transformationMatrix[1, 2] = Math.Sin(psi) * Math.Cos(phi2);
            transformationMatrix[2, 0] = Math.Cos(phi1) * Math.Sin(psi);
            transformationMatrix[2, 1] = Math.Sin(phi1) * Math.Sin(psi);
            transformationMatrix[2, 2] = Math.Cos(psi);

            FourthRankTensor ret = new FourthRankTensor();

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        for (int l = 0; l < 3; l++)
                        {
                            //-----------------------
                            for (int m = 0; m < 3; m++)
                            {
                                for (int n = 0; n < 3; n++)
                                {
                                    for (int o = 0; o < 3; o++)
                                    {
                                        for (int p = 0; p < 3; p++)
                                        {
                                            ret[i, j, k, l] += transformationMatrix[i, m] * transformationMatrix[j, n] * transformationMatrix[k, o] * transformationMatrix[l, p] * this[m, n, o, p];
                                        }
                                    }
                                }
                            }
                            //-----------------------
                        }
                    }
                }
            }

            return ret;
        }

        public static FourthRankTensor InnerProduct(FourthRankTensor a, FourthRankTensor b)
        {
            FourthRankTensor ret = new FourthRankTensor();

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        for (int l = 0; l < 3; l++)
                        {
                            for (int m = 0; m < 3; m++)
                            {
                                for (int n = 0; n < 3; n++)
                                {
                                    ret[i, j, k, l] += a[i, j, m, n] * b[m, n, k, l];
                                }
                            }
                        }
                    }
                }
            }

            return ret;
        }
        public static FourthRankTensor AverageInnerProduct(List<FourthRankTensor> lc, List<FourthRankTensor> ac)
        {
            FourthRankTensor ret = new FourthRankTensor();

            for (int z = 0; z < lc.Count; z++)
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            for (int l = 0; l < 3; l++)
                            {
                                for (int m = 0; m < 3; m++)
                                {
                                    for (int n = 0; n < 3; n++)
                                    {
                                        ret[i, j, k, l] += lc[z][i, j, m, n] * ac[z][m, n, k, l];
                                    }
                                }
                            }
                        }
                    }
                }
            }

            ret /= lc.Count;

            return ret;
        }

        public static FourthRankTensor AverageInnerProduct(List<Analysis.Stress.Plasticity.PlasticityTensor> pT)
        {
            FourthRankTensor ret = new FourthRankTensor();

            for (int z = 0; z < pT.Count; z++)
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            for (int l = 0; l < 3; l++)
                            {
                                for (int m = 0; m < 3; m++)
                                {
                                    for (int n = 0; n < 3; n++)
                                    {
                                        ret[i, j, k, l] += pT[z].GrainStiffness[i, j, m, n] * pT[z].GrainTransitionStiffness[m, n, k, l];
                                    }
                                }
                            }
                        }
                    }
                }
            }

            ret /= pT.Count;

            return ret;
        }

        public double GetDifference(FourthRankTensor compTensor)
        {
            double ret = 0;

            for (int m = 0; m < 3; m++)
            {
                for (int n = 0; n < 3; n++)
                {
                    for (int o = 0; o < 3; o++)
                    {
                        for (int p = 0; p < 3; p++)
                        {
                            ret += Math.Pow(this[m, n, o, p] - compTensor[m, n, o, p], 2);
                        }
                    }
                }
            }

            return Math.Sqrt(ret) / 81.0;
        }

        public void SetHexagonalSymmetryCorrection()
        {
            MathNet.Numerics.LinearAlgebra.Matrix<double> inv = this.GetVoigtTensor();
            MathNet.Numerics.LinearAlgebra.Matrix<double> corrected = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense<double>(6, 6, 0.0);

            corrected[0, 0] = inv[0, 0];
            corrected[1, 1] = inv[0, 0];
            corrected[2, 2] = inv[2, 2];

            corrected[0, 1] = inv[0, 1];
            corrected[1, 0] = inv[0, 1];
            corrected[0, 2] = inv[0, 2];
            corrected[2, 0] = inv[0, 2];
            corrected[1, 2] = inv[0, 2];
            corrected[2, 1] = inv[0, 2];

            corrected[3, 3] = inv[3, 3];
            corrected[4, 4] = inv[3, 3];
            corrected[5, 5] = inv[5, 5];
            //corrected[5, 5] = 0.5 * (inv[0, 0] - inv[0, 1]);

            FourthRankTensor transformed = new FourthRankTensor(corrected);
            this._components = transformed._components;

        }

        #region Operatoren

        public static FourthRankTensor operator *(FourthRankTensor a, FourthRankTensor b)
        {
            FourthRankTensor ret = new FourthRankTensor();

            for (int m = 0; m < 3; m++)
            {
                for (int n = 0; n < 3; n++)
                {
                    for (int o = 0; o < 3; o++)
                    {
                        for (int p = 0; p < 3; p++)
                        {
                            for(int q = 0; q < 3; q++)
                            {
                                for (int r = 0; r < 3; r++)
                                {
                                    ret[m, n, o, p] += a[m, n, q, r] * b[q, r, o, p];
                                }
                            }
                        }
                    }
                }
            }

            return ret;
        }

        public static MathNet.Numerics.LinearAlgebra.Matrix<double> operator *(FourthRankTensor a, MathNet.Numerics.LinearAlgebra.Matrix<double> b)
        {
            if (b.RowCount == b.ColumnCount && b.RowCount == 3)
            {
                MathNet.Numerics.LinearAlgebra.Matrix<double> ret = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense<double>(3, 3, 0);
                for (int o = 0; o < 3; o++)
                {
                    for (int p = 0; p < 3; p++)
                    {
                        for (int q = 0; q < 3; q++)
                        {
                            for (int r = 0; r < 3; r++)
                            {
                                ret[o, p] += a[o, p, q, r] * b[q, r];
                            }
                        }
                    }
                }
                return ret;
            }
            else
            {
                return null;
            }
        }

        public static FourthRankTensor operator *(double a, FourthRankTensor b)
        {
            FourthRankTensor ret = new FourthRankTensor();

            for (int m = 0; m < 3; m++)
            {
                for (int n = 0; n < 3; n++)
                {
                    for (int o = 0; o < 3; o++)
                    {
                        for (int p = 0; p < 3; p++)
                        {
                            ret[m, n, o, p] = a * b[m, n, o, p];
                        }
                    }
                }
            }

            return ret;
        }
        public static FourthRankTensor operator *(FourthRankTensor a, double b)
        {
            FourthRankTensor ret = new FourthRankTensor();

            for (int m = 0; m < 3; m++)
            {
                for (int n = 0; n < 3; n++)
                {
                    for (int o = 0; o < 3; o++)
                    {
                        for (int p = 0; p < 3; p++)
                        {
                            ret[m, n, o, p] = b * a[m, n, o, p];
                        }
                    }
                }
            }

            return ret;
        }

        public static FourthRankTensor operator /(FourthRankTensor a, double b)
        {
            FourthRankTensor ret = new FourthRankTensor();

            for (int m = 0; m < 3; m++)
            {
                for (int n = 0; n < 3; n++)
                {
                    for (int o = 0; o < 3; o++)
                    {
                        for (int p = 0; p < 3; p++)
                        {
                            ret[m, n, o, p] = a[m, n, o, p] / b;
                        }
                    }
                }
            }

            return ret;
        }

        public static FourthRankTensor operator +(FourthRankTensor a, FourthRankTensor b)
        {
            FourthRankTensor ret = new FourthRankTensor();

            for (int m = 0; m < 3; m++)
            {
                for (int n = 0; n < 3; n++)
                {
                    for (int o = 0; o < 3; o++)
                    {
                        for (int p = 0; p < 3; p++)
                        {
                            ret[m, n, o, p] = a[m, n, o, p] + b[m, n, o, p];
                        }
                    }
                }
            }

            return ret;
        }

        public static FourthRankTensor operator -(FourthRankTensor a, FourthRankTensor b)
        {
            FourthRankTensor ret = new FourthRankTensor();

            for (int m = 0; m < 3; m++)
            {
                for (int n = 0; n < 3; n++)
                {
                    for (int o = 0; o < 3; o++)
                    {
                        for (int p = 0; p < 3; p++)
                        {
                            ret[m, n, o, p] = a[m, n, o, p] - b[m, n, o, p];
                        }
                    }
                }
            }

            return ret;
        }

        #endregion

        public object Clone()
        {
            FourthRankTensor ret = new FourthRankTensor();

            for (int m = 0; m < 3; m++)
            {
                for (int n = 0; n < 3; n++)
                {
                    for (int o = 0; o < 3; o++)
                    {
                        for (int p = 0; p < 3; p++)
                        {
                            ret[m, n, o, p] = this[m, n, o, p];
                        }
                    }
                }
            }

            return ret;
        }
    }
}
