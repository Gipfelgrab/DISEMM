using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalScec.DataManagment.Files.SCEC;

namespace CalScec.DataManagment.Files.Simulation
{
    [Serializable]
    public class SimHeader
    {
        public string SavePath = "";
        public string SaveName = "";
        public int StepWidth = 20;
        
        private double _chiAngle;
        private double _omegaAngle;
        private double _sampleArea;

        public List<YieldSurfaceInformation> YieldInformation = new List<YieldSurfaceInformation>();
        public List<List<Analysis.Stress.Plasticity.GrainOrientationParameter>> GrainOrientations = new List<List<Analysis.Stress.Plasticity.GrainOrientationParameter>>();

        public int totalBodies;

        public List<string> BodyPaths = new List<string>();

        public List<SimBody> SimulationData = new List<SimBody>();


        public SimHeader(Analysis.Stress.Plasticity.ElastoPlasticExperiment Ep, string path, string name)
        {
            //Setting header information
            this.SavePath = path;
            this.SaveName = name;
            this._chiAngle = Ep.ChiAngle;
            this._omegaAngle = Ep.OmegaAngle;
            this._sampleArea = Ep.SampleArea;

            for (int n = 0; n < Ep.YieldInformation.Count; n++)
            {
                this.YieldInformation.Add(new YieldSurfaceInformation(Ep.YieldInformation[n]));
            }

            if (Ep.GrainOrientations != null)
            {
                this.GrainOrientations = Ep.GrainOrientations;
            }
            else
            {
                this.GrainOrientations = new List<List<Analysis.Stress.Plasticity.GrainOrientationParameter>>();
            }

            //Seperating the cases for the body
            if(Ep.StressRateSFHistory.Count == Ep.StrainRateSFHistory.Count)
            {
                if(Ep.StressSFHistory.Count == 0)
                {
                    this.totalBodies = 0;
                }
                else
                {
                    this.SetSymmetricBodies(Ep);
                }
            }
            else
            {
                this.SetAssymmetricBodies(Ep);
            }
        }

        public SimHeader(Analysis.Stress.Plasticity.ElastoPlasticExperiment Ep, string path, string name, int stepWidth)
        {
            //Setting header information
            this.SavePath = path;
            this.SaveName = name;
            this._chiAngle = Ep.ChiAngle;
            this._omegaAngle = Ep.OmegaAngle;
            this._sampleArea = Ep.SampleArea;
            this.StepWidth = stepWidth;

            for (int n = 0; n < Ep.YieldInformation.Count; n++)
            {
                this.YieldInformation.Add(new YieldSurfaceInformation(Ep.YieldInformation[n]));
            }

            if (Ep.GrainOrientations != null)
            {
                this.GrainOrientations = Ep.GrainOrientations;
            }
            else
            {
                this.GrainOrientations = new List<List<Analysis.Stress.Plasticity.GrainOrientationParameter>>();
            }

            //Seperating the cases for the body
            if (Ep.StressRateSFHistory.Count == Ep.StrainRateSFHistory.Count)
            {
                if (Ep.StressSFHistory.Count == 0)
                {
                    this.totalBodies = 0;
                }
                else
                {
                    this.SetSymmetricBodies(Ep);
                }
            }
            else
            {
                this.SetAssymmetricBodies(Ep);
            }
        }

        public SimHeader()
        {

        }

