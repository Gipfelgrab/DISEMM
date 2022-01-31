using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.DataManagment.Files.SCEC
{
    [Serializable]
    public class SimulatedExperimentInformation
    {
        public string savePath = "";
        private double _simulationIndex;
        private double _chiAngle;
        private double _omegaAngle;
        private double _sampleArea;
        public string _name;

        public List<YieldSurfaceInformation> YieldInformation = new List<YieldSurfaceInformation>();

        public List<MathNet.Numerics.LinearAlgebra.Matrix<double>> _hardenningTensor = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

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

        public List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>> StressRateCFHistory = new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>();
        public List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>> StrainRateCFHistory = new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>();

        public List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>> HardeningCFHistory = new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>();
        public List<List<double>> ShearRateCFHistory = new List<List<double>>();


        public List<List<Analysis.Stress.Plasticity.GrainOrientationParameter>> GrainOrientations = new List<List<Analysis.Stress.Plasticity.GrainOrientationParameter>>();

        public List<List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>> StressRateCFOrientedHistory = new List<List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>>();
        public List<List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>> StrainRateCFOrientedHistory = new List<List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>>();


        //public List<List<List<List<UInt16>>>> ActiveSystemsCFOrientedHistoryIndex = new List<List<List<List<UInt16>>>>();

        public List<List<List<List<ReflexYieldInformation>>>> ActiveSystemsCFOrientedHistory = new List<List<List<List<ReflexYieldInformation>>>>();
        public List<List<List<ReflexYieldInformation>>> ActiveSystemsCFHistory = new List<List<List<ReflexYieldInformation>>>();

        public SimulatedExperimentInformation(Analysis.Stress.Plasticity.ElastoPlasticExperiment Ep)
        {
            this._simulationIndex = Ep.SimulationIndex;
            this._chiAngle = Ep.ChiAngle;
            this._omegaAngle = Ep.OmegaAngle;
            this._sampleArea = Ep.SampleArea;
            this._name = Ep.Name;

            if (Ep.GrainOrientations != null)
            {
                this.GrainOrientations = Ep.GrainOrientations;
            }
            else
            {
                this.GrainOrientations = new List<List<Analysis.Stress.Plasticity.GrainOrientationParameter>>();
            }
            
            for (int n = 0; n < Ep.YieldInformation.Count; n++)
            {
                this.YieldInformation.Add(new YieldSurfaceInformation(Ep.YieldInformation[n]));
            }
            for (int n = 0; n < Ep._hardenningTensor.Count; n++)
            {
                this._hardenningTensor.Add(Ep._hardenningTensor[n].Clone());
            }

            for (int n = 0; n < Ep.StressSFHistory.Count; n++)
            {
                this.StressSFHistory.Add(Ep.StressSFHistory[n].Clone());
            }
            if (Ep.StressSFHistoryAreaCorrected == null)
            {
                this.StressSFHistoryAreaCorrected = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
            }
            else
            {
                for (int n = 0; n < Ep.StressSFHistoryAreaCorrected.Count; n++)
                {
                    this.StressSFHistoryAreaCorrected.Add(Ep.StressSFHistoryAreaCorrected[n].Clone());
                }
            }
            for (int n = 0; n < Ep.StrainSFHistory.Count; n++)
            {
                this.StrainSFHistory.Add(Ep.StrainSFHistory[n].Clone());
            }
            for (int n = 0; n < Ep.StressRateSFHistory.Count; n++)
            {
                this.StressRateSFHistory.Add(Ep.StressRateSFHistory[n].Clone());
            }
            for (int n = 0; n < Ep.StrainRateSFHistory.Count; n++)
            {
                this.StrainRateSFHistory.Add(Ep.StrainRateSFHistory[n].Clone());
            }

            for(int n = 0; n < Ep.StressCFHistory.Count; n++)
            {
                List<MathNet.Numerics.LinearAlgebra.Matrix<double>> crystalStress = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

                for(int i = 0; i < Ep.StressCFHistory[n].Count; i++)
                {
                    crystalStress.Add(Ep.StressCFHistory[n][i].Clone());
                }

                this.StressCFHistory.Add(crystalStress);
            }
            for (int n = 0; n < Ep.StrainCFHistory.Count; n++)
            {
                List<MathNet.Numerics.LinearAlgebra.Matrix<double>> crystalStrain = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

                for (int i = 0; i < Ep.StrainCFHistory[n].Count; i++)
                {
                    crystalStrain.Add(Ep.StrainCFHistory[n][i].Clone());
                }

                this.StrainCFHistory.Add(crystalStrain);
            }
            for (int n = 0; n < Ep.StressRateCFHistory.Count; n++)
            {
                List<MathNet.Numerics.LinearAlgebra.Matrix<double>> crystalStressRate = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

                for (int i = 0; i < Ep.StressRateCFHistory[n].Count; i++)
                {
                    crystalStressRate.Add(Ep.StressRateCFHistory[n][i].Clone());
                }

                this.StressRateCFHistory.Add(crystalStressRate);
            }
            for (int n = 0; n < Ep.StrainRateCFHistory.Count; n++)
            {
                List<MathNet.Numerics.LinearAlgebra.Matrix<double>> crystalStrainRate = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

                for (int i = 0; i < Ep.StrainRateCFHistory[n].Count; i++)
                {
                    crystalStrainRate.Add(Ep.StrainRateCFHistory[n][i].Clone());
                }

                this.StrainRateCFHistory.Add(crystalStrainRate);
            }

            //for (int n = 0; n < Ep.ActiveSystemsCFOrientedHistory.Count; n++)
            //{
            //    List<List<List<UInt16>>> activePhaseSystems = new List<List<List<UInt16>>>();

            //    for (int i = 0; i < Ep.ActiveSystemsCFOrientedHistory[n].Count; i++)
            //    {
            //        List<List<UInt16>> activeStepSystems = new List<List<UInt16>>();

            //        for (int j = 0; j < Ep.ActiveSystemsCFOrientedHistory[n][i].Count; j++)
            //        {
            //            List<UInt16> activeSystems = new List<UInt16>();

            //            for (int k = 0; k < Ep.ActiveSystemsCFOrientedHistory[n][i][j].Count; k++)
            //            {
            //                for (UInt16 l = 0; l < Ep.YieldInformation[n].PotentialSlipSystems.Count; l++)
            //                {
            //                    if (Ep.ActiveSystemsCFOrientedHistory[n][i][j][k].HKLString == Ep.YieldInformation[n].PotentialSlipSystems[l].HKLString)
            //                    {
            //                        activeSystems.Add(l);
            //                    }
            //                }
            //            }

            //            activeStepSystems.Add(activeSystems);
            //        }

            //        activePhaseSystems.Add(activeStepSystems);
            //    }

            //    this.ActiveSystemsCFOrientedHistoryIndex.Add(activePhaseSystems);
            //}


            for (int n = 0; n < Ep.StrainRateCFOrientedHistory.Count; n++)
            {
                List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>> crystalStrainRatePhase = new List<List< MathNet.Numerics.LinearAlgebra.Matrix<double>>>();

                for (int i = 0; i < Ep.StrainRateCFOrientedHistory[n].Count; i++)
                {
                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> crystalStrainRate1 = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

                    for (int j = 0; j < Ep.StrainRateCFOrientedHistory[n][i].Count; j++)
                    {
                        crystalStrainRate1.Add(Ep.StrainRateCFOrientedHistory[n][i][j]);
                    }

                    crystalStrainRatePhase.Add(crystalStrainRate1);
                }

                this.StrainRateCFOrientedHistory.Add(crystalStrainRatePhase);
            }
            for (int n = 0; n < Ep.StressRateCFOrientedHistory.Count; n++)
            {
                List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>> crystalStressRatePhase = new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>();

                for (int i = 0; i < Ep.StressRateCFOrientedHistory[n].Count; i++)
                {
                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> crystalStressRate1 = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

                    for (int j = 0; j < Ep.StressRateCFOrientedHistory[n][i].Count; j++)
                    {
                        crystalStressRate1.Add(Ep.StressRateCFOrientedHistory[n][i][j]);
                    }

                    crystalStressRatePhase.Add(crystalStressRate1);
                }

                this.StressRateCFOrientedHistory.Add(crystalStressRatePhase);
            }

            for (int n = 0; n < Ep.ActiveSystemsCFHistory.Count; n++)
            {
                List<List<ReflexYieldInformation>> activePhaseSystems = new List<List<ReflexYieldInformation>>();

                for(int i = 0; i < Ep.ActiveSystemsCFHistory[n].Count; i++)
                {
                    List<ReflexYieldInformation> activeSystems = new List<ReflexYieldInformation>();

                    for(int j = 0; j < Ep.ActiveSystemsCFHistory[n][i].Count; j++)
                    {
                        activeSystems.Add(new ReflexYieldInformation(Ep.ActiveSystemsCFHistory[n][i][j]));
                    }

                    activePhaseSystems.Add(activeSystems);
                }

                this.ActiveSystemsCFHistory.Add(activePhaseSystems);
            }
        }

        public Analysis.Stress.Plasticity.ElastoPlasticExperiment GetElastoPlasticExperiment()
        {
            Analysis.Stress.Plasticity.ElastoPlasticExperiment ret = new Analysis.Stress.Plasticity.ElastoPlasticExperiment();

            ret.SimulationIndex = Convert.ToInt32(this._simulationIndex);
            ret.ChiAngle = this._chiAngle;
            ret.OmegaAngle = this._omegaAngle;
            ret.SampleArea = this._sampleArea;
            ret.Name = this._name;

            if (this.GrainOrientations != null)
            {
                ret.GrainOrientations = this.GrainOrientations;
            }
            else
            {
                ret.GrainOrientations = new List<List<Analysis.Stress.Plasticity.GrainOrientationParameter>>();
            }

            if (this.YieldInformation != null && this.YieldInformation.Count != 0)
            {
                for (int n = 0; n < this.YieldInformation.Count; n++)
                {
                    ret.YieldInformation.Add(this.YieldInformation[n].GetYieldSurface());
                }
            }
            if (this._hardenningTensor != null && this._hardenningTensor.Count != 0)
            {
                for (int n = 0; n < this._hardenningTensor.Count; n++)
                {
                    ret._hardenningTensor.Add(this._hardenningTensor[n].Clone());
                }
            }

            for (int n = 0; n < this.StressSFHistory.Count; n++)
            {
                ret.StressSFHistory.Add(this.StressSFHistory[n].Clone());
            }
            if (this.StressSFHistoryAreaCorrected != null)
            {
                for (int n = 0; n < this.StressSFHistoryAreaCorrected.Count; n++)
                {
                    ret.StressSFHistoryAreaCorrected.Add(this.StressSFHistoryAreaCorrected[n].Clone());
                }
            }
            else
            {
                ret.StressSFHistoryAreaCorrected = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
            }
            for (int n = 0; n < this.StrainSFHistory.Count; n++)
            {
                ret.StrainSFHistory.Add(this.StrainSFHistory[n].Clone());
            }
            for (int n = 0; n < this.StressRateSFHistory.Count; n++)
            {
                ret.StressRateSFHistory.Add(this.StressRateSFHistory[n].Clone());
            }
            for (int n = 0; n < this.StrainRateSFHistory.Count; n++)
            {
                ret.StrainRateSFHistory.Add(this.StrainRateSFHistory[n].Clone());
            }

            for (int n = 0; n < this.StressCFHistory.Count; n++)
            {
                List<MathNet.Numerics.LinearAlgebra.Matrix<double>> crystalStress = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

                for (int i = 0; i < this.StressCFHistory[n].Count; i++)
                {
                    crystalStress.Add(this.StressCFHistory[n][i].Clone());
                }

                ret.StressCFHistory.Add(crystalStress);
            }
            for (int n = 0; n < this.StrainCFHistory.Count; n++)
            {
                List<MathNet.Numerics.LinearAlgebra.Matrix<double>> crystalStrain = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

                for (int i = 0; i < this.StrainCFHistory[n].Count; i++)
                {
                    crystalStrain.Add(this.StrainCFHistory[n][i].Clone());
                }

                ret.StrainCFHistory.Add(crystalStrain);
            }
            for (int n = 0; n < this.StressRateCFHistory.Count; n++)
            {
                List<MathNet.Numerics.LinearAlgebra.Matrix<double>> crystalStressRate = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

                for (int i = 0; i < this.StressRateCFHistory[n].Count; i++)
                {
                    crystalStressRate.Add(this.StressRateCFHistory[n][i].Clone());
                }

                ret.StressRateCFHistory.Add(crystalStressRate);
            }
            for (int n = 0; n < this.StrainRateCFHistory.Count; n++)
            {
                List<MathNet.Numerics.LinearAlgebra.Matrix<double>> crystalStrainRate = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

                for (int i = 0; i < this.StrainRateCFHistory[n].Count; i++)
                {
                    crystalStrainRate.Add(this.StrainRateCFHistory[n][i].Clone());
                }

                ret.StrainRateCFHistory.Add(crystalStrainRate);
            }
            if (this.ActiveSystemsCFOrientedHistory != null)
            {
                for (int n = 0; n < this.ActiveSystemsCFOrientedHistory.Count; n++)
                {
                    List<List<List<Analysis.Stress.Plasticity.ReflexYield>>> activePhaseSystems = new List<List<List<Analysis.Stress.Plasticity.ReflexYield>>>();

                    for (int i = 0; i < this.ActiveSystemsCFOrientedHistory[n].Count; i++)
                    {
                        List<List<Analysis.Stress.Plasticity.ReflexYield>> activeStepSystems = new List<List<Analysis.Stress.Plasticity.ReflexYield>>();

                        for (int j = 0; j < this.ActiveSystemsCFOrientedHistory[n][i].Count; j++)
                        {
                            List<Analysis.Stress.Plasticity.ReflexYield> activeSystems = new List<Analysis.Stress.Plasticity.ReflexYield>();

                            for (int k = 0; k < this.ActiveSystemsCFOrientedHistory[n][i][j].Count; k++)
                            {
                                activeSystems.Add(this.ActiveSystemsCFOrientedHistory[n][i][j][k].GetReflexYield());
                            }

                            activeStepSystems.Add(activeSystems);
                        }

                        activePhaseSystems.Add(activeStepSystems);
                    }

                    ret.ActiveSystemsCFOrientedHistory.Add(activePhaseSystems);
                }
            }
            else
            {
                for (int n = 0; n < this.YieldInformation.Count; n++)
                {
                    ret.ActiveSystemsCFOrientedHistory.Add(new List<List<List<Analysis.Stress.Plasticity.ReflexYield>>>());
                }
            }
            if (this.StrainRateCFOrientedHistory != null)
            {
                for (int n = 0; n < this.StrainRateCFOrientedHistory.Count; n++)
                {
                    List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>> crystalStrainRatePhase = new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>();

                    for (int i = 0; i < this.StrainRateCFOrientedHistory[n].Count; i++)
                    {
                        List<MathNet.Numerics.LinearAlgebra.Matrix<double>> crystalStrainRate1 = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

                        for (int j = 0; j < this.StrainRateCFOrientedHistory[n][i].Count; j++)
                        {
                            crystalStrainRate1.Add(this.StrainRateCFOrientedHistory[n][i][j]);
                        }

                        crystalStrainRatePhase.Add(crystalStrainRate1);
                    }

                    ret.StrainRateCFOrientedHistory.Add(crystalStrainRatePhase);
                }
            }
            else
            {
                for (int n = 0; n < this.YieldInformation.Count; n++)
                {
                    ret.StrainRateCFOrientedHistory.Add(new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>());
                }
            }

            if (this.StressRateCFOrientedHistory != null)
            {
                for (int n = 0; n < this.StressRateCFOrientedHistory.Count; n++)
                {
                    List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>> crystalStressRatePhase = new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>();

                    for (int i = 0; i < this.StressRateCFOrientedHistory[n].Count; i++)
                    {
                        List<MathNet.Numerics.LinearAlgebra.Matrix<double>> crystalStressRate1 = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

                        for (int j = 0; j < this.StressRateCFOrientedHistory[n][i].Count; j++)
                        {
                            crystalStressRate1.Add(this.StressRateCFOrientedHistory[n][i][j]);
                        }

                        crystalStressRatePhase.Add(crystalStressRate1);
                    }

                    ret.StressRateCFOrientedHistory.Add(crystalStressRatePhase);
                }
            }
            else
            {
                for (int n = 0; n < this.YieldInformation.Count; n++)
                {
                    ret.StressRateCFOrientedHistory.Add(new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>());
                }
            }

            return ret;
        }
    }

    [Serializable]
    public class ActiveSystemInformation
    {
        public List<List<List<List<UInt16>>>> ActiveSystemsCFOrientedHistoryIndex = new List<List<List<List<UInt16>>>>();
        public string savePath = "";

        public ActiveSystemInformation(Analysis.Stress.Plasticity.ElastoPlasticExperiment Ep)
        {
            for (int n = 0; n < Ep.ActiveSystemsCFOrientedHistory.Count; n++)
            {
                List<List<List<UInt16>>> activePhaseSystems = new List<List<List<UInt16>>>();

                for (int i = 0; i < Ep.ActiveSystemsCFOrientedHistory[n].Count; i++)
                {
                    List<List<UInt16>> activeStepSystems = new List<List<UInt16>>();

                    for (int j = 0; j < Ep.ActiveSystemsCFOrientedHistory[n][i].Count; j++)
                    {
                        List<UInt16> activeSystems = new List<UInt16>();

                        for (int k = 0; k < Ep.ActiveSystemsCFOrientedHistory[n][i][j].Count; k++)
                        {
                            for (UInt16 l = 0; l < Ep.YieldInformation[n].PotentialSlipSystems.Count; l++)
                            {
                                if (Ep.ActiveSystemsCFOrientedHistory[n][i][j][k].HKLString == Ep.YieldInformation[n].PotentialSlipSystems[l].HKLString && Ep.ActiveSystemsCFOrientedHistory[n][i][j][k].SecondarySlipDirection.HKLString == Ep.YieldInformation[n].PotentialSlipSystems[l].SecondarySlipDirection.HKLString)
                                {
                                    activeSystems.Add(l);
                                    break;
                                }
                            }
                        }

                        activeStepSystems.Add(activeSystems);
                    }

                    activePhaseSystems.Add(activeStepSystems);
                }

                this.ActiveSystemsCFOrientedHistoryIndex.Add(activePhaseSystems);
            }
        }

        public List<List<List<List<Analysis.Stress.Plasticity.ReflexYield>>>> GetActiveSystems(List<Analysis.Stress.Plasticity.YieldSurface> yieldInformation)
        {
            List<List<List<List<Analysis.Stress.Plasticity.ReflexYield>>>> ret = new List<List<List<List<Analysis.Stress.Plasticity.ReflexYield>>>>();

            for (int n = 0; n < this.ActiveSystemsCFOrientedHistoryIndex.Count; n++)
            {
                List<List<List<Analysis.Stress.Plasticity.ReflexYield>>> activePhaseSystems = new List<List<List<Analysis.Stress.Plasticity.ReflexYield>>>();

                for (int i = 0; i < this.ActiveSystemsCFOrientedHistoryIndex[n].Count; i++)
                {
                    List<List<Analysis.Stress.Plasticity.ReflexYield>> activeStepSystems = new List<List<Analysis.Stress.Plasticity.ReflexYield>>();

                    for (int j = 0; j < this.ActiveSystemsCFOrientedHistoryIndex[n][i].Count; j++)
                    {
                        List<Analysis.Stress.Plasticity.ReflexYield> activeSystems = new List<Analysis.Stress.Plasticity.ReflexYield>();

                        for (int k = 0; k < this.ActiveSystemsCFOrientedHistoryIndex[n][i][j].Count; k++)
                        {
                            activeSystems.Add(yieldInformation[n].PotentialSlipSystems[this.ActiveSystemsCFOrientedHistoryIndex[n][i][j][k]]);
                        }

                        activeStepSystems.Add(activeSystems);
                    }

                    activePhaseSystems.Add(activeStepSystems);
                }

                ret.Add(activePhaseSystems);
            }

            return ret;
        }
    }
}
