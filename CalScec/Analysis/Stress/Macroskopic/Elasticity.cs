using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.Analysis.Stress.Macroskopic
{
    public class Elasticity : List<PeakStressAssociation>, ICloneable
    {
        public double EModulError
        {
            get
            {
                if (this.FittingFunction != null)
                {
                    return this.FittingFunction.AclivityError;
                }
                else
                {
                    return 0.0;
                }
            }
        }
        public double EModul
        {
            get
            {
                if (this.FittingFunction != null)
                {
                    return this.FittingFunction.Aclivity;
                }
                else
                {
                    return 0.0;
                }
            }
        }

        public double PsiAngle
        {
            get
            {
                if(this.Count > 0)
                {
                    return this[0].PsiAngle;
                }
                else
                {
                    return -1.0;
                }
            }
        }

        public Fitting.LinearFunction FittingFunction;

        public double DInital
        {
            get
            {
                double[] ret = { 0, Double.MaxValue };

                for(int n = 0; n < this.Count; n++)
                {
                    if(this[n].Stress < ret[1])
                    {
                        ret[0] = this[n].DPeak.LatticeDistance;
                        ret[1] = this[n].Stress;
                    }
                }

                return ret[0];
            }
        }

        public Pattern.Counts CalculatedCounts
        {
            get
            {
                Pattern.Counts ret = new Pattern.Counts();

                double dInital = DInital;

                for(int n = 0; n < this.Count; n++)
                {
                    double[] Tmp = { (this[n].DPeak.LatticeDistance - dInital) / dInital, this[n].Stress, this[n].DPeak.LatticeDistanceError };

                    ret.Add(Tmp);
                }

                return ret;
            }
        }

        public Elasticity()
        {
            FittingFunction = new Fitting.LinearFunction(1);
        }

        public Elasticity(List<Peaks.DiffractionPeak> DPList)
        {
            for(int n = 0; n < DPList.Count; n++)
            {
                double appStress = n;
                double useAngle = n;
                double usePhiAngle = n;

                this.Add(new PeakStressAssociation(appStress, useAngle, DPList[n], usePhiAngle));
            }

            FittingFunction = new Fitting.LinearFunction(1);
        }

        public Elasticity(DataManagment.Files.SCEC.MacroElasticInformation MEI)
        {
            this.FittingFunction = MEI.FittingFunction;

            for(int n = 0; n < MEI.Count; n++)
            {
                this.Add(new PeakStressAssociation(MEI[n]));
            }
        }

        public bool FitConverged;
        public void FitToCounts()
        {
            if(this.Count == 2)
            {
                Pattern.Counts UsedCounts = this.CalculatedCounts;

                double aclivity = (UsedCounts[0][1] - UsedCounts[1][1]) / (UsedCounts[0][0] - UsedCounts[1][0]);

                double _constant = UsedCounts[0][1] - (aclivity * UsedCounts[0][0]);

                FittingFunction.Aclivity = aclivity;
                FittingFunction.Constant = _constant;

                MathNet.Numerics.LinearAlgebra.Matrix<double> ErrorMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(2, 2, 0.0);

                double aclivityError = Math.Pow((1 / (UsedCounts[0][0] - UsedCounts[1][0])) * UsedCounts[0][2], 2) + Math.Pow((-1 / (UsedCounts[0][0] - UsedCounts[1][0])) * UsedCounts[1][2], 2);
                ErrorMatrix[0, 0] = Math.Sqrt(aclivityError);

                double ConstantError = Math.Pow(UsedCounts[0][0] * aclivityError, 2) + Math.Pow(UsedCounts[0][2], 2);

                ErrorMatrix[1, 1] = Math.Sqrt(ConstantError);

                FittingFunction._hessianMatrix = ErrorMatrix;
            }
            else if (this.Count > 2)
            {
                FitConverged = Fitting.LMA.FitMacroElasticModul(this.FittingFunction, this.CalculatedCounts);
            }
            else
            {
                this.FitConverged = false;
            }
        }

        public object Clone()
        {
            Elasticity Ret = new Elasticity();
            Ret.FittingFunction = this.FittingFunction.Clone() as Fitting.LinearFunction;

            for(int n = 0; n < this.Count; n++)
            {
                Ret.Add(new PeakStressAssociation(this[n].Stress, this[n].PsiAngle, this[n].DPeak.Clone() as Analysis.Peaks.DiffractionPeak, this[n].phiAngle));
            }

            return Ret;
        }
    }
    
    public class PeakStressAssociation
    {
        #region View

        public string psiAngleView
        {
            get
            {
                return this.psiAngle.ToString("F3");
            }
        }
        public string strainView
        {
            get
            {
                return this.Strain.ToString("F3");
            }
        }

        #endregion

        public double PhaseFractionStress;
        public double Stress;
        public string stress
        {
            get
            {
                return this.Stress.ToString("F3");
            }
        }

        public double PsiAngle;
        public double psiAngle
        {
            get
            {
                return this.PsiAngle;
            }
        }
        public double PhiAngle;
        public double phiAngle
        {
            get
            {
                return this.PhiAngle;
            }
        }

        private double _mainSlipDirectionAngle;
        public double MainSlipDirectionAngle
        {
            get
            {
                return this._mainSlipDirectionAngle;
            }
            set
            {
                this._mainSlipDirectionAngle = value;
            }
        }
        private double _secondarySlipDirectionAngle;
        public double SecondarySlipDirectionAngle
        {
            get
            {
                return this._secondarySlipDirectionAngle;
            }
            set
            {
                this._secondarySlipDirectionAngle = value;
            }
        }

        private bool _elasticRegime;
        public bool ElasticRegime
        {
            get
            {
                return this._elasticRegime;
            }
            set
            {
                this._elasticRegime = value;
            }
        }

        public double _macroskopicStrain;
        public double MacroskopicStrain
        {
            get
            {
                return _macroskopicStrain;
            }
        }
        public double _Strain;
        public double Strain
        {
            get
            {
                return _Strain;
            }
            set
            {
                this._Strain = value;
            }
        }
        public double _StrainError;
        public double StrainError
        {
            get
            {
                return _StrainError;
            }
        }

        private double _mRDValue = 0;
        public double MRDValue
        {
            get
            {
                return this._mRDValue;
            }
            set
            {
                this._mRDValue = value;
            }
        }

        public MathNet.Numerics.LinearAlgebra.Vector<double> MeasurementDirektionVektor
        {
            get
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> Ret = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

                //Ret[0] = Math.Cos(this.PhiAngle * (Math.PI / 180.0)) * Math.Sin(this.PsiAngle * (Math.PI / 180.0));
                //Ret[1] = Math.Sin(this.PhiAngle * (Math.PI / 180.0)) * Math.Sin(this.PsiAngle * (Math.PI / 180.0));
                //Ret[2] = Math.Cos(this.PsiAngle * (Math.PI / 180.0));

                Ret[0] = Math.Cos((this.PhiAngle + 90) * (Math.PI / 180.0)) * Math.Sin(this.PsiAngle * (Math.PI / 180.0));
                Ret[1] = Math.Sin((this.PhiAngle + 90) * (Math.PI / 180.0)) * Math.Sin(this.PsiAngle * (Math.PI / 180.0));
                Ret[2] = Math.Cos(this.PsiAngle * (Math.PI / 180.0));

                return Ret;
            }
        }

        public MathNet.Numerics.LinearAlgebra.Matrix<double> CrystalSystemStress = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense<double>(3, 3, 0);
        public MathNet.Numerics.LinearAlgebra.Matrix<double> CrystalSystemStrain = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense<double>(3, 3, 0);

        public MathNet.Numerics.LinearAlgebra.Matrix<double> MeasurementSystemStress()
        {
            MathNet.Numerics.LinearAlgebra.Matrix<double> ret = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense<double>(3, 3, 0);

            ret[0, 0] = Math.Pow(Math.Sin(this.PsiAngle * (Math.PI / 180.0)), 2) * this.Stress;
            ret[0, 2] = -1 * Math.Cos(this.PsiAngle * (Math.PI / 180.0)) * Math.Sin(this.PsiAngle * (Math.PI / 180.0)) * this.Stress;
            ret[2, 0] = -1 * Math.Cos(this.PsiAngle * (Math.PI / 180.0)) * Math.Sin(this.PsiAngle * (Math.PI / 180.0)) * this.Stress;
            ret[2, 2] = Math.Pow(Math.Cos(this.PsiAngle * (Math.PI / 180.0)), 2) * this.Stress;

            return ret;
        }

        public Analysis.Peaks.DiffractionPeak DPeak;
        public Analysis.Peaks.DiffractionPeak DifPeak
        {
            get
            {
                return this.DPeak;
            }
            set
            {
                this.DPeak = value;
            }
        }

        public string HKLAssociation
        {
            get
            {
                return this.DPeak.HKLAssociation;
            }
        }

        public PeakStressAssociation(double stress, double psiAngle, Analysis.Peaks.DiffractionPeak DP)
        {
            this.Stress = stress;
            this.PsiAngle = psiAngle;
            this.PhiAngle = -1;

            this._mainSlipDirectionAngle = -1;
            this._secondarySlipDirectionAngle = -1;

            this._Strain = -1;
            this._StrainError = -1;

            this.DPeak = DP;

            this._elasticRegime = true;
            this._macroskopicStrain = -1;
        }

        public PeakStressAssociation(double stress, double psiAngle, Analysis.Peaks.DiffractionPeak DP, double phiAngle)
        {
            this.Stress = stress;
            this.PsiAngle = psiAngle;
            this.PhiAngle = phiAngle;

            this._mainSlipDirectionAngle = -1;
            this._secondarySlipDirectionAngle = -1;

            this._Strain = -1;
            this._StrainError = -1;

            this.DPeak = DP;

            this._elasticRegime = true;
            this._macroskopicStrain = -1;
        }

        public PeakStressAssociation(double stress, double psiAngle, double macroStrain, Analysis.Peaks.DiffractionPeak DP)
        {
            this.Stress = stress;
            this.PsiAngle = psiAngle;
            this.PhiAngle = -1;

            this._mainSlipDirectionAngle = -1;
            this._secondarySlipDirectionAngle = -1;

            this._Strain = -1;
            this._StrainError = -1;

            this.DPeak = DP;

            this._elasticRegime = true;
            this._macroskopicStrain = macroStrain;
        }

        public PeakStressAssociation(double stress, double psiAngle, double phiAngle, double macroStrain, Analysis.Peaks.DiffractionPeak DP)
        {
            this.Stress = stress;
            this.PsiAngle = psiAngle;
            this.PhiAngle = phiAngle;

            this._mainSlipDirectionAngle = -1;
            this._secondarySlipDirectionAngle = -1;

            this._Strain = -1;
            this._StrainError = -1;

            this.DPeak = DP;

            this._elasticRegime = true;
            this._macroskopicStrain = macroStrain;
        }

        public PeakStressAssociation(double stress, double psiAngle, double macroStrain, bool elatsticRegime, Analysis.Peaks.DiffractionPeak DP)
        {
            this.Stress = stress;
            this.PsiAngle = psiAngle;
            this.PhiAngle = -1;

            this._mainSlipDirectionAngle = -1;
            this._secondarySlipDirectionAngle = -1;

            this._Strain = -1;
            this._StrainError = -1;

            this.DPeak = DP;

            this._elasticRegime = elatsticRegime;
            this._macroskopicStrain = macroStrain;
        }

        public PeakStressAssociation(double stress, double psiAngle, double phiAngle, double macroStrain, bool elatsticRegime, Analysis.Peaks.DiffractionPeak DP)
        {
            this.Stress = stress;
            this.PsiAngle = psiAngle;
            this.PhiAngle = phiAngle;

            this._mainSlipDirectionAngle = -1;
            this._secondarySlipDirectionAngle = -1;

            this._Strain = -1;
            this._StrainError = -1;

            this.DPeak = DP;

            this._elasticRegime = elatsticRegime;
            this._macroskopicStrain = macroStrain;
        }

        public PeakStressAssociation(DataManagment.Files.SCEC.PeakStressInformation PSI)
        {
            this.Stress = PSI.Stress;
            this.PsiAngle = PSI.PsiAngle;
            this.PhiAngle = PSI.PhiAngle;

            this._mainSlipDirectionAngle = PSI.MainSlipDirectionAngle;
            this._secondarySlipDirectionAngle = PSI.SecondarySlipDirectionAngle;

            this._Strain = -1;
            this._StrainError = -1;

            this._elasticRegime = PSI._elasticRegime;
            this._macroskopicStrain = PSI._macroskopicStrain;

            this.DPeak = new Peaks.DiffractionPeak(PSI.DPeak);
        }

        public PeakStressAssociation(PeakStressAssociation PSA)
        {
            this.Stress = PSA.Stress;
            this.PsiAngle = PSA.PsiAngle;
            this.PhiAngle = PSA.PhiAngle;

            this._mainSlipDirectionAngle = PSA.MainSlipDirectionAngle;
            this._secondarySlipDirectionAngle = PSA.SecondarySlipDirectionAngle;

            this._Strain = PSA._Strain;
            this._StrainError = PSA._StrainError;

            this.MRDValue = PSA.MRDValue;

            this._elasticRegime = PSA._elasticRegime;
            this._macroskopicStrain = PSA._macroskopicStrain;

            this.DPeak = PSA.DPeak.Clone() as Analysis.Peaks.DiffractionPeak;
        }
    }
}
