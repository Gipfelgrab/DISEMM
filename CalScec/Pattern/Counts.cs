using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.Pattern
{
    [Serializable]
    public class Counts : List<double[]>, ICloneable
    {

        #region Calculations

        public double GetMaximum(int StartIdx, int EndIdx)
        {
            double ret = 0.0;
            for(int n = StartIdx; n <= EndIdx; n++)
            {
                if(ret < this[n][1])
                {
                    ret = this[n][1];
                }
            }

            return ret;
        }

        public double GetMinimum(int StartIdx, int EndIdx)
        {
            double ret = double.MaxValue;
            for (int n = StartIdx; n <= EndIdx; n++)
            {
                if (ret > this[n][1])
                {
                    ret = this[n][1];
                }
            }

            return ret;
        }

        public double GetMaximum(double StartDegree, double EndDegree)
        {
            double ret = 0.0;
            double Start = StartDegree;
            double End = EndDegree;
            if (Start > End)
            {
                Start = EndDegree;
                End = StartDegree;
            }

            for (int n = 1; n < this.Count; n++)
            {
                if (this[n][0] > Start)
                {
                    for (int i = n; i < this.Count; i++)
                    {
                        if (this[i][0] > End)
                        {
                            if (i == 0)
                            {
                                return this.GetMaximum(n, i);
                            }
                            else
                            {
                                return this.GetMaximum(n, i - 1);
                            }
                        }
                    }
                }
            }

            return ret;
        }

        public double GetMinimum(double StartDegree, double EndDegree)
        {
            double ret = double.MaxValue;
            double Start = StartDegree;
            double End = EndDegree;
            if (Start > End)
            {
                Start = EndDegree;
                End = StartDegree;
            }

            for (int n = 1; n < this.Count; n++)
            {
                if (this[n][0] > Start)
                {
                    for (int i = n; i < this.Count; i++)
                    {
                        if (this[i][0] > End)
                        {
                            if (i == 0)
                            {
                                return this.GetMinimum(n, i);
                            }
                            else
                            {
                                return this.GetMinimum(n, i - 1);
                            }
                        }
                    }
                }
            }

            return ret;
        }

        public double GetMinimum()
        {
            return this.GetMinimum(0, this.Count - 1);
        }

        public double GetMaximum()
        {
            return this.GetMaximum(0, this.Count - 1);
        }

        public double[] GetFirstDerivative(int idx)
        {
            if(idx == 0)
            {
                double[] ret = { this[idx][0], this[1][1] - this[0][1] };
                return ret;
            }
            else if(idx == this.Count - 1)
            {
                double[] ret = { this[idx][0], this[this.Count - 1][1] - this[this.Count - 2][1] };
                return ret;
            }
            else
            {
                double[] ret = { this[idx][0], (this[idx + 1][1] - this[idx - 1][1]) / 2.0 };
                return ret;
            }
        }

        public double[] GetSecondDerivative(int idx)
        {
            if (idx == 0 || idx == this.Count - 1)
            {
                double[] ret = { this[idx][0], 0.0 };
                return ret;
            }
            else
            {
                double[] ret = { this[idx][0], this[idx - 1][1] - (2.0 * this[idx][1]) + this[idx + 1][1] };
                return ret;
            }
        }

        new public Counts GetRange(int StartIdx, int EndIdx)
        {
            Counts Ret = new Counts();
            int startIdx = 0;
            int endIdx = 0;

            if(StartIdx < 0)
            {
                StartIdx = 0;
            }
            if(EndIdx < 0)
            {
                EndIdx = 0;
            }
            if(EndIdx >= this.Count)
            {
                EndIdx = this.Count - 1;
            }

            if(EndIdx < StartIdx)
            {
                startIdx = EndIdx;
                endIdx = StartIdx;
            }
            else
            {
                startIdx = StartIdx;
                endIdx = EndIdx;
            }

            for(int n = startIdx; n <= endIdx; n++)
            {
                List<double> ChannelTmp = new List<double>();

                for(int i = 0; i < this[n].Length; i++)
                {
                    ChannelTmp.Add(this[n][i]);
                }
                Ret.Add(ChannelTmp.ToArray());
            }

            return Ret;
        }

        public Counts GetRange(double StartDegree, double EndDegree)
        {
            double Start = StartDegree;
            double End = EndDegree;
            if(Start > End)
            {
                Start = EndDegree;
                End = StartDegree;
            }

            for(int n = 1; n < this.Count; n++)
            {
                if(this[n][0] > Start)
                {
                    for(int i = n; i < this.Count; i++)
                    {
                        if(this[i][0] > End)
                        {
                            if(i == 0)
                            {
                                return this.GetRange(n, i);
                            }
                            else
                            {
                                return this.GetRange(n, i - 1);
                            }
                        }
                    }
                }
            }

            return this.Clone() as Counts;
        }

        #endregion

        #region IClonable

        public object Clone()
        {

            double[][] RetTmp = this.ToArray();
            Counts Ret = new Counts();
            Ret.AddRange(RetTmp);

            return Ret;
        }

        #endregion
    }
}
