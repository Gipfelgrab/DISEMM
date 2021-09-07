using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.Analysis.Stress.Macroskopic
{
    [Serializable]
    public class TensileTest
    {
        public DateTime Executed = DateTime.Now;
        public string ExecutedDisplay
        {
            get
            {
                return Executed.ToShortDateString();
            }
        }

        public List<double> TimeData = new List<double>();

        public List<double> ForceData = new List<double>();
        public List<double> StressData = new List<double>();

        public List<double> ExtensionData = new List<double>();
        public List<double> StrainData = new List<double>();

        public double ExtensionOffset;
        public double ExtensionBaselength;
        public double SampleArea;

        public double EModul = 0.0;
        public double ElasticLimit = 0.0;

        public TensileTest(DateTime _executed, double _extensionBaseLength, double _sampleArea)
        {
            this.Executed = _executed;
            this.ExtensionBaselength = _extensionBaseLength;
            this.SampleArea = _sampleArea;
            this.ExtensionOffset = 0.0;
        }

        public TensileTest(DateTime _executed, double _extensionBaseLength, double _sampleArea, double _extensionOffset)
        {
            this.Executed = _executed;
            this.ExtensionBaselength = _extensionBaseLength;
            this.SampleArea = _sampleArea;
            this.ExtensionOffset = _extensionOffset;
        }

        public void SetMechanicalProperties(int startIndex)
        {
            this.EModul = 0.0;
            this.ElasticLimit = 0.0;
            if(startIndex + 1 < StressData.Count)
            {
                Pattern.Counts UsedCounts = new Pattern.Counts();
                Fitting.LinearFunction classicFittingFunction = new Fitting.LinearFunction();

                double[] count0 = { this.StrainData[startIndex], StressData[startIndex], StressData[startIndex] * 0.01 };
                UsedCounts.Add(count0);
                bool firstTouch = false;

                for (int n = startIndex + 1; n < StressData.Count - 1; n++)
                {
                    double[] countTmp = { this.StrainData[n], StressData[n], StressData[n] * 0.01 };
                    UsedCounts.Add(countTmp);

                    if (UsedCounts.Count == 2)
                    {
                        double aclivity = (UsedCounts[0][1] - UsedCounts[1][1]) / (UsedCounts[0][0] - UsedCounts[1][0]);

                        double _constant = UsedCounts[0][1] - (aclivity * UsedCounts[0][0]);

                        classicFittingFunction.Aclivity = aclivity;
                        classicFittingFunction.Constant = _constant;

                        MathNet.Numerics.LinearAlgebra.Matrix<double> ErrorMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(2, 2, 0.0);

                        double aclivityError = Math.Pow((1 / (UsedCounts[0][0] - UsedCounts[1][0])) * UsedCounts[0][2], 2) + Math.Pow((-1 / (UsedCounts[0][0] - UsedCounts[1][0])) * UsedCounts[1][2], 2);
                        ErrorMatrix[0, 0] = Math.Sqrt(aclivityError);

                        double ConstantError = Math.Pow(UsedCounts[0][0] * aclivityError, 2) + Math.Pow(UsedCounts[0][2], 2);

                        ErrorMatrix[1, 1] = Math.Sqrt(ConstantError);

                        classicFittingFunction._hessianMatrix = ErrorMatrix;
                    }
                    else if (UsedCounts.Count > 2)
                    {
                        bool classicFittingConverged = Fitting.LMA.FitMacroElasticModul(classicFittingFunction, UsedCounts);
                    }

                    double nextElasticStrainRate = this.StrainData[n + 1] - this.StrainData[n];
                    nextElasticStrainRate /= this.StressData[n + 1] - this.StressData[n];

                    if (Math.Abs((1.0 / nextElasticStrainRate) - classicFittingFunction.Aclivity) > Math.Abs(3.0 * classicFittingFunction.AclivityError))
                    {
                        this.EModul = classicFittingFunction.Aclivity;
                        this.ElasticLimit = this.StressData[n + 1];
                        if(firstTouch)
                        {
                            break;
                        }
                        else
                        {
                            firstTouch = true;
                        }
                    }
                    else if(firstTouch)
                    {
                        firstTouch = false;
                    }
                }
            }
        }
    }
}
