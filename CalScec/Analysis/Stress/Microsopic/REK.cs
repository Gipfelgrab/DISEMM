using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.Analysis.Stress.Microsopic
{
    public class REK : ICloneable
    {
        #region General information

        public DataManagment.CrystalData.CODData PhaseInformation;
        public DataManagment.CrystalData.HKLReflex UsedReflex;

        public string HKLAssociation
        {
            get
            {
                string ret = PhaseInformation.SymmetryGroup + " ( " + UsedReflex.H + ", " + UsedReflex.K + ", " + UsedReflex.L + " )";
                return ret;
            }
        }

        #endregion

        public REK(DataManagment.CrystalData.CODData phaseInformation, DataManagment.CrystalData.HKLReflex hKLReflex)
        {
            this.PhaseInformation = phaseInformation;
            this.UsedReflex = hKLReflex;
        }

        public REK(DataManagment.CrystalData.CODData phaseInformation, DataManagment.CrystalData.HKLReflex hKLReflex, double strainFreeD0)
        {
            this.PhaseInformation = phaseInformation;
            this.UsedReflex = hKLReflex;
            this._strainFreeD0 = strainFreeD0;
        }

        #region classical calculation

        public List<Macroskopic.PeakStressAssociation> ElasticStressData = new List<Macroskopic.PeakStressAssociation>();

        public bool ClassicREKFittingConverged = false;
        public Fitting.LinearFunction ClassicFittingFunction = new Fitting.LinearFunction();

        public void SetClassicDeviation()
        {
            Pattern.Counts UsedCounts = this.GetClassicREKFittingData();
            this._classicChi2 = Fitting.Chi2.Chi2LinearFunction(ClassicFittingFunction, UsedCounts);

            double S1Error = 0.0;
            double S2Error = 0.0;
            double TotalWeight = 0.0;
            double TotalWeightS1 = 0.0;
            double TotalWeightS2 = 0.0;

            for (int n = 0; n < UsedCounts.Count; n++)
            {
                double S1Tmp = Math.Pow(UsedCounts[n][1] - ClassicFittingFunction.Y(UsedCounts[n][0]), 2);
                S1Tmp *= Math.Pow(this.ClassicFittingFunction.FirstDerivativeConstant(UsedCounts[n][0]), 2);
                S1Tmp *= UsedCounts[n][2];

                double S2Tmp = Math.Pow(UsedCounts[n][1] - ClassicFittingFunction.Y(UsedCounts[n][0]), 2);
                S2Tmp *= Math.Pow(this.ClassicFittingFunction.FirstDerivativeAclivity(UsedCounts[n][0]), 2);
                S2Tmp *= UsedCounts[n][2];

                S1Error += S1Tmp;
                S2Error += S2Tmp;
                TotalWeight += UsedCounts[n][2];
                TotalWeightS1 += Math.Pow(this.ClassicFittingFunction.FirstDerivativeConstant(UsedCounts[n][0]), 2);
                TotalWeightS2 += Math.Pow(this.ClassicFittingFunction.FirstDerivativeAclivity(UsedCounts[n][0]), 2);
            }

            //S1Error /= (TotalWeight * TotalWeightS1);
            //S2Error /= (TotalWeight * TotalWeightS2);
            S1Error /= (TotalWeight);
            S2Error /= (TotalWeight);

            this._classicS1Error = Math.Sqrt(S1Error);
            this._classicS2Error = Math.Sqrt(S2Error);
        }

        private double _classicChi2;
        public string ClassicChi2
        {
            get
            {
                return _classicChi2.ToString("G3");
            }
        }

        private double _classicS1Error;
        public double ClassicS1Error
        {
            get
            {
                return this._classicS1Error;
            }
        }
        public double ClassicS1
        {
            get
            {
                return this.ClassicFittingFunction.Constant;
            }
        }
        
        private double _classicS2Error;
        public double ClassicHS2Error
        {
            get
            {
                return this._classicS2Error;
            }
        }
        public double ClassicHS2
        {
            get
            {
                return this.ClassicFittingFunction.Aclivity;
            }
        }
        
        private double _strainFreeD0;
        public double StrainFreeD0
        {
            get
            {
                return this._strainFreeD0;
            }
        }

        public void CalculateStrainFreeD0()
        {
            this._strainFreeD0 = 0;
        }

        public Pattern.Counts GetClassicREKFittingData()
        {
            Pattern.Counts Ret = new Pattern.Counts();

            List<List<Macroskopic.PeakStressAssociation>> SortedData = new List<List<Macroskopic.PeakStressAssociation>>();

            List<double> UsedPsiAngles = new List<double>();
            int AllDataUsed = 0;
            
            while (AllDataUsed < this.ElasticStressData.Count)
            {
                List<Macroskopic.PeakStressAssociation> ActAngle = new List<Macroskopic.PeakStressAssociation>();
                int StartingNumber = 0;
                double ActPsiAngle = 0;

                for (int n = 0; n < this.ElasticStressData.Count; n++)
                {
                    bool Contained = false;
                    for (int i = 0; i < UsedPsiAngles.Count; i++)
                    {
                        if(Math.Abs(this.ElasticStressData[n].PsiAngle - UsedPsiAngles[i]) < CalScec.Properties.Settings.Default.PsyAcceptanceAngle)
                        {
                            Contained = true;
                            break;
                        }
                    }

                    if(!Contained)
                    {
                        StartingNumber = n + 1;
                        ActPsiAngle = this.ElasticStressData[n].PsiAngle;
                        ActAngle.Add(this.ElasticStressData[n]);
                        AllDataUsed++;
                        break;
                    }
                }

                for(int n = StartingNumber; n < this.ElasticStressData.Count; n++)
                {
                    if (Math.Abs(ActPsiAngle - this.ElasticStressData[n].PsiAngle) < CalScec.Properties.Settings.Default.PsyAcceptanceAngle)
                    {
                        ActAngle.Add(this.ElasticStressData[n]);
                        AllDataUsed++;
                    }
                }

                UsedPsiAngles.Add(ActPsiAngle);
                SortedData.Add(ActAngle);
            }

            for(int n = 0; n < SortedData.Count; n++)
            {
                if (SortedData[n].Count > 1)
                {
                    double SmallestStress = SortedData[n][0].Stress;
                    double SmallestDistance = SortedData[n][0].DPeak.LatticeDistance;

                    for (int i = 1; i < SortedData[n].Count; i++)
                    {
                        if(SmallestStress > SortedData[n][i].Stress)
                        {
                            SmallestStress = SortedData[n][i].Stress;
                            SmallestDistance = SortedData[n][i].DPeak.LatticeDistance;
                        }
                    }

                    for (int i = 0; i < SortedData[n].Count; i++)
                    {
                        if(SmallestStress != SortedData[n][i].Stress)
                        {
                            double FValue = (SortedData[n][i].DPeak.LatticeDistance - SmallestDistance);
                            FValue /= SmallestDistance;
                            FValue /= (SortedData[n][i].Stress - SmallestStress);

                            double[] NewCount = { Math.Pow(Math.Cos((Math.PI * SortedData[n][i].PsiAngle) / 180.0), 2), FValue, SortedData[n][i].DPeak.LatticeDistanceError };
                            Ret.Add(NewCount);
                        }
                    }
                }
            }

            return Ret;
        }

        public void FitClassicREKFunction()
        {
            Pattern.Counts UsedCounts = this.GetClassicREKFittingData();
            if (UsedCounts.Count == 2)
            {
                double aclivity = (UsedCounts[0][1] - UsedCounts[1][1]) / (UsedCounts[0][0] - UsedCounts[1][0]);

                double _constant = UsedCounts[0][1] - (aclivity * UsedCounts[0][0]);

                this.ClassicFittingFunction.Aclivity = aclivity;
                this.ClassicFittingFunction.Constant = _constant;

                MathNet.Numerics.LinearAlgebra.Matrix<double> ErrorMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(2, 2, 0.0);

                double aclivityError = Math.Pow((1 / (UsedCounts[0][0] - UsedCounts[1][0])) * UsedCounts[0][2], 2) + Math.Pow((-1 / (UsedCounts[0][0] - UsedCounts[1][0])) * UsedCounts[1][2], 2);
                ErrorMatrix[0, 0] = Math.Sqrt(aclivityError);

                double ConstantError = Math.Pow(UsedCounts[0][0] * aclivityError, 2) + Math.Pow(UsedCounts[0][2], 2);

                ErrorMatrix[1, 1] = Math.Sqrt(ConstantError);

                this.ClassicFittingFunction._hessianMatrix = ErrorMatrix;
                this.ClassicREKFittingConverged = true;
            }
            else if (UsedCounts.Count > 2)
            {
                this.ClassicREKFittingConverged = Fitting.LMA.FitMacroElasticModul(this.ClassicFittingFunction, UsedCounts);
                this.SetClassicDeviation();
            }
            else
            {
                this.ClassicREKFittingConverged = false;
            }
        }

        #endregion

        #region Macroscopic calculation

        public Macroskopic.Elasticity LongitudionalElasticity = new Macroskopic.Elasticity();
        public Macroskopic.Elasticity TransversalElasticity = new Macroskopic.Elasticity();

        public double MacroscopicS1Error
        {
            get
            {
                if (this.TransversalElasticity != null)
                {
                    double Ret = this.TransversalElasticity.EModulError;
                    Ret /= Math.Pow(this.TransversalElasticity.EModul, 2);

                    return Ret;
                }
                else
                {
                    return 0.0;
                }
            }
        }
        public double MacroscopicS1
        {
            get
            {
                if (this.TransversalElasticity != null)
                {
                    double Ret = 1 / this.TransversalElasticity.EModul;

                    return Ret;
                }
                else
                {
                    return 0.0;
                }
            }
        }

        public double MacroscopicHS2Error
        {
            get
            {
                if (this.TransversalElasticity != null && this.LongitudionalElasticity != null)
                {
                    double Ret = Math.Pow(LongitudionalElasticity.EModulError / Math.Pow(this.LongitudionalElasticity.EModul, 2), 2);
                    Ret += Math.Pow(TransversalElasticity.EModulError / Math.Pow(this.TransversalElasticity.EModul, 2), 2);
                    return Ret;
                }
                else
                {
                    return 0.0;
                }
            }
        }
        public double MacroscopicHS2
        {
            get
            {
                if (this.TransversalElasticity != null && this.LongitudionalElasticity != null)
                {
                    //double Ret = 1.0 - (this.LongitudionalElasticity.EModul / this.TransversalElasticity.EModul);
                    //Ret /= this.LongitudionalElasticity.EModul;
                    double Ret = 1.0 / this.LongitudionalElasticity.EModul;
                    Ret -= 1.0 / this.TransversalElasticity.EModul;
                    return Ret;
                }
                else
                {
                    return 0.0;
                }
            }
        }

        #endregion

        #region Derived parameters

        public double ClassicTransverseContraction
        {
            get
            {
                double Ret = -1;
                double N = this.ClassicHS2 / this.ClassicS1;
                N += 1;

                Ret /= N;
                return Ret;
            }
        }
        public double ClassicEModulus
        {
            get
            {
                double Ret = 1;
                double N = this.ClassicS1 + this.ClassicHS2;

                Ret /= N;
                return Ret;
            }
        }

        public double ClassicBulkModulus
        {
            get
            {
                #region Inverse Calculation

                double Ret = this.ClassicS1;
                Ret += (1.0 / 12.0) * this.ClassicHS2;
                Ret *= 9.0;

                return Ret;

                #endregion

                #region Linear Calculation

                //double Ret = this.ClassicS1;
                //Ret += (2.0 / 6.0) / this.ClassicHS2;
                //Ret *= 9.0;

                //return Ret;

                #endregion
            }
        }
        public double ClassicShearModulus
        {
            get
            {
                //double Ret = 1.0;
                double Ret = 0.5;
                double N = this.ClassicHS2;

                Ret /= N;
                return Ret;
            }
        }

        #endregion

        #region Cloning

        public object Clone()
        {
            REK Ret = new REK(this.PhaseInformation, new DataManagment.CrystalData.HKLReflex(this.UsedReflex.H, this.UsedReflex.K, this.UsedReflex.L, this.UsedReflex.Distance));
            Ret.ClassicREKFittingConverged = this.ClassicREKFittingConverged;

            Ret.ElasticStressData = new List<Macroskopic.PeakStressAssociation>();

            for(int n = 0; n < this.ElasticStressData.Count; n++)
            {
                Macroskopic.PeakStressAssociation Tmp = new Macroskopic.PeakStressAssociation(this.ElasticStressData[n].Stress, this.ElasticStressData[n].PsiAngle, this.ElasticStressData[n].DifPeak.Clone() as Analysis.Peaks.DiffractionPeak, this.ElasticStressData[n].PhiAngle);

                Ret.ElasticStressData.Add(Tmp);
            }

            Ret.ClassicFittingFunction = this.ClassicFittingFunction.Clone() as Analysis.Fitting.LinearFunction;
            Ret._strainFreeD0 = this._strainFreeD0;

            if (this.LongitudionalElasticity != null)
            {
                Ret.LongitudionalElasticity = this.LongitudionalElasticity.Clone() as Macroskopic.Elasticity;
            }
            if (this.TransversalElasticity != null)
            {
                Ret.TransversalElasticity = this.TransversalElasticity.Clone() as Macroskopic.Elasticity;
            }


            Ret._classicChi2 = this._classicChi2;
            Ret._classicS1Error = this._classicS1Error;
            Ret._classicS2Error = this._classicS2Error;

            return Ret;
        }

        #endregion
    }
}
