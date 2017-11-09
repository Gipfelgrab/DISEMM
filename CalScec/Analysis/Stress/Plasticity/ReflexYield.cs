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
            }
        }
        private double _yieldSecondaryStrength = -1;
        public double YieldSecondaryStrength
        {
            get
            {
                return this._yieldSecondaryStrength;
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

        public List<Stress.Macroskopic.PeakStressAssociation> PeakData = new List<Macroskopic.PeakStressAssociation>();

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

                                                    int PH = Convert.ToInt32(PlainString[0]);
                                                    int PK = Convert.ToInt32(PlainString[1]);
                                                    int PL = Convert.ToInt32(PlainString[2]);

                                                    if(this.SlipPlane.H == PH && this.SlipPlane.K == PK && this.SlipPlane.L == PL)
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

        public double LowerElasticBorder()
        {
            double ret = double.MaxValue;

            for(int n = 0; n < PeakData.Count; n++)
            {
                if(PeakData[n].ElasticRegime == true)
                {
                    if(ret > PeakData[n].MacroskopicStrain)
                    {
                        ret = PeakData[n].MacroskopicStrain;
                    }
                }
            }

            return ret;
        }
        public double UpperElasticBorder()
        {
            double ret = double.MinValue;

            for (int n = 0; n < PeakData.Count; n++)
            {
                if (PeakData[n].ElasticRegime == true)
                {
                    if (ret < PeakData[n].MacroskopicStrain)
                    {
                        ret = PeakData[n].MacroskopicStrain;
                    }
                }
            }

            return ret;
        }
        public double LowerPlasticBorder()
        {
            double ret = double.MaxValue;

            for (int n = 0; n < PeakData.Count; n++)
            {
                if (PeakData[n].ElasticRegime == false)
                {
                    if (ret > PeakData[n].MacroskopicStrain)
                    {
                        ret = PeakData[n].MacroskopicStrain;
                    }
                }
            }

            return ret;
        }
        public double UpperPlasticBorder()
        {
            double ret = double.MinValue;

            for (int n = 0; n < PeakData.Count; n++)
            {
                if (PeakData[n].ElasticRegime == false)
                {
                    if (ret < PeakData[n].MacroskopicStrain)
                    {
                        ret = PeakData[n].MacroskopicStrain;
                    }
                }
            }

            return ret;
        }

        public Pattern.Counts MicroStrainOverMacroStrainData(bool elastic)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            for(int n = 0; n < this.PeakData.Count; n++)
            {
                if(this.PeakData[n].ElasticRegime == elastic)
                {
                    double[] CTmp = { PeakData[n].MacroskopicStrain, PeakData[n].DifPeak.LatticeDistance, (PeakData[n].Stress * Math.Cos(PeakData[n].PsiAngle) * Math.Cos(PeakData[n].MainSlipDirectionAngle)) };

                    Ret.Add(CTmp);
                }
            }

            return Ret;
        }
        public Pattern.Counts StressOverMicroStrainData(bool elastic)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            for (int n = 0; n < this.PeakData.Count; n++)
            {
                if (this.PeakData[n].ElasticRegime == elastic)
                {
                    double[] CTmp = { PeakData[n].DifPeak.LatticeDistance, (PeakData[n].Stress * Math.Cos(PeakData[n].PsiAngle) * Math.Cos(PeakData[n].MainSlipDirectionAngle)), PeakData[n].MacroskopicStrain };

                    Ret.Add(CTmp);
                }
            }

            return Ret;
        }
        public Pattern.Counts MicroStrainDataOverStress(bool elastic)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            for (int n = 0; n < this.PeakData.Count; n++)
            {
                if (this.PeakData[n].ElasticRegime == elastic)
                {
                    double[] CTmp = { (PeakData[n].Stress * Math.Cos(PeakData[n].PsiAngle) * Math.Cos(PeakData[n].MainSlipDirectionAngle)), PeakData[n].DifPeak.LatticeDistance, PeakData[n].MacroskopicStrain };

                    Ret.Add(CTmp);
                }
            }

            return Ret;
        }

        public void FitElasticData()
        {
            Pattern.Counts ElasticData1 = this.MicroStrainOverMacroStrainData(true);
            Pattern.Counts ElasticData2 = this.MicroStrainDataOverStress(true);

            Fitting.LMA.FitMacroElasticModul(this.ElasticLinearFit1, ElasticData1);
            Fitting.LMA.FitMacroElasticModul(this.ElasticLinearFit2, ElasticData2);
        }

        public void FitPlasticData()
        {
            Pattern.Counts PlasticData1 = this.MicroStrainOverMacroStrainData(false);
            Pattern.Counts PlasticData2 = this.MicroStrainDataOverStress(false);

            Fitting.LMA.FitMacroElasticModul(this.PlasticLinearFit1, PlasticData1);
            Fitting.LMA.FitMacroElasticModul(this.PlasticLinearFit2, PlasticData2);
        }
    }
}
