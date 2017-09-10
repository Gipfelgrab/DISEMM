using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.Tools
{
    public static class Calculation
    {

        #region HKL Calculations

        public static void AddHKLList(DataManagment.CrystalData.CODData cODData)
        {
            DataManagment.CrystalData.ReflexCondition RC = null;
            if(cODData.SymmetryGroupID != -1)
            {
                RC = new DataManagment.CrystalData.ReflexCondition(cODData.SymmetryGroupID);
            }
            else
            {
                RC = new DataManagment.CrystalData.ReflexCondition(cODData.SymmetryGroup);
            }

            double QN = Math.Pow(cODData.A, 2) * Math.Pow(cODData.B, 2) * Math.Pow(cODData.C, 2);

            double CalcTmp = 1;
            CalcTmp -= Math.Pow(Math.Cos(cODData.AlphaRad), 2);
            CalcTmp -= Math.Pow(Math.Cos(cODData.BetaRad), 2);
            CalcTmp -= Math.Pow(Math.Cos(cODData.GammaRad), 2);
            CalcTmp += (2 * Math.Cos(cODData.AlphaRad) * Math.Cos(cODData.BetaRad) * Math.Cos(cODData.GammaRad));

            QN *= CalcTmp;

            List<double> ExistantDistances = new List<double>();

            for (int l = 0; l < CalScec.Properties.Settings.Default.MaxHKLReflection; l++)
            {
                for (int k = 0; k < CalScec.Properties.Settings.Default.MaxHKLReflection; k++)
                {
                    for (int h = 0; h < CalScec.Properties.Settings.Default.MaxHKLReflection; h++)
                    {
                        if (!(h == 0 && k == 0 && l == 0))
                        {
                            double Q = 0.0;

                            Q = Math.Pow(cODData.B * cODData.C * h * Math.Sin(cODData.AlphaRad), 2);
                            Q += Math.Pow(cODData.A * cODData.C * k * Math.Sin(cODData.BetaRad), 2);
                            Q += Math.Pow(cODData.B * cODData.A * l * Math.Sin(cODData.GammaRad), 2);

                            CalcTmp = (Math.Cos(cODData.AlphaRad) * Math.Cos(cODData.BetaRad)) - Math.Cos(cODData.GammaRad);
                            CalcTmp *= 2 * cODData.A * cODData.B * Math.Pow(cODData.C, 2) * h * k;
                            Q += CalcTmp;

                            CalcTmp = (Math.Cos(cODData.AlphaRad) * Math.Cos(cODData.GammaRad)) - Math.Cos(cODData.BetaRad);
                            CalcTmp *= 2 * cODData.A * cODData.C * Math.Pow(cODData.B, 2) * h * l;
                            Q += CalcTmp;

                            CalcTmp = (Math.Cos(cODData.BetaRad) * Math.Cos(cODData.GammaRad)) - Math.Cos(cODData.AlphaRad);
                            CalcTmp *= 2 * cODData.B * cODData.C * Math.Pow(cODData.A, 2) * k * l;
                            Q += CalcTmp;

                            double Distance = Math.Sqrt(QN / Q);
                            bool DistanceFound = false;

                            foreach (double ED in ExistantDistances)
                            {
                                if (Math.Abs(ED - Distance) < 0.001)
                                {
                                    DistanceFound = true;
                                    break;
                                }
                            }

                            if (!DistanceFound)
                            {
                                int[] NewHKL = { h, k, l };
                                ExistantDistances.Add(Distance);

                                DataManagment.CrystalData.HKLReflex Tmp = new DataManagment.CrystalData.HKLReflex(NewHKL, Distance);
                                if (Tmp.EstimatedAngle < CalScec.Properties.Settings.Default.MaxMeasurmentAngle)
                                {
                                    if (RC.CheckHKLReflex(Tmp))
                                    {
                                        cODData.HKLList.Add(Tmp);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            cODData.HKLList.Sort((A, B) => (1) * (A.EstimatedAngle).CompareTo(B.EstimatedAngle));
        }

        public static double CalculateHKLDistance(int h, int k, int l, double a, double b, double c, double alphaRad, double betaRad, double gammaRad)
        {
            double ret = 0.0;

            if (!(h == 0 && k == 0 && l == 0))
            {
                double QN = Math.Pow(a, 2) * Math.Pow(b, 2) * Math.Pow(c, 2);
                QN *= 1 - Math.Pow(Math.Cos(alphaRad), 2) - Math.Pow(Math.Cos(betaRad), 2) - Math.Pow(Math.Cos(gammaRad), 2) + (2 * Math.Cos(alphaRad) * Math.Cos(betaRad) * Math.Cos(gammaRad));

                double Q = 0.0;

                Q += Math.Pow(b * c * h * Math.Sin(alphaRad), 2);
                Q += Math.Pow(a * c * k * Math.Sin(betaRad), 2);
                Q += Math.Pow(b * a * l * Math.Sin(gammaRad), 2);

                Q += (2 * a * b * Math.Pow(c, 2) * h * k) * ((Math.Cos(alphaRad) * Math.Cos(betaRad)) - Math.Cos(gammaRad));
                Q += (2 * a * c * Math.Pow(b, 2) * h * l) * ((Math.Cos(alphaRad) * Math.Cos(gammaRad)) - Math.Cos(betaRad));
                Q += (2 * b * c * Math.Pow(a, 2) * k * l) * ((Math.Cos(betaRad) * Math.Cos(gammaRad)) - Math.Cos(alphaRad));

                ret = Math.Sqrt(QN / Q);
            }

            return ret;
        }

        public static double CalculateHKLDistance(int h, int k, int l, DataManagment.CrystalData.CODData cODData)
        {
            double ret = 0.0;

            if (!(h == 0 && k == 0 && l == 0))
            {
                double QN = Math.Pow(cODData.A, 2) * Math.Pow(cODData.B, 2) * Math.Pow(cODData.C, 2);
                QN *= (1 - Math.Pow(Math.Cos(cODData.AlphaRad), 2) - Math.Pow(Math.Cos(cODData.BetaRad), 2) - Math.Pow(Math.Cos(cODData.GammaRad), 2) + (2 * Math.Cos(cODData.AlphaRad) * Math.Cos(cODData.BetaRad) * Math.Cos(cODData.GammaRad)));

                double Q = 0.0;

                Q += Math.Pow(cODData.B * cODData.C * h * Math.Sin(cODData.AlphaRad), 2);
                Q += Math.Pow(cODData.A * cODData.C * k * Math.Sin(cODData.BetaRad), 2);
                Q += Math.Pow(cODData.B * cODData.A * l * Math.Sin(cODData.GammaRad), 2);

                Q += (2 * cODData.A * cODData.B * Math.Pow(cODData.C, 2) * h * k) * ((Math.Cos(cODData.AlphaRad) * Math.Cos(cODData.BetaRad)) - Math.Cos(cODData.GammaRad));
                Q += (2 * cODData.A * cODData.C * Math.Pow(cODData.B, 2) * h * l) * ((Math.Cos(cODData.AlphaRad) * Math.Cos(cODData.GammaRad)) - Math.Cos(cODData.BetaRad));
                Q += (2 * cODData.B * cODData.C * Math.Pow(cODData.A, 2) * k * l) * ((Math.Cos(cODData.BetaRad) * Math.Cos(cODData.GammaRad)) - Math.Cos(cODData.AlphaRad));

                ret = Math.Sqrt(QN / Q);
            }

            return ret;
        }

        #endregion

        public static double GetEstimatedFWHM(double angle)
        {
            double Angle = angle / 2.0;
            double Ret = CalScec.Properties.Settings.Default.FWHMU * Math.Pow(Math.Tan(Angle), 2);
            Ret += CalScec.Properties.Settings.Default.FWHMV * Math.Tan(Angle);
            Ret += CalScec.Properties.Settings.Default.FWHMW;

            Ret = Math.Sqrt(Ret);

            if(Ret > 1)
            {
                Ret = 1;
            }

            return Ret;
        }
    }

    public class BulkElasticPhaseEvaluations
    {
        public string HKLPase
        {
            get;
            set;
        }

        public double BulkElasticity
        {
            get;
            set;
        }
        public double BulkElasticityError
        {
            get;
            set;
        }

        public double PsiAngle
        {
            get;
            set;
        }
    }
}
