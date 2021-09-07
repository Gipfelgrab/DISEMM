using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.Analysis.Stress.Plasticity
{
    public class ReflexYield
    {
        public DataManagment.CrystalData.HKLReflex SlipPlane;

        public DataManagment.CrystalData.HKLReflex MainSlipDirection;
        public DataManagment.CrystalData.HKLReflex SecondarySlipDirection;

        private int activeSystem = 1;
        public int ActiveSystem
        {
            get
            {
                return this.activeSystem;
            }
            set
            {
                this.activeSystem = value;
            }
        }

        public string HKLString
        {
            get
            {
                return this.SlipPlane.HKLString;
            }
            set
            {

            }
        }
        public string HKLStringSlipDirection
        {
            get
            {
                return this.MainSlipDirection.HKLString;
            }
            set
            {

            }
        }

        private double _yieldMainHardennedStrength = -1;
        public double YieldMainHardennedStrength
        {
            get
            {
                return this._yieldMainHardennedStrength;
            }
            set
            {
                this._yieldMainHardennedStrength = value;
            }
        }

        private double _yieldMainAvgHardenning = -1;
        public double YieldMainAvgHardenning
        {
            get
            {
                return this._yieldMainAvgHardenning;
            }
            set
            {
                this._yieldMainAvgHardenning = value;
            }
        }

        private double _yieldHardenning = -1;
        public double YieldHardenning
        {
            get
            {
                return this._yieldHardenning;
            }
            set
            {
                this._yieldHardenning = value;
            }
        }
        private double _yieldLimit = 1000;
        public double YieldLimit
        {
            get
            {
                return this._yieldLimit;
            }
            set
            {
                this._yieldLimit = value;
            }
        }

        private double _yieldMainStrength = -1;
        public double YieldMainStrength
        {
            get
            {
                return this._yieldMainStrength;
            }
            set
            {
                this._yieldMainStrength = value;
                this._yieldMainHardennedStrength = value;
            }
        }
        private double _yieldSecondaryStrength = -1;
        public double YieldSecondaryStrength
        {
            get
            {
                if (this._yieldSecondaryStrength == -1)
                {
                    return this._yieldMainStrength;
                }
                else
                {
                    return this._yieldSecondaryStrength;
                }
            }
            set
            {
                this._yieldSecondaryStrength = value;
            }
        }

        private int _plainMainMultiplizity;
        public int PlainMainMultiplizity
        {
            get
            {
                return this._plainMainMultiplizity;
            }
            set
            {
                this._plainMainMultiplizity = value;
            }
        }
        private int _plainSecondaryMultiplizity;
        public int PlainSecondaryMultiplizity
        {
            get
            {
                return this._plainSecondaryMultiplizity;
            }
            set
            {
                this._plainSecondaryMultiplizity = value;
            }
        }

        private int _directionMainMultiplizity;
        public int DirectionMainMultiplizity
        {
            get
            {
                return this._directionMainMultiplizity;
            }
            set
            {
                this._directionMainMultiplizity = value;
            }
        }
        private int _directionSecondaryMultiplizity;
        public int DirectionSecondaryMultiplizity
        {
            get
            {
                return this._directionSecondaryMultiplizity;
            }
            set
            {
                this._directionSecondaryMultiplizity = value;
            }
        }

        public int TotalMainSystems
        {
            get
            {
                return this._plainMainMultiplizity * this._directionMainMultiplizity;
            }
        }
        public int TotalSecondarySystems
        {
            get
            {
                return this._plainSecondaryMultiplizity * this._directionSecondaryMultiplizity;
            }
        }

        public Fitting.LinearFunction ElasticLinearFit1;
        public Fitting.LinearFunction PlasticLinearFit1;
        public Fitting.LinearFunction ElasticLinearFit2;
        public Fitting.LinearFunction PlasticLinearFit2;

        public DiffractionOrientation.OrientationMatrix MeasurementCrystalTransformation = new DiffractionOrientation.OrientationMatrix();

        public List<List<Stress.Macroskopic.PeakStressAssociation>> PeakData = new List<List<Stress.Macroskopic.PeakStressAssociation>>();
        public List<Stress.Macroskopic.PeakStressAssociation> StressStrainData = new List<Stress.Macroskopic.PeakStressAssociation>();

        public List<Macroskopic.PeakStressAssociation> GetPeakData(double psiAngle)
        {
            for (int n = 0; n < this.PeakData.Count; n++)
            {
                if (Math.Abs(psiAngle - this.PeakData[n][0].PsiAngle) < CalScec.Properties.Settings.Default.PsyAcceptanceAngle)
                {
                    return this.PeakData[n];
                }
            }

            return new List<Macroskopic.PeakStressAssociation>();
        }
        //List<List<Stress.Macroskopic.PeakStressAssociation>>

        public ReflexYield(DataManagment.CrystalData.HKLReflex reflex, DataManagment.CrystalData.CODData crystalData)
        {
            this.SlipPlane = reflex;

            this.ElasticLinearFit1 = new Fitting.LinearFunction(1);
            this.PlasticLinearFit1 = new Fitting.LinearFunction(1);
            this.ElasticLinearFit2 = new Fitting.LinearFunction(1);
            this.PlasticLinearFit2 = new Fitting.LinearFunction(1);

            bool MainAdded = false;

            #region Slip direction

            string RessourcePath = System.IO.Path.Combine(Environment.CurrentDirectory, @"Res\ReflectionConditions\SlipSystems.xml");
            using (System.IO.StreamReader XMLResStream = new System.IO.StreamReader(RessourcePath))
            {
                using (System.Xml.XmlTextReader ReflexReader = new System.Xml.XmlTextReader(XMLResStream))
                {
                    while (ReflexReader.Read())
                    {
                        if (ReflexReader.NodeType == System.Xml.XmlNodeType.Element)
                        {
                            if (ReflexReader.Name == "SlipSystem")
                            {
                                bool systemReadAktive = true;
                                int conditionFailed = 0;
                                while (systemReadAktive)
                                {
                                    ReflexReader.Read();

                                    if (ReflexReader.NodeType == System.Xml.XmlNodeType.Element)
                                    {
                                        switch (ReflexReader.Name)
                                        {
                                            case ("SpaceGroup"):
                                                ReflexReader.Read();
                                                if (ReflexReader.NodeType == System.Xml.XmlNodeType.Text)
                                                {
                                                    if (!(crystalData.SymmetryGroup == ReflexReader.Value))
                                                    {
                                                        conditionFailed++;
                                                    }
                                                }
                                                break;
                                            case ("SpaceGroupId"):
                                                ReflexReader.Read();
                                                if (ReflexReader.NodeType == System.Xml.XmlNodeType.Text)
                                                {
                                                    if (!(crystalData.SymmetryGroupID == Convert.ToInt32(ReflexReader.Value)))
                                                    {
                                                        conditionFailed++;
                                                    }
                                                }
                                                break;
                                            case ("SlipPlain"):
                                                ReflexReader.Read();
                                                if (ReflexReader.NodeType == System.Xml.XmlNodeType.Text)
                                                {
                                                    string PlainString = ReflexReader.Value;
                                                    int aktIndex = 0;
                                                    int PH = 0;
                                                    if (PlainString[aktIndex].ToString() == "-")
                                                    {
                                                        PH = Convert.ToInt32(PlainString[aktIndex].ToString() + PlainString[aktIndex + 1].ToString());
                                                        aktIndex += 2;
                                                    }
                                                    else
                                                    {
                                                        PH = Convert.ToInt32(PlainString[aktIndex].ToString());
                                                        aktIndex++;
                                                    }
                                                    int PK = 0;
                                                    if (PlainString[aktIndex].ToString() == "-")
                                                    {
                                                        PK = Convert.ToInt32(PlainString[aktIndex].ToString() + PlainString[aktIndex + 1].ToString());
                                                        aktIndex += 2;
                                                    }
                                                    else
                                                    {
                                                        PK = Convert.ToInt32(PlainString[aktIndex].ToString());
                                                        aktIndex++;
                                                    }
                                                    int PL = 0;
                                                    if (PlainString[aktIndex].ToString() == "-")
                                                    {
                                                        PL = Convert.ToInt32(PlainString[aktIndex].ToString() + PlainString[aktIndex + 1].ToString());
                                                        aktIndex += 2;
                                                    }
                                                    else
                                                    {
                                                        PL = Convert.ToInt32(PlainString[aktIndex].ToString());
                                                        aktIndex++;
                                                    }

                                                    if (this.SlipPlane.H == PH && this.SlipPlane.K == PK && this.SlipPlane.L == PL)
                                                    {
                                                        bool InnerSystemRead = true;
                                                        while(InnerSystemRead)
                                                        {
                                                            ReflexReader.Read();
                                                            if (ReflexReader.NodeType == System.Xml.XmlNodeType.Element)
                                                            {
                                                                switch (ReflexReader.Name)
                                                                {
                                                                    case ("SlipDirection"):
                                                                        ReflexReader.Read();
                                                                        if (ReflexReader.NodeType == System.Xml.XmlNodeType.Text)
                                                                        {
                                                                            int TextIndex = 0;
                                                                            string HKLString = ReflexReader.Value;
                                                                            int SlipH = 0;
                                                                            int SlipK = 0;
                                                                            int SlipL = 0;

                                                                            if (HKLString[TextIndex] == '-')
                                                                            {
                                                                                SlipH = Convert.ToInt32(HKLString.Substring(TextIndex, 2));
                                                                                TextIndex += 2;
                                                                            }
                                                                            else
                                                                            {
                                                                                SlipH = Convert.ToInt32(HKLString.Substring(TextIndex, 1));
                                                                                TextIndex++;
                                                                            }
                                                                            if (HKLString[TextIndex] == '-')
                                                                            {
                                                                                SlipK = Convert.ToInt32(HKLString.Substring(TextIndex, 2));
                                                                                TextIndex += 2;
                                                                            }
                                                                            else
                                                                            {
                                                                                SlipK = Convert.ToInt32(HKLString.Substring(TextIndex, 1));
                                                                                TextIndex++;
                                                                            }

                                                                            if (HKLString[TextIndex] == '-')
                                                                            {
                                                                                SlipL = Convert.ToInt32(HKLString.Substring(TextIndex, 2));
                                                                                TextIndex += 2;
                                                                            }
                                                                            else
                                                                            {
                                                                                SlipL = Convert.ToInt32(HKLString.Substring(TextIndex, 1));
                                                                                TextIndex++;
                                                                            }

                                                                            if (!MainAdded)
                                                                            {
                                                                                this.MainSlipDirection = new DataManagment.CrystalData.HKLReflex(SlipH, SlipK, SlipL, Tools.Calculation.CalculateHKLDistance(SlipH, SlipK, SlipL, crystalData));
                                                                            }
                                                                            else
                                                                            {
                                                                                this.SecondarySlipDirection = new DataManagment.CrystalData.HKLReflex(SlipH, SlipK, SlipL, Tools.Calculation.CalculateHKLDistance(SlipH, SlipK, SlipL, crystalData));
                                                                            }
                                                                        }
                                                                        break;
                                                                    case ("PlainCount"):
                                                                        ReflexReader.Read();
                                                                        if (ReflexReader.NodeType == System.Xml.XmlNodeType.Text)
                                                                        {
                                                                            if(!MainAdded)
                                                                            {
                                                                                this.PlainMainMultiplizity = Convert.ToInt32(ReflexReader.Value);
                                                                            }
                                                                            else
                                                                            {
                                                                                this.PlainSecondaryMultiplizity = Convert.ToInt32(ReflexReader.Value);
                                                                            }
                                                                        }
                                                                        break;
                                                                    case ("DirectionCount"):
                                                                        ReflexReader.Read();
                                                                        if (ReflexReader.NodeType == System.Xml.XmlNodeType.Text)
                                                                        {
                                                                            if (!MainAdded)
                                                                            {
                                                                                this.DirectionMainMultiplizity = Convert.ToInt32(ReflexReader.Value);
                                                                            }
                                                                            else
                                                                            {
                                                                                this.DirectionSecondaryMultiplizity = Convert.ToInt32(ReflexReader.Value);
                                                                            }
                                                                        }
                                                                        InnerSystemRead = false;
                                                                        conditionFailed = 2;
                                                                        break;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        systemReadAktive = false;
                                                    }
                                                }
                                                break;
                                        }
                                    }

                                    if (conditionFailed == 2)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            #endregion
        }

        public ReflexYield()
        {

        }
        
        public void SetCrystalStressAndStrain(int sym)
        {
            MathNet.Numerics.LinearAlgebra.Vector<double> slipPlaneIndices = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3, 0);

            slipPlaneIndices[0] = this.SlipPlane.H;
            slipPlaneIndices[1] = this.SlipPlane.K;
            slipPlaneIndices[2] = this.SlipPlane.L;

            DiffractionOrientation.OrientationMatrix RotationMatrixCrystalQI = new DiffractionOrientation.OrientationMatrix();
            RotationMatrixCrystalQI.AsMainAxisTransformation(this.SlipPlane.H, this.SlipPlane.K, this.SlipPlane.L, sym);

            for (int n = 0; n < StressStrainData.Count; n++)
            {
                MathNet.Numerics.LinearAlgebra.Matrix<double> measurementSystemStress = this.StressStrainData[n].MeasurementSystemStress();

                this.StressStrainData[n].CrystalSystemStress = RotationMatrixCrystalQI.RotateTensor(measurementSystemStress);


                MathNet.Numerics.LinearAlgebra.Matrix<double> crystalSystemStrain = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense<double>(3, 3, 0);

                crystalSystemStrain[0, 0] = this.StressStrainData[n].Strain / 3.0;
                crystalSystemStrain[1, 1] = this.StressStrainData[n].Strain / 3.0;
                crystalSystemStrain[2, 2] = this.StressStrainData[n].Strain / 3.0;

                this.StressStrainData[n].CrystalSystemStrain = RotationMatrixCrystalQI.RotateTensor(crystalSystemStrain);

                string Test = "";
            }
        }
        public void SetSlipDirectionAngles(Sample actSample, int sym)
        {
            MathNet.Numerics.LinearAlgebra.Vector<double> slipPlaneIndices = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3, 0);
            //MathNet.Numerics.LinearAlgebra.Vector<double> C3 = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3, 0);
            //MathNet.Numerics.LinearAlgebra.Vector<double> C1 = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3, 0);

            slipPlaneIndices[0] = this.SlipPlane.H;
            slipPlaneIndices[1] = this.SlipPlane.K;
            slipPlaneIndices[2] = this.SlipPlane.L;

            DiffractionOrientation.OrientationMatrix RotationMatrixCrystalQI = new DiffractionOrientation.OrientationMatrix();
            RotationMatrixCrystalQI.AsMainAxisTransformation(this.SlipPlane.H, this.SlipPlane.K, this.SlipPlane.L, sym);

            #region Old Transormation code

            //slipPlaneIndices[0] /= norm;
            //slipPlaneIndices[1] /= norm;
            //slipPlaneIndices[3] /= norm;


            //C3[0] = 0;
            //C3[1] = 0;
            //C3[2] = 1;

            //C1[0] = 1;
            //C1[1] = 0;
            //C1[2] = 0;

            //C3[0] /= norm;
            //C3[1] /= norm;
            //C3[2] /= norm;

            //MathNet.Numerics.LinearAlgebra.Vector<double> rotationAxis = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3, 0);

            //rotationAxis[0] = (slipPlaneIndices[1] * C3[2]) - (C3[1] * slipPlaneIndices[2]);
            //rotationAxis[1] = (C3[0] * slipPlaneIndices[2]) - (slipPlaneIndices[0] * C3[2]);
            //rotationAxis[2] = (slipPlaneIndices[0] * C3[1]) - (C3[0] * slipPlaneIndices[1]);

            //double ScalarProduct = slipPlaneIndices * C3;
            //double TotalLength = slipPlaneIndices.Norm(2);
            //TotalLength *= C3.Norm(2);
            //if (TotalLength == 0)
            //{
            //    TotalLength = 1;
            //}

            //double firstEulerAngle = -1 * Math.Acos(ScalarProduct / TotalLength) + (Math.PI / 2);
            //double firstEulerAngle = -1 * Math.Acos(ScalarProduct / TotalLength);

            //Koordinaten transformation zwischen Gleitebene und Gleitrichtung --> Führt die Gleitebene in die Gleitrichtung über



            //double[] AngleAxisPairValues = { firstEulerAngle, rotationAxis[0], rotationAxis[1], rotationAxis[2] };
            //RotationMatrixCrystalQI.AngleAxisPair = AngleAxisPairValues;

            //double det1 = RotationMatrixCrystalQI.OM.Determinant();

            //DiffractionOrientation.OrientationMatrix Rotation3 = new DiffractionOrientation.OrientationMatrix();

            //ScalarProduct = rotationAxis * C1;
            //TotalLength = rotationAxis.L2Norm();
            //if (TotalLength == 0)
            //{
            //    TotalLength = 1;
            //}

            //double[] lastEulerAngles = { 1 * Math.Acos(ScalarProduct / TotalLength), 0, 0 };
            //Rotation3.EulerAngles = lastEulerAngles;

            //RotationMatrixCrystalQI.OM = Rotation3.OM * RotationMatrixCrystalQI.OM;
            //this.MeasurementCrystalTransformation = RotationMatrixCrystalQI;

            //double det2 = Rotation3.OM.Determinant();
            //det1 = RotationMatrixCrystalQI.OM.Determinant();

            //RotationMatrixCrystalQI.MM = RotationMatrixCrystalQI.OM.Transpose();

            #endregion

            //MathNet.Numerics.LinearAlgebra.Vector<double> rotationTest1 = RotationMatrixCrystalQI.OM * C3;
            //MathNet.Numerics.LinearAlgebra.Vector<double> rotationTest2 = RotationMatrixCrystalQI.MM * C3;

            MathNet.Numerics.LinearAlgebra.Vector<double> rotationTest3 = RotationMatrixCrystalQI.OM * slipPlaneIndices;
            MathNet.Numerics.LinearAlgebra.Vector<double> rotationTest4 = RotationMatrixCrystalQI.MM * slipPlaneIndices;

            this.MeasurementCrystalTransformation = RotationMatrixCrystalQI;

            for (int n = 0; n < PeakData.Count; n++)
            {
                for(int i = 0; i < PeakData[n].Count; i++)
                {
                    for(int j = 0; j < actSample.DiffractionPatterns.Count; j++)
                    {
                        if (actSample.DiffractionPatterns[j].Name == this.PeakData[n][i].DPeak.AssociatedPatternName)
                        {
                            MathNet.Numerics.LinearAlgebra.Matrix<double> measurementSystemStress = this.PeakData[n][i].MeasurementSystemStress();

                            //MathNet.Numerics.LinearAlgebra.Vector<double> loadAxis = actSample.DiffractionPatterns[j].GetRotatedS3();

                            /////Koordinatentransformation zwischen Krsitall und Probensystem
                            //MathNet.Numerics.LinearAlgebra.Vector<double> measurementVektor = actSample.DiffractionPatterns[j].GetQI(this.PeakData[n][i].DPeak.Angle);

                            this.PeakData[n][i].CrystalSystemStress = RotationMatrixCrystalQI.RotateTensor(measurementSystemStress);
                            
                            this.PeakData[n][i].CrystalSystemStrain[0, 0] = RotationMatrixCrystalQI.OM[0, 2] * RotationMatrixCrystalQI.OM[0, 2] * this.PeakData[n][i].Strain;
                            this.PeakData[n][i].CrystalSystemStrain[0, 1] = RotationMatrixCrystalQI.OM[0, 2] * RotationMatrixCrystalQI.OM[1, 2] * this.PeakData[n][i].Strain;
                            this.PeakData[n][i].CrystalSystemStrain[0, 2] = RotationMatrixCrystalQI.OM[0, 2] * RotationMatrixCrystalQI.OM[2, 2] * this.PeakData[n][i].Strain;
                            this.PeakData[n][i].CrystalSystemStrain[1, 0] = RotationMatrixCrystalQI.OM[1, 2] * RotationMatrixCrystalQI.OM[0, 2] * this.PeakData[n][i].Strain;
                            this.PeakData[n][i].CrystalSystemStrain[1, 1] = RotationMatrixCrystalQI.OM[1, 2] * RotationMatrixCrystalQI.OM[1, 2] * this.PeakData[n][i].Strain;
                            this.PeakData[n][i].CrystalSystemStrain[1, 2] = RotationMatrixCrystalQI.OM[1, 2] * RotationMatrixCrystalQI.OM[2, 2] * this.PeakData[n][i].Strain;
                            this.PeakData[n][i].CrystalSystemStrain[2, 0] = RotationMatrixCrystalQI.OM[2, 2] * RotationMatrixCrystalQI.OM[0, 2] * this.PeakData[n][i].Strain;
                            this.PeakData[n][i].CrystalSystemStrain[2, 1] = RotationMatrixCrystalQI.OM[2, 2] * RotationMatrixCrystalQI.OM[1, 2] * this.PeakData[n][i].Strain;
                            this.PeakData[n][i].CrystalSystemStrain[2, 2] = RotationMatrixCrystalQI.OM[2, 2] * RotationMatrixCrystalQI.OM[2, 2] * this.PeakData[n][i].Strain;
                            string Test = "";
                            #region Old test Stuff

                            ///Testteil, der kann auch später noch verwendet werden falls nötig
                            ////Winkel in der x-y Ebene zwischen der LastAchse und dem Streuvektor im Messsystem 1. Euler Winkel
                            //MathNet.Numerics.LinearAlgebra.Vector<double> projectedLoadAxis = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3, 0);
                            //projectedLoadAxis[0] = loadAxis[0];
                            //projectedLoadAxis[1] = loadAxis[1];
                            //MathNet.Numerics.LinearAlgebra.Vector<double> projectedMeasurementVektor = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3, 0);
                            //projectedMeasurementVektor[0] = measurementVektor[0];
                            //projectedMeasurementVektor[1] = measurementVektor[1];

                            //double ScalarProduct = projectedLoadAxis * projectedMeasurementVektor;
                            //double TotalLength = projectedLoadAxis.Norm(2);
                            //TotalLength *= projectedMeasurementVektor.Norm(2);

                            //double firstEulerAngle = Math.Acos(ScalarProduct / TotalLength);
                            //firstEulerAngle /= (Math.PI / 180.0);
                            ////2. Euler Winkel
                            //projectedLoadAxis[0] = loadAxis[0];
                            //projectedLoadAxis[1] = 0.0;
                            //projectedLoadAxis[2] = loadAxis[2];
                            //projectedMeasurementVektor[0] = measurementVektor[0];
                            //projectedMeasurementVektor[1] = 0.0;
                            //projectedMeasurementVektor[2] = measurementVektor[2];

                            //ScalarProduct = projectedLoadAxis * projectedMeasurementVektor;
                            //TotalLength = projectedLoadAxis.Norm(2);
                            //TotalLength *= projectedMeasurementVektor.Norm(2);

                            //double secondEulerAngle = Math.Acos(ScalarProduct / TotalLength);
                            //secondEulerAngle /= (Math.PI / 180.0);

                            //double[] eulerAngles = { firstEulerAngle, secondEulerAngle, 0 };

                            //DiffractionOrientation.OrientationMatrix RotationMatrixLoadQI = new DiffractionOrientation.OrientationMatrix();
                            //RotationMatrixLoadQI.EulerAngles = eulerAngles;

                            ////eulerAngles[0] -= Math.Asin(slipPlaneIndices[1] / slipPlaneIndices.Norm(2));
                            ////eulerAngles[1] -= Math.Asin(slipPlaneIndices[2] / slipPlaneIndices.Norm(2));

                            //DiffractionOrientation.OrientationMatrix RotationMatrixMeasCrys = new DiffractionOrientation.OrientationMatrix();
                            //RotationMatrixMeasCrys.EulerAngles = eulerAngles;

                            //SlipDirectionIndices = RotationMatrixMeasCrys.OM.Transpose() * SlipDirectionIndices;


                            ////X-Achse wird erst um Omega in position gedreht. Es wird von einem Rechtshändigen Koordinatensystemausgegangen
                            ////Dabei ist ein positiver Drehwinkel im Uhrzeigersinn!!!!!
                            ////Somit muss in negative Oemga gedreht werden da die Drehung im Experiment gegen den Uhrzeigersinn läuft
                            //DiffractionOrientation.OrientationMatrix RotationMatrixXAxis = new DiffractionOrientation.OrientationMatrix();
                            //double[] RotationAngles1 = { 1 * Math.Asin(measurementVektor[1] / measurementVektor.Norm(2)), 0, 0 };
                            //MathNet.Numerics.LinearAlgebra.Vector<double> XAxis = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3, 0);
                            //XAxis[0] = 1;
                            ////XAxis[0] = slipPlaneIndices[0];
                            ////XAxis[1] = slipPlaneIndices[1];
                            ////XAxis[2] = slipPlaneIndices[2];
                            //RotationMatrixXAxis.EulerAngles = RotationAngles1;

                            //XAxis = RotationMatrixXAxis.OM * XAxis;

                            ////Nun wird die Drehung um Chi vollzogen. Dabei wird um die neue X-Achse gedreht!!!
                            ////positiver Winkel heißt im Urzeigersinn und Negativer Winkel gegen Uhrzeigersinn
                            ////Die Chi-Kippung wird im Uhrzeigersinn vollzogen --> positiver winkel

                            //DiffractionOrientation.OrientationMatrix RotationMatrixChiAxis = new DiffractionOrientation.OrientationMatrix();

                            //double[] AngleAxisPairValues = { Math.Asin(measurementVektor[2] / measurementVektor.Norm(2)), XAxis[0], XAxis[1], XAxis[2] };
                            //RotationMatrixChiAxis.AngleAxisPair = AngleAxisPairValues;


                            //MathNet.Numerics.LinearAlgebra.Vector<double> ret = RotationMatrixChiAxis.OM.Transpose() * SlipDirectionIndices;

                            //double ScalarProduct = loadAxis * ret;
                            //double TotalLength = loadAxis.Norm(2);
                            //TotalLength *= ret.Norm(2);

                            //double mainAngle = Math.Acos(ScalarProduct / TotalLength);
                            //mainAngle /= (Math.PI / 180.0);

                            //this.PeakData[n][i].MainSlipDirectionAngle = mainAngle;
                            //this.PeakData[n][i].SecondarySlipDirectionAngle = mainAngle;

                            #endregion
                        }
                    }
                }
            }
        }

        public double LowerElasticBorder(double psiAngle)
        {
            double ret = double.MaxValue;

            for(int n = 0; n < PeakData.Count; n++)
            {
                if (Math.Abs(psiAngle - this.PeakData[n][0].PsiAngle) < 5.0)
                {
                    for (int i = 0; i < this.PeakData[n].Count; i++)
                    {
                        if (PeakData[n][i].ElasticRegime == true)
                        {
                            if (ret > PeakData[n][i].MacroskopicStrain)
                            {
                                ret = PeakData[n][i].MacroskopicStrain;
                            }
                        }
                    }
                }
            }

            return ret;
        }
        public double UpperElasticBorder(double psiAngle)
        {
            double ret = double.MinValue;

            for (int n = 0; n < PeakData.Count; n++)
            {
                if (Math.Abs(psiAngle - this.PeakData[n][0].PsiAngle) < 5.0)
                {
                    for (int i = 0; i < this.PeakData[n].Count; i++)
                    {
                        if (PeakData[n][i].ElasticRegime == true)
                        {
                            if (ret < PeakData[n][i].MacroskopicStrain)
                            {
                                ret = PeakData[n][i].MacroskopicStrain;
                            }
                        }
                    }
                }
            }

            return ret;
        }
        public double LowerPlasticBorder(double psiAngle)
        {
            double ret = double.MaxValue;

            for (int n = 0; n < PeakData.Count; n++)
            {
                if (Math.Abs(psiAngle - this.PeakData[n][0].PsiAngle) < 5.0)
                {
                    for (int i = 0; i < this.PeakData[n].Count; i++)
                    {
                        if (PeakData[n][i].ElasticRegime == false)
                        {
                            if (ret > PeakData[n][i].MacroskopicStrain)
                            {
                                ret = PeakData[n][i].MacroskopicStrain;
                            }
                        }
                    }
                }
            }

            return ret;
        }
        public double UpperPlasticBorder(double psiAngle)
        {
            double ret = double.MinValue;

            for (int n = 0; n < PeakData.Count; n++)
            {
                if (Math.Abs(psiAngle - this.PeakData[n][0].PsiAngle) < 5.0)
                {
                    for (int i = 0; i < this.PeakData[n].Count; i++)
                    {
                        if (PeakData[n][i].ElasticRegime == false)
                        {
                            if (ret < PeakData[n][i].MacroskopicStrain)
                            {
                                ret = PeakData[n][i].MacroskopicStrain;
                            }
                        }
                    }
                }
            }

            return ret;
        }

        #region EPSC

        private double _actShearRate = -1;
        public double ActShearRate
        {
            get
            {
                return this._actShearRate;
            }
            set
            {
                this._actShearRate = value;
            }
        }

        public double Shearforce(MathNet.Numerics.LinearAlgebra.Matrix<double> aStress)
        {
            double shearForce = Math.Pow(aStress[0, 0] - aStress[1, 1], 2);
            shearForce += Math.Pow(aStress[1, 1] - aStress[2, 2], 2);
            shearForce += Math.Pow(aStress[0, 0] - aStress[2, 2], 2);
            shearForce += Math.Pow(aStress[0, 1], 2);
            shearForce += Math.Pow(aStress[0, 2], 2);
            shearForce += Math.Pow(aStress[1, 2], 2);
            double ret = Math.Sqrt((shearForce / 6.0));

            return ret;
        }
        
        /// <summary>
        /// Alpha, multiplied with the applied stress gets the resolved shear stress
        /// </summary>
        /// <param name="slipPlane"></param>
        /// <param name="slipDirection"></param>
        /// <returns>alpha</returns>
        public MathNet.Numerics.LinearAlgebra.Matrix<double> GetResolvingParameter(DataManagment.CrystalData.HKLReflex slipPlane, DataManagment.CrystalData.HKLReflex slipDirection)
        {
            MathNet.Numerics.LinearAlgebra.Matrix<double> ret = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
            double slipPlaneNorm = slipPlane.NormFaktor;
            double slipDirectionNorm = slipDirection.NormFaktor;

            ret[1, 1] = (slipDirection.H * slipPlane.H) * (slipPlaneNorm * slipDirectionNorm);
            ret[2, 2] = (slipDirection.K * slipPlane.K) * (slipPlaneNorm * slipDirectionNorm);
            ret[3, 3] = (slipDirection.L * slipPlane.L) * (slipPlaneNorm * slipDirectionNorm);

            ret[1, 2] = slipDirection.H * slipPlane.K * (slipPlaneNorm * slipDirectionNorm);
            ret[1, 2] += slipDirection.K * slipPlane.H * (slipPlaneNorm * slipDirectionNorm);
            ret[1, 2] *= 0.5;

            ret[2, 1] = slipDirection.H * slipPlane.K * (slipPlaneNorm * slipDirectionNorm);
            ret[2, 1] += slipDirection.K * slipPlane.H * (slipPlaneNorm * slipDirectionNorm);
            ret[2, 1] *= 0.5;

            ret[1, 3] = slipDirection.H * slipPlane.L * (slipPlaneNorm * slipDirectionNorm);
            ret[1, 3] += slipDirection.L * slipPlane.H * (slipPlaneNorm * slipDirectionNorm);
            ret[1, 3] *= 0.5;

            ret[3, 1] = slipDirection.H * slipPlane.L * (slipPlaneNorm * slipDirectionNorm);
            ret[3, 1] += slipDirection.L * slipPlane.H * (slipPlaneNorm * slipDirectionNorm);
            ret[3, 1] *= 0.5;

            ret[2, 3] = slipDirection.K * slipPlane.L * (slipPlaneNorm * slipDirectionNorm);
            ret[2, 3] += slipDirection.L * slipPlane.K * (slipPlaneNorm * slipDirectionNorm);
            ret[2, 3] *= 0.5;

            ret[3, 2] = slipDirection.K * slipPlane.L * (slipPlaneNorm * slipDirectionNorm);
            ret[3, 2] += slipDirection.L * slipPlane.K * (slipPlaneNorm * slipDirectionNorm);
            ret[3, 2] *= 0.5;

            return ret;
        }

        /// <summary>
        /// Alpha, multiplied with the applied stress gets the resolved shear stress
        /// </summary>
        /// <param name="slipPlane"></param>
        /// <param name="slipDirection"></param>
        /// <returns>alpha</returns>
        public MathNet.Numerics.LinearAlgebra.Matrix<double> GetResolvingParameterMain()
        {
            MathNet.Numerics.LinearAlgebra.Matrix<double> ret = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
            double slipPlaneNorm = this.SlipPlane.NormFaktor;
            double slipDirectionNorm = this.MainSlipDirection.NormFaktor;
            if (slipPlaneNorm != 0 && slipDirectionNorm != 0)
            {
                ret[0, 0] = (this.MainSlipDirection.H * this.SlipPlane.H) * (slipPlaneNorm * slipDirectionNorm);
                ret[1, 1] = (this.MainSlipDirection.K * this.SlipPlane.K) * (slipPlaneNorm * slipDirectionNorm);
                ret[2, 2] = (this.MainSlipDirection.L * this.SlipPlane.L) * (slipPlaneNorm * slipDirectionNorm);

                ret[0, 1] = this.MainSlipDirection.H * this.SlipPlane.K * (slipPlaneNorm * slipDirectionNorm);
                ret[0, 1] += this.MainSlipDirection.K * this.SlipPlane.H * (slipPlaneNorm * slipDirectionNorm);
                ret[0, 1] *= 0.5;

                ret[1, 0] = this.MainSlipDirection.H * this.SlipPlane.K * (slipPlaneNorm * slipDirectionNorm);
                ret[1, 0] += this.MainSlipDirection.K * this.SlipPlane.H * (slipPlaneNorm * slipDirectionNorm);
                ret[1, 0] *= 0.5;

                ret[0, 2] = this.MainSlipDirection.H * this.SlipPlane.L * (slipPlaneNorm * slipDirectionNorm);
                ret[0, 2] += this.MainSlipDirection.L * this.SlipPlane.H * (slipPlaneNorm * slipDirectionNorm);
                ret[0, 2] *= 0.5;

                ret[2, 0] = this.MainSlipDirection.H * this.SlipPlane.L * (slipPlaneNorm * slipDirectionNorm);
                ret[2, 0] += this.MainSlipDirection.L * this.SlipPlane.H * (slipPlaneNorm * slipDirectionNorm);
                ret[2, 0] *= 0.5;

                ret[1, 2] = this.MainSlipDirection.K * this.SlipPlane.L * (slipPlaneNorm * slipDirectionNorm);
                ret[1, 2] += this.MainSlipDirection.L * this.SlipPlane.K * (slipPlaneNorm * slipDirectionNorm);
                ret[1, 2] *= 0.5;

                ret[2, 1] = this.MainSlipDirection.K * this.SlipPlane.L * (slipPlaneNorm * slipDirectionNorm);
                ret[2, 1] += this.MainSlipDirection.L * this.SlipPlane.K * (slipPlaneNorm * slipDirectionNorm);
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
        public MathNet.Numerics.LinearAlgebra.Matrix<double> GetResolvingParameterMainHexTestAC1(double a, double c)
        {
            MathNet.Numerics.LinearAlgebra.Matrix<double> ret = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
            double slipPlaneNorm = this.SlipPlane.NormFaktor;
            double slipDirectionNorm = this.MainSlipDirection.NormFaktor;

            double acFactor = a / c;
            if (slipPlaneNorm != 0 && slipDirectionNorm != 0)
            {
                ret[0, 0] = (this.MainSlipDirection.H * this.SlipPlane.H) * (slipPlaneNorm * slipDirectionNorm);
                ret[1, 1] = (this.MainSlipDirection.K * this.SlipPlane.K) * (slipPlaneNorm * slipDirectionNorm);
                ret[2, 2] = Math.Pow(acFactor, 2) * (this.MainSlipDirection.L * this.SlipPlane.L) * (slipPlaneNorm * slipDirectionNorm);

                ret[0, 1] = this.MainSlipDirection.H * this.SlipPlane.K * (slipPlaneNorm * slipDirectionNorm);
                ret[0, 1] += this.MainSlipDirection.K * this.SlipPlane.H * (slipPlaneNorm * slipDirectionNorm);
                ret[0, 1] *= 0.5;

                ret[1, 0] = this.MainSlipDirection.H * this.SlipPlane.K * (slipPlaneNorm * slipDirectionNorm);
                ret[1, 0] += this.MainSlipDirection.K * this.SlipPlane.H * (slipPlaneNorm * slipDirectionNorm);
                ret[1, 0] *= 0.5;

                ret[0, 2] = acFactor * this.MainSlipDirection.H * this.SlipPlane.L * (slipPlaneNorm * slipDirectionNorm);
                ret[0, 2] += acFactor * this.MainSlipDirection.L * this.SlipPlane.H * (slipPlaneNorm * slipDirectionNorm);
                ret[0, 2] *= 0.5;

                ret[2, 0] = acFactor * this.MainSlipDirection.H * this.SlipPlane.L * (slipPlaneNorm * slipDirectionNorm);
                ret[2, 0] += acFactor * this.MainSlipDirection.L * this.SlipPlane.H * (slipPlaneNorm * slipDirectionNorm);
                ret[2, 0] *= 0.5;

                ret[1, 2] = acFactor * this.MainSlipDirection.K * this.SlipPlane.L * (slipPlaneNorm * slipDirectionNorm);
                ret[1, 2] += acFactor * this.MainSlipDirection.L * this.SlipPlane.K * (slipPlaneNorm * slipDirectionNorm);
                ret[1, 2] *= 0.5;

                ret[2, 1] = acFactor * this.MainSlipDirection.K * this.SlipPlane.L * (slipPlaneNorm * slipDirectionNorm);
                ret[2, 1] += acFactor * this.MainSlipDirection.L * this.SlipPlane.K * (slipPlaneNorm * slipDirectionNorm);
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
        public MathNet.Numerics.LinearAlgebra.Matrix<double> GetResolvingParameterMainHexTestAC2(double a, double c)
        {
            MathNet.Numerics.LinearAlgebra.Matrix<double> ret = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
            double slipPlaneNorm = this.SlipPlane.NormFaktor;
            double slipDirectionNorm = this.MainSlipDirection.NormFaktor;

            double aFactor = 1 / a;
            double cFactor = 1 / c;
            if (slipPlaneNorm != 0 && slipDirectionNorm != 0)
            {
                ret[0, 0] = Math.Pow(aFactor, 2) * (this.MainSlipDirection.H * this.SlipPlane.H) * (slipPlaneNorm * slipDirectionNorm);
                ret[1, 1] = Math.Pow(aFactor, 2) * (this.MainSlipDirection.K * this.SlipPlane.K) * (slipPlaneNorm * slipDirectionNorm);
                ret[2, 2] = Math.Pow(cFactor, 2) * (this.MainSlipDirection.L * this.SlipPlane.L) * (slipPlaneNorm * slipDirectionNorm);

                ret[0, 1] = Math.Pow(aFactor, 2) * this.MainSlipDirection.H * this.SlipPlane.K * (slipPlaneNorm * slipDirectionNorm);
                ret[0, 1] += Math.Pow(aFactor, 2) * this.MainSlipDirection.K * this.SlipPlane.H * (slipPlaneNorm * slipDirectionNorm);
                ret[0, 1] *= 0.5;

                ret[1, 0] = Math.Pow(aFactor, 2) * this.MainSlipDirection.H * this.SlipPlane.K * (slipPlaneNorm * slipDirectionNorm);
                ret[1, 0] += Math.Pow(aFactor, 2) * this.MainSlipDirection.K * this.SlipPlane.H * (slipPlaneNorm * slipDirectionNorm);
                ret[1, 0] *= 0.5;

                ret[0, 2] = aFactor * cFactor * this.MainSlipDirection.H * this.SlipPlane.L * (slipPlaneNorm * slipDirectionNorm);
                ret[0, 2] += aFactor * cFactor * this.MainSlipDirection.L * this.SlipPlane.H * (slipPlaneNorm * slipDirectionNorm);
                ret[0, 2] *= 0.5;

                ret[2, 0] = aFactor * cFactor * this.MainSlipDirection.H * this.SlipPlane.L * (slipPlaneNorm * slipDirectionNorm);
                ret[2, 0] += aFactor * cFactor * this.MainSlipDirection.L * this.SlipPlane.H * (slipPlaneNorm * slipDirectionNorm);
                ret[2, 0] *= 0.5;

                ret[1, 2] = aFactor * cFactor * this.MainSlipDirection.K * this.SlipPlane.L * (slipPlaneNorm * slipDirectionNorm);
                ret[1, 2] += aFactor * cFactor * this.MainSlipDirection.L * this.SlipPlane.K * (slipPlaneNorm * slipDirectionNorm);
                ret[1, 2] *= 0.5;

                ret[2, 1] = aFactor * cFactor * this.MainSlipDirection.K * this.SlipPlane.L * (slipPlaneNorm * slipDirectionNorm);
                ret[2, 1] += aFactor * cFactor * this.MainSlipDirection.L * this.SlipPlane.K * (slipPlaneNorm * slipDirectionNorm);
                ret[2, 1] *= 0.5;
            }

            return ret;
        }

        public MathNet.Numerics.LinearAlgebra.Matrix<double> GetResolvingParameter()
        {
            MathNet.Numerics.LinearAlgebra.Matrix<double> ret = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
            double slipPlaneNorm = this.SlipPlane.NormFaktor;
            double slipDirectionNorm = this.MainSlipDirection.NormFaktor;

            ret[0, 0] = (this.MainSlipDirection.H * this.SlipPlane.H) * (slipPlaneNorm * slipDirectionNorm);
            ret[1, 1] = (this.MainSlipDirection.K * this.SlipPlane.K) * (slipPlaneNorm * slipDirectionNorm);
            ret[2, 2] = (this.MainSlipDirection.L * this.SlipPlane.L) * (slipPlaneNorm * slipDirectionNorm);

            ret[0, 1] = this.MainSlipDirection.H * this.SlipPlane.K * (slipPlaneNorm * slipDirectionNorm);
            ret[0, 1] += this.MainSlipDirection.K * this.SlipPlane.H * (slipPlaneNorm * slipDirectionNorm);
            ret[0, 1] *= 0.5;

            ret[1, 0] = this.MainSlipDirection.H * this.SlipPlane.K * (slipPlaneNorm * slipDirectionNorm);
            ret[1, 0] += this.MainSlipDirection.K * this.SlipPlane.H * (slipPlaneNorm * slipDirectionNorm);
            ret[1, 0] *= 0.5;

            ret[0, 2] = this.MainSlipDirection.H * this.SlipPlane.L * (slipPlaneNorm * slipDirectionNorm);
            ret[0, 2] += this.MainSlipDirection.L * this.SlipPlane.H * (slipPlaneNorm * slipDirectionNorm);
            ret[0, 2] *= 0.5;

            ret[2, 0] = this.MainSlipDirection.H * this.SlipPlane.L * (slipPlaneNorm * slipDirectionNorm);
            ret[2, 0] += this.MainSlipDirection.L * this.SlipPlane.H * (slipPlaneNorm * slipDirectionNorm);
            ret[2, 0] *= 0.5;

            ret[1, 2] = this.MainSlipDirection.K * this.SlipPlane.L * (slipPlaneNorm * slipDirectionNorm);
            ret[1, 2] += this.MainSlipDirection.L * this.SlipPlane.K * (slipPlaneNorm * slipDirectionNorm);
            ret[1, 2] *= 0.5;

            ret[2, 1] = this.MainSlipDirection.K * this.SlipPlane.L * (slipPlaneNorm * slipDirectionNorm);
            ret[2, 1] += this.MainSlipDirection.L * this.SlipPlane.K * (slipPlaneNorm * slipDirectionNorm);
            ret[2, 1] *= 0.5;

            return ret;
        }

        #endregion

        #region Data Visual

        

        #region Old Stuff (still active)

        public Pattern.Counts CalculatedCrystalStrainElastic(Microsopic.ElasticityTensors eT)
        {
            Pattern.Counts Ret = new Pattern.Counts();
            Tools.FourthRankTensor compliances = eT.GetFourtRankCompliances();
            for (int n = 0; n < this.PeakData.Count; n++)
            {
                this.PeakData[n].Sort((a, b) => a.Stress.CompareTo(b.Stress));
                for (int i = 0; i < this.PeakData[n].Count; i++)
                {
                    MathNet.Numerics.LinearAlgebra.Matrix<double> strainTensor = compliances * PeakData[n][i].CrystalSystemStress;

                    #region OldStuff
                    //MathNet.Numerics.LinearAlgebra.Matrix<double> strainTensor = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

                    //strainTensor[0, 0] += eT.S11 * PeakData[n][i].CrystalSystemStress[0, 0];
                    //strainTensor[0, 0] += eT.S12 * PeakData[n][i].CrystalSystemStress[1, 1];
                    //strainTensor[0, 0] += eT.S13 * PeakData[n][i].CrystalSystemStress[2, 2];
                    //strainTensor[0, 0] += eT.S14 * PeakData[n][i].CrystalSystemStress[1, 2];
                    //strainTensor[0, 0] += eT.S15 * PeakData[n][i].CrystalSystemStress[0, 2];
                    //strainTensor[0, 0] += eT.S16 * PeakData[n][i].CrystalSystemStress[0, 1];

                    //strainTensor[1, 1] += eT.S21 * PeakData[n][i].CrystalSystemStress[0, 0];
                    //strainTensor[1, 1] += eT.S22 * PeakData[n][i].CrystalSystemStress[1, 1];
                    //strainTensor[1, 1] += eT.S23 * PeakData[n][i].CrystalSystemStress[2, 2];
                    //strainTensor[1, 1] += eT.S24 * PeakData[n][i].CrystalSystemStress[1, 2];
                    //strainTensor[1, 1] += eT.S25 * PeakData[n][i].CrystalSystemStress[0, 2];
                    //strainTensor[1, 1] += eT.S26 * PeakData[n][i].CrystalSystemStress[0, 1];

                    //strainTensor[2, 2] += eT.S31 * PeakData[n][i].CrystalSystemStress[0, 0];
                    //strainTensor[2, 2] += eT.S32 * PeakData[n][i].CrystalSystemStress[1, 1];
                    //strainTensor[2, 2] += eT.S33 * PeakData[n][i].CrystalSystemStress[2, 2];
                    //strainTensor[2, 2] += eT.S34 * PeakData[n][i].CrystalSystemStress[1, 2];
                    //strainTensor[2, 2] += eT.S35 * PeakData[n][i].CrystalSystemStress[0, 2];
                    //strainTensor[2, 2] += eT.S36 * PeakData[n][i].CrystalSystemStress[0, 1];

                    //strainTensor[1, 2] += eT.S41 * PeakData[n][i].CrystalSystemStress[0, 0];
                    //strainTensor[1, 2] += eT.S42 * PeakData[n][i].CrystalSystemStress[1, 1];
                    //strainTensor[1, 2] += eT.S43 * PeakData[n][i].CrystalSystemStress[2, 2];
                    //strainTensor[1, 2] += eT.S44 * PeakData[n][i].CrystalSystemStress[1, 2];
                    //strainTensor[1, 2] += eT.S45 * PeakData[n][i].CrystalSystemStress[0, 2];
                    //strainTensor[1, 2] += eT.S46 * PeakData[n][i].CrystalSystemStress[0, 1];

                    //strainTensor[0, 2] += eT.S51 * PeakData[n][i].CrystalSystemStress[0, 0];
                    //strainTensor[0, 2] += eT.S52 * PeakData[n][i].CrystalSystemStress[1, 1];
                    //strainTensor[0, 2] += eT.S53 * PeakData[n][i].CrystalSystemStress[2, 2];
                    //strainTensor[0, 2] += eT.S54 * PeakData[n][i].CrystalSystemStress[1, 2];
                    //strainTensor[0, 2] += eT.S55 * PeakData[n][i].CrystalSystemStress[0, 2];
                    //strainTensor[0, 2] += eT.S56 * PeakData[n][i].CrystalSystemStress[0, 1];

                    //strainTensor[0, 1] += eT.S61 * PeakData[n][i].CrystalSystemStress[0, 0];
                    //strainTensor[0, 1] += eT.S62 * PeakData[n][i].CrystalSystemStress[1, 1];
                    //strainTensor[0, 1] += eT.S63 * PeakData[n][i].CrystalSystemStress[2, 2];
                    //strainTensor[0, 1] += eT.S64 * PeakData[n][i].CrystalSystemStress[1, 2];
                    //strainTensor[0, 1] += eT.S65 * PeakData[n][i].CrystalSystemStress[0, 2];
                    //strainTensor[0, 1] += eT.S66 * PeakData[n][i].CrystalSystemStress[0, 1];

                    #endregion

                    this.MeasurementCrystalTransformation.MM = this.MeasurementCrystalTransformation.OM.Transpose();

                    double measStrain = this.MeasurementCrystalTransformation.MM[2, 0] * this.MeasurementCrystalTransformation.MM[2, 0] * strainTensor[0, 0];
                    measStrain += this.MeasurementCrystalTransformation.MM[2, 1] * this.MeasurementCrystalTransformation.MM[2, 0] * strainTensor[1, 0];
                    measStrain += this.MeasurementCrystalTransformation.MM[2, 2] * this.MeasurementCrystalTransformation.MM[2, 0] * strainTensor[2, 0];

                    measStrain += this.MeasurementCrystalTransformation.MM[2, 0] * this.MeasurementCrystalTransformation.MM[2, 1] * strainTensor[0, 1];
                    measStrain += this.MeasurementCrystalTransformation.MM[2, 1] * this.MeasurementCrystalTransformation.MM[2, 1] * strainTensor[1, 1];
                    measStrain += this.MeasurementCrystalTransformation.MM[2, 2] * this.MeasurementCrystalTransformation.MM[2, 1] * strainTensor[2, 1];

                    measStrain += this.MeasurementCrystalTransformation.MM[2, 0] * this.MeasurementCrystalTransformation.MM[2, 2] * strainTensor[0, 2];
                    measStrain += this.MeasurementCrystalTransformation.MM[2, 1] * this.MeasurementCrystalTransformation.MM[2, 2] * strainTensor[1, 2];
                    measStrain += this.MeasurementCrystalTransformation.MM[2, 2] * this.MeasurementCrystalTransformation.MM[2, 2] * strainTensor[2, 2];

                    //MathNet.Numerics.LinearAlgebra.Matrix<double> stressTensor = PeakData[n][i].MeasurementSystemStress();

                    double[] dTmp = { PeakData[n][i].Stress, measStrain };

                    Ret.Add(dTmp);
                }
            }

            return Ret;
        }

        public Pattern.Counts MacroStrainOverMacroStrainData(bool elastic, double psiAngle)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            for (int n = 0; n < this.PeakData.Count; n++)
            {
                if (Math.Abs(psiAngle - this.PeakData[n][0].PsiAngle) < CalScec.Properties.Settings.Default.PsyAcceptanceAngle)
                {
                    for (int i = 0; i < this.PeakData[n].Count; i++)
                    {
                        this.PeakData[n].Sort((a, b) => a.MacroskopicStrain.CompareTo(b.MacroskopicStrain));
                        if (this.PeakData[n][i].ElasticRegime == elastic)
                        {
                            double[] CTmp = { PeakData[n][i].MacroskopicStrain, PeakData[n][i].MacroskopicStrain, (PeakData[n][i].Stress * Math.Cos(PeakData[n][i].PsiAngle) * Math.Cos(PeakData[n][i].MainSlipDirectionAngle)) };

                            Ret.Add(CTmp);
                        }
                    }
                }
            }

            return Ret;
        }
        public Pattern.Counts MacroStrainOverMicroStrainData(bool elastic, double psiAngle)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            for (int n = 0; n < this.PeakData.Count; n++)
            {
                if (Math.Abs(psiAngle - this.PeakData[n][0].PsiAngle) < CalScec.Properties.Settings.Default.PsyAcceptanceAngle)
                {
                    this.PeakData[n].Sort((a, b) => a.MacroskopicStrain.CompareTo(b.MacroskopicStrain));
                    for (int i = 0; i < this.PeakData[n].Count; i++)
                    {
                        if (this.PeakData[n][i].ElasticRegime == elastic)
                        {
                            double[] CTmp = { PeakData[n][i].Strain, PeakData[n][i].MacroskopicStrain, (PeakData[n][i].Stress * Math.Cos(PeakData[n][i].PsiAngle) * Math.Cos(PeakData[n][i].MainSlipDirectionAngle)) };

                            Ret.Add(CTmp);
                        }
                    }
                }
            }

            return Ret;
        }
        public Pattern.Counts MacroStrainDataOverStress(bool elastic, double psiAngle)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            for (int n = 0; n < this.PeakData.Count; n++)
            {
                if (Math.Abs(psiAngle - this.PeakData[n][0].PsiAngle) < CalScec.Properties.Settings.Default.PsyAcceptanceAngle)
                {
                    this.PeakData[n].Sort((a, b) => a.MacroskopicStrain.CompareTo(b.MacroskopicStrain));
                    for (int i = 0; i < this.PeakData[n].Count; i++)
                    {
                        if (this.PeakData[n][i].ElasticRegime == elastic)
                        {
                            double[] CTmp = { (PeakData[n][i].Stress), PeakData[n][i].MacroskopicStrain, PeakData[n][i].MacroskopicStrain };

                            Ret.Add(CTmp);
                        }
                    }
                }
            }

            return Ret;
        }
        public Pattern.Counts MacroStrainDataOverPlainAdjustedStress(bool elastic, double psiAngle)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            for (int n = 0; n < this.PeakData.Count; n++)
            {
                if (Math.Abs(psiAngle - this.PeakData[n][0].PsiAngle) < CalScec.Properties.Settings.Default.PsyAcceptanceAngle)
                {
                    this.PeakData[n].Sort((a, b) => a.MacroskopicStrain.CompareTo(b.MacroskopicStrain));
                    for (int i = 0; i < this.PeakData[n].Count; i++)
                    {
                        if (this.PeakData[n][i].ElasticRegime == elastic)
                        {
                            //double[] CTmp = { (PeakData[n][i].Stress * Math.Cos(PeakData[n][i].PsiAngle * (Math.PI / 180.0)) * Math.Cos(PeakData[n][i].MainSlipDirectionAngle * (Math.PI / 180.0))), PeakData[n][i].MacroskopicStrain, PeakData[n][i].MacroskopicStrain };
                            //double[] CTmp = { PeakData[n][i].Stress * Math.Cos((PeakData[n][i].MainSlipDirectionAngle + PeakData[n][i].PsiAngle) * (Math.PI / 180.0)), PeakData[n][i].MacroskopicStrain, PeakData[n][i].MacroskopicStrain };
                            double[] CTmp = { (PeakData[n][i].Stress * Math.Cos(PeakData[n][i].MainSlipDirectionAngle * (Math.PI / 180.0))), PeakData[n][i].MacroskopicStrain, PeakData[n][i].MacroskopicStrain };

                            Ret.Add(CTmp);
                        }
                    }
                }
            }

            return Ret;
        }
        public Pattern.Counts MacroStrainDataOverPsiAdjustedStress(bool elastic, double psiAngle)
        {
            Pattern.Counts Ret = new Pattern.Counts();
            MathNet.Numerics.LinearAlgebra.Matrix<double> resolvingParam = this.GetResolvingParameterMain();
            for (int n = 0; n < this.PeakData.Count; n++)
            {
                if (Math.Abs(psiAngle - this.PeakData[n][0].PsiAngle) < CalScec.Properties.Settings.Default.PsyAcceptanceAngle)
                {
                    this.PeakData[n].Sort((a, b) => a.MacroskopicStrain.CompareTo(b.MacroskopicStrain));
                    for (int i = 0; i < this.PeakData[n].Count; i++)
                    {
                        if (this.PeakData[n][i].ElasticRegime == elastic)
                        {
                            //double[] CTmp = { (PeakData[n][i].Stress * Math.Cos(PeakData[n][i].PsiAngle * (Math.PI / 180.0))), PeakData[n][i].MacroskopicStrain, PeakData[n][i].MacroskopicStrain };
                            double[] CTmp = { this.Shearforce(PeakData[n][i].CrystalSystemStress * resolvingParam), PeakData[n][i].MacroskopicStrain, PeakData[n][i].MacroskopicStrain };

                            Ret.Add(CTmp);
                        }
                    }
                }
            }

            return Ret;
        }

        public Pattern.Counts MicroStrainOverMacroStrainData(bool elastic, double psiAngle)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            for(int n = 0; n < this.PeakData.Count; n++)
            {
                if (Math.Abs(psiAngle - this.PeakData[n][0].PsiAngle) < CalScec.Properties.Settings.Default.PsyAcceptanceAngle)
                {
                    this.PeakData[n].Sort((a, b) => a.MacroskopicStrain.CompareTo(b.MacroskopicStrain));
                    for (int i = 0; i < this.PeakData[n].Count; i++)
                    {
                        if (this.PeakData[n][i].ElasticRegime == elastic)
                        {
                            double[] CTmp = { PeakData[n][i].MacroskopicStrain, PeakData[n][i].Strain, (PeakData[n][i].Stress * Math.Cos(PeakData[n][i].PsiAngle) * Math.Cos(PeakData[n][i].MainSlipDirectionAngle)) };

                            Ret.Add(CTmp);
                        }
                    }
                }
            }

            return Ret;
        }
        public Pattern.Counts MicroStrainOverMicroStrainData(bool elastic, double psiAngle)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            for (int n = 0; n < this.PeakData.Count; n++)
            {
                if (Math.Abs(psiAngle - this.PeakData[n][0].PsiAngle) < CalScec.Properties.Settings.Default.PsyAcceptanceAngle)
                {
                    this.PeakData[n].Sort((a, b) => a.MacroskopicStrain.CompareTo(b.MacroskopicStrain));
                    for (int i = 0; i < this.PeakData[n].Count; i++)
                    {
                        if (this.PeakData[n][i].ElasticRegime == elastic)
                        {
                            double[] CTmp = { PeakData[n][i].Strain, PeakData[n][i].Strain, (PeakData[n][i].Stress * Math.Cos(PeakData[n][i].PsiAngle) * Math.Cos(PeakData[n][i].MainSlipDirectionAngle)) };

                            Ret.Add(CTmp);
                        }
                    }
                }
            }

            return Ret;
        }
        public Pattern.Counts MicroStrainDataOverStress(bool elastic, double psiAngle)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            for (int n = 0; n < this.PeakData.Count; n++)
            {
                if (Math.Abs(psiAngle - this.PeakData[n][0].PsiAngle) < CalScec.Properties.Settings.Default.PsyAcceptanceAngle)
                {
                    this.PeakData[n].Sort((a, b) => a.MacroskopicStrain.CompareTo(b.MacroskopicStrain));
                    for (int i = 0; i < this.PeakData[n].Count; i++)
                    {
                        if (this.PeakData[n][i].ElasticRegime == elastic)
                        {
                            double[] CTmp = { (PeakData[n][i].Stress), PeakData[n][i].Strain, PeakData[n][i].MacroskopicStrain };

                            Ret.Add(CTmp);
                        }
                    }
                }
            }

            return Ret;
        }
        public Pattern.Counts MicroStrainDataOverPsiAdjustedStress(bool elastic, double psiAngle)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            for (int n = 0; n < this.PeakData.Count; n++)
            {
                if (Math.Abs(psiAngle - this.PeakData[n][0].PsiAngle) < CalScec.Properties.Settings.Default.PsyAcceptanceAngle)
                {
                    this.PeakData[n].Sort((a, b) => a.MacroskopicStrain.CompareTo(b.MacroskopicStrain));
                    for (int i = 0; i < this.PeakData[n].Count; i++)
                    {
                        if (this.PeakData[n][i].ElasticRegime == elastic)
                        {
                            double[] CTmp = { (PeakData[n][i].Stress * Math.Cos(PeakData[n][i].PsiAngle * (Math.PI / 180.0))), PeakData[n][i].Strain, PeakData[n][i].MacroskopicStrain };

                            Ret.Add(CTmp);
                        }
                    }
                }
            }

            return Ret;
        }
        public Pattern.Counts MicroStrainDataOverPlainAdjustedStress(bool elastic, double psiAngle)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            MathNet.Numerics.LinearAlgebra.Matrix<double> resolvingParam = this.GetResolvingParameterMain();
            for (int n = 0; n < this.PeakData.Count; n++)
            {
                if (Math.Abs(psiAngle - this.PeakData[n][0].PsiAngle) < CalScec.Properties.Settings.Default.PsyAcceptanceAngle)
                {
                    this.PeakData[n].Sort((a, b) => a.MacroskopicStrain.CompareTo(b.MacroskopicStrain));
                    for (int i = 0; i < this.PeakData[n].Count; i++)
                    {
                        if (this.PeakData[n][i].ElasticRegime == elastic)
                        {
                            //double[] CTmp = { (PeakData[n][i].Stress * Math.Cos(PeakData[n][i].PsiAngle * (Math.PI / 180.0)) * Math.Cos(PeakData[n][i].MainSlipDirectionAngle * (Math.PI / 180.0))), PeakData[n][i].Strain, PeakData[n][i].MacroskopicStrain };
                            //double[] CTmp = { (PeakData[n][i].Stress) * Math.Cos((PeakData[n][i].MainSlipDirectionAngle + PeakData[n][i].PsiAngle) * (Math.PI / 180.0)), PeakData[n][i].Strain, PeakData[n][i].MacroskopicStrain };
                            double[] CTmp = { this.Shearforce(PeakData[n][i].CrystalSystemStress * resolvingParam), PeakData[n][i].Strain, PeakData[n][i].MacroskopicStrain };

                            Ret.Add(CTmp);
                        }
                    }
                }
            }

            return Ret;
        }

        public Pattern.Counts StressOverMacroStrainData(bool elastic, double psiAngle)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            for (int n = 0; n < this.PeakData.Count; n++)
            {
                if (Math.Abs(psiAngle - this.PeakData[n][0].PsiAngle) < CalScec.Properties.Settings.Default.PsyAcceptanceAngle)
                {
                    this.PeakData[n].Sort((a, b) => a.MacroskopicStrain.CompareTo(b.MacroskopicStrain));
                    for (int i = 0; i < this.PeakData[n].Count; i++)
                    {
                        if (this.PeakData[n][i].ElasticRegime == elastic)
                        {
                            double[] CTmp = { PeakData[n][i].MacroskopicStrain, (PeakData[n][i].Stress), PeakData[n][i].MacroskopicStrain };

                            Ret.Add(CTmp);
                        }
                    }
                }
            }

            return Ret;
        }
        public Pattern.Counts StressOverMicroStrainData(bool elastic, double psiAngle)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            for (int n = 0; n < this.PeakData.Count; n++)
            {
                if (Math.Abs(psiAngle - this.PeakData[n][0].PsiAngle) < CalScec.Properties.Settings.Default.PsyAcceptanceAngle)
                {
                    this.PeakData[n].Sort((a, b) => a.Strain.CompareTo(b.Strain));
                    for (int i = 0; i < this.PeakData[n].Count; i++)
                    {
                        if (this.PeakData[n][i].ElasticRegime == elastic)
                        {
                            double[] CTmp = { PeakData[n][i].Strain, (PeakData[n][i].Stress), PeakData[n][i].MacroskopicStrain };

                            Ret.Add(CTmp);
                        }
                    }
                }
            }

            return Ret;
        }
        public Pattern.Counts StressOverStressData(bool elastic, double psiAngle)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            for (int n = 0; n < this.PeakData.Count; n++)
            {
                if (Math.Abs(psiAngle - this.PeakData[n][0].PsiAngle) < CalScec.Properties.Settings.Default.PsyAcceptanceAngle)
                {
                    this.PeakData[n].Sort((a, b) => a.Stress.CompareTo(b.Stress));
                    for (int i = 0; i < this.PeakData[n].Count; i++)
                    {
                        if (this.PeakData[n][i].ElasticRegime == elastic)
                        {
                            double[] CTmp = { PeakData[n][i].Stress, (PeakData[n][i].Stress), PeakData[n][i].MacroskopicStrain };

                            Ret.Add(CTmp);
                        }
                    }
                }
            }

            return Ret;
        }
        public Pattern.Counts StressOverPsiAdjustedStressData(bool elastic, double psiAngle)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            for (int n = 0; n < this.PeakData.Count; n++)
            {
                if (Math.Abs(psiAngle - this.PeakData[n][0].PsiAngle) < CalScec.Properties.Settings.Default.PsyAcceptanceAngle)
                {
                    this.PeakData[n].Sort((a, b) => a.Stress.CompareTo(b.Stress));
                    for (int i = 0; i < this.PeakData[n].Count; i++)
                    {
                        if (this.PeakData[n][i].ElasticRegime == elastic)
                        {
                            double[] CTmp = { (PeakData[n][i].Stress * Math.Cos(PeakData[n][i].PsiAngle * (Math.PI / 180.0))), (PeakData[n][i].Stress), PeakData[n][i].MacroskopicStrain };

                            Ret.Add(CTmp);
                        }
                    }
                }
            }

            return Ret;
        }
        public Pattern.Counts StressOverPlainAdjustedStressData(bool elastic, double psiAngle)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            MathNet.Numerics.LinearAlgebra.Matrix<double> resolvingParam = this.GetResolvingParameterMain();
            for (int n = 0; n < this.PeakData.Count; n++)
            {
                if (Math.Abs(psiAngle - this.PeakData[n][0].PsiAngle) < CalScec.Properties.Settings.Default.PsyAcceptanceAngle)
                {
                    this.PeakData[n].Sort((a, b) => a.Stress.CompareTo(b.Stress));
                    for (int i = 0; i < this.PeakData[n].Count; i++)
                    {
                        if (this.PeakData[n][i].ElasticRegime == elastic)
                        {
                            double resolvingForce = PeakData[n][i].CrystalSystemStress[0, 0] * resolvingParam[0, 0];
                            resolvingForce += PeakData[n][i].CrystalSystemStress[1, 0] * resolvingParam[1, 0];
                            resolvingForce += PeakData[n][i].CrystalSystemStress[2, 0] * resolvingParam[2, 0];
                            resolvingForce += PeakData[n][i].CrystalSystemStress[0, 1] * resolvingParam[0, 1];
                            resolvingForce += PeakData[n][i].CrystalSystemStress[1, 1] * resolvingParam[1, 1];
                            resolvingForce += PeakData[n][i].CrystalSystemStress[2, 1] * resolvingParam[2, 1];
                            resolvingForce += PeakData[n][i].CrystalSystemStress[0, 2] * resolvingParam[0, 2];
                            resolvingForce += PeakData[n][i].CrystalSystemStress[1, 2] * resolvingParam[1, 2];
                            resolvingForce += PeakData[n][i].CrystalSystemStress[2, 2] * resolvingParam[2, 2];

                            //double[] CTmp = { (PeakData[n][i].Stress * Math.Cos(PeakData[n][i].PsiAngle * (Math.PI / 180.0)) * Math.Cos(PeakData[n][i].MainSlipDirectionAngle * (Math.PI / 180.0))), (PeakData[n][i].Stress), PeakData[n][i].MacroskopicStrain };
                            //double[] CTmp = { (PeakData[n][i].Stress) * Math.Cos((PeakData[n][i].MainSlipDirectionAngle + PeakData[n][i].PsiAngle) * (Math.PI / 180.0)), (PeakData[n][i].Stress), PeakData[n][i].MacroskopicStrain };
                            double[] CTmp = { resolvingForce, (PeakData[n][i].Stress), PeakData[n][i].MacroskopicStrain };

                            Ret.Add(CTmp);
                        }
                    }
                }
            }

            return Ret;
        }

        public Pattern.Counts PsiAdjustedStressOverMacroStrainData(bool elastic, double psiAngle)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            for (int n = 0; n < this.PeakData.Count; n++)
            {
                if (Math.Abs(psiAngle - this.PeakData[n][0].PsiAngle) < CalScec.Properties.Settings.Default.PsyAcceptanceAngle)
                {
                    //this.PeakData[n].Sort((a, b) => a.MacroskopicStrain.CompareTo(b.MacroskopicStrain));
                    for (int i = 0; i < this.PeakData[n].Count; i++)
                    {
                        if (this.PeakData[n][i].ElasticRegime == elastic)
                        {
                            double[] CTmp = { PeakData[n][i].MacroskopicStrain, (PeakData[n][i].Stress * Math.Cos(PeakData[n][i].PsiAngle * (Math.PI / 180.0))), PeakData[n][i].MacroskopicStrain };

                            Ret.Add(CTmp);
                        }
                    }
                }
            }

            return Ret;
        }
        public Pattern.Counts PsiAdjustedStressOverMicroStrainData(bool elastic, double psiAngle)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            for (int n = 0; n < this.PeakData.Count; n++)
            {
                if (Math.Abs(psiAngle - this.PeakData[n][0].PsiAngle) < CalScec.Properties.Settings.Default.PsyAcceptanceAngle)
                {
                    //this.PeakData[n].Sort((a, b) => a.Strain.CompareTo(b.Strain));
                    for (int i = 0; i < this.PeakData[n].Count; i++)
                    {
                        if (this.PeakData[n][i].ElasticRegime == elastic)
                        {
                            double[] CTmp = { PeakData[n][i].Strain, (PeakData[n][i].Stress * Math.Cos(PeakData[n][i].PsiAngle * (Math.PI / 180.0))), PeakData[n][i].MacroskopicStrain };

                            Ret.Add(CTmp);
                        }
                    }
                }
            }

            return Ret;
        }
        public Pattern.Counts PsiAdjustedStressOverStressData(bool elastic, double psiAngle)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            for (int n = 0; n < this.PeakData.Count; n++)
            {
                if (Math.Abs(psiAngle - this.PeakData[n][0].PsiAngle) < CalScec.Properties.Settings.Default.PsyAcceptanceAngle)
                {
                    //this.PeakData[n].Sort((a, b) => a.Stress.CompareTo(b.Stress));
                    for (int i = 0; i < this.PeakData[n].Count; i++)
                    {
                        if (this.PeakData[n][i].ElasticRegime == elastic)
                        {
                            double[] CTmp = { PeakData[n][i].Stress, (PeakData[n][i].Stress * Math.Cos(PeakData[n][i].PsiAngle * (Math.PI / 180.0))), PeakData[n][i].MacroskopicStrain };

                            Ret.Add(CTmp);
                        }
                    }
                }
            }

            return Ret;
        }
        public Pattern.Counts PsiAdjustedStressOverPsiAdjustedStressData(bool elastic, double psiAngle)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            for (int n = 0; n < this.PeakData.Count; n++)
            {
                if (Math.Abs(psiAngle - this.PeakData[n][0].PsiAngle) < CalScec.Properties.Settings.Default.PsyAcceptanceAngle)
                {
                    //this.PeakData[n].Sort((a, b) => a.Stress.CompareTo(b.Stress));
                    for (int i = 0; i < this.PeakData[n].Count; i++)
                    {
                        if (this.PeakData[n][i].ElasticRegime == elastic)
                        {
                            double[] CTmp = { (PeakData[n][i].Stress * Math.Cos(PeakData[n][i].PsiAngle * (Math.PI / 180.0))), (PeakData[n][i].Stress * Math.Cos(PeakData[n][i].PsiAngle * (Math.PI / 180.0))), PeakData[n][i].MacroskopicStrain };

                            Ret.Add(CTmp);
                        }
                    }
                }
            }

            return Ret;
        }
        public Pattern.Counts PsiAdjustedStressOverPlainAdjustedStressData(bool elastic, double psiAngle)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            MathNet.Numerics.LinearAlgebra.Matrix<double> resolvingParam = this.GetResolvingParameterMain();
            for (int n = 0; n < this.PeakData.Count; n++)
            {
                if (Math.Abs(psiAngle - this.PeakData[n][0].PsiAngle) < CalScec.Properties.Settings.Default.PsyAcceptanceAngle)
                {
                    //this.PeakData[n].Sort((a, b) => a.Stress.CompareTo(b.Stress));
                    for (int i = 0; i < this.PeakData[n].Count; i++)
                    {
                        if (this.PeakData[n][i].ElasticRegime == elastic)
                        {
                            //double[] CTmp = { (PeakData[n][i].Stress * Math.Cos(PeakData[n][i].PsiAngle * (Math.PI / 180.0)) * Math.Cos(PeakData[n][i].MainSlipDirectionAngle * (Math.PI / 180.0))), (PeakData[n][i].Stress * Math.Cos(PeakData[n][i].PsiAngle * (Math.PI / 180.0))), PeakData[n][i].MacroskopicStrain };
                            //double[] CTmp = { (PeakData[n][i].Stress) * Math.Cos((PeakData[n][i].MainSlipDirectionAngle + PeakData[n][i].PsiAngle) * (Math.PI / 180.0)), (PeakData[n][i].Stress * Math.Cos(PeakData[n][i].PsiAngle * (Math.PI / 180.0))), PeakData[n][i].MacroskopicStrain };
                            double[] CTmp = { this.Shearforce(PeakData[n][i].CrystalSystemStress * resolvingParam), (PeakData[n][i].Stress * Math.Cos(PeakData[n][i].PsiAngle * (Math.PI / 180.0))), PeakData[n][i].MacroskopicStrain };

                            Ret.Add(CTmp);
                        }
                    }
                }
            }

            return Ret;
        }

        public Pattern.Counts PlainAdjustedStressOverMacroStrainData(bool elastic, double psiAngle)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            MathNet.Numerics.LinearAlgebra.Matrix<double> resolvingParam = this.GetResolvingParameterMain();
            for (int n = 0; n < this.PeakData.Count; n++)
            {
                if (Math.Abs(psiAngle - this.PeakData[n][0].PsiAngle) < CalScec.Properties.Settings.Default.PsyAcceptanceAngle)
                {
                    this.PeakData[n].Sort((a, b) => a.MacroskopicStrain.CompareTo(b.MacroskopicStrain));
                    for (int i = 0; i < this.PeakData[n].Count; i++)
                    {
                        if (this.PeakData[n][i].ElasticRegime == elastic)
                        {
                            //double[] CTmp = { PeakData[n][i].MacroskopicStrain, (PeakData[n][i].Stress * Math.Cos(PeakData[n][i].PsiAngle * (Math.PI / 180.0)) * Math.Cos(PeakData[n][i].MainSlipDirectionAngle * (Math.PI / 180.0))), PeakData[n][i].MacroskopicStrain };
                            //double[] CTmp = { PeakData[n][i].MacroskopicStrain, (PeakData[n][i].Stress * Math.Cos((PeakData[n][i].MainSlipDirectionAngle + PeakData[n][i].PsiAngle) * (Math.PI / 180.0))), PeakData[n][i].MacroskopicStrain };
                            double[] CTmp = { PeakData[n][i].MacroskopicStrain, this.Shearforce(PeakData[n][i].CrystalSystemStress), PeakData[n][i].MacroskopicStrain };

                            Ret.Add(CTmp);
                        }
                    }
                }
            }

            return Ret;
        }
        public Pattern.Counts PlainAdjustedStressOverMicroStrainData(bool elastic, double psiAngle)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            MathNet.Numerics.LinearAlgebra.Matrix<double> resolvingParam = this.GetResolvingParameterMain();
            for (int n = 0; n < this.PeakData.Count; n++)
            {
                if (Math.Abs(psiAngle - this.PeakData[n][0].PsiAngle) < CalScec.Properties.Settings.Default.PsyAcceptanceAngle)
                {
                    this.PeakData[n].Sort((a, b) => a.Stress.CompareTo(b.Stress));
                    for (int i = 0; i < this.PeakData[n].Count; i++)
                    {
                        if (this.PeakData[n][i].ElasticRegime == elastic)
                        {
                            //double resolvingForce = PeakData[n][i].CrystalSystemStress[0, 0] * resolvingParam[0, 0];
                            //resolvingForce += PeakData[n][i].CrystalSystemStress[1, 0] * resolvingParam[1, 0];
                            //resolvingForce += PeakData[n][i].CrystalSystemStress[2, 0] * resolvingParam[2, 0];
                            //resolvingForce += PeakData[n][i].CrystalSystemStress[0, 1] * resolvingParam[0, 1];
                            //resolvingForce += PeakData[n][i].CrystalSystemStress[1, 1] * resolvingParam[1, 1];
                            //resolvingForce += PeakData[n][i].CrystalSystemStress[2, 1] * resolvingParam[2, 1];
                            //resolvingForce += PeakData[n][i].CrystalSystemStress[0, 2] * resolvingParam[0, 2];
                            //resolvingForce += PeakData[n][i].CrystalSystemStress[1, 2] * resolvingParam[1, 2];
                            //resolvingForce += PeakData[n][i].CrystalSystemStress[2, 2] * resolvingParam[2, 2];
                            //double[] CTmp = { PeakData[n][i].Strain, (PeakData[n][i].Stress * Math.Cos(PeakData[n][i].PsiAngle * (Math.PI / 180.0)) * Math.Cos(PeakData[n][i].MainSlipDirectionAngle * (Math.PI / 180.0))), PeakData[n][i].MacroskopicStrain };
                            //double[] CTmp = { PeakData[n][i].Strain, (PeakData[n][i].Stress * Math.Cos((PeakData[n][i].MainSlipDirectionAngle + PeakData[n][i].PsiAngle) * (Math.PI / 180.0))), PeakData[n][i].MacroskopicStrain };
                            double[] CTmp = { PeakData[n][i].Strain, this.Shearforce(PeakData[n][i].CrystalSystemStress), PeakData[n][i].MacroskopicStrain };

                            Ret.Add(CTmp);
                        }
                    }
                }
            }

            return Ret;
        }
        public Pattern.Counts PlainAdjustedStressOverStressData(bool elastic, double psiAngle)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            MathNet.Numerics.LinearAlgebra.Matrix<double> resolvingParam = this.GetResolvingParameterMain();
            for (int n = 0; n < this.PeakData.Count; n++)
            {
                if (Math.Abs(psiAngle - this.PeakData[n][0].PsiAngle) < CalScec.Properties.Settings.Default.PsyAcceptanceAngle)
                {
                    this.PeakData[n].Sort((a, b) => a.Stress.CompareTo(b.Stress));
                    for (int i = 0; i < this.PeakData[n].Count; i++)
                    {
                        if (this.PeakData[n][i].ElasticRegime == elastic)
                        {
                            //double[] CTmp = { PeakData[n][i].Stress, (PeakData[n][i].Stress * Math.Cos(PeakData[n][i].PsiAngle * (Math.PI / 180.0)) * Math.Cos(PeakData[n][i].MainSlipDirectionAngle * (Math.PI / 180.0))), PeakData[n][i].MacroskopicStrain };
                            //double[] CTmp = { PeakData[n][i].Stress, (PeakData[n][i].Stress * Math.Cos((PeakData[n][i].MainSlipDirectionAngle + PeakData[n][i].PsiAngle) * (Math.PI / 180.0))), PeakData[n][i].MacroskopicStrain };
                            double[] CTmp = { PeakData[n][i].Stress, this.Shearforce(PeakData[n][i].CrystalSystemStress), PeakData[n][i].MacroskopicStrain };

                            Ret.Add(CTmp);
                        }
                    }
                }
            }

            return Ret;
        }
        public Pattern.Counts PlainAdjustedStressOverPsiAdjustedStressData(bool elastic, double psiAngle)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            MathNet.Numerics.LinearAlgebra.Matrix<double> resolvingParam = this.GetResolvingParameterMain();
            for (int n = 0; n < this.PeakData.Count; n++)
            {
                if (Math.Abs(psiAngle - this.PeakData[n][0].PsiAngle) < CalScec.Properties.Settings.Default.PsyAcceptanceAngle)
                {
                    this.PeakData[n].Sort((a, b) => a.Stress.CompareTo(b.Stress));
                    for (int i = 0; i < this.PeakData[n].Count; i++)
                    {
                        if (this.PeakData[n][i].ElasticRegime == elastic)
                        {
                            //double[] CTmp = { PeakData[n][i].Stress * Math.Cos(PeakData[n][i].PsiAngle * (Math.PI / 180.0)), (PeakData[n][i].Stress * Math.Cos(PeakData[n][i].PsiAngle * (Math.PI / 180.0)) * Math.Cos(PeakData[n][i].MainSlipDirectionAngle * (Math.PI / 180.0))), PeakData[n][i].MacroskopicStrain };
                            //double[] CTmp = { PeakData[n][i].Stress * Math.Cos((PeakData[n][i].PsiAngle + PeakData[n][i].PsiAngle) * (Math.PI / 180.0)), (PeakData[n][i].Stress * Math.Cos(PeakData[n][i].MainSlipDirectionAngle * (Math.PI / 180.0))), PeakData[n][i].MacroskopicStrain };
                            double[] CTmp = { PeakData[n][i].Stress * Math.Cos(PeakData[n][i].PsiAngle * (Math.PI / 180.0)), this.Shearforce(PeakData[n][i].CrystalSystemStress), PeakData[n][i].MacroskopicStrain };

                            Ret.Add(CTmp);
                        }
                    }
                }
            }

            return Ret;
        }
        public Pattern.Counts PlainAdjustedStressOverPlainAdjustedStressData(bool elastic, double psiAngle)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            MathNet.Numerics.LinearAlgebra.Matrix<double> resolvingParam = this.GetResolvingParameterMain();
            for (int n = 0; n < this.PeakData.Count; n++)
            {
                if (Math.Abs(psiAngle - this.PeakData[n][0].PsiAngle) < CalScec.Properties.Settings.Default.PsyAcceptanceAngle)
                {
                    this.PeakData[n].Sort((a, b) => a.Stress.CompareTo(b.Stress));
                    for (int i = 0; i < this.PeakData[n].Count; i++)
                    {
                        if (this.PeakData[n][i].ElasticRegime == elastic)
                        {
                            //double[] CTmp = { (PeakData[n][i].Stress * Math.Cos(PeakData[n][i].PsiAngle * (Math.PI / 180.0)) * Math.Cos(PeakData[n][i].MainSlipDirectionAngle * (Math.PI / 180.0))), (PeakData[n][i].Stress * Math.Cos(PeakData[n][i].PsiAngle * (Math.PI / 180.0)) * Math.Cos(PeakData[n][i].MainSlipDirectionAngle * (Math.PI / 180.0))), PeakData[n][i].MacroskopicStrain };
                            //double[] CTmp = { (PeakData[n][i].Stress * Math.Cos((PeakData[n][i].MainSlipDirectionAngle + PeakData[n][i].PsiAngle) * (Math.PI / 180.0))), (PeakData[n][i].Stress * Math.Cos(PeakData[n][i].MainSlipDirectionAngle * (Math.PI / 180.0))), PeakData[n][i].MacroskopicStrain };
                            double[] CTmp = { this.Shearforce(PeakData[n][i].CrystalSystemStress * resolvingParam), this.Shearforce(PeakData[n][i].CrystalSystemStress), PeakData[n][i].MacroskopicStrain };

                            Ret.Add(CTmp);
                        }
                    }
                }
            }

            return Ret;
        }

        #endregion

        #endregion

        public void FitElasticData(double psiAngle)
        {
            Pattern.Counts ElasticData1 = this.MicroStrainOverMacroStrainData(true, psiAngle);
            Pattern.Counts ElasticData2 = this.MicroStrainDataOverStress(true, psiAngle);

            Fitting.LMA.FitMacroElasticModul(this.ElasticLinearFit1, ElasticData1);
            Fitting.LMA.FitMacroElasticModul(this.ElasticLinearFit2, ElasticData2);
        }

        public void FitPlasticData(double psiAngle)
        {
            Pattern.Counts PlasticData1 = this.MicroStrainOverMacroStrainData(false, psiAngle);
            Pattern.Counts PlasticData2 = this.MicroStrainDataOverStress(false, psiAngle);

            Fitting.LMA.FitMacroElasticModul(this.PlasticLinearFit1, PlasticData1);
            Fitting.LMA.FitMacroElasticModul(this.PlasticLinearFit2, PlasticData2);
        }
    }
}
