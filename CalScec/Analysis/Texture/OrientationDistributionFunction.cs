using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.Analysis.Texture
{
    public class OrientationDistributionFunction : ICloneable
    {
        /// <summary>
        /// [0] phi1
        /// [1] phi
        /// [2] phi2
        /// [3] value
        /// </summary>
        public List<double[]> TDData = new List<double[]>();
        private bool _symmetryReducedCalculation = CalScec.Properties.Settings.Default.ODFSymmetricCalculation;

        public double this[double varPhi1, double phi, double varPhi2]
        {
            get
            {
                for(int n = 0; n < this.TDData.Count; n++)
                {
                    if (this._symmetryReducedCalculation)
                    {
                        double symPhi = phi % 95;
                        double symVarPhi2 = varPhi2 % 60;

                        if (this.TDData[n][0] == varPhi1 && this.TDData[n][1] == symPhi && this.TDData[n][2] == symVarPhi2)
                        {
                            return this.TDData[n][3];
                        }
                    }
                    else
                    {
                        if (this.TDData[n][0] == varPhi1 && this.TDData[n][1] == phi && this.TDData[n][2] == varPhi2)
                        {
                            return this.TDData[n][3];
                        }
                    }
                }

                return -1;
            }
            set
            {
                for (int n = 0; n < this.TDData.Count; n++)
                {
                    if (this.TDData[n][0] == varPhi1 && this.TDData[n][1] == phi && this.TDData[n][2] == varPhi2)
                    {
                        this.TDData[n][3] = value;
                    }
                }
            }
        }

        public void SetStepSizes()
        {
            double FirstValuePhi1 = TDData[0][0];
            double FirstValuePhi = TDData[0][1];
            double FirstValuePhi2 = TDData[0][2];

            bool[] ParamFound = { false, false, false };

            for (int n = 0; n < TDData.Count; n++)
            {
                if(!ParamFound[0])
                {
                    if(TDData[n][0] != FirstValuePhi1)
                    {
                        this._stepSizePhi1 = Math.Abs(FirstValuePhi1 - TDData[n][0]);
                        ParamFound[0] = true;
                    }
                }
                if (!ParamFound[1])
                {
                    if (TDData[n][1] != FirstValuePhi)
                    {
                        this._stepSizePhi = Math.Abs(FirstValuePhi - TDData[n][1]);
                        ParamFound[1] = true;
                    }
                }
                if (!ParamFound[2])
                {
                    if (TDData[n][2] != FirstValuePhi2)
                    {
                        this._stepSizePhi2 = Math.Abs(FirstValuePhi2 - TDData[n][2]);
                        ParamFound[2] = true;
                    }
                }

                if(this._maxMRD < TDData[n][3])
                {
                    this._maxMRD = TDData[n][3];
                }
            }
        }

        private double _stepSizePhi1;
        public double StepSizePhi1
        {
            get
            {
                return _stepSizePhi1;
            }
        }

        private double _stepSizePhi;
        public double StepPhi
        {
            get
            {
                return _stepSizePhi;
            }
        }

        private double _stepSizePhi2;
        public double StepSizePhi2
        {
            get
            {
                return _stepSizePhi2;
            }
        }

        private double _maxMRD = 0;
        public double MaxMRD
        {
            get
            {
                return _maxMRD;
            }
        }

        public System.Threading.ManualResetEvent _doneEvent;

        private bool _fitConverged;
        public bool FitConverged
        {
            get
            {
                return this._fitConverged;
            }
        }

        /// <summary>
        /// 0: Voigt
        /// 1: Reuss
        /// 2:Hill
        /// 3:GeoHill
        /// 4:Kroener
        /// 5:DeWit
        /// </summary>
        public int FittingModel = 0;
        public string fittingModel
        {
            get
            {
                switch(FittingModel)
                {
                    case 0:
                        return "Voigt";
                    case 1:
                        return "Reuss";
                    case 2:
                        return "Hill";
                    case 3:
                        return "Geometric Hill";
                    case 4:
                        return "Kroener";
                    case 5:
                        return "DeWit";
                    default:
                        return "Hill";
                }
            }
        }
        public bool ClassicalCalculation = true;
        public bool UseStifnessCalculation = true;

        public bool fitActive = false;

        public Stress.Microsopic.ElasticityTensors BaseTensor;
        public Stress.Microsopic.ElasticityTensors TextureTensor;

        public List<CalScec.Analysis.Stress.Macroskopic.PeakStressAssociation> UsedPSA = new List<Stress.Macroskopic.PeakStressAssociation>();

        public void SetPeakStressAssociation(Sample actSample)
        {
            for (int i = 0; i < this.BaseTensor.GetPhaseInformation.HKLList.Count; i++)
            {
                for (int j = 0; j < actSample.DiffractionPatterns.Count; j++)
                {
                    for (int k = 0; k < actSample.DiffractionPatterns[j].FoundPeaks.Count; k++)
                    {
                        if (actSample.DiffractionPatterns[j].FoundPeaks[k].AssociatedCrystalData.SymmetryGroupID == this.BaseTensor.GetPhaseInformation.SymmetryGroupID)
                        {
                            if (actSample.DiffractionPatterns[j].FoundPeaks[k].AssociatedHKLReflex.HKLString == this.BaseTensor.GetPhaseInformation.HKLList[i].HKLString)
                            {
                                Stress.Macroskopic.PeakStressAssociation NewAssociation = new Stress.Macroskopic.PeakStressAssociation(actSample.DiffractionPatterns[j].Stress, actSample.DiffractionPatterns[j].PsiAngle(actSample.DiffractionPatterns[j].FoundPeaks[k].Angle), actSample.DiffractionPatterns[j].FoundPeaks[k], actSample.DiffractionPatterns[j].PhiAngle(actSample.DiffractionPatterns[j].FoundPeaks[k].Angle));

                                this.UsedPSA.Add(NewAssociation);
                            }
                        }
                    }
                }
            }
        }

        public void SetStrainData()
        {
            List<List<Stress.Macroskopic.PeakStressAssociation>> SortedDataPre = new List<List<Stress.Macroskopic.PeakStressAssociation>>();

            List<double> UsedPsiAngles = new List<double>();
            int AllDataUsed = 0;

            while (AllDataUsed < this.UsedPSA.Count)
            {
                List<Stress.Macroskopic.PeakStressAssociation> ActAngle = new List<Stress.Macroskopic.PeakStressAssociation>();
                int StartingNumber = 0;
                double ActPsiAngle = 0;

                for (int n = 0; n < this.UsedPSA.Count; n++)
                {
                    bool Contained = false;
                    for (int i = 0; i < UsedPsiAngles.Count; i++)
                    {
                        if (Math.Abs(this.UsedPSA[n].PsiAngle - UsedPsiAngles[i]) < CalScec.Properties.Settings.Default.PsyAcceptanceAngle)
                        {
                            Contained = true;
                            break;
                        }
                    }

                    if (!Contained)
                    {
                        StartingNumber = n + 1;
                        ActPsiAngle = this.UsedPSA[n].PsiAngle;
                        ActAngle.Add(this.UsedPSA[n]);
                        AllDataUsed++;
                        break;
                    }
                }

                for (int n = StartingNumber; n < this.UsedPSA.Count; n++)
                {
                    if (Math.Abs(ActPsiAngle - this.UsedPSA[n].PsiAngle) < CalScec.Properties.Settings.Default.PsyAcceptanceAngle)
                    {
                        ActAngle.Add(this.UsedPSA[n]);
                        AllDataUsed++;
                    }
                }

                UsedPsiAngles.Add(ActPsiAngle);
                SortedDataPre.Add(ActAngle);
            }

            List<List<Stress.Macroskopic.PeakStressAssociation>> SortedData = new List<List<Stress.Macroskopic.PeakStressAssociation>>();

            for (int n = 0; n < SortedDataPre.Count; n++)
            {
                List<Stress.Macroskopic.PeakStressAssociation> HKLSortTmp = new List<Stress.Macroskopic.PeakStressAssociation>();
                DataManagment.CrystalData.HKLReflex ActReflex = new DataManagment.CrystalData.HKLReflex(-1);
                for (int i = 0; i < SortedDataPre[n].Count; i++)
                {
                    if(ActReflex.Distance != -1)
                    {
                        if(ActReflex.HKLString == SortedDataPre[n][i].DPeak.AssociatedHKLReflex.HKLString)
                        {
                            HKLSortTmp.Add(SortedDataPre[n][i]);
                        }
                        else
                        {
                            List<Stress.Macroskopic.PeakStressAssociation> HKLSort = new List<Stress.Macroskopic.PeakStressAssociation>();
                            for(int j = 0; j < HKLSortTmp.Count; j++)
                            {
                                Stress.Macroskopic.PeakStressAssociation Tmp = new Stress.Macroskopic.PeakStressAssociation(HKLSortTmp[j]);
                                HKLSort.Add(Tmp);
                            }
                            SortedData.Add(HKLSort);
                            HKLSortTmp.Clear();

                            HKLSortTmp.Add(SortedDataPre[n][i]);
                            ActReflex = SortedDataPre[n][i].DPeak.AssociatedHKLReflex;
                        }
                    }
                    else
                    {
                        HKLSortTmp.Add(SortedDataPre[n][i]);
                        ActReflex = SortedDataPre[n][i].DPeak.AssociatedHKLReflex;
                    }
                }

                List<Stress.Macroskopic.PeakStressAssociation> HKLSort1 = new List<Stress.Macroskopic.PeakStressAssociation>();
                for (int j = 0; j < HKLSortTmp.Count; j++)
                {
                    Stress.Macroskopic.PeakStressAssociation Tmp = new Stress.Macroskopic.PeakStressAssociation(HKLSortTmp[j]);
                    HKLSort1.Add(Tmp);
                }
                SortedData.Add(HKLSort1);
            }

            for (int n = 0; n < SortedData.Count; n++)
            {
                if (SortedData[n].Count > 1)
                {
                    double SmallestStress = SortedData[n][0].Stress;
                    double SmallestDistance = SortedData[n][0].DPeak.LatticeDistance;

                    for (int i = 1; i < SortedData[n].Count; i++)
                    {
                        if (SmallestStress > SortedData[n][i].Stress)
                        {
                            SmallestStress = SortedData[n][i].Stress;
                            SmallestDistance = SortedData[n][i].DPeak.LatticeDistance;
                        }
                    }

                    for (int i = 0; i < SortedData[n].Count; i++)
                    {
                        if (SmallestStress != SortedData[n][i].Stress)
                        {
                            double StrainValue = (SortedData[n][i].DPeak.LatticeDistance - SmallestDistance);
                            StrainValue /= SmallestDistance;
                            double StressValue = (SortedData[n][i].Stress - SmallestStress);

                            SortedData[n][i].Strain = StrainValue;
                            SortedData[n][i].Stress = StressValue;
                            double StrainValueError = (SortedData[n][i].DPeak.LatticeDistanceError);
                            StrainValueError /= SmallestDistance;
                            StrainValueError *= StrainValue;

                            if(StrainValueError == 0)
                            {
                                StrainValueError = 1;
                            }

                            SortedData[n][i]._StrainError = StrainValueError;
                        }
                        else
                        {
                            double StrainValue = 0;
                            double StressValue = 0;

                            SortedData[n][i].Strain = StrainValue;
                            SortedData[n][i]._StrainError = 1;
                            SortedData[n][i].Stress = StressValue;
                        }
                    }
                }
            }

            this.UsedPSA.Clear();

            for (int n = 0; n < SortedData.Count; n++)
            {
                for(int i = 0; i < SortedData[n].Count; i++)
                {
                    this.UsedPSA.Add(SortedData[n][i]);
                }
            }
        }

        public void SetMRDValues(List<Stress.Macroskopic.PeakStressAssociation> PSA)
        {
            for(int n = 0; n < PSA.Count; n++)
            {
                List<double[]> IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(PSA[n].DPeak.AssociatedHKLReflex, PSA[n].phiAngle, PSA[n].psiAngle);
                PSA[n].MRDValue = 0;

                for (int i = 0; i < IntegrationAngles.Count; i++)
                {
                    double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];
                    PSA[n].MRDValue += ODFValue;
                }
            }
        }

        public OrientationDistributionFunction(string filePath)
        {
            TDData = new List<double[]>();

            string[] PatternFileLines = System.IO.File.ReadLines(filePath).ToArray();

            for(int n = 0; n < PatternFileLines.Count(); n++)
            {
                if (PatternFileLines[n][0] != '#' && PatternFileLines[n][0] != '%' && PatternFileLines[n][0] != '/')
                {
                    string[] SplitData = PatternFileLines[n].Split(' ');
                    if(SplitData.Count() == 4)
                    {
                        double phi1tmp = 0.0;
                        double phitmp = 0.0;
                        double phi2tmp = 0.0;
                        double valuetmp = 0.0;

                        try
                        {
                            phi1tmp = Convert.ToDouble(SplitData[0]);
                            phitmp = Convert.ToDouble(SplitData[1]);
                            phi2tmp = Convert.ToDouble(SplitData[2]);
                            valuetmp = Convert.ToDouble(SplitData[3]);

                            double[] NewCount = { phi1tmp, phitmp, phi2tmp, valuetmp };

                            TDData.Add(NewCount);
                        }
                        catch
                        {
                            phi1tmp = 0.0;
                        }
                    }
                    else if(SplitData.Count() > 4)
                    {
                        int ParamIndex = 0;
                        double[] NewCount = { 0.0, 0.0, 0.0, 0.0 };

                        for(int i = 0; i < SplitData.Count(); i++)
                        {
                            if(SplitData[i] != "")
                            {
                                try
                                {
                                    NewCount[ParamIndex] = Convert.ToDouble(SplitData[i]);
                                    ParamIndex++;

                                    if(ParamIndex == 4)
                                    {
                                        TDData.Add(NewCount);
                                        break;
                                    }
                                }
                                catch
                                {
                                    
                                }
                            }
                        }
                    }
                }
            }

            this.SetStepSizes();

            this.BaseTensor = new Stress.Microsopic.ElasticityTensors();
            this.TextureTensor = new Stress.Microsopic.ElasticityTensors();
        }

        public OrientationDistributionFunction()
        {

        }
        
        #region PoleFigures

        public List<double[]> GetPoleFigureVarPhi2(DataManagment.CrystalData.HKLReflex PFReflex)
        {
            double[] directionh00 = { 90.0, 90.0 };
            double[] direction0k0 = { 90.0, 0.0 };
            double[] direction00l = { 0.0, 0.0 };

            if(PFReflex.H < 0)
            {
                directionh00[1] = 270.0;
                direction0k0[1] = 360.0;
            }

            if(PFReflex.K < 0)
            {
                direction0k0[1] = 180.0;

            }

            directionh00[0] *= Math.Abs(PFReflex.H);
            directionh00[1] *= Math.Abs(PFReflex.H);
            direction0k0[0] *= Math.Abs(PFReflex.K);
            direction0k0[1] *= Math.Abs(PFReflex.K);
            direction00l[0] *= Math.Abs(PFReflex.L);
            direction00l[1] *= Math.Abs(PFReflex.L);

            double VarPhi2 = directionh00[1] + direction0k0[1];
            VarPhi2 /= PFReflex.H + PFReflex.K;
            double Phi = directionh00[0] + direction00l[0] + direction0k0[0];
            Phi /= PFReflex.H + PFReflex.K + PFReflex.L;

            double Stepsize = this.StepSizePhi2;

            if(VarPhi2 % Stepsize != 0.0)
            {
                VarPhi2 -= VarPhi2 % Stepsize;
            }

            List<double[]> Ret = new List<double[]>();

            for(int n = 0; n < TDData.Count; n++)
            {
                if(TDData[n][1] < 90.0 && Convert.ToInt32(TDData[n][2]) == Convert.ToInt32(VarPhi2))
                {
                    double PhiTmp = TDData[n][1] - Phi;
                    if(PhiTmp < 0)
                    {
                        PhiTmp += 90;
                    }
                    double[] Tmp = { TDData[n][0], PhiTmp, TDData[n][3] };
                    Ret.Add(Tmp);
                }
            }

            return Ret;
        }

        public List<double[]> GetPoleFigurePhi(DataManagment.CrystalData.HKLReflex PFReflex)
        {
            double[] directionh00 = { 90.0, 90.0 };
            double[] direction0k0 = { 90.0, 0.0 };
            double[] direction00l = { 0.0, 0.0 };

            if (PFReflex.H < 0)
            {
                directionh00[1] = 270.0;
                direction0k0[1] = 360.0;
            }

            if (PFReflex.K < 0)
            {
                direction0k0[1] = 180.0;

            }

            directionh00[0] *= Math.Abs(PFReflex.H);
            directionh00[1] *= Math.Abs(PFReflex.H);
            direction0k0[0] *= Math.Abs(PFReflex.K);
            direction0k0[1] *= Math.Abs(PFReflex.K);
            direction00l[0] *= Math.Abs(PFReflex.L);
            direction00l[1] *= Math.Abs(PFReflex.L);

            double VarPhi2 = directionh00[1] + direction0k0[1];
            VarPhi2 /= PFReflex.H + PFReflex.K;
            double Phi = directionh00[0] + direction00l[0] + direction0k0[0];
            Phi /= PFReflex.H + PFReflex.K + PFReflex.L;

            double Stepsize = this.StepPhi;

            if (Phi % Stepsize != 0.0)
            {
                Phi -= Phi % Stepsize;
            }

            List<double[]> Ret = new List<double[]>();

            for (int n = 0; n < TDData.Count; n++)
            {
                if (Convert.ToInt32(TDData[n][1]) == Convert.ToInt32(Phi))
                {
                    double VarPhi2Tmp = TDData[n][2] - VarPhi2;
                    if (VarPhi2Tmp < 0)
                    {
                        VarPhi2Tmp += 360;
                    }
                    double[] Tmp = { TDData[n][0], VarPhi2Tmp, TDData[n][3] };
                    Ret.Add(Tmp);
                }
            }

            return Ret;
        }

        #endregion

        #region Calculation

        /// <summary>
        /// Calculates the Stress wiht voigts model 
        /// </summary>
        /// <param name="appliedStrain"></param>
        /// <returns></returns>
        public double GetStrainVoigtCubic(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            double F33 = this.GetStressFactor33VoigtCubic(usedAssociation);

            return F33 * usedAssociation.Stress;
        }

        /// <summary>
        /// Calculates the Stress wiht voigts model 
        /// </summary>
        /// <param name="appliedStrain"></param>
        /// <returns></returns>
        public double GetStrainReussCubic(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            MathNet.Numerics.LinearAlgebra.Matrix<double> StressFactors = this.GetStressFactorMatrixReuss(usedAssociation);
            //Totaler TestUmbau
            //double F33 = this.GetStressFactor33ReussCubic(usedAssociation);
            ////ACHTUNG/////////////////
            ////COs^2 psi wird getestet
            //return F33 * usedAssociation.Stress * Math.Pow(Math.Cos(usedAssociation.PsiAngle), 2);

            double ret = StressFactors[0, 0] * Math.Pow(Math.Sin((usedAssociation.PsiAngle * Math.PI) / 180.0), 2);
            ret -= StressFactors[0, 2] * Math.Sin(2 * (usedAssociation.PhiAngle * Math.PI) / 180.0);
            ret = StressFactors[2, 2] * Math.Pow(Math.Cos((usedAssociation.PsiAngle * Math.PI) / 180.0), 2);

            return ret * usedAssociation.Stress;
        }

        /// <summary>
        /// Calculates the Stress wiht voigts model 
        /// </summary>
        /// <param name="appliedStrain"></param>
        /// <returns></returns>
        public double GetStrainHillCubic(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            double F33 = this.GetStressFactor33HillCubic(usedAssociation);
            //ACHTUNG/////////////////
            //COs^2 psi wird getestet
            return F33 * usedAssociation.Stress * Math.Pow(Math.Cos(usedAssociation.PsiAngle), 2);
        }

        /// <summary>
        /// Calculates the Stress wiht voigts model 
        /// </summary>
        /// <param name="appliedStrain"></param>
        /// <returns></returns>
        public double GetStrainVoigtHexagonal(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            double F33 = this.GetStressFactor33VoigtHexagonal(usedAssociation);
            //ACHTUNG/////////////////
            //COs^2 psi wird getestet
            return F33 * usedAssociation.Stress * Math.Pow(Math.Cos(usedAssociation.PsiAngle), 2);
        }

        /// <summary>
        /// Calculates the Stress wiht voigts model 
        /// </summary>
        /// <param name="appliedStrain"></param>
        /// <returns></returns>
        public double GetStrainReussHexagonal(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            double F33 = this.GetStressFactor33ReussHexagonal(usedAssociation);
            //ACHTUNG/////////////////
            //COs^2 psi wird getestet
            return F33 * usedAssociation.Stress * Math.Pow(Math.Cos(usedAssociation.PsiAngle), 2);
        }

        /// <summary>
        /// Calculates the Stress wiht voigts model 
        /// </summary>
        /// <param name="appliedStrain"></param>
        /// <returns></returns>
        public double GetStrainHillHexagonal(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            double F33 = this.GetStressFactor33HillHexagonal(usedAssociation);
            //ACHTUNG/////////////////
            //COs^2 psi wird getestet
            return F33 * usedAssociation.Stress * Math.Pow(Math.Cos(usedAssociation.PsiAngle), 2);
        }

        ///// <summary>
        ///// Calculates the Stress wiht voigts model 
        ///// </summary>
        ///// <param name="appliedStrain"></param>
        ///// <returns></returns>
        //public double GetStrainVoigtHexagonal(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        //{
        //    double F33 = this.GetStressFactor33VoigtHexagonal(usedAssociation);

        //    return F33 * usedAssociation.Stress;
        //}

        ///// <summary>
        ///// Calculates the Stress wiht voigts model 
        ///// </summary>
        ///// <param name="appliedStrain"></param>
        ///// <returns></returns>
        //public double GetStrainReussHexagonal(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        //{
        //    double F33 = this.GetStressFactor33ReussHexagonal(usedAssociation);

        //    return F33 * usedAssociation.Stress;
        //}

        ///// <summary>
        ///// Calculates the Stress wiht voigts model 
        ///// </summary>
        ///// <param name="appliedStrain"></param>
        ///// <returns></returns>
        //public double GetStrainHillHexagonal(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        //{
        //    double F33 = this.GetStressFactor33HillHexagonal(usedAssociation);

        //    return F33 * usedAssociation.Stress;
        //}

        #region First derivatives

        #region Cubic

        /// <summary>
        /// Calculates the first derivatives after voigt
        /// </summary>
        /// <param name="usedAssociation"></param>
        /// <param name="parameter">[0]:C11; [1]:C12, [2]:C44</param>
        /// <returns></returns>
        public double GetStrainVoigtCubicFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            double F33 = this.GetStressFactor33VoigtCubicFD(usedAssociation, parameter);
            //ACHTUNG/////////////////
            //COs^2 psi wird getestet
            return F33 * usedAssociation.Stress;

            //MathNet.Numerics.LinearAlgebra.Matrix<double> StressFactors = this.GetStressFactorMatrixVoigtCubicFD(usedAssociation, parameter);

            //double ret = StressFactors[0, 0] * Math.Pow(Math.Sin((usedAssociation.PsiAngle * Math.PI) / 180.0), 2);
            //ret -= StressFactors[0, 2] * Math.Sin(2 * (usedAssociation.PhiAngle * Math.PI) / 180.0);
            //ret = StressFactors[2, 2] * Math.Pow(Math.Cos((usedAssociation.PsiAngle * Math.PI) / 180.0), 2);

            //return ret * usedAssociation.Stress;
        }

        /// <summary>
        /// Calculates the first derivatives after voigt
        /// </summary>
        /// <param name="usedAssociation"></param>
        /// <param name="parameter">[0]:S11; [1]:S12, [2]:S44</param>
        /// <returns></returns>
        public double GetStrainReussCubicFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            //Totaler TestUmbau
            //double F33 = this.GetStressFactor33ReussCubicFD(usedAssociation, parameter);
            ////ACHTUNG/////////////////
            ////COs^2 psi wird getestet
            //return F33 * usedAssociation.Stress * Math.Pow(Math.Cos(usedAssociation.PsiAngle), 2);

            MathNet.Numerics.LinearAlgebra.Matrix<double> StressFactors = this.GetStressFactorMatrixReussCubicFD(usedAssociation, parameter);

            double ret = StressFactors[0, 0] * Math.Pow(Math.Sin((usedAssociation.PsiAngle * Math.PI) / 180.0), 2);
            ret -= StressFactors[0, 2] * Math.Sin(2 * (usedAssociation.PhiAngle * Math.PI) / 180.0);
            ret = StressFactors[2, 2] * Math.Pow(Math.Cos((usedAssociation.PsiAngle * Math.PI) / 180.0), 2);

            return ret * usedAssociation.Stress;
        }

        /// <summary>
        /// Calculates the first derivatives after voigt
        /// </summary>
        /// <param name="usedAssociation"></param>
        /// <param name="parameter">[0]:S11; [1]:S12, [2]:S44</param>
        /// <returns></returns>
        public double GetStrainHillCubicFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            double F33 = this.GetStressFactor33HillCubicFD(usedAssociation, parameter);
            //ACHTUNG/////////////////
            //COs^2 psi wird getestet
            return F33 * usedAssociation.Stress;
        }

        #endregion

        #region Hexagonal

        /// <summary>
        /// Calculates the first derivatives after voigt
        /// </summary>
        /// <param name="usedAssociation"></param>
        /// <param name="parameter">[0]:C11; [1]:C12, [2]:C44</param>
        /// <returns></returns>
        public double GetStrainVoigtHexagonalFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            double F33 = this.GetStressFactor33VoigtHexagonalFD(usedAssociation, parameter);
            //ACHTUNG/////////////////
            //COs^2 psi wird getestet
            return F33 * usedAssociation.Stress * Math.Pow(Math.Cos(usedAssociation.PsiAngle), 2);
        }

        /// <summary>
        /// Calculates the first derivatives after voigt
        /// </summary>
        /// <param name="usedAssociation"></param>
        /// <param name="parameter">[0]:S11; [1]:S12, [2]:S44</param>
        /// <returns></returns>
        public double GetStrainReussHexagonalFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            double F33 = this.GetStressFactor33ReussHexagonalFD(usedAssociation, parameter);
            //ACHTUNG/////////////////
            //COs^2 psi wird getestet
            return F33 * usedAssociation.Stress * Math.Pow(Math.Cos(usedAssociation.PsiAngle), 2);
        }

        /// <summary>
        /// Calculates the first derivatives after voigt
        /// </summary>
        /// <param name="usedAssociation"></param>
        /// <param name="parameter">[0]:S11; [1]:S12, [2]:S44</param>
        /// <returns></returns>
        public double GetStrainHillHexagonalFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            double F33 = this.GetStressFactor33HillHexagonalFD(usedAssociation, parameter);
            //ACHTUNG/////////////////
            //COs^2 psi wird getestet
            return F33 * usedAssociation.Stress * Math.Pow(Math.Cos(usedAssociation.PsiAngle), 2);
        }

        #endregion

        #endregion

        #region Macro

        //TODO: Die Funktionen entsprechend umbenennen. Jetzt sind es Makroskopische Wichtungen
        public void SetTextureTensor()
        {
            switch (BaseTensor.Symmetry)
            {
                case "cubic":
                    this.SetTextureTensorCubic();
                    break;
                case "hexagonal":
                    this.SetTextureTensorHexagonal();
                    break;
                case "tetragonal type 1":
                    this.SetTextureTensorTetragonalType1();
                    break;
                case "tetragonal type 2":

                    break;
                case "trigonal type 1":

                    break;
                case "trigonal type 2":

                    break;
                case "rhombic":
                    this.SetTextureTensorRhombic();
                    break;
                case "monoclinic":

                    break;
                case "triclinic":

                    break;
                default:

                    break;
            }

            this.TextureTensor.CalculateCompliances();
        }

        private void SetTextureTensorCubic()
        {
            double TC11 = 0;
            double TC12 = 0;
            double TC44 = 0;

            double normFactor = 0.0;

            for (int n = 0; n < this.TDData.Count; n++)
            {
                normFactor += this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC11 += Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 4) * this.BaseTensor.C11 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC11 += Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 4) * this.BaseTensor.C22 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC11 += Math.Pow(SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 4) * this.BaseTensor.C33 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC11 += 2 * Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C12 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC11 += 2 * Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C13 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC11 += 2 * Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C23 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC11 += 4 * Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C44 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC11 += 4 * Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C55 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC11 += 4 * Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C66 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC12 += Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C11 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC12 += Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C22 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC12 += Math.Pow(SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C33 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC12 += Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C12 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC12 += Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C12 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC12 += Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C13 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC12 += Math.Pow(SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C13 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC12 += Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C23 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC12 += Math.Pow(SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C23 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC12 += 4 * SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C44 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC12 += 4 * SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C55 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC12 += 4 * SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C66 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC44 += Math.Pow(SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C11 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC44 += Math.Pow(SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C22 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC44 += Math.Pow(SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C33 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC44 += 2 * SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C12 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC44 += 2 * SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C13 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC44 += 2 * SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C23 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC44 += Math.Pow(SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C44 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC44 += 2 * SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C44 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC44 += Math.Pow(SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C44 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC44 += Math.Pow(SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C55 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC44 += 2 * SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C55 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC44 += Math.Pow(SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C55 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC44 += Math.Pow(SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C66 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC44 += 2 * SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C66 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC44 += Math.Pow(SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C66 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);


            }

            this.TextureTensor.C11 = (TC11 / normFactor);
            this.TextureTensor.C12 = (TC12 / normFactor);
            this.TextureTensor.C44 = (TC44 / normFactor);
        }

        private void SetTextureTensorHexagonal()
        {
            double TC11 = 0;
            double TC33 = 0;
            double TC12 = 0;
            double TC13 = 0;
            double TC44 = 0;

            double normFactor = 0.0;

            for (int n = 0; n < this.TDData.Count; n++)
            {
                normFactor += this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC11 += Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 4) * this.BaseTensor.C11 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC11 += Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 4) * this.BaseTensor.C22 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC11 += Math.Pow(SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 4) * this.BaseTensor.C33 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC11 += 2 * Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C12 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC11 += 2 * Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C13 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC11 += 2 * Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C23 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC11 += 4 * Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C44 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC11 += 4 * Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C55 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC11 += 4 * Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C66 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC33 += Math.Pow(SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 4) * this.BaseTensor.C11 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC33 += Math.Pow(SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 4) * this.BaseTensor.C22 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC33 += Math.Pow(SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 4) * this.BaseTensor.C33 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC33 += 2 * Math.Pow(SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C12 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC33 += 2 * Math.Pow(SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C13 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC33 += 2 * Math.Pow(SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C23 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC33 += 4 * Math.Pow(SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C44 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC33 += 4 * Math.Pow(SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C55 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC33 += 4 * Math.Pow(SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C66 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC12 += Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C11 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC12 += Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C22 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC12 += Math.Pow(SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C33 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC12 += Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C12 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC12 += Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C12 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC12 += Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C13 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC12 += Math.Pow(SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C13 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC12 += Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C23 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC12 += Math.Pow(SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C23 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC12 += 4 * SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C44 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC12 += 4 * SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C55 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC12 += 4 * SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C66 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC13 += Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C11 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC13 += Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C22 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC13 += Math.Pow(SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C33 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC13 += Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C12 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC13 += Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C12 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC13 += Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C13 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC13 += Math.Pow(SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C13 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC13 += Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C23 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC13 += Math.Pow(SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C23 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC13 += 4 * SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C44 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC13 += 4 * SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C55 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC13 += 4 * SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C66 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC44 += Math.Pow(SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C11 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC44 += Math.Pow(SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C22 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC44 += Math.Pow(SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C22 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC44 += 2 * SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C12 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC44 += 2 * SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C13 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC44 += 2 * SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C23 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC44 += 2 * Math.Pow(SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C44 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC44 += 2 * SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C44 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC44 += 2 * Math.Pow(SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C55 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC44 += 2 * SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C55 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC44 += 2 * Math.Pow(SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C66 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC44 += 2 * SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C66 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
            }

            normFactor *= 8.0 * Math.Pow(Math.PI, 2);

            this.TextureTensor.C11 = TC11 / normFactor;
            this.TextureTensor.C33 = TC33 / normFactor;
            this.TextureTensor.C12 = TC12 / normFactor;
            this.TextureTensor.C13 = TC13 / normFactor;
            this.TextureTensor.C44 = TC44 / normFactor;

        }

        private void SetTextureTensorTetragonalType1()
        {
            double TC11 = 0;
            double TC33 = 0;
            double TC12 = 0;
            double TC13 = 0;
            double TC44 = 0;
            double TC66 = 0;

            double normFactor = 0.0;

            for (int n = 0; n < this.TDData.Count; n++)
            {
                normFactor += this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC11 += Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 4) * this.BaseTensor.C11 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC11 += Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 4) * this.BaseTensor.C22 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC11 += Math.Pow(SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 4) * this.BaseTensor.C33 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC11 += 2 * Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C12 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC11 += 2 * Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C13 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC11 += 2 * Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C23 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC11 += 4 * Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C44 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC11 += 4 * Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C55 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC11 += 4 * Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C66 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC33 += Math.Pow(SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 4) * this.BaseTensor.C11 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC33 += Math.Pow(SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 4) * this.BaseTensor.C22 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC33 += Math.Pow(SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 4) * this.BaseTensor.C33 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC33 += 2 * Math.Pow(SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C12 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC33 += 2 * Math.Pow(SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C13 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC33 += 2 * Math.Pow(SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C23 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC33 += 4 * Math.Pow(SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C44 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC33 += 4 * Math.Pow(SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C55 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC33 += 4 * Math.Pow(SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C66 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC12 += Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C11 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC12 += Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C22 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC12 += Math.Pow(SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C33 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC12 += Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C12 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC12 += Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C12 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC12 += Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C13 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC12 += Math.Pow(SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C13 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC12 += Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C23 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC12 += Math.Pow(SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C23 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC12 += 4 * SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C44 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC12 += 4 * SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C55 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC12 += 4 * SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C66 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC13 += Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C11 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC13 += Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C22 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC13 += Math.Pow(SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C33 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC13 += Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C12 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC13 += Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C12 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC13 += Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C13 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC13 += Math.Pow(SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C13 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC13 += Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C23 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC13 += Math.Pow(SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C23 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC13 += 4 * SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C44 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC13 += 4 * SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C55 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC13 += 4 * SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C66 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC44 += Math.Pow(SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C11 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC44 += Math.Pow(SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C22 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC44 += Math.Pow(SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C22 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC44 += 2 * SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C12 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC44 += 2 * SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C13 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC44 += 2 * SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C23 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC44 += 2 * Math.Pow(SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C44 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC44 += 2 * SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C44 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC44 += 2 * Math.Pow(SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C55 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC44 += 2 * SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C55 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC44 += 2 * Math.Pow(SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C66 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC44 += 2 * SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C66 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC66 += Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C11 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC66 += Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C22 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC66 += Math.Pow(SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C22 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC66 += 2 * SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C12 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC66 += 2 * SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C13 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC66 += 2 * SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C23 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC66 += 2 * Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C44 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC66 += 2 * SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C44 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC66 += 2 * Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C55 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC66 += 2 * SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C55 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC66 += 2 * Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C66 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC66 += 2 * SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C66 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
            }

            normFactor /= 8.0 * Math.PI;

            this.TextureTensor.C11 = TC11 / normFactor;
            this.TextureTensor.C33 = TC33 / normFactor;
            this.TextureTensor.C12 = TC12 / normFactor;
            this.TextureTensor.C13 = TC13 / normFactor;
            this.TextureTensor.C44 = TC44 / normFactor;
            this.TextureTensor.C66 = TC66 / normFactor;

        }

        private void SetTextureTensorRhombic()
        {
            double TC11 = 0;
            double TC22 = 0;
            double TC33 = 0;
            double TC12 = 0;
            double TC13 = 0;
            double TC23 = 0;
            double TC44 = 0;
            double TC55 = 0;
            double TC66 = 0;

            double normFactor = 0.0;

            for (int n = 0; n < this.TDData.Count; n++)
            {
                normFactor += this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC11 += Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 4) * this.BaseTensor.C11 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC11 += Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 4) * this.BaseTensor.C22 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC11 += Math.Pow(SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 4) * this.BaseTensor.C33 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC11 += 2 * Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C12 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC11 += 2 * Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C13 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC11 += 2 * Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C23 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC11 += 4 * Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C44 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC11 += 4 * Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C55 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC11 += 4 * Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C66 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC22 += Math.Pow(SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 4) * this.BaseTensor.C11 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC22 += Math.Pow(SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 4) * this.BaseTensor.C22 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC22 += Math.Pow(SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 4) * this.BaseTensor.C33 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC22 += 2 * Math.Pow(SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C12 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC22 += 2 * Math.Pow(SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C13 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC22 += 2 * Math.Pow(SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C23 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC22 += 4 * Math.Pow(SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C44 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC22 += 4 * Math.Pow(SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C55 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC22 += 4 * Math.Pow(SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C66 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC33 += Math.Pow(SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 4) * this.BaseTensor.C11 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC33 += Math.Pow(SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 4) * this.BaseTensor.C22 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC33 += Math.Pow(SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 4) * this.BaseTensor.C33 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC33 += 2 * Math.Pow(SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C12 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC33 += 2 * Math.Pow(SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C13 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC33 += 2 * Math.Pow(SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C23 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC33 += 4 * Math.Pow(SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C44 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC33 += 4 * Math.Pow(SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C55 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC33 += 4 * Math.Pow(SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C66 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC12 += Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C11 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC12 += Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C22 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC12 += Math.Pow(SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C33 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC12 += Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C12 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC12 += Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C12 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC12 += Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C13 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC12 += Math.Pow(SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C13 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC12 += Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C23 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC12 += Math.Pow(SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C23 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC12 += 4 * SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C44 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC12 += 4 * SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C55 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC12 += 4 * SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C66 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC13 += Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C11 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC13 += Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C22 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC13 += Math.Pow(SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C33 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC13 += Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C12 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC13 += Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C12 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC13 += Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C13 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC13 += Math.Pow(SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C13 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC13 += Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C23 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC13 += Math.Pow(SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C23 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC13 += 4 * SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C44 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC13 += 4 * SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C55 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC13 += 4 * SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C66 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC23 += Math.Pow(SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C11 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC23 += Math.Pow(SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C22 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC23 += Math.Pow(SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C33 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC23 += Math.Pow(SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C12 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC23 += Math.Pow(SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C12 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC23 += Math.Pow(SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C13 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC23 += Math.Pow(SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C13 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC23 += Math.Pow(SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C23 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC23 += Math.Pow(SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C23 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC23 += 4 * SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C44 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC23 += 4 * SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C55 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC23 += 4 * SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C66 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC44 += Math.Pow(SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C11 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC44 += Math.Pow(SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C22 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC44 += Math.Pow(SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C22 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC44 += 2 * SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C12 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC44 += 2 * SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C13 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC44 += 2 * SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C23 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC44 += 2 * Math.Pow(SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C44 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC44 += 2 * SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C44 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC44 += 2 * Math.Pow(SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C55 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC44 += 2 * SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C55 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC44 += 2 * Math.Pow(SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C66 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC44 += 2 * SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C66 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC55 += Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C11 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC55 += Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C22 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC55 += Math.Pow(SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C22 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC55 += 2 * SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C12 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC55 += 2 * SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C13 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC55 += 2 * SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C23 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC55 += 2 * Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C44 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC55 += 2 * SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C44 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC55 += 2 * Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C55 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC55 += 2 * SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM33(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C55 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC55 += 2 * Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C66 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC55 += 2 * SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM32(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM31(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C66 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC66 += Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C11 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC66 += Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C22 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC66 += Math.Pow(SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C22 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC66 += 2 * SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C12 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC66 += 2 * SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C13 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC66 += 2 * SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C23 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC66 += 2 * Math.Pow(SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C44 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC66 += 2 * SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C44 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC66 += 2 * Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C55 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC66 += 2 * SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM23(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM13(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C55 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);

                TC66 += 2 * Math.Pow(SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * Math.Pow(SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]), 2) * this.BaseTensor.C66 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
                TC66 += 2 * SCTM11(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM22(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM12(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * SCTM21(this.TDData[n][0], this.TDData[n][1], this.TDData[n][2]) * this.BaseTensor.C66 * this.TDData[n][3] * Math.Sin((this.TDData[n][1] * Math.PI) / 180.0);
            }

            normFactor /= 8.0 * Math.PI;

            this.TextureTensor.C11 = TC11 / normFactor;
            this.TextureTensor.C22 = TC22 / normFactor;
            this.TextureTensor.C33 = TC33 / normFactor;
            this.TextureTensor.C12 = TC12 / normFactor;
            this.TextureTensor.C13 = TC13 / normFactor;
            this.TextureTensor.C23 = TC23 / normFactor;
            this.TextureTensor.C44 = TC44 / normFactor;
            this.TextureTensor.C55 = TC55 / normFactor;
            this.TextureTensor.C66 = TC66 / normFactor;

        }

        public void SetTextureTensorIso()
        {
            switch (BaseTensor.Symmetry)
            {
                case "cubic":
                    this.SetTextureTensorCubicIso();
                    break;
                case "hexagonal":
                    this.SetTextureTensorHexagonalIso();
                    break;
                case "tetragonal type 1":
                    this.SetTextureTensorTetragonalType1Iso();
                    break;
                case "tetragonal type 2":

                    break;
                case "trigonal type 1":

                    break;
                case "trigonal type 2":

                    break;
                case "rhombic":
                    this.SetTextureTensorRhombicIso();
                    break;
                case "monoclinic":

                    break;
                case "triclinic":

                    break;
                default:

                    break;
            }

            this.TextureTensor.CalculateCompliances();
        }
        public void SetTextureTensorIso(Stress.Microsopic.ElasticityTensors averagingTensor)
        {
            switch (BaseTensor.Symmetry)
            {
                case "cubic":
                    this.SetTextureTensorCubicIso(averagingTensor);
                    break;
                case "hexagonal":
                    this.SetTextureTensorHexagonalIso(averagingTensor);
                    break;
                case "tetragonal type 1":
                    this.SetTextureTensorTetragonalType1Iso(averagingTensor);
                    break;
                case "tetragonal type 2":

                    break;
                case "trigonal type 1":

                    break;
                case "trigonal type 2":

                    break;
                case "rhombic":
                    this.SetTextureTensorRhombicIso(averagingTensor);
                    break;
                case "monoclinic":

                    break;
                case "triclinic":

                    break;
                default:

                    break;
            }

            this.TextureTensor.CalculateCompliances();
        }

        private void SetTextureTensorCubicIso()
        {
            double TC11 = 0;
            double TC12 = 0;
            double TC44 = 0;

            double normFactor = 0.0;

            for(double phi1 = 0.0; phi1 < 360.0; phi1 += 5.0)
            {
                for (double psi1 = 0.0; psi1 < 360.0; psi1 += 5.0)
                {
                    for (double phi2 = 0.0; phi2 < 360.0; phi2 += 5.0)
                    {
                        normFactor += Math.Sin((psi1 * Math.PI) / 180.0);

                        TC11 += Math.Pow(SCTM11(phi1, psi1, phi2), 4) * this.BaseTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += Math.Pow(SCTM12(phi1, psi1, phi2), 4) * this.BaseTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += Math.Pow(SCTM13(phi1, psi1, phi2), 4) * this.BaseTensor.C33 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC11 += 2 * Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM12(phi1, psi1, phi2), 2) * this.BaseTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += 2 * Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM13(phi1, psi1, phi2), 2) * this.BaseTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += 2 * Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM13(phi1, psi1, phi2), 2) * this.BaseTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC11 += 4 * Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM13(phi1, psi1, phi2), 2) * this.BaseTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += 4 * Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM13(phi1, psi1, phi2), 2) * this.BaseTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += 4 * Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM12(phi1, psi1, phi2), 2) * this.BaseTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC12 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * this.BaseTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * this.BaseTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM23(phi1, psi1, phi2), 2) * this.BaseTensor.C33 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC12 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * this.BaseTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * this.BaseTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC12 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM23(phi1, psi1, phi2), 2) * this.BaseTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * this.BaseTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC12 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM23(phi1, psi1, phi2), 2) * this.BaseTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * this.BaseTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC12 += 4 * SCTM12(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * this.BaseTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += 4 * SCTM11(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM21(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * this.BaseTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += 4 * SCTM11(phi1, psi1, phi2) * SCTM12(phi1, psi1, phi2) * SCTM21(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * this.BaseTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC44 += Math.Pow(SCTM21(phi1, psi1, phi2), 2) * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * this.BaseTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += Math.Pow(SCTM22(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * this.BaseTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += Math.Pow(SCTM23(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * this.BaseTensor.C33 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC44 += 2 * SCTM21(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * this.BaseTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += 2 * SCTM21(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * this.BaseTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += 2 * SCTM22(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * this.BaseTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC44 += Math.Pow(SCTM22(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * this.BaseTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += 2 * SCTM23(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * this.BaseTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += Math.Pow(SCTM23(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * this.BaseTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC44 += Math.Pow(SCTM23(phi1, psi1, phi2), 2) * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * this.BaseTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += 2 * SCTM21(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * this.BaseTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += Math.Pow(SCTM21(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * this.BaseTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC44 += Math.Pow(SCTM21(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * this.BaseTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += 2 * SCTM21(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * this.BaseTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += Math.Pow(SCTM22(phi1, psi1, phi2), 2) * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * this.BaseTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);

                    }
                }
            }

            this.TextureTensor.C11 = (TC11 / normFactor);
            this.TextureTensor.C12 = (TC12 / normFactor);
            this.TextureTensor.C44 = (TC44 / normFactor);
        }

        private void SetTextureTensorHexagonalIso()
        {
            double TC11 = 0;
            double TC33 = 0;
            double TC12 = 0;
            double TC13 = 0;
            double TC44 = 0;

            double normFactor = 0.0;

            for (double phi1 = 0.0; phi1 < 360.0; phi1 += 5.0)
            {
                for (double psi1 = 0.0; psi1 < 360.0; psi1 += 5.0)
                {
                    for (double phi2 = 0.0; phi2 < 360.0; phi2 += 5.0)
                    {
                        normFactor += Math.Sin((psi1 * Math.PI) / 180.0);

                        TC11 += Math.Pow(SCTM11(phi1, psi1, phi2), 4) * this.BaseTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += Math.Pow(SCTM12(phi1, psi1, phi2), 4) * this.BaseTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += Math.Pow(SCTM13(phi1, psi1, phi2), 4) * this.BaseTensor.C33 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC11 += 2 * Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM12(phi1, psi1, phi2), 2) * this.BaseTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += 2 * Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM13(phi1, psi1, phi2), 2) * this.BaseTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += 2 * Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM13(phi1, psi1, phi2), 2) * this.BaseTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC11 += 4 * Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM13(phi1, psi1, phi2), 2) * this.BaseTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += 4 * Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM13(phi1, psi1, phi2), 2) * this.BaseTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += 4 * Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM12(phi1, psi1, phi2), 2) * this.BaseTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC33 += Math.Pow(SCTM31(phi1, psi1, phi2), 4) * this.BaseTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC33 += Math.Pow(SCTM32(phi1, psi1, phi2), 4) * this.BaseTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC33 += Math.Pow(SCTM33(phi1, psi1, phi2), 4) * this.BaseTensor.C33 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC33 += 2 * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * this.BaseTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC33 += 2 * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * this.BaseTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC33 += 2 * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * this.BaseTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC33 += 4 * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * this.BaseTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC33 += 4 * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * this.BaseTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC33 += 4 * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * this.BaseTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC12 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * this.BaseTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * this.BaseTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM23(phi1, psi1, phi2), 2) * this.BaseTensor.C33 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC12 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * this.BaseTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * this.BaseTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC12 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM23(phi1, psi1, phi2), 2) * this.BaseTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * this.BaseTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC12 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM23(phi1, psi1, phi2), 2) * this.BaseTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * this.BaseTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC12 += 4 * SCTM12(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * this.BaseTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += 4 * SCTM11(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM21(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * this.BaseTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += 4 * SCTM11(phi1, psi1, phi2) * SCTM12(phi1, psi1, phi2) * SCTM21(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * this.BaseTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC13 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * this.BaseTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC13 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * this.BaseTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC13 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * this.BaseTensor.C33 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC13 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * this.BaseTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC13 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * this.BaseTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC13 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * this.BaseTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC13 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * this.BaseTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC13 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * this.BaseTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC13 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * this.BaseTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC13 += 4 * SCTM12(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * this.BaseTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC13 += 4 * SCTM11(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * this.BaseTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC13 += 4 * SCTM11(phi1, psi1, phi2) * SCTM12(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * this.BaseTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC44 += Math.Pow(SCTM21(phi1, psi1, phi2), 2) * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * this.BaseTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += Math.Pow(SCTM22(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * this.BaseTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += Math.Pow(SCTM23(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * this.BaseTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC44 += 2 * SCTM21(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * this.BaseTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += 2 * SCTM21(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * this.BaseTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += 2 * SCTM22(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * this.BaseTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC44 += 2 * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * this.BaseTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += 2 * SCTM23(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * this.BaseTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC44 += 2 * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * this.BaseTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += 2 * SCTM21(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * this.BaseTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC44 += 2 * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * this.BaseTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += 2 * SCTM21(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * this.BaseTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);
                    }
                }
            }

            normFactor *= 8.0 * Math.Pow(Math.PI, 2);

            this.TextureTensor.C11 = TC11 / normFactor;
            this.TextureTensor.C33 = TC33 / normFactor;
            this.TextureTensor.C12 = TC12 / normFactor;
            this.TextureTensor.C13 = TC13 / normFactor;
            this.TextureTensor.C44 = TC44 / normFactor;

        }

        private void SetTextureTensorTetragonalType1Iso()
        {
            double TC11 = 0;
            double TC33 = 0;
            double TC12 = 0;
            double TC13 = 0;
            double TC44 = 0;
            double TC66 = 0;

            double normFactor = 0.0;

            for (double phi1 = 0.0; phi1 < 360.0; phi1 += 5.0)
            {
                for (double psi1 = 0.0; psi1 < 360.0; psi1 += 5.0)
                {
                    for (double phi2 = 0.0; phi2 < 360.0; phi2 += 5.0)
                    {
                        normFactor += Math.Sin((psi1 * Math.PI) / 180.0);

                        TC11 += Math.Pow(SCTM11(phi1, psi1, phi2), 4) * this.BaseTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += Math.Pow(SCTM12(phi1, psi1, phi2), 4) * this.BaseTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += Math.Pow(SCTM13(phi1, psi1, phi2), 4) * this.BaseTensor.C33 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC11 += 2 * Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM12(phi1, psi1, phi2), 2) * this.BaseTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += 2 * Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM13(phi1, psi1, phi2), 2) * this.BaseTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += 2 * Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM13(phi1, psi1, phi2), 2) * this.BaseTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC11 += 4 * Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM13(phi1, psi1, phi2), 2) * this.BaseTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += 4 * Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM13(phi1, psi1, phi2), 2) * this.BaseTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += 4 * Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM12(phi1, psi1, phi2), 2) * this.BaseTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC33 += Math.Pow(SCTM31(phi1, psi1, phi2), 4) * this.BaseTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC33 += Math.Pow(SCTM32(phi1, psi1, phi2), 4) * this.BaseTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC33 += Math.Pow(SCTM33(phi1, psi1, phi2), 4) * this.BaseTensor.C33 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC33 += 2 * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * this.BaseTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC33 += 2 * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * this.BaseTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC33 += 2 * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * this.BaseTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC33 += 4 * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * this.BaseTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC33 += 4 * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * this.BaseTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC33 += 4 * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * this.BaseTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC12 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * this.BaseTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * this.BaseTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM23(phi1, psi1, phi2), 2) * this.BaseTensor.C33 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC12 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * this.BaseTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * this.BaseTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC12 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM23(phi1, psi1, phi2), 2) * this.BaseTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * this.BaseTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC12 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM23(phi1, psi1, phi2), 2) * this.BaseTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * this.BaseTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC12 += 4 * SCTM12(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * this.BaseTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += 4 * SCTM11(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM21(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * this.BaseTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += 4 * SCTM11(phi1, psi1, phi2) * SCTM12(phi1, psi1, phi2) * SCTM21(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * this.BaseTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC13 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * this.BaseTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC13 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * this.BaseTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC13 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * this.BaseTensor.C33 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC13 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * this.BaseTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC13 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * this.BaseTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC13 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * this.BaseTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC13 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * this.BaseTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC13 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * this.BaseTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC13 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * this.BaseTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC13 += 4 * SCTM12(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * this.BaseTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC13 += 4 * SCTM11(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * this.BaseTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC13 += 4 * SCTM11(phi1, psi1, phi2) * SCTM12(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * this.BaseTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC44 += Math.Pow(SCTM21(phi1, psi1, phi2), 2) * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * this.BaseTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += Math.Pow(SCTM22(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * this.BaseTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += Math.Pow(SCTM23(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * this.BaseTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC44 += 2 * SCTM21(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * this.BaseTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += 2 * SCTM21(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * this.BaseTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += 2 * SCTM22(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * this.BaseTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC44 += 2 * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * this.BaseTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += 2 * SCTM23(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * this.BaseTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC44 += 2 * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * this.BaseTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += 2 * SCTM21(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * this.BaseTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC44 += 2 * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * this.BaseTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += 2 * SCTM21(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * this.BaseTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC66 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * this.BaseTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC66 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * this.BaseTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC66 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM23(phi1, psi1, phi2), 2) * this.BaseTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC66 += 2 * SCTM11(phi1, psi1, phi2) * SCTM21(phi1, psi1, phi2) * SCTM12(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * this.BaseTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC66 += 2 * SCTM11(phi1, psi1, phi2) * SCTM21(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * this.BaseTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC66 += 2 * SCTM12(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * this.BaseTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC66 += 2 * Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM23(phi1, psi1, phi2), 2) * this.BaseTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC66 += 2 * SCTM13(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM12(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * this.BaseTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC66 += 2 * Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM23(phi1, psi1, phi2), 2) * this.BaseTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC66 += 2 * SCTM11(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM21(phi1, psi1, phi2) * this.BaseTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC66 += 2 * Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * this.BaseTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC66 += 2 * SCTM11(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM12(phi1, psi1, phi2) * SCTM21(phi1, psi1, phi2) * this.BaseTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);
                    }
                }
            }

            normFactor /= 8.0 * Math.PI;

            this.TextureTensor.C11 = TC11 / normFactor;
            this.TextureTensor.C33 = TC33 / normFactor;
            this.TextureTensor.C12 = TC12 / normFactor;
            this.TextureTensor.C13 = TC13 / normFactor;
            this.TextureTensor.C44 = TC44 / normFactor;
            this.TextureTensor.C66 = TC66 / normFactor;

        }

        private void SetTextureTensorRhombicIso()
        {
            double TC11 = 0;
            double TC22 = 0;
            double TC33 = 0;
            double TC12 = 0;
            double TC13 = 0;
            double TC23 = 0;
            double TC44 = 0;
            double TC55 = 0;
            double TC66 = 0;

            double normFactor = 0.0;

            for (double phi1 = 0.0; phi1 < 360.0; phi1 += 5.0)
            {
                for (double psi1 = 0.0; psi1 < 360.0; psi1 += 5.0)
                {
                    for (double phi2 = 0.0; phi2 < 360.0; phi2 += 5.0)
                    {
                        normFactor += Math.Sin((psi1 * Math.PI) / 180.0);

                        TC11 += Math.Pow(SCTM11(phi1, psi1, phi2), 4) * this.BaseTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += Math.Pow(SCTM12(phi1, psi1, phi2), 4) * this.BaseTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += Math.Pow(SCTM13(phi1, psi1, phi2), 4) * this.BaseTensor.C33 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC11 += 2 * Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM12(phi1, psi1, phi2), 2) * this.BaseTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += 2 * Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM13(phi1, psi1, phi2), 2) * this.BaseTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += 2 * Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM13(phi1, psi1, phi2), 2) * this.BaseTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC11 += 4 * Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM13(phi1, psi1, phi2), 2) * this.BaseTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += 4 * Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM13(phi1, psi1, phi2), 2) * this.BaseTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += 4 * Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM12(phi1, psi1, phi2), 2) * this.BaseTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC22 += Math.Pow(SCTM21(phi1, psi1, phi2), 4) * this.BaseTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC22 += Math.Pow(SCTM22(phi1, psi1, phi2), 4) * this.BaseTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC22 += Math.Pow(SCTM23(phi1, psi1, phi2), 4) * this.BaseTensor.C33 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC22 += 2 * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * this.BaseTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC22 += 2 * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * Math.Pow(SCTM23(phi1, psi1, phi2), 2) * this.BaseTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC22 += 2 * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * Math.Pow(SCTM23(phi1, psi1, phi2), 2) * this.BaseTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC22 += 4 * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * Math.Pow(SCTM23(phi1, psi1, phi2), 2) * this.BaseTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC22 += 4 * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * Math.Pow(SCTM23(phi1, psi1, phi2), 2) * this.BaseTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC22 += 4 * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * this.BaseTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC33 += Math.Pow(SCTM31(phi1, psi1, phi2), 4) * this.BaseTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC33 += Math.Pow(SCTM32(phi1, psi1, phi2), 4) * this.BaseTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC33 += Math.Pow(SCTM33(phi1, psi1, phi2), 4) * this.BaseTensor.C33 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC33 += 2 * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * this.BaseTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC33 += 2 * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * this.BaseTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC33 += 2 * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * this.BaseTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC33 += 4 * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * this.BaseTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC33 += 4 * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * this.BaseTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC33 += 4 * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * this.BaseTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC12 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * this.BaseTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * this.BaseTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM23(phi1, psi1, phi2), 2) * this.BaseTensor.C33 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC12 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * this.BaseTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * this.BaseTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC12 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM23(phi1, psi1, phi2), 2) * this.BaseTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * this.BaseTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC12 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM23(phi1, psi1, phi2), 2) * this.BaseTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * this.BaseTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC12 += 4 * SCTM12(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * this.BaseTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += 4 * SCTM11(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM21(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * this.BaseTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += 4 * SCTM11(phi1, psi1, phi2) * SCTM12(phi1, psi1, phi2) * SCTM21(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * this.BaseTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC13 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * this.BaseTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC13 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * this.BaseTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC13 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * this.BaseTensor.C33 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC13 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * this.BaseTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC13 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * this.BaseTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC13 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * this.BaseTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC13 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * this.BaseTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC13 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * this.BaseTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC13 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * this.BaseTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC13 += 4 * SCTM12(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * this.BaseTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC13 += 4 * SCTM11(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * this.BaseTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC13 += 4 * SCTM11(phi1, psi1, phi2) * SCTM12(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * this.BaseTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC23 += Math.Pow(SCTM21(phi1, psi1, phi2), 2) * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * this.BaseTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC23 += Math.Pow(SCTM22(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * this.BaseTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC23 += Math.Pow(SCTM23(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * this.BaseTensor.C33 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC23 += Math.Pow(SCTM21(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * this.BaseTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC23 += Math.Pow(SCTM22(phi1, psi1, phi2), 2) * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * this.BaseTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC23 += Math.Pow(SCTM21(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * this.BaseTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC23 += Math.Pow(SCTM23(phi1, psi1, phi2), 2) * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * this.BaseTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC23 += Math.Pow(SCTM22(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * this.BaseTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC23 += Math.Pow(SCTM23(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * this.BaseTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC23 += 4 * SCTM22(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * this.BaseTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC23 += 4 * SCTM21(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * this.BaseTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC23 += 4 * SCTM21(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * this.BaseTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC44 += Math.Pow(SCTM21(phi1, psi1, phi2), 2) * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * this.BaseTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += Math.Pow(SCTM22(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * this.BaseTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += Math.Pow(SCTM23(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * this.BaseTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC44 += 2 * SCTM21(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * this.BaseTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += 2 * SCTM21(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * this.BaseTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += 2 * SCTM22(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * this.BaseTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC44 += 2 * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * this.BaseTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += 2 * SCTM23(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * this.BaseTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC44 += 2 * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * this.BaseTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += 2 * SCTM21(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * this.BaseTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC44 += 2 * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * this.BaseTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += 2 * SCTM21(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * this.BaseTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC55 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * this.BaseTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC55 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * this.BaseTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC55 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * this.BaseTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC55 += 2 * SCTM11(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * SCTM12(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * this.BaseTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC55 += 2 * SCTM11(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * this.BaseTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC55 += 2 * SCTM12(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * this.BaseTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC55 += 2 * Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * this.BaseTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC55 += 2 * SCTM13(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * SCTM12(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * this.BaseTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC55 += 2 * Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * this.BaseTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC55 += 2 * SCTM11(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * this.BaseTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC55 += 2 * Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * this.BaseTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC55 += 2 * SCTM11(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * SCTM12(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * this.BaseTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC66 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * this.BaseTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC66 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * this.BaseTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC66 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM23(phi1, psi1, phi2), 2) * this.BaseTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC66 += 2 * SCTM11(phi1, psi1, phi2) * SCTM21(phi1, psi1, phi2) * SCTM12(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * this.BaseTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC66 += 2 * SCTM11(phi1, psi1, phi2) * SCTM21(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * this.BaseTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC66 += 2 * SCTM12(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * this.BaseTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC66 += 2 * Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM23(phi1, psi1, phi2), 2) * this.BaseTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC66 += 2 * SCTM13(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM12(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * this.BaseTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC66 += 2 * Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM23(phi1, psi1, phi2), 2) * this.BaseTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC66 += 2 * SCTM11(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM21(phi1, psi1, phi2) * this.BaseTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC66 += 2 * Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * this.BaseTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC66 += 2 * SCTM11(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM12(phi1, psi1, phi2) * SCTM21(phi1, psi1, phi2) * this.BaseTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);
                    }
                }
            }

            normFactor /= 8.0 * Math.PI;

            this.TextureTensor.C11 = TC11 / normFactor;
            this.TextureTensor.C22 = TC22 / normFactor;
            this.TextureTensor.C33 = TC33 / normFactor;
            this.TextureTensor.C12 = TC12 / normFactor;
            this.TextureTensor.C13 = TC13 / normFactor;
            this.TextureTensor.C23 = TC23 / normFactor;
            this.TextureTensor.C44 = TC44 / normFactor;
            this.TextureTensor.C55 = TC55 / normFactor;
            this.TextureTensor.C66 = TC66 / normFactor;

        }

        private void SetTextureTensorCubicIso(Stress.Microsopic.ElasticityTensors averagingTensor)
        {
            double TC11 = 0;
            double TC12 = 0;
            double TC44 = 0;

            double normFactor = 0.0;

            for (double phi1 = 0.0; phi1 < 360.0; phi1 += 5.0)
            {
                for (double psi1 = 0.0; psi1 < 360.0; psi1 += 5.0)
                {
                    for (double phi2 = 0.0; phi2 < 360.0; phi2 += 5.0)
                    {
                        normFactor += Math.Sin((psi1 * Math.PI) / 180.0);

                        TC11 += Math.Pow(SCTM11(phi1, psi1, phi2), 4) * averagingTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += Math.Pow(SCTM12(phi1, psi1, phi2), 4) * averagingTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += Math.Pow(SCTM13(phi1, psi1, phi2), 4) * averagingTensor.C33 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC11 += 2 * Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM12(phi1, psi1, phi2), 2) * averagingTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += 2 * Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM13(phi1, psi1, phi2), 2) * averagingTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += 2 * Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM13(phi1, psi1, phi2), 2) * averagingTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC11 += 4 * Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM13(phi1, psi1, phi2), 2) * averagingTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += 4 * Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM13(phi1, psi1, phi2), 2) * averagingTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += 4 * Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM12(phi1, psi1, phi2), 2) * averagingTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC12 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * averagingTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * averagingTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM23(phi1, psi1, phi2), 2) * averagingTensor.C33 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC12 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * averagingTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * averagingTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC12 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM23(phi1, psi1, phi2), 2) * averagingTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * averagingTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC12 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM23(phi1, psi1, phi2), 2) * averagingTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * averagingTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC12 += 4 * SCTM12(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * averagingTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += 4 * SCTM11(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM21(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * averagingTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += 4 * SCTM11(phi1, psi1, phi2) * SCTM12(phi1, psi1, phi2) * SCTM21(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * averagingTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC44 += Math.Pow(SCTM21(phi1, psi1, phi2), 2) * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * averagingTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += Math.Pow(SCTM22(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * averagingTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += Math.Pow(SCTM23(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * averagingTensor.C33 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC44 += 2 * SCTM21(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * averagingTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += 2 * SCTM21(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * averagingTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += 2 * SCTM22(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * averagingTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC44 += Math.Pow(SCTM22(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * averagingTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += 2 * SCTM23(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * averagingTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += Math.Pow(SCTM23(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * averagingTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC44 += Math.Pow(SCTM23(phi1, psi1, phi2), 2) * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * averagingTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += 2 * SCTM21(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * averagingTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += Math.Pow(SCTM21(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * averagingTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC44 += Math.Pow(SCTM21(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * averagingTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += 2 * SCTM21(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * averagingTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += Math.Pow(SCTM22(phi1, psi1, phi2), 2) * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * averagingTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);

                    }
                }
            }

            this.TextureTensor.C11 = (TC11 / normFactor);
            this.TextureTensor.C12 = (TC12 / normFactor);
            this.TextureTensor.C44 = (TC44 / normFactor);
        }

        private void SetTextureTensorHexagonalIso(Stress.Microsopic.ElasticityTensors averagingTensor)
        {
            double TC11 = 0;
            double TC33 = 0;
            double TC12 = 0;
            double TC13 = 0;
            double TC44 = 0;

            double normFactor = 0.0;

            for (double phi1 = 0.0; phi1 < 360.0; phi1 += 5.0)
            {
                for (double psi1 = 0.0; psi1 < 360.0; psi1 += 5.0)
                {
                    for (double phi2 = 0.0; phi2 < 360.0; phi2 += 5.0)
                    {
                        normFactor += Math.Sin((psi1 * Math.PI) / 180.0);

                        TC11 += Math.Pow(SCTM11(phi1, psi1, phi2), 4) * averagingTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += Math.Pow(SCTM12(phi1, psi1, phi2), 4) * averagingTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += Math.Pow(SCTM13(phi1, psi1, phi2), 4) * averagingTensor.C33 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC11 += 2 * Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM12(phi1, psi1, phi2), 2) * averagingTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += 2 * Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM13(phi1, psi1, phi2), 2) * averagingTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += 2 * Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM13(phi1, psi1, phi2), 2) * averagingTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC11 += 4 * Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM13(phi1, psi1, phi2), 2) * averagingTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += 4 * Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM13(phi1, psi1, phi2), 2) * averagingTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += 4 * Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM12(phi1, psi1, phi2), 2) * averagingTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC33 += Math.Pow(SCTM31(phi1, psi1, phi2), 4) * averagingTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC33 += Math.Pow(SCTM32(phi1, psi1, phi2), 4) * averagingTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC33 += Math.Pow(SCTM33(phi1, psi1, phi2), 4) * averagingTensor.C33 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC33 += 2 * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * averagingTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC33 += 2 * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * averagingTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC33 += 2 * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * averagingTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC33 += 4 * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * averagingTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC33 += 4 * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * averagingTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC33 += 4 * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * averagingTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC12 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * averagingTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * averagingTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM23(phi1, psi1, phi2), 2) * averagingTensor.C33 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC12 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * averagingTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * averagingTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC12 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM23(phi1, psi1, phi2), 2) * averagingTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * averagingTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC12 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM23(phi1, psi1, phi2), 2) * averagingTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * averagingTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC12 += 4 * SCTM12(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * averagingTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += 4 * SCTM11(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM21(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * averagingTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += 4 * SCTM11(phi1, psi1, phi2) * SCTM12(phi1, psi1, phi2) * SCTM21(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * averagingTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC13 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * averagingTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC13 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * averagingTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC13 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * averagingTensor.C33 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC13 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * averagingTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC13 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * averagingTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC13 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * averagingTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC13 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * averagingTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC13 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * averagingTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC13 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * averagingTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC13 += 4 * SCTM12(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * averagingTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC13 += 4 * SCTM11(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * averagingTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC13 += 4 * SCTM11(phi1, psi1, phi2) * SCTM12(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * averagingTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC44 += Math.Pow(SCTM21(phi1, psi1, phi2), 2) * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * averagingTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += Math.Pow(SCTM22(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * averagingTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += Math.Pow(SCTM23(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * averagingTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC44 += 2 * SCTM21(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * averagingTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += 2 * SCTM21(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * averagingTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += 2 * SCTM22(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * averagingTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC44 += 2 * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * averagingTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += 2 * SCTM23(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * averagingTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC44 += 2 * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * averagingTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += 2 * SCTM21(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * averagingTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC44 += 2 * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * averagingTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += 2 * SCTM21(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * averagingTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);
                    }
                }
            }

            normFactor *= 8.0 * Math.Pow(Math.PI, 2);

            this.TextureTensor.C11 = TC11 / normFactor;
            this.TextureTensor.C33 = TC33 / normFactor;
            this.TextureTensor.C12 = TC12 / normFactor;
            this.TextureTensor.C13 = TC13 / normFactor;
            this.TextureTensor.C44 = TC44 / normFactor;

        }

        private void SetTextureTensorTetragonalType1Iso(Stress.Microsopic.ElasticityTensors averagingTensor)
        {
            double TC11 = 0;
            double TC33 = 0;
            double TC12 = 0;
            double TC13 = 0;
            double TC44 = 0;
            double TC66 = 0;

            double normFactor = 0.0;

            for (double phi1 = 0.0; phi1 < 360.0; phi1 += 5.0)
            {
                for (double psi1 = 0.0; psi1 < 360.0; psi1 += 5.0)
                {
                    for (double phi2 = 0.0; phi2 < 360.0; phi2 += 5.0)
                    {
                        normFactor += Math.Sin((psi1 * Math.PI) / 180.0);

                        TC11 += Math.Pow(SCTM11(phi1, psi1, phi2), 4) * averagingTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += Math.Pow(SCTM12(phi1, psi1, phi2), 4) * averagingTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += Math.Pow(SCTM13(phi1, psi1, phi2), 4) * averagingTensor.C33 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC11 += 2 * Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM12(phi1, psi1, phi2), 2) * averagingTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += 2 * Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM13(phi1, psi1, phi2), 2) * averagingTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += 2 * Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM13(phi1, psi1, phi2), 2) * averagingTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC11 += 4 * Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM13(phi1, psi1, phi2), 2) * averagingTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += 4 * Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM13(phi1, psi1, phi2), 2) * averagingTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += 4 * Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM12(phi1, psi1, phi2), 2) * averagingTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC33 += Math.Pow(SCTM31(phi1, psi1, phi2), 4) * averagingTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC33 += Math.Pow(SCTM32(phi1, psi1, phi2), 4) * averagingTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC33 += Math.Pow(SCTM33(phi1, psi1, phi2), 4) * averagingTensor.C33 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC33 += 2 * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * averagingTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC33 += 2 * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * averagingTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC33 += 2 * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * averagingTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC33 += 4 * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * averagingTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC33 += 4 * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * averagingTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC33 += 4 * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * averagingTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC12 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * averagingTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * averagingTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM23(phi1, psi1, phi2), 2) * averagingTensor.C33 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC12 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * averagingTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * averagingTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC12 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM23(phi1, psi1, phi2), 2) * averagingTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * averagingTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC12 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM23(phi1, psi1, phi2), 2) * averagingTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * averagingTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC12 += 4 * SCTM12(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * averagingTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += 4 * SCTM11(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM21(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * averagingTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += 4 * SCTM11(phi1, psi1, phi2) * SCTM12(phi1, psi1, phi2) * SCTM21(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * averagingTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC13 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * averagingTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC13 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * averagingTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC13 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * averagingTensor.C33 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC13 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * averagingTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC13 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * averagingTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC13 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * averagingTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC13 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * averagingTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC13 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * averagingTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC13 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * averagingTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC13 += 4 * SCTM12(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * averagingTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC13 += 4 * SCTM11(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * averagingTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC13 += 4 * SCTM11(phi1, psi1, phi2) * SCTM12(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * averagingTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC44 += Math.Pow(SCTM21(phi1, psi1, phi2), 2) * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * averagingTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += Math.Pow(SCTM22(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * averagingTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += Math.Pow(SCTM23(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * averagingTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC44 += 2 * SCTM21(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * averagingTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += 2 * SCTM21(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * averagingTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += 2 * SCTM22(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * averagingTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC44 += 2 * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * averagingTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += 2 * SCTM23(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * averagingTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC44 += 2 * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * averagingTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += 2 * SCTM21(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * averagingTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC44 += 2 * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * averagingTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += 2 * SCTM21(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * averagingTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC66 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * averagingTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC66 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * averagingTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC66 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM23(phi1, psi1, phi2), 2) * averagingTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC66 += 2 * SCTM11(phi1, psi1, phi2) * SCTM21(phi1, psi1, phi2) * SCTM12(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * averagingTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC66 += 2 * SCTM11(phi1, psi1, phi2) * SCTM21(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * averagingTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC66 += 2 * SCTM12(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * averagingTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC66 += 2 * Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM23(phi1, psi1, phi2), 2) * averagingTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC66 += 2 * SCTM13(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM12(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * averagingTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC66 += 2 * Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM23(phi1, psi1, phi2), 2) * averagingTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC66 += 2 * SCTM11(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM21(phi1, psi1, phi2) * averagingTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC66 += 2 * Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * averagingTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC66 += 2 * SCTM11(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM12(phi1, psi1, phi2) * SCTM21(phi1, psi1, phi2) * averagingTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);
                    }
                }
            }

            normFactor /= 8.0 * Math.PI;

            this.TextureTensor.C11 = TC11 / normFactor;
            this.TextureTensor.C33 = TC33 / normFactor;
            this.TextureTensor.C12 = TC12 / normFactor;
            this.TextureTensor.C13 = TC13 / normFactor;
            this.TextureTensor.C44 = TC44 / normFactor;
            this.TextureTensor.C66 = TC66 / normFactor;

        }

        private void SetTextureTensorRhombicIso(Stress.Microsopic.ElasticityTensors averagingTensor)
        {
            double TC11 = 0;
            double TC22 = 0;
            double TC33 = 0;
            double TC12 = 0;
            double TC13 = 0;
            double TC23 = 0;
            double TC44 = 0;
            double TC55 = 0;
            double TC66 = 0;

            double normFactor = 0.0;

            for (double phi1 = 0.0; phi1 < 360.0; phi1 += 5.0)
            {
                for (double psi1 = 0.0; psi1 < 360.0; psi1 += 5.0)
                {
                    for (double phi2 = 0.0; phi2 < 360.0; phi2 += 5.0)
                    {
                        normFactor += Math.Sin((psi1 * Math.PI) / 180.0);

                        TC11 += Math.Pow(SCTM11(phi1, psi1, phi2), 4) * averagingTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += Math.Pow(SCTM12(phi1, psi1, phi2), 4) * averagingTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += Math.Pow(SCTM13(phi1, psi1, phi2), 4) * averagingTensor.C33 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC11 += 2 * Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM12(phi1, psi1, phi2), 2) * averagingTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += 2 * Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM13(phi1, psi1, phi2), 2) * averagingTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += 2 * Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM13(phi1, psi1, phi2), 2) * averagingTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC11 += 4 * Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM13(phi1, psi1, phi2), 2) * averagingTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += 4 * Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM13(phi1, psi1, phi2), 2) * averagingTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC11 += 4 * Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM12(phi1, psi1, phi2), 2) * averagingTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC22 += Math.Pow(SCTM21(phi1, psi1, phi2), 4) * averagingTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC22 += Math.Pow(SCTM22(phi1, psi1, phi2), 4) * averagingTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC22 += Math.Pow(SCTM23(phi1, psi1, phi2), 4) * averagingTensor.C33 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC22 += 2 * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * averagingTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC22 += 2 * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * Math.Pow(SCTM23(phi1, psi1, phi2), 2) * averagingTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC22 += 2 * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * Math.Pow(SCTM23(phi1, psi1, phi2), 2) * averagingTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC22 += 4 * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * Math.Pow(SCTM23(phi1, psi1, phi2), 2) * averagingTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC22 += 4 * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * Math.Pow(SCTM23(phi1, psi1, phi2), 2) * averagingTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC22 += 4 * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * averagingTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC33 += Math.Pow(SCTM31(phi1, psi1, phi2), 4) * averagingTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC33 += Math.Pow(SCTM32(phi1, psi1, phi2), 4) * averagingTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC33 += Math.Pow(SCTM33(phi1, psi1, phi2), 4) * averagingTensor.C33 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC33 += 2 * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * averagingTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC33 += 2 * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * averagingTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC33 += 2 * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * averagingTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC33 += 4 * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * averagingTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC33 += 4 * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * averagingTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC33 += 4 * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * averagingTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC12 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * averagingTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * averagingTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM23(phi1, psi1, phi2), 2) * averagingTensor.C33 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC12 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * averagingTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * averagingTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC12 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM23(phi1, psi1, phi2), 2) * averagingTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * averagingTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC12 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM23(phi1, psi1, phi2), 2) * averagingTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * averagingTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC12 += 4 * SCTM12(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * averagingTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += 4 * SCTM11(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM21(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * averagingTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC12 += 4 * SCTM11(phi1, psi1, phi2) * SCTM12(phi1, psi1, phi2) * SCTM21(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * averagingTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC13 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * averagingTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC13 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * averagingTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC13 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * averagingTensor.C33 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC13 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * averagingTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC13 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * averagingTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC13 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * averagingTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC13 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * averagingTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC13 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * averagingTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC13 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * averagingTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC13 += 4 * SCTM12(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * averagingTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC13 += 4 * SCTM11(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * averagingTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC13 += 4 * SCTM11(phi1, psi1, phi2) * SCTM12(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * averagingTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC23 += Math.Pow(SCTM21(phi1, psi1, phi2), 2) * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * averagingTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC23 += Math.Pow(SCTM22(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * averagingTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC23 += Math.Pow(SCTM23(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * averagingTensor.C33 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC23 += Math.Pow(SCTM21(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * averagingTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC23 += Math.Pow(SCTM22(phi1, psi1, phi2), 2) * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * averagingTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC23 += Math.Pow(SCTM21(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * averagingTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC23 += Math.Pow(SCTM23(phi1, psi1, phi2), 2) * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * averagingTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC23 += Math.Pow(SCTM22(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * averagingTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC23 += Math.Pow(SCTM23(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * averagingTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC23 += 4 * SCTM22(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * averagingTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC23 += 4 * SCTM21(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * averagingTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC23 += 4 * SCTM21(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * averagingTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC44 += Math.Pow(SCTM21(phi1, psi1, phi2), 2) * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * averagingTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += Math.Pow(SCTM22(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * averagingTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += Math.Pow(SCTM23(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * averagingTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC44 += 2 * SCTM21(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * averagingTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += 2 * SCTM21(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * averagingTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += 2 * SCTM22(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * averagingTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC44 += 2 * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * averagingTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += 2 * SCTM23(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * averagingTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC44 += 2 * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * averagingTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += 2 * SCTM21(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * averagingTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC44 += 2 * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * averagingTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC44 += 2 * SCTM21(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * averagingTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC55 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM31(phi1, psi1, phi2), 2) * averagingTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC55 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * averagingTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC55 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * averagingTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC55 += 2 * SCTM11(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * SCTM12(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * averagingTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC55 += 2 * SCTM11(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * averagingTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC55 += 2 * SCTM12(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * averagingTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC55 += 2 * Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * averagingTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC55 += 2 * SCTM13(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * SCTM12(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * averagingTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC55 += 2 * Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM33(phi1, psi1, phi2), 2) * averagingTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC55 += 2 * SCTM11(phi1, psi1, phi2) * SCTM33(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * averagingTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC55 += 2 * Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM32(phi1, psi1, phi2), 2) * averagingTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC55 += 2 * SCTM11(phi1, psi1, phi2) * SCTM32(phi1, psi1, phi2) * SCTM12(phi1, psi1, phi2) * SCTM31(phi1, psi1, phi2) * averagingTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC66 += Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM21(phi1, psi1, phi2), 2) * averagingTensor.C11 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC66 += Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * averagingTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC66 += Math.Pow(SCTM13(phi1, psi1, phi2), 2) * Math.Pow(SCTM23(phi1, psi1, phi2), 2) * averagingTensor.C22 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC66 += 2 * SCTM11(phi1, psi1, phi2) * SCTM21(phi1, psi1, phi2) * SCTM12(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * averagingTensor.C12 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC66 += 2 * SCTM11(phi1, psi1, phi2) * SCTM21(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * averagingTensor.C13 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC66 += 2 * SCTM12(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * averagingTensor.C23 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC66 += 2 * Math.Pow(SCTM12(phi1, psi1, phi2), 2) * Math.Pow(SCTM23(phi1, psi1, phi2), 2) * averagingTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC66 += 2 * SCTM13(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM12(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * averagingTensor.C44 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC66 += 2 * Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM23(phi1, psi1, phi2), 2) * averagingTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC66 += 2 * SCTM11(phi1, psi1, phi2) * SCTM23(phi1, psi1, phi2) * SCTM13(phi1, psi1, phi2) * SCTM21(phi1, psi1, phi2) * averagingTensor.C55 * Math.Sin((psi1 * Math.PI) / 180.0);

                        TC66 += 2 * Math.Pow(SCTM11(phi1, psi1, phi2), 2) * Math.Pow(SCTM22(phi1, psi1, phi2), 2) * averagingTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);
                        TC66 += 2 * SCTM11(phi1, psi1, phi2) * SCTM22(phi1, psi1, phi2) * SCTM12(phi1, psi1, phi2) * SCTM21(phi1, psi1, phi2) * averagingTensor.C66 * Math.Sin((psi1 * Math.PI) / 180.0);
                    }
                }
            }

            normFactor /= 8.0 * Math.PI;

            this.TextureTensor.C11 = TC11 / normFactor;
            this.TextureTensor.C22 = TC22 / normFactor;
            this.TextureTensor.C33 = TC33 / normFactor;
            this.TextureTensor.C12 = TC12 / normFactor;
            this.TextureTensor.C13 = TC13 / normFactor;
            this.TextureTensor.C23 = TC23 / normFactor;
            this.TextureTensor.C44 = TC44 / normFactor;
            this.TextureTensor.C55 = TC55 / normFactor;
            this.TextureTensor.C66 = TC66 / normFactor;

        }

        #endregion

        #region StressFactors

        #region cubic

        #region Voigt

        public MathNet.Numerics.LinearAlgebra.Matrix<double> GetStressFactorMatrixVoigt(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            MathNet.Numerics.LinearAlgebra.Matrix<double> ret = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
            double normFactor = 0;

            List<double[]> IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

            for (int n = 0; n < IntegrationAngles.Count; n++)
            {
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedStiffnessTensorCubic(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorCubic(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                ret[0, 0] += stressFactor11IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                ret[0, 1] += stressFactor12IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                ret[0, 2] += stressFactor13IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                ret[1, 1] += stressFactor22IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                ret[1, 2] += stressFactor23IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                ret[2, 2] += stressFactor33IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
            }

            ret[1, 0] = ret[0, 1];
            ret[2, 0] = ret[0, 2];
            ret[2, 1] = ret[1, 2];

            if (normFactor == 0)
            {
                return ret;
            }
            else
            {
                return ret / normFactor;
            }
        }

        private double GetStressFactor11VoigtCubic(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            double ret = 0;
            double normFactor = 0;

            List<double[]> IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

            for (int n = 0; n < IntegrationAngles.Count; n++)
            {
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedStiffnessTensorCubic(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorCubic(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                ret += stressFactor11IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
            }

            if (normFactor == 0)
            {
                return 0;
            }
            else
            {
                return ret / normFactor;
            }
        }

        private double GetStressFactor12VoigtCubic(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            double ret = 0;
            double normFactor = 0;

            List<double[]> IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

            for (int n = 0; n < IntegrationAngles.Count; n++)
            {
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedStiffnessTensorCubic(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorCubic(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                ret += stressFactor12IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
            }

            if (normFactor == 0)
            {
                return 0;
            }
            else
            {
                return ret / normFactor;
            }
        }

        private double GetStressFactor13VoigtCubic(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            double ret = 0;
            double normFactor = 0;

            List<double[]> IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

            for (int n = 0; n < IntegrationAngles.Count; n++)
            {
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedStiffnessTensorCubic(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorCubic(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                ret += stressFactor13IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
            }

            if (normFactor == 0)
            {
                return 0;
            }
            else
            {
                return ret / normFactor;
            }
        }

        private double GetStressFactor22VoigtCubic(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            double ret = 0;
            double normFactor = 0;

            List<double[]> IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

            for (int n = 0; n < IntegrationAngles.Count; n++)
            {
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedStiffnessTensorCubic(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorCubic(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                ret += stressFactor22IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
            }

            if (normFactor == 0)
            {
                return 0;
            }
            else
            {
                return ret / normFactor;
            }
        }

        private double GetStressFactor23VoigtCubic(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            double ret = 0;
            double normFactor = 0;

            List<double[]> IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

            for (int n = 0; n < IntegrationAngles.Count; n++)
            {
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedStiffnessTensorCubic(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorCubic(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                ret += stressFactor23IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
            }

            if (normFactor == 0)
            {
                return 0;
            }
            else
            {
                return ret / normFactor;
            }
        }

        private double GetStressFactor33VoigtCubic(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            double ret = 0;
            double normFactor = 0;

            List<double[]> IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

            for (int n = 0; n < IntegrationAngles.Count; n++)
            {
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedStiffnessTensorCubic(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorCubic(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                if (!double.IsNaN(orientedStiffnessTensor.S11) && !double.IsNaN(orientedStiffnessTensor.S12) && !double.IsNaN(orientedStiffnessTensor.S44))
                {
                    double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                    ret += stressFactor33IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                    normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                }
            }

            if (normFactor == 0)
            {
                return 0;
            }
            else
            {
                return ret / normFactor;
            }
        }

        private double stressFactor11IntegrantVoigt(Stress.Macroskopic.PeakStressAssociation usedAssociation, Stress.Microsopic.ElasticityTensors orientedStiffnessTensor, double oDFValue)
        {
            double ret = 0.0;
            MathNet.Numerics.LinearAlgebra.Vector<double> measurmentDirection = usedAssociation.MeasurementDirektionVektor;

            ret += orientedStiffnessTensor.S11 * measurmentDirection[0] * measurmentDirection[0] * oDFValue;
            ret += orientedStiffnessTensor.S21 * measurmentDirection[1] * measurmentDirection[1] * oDFValue;
            ret += orientedStiffnessTensor.S31 * measurmentDirection[2] * measurmentDirection[2] * oDFValue;

            return ret;
        }

        private double stressFactor12IntegrantVoigt(Stress.Macroskopic.PeakStressAssociation usedAssociation, Stress.Microsopic.ElasticityTensors orientedStiffnessTensor, double oDFValue)
        {
            double ret = 0.0;
            MathNet.Numerics.LinearAlgebra.Vector<double> measurmentDirection = usedAssociation.MeasurementDirektionVektor;

            ret += orientedStiffnessTensor.S66 * measurmentDirection[0] * measurmentDirection[1] * oDFValue;
            ret += orientedStiffnessTensor.S66 * measurmentDirection[1] * measurmentDirection[0] * oDFValue;

            return ret;
        }

        private double stressFactor13IntegrantVoigt(Stress.Macroskopic.PeakStressAssociation usedAssociation, Stress.Microsopic.ElasticityTensors orientedStiffnessTensor, double oDFValue)
        {
            double ret = 0.0;
            MathNet.Numerics.LinearAlgebra.Vector<double> measurmentDirection = usedAssociation.MeasurementDirektionVektor;

            ret += orientedStiffnessTensor.S55 * measurmentDirection[0] * measurmentDirection[2] * oDFValue;
            ret += orientedStiffnessTensor.S55 * measurmentDirection[2] * measurmentDirection[0] * oDFValue;

            return ret;
        }

        private double stressFactor22IntegrantVoigt(Stress.Macroskopic.PeakStressAssociation usedAssociation, Stress.Microsopic.ElasticityTensors orientedStiffnessTensor, double oDFValue)
        {
            double ret = 0.0;
            MathNet.Numerics.LinearAlgebra.Vector<double> measurmentDirection = usedAssociation.MeasurementDirektionVektor;

            ret += orientedStiffnessTensor.S12 * measurmentDirection[0] * measurmentDirection[1] * oDFValue;
            ret += orientedStiffnessTensor.S22 * measurmentDirection[1] * measurmentDirection[1] * oDFValue;
            ret += orientedStiffnessTensor.S23 * measurmentDirection[1] * measurmentDirection[2] * oDFValue;

            return ret;
        }

        private double stressFactor23IntegrantVoigt(Stress.Macroskopic.PeakStressAssociation usedAssociation, Stress.Microsopic.ElasticityTensors orientedStiffnessTensor, double oDFValue)
        {
            double ret = 0.0;
            MathNet.Numerics.LinearAlgebra.Vector<double> measurmentDirection = usedAssociation.MeasurementDirektionVektor;

            ret += orientedStiffnessTensor.S44 * measurmentDirection[1] * measurmentDirection[2] * oDFValue;
            ret += orientedStiffnessTensor.S44 * measurmentDirection[2] * measurmentDirection[1] * oDFValue;

            return ret;
        }

        private double stressFactor33IntegrantVoigt(Stress.Macroskopic.PeakStressAssociation usedAssociation, Stress.Microsopic.ElasticityTensors orientedStiffnessTensor, double oDFValue)
        {
            double ret = 0.0;
            MathNet.Numerics.LinearAlgebra.Vector<double> measurmentDirection = usedAssociation.MeasurementDirektionVektor;

            ret += orientedStiffnessTensor.S13 * measurmentDirection[0] * measurmentDirection[0] * oDFValue;
            ret += orientedStiffnessTensor.S23 * measurmentDirection[1] * measurmentDirection[1] * oDFValue;
            ret += orientedStiffnessTensor.S33 * measurmentDirection[2] * measurmentDirection[2] * oDFValue;

            return ret;
        }

        #endregion

        #region Reuss

        public MathNet.Numerics.LinearAlgebra.Matrix<double> GetStressFactorMatrixReuss(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            MathNet.Numerics.LinearAlgebra.Matrix<double> ret = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
            double normFactor = 0;

            List<double[]> IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

            for (int n = 0; n < IntegrationAngles.Count; n++)
            {
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorCubic(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorCubic(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                ret[0, 0] += stressFactor11IntegrantReuss(usedAssociation, orientedTensor, ODFValue);
                ret[0, 1] += stressFactor12IntegrantReuss(usedAssociation, orientedTensor, ODFValue);
                ret[0, 2] += stressFactor13IntegrantReuss(usedAssociation, orientedTensor, ODFValue);
                ret[1, 1] += stressFactor22IntegrantReuss(usedAssociation, orientedTensor, ODFValue);
                ret[1, 2] += stressFactor23IntegrantReuss(usedAssociation, orientedTensor, ODFValue);
                ret[2, 2] += stressFactor33IntegrantReuss(usedAssociation, orientedTensor, ODFValue);

                normFactor += ODFValue;
            }

            ret[1, 0] = ret[0, 1];
            ret[2, 0] = ret[0, 2];
            ret[2, 1] = ret[1, 2];

            if (normFactor == 0)
            {
                return ret;
            }
            else
            {
                return ret / normFactor;
            }
        }

        private double GetStressFactor11ReussCubic(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            double ret = 0;
            double normFactor = 0;

            List<double[]> IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

            for (int n = 0; n < IntegrationAngles.Count; n++)
            {
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorCubic(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorCubic(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                ret += stressFactor11IntegrantReuss(usedAssociation, orientedTensor, ODFValue);

                normFactor += ODFValue;
            }

            return ret / normFactor;
        }

        private double GetStressFactor12ReussCubic(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            double ret = 0;
            double normFactor = 0;

            List<double[]> IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

            for (int n = 0; n < IntegrationAngles.Count; n++)
            {
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorCubic(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorCubic(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                ret += stressFactor12IntegrantReuss(usedAssociation, orientedTensor, ODFValue);

                normFactor += ODFValue;
            }

            return ret / normFactor;
        }

        private double GetStressFactor13ReussCubic(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            double ret = 0;
            double normFactor = 0;

            List<double[]> IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

            for (int n = 0; n < IntegrationAngles.Count; n++)
            {
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorCubic(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorCubic(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                ret += stressFactor13IntegrantReuss(usedAssociation, orientedTensor, ODFValue);

                normFactor += ODFValue;
            }

            return ret / normFactor;
        }

        private double GetStressFactor22ReussCubic(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            double ret = 0;
            double normFactor = 0;

            List<double[]> IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

            for (int n = 0; n < IntegrationAngles.Count; n++)
            {
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorCubic(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorCubic(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                ret += stressFactor22IntegrantReuss(usedAssociation, orientedTensor, ODFValue);

                normFactor += ODFValue;
            }

            return ret / normFactor;
        }

        private double GetStressFactor23ReussCubic(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            double ret = 0;
            double normFactor = 0;

            List<double[]> IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

            for (int n = 0; n < IntegrationAngles.Count; n++)
            {
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorCubic(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorCubic(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                ret += stressFactor23IntegrantReuss(usedAssociation, orientedTensor, ODFValue);

                normFactor += ODFValue;
            }

            return ret / normFactor;
        }

        private double GetStressFactor33ReussCubic(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            double ret = 0;
            double normFactor = 0;

            List<double[]> IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

            for (int n = 0; n < IntegrationAngles.Count; n++)
            {
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorCubic(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorCubic(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                ret += stressFactor33IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin((IntegrationAngles[n][1] * Math.PI) / 180.0);

                normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
            }

            //normFactor *= 8.0 * Math.Pow(Math.PI, 2);


            if (normFactor == 0)
            {
                return 0;
            }
            else
            {
                return ret / normFactor;
            }
        }

        private double stressFactor11IntegrantReuss(Stress.Macroskopic.PeakStressAssociation usedAssociation, Stress.Microsopic.ElasticityTensors orientedStiffnessTensor, double oDFValue)
        {
            double ret = 0.0;
            MathNet.Numerics.LinearAlgebra.Vector<double> measurmentDirection = usedAssociation.MeasurementDirektionVektor;

            ret += orientedStiffnessTensor.S11 * oDFValue;
            ret += orientedStiffnessTensor.S21 * oDFValue;
            ret += orientedStiffnessTensor.S31 * oDFValue;

            return ret;
        }

        private double stressFactor12IntegrantReuss(Stress.Macroskopic.PeakStressAssociation usedAssociation, Stress.Microsopic.ElasticityTensors orientedStiffnessTensor, double oDFValue)
        {
            double ret = 0.0;
            MathNet.Numerics.LinearAlgebra.Vector<double> measurmentDirection = usedAssociation.MeasurementDirektionVektor;

            ret += orientedStiffnessTensor.S66 * oDFValue;
            ret += orientedStiffnessTensor.S66 * oDFValue;

            return ret;
        }

        private double stressFactor13IntegrantReuss(Stress.Macroskopic.PeakStressAssociation usedAssociation, Stress.Microsopic.ElasticityTensors orientedStiffnessTensor, double oDFValue)
        {
            double ret = 0.0;
            MathNet.Numerics.LinearAlgebra.Vector<double> measurmentDirection = usedAssociation.MeasurementDirektionVektor;

            ret += orientedStiffnessTensor.S55 * oDFValue;
            ret += orientedStiffnessTensor.S55 * oDFValue;

            return ret;
        }

        private double stressFactor22IntegrantReuss(Stress.Macroskopic.PeakStressAssociation usedAssociation, Stress.Microsopic.ElasticityTensors orientedStiffnessTensor, double oDFValue)
        {
            double ret = 0.0;

            ret += orientedStiffnessTensor.S12 * oDFValue;
            ret += orientedStiffnessTensor.S22 * oDFValue;
            ret += orientedStiffnessTensor.S23 * oDFValue;

            return ret;
        }

        private double stressFactor23IntegrantReuss(Stress.Macroskopic.PeakStressAssociation usedAssociation, Stress.Microsopic.ElasticityTensors orientedStiffnessTensor, double oDFValue)
        {
            double ret = 0.0;

            ret += orientedStiffnessTensor.S44 * oDFValue;
            ret += orientedStiffnessTensor.S44 * oDFValue;

            return ret;
        }

        private double stressFactor33IntegrantReuss(Stress.Macroskopic.PeakStressAssociation usedAssociation, Stress.Microsopic.ElasticityTensors orientedStiffnessTensor, double oDFValue)
        {
            double ret = 0.0;
            MathNet.Numerics.LinearAlgebra.Vector<double> measurmentDirection = usedAssociation.MeasurementDirektionVektor;

            ret += orientedStiffnessTensor.S13 * oDFValue;
            ret += orientedStiffnessTensor.S23 * oDFValue;
            ret += orientedStiffnessTensor.S33 * oDFValue;

            return ret;
        }

        #endregion

        #region Hill

        public MathNet.Numerics.LinearAlgebra.Matrix<double> GetStressFactorMatrixHill(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            MathNet.Numerics.LinearAlgebra.Matrix<double> ret = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
            double normFactor = 0;

            List<double[]> IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

            for (int n = 0; n < IntegrationAngles.Count; n++)
            {
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorCubic(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorCubic(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorCubic(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorCubic(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                orientedTensor.S11 = (orientedCTensor.S11 + orientedSTensor.S11) / 2.0;
                orientedTensor.S12 = (orientedCTensor.S12 + orientedSTensor.S11) / 2.0;
                orientedTensor.S44 = (orientedCTensor.S44 + orientedSTensor.S11) / 2.0;

                double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                ret[0, 0] += stressFactor11IntegrantVoigt(usedAssociation, orientedTensor, ODFValue);
                ret[0, 1] += stressFactor12IntegrantVoigt(usedAssociation, orientedTensor, ODFValue);
                ret[0, 2] += stressFactor13IntegrantVoigt(usedAssociation, orientedTensor, ODFValue);
                ret[1, 1] += stressFactor22IntegrantVoigt(usedAssociation, orientedTensor, ODFValue);
                ret[1, 2] += stressFactor23IntegrantVoigt(usedAssociation, orientedTensor, ODFValue);
                ret[2, 2] += stressFactor33IntegrantVoigt(usedAssociation, orientedTensor, ODFValue);

                normFactor += ODFValue;
            }

            ret[1, 0] = ret[0, 1];
            ret[2, 0] = ret[0, 2];
            ret[2, 1] = ret[1, 2];

            return ret / normFactor;
        }

        private double GetStressFactor11HillCubic(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            double ret = 0;
            double normFactor = 0;

            List<double[]> IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

            for (int n = 0; n < IntegrationAngles.Count; n++)
            {
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorCubic(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorCubic(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorCubic(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorCubic(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                orientedTensor.S11 = (orientedCTensor.S11 + orientedSTensor.S11) / 2.0;
                orientedTensor.S12 = (orientedCTensor.S12 + orientedSTensor.S11) / 2.0;
                orientedTensor.S44 = (orientedCTensor.S44 + orientedSTensor.S11) / 2.0;

                ret += stressFactor11IntegrantVoigt(usedAssociation, orientedTensor, ODFValue);

                normFactor += ODFValue;
            }

            return ret / normFactor;
        }

        private double GetStressFactor12HillCubic(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            double ret = 0;
            double normFactor = 0;

            List<double[]> IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

            for (int n = 0; n < IntegrationAngles.Count; n++)
            {
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorCubic(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorCubic(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorCubic(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorCubic(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                orientedTensor.S11 = (orientedCTensor.S11 + orientedSTensor.S11) / 2.0;
                orientedTensor.S12 = (orientedCTensor.S12 + orientedSTensor.S11) / 2.0;
                orientedTensor.S44 = (orientedCTensor.S44 + orientedSTensor.S11) / 2.0;

                ret += stressFactor12IntegrantVoigt(usedAssociation, orientedTensor, ODFValue);

                normFactor += ODFValue;
            }

            return ret / normFactor;
        }

        private double GetStressFactor13HillCubic(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            double ret = 0;
            double normFactor = 0;

            List<double[]> IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

            for (int n = 0; n < IntegrationAngles.Count; n++)
            {
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorCubic(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorCubic(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorCubic(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorCubic(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                orientedTensor.S11 = (orientedCTensor.S11 + orientedSTensor.S11) / 2.0;
                orientedTensor.S12 = (orientedCTensor.S12 + orientedSTensor.S11) / 2.0;
                orientedTensor.S44 = (orientedCTensor.S44 + orientedSTensor.S11) / 2.0;

                ret += stressFactor13IntegrantVoigt(usedAssociation, orientedTensor, ODFValue);

                normFactor += ODFValue;
            }

            return ret / normFactor;
        }

        private double GetStressFactor22HillCubic(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            double ret = 0;
            double normFactor = 0;

            List<double[]> IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

            for (int n = 0; n < IntegrationAngles.Count; n++)
            {
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorCubic(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorCubic(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorCubic(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorCubic(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                orientedTensor.S11 = (orientedCTensor.S11 + orientedSTensor.S11) / 2.0;
                orientedTensor.S12 = (orientedCTensor.S12 + orientedSTensor.S11) / 2.0;
                orientedTensor.S44 = (orientedCTensor.S44 + orientedSTensor.S11) / 2.0;

                ret += stressFactor22IntegrantVoigt(usedAssociation, orientedTensor, ODFValue);

                normFactor += ODFValue;
            }

            return ret / normFactor;
        }

        private double GetStressFactor23HillCubic(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            double ret = 0;
            double normFactor = 0;

            List<double[]> IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

            for (int n = 0; n < IntegrationAngles.Count; n++)
            {
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorCubic(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorCubic(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorCubic(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorCubic(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                orientedTensor.S11 = (orientedCTensor.S11 + orientedSTensor.S11) / 2.0;
                orientedTensor.S12 = (orientedCTensor.S12 + orientedSTensor.S11) / 2.0;
                orientedTensor.S44 = (orientedCTensor.S44 + orientedSTensor.S11) / 2.0;

                ret += stressFactor23IntegrantVoigt(usedAssociation, orientedTensor, ODFValue);

                normFactor += ODFValue;
            }

            return ret / normFactor;
        }

        private double GetStressFactor33HillCubic(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            double ret = 0;
            double normFactor = 0;

            List<double[]> IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

            for (int n = 0; n < IntegrationAngles.Count; n++)
            {
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorCubic(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorCubic(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorCubic(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorCubic(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                orientedTensor.S11 = (orientedCTensor.S11 + orientedSTensor.S11) / 2.0;
                orientedTensor.S12 = (orientedCTensor.S12 + orientedSTensor.S11) / 2.0;
                orientedTensor.S44 = (orientedCTensor.S44 + orientedSTensor.S11) / 2.0;

                ret += stressFactor33IntegrantVoigt(usedAssociation, orientedTensor, ODFValue);

                normFactor += ODFValue;
            }

            return ret / normFactor;
        }

        #endregion

        #region First Derivatives
        
        #region voigt

        /// <summary>
        /// 
        /// </summary>
        /// <param name="usedAssociation"></param>
        /// <param name="Parameter">[0:C11][1:C12][2:C44]</param>
        /// <returns></returns>
        public MathNet.Numerics.LinearAlgebra.Matrix<double> GetStressFactorMatrixVoigtCubicFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            MathNet.Numerics.LinearAlgebra.Matrix<double> ret = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

            double normFactor = 0;
            List<double[]> IntegrationAngles = new List<double[]>();
            switch (parameter)
            {
                case 0:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorCubicFDS11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret[0, 0] += stressFactor11IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 1] += stressFactor12IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 2] += stressFactor13IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 1] += stressFactor22IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 2] += stressFactor23IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[2, 2] += stressFactor33IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    ret[1, 0] = ret[0, 1];
                    ret[2, 0] = ret[0, 2];
                    ret[2, 1] = ret[1, 2];

                    if (normFactor == 0)
                    {
                        return ret;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 1:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorCubicFDS12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret[0, 0] += stressFactor11IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 1] += stressFactor12IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 2] += stressFactor13IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 1] += stressFactor22IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 2] += stressFactor23IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[2, 2] += stressFactor33IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    ret[1, 0] = ret[0, 1];
                    ret[2, 0] = ret[0, 2];
                    ret[2, 1] = ret[1, 2];

                    if (normFactor == 0)
                    {
                        return ret;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 2:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorCubicFDS44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret[0, 0] += stressFactor11IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 1] += stressFactor12IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 2] += stressFactor13IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 1] += stressFactor22IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 2] += stressFactor23IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[2, 2] += stressFactor33IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    ret[1, 0] = ret[0, 1];
                    ret[2, 0] = ret[0, 2];
                    ret[2, 1] = ret[1, 2];

                    if (normFactor == 0)
                    {
                        return ret;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                default:
                    return ret;
            }
        }

        private double GetStressFactor11VoigtCubicFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            double ret = 0;
            double normFactor = 0;
            List<double[]> IntegrationAngles = new List<double[]>();

            switch (parameter)
            {
                case 0:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorCubicFDS11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor11IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0)));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 1:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorCubicFDS12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor11IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 2:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorCubicFDS44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor11IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                default:
                    return ret;
            }
        }

        private double GetStressFactor12VoigtCubicFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            double ret = 0;
            double normFactor = 0;
            List<double[]> IntegrationAngles = new List<double[]>();

            switch (parameter)
            {
                case 0:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorCubicFDS11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor12IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 1:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorCubicFDS12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor12IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 2:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorCubicFDS44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor12IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                default:
                    return ret;
            }
        }

        private double GetStressFactor13VoigtCubicFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            double ret = 0;
            double normFactor = 0;
            List<double[]> IntegrationAngles = new List<double[]>();

            switch (parameter)
            {
                case 0:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorCubicFDS11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor13IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 1:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorCubicFDS12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor13IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 2:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorCubicFDS44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor13IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                default:
                    return ret;
            }
        }

        private double GetStressFactor22VoigtCubicFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            double ret = 0;
            double normFactor = 0;
            List<double[]> IntegrationAngles = new List<double[]>();

            switch (parameter)
            {
                case 0:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorCubicFDS11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor22IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 1:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorCubicFDS12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor22IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 2:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorCubicFDS44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor22IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                default:
                    return ret;
            }
        }

        private double GetStressFactor23VoigtCubicFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            double ret = 0;
            double normFactor = 0;
            List<double[]> IntegrationAngles = new List<double[]>();

            switch (parameter)
            {
                case 0:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorCubicFDS11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor23IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 1:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorCubicFDS12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor23IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 2:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorCubicFDS44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor23IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                default:
                    return ret;
            }
        }

        private double GetStressFactor33VoigtCubicFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            double ret = 0;
            double normFactor = 0;
            List<double[]> IntegrationAngles = new List<double[]>();

            switch (parameter)
            {
                case 0:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorCubicFDS11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];
                        if (!double.IsNaN(orientedStiffnessTensor.S11) && !double.IsNaN(orientedStiffnessTensor.S12) && !double.IsNaN(orientedStiffnessTensor.S44))
                        {
                            ret += stressFactor33IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                            normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        }
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 1:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorCubicFDS12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];
                        if (!double.IsNaN(orientedStiffnessTensor.S11) && !double.IsNaN(orientedStiffnessTensor.S12) && !double.IsNaN(orientedStiffnessTensor.S44))
                        {
                            ret += stressFactor33IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                            normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        }
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 2:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorCubicFDS44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];
                        if (!double.IsNaN(orientedStiffnessTensor.S11) && !double.IsNaN(orientedStiffnessTensor.S12) && !double.IsNaN(orientedStiffnessTensor.S44))
                        {
                            ret += stressFactor33IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                            normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        }
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                default:
                    return ret;
            }
        }

        #endregion

        #region Reuss

        /// <summary>
        /// 
        /// </summary>
        /// <param name="usedAssociation"></param>
        /// <param name="Parameter">[0:C11][1:C12][2:C44]</param>
        /// <returns></returns>
        public MathNet.Numerics.LinearAlgebra.Matrix<double> GetStressFactorMatrixReussCubicFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            MathNet.Numerics.LinearAlgebra.Matrix<double> ret = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
            double normFactor = 0;
            List<double[]> IntegrationAngles = new List<double[]>();

            switch (parameter)
            {
                case 0:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorCubicFDS11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret[0, 0] += stressFactor11IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 1] += stressFactor12IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 2] += stressFactor13IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 1] += stressFactor22IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 2] += stressFactor23IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[2, 2] += stressFactor33IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    ret[1, 0] = ret[0, 1];
                    ret[2, 0] = ret[0, 2];
                    ret[2, 1] = ret[1, 2];

                    if (normFactor == 0)
                    {
                        return ret;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 1:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorCubicFDS12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret[0, 0] += stressFactor11IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 1] += stressFactor12IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 2] += stressFactor13IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 1] += stressFactor22IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 2] += stressFactor23IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[2, 2] += stressFactor33IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    ret[1, 0] = ret[0, 1];
                    ret[2, 0] = ret[0, 2];
                    ret[2, 1] = ret[1, 2];

                    if (normFactor == 0)
                    {
                        return ret;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 2:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorCubicFDS44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret[0, 0] += stressFactor11IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 1] += stressFactor12IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 2] += stressFactor13IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 1] += stressFactor22IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 2] += stressFactor23IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[2, 2] += stressFactor33IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    ret[1, 0] = ret[0, 1];
                    ret[2, 0] = ret[0, 2];
                    ret[2, 1] = ret[1, 2];

                    if (normFactor == 0)
                    {
                        return ret;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                default:
                    return ret;
            }
        }

        private double GetStressFactor11ReussCubicFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            double ret = 0;
            double normFactor = 0;
            List<double[]> IntegrationAngles = new List<double[]>();

            switch (parameter)
            {
                case 0:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorCubicFDS11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor11IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if(normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 1:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorCubicFDS12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor11IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 2:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorCubicFDS44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor11IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                default:
                    return ret;
            }
        }

        private double GetStressFactor12ReussCubicFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            double ret = 0;
            double normFactor = 0;
            List<double[]> IntegrationAngles = new List<double[]>();

            switch (parameter)
            {
                case 0:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorCubicFDS11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor12IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 1:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorCubicFDS12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor12IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 2:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorCubicFDS44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor12IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                default:
                    return ret;
            }
        }

        private double GetStressFactor13ReussCubicFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            double ret = 0;
            double normFactor = 0;
            List<double[]> IntegrationAngles = new List<double[]>();

            switch (parameter)
            {
                case 0:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorCubicFDS11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor13IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 1:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorCubicFDS12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor13IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 2:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorCubicFDS44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor13IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                default:
                    return ret;
            }
        }

        private double GetStressFactor22ReussCubicFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            double ret = 0;
            double normFactor = 0;
            List<double[]> IntegrationAngles = new List<double[]>();

            switch (parameter)
            {
                case 0:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorCubicFDS11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor22IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 1:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorCubicFDS12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor22IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 2:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorCubicFDS44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor22IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                default:
                    return ret;
            }
        }

        private double GetStressFactor23ReussCubicFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            double ret = 0;
            double normFactor = 0;
            List<double[]> IntegrationAngles = new List<double[]>();

            switch (parameter)
            {
                case 0:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorCubicFDS11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor23IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 1:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorCubicFDS12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor23IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 2:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorCubicFDS44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor23IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                default:
                    return ret;
            }
        }

        private double GetStressFactor33ReussCubicFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            double ret = 0;
            double normFactor = 0;
            List<double[]> IntegrationAngles = new List<double[]>();

            switch (parameter)
            {
                case 0:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorCubicFDS11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor33IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }
                    //normFactor *= 8.0 * Math.Pow(Math.PI, 2);
                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 1:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorCubicFDS12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor33IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }
                    //normFactor *= 8.0 * Math.Pow(Math.PI, 2);


                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 2:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorCubicFDS44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor33IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }
                    //normFactor *= 8.0 * Math.Pow(Math.PI, 2);

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                default:
                    return ret;
            }
        }

        #endregion

        #region Hill

        /// <summary>
        /// 
        /// </summary>
        /// <param name="usedAssociation"></param>
        /// <param name="Parameter">[0:C11][1:C12][2:C44]</param>
        /// <returns></returns>
        public MathNet.Numerics.LinearAlgebra.Matrix<double> GetStressFactorMatrixHillCubicFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            MathNet.Numerics.LinearAlgebra.Matrix<double> ret = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
            double normFactor = 0;
            List<double[]> IntegrationAngles = new List<double[]>();

            switch (parameter)
            {
                case 0:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorCubicFDS11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorCubicFDC11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret[0, 0] += stressFactor11IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 1] += stressFactor12IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 2] += stressFactor13IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 1] += stressFactor22IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 2] += stressFactor23IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[2, 2] += stressFactor33IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    ret[1, 0] = ret[0, 1];
                    ret[2, 0] = ret[0, 2];
                    ret[2, 1] = ret[1, 2];

                    if (normFactor == 0)
                    {
                        return ret;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 1:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorCubicFDS12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorCubicFDC12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret[0, 0] += stressFactor11IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 1] += stressFactor12IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 2] += stressFactor13IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 1] += stressFactor22IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 2] += stressFactor23IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[2, 2] += stressFactor33IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    ret[1, 0] = ret[0, 1];
                    ret[2, 0] = ret[0, 2];
                    ret[2, 1] = ret[1, 2];

                    if (normFactor == 0)
                    {
                        return ret;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 2:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorCubicFDS44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorCubicFDC44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret[0, 0] += stressFactor11IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 1] += stressFactor12IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 2] += stressFactor13IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 1] += stressFactor22IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 2] += stressFactor23IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[2, 2] += stressFactor33IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    ret[1, 0] = ret[0, 1];
                    ret[2, 0] = ret[0, 2];
                    ret[2, 1] = ret[1, 2];

                    if (normFactor == 0)
                    {
                        return ret;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                default:
                    return ret;
            }
        }

        private double GetStressFactor11HillCubicFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            double ret = 0;
            double normFactor = 0;
            List<double[]> IntegrationAngles = new List<double[]>();

            switch (parameter)
            {
                case 0:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorCubicFDS11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorCubicFDC11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor11IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 1:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorCubicFDS12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorCubicFDC12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor11IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 2:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorCubicFDS44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorCubicFDC44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor11IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                default:
                    return ret;
            }
        }

        private double GetStressFactor12HillCubicFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            double ret = 0;
            double normFactor = 0;
            List<double[]> IntegrationAngles = new List<double[]>();

            switch (parameter)
            {
                case 0:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorCubicFDS11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorCubicFDC11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor12IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 1:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorCubicFDS12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorCubicFDC12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor12IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 2:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorCubicFDS44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorCubicFDC44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor12IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                default:
                    return ret;
            }
        }

        private double GetStressFactor13HillCubicFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            double ret = 0;
            double normFactor = 0;
            List<double[]> IntegrationAngles = new List<double[]>();

            switch (parameter)
            {
                case 0:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorCubicFDS11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorCubicFDC11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor13IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 1:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorCubicFDS12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorCubicFDC12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor13IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 2:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorCubicFDS44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorCubicFDC44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor13IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                default:
                    return ret;
            }
        }

        private double GetStressFactor22HillCubicFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            double ret = 0;
            double normFactor = 0;
            List<double[]> IntegrationAngles = new List<double[]>();

            switch (parameter)
            {
                case 0:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorCubicFDS11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorCubicFDC11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor22IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 1:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorCubicFDS12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorCubicFDC12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor22IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 2:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorCubicFDS44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorCubicFDC44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor22IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                default:
                    return ret;
            }
        }

        private double GetStressFactor23HillCubicFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            double ret = 0;
            double normFactor = 0;
            List<double[]> IntegrationAngles = new List<double[]>();

            switch (parameter)
            {
                case 0:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorCubicFDS11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorCubicFDC11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor23IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 1:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorCubicFDS12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorCubicFDC12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor23IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 2:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorCubicFDS44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorCubicFDC44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor23IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                default:
                    return ret;
            }
        }

        private double GetStressFactor33HillCubicFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            double ret = 0;
            double normFactor = 0;
            List<double[]> IntegrationAngles = new List<double[]>();

            switch (parameter)
            {
                case 0:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorCubicFDS11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorCubicFDC11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor33IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 1:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorCubicFDS12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorCubicFDC12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor33IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 2:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorCubicFDS44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorCubicFDC44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor33IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                default:
                    return ret;
            }
        }

        #endregion

        #endregion

        #endregion

        #region Hexagonal

        #region Voigt

        public MathNet.Numerics.LinearAlgebra.Matrix<double> GetStressFactorMatrixVoigtHexagonal(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            MathNet.Numerics.LinearAlgebra.Matrix<double> ret = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
            double normFactor = 0;

            List<double[]> IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

            for (int n = 0; n < IntegrationAngles.Count; n++)
            {
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedStiffnessTensorCubic(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorHexagonal(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                ret[0, 0] += stressFactor11IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                ret[0, 1] += stressFactor12IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                ret[0, 2] += stressFactor13IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                ret[1, 1] += stressFactor22IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                ret[1, 2] += stressFactor23IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                ret[2, 2] += stressFactor33IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
            }

            ret[1, 0] = ret[0, 1];
            ret[2, 0] = ret[0, 2];
            ret[2, 1] = ret[1, 2];

            if (normFactor == 0)
            {
                return ret;
            }
            else
            {
                return ret / normFactor;
            }
        }

        private double GetStressFactor11VoigtHexagonal(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            double ret = 0;
            double normFactor = 0;

            List<double[]> IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

            for (int n = 0; n < IntegrationAngles.Count; n++)
            {
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedStiffnessTensorHexagonal(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorHexagonal(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                ret += stressFactor11IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
            }

            if (normFactor == 0)
            {
                return 0;
            }
            else
            {
                return ret / normFactor;
            }
        }

        private double GetStressFactor12VoigtHexagonal(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            double ret = 0;
            double normFactor = 0;

            List<double[]> IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

            for (int n = 0; n < IntegrationAngles.Count; n++)
            {
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedStiffnessTensorHexagonal(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorHexagonal(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                ret += stressFactor12IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
            }

            if (normFactor == 0)
            {
                return 0;
            }
            else
            {
                return ret / normFactor;
            }
        }

        private double GetStressFactor13VoigtHexagonal(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            double ret = 0;
            double normFactor = 0;

            List<double[]> IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

            for (int n = 0; n < IntegrationAngles.Count; n++)
            {
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedStiffnessTensorHexagonal(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorHexagonal(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                ret += stressFactor13IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
            }

            if (normFactor == 0)
            {
                return 0;
            }
            else
            {
                return ret / normFactor;
            }
        }

        private double GetStressFactor22VoigtHexagonal(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            double ret = 0;
            double normFactor = 0;

            List<double[]> IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

            for (int n = 0; n < IntegrationAngles.Count; n++)
            {
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedStiffnessTensorHexagonal(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorHexagonal(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                ret += stressFactor22IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
            }

            if (normFactor == 0)
            {
                return 0;
            }
            else
            {
                return ret / normFactor;
            }
        }

        private double GetStressFactor23VoigtHexagonal(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            double ret = 0;
            double normFactor = 0;

            List<double[]> IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

            for (int n = 0; n < IntegrationAngles.Count; n++)
            {
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedStiffnessTensorHexagonal(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorHexagonal(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                ret += stressFactor23IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
            }

            if (normFactor == 0)
            {
                return 0;
            }
            else
            {
                return ret / normFactor;
            }
        }

        private double GetStressFactor33VoigtHexagonal(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            double ret = 0;
            double normFactor = 0;

            List<double[]> IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

            for (int n = 0; n < IntegrationAngles.Count; n++)
            {
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedStiffnessTensorHexagonal(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorHexagonal(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                if (!double.IsNaN(orientedStiffnessTensor.S11) && !double.IsNaN(orientedStiffnessTensor.S12) && !double.IsNaN(orientedStiffnessTensor.S44))
                {
                    double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                    ret += stressFactor33IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                    normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                }
            }

            if (normFactor == 0)
            {
                return 0;
            }
            else
            {
                return ret / normFactor;
            }
        }

        #endregion

        #region Reuss

        public MathNet.Numerics.LinearAlgebra.Matrix<double> GetStressFactorMatrixReussHexagonal(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            MathNet.Numerics.LinearAlgebra.Matrix<double> ret = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
            double normFactor = 0;

            List<double[]> IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

            for (int n = 0; n < IntegrationAngles.Count; n++)
            {
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonal(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonal(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                ret[0, 0] += stressFactor11IntegrantReuss(usedAssociation, orientedTensor, ODFValue);
                ret[0, 1] += stressFactor12IntegrantReuss(usedAssociation, orientedTensor, ODFValue);
                ret[0, 2] += stressFactor13IntegrantReuss(usedAssociation, orientedTensor, ODFValue);
                ret[1, 1] += stressFactor22IntegrantReuss(usedAssociation, orientedTensor, ODFValue);
                ret[1, 2] += stressFactor23IntegrantReuss(usedAssociation, orientedTensor, ODFValue);
                ret[2, 2] += stressFactor33IntegrantReuss(usedAssociation, orientedTensor, ODFValue);

                normFactor += ODFValue;
            }

            ret[1, 0] = ret[0, 1];
            ret[2, 0] = ret[0, 2];
            ret[2, 1] = ret[1, 2];

            if (normFactor == 0)
            {
                return ret;
            }
            else
            {
                return ret / normFactor;
            }
        }

        private double GetStressFactor11ReussHexagonal(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            double ret = 0;
            double normFactor = 0;

            List<double[]> IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

            for (int n = 0; n < IntegrationAngles.Count; n++)
            {
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonal(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonal(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                ret += stressFactor11IntegrantReuss(usedAssociation, orientedTensor, ODFValue);

                normFactor += ODFValue;
            }

            return ret / normFactor;
        }

        private double GetStressFactor12ReussHexagonal(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            double ret = 0;
            double normFactor = 0;

            List<double[]> IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

            for (int n = 0; n < IntegrationAngles.Count; n++)
            {
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonal(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonal(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                ret += stressFactor12IntegrantReuss(usedAssociation, orientedTensor, ODFValue);

                normFactor += ODFValue;
            }

            return ret / normFactor;
        }

        private double GetStressFactor13ReussHexagonal(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            double ret = 0;
            double normFactor = 0;

            List<double[]> IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

            for (int n = 0; n < IntegrationAngles.Count; n++)
            {
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonal(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonal(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                ret += stressFactor13IntegrantReuss(usedAssociation, orientedTensor, ODFValue);

                normFactor += ODFValue;
            }

            return ret / normFactor;
        }

        private double GetStressFactor22ReussHexagonal(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            double ret = 0;
            double normFactor = 0;

            List<double[]> IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

            for (int n = 0; n < IntegrationAngles.Count; n++)
            {
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonal(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonal(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                ret += stressFactor22IntegrantReuss(usedAssociation, orientedTensor, ODFValue);

                normFactor += ODFValue;
            }

            return ret / normFactor;
        }

        private double GetStressFactor23ReussHexagonal(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            double ret = 0;
            double normFactor = 0;

            List<double[]> IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

            for (int n = 0; n < IntegrationAngles.Count; n++)
            {
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonal(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonal(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                ret += stressFactor23IntegrantReuss(usedAssociation, orientedTensor, ODFValue);

                normFactor += ODFValue;
            }

            return ret / normFactor;
        }

        private double GetStressFactor33ReussHexagonal(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            double ret = 0;
            double normFactor = 0;

            List<double[]> IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

            for (int n = 0; n < IntegrationAngles.Count; n++)
            {
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonal(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonal(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                ret += stressFactor33IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin((IntegrationAngles[n][1] * Math.PI) / 180.0);

                normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
            }

            //normFactor *= 8.0 * Math.Pow(Math.PI, 2);


            if (normFactor == 0)
            {
                return 0;
            }
            else
            {
                return ret / normFactor;
            }
        }

        #endregion

        #region Hill

        public MathNet.Numerics.LinearAlgebra.Matrix<double> GetStressFactorMatrixHillHexagonal(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            MathNet.Numerics.LinearAlgebra.Matrix<double> ret = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
            double normFactor = 0;

            List<double[]> IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

            for (int n = 0; n < IntegrationAngles.Count; n++)
            {
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonal(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonal(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonal(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonal(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                orientedTensor.S11 = (orientedCTensor.S11 + orientedSTensor.S11) / 2.0;
                orientedTensor.S12 = (orientedCTensor.S12 + orientedSTensor.S11) / 2.0;
                orientedTensor.S44 = (orientedCTensor.S44 + orientedSTensor.S11) / 2.0;

                double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                ret[0, 0] += stressFactor11IntegrantVoigt(usedAssociation, orientedTensor, ODFValue);
                ret[0, 1] += stressFactor12IntegrantVoigt(usedAssociation, orientedTensor, ODFValue);
                ret[0, 2] += stressFactor13IntegrantVoigt(usedAssociation, orientedTensor, ODFValue);
                ret[1, 1] += stressFactor22IntegrantVoigt(usedAssociation, orientedTensor, ODFValue);
                ret[1, 2] += stressFactor23IntegrantVoigt(usedAssociation, orientedTensor, ODFValue);
                ret[2, 2] += stressFactor33IntegrantVoigt(usedAssociation, orientedTensor, ODFValue);

                normFactor += ODFValue;
            }

            ret[1, 0] = ret[0, 1];
            ret[2, 0] = ret[0, 2];
            ret[2, 1] = ret[1, 2];

            return ret / normFactor;
        }

        private double GetStressFactor11HillHexagonal(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            double ret = 0;
            double normFactor = 0;

            List<double[]> IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

            for (int n = 0; n < IntegrationAngles.Count; n++)
            {
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonal(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonal(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonal(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonal(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                orientedTensor.S11 = (orientedCTensor.S11 + orientedSTensor.S11) / 2.0;
                orientedTensor.S12 = (orientedCTensor.S12 + orientedSTensor.S11) / 2.0;
                orientedTensor.S44 = (orientedCTensor.S44 + orientedSTensor.S11) / 2.0;

                ret += stressFactor11IntegrantVoigt(usedAssociation, orientedTensor, ODFValue);

                normFactor += ODFValue;
            }

            return ret / normFactor;
        }

        private double GetStressFactor12HillHexagonal(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            double ret = 0;
            double normFactor = 0;

            List<double[]> IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

            for (int n = 0; n < IntegrationAngles.Count; n++)
            {
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonal(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonal(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonal(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonal(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                orientedTensor.S11 = (orientedCTensor.S11 + orientedSTensor.S11) / 2.0;
                orientedTensor.S12 = (orientedCTensor.S12 + orientedSTensor.S11) / 2.0;
                orientedTensor.S44 = (orientedCTensor.S44 + orientedSTensor.S11) / 2.0;

                ret += stressFactor12IntegrantVoigt(usedAssociation, orientedTensor, ODFValue);

                normFactor += ODFValue;
            }

            return ret / normFactor;
        }

        private double GetStressFactor13HillHexagonal(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            double ret = 0;
            double normFactor = 0;

            List<double[]> IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

            for (int n = 0; n < IntegrationAngles.Count; n++)
            {
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonal(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonal(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonal(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonal(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                orientedTensor.S11 = (orientedCTensor.S11 + orientedSTensor.S11) / 2.0;
                orientedTensor.S12 = (orientedCTensor.S12 + orientedSTensor.S11) / 2.0;
                orientedTensor.S44 = (orientedCTensor.S44 + orientedSTensor.S11) / 2.0;

                ret += stressFactor13IntegrantVoigt(usedAssociation, orientedTensor, ODFValue);

                normFactor += ODFValue;
            }

            return ret / normFactor;
        }

        private double GetStressFactor22HillHexagonal(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            double ret = 0;
            double normFactor = 0;

            List<double[]> IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

            for (int n = 0; n < IntegrationAngles.Count; n++)
            {
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonal(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonal(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonal(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonal(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                orientedTensor.S11 = (orientedCTensor.S11 + orientedSTensor.S11) / 2.0;
                orientedTensor.S12 = (orientedCTensor.S12 + orientedSTensor.S11) / 2.0;
                orientedTensor.S44 = (orientedCTensor.S44 + orientedSTensor.S11) / 2.0;

                ret += stressFactor22IntegrantVoigt(usedAssociation, orientedTensor, ODFValue);

                normFactor += ODFValue;
            }

            return ret / normFactor;
        }

        private double GetStressFactor23HillHexagonal(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            double ret = 0;
            double normFactor = 0;

            List<double[]> IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

            for (int n = 0; n < IntegrationAngles.Count; n++)
            {
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonal(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonal(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonal(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonal(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                orientedTensor.S11 = (orientedCTensor.S11 + orientedSTensor.S11) / 2.0;
                orientedTensor.S12 = (orientedCTensor.S12 + orientedSTensor.S11) / 2.0;
                orientedTensor.S44 = (orientedCTensor.S44 + orientedSTensor.S11) / 2.0;

                ret += stressFactor23IntegrantVoigt(usedAssociation, orientedTensor, ODFValue);

                normFactor += ODFValue;
            }

            return ret / normFactor;
        }

        private double GetStressFactor33HillHexagonal(Stress.Macroskopic.PeakStressAssociation usedAssociation)
        {
            double ret = 0;
            double normFactor = 0;

            List<double[]> IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

            for (int n = 0; n < IntegrationAngles.Count; n++)
            {
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonal(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonal(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                //EigeneEntwicklung
                //Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonal(IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]);
                //Korrektur
                Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonal(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                orientedTensor.S11 = (orientedCTensor.S11 + orientedSTensor.S11) / 2.0;
                orientedTensor.S12 = (orientedCTensor.S12 + orientedSTensor.S11) / 2.0;
                orientedTensor.S44 = (orientedCTensor.S44 + orientedSTensor.S11) / 2.0;

                ret += stressFactor33IntegrantVoigt(usedAssociation, orientedTensor, ODFValue);

                normFactor += ODFValue;
            }

            return ret / normFactor;
        }

        #endregion

        #region First Derivatives

        #region voigt

        /// <summary>
        /// 
        /// </summary>
        /// <param name="usedAssociation"></param>
        /// <param name="Parameter">[0:C11][1:C12][2:C44]</param>
        /// <returns></returns>
        public MathNet.Numerics.LinearAlgebra.Matrix<double> GetStressFactorMatrixVoigtHexagonalFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            MathNet.Numerics.LinearAlgebra.Matrix<double> ret = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

            double normFactor = 0;
            List<double[]> IntegrationAngles = new List<double[]>();
            switch (parameter)
            {
                case 0:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorHexagonalFDS11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret[0, 0] += stressFactor11IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 1] += stressFactor12IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 2] += stressFactor13IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 1] += stressFactor22IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 2] += stressFactor23IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[2, 2] += stressFactor33IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    ret[1, 0] = ret[0, 1];
                    ret[2, 0] = ret[0, 2];
                    ret[2, 1] = ret[1, 2];

                    if (normFactor == 0)
                    {
                        return ret;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 1:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorHexagonalFDS12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret[0, 0] += stressFactor11IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 1] += stressFactor12IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 2] += stressFactor13IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 1] += stressFactor22IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 2] += stressFactor23IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[2, 2] += stressFactor33IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    ret[1, 0] = ret[0, 1];
                    ret[2, 0] = ret[0, 2];
                    ret[2, 1] = ret[1, 2];

                    if (normFactor == 0)
                    {
                        return ret;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 2:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorHexagonalFDS44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret[0, 0] += stressFactor11IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 1] += stressFactor12IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 2] += stressFactor13IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 1] += stressFactor22IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 2] += stressFactor23IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[2, 2] += stressFactor33IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    ret[1, 0] = ret[0, 1];
                    ret[2, 0] = ret[0, 2];
                    ret[2, 1] = ret[1, 2];

                    if (normFactor == 0)
                    {
                        return ret;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 3:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorHexagonalFDS13(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret[0, 0] += stressFactor11IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 1] += stressFactor12IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 2] += stressFactor13IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 1] += stressFactor22IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 2] += stressFactor23IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[2, 2] += stressFactor33IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    ret[1, 0] = ret[0, 1];
                    ret[2, 0] = ret[0, 2];
                    ret[2, 1] = ret[1, 2];

                    if (normFactor == 0)
                    {
                        return ret;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 4:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorHexagonalFDS33(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret[0, 0] += stressFactor11IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 1] += stressFactor12IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 2] += stressFactor13IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 1] += stressFactor22IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 2] += stressFactor23IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[2, 2] += stressFactor33IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    ret[1, 0] = ret[0, 1];
                    ret[2, 0] = ret[0, 2];
                    ret[2, 1] = ret[1, 2];

                    if (normFactor == 0)
                    {
                        return ret;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                default:
                    return ret;
            }
        }

        private double GetStressFactor11VoigtHexagonalFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            double ret = 0;
            double normFactor = 0;
            List<double[]> IntegrationAngles = new List<double[]>();

            switch (parameter)
            {
                case 0:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorHexagonalFDS11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor11IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0)));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 1:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorHexagonalFDS12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor11IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 2:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorHexagonalFDS44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor11IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 3:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorHexagonalFDS13(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor11IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 4:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorHexagonalFDS33(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor11IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                default:
                    return ret;
            }
        }

        private double GetStressFactor12VoigtHexagonalFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            double ret = 0;
            double normFactor = 0;
            List<double[]> IntegrationAngles = new List<double[]>();

            switch (parameter)
            {
                case 0:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorHexagonalFDS11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor12IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 1:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorHexagonalFDS12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor12IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 2:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorHexagonalFDS44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor12IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 3:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorHexagonalFDS13(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor12IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 4:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorHexagonalFDS33(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor12IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                default:
                    return ret;
            }
        }

        private double GetStressFactor13VoigtHexagonalFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            double ret = 0;
            double normFactor = 0;
            List<double[]> IntegrationAngles = new List<double[]>();

            switch (parameter)
            {
                case 0:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorHexagonalFDS11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor13IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 1:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorHexagonalFDS12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor13IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 2:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorHexagonalFDS44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor13IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 3:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorHexagonalFDS13(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor13IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 4:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorHexagonalFDS33(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor13IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                default:
                    return ret;
            }
        }

        private double GetStressFactor22VoigtHexagonalFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            double ret = 0;
            double normFactor = 0;
            List<double[]> IntegrationAngles = new List<double[]>();

            switch (parameter)
            {
                case 0:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorHexagonalFDS11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor22IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 1:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorHexagonalFDS12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor22IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 2:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorHexagonalFDS44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor22IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 3:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorHexagonalFDS13(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor22IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 4:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorHexagonalFDS33(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor22IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                default:
                    return ret;
            }
        }

        private double GetStressFactor23VoigtHexagonalFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            double ret = 0;
            double normFactor = 0;
            List<double[]> IntegrationAngles = new List<double[]>();

            switch (parameter)
            {
                case 0:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorHexagonalFDS11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor23IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 1:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorHexagonalFDS12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor23IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 2:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorHexagonalFDS44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor23IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 3:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorHexagonalFDS13(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor23IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 4:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorHexagonalFDS33(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor23IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                default:
                    return ret;
            }
        }

        private double GetStressFactor33VoigtHexagonalFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            double ret = 0;
            double normFactor = 0;
            List<double[]> IntegrationAngles = new List<double[]>();

            switch (parameter)
            {
                case 0:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorHexagonalFDS11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];
                        if (!double.IsNaN(orientedStiffnessTensor.S11) && !double.IsNaN(orientedStiffnessTensor.S12) && !double.IsNaN(orientedStiffnessTensor.S44))
                        {
                            ret += stressFactor33IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                            normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        }
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 1:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorHexagonalFDS12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];
                        if (!double.IsNaN(orientedStiffnessTensor.S11) && !double.IsNaN(orientedStiffnessTensor.S12) && !double.IsNaN(orientedStiffnessTensor.S44))
                        {
                            ret += stressFactor33IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                            normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        }
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 2:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorHexagonalFDS44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];
                        if (!double.IsNaN(orientedStiffnessTensor.S11) && !double.IsNaN(orientedStiffnessTensor.S12) && !double.IsNaN(orientedStiffnessTensor.S44))
                        {
                            ret += stressFactor33IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                            normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        }
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 3:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorHexagonalFDS13(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];
                        if (!double.IsNaN(orientedStiffnessTensor.S11) && !double.IsNaN(orientedStiffnessTensor.S12) && !double.IsNaN(orientedStiffnessTensor.S44))
                        {
                            ret += stressFactor33IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                            normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        }
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 4:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedStiffnessTensor = this.OrientedComplianceTensorHexagonalFDS33(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];
                        if (!double.IsNaN(orientedStiffnessTensor.S11) && !double.IsNaN(orientedStiffnessTensor.S12) && !double.IsNaN(orientedStiffnessTensor.S44))
                        {
                            ret += stressFactor33IntegrantVoigt(usedAssociation, orientedStiffnessTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                            normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        }
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                default:
                    return ret;
            }
        }

        #endregion

        #region Reuss

        /// <summary>
        /// 
        /// </summary>
        /// <param name="usedAssociation"></param>
        /// <param name="Parameter">[0:C11][1:C12][2:C44]</param>
        /// <returns></returns>
        public MathNet.Numerics.LinearAlgebra.Matrix<double> GetStressFactorMatrixReussHexagonalFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            MathNet.Numerics.LinearAlgebra.Matrix<double> ret = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
            double normFactor = 0;
            List<double[]> IntegrationAngles = new List<double[]>();

            switch (parameter)
            {
                case 0:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonalFDS11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret[0, 0] += stressFactor11IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 1] += stressFactor12IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 2] += stressFactor13IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 1] += stressFactor22IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 2] += stressFactor23IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[2, 2] += stressFactor33IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    ret[1, 0] = ret[0, 1];
                    ret[2, 0] = ret[0, 2];
                    ret[2, 1] = ret[1, 2];

                    if (normFactor == 0)
                    {
                        return ret;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 1:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonalFDS12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret[0, 0] += stressFactor11IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 1] += stressFactor12IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 2] += stressFactor13IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 1] += stressFactor22IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 2] += stressFactor23IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[2, 2] += stressFactor33IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    ret[1, 0] = ret[0, 1];
                    ret[2, 0] = ret[0, 2];
                    ret[2, 1] = ret[1, 2];

                    if (normFactor == 0)
                    {
                        return ret;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 2:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonalFDS44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret[0, 0] += stressFactor11IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 1] += stressFactor12IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 2] += stressFactor13IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 1] += stressFactor22IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 2] += stressFactor23IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[2, 2] += stressFactor33IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    ret[1, 0] = ret[0, 1];
                    ret[2, 0] = ret[0, 2];
                    ret[2, 1] = ret[1, 2];

                    if (normFactor == 0)
                    {
                        return ret;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 3:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonalFDS13(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret[0, 0] += stressFactor11IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 1] += stressFactor12IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 2] += stressFactor13IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 1] += stressFactor22IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 2] += stressFactor23IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[2, 2] += stressFactor33IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    ret[1, 0] = ret[0, 1];
                    ret[2, 0] = ret[0, 2];
                    ret[2, 1] = ret[1, 2];

                    if (normFactor == 0)
                    {
                        return ret;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 4:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonalFDS33(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret[0, 0] += stressFactor11IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 1] += stressFactor12IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 2] += stressFactor13IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 1] += stressFactor22IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 2] += stressFactor23IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[2, 2] += stressFactor33IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));

                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    ret[1, 0] = ret[0, 1];
                    ret[2, 0] = ret[0, 2];
                    ret[2, 1] = ret[1, 2];

                    if (normFactor == 0)
                    {
                        return ret;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                default:
                    return ret;
            }
        }

        private double GetStressFactor11ReussHexagonalFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            double ret = 0;
            double normFactor = 0;
            List<double[]> IntegrationAngles = new List<double[]>();

            switch (parameter)
            {
                case 0:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonalFDS11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor11IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 1:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonalFDS12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor11IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 2:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonalFDS44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor11IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 3:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonalFDS13(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor11IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 4:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonalFDS33(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor11IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                default:
                    return ret;
            }
        }

        private double GetStressFactor12ReussHexagonalFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            double ret = 0;
            double normFactor = 0;
            List<double[]> IntegrationAngles = new List<double[]>();

            switch (parameter)
            {
                case 0:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonalFDS11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor12IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 1:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonalFDS12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor12IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 2:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonalFDS44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor12IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 3:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonalFDS13(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor12IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 4:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonalFDS33(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor12IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                default:
                    return ret;
            }
        }

        private double GetStressFactor13ReussHexagonalFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            double ret = 0;
            double normFactor = 0;
            List<double[]> IntegrationAngles = new List<double[]>();

            switch (parameter)
            {
                case 0:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonalFDS11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor13IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 1:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonalFDS12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor13IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 2:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonalFDS44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor13IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 3:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonalFDS13(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor13IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 4:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonalFDS33(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor13IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                default:
                    return ret;
            }
        }

        private double GetStressFactor22ReussHexagonalFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            double ret = 0;
            double normFactor = 0;
            List<double[]> IntegrationAngles = new List<double[]>();

            switch (parameter)
            {
                case 0:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonalFDS11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor22IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 1:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonalFDS12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor22IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 2:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonalFDS44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor22IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 3:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonalFDS13(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor22IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 4:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonalFDS33(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor22IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                default:
                    return ret;
            }
        }

        private double GetStressFactor23ReussHexagonalFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            double ret = 0;
            double normFactor = 0;
            List<double[]> IntegrationAngles = new List<double[]>();

            switch (parameter)
            {
                case 0:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonalFDS11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor23IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 1:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonalFDS12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor23IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 2:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonalFDS44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor23IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 3:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonalFDS13(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor23IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 4:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonalFDS33(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor23IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                default:
                    return ret;
            }
        }

        private double GetStressFactor33ReussHexagonalFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            double ret = 0;
            double normFactor = 0;
            List<double[]> IntegrationAngles = new List<double[]>();

            switch (parameter)
            {
                case 0:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonalFDS11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor33IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }
                    //normFactor *= 8.0 * Math.Pow(Math.PI, 2);
                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 1:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonalFDS12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor33IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }
                    //normFactor *= 8.0 * Math.Pow(Math.PI, 2);


                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 2:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonalFDS44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor33IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }
                    //normFactor *= 8.0 * Math.Pow(Math.PI, 2);

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 3:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonalFDS13(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor33IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }
                    //normFactor *= 8.0 * Math.Pow(Math.PI, 2);

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 4:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedTensor = this.OrientedComplianceTensorHexagonalFDS33(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor33IntegrantReuss(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }
                    //normFactor *= 8.0 * Math.Pow(Math.PI, 2);

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                default:
                    return ret;
            }
        }

        #endregion

        #region Hill

        /// <summary>
        /// 
        /// </summary>
        /// <param name="usedAssociation"></param>
        /// <param name="Parameter">[0:C11][1:C12][2:C44]</param>
        /// <returns></returns>
        public MathNet.Numerics.LinearAlgebra.Matrix<double> GetStressFactorMatrixHillHexagonalFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            MathNet.Numerics.LinearAlgebra.Matrix<double> ret = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
            double normFactor = 0;
            List<double[]> IntegrationAngles = new List<double[]>();

            switch (parameter)
            {
                case 0:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonalFDS11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonalFDC11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret[0, 0] += stressFactor11IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 1] += stressFactor12IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 2] += stressFactor13IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 1] += stressFactor22IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 2] += stressFactor23IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[2, 2] += stressFactor33IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    ret[1, 0] = ret[0, 1];
                    ret[2, 0] = ret[0, 2];
                    ret[2, 1] = ret[1, 2];

                    if (normFactor == 0)
                    {
                        return ret;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 1:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonalFDS12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonalFDC12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret[0, 0] += stressFactor11IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 1] += stressFactor12IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 2] += stressFactor13IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 1] += stressFactor22IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 2] += stressFactor23IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[2, 2] += stressFactor33IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    ret[1, 0] = ret[0, 1];
                    ret[2, 0] = ret[0, 2];
                    ret[2, 1] = ret[1, 2];

                    if (normFactor == 0)
                    {
                        return ret;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 2:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonalFDS44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonalFDC44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret[0, 0] += stressFactor11IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 1] += stressFactor12IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 2] += stressFactor13IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 1] += stressFactor22IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 2] += stressFactor23IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[2, 2] += stressFactor33IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    ret[1, 0] = ret[0, 1];
                    ret[2, 0] = ret[0, 2];
                    ret[2, 1] = ret[1, 2];

                    if (normFactor == 0)
                    {
                        return ret;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 3:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonalFDS13(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonalFDC13(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret[0, 0] += stressFactor11IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 1] += stressFactor12IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 2] += stressFactor13IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 1] += stressFactor22IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 2] += stressFactor23IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[2, 2] += stressFactor33IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    ret[1, 0] = ret[0, 1];
                    ret[2, 0] = ret[0, 2];
                    ret[2, 1] = ret[1, 2];

                    if (normFactor == 0)
                    {
                        return ret;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 4:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonalFDS33(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonalFDC33(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret[0, 0] += stressFactor11IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 1] += stressFactor12IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[0, 2] += stressFactor13IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 1] += stressFactor22IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[1, 2] += stressFactor23IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        ret[2, 2] += stressFactor33IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    ret[1, 0] = ret[0, 1];
                    ret[2, 0] = ret[0, 2];
                    ret[2, 1] = ret[1, 2];

                    if (normFactor == 0)
                    {
                        return ret;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                default:
                    return ret;
            }
        }

        private double GetStressFactor11HillHexagonalFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            double ret = 0;
            double normFactor = 0;
            List<double[]> IntegrationAngles = new List<double[]>();

            switch (parameter)
            {
                case 0:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonalFDS11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonalFDC11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor11IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 1:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonalFDS12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonalFDC12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor11IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 2:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonalFDS44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonalFDC44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor11IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 3:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonalFDS13(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonalFDC13(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor11IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 4:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonalFDS33(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonalFDC33(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor11IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                default:
                    return ret;
            }
        }

        private double GetStressFactor12HillHexagonalFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            double ret = 0;
            double normFactor = 0;
            List<double[]> IntegrationAngles = new List<double[]>();

            switch (parameter)
            {
                case 0:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonalFDS11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonalFDC11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor12IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 1:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonalFDS12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonalFDC12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor12IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 2:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonalFDS44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonalFDC44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor12IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 3:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonalFDS13(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonalFDC13(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor12IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 4:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonalFDS33(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonalFDC33(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor12IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                default:
                    return ret;
            }
        }

        private double GetStressFactor13HillHexagonalFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            double ret = 0;
            double normFactor = 0;
            List<double[]> IntegrationAngles = new List<double[]>();

            switch (parameter)
            {
                case 0:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonalFDS11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonalFDC11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor13IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 1:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonalFDS12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonalFDC12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor13IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 2:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonalFDS44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonalFDC44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor13IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 3:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonalFDS13(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonalFDC13(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor13IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 4:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonalFDS33(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonalFDC33(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor13IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                default:
                    return ret;
            }
        }

        private double GetStressFactor22HillHexagonalFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            double ret = 0;
            double normFactor = 0;
            List<double[]> IntegrationAngles = new List<double[]>();

            switch (parameter)
            {
                case 0:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonalFDS11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonalFDC11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor22IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 1:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonalFDS12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonalFDC12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor22IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 2:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonalFDS44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonalFDC44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor22IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 3:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonalFDS13(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonalFDC13(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor22IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 4:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonalFDS33(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonalFDC33(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor22IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                default:
                    return ret;
            }
        }

        private double GetStressFactor23HillHexagonalFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            double ret = 0;
            double normFactor = 0;
            List<double[]> IntegrationAngles = new List<double[]>();

            switch (parameter)
            {
                case 0:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonalFDS11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonalFDC11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor23IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 1:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonalFDS12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonalFDC12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor23IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 2:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonalFDS44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonalFDC44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor23IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 3:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonalFDS13(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonalFDC13(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor23IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 4:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonalFDS33(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonalFDC33(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor23IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                default:
                    return ret;
            }
        }

        private double GetStressFactor33HillHexagonalFD(Stress.Macroskopic.PeakStressAssociation usedAssociation, int parameter)
        {
            double ret = 0;
            double normFactor = 0;
            List<double[]> IntegrationAngles = new List<double[]>();

            switch (parameter)
            {
                case 0:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonalFDS11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonalFDC11(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor33IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 1:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonalFDS12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonalFDC12(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor33IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 2:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonalFDS44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonalFDC44(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor33IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 3:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonalFDS13(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonalFDC13(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor33IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                case 4:
                    IntegrationAngles = GetEulerAnglesParallelToMeasurmentVektor(usedAssociation.DPeak.AssociatedHKLReflex, usedAssociation.phiAngle, usedAssociation.psiAngle);

                    for (int n = 0; n < IntegrationAngles.Count; n++)
                    {
                        Stress.Microsopic.ElasticityTensors orientedCTensor = this.OrientedComplianceTensorHexagonalFDS33(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);
                        Stress.Microsopic.ElasticityTensors orientedSTensor = this.OrientedStiffnessTensorHexagonalFDC33(n * 5, usedAssociation.DPeak.AssociatedHKLReflex);

                        Stress.Microsopic.ElasticityTensors orientedTensor = orientedCTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                        double ODFValue = this[IntegrationAngles[n][0], IntegrationAngles[n][1], IntegrationAngles[n][2]];

                        ret += stressFactor33IntegrantVoigt(usedAssociation, orientedTensor, ODFValue) * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                        normFactor += ODFValue * Math.Sin(IntegrationAngles[n][1] * (Math.PI / 180.0));
                    }

                    if (normFactor == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return ret / normFactor;
                    }
                default:
                    return ret;
            }
        }

        #endregion

        #endregion

        #endregion

        #region Integration Limits in Eulerangles

        public List<double[]> GetEulerAnglesParallelToMeasurmentVektor(DataManagment.CrystalData.HKLReflex hKL, double phi, double psi)
        {
            List<double[]> ret = new List<double[]>();

            for(int n = 0; n < 360; n += 5)
            {
                ret.Add(GetEulerAngleParallelToMeasurmentVektor(n, hKL, phi, psi));
            }

            return ret;
        }

        private double[] GetEulerAngleParallelToMeasurmentVektor(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL, double phi, double psi)
        {
            double[] ret = { 0.0, 0.0, 0.0 };

            MathNet.Numerics.LinearAlgebra.Matrix<double> SampleToMeasurmentTransformation = this.GetSMTM(phi, psi);
            MathNet.Numerics.LinearAlgebra.Matrix<double> MeasurmentToCrystalTransformation = this.GetMCTM(rotationAngle, hKL);

            //MathNet.Numerics.LinearAlgebra.Matrix<double> CrystalToSample = SampleToMeasurmentTransformation.Transpose() * MeasurmentToCrystalTransformation.Transpose();
            MathNet.Numerics.LinearAlgebra.Matrix<double> CrystalToSample = SampleToMeasurmentTransformation.Transpose() * MeasurmentToCrystalTransformation;

            ret[1] = Math.Acos(CrystalToSample[2, 2]);

            #region Martins Ansatz

            //MathNet.Numerics.LinearAlgebra.Vector<double> SampleFrameUV1 = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3, 0.0);
            //MathNet.Numerics.LinearAlgebra.Vector<double> SampleFrameUV2 = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3, 0.0);
            //MathNet.Numerics.LinearAlgebra.Vector<double> SampleFrameUV3 = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3, 0.0);

            //SampleFrameUV1[0] = 1;
            //SampleFrameUV2[1] = 1;
            //SampleFrameUV3[2] = 1;

            //MathNet.Numerics.LinearAlgebra.Vector<double> CrystalFrameUV1 = CrystalToSample * SampleFrameUV1;
            //MathNet.Numerics.LinearAlgebra.Vector<double> CrystalFrameUV2 = CrystalToSample * SampleFrameUV2;
            //MathNet.Numerics.LinearAlgebra.Vector<double> CrystalFrameUV3 = CrystalToSample * SampleFrameUV3;

            //if (Math.Abs(ret[1]) < 10e-5 || Math.Abs(ret[1] - Math.PI) < 10e-5)
            //{
            //    ret[0] = Math.Acos((SampleFrameUV1 * CrystalFrameUV1) / (SampleFrameUV1.L2Norm() * CrystalFrameUV1.L2Norm()));

            //    if (Math.Abs(CrystalFrameUV1[1]) < 10e-5)
            //    {
            //        if (CrystalFrameUV1[0] < 0)
            //        {
            //            ret[0] = Math.PI;
            //        }
            //        else
            //        {
            //            ret[0] = 0.0;
            //        }
            //    }
            //    else if (CrystalFrameUV1[1] <= 10e-5)
            //    {
            //        ret[0] = (2.0 * Math.PI) - ret[0];
            //    }
            //}
            //else
            //{
            //    MathNet.Numerics.LinearAlgebra.Vector<double> CrystalFrameProjectionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3, 0.0);

            //    CrystalFrameProjectionVector[0] = CrystalFrameUV3[0];
            //    CrystalFrameProjectionVector[1] = CrystalFrameUV3[1];

            //    ret[0] = Math.Acos((SampleFrameUV2 * CrystalFrameProjectionVector) / (SampleFrameUV2.L2Norm() * CrystalFrameProjectionVector.L2Norm()));

            //    if (Math.Abs(CrystalFrameUV3[0]) < 10e-5)
            //    {
            //        if (CrystalFrameUV3[1] < 0)
            //        {
            //            ret[0] = Math.PI;
            //        }
            //        else
            //        {
            //            ret[0] = 0.0;
            //        }
            //    }
            //    else if (CrystalFrameUV1[0] <= 10e-5)
            //    {
            //        ret[0] = (2.0 * Math.PI) - ret[0];
            //    }
            //}

            //if (!(Math.Abs(ret[1]) < 10e-5 || Math.Abs(ret[1] - Math.PI) < 10e-5))
            //{
            //    MathNet.Numerics.LinearAlgebra.Vector<double> PolarVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3, 0.0);

            //    PolarVector[0] = Math.Cos(ret[0]);
            //    PolarVector[1] = Math.Sin(ret[0]);

            //    ret[2] = Math.Acos((SampleFrameUV1 * PolarVector) / (SampleFrameUV1.L2Norm() * PolarVector.L2Norm()));

            //    if (Math.Abs(CrystalFrameUV1[2]) < 10e-5)
            //    {
            //        if (Math.Abs(PolarVector[0] - CrystalFrameUV1[0]) < 10e-5 && Math.Abs(PolarVector[1] - CrystalFrameUV1[1]) < 10e-5 && Math.Abs(PolarVector[2] - CrystalFrameUV1[2]) < 10e-5)
            //        {
            //            ret[2] = 0.0;
            //        }
            //        else if (Math.Abs(PolarVector[0] + CrystalFrameUV1[0]) < 10e-5 && Math.Abs(PolarVector[1] + CrystalFrameUV1[1]) < 10e-5 && Math.Abs(PolarVector[2] + CrystalFrameUV1[2]) < 10e-5)
            //        {
            //            ret[2] = Math.PI;
            //        }
            //    }
            //    else if (CrystalFrameUV1[2] <= 10e-5)
            //    {
            //        ret[2] = (2.0 * Math.PI) - ret[2];
            //    }
            //}

            #endregion

            #region Parametervergleich
            //Die Transformationansmatrizen Sample to Krystall wird mit der Transponierten Krystall zu Sample direkt verglichen und die Eulerwinkel berechnet

            if (Math.Abs(ret[1]) > 10e-5 && Math.Abs(ret[1] - Math.PI) > 10e-5)
            {
                //Achtung
                //ret[2] = Math.Acos(CrystalToSample[1, 2] / Math.Sin(ret[1]));
                //double MachineCheck = -1 * CrystalToSample[2, 1] / Math.Sin(ret[1]);
                double MachineCheck = CrystalToSample[2, 1] / Math.Sin(ret[1]);

                if (MachineCheck < -1)
                {
                    MachineCheck = -1;
                }
                if (MachineCheck > 1)
                {
                    MachineCheck = 1;
                }

                ret[2] = Math.Acos(MachineCheck);

                MachineCheck = -1 * CrystalToSample[1, 2] / Math.Sin(ret[1]);

                if (MachineCheck < -1)
                {
                    MachineCheck = -1;
                }
                if (MachineCheck > 1)
                {
                    MachineCheck = 1;
                }

                ret[0] = Math.Acos(MachineCheck);
            }
            else
            {
                double MachineCheck = CrystalToSample[0, 0];

                if (MachineCheck < -1)
                {
                    MachineCheck = -1;
                }
                if (MachineCheck > 1)
                {
                    MachineCheck = 1;
                }

                ret[0] = Math.Acos(MachineCheck);
                ret[2] = 0.0;
            }

            #endregion

            ret[0] = Convert.ToInt32((ret[0] * 180) / Math.PI);
            ret[1] = Convert.ToInt32((ret[1] * 180) / Math.PI);
            ret[2] = Convert.ToInt32((ret[2] * 180) / Math.PI);

            double ResolutionCheck = Convert.ToInt32(ret[0]) % Convert.ToInt32(this._stepSizePhi1);
            if (ResolutionCheck < 2.5)
            {
                ret[0] = ret[0] - ResolutionCheck;
            }
            else
            {
                ret[0] = ret[0] - ResolutionCheck + 5;
            }
            ResolutionCheck = Convert.ToInt32(ret[1]) % Convert.ToInt32(this._stepSizePhi);
            if (ResolutionCheck < 2.5)
            {
                ret[1] = ret[1] - ResolutionCheck;
            }
            else
            {
                ret[1] = ret[1] - ResolutionCheck + 5;
            }
            ResolutionCheck = Convert.ToInt32(ret[2]) % Convert.ToInt32(this._stepSizePhi2);
            if (ResolutionCheck < 2.5)
            {
                ret[2] = ret[2] - ResolutionCheck;
            }
            else
            {
                ret[2] = ret[2] - ResolutionCheck + 5;
            }

            return ret;

        }

        #endregion

        #endregion

        #region Probensystem zu Kristallsystem (Falsch?!?)

        //#region Oriented elastic constants calculation

        ///// <summary>
        ///// Used For Voigt
        ///// </summary>
        ///// <param name="varPhi1"></param>
        ///// <param name="phi"></param>
        ///// <param name="varPhi2"></param>
        ///// <returns></returns>
        //public Stress.Microsopic.ElasticityTensors OrientedStiffnessTensorCubic(double varPhi1, double phi, double varPhi2)
        //{
        //    Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

        //    ret.C11 = 0;
        //    ret.C12 = 0;
        //    ret.C44 = 0;

        //    ret.C11 += Math.Pow(SCTM11(varPhi1, phi, varPhi2), 4) * this.BaseTensor.C11;
        //    ret.C11 += Math.Pow(SCTM12(varPhi1, phi, varPhi2), 4) * this.BaseTensor.C22;
        //    ret.C11 += Math.Pow(SCTM13(varPhi1, phi, varPhi2), 4) * this.BaseTensor.C33;

        //    ret.C11 += 2 * Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C12;
        //    ret.C11 += 2 * Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C13;
        //    ret.C11 += 2 * Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C23;

        //    ret.C11 += 4 * Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C44;
        //    ret.C11 += 4 * Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C55;
        //    ret.C11 += 4 * Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C66;

        //    ret.C12 += Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C11;
        //    ret.C12 += Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM22(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C22;
        //    ret.C12 += Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM23(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C33;

        //    ret.C12 += Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM22(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C12;
        //    ret.C12 += Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C12;

        //    ret.C12 += Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM23(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C13;
        //    ret.C12 += Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C13;

        //    ret.C12 += Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM23(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C23;
        //    ret.C12 += Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM22(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C23;

        //    ret.C12 += 4 * SCTM12(varPhi1, phi, varPhi2) * SCTM13(varPhi1, phi, varPhi2) * SCTM22(varPhi1, phi, varPhi2) * SCTM23(varPhi1, phi, varPhi2) * this.BaseTensor.C44;
        //    ret.C12 += 4 * SCTM11(varPhi1, phi, varPhi2) * SCTM13(varPhi1, phi, varPhi2) * SCTM21(varPhi1, phi, varPhi2) * SCTM23(varPhi1, phi, varPhi2) * this.BaseTensor.C55;
        //    ret.C12 += 4 * SCTM11(varPhi1, phi, varPhi2) * SCTM12(varPhi1, phi, varPhi2) * SCTM21(varPhi1, phi, varPhi2) * SCTM22(varPhi1, phi, varPhi2) * this.BaseTensor.C66;

        //    ret.C44 += Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C11;
        //    ret.C44 += Math.Pow(SCTM22(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C22;
        //    ret.C44 += Math.Pow(SCTM23(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C33;

        //    ret.C44 += 2 * SCTM21(varPhi1, phi, varPhi2) * SCTM31(varPhi1, phi, varPhi2) * SCTM22(varPhi1, phi, varPhi2) * SCTM32(varPhi1, phi, varPhi2) * this.BaseTensor.C12;
        //    ret.C44 += 2 * SCTM21(varPhi1, phi, varPhi2) * SCTM31(varPhi1, phi, varPhi2) * SCTM23(varPhi1, phi, varPhi2) * SCTM33(varPhi1, phi, varPhi2) * this.BaseTensor.C13;
        //    ret.C44 += 2 * SCTM22(varPhi1, phi, varPhi2) * SCTM32(varPhi1, phi, varPhi2) * SCTM23(varPhi1, phi, varPhi2) * SCTM33(varPhi1, phi, varPhi2) * this.BaseTensor.C23;

        //    ret.C44 += Math.Pow(SCTM22(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C44;
        //    ret.C44 += 2 * SCTM23(varPhi1, phi, varPhi2) * SCTM32(varPhi1, phi, varPhi2) * SCTM22(varPhi1, phi, varPhi2) * SCTM33(varPhi1, phi, varPhi2) * this.BaseTensor.C44;
        //    ret.C44 += Math.Pow(SCTM23(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C44;

        //    ret.C44 += Math.Pow(SCTM23(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C55;
        //    ret.C44 += 2 * SCTM21(varPhi1, phi, varPhi2) * SCTM33(varPhi1, phi, varPhi2) * SCTM23(varPhi1, phi, varPhi2) * SCTM31(varPhi1, phi, varPhi2) * this.BaseTensor.C55;
        //    ret.C44 += Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C55;

        //    ret.C44 += Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C66;
        //    ret.C44 += 2 * SCTM21(varPhi1, phi, varPhi2) * SCTM32(varPhi1, phi, varPhi2) * SCTM22(varPhi1, phi, varPhi2) * SCTM31(varPhi1, phi, varPhi2) * this.BaseTensor.C66;
        //    ret.C44 += Math.Pow(SCTM22(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C66;

        //    ret.CalculateCompliances();

        //    return ret;

        //}

        ///// <summary>
        ///// Used for reuss
        ///// </summary>
        ///// <param name="varPhi1"></param>
        ///// <param name="phi"></param>
        ///// <param name="varPhi2"></param>
        ///// <returns></returns>
        //public Stress.Microsopic.ElasticityTensors OrientedComplianceTensorCubic(double varPhi1, double phi, double varPhi2)
        //{
        //    Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

        //    ret.S11 = 0;
        //    ret.S12 = 0;
        //    ret.S44 = 0;

        //    ret.S11 += Math.Pow(SCTM11(varPhi1, phi, varPhi2), 4) * this.BaseTensor.S11;
        //    ret.S11 += Math.Pow(SCTM12(varPhi1, phi, varPhi2), 4) * this.BaseTensor.S22;
        //    ret.S11 += Math.Pow(SCTM13(varPhi1, phi, varPhi2), 4) * this.BaseTensor.S33;

        //    ret.S11 += 2 * Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S12;
        //    ret.S11 += 2 * Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S13;
        //    ret.S11 += 2 * Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S23;

        //    ret.S11 += 4 * Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S44;
        //    ret.S11 += 4 * Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S55;
        //    ret.S11 += 4 * Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S66;

        //    ret.S12 += Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S11;
        //    ret.S12 += Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM22(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S22;
        //    ret.S12 += Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM23(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S33;

        //    ret.S12 += Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM22(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S12;
        //    ret.S12 += Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S12;

        //    ret.S12 += Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM23(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S13;
        //    ret.S12 += Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S13;

        //    ret.S12 += Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM23(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S23;
        //    ret.S12 += Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM22(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S23;

        //    ret.S12 += 4 * SCTM12(varPhi1, phi, varPhi2) * SCTM13(varPhi1, phi, varPhi2) * SCTM22(varPhi1, phi, varPhi2) * SCTM23(varPhi1, phi, varPhi2) * this.BaseTensor.S44;
        //    ret.S12 += 4 * SCTM11(varPhi1, phi, varPhi2) * SCTM13(varPhi1, phi, varPhi2) * SCTM21(varPhi1, phi, varPhi2) * SCTM23(varPhi1, phi, varPhi2) * this.BaseTensor.S55;
        //    ret.S12 += 4 * SCTM11(varPhi1, phi, varPhi2) * SCTM12(varPhi1, phi, varPhi2) * SCTM21(varPhi1, phi, varPhi2) * SCTM22(varPhi1, phi, varPhi2) * this.BaseTensor.S66;

        //    ret.S44 += Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S11;
        //    ret.S44 += Math.Pow(SCTM22(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S22;
        //    ret.S44 += Math.Pow(SCTM23(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S33;

        //    ret.S44 += 2 * SCTM21(varPhi1, phi, varPhi2) * SCTM31(varPhi1, phi, varPhi2) * SCTM22(varPhi1, phi, varPhi2) * SCTM32(varPhi1, phi, varPhi2) * this.BaseTensor.S12;
        //    ret.S44 += 2 * SCTM21(varPhi1, phi, varPhi2) * SCTM31(varPhi1, phi, varPhi2) * SCTM23(varPhi1, phi, varPhi2) * SCTM33(varPhi1, phi, varPhi2) * this.BaseTensor.S13;
        //    ret.S44 += 2 * SCTM22(varPhi1, phi, varPhi2) * SCTM32(varPhi1, phi, varPhi2) * SCTM23(varPhi1, phi, varPhi2) * SCTM33(varPhi1, phi, varPhi2) * this.BaseTensor.S23;

        //    ret.S44 += Math.Pow(SCTM22(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S44;
        //    ret.S44 += 2 * SCTM23(varPhi1, phi, varPhi2) * SCTM32(varPhi1, phi, varPhi2) * SCTM22(varPhi1, phi, varPhi2) * SCTM33(varPhi1, phi, varPhi2) * this.BaseTensor.S44;
        //    ret.S44 += Math.Pow(SCTM23(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S44;

        //    ret.S44 += Math.Pow(SCTM23(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S55;
        //    ret.S44 += 2 * SCTM21(varPhi1, phi, varPhi2) * SCTM33(varPhi1, phi, varPhi2) * SCTM23(varPhi1, phi, varPhi2) * SCTM31(varPhi1, phi, varPhi2) * this.BaseTensor.S55;
        //    ret.S44 += Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S55;

        //    ret.S44 += Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S66;
        //    ret.S44 += 2 * SCTM21(varPhi1, phi, varPhi2) * SCTM32(varPhi1, phi, varPhi2) * SCTM22(varPhi1, phi, varPhi2) * SCTM31(varPhi1, phi, varPhi2) * this.BaseTensor.S66;
        //    ret.S44 += Math.Pow(SCTM22(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S66;

        //    ret.CalculateStiffnesses();

        //    return ret;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="varPhi1"></param>
        ///// <param name="phi"></param>
        ///// <param name="varPhi2"></param>
        ///// <returns></returns>
        //public Stress.Microsopic.ElasticityTensors OrientedComplianceTensorHexagonal(double varPhi1, double phi, double varPhi2)
        //{
        //    Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

        //    ret.S11 = 0;
        //    ret.S33 = 0;
        //    ret.S12 = 0;
        //    ret.S13 = 0;
        //    ret.S44 = 0;

        //    ret.S11 += Math.Pow(SCTM11(varPhi1, phi, varPhi2), 4) * this.BaseTensor.S11;
        //    ret.S11 += Math.Pow(SCTM12(varPhi1, phi, varPhi2), 4) * this.BaseTensor.S22;
        //    ret.S11 += Math.Pow(SCTM13(varPhi1, phi, varPhi2), 4) * this.BaseTensor.S33;

        //    ret.S11 += 2 * Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S12;
        //    ret.S11 += 2 * Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S13;
        //    ret.S11 += 2 * Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S23;

        //    ret.S11 += 4 * Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S44;
        //    ret.S11 += 4 * Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S55;
        //    ret.S11 += 4 * Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S66;

        //    ret.S33 += Math.Pow(SCTM31(varPhi1, phi, varPhi2), 4) * this.BaseTensor.S11;
        //    ret.S33 += Math.Pow(SCTM32(varPhi1, phi, varPhi2), 4) * this.BaseTensor.S22;
        //    ret.S33 += Math.Pow(SCTM33(varPhi1, phi, varPhi2), 4) * this.BaseTensor.S33;

        //    ret.S33 += 2 * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S12;
        //    ret.S33 += 2 * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S13;
        //    ret.S33 += 2 * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S23;

        //    ret.S33 += 4 * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S44;
        //    ret.S33 += 4 * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S55;
        //    ret.S33 += 4 * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S66;

        //    ret.S12 += Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S11;
        //    ret.S12 += Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM22(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S22;
        //    ret.S12 += Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM23(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S33;

        //    ret.S12 += Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM22(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S12;
        //    ret.S12 += Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S12;

        //    ret.S12 += Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM23(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S13;
        //    ret.S12 += Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S13;

        //    ret.S12 += Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM23(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S23;
        //    ret.S12 += Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM22(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S23;

        //    ret.S12 += 4 * SCTM12(varPhi1, phi, varPhi2) * SCTM13(varPhi1, phi, varPhi2) * SCTM22(varPhi1, phi, varPhi2) * SCTM23(varPhi1, phi, varPhi2) * this.BaseTensor.S44;
        //    ret.S12 += 4 * SCTM11(varPhi1, phi, varPhi2) * SCTM13(varPhi1, phi, varPhi2) * SCTM21(varPhi1, phi, varPhi2) * SCTM23(varPhi1, phi, varPhi2) * this.BaseTensor.S55;
        //    ret.S12 += 4 * SCTM11(varPhi1, phi, varPhi2) * SCTM12(varPhi1, phi, varPhi2) * SCTM21(varPhi1, phi, varPhi2) * SCTM22(varPhi1, phi, varPhi2) * this.BaseTensor.S66;

        //    ret.S13 += Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S11;
        //    ret.S13 += Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S22;
        //    ret.S13 += Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S33;

        //    ret.S13 += Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S12;
        //    ret.S13 += Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S12;

        //    ret.S13 += Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S13;
        //    ret.S13 += Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S13;

        //    ret.S13 += Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S23;
        //    ret.S13 += Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S23;

        //    ret.S13 += 4 * SCTM12(varPhi1, phi, varPhi2) * SCTM13(varPhi1, phi, varPhi2) * SCTM32(varPhi1, phi, varPhi2) * SCTM33(varPhi1, phi, varPhi2) * this.BaseTensor.S44;
        //    ret.S13 += 4 * SCTM11(varPhi1, phi, varPhi2) * SCTM13(varPhi1, phi, varPhi2) * SCTM31(varPhi1, phi, varPhi2) * SCTM33(varPhi1, phi, varPhi2) * this.BaseTensor.S55;
        //    ret.S13 += 4 * SCTM11(varPhi1, phi, varPhi2) * SCTM12(varPhi1, phi, varPhi2) * SCTM31(varPhi1, phi, varPhi2) * SCTM32(varPhi1, phi, varPhi2) * this.BaseTensor.S66;

        //    ret.S44 += Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S11;
        //    ret.S44 += Math.Pow(SCTM22(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S22;
        //    ret.S44 += Math.Pow(SCTM23(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S33;

        //    ret.S44 += 2 * SCTM21(varPhi1, phi, varPhi2) * SCTM31(varPhi1, phi, varPhi2) * SCTM22(varPhi1, phi, varPhi2) * SCTM32(varPhi1, phi, varPhi2) * this.BaseTensor.S12;
        //    ret.S44 += 2 * SCTM21(varPhi1, phi, varPhi2) * SCTM31(varPhi1, phi, varPhi2) * SCTM23(varPhi1, phi, varPhi2) * SCTM33(varPhi1, phi, varPhi2) * this.BaseTensor.S13;
        //    ret.S44 += 2 * SCTM22(varPhi1, phi, varPhi2) * SCTM32(varPhi1, phi, varPhi2) * SCTM23(varPhi1, phi, varPhi2) * SCTM33(varPhi1, phi, varPhi2) * this.BaseTensor.S23;

        //    ret.S44 += 2 * Math.Pow(SCTM22(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S44;
        //    ret.S44 += 2 * SCTM23(varPhi1, phi, varPhi2) * SCTM32(varPhi1, phi, varPhi2) * SCTM22(varPhi1, phi, varPhi2) * SCTM33(varPhi1, phi, varPhi2) * this.BaseTensor.S44;

        //    ret.S44 += 2 * Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S55;
        //    ret.S44 += 2 * SCTM21(varPhi1, phi, varPhi2) * SCTM33(varPhi1, phi, varPhi2) * SCTM23(varPhi1, phi, varPhi2) * SCTM31(varPhi1, phi, varPhi2) * this.BaseTensor.S55;

        //    ret.S44 += 2 * Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2) * this.BaseTensor.S66;
        //    ret.S44 += 2 * SCTM21(varPhi1, phi, varPhi2) * SCTM32(varPhi1, phi, varPhi2) * SCTM22(varPhi1, phi, varPhi2) * SCTM31(varPhi1, phi, varPhi2) * this.BaseTensor.S66;

        //    return ret;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="varPhi1"></param>
        ///// <param name="phi"></param>
        ///// <param name="varPhi2"></param>
        ///// <returns></returns>
        //public Stress.Microsopic.ElasticityTensors OrientedStiffnessTensorHexagonal(double varPhi1, double phi, double varPhi2)
        //{
        //    Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

        //    ret.C11 = 0;
        //    ret.C33 = 0;
        //    ret.C12 = 0;
        //    ret.C13 = 0;
        //    ret.C44 = 0;

        //    ret.C11 += Math.Pow(SCTM11(varPhi1, phi, varPhi2), 4) * this.BaseTensor.C11;
        //    ret.C11 += Math.Pow(SCTM12(varPhi1, phi, varPhi2), 4) * this.BaseTensor.C22;
        //    ret.C11 += Math.Pow(SCTM13(varPhi1, phi, varPhi2), 4) * this.BaseTensor.C33;

        //    ret.C11 += 2 * Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C12;
        //    ret.C11 += 2 * Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C13;
        //    ret.C11 += 2 * Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C23;

        //    ret.C11 += 4 * Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C44;
        //    ret.C11 += 4 * Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C55;
        //    ret.C11 += 4 * Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C66;

        //    ret.C33 += Math.Pow(SCTM31(varPhi1, phi, varPhi2), 4) * this.BaseTensor.C11;
        //    ret.C33 += Math.Pow(SCTM32(varPhi1, phi, varPhi2), 4) * this.BaseTensor.C22;
        //    ret.C33 += Math.Pow(SCTM33(varPhi1, phi, varPhi2), 4) * this.BaseTensor.C33;

        //    ret.C33 += 2 * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C12;
        //    ret.C33 += 2 * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C13;
        //    ret.C33 += 2 * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C23;

        //    ret.C33 += 4 * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C44;
        //    ret.C33 += 4 * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C55;
        //    ret.C33 += 4 * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C66;

        //    ret.C12 += Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C11;
        //    ret.C12 += Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM22(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C22;
        //    ret.C12 += Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM23(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C33;

        //    ret.C12 += Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM22(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C12;
        //    ret.C12 += Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C12;

        //    ret.C12 += Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM23(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C13;
        //    ret.C12 += Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C13;

        //    ret.C12 += Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM23(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C23;
        //    ret.C12 += Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM22(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C23;

        //    ret.C12 += 4 * SCTM12(varPhi1, phi, varPhi2) * SCTM13(varPhi1, phi, varPhi2) * SCTM22(varPhi1, phi, varPhi2) * SCTM23(varPhi1, phi, varPhi2) * this.BaseTensor.C44;
        //    ret.C12 += 4 * SCTM11(varPhi1, phi, varPhi2) * SCTM13(varPhi1, phi, varPhi2) * SCTM21(varPhi1, phi, varPhi2) * SCTM23(varPhi1, phi, varPhi2) * this.BaseTensor.C55;
        //    ret.C12 += 4 * SCTM11(varPhi1, phi, varPhi2) * SCTM12(varPhi1, phi, varPhi2) * SCTM21(varPhi1, phi, varPhi2) * SCTM22(varPhi1, phi, varPhi2) * this.BaseTensor.C66;

        //    ret.C13 += Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C11;
        //    ret.C13 += Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C22;
        //    ret.C13 += Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C33;

        //    ret.C13 += Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C12;
        //    ret.C13 += Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C12;

        //    ret.C13 += Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C13;
        //    ret.C13 += Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C13;

        //    ret.C13 += Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C23;
        //    ret.C13 += Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C23;

        //    ret.C13 += 4 * SCTM12(varPhi1, phi, varPhi2) * SCTM13(varPhi1, phi, varPhi2) * SCTM32(varPhi1, phi, varPhi2) * SCTM33(varPhi1, phi, varPhi2) * this.BaseTensor.C44;
        //    ret.C13 += 4 * SCTM11(varPhi1, phi, varPhi2) * SCTM13(varPhi1, phi, varPhi2) * SCTM31(varPhi1, phi, varPhi2) * SCTM33(varPhi1, phi, varPhi2) * this.BaseTensor.C55;
        //    ret.C13 += 4 * SCTM11(varPhi1, phi, varPhi2) * SCTM12(varPhi1, phi, varPhi2) * SCTM31(varPhi1, phi, varPhi2) * SCTM32(varPhi1, phi, varPhi2) * this.BaseTensor.C66;

        //    ret.C44 += Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C11;
        //    ret.C44 += Math.Pow(SCTM22(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C22;
        //    ret.C44 += Math.Pow(SCTM23(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C33;

        //    ret.C44 += 2 * SCTM21(varPhi1, phi, varPhi2) * SCTM31(varPhi1, phi, varPhi2) * SCTM22(varPhi1, phi, varPhi2) * SCTM32(varPhi1, phi, varPhi2) * this.BaseTensor.C12;
        //    ret.C44 += 2 * SCTM21(varPhi1, phi, varPhi2) * SCTM31(varPhi1, phi, varPhi2) * SCTM23(varPhi1, phi, varPhi2) * SCTM33(varPhi1, phi, varPhi2) * this.BaseTensor.C13;
        //    ret.C44 += 2 * SCTM22(varPhi1, phi, varPhi2) * SCTM32(varPhi1, phi, varPhi2) * SCTM23(varPhi1, phi, varPhi2) * SCTM33(varPhi1, phi, varPhi2) * this.BaseTensor.C23;

        //    ret.C44 += 2 * Math.Pow(SCTM22(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C44;
        //    ret.C44 += 2 * SCTM23(varPhi1, phi, varPhi2) * SCTM32(varPhi1, phi, varPhi2) * SCTM22(varPhi1, phi, varPhi2) * SCTM33(varPhi1, phi, varPhi2) * this.BaseTensor.C44;

        //    ret.C44 += 2 * Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C55;
        //    ret.C44 += 2 * SCTM21(varPhi1, phi, varPhi2) * SCTM33(varPhi1, phi, varPhi2) * SCTM23(varPhi1, phi, varPhi2) * SCTM31(varPhi1, phi, varPhi2) * this.BaseTensor.C55;

        //    ret.C44 += 2 * Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C66;
        //    ret.C44 += 2 * SCTM21(varPhi1, phi, varPhi2) * SCTM32(varPhi1, phi, varPhi2) * SCTM22(varPhi1, phi, varPhi2) * SCTM31(varPhi1, phi, varPhi2) * this.BaseTensor.C66;

        //    return ret;

        //}

        //#region First derivatives

        //#region Cubic

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="varPhi1"></param>
        ///// <param name="phi"></param>
        ///// <param name="varPhi2"></param>
        ///// <returns></returns>
        //public Stress.Microsopic.ElasticityTensors OrientedStiffnessTensorCubicFDC11(double varPhi1, double phi, double varPhi2)
        //{
        //    Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

        //    ret.C11 = 0;
        //    ret.C12 = 0;
        //    ret.C44 = 0;

        //    ret.C11 += Math.Pow(SCTM11(varPhi1, phi, varPhi2), 4);
        //    ret.C11 += Math.Pow(SCTM12(varPhi1, phi, varPhi2), 4);
        //    ret.C11 += Math.Pow(SCTM13(varPhi1, phi, varPhi2), 4);

        //    ret.C12 += Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2);
        //    ret.C12 += Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM22(varPhi1, phi, varPhi2), 2);
        //    ret.C12 += Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM23(varPhi1, phi, varPhi2), 2);

        //    ret.C44 += Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2);
        //    ret.C44 += Math.Pow(SCTM22(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2);
        //    ret.C44 += Math.Pow(SCTM23(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2);

        //    Stress.Microsopic.ElasticityTensors InvertedOrigTensor = this.OrientedStiffnessTensorCubic(varPhi1, phi, varPhi2);

        //    ret._complianceTensor = -1.0 * InvertedOrigTensor._complianceTensor * ret._stiffnessTensor * InvertedOrigTensor._complianceTensor;

        //    ret.CalculateStiffnesses();

        //    return ret;

        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="varPhi1"></param>
        ///// <param name="phi"></param>
        ///// <param name="varPhi2"></param>
        ///// <returns></returns>
        //public Stress.Microsopic.ElasticityTensors OrientedStiffnessTensorCubicFDC12(double varPhi1, double phi, double varPhi2)
        //{
        //    Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

        //    ret.C11 = 0;
        //    ret.C12 = 0;
        //    ret.C44 = 0;

        //    ret.C11 += 2 * Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2);
        //    ret.C11 += 2 * Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2);
        //    ret.C11 += 2 * Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2);

        //    ret.C12 += Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM22(varPhi1, phi, varPhi2), 2);
        //    ret.C12 += Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2);

        //    ret.C12 += Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM23(varPhi1, phi, varPhi2), 2);
        //    ret.C12 += Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2);

        //    ret.C12 += Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM23(varPhi1, phi, varPhi2), 2);
        //    ret.C12 += Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM22(varPhi1, phi, varPhi2), 2);

        //    ret.C44 += 2 * SCTM21(varPhi1, phi, varPhi2) * SCTM31(varPhi1, phi, varPhi2) * SCTM22(varPhi1, phi, varPhi2) * SCTM32(varPhi1, phi, varPhi2);
        //    ret.C44 += 2 * SCTM21(varPhi1, phi, varPhi2) * SCTM31(varPhi1, phi, varPhi2) * SCTM23(varPhi1, phi, varPhi2) * SCTM33(varPhi1, phi, varPhi2);
        //    ret.C44 += 2 * SCTM22(varPhi1, phi, varPhi2) * SCTM32(varPhi1, phi, varPhi2) * SCTM23(varPhi1, phi, varPhi2) * SCTM33(varPhi1, phi, varPhi2);

        //    Stress.Microsopic.ElasticityTensors InvertedOrigTensor = this.OrientedStiffnessTensorCubic(varPhi1, phi, varPhi2);

        //    ret._complianceTensor = -1.0 * InvertedOrigTensor._complianceTensor * ret._stiffnessTensor * InvertedOrigTensor._complianceTensor;

        //    ret.CalculateStiffnesses();
        //    return ret;

        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="varPhi1"></param>
        ///// <param name="phi"></param>
        ///// <param name="varPhi2"></param>
        ///// <returns></returns>
        //public Stress.Microsopic.ElasticityTensors OrientedStiffnessTensorCubicFDC44(double varPhi1, double phi, double varPhi2)
        //{
        //    Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

        //    ret.C11 = 0;
        //    ret.C12 = 0;
        //    ret.C44 = 0;

        //    ret.C11 += 4 * Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2);
        //    ret.C11 += 4 * Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2);
        //    ret.C11 += 4 * Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2);

        //    ret.C12 += 4 * SCTM12(varPhi1, phi, varPhi2) * SCTM13(varPhi1, phi, varPhi2) * SCTM22(varPhi1, phi, varPhi2) * SCTM23(varPhi1, phi, varPhi2);
        //    ret.C12 += 4 * SCTM11(varPhi1, phi, varPhi2) * SCTM13(varPhi1, phi, varPhi2) * SCTM21(varPhi1, phi, varPhi2) * SCTM23(varPhi1, phi, varPhi2);
        //    ret.C12 += 4 * SCTM11(varPhi1, phi, varPhi2) * SCTM12(varPhi1, phi, varPhi2) * SCTM21(varPhi1, phi, varPhi2) * SCTM22(varPhi1, phi, varPhi2);

        //    ret.C44 += Math.Pow(SCTM22(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2);
        //    ret.C44 += 2 * SCTM23(varPhi1, phi, varPhi2) * SCTM32(varPhi1, phi, varPhi2) * SCTM22(varPhi1, phi, varPhi2) * SCTM33(varPhi1, phi, varPhi2);
        //    ret.C44 += Math.Pow(SCTM23(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2);

        //    ret.C44 += Math.Pow(SCTM23(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2);
        //    ret.C44 += 2 * SCTM21(varPhi1, phi, varPhi2) * SCTM33(varPhi1, phi, varPhi2) * SCTM23(varPhi1, phi, varPhi2) * SCTM31(varPhi1, phi, varPhi2);
        //    ret.C44 += Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2);

        //    ret.C44 += Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2);
        //    ret.C44 += 2 * SCTM21(varPhi1, phi, varPhi2) * SCTM32(varPhi1, phi, varPhi2) * SCTM22(varPhi1, phi, varPhi2) * SCTM31(varPhi1, phi, varPhi2);
        //    ret.C44 += Math.Pow(SCTM22(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2);

        //    Stress.Microsopic.ElasticityTensors InvertedOrigTensor = this.OrientedStiffnessTensorCubic(varPhi1, phi, varPhi2);

        //    ret._complianceTensor = -1.0 * InvertedOrigTensor._complianceTensor * ret._stiffnessTensor * InvertedOrigTensor._complianceTensor;

        //    ret.CalculateStiffnesses();

        //    return ret;

        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="varPhi1"></param>
        ///// <param name="phi"></param>
        ///// <param name="varPhi2"></param>
        ///// <returns></returns>
        //public Stress.Microsopic.ElasticityTensors OrientedComplianceTensorCubicFDS11(double varPhi1, double phi, double varPhi2)
        //{
        //    Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

        //    ret.S11 = 0;
        //    ret.S12 = 0;
        //    ret.S44 = 0;

        //    ret.S11 += Math.Pow(SCTM11(varPhi1, phi, varPhi2), 4);
        //    ret.S11 += Math.Pow(SCTM12(varPhi1, phi, varPhi2), 4);
        //    ret.S11 += Math.Pow(SCTM13(varPhi1, phi, varPhi2), 4);

        //    ret.S12 += Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2);
        //    ret.S12 += Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM22(varPhi1, phi, varPhi2), 2);
        //    ret.S12 += Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM23(varPhi1, phi, varPhi2), 2);

        //    ret.S44 += Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2);
        //    ret.S44 += Math.Pow(SCTM22(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2);
        //    ret.S44 += Math.Pow(SCTM23(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2);

        //    ret.CalculateStiffnesses();

        //    return ret;

        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="varPhi1"></param>
        ///// <param name="phi"></param>
        ///// <param name="varPhi2"></param>
        ///// <returns></returns>
        //public Stress.Microsopic.ElasticityTensors OrientedComplianceTensorCubicFDS12(double varPhi1, double phi, double varPhi2)
        //{
        //    Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

        //    ret.S11 = 0;
        //    ret.S12 = 0;
        //    ret.S44 = 0;

        //    ret.S11 += 2 * Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2);
        //    ret.S11 += 2 * Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2);
        //    ret.S11 += 2 * Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2);

        //    ret.S12 += Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM22(varPhi1, phi, varPhi2), 2);
        //    ret.S12 += Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2);

        //    ret.S12 += Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM23(varPhi1, phi, varPhi2), 2);
        //    ret.S12 += Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2);

        //    ret.S12 += Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM23(varPhi1, phi, varPhi2), 2);
        //    ret.S12 += Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM22(varPhi1, phi, varPhi2), 2);

        //    ret.S44 += 2 * SCTM21(varPhi1, phi, varPhi2) * SCTM31(varPhi1, phi, varPhi2) * SCTM22(varPhi1, phi, varPhi2) * SCTM32(varPhi1, phi, varPhi2);
        //    ret.S44 += 2 * SCTM21(varPhi1, phi, varPhi2) * SCTM31(varPhi1, phi, varPhi2) * SCTM23(varPhi1, phi, varPhi2) * SCTM33(varPhi1, phi, varPhi2);
        //    ret.S44 += 2 * SCTM22(varPhi1, phi, varPhi2) * SCTM32(varPhi1, phi, varPhi2) * SCTM23(varPhi1, phi, varPhi2) * SCTM33(varPhi1, phi, varPhi2);

        //    ret.CalculateStiffnesses();
        //    return ret;

        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="varPhi1"></param>
        ///// <param name="phi"></param>
        ///// <param name="varPhi2"></param>
        ///// <returns></returns>
        //public Stress.Microsopic.ElasticityTensors OrientedComplianceTensorCubicFDS44(double varPhi1, double phi, double varPhi2)
        //{
        //    Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

        //    ret.S11 = 0;
        //    ret.S12 = 0;
        //    ret.S44 = 0;

        //    ret.S11 += 4 * Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2);
        //    ret.S11 += 4 * Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2);
        //    ret.S11 += 4 * Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2);

        //    ret.S12 += 4 * SCTM12(varPhi1, phi, varPhi2) * SCTM13(varPhi1, phi, varPhi2) * SCTM22(varPhi1, phi, varPhi2) * SCTM23(varPhi1, phi, varPhi2);
        //    ret.S12 += 4 * SCTM11(varPhi1, phi, varPhi2) * SCTM13(varPhi1, phi, varPhi2) * SCTM21(varPhi1, phi, varPhi2) * SCTM23(varPhi1, phi, varPhi2);
        //    ret.S12 += 4 * SCTM11(varPhi1, phi, varPhi2) * SCTM12(varPhi1, phi, varPhi2) * SCTM21(varPhi1, phi, varPhi2) * SCTM22(varPhi1, phi, varPhi2);

        //    ret.S44 += Math.Pow(SCTM22(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2);
        //    ret.S44 += 2 * SCTM23(varPhi1, phi, varPhi2) * SCTM32(varPhi1, phi, varPhi2) * SCTM22(varPhi1, phi, varPhi2) * SCTM33(varPhi1, phi, varPhi2);
        //    ret.S44 += Math.Pow(SCTM23(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2);

        //    ret.S44 += Math.Pow(SCTM23(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2);
        //    ret.S44 += 2 * SCTM21(varPhi1, phi, varPhi2) * SCTM33(varPhi1, phi, varPhi2) * SCTM23(varPhi1, phi, varPhi2) * SCTM31(varPhi1, phi, varPhi2);
        //    ret.S44 += Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2);

        //    ret.S44 += Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2);
        //    ret.S44 += 2 * SCTM21(varPhi1, phi, varPhi2) * SCTM32(varPhi1, phi, varPhi2) * SCTM22(varPhi1, phi, varPhi2) * SCTM31(varPhi1, phi, varPhi2);
        //    ret.S44 += Math.Pow(SCTM22(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2);

        //    ret.CalculateStiffnesses();

        //    return ret;
        //}

        //#endregion

        //#region Hexagonal

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="varPhi1"></param>
        ///// <param name="phi"></param>
        ///// <param name="varPhi2"></param>
        ///// <returns></returns>
        //public Stress.Microsopic.ElasticityTensors OrientedStiffnessTensorHexagonalFDC11(double varPhi1, double phi, double varPhi2)
        //{
        //    Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

        //    ret.C11 = 0;
        //    ret.C33 = 0;
        //    ret.C12 = 0;
        //    ret.C13 = 0;
        //    ret.C44 = 0;

        //    ret.C11 += Math.Pow(SCTM11(varPhi1, phi, varPhi2), 4);
        //    ret.C11 += Math.Pow(SCTM12(varPhi1, phi, varPhi2), 4);
        //    ret.C11 += 4 * Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * 2;

        //    ret.C33 += Math.Pow(SCTM31(varPhi1, phi, varPhi2), 4);
        //    ret.C33 += Math.Pow(SCTM32(varPhi1, phi, varPhi2), 4);
        //    ret.C33 += 4 * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2) * 2;

        //    ret.C12 += Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2);
        //    ret.C12 += Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM22(varPhi1, phi, varPhi2), 2);
        //    ret.C12 += 4 * SCTM11(varPhi1, phi, varPhi2) * SCTM12(varPhi1, phi, varPhi2) * SCTM21(varPhi1, phi, varPhi2) * SCTM22(varPhi1, phi, varPhi2) * 2;

        //    ret.C13 += Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2);
        //    ret.C13 += Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2);
        //    ret.C13 += 4 * SCTM11(varPhi1, phi, varPhi2) * SCTM12(varPhi1, phi, varPhi2) * SCTM31(varPhi1, phi, varPhi2) * SCTM32(varPhi1, phi, varPhi2) * 2;

        //    ret.C44 += Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2);
        //    ret.C44 += Math.Pow(SCTM22(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2);

        //    ret.C44 += 2 * Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2) * 2;
        //    ret.C44 += 2 * SCTM21(varPhi1, phi, varPhi2) * SCTM32(varPhi1, phi, varPhi2) * SCTM22(varPhi1, phi, varPhi2) * SCTM31(varPhi1, phi, varPhi2) * 2;

        //    return ret;

        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="varPhi1"></param>
        ///// <param name="phi"></param>
        ///// <param name="varPhi2"></param>
        ///// <returns></returns>
        //public Stress.Microsopic.ElasticityTensors OrientedStiffnessTensorHexagonalFDC33(double varPhi1, double phi, double varPhi2)
        //{
        //    Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

        //    ret.C11 = 0;
        //    ret.C33 = 0;
        //    ret.C12 = 0;
        //    ret.C13 = 0;
        //    ret.C44 = 0;

        //    ret.C11 += Math.Pow(SCTM13(varPhi1, phi, varPhi2), 4);

        //    ret.C33 += Math.Pow(SCTM33(varPhi1, phi, varPhi2), 4);

        //    ret.C12 += Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM23(varPhi1, phi, varPhi2), 2);

        //    ret.C13 += Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2);

        //    ret.C44 += Math.Pow(SCTM23(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2);

        //    return ret;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="varPhi1"></param>
        ///// <param name="phi"></param>
        ///// <param name="varPhi2"></param>
        ///// <returns></returns>
        //public Stress.Microsopic.ElasticityTensors OrientedStiffnessTensorHexagonalFDC12(double varPhi1, double phi, double varPhi2)
        //{
        //    Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

        //    ret.C11 = 0;
        //    ret.C33 = 0;
        //    ret.C12 = 0;
        //    ret.C13 = 0;
        //    ret.C44 = 0;

        //    ret.C11 += 2 * Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2);
        //    ret.C11 += 4 * Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * -2;

        //    ret.C33 += 2 * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2);

        //    ret.C33 += 4 * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2) * -2;

        //    ret.C12 += Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM22(varPhi1, phi, varPhi2), 2);
        //    ret.C12 += Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2);

        //    ret.C12 += 4 * SCTM11(varPhi1, phi, varPhi2) * SCTM12(varPhi1, phi, varPhi2) * SCTM21(varPhi1, phi, varPhi2) * SCTM22(varPhi1, phi, varPhi2) * -2;

        //    ret.C13 += Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C12;
        //    ret.C13 += Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C12;
        //    ret.C13 += 4 * SCTM11(varPhi1, phi, varPhi2) * SCTM12(varPhi1, phi, varPhi2) * SCTM31(varPhi1, phi, varPhi2) * SCTM32(varPhi1, phi, varPhi2) * -2;

        //    ret.C44 += 2 * SCTM21(varPhi1, phi, varPhi2) * SCTM31(varPhi1, phi, varPhi2) * SCTM22(varPhi1, phi, varPhi2) * SCTM32(varPhi1, phi, varPhi2);

        //    ret.C44 += 2 * Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C66;
        //    ret.C44 += 2 * SCTM21(varPhi1, phi, varPhi2) * SCTM32(varPhi1, phi, varPhi2) * SCTM22(varPhi1, phi, varPhi2) * SCTM31(varPhi1, phi, varPhi2) *-2;

        //    return ret;

        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="varPhi1"></param>
        ///// <param name="phi"></param>
        ///// <param name="varPhi2"></param>
        ///// <returns></returns>
        //public Stress.Microsopic.ElasticityTensors OrientedStiffnessTensorHexagonalFDC13(double varPhi1, double phi, double varPhi2)
        //{
        //    Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

        //    ret.C11 = 0;
        //    ret.C33 = 0;
        //    ret.C12 = 0;
        //    ret.C13 = 0;
        //    ret.C44 = 0;

        //    ret.C11 += 2 * Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2);
        //    ret.C11 += 2 * Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2);

        //    ret.C33 += 2 * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2);
        //    ret.C33 += 2 * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2);

        //    ret.C12 += Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM23(varPhi1, phi, varPhi2), 2);
        //    ret.C12 += Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2);

        //    ret.C12 += Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM23(varPhi1, phi, varPhi2), 2);
        //    ret.C12 += Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM22(varPhi1, phi, varPhi2), 2);

        //    ret.C13 += Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2);
        //    ret.C13 += Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2);

        //    ret.C13 += Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2);
        //    ret.C13 += Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2);

        //    ret.C44 += 2 * SCTM21(varPhi1, phi, varPhi2) * SCTM31(varPhi1, phi, varPhi2) * SCTM23(varPhi1, phi, varPhi2) * SCTM33(varPhi1, phi, varPhi2);
        //    ret.C44 += 2 * SCTM22(varPhi1, phi, varPhi2) * SCTM32(varPhi1, phi, varPhi2) * SCTM23(varPhi1, phi, varPhi2) * SCTM33(varPhi1, phi, varPhi2);

        //    return ret;

        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="varPhi1"></param>
        ///// <param name="phi"></param>
        ///// <param name="varPhi2"></param>
        ///// <returns></returns>
        //public Stress.Microsopic.ElasticityTensors OrientedStiffnessTensorHexagonalFDC44(double varPhi1, double phi, double varPhi2)
        //{
        //    Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

        //    ret.C11 = 0;
        //    ret.C33 = 0;
        //    ret.C12 = 0;
        //    ret.C13 = 0;
        //    ret.C44 = 0;

        //    ret.C11 += 4 * Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2);
        //    ret.C11 += 4 * Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2);

        //    ret.C33 += 4 * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2);
        //    ret.C33 += 4 * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2);

        //    ret.C12 += 4 * SCTM12(varPhi1, phi, varPhi2) * SCTM13(varPhi1, phi, varPhi2) * SCTM22(varPhi1, phi, varPhi2) * SCTM23(varPhi1, phi, varPhi2);
        //    ret.C12 += 4 * SCTM11(varPhi1, phi, varPhi2) * SCTM13(varPhi1, phi, varPhi2) * SCTM21(varPhi1, phi, varPhi2) * SCTM23(varPhi1, phi, varPhi2);

        //    ret.C13 += 4 * SCTM12(varPhi1, phi, varPhi2) * SCTM13(varPhi1, phi, varPhi2) * SCTM32(varPhi1, phi, varPhi2) * SCTM33(varPhi1, phi, varPhi2);
        //    ret.C13 += 4 * SCTM11(varPhi1, phi, varPhi2) * SCTM13(varPhi1, phi, varPhi2) * SCTM31(varPhi1, phi, varPhi2) * SCTM33(varPhi1, phi, varPhi2);

        //    ret.C44 += 2 * Math.Pow(SCTM22(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2);
        //    ret.C44 += 2 * SCTM23(varPhi1, phi, varPhi2) * SCTM32(varPhi1, phi, varPhi2) * SCTM22(varPhi1, phi, varPhi2) * SCTM33(varPhi1, phi, varPhi2);

        //    ret.C44 += 2 * Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2);
        //    ret.C44 += 2 * SCTM21(varPhi1, phi, varPhi2) * SCTM33(varPhi1, phi, varPhi2) * SCTM23(varPhi1, phi, varPhi2) * SCTM31(varPhi1, phi, varPhi2);

        //    return ret;

        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="varPhi1"></param>
        ///// <param name="phi"></param>
        ///// <param name="varPhi2"></param>
        ///// <returns></returns>
        //public Stress.Microsopic.ElasticityTensors OrientedComplianceTensorHexagonalFDC11(double varPhi1, double phi, double varPhi2)
        //{
        //    Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

        //    ret.S11 = 0;
        //    ret.S33 = 0;
        //    ret.S12 = 0;
        //    ret.S13 = 0;
        //    ret.S44 = 0;

        //    ret.S11 += Math.Pow(SCTM11(varPhi1, phi, varPhi2), 4);
        //    ret.S11 += Math.Pow(SCTM12(varPhi1, phi, varPhi2), 4);
        //    ret.S11 += 4 * Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * 2;

        //    ret.S33 += Math.Pow(SCTM31(varPhi1, phi, varPhi2), 4);
        //    ret.S33 += Math.Pow(SCTM32(varPhi1, phi, varPhi2), 4);
        //    ret.S33 += 4 * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2) * 2;

        //    ret.S12 += Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2);
        //    ret.S12 += Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM22(varPhi1, phi, varPhi2), 2);
        //    ret.S12 += 4 * SCTM11(varPhi1, phi, varPhi2) * SCTM12(varPhi1, phi, varPhi2) * SCTM21(varPhi1, phi, varPhi2) * SCTM22(varPhi1, phi, varPhi2) * 2;

        //    ret.S13 += Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2);
        //    ret.S13 += Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2);
        //    ret.S13 += 4 * SCTM11(varPhi1, phi, varPhi2) * SCTM12(varPhi1, phi, varPhi2) * SCTM31(varPhi1, phi, varPhi2) * SCTM32(varPhi1, phi, varPhi2) * 2;

        //    ret.S44 += Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2);
        //    ret.S44 += Math.Pow(SCTM22(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2);

        //    ret.S44 += 2 * Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2) * 2;
        //    ret.S44 += 2 * SCTM21(varPhi1, phi, varPhi2) * SCTM32(varPhi1, phi, varPhi2) * SCTM22(varPhi1, phi, varPhi2) * SCTM31(varPhi1, phi, varPhi2) * 2;

        //    return ret;

        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="varPhi1"></param>
        ///// <param name="phi"></param>
        ///// <param name="varPhi2"></param>
        ///// <returns></returns>
        //public Stress.Microsopic.ElasticityTensors OrientedComplianceTensorHexagonalFDC33(double varPhi1, double phi, double varPhi2)
        //{
        //    Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

        //    ret.S11 = 0;
        //    ret.S33 = 0;
        //    ret.S12 = 0;
        //    ret.S13 = 0;
        //    ret.S44 = 0;

        //    ret.S11 += Math.Pow(SCTM13(varPhi1, phi, varPhi2), 4);

        //    ret.S33 += Math.Pow(SCTM33(varPhi1, phi, varPhi2), 4);

        //    ret.S12 += Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM23(varPhi1, phi, varPhi2), 2);

        //    ret.S13 += Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2);

        //    ret.S44 += Math.Pow(SCTM23(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2);

        //    return ret;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="varPhi1"></param>
        ///// <param name="phi"></param>
        ///// <param name="varPhi2"></param>
        ///// <returns></returns>
        //public Stress.Microsopic.ElasticityTensors OrientedComplianceTensorHexagonalFDC12(double varPhi1, double phi, double varPhi2)
        //{
        //    Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

        //    ret.S11 = 0;
        //    ret.S33 = 0;
        //    ret.S12 = 0;
        //    ret.S13 = 0;
        //    ret.S44 = 0;

        //    ret.S11 += 2 * Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2);
        //    ret.S11 += 4 * Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * -2;

        //    ret.S33 += 2 * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2);

        //    ret.S33 += 4 * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2) * -2;

        //    ret.S12 += Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM22(varPhi1, phi, varPhi2), 2);
        //    ret.S12 += Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2);

        //    ret.S12 += 4 * SCTM11(varPhi1, phi, varPhi2) * SCTM12(varPhi1, phi, varPhi2) * SCTM21(varPhi1, phi, varPhi2) * SCTM22(varPhi1, phi, varPhi2) * -2;

        //    ret.S13 += Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C12;
        //    ret.S13 += Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C12;
        //    ret.S13 += 4 * SCTM11(varPhi1, phi, varPhi2) * SCTM12(varPhi1, phi, varPhi2) * SCTM31(varPhi1, phi, varPhi2) * SCTM32(varPhi1, phi, varPhi2) * -2;

        //    ret.S44 += 2 * SCTM21(varPhi1, phi, varPhi2) * SCTM31(varPhi1, phi, varPhi2) * SCTM22(varPhi1, phi, varPhi2) * SCTM32(varPhi1, phi, varPhi2);

        //    ret.S44 += 2 * Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2) * this.BaseTensor.C66;
        //    ret.S44 += 2 * SCTM21(varPhi1, phi, varPhi2) * SCTM32(varPhi1, phi, varPhi2) * SCTM22(varPhi1, phi, varPhi2) * SCTM31(varPhi1, phi, varPhi2) * -2;

        //    return ret;

        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="varPhi1"></param>
        ///// <param name="phi"></param>
        ///// <param name="varPhi2"></param>
        ///// <returns></returns>
        //public Stress.Microsopic.ElasticityTensors OrientedComplianceTensorHexagonalFDC13(double varPhi1, double phi, double varPhi2)
        //{
        //    Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

        //    ret.S11 = 0;
        //    ret.S33 = 0;
        //    ret.S12 = 0;
        //    ret.S13 = 0;
        //    ret.S44 = 0;

        //    ret.S11 += 2 * Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2);
        //    ret.S11 += 2 * Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2);

        //    ret.S33 += 2 * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2);
        //    ret.S33 += 2 * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2);

        //    ret.S12 += Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM23(varPhi1, phi, varPhi2), 2);
        //    ret.S12 += Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2);

        //    ret.S12 += Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM23(varPhi1, phi, varPhi2), 2);
        //    ret.S12 += Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM22(varPhi1, phi, varPhi2), 2);

        //    ret.S13 += Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2);
        //    ret.S13 += Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2);

        //    ret.S13 += Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2);
        //    ret.S13 += Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2);

        //    ret.S44 += 2 * SCTM21(varPhi1, phi, varPhi2) * SCTM31(varPhi1, phi, varPhi2) * SCTM23(varPhi1, phi, varPhi2) * SCTM33(varPhi1, phi, varPhi2);
        //    ret.S44 += 2 * SCTM22(varPhi1, phi, varPhi2) * SCTM32(varPhi1, phi, varPhi2) * SCTM23(varPhi1, phi, varPhi2) * SCTM33(varPhi1, phi, varPhi2);

        //    return ret;

        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="varPhi1"></param>
        ///// <param name="phi"></param>
        ///// <param name="varPhi2"></param>
        ///// <returns></returns>
        //public Stress.Microsopic.ElasticityTensors OrientedComplianceTensorHexagonalFDC44(double varPhi1, double phi, double varPhi2)
        //{
        //    Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

        //    ret.S11 = 0;
        //    ret.S33 = 0;
        //    ret.S12 = 0;
        //    ret.S13 = 0;
        //    ret.S44 = 0;

        //    ret.S11 += 4 * Math.Pow(SCTM12(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2);
        //    ret.S11 += 4 * Math.Pow(SCTM11(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM13(varPhi1, phi, varPhi2), 2);

        //    ret.S33 += 4 * Math.Pow(SCTM32(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2);
        //    ret.S33 += 4 * Math.Pow(SCTM31(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2);

        //    ret.S12 += 4 * SCTM12(varPhi1, phi, varPhi2) * SCTM13(varPhi1, phi, varPhi2) * SCTM22(varPhi1, phi, varPhi2) * SCTM23(varPhi1, phi, varPhi2);
        //    ret.S12 += 4 * SCTM11(varPhi1, phi, varPhi2) * SCTM13(varPhi1, phi, varPhi2) * SCTM21(varPhi1, phi, varPhi2) * SCTM23(varPhi1, phi, varPhi2);

        //    ret.S13 += 4 * SCTM12(varPhi1, phi, varPhi2) * SCTM13(varPhi1, phi, varPhi2) * SCTM32(varPhi1, phi, varPhi2) * SCTM33(varPhi1, phi, varPhi2);
        //    ret.S13 += 4 * SCTM11(varPhi1, phi, varPhi2) * SCTM13(varPhi1, phi, varPhi2) * SCTM31(varPhi1, phi, varPhi2) * SCTM33(varPhi1, phi, varPhi2);

        //    ret.S44 += 2 * Math.Pow(SCTM22(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2);
        //    ret.S44 += 2 * SCTM23(varPhi1, phi, varPhi2) * SCTM32(varPhi1, phi, varPhi2) * SCTM22(varPhi1, phi, varPhi2) * SCTM33(varPhi1, phi, varPhi2);

        //    ret.S44 += 2 * Math.Pow(SCTM21(varPhi1, phi, varPhi2), 2) * Math.Pow(SCTM33(varPhi1, phi, varPhi2), 2);
        //    ret.S44 += 2 * SCTM21(varPhi1, phi, varPhi2) * SCTM33(varPhi1, phi, varPhi2) * SCTM23(varPhi1, phi, varPhi2) * SCTM31(varPhi1, phi, varPhi2);

        //    return ret;

        //}

        //#endregion

        //#endregion

        //#endregion

        #endregion

        #region None transposed stuff

        //#region Oriented elastic constants calculation

        ///// <summary>
        ///// Used For Voigt
        ///// </summary>
        ///// <param name="varPhi1"></param>
        ///// <param name="phi"></param>
        ///// <param name="varPhi2"></param>
        ///// <returns></returns>
        //public Stress.Microsopic.ElasticityTensors OrientedStiffnessTensorCubic(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        //{
        //    Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

        //    ret.C11 = 0;
        //    ret.C12 = 0;
        //    ret.C44 = 0;

        //    ret.C11 += Math.Pow(MCTM11(rotationAngle, hKL), 4) * this.BaseTensor.C11;
        //    ret.C11 += Math.Pow(MCTM12(rotationAngle, hKL), 4) * this.BaseTensor.C22;
        //    ret.C11 += Math.Pow(MCTM13(rotationAngle, hKL), 4) * this.BaseTensor.C33;

        //    ret.C11 += 2 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM12(rotationAngle, hKL), 2) * this.BaseTensor.C12;
        //    ret.C11 += 2 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2) * this.BaseTensor.C13;
        //    ret.C11 += 2 * Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2) * this.BaseTensor.C23;

        //    ret.C11 += 4 * Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2) * this.BaseTensor.C44;
        //    ret.C11 += 4 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2) * this.BaseTensor.C55;
        //    ret.C11 += 4 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM12(rotationAngle, hKL), 2) * this.BaseTensor.C66;

        //    ret.C12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM21(rotationAngle, hKL), 2) * this.BaseTensor.C11;
        //    ret.C12 += Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2) * this.BaseTensor.C22;
        //    ret.C12 += Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2) * this.BaseTensor.C33;

        //    ret.C12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2) * this.BaseTensor.C12;
        //    ret.C12 += Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM21(rotationAngle, hKL), 2) * this.BaseTensor.C12;

        //    ret.C12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2) * this.BaseTensor.C13;
        //    ret.C12 += Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2) * this.BaseTensor.C13;

        //    ret.C12 += Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2) * this.BaseTensor.C23;
        //    ret.C12 += Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2) * this.BaseTensor.C23;

        //    ret.C12 += 4 * MCTM12(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * this.BaseTensor.C44;
        //    ret.C12 += 4 * MCTM11(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM21(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * this.BaseTensor.C55;
        //    ret.C12 += 4 * MCTM11(rotationAngle, hKL) * MCTM12(rotationAngle, hKL) * MCTM21(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * this.BaseTensor.C66;

        //    ret.C44 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2) * this.BaseTensor.C11;
        //    ret.C44 += Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2) * this.BaseTensor.C22;
        //    ret.C44 += Math.Pow(MCTM23(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.C33;

        //    ret.C44 += 2 * MCTM21(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * this.BaseTensor.C12;
        //    ret.C44 += 2 * MCTM21(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * this.BaseTensor.C13;
        //    ret.C44 += 2 * MCTM22(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * this.BaseTensor.C23;

        //    ret.C44 += Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.C44;
        //    ret.C44 += 2 * MCTM23(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * this.BaseTensor.C44;
        //    ret.C44 += Math.Pow(MCTM23(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2) * this.BaseTensor.C44;

        //    ret.C44 += Math.Pow(MCTM23(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2) * this.BaseTensor.C55;
        //    ret.C44 += 2 * MCTM21(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * this.BaseTensor.C55;
        //    ret.C44 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.C55;

        //    ret.C44 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2) * this.BaseTensor.C66;
        //    ret.C44 += 2 * MCTM21(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * this.BaseTensor.C66;
        //    ret.C44 += Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2) * this.BaseTensor.C66;

        //    ret.CalculateCompliances();

        //    return ret;

        //}

        ///// <summary>
        ///// Used for reuss
        ///// </summary>
        ///// <param name="varPhi1"></param>
        ///// <param name="phi"></param>
        ///// <param name="varPhi2"></param>
        ///// <returns></returns>
        //public Stress.Microsopic.ElasticityTensors OrientedComplianceTensorCubic(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        //{
        //    Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

        //    ret.S11 = 0;
        //    ret.S12 = 0;
        //    ret.S44 = 0;

        //    ret.S11 += Math.Pow(MCTM11(rotationAngle, hKL), 4) * this.BaseTensor.S11;
        //    ret.S11 += Math.Pow(MCTM12(rotationAngle, hKL), 4) * this.BaseTensor.S22;
        //    ret.S11 += Math.Pow(MCTM13(rotationAngle, hKL), 4) * this.BaseTensor.S33;

        //    ret.S11 += 2 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM12(rotationAngle, hKL), 2) * this.BaseTensor.S12;
        //    ret.S11 += 2 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2) * this.BaseTensor.S13;
        //    ret.S11 += 2 * Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2) * this.BaseTensor.S23;

        //    ret.S11 += 4 * Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2) * this.BaseTensor.S44;
        //    ret.S11 += 4 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2) * this.BaseTensor.S55;
        //    ret.S11 += 4 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM12(rotationAngle, hKL), 2) * this.BaseTensor.S66;

        //    ret.S12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM21(rotationAngle, hKL), 2) * this.BaseTensor.S11;
        //    ret.S12 += Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2) * this.BaseTensor.S22;
        //    ret.S12 += Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2) * this.BaseTensor.S33;

        //    ret.S12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2) * this.BaseTensor.S12;
        //    ret.S12 += Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM21(rotationAngle, hKL), 2) * this.BaseTensor.S12;

        //    ret.S12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2) * this.BaseTensor.S13;
        //    ret.S12 += Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2) * this.BaseTensor.S13;

        //    ret.S12 += Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2) * this.BaseTensor.S23;
        //    ret.S12 += Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2) * this.BaseTensor.S23;

        //    ret.S12 += 4 * MCTM12(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * this.BaseTensor.S44;
        //    ret.S12 += 4 * MCTM11(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM21(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * this.BaseTensor.S55;
        //    ret.S12 += 4 * MCTM11(rotationAngle, hKL) * MCTM12(rotationAngle, hKL) * MCTM21(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * this.BaseTensor.S66;

        //    ret.S44 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2) * this.BaseTensor.S11;
        //    ret.S44 += Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2) * this.BaseTensor.S22;
        //    ret.S44 += Math.Pow(MCTM23(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.S33;

        //    ret.S44 += 2 * MCTM21(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * this.BaseTensor.S12;
        //    ret.S44 += 2 * MCTM21(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * this.BaseTensor.S13;
        //    ret.S44 += 2 * MCTM22(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * this.BaseTensor.S23;

        //    ret.S44 += Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.S44;
        //    ret.S44 += 2 * MCTM23(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * this.BaseTensor.S44;
        //    ret.S44 += Math.Pow(MCTM23(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2) * this.BaseTensor.S44;

        //    ret.S44 += Math.Pow(MCTM23(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2) * this.BaseTensor.S55;
        //    ret.S44 += 2 * MCTM21(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * this.BaseTensor.S55;
        //    ret.S44 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.S55;

        //    ret.S44 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2) * this.BaseTensor.S66;
        //    ret.S44 += 2 * MCTM21(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * this.BaseTensor.S66;
        //    ret.S44 += Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2) * this.BaseTensor.S66;

        //    ret.CalculateStiffnesses();

        //    return ret;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="varPhi1"></param>
        ///// <param name="phi"></param>
        ///// <param name="varPhi2"></param>
        ///// <returns></returns>
        //public Stress.Microsopic.ElasticityTensors OrientedComplianceTensorHexagonal(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        //{
        //    Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

        //    ret.S11 = 0;
        //    ret.S33 = 0;
        //    ret.S12 = 0;
        //    ret.S13 = 0;
        //    ret.S44 = 0;

        //    ret.S11 += Math.Pow(MCTM11(rotationAngle, hKL), 4) * this.BaseTensor.S11;
        //    ret.S11 += Math.Pow(MCTM12(rotationAngle, hKL), 4) * this.BaseTensor.S22;
        //    ret.S11 += Math.Pow(MCTM13(rotationAngle, hKL), 4) * this.BaseTensor.S33;

        //    ret.S11 += 2 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM12(rotationAngle, hKL), 2) * this.BaseTensor.S12;
        //    ret.S11 += 2 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2) * this.BaseTensor.S13;
        //    ret.S11 += 2 * Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2) * this.BaseTensor.S23;

        //    ret.S11 += 4 * Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2) * this.BaseTensor.S44;
        //    ret.S11 += 4 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2) * this.BaseTensor.S55;
        //    ret.S11 += 4 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM12(rotationAngle, hKL), 2) * this.BaseTensor.S66;

        //    ret.S33 += Math.Pow(MCTM31(rotationAngle, hKL), 4) * this.BaseTensor.S11;
        //    ret.S33 += Math.Pow(MCTM32(rotationAngle, hKL), 4) * this.BaseTensor.S22;
        //    ret.S33 += Math.Pow(MCTM33(rotationAngle, hKL), 4) * this.BaseTensor.S33;

        //    ret.S33 += 2 * Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2) * this.BaseTensor.S12;
        //    ret.S33 += 2 * Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.S13;
        //    ret.S33 += 2 * Math.Pow(MCTM32(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.S23;

        //    ret.S33 += 4 * Math.Pow(MCTM32(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.S44;
        //    ret.S33 += 4 * Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.S55;
        //    ret.S33 += 4 * Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2) * this.BaseTensor.S66;

        //    ret.S12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM21(rotationAngle, hKL), 2) * this.BaseTensor.S11;
        //    ret.S12 += Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2) * this.BaseTensor.S22;
        //    ret.S12 += Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2) * this.BaseTensor.S33;

        //    ret.S12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2) * this.BaseTensor.S12;
        //    ret.S12 += Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM21(rotationAngle, hKL), 2) * this.BaseTensor.S12;

        //    ret.S12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2) * this.BaseTensor.S13;
        //    ret.S12 += Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM21(rotationAngle, hKL), 2) * this.BaseTensor.S13;

        //    ret.S12 += Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2) * this.BaseTensor.S23;
        //    ret.S12 += Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2) * this.BaseTensor.S23;

        //    ret.S12 += 4 * MCTM12(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * this.BaseTensor.S44;
        //    ret.S12 += 4 * MCTM11(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM21(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * this.BaseTensor.S55;
        //    ret.S12 += 4 * MCTM11(rotationAngle, hKL) * MCTM12(rotationAngle, hKL) * MCTM21(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * this.BaseTensor.S66;

        //    ret.S13 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2) * this.BaseTensor.S11;
        //    ret.S13 += Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2) * this.BaseTensor.S22;
        //    ret.S13 += Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.S33;

        //    ret.S13 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2) * this.BaseTensor.S12;
        //    ret.S13 += Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2) * this.BaseTensor.S12;

        //    ret.S13 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.S13;
        //    ret.S13 += Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2) * this.BaseTensor.S13;

        //    ret.S13 += Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.S23;
        //    ret.S13 += Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2) * this.BaseTensor.S23;

        //    ret.S13 += 4 * MCTM12(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * this.BaseTensor.S44;
        //    ret.S13 += 4 * MCTM11(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * this.BaseTensor.S55;
        //    ret.S13 += 4 * MCTM11(rotationAngle, hKL) * MCTM12(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * this.BaseTensor.S66;

        //    ret.S44 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2) * this.BaseTensor.S11;
        //    ret.S44 += Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2) * this.BaseTensor.S22;
        //    ret.S44 += Math.Pow(MCTM23(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.S33;

        //    ret.S44 += 2 * MCTM21(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * this.BaseTensor.S12;
        //    ret.S44 += 2 * MCTM21(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * this.BaseTensor.S13;
        //    ret.S44 += 2 * MCTM22(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * this.BaseTensor.S23;

        //    ret.S44 += 2 * Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.S44;
        //    ret.S44 += 2 * MCTM23(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * this.BaseTensor.S44;

        //    ret.S44 += 2 * Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.S55;
        //    ret.S44 += 2 * MCTM21(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * this.BaseTensor.S55;

        //    ret.S44 += 2 * Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2) * this.BaseTensor.S66;
        //    ret.S44 += 2 * MCTM21(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * this.BaseTensor.S66;

        //    return ret;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="varPhi1"></param>
        ///// <param name="phi"></param>
        ///// <param name="varPhi2"></param>
        ///// <returns></returns>
        //public Stress.Microsopic.ElasticityTensors OrientedStiffnessTensorHexagonal(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        //{
        //    Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

        //    ret.C11 = 0;
        //    ret.C33 = 0;
        //    ret.C12 = 0;
        //    ret.C13 = 0;
        //    ret.C44 = 0;

        //    ret.C11 += Math.Pow(MCTM11(rotationAngle, hKL), 4) * this.BaseTensor.C11;
        //    ret.C11 += Math.Pow(MCTM12(rotationAngle, hKL), 4) * this.BaseTensor.C22;
        //    ret.C11 += Math.Pow(MCTM13(rotationAngle, hKL), 4) * this.BaseTensor.C33;

        //    ret.C11 += 2 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM12(rotationAngle, hKL), 2) * this.BaseTensor.C12;
        //    ret.C11 += 2 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2) * this.BaseTensor.C13;
        //    ret.C11 += 2 * Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2) * this.BaseTensor.C23;

        //    ret.C11 += 4 * Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2) * this.BaseTensor.C44;
        //    ret.C11 += 4 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2) * this.BaseTensor.C55;
        //    ret.C11 += 4 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM12(rotationAngle, hKL), 2) * this.BaseTensor.C66;

        //    ret.C33 += Math.Pow(MCTM31(rotationAngle, hKL), 4) * this.BaseTensor.C11;
        //    ret.C33 += Math.Pow(MCTM32(rotationAngle, hKL), 4) * this.BaseTensor.C22;
        //    ret.C33 += Math.Pow(MCTM33(rotationAngle, hKL), 4) * this.BaseTensor.C33;

        //    ret.C33 += 2 * Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2) * this.BaseTensor.C12;
        //    ret.C33 += 2 * Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.C13;
        //    ret.C33 += 2 * Math.Pow(MCTM32(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.C23;

        //    ret.C33 += 4 * Math.Pow(MCTM32(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.C44;
        //    ret.C33 += 4 * Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.C55;
        //    ret.C33 += 4 * Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2) * this.BaseTensor.C66;

        //    ret.C12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM21(rotationAngle, hKL), 2) * this.BaseTensor.C11;
        //    ret.C12 += Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2) * this.BaseTensor.C22;
        //    ret.C12 += Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2) * this.BaseTensor.C33;

        //    ret.C12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2) * this.BaseTensor.C12;
        //    ret.C12 += Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM21(rotationAngle, hKL), 2) * this.BaseTensor.C12;

        //    ret.C12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2) * this.BaseTensor.C13;
        //    ret.C12 += Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM21(rotationAngle, hKL), 2) * this.BaseTensor.C13;

        //    ret.C12 += Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2) * this.BaseTensor.C23;
        //    ret.C12 += Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2) * this.BaseTensor.C23;

        //    ret.C12 += 4 * MCTM12(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * this.BaseTensor.C44;
        //    ret.C12 += 4 * MCTM11(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM21(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * this.BaseTensor.C55;
        //    ret.C12 += 4 * MCTM11(rotationAngle, hKL) * MCTM12(rotationAngle, hKL) * MCTM21(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * this.BaseTensor.C66;

        //    ret.C13 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2) * this.BaseTensor.C11;
        //    ret.C13 += Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2) * this.BaseTensor.C22;
        //    ret.C13 += Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.C33;

        //    ret.C13 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2) * this.BaseTensor.C12;
        //    ret.C13 += Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2) * this.BaseTensor.C12;

        //    ret.C13 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.C13;
        //    ret.C13 += Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2) * this.BaseTensor.C13;

        //    ret.C13 += Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.C23;
        //    ret.C13 += Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2) * this.BaseTensor.C23;

        //    ret.C13 += 4 * MCTM12(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * this.BaseTensor.C44;
        //    ret.C13 += 4 * MCTM11(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * this.BaseTensor.C55;
        //    ret.C13 += 4 * MCTM11(rotationAngle, hKL) * MCTM12(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * this.BaseTensor.C66;

        //    ret.C44 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2) * this.BaseTensor.C11;
        //    ret.C44 += Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2) * this.BaseTensor.C22;
        //    ret.C44 += Math.Pow(MCTM23(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.C33;

        //    ret.C44 += 2 * MCTM21(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * this.BaseTensor.C12;
        //    ret.C44 += 2 * MCTM21(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * this.BaseTensor.C13;
        //    ret.C44 += 2 * MCTM22(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * this.BaseTensor.C23;

        //    ret.C44 += 2 * Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.C44;
        //    ret.C44 += 2 * MCTM23(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * this.BaseTensor.C44;

        //    ret.C44 += 2 * Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.C55;
        //    ret.C44 += 2 * MCTM21(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * this.BaseTensor.C55;

        //    ret.C44 += 2 * Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2) * this.BaseTensor.C66;
        //    ret.C44 += 2 * MCTM21(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * this.BaseTensor.C66;

        //    return ret;

        //}

        //#region First derivatives

        //#region Cubic

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="varPhi1"></param>
        ///// <param name="phi"></param>
        ///// <param name="varPhi2"></param>
        ///// <returns></returns>
        //public Stress.Microsopic.ElasticityTensors OrientedStiffnessTensorCubicFDC11(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        //{
        //    Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

        //    ret.C11 = 0;
        //    ret.C12 = 0;
        //    ret.C44 = 0;

        //    ret.C11 += Math.Pow(MCTM11(rotationAngle, hKL), 4);
        //    ret.C11 += Math.Pow(MCTM12(rotationAngle, hKL), 4);
        //    ret.C11 += Math.Pow(MCTM13(rotationAngle, hKL), 4);

        //    ret.C12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM21(rotationAngle, hKL), 2);
        //    ret.C12 += Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2);
        //    ret.C12 += Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2);

        //    ret.C44 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2);
        //    ret.C44 += Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2);
        //    ret.C44 += Math.Pow(MCTM23(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);

        //    Stress.Microsopic.ElasticityTensors InvertedOrigTensor = this.OrientedStiffnessTensorCubic(rotationAngle, hKL);

        //    ret._complianceTensor = -1.0 * InvertedOrigTensor._complianceTensor * ret._stiffnessTensor * InvertedOrigTensor._complianceTensor;

        //    ret.CalculateStiffnesses();

        //    return ret;

        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="varPhi1"></param>
        ///// <param name="phi"></param>
        ///// <param name="varPhi2"></param>
        ///// <returns></returns>
        //public Stress.Microsopic.ElasticityTensors OrientedStiffnessTensorCubicFDC12(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        //{
        //    Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

        //    ret.C11 = 0;
        //    ret.C12 = 0;
        //    ret.C44 = 0;

        //    ret.C11 += 2 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM12(rotationAngle, hKL), 2);
        //    ret.C11 += 2 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2);
        //    ret.C11 += 2 * Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2);

        //    ret.C12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2);
        //    ret.C12 += Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM21(rotationAngle, hKL), 2);

        //    ret.C12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2);
        //    ret.C12 += Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2);

        //    ret.C12 += Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2);
        //    ret.C12 += Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2);

        //    ret.C44 += 2 * MCTM21(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM32(rotationAngle, hKL);
        //    ret.C44 += 2 * MCTM21(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM33(rotationAngle, hKL);
        //    ret.C44 += 2 * MCTM22(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM33(rotationAngle, hKL);

        //    Stress.Microsopic.ElasticityTensors InvertedOrigTensor = this.OrientedStiffnessTensorCubic(rotationAngle, hKL);

        //    ret._complianceTensor = -1.0 * InvertedOrigTensor._complianceTensor * ret._stiffnessTensor * InvertedOrigTensor._complianceTensor;

        //    ret.CalculateStiffnesses();
        //    return ret;

        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="varPhi1"></param>
        ///// <param name="phi"></param>
        ///// <param name="varPhi2"></param>
        ///// <returns></returns>
        //public Stress.Microsopic.ElasticityTensors OrientedStiffnessTensorCubicFDC44(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        //{
        //    Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

        //    ret.C11 = 0;
        //    ret.C12 = 0;
        //    ret.C44 = 0;

        //    ret.C11 += 4 * Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2);
        //    ret.C11 += 4 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2);
        //    ret.C11 += 4 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM12(rotationAngle, hKL), 2);

        //    ret.C12 += 4 * MCTM12(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM23(rotationAngle, hKL);
        //    ret.C12 += 4 * MCTM11(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM21(rotationAngle, hKL) * MCTM23(rotationAngle, hKL);
        //    ret.C12 += 4 * MCTM11(rotationAngle, hKL) * MCTM12(rotationAngle, hKL) * MCTM21(rotationAngle, hKL) * MCTM22(rotationAngle, hKL);

        //    ret.C44 += Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);
        //    ret.C44 += 2 * MCTM23(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM33(rotationAngle, hKL);
        //    ret.C44 += Math.Pow(MCTM23(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2);

        //    ret.C44 += Math.Pow(MCTM23(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2);
        //    ret.C44 += 2 * MCTM21(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM31(rotationAngle, hKL);
        //    ret.C44 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);

        //    ret.C44 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2);
        //    ret.C44 += 2 * MCTM21(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM31(rotationAngle, hKL);
        //    ret.C44 += Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2);

        //    Stress.Microsopic.ElasticityTensors InvertedOrigTensor = this.OrientedStiffnessTensorCubic(rotationAngle, hKL);

        //    ret._complianceTensor = -1.0 * InvertedOrigTensor._complianceTensor * ret._stiffnessTensor * InvertedOrigTensor._complianceTensor;

        //    ret.CalculateStiffnesses();

        //    return ret;

        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="varPhi1"></param>
        ///// <param name="phi"></param>
        ///// <param name="varPhi2"></param>
        ///// <returns></returns>
        //public Stress.Microsopic.ElasticityTensors OrientedComplianceTensorCubicFDS11(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        //{
        //    Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

        //    ret.S11 = 0;
        //    ret.S12 = 0;
        //    ret.S44 = 0;

        //    ret.S11 += Math.Pow(MCTM11(rotationAngle, hKL), 4);
        //    ret.S11 += Math.Pow(MCTM12(rotationAngle, hKL), 4);
        //    ret.S11 += Math.Pow(MCTM13(rotationAngle, hKL), 4);

        //    ret.S12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM21(rotationAngle, hKL), 2);
        //    ret.S12 += Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2);
        //    ret.S12 += Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2);

        //    ret.S44 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2);
        //    ret.S44 += Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2);
        //    ret.S44 += Math.Pow(MCTM23(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);

        //    ret.CalculateStiffnesses();

        //    return ret;

        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="varPhi1"></param>
        ///// <param name="phi"></param>
        ///// <param name="varPhi2"></param>
        ///// <returns></returns>
        //public Stress.Microsopic.ElasticityTensors OrientedComplianceTensorCubicFDS12(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        //{
        //    Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

        //    ret.S11 = 0;
        //    ret.S12 = 0;
        //    ret.S44 = 0;

        //    ret.S11 += 2 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM12(rotationAngle, hKL), 2);
        //    ret.S11 += 2 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2);
        //    ret.S11 += 2 * Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2);

        //    ret.S12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2);
        //    ret.S12 += Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM21(rotationAngle, hKL), 2);

        //    ret.S12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2);
        //    ret.S12 += Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2);

        //    ret.S12 += Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2);
        //    ret.S12 += Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2);

        //    ret.S44 += 2 * MCTM21(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM32(rotationAngle, hKL);
        //    ret.S44 += 2 * MCTM21(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM33(rotationAngle, hKL);
        //    ret.S44 += 2 * MCTM22(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM33(rotationAngle, hKL);

        //    ret.CalculateStiffnesses();
        //    return ret;

        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="varPhi1"></param>
        ///// <param name="phi"></param>
        ///// <param name="varPhi2"></param>
        ///// <returns></returns>
        //public Stress.Microsopic.ElasticityTensors OrientedComplianceTensorCubicFDS44(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        //{
        //    Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

        //    ret.S11 = 0;
        //    ret.S12 = 0;
        //    ret.S44 = 0;

        //    ret.S11 += 4 * Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2);
        //    ret.S11 += 4 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2);
        //    ret.S11 += 4 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM12(rotationAngle, hKL), 2);

        //    ret.S12 += 4 * MCTM12(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM23(rotationAngle, hKL);
        //    ret.S12 += 4 * MCTM11(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM21(rotationAngle, hKL) * MCTM23(rotationAngle, hKL);
        //    ret.S12 += 4 * MCTM11(rotationAngle, hKL) * MCTM12(rotationAngle, hKL) * MCTM21(rotationAngle, hKL) * MCTM22(rotationAngle, hKL);

        //    ret.S44 += Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);
        //    ret.S44 += 2 * MCTM23(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM33(rotationAngle, hKL);
        //    ret.S44 += Math.Pow(MCTM23(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2);

        //    ret.S44 += Math.Pow(MCTM23(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2);
        //    ret.S44 += 2 * MCTM21(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM31(rotationAngle, hKL);
        //    ret.S44 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);

        //    ret.S44 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2);
        //    ret.S44 += 2 * MCTM21(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM31(rotationAngle, hKL);
        //    ret.S44 += Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2);

        //    ret.CalculateStiffnesses();

        //    return ret;
        //}

        //#endregion

        //#region Hexagonal

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="varPhi1"></param>
        ///// <param name="phi"></param>
        ///// <param name="varPhi2"></param>
        ///// <returns></returns>
        //public Stress.Microsopic.ElasticityTensors OrientedStiffnessTensorHexagonalFDC11(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        //{
        //    Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

        //    ret.C11 = 0;
        //    ret.C33 = 0;
        //    ret.C12 = 0;
        //    ret.C13 = 0;
        //    ret.C44 = 0;

        //    ret.C11 += Math.Pow(MCTM11(rotationAngle, hKL), 4);
        //    ret.C11 += Math.Pow(MCTM12(rotationAngle, hKL), 4);
        //    ret.C11 += 4 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM12(rotationAngle, hKL), 2) * 2;

        //    ret.C33 += Math.Pow(MCTM31(rotationAngle, hKL), 4);
        //    ret.C33 += Math.Pow(MCTM32(rotationAngle, hKL), 4);
        //    ret.C33 += 4 * Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2) * 2;

        //    ret.C12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM21(rotationAngle, hKL), 2);
        //    ret.C12 += Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2);
        //    ret.C12 += 4 * MCTM11(rotationAngle, hKL) * MCTM12(rotationAngle, hKL) * MCTM21(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * 2;

        //    ret.C13 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2);
        //    ret.C13 += Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2);
        //    ret.C13 += 4 * MCTM11(rotationAngle, hKL) * MCTM12(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * 2;

        //    ret.C44 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2);
        //    ret.C44 += Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2);

        //    ret.C44 += 2 * Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2) * 2;
        //    ret.C44 += 2 * MCTM21(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * 2;

        //    return ret;

        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="varPhi1"></param>
        ///// <param name="phi"></param>
        ///// <param name="varPhi2"></param>
        ///// <returns></returns>
        //public Stress.Microsopic.ElasticityTensors OrientedStiffnessTensorHexagonalFDC33(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        //{
        //    Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

        //    ret.C11 = 0;
        //    ret.C33 = 0;
        //    ret.C12 = 0;
        //    ret.C13 = 0;
        //    ret.C44 = 0;

        //    ret.C11 += Math.Pow(MCTM13(rotationAngle, hKL), 4);

        //    ret.C33 += Math.Pow(MCTM33(rotationAngle, hKL), 4);

        //    ret.C12 += Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2);

        //    ret.C13 += Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);

        //    ret.C44 += Math.Pow(MCTM23(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);

        //    return ret;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="varPhi1"></param>
        ///// <param name="phi"></param>
        ///// <param name="varPhi2"></param>
        ///// <returns></returns>
        //public Stress.Microsopic.ElasticityTensors OrientedStiffnessTensorHexagonalFDC12(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        //{
        //    Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

        //    ret.C11 = 0;
        //    ret.C33 = 0;
        //    ret.C12 = 0;
        //    ret.C13 = 0;
        //    ret.C44 = 0;

        //    ret.C11 += 2 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM12(rotationAngle, hKL), 2);
        //    ret.C11 += 4 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM12(rotationAngle, hKL), 2) * -2;

        //    ret.C33 += 2 * Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2);

        //    ret.C33 += 4 * Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2) * -2;

        //    ret.C12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2);
        //    ret.C12 += Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM21(rotationAngle, hKL), 2);

        //    ret.C12 += 4 * MCTM11(rotationAngle, hKL) * MCTM12(rotationAngle, hKL) * MCTM21(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * -2;

        //    ret.C13 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2) * this.BaseTensor.C12;
        //    ret.C13 += Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2) * this.BaseTensor.C12;
        //    ret.C13 += 4 * MCTM11(rotationAngle, hKL) * MCTM12(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * -2;

        //    ret.C44 += 2 * MCTM21(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM32(rotationAngle, hKL);

        //    ret.C44 += 2 * Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2) * this.BaseTensor.C66;
        //    ret.C44 += 2 * MCTM21(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * -2;

        //    return ret;

        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="varPhi1"></param>
        ///// <param name="phi"></param>
        ///// <param name="varPhi2"></param>
        ///// <returns></returns>
        //public Stress.Microsopic.ElasticityTensors OrientedStiffnessTensorHexagonalFDC13(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        //{
        //    Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

        //    ret.C11 = 0;
        //    ret.C33 = 0;
        //    ret.C12 = 0;
        //    ret.C13 = 0;
        //    ret.C44 = 0;

        //    ret.C11 += 2 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2);
        //    ret.C11 += 2 * Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2);

        //    ret.C33 += 2 * Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);
        //    ret.C33 += 2 * Math.Pow(MCTM32(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);

        //    ret.C12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2);
        //    ret.C12 += Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM21(rotationAngle, hKL), 2);

        //    ret.C12 += Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2);
        //    ret.C12 += Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2);

        //    ret.C13 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);
        //    ret.C13 += Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2);

        //    ret.C13 += Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);
        //    ret.C13 += Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2);

        //    ret.C44 += 2 * MCTM21(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM33(rotationAngle, hKL);
        //    ret.C44 += 2 * MCTM22(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM33(rotationAngle, hKL);

        //    return ret;

        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="varPhi1"></param>
        ///// <param name="phi"></param>
        ///// <param name="varPhi2"></param>
        ///// <returns></returns>
        //public Stress.Microsopic.ElasticityTensors OrientedStiffnessTensorHexagonalFDC44(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        //{
        //    Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

        //    ret.C11 = 0;
        //    ret.C33 = 0;
        //    ret.C12 = 0;
        //    ret.C13 = 0;
        //    ret.C44 = 0;

        //    ret.C11 += 4 * Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2);
        //    ret.C11 += 4 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2);

        //    ret.C33 += 4 * Math.Pow(MCTM32(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);
        //    ret.C33 += 4 * Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);

        //    ret.C12 += 4 * MCTM12(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM23(rotationAngle, hKL);
        //    ret.C12 += 4 * MCTM11(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM21(rotationAngle, hKL) * MCTM23(rotationAngle, hKL);

        //    ret.C13 += 4 * MCTM12(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM33(rotationAngle, hKL);
        //    ret.C13 += 4 * MCTM11(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM33(rotationAngle, hKL);

        //    ret.C44 += 2 * Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);
        //    ret.C44 += 2 * MCTM23(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM33(rotationAngle, hKL);

        //    ret.C44 += 2 * Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);
        //    ret.C44 += 2 * MCTM21(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM31(rotationAngle, hKL);

        //    return ret;

        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="varPhi1"></param>
        ///// <param name="phi"></param>
        ///// <param name="varPhi2"></param>
        ///// <returns></returns>
        //public Stress.Microsopic.ElasticityTensors OrientedComplianceTensorHexagonalFDC11(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        //{
        //    Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

        //    ret.S11 = 0;
        //    ret.S33 = 0;
        //    ret.S12 = 0;
        //    ret.S13 = 0;
        //    ret.S44 = 0;

        //    ret.S11 += Math.Pow(MCTM11(rotationAngle, hKL), 4);
        //    ret.S11 += Math.Pow(MCTM12(rotationAngle, hKL), 4);
        //    ret.S11 += 4 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM12(rotationAngle, hKL), 2) * 2;

        //    ret.S33 += Math.Pow(MCTM31(rotationAngle, hKL), 4);
        //    ret.S33 += Math.Pow(MCTM32(rotationAngle, hKL), 4);
        //    ret.S33 += 4 * Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2) * 2;

        //    ret.S12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM21(rotationAngle, hKL), 2);
        //    ret.S12 += Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2);
        //    ret.S12 += 4 * MCTM11(rotationAngle, hKL) * MCTM12(rotationAngle, hKL) * MCTM21(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * 2;

        //    ret.S13 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2);
        //    ret.S13 += Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2);
        //    ret.S13 += 4 * MCTM11(rotationAngle, hKL) * MCTM12(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * 2;

        //    ret.S44 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2);
        //    ret.S44 += Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2);

        //    ret.S44 += 2 * Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2) * 2;
        //    ret.S44 += 2 * MCTM21(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * 2;

        //    return ret;

        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="varPhi1"></param>
        ///// <param name="phi"></param>
        ///// <param name="varPhi2"></param>
        ///// <returns></returns>
        //public Stress.Microsopic.ElasticityTensors OrientedComplianceTensorHexagonalFDC33(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        //{
        //    Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

        //    ret.S11 = 0;
        //    ret.S33 = 0;
        //    ret.S12 = 0;
        //    ret.S13 = 0;
        //    ret.S44 = 0;

        //    ret.S11 += Math.Pow(MCTM13(rotationAngle, hKL), 4);

        //    ret.S33 += Math.Pow(MCTM33(rotationAngle, hKL), 4);

        //    ret.S12 += Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2);

        //    ret.S13 += Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);

        //    ret.S44 += Math.Pow(MCTM23(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);

        //    return ret;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="varPhi1"></param>
        ///// <param name="phi"></param>
        ///// <param name="varPhi2"></param>
        ///// <returns></returns>
        //public Stress.Microsopic.ElasticityTensors OrientedComplianceTensorHexagonalFDC12(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        //{
        //    Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

        //    ret.S11 = 0;
        //    ret.S33 = 0;
        //    ret.S12 = 0;
        //    ret.S13 = 0;
        //    ret.S44 = 0;

        //    ret.S11 += 2 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM12(rotationAngle, hKL), 2);
        //    ret.S11 += 4 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM12(rotationAngle, hKL), 2) * -2;

        //    ret.S33 += 2 * Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2);

        //    ret.S33 += 4 * Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2) * -2;

        //    ret.S12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2);
        //    ret.S12 += Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM21(rotationAngle, hKL), 2);

        //    ret.S12 += 4 * MCTM11(rotationAngle, hKL) * MCTM12(rotationAngle, hKL) * MCTM21(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * -2;

        //    ret.S13 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2) * this.BaseTensor.C12;
        //    ret.S13 += Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2) * this.BaseTensor.C12;
        //    ret.S13 += 4 * MCTM11(rotationAngle, hKL) * MCTM12(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * -2;

        //    ret.S44 += 2 * MCTM21(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM32(rotationAngle, hKL);

        //    ret.S44 += 2 * Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2) * this.BaseTensor.C66;
        //    ret.S44 += 2 * MCTM21(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * -2;

        //    return ret;

        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="varPhi1"></param>
        ///// <param name="phi"></param>
        ///// <param name="varPhi2"></param>
        ///// <returns></returns>
        //public Stress.Microsopic.ElasticityTensors OrientedComplianceTensorHexagonalFDC13(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        //{
        //    Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

        //    ret.S11 = 0;
        //    ret.S33 = 0;
        //    ret.S12 = 0;
        //    ret.S13 = 0;
        //    ret.S44 = 0;

        //    ret.S11 += 2 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2);
        //    ret.S11 += 2 * Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2);

        //    ret.S33 += 2 * Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);
        //    ret.S33 += 2 * Math.Pow(MCTM32(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);

        //    ret.S12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2);
        //    ret.S12 += Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM21(rotationAngle, hKL), 2);

        //    ret.S12 += Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2);
        //    ret.S12 += Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2);

        //    ret.S13 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);
        //    ret.S13 += Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2);

        //    ret.S13 += Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);
        //    ret.S13 += Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2);

        //    ret.S44 += 2 * MCTM21(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM33(rotationAngle, hKL);
        //    ret.S44 += 2 * MCTM22(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM33(rotationAngle, hKL);

        //    return ret;

        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="varPhi1"></param>
        ///// <param name="phi"></param>
        ///// <param name="varPhi2"></param>
        ///// <returns></returns>
        //public Stress.Microsopic.ElasticityTensors OrientedComplianceTensorHexagonalFDC44(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        //{
        //    Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

        //    ret.S11 = 0;
        //    ret.S33 = 0;
        //    ret.S12 = 0;
        //    ret.S13 = 0;
        //    ret.S44 = 0;

        //    ret.S11 += 4 * Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2);
        //    ret.S11 += 4 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2);

        //    ret.S33 += 4 * Math.Pow(MCTM32(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);
        //    ret.S33 += 4 * Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);

        //    ret.S12 += 4 * MCTM12(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM23(rotationAngle, hKL);
        //    ret.S12 += 4 * MCTM11(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM21(rotationAngle, hKL) * MCTM23(rotationAngle, hKL);

        //    ret.S13 += 4 * MCTM12(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM33(rotationAngle, hKL);
        //    ret.S13 += 4 * MCTM11(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM33(rotationAngle, hKL);

        //    ret.S44 += 2 * Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);
        //    ret.S44 += 2 * MCTM23(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM33(rotationAngle, hKL);

        //    ret.S44 += 2 * Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);
        //    ret.S44 += 2 * MCTM21(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM31(rotationAngle, hKL);

        //    return ret;

        //}

        //#endregion

        //#endregion

        //#endregion

        #endregion

        #region Oriented elastic constants calculation

        /// <summary>
        /// Used For Voigt
        /// </summary>
        /// <param name="varPhi1"></param>
        /// <param name="phi"></param>
        /// <param name="varPhi2"></param>
        /// <returns></returns>
        public Stress.Microsopic.ElasticityTensors OrientedStiffnessTensorCubic(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        {
            Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

            ret.C11 = 0;
            ret.C12 = 0;
            ret.C44 = 0;

            ret.C11 += Math.Pow(MCTM11(rotationAngle, hKL), 4) * this.BaseTensor.C11;
            ret.C11 += Math.Pow(MCTM21(rotationAngle, hKL), 4) * this.BaseTensor.C22;
            ret.C11 += Math.Pow(MCTM31(rotationAngle, hKL), 4) * this.BaseTensor.C33;

            ret.C11 += 2 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM21(rotationAngle, hKL), 2) * this.BaseTensor.C12;
            ret.C11 += 2 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2) * this.BaseTensor.C13;
            ret.C11 += 2 * Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2) * this.BaseTensor.C23;

            ret.C11 += 0.25 * 4 * Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2) * this.BaseTensor.C44;
            ret.C11 += 0.25 * 4 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2) * this.BaseTensor.C55;
            ret.C11 += 0.25 * 4 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM21(rotationAngle, hKL), 2) * this.BaseTensor.C66;

            ret.C12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM12(rotationAngle, hKL), 2) * this.BaseTensor.C11;
            ret.C12 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2) * this.BaseTensor.C22;
            ret.C12 += Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2) * this.BaseTensor.C33;

            ret.C12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2) * this.BaseTensor.C12;
            ret.C12 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM12(rotationAngle, hKL), 2) * this.BaseTensor.C12;

            ret.C12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2) * this.BaseTensor.C13;
            ret.C12 += Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM12(rotationAngle, hKL), 2) * this.BaseTensor.C13;

            ret.C12 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2) * this.BaseTensor.C23;
            ret.C12 += Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2) * this.BaseTensor.C23;

            ret.C12 += 0.25 * 4 * MCTM21(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * this.BaseTensor.C44;
            ret.C12 += 0.25 * 4 * MCTM11(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM21(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * this.BaseTensor.C55;
            ret.C12 += 0.25 * 4 * MCTM11(rotationAngle, hKL) * MCTM21(rotationAngle, hKL) * MCTM21(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * this.BaseTensor.C66;

            ret.C44 += Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2) * this.BaseTensor.C11;
            ret.C44 += Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2) * this.BaseTensor.C22;
            ret.C44 += Math.Pow(MCTM32(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.C33;

            ret.C44 += 2 * MCTM12(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * this.BaseTensor.C12;
            ret.C44 += 2 * MCTM12(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * this.BaseTensor.C13;
            ret.C44 += 2 * MCTM22(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * this.BaseTensor.C23;

            ret.C44 += 0.25 * Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.C44;
            ret.C44 += 0.25 * 2 * MCTM32(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * this.BaseTensor.C44;
            ret.C44 += 0.25 * Math.Pow(MCTM32(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2) * this.BaseTensor.C44;

            ret.C44 += 0.25 * Math.Pow(MCTM32(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2) * this.BaseTensor.C55;
            ret.C44 += 0.25 * 2 * MCTM12(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * this.BaseTensor.C55;
            ret.C44 += 0.25 * Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.C55;

            ret.C44 += 0.25 * Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2) * this.BaseTensor.C66;
            ret.C44 += 0.25 * 2 * MCTM12(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * this.BaseTensor.C66;
            ret.C44 += 0.25 * Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2) * this.BaseTensor.C66;
            
            ret.CalculateCompliances();

            return ret;

        }

        /// <summary>
        /// Used for reuss
        /// </summary>
        /// <param name="varPhi1"></param>
        /// <param name="phi"></param>
        /// <param name="varPhi2"></param>
        /// <returns></returns>
        public Stress.Microsopic.ElasticityTensors OrientedComplianceTensorCubic(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        {
            Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

            ret.S11 = 0;
            ret.S12 = 0;
            ret.S44 = 0;

            ret.S11 += Math.Pow(MCTM11(rotationAngle, hKL), 4) * this.BaseTensor.S11;
            ret.S11 += Math.Pow(MCTM21(rotationAngle, hKL), 4) * this.BaseTensor.S22;
            ret.S11 += Math.Pow(MCTM31(rotationAngle, hKL), 4) * this.BaseTensor.S33;

            ret.S11 += 2 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM21(rotationAngle, hKL), 2) * this.BaseTensor.S12;
            ret.S11 += 2 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2) * this.BaseTensor.S13;
            ret.S11 += 2 * Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2) * this.BaseTensor.S23;

            ret.S11 += 0.25 * 4 * Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2) * this.BaseTensor.S44;
            ret.S11 += 0.25 * 4 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2) * this.BaseTensor.S55;
            ret.S11 += 0.25 * 4 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM21(rotationAngle, hKL), 2) * this.BaseTensor.S66;

            ret.S12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM12(rotationAngle, hKL), 2) * this.BaseTensor.S11;
            ret.S12 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2) * this.BaseTensor.S22;
            ret.S12 += Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2) * this.BaseTensor.S33;

            ret.S12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2) * this.BaseTensor.S12;
            ret.S12 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM12(rotationAngle, hKL), 2) * this.BaseTensor.S12;

            ret.S12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2) * this.BaseTensor.S13;
            ret.S12 += Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM12(rotationAngle, hKL), 2) * this.BaseTensor.S13;

            ret.S12 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2) * this.BaseTensor.S23;
            ret.S12 += Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2) * this.BaseTensor.S23;

            ret.S12 += 0.25 * 4 * MCTM21(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * this.BaseTensor.S44;
            ret.S12 += 0.25 * 4 * MCTM11(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM12(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * this.BaseTensor.S55;
            ret.S12 += 0.25 * 4 * MCTM11(rotationAngle, hKL) * MCTM21(rotationAngle, hKL) * MCTM12(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * this.BaseTensor.S66;

            ret.S44 += Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2) * this.BaseTensor.S11;
            ret.S44 += Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2) * this.BaseTensor.S22;
            ret.S44 += Math.Pow(MCTM32(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.S33;

            ret.S44 += 2 * MCTM12(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * this.BaseTensor.S12;
            ret.S44 += 2 * MCTM12(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * this.BaseTensor.S13;
            ret.S44 += 2 * MCTM22(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * this.BaseTensor.S23;

            ret.S44 += 0.25 * Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.S44;
            ret.S44 += 0.25 * 2 * MCTM32(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * this.BaseTensor.S44;
            ret.S44 += 0.25 * Math.Pow(MCTM32(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2) * this.BaseTensor.S44;

            ret.S44 += 0.25 * Math.Pow(MCTM32(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2) * this.BaseTensor.S55;
            ret.S44 += 0.25 * 2 * MCTM12(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * this.BaseTensor.S55;
            ret.S44 += 0.25 * Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.S55;

            ret.S44 += 0.25 * Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2) * this.BaseTensor.S66;
            ret.S44 += 0.25 * 2 * MCTM12(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * this.BaseTensor.S66;
            ret.S44 += 0.25 * Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2) * this.BaseTensor.S66;

            ret.CalculateStiffnesses();

            return ret;
        }

        /// <summary>
        /// None Transposed yet for testing
        /// </summary>
        /// <param name="varPhi1"></param>
        /// <param name="phi"></param>
        /// <param name="varPhi2"></param>
        /// <returns></returns>
        public Stress.Microsopic.ElasticityTensors OrientedComplianceTensorHexagonal(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        {
            Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

            ret.S11 = 0;
            ret.S33 = 0;
            ret.S12 = 0;
            ret.S13 = 0;
            ret.S44 = 0;

            ret.S11 += Math.Pow(MCTM11(rotationAngle, hKL), 4) * this.BaseTensor.S11;
            ret.S11 += Math.Pow(MCTM21(rotationAngle, hKL), 4) * this.BaseTensor.S22;
            ret.S11 += Math.Pow(MCTM31(rotationAngle, hKL), 4) * this.BaseTensor.S33;

            ret.S11 += 2 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM21(rotationAngle, hKL), 2) * this.BaseTensor.S12;
            ret.S11 += 2 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2) * this.BaseTensor.S13;
            ret.S11 += 2 * Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2) * this.BaseTensor.S23;

            ret.S11 += 0.25 * 4 * Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2) * this.BaseTensor.S44;
            ret.S11 += 0.25 * 4 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2) * this.BaseTensor.S55;
            ret.S11 += 0.25 * 4 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM21(rotationAngle, hKL), 2) * this.BaseTensor.S66;
            
            ret.S33 += Math.Pow(MCTM13(rotationAngle, hKL), 4) * this.BaseTensor.S11;
            ret.S33 += Math.Pow(MCTM23(rotationAngle, hKL), 4) * this.BaseTensor.S22;
            ret.S33 += Math.Pow(MCTM33(rotationAngle, hKL), 4) * this.BaseTensor.S33;

            ret.S33 += 2 * Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2) * this.BaseTensor.S12;
            ret.S33 += 2 * Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.S13;
            ret.S33 += 2 * Math.Pow(MCTM23(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.S23;

            ret.S33 += 0.25 * 4 * Math.Pow(MCTM23(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.S44;
            ret.S33 += 0.25 * 4 * Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.S55;
            ret.S33 += 0.25 * 4 * Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2) * this.BaseTensor.S66;
            
            ret.S12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM12(rotationAngle, hKL), 2) * this.BaseTensor.S11;
            ret.S12 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2) * this.BaseTensor.S22;
            ret.S12 += Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2) * this.BaseTensor.S33;

            ret.S12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2) * this.BaseTensor.S12;
            ret.S12 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM12(rotationAngle, hKL), 2) * this.BaseTensor.S12;

            ret.S12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2) * this.BaseTensor.S13;
            ret.S12 += Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM12(rotationAngle, hKL), 2) * this.BaseTensor.S13;

            ret.S12 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2) * this.BaseTensor.S23;
            ret.S12 += Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2) * this.BaseTensor.S23;

            ret.S12 += 0.25 * 4 * MCTM21(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * this.BaseTensor.S44;
            ret.S12 += 0.25 * 4 * MCTM11(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM12(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * this.BaseTensor.S55;
            ret.S12 += 0.25 * 4 * MCTM11(rotationAngle, hKL) * MCTM21(rotationAngle, hKL) * MCTM12(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * this.BaseTensor.S66;

            ret.S13 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2) * this.BaseTensor.S11;
            ret.S13 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2) * this.BaseTensor.S22;
            ret.S13 += Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.S33;

            ret.S13 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2) * this.BaseTensor.S12;
            ret.S13 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2) * this.BaseTensor.S12;

            ret.S13 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.S13;
            ret.S13 += Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2) * this.BaseTensor.S13;

            ret.S13 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.S23;
            ret.S13 += Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2) * this.BaseTensor.S23;

            ret.S13 += 0.25 * 4 * MCTM21(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * this.BaseTensor.S44;
            ret.S13 += 0.25 * 4 * MCTM11(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * this.BaseTensor.S55;
            ret.S13 += 0.25 * 4 * MCTM11(rotationAngle, hKL) * MCTM21(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * this.BaseTensor.S66;

            ret.S44 += Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2) * this.BaseTensor.S11;
            ret.S44 += Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2) * this.BaseTensor.S22;
            ret.S44 += Math.Pow(MCTM32(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.S33;

            ret.S44 += 2 * MCTM12(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * this.BaseTensor.S12;
            ret.S44 += 2 * MCTM12(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * this.BaseTensor.S13;
            ret.S44 += 2 * MCTM22(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * this.BaseTensor.S23;

            ret.S44 += 0.25 * Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.S44;
            ret.S44 += 0.25 * 2 * MCTM32(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * this.BaseTensor.S44;
            ret.S44 += 0.25 * Math.Pow(MCTM32(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2) * this.BaseTensor.S44;

            ret.S44 += 0.25 * Math.Pow(MCTM32(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2) * this.BaseTensor.S55;
            ret.S44 += 0.25 * 2 * MCTM12(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * this.BaseTensor.S55;
            ret.S44 += 0.25 * Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.S55;

            ret.S44 += 0.25 * Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2) * this.BaseTensor.S66;
            ret.S44 += 0.25 * 2 * MCTM12(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * this.BaseTensor.S66;
            ret.S44 += 0.25 * Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2) * this.BaseTensor.S66;

            return ret;
        }

        /// <summary>
        /// None transposed yet for testing
        /// </summary>
        /// <param name="varPhi1"></param>
        /// <param name="phi"></param>
        /// <param name="varPhi2"></param>
        /// <returns></returns>
        public Stress.Microsopic.ElasticityTensors OrientedStiffnessTensorHexagonal(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        {
            Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

            ret.C11 = 0;
            ret.C33 = 0;
            ret.C12 = 0;
            ret.C13 = 0;
            ret.C44 = 0;

            ret.C11 += Math.Pow(MCTM11(rotationAngle, hKL), 4) * this.BaseTensor.C11;
            ret.C11 += Math.Pow(MCTM21(rotationAngle, hKL), 4) * this.BaseTensor.C22;
            ret.C11 += Math.Pow(MCTM31(rotationAngle, hKL), 4) * this.BaseTensor.C33;

            ret.C11 += 2 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM21(rotationAngle, hKL), 2) * this.BaseTensor.C12;
            ret.C11 += 2 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2) * this.BaseTensor.C13;
            ret.C11 += 2 * Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2) * this.BaseTensor.C23;

            ret.C11 += 0.25 * 4 * Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2) * this.BaseTensor.C44;
            ret.C11 += 0.25 * 4 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2) * this.BaseTensor.C55;
            ret.C11 += 0.25 * 4 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM21(rotationAngle, hKL), 2) * this.BaseTensor.C66;

            ret.C33 += Math.Pow(MCTM13(rotationAngle, hKL), 4) * this.BaseTensor.C11;
            ret.C33 += Math.Pow(MCTM23(rotationAngle, hKL), 4) * this.BaseTensor.C22;
            ret.C33 += Math.Pow(MCTM33(rotationAngle, hKL), 4) * this.BaseTensor.C33;

            ret.C33 += 2 * Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2) * this.BaseTensor.C12;
            ret.C33 += 2 * Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.C13;
            ret.C33 += 2 * Math.Pow(MCTM23(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.C23;

            ret.C33 += 0.25 * 4 * Math.Pow(MCTM23(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.C44;
            ret.C33 += 0.25 * 4 * Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.C55;
            ret.C33 += 0.25 * 4 * Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2) * this.BaseTensor.C66;

            ret.C12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM12(rotationAngle, hKL), 2) * this.BaseTensor.C11;
            ret.C12 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2) * this.BaseTensor.C22;
            ret.C12 += Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2) * this.BaseTensor.C33;

            ret.C12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2) * this.BaseTensor.C12;
            ret.C12 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM12(rotationAngle, hKL), 2) * this.BaseTensor.C12;

            ret.C12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2) * this.BaseTensor.C13;
            ret.C12 += Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM12(rotationAngle, hKL), 2) * this.BaseTensor.C13;

            ret.C12 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2) * this.BaseTensor.C23;
            ret.C12 += Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2) * this.BaseTensor.C23;

            ret.C12 += 0.25 * 4 * MCTM21(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * this.BaseTensor.C44;
            ret.C12 += 0.25 * 4 * MCTM11(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM12(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * this.BaseTensor.C55;
            ret.C12 += 0.25 * 4 * MCTM11(rotationAngle, hKL) * MCTM21(rotationAngle, hKL) * MCTM12(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * this.BaseTensor.C66;

            ret.C13 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2) * this.BaseTensor.C11;
            ret.C13 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2) * this.BaseTensor.C22;
            ret.C13 += Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.C33;

            ret.C13 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2) * this.BaseTensor.C12;
            ret.C13 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2) * this.BaseTensor.C12;

            ret.C13 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.C13;
            ret.C13 += Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2) * this.BaseTensor.C13;

            ret.C13 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.C23;
            ret.C13 += Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2) * this.BaseTensor.C23;

            ret.C13 += 0.25 * 4 * MCTM21(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * this.BaseTensor.C44;
            ret.C13 += 0.25 * 4 * MCTM11(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * this.BaseTensor.C55;
            ret.C13 += 0.25 * 4 * MCTM11(rotationAngle, hKL) * MCTM21(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * this.BaseTensor.C66;

            ret.C44 += Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2) * this.BaseTensor.C11;
            ret.C44 += Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2) * this.BaseTensor.C22;
            ret.C44 += Math.Pow(MCTM32(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.C33;

            ret.C44 += 2 * MCTM12(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * this.BaseTensor.C12;
            ret.C44 += 2 * MCTM12(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * this.BaseTensor.C13;
            ret.C44 += 2 * MCTM22(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * this.BaseTensor.C23;

            ret.C44 += 0.25 * Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.C44;
            ret.C44 += 0.25 * 2 * MCTM32(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * this.BaseTensor.C44;
            ret.C44 += 0.25 * Math.Pow(MCTM32(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2) * this.BaseTensor.C44;

            ret.C44 += 0.25 * Math.Pow(MCTM32(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2) * this.BaseTensor.C55;
            ret.C44 += 0.25 * 2 * MCTM12(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * this.BaseTensor.C55;
            ret.C44 += 0.25 * Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2) * this.BaseTensor.C55;

            ret.C44 += 0.25 * Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2) * this.BaseTensor.C66;
            ret.C44 += 0.25 * 2 * MCTM12(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * this.BaseTensor.C66;
            ret.C44 += 0.25 * Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2) * this.BaseTensor.C66;

            return ret;

        }

        #region First derivatives

        #region Cubic

        /// <summary>
        /// 
        /// </summary>
        /// <param name="varPhi1"></param>
        /// <param name="phi"></param>
        /// <param name="varPhi2"></param>
        /// <returns></returns>
        public Stress.Microsopic.ElasticityTensors OrientedStiffnessTensorCubicFDC11(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        {
            Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

            ret.C11 = 0;
            ret.C12 = 0;
            ret.C44 = 0;

            ret.C11 += Math.Pow(MCTM11(rotationAngle, hKL), 4);
            ret.C11 += Math.Pow(MCTM21(rotationAngle, hKL), 4);
            ret.C11 += Math.Pow(MCTM31(rotationAngle, hKL), 4);

            ret.C12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM12(rotationAngle, hKL), 2);
            ret.C12 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2);
            ret.C12 += Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2);

            ret.C44 += Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2);
            ret.C44 += Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2);
            ret.C44 += Math.Pow(MCTM32(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);

            Stress.Microsopic.ElasticityTensors InvertedOrigTensor = this.OrientedStiffnessTensorCubic(rotationAngle, hKL);

            ret._complianceTensor = -1.0 * InvertedOrigTensor._complianceTensor * ret._stiffnessTensor * InvertedOrigTensor._complianceTensor;

            ret.CalculateStiffnesses();

            return ret;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="varPhi1"></param>
        /// <param name="phi"></param>
        /// <param name="varPhi2"></param>
        /// <returns></returns>
        public Stress.Microsopic.ElasticityTensors OrientedStiffnessTensorCubicFDC12(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        {
            Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

            ret.C11 = 0;
            ret.C12 = 0;
            ret.C44 = 0;

            ret.C11 += 2 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM21(rotationAngle, hKL), 2);
            ret.C11 += 2 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2);
            ret.C11 += 2 * Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2);

            ret.C12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2);
            ret.C12 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM12(rotationAngle, hKL), 2);
            
            ret.C12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2);
            ret.C12 += Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM12(rotationAngle, hKL), 2);

            ret.C12 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2);
            ret.C12 += Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2);

            ret.C44 += 2 * MCTM12(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM23(rotationAngle, hKL);
            ret.C44 += 2 * MCTM12(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM33(rotationAngle, hKL);
            ret.C44 += 2 * MCTM22(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM33(rotationAngle, hKL);

            Stress.Microsopic.ElasticityTensors InvertedOrigTensor = this.OrientedStiffnessTensorCubic(rotationAngle, hKL);

            ret._complianceTensor = -1.0 * InvertedOrigTensor._complianceTensor * ret._stiffnessTensor * InvertedOrigTensor._complianceTensor;

            ret.CalculateStiffnesses();
            return ret;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="varPhi1"></param>
        /// <param name="phi"></param>
        /// <param name="varPhi2"></param>
        /// <returns></returns>
        public Stress.Microsopic.ElasticityTensors OrientedStiffnessTensorCubicFDC44(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        {
            Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

            ret.C11 = 0;
            ret.C12 = 0;
            ret.C44 = 0;

            ret.C11 += 0.25 * 4 * Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2);
            ret.C11 += 0.25 * 4 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2);
            ret.C11 += 0.25 * 4 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM21(rotationAngle, hKL), 2);

            ret.C12 += 0.25 * 4 * MCTM21(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM32(rotationAngle, hKL);
            ret.C12 += 0.25 * 4 * MCTM11(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM12(rotationAngle, hKL) * MCTM32(rotationAngle, hKL);
            ret.C12 += 0.25 * 4 * MCTM11(rotationAngle, hKL) * MCTM21(rotationAngle, hKL) * MCTM12(rotationAngle, hKL) * MCTM22(rotationAngle, hKL);

            ret.C44 += 0.25 * Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);
            ret.C44 += 0.25 * 2 * MCTM32(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM33(rotationAngle, hKL);
            ret.C44 += 0.25 * Math.Pow(MCTM32(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2);

            ret.C44 += 0.25 * Math.Pow(MCTM32(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2);
            ret.C44 += 0.25 * 2 * MCTM12(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM13(rotationAngle, hKL);
            ret.C44 += 0.25 * Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);

            ret.C44 += 0.25 * Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2);
            ret.C44 += 0.25 * 2 * MCTM12(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM13(rotationAngle, hKL);
            ret.C44 += 0.25 * Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2);

            Stress.Microsopic.ElasticityTensors InvertedOrigTensor = this.OrientedStiffnessTensorCubic(rotationAngle, hKL);

            ret._complianceTensor = -1.0 * InvertedOrigTensor._complianceTensor * ret._stiffnessTensor * InvertedOrigTensor._complianceTensor;

            ret.CalculateStiffnesses();

            return ret;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="varPhi1"></param>
        /// <param name="phi"></param>
        /// <param name="varPhi2"></param>
        /// <returns></returns>
        public Stress.Microsopic.ElasticityTensors OrientedComplianceTensorCubicFDS11(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        {
            Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

            ret.S11 = 0;
            ret.S12 = 0;
            ret.S44 = 0;

            ret.S11 += Math.Pow(MCTM11(rotationAngle, hKL), 4);
            ret.S11 += Math.Pow(MCTM21(rotationAngle, hKL), 4);
            ret.S11 += Math.Pow(MCTM31(rotationAngle, hKL), 4);

            ret.S12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM12(rotationAngle, hKL), 2);
            ret.S12 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2);
            ret.S12 += Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2);

            ret.S44 += Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2);
            ret.S44 += Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2);
            ret.S44 += Math.Pow(MCTM32(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);

            ret.CalculateStiffnesses();

            return ret;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="varPhi1"></param>
        /// <param name="phi"></param>
        /// <param name="varPhi2"></param>
        /// <returns></returns>
        public Stress.Microsopic.ElasticityTensors OrientedComplianceTensorCubicFDS12(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        {
            Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

            ret.S11 = 0;
            ret.S12 = 0;
            ret.S44 = 0;

            ret.S11 += 2 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM21(rotationAngle, hKL), 2);
            ret.S11 += 2 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2);
            ret.S11 += 2 * Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2);

            ret.S12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2);
            ret.S12 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM12(rotationAngle, hKL), 2);
            
            ret.S12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2);
            ret.S12 += Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM12(rotationAngle, hKL), 2);

            ret.S12 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2);
            ret.S12 += Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2);

            ret.S44 += 2 * MCTM12(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM23(rotationAngle, hKL);
            ret.S44 += 2 * MCTM12(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM33(rotationAngle, hKL);
            ret.S44 += 2 * MCTM22(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM33(rotationAngle, hKL);

            ret.CalculateStiffnesses();
            return ret;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="varPhi1"></param>
        /// <param name="phi"></param>
        /// <param name="varPhi2"></param>
        /// <returns></returns>
        public Stress.Microsopic.ElasticityTensors OrientedComplianceTensorCubicFDS44(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        {
            Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

            ret.S11 = 0;
            ret.S12 = 0;
            ret.S44 = 0;

            ret.S11 += 0.25 * 4 * Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2);
            ret.S11 += 0.25 * 4 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2);
            ret.S11 += 0.25 * 4 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM21(rotationAngle, hKL), 2);

            ret.S12 += 0.25 * 4 * MCTM21(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM32(rotationAngle, hKL);
            ret.S12 += 0.25 * 4 * MCTM11(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM12(rotationAngle, hKL) * MCTM32(rotationAngle, hKL);
            ret.S12 += 0.25 * 4 * MCTM11(rotationAngle, hKL) * MCTM21(rotationAngle, hKL) * MCTM12(rotationAngle, hKL) * MCTM22(rotationAngle, hKL);

            ret.S44 += 0.25 * Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);
            ret.S44 += 0.25 * 2 * MCTM32(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM33(rotationAngle, hKL);
            ret.S44 += 0.25 * Math.Pow(MCTM32(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2);

            ret.S44 += 0.25 * Math.Pow(MCTM32(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2);
            ret.S44 += 0.25 * 2 * MCTM12(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM13(rotationAngle, hKL);
            ret.S44 += 0.25 * Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);

            ret.S44 += 0.25 * Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2);
            ret.S44 += 0.25 * 2 * MCTM12(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM13(rotationAngle, hKL);
            ret.S44 += 0.25 * Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2);

            ret.CalculateStiffnesses();

            return ret;
        }

        #endregion
        
        #region Hexagonal

        /// <summary>
        /// 
        /// </summary>
        /// <param name="varPhi1"></param>
        /// <param name="phi"></param>
        /// <param name="varPhi2"></param>
        /// <returns></returns>
        public Stress.Microsopic.ElasticityTensors OrientedStiffnessTensorHexagonalFDC11(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        {
            Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

            ret.C11 = 0;
            ret.C33 = 0;
            ret.C12 = 0;
            ret.C13 = 0;
            ret.C44 = 0;

            ret.C11 += Math.Pow(MCTM11(rotationAngle, hKL), 4);
            ret.C11 += Math.Pow(MCTM21(rotationAngle, hKL), 4);

            ret.C33 += Math.Pow(MCTM13(rotationAngle, hKL), 4);
            ret.C33 += Math.Pow(MCTM23(rotationAngle, hKL), 4);

            ret.C12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM12(rotationAngle, hKL), 2);
            ret.C12 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2);

            ret.C13 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2);
            ret.C13 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2);

            ret.C44 += Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2);
            ret.C44 += Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2);

            ret.C44 += 0.5 * 0.25 * Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2);
            ret.C44 += 0.5 * 0.25 * 2 * MCTM12(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM13(rotationAngle, hKL);
            ret.C44 += 0.5 * 0.25 * Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2);
            ret.C13 += 0.5 * 0.25 * 4 * MCTM11(rotationAngle, hKL) * MCTM21(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM23(rotationAngle, hKL);
            ret.C12 += 0.5 * 0.25 * 4 * MCTM11(rotationAngle, hKL) * MCTM21(rotationAngle, hKL) * MCTM12(rotationAngle, hKL) * MCTM22(rotationAngle, hKL);
            ret.C33 += 0.5 * 0.25 * 4 * Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2);
            ret.C11 += 0.5 * 0.25 * 4 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM21(rotationAngle, hKL), 2);
            
            Stress.Microsopic.ElasticityTensors InvertedOrigTensor = this.OrientedStiffnessTensorCubic(rotationAngle, hKL);

            ret._complianceTensor = -1.0 * InvertedOrigTensor._complianceTensor * ret._stiffnessTensor * InvertedOrigTensor._complianceTensor;

            ret.CalculateStiffnesses();

            return ret;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="varPhi1"></param>
        /// <param name="phi"></param>
        /// <param name="varPhi2"></param>
        /// <returns></returns>
        public Stress.Microsopic.ElasticityTensors OrientedStiffnessTensorHexagonalFDC33(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        {
            Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

            ret.C11 = 0;
            ret.C33 = 0;
            ret.C12 = 0;
            ret.C13 = 0;
            ret.C44 = 0;
            
            ret.C11 += Math.Pow(MCTM31(rotationAngle, hKL), 4);

            ret.C33 += Math.Pow(MCTM33(rotationAngle, hKL), 4);
            
            ret.C12 += Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2);
            
            ret.C13 += Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);
            
            ret.C44 += Math.Pow(MCTM32(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);

            Stress.Microsopic.ElasticityTensors InvertedOrigTensor = this.OrientedStiffnessTensorCubic(rotationAngle, hKL);

            ret._complianceTensor = -1.0 * InvertedOrigTensor._complianceTensor * ret._stiffnessTensor * InvertedOrigTensor._complianceTensor;

            ret.CalculateStiffnesses();

            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="varPhi1"></param>
        /// <param name="phi"></param>
        /// <param name="varPhi2"></param>
        /// <returns></returns>
        public Stress.Microsopic.ElasticityTensors OrientedStiffnessTensorHexagonalFDC12(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        {
            Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

            ret.C11 = 0;
            ret.C33 = 0;
            ret.C12 = 0;
            ret.C13 = 0;
            ret.C44 = 0;

            ret.C11 += 2 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM21(rotationAngle, hKL), 2);
            
            ret.C33 += 2 * Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2);
            
            ret.C12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2);
            ret.C12 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM12(rotationAngle, hKL), 2);
            
            ret.C13 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2);
            ret.C13 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2);
            
            ret.C44 += 2 * MCTM12(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM23(rotationAngle, hKL);

            ret.C44 -= 0.5 * 0.25 * Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2);
            ret.C44 -= 0.5 * 0.25 * 2 * MCTM12(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM13(rotationAngle, hKL);
            ret.C44 -= 0.5 * 0.25 * Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2);
            ret.C13 -= 0.5 * 0.25 * 4 * MCTM11(rotationAngle, hKL) * MCTM21(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM23(rotationAngle, hKL);
            ret.C12 -= 0.5 * 0.25 * 4 * MCTM11(rotationAngle, hKL) * MCTM21(rotationAngle, hKL) * MCTM12(rotationAngle, hKL) * MCTM22(rotationAngle, hKL);
            ret.C33 -= 0.5 * 0.25 * 4 * Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2);
            ret.C11 -= 0.5 * 0.25 * 4 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM21(rotationAngle, hKL), 2);

            Stress.Microsopic.ElasticityTensors InvertedOrigTensor = this.OrientedStiffnessTensorCubic(rotationAngle, hKL);

            ret._complianceTensor = -1.0 * InvertedOrigTensor._complianceTensor * ret._stiffnessTensor * InvertedOrigTensor._complianceTensor;

            ret.CalculateStiffnesses();

            return ret;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="varPhi1"></param>
        /// <param name="phi"></param>
        /// <param name="varPhi2"></param>
        /// <returns></returns>
        public Stress.Microsopic.ElasticityTensors OrientedStiffnessTensorHexagonalFDC13(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        {
            Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

            ret.C11 = 0;
            ret.C33 = 0;
            ret.C12 = 0;
            ret.C13 = 0;
            ret.C44 = 0;
            
            ret.C11 += 2 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2);
            ret.C11 += 2 * Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2);
            
            ret.C33 += 2 * Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);
            ret.C33 += 2 * Math.Pow(MCTM23(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);
            
            ret.C12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2);
            ret.C12 += Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM12(rotationAngle, hKL), 2);

            ret.C12 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2);
            ret.C12 += Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2);
            
            ret.C13 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);
            ret.C13 += Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2);

            ret.C13 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);
            ret.C13 += Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2);
            
            ret.C44 += 2 * MCTM12(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM33(rotationAngle, hKL);
            ret.C44 += 2 * MCTM22(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM33(rotationAngle, hKL);

            Stress.Microsopic.ElasticityTensors InvertedOrigTensor = this.OrientedStiffnessTensorCubic(rotationAngle, hKL);

            ret._complianceTensor = -1.0 * InvertedOrigTensor._complianceTensor * ret._stiffnessTensor * InvertedOrigTensor._complianceTensor;

            ret.CalculateStiffnesses();

            return ret;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="varPhi1"></param>
        /// <param name="phi"></param>
        /// <param name="varPhi2"></param>
        /// <returns></returns>
        public Stress.Microsopic.ElasticityTensors OrientedStiffnessTensorHexagonalFDC44(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        {
            Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

            ret.C11 = 0;
            ret.C33 = 0;
            ret.C12 = 0;
            ret.C13 = 0;
            ret.C44 = 0;

            ret.C11 += 0.25 * 4 * Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2);
            ret.C11 += 0.25 * 4 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2);
            
            ret.C33 += 0.25 * 4 * Math.Pow(MCTM23(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);
            ret.C33 += 0.25 * 4 * Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);
            
            ret.C12 += 0.25 * 4 * MCTM21(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM32(rotationAngle, hKL);
            ret.C12 += 0.25 * 4 * MCTM11(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM12(rotationAngle, hKL) * MCTM32(rotationAngle, hKL);
            

            ret.C13 += 0.25 * 4 * MCTM21(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM33(rotationAngle, hKL);
            ret.C13 += 0.25 * 4 * MCTM11(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM33(rotationAngle, hKL);
            
            ret.C44 += 0.25 * Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);
            ret.C44 += 0.25 * 2 * MCTM32(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM33(rotationAngle, hKL);
            ret.C44 += 0.25 * Math.Pow(MCTM32(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2);

            ret.C44 += 0.25 * Math.Pow(MCTM32(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2);
            ret.C44 += 0.25 * 2 * MCTM12(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM13(rotationAngle, hKL);
            ret.C44 += 0.25 * Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);

            Stress.Microsopic.ElasticityTensors InvertedOrigTensor = this.OrientedStiffnessTensorCubic(rotationAngle, hKL);

            ret._complianceTensor = -1.0 * InvertedOrigTensor._complianceTensor * ret._stiffnessTensor * InvertedOrigTensor._complianceTensor;

            ret.CalculateStiffnesses();

            return ret;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="varPhi1"></param>
        /// <param name="phi"></param>
        /// <param name="varPhi2"></param>
        /// <returns></returns>
        public Stress.Microsopic.ElasticityTensors OrientedComplianceTensorHexagonalFDS11(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        {
            Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

            ret.S11 = 0;
            ret.S33 = 0;
            ret.S12 = 0;
            ret.S13 = 0;
            ret.S44 = 0;

            ret.S11 += Math.Pow(MCTM11(rotationAngle, hKL), 4);
            ret.S11 += Math.Pow(MCTM21(rotationAngle, hKL), 4);

            ret.S33 += Math.Pow(MCTM13(rotationAngle, hKL), 4);
            ret.S33 += Math.Pow(MCTM23(rotationAngle, hKL), 4);

            ret.S12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM12(rotationAngle, hKL), 2);
            ret.S12 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2);

            ret.S13 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2);
            ret.S13 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2);

            ret.S44 += Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2);
            ret.S44 += Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2);

            ret.S44 += 2 * 0.25 * Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2);
            ret.S44 += 2 * 0.25 * 2 * MCTM12(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM13(rotationAngle, hKL);
            ret.S44 += 2 * 0.25 * Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2);
            ret.S13 += 2 * 0.25 * 4 * MCTM11(rotationAngle, hKL) * MCTM21(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM23(rotationAngle, hKL);
            ret.S12 += 2 * 0.25 * 4 * MCTM11(rotationAngle, hKL) * MCTM21(rotationAngle, hKL) * MCTM12(rotationAngle, hKL) * MCTM22(rotationAngle, hKL);
            ret.S33 += 2 * 0.25 * 4 * Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2);
            ret.S11 += 2 * 0.25 * 4 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM21(rotationAngle, hKL), 2);
            
            return ret;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="varPhi1"></param>
        /// <param name="phi"></param>
        /// <param name="varPhi2"></param>
        /// <returns></returns>
        public Stress.Microsopic.ElasticityTensors OrientedComplianceTensorHexagonalFDS33(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        {
            Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

            ret.S11 = 0;
            ret.S33 = 0;
            ret.S12 = 0;
            ret.S13 = 0;
            ret.S44 = 0;

            ret.S11 += Math.Pow(MCTM31(rotationAngle, hKL), 4);

            ret.S33 += Math.Pow(MCTM33(rotationAngle, hKL), 4);

            ret.S12 += Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2);

            ret.S13 += Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);

            ret.S44 += Math.Pow(MCTM32(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);

            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="varPhi1"></param>
        /// <param name="phi"></param>
        /// <param name="varPhi2"></param>
        /// <returns></returns>
        public Stress.Microsopic.ElasticityTensors OrientedComplianceTensorHexagonalFDS12(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        {
            Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

            ret.S11 = 0;
            ret.S33 = 0;
            ret.S12 = 0;
            ret.S13 = 0;
            ret.S44 = 0;

            ret.S11 += 2 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM21(rotationAngle, hKL), 2);

            ret.S33 += 2 * Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2);

            ret.S12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2);
            ret.S12 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM12(rotationAngle, hKL), 2);

            ret.S13 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2);
            ret.S13 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2);

            ret.S44 += 2 * MCTM12(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM23(rotationAngle, hKL);

            ret.S44 -= 2 * 0.25 * Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2);
            ret.S44 -= 2 * 0.25 * 2 * MCTM12(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM13(rotationAngle, hKL);
            ret.S44 -= 2 * 0.25 * Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2);
            ret.S13 -= 2 * 0.25 * 4 * MCTM11(rotationAngle, hKL) * MCTM21(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM23(rotationAngle, hKL);
            ret.S12 -= 2 * 0.25 * 4 * MCTM11(rotationAngle, hKL) * MCTM21(rotationAngle, hKL) * MCTM12(rotationAngle, hKL) * MCTM22(rotationAngle, hKL);
            ret.S33 -= 2 * 0.25 * 4 * Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2);
            ret.S11 -= 2 * 0.25 * 4 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM21(rotationAngle, hKL), 2);

            return ret;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="varPhi1"></param>
        /// <param name="phi"></param>
        /// <param name="varPhi2"></param>
        /// <returns></returns>
        public Stress.Microsopic.ElasticityTensors OrientedComplianceTensorHexagonalFDS13(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        {
            Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

            ret.S11 = 0;
            ret.S33 = 0;
            ret.S12 = 0;
            ret.S13 = 0;
            ret.S44 = 0;

            ret.S11 += 2 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2);
            ret.S11 += 2 * Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2);

            ret.S33 += 2 * Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);
            ret.S33 += 2 * Math.Pow(MCTM23(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);

            ret.S12 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2);
            ret.S12 += Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM12(rotationAngle, hKL), 2);

            ret.S12 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM32(rotationAngle, hKL), 2);
            ret.S12 += Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM22(rotationAngle, hKL), 2);

            ret.S13 += Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);
            ret.S13 += Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2);

            ret.S13 += Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);
            ret.S13 += Math.Pow(MCTM31(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2);

            ret.S44 += 2 * MCTM12(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM33(rotationAngle, hKL);
            ret.S44 += 2 * MCTM22(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM33(rotationAngle, hKL);

            return ret;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="varPhi1"></param>
        /// <param name="phi"></param>
        /// <param name="varPhi2"></param>
        /// <returns></returns>
        public Stress.Microsopic.ElasticityTensors OrientedComplianceTensorHexagonalFDS44(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        {
            Stress.Microsopic.ElasticityTensors ret = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;

            ret.S11 = 0;
            ret.S33 = 0;
            ret.S12 = 0;
            ret.S13 = 0;
            ret.S44 = 0;

            ret.S11 += 0.25 * 4 * Math.Pow(MCTM21(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2);
            ret.S11 += 0.25 * 4 * Math.Pow(MCTM11(rotationAngle, hKL), 2) * Math.Pow(MCTM31(rotationAngle, hKL), 2);

            ret.S33 += 0.25 * 4 * Math.Pow(MCTM23(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);
            ret.S33 += 0.25 * 4 * Math.Pow(MCTM13(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);

            ret.S12 += 0.25 * 4 * MCTM21(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM32(rotationAngle, hKL);
            ret.S12 += 0.25 * 4 * MCTM11(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM12(rotationAngle, hKL) * MCTM32(rotationAngle, hKL);
            
            ret.S13 += 0.25 * 4 * MCTM21(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM33(rotationAngle, hKL);
            ret.S13 += 0.25 * 4 * MCTM11(rotationAngle, hKL) * MCTM31(rotationAngle, hKL) * MCTM13(rotationAngle, hKL) * MCTM33(rotationAngle, hKL);

            ret.S44 += 0.25 * Math.Pow(MCTM22(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);
            ret.S44 += 0.25 * 2 * MCTM32(rotationAngle, hKL) * MCTM23(rotationAngle, hKL) * MCTM22(rotationAngle, hKL) * MCTM33(rotationAngle, hKL);
            ret.S44 += 0.25 * Math.Pow(MCTM32(rotationAngle, hKL), 2) * Math.Pow(MCTM23(rotationAngle, hKL), 2);

            ret.S44 += 0.25 * Math.Pow(MCTM32(rotationAngle, hKL), 2) * Math.Pow(MCTM13(rotationAngle, hKL), 2);
            ret.S44 += 0.25 * 2 * MCTM12(rotationAngle, hKL) * MCTM33(rotationAngle, hKL) * MCTM32(rotationAngle, hKL) * MCTM13(rotationAngle, hKL);
            ret.S44 += 0.25 * Math.Pow(MCTM12(rotationAngle, hKL), 2) * Math.Pow(MCTM33(rotationAngle, hKL), 2);

            return ret;

        }

        #endregion

        #endregion

        #endregion

        #region Orientation Transformation Operations

        #region Sample-Crystal-System transormation Matrix

        public static MathNet.Numerics.LinearAlgebra.Matrix<double> GetSCTM(double varPhi1, double phi, double varPhi2)
        {
            MathNet.Numerics.LinearAlgebra.Matrix<double> ret = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

            ret[0, 0] += SCTM11(varPhi1, phi, varPhi2);
            ret[0, 1] += SCTM12(varPhi1, phi, varPhi2);
            ret[0, 2] += SCTM13(varPhi1, phi, varPhi2);
            ret[1, 0] += SCTM21(varPhi1, phi, varPhi2);
            ret[1, 1] += SCTM22(varPhi1, phi, varPhi2);
            ret[1, 2] += SCTM23(varPhi1, phi, varPhi2);
            ret[2, 0] += SCTM31(varPhi1, phi, varPhi2);
            ret[2, 1] += SCTM32(varPhi1, phi, varPhi2);
            ret[2, 2] += SCTM33(varPhi1, phi, varPhi2);

            return ret;
        }

        public static double SCTM11(double varPhi1, double phi, double varPhi2)
        {
            double Ret = Math.Cos((varPhi1 * Math.PI) / 180.0) * Math.Cos((varPhi2 * Math.PI) / 180.0);
            Ret -= Math.Sin((varPhi1 * Math.PI) / 180.0) * Math.Sin((varPhi2 * Math.PI) / 180.0) * Math.Cos((phi * Math.PI) / 180.0);

            return Ret;
        }

        public static double SCTM12(double varPhi1, double phi, double varPhi2)
        {
            double Ret = Math.Sin((varPhi1 * Math.PI) / 180.0) * Math.Cos((varPhi2 * Math.PI) / 180.0);
            Ret += Math.Cos((varPhi1 * Math.PI) / 180.0) * Math.Sin((varPhi2 * Math.PI) / 180.0) * Math.Cos((phi * Math.PI) / 180.0);

            return Ret;
        }

        public static double SCTM13(double varPhi1, double phi, double varPhi2)
        {
            double Ret = Math.Sin((varPhi2 * Math.PI) / 180.0) * Math.Sin((phi * Math.PI) / 180.0);

            return Ret;
        }

        public static double SCTM21(double varPhi1, double phi, double varPhi2)
        {
            double Ret = -1 * Math.Cos((varPhi1 * Math.PI) / 180.0) * Math.Sin((varPhi2 * Math.PI) / 180.0);
            Ret -= Math.Sin((varPhi1 * Math.PI) / 180.0) * Math.Cos((varPhi2 * Math.PI) / 180.0) * Math.Cos((phi * Math.PI) / 180.0);

            return Ret;
        }

        public static double SCTM22(double varPhi1, double phi, double varPhi2)
        {
            double Ret = -1 * Math.Sin((varPhi1 * Math.PI) / 180.0) * Math.Sin((varPhi2 * Math.PI) / 180.0);
            Ret += Math.Cos((varPhi1 * Math.PI) / 180.0) * Math.Cos((varPhi2 * Math.PI) / 180.0) * Math.Cos((phi * Math.PI) / 180.0);

            return Ret;
        }

        public static double SCTM23(double varPhi1, double phi, double varPhi2)
        {
            double Ret = Math.Cos((varPhi2 * Math.PI) / 180.0) * Math.Sin((phi * Math.PI) / 180.0);

            return Ret;
        }

        public static double SCTM31(double varPhi1, double phi, double varPhi2)
        {
            double Ret = Math.Sin((varPhi1 * Math.PI) / 180.0) * Math.Sin((phi * Math.PI) / 180.0);

            return Ret;
        }

        public static double SCTM32(double varPhi1, double phi, double varPhi2)
        {
            double Ret = -1 * Math.Cos((varPhi1 * Math.PI) / 180.0) * Math.Sin((phi * Math.PI) / 180.0);

            return Ret;
        }

        public static double SCTM33(double varPhi1, double phi, double varPhi2)
        {
            double Ret = Math.Cos((phi * Math.PI) / 180.0);

            return Ret;
        }

        #endregion

        #region Measurement-Crystal-System transformation amtrix

        public double HKLPolarAngle(DataManagment.CrystalData.HKLReflex hKL)
        {
            double ret = Math.Acos(hKL.L / Math.Sqrt(Math.Pow(hKL.H, 2) + Math.Pow(hKL.K, 2) + Math.Pow(hKL.L, 2)));

            return ret;
        }

        public double HKLAzimutAngle(DataManagment.CrystalData.HKLReflex hKL)
        {
            if(hKL.H > 0)
            {
                return Math.Atan(Convert.ToDouble(hKL.K) / Convert.ToDouble(hKL.H));
            }
            else if(hKL.H == 0)
            {
                return Math.Sign(hKL.K) * (Math.PI / 2.0);
            }
            else
            {
                if(hKL.K < 0)
                {
                    return Math.Atan(Convert.ToDouble(hKL.K) / Convert.ToDouble(hKL.H)) - Math.PI;
                }
                else
                {
                    return Math.Atan(Convert.ToDouble(hKL.K) / Convert.ToDouble(hKL.H)) + Math.PI;
                }
            }
        }

        #region Mess zu Krystallsystem (Eigene berechnung)

        //public MathNet.Numerics.LinearAlgebra.Matrix<double> GetMCTM(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        //{
        //    MathNet.Numerics.LinearAlgebra.Matrix<double> ret = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

        //    ret[0, 0] += this.MCTM11(rotationAngle, hKL);
        //    ret[0, 1] += this.MCTM12(rotationAngle, hKL);
        //    ret[0, 2] += this.MCTM13(rotationAngle, hKL);
        //    ret[1, 0] += this.MCTM21(rotationAngle, hKL);
        //    ret[1, 1] += this.MCTM22(rotationAngle, hKL);
        //    ret[1, 2] += this.MCTM23(rotationAngle, hKL);
        //    ret[2, 0] += this.MCTM31(rotationAngle, hKL);
        //    ret[2, 1] += this.MCTM32(rotationAngle, hKL);
        //    ret[2, 2] += this.MCTM33(rotationAngle, hKL);

        //    return ret;
        //}

        //public double MCTM11(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        //{
        //    //double correctedAzimutAngle = (Math.PI / 2.0) - this.HKLAzimutAngle(hKL);
        //    double correctedAzimutAngle = this.HKLAzimutAngle(hKL);
        //    double polarAngle = this.HKLPolarAngle(hKL);

        //    double Ret = Math.Cos((rotationAngle * Math.PI) / 180.0) * Math.Cos(correctedAzimutAngle) * Math.Cos(polarAngle);
        //    Ret -= Math.Sin((rotationAngle * Math.PI) / 180.0) * Math.Sin(correctedAzimutAngle);

        //    return Ret;
        //}

        //public double MCTM12(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        //{
        //    //double correctedAzimutAngle = (Math.PI / 2.0) - this.HKLAzimutAngle(hKL);
        //    double correctedAzimutAngle = this.HKLAzimutAngle(hKL);
        //    double polarAngle = this.HKLPolarAngle(hKL);

        //    double Ret = Math.Cos((rotationAngle * Math.PI) / 180.0) * Math.Sin(correctedAzimutAngle) * Math.Cos(polarAngle);
        //    Ret += Math.Sin((rotationAngle * Math.PI) / 180.0) * Math.Cos(correctedAzimutAngle);

        //    return Ret;
        //}

        //public double MCTM13(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        //{
        //    //double correctedAzimutAngle = (Math.PI / 2.0) - this.HKLAzimutAngle(hKL);
        //    //double correctedAzimutAngle = this.HKLAzimutAngle(hKL);
        //    double polarAngle = this.HKLPolarAngle(hKL);

        //    double Ret = Math.Cos((rotationAngle * Math.PI) / 180.0) * Math.Sin(polarAngle);

        //    return Ret;
        //}

        //public double MCTM21(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        //{
        //    //double correctedAzimutAngle = (Math.PI / 2.0) - this.HKLAzimutAngle(hKL);
        //    double correctedAzimutAngle = this.HKLAzimutAngle(hKL);
        //    double polarAngle = this.HKLPolarAngle(hKL);

        //    double Ret = -1.0 * Math.Sin((rotationAngle * Math.PI) / 180.0) * Math.Cos(correctedAzimutAngle) * Math.Cos(polarAngle);
        //    Ret -= Math.Cos((rotationAngle * Math.PI) / 180.0) * Math.Cos(correctedAzimutAngle);

        //    return Ret;
        //}

        //public double MCTM22(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        //{
        //    //double correctedAzimutAngle = (Math.PI / 2.0) - this.HKLAzimutAngle(hKL);
        //    double correctedAzimutAngle = this.HKLAzimutAngle(hKL);
        //    double polarAngle = this.HKLPolarAngle(hKL);

        //    double Ret = -1.0 * Math.Sin((rotationAngle * Math.PI) / 180.0) * Math.Sin(correctedAzimutAngle) * Math.Cos(polarAngle);
        //    Ret += Math.Cos((rotationAngle * Math.PI) / 180.0) * Math.Cos(correctedAzimutAngle);

        //    return Ret;
        //}

        //public double MCTM23(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        //{
        //    //double correctedAzimutAngle = (Math.PI / 2.0) - this.HKLAzimutAngle(hKL);
        //    //double correctedAzimutAngle = this.HKLAzimutAngle(hKL);
        //    double polarAngle = this.HKLPolarAngle(hKL);

        //    double Ret = -1.0 * Math.Sin((rotationAngle * Math.PI) / 180.0) * Math.Sin(polarAngle);

        //    return Ret;
        //}

        //public double MCTM31(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        //{
        //    //double correctedAzimutAngle = (Math.PI / 2.0) - this.HKLAzimutAngle(hKL);
        //    double correctedAzimutAngle = this.HKLAzimutAngle(hKL);
        //    double polarAngle = this.HKLPolarAngle(hKL);

        //    double Ret = -1.0 * Math.Cos(correctedAzimutAngle) * Math.Sin(polarAngle);

        //    return Ret;
        //}

        //public double MCTM32(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        //{
        //    //double correctedAzimutAngle = (Math.PI / 2.0) - this.HKLAzimutAngle(hKL);
        //    double correctedAzimutAngle = this.HKLAzimutAngle(hKL);
        //    double polarAngle = this.HKLPolarAngle(hKL);

        //    double Ret = -1 * Math.Sin(correctedAzimutAngle) * Math.Sin(polarAngle);

        //    return Ret;
        //}

        //public double MCTM33(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        //{
        //    //double correctedAzimutAngle = (Math.PI / 2.0) - this.HKLAzimutAngle(hKL);
        //    //double correctedAzimutAngle = this.HKLAzimutAngle(hKL);
        //    double polarAngle = this.HKLPolarAngle(hKL);

        //    double Ret = Math.Cos(polarAngle);

        //    return Ret;
        //}

        #endregion

        #region Martin Implementierung

        public MathNet.Numerics.LinearAlgebra.Matrix<double> GetMCTM(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        {
            MathNet.Numerics.LinearAlgebra.Matrix<double> ret = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

            ret[0, 0] += this.MCTM11(rotationAngle, hKL);
            ret[0, 1] += this.MCTM12(rotationAngle, hKL);
            ret[0, 2] += this.MCTM13(rotationAngle, hKL);
            ret[1, 0] += this.MCTM21(rotationAngle, hKL);
            ret[1, 1] += this.MCTM22(rotationAngle, hKL);
            ret[1, 2] += this.MCTM23(rotationAngle, hKL);
            ret[2, 0] += this.MCTM31(rotationAngle, hKL);
            ret[2, 1] += this.MCTM32(rotationAngle, hKL);
            ret[2, 2] += this.MCTM33(rotationAngle, hKL);

            return ret;
        }

        public double MCTM11(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        {
            double correctedAzimutAngle = (Math.PI / 2.0) - this.HKLAzimutAngle(hKL);
            //double correctedAzimutAngle = this.HKLAzimutAngle(hKL);
            double polarAngle = this.HKLPolarAngle(hKL);

            double Ret = Math.Cos((rotationAngle * Math.PI) / 180.0) * Math.Sin(correctedAzimutAngle);
            Ret -= Math.Sin((rotationAngle * Math.PI) / 180.0) * Math.Cos(correctedAzimutAngle) * Math.Cos(polarAngle);

            return Ret;
        }

        public double MCTM12(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        {
            double correctedAzimutAngle = (Math.PI / 2.0) - this.HKLAzimutAngle(hKL);
            //double correctedAzimutAngle = this.HKLAzimutAngle(hKL);
            double polarAngle = this.HKLPolarAngle(hKL);

            double Ret = Math.Sin((rotationAngle * Math.PI) / 180.0) * Math.Sin(correctedAzimutAngle);
            Ret -= Math.Cos((rotationAngle * Math.PI) / 180.0) * Math.Cos(correctedAzimutAngle) * Math.Cos(polarAngle);

            return Ret;
        }

        public double MCTM13(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        {
            double correctedAzimutAngle = (Math.PI / 2.0) - this.HKLAzimutAngle(hKL);
            //double correctedAzimutAngle = this.HKLAzimutAngle(hKL);
            double polarAngle = this.HKLPolarAngle(hKL);

            double Ret = Math.Cos(correctedAzimutAngle) * Math.Sin(polarAngle);

            return Ret;
        }

        public double MCTM21(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        {
            double correctedAzimutAngle = (Math.PI / 2.0) - this.HKLAzimutAngle(hKL);
            //double correctedAzimutAngle = this.HKLAzimutAngle(hKL);
            double polarAngle = this.HKLPolarAngle(hKL);

            double Ret = -1 * Math.Cos((rotationAngle * Math.PI) / 180.0) * Math.Cos(correctedAzimutAngle);
            Ret -= Math.Sin((rotationAngle * Math.PI) / 180.0) * Math.Sin(correctedAzimutAngle) * Math.Cos(polarAngle);

            return Ret;
        }

        public double MCTM22(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        {
            double correctedAzimutAngle = (Math.PI / 2.0) - this.HKLAzimutAngle(hKL);
            //double correctedAzimutAngle = this.HKLAzimutAngle(hKL);
            double polarAngle = this.HKLPolarAngle(hKL);

            double Ret = -1 * Math.Sin((rotationAngle * Math.PI) / 180.0) * Math.Cos(correctedAzimutAngle);
            Ret += Math.Cos((rotationAngle * Math.PI) / 180.0) * Math.Sin(correctedAzimutAngle) * Math.Cos(polarAngle);

            return Ret;
        }

        public double MCTM23(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        {
            double correctedAzimutAngle = (Math.PI / 2.0) - this.HKLAzimutAngle(hKL);
            //double correctedAzimutAngle = this.HKLAzimutAngle(hKL);
            double polarAngle = this.HKLPolarAngle(hKL);

            double Ret = Math.Sin(correctedAzimutAngle) * Math.Sin(polarAngle);

            return Ret;
        }

        public double MCTM31(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        {
            //double correctedAzimutAngle = (Math.PI / 2.0) - this.HKLAzimutAngle(hKL);
            //double correctedAzimutAngle = this.HKLAzimutAngle(hKL);
            double polarAngle = this.HKLPolarAngle(hKL);

            double Ret = Math.Sin((rotationAngle * Math.PI) / 180.0) * Math.Sin(polarAngle);

            return Ret;
        }

        public double MCTM32(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        {
            //double correctedAzimutAngle = (Math.PI / 2.0) - this.HKLAzimutAngle(hKL);
            //double correctedAzimutAngle = this.HKLAzimutAngle(hKL);
            double polarAngle = this.HKLPolarAngle(hKL);

            double Ret = -1 * Math.Cos((rotationAngle * Math.PI) / 180.0) * Math.Sin(polarAngle);

            return Ret;
        }

        public double MCTM33(double rotationAngle, DataManagment.CrystalData.HKLReflex hKL)
        {
            //double correctedAzimutAngle = (Math.PI / 2.0) - this.HKLAzimutAngle(hKL);
            //double correctedAzimutAngle = this.HKLAzimutAngle(hKL);
            double polarAngle = this.HKLPolarAngle(hKL);

            double Ret = Math.Cos(polarAngle);

            return Ret;
        }

        #endregion

        #endregion

        //#region Sample-Measurement-System transormation Matrix (komisch)

        ///// <summary>
        ///// Transforms from the sample into the Measurement system
        ///// </summary>
        ///// <param name="phi">Azimutangle of the scattering vektor</param>
        ///// <param name="psi">Polar angle of the scattering vektor</param>
        ///// <param name="varPhi2">unknown function</param>
        ///// <returns></returns>
        //public MathNet.Numerics.LinearAlgebra.Matrix<double> GetSMTM(double phi, double psi, double varPhi2)
        //{
        //    MathNet.Numerics.LinearAlgebra.Matrix<double> ret = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

        //    ret[0, 0] += this.SMTM11(phi, psi, varPhi2);
        //    ret[0, 1] += this.SMTM12(phi, psi, varPhi2);
        //    ret[0, 2] += this.SMTM13(phi, psi, varPhi2);
        //    ret[1, 0] += this.SMTM21(phi, psi, varPhi2);
        //    ret[1, 1] += this.SMTM22(phi, psi, varPhi2);
        //    ret[1, 2] += this.SMTM23(phi, psi, varPhi2);
        //    ret[2, 0] += this.SMTM31(phi, psi, varPhi2);
        //    ret[2, 1] += this.SMTM32(phi, psi, varPhi2);
        //    ret[2, 2] += this.SMTM33(phi, psi, varPhi2);

        //    return ret;
        //}

        //public double SMTM11(double phi, double psi, double varPhi2)
        //{
        //    double Ret = -1 * Math.Sin((phi * Math.PI) / 180.0) * Math.Cos((varPhi2 * Math.PI) / 180.0);
        //    Ret -= Math.Cos((psi * Math.PI) / 180.0) * Math.Cos((phi * Math.PI) / 180.0) * Math.Sin((varPhi2 * Math.PI) / 180.0);

        //    return Ret;
        //}

        //public double SMTM12(double phi, double psi, double varPhi2)
        //{
        //    double Ret = Math.Cos((phi * Math.PI) / 180.0) * Math.Cos((varPhi2 * Math.PI) / 180.0);
        //    Ret -= Math.Cos((psi * Math.PI) / 180.0) * Math.Sin((phi * Math.PI) / 180.0) * Math.Sin((varPhi2 * Math.PI) / 180.0);

        //    return Ret;
        //}

        //public double SMTM13(double phi, double psi, double varPhi2)
        //{
        //    double Ret = Math.Sin((psi * Math.PI) / 180.0) * Math.Sin((varPhi2 * Math.PI) / 180.0);

        //    return Ret;
        //}

        //public double SMTM21(double phi, double psi, double varPhi2)
        //{
        //    double Ret = Math.Sin((phi * Math.PI) / 180.0) * Math.Sin((varPhi2 * Math.PI) / 180.0);
        //    Ret -= Math.Cos((psi * Math.PI) / 180.0) * Math.Cos((phi * Math.PI) / 180.0) * Math.Cos((varPhi2 * Math.PI) / 180.0);

        //    return Ret;
        //}

        //public double SMTM22(double phi, double psi, double varPhi2)
        //{
        //    double Ret = -1 * Math.Cos((phi * Math.PI) / 180.0) * Math.Sin((varPhi2 * Math.PI) / 180.0);
        //    Ret -= Math.Cos((psi * Math.PI) / 180.0) * Math.Sin((phi * Math.PI) / 180.0) * Math.Cos((varPhi2 * Math.PI) / 180.0);

        //    return Ret;
        //}

        //public double SMTM23(double phi, double psi, double varPhi2)
        //{
        //    double Ret = Math.Sin((psi * Math.PI) / 180.0) * Math.Cos((varPhi2 * Math.PI) / 180.0);

        //    return Ret;
        //}

        //public double SMTM31(double phi, double psi, double varPhi2)
        //{
        //    double Ret = Math.Sin((psi * Math.PI) / 180.0) * Math.Cos((phi * Math.PI) / 180.0);

        //    return Ret;
        //}

        //public double SMTM32(double phi, double psi, double varPhi2)
        //{
        //    double Ret = Math.Sin((psi * Math.PI) / 180.0) * Math.Sin((phi * Math.PI) / 180.0);

        //    return Ret;
        //}

        //public double SMTM33(double phi, double psi, double varPhi2)
        //{
        //    double Ret = Math.Cos((psi * Math.PI) / 180.0);

        //    return Ret;
        //}

        //#endregion

        #region Sample-Measurement-System transormation Matrix (Behnken)

        /// <summary>
        /// Transforms from the sample into the Measurement system
        /// </summary>
        /// <param name="phi">Azimutangle of the scattering vektor</param>
        /// <param name="psi">Polar angle of the scattering vektor</param>
        /// <param name="varPhi2">unknown function</param>
        /// <returns></returns>
        public MathNet.Numerics.LinearAlgebra.Matrix<double> GetSMTM(double phi, double psi)
        {
            MathNet.Numerics.LinearAlgebra.Matrix<double> ret = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

            ret[0, 0] += this.SMTM11(phi, psi);
            ret[0, 1] += this.SMTM12(phi, psi);
            ret[0, 2] += this.SMTM13(phi, psi);
            ret[1, 0] += this.SMTM21(phi, psi);
            ret[1, 1] += this.SMTM22(phi, psi);
            ret[1, 2] += this.SMTM23(phi, psi);
            ret[2, 0] += this.SMTM31(phi, psi);
            ret[2, 1] += this.SMTM32(phi, psi);
            ret[2, 2] += this.SMTM33(phi, psi);

            return ret;
        }

        public double SMTM11(double phi, double psi)
        {
            double correctedPhi = phi + 90.0;
            double Ret = Math.Cos((correctedPhi * Math.PI) / 180.0) * Math.Cos((psi * Math.PI) / 180.0);

            return Ret;
        }

        public double SMTM12(double phi, double psi)
        {
            double correctedPhi = phi + 90.0;
            double Ret = Math.Sin((correctedPhi * Math.PI) / 180.0) * Math.Cos((psi * Math.PI) / 180.0);

            return Ret;
        }

        public double SMTM13(double phi, double psi)
        {
            double Ret = -1.0 * Math.Sin((psi * Math.PI) / 180.0);

            return Ret;
        }

        public double SMTM21(double phi, double psi)
        {
            double correctedPhi = phi + 90.0;
            double Ret = -1.0 * Math.Sin((correctedPhi * Math.PI) / 180.0);

            return Ret;
        }

        public double SMTM22(double phi, double psi)
        {
            double correctedPhi = phi + 90.0;
            double Ret = Math.Cos((correctedPhi * Math.PI) / 180.0);

            return Ret;
        }

        public double SMTM23(double phi, double psi)
        {
            double Ret = 0;

            return Ret;
        }

        public double SMTM31(double phi, double psi)
        {
            double correctedPhi = phi + 90.0;
            double Ret = Math.Sin((psi * Math.PI) / 180.0) * Math.Cos((correctedPhi * Math.PI) / 180.0);

            return Ret;
        }

        public double SMTM32(double phi, double psi)
        {
            double correctedPhi = phi + 90.0;
            double Ret = Math.Sin((psi * Math.PI) / 180.0) * Math.Sin((correctedPhi * Math.PI) / 180.0);

            return Ret;
        }

        public double SMTM33(double phi, double psi)
        {
            double Ret = Math.Cos((psi * Math.PI) / 180.0);

            return Ret;
        }

        #endregion

        #endregion

        #endregion

        #region Fitting

        public void FitVoigt(bool classicCalculation)
        {
            switch (this.BaseTensor.Symmetry)
            {
                case "cubic":
                    this.BaseTensor.FitConverged = Analysis.Fitting.LMA.FitNElasticityTensorTextureVoigtCubic(this);
                    break;
                case "hexagonal":
                    this.BaseTensor.FitConverged = Analysis.Fitting.LMA.FitNElasticityTensorTextureVoigtType1(this, classicCalculation);
                    break;
                case "tetragonal type 1":
                    this.BaseTensor.FitConverged = Analysis.Fitting.LMA.FitNElasticityTensorTextureVoigtType2(this, classicCalculation);
                    break;
                case "tetragonal type 2":
                    this.BaseTensor.FitConverged = Analysis.Fitting.LMA.FitNElasticityTensorTextureVoigtType2(this, classicCalculation);
                    break;
                case "trigonal type 1":
                    this.BaseTensor.FitConverged = Analysis.Fitting.LMA.FitNElasticityTensorTextureVoigtType1(this, classicCalculation);
                    break;
                case "trigonal type 2":
                    this.BaseTensor.FitConverged = Analysis.Fitting.LMA.FitNElasticityTensorTextureVoigtType1(this, classicCalculation);
                    break;
                case "rhombic":
                    this.BaseTensor.FitConverged = Analysis.Fitting.LMA.FitNElasticityTensorTextureVoigtType3(this, classicCalculation);
                    break;
                case "monoclinic":
                    this.BaseTensor.FitConverged = Analysis.Fitting.LMA.FitNElasticityTensorTextureVoigtType3(this, classicCalculation);
                    break;
                case "triclinic":
                    this.BaseTensor.FitConverged = Analysis.Fitting.LMA.FitNElasticityTensorTextureVoigtType3(this, classicCalculation);
                    break;
                default:
                    this.BaseTensor.FitConverged = Analysis.Fitting.LMA.FitNElasticityTensorTextureVoigtType3(this, classicCalculation);
                    break;
            }

            this.BaseTensor.CalculateCompliances();
        }

        public void FitReuss(bool classicCalculation)
        {
            switch (this.BaseTensor.Symmetry)
            {
                case "cubic":
                    this.BaseTensor.FitConverged = Analysis.Fitting.LMA.FitNElasticityTensorTextureReussCubic(this);
                    break;
                case "hexagonal":
                    this.BaseTensor.FitConverged = Analysis.Fitting.LMA.FitNElasticityTensorTextureReussHexagonal(this, classicCalculation);
                    break;
                case "tetragonal type 1":

                    break;
                case "tetragonal type 2":

                    break;
                case "trigonal type 1":

                    break;
                case "trigonal type 2":

                    break;
                case "rhombic":

                    break;
                case "monoclinic":

                    break;
                case "triclinic":

                    break;
                default:

                    break;
            }

            this.BaseTensor.CalculateStiffnesses();
            this.BaseTensor.SetFittingErrorsReuss(classicCalculation);
        }

        public void FitHill(bool classicCalculation)
        {
            switch (this.BaseTensor.Symmetry)
            {
                case "cubic":
                    //this.BaseTensor.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorTextureHillCubic(this);
                    break;
                case "hexagonal":
                    //this.BaseTensor.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorTextureHillHexagonal(this, classicCalculation);
                    break;
                case "tetragonal type 1":

                    break;
                case "tetragonal type 2":

                    break;
                case "trigonal type 1":

                    break;
                case "trigonal type 2":

                    break;
                case "rhombic":

                    break;
                case "monoclinic":

                    break;
                case "triclinic":

                    break;
                default:

                    break;
            }

            this.BaseTensor.CalculateStiffnesses();
            this.BaseTensor.SetFittingErrorsHill(classicCalculation);
        }

        public void FitKroener(bool classicCalculation, bool stiffnessCalc)
        {
            if (stiffnessCalc)
            {
                switch (this.BaseTensor.Symmetry)
                {
                    case "cubic":
                        //this.BaseTensor.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorTextureKroenerCubicStiffness(this, classicCalculation);
                        break;
                    case "hexagonal":
                        break;
                    case "tetragonal type 1":

                        break;
                    case "tetragonal type 2":

                        break;
                    case "trigonal type 1":

                        break;
                    case "trigonal type 2":

                        break;
                    case "rhombic":

                        break;
                    case "monoclinic":

                        break;
                    case "triclinic":

                        break;
                    default:

                        break;
                }

                this.BaseTensor.CalculateCompliances();
                this.BaseTensor.SetFittingErrorsKroener(classicCalculation);
            }
            else
            {
                switch (this.BaseTensor.Symmetry)
                {
                    case "cubic":
                        //this.BaseTensor.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorTextureKroenerCubicCompliance(this, classicCalculation);
                        break;
                    case "hexagonal":
                        break;
                    case "tetragonal type 1":

                        break;
                    case "tetragonal type 2":

                        break;
                    case "trigonal type 1":

                        break;
                    case "trigonal type 2":

                        break;
                    case "rhombic":

                        break;
                    case "monoclinic":

                        break;
                    case "triclinic":

                        break;
                    default:

                        break;
                }

                this.BaseTensor.CalculateStiffnesses();
                this.BaseTensor.SetFittingErrorsKroener(classicCalculation);
            }
        }

        public void FitDeWitt(bool classicCalculation, bool stiffnessCalc)
        {
            if (stiffnessCalc)
            {
                switch (this.BaseTensor.Symmetry)
                {
                    case "cubic":
                        //this.BaseTensor.FitConverged = Analysis.Fitting.LMA.FitNElasticityTensorTextureDeWittCubicStiffness(this, classicCalculation);
                        break;
                    case "hexagonal":
                        break;
                    case "tetragonal type 1":

                        break;
                    case "tetragonal type 2":

                        break;
                    case "trigonal type 1":

                        break;
                    case "trigonal type 2":

                        break;
                    case "rhombic":

                        break;
                    case "monoclinic":

                        break;
                    case "triclinic":

                        break;
                    default:

                        break;
                }

                this.BaseTensor.CalculateCompliances();
                this.BaseTensor.SetFittingErrorsDeWitt(classicCalculation);
            }
            else
            {
                switch (this.BaseTensor.Symmetry)
                {
                    case "cubic":
                        //this.BaseTensor.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorTextureDeWittCubicCompliance(this, classicCalculation);
                        break;
                    case "hexagonal":
                        break;
                    case "tetragonal type 1":

                        break;
                    case "tetragonal type 2":

                        break;
                    case "trigonal type 1":

                        break;
                    case "trigonal type 2":

                        break;
                    case "rhombic":

                        break;
                    case "monoclinic":

                        break;
                    case "triclinic":

                        break;
                    default:

                        break;
                }

                this.BaseTensor.CalculateStiffnesses();
                this.BaseTensor.SetFittingErrorsDeWitt(classicCalculation);
            }
        }

        public void FitGeometricHill(bool classicCalculation)
        {
            switch (this.BaseTensor.Symmetry)
            {
                case "cubic":
                    //this.BaseTensor.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorTextureGeometricHillCubic(this, classicCalculation);
                    break;
                case "hexagonal":
                    //this.BaseTensor.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorTextureGeometricHillHexagonal(this, classicCalculation);
                    break;
                case "tetragonal type 1":

                    break;
                case "tetragonal type 2":

                    break;
                case "trigonal type 1":

                    break;
                case "trigonal type 2":

                    break;
                case "rhombic":

                    break;
                case "monoclinic":

                    break;
                case "triclinic":

                    break;
                default:

                    break;
            }

            this.BaseTensor.CalculateStiffnesses();
            this.BaseTensor.SetFittingErrorsHill(classicCalculation);
        }

        #region parameter delta calculation

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C12
        ///[2] C44
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorF33VoigtCubic(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            //[2][2] C44
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

            //[0] C11
            //[1] C12
            //[1] C44
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);
            int remains = this.UsedPSA.Count % 10;

            Parallel.For(0, 10, i =>
            {
                int step = (this.UsedPSA.Count - remains) / 10;

                int lowerBoarder = i * step;
                int upperBoarder = (i + 1) * step;


                for (int n = lowerBoarder; n < upperBoarder; n++)
                {
                    #region Matrix Build

                    HessianMatrix[0, 0] += (this.GetStrainVoigtCubicFD(this.UsedPSA[n], 0) * this.GetStrainVoigtCubicFD(this.UsedPSA[n], 0)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
        
                    HessianMatrix[1, 1] += (this.GetStrainVoigtCubicFD(this.UsedPSA[n], 1) * this.GetStrainVoigtCubicFD(this.UsedPSA[n], 1)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                    HessianMatrix[0, 1] += (this.GetStrainVoigtCubicFD(this.UsedPSA[n], 1) * this.GetStrainVoigtCubicFD(this.UsedPSA[n], 0)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                    HessianMatrix[1, 0] += (this.GetStrainVoigtCubicFD(this.UsedPSA[n], 1) * this.GetStrainVoigtCubicFD(this.UsedPSA[n], 0)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);

                    HessianMatrix[2, 2] += (this.GetStrainVoigtCubicFD(this.UsedPSA[n], 2) * this.GetStrainVoigtCubicFD(this.UsedPSA[n], 2)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                    HessianMatrix[0, 2] += (this.GetStrainVoigtCubicFD(this.UsedPSA[n], 2) * this.GetStrainVoigtCubicFD(this.UsedPSA[n], 0)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                    HessianMatrix[2, 0] += (this.GetStrainVoigtCubicFD(this.UsedPSA[n], 2) * this.GetStrainVoigtCubicFD(this.UsedPSA[n], 0)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                    HessianMatrix[1, 2] += (this.GetStrainVoigtCubicFD(this.UsedPSA[n], 2) * this.GetStrainVoigtCubicFD(this.UsedPSA[n], 1)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                    HessianMatrix[2, 1] += (this.GetStrainVoigtCubicFD(this.UsedPSA[n], 2) * this.GetStrainVoigtCubicFD(this.UsedPSA[n], 1)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                    #endregion

                    #region Vector build

                    SolutionVector[0] += (this.UsedPSA[n]._Strain - this.GetStrainVoigtCubic(this.UsedPSA[n]) / Math.Pow(this.UsedPSA[n]._StrainError, 2)) * this.GetStrainVoigtCubicFD(this.UsedPSA[n], 0);
                    SolutionVector[1] += (this.UsedPSA[n]._Strain - this.GetStrainVoigtCubic(this.UsedPSA[n]) / Math.Pow(this.UsedPSA[n]._StrainError, 2)) * this.GetStrainVoigtCubicFD(this.UsedPSA[n], 1);
                    SolutionVector[2] += (this.UsedPSA[n]._Strain - this.GetStrainVoigtCubic(this.UsedPSA[n]) / Math.Pow(this.UsedPSA[n]._StrainError, 2)) * this.GetStrainVoigtCubicFD(this.UsedPSA[n], 2);

                        #endregion
                }
            });

            for (int n = 0; n < 3; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vector
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] S11
        ///[1] S12
        ///[2] S44
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorF33ReussCubic(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            //[2][2] C44
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

            //[0] C11
            //[1] C12
            //[1] C44
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            int remains = this.UsedPSA.Count % 10;

            Parallel.For(0, 10, i =>
            {
                int step = (this.UsedPSA.Count - remains) / 10;

                int lowerBoarder = i * step;
                int upperBoarder = (i + 1) * step;

                for (int n = lowerBoarder; n < upperBoarder; n++)
                {
                    #region Matrix Build

                    HessianMatrix[0, 0] += (this.GetStrainReussCubicFD(this.UsedPSA[n], 0) * this.GetStrainReussCubicFD(this.UsedPSA[n], 0)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);

                    HessianMatrix[1, 1] += (this.GetStrainReussCubicFD(this.UsedPSA[n], 1) * this.GetStrainReussCubicFD(this.UsedPSA[n], 1)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                    HessianMatrix[0, 1] += (this.GetStrainReussCubicFD(this.UsedPSA[n], 1) * this.GetStrainReussCubicFD(this.UsedPSA[n], 0)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                    HessianMatrix[1, 0] += (this.GetStrainReussCubicFD(this.UsedPSA[n], 1) * this.GetStrainReussCubicFD(this.UsedPSA[n], 0)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);

                    HessianMatrix[2, 2] += (this.GetStrainReussCubicFD(this.UsedPSA[n], 2) * this.GetStrainReussCubicFD(this.UsedPSA[n], 2)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                    HessianMatrix[0, 2] += (this.GetStrainReussCubicFD(this.UsedPSA[n], 2) * this.GetStrainReussCubicFD(this.UsedPSA[n], 0)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                    HessianMatrix[2, 0] += (this.GetStrainReussCubicFD(this.UsedPSA[n], 2) * this.GetStrainReussCubicFD(this.UsedPSA[n], 0)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                    HessianMatrix[1, 2] += (this.GetStrainReussCubicFD(this.UsedPSA[n], 2) * this.GetStrainReussCubicFD(this.UsedPSA[n], 1)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                    HessianMatrix[2, 1] += (this.GetStrainReussCubicFD(this.UsedPSA[n], 2) * this.GetStrainReussCubicFD(this.UsedPSA[n], 1)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                    #endregion

                    #region Vector build

                    SolutionVector[0] += (this.UsedPSA[n]._Strain - this.GetStrainReussCubic(this.UsedPSA[n]) / Math.Pow(this.UsedPSA[n]._StrainError, 2)) * this.GetStrainReussCubicFD(this.UsedPSA[n], 0);
                    SolutionVector[1] += (this.UsedPSA[n]._Strain - this.GetStrainReussCubic(this.UsedPSA[n]) / Math.Pow(this.UsedPSA[n]._StrainError, 2)) * this.GetStrainReussCubicFD(this.UsedPSA[n], 1);
                    SolutionVector[2] += (this.UsedPSA[n]._Strain - this.GetStrainReussCubic(this.UsedPSA[n]) / Math.Pow(this.UsedPSA[n]._StrainError, 2)) * this.GetStrainReussCubicFD(this.UsedPSA[n], 2);

                    #endregion
                }
            });

            for (int n = 0; n < 3; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vector
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] S11
        ///[1] S12
        ///[2] S44
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorF33HillCubic(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            //[2][2] C44
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

            //[0] C11
            //[1] C12
            //[1] C44
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            int remains = this.UsedPSA.Count % 10;

            Parallel.For(0, 10, i =>
            {
                int step = (this.UsedPSA.Count - remains) / 10;

                int lowerBoarder = i * step;
                int upperBoarder = (i + 1) * step;

                for (int n = lowerBoarder; n < upperBoarder; n++)
                {
                    #region Matrix Build

                HessianMatrix[0, 0] += (this.GetStrainHillCubicFD(this.UsedPSA[n], 0) * this.GetStrainHillCubicFD(this.UsedPSA[n], 0)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);

                HessianMatrix[1, 1] += (this.GetStrainHillCubicFD(this.UsedPSA[n], 1) * this.GetStrainHillCubicFD(this.UsedPSA[n], 1)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[0, 1] += (this.GetStrainHillCubicFD(this.UsedPSA[n], 1) * this.GetStrainHillCubicFD(this.UsedPSA[n], 0)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[1, 0] += (this.GetStrainHillCubicFD(this.UsedPSA[n], 1) * this.GetStrainHillCubicFD(this.UsedPSA[n], 0)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);

                HessianMatrix[2, 2] += (this.GetStrainHillCubicFD(this.UsedPSA[n], 2) * this.GetStrainHillCubicFD(this.UsedPSA[n], 2)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[0, 2] += (this.GetStrainHillCubicFD(this.UsedPSA[n], 2) * this.GetStrainHillCubicFD(this.UsedPSA[n], 0)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[2, 0] += (this.GetStrainHillCubicFD(this.UsedPSA[n], 2) * this.GetStrainHillCubicFD(this.UsedPSA[n], 0)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[1, 2] += (this.GetStrainHillCubicFD(this.UsedPSA[n], 2) * this.GetStrainHillCubicFD(this.UsedPSA[n], 1)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[2, 1] += (this.GetStrainHillCubicFD(this.UsedPSA[n], 2) * this.GetStrainHillCubicFD(this.UsedPSA[n], 1)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                #endregion

                    #region Vector build

                    SolutionVector[0] += (this.UsedPSA[n]._Strain - this.GetStrainHillCubic(this.UsedPSA[n]) / Math.Pow(this.UsedPSA[n]._StrainError, 2)) * this.GetStrainHillCubicFD(this.UsedPSA[n], 0);
                    SolutionVector[1] += (this.UsedPSA[n]._Strain - this.GetStrainHillCubic(this.UsedPSA[n]) / Math.Pow(this.UsedPSA[n]._StrainError, 2)) * this.GetStrainHillCubicFD(this.UsedPSA[n], 1);
                    SolutionVector[2] += (this.UsedPSA[n]._Strain - this.GetStrainHillCubic(this.UsedPSA[n]) / Math.Pow(this.UsedPSA[n]._StrainError, 2)) * this.GetStrainHillCubicFD(this.UsedPSA[n], 2);

                        #endregion
                }
            });

            for (int n = 0; n < 3; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C12
        ///[2] C44
        ///[3] C13
        ///[4] C33
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorF33VoigtHexagonal(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            //[2][2] C44
            //[3][3] C13
            //[4][4] C33
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(5, 5, 0.0);

            //[0] C11
            //[1] C12
            //[2] C44
            //[3] C13
            //[4] C33
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(5);

            int remains = this.UsedPSA.Count % 10;

            Parallel.For(0, 10, i =>
            {
                int step = (this.UsedPSA.Count - remains) / 10;

                int lowerBoarder = i * step;
                int upperBoarder = (i + 1) * step;

                for (int n = lowerBoarder; n < upperBoarder; n++)  
                {
                    #region Matrix Build

                HessianMatrix[0, 0] += (this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 0) * this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 0)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);

                HessianMatrix[1, 1] += (this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 1) * this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 1)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[0, 1] += (this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 1) * this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 0)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[1, 0] += (this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 1) * this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 0)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);

                HessianMatrix[2, 2] += (this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 2) * this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 2)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[0, 2] += (this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 2) * this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 0)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[2, 0] += (this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 2) * this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 0)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[1, 2] += (this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 2) * this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 1)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[2, 1] += (this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 2) * this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 1)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);

                HessianMatrix[3, 3] += (this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 3) * this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 3)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[0, 3] += (this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 3) * this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 0)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[3, 0] += (this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 3) * this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 0)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[1, 3] += (this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 3) * this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 1)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[3, 1] += (this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 3) * this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 1)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[2, 3] += (this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 3) * this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 2)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[3, 2] += (this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 3) * this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 2)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);

                HessianMatrix[4, 4] += (this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 4) * this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 4)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[0, 4] += (this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 4) * this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 0)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[4, 0] += (this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 4) * this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 0)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[1, 4] += (this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 4) * this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 1)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[4, 1] += (this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 4) * this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 1)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[2, 4] += (this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 4) * this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 2)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[4, 2] += (this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 4) * this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 2)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[3, 4] += (this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 4) * this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 3)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[4, 3] += (this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 4) * this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 3)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);

                #endregion

                    #region Vector build

                    SolutionVector[0] += (this.UsedPSA[n]._Strain - this.GetStrainVoigtHexagonal(this.UsedPSA[n]) / Math.Pow(this.UsedPSA[n]._StrainError, 2)) * this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 0);
                    SolutionVector[1] += (this.UsedPSA[n]._Strain - this.GetStrainVoigtHexagonal(this.UsedPSA[n]) / Math.Pow(this.UsedPSA[n]._StrainError, 2)) * this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 1);
                    SolutionVector[2] += (this.UsedPSA[n]._Strain - this.GetStrainVoigtHexagonal(this.UsedPSA[n]) / Math.Pow(this.UsedPSA[n]._StrainError, 2)) * this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 2);
                    SolutionVector[3] += (this.UsedPSA[n]._Strain - this.GetStrainVoigtHexagonal(this.UsedPSA[n]) / Math.Pow(this.UsedPSA[n]._StrainError, 2)) * this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 3);
                    SolutionVector[4] += (this.UsedPSA[n]._Strain - this.GetStrainVoigtHexagonal(this.UsedPSA[n]) / Math.Pow(this.UsedPSA[n]._StrainError, 2)) * this.GetStrainVoigtHexagonalFD(this.UsedPSA[n], 4);

                    #endregion
                }
            });

            for (int n = 0; n < 5; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(5);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vector
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] S11
        ///[1] S12
        ///[2] S44
        ///[3] C13
        ///[4] C33
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorF33ReussHexagonal(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            //[2][2] C44
            //[3][3] C13
            //[4][4] C33
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(5, 5, 0.0);

            //[0] C11
            //[1] C12
            //[1] C44
            //[3] C13
            //[4] C33
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(5);

            int remains = this.UsedPSA.Count % 10;

            Parallel.For(0, 10, i =>
            {
                int step = (this.UsedPSA.Count - remains) / 10;

                int lowerBoarder = i * step;
                int upperBoarder = (i + 1) * step;

                for (int n = lowerBoarder; n < upperBoarder; n++)
                {
                    #region Matrix Build

                HessianMatrix[0, 0] += (this.GetStrainReussHexagonalFD(this.UsedPSA[n], 0) * this.GetStrainReussHexagonalFD(this.UsedPSA[n], 0)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);

                HessianMatrix[1, 1] += (this.GetStrainReussHexagonalFD(this.UsedPSA[n], 1) * this.GetStrainReussHexagonalFD(this.UsedPSA[n], 1)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[0, 1] += (this.GetStrainReussHexagonalFD(this.UsedPSA[n], 1) * this.GetStrainReussHexagonalFD(this.UsedPSA[n], 0)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[1, 0] += (this.GetStrainReussHexagonalFD(this.UsedPSA[n], 1) * this.GetStrainReussHexagonalFD(this.UsedPSA[n], 0)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);

                HessianMatrix[2, 2] += (this.GetStrainReussHexagonalFD(this.UsedPSA[n], 2) * this.GetStrainReussHexagonalFD(this.UsedPSA[n], 2)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[0, 2] += (this.GetStrainReussHexagonalFD(this.UsedPSA[n], 2) * this.GetStrainReussHexagonalFD(this.UsedPSA[n], 0)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[2, 0] += (this.GetStrainReussHexagonalFD(this.UsedPSA[n], 2) * this.GetStrainReussHexagonalFD(this.UsedPSA[n], 0)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[1, 2] += (this.GetStrainReussHexagonalFD(this.UsedPSA[n], 2) * this.GetStrainReussHexagonalFD(this.UsedPSA[n], 1)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[2, 1] += (this.GetStrainReussHexagonalFD(this.UsedPSA[n], 2) * this.GetStrainReussHexagonalFD(this.UsedPSA[n], 1)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);

                HessianMatrix[3, 3] += (this.GetStrainReussHexagonalFD(this.UsedPSA[n], 3) * this.GetStrainReussHexagonalFD(this.UsedPSA[n], 3)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[0, 3] += (this.GetStrainReussHexagonalFD(this.UsedPSA[n], 3) * this.GetStrainReussHexagonalFD(this.UsedPSA[n], 0)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[3, 0] += (this.GetStrainReussHexagonalFD(this.UsedPSA[n], 3) * this.GetStrainReussHexagonalFD(this.UsedPSA[n], 0)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[1, 3] += (this.GetStrainReussHexagonalFD(this.UsedPSA[n], 3) * this.GetStrainReussHexagonalFD(this.UsedPSA[n], 1)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[3, 1] += (this.GetStrainReussHexagonalFD(this.UsedPSA[n], 3) * this.GetStrainReussHexagonalFD(this.UsedPSA[n], 1)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[2, 3] += (this.GetStrainReussHexagonalFD(this.UsedPSA[n], 3) * this.GetStrainReussHexagonalFD(this.UsedPSA[n], 2)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[3, 2] += (this.GetStrainReussHexagonalFD(this.UsedPSA[n], 3) * this.GetStrainReussHexagonalFD(this.UsedPSA[n], 2)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);

                HessianMatrix[4, 4] += (this.GetStrainReussHexagonalFD(this.UsedPSA[n], 4) * this.GetStrainReussHexagonalFD(this.UsedPSA[n], 4)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[0, 4] += (this.GetStrainReussHexagonalFD(this.UsedPSA[n], 4) * this.GetStrainReussHexagonalFD(this.UsedPSA[n], 0)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[4, 0] += (this.GetStrainReussHexagonalFD(this.UsedPSA[n], 4) * this.GetStrainReussHexagonalFD(this.UsedPSA[n], 0)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[1, 4] += (this.GetStrainReussHexagonalFD(this.UsedPSA[n], 4) * this.GetStrainReussHexagonalFD(this.UsedPSA[n], 1)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[4, 1] += (this.GetStrainReussHexagonalFD(this.UsedPSA[n], 4) * this.GetStrainReussHexagonalFD(this.UsedPSA[n], 1)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[2, 4] += (this.GetStrainReussHexagonalFD(this.UsedPSA[n], 4) * this.GetStrainReussHexagonalFD(this.UsedPSA[n], 2)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[4, 2] += (this.GetStrainReussHexagonalFD(this.UsedPSA[n], 4) * this.GetStrainReussHexagonalFD(this.UsedPSA[n], 2)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[3, 4] += (this.GetStrainReussHexagonalFD(this.UsedPSA[n], 4) * this.GetStrainReussHexagonalFD(this.UsedPSA[n], 3)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[4, 3] += (this.GetStrainReussHexagonalFD(this.UsedPSA[n], 4) * this.GetStrainReussHexagonalFD(this.UsedPSA[n], 3)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);

                #endregion

                    #region Vector build

                SolutionVector[0] += (this.UsedPSA[n]._Strain - this.GetStrainReussHexagonal(this.UsedPSA[n]) / Math.Pow(this.UsedPSA[n]._StrainError, 2)) * this.GetStrainReussHexagonalFD(this.UsedPSA[n], 0);
                SolutionVector[1] += (this.UsedPSA[n]._Strain - this.GetStrainReussHexagonal(this.UsedPSA[n]) / Math.Pow(this.UsedPSA[n]._StrainError, 2)) * this.GetStrainReussHexagonalFD(this.UsedPSA[n], 1);
                SolutionVector[2] += (this.UsedPSA[n]._Strain - this.GetStrainReussHexagonal(this.UsedPSA[n]) / Math.Pow(this.UsedPSA[n]._StrainError, 2)) * this.GetStrainReussHexagonalFD(this.UsedPSA[n], 2);
                SolutionVector[3] += (this.UsedPSA[n]._Strain - this.GetStrainReussHexagonal(this.UsedPSA[n]) / Math.Pow(this.UsedPSA[n]._StrainError, 2)) * this.GetStrainReussHexagonalFD(this.UsedPSA[n], 3);
                SolutionVector[4] += (this.UsedPSA[n]._Strain - this.GetStrainReussHexagonal(this.UsedPSA[n]) / Math.Pow(this.UsedPSA[n]._StrainError, 2)) * this.GetStrainReussHexagonalFD(this.UsedPSA[n], 4);

                        #endregion
                }
            });

            for (int n = 0; n < 5; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(5);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vector
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] S11
        ///[1] S12
        ///[2] S44
        ///[3] C13
        ///[4] C33
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorF33HillHexagonal(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            //[2][2] C44
            ///[3] C13
            ///[4] C33
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(5, 5, 0.0);

            //[0] C11
            //[1] C12
            //[2] C44
            //[3] C13
            //[4] C33
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(5);

            int remains = this.UsedPSA.Count % 10;

            Parallel.For(0, 10, i =>
            {
                int step = (this.UsedPSA.Count - remains) / 10;

                int lowerBoarder = i * step;
                int upperBoarder = (i + 1) * step;

                for (int n = lowerBoarder; n < upperBoarder; n++)
                {
                    #region Matrix Build

                HessianMatrix[0, 0] += (this.GetStrainHillHexagonalFD(this.UsedPSA[n], 0) * this.GetStrainHillHexagonalFD(this.UsedPSA[n], 0)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);

                HessianMatrix[1, 1] += (this.GetStrainHillHexagonalFD(this.UsedPSA[n], 1) * this.GetStrainHillHexagonalFD(this.UsedPSA[n], 1)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[0, 1] += (this.GetStrainHillHexagonalFD(this.UsedPSA[n], 1) * this.GetStrainHillHexagonalFD(this.UsedPSA[n], 0)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[1, 0] += (this.GetStrainHillHexagonalFD(this.UsedPSA[n], 1) * this.GetStrainHillHexagonalFD(this.UsedPSA[n], 0)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);

                HessianMatrix[2, 2] += (this.GetStrainHillHexagonalFD(this.UsedPSA[n], 2) * this.GetStrainHillHexagonalFD(this.UsedPSA[n], 2)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[0, 2] += (this.GetStrainHillHexagonalFD(this.UsedPSA[n], 2) * this.GetStrainHillHexagonalFD(this.UsedPSA[n], 0)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[2, 0] += (this.GetStrainHillHexagonalFD(this.UsedPSA[n], 2) * this.GetStrainHillHexagonalFD(this.UsedPSA[n], 0)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[1, 2] += (this.GetStrainHillHexagonalFD(this.UsedPSA[n], 2) * this.GetStrainHillHexagonalFD(this.UsedPSA[n], 1)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[2, 1] += (this.GetStrainHillHexagonalFD(this.UsedPSA[n], 2) * this.GetStrainHillHexagonalFD(this.UsedPSA[n], 1)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);

                HessianMatrix[3, 3] += (this.GetStrainHillHexagonalFD(this.UsedPSA[n], 3) * this.GetStrainHillHexagonalFD(this.UsedPSA[n], 3)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[0, 3] += (this.GetStrainHillHexagonalFD(this.UsedPSA[n], 3) * this.GetStrainHillHexagonalFD(this.UsedPSA[n], 0)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[3, 0] += (this.GetStrainHillHexagonalFD(this.UsedPSA[n], 3) * this.GetStrainHillHexagonalFD(this.UsedPSA[n], 0)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[1, 3] += (this.GetStrainHillHexagonalFD(this.UsedPSA[n], 3) * this.GetStrainHillHexagonalFD(this.UsedPSA[n], 1)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[3, 1] += (this.GetStrainHillHexagonalFD(this.UsedPSA[n], 3) * this.GetStrainHillHexagonalFD(this.UsedPSA[n], 1)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[2, 3] += (this.GetStrainHillHexagonalFD(this.UsedPSA[n], 3) * this.GetStrainHillHexagonalFD(this.UsedPSA[n], 2)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[3, 2] += (this.GetStrainHillHexagonalFD(this.UsedPSA[n], 3) * this.GetStrainHillHexagonalFD(this.UsedPSA[n], 2)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);

                HessianMatrix[4, 4] += (this.GetStrainHillHexagonalFD(this.UsedPSA[n], 4) * this.GetStrainHillHexagonalFD(this.UsedPSA[n], 4)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[0, 4] += (this.GetStrainHillHexagonalFD(this.UsedPSA[n], 4) * this.GetStrainHillHexagonalFD(this.UsedPSA[n], 0)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[4, 0] += (this.GetStrainHillHexagonalFD(this.UsedPSA[n], 4) * this.GetStrainHillHexagonalFD(this.UsedPSA[n], 0)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[1, 4] += (this.GetStrainHillHexagonalFD(this.UsedPSA[n], 4) * this.GetStrainHillHexagonalFD(this.UsedPSA[n], 1)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[4, 1] += (this.GetStrainHillHexagonalFD(this.UsedPSA[n], 4) * this.GetStrainHillHexagonalFD(this.UsedPSA[n], 1)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[2, 4] += (this.GetStrainHillHexagonalFD(this.UsedPSA[n], 4) * this.GetStrainHillHexagonalFD(this.UsedPSA[n], 2)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[4, 2] += (this.GetStrainHillHexagonalFD(this.UsedPSA[n], 4) * this.GetStrainHillHexagonalFD(this.UsedPSA[n], 2)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[3, 4] += (this.GetStrainHillHexagonalFD(this.UsedPSA[n], 4) * this.GetStrainHillHexagonalFD(this.UsedPSA[n], 3)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);
                HessianMatrix[4, 3] += (this.GetStrainHillHexagonalFD(this.UsedPSA[n], 4) * this.GetStrainHillHexagonalFD(this.UsedPSA[n], 3)) / Math.Pow(this.UsedPSA[n]._StrainError, 2);

                #endregion

                    #region Vector build

                    SolutionVector[0] += (this.UsedPSA[n]._Strain - this.GetStrainHillHexagonal(this.UsedPSA[n]) / Math.Pow(this.UsedPSA[n]._StrainError, 2)) * this.GetStrainHillHexagonalFD(this.UsedPSA[n], 0);
                    SolutionVector[1] += (this.UsedPSA[n]._Strain - this.GetStrainHillHexagonal(this.UsedPSA[n]) / Math.Pow(this.UsedPSA[n]._StrainError, 2)) * this.GetStrainHillHexagonalFD(this.UsedPSA[n], 1);
                    SolutionVector[2] += (this.UsedPSA[n]._Strain - this.GetStrainHillHexagonal(this.UsedPSA[n]) / Math.Pow(this.UsedPSA[n]._StrainError, 2)) * this.GetStrainHillHexagonalFD(this.UsedPSA[n], 2);
                    SolutionVector[3] += (this.UsedPSA[n]._Strain - this.GetStrainHillHexagonal(this.UsedPSA[n]) / Math.Pow(this.UsedPSA[n]._StrainError, 2)) * this.GetStrainHillHexagonalFD(this.UsedPSA[n], 3);
                    SolutionVector[4] += (this.UsedPSA[n]._Strain - this.GetStrainHillHexagonal(this.UsedPSA[n]) / Math.Pow(this.UsedPSA[n]._StrainError, 2)) * this.GetStrainHillHexagonalFD(this.UsedPSA[n], 4);

                        #endregion
                }
            });

            for (int n = 0; n < 5; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(5);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        #endregion

        #endregion

        #region Fitting using Multi Threading

        public int _fittingTrial = 0;
        public int FittingTrial
        {
            get
            {
                return this._fittingTrial;
            }
            set
            {
                this._fittingTrial = value;
                this.OnFitUpdated();

            }
        }
        public Tools.TextureFitInformation FitDisplayInfo;
        public DateTime FitActiveSince = new DateTime();

        public event System.ComponentModel.PropertyChangedEventHandler FitFinished;
        public event System.ComponentModel.PropertyChangedEventHandler FitStarted;
        public event System.ComponentModel.PropertyChangedEventHandler FitUpdated;

        protected void OnFitStarted()
        {
            this._fitConverged = false;
            this.fitActive = true;
            this.FittingTrial = 0;

            System.ComponentModel.PropertyChangedEventHandler handler = FitStarted;
            if (handler != null)
            {
                handler(this, new System.ComponentModel.PropertyChangedEventArgs("FitStarted"));
            }
        }

        protected void OnFitUpdated()
        {
            if (this.FitDisplayInfo != null)
            {
                this.FitDisplayInfo.LMATrial = this._fittingTrial;
            }

            switch (this.fittingModel)
            {
                case "Voigt":
                    this.BaseTensor.CalculateStiffnesses();
                    break;
                case "Reuss":
                    this.BaseTensor.CalculateStiffnesses();
                    break;
                case "Hill":
                    this.BaseTensor.CalculateStiffnesses();
                    break;
                default:
                    break;

            }

            System.ComponentModel.PropertyChangedEventHandler handler = FitUpdated;
            if (handler != null)
            {
                handler(this, new System.ComponentModel.PropertyChangedEventArgs("FitUpdated"));
            }
        }

        protected void OnFitFinished()
        {
            this._fitConverged = true;
            this.fitActive = false;

            SetFittingErrors();
            this._doneEvent.Set();

            System.ComponentModel.PropertyChangedEventHandler handler = FitFinished;
            if (handler != null)
            {
                handler(this, new System.ComponentModel.PropertyChangedEventArgs("FitFinished"));
            }
        }

        // Wrapper method for use with thread pool. 
        public void FitTensorCallback(Object threadContext)
        {
            OnFitStarted();

            //Fit Zeug kommt hier hin
            switch(this.FittingModel)
            {
                case 0:
                    this.FitVoigt(this.ClassicalCalculation);
                    break;
                case 1:
                    this.FitReuss(this.ClassicalCalculation);
                    break;
                case 2:
                    this.FitHill(this.ClassicalCalculation);
                    break;
                case 3:
                    this.FitGeometricHill(this.ClassicalCalculation);
                    break;
                case 4:
                    this.FitKroener(this.ClassicalCalculation, this.UseStifnessCalculation);
                    break;
                case 5:
                    this.FitDeWitt(this.ClassicalCalculation, this.UseStifnessCalculation);
                    break;
                default:
                    this.FitHill(this.ClassicalCalculation);
                    break;
            }

            OnFitFinished();
        }

        public void SetResetEvent(System.Threading.ManualResetEvent DoneEvent)
        {
            this._doneEvent = DoneEvent;
        }

        private void SetFittingErrors()
        {

        }

        #endregion

        #region Cloning

        public object Clone()
        {
            OrientationDistributionFunction Ret = new OrientationDistributionFunction();

            List<double[]> NewTData = new List<double[]>();

            for(int n = 0; n < this.TDData.Count; n++)
            {
                double[] TTmp = { this.TDData[n][0], this.TDData[n][1], this.TDData[n][2], this.TDData[n][3] };

                NewTData.Add(TTmp);
            }

            for(int n = 0; n < this.UsedPSA.Count; n++)
            {
                Ret.UsedPSA.Add(new Stress.Macroskopic.PeakStressAssociation(this.UsedPSA[n]));
            }

            Ret.TDData = NewTData;
            Ret._stepSizePhi = this._stepSizePhi;
            Ret._stepSizePhi1 = this._stepSizePhi1;
            Ret._stepSizePhi2 = this._stepSizePhi2;

            Ret._maxMRD = this._maxMRD;

            Ret.BaseTensor = this.BaseTensor.Clone() as Stress.Microsopic.ElasticityTensors;
            Ret.TextureTensor = this.TextureTensor.Clone() as Stress.Microsopic.ElasticityTensors;

            return Ret;
        }

        #endregion
    }
}
