using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.Analysis.Stress.Plasticity
{
    [Serializable]
    public class ElastoPlasticExperiment
    {
        public bool cancel = false;
        private int _simulationIndex;
        public int SimulationIndex
        {
            get
            {
                return this._simulationIndex;
            }
            set
            {
                this._simulationIndex = value;
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
        private double _sampleArea;
        public double SampleArea
        {
            get
            {
                return this._sampleArea;
            }
            set
            {
                this._sampleArea = value;
            }
        }

        public bool useHardeningMatrix = false;
        public bool singleCrystalTracking = true;
        public bool useAreaCorrection = false;
        public bool useMultiThreading = false;
        public bool useYieldLimit = true;
        public int slipCritereon = 0;
        public int ElasticModel = 2;
        public int LastActiveSystems = 0;
        public int LastFailedGrains = 0;

        public List<YieldSurface> YieldInformation = new List<YieldSurface>();

        public List<MathNet.Numerics.LinearAlgebra.Matrix<double>> _hardenningTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

        public ElastoPlasticExperiment(Sample actSample)
        {
            this._sampleArea = actSample.Area;
            //this.GrainOrientations = new List<List<GrainOrientationParameter>>();

            for(int n = 0; n < actSample.CrystalData.Count; n++)
            {
                this.GrainOrientations.Add(new List<GrainOrientationParameter>());
                List<MathNet.Numerics.LinearAlgebra.Matrix<double>> stressHis = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
                List<MathNet.Numerics.LinearAlgebra.Matrix<double>> strainHis = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
                List<MathNet.Numerics.LinearAlgebra.Matrix<double>> stressRateHis = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
                List<MathNet.Numerics.LinearAlgebra.Matrix<double>> strainRateHis = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
                List<MathNet.Numerics.LinearAlgebra.Matrix<double>> hardHis = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
                List<double> shearHist = new List<double>();
                List<List<ReflexYield>> aSystemsHist = new List<List<ReflexYield>>();

                List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>> stressHisOr = new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>();
                List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>> strainHisOr = new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>();
                List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>> stressRateHisOr = new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>();
                List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>> strainRateHisOr = new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>();

                List<List<List<ReflexYield>>> ActiveSystemsCFHisOr = new List<List<List<ReflexYield>>>();

                this.StressCFHistory.Add(stressHis);
                this.StrainCFHistory.Add(strainHis);
                this.StressRateCFHistory.Add(stressRateHis);
                this.StrainRateCFHistory.Add(strainRateHis);
                //this.StressCFOrientedHistory.Add(stressHisOr);
                //this.StrainCFOrientedHistory.Add(strainHisOr);
                this.StressRateCFOrientedHistory.Add(stressRateHisOr);
                this.StrainRateCFOrientedHistory.Add(strainRateHisOr);
                this.HardeningCFHistory.Add(hardHis);
                this.ShearRateCFHistory.Add(shearHist);
                this.ActiveSystemsCFHistory.Add(aSystemsHist);
                this.ActiveSystemsCFOrientedHistory.Add(ActiveSystemsCFHisOr);

                this.YieldInformation.Add(new YieldSurface(actSample.CrystalData[n]));

                MathNet.Numerics.LinearAlgebra.Matrix<double> tmpHardenning = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense<double>(3, 3, 0.0);
                tmpHardenning[0, 0] = 1;
                tmpHardenning[1, 1] = 1;
                tmpHardenning[2, 2] = 1;
                this._hardenningTensor.Add(tmpHardenning);
            }
        }

        public ElastoPlasticExperiment()
        {
            
        }

        public List<MathNet.Numerics.LinearAlgebra.Matrix<double>> StressSFHistoryAreaCorrected = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

        public List<MathNet.Numerics.LinearAlgebra.Matrix<double>> StressSFHistory = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        public List<MathNet.Numerics.LinearAlgebra.Matrix<double>> StrainSFHistory = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

        public List<MathNet.Numerics.LinearAlgebra.Matrix<double>> StressRateSFHistory = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        public List<MathNet.Numerics.LinearAlgebra.Matrix<double>> StrainRateSFHistory = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

        public List<MathNet.Numerics.LinearAlgebra.Matrix<double>> HardeningSFHistory = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

        public List<double> ShearRateSFHistory = new List<double>();
        public List<double> ActiveSystemsSFHistory = new List<double>();

        public List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>> StressCFHistory = new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>();
        public List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>> StrainCFHistory = new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>();

        //[phase][Step][grainIndex]
        public List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>> StressRateCFHistory = new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>();
        public List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>> StrainRateCFHistory = new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>();

        //Achtung Speichereihenfolge ist bsonders [step][phase][grain][system]
        public List<List<List<List<double>>>> YieldChangeCFHistory = new List<List<List<List<double>>>>();

        public List<MathNet.Numerics.LinearAlgebra.Matrix<double>> GetSampleStrainWeighted()
        {
            List<MathNet.Numerics.LinearAlgebra.Matrix<double>> ret = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
            for (int phase = 0; phase < YieldInformation.Count; phase++)
            {
                List<MathNet.Numerics.LinearAlgebra.Matrix<double>> phaseTmp = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
                for (int n = 0; n < this.StrainRateCFOrientedHistory[phase].Count; n++)
                {
                    MathNet.Numerics.LinearAlgebra.Matrix<double> tmp = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense<double>(3, 3, 0);
                    //MathNet.Numerics.LinearAlgebra.Matrix<double> tmp = elasticCompliances * StressRateCFOrientedHistory[phase][n][orientationIndex];
                    for(int i = 0; i < this.StrainRateCFOrientedHistory[phase][n].Count; i++)
                    {
                        tmp += this.StrainRateCFOrientedHistory[phase][n][i];
                    }

                    tmp /= this.StrainRateCFOrientedHistory[phase][n].Count;

                    phaseTmp.Add(tmp);
                }
                if (phase == 0)
                {
                    for (int n = 0; n < phaseTmp.Count; n++)
                    {
                        ret.Add(YieldInformation[phase].CrystalData.PhaseFraction * phaseTmp[n]);
                    }
                }
                else
                {
                    for (int n = 0; n < phaseTmp.Count; n++)
                    {
                        ret[n] += (YieldInformation[phase].CrystalData.PhaseFraction * phaseTmp[n]);
                    }
                }
            }

            return ret;
        }

        public MathNet.Numerics.LinearAlgebra.Matrix<double> GetLatticeStrainRate(int phase, int index, int orientationIndex, Tools.FourthRankTensor elasticCompliances)
        {
            MathNet.Numerics.LinearAlgebra.Matrix<double> ret = elasticCompliances * StressRateCFOrientedHistory[phase][index][orientationIndex];

            return ret;
        }
        public List<MathNet.Numerics.LinearAlgebra.Matrix<double>> GetLatticeStrainRate(int phase, int orientationIndex, Tools.FourthRankTensor elasticCompliances)
        {
            List<MathNet.Numerics.LinearAlgebra.Matrix<double>> ret = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
            for (int n = 0; n < StressRateCFOrientedHistory[phase].Count; n++)
            {
                MathNet.Numerics.LinearAlgebra.Matrix<double> tmp = elasticCompliances * StressRateCFOrientedHistory[phase][n][orientationIndex];
                ret.Add(tmp);
            }

            return ret;
        }

        public MathNet.Numerics.LinearAlgebra.Matrix<double> GetLatticeStrain(int phase, int index, int orientationIndex, Tools.FourthRankTensor elasticCompliances)
        {
            MathNet.Numerics.LinearAlgebra.Matrix<double> grainStress = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense<double>(3, 3, 0);

            for (int n = 0; n < index + 1; n++)
            {
                grainStress += StressRateCFOrientedHistory[phase][n][orientationIndex];
            }

            MathNet.Numerics.LinearAlgebra.Matrix<double> ret = elasticCompliances * grainStress;

            return ret;
        }
        public List<MathNet.Numerics.LinearAlgebra.Matrix<double>> GetLatticeStrain(int phase, int orientationIndex, Tools.FourthRankTensor elasticCompliances)
        {
            List<MathNet.Numerics.LinearAlgebra.Matrix<double>> ret = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
            for (int n = 0; n < StressRateCFOrientedHistory[phase].Count; n++)
            {
                MathNet.Numerics.LinearAlgebra.Matrix<double> grainStress = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense<double>(3, 3, 0);

                for (int i = 0; i < n; i++)
                {
                    grainStress += StressRateCFOrientedHistory[phase][i][orientationIndex];
                }

                ret.Add(elasticCompliances * grainStress);
            }

            return ret;
        }

        public List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>> HardeningCFHistory = new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>();
        public List<List<double>> ShearRateCFHistory = new List<List<double>>();

        //public List<List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>> StressCFOrientedHistory = new List<List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>>();
        //public List<List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>> StrainCFOrientedHistory = new List<List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>>();

        public List<List<GrainOrientationParameter>> GrainOrientations = new List<List<GrainOrientationParameter>>();

        public List<List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>> StressRateCFOrientedHistory = new List<List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>>();
        public List<List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>> StrainRateCFOrientedHistory = new List<List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>>();

        public List<List<List<double>>> HardeningCFOrientedHistory = new List<List<List<double>>>();

        public List<List<List<double>>> ShearRateCFOrientedHistory = new List<List<List<double>>>();

        /// <summary>
        /// Phase -> ExperimentalStep -> Orientation -> active Slipsystems
        /// </summary>
        public List<List<List<List<ReflexYield>>>> ActiveSystemsCFOrientedHistory = new List<List<List<List<ReflexYield>>>>();

        #region Old Stuff needs to be deleted but Seriealized
        
        public List<List<List<ReflexYield>>> ActiveSystemsCFHistory = new List<List<List<ReflexYield>>>();

        #endregion


        #region Simulation using Multi Threading

        public System.Threading.ManualResetEvent _doneEvent;

        public event System.ComponentModel.PropertyChangedEventHandler FitFinished;
        public event System.ComponentModel.PropertyChangedEventHandler FitStarted;

        protected void OnFitStarted()
        {
            System.ComponentModel.PropertyChangedEventHandler handler = FitStarted;
            if (handler != null)
            {
                handler(this, new System.ComponentModel.PropertyChangedEventArgs("FitStarted"));
            }
        }

        protected void OnFitFinished()
        {
            this._doneEvent.Set();

            System.ComponentModel.PropertyChangedEventHandler handler = FitFinished;
            if (handler != null)
            {
                handler(this, new System.ComponentModel.PropertyChangedEventArgs("FitFinished"));
            }
        }

        // Wrapper method for use with thread pool. 
        public void FitRegionCallback(Object threadContext)
        {
            OnFitStarted();

            

            OnFitFinished();
        }

        public void SetResetEvent(System.Threading.ManualResetEvent DoneEvent)
        {
            this._doneEvent = DoneEvent;
        }
        

        #endregion
    }

    [Serializable]
    public class GrainOrientationParameter
    {
        private double _phi1;
        public double Phi1
        {
            get
            {
                return this._phi1;
            }
            set
            {
                this._phi1 = value;
            }
        }
        private double _psi;
        public double Psi
        {
            get
            {
                return this._psi;
            }
            set
            {
                this._psi = value;
            }
        }
        private double _phi2;
        public double Phi2
        {
            get
            {
                return this._phi2;
            }
            set
            {
                this._phi2 = value;
            }
        }

        public GrainOrientationParameter(double phi1, double psi, double phi2)
        {
            this._phi1 = phi1;
            this._psi = psi;
            this._phi2 = phi2;
        }
    }
}
