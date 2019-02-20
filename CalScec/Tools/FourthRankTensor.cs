using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.Tools
{
    public class FourthRankTensor
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
    }
}