        private void SetSymmetricBodies(Analysis.Stress.Plasticity.ElastoPlasticExperiment Ep)
        {
            int index = 0;
            totalBodies = 0;

            while (index < Ep.StressRateSFHistory.Count)
            {
                SimBody bodyTmp = new SimBody(totalBodies, true);
                bodyTmp.SavePath = SaveName + "-" + totalBodies.ToString() + ".simb";
                totalBodies += 1;
                
                for (int phase = 0; phase < this.YieldInformation.Count; phase++)
                {
                    bodyTmp.StrainRateCFOrientedHistory.Add(new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>());
                    bodyTmp.StressRateCFOrientedHistory.Add(new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>());

                    bodyTmp.ActiveSystemsCFOrientedHistory.Add(new List<List<List<ReflexYieldInformation>>>());
                }

                for (int placeholder = 0; index < Ep.StressRateSFHistory.Count; index++)
                {
                    bodyTmp.StressRateSFHistory.Add(Ep.StressRateSFHistory[index].Clone());
                    bodyTmp.StrainRateSFHistory.Add(Ep.StrainRateSFHistory[index].Clone());

                    bodyTmp.YieldChangeCFHistory.Add(Ep.YieldChangeCFHistory[index]);

                    for (int phase = 0; phase < this.YieldInformation.Count; phase++)
                    {
                        List<MathNet.Numerics.LinearAlgebra.Matrix<double>> crystalStrainRate1 = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

                        for (int j = 0; j < Ep.StrainRateCFOrientedHistory[phase][index].Count; j++)
                        {
                            crystalStrainRate1.Add(Ep.StrainRateCFOrientedHistory[phase][index][j].Clone());
                        }

                        bodyTmp.StrainRateCFOrientedHistory[phase].Add(crystalStrainRate1);

                        List<MathNet.Numerics.LinearAlgebra.Matrix<double>> crystalStressRate1 = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

                        for (int j = 0; j < Ep.StressRateCFOrientedHistory[phase][index].Count; j++)
                        {
                            crystalStressRate1.Add(Ep.StressRateCFOrientedHistory[phase][index][j].Clone());
                        }

                        bodyTmp.StressRateCFOrientedHistory[phase].Add(crystalStressRate1);

                        List<List<ReflexYieldInformation>> activeSystems = new List<List<ReflexYieldInformation>>();

                        for(int n = 0; n < Ep.ActiveSystemsCFOrientedHistory[phase][index].Count; n++)
                        {
                            if(Ep.ActiveSystemsCFOrientedHistory[phase][index][n].Count == 0)
                            {
                                activeSystems.Add(new List<ReflexYieldInformation>());
                            }
                            else
                            {
                                List<ReflexYieldInformation> activeSystemsGrain = new List<ReflexYieldInformation>();

                                for(int i = 0; i < Ep.ActiveSystemsCFOrientedHistory[phase][index][n].Count; i++)
                                {
                                    activeSystemsGrain.Add(new ReflexYieldInformation(Ep.ActiveSystemsCFOrientedHistory[phase][index][n][i]));
                                }

                                activeSystems.Add(activeSystemsGrain);
                            }
                        }

                        bodyTmp.ActiveSystemsCFOrientedHistory[phase].Add(activeSystems);
                    }

                    if (index == StepWidth * totalBodies)
                    {
                        this.SimulationData.Add(bodyTmp);
                        this.BodyPaths.Add(bodyTmp.SavePath);
                        index++;
                        break;
                    }
                    else if (index == Ep.StressRateSFHistory.Count - 1)
                    {
                        this.SimulationData.Add(bodyTmp);
                        this.BodyPaths.Add(bodyTmp.SavePath);
                        index++;
                        break;
                    }
                }
            }
        }

