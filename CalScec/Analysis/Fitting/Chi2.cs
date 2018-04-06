using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.Analysis.Fitting
{
    public static class Chi2
    {
        #region Peaks and regions

        private static double Chi2PeakFunctionPre(Peaks.Functions.PeakFunction PF)
        {
            double Ret = 0;

            if (PF.FittingCounts[0].Length < 3)
            {
                for (int n = 0; n < PF.FittingCounts.Count; n++)
                {
                    Ret += Math.Pow((PF.FittingCounts[n][1] - PF.Y(PF.FittingCounts[n][0])) / Math.Sqrt(PF.FittingCounts[n][1]), 2);
                }
            }
            else
            {
                for (int n = 0; n < PF.FittingCounts.Count; n++)
                {
                    Ret += Math.Pow((PF.FittingCounts[n][1] - PF.Y(PF.FittingCounts[n][0])) / PF.FittingCounts[n][2], 2);
                }
            }

            return Ret;
        }

        public static double Chi2PeakFunction(Peaks.Functions.PeakFunction PF)
        {
            double Ret = Chi2.Chi2PeakFunctionPre(PF);

            return Ret / PF.FittingCounts.Count;
        }

        public static double ReducedChi2PeakFunction(Peaks.Functions.PeakFunction PF)
        {
            double Ret = Chi2.Chi2PeakFunctionPre(PF);

            if(PF.backgroundSwitch)
            {
                switch (PF.functionType)
                {
                    case 0:
                        Ret /= PF.FittingCounts.Count - 4;
                        break;
                    case 1:
                        goto case 0;
                    case 2:
                        goto default;
                    default:
                        Ret /= PF.FittingCounts.Count - 5;
                        break;
                }
            }
            else
            {
                switch (PF.functionType)
                {
                    case 0:
                        Ret /= PF.FittingCounts.Count;
                        break;
                    case 1:
                        goto case 0;
                    case 2:
                        goto default;
                    default:
                        Ret /= PF.FittingCounts.Count - 2;
                        break;
                }
            }

            return Ret;
        }

        private static double Chi2PeakRegionPre(Peaks.Functions.PeakRegionFunction PRF)
        {
            double Ret = 0;

            if (PRF.FittingCounts[0].Length < 3)
            {
                for (int n = 0; n < PRF.FittingCounts.Count; n++)
                {
                    Ret += Math.Pow((PRF.FittingCounts[n][1] - PRF.Y(PRF.FittingCounts[n][0])) / Math.Sqrt(PRF.FittingCounts[n][1]), 2);
                }
            }
            else
            {
                for (int n = 0; n < PRF.FittingCounts.Count; n++)
                {
                    Ret += Math.Pow((PRF.FittingCounts[n][1] - PRF.Y(PRF.FittingCounts[n][0])) / PRF.FittingCounts[n][2], 2);
                }
            }

            return Ret;
        }

        public static double Chi2PeakRegion(Peaks.Functions.PeakRegionFunction PRF)
        {
            double Ret = Chi2PeakRegionPre(PRF);

            return Ret / PRF.FittingCounts.Count;
        }

        public static double ReducedChi2PeakRegion(Peaks.Functions.PeakRegionFunction PRF)
        {
            double Ret = Chi2PeakRegionPre(PRF);

            if(PRF.backgroundSwitch)
            {
                switch (PRF.functionType)
                {
                    case 0:
                        Ret /= PRF.FittingCounts.Count - ((PRF.Count * 2) + 1);
                        break;
                    case 1:
                        goto case 0;
                    case 2:
                        goto default;
                    default:
                        Ret /= PRF.FittingCounts.Count - ((PRF.Count * 3) + 1);
                        break;
                }
            }
            else
            {
                switch (PRF.functionType)
                {
                    case 0:
                        Ret /= PRF.FittingCounts.Count - ((PRF.Count * 2) - 2);
                        break;
                    case 1:
                        goto case 0;
                    case 2:
                        goto default;
                    default:
                        Ret /= PRF.FittingCounts.Count - ((PRF.Count * 3) - 2);
                        break;
                }
            }

            return Ret;
        }

        #endregion

        public static double Chi2LinearFunction(LinearFunction LF, Pattern.Counts UsedCounts)
        {
            double Ret = 0;

            if (UsedCounts.Count > 0)
            {
                if (UsedCounts[0].Length < 3)
                {
                    for (int n = 0; n < UsedCounts.Count; n++)
                    {
                        Ret += Math.Pow((UsedCounts[n][1] - LF.Y(UsedCounts[n][0])) / Math.Sqrt(UsedCounts[n][1]), 2);
                    }
                }
                else
                {
                    for (int n = 0; n < UsedCounts.Count; n++)
                    {
                        Ret += Math.Pow((UsedCounts[n][1] - LF.Y(UsedCounts[n][0])) / UsedCounts[n][2], 2);
                    }
                }
            }

            return Ret;
        }

        #region Elastic constants

        #region Voigt

        public static double Chi2ClassicVoigtCubicIsotrope(Stress.Microsopic.ElasticityTensors ET)
        {
            double Ret = 0;
            for(int n = 0; n < ET.DiffractionConstants.Count; n++)
            {
                Ret += Math.Pow(ET.S1VoigtCubicIsotrope() - ET.DiffractionConstants[n].ClassicS1 / ET.DiffractionConstants[n].ClassicS1Error, 2);
                Ret += Math.Pow(ET.HS2VoigtCubicIsotrope() - ET.DiffractionConstants[n].ClassicHS2 / ET.DiffractionConstants[n].ClassicHS2Error, 2);
            }

            return Ret / (ET.DiffractionConstants.Count - 1);
        }

        public static double Chi2ClassicVoigtCubic(Stress.Microsopic.ElasticityTensors ET)
        {
            double Ret = 0;
            for (int n = 0; n < ET.DiffractionConstants.Count; n++)
            {
                double CS1 = ET.S1VoigtCubic();
                double CHS2 = ET.HS2VoigtCubic();
                CS1 -= ET.DiffractionConstants[n].ClassicS1;
                CHS2 -= ET.DiffractionConstants[n].ClassicHS2;
                CS1 /= ET.DiffractionConstants[n].ClassicS1Error;
                CHS2 /= ET.DiffractionConstants[n].ClassicHS2Error;
                Ret += Math.Pow(CS1, 2);
                Ret += Math.Pow(CHS2, 2);
            }

            return Ret / (ET.DiffractionConstants.Count - 2);
        }

        public static double Chi2ClassicVoigtType1(Stress.Microsopic.ElasticityTensors ET)
        {
            double Ret = 0;
            for (int n = 0; n < ET.DiffractionConstants.Count; n++)
            {
                double CS1 = ET.S1VoigtType1();
                double CHS2 = ET.HS2VoigtType1();
                CS1 -= ET.DiffractionConstants[n].ClassicS1;
                CHS2 -= ET.DiffractionConstants[n].ClassicHS2;
                CS1 /= ET.DiffractionConstants[n].ClassicS1Error;
                CHS2 /= ET.DiffractionConstants[n].ClassicHS2Error;
                Ret += Math.Pow(CS1, 2);
                Ret += Math.Pow(CHS2, 2);
            }

            return Ret / (ET.DiffractionConstants.Count - 4);
        }

        public static double Chi2ClassicVoigtType2(Stress.Microsopic.ElasticityTensors ET)
        {
            double Ret = 0;
            for (int n = 0; n < ET.DiffractionConstants.Count; n++)
            {
                double CS1 = ET.S1VoigtType2();
                double CHS2 = ET.HS2VoigtType2();
                CS1 -= ET.DiffractionConstants[n].ClassicS1;
                CHS2 -= ET.DiffractionConstants[n].ClassicHS2;
                CS1 /= ET.DiffractionConstants[n].ClassicS1Error;
                CHS2 /= ET.DiffractionConstants[n].ClassicHS2Error;
                Ret += Math.Pow(CS1, 2);
                Ret += Math.Pow(CHS2, 2);
            }

            return Ret / (ET.DiffractionConstants.Count - 5);
        }

        public static double Chi2ClassicVoigtType3(Stress.Microsopic.ElasticityTensors ET)
        {
            double Ret = 0;
            for (int n = 0; n < ET.DiffractionConstants.Count; n++)
            {
                double CS1 = ET.S1VoigtType3();
                double CHS2 = ET.HS2VoigtType3();
                CS1 -= ET.DiffractionConstants[n].ClassicS1;
                CHS2 -= ET.DiffractionConstants[n].ClassicHS2;
                CS1 /= ET.DiffractionConstants[n].ClassicS1Error;
                CHS2 /= ET.DiffractionConstants[n].ClassicHS2Error;
                Ret += Math.Pow(CS1, 2);
                Ret += Math.Pow(CHS2, 2);
            }

            return Ret / (ET.DiffractionConstants.Count - 8);
        }

        public static double Chi2StrainVoigtCubic(Stress.Microsopic.ElasticityTensors ET)
        {
            double Ret = 0;
            for (int n = 0; n < ET.UsedPSA.Count; n++)
            {
                double CS1 = ET.UsedPSA[n]._Strain;
                CS1 -= ET.GetStrainCubic(ET.UsedPSA[n], ET);
                CS1 /= ET.UsedPSA[n]._StrainError;
                Ret += Math.Pow(CS1, 2);
            }

            return Ret / (ET.UsedPSA.Count - 2);
        }

        #endregion

        #region Reuss

        //public static double Chi2ClassicReussCubicIsotrope(Stress.Microsopic.ElasticityTensors ET)
        //{
        //    double Ret = 0;
        //    for (int n = 0; n < ET.DiffractionConstants.Count; n++)
        //    {
        //        Ret += Math.Pow(ET.S1ReussCubicIsotrope() - ET.DiffractionConstants[n].ClassicS1 / ET.DiffractionConstants[n].ClassicS1Error, 2);
        //        Ret += Math.Pow(ET.HS2VoigtCubicIsotrope() - ET.DiffractionConstants[n].ClassicHS2 / ET.DiffractionConstants[n].ClassicHS2Error, 2);
        //    }

        //    return Ret / (ET.DiffractionConstants.Count - 1);
        //}

        public static double Chi2ClassicReussCubic(Stress.Microsopic.ElasticityTensors ET)
        {
            double Ret = 0;
            for (int n = 0; n < ET.DiffractionConstants.Count; n++)
            {
                double CS1 = ET.S1ReussCubic(ET.DiffractionConstants[n].UsedReflex);
                double CHS2 = ET.HS2ReussCubic(ET.DiffractionConstants[n].UsedReflex);
                CS1 -= ET.DiffractionConstants[n].ClassicS1;
                CHS2 -= ET.DiffractionConstants[n].ClassicHS2;
                CS1 /= ET.DiffractionConstants[n].ClassicS1Error;
                CHS2 /= ET.DiffractionConstants[n].ClassicHS2Error;
                Ret += Math.Pow(CS1, 2);
                Ret += Math.Pow(CHS2, 2);
            }

            return Ret / (ET.DiffractionConstants.Count - 2);
        }

        public static double Chi2ClassicReussHexagonal(Stress.Microsopic.ElasticityTensors ET)
        {
            double Ret = 0;
            for (int n = 0; n < ET.DiffractionConstants.Count; n++)
            {
                double CS1 = ET.S1ReussHexagonal(ET.DiffractionConstants[n].UsedReflex);
                double CHS2 = ET.HS2ReussHexagonal(ET.DiffractionConstants[n].UsedReflex);
                CS1 -= ET.DiffractionConstants[n].ClassicS1;
                CHS2 -= ET.DiffractionConstants[n].ClassicHS2;
                CS1 /= ET.DiffractionConstants[n].ClassicS1Error;
                CHS2 /= ET.DiffractionConstants[n].ClassicHS2Error;
                Ret += Math.Pow(CS1, 2);
                Ret += Math.Pow(CHS2, 2);
            }

            return Ret / (ET.DiffractionConstants.Count - 2);
        }

        public static double Chi2StrainReussCubic(Stress.Microsopic.ElasticityTensors ET)
        {
            double Ret = 0;
            for (int n = 0; n < ET.UsedPSA.Count; n++)
            {
                double CS1 = ET.UsedPSA[n]._Strain;
                CS1 -= ET.GetStrainCubic(ET.UsedPSA[n]);
                CS1 /= ET.UsedPSA[n]._StrainError;
                Ret += Math.Pow(CS1, 2);
            }

            return Ret / (ET.UsedPSA.Count - 2);
        }

        #endregion

        #region Hill

        public static double Chi2ClassicHillCubic(Stress.Microsopic.ElasticityTensors ET)
        {
            double Ret = 0;
            for (int n = 0; n < ET.DiffractionConstants.Count; n++)
            {
                double CS1 = ET.S1HillCubic(ET.DiffractionConstants[n].UsedReflex);
                double CHS2 = ET.HS2HillCubic(ET.DiffractionConstants[n].UsedReflex);
                CS1 -= ET.DiffractionConstants[n].ClassicS1;
                CHS2 -= ET.DiffractionConstants[n].ClassicHS2;
                CS1 /= ET.DiffractionConstants[n].ClassicS1Error;
                CHS2 /= ET.DiffractionConstants[n].ClassicHS2Error;
                Ret += Math.Pow(CS1, 2);
                Ret += Math.Pow(CHS2, 2);
            }

            return Ret / (ET.DiffractionConstants.Count - 2);
        }

        public static double Chi2ClassicHillHexagonal(Stress.Microsopic.ElasticityTensors ET)
        {
            double Ret = 0;
            for (int n = 0; n < ET.DiffractionConstants.Count; n++)
            {
                double CS1 = ET.S1HillHexagonal(ET.DiffractionConstants[n].UsedReflex);
                double CHS2 = ET.HS2HillHexagonal(ET.DiffractionConstants[n].UsedReflex);
                CS1 -= ET.DiffractionConstants[n].ClassicS1;
                CHS2 -= ET.DiffractionConstants[n].ClassicHS2;
                CS1 /= ET.DiffractionConstants[n].ClassicS1Error;
                CHS2 /= ET.DiffractionConstants[n].ClassicHS2Error;
                Ret += Math.Pow(CS1, 2);
                Ret += Math.Pow(CHS2, 2);
            }

            return Ret / (ET.DiffractionConstants.Count - 2);
        }

        #endregion

        #region  Geometric Hill

        public static double Chi2ClassicGeometricHillCubic(Stress.Microsopic.ElasticityTensors ET)
        {
            double Ret = 0;
            for (int n = 0; n < ET.DiffractionConstants.Count; n++)
            {
                double CS1 = ET.S1GeometricHillCubic(ET.DiffractionConstants[n].UsedReflex);
                double CHS2 = ET.HS2GeometricHillCubic(ET.DiffractionConstants[n].UsedReflex);
                CS1 -= ET.DiffractionConstants[n].ClassicS1;
                CHS2 -= ET.DiffractionConstants[n].ClassicHS2;
                CS1 /= ET.DiffractionConstants[n].ClassicS1Error;
                CHS2 /= ET.DiffractionConstants[n].ClassicHS2Error;
                Ret += Math.Pow(CS1, 2);
                Ret += Math.Pow(CHS2, 2);
            }

            return Ret / (ET.DiffractionConstants.Count - 2);
        }

        public static double Chi2ClassicGeometricHillHexagonal(Stress.Microsopic.ElasticityTensors ET)
        {
            double Ret = 0;
            for (int n = 0; n < ET.DiffractionConstants.Count; n++)
            {
                double CS1 = ET.S1GeometricHillHexagonal(ET.DiffractionConstants[n].UsedReflex);
                double CHS2 = ET.HS2GeometricHillHexagonal(ET.DiffractionConstants[n].UsedReflex);
                CS1 -= ET.DiffractionConstants[n].ClassicS1;
                CHS2 -= ET.DiffractionConstants[n].ClassicHS2;
                CS1 /= ET.DiffractionConstants[n].ClassicS1Error;
                CHS2 /= ET.DiffractionConstants[n].ClassicHS2Error;
                Ret += Math.Pow(CS1, 2);
                Ret += Math.Pow(CHS2, 2);
            }

            return Ret / (ET.DiffractionConstants.Count - 2);
        }

        #endregion

        #region Kroener

        public static double Chi2ClassicKroenerCubicStiffness(Stress.Microsopic.ElasticityTensors ET)
        {
            double Ret = 0;
            for (int n = 0; n < ET.DiffractionConstants.Count; n++)
            {
                double CS1 = ET.S1KroenerCubicStiffness();
                double CHS2 = ET.HS2KroenerCubicStiffness();
                CS1 -= ET.DiffractionConstants[n].ClassicS1;
                CHS2 -= ET.DiffractionConstants[n].ClassicHS2;
                CS1 /= ET.DiffractionConstants[n].ClassicS1Error;
                CHS2 /= ET.DiffractionConstants[n].ClassicHS2Error;
                Ret += Math.Pow(CS1, 2);
                Ret += Math.Pow(CHS2, 2);
            }

            return Ret / (ET.DiffractionConstants.Count - 2);
        }

        public static double Chi2ClassicKroenerCubicCompliance(Stress.Microsopic.ElasticityTensors ET)
        {
            double Ret = 0;
            for (int n = 0; n < ET.DiffractionConstants.Count; n++)
            {
                double CS1 = ET.S1KroenerCubicCompliance();
                double CHS2 = ET.HS2KroenerCubicCompliance();
                CS1 -= ET.DiffractionConstants[n].ClassicS1;
                CHS2 -= ET.DiffractionConstants[n].ClassicHS2;
                CS1 /= ET.DiffractionConstants[n].ClassicS1Error;
                CHS2 /= ET.DiffractionConstants[n].ClassicHS2Error;
                Ret += Math.Pow(CS1, 2);
                Ret += Math.Pow(CHS2, 2);
            }

            return Ret / (ET.DiffractionConstants.Count - 2);
        }

        #endregion

        #region DeWitt

        public static double Chi2ClassicDeWittCubicStiffness(Stress.Microsopic.ElasticityTensors ET)
        {
            double Ret = 0;
            for (int n = 0; n < ET.DiffractionConstants.Count; n++)
            {
                double CS1 = ET.S1DeWittCubicStiffness(ET.DiffractionConstants[n].UsedReflex);
                double CHS2 = ET.HS2DeWittCubicStiffness(ET.DiffractionConstants[n].UsedReflex);
                CS1 -= ET.DiffractionConstants[n].ClassicS1;
                CHS2 -= ET.DiffractionConstants[n].ClassicHS2;
                CS1 /= ET.DiffractionConstants[n].ClassicS1Error;
                CHS2 /= ET.DiffractionConstants[n].ClassicHS2Error;
                Ret += Math.Pow(CS1, 2);
                Ret += Math.Pow(CHS2, 2);
            }

            return Ret / (ET.DiffractionConstants.Count - 2);
        }

        public static double Chi2ClassicDeWittCubicCompliance(Stress.Microsopic.ElasticityTensors ET)
        {
            double Ret = 0;
            for (int n = 0; n < ET.DiffractionConstants.Count; n++)
            {
                double CS1 = ET.S1DeWittCubicCompliance(ET.DiffractionConstants[n].UsedReflex);
                double CHS2 = ET.HS2DeWittCubicCompliance(ET.DiffractionConstants[n].UsedReflex);
                CS1 -= ET.DiffractionConstants[n].ClassicS1;
                CHS2 -= ET.DiffractionConstants[n].ClassicHS2;
                CS1 /= ET.DiffractionConstants[n].ClassicS1Error;
                CHS2 /= ET.DiffractionConstants[n].ClassicHS2Error;
                Ret += Math.Pow(CS1, 2);
                Ret += Math.Pow(CHS2, 2);
            }

            return Ret / (ET.DiffractionConstants.Count - 2);
        }

        #endregion

        #region Texture

        public static double Chi2TextureVoigtCubic(Texture.OrientationDistributionFunction ODF)
        {
            double Ret = 0;
            for (int n = 0; n < ODF.UsedPSA.Count; n++)
            {
                double CS1 = ODF.UsedPSA[n]._Strain;
                CS1 -= ODF.GetStrainVoigtCubic(ODF.UsedPSA[n]);
                CS1 /= ODF.UsedPSA[n]._StrainError;
                Ret += Math.Pow(CS1, 2);
            }

            return Ret / (ODF.UsedPSA.Count - 2);
        }

        public static double Chi2TextureReussCubic(Texture.OrientationDistributionFunction ODF)
        {
            double Ret = 0;
            for (int n = 0; n < ODF.UsedPSA.Count; n++)
            {
                double CS1 = ODF.UsedPSA[n]._Strain;
                CS1 -= ODF.GetStrainReussCubic(ODF.UsedPSA[n]);
                CS1 /= ODF.UsedPSA[n]._StrainError;

                Ret += Math.Pow(CS1, 2);
            }

            return Ret / (ODF.UsedPSA.Count - 2);
        }

        public static double Chi2TextureHillCubic(Texture.OrientationDistributionFunction ODF)
        {
            double Ret = 0;
            for (int n = 0; n < ODF.UsedPSA.Count; n++)
            {
                double CS1 = ODF.UsedPSA[n]._Strain;
                CS1 -= ODF.GetStrainHillCubic(ODF.UsedPSA[n]);
                CS1 /= ODF.UsedPSA[n]._StrainError;
                Ret += Math.Pow(CS1, 2);
            }

            return Ret / (ODF.UsedPSA.Count - 2);
        }

        #endregion

        #endregion
    }
}
