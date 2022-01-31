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

        public static void AutoSetDEC(Analysis.Sample sample, bool texture, bool stressPartitioning)
        {
            for (int n = 0; n < sample.CrystalData.Count; n++)
            {

                if (texture)
                {
                    sample.DiffractionConstantsTexture[n].Clear();
                }
                else
                {
                    sample.DiffractionConstants[n].Clear();
                }

                for (int i = 0; i < sample.CrystalData[n].HKLList.Count; i++)
                {
                    Analysis.Stress.Microsopic.REK ActualREK = new Analysis.Stress.Microsopic.REK(sample.CrystalData[n], sample.CrystalData[n].HKLList[i]);

                    for (int j = 0; j < sample.DiffractionPatterns.Count; j++)
                    {
                        for (int k = 0; k < sample.DiffractionPatterns[j].FoundPeaks.Count; k++)
                        {
                            if (sample.DiffractionPatterns[j].FoundPeaks[k].AssociatedCrystalData.SymmetryGroupID == sample.CrystalData[n].SymmetryGroupID)
                            {
                                if (sample.DiffractionPatterns[j].FoundPeaks[k].AssociatedHKLReflex.HKLString == sample.CrystalData[n].HKLList[i].HKLString)
                                {
                                    Analysis.Stress.Macroskopic.PeakStressAssociation NewAssociation = new Analysis.Stress.Macroskopic.PeakStressAssociation(sample.DiffractionPatterns[j].Stress, sample.DiffractionPatterns[j].PsiAngle(sample.DiffractionPatterns[j].FoundPeaks[k].Angle), sample.DiffractionPatterns[j].FoundPeaks[k], sample.DiffractionPatterns[j].PhiAngle(sample.DiffractionPatterns[j].FoundPeaks[k].Angle));
                                    NewAssociation._macroskopicStrain = sample.DiffractionPatterns[j].MacroStrain;
                                    ActualREK.ElasticStressData.Add(NewAssociation);
                                }
                            }
                        }
                    }

                    for (int j = 0; j < sample.MacroElasticData.Count; j++)
                    {
                        if (sample.MacroElasticData[j][0].HKLAssociation == ActualREK.HKLAssociation)
                        {
                            if (sample.MacroElasticData[j][0].PsiAngle == 0)
                            {
                                ActualREK.LongitudionalElasticity = sample.MacroElasticData[j];
                            }
                            else if (sample.MacroElasticData[j][0].PsiAngle == 90)
                            {
                                ActualREK.TransversalElasticity = sample.MacroElasticData[j];
                            }
                        }
                    }
                    if (texture)
                    {
                        sample.DiffractionConstantsTexture[n].Add(ActualREK);
                    }
                    else
                    {
                        sample.DiffractionConstants[n].Add(ActualREK);
                    }
                }

                //Removing empty DEC
                bool run = true;
                while (run)
                {
                    int deleteIndex = -1;
                    if (texture)
                    {
                        for (int i = 0; i < sample.DiffractionConstantsTexture[n].Count; i++)
                        {

                            if (sample.DiffractionConstantsTexture[n][i].ElasticStressData.Count == 0)
                            {
                                deleteIndex = i;
                            }

                            if (deleteIndex > -1)
                            {
                                sample.DiffractionConstantsTexture[n].RemoveAt(deleteIndex);
                                break;
                            }
                        }
                        if (deleteIndex == -1)
                        {
                            run = false;
                            break;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < sample.DiffractionConstants[n].Count; i++)
                        {

                            if (sample.DiffractionConstants[n][i].ElasticStressData.Count == 0)
                            {
                                deleteIndex = i;
                            }

                            if (deleteIndex > -1)
                            {
                                sample.DiffractionConstants[n].RemoveAt(deleteIndex);
                                break;
                            }
                        }
                        if (deleteIndex == -1)
                        {
                            run = false;
                            break;
                        }
                    }
                }

                if (texture)
                {
                    for (int i = 0; i < sample.DiffractionConstantsTexture[n].Count; i++)
                    {
                        if (stressPartitioning)
                        {
                            sample.ODFList[n].SetMRDValues(sample.DiffractionConstantsTexture[n][i].ElasticStressData);
                            sample.DiffractionConstantsTexture[n][i].FitTexturedPhaseREKFunction();
                        }
                        else
                        {
                            sample.ODFList[n].SetMRDValues(sample.DiffractionConstantsTexture[n][i].ElasticStressData);
                            sample.DiffractionConstantsTexture[n][i].FitTexturedREKFunction();
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < sample.DiffractionConstants[n].Count; i++)
                    {
                        if (Convert.ToBoolean(stressPartitioning))
                        {
                            sample.DiffractionConstants[n][i].FitClassicPhaseREKFunction();
                        }
                        else
                        {
                            sample.DiffractionConstants[n][i].FitClassicREKFunction();
                        }
                    }
                }
            }
        }

        public static List<int> IntegrateIndices(double psi, List<Analysis.Stress.Plasticity.GrainOrientationParameter> orientations)
        {
            List<int> ret = new List<int>();

            for(int n = 0; n < orientations.Count; n++)
            {
                if(orientations[n].Psi == psi)
                {
                    ret.Add(n);
                }
            }

            return ret;
        }

        public static double GetEstimatedFWHM(double angle)
        {
            double Angle = angle / 2.0;
            double Ret = CalScec.Properties.Settings.Default.FWHMU * Math.Pow(Math.Tan(Angle * Math.PI / 180.0), 2);
            Ret += CalScec.Properties.Settings.Default.FWHMV * Math.Tan(Angle * Math.PI / 180.0);
            Ret += CalScec.Properties.Settings.Default.FWHMW;

            Ret = Math.Sqrt(Ret);

            if(Ret > 0.7)
            {
                Ret = 0.7;
            }

            return Ret;
        }

        public static MathNet.Numerics.LinearAlgebra.Matrix<double> GetResolvingParameter(DataManagment.CrystalData.HKLReflex slipPlane, DataManagment.CrystalData.HKLReflex slipDirection)
        {
            MathNet.Numerics.LinearAlgebra.Matrix<double> ret = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
            double slipPlaneNorm = slipPlane.NormFaktor;
            double slipDirectionNorm = slipDirection.NormFaktor;

            ret[0, 0] = (slipDirection.H * slipPlane.H) * (slipPlaneNorm * slipDirectionNorm);
            ret[1, 1] = (slipDirection.K * slipPlane.K) * (slipPlaneNorm * slipDirectionNorm);
            ret[2, 2] = (slipDirection.L * slipPlane.L) * (slipPlaneNorm * slipDirectionNorm);

            ret[0, 1] = slipDirection.H * slipPlane.K * (slipPlaneNorm * slipDirectionNorm);
            ret[0, 1] += slipDirection.K * slipPlane.H * (slipPlaneNorm * slipDirectionNorm);
            ret[0, 1] *= 0.5;

            ret[1, 0] = slipDirection.H * slipPlane.K * (slipPlaneNorm * slipDirectionNorm);
            ret[1, 0] += slipDirection.K * slipPlane.H * (slipPlaneNorm * slipDirectionNorm);
            ret[1, 0] *= 0.5;

            ret[0, 2] = slipDirection.H * slipPlane.L * (slipPlaneNorm * slipDirectionNorm);
            ret[0, 2] += slipDirection.L * slipPlane.H * (slipPlaneNorm * slipDirectionNorm);
            ret[0, 2] *= 0.5;

            ret[2, 0] = slipDirection.H * slipPlane.L * (slipPlaneNorm * slipDirectionNorm);
            ret[2, 0] += slipDirection.L * slipPlane.H * (slipPlaneNorm * slipDirectionNorm);
            ret[2, 0] *= 0.5;

            ret[1, 2] = slipDirection.K * slipPlane.L * (slipPlaneNorm * slipDirectionNorm);
            ret[1, 2] += slipDirection.L * slipPlane.K * (slipPlaneNorm * slipDirectionNorm);
            ret[1, 2] *= 0.5;

            ret[2, 1] = slipDirection.K * slipPlane.L * (slipPlaneNorm * slipDirectionNorm);
            ret[2, 1] += slipDirection.L * slipPlane.K * (slipPlaneNorm * slipDirectionNorm);
            ret[2, 1] *= 0.5;

            return ret;
        }

        /// <summary>
        /// Alpha, multiplied with the applied stress gets the resolved shear stress
        /// </summary>
        /// <param name="slipPlane"></param>
        /// <param name="slipDirection"></param>
        /// <returns>alpha</returns>
        public static MathNet.Numerics.LinearAlgebra.Matrix<double> GetResolvingParameterMainHex(DataManagment.CrystalData.HKLReflex slipPlane, DataManagment.CrystalData.HKLReflex slipDirection, double a, double c)
        {
            MathNet.Numerics.LinearAlgebra.Matrix<double> ret = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

            double normFactorPlane = Math.Pow(a, 2) * (Math.Pow(slipPlane.H, 2) + Math.Pow(slipPlane.K, 2));
            normFactorPlane += Math.Pow(c, 2) * Math.Pow(slipPlane.L, 2);
            normFactorPlane = Math.Sqrt(normFactorPlane);
            double normFactorDirection = Math.Pow(a, 2) * (Math.Pow(slipDirection.H, 2) + Math.Pow(slipDirection.K, 2));
            normFactorDirection += Math.Pow(c, 2) * Math.Pow(slipDirection.L, 2);
            normFactorDirection = Math.Sqrt(normFactorDirection);

            double hStarPlane = (a * slipPlane.H) / normFactorPlane;
            double kStarPlane = (a * slipPlane.K) / normFactorPlane;
            double lStarPlane = (c * slipPlane.L) / normFactorPlane;
            double hStarDirection = (a * slipDirection.H) / normFactorDirection;
            double kStarDirection = (a * slipDirection.K) / normFactorDirection;
            double lStarDirection = (c * slipDirection.L) / normFactorDirection;
            
            if (normFactorPlane != 0 && normFactorDirection != 0)
            {
                ret[0, 0] = hStarPlane * hStarDirection;
                ret[1, 1] = kStarPlane * kStarDirection;
                ret[2, 2] = lStarPlane * lStarDirection;

                ret[0, 1] = hStarDirection * kStarPlane;
                ret[0, 1] += kStarDirection * hStarPlane;
                ret[0, 1] *= 0.5;

                ret[1, 0] = hStarDirection * kStarPlane;
                ret[1, 0] += kStarDirection * hStarPlane;
                ret[1, 0] *= 0.5;

                ret[0, 2] = hStarDirection * lStarPlane;
                ret[0, 2] += lStarDirection * hStarPlane;
                ret[0, 2] *= 0.5;

                ret[2, 0] = hStarDirection * lStarPlane;
                ret[2, 0] += lStarDirection * hStarPlane;
                ret[2, 0] *= 0.5;

                ret[1, 2] = kStarDirection * lStarPlane;
                ret[1, 2] += lStarDirection * kStarPlane;
                ret[1, 2] *= 0.5;

                ret[2, 1] = kStarDirection * lStarPlane;
                ret[2, 1] += lStarDirection * kStarPlane;
                ret[2, 1] *= 0.5;
            }

            return ret;
        }


        /// <summary>
        /// Alpha, multiplied with the applied stress gets the resolved shear stress
        /// </summary>
        /// <param name="slipPlane"></param>
        /// <param name="slipDirection"></param>
        /// <returns>alpha</returns>
        public static MathNet.Numerics.LinearAlgebra.Matrix<double> GetResolvingParameterMainHexTestAC2(DataManagment.CrystalData.HKLReflex slipPlane, DataManagment.CrystalData.HKLReflex slipDirection, double a, double c)
        {
            MathNet.Numerics.LinearAlgebra.Matrix<double> ret = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
            double slipPlaneNorm = slipPlane.NormFaktor;
            double slipDirectionNorm = slipDirection.NormFaktor;

            double aFactor = 1 / a;
            double cFactor = 1 / c;
            if (slipPlaneNorm != 0 && slipDirectionNorm != 0)
            {
                ret[0, 0] = Math.Pow(aFactor, 2) * (slipDirection.H * slipPlane.H) * (slipPlaneNorm * slipDirectionNorm);
                ret[1, 1] = Math.Pow(aFactor, 2) * (slipDirection.K * slipPlane.K) * (slipPlaneNorm * slipDirectionNorm);
                ret[2, 2] = (slipDirection.L * slipPlane.L) * (slipPlaneNorm * slipDirectionNorm);

                ret[0, 1] = Math.Pow(aFactor, 2) * slipDirection.H * slipPlane.K * (slipPlaneNorm * slipDirectionNorm);
                ret[0, 1] += Math.Pow(aFactor, 2) * slipDirection.K * slipPlane.H * (slipPlaneNorm * slipDirectionNorm);
                ret[0, 1] *= 0.5;

                ret[1, 0] = Math.Pow(aFactor, 2) * slipDirection.H * slipPlane.K * (slipPlaneNorm * slipDirectionNorm);
                ret[1, 0] += Math.Pow(aFactor, 2) * slipDirection.K * slipPlane.H * (slipPlaneNorm * slipDirectionNorm);
                ret[1, 0] *= 0.5;

                ret[0, 2] = aFactor * cFactor * slipDirection.H * slipPlane.L * (slipPlaneNorm * slipDirectionNorm);
                ret[0, 2] += aFactor * cFactor * slipDirection.L * slipPlane.H * (slipPlaneNorm * slipDirectionNorm);
                ret[0, 2] *= 0.5;

                ret[2, 0] = aFactor * cFactor * slipDirection.H * slipPlane.L * (slipPlaneNorm * slipDirectionNorm);
                ret[2, 0] += aFactor * cFactor * slipDirection.L * slipPlane.H * (slipPlaneNorm * slipDirectionNorm);
                ret[2, 0] *= 0.5;

                ret[1, 2] = aFactor * cFactor * slipDirection.K * slipPlane.L * (slipPlaneNorm * slipDirectionNorm);
                ret[1, 2] += aFactor * cFactor * slipDirection.L * slipPlane.K * (slipPlaneNorm * slipDirectionNorm);
                ret[1, 2] *= 0.5;

                ret[2, 1] = aFactor * cFactor * slipDirection.K * slipPlane.L * (slipPlaneNorm * slipDirectionNorm);
                ret[2, 1] += aFactor * cFactor * slipDirection.L * slipPlane.K * (slipPlaneNorm * slipDirectionNorm);
                ret[2, 1] *= 0.5;
                
            }

            return ret;
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