        private void SetAssymmetricBodies(Analysis.Stress.Plasticity.ElastoPlasticExperiment Ep)
        {
            int index = 0;
            totalBodies = 0;

            if(Ep.StressRateSFHistory.Count > Ep.StrainRateSFHistory.Count)
            {
                while (index < Ep.StrainRateSFHistory.Count)
                {
                    SimBody bodyTmp = new SimBody(totalBodies, true);
                    bodyTmp.SavePath = SaveName + "-" + totalBodies.ToString() + ".simb";

                    totalBodies += 1;

                    for (int phase = 0; phase < this.YieldInformation.Count; phase++)
                    {
                        bodyTmp.StrainRateCFOrientedHistory.Add(new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>());
                        bodyTmp.StressRateCFOrientedHistory.Add(new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>());
                    }

                    for (int placeholder = 0; index < Ep.StrainRateSFHistory.Count; index++)
                    {
                        bodyTmp.StressRateSFHistory.Add(Ep.StressRateSFHistory[index].Clone());
                        bodyTmp.StrainRateSFHistory.Add(Ep.StrainRateSFHistory[index].Clone());

                        for (int phase = 0; phase < this.YieldInformation.Count; phase++)
                        {
                            List<MathNet.Numerics.LinearAlgebra.Matrix<double>> crystalStrainRate1 = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

                            for (int j = 0; j < Ep.StrainRateCFOrientedHistory[phase][index].Count; j++)
                            {
                                crystalStrainRate1.Add(Ep.StrainRateCFOrientedHistory[phase][index][j].Clone());
                            }

                            bodyTmp.StrainRateCFOrientedHistory[phase].Add(crystalStrainRate1);

                            List<MathNet.Numerics.LinearAlgebra.Matrix<double>> crystalStressRate1 = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

                            for (int j = 0; j < Ep.StressRateCFOrientedHistory[phase][index].Count; j++)
                            {
                                crystalStressRate1.Add(Ep.StressRateCFOrientedHistory[phase][index][j].Clone());
                            }

                            bodyTmp.StressRateCFOrientedHistory[phase].Add(crystalStressRate1);

                            List<List<ReflexYieldInformation>> activeSystems = new List<List<ReflexYieldInformation>>();

                            for (int n = 0; n < Ep.ActiveSystemsCFOrientedHistory[phase][index].Count; n++)
                            {
                                if (Ep.ActiveSystemsCFOrientedHistory[phase][index][n].Count == 0)
                                {
                                    activeSystems.Add(new List<ReflexYieldInformation>());
                                }
                                else
                                {
                                    List<ReflexYieldInformation> activeSystemsGrain = new List<ReflexYieldInformation>();

                                    for (int i = 0; i < Ep.ActiveSystemsCFOrientedHistory[phase][index][n].Count; i++)
                                    {
                                        activeSystemsGrain.Add(new ReflexYieldInformation(Ep.ActiveSystemsCFOrientedHistory[phase][index][n][i]));
                                    }

                                    activeSystems.Add(activeSystemsGrain);
                                }
                            }

                            bodyTmp.ActiveSystemsCFOrientedHistory[phase].Add(activeSystems);
                        }

                        bodyTmp.YieldChangeCFHistory.Add(Ep.YieldChangeCFHistory[index]);

                        if (index == StepWidth * totalBodies)
                        {
                            this.SimulationData.Add(bodyTmp);
                            this.BodyPaths.Add(bodyTmp.SavePath);
                            index++;
                            break;
                        }
                        else if(index == Ep.StrainRateSFHistory.Count - 1)
                        {
                            this.SimulationData.Add(bodyTmp);
                            this.BodyPaths.Add(bodyTmp.SavePath);
                            index++;
                            break;
                        }
                    }
                }

                SimBody bodyTmp1 = new SimBody(totalBodies, false);
                bodyTmp1.SavePath = SaveName + "-" + totalBodies.ToString() + ".simb";

                totalBodies += 1;

                for (int phase = 0; phase < this.YieldInformation.Count; phase++)
                {
                    bodyTmp1.StrainRateCFOrientedHistory.Add(new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>());
                    bodyTmp1.StressRateCFOrientedHistory.Add(new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>());
                }

                for (int placeholder = 0; index < Ep.StressRateSFHistory.Count; index++)
                {
                    bodyTmp1.StressRateSFHistory.Add(Ep.StressRateSFHistory[index].Clone());
                }

                this.SimulationData.Add(bodyTmp1);
                this.BodyPaths.Add(bodyTmp1.SavePath);
            }
            else
            {
                this.SetSymmetricBodies(Ep);

                SimBody bodyTmp1 = new SimBody(totalBodies, false);
                bodyTmp1.SavePath = SaveName + "-" + totalBodies.ToString() + ".simb";

                totalBodies += 1;

                for (int phase = 0; phase < this.YieldInformation.Count; phase++)
                {
                    bodyTmp1.StrainRateCFOrientedHistory.Add(new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>());
                    bodyTmp1.StressRateCFOrientedHistory.Add(new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>());
                }

                index = Ep.StressRateSFHistory.Count;
                for (int placeholder = 0; index < Ep.StrainRateSFHistory.Count; index++)
                {
                    bodyTmp1.StrainRateSFHistory.Add(Ep.StrainRateSFHistory[index].Clone());
                }

                this.SimulationData.Add(bodyTmp1);
                this.BodyPaths.Add(bodyTmp1.SavePath);
            }
        }

