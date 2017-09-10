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

        #region Sample Crystal transormation Matrix

        public double SCTM11(double varPhi1, double phi, double varPhi2)
        {
            double Ret = Math.Cos((varPhi1 * Math.PI) / 180.0) * Math.Cos((varPhi2 * Math.PI) / 180.0);
            Ret -= Math.Sin((varPhi1 * Math.PI) / 180.0) * Math.Sin((varPhi2 * Math.PI) / 180.0) * Math.Cos((phi * Math.PI) / 180.0);

            return Ret;
        }

        public double SCTM12(double varPhi1, double phi, double varPhi2)
        {
            double Ret = Math.Sin((varPhi1 * Math.PI) / 180.0) * Math.Cos((varPhi2 * Math.PI) / 180.0);
            Ret += Math.Cos((varPhi1 * Math.PI) / 180.0) * Math.Sin((varPhi2 * Math.PI) / 180.0) * Math.Cos((phi * Math.PI) / 180.0);

            return Ret;
        }

        public double SCTM13(double varPhi1, double phi, double varPhi2)
        {
            double Ret = Math.Sin((varPhi2 * Math.PI) / 180.0) * Math.Sin((phi * Math.PI) / 180.0);

            return Ret;
        }

        public double SCTM21(double varPhi1, double phi, double varPhi2)
        {
            double Ret = -1 * Math.Cos((varPhi1 * Math.PI) / 180.0) * Math.Sin((varPhi2 * Math.PI) / 180.0);
            Ret -= Math.Sin((varPhi1 * Math.PI) / 180.0) * Math.Cos((varPhi2 * Math.PI) / 180.0) * Math.Cos((phi * Math.PI) / 180.0);

            return Ret;
        }

        public double SCTM22(double varPhi1, double phi, double varPhi2)
        {
            double Ret = -1 * Math.Sin((varPhi1 * Math.PI) / 180.0) * Math.Sin((varPhi2 * Math.PI) / 180.0);
            Ret += Math.Cos((varPhi1 * Math.PI) / 180.0) * Math.Cos((varPhi2 * Math.PI) / 180.0) * Math.Cos((phi * Math.PI) / 180.0);

            return Ret;
        }

        public double SCTM23(double varPhi1, double phi, double varPhi2)
        {
            double Ret = Math.Cos((varPhi2 * Math.PI) / 180.0) * Math.Sin((phi * Math.PI) / 180.0);

            return Ret;
        }

        public double SCTM31(double varPhi1, double phi, double varPhi2)
        {
            double Ret = Math.Sin((varPhi1 * Math.PI) / 180.0) * Math.Sin((phi * Math.PI) / 180.0);

            return Ret;
        }

        public double SCTM32(double varPhi1, double phi, double varPhi2)
        {
            double Ret = -1 * Math.Cos((varPhi1 * Math.PI) / 180.0) * Math.Sin((phi * Math.PI) / 180.0);

            return Ret;
        }

        public double SCTM33(double varPhi1, double phi, double varPhi2)
        {
            double Ret = Math.Cos((phi * Math.PI) / 180.0);

            return Ret;
        }

        #endregion

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

        #region Fitting

        public void FitVoigt(bool classicCalculation)
        {
            switch (this.BaseTensor.Symmetry)
            {
                case "cubic":
                    if (this.BaseTensor.IsIsotropic)
                    {
                        this.BaseTensor.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorTextureVoigtCubicIsotrope(this, classicCalculation);
                    }
                    else
                    {
                        this.BaseTensor.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorTextureVoigtCubic(this, classicCalculation);
                    }
                    break;
                case "hexagonal":
                    this.BaseTensor.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorTextureVoigtType1(this, classicCalculation);
                    break;
                case "tetragonal type 1":
                    this.BaseTensor.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorTextureVoigtType2(this, classicCalculation);
                    break;
                case "tetragonal type 2":
                    this.BaseTensor.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorTextureVoigtType2(this, classicCalculation);
                    break;
                case "trigonal type 1":
                    this.BaseTensor.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorTextureVoigtType1(this, classicCalculation);
                    break;
                case "trigonal type 2":
                    this.BaseTensor.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorTextureVoigtType1(this, classicCalculation);
                    break;
                case "rhombic":
                    this.BaseTensor.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorTextureVoigtType3(this, classicCalculation);
                    break;
                case "monoclinic":
                    this.BaseTensor.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorTextureVoigtType3(this, classicCalculation);
                    break;
                case "triclinic":
                    this.BaseTensor.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorTextureVoigtType3(this, classicCalculation);
                    break;
                default:
                    this.BaseTensor.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorTextureVoigtType3(this, classicCalculation);
                    break;
            }

            this.BaseTensor.CalculateCompliances();
        }

        public void FitReuss(bool classicCalculation)
        {
            switch (this.BaseTensor.Symmetry)
            {
                case "cubic":
                    this.BaseTensor.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorTextureReussCubic(this, classicCalculation);
                    break;
                case "hexagonal":
                    this.BaseTensor.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorTextureReussHexagonal(this, classicCalculation);
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
                    this.BaseTensor.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorTextureHillCubic(this, classicCalculation);
                    break;
                case "hexagonal":
                    this.BaseTensor.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorTextureHillHexagonal(this, classicCalculation);
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
                        this.BaseTensor.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorTextureKroenerCubicStiffness(this, classicCalculation);
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
                        this.BaseTensor.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorTextureKroenerCubicCompliance(this, classicCalculation);
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
                        this.BaseTensor.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorTextureDeWittCubicStiffness(this, classicCalculation);
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
                        this.BaseTensor.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorTextureDeWittCubicCompliance(this, classicCalculation);
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
                    this.BaseTensor.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorTextureGeometricHillCubic(this, classicCalculation);
                    break;
                case "hexagonal":
                    this.BaseTensor.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorTextureGeometricHillHexagonal(this, classicCalculation);
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

        #endregion

        #region Fitting using Multi Threading

        public event System.ComponentModel.PropertyChangedEventHandler FitFinished;
        public event System.ComponentModel.PropertyChangedEventHandler FitStarted;

        protected void OnFitStarted()
        {
            this._fitConverged = false;
            this.fitActive = true;

            System.ComponentModel.PropertyChangedEventHandler handler = FitStarted;
            if (handler != null)
            {
                handler(this, new System.ComponentModel.PropertyChangedEventArgs("FitStarted"));
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
