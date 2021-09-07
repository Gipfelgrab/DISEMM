using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DiffractionOrientation;

namespace CalScec.Pattern
{
    public class DiffractionPattern : ICloneable
    {

        #region Parameters

        private int _id = 0;
        public int Id
        {
            get
            {
                return this._id;
            }
        }

        private string _name;
        public string Name
        {
            get
            {
                if (this._name == "")
                {
                    return this._path;
                }
                else
                {
                    return this._name;
                }
            }
            set
            {
                this._name = value;
            }
        }

        private string _path;
        public string Path
        {
            get
            {
                return this._path;
            }
        }

        private MathNet.Numerics.LinearAlgebra.Vector<double> GetRotatedS1()
        {
            DiffractionOrientation.OrientationMatrix RotationMatrix = new OrientationMatrix();

            double[] RotationAngles = { this._omegaAngle * (Math.PI / 180.0), this._chiAngle * (Math.PI / 180.0), this._phiSampleAngle * (Math.PI / 180.0) };

            RotationMatrix.EulerAngles = RotationAngles;

            #region Martin Master

            //MathNet.Numerics.LinearAlgebra.Matrix<double> FirstRotation = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense<double>(3, 3, 0);
            //MathNet.Numerics.LinearAlgebra.Matrix<double> SecondRotation = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense<double>(3, 3, 0);
            //MathNet.Numerics.LinearAlgebra.Matrix<double> ThirdRotation = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense<double>(3, 3, 0);

            //double RadOmega = this._omegaAngle * (Math.PI / 180.0);
            //FirstRotation[0, 0] = Math.Cos(RadOmega);
            //FirstRotation[0, 1] = Math.Sin(RadOmega);
            //FirstRotation[1, 0] = -1 * Math.Sin(RadOmega);
            //FirstRotation[1, 1] = Math.Cos(RadOmega);
            //FirstRotation[2, 2] = 1;

            //double RadChi = this._chiAngle * (Math.PI / 180.0);
            //SecondRotation[0, 0] = Math.Cos(RadChi);
            //SecondRotation[0, 2] = -1 * Math.Sin(RadChi);
            //SecondRotation[2, 0] = Math.Sin(RadChi);
            //SecondRotation[1, 1] = 1;
            //SecondRotation[2, 2] = Math.Cos(RadChi);

            //double RadPhiSample = this._phiSampleAngle * (Math.PI / 180.0);
            //ThirdRotation[0, 0] = Math.Cos(RadPhiSample);
            //ThirdRotation[0, 1] = Math.Sin(RadPhiSample);
            //ThirdRotation[1, 0] = -1 * Math.Sin(RadPhiSample);
            //ThirdRotation[1, 1] = Math.Cos(RadPhiSample);
            //ThirdRotation[2, 2] = 1;

            //MathNet.Numerics.LinearAlgebra.Matrix<double> RotationMatrix = FirstRotation * SecondRotation * ThirdRotation;
            //MathNet.Numerics.LinearAlgebra.Matrix<double> RotationMatrix = FirstRotation * SecondRotation;

            #endregion

            MathNet.Numerics.LinearAlgebra.Vector<double> S1 = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3, 0);
            S1[0] = 1;

            MathNet.Numerics.LinearAlgebra.Vector<double> Ret = RotationMatrix.OM * S1;

            return Ret;
        }
        public MathNet.Numerics.LinearAlgebra.Vector<double> GetRotatedS3()
        {
            //X-Achse wird erst um Omega in position gedreht. Es wird von einem Rechtshändigen Koordinatensystemausgegangen
            //Dabei ist ein positiver Drehwinkel im Uhrzeigersinn!!!!!
            //Somit muss in negative Oemga gedreht werden da die Drehung im Experiment gegen den Uhrzeigersinn läuft
            DiffractionOrientation.OrientationMatrix RotationMatrixXAxis = new OrientationMatrix();
            double[] RotationAngles1 = { -1 * this._omegaAngle * (Math.PI / 180.0), 0, 0 };
            MathNet.Numerics.LinearAlgebra.Vector<double> XAxis = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3, 0);
            XAxis[0] = 1;
            RotationMatrixXAxis.EulerAngles = RotationAngles1;

            XAxis = RotationMatrixXAxis.OM * XAxis;

            //Nun wird die Drehung um Chi vollzogen. Dabei wird um die neue X-Achse gedreht!!!
            //positiver Winkel heißt im Urzeigersinn und Negativer Winkel gegen Uhrzeigersinn
            //Die Chi-Kippung wird im Uhrzeigersinn vollzogen --> positiver winkel

            DiffractionOrientation.OrientationMatrix RotationMatrixChiAxis = new OrientationMatrix();

