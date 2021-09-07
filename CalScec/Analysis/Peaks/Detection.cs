using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.Analysis.Peaks
{
    public static class Detection
    {
        public static void HyperDetection(Pattern.DiffractionPattern DP)
        {
            List<DiffractionPeak> Ret = new List<DiffractionPeak>();
            List<DiffractionPeak> FilterList = new List<DiffractionPeak>();

            bool StartDetection = false;
            double ActualBackground = 0;

            double AngleToCount = DP.PatternCounts[1][0] - DP.PatternCounts[0][0];

            for (int n = 0; n < DP.PatternCounts.Count; n++)
            {
                if (DP.PatternCounts[n][0] > CalScec.Properties.Settings.Default.PatternLowerLimit && DP.PatternCounts[n][0] < CalScec.Properties.Settings.Default.PatternUpperLimit)
                {
                    double FWHMD = Tools.Calculation.GetEstimatedFWHM(DP.PatternCounts[n][0]);

                    int FWHMI = Convert.ToInt32(FWHMD / AngleToCount);

                    if (!((n - FWHMI) < 0) && !((n + (2 * FWHMI)) > DP.PatternCounts.Count))
                    {
                        double Kon = 0;
                        double Var = 0;

                        for (int i = n - FWHMI; i < n + (2 * FWHMI); i++)
                        {
                            int KonvCoeff = -1;

                            if ((i >= n) && (i <= (n + FWHMI - 1)))
                            {
                                KonvCoeff = 2;
                            }

                            Kon += KonvCoeff * DP.PatternCounts[i][1];
                            Var += Math.Pow(KonvCoeff, 2) * DP.PatternCounts[i][1];
                        }

                        double Conv = Kon / Math.Sqrt(Var);

                        if (Conv > CalScec.Properties.Settings.Default.HyperSensitivity)
                        {
                            if (DP.PatternCounts[n][1] / ActualBackground >= CalScec.Properties.Settings.Default.AcceptedPeakBackgroundRatio)
                            {
                                StartDetection = true;
                                Pattern.Counts NewFittingCounts = DP.PatternCounts.GetRange(DP.PatternCounts[n][0] - (FWHMD * CalScec.Properties.Settings.Default.PeakWidthFWHM), DP.PatternCounts[n][0] + (FWHMD * CalScec.Properties.Settings.Default.PeakWidthFWHM));
                                DiffractionPeak P = new DiffractionPeak(n, DP.PatternCounts[n][0], DP.PatternCounts[n][1], ActualBackground, FWHMD, NewFittingCounts);
                                FilterList.Add(P);
                            }
                        }
                        else
                        {
                            if (StartDetection)
                            {
                                StartDetection = false;

                                int HighPeakPos = 0;
                                double MaxHeight = 0;

                                for (int i = 0; i < FilterList.Count; i++)
                                {
                                    if (MaxHeight < FilterList[i].DetectedHeight)
                                    {
                                        MaxHeight = FilterList[i].DetectedHeight;
                                        HighPeakPos = i;
                                    }
                                }

                                Ret.Add(FilterList[HighPeakPos]);

                                FilterList.Clear();
                            }
                            else
                            {
                                if (n > 1)
                                {
                                    ActualBackground = (DP.PatternCounts[n - 2][1] + DP.PatternCounts[n - 1][1] + DP.PatternCounts[n][1]) / 3.0;
                                }
                                else if (n > 0)
                                {
                                    ActualBackground = (DP.PatternCounts[n - 1][1] + DP.PatternCounts[n][1]) / 2.0;
                                }
                                else
                                {
                                    ActualBackground = DP.PatternCounts[n][1];
                                }
                            }
                        }
                    }
                }
            }

            DP.FoundPeaks = Ret;
            DP.SetPeakAssociations();
        }

        private static int GetPatternLowerIndex(Pattern.DiffractionPattern DP)
        {
            int Ret = 0;

            for(int n = 0; n < DP.PatternCounts.Count; n++)
            {
                if(DP.PatternCounts[n][0] > CalScec.Properties.Settings.Default.PatternLowerLimit)
                {
                    Ret = n;
                    break;
                }
            }

            return Ret;
        }

        private static int GetPatternUpperIndex(Pattern.DiffractionPattern DP)
        {
            int Ret = 0;

            for (int n = 0; n < DP.PatternCounts.Count; n++)
            {
                if (DP.PatternCounts[n][0] > CalScec.Properties.Settings.Default.PatternUpperLimit)
                {
                    Ret = n;
                    break;
                }
            }

            return Ret;
        }

        public static void AssociateFoundPeaksToHKL(Pattern.DiffractionPattern dP, List<DataManagment.CrystalData.CODData> crystalData)
        {
            foreach(DiffractionPeak DPP in dP.FoundPeaks)
            {
                int[] HKLIndexMain = { 0, 0 };
                int[] HKLIndex1 = { 0, 0 };
                double MainDifference = Double.MaxValue;
                double DegDifference1 = Double.MaxValue;
                double DegDifference2 = Double.MaxValue;

                for(int n = 0; n < crystalData.Count; n++)
                {
                    DegDifference1 = Double.MaxValue;
                    DegDifference2 = Double.MaxValue;

                    for(int i = 0; i < crystalData[n].HKLList.Count; i++)
                    {
                        DegDifference2 = Math.Abs(DPP.DetectedAngle - crystalData[n].HKLList[i].EstimatedAngle);

                        if (DegDifference1 > DegDifference2)
                        {
                            HKLIndex1[0] = n;
                            HKLIndex1[1] = i;
                            DegDifference1 = DegDifference2;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if(MainDifference > DegDifference1)
                    {
                        HKLIndexMain[0] = HKLIndex1[0];
                        HKLIndexMain[1] = HKLIndex1[1];
                        MainDifference = DegDifference1;
                    }
                }

                if (MainDifference < CalScec.Properties.Settings.Default.HKLAssociationRangeDeg)
                {
                    DPP.AddHKLAssociation(crystalData[HKLIndexMain[0]].HKLList[HKLIndexMain[1]], crystalData[HKLIndexMain[0]]);
                }
            }
        }

        public static bool RemoveAssociationFromSimilarPeak(Pattern.DiffractionPattern dP, DiffractionPeak dPP)
        {
            int PeakIndex = 0;
            double DegDifference1 = Double.MaxValue;
            double DegDifference2 = Double.MaxValue;

            for (int n = 0; n < dP.FoundPeaks.Count; n++ )
            {
                DegDifference2 = Math.Abs(dP.FoundPeaks[n].DetectedAngle - dPP.DetectedAngle);
                if(DegDifference1 > DegDifference2)
                {
                    DegDifference1 = DegDifference2;
                    PeakIndex = n;
                }
                else
                {
                    break;
                }
            }

            if(DegDifference1 < CalScec.Properties.Settings.Default.HKLAssociationRangeDeg)
            {
                dP.FoundPeaks[PeakIndex].RemoveHKLAssociation();
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool ManualAssociationFromSimilarPeak(Pattern.DiffractionPattern dP, DiffractionPeak dPP, DataManagment.CrystalData.CODData crystalData, DataManagment.CrystalData.HKLReflex hKLReflex)
        {
            int PeakIndex = 0;
            double DegDifference1 = Double.MaxValue;
            double DegDifference2 = Double.MaxValue;

            for (int n = 0; n < dP.FoundPeaks.Count; n++)
            {
                DegDifference2 = Math.Abs(dP.FoundPeaks[n].DetectedAngle - dPP.DetectedAngle);
                if (DegDifference1 > DegDifference2)
                {
                    DegDifference1 = DegDifference2;
                    PeakIndex = n;
                }
                //else
                //{
                //    break;
                //}
            }

            if (DegDifference1 < CalScec.Properties.Settings.Default.HKLAssociationRangeDeg)
            {
                dP.FoundPeaks[PeakIndex].AddHKLAssociation(hKLReflex, crystalData);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void CIFDetection(Pattern.DiffractionPattern DP, List<DataManagment.CrystalData.CODData> CrystalData)
        {
            List<DiffractionPeak> DPeakList = new List<DiffractionPeak>();

            double AngleToCount = DP.PatternCounts[1][0] - DP.PatternCounts[0][0];

            for (int n = 0; n < CrystalData.Count; n++)
            {
                for( int i = 0; i < CrystalData[n].HKLList.Count; i++)
                {
                    if (CrystalData[n].HKLList[i].EstimatedAngle > CalScec.Properties.Settings.Default.PatternLowerLimit && CrystalData[n].HKLList[i].EstimatedAngle < CalScec.Properties.Settings.Default.PatternUpperLimit)
                    {
                        double FWHMD = Tools.Calculation.GetEstimatedFWHM(CrystalData[n].HKLList[i].EstimatedAngle);
                        int dChannel = 0;
                        double dHeight = 0.0;

                        double StartDegree = CrystalData[n].HKLList[i].EstimatedAngle - (2.0 * FWHMD);
                        if (StartDegree < CalScec.Properties.Settings.Default.PatternLowerLimit)
                        {
                            StartDegree = CalScec.Properties.Settings.Default.PatternLowerLimit;
                        }

                        double StopDegree = CrystalData[n].HKLList[i].EstimatedAngle + (2.0 * FWHMD);
                        if (StopDegree > CalScec.Properties.Settings.Default.PatternUpperLimit)
                        {
                            StopDegree = CalScec.Properties.Settings.Default.PatternUpperLimit;
                        }

                        Pattern.Counts FittingCounts = new Pattern.Counts();

                        for (int j = 0; j < DP.PatternCounts.Count; j++)
                        {
                            if (StartDegree < DP.PatternCounts[j][0])
                            {
                                if (StopDegree > DP.PatternCounts[j][0])
                                {
                                    FittingCounts.Add(DP.PatternCounts[j]);

                                    if (dHeight < DP.PatternCounts[j][1])
                                    {
                                        dHeight = DP.PatternCounts[j][1];
                                        dChannel = j;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }

                        double dBackground = FittingCounts[0][1];

                        DiffractionPeak DPeak = new DiffractionPeak(dChannel, CrystalData[n].HKLList[i].EstimatedAngle, dHeight, dBackground, FittingCounts);

                        DPeak.AddHKLAssociation(CrystalData[n].HKLList[i], CrystalData[n]);

                        DPeak.PFunction.functionType = 2;

                        DPeakList.Add(DPeak);
                    }
                }
            }

            DP.FoundPeaks = DPeakList;
            DP.SetPeakAssociations();
        }

        public static void PrevPatternDetection(Pattern.DiffractionPattern DP, Pattern.DiffractionPattern PrevDP)
        {
            List<DiffractionPeak> DPeakList = new List<DiffractionPeak>();

            for (int n = 0; n < PrevDP.FoundPeaks.Count; n++)
            {
                double FWHMD = PrevDP.FoundPeaks[n].PFunction.FWHM;
                int dChannel = 0;
                double dHeight = 0.0;

                double StartDegree = PrevDP.FoundPeaks[n].PFunction.Angle - (CalScec.Properties.Settings.Default.PeakWidthFWHM * FWHMD);
                if (StartDegree < CalScec.Properties.Settings.Default.PatternLowerLimit)
                {
                    StartDegree = CalScec.Properties.Settings.Default.PatternLowerLimit;
                }

                double StopDegree = PrevDP.FoundPeaks[n].PFunction.Angle + (CalScec.Properties.Settings.Default.PeakWidthFWHM * FWHMD);
                if (StopDegree > CalScec.Properties.Settings.Default.PatternUpperLimit)
                {
                    StopDegree = CalScec.Properties.Settings.Default.PatternUpperLimit;
                }

                Pattern.Counts FittingCounts = new Pattern.Counts();

                for (int j = 0; j < DP.PatternCounts.Count; j++)
                {
                    if (StartDegree < DP.PatternCounts[j][0])
                    {
                        if (StopDegree > DP.PatternCounts[j][0])
                        {
                            FittingCounts.Add(DP.PatternCounts[j]);

                            if (dHeight < DP.PatternCounts[j][1])
                            {
                                dHeight = DP.PatternCounts[j][1];
                                dChannel = j;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                double dBackground = FittingCounts[0][1];

                DiffractionPeak DPeak = new DiffractionPeak(PrevDP.FoundPeaks[n].DetectedChannel, PrevDP.FoundPeaks[n].PFunction.Angle, PrevDP.FoundPeaks[n].PFunction.Intensity, dBackground, FittingCounts);
                DPeak.PFunction.functionType = PrevDP.FoundPeaks[n].PFunction.functionType;
                DPeak.PFunction.FWHM = PrevDP.FoundPeaks[n].PFunction.FWHM;
                DPeak.PFunction.LorentzRatio = PrevDP.FoundPeaks[n].PFunction.LorentzRatio;
                DPeak.AddHKLAssociation(PrevDP.FoundPeaks[n].AssociatedHKLReflex, PrevDP.FoundPeaks[n].AssociatedCrystalData);

                DPeakList.Add(DPeak);
            }

            DP.FoundPeaks = DPeakList;
            DP.SetPeakAssociations();
        }
    }
}
