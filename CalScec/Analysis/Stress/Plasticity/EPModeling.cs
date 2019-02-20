using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.Analysis.Stress.Plasticity
{
    public static class EPModeling
    {
        /// <summary>
        /// returns the total strains of the grains in Crystal system!!
        /// </summary>
        /// <param name="elasticModel"></param>
        /// <param name="phase"></param>
        /// <returns></returns>
        public static void PerformStandardExperimentGrain(Microsopic.ElasticityTensors eT, PlasticityTensor pT, ElastoPlasticExperiment ePE, int phase)
        {
            for (int n = 0; n < pT.YieldSurfaceData.PotentialSlipSystems.Count; n++)
            {
                pT.YieldSurfaceData.PotentialSlipSystems[n].YieldMainHardennedStrength = pT.YieldSurfaceData.PotentialSlipSystems[n].YieldMainStrength;
            }

            MathNet.Numerics.LinearAlgebra.Matrix<double> placsticStrain = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
            for (int n = 0; n < ePE.StressCFHistory[phase].Count; n++)
            {
                Tools.FourthRankTensor compliances = eT.GetFourtRankCompliances();
                MathNet.Numerics.LinearAlgebra.Vector<double> applStress = MathNet.Numerics.LinearAlgebra.CreateVector.Dense(6, 0.0);
                applStress[0] = ePE.StressCFHistory[phase][n][0, 0];
                applStress[1] = ePE.StressCFHistory[phase][n][1, 1];
                applStress[2] = ePE.StressCFHistory[phase][n][2, 2];
                applStress[3] = ePE.StressCFHistory[phase][n][1, 2];
                applStress[4] = ePE.StressCFHistory[phase][n][0, 2];
                applStress[5] = ePE.StressCFHistory[phase][n][0, 1];

                MathNet.Numerics.LinearAlgebra.Matrix<double> actElasticStrain = compliances * ePE.StressCFHistory[phase][n];

                MathNet.Numerics.LinearAlgebra.Matrix<double> actStrainRatePl = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
                List<ReflexYield> potentialActive = pT.YieldSurfaceData.GetPotentiallyActiveSlipSystems(applStress);
                double activeSystems = 0.0;
                double totalShearRate = 0.0;

                for (int i = 0; i < potentialActive.Count; i++)
                {
                    MathNet.Numerics.LinearAlgebra.Matrix<double> complianceFactors = pT.YieldSurfaceData.GetInstComplianceFactors(potentialActive, pT._hardenningTensor, eT, i);
                    double shearRate = pT.YieldSurfaceData.GetShearRate(complianceFactors, ePE.StrainRateCFHistory[phase][n]);
                    MathNet.Numerics.LinearAlgebra.Matrix<double> alphaAct = potentialActive[i].GetResolvingParameter();
                    potentialActive[i].ActShearRate = shearRate;

                    if (shearRate >= 0)
                    {
                        actStrainRatePl[0, 0] += shearRate * alphaAct[0, 0];
                        actStrainRatePl[1, 0] += shearRate * alphaAct[1, 0];
                        actStrainRatePl[2, 0] += shearRate * alphaAct[2, 0];
                        actStrainRatePl[0, 1] += shearRate * alphaAct[0, 1];
                        actStrainRatePl[1, 1] += shearRate * alphaAct[1, 1];
                        actStrainRatePl[2, 1] += shearRate * alphaAct[2, 1];
                        actStrainRatePl[0, 2] += shearRate * alphaAct[0, 2];
                        actStrainRatePl[1, 2] += shearRate * alphaAct[1, 2];
                        actStrainRatePl[2, 2] += shearRate * alphaAct[2, 2];
                        
                        totalShearRate += shearRate;
                    }
                    

                    activeSystems += potentialActive[i].DirectionMainMultiplizity * potentialActive[i].PlainMainMultiplizity;

                }

                MathNet.Numerics.LinearAlgebra.Matrix<double> hardening = pT.YieldSurfaceData.HardeningMatrixSlipSystem(potentialActive, pT._hardenningTensor);

                for (int i = 0; i < potentialActive.Count; i++)
                {
                    double directionHardeningRate = 0.0;
                    for (int j = 0; j < hardening.ColumnCount; j++)
                    {
                        directionHardeningRate += hardening[i, j] * potentialActive[j].ActShearRate;
                    }

                    potentialActive[i].YieldMainHardennedStrength += directionHardeningRate;
                }



                ePE.ShearRateCFHistory[phase].Add(totalShearRate);
                
                ePE.StrainRateCFHistory[phase][n][0, 0] = actStrainRatePl[0, 0];
                ePE.StrainRateCFHistory[phase][n][1, 0] = actStrainRatePl[1, 0];
                ePE.StrainRateCFHistory[phase][n][2, 0] = actStrainRatePl[2, 0];
                ePE.StrainRateCFHistory[phase][n][0, 1] = actStrainRatePl[0, 1];
                ePE.StrainRateCFHistory[phase][n][1, 1] = actStrainRatePl[1, 1];
                ePE.StrainRateCFHistory[phase][n][2, 1] = actStrainRatePl[2, 1];
                ePE.StrainRateCFHistory[phase][n][0, 2] = actStrainRatePl[0, 2];
                ePE.StrainRateCFHistory[phase][n][1, 2] = actStrainRatePl[1, 2];
                ePE.StrainRateCFHistory[phase][n][2, 2] = actStrainRatePl[2, 2];
                //ePE.StrainRateCFHistory[phase].Add(actStrainRatePl);
                //ePE.StrainRateCFHistory[phase][n] = actStrainRatePl;

                ePE.ActiveSystemsCFHistory[phase].Add(activeSystems);

                MathNet.Numerics.LinearAlgebra.Matrix<double> strainH = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

                placsticStrain[0, 0] += actStrainRatePl[0, 0];
                placsticStrain[1, 0] += actStrainRatePl[1, 0];
                placsticStrain[2, 0] += actStrainRatePl[2, 0];
                placsticStrain[0, 1] += actStrainRatePl[0, 1];
                placsticStrain[1, 1] += actStrainRatePl[1, 1];
                placsticStrain[2, 1] += actStrainRatePl[2, 1];
                placsticStrain[0, 2] += actStrainRatePl[0, 2];
                placsticStrain[1, 2] += actStrainRatePl[1, 2];
                placsticStrain[2, 2] += actStrainRatePl[2, 2];

                strainH[0, 0] += placsticStrain[0, 0] + actElasticStrain[0, 0];
                strainH[1, 0] += placsticStrain[1, 0] + actElasticStrain[1, 0];
                strainH[2, 0] += placsticStrain[2, 0] + actElasticStrain[2, 0];
                strainH[0, 1] += placsticStrain[0, 1] + actElasticStrain[0, 1];
                strainH[1, 1] += placsticStrain[1, 1] + actElasticStrain[1, 1];
                strainH[2, 1] += placsticStrain[2, 1] + actElasticStrain[2, 1];
                strainH[0, 2] += placsticStrain[0, 2] + actElasticStrain[0, 2];
                strainH[1, 2] += placsticStrain[1, 2] + actElasticStrain[1, 2];
                strainH[2, 2] += placsticStrain[2, 2] + actElasticStrain[2, 2];

                ePE.StrainCFHistory[phase].Add(strainH);

            }
        }

        /// <summary>
        /// returns the total strains of the grains in Crystal system!!
        /// </summary>
        /// <param name="elasticModel"></param>
        /// <param name="phase"></param>
        /// <returns></returns>
        public static List<MathNet.Numerics.LinearAlgebra.Matrix<double>> GetTotalStrainGrains(Microsopic.ElasticityTensors eT, PlasticityTensor pT, YieldSurface yS, List<MathNet.Numerics.LinearAlgebra.Matrix<double>> appliedStress, MathNet.Numerics.LinearAlgebra.Vector<double> strainRate)
        {
            List<MathNet.Numerics.LinearAlgebra.Matrix<double>> ret = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

            for (int n = 0; n < appliedStress.Count; n++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> applStress = MathNet.Numerics.LinearAlgebra.CreateVector.Dense(6, 0.0);
                applStress[0] = appliedStress[n][0, 0];
                applStress[1] = appliedStress[n][1, 1];
                applStress[2] = appliedStress[n][2, 2];
                applStress[3] = appliedStress[n][1, 2];
                applStress[4] = appliedStress[n][0, 2];
                applStress[5] = appliedStress[n][0, 1];

                MathNet.Numerics.LinearAlgebra.Vector<double> actStrain = eT._complianceTensor * applStress;

                MathNet.Numerics.LinearAlgebra.Vector<double> actStrainPl = MathNet.Numerics.LinearAlgebra.CreateVector.Dense(6, 0.0);

                for (int i = 0; i < yS.ReflexYieldData.Count; i++)
                {
                    MathNet.Numerics.LinearAlgebra.Vector<double> slipStress = MathNet.Numerics.LinearAlgebra.CreateVector.Dense(3, 0.0);

                    MathNet.Numerics.LinearAlgebra.Vector<double> slipDirection = MathNet.Numerics.LinearAlgebra.CreateVector.Dense(3, 0.0);

                    slipDirection[0] = yS.ReflexYieldData[i].MainSlipDirection.H;
                    slipDirection[1] = yS.ReflexYieldData[i].MainSlipDirection.K;
                    slipDirection[2] = yS.ReflexYieldData[i].MainSlipDirection.L;

                    slipStress[0] += applStress[0] * yS.ReflexYieldData[i].MainSlipDirection.H;
                    slipStress[0] += applStress[5] * yS.ReflexYieldData[i].MainSlipDirection.K;
                    slipStress[0] += applStress[4] * yS.ReflexYieldData[i].MainSlipDirection.L;

                    if(slipStress.L2Norm() > yS.ReflexYieldData[i].YieldMainStrength)
                    {
                        MathNet.Numerics.LinearAlgebra.Vector<double> sliphardening = pT._hardenningTensor * slipDirection;
                        yS.ReflexYieldData[i].YieldMainHardennedStrength += sliphardening.L2Norm();

                        //actStrainPl = actStrainPl + ()
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Lattice Strains and stresses in a specific direction (in grain). 
        /// </summary>
        /// <param name="eT"></param>
        /// <param name="pT"></param>
        /// <param name="rY"></param>
        /// <param name="appliedStress"></param>
        /// <returns></returns>
        public static List<double[]> GetLatticeStrains(Microsopic.ElasticityTensors eT, PlasticityTensor pT, ReflexYield rY, List<MathNet.Numerics.LinearAlgebra.Matrix<double>> appliedStress)
        {
            List<double[]> ret = new List<double[]>();
            rY.YieldMainHardennedStrength = rY.YieldMainStrength;

            for (int n = 0; n < appliedStress.Count; n++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> applStress = MathNet.Numerics.LinearAlgebra.CreateVector.Dense(6, 0.0);
                applStress[0] = appliedStress[n][0, 0];
                applStress[1] = appliedStress[n][1, 1];
                applStress[2] = appliedStress[n][2, 2];
                applStress[3] = appliedStress[n][1, 2];
                applStress[4] = appliedStress[n][0, 2];
                applStress[5] = appliedStress[n][0, 1];

                MathNet.Numerics.LinearAlgebra.Vector<double> actStrain = eT._complianceTensor * applStress;

                MathNet.Numerics.LinearAlgebra.Vector<double> actStrainPl = MathNet.Numerics.LinearAlgebra.CreateVector.Dense(6, 0.0);

                MathNet.Numerics.LinearAlgebra.Vector<double> slipStress = MathNet.Numerics.LinearAlgebra.CreateVector.Dense(3, 0.0);

                MathNet.Numerics.LinearAlgebra.Vector<double> slipDirection = MathNet.Numerics.LinearAlgebra.CreateVector.Dense(3, 0.0);

                slipDirection[0] = rY.MainSlipDirection.H;
                slipDirection[1] = rY.MainSlipDirection.K;
                slipDirection[2] = rY.MainSlipDirection.L;

                slipStress = appliedStress[n] * slipDirection;

                if (slipStress.L2Norm() > rY.YieldMainHardennedStrength)
                {
                    MathNet.Numerics.LinearAlgebra.Vector<double> sliphardening = pT._hardenningTensor * slipDirection;
                    rY.YieldMainHardennedStrength += sliphardening.L2Norm();

                    for(int i = 0; i < eT.DiffractionConstants.Count; i++)
                    {
                        if(eT.DiffractionConstants[i].UsedReflex.HKLString == rY.SlipPlane.HKLString)
                        {
                            double[] tmp = { slipStress.L2Norm(), 1 / (eT.DiffractionConstants[i].ClassicEModulus) * rY.YieldMainHardennedStrength };
                            ret.Add(tmp);
                        }
                    }
                }
                else
                {
                    MathNet.Numerics.LinearAlgebra.Vector<double> slipStrain = MathNet.Numerics.LinearAlgebra.CreateVector.Dense(3, 0.0);

                    slipStrain[0] += actStrain[0] * rY.MainSlipDirection.H;
                    slipStrain[0] += actStrain[5] * rY.MainSlipDirection.K;
                    slipStrain[0] += actStrain[4] * rY.MainSlipDirection.L;

                    slipStrain[1] += actStrain[5] * rY.MainSlipDirection.H;
                    slipStrain[1] += actStrain[1] * rY.MainSlipDirection.K;
                    slipStrain[1] += actStrain[3] * rY.MainSlipDirection.L;

                    slipStrain[2] += actStrain[4] * rY.MainSlipDirection.H;
                    slipStrain[2] += actStrain[3] * rY.MainSlipDirection.K;
                    slipStrain[2] += actStrain[2] * rY.MainSlipDirection.L;

                    double[] tmp = { slipStress.L2Norm(), slipStrain.L2Norm() };
                    ret.Add(tmp);
                }
            }

            return ret;
        }

        public static List<double[]> GetPhaseStrainLD(Microsopic.ElasticityTensors eT, PlasticityTensor pT, List<MathNet.Numerics.LinearAlgebra.Matrix<double>> appliedStress)
        {
            List<double[]> ret = new List<double[]>();

            double plasticStrain = 0.0;
            pT.PhaseActYieldStrength = pT.PhaseYieldStrength;
            for (int n = 0; n < appliedStress.Count; n++)
            {
                if (appliedStress[n][2, 2] > pT.PhaseActYieldStrength)
                {
                    plasticStrain += pT.PhaseStrainRate;
                    pT.PhaseActYieldStrength += pT.PhaseStrainRate;
                }

                double[] dataTmp = { appliedStress[n][2, 2], ((1 / eT.AveragedEModul) * appliedStress[n][2, 2]) + plasticStrain };

                ret.Add(dataTmp);
            }

            return ret;
        }

    }
}