            double[] AngleAxisPairValues = { this._chiAngle * (Math.PI / 180.0), XAxis[0], XAxis[1], XAxis[2] };
            RotationMatrixChiAxis.AngleAxisPair = AngleAxisPairValues;

            MathNet.Numerics.LinearAlgebra.Vector<double> S3 = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3, 0);
            S3[2] = 1;

            MathNet.Numerics.LinearAlgebra.Vector<double> Ret = RotationMatrixChiAxis.OM * S3;

            return Ret;
        }
        /// <summary>
        /// Der Vektor der Messrichtung
        /// </summary>
        /// <param name="Measured2Theta"> Gemessener Winkel der Netzebenschar</param>
        /// <returns></returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> GetQI(double Measured2Theta)
        {
            MathNet.Numerics.LinearAlgebra.Vector<double> Ret = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3, 0);

            double XYAngle = Measured2Theta / 2.0;
            XYAngle += 90;
            //XYAngle *= -1;

            XYAngle *= (Math.PI / 180.0);

            Ret[0] = Math.Cos(XYAngle);
            Ret[1] = Math.Sin(XYAngle);
            //Ret[0] = 0;
            //Ret[1] = 1;
            //Ret[2] = 0;

            return Ret;
        }

        private double _phiSampleAngle;
        public double PhiSampleAngle
        {
            get
            {
                return this._phiSampleAngle;
            }
            set
            {
                this._phiSampleAngle = value;
            }
        }

        private double _macroStrain;
        public double MacroStrain
        {
            get
            {
                return this._macroStrain;
            }
            set
            {
                this._macroStrain = value;
            }
        }
        private double _macroExtension;
        public double MacroExtension
        {
            get
            {
                return this._macroExtension;
            }
            set
            {
                this._macroExtension = value;
            }
        }
        private double _chiAngle;
        public double ChiAngle
        {
            get
            {
                return this._chiAngle;
            }
            set
            {
                this._chiAngle = value;
            }
        }
        public double PsiAngle(double Measured2Theta)
        {
            MathNet.Numerics.LinearAlgebra.Vector<double> QI = this.GetQI(Measured2Theta);
            MathNet.Numerics.LinearAlgebra.Vector<double> S3 = this.GetRotatedS3();

            double ScalarProduct = QI * S3;
            double TotalLength = QI.Norm(2);
            TotalLength *= S3.Norm(2);

            double Ret = Math.Acos(ScalarProduct / TotalLength);
            Ret /= (Math.PI / 180.0);
            return Ret;
        }
        
        private double _omegaAngle;
        public double OmegaAngle
        {
            get
            {
                return this._omegaAngle;
            }
            set
            {
                this._omegaAngle = value;
            }
        }
        public double PhiAngle(double Measured2Theta)
        {
            MathNet.Numerics.LinearAlgebra.Vector<double> QI = this.GetQI(Measured2Theta);
            MathNet.Numerics.LinearAlgebra.Vector<double> S1 = this.GetRotatedS1();

            double ScalarProduct = QI * S1;
            double TotalLength = QI.Norm(2);
            TotalLength += S1.Norm(2);

            double Ret = Math.Acos(ScalarProduct / TotalLength);
            Ret /= (Math.PI / 180.0);
            return Ret;
        }

        public double SlipDirectionAngle(double measured2Theta, DataManagment.CrystalData.HKLReflex slipPlane, DataManagment.CrystalData.HKLReflex slipDirection)
        {
            //First the scattering vector QI is calculated 
            //second QI is rotated from the slip plane into the slipdirection
            //third the Angle between S3 and the rotated QI is returned

            MathNet.Numerics.LinearAlgebra.Vector<double> QI = this.GetQI(measured2Theta);

            DiffractionOrientation.OrientationMatrix RotationMatrix = new OrientationMatrix();

            MathNet.Numerics.LinearAlgebra.Vector<double> SlipPlaneIndices = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3, 0);
            MathNet.Numerics.LinearAlgebra.Vector<double> SlipDirectionIndices = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3, 0);

            SlipPlaneIndices[0] = slipPlane.H;
            SlipPlaneIndices[1] = slipPlane.K;
            SlipPlaneIndices[2] = slipPlane.L;

            SlipDirectionIndices[0] = slipDirection.H;
            SlipDirectionIndices[1] = slipDirection.K;
            SlipDirectionIndices[2] = slipDirection.L;

            List<MathNet.Numerics.LinearAlgebra.Vector<double>> RotIndices = new List<MathNet.Numerics.LinearAlgebra.Vector<double>>();
            RotIndices.Add(SlipPlaneIndices);
            RotIndices.Add(SlipDirectionIndices);

            RotationMatrix.MillerIndices = RotIndices;

            MathNet.Numerics.LinearAlgebra.Vector<double> SampleSlipDirection = RotationMatrix.OM * QI;

            MathNet.Numerics.LinearAlgebra.Vector<double> S3 = this.GetRotatedS3();

            double ScalarProduct = SampleSlipDirection * S3;
            double TotalLength = SampleSlipDirection.Norm(2);
            TotalLength *= S3.Norm(2);

            double Ret = Math.Acos(ScalarProduct / TotalLength);
            Ret /= (Math.PI / 180.0);
            return Ret;
        }

        private double _stress;
        public double Stress
        {
            get
            {
                return this._stress;
            }
            set
            {
                this._stress = value;
            }
        }

        private double _force;
        public double Force
        {
            get
            {
                return this._force;
            }
            set
            {
                this._force = value;
            }
        }

        public Counts PatternCounts;

        public List<Analysis.Peaks.DiffractionPeak> FoundPeaks = new List<Analysis.Peaks.DiffractionPeak>();
        public List<Analysis.Peaks.Functions.PeakRegionFunction> PeakRegions = new List<Analysis.Peaks.Functions.PeakRegionFunction>();

        public List<double> PhaseStresses = new List<double>();

        #endregion

        public DiffractionPattern(string FilePath, int id)
        {
            this._id = id;
            this._path = FilePath;

            this._name = System.IO.Path.GetFileName(FilePath);
            this.PatternCounts = new Counts();

            string[] PatternFileLines = System.IO.File.ReadLines(FilePath).ToArray();
            foreach(string line in PatternFileLines)
            {
                if(line[0] != '#' && line[0] != '%' && line[0] != '/')
                {
                    string[] Parts = line.Split(' ');

                    List<double> ActCount = new List<double>();
                    foreach(string s in Parts)
                    {
                        if(s != "")
                        {
                            try
                            {
                                ActCount.Add(Convert.ToDouble(s));
                            }
                            catch
                            {

                            }
                        }
                    }
                    if (ActCount.Count > 1)
                    {
                        if (ActCount.Count < 3)
                        {
                            ActCount.Add(Math.Sqrt(ActCount[1]));
                        }

                        this.PatternCounts.Add(ActCount.ToArray());
                    }
                }
            }
        }

        public DiffractionPattern(int id)
        {
            this._id = id;
        }

        public void SetPeakAssociations()
        {
            for (int n = 0; n < this.PeakRegions.Count; n++)
            {
                this.PeakRegions[n].AssociatedPatternName = this.Name;
            }
            for (int n = 0; n < this.FoundPeaks.Count; n++)
            {
                this.FoundPeaks[n].AssociatedPatternName = this.Name;
            }
        }

        #region Calculations

        #region Peak regions

        public void SetRegions()
        {
            this.PeakRegions = new List<Analysis.Peaks.Functions.PeakRegionFunction>();
            FoundPeaks.Sort((A, B) => (1) * (A.PFunction.Angle).CompareTo(B.PFunction.Angle));

            for(int n = 0; n < this.FoundPeaks.Count;)
            {
                if( n + 1 < this.FoundPeaks.Count)
                {
                    for (int i = 1; n + i < this.FoundPeaks.Count + 1; i++)
                    {
                        if(n + i == this.FoundPeaks.Count)
                        {
                            Pattern.Counts fittingData = this.PatternCounts.GetRange(this.FoundPeaks[n].PFunction.Angle - (this.FoundPeaks[n].PFunction.FWHM * CalScec.Properties.Settings.Default.PeakWidthFWHM), this.FoundPeaks[n + (i - 1)].PFunction.Angle + (this.FoundPeaks[n + (i - 1)].PFunction.FWHM * CalScec.Properties.Settings.Default.PeakWidthFWHM));

                            List<Analysis.Peaks.Functions.PeakFunction> ForAdd = new List<Analysis.Peaks.Functions.PeakFunction>();
                            foreach (Analysis.Peaks.DiffractionPeak DP in FoundPeaks.GetRange(n, i))
                            {
                                ForAdd.Add(DP.PFunction);
                            }

                            PeakRegions.Add(new Analysis.Peaks.Functions.PeakRegionFunction(this.FoundPeaks[n].PFunction.ConstantBackground, fittingData, ForAdd));
                            n += i;
                            break;
                        }

                        double PeakDistance = Math.Abs(this.FoundPeaks[n + (i - 1)].PFunction.Angle - this.FoundPeaks[n + i].PFunction.Angle);
                        if(PeakDistance > 2 * this.FoundPeaks[n].PFunction.FWHM * CalScec.Properties.Settings.Default.PeakWidthFWHM)
                        {
                            if(i == 1)
                            {
                                PeakRegions.Add(new Analysis.Peaks.Functions.PeakRegionFunction(this.FoundPeaks[n].PFunction.ConstantBackground, this.FoundPeaks[n].PFunction.FittingCounts, FoundPeaks[n].PFunction));
                                n++;
                                break;
                            }
                            else
                            {
                                Pattern.Counts fittingData = this.PatternCounts.GetRange(this.FoundPeaks[n].PFunction.Angle - (this.FoundPeaks[n].PFunction.FWHM * CalScec.Properties.Settings.Default.PeakWidthFWHM), this.FoundPeaks[n + (i - 1)].PFunction.Angle + (this.FoundPeaks[n + (i - 1)].PFunction.FWHM * CalScec.Properties.Settings.Default.PeakWidthFWHM));

                                List<Analysis.Peaks.Functions.PeakFunction> ForAdd = new List<Analysis.Peaks.Functions.PeakFunction>();
                                foreach(Analysis.Peaks.DiffractionPeak DP in FoundPeaks.GetRange(n, i))
                                {
                                    ForAdd.Add(DP.PFunction);
                                }

                                PeakRegions.Add(new Analysis.Peaks.Functions.PeakRegionFunction(this.FoundPeaks[n].PFunction.ConstantBackground, fittingData, ForAdd));
                                n += i;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    PeakRegions.Add(new Analysis.Peaks.Functions.PeakRegionFunction(this.FoundPeaks[n].PFunction.ConstantBackground, this.FoundPeaks[n].PFunction.FittingCounts, FoundPeaks[n].PFunction));
                    n++;
                }
            }

            for(int n = 0; n < this.PeakRegions.Count; n++)
            {
                this.PeakRegions[n].AssociatedPatternName = this.Name;
                this.PeakRegions[n].PolynomialBackgroundFunction.Constant = this.PeakRegions[n][0].FittingCounts[0][1];
            }
        }

        public void SetRegionsFromPrevious(Pattern.DiffractionPattern prevDP)
        {
            this.PeakRegions = new List<Analysis.Peaks.Functions.PeakRegionFunction>();
            FoundPeaks.Sort((A, B) => (1) * (A.PFunction.Angle).CompareTo(B.PFunction.Angle));

            for(int n = 0; n < prevDP.PeakRegions.Count; n++)
            {
                List<Analysis.Peaks.Functions.PeakFunction> forAdd = new List<Analysis.Peaks.Functions.PeakFunction>();
                for(int i = 0; i < prevDP.PeakRegions[n].Count; i++)
                {
                    for(int j = 0; j < this.FoundPeaks.Count; j++)
                    {
                        if(prevDP.PeakRegions[n][i].Angle == this.FoundPeaks[j].Angle)
                        {
                            forAdd.Add(this.FoundPeaks[j].PFunction);
                            break;
                        }
                    }
                }

                Pattern.Counts fittingData = this.PatternCounts.GetRange(forAdd[0].Angle - (CalScec.Properties.Settings.Default.PeakWidthFWHM * forAdd[0].FWHM), forAdd[forAdd.Count - 1].Angle + (CalScec.Properties.Settings.Default.PeakWidthFWHM * forAdd[forAdd.Count - 1].FWHM));

                Analysis.Peaks.Functions.PeakRegionFunction regionTmp = new Analysis.Peaks.Functions.PeakRegionFunction(prevDP.PeakRegions[n].PolynomialBackgroundFunction.Constant, fittingData, forAdd);
                regionTmp.PolynomialBackgroundFunction.Aclivity = prevDP.PeakRegions[n].PolynomialBackgroundFunction.Aclivity;
                regionTmp.PolynomialBackgroundFunction.Center = prevDP.PeakRegions[n].PolynomialBackgroundFunction.Center;
                this.PeakRegions.Add(regionTmp);
            }

            for (int n = 0; n < this.PeakRegions.Count; n++)
            {
                this.PeakRegions[n].AssociatedPatternName = this.Name;
            }
        }

        #endregion

        #endregion

        #region IClonable

        public object Clone()
        {
            DiffractionPattern Ret = new DiffractionPattern(this._id);
            
            Ret._name = this._name;
            Ret._path = this._path;
            Ret._chiAngle = this._chiAngle;
            Ret._omegaAngle = this._omegaAngle;
            Ret._phiSampleAngle = this._phiSampleAngle;
            Ret._macroStrain = this._macroStrain;
            Ret._stress = this._stress;
            Ret._force = this._force;

            Ret.PatternCounts = (Counts)this.PatternCounts.Clone();

            foreach(Analysis.Peaks.DiffractionPeak DP in this.FoundPeaks)
            {
                Ret.FoundPeaks.Add(DP.Clone() as Analysis.Peaks.DiffractionPeak);
            }
            foreach (Analysis.Peaks.Functions.PeakRegionFunction PR in this.PeakRegions)
            {
                Ret.PeakRegions.Add(PR.Clone() as Analysis.Peaks.Functions.PeakRegionFunction);
            }

            return Ret;
        }

        #endregion
    }
}