        public Analysis.Stress.Plasticity.ElastoPlasticExperiment GetExperiment()
        {
            Analysis.Stress.Plasticity.ElastoPlasticExperiment ret = new Analysis.Stress.Plasticity.ElastoPlasticExperiment();
            
            ret.ChiAngle = this._chiAngle;
            ret.OmegaAngle = this._omegaAngle;
            ret.SampleArea = this._sampleArea;

            for (int n = 0; n < this.YieldInformation.Count; n++)
            {
                ret.YieldInformation.Add(this.YieldInformation[n].GetYieldSurface());
            }

            if (this.GrainOrientations != null)
            {
                ret.GrainOrientations = this.GrainOrientations;
            }
            else
            {
                ret.GrainOrientations = new List<List<Analysis.Stress.Plasticity.GrainOrientationParameter>>();
            }

            for (int phase = 0; phase < this.YieldInformation.Count; phase++)
            {
                ret.StrainRateCFOrientedHistory.Add(new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>());
                ret.StressRateCFOrientedHistory.Add(new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>());

                ret.ActiveSystemsCFOrientedHistory.Add(new List<List<List<Analysis.Stress.Plasticity.ReflexYield>>>());
            }
            MathNet.Numerics.LinearAlgebra.Matrix<double> totalStress = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
            MathNet.Numerics.LinearAlgebra.Matrix<double> totalStrain = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
            for (int n = 0; n < this.SimulationData.Count; n++)
            {
                if(SimulationData[n].Symmetric)
                {
                    for (int i = 0; i < SimulationData[n].StrainRateSFHistory.Count; i++)
                    {
                        ret.StressRateSFHistory.Add(SimulationData[n].StressRateSFHistory[i]);
                        ret.StrainRateSFHistory.Add(SimulationData[n].StrainRateSFHistory[i]);

                        totalStress += SimulationData[n].StressRateSFHistory[i];
                        totalStrain += SimulationData[n].StrainRateSFHistory[i];

                        ret.StressSFHistory.Add(totalStress.Clone());
                        ret.StrainSFHistory.Add(totalStrain.Clone());

                        ret.YieldChangeCFHistory.Add(SimulationData[n].YieldChangeCFHistory[i]);

                        for (int phase = 0; phase < this.YieldInformation.Count; phase++)
                        {
                            List<MathNet.Numerics.LinearAlgebra.Matrix<double>> crystalStrainRate1 = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

                            for (int j = 0; j < SimulationData[n].StrainRateCFOrientedHistory[phase][i].Count; j++)
                            {
                                crystalStrainRate1.Add(SimulationData[n].StrainRateCFOrientedHistory[phase][i][j].Clone());
                            }

                            ret.StrainRateCFOrientedHistory[phase].Add(crystalStrainRate1);

                            List<MathNet.Numerics.LinearAlgebra.Matrix<double>> crystalStressRate1 = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

                            for (int j = 0; j < SimulationData[n].StressRateCFOrientedHistory[phase][i].Count; j++)
                            {
                                crystalStressRate1.Add(SimulationData[n].StressRateCFOrientedHistory[phase][i][j].Clone());
                            }

                            ret.StressRateCFOrientedHistory[phase].Add(crystalStressRate1);

                            List<List<Analysis.Stress.Plasticity.ReflexYield>> activeSystems = new List<List<Analysis.Stress.Plasticity.ReflexYield>>();

                            for (int j = 0; j < SimulationData[n].ActiveSystemsCFOrientedHistory[phase][i].Count; j++)
                            {
                                if (SimulationData[n].ActiveSystemsCFOrientedHistory[phase][i][j].Count == 0)
                                {
                                    activeSystems.Add(new List<Analysis.Stress.Plasticity.ReflexYield>());
                                }
                                else
                                {
                                    List<Analysis.Stress.Plasticity.ReflexYield> activeSystemsGrain = new List<Analysis.Stress.Plasticity.ReflexYield>();

                                    for (int k = 0; k < SimulationData[n].ActiveSystemsCFOrientedHistory[phase][i][j].Count; k++)
                                    {
                                        activeSystemsGrain.Add(SimulationData[n].ActiveSystemsCFOrientedHistory[phase][i][j][k].GetReflexYield());
                                    }

                                    activeSystems.Add(activeSystemsGrain);
                                }
                            }

                            ret.ActiveSystemsCFOrientedHistory[phase].Add(activeSystems);
                        }
                    }
                }
                else
                {
                    if(SimulationData[n].StressRateSFHistory.Count == 0)
                    {
                        for (int i = 0; i < SimulationData[n].StrainRateSFHistory.Count; i++)
                        {
                            ret.StrainRateSFHistory.Add(SimulationData[n].StrainRateSFHistory[i]);
                            totalStrain += SimulationData[n].StrainRateSFHistory[i];
                            ret.StrainSFHistory.Add(totalStrain.Clone());
                        }
                    }
                    else
                    {
                        for (int i = 0; i < SimulationData[n].StressRateSFHistory.Count; i++)
                        {
                            ret.StressRateSFHistory.Add(SimulationData[n].StressRateSFHistory[i]);
                            totalStress += SimulationData[n].StressRateSFHistory[i];
                            ret.StressSFHistory.Add(totalStress.Clone());
                        }
                    }
                }
            }

            return ret;
        }

        public void LoadSimulationData(int index)
        {
            string actFilePath = this.SavePath + BodyPaths[index];
            try
            {
                using (System.IO.Stream fileStream = System.IO.File.OpenRead(actFilePath))
                {
                    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                    object DataObj = bf.Deserialize(fileStream);

                    SimBody Loaded = DataObj as SimBody;

                    this.SimulationData.Add(Loaded);

                }
            }
            catch
            {
                using (System.IO.Stream fileStream = System.IO.File.OpenRead(BodyPaths[index]))
                {
                    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                    object DataObj = bf.Deserialize(fileStream);

                    SimBody Loaded = DataObj as SimBody;

                    this.SimulationData.Add(Loaded);

                }
            }
        }

        public void SaveSimulationData(int index)
        {
            using (System.IO.Stream fileSaveStream = System.IO.File.Create(this.SavePath + SimulationData[index].SavePath))
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    bf.Serialize(ms, SimulationData[index]);

                    ms.WriteTo(fileSaveStream);
                }
            }
        }
    }
}
