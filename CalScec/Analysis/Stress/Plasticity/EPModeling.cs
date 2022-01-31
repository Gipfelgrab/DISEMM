using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace CalScec.Analysis.Stress.Plasticity
{
    public static class EPModeling
    {
        #region Step by Step Modeling for multithreading

        public static void PerformStrainExperimentSt(Sample actSample, int experimentIndex, int n, int cycleLimit, bool textureActive, bool useHardenningMatrix, bool invertSlipSystems)
        {
            // Parameter definition
            //Potentiel aktivierte Gleitsysteme in 
            List<List<List<ReflexYield>>> potentialActiveGOriented = new List<List<List<ReflexYield>>>();
            //Stress rate for the sample
            MathNet.Numerics.LinearAlgebra.Matrix<double> strainRateS = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
            //Dehnungstensoren Tensoren guess ist einfach elastisch
            Tools.FourthRankTensor overallStiffnesses = actSample.GetSampleStiffnesses(textureActive);
            Tools.FourthRankTensor overallStiffnessesComp = overallStiffnesses.Clone() as Tools.FourthRankTensor;
            //Constraint Tensor L* Komplettes Sample
            Tools.FourthRankTensor constraintStiffness = new Tools.FourthRankTensor();
            //Lc
            List<List<Tools.FourthRankTensor>> grainStiffnesses = new List<List<Tools.FourthRankTensor>>();
            //Ac
            List<List<Tools.FourthRankTensor>> grainTransitionStiffnesses = new List<List<Tools.FourthRankTensor>>();
            //gemittelten Phasen Nachgiebigkeiten
            List<Tools.FourthRankTensor> overallStiffnessesPhase = new List<Tools.FourthRankTensor>();
            //Grain Spannungen der Phasen und Orientierungen
            List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>> grainStressesOriented = new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>();
            //Grain Dehnungen der Phasen und Orientierungen
            List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>> grainStrainsOriented = new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>();
            List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>> hardeningMatrixList = new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>();

            List<List<List<double>>> yieldChangeOriented = new List<List<List<double>>>();
            List<double[]> averageYieldChange = new List<double[]>();

            //Phasen werden in den Listen definiert
            for (int phase = 0; phase < actSample.CrystalData.Count; phase++)
            {
                overallStiffnessesPhase.Add(actSample.HillTensorData[phase].GetFourtRankStiffnesses());
                grainStiffnesses.Add(new List<Tools.FourthRankTensor>());
                grainTransitionStiffnesses.Add(new List<Tools.FourthRankTensor>());
                potentialActiveGOriented.Add(new List<List<ReflexYield>>());
                grainStressesOriented.Add(new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>());
                grainStrainsOriented.Add(new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>());
                hardeningMatrixList.Add(new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>());
                yieldChangeOriented.Add(new List<List<double>>());
                averageYieldChange.Add(new double[5]);
            }

            //Macroskopische Spannungsrate berechnen
            if (n == 0)
            {
                strainRateS[0, 0] += actSample.SimulationData[experimentIndex].StrainSFHistory[n][0, 0];
                strainRateS[1, 0] += actSample.SimulationData[experimentIndex].StrainSFHistory[n][1, 0];
                strainRateS[2, 0] += actSample.SimulationData[experimentIndex].StrainSFHistory[n][2, 0];
                strainRateS[0, 1] += actSample.SimulationData[experimentIndex].StrainSFHistory[n][0, 1];
                strainRateS[1, 1] += actSample.SimulationData[experimentIndex].StrainSFHistory[n][1, 1];
                strainRateS[2, 1] += actSample.SimulationData[experimentIndex].StrainSFHistory[n][2, 1];
                strainRateS[0, 2] += actSample.SimulationData[experimentIndex].StrainSFHistory[n][0, 2];
                strainRateS[1, 2] += actSample.SimulationData[experimentIndex].StrainSFHistory[n][1, 2];
                strainRateS[2, 2] += actSample.SimulationData[experimentIndex].StrainSFHistory[n][2, 2];
            }
            else
            {
                strainRateS[0, 0] += actSample.SimulationData[experimentIndex].StrainSFHistory[n][0, 0] - actSample.SimulationData[experimentIndex].StrainSFHistory[n - 1][0, 0];
                strainRateS[1, 0] += actSample.SimulationData[experimentIndex].StrainSFHistory[n][1, 0] - actSample.SimulationData[experimentIndex].StrainSFHistory[n - 1][1, 0];
                strainRateS[2, 0] += actSample.SimulationData[experimentIndex].StrainSFHistory[n][2, 0] - actSample.SimulationData[experimentIndex].StrainSFHistory[n - 1][2, 0];
                strainRateS[0, 1] += actSample.SimulationData[experimentIndex].StrainSFHistory[n][0, 1] - actSample.SimulationData[experimentIndex].StrainSFHistory[n - 1][0, 1];
                strainRateS[1, 1] += actSample.SimulationData[experimentIndex].StrainSFHistory[n][1, 1] - actSample.SimulationData[experimentIndex].StrainSFHistory[n - 1][1, 1];
                strainRateS[2, 1] += actSample.SimulationData[experimentIndex].StrainSFHistory[n][2, 1] - actSample.SimulationData[experimentIndex].StrainSFHistory[n - 1][2, 1];
                strainRateS[0, 2] += actSample.SimulationData[experimentIndex].StrainSFHistory[n][0, 2] - actSample.SimulationData[experimentIndex].StrainSFHistory[n - 1][0, 2];
                strainRateS[1, 2] += actSample.SimulationData[experimentIndex].StrainSFHistory[n][1, 2] - actSample.SimulationData[experimentIndex].StrainSFHistory[n - 1][1, 2];
                strainRateS[2, 2] += actSample.SimulationData[experimentIndex].StrainSFHistory[n][2, 2] - actSample.SimulationData[experimentIndex].StrainSFHistory[n - 1][2, 2];
            }

            MathNet.Numerics.LinearAlgebra.Matrix<double> stressRateS = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
            MathNet.Numerics.LinearAlgebra.Matrix<double> stressS = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);


            //Selbstkonsistente Rechung
            for (int actCycle = 0; actCycle < cycleLimit; actCycle++)
            {
                Microsopic.ElasticityTensors eTmp = new Microsopic.ElasticityTensors();
                eTmp._stiffnessTensor = overallStiffnesses.GetVoigtTensor();
                eTmp.CalculateCompliances();
                constraintStiffness = actSample.PlasticTensor[0].GetConstraintStiffnessCubicIsotropic(eTmp, 2);

                stressRateS = overallStiffnesses * strainRateS;

                if (n == 0)
                {
                    stressS += stressRateS;
                }
                else
                {
                    stressS = actSample.SimulationData[experimentIndex].StressSFHistory[n - 1] + stressRateS;
                }

                //Berechnungen im Krystallsystem
                for (int phase = 0; phase < actSample.CrystalData.Count; phase++)
                {
                    double maxPhi1 = 360;
                    double maxPsi = 360;
                    double maxPhi2 = 360;

                    //Setzen der gemittelten Orientierungen
                    if (actSample.CrystalData[phase].SymmetryGroupID == 225 || actSample.CrystalData[phase].SymmetryGroupID == 229)
                    {
                        //maxPhi1 = 45;
                        //maxPsi = 45;
                        //maxPhi2 = 45;
                        maxPhi1 = 90;
                        maxPsi = 90;
                        maxPhi2 = 90;
                    }
                    if (actSample.CrystalData[phase].SymmetryGroupID == 194)
                    {
                        maxPhi1 = 60;
                        maxPsi = 90;
                        maxPhi2 = 60;
                    }
                    if (actSample.SimulationData[experimentIndex].GrainOrientations[phase].Count == 0)
                    {
                        for (double phi1 = 0.0; phi1 < maxPhi1; phi1 += 5.0)
                        {
                            for (double psi = 0.0; psi < maxPsi; psi += 5.0)
                            {
                                for (double phi2 = 0.0; phi2 < maxPhi2; phi2 += 5.0)
                                {
                                    actSample.SimulationData[experimentIndex].GrainOrientations[phase].Add(new GrainOrientationParameter(phi1, psi, phi2));
                                }
                            }
                        }
                    }

                    //Alle Grain parameter für die verschiedenen Orientierungen werden gesetzt
                    //Lc für aktuelle phase
                    List<Tools.FourthRankTensor> grainInstStiffnessesOriented = new List<Tools.FourthRankTensor>();
                    //Ac für die aktuelle Phase
                    List<Tools.FourthRankTensor> grainTransitionStiffnessesOriented = new List<Tools.FourthRankTensor>();
                    //GrainSpanungen für alle orientierungen
                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> grainStressesPhase = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
                    //Grain Dehnungen für alle orientierungen
                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> grainStrainsPhase = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
                    //Active Gleitsysteme
                    List<List<ReflexYield>> potentialActiveGPhase = new List<List<ReflexYield>>();
                    //Härtungsmatrix in den verschiedenen Orientierungen
                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> hardeningMatrixOriented = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

                    List<List<double>> yieldChangePhase = new List<List<double>>();

                    int grainIndexCounter = 0;

                    for (double phi1 = 0.0; phi1 < maxPhi1; phi1 += 5.0)
                    {
                        for (double psi = 0.0; psi < maxPsi; psi += 5.0)
                        {
                            for (double phi2 = 0.0; phi2 < maxPhi2; phi2 += 5.0)
                            {
                                //Lc einzeln
                                Tools.FourthRankTensor instGrainTensor = new Tools.FourthRankTensor();
                                //Ac einzeln
                                Tools.FourthRankTensor transitionGrainTensor = new Tools.FourthRankTensor();
                                MathNet.Numerics.LinearAlgebra.Matrix<double> hardeningMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense<double>(3, 3, 0);

                                //Drehen des Spannungstensor in die aktuelle Orientierung
                                MathNet.Numerics.LinearAlgebra.Matrix<double> transformationMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense<double>(3, 3, 0);

                                transformationMatrix[0, 0] = -1 * Math.Cos(phi1 * (Math.PI / 180)) * Math.Cos(psi * (Math.PI / 180)) * Math.Sin(phi2 * (Math.PI / 180));
                                transformationMatrix[0, 0] -= Math.Sin(phi1 * (Math.PI / 180)) * Math.Cos(phi2 * (Math.PI / 180));
                                transformationMatrix[0, 1] = -1 * Math.Cos(psi * (Math.PI / 180)) * Math.Sin(phi1 * (Math.PI / 180)) * Math.Sin(phi2 * (Math.PI / 180));
                                transformationMatrix[0, 1] -= Math.Cos(phi1 * (Math.PI / 180)) * Math.Cos(phi2 * (Math.PI / 180));
                                transformationMatrix[0, 2] = Math.Sin(psi * (Math.PI / 180)) * Math.Sin(phi2 * (Math.PI / 180));
                                transformationMatrix[1, 0] = -1 * Math.Cos(psi * (Math.PI / 180)) * Math.Cos(phi1 * (Math.PI / 180)) * Math.Cos(phi2 * (Math.PI / 180));
                                transformationMatrix[1, 0] -= Math.Sin(phi1 * (Math.PI / 180)) * Math.Sin(phi2 * (Math.PI / 180));
                                transformationMatrix[1, 1] = -1 * Math.Sin(phi1 * (Math.PI / 180)) * Math.Cos(psi * (Math.PI / 180)) * Math.Cos(phi2 * (Math.PI / 180));
                                transformationMatrix[1, 1] -= Math.Cos(phi1 * (Math.PI / 180)) * Math.Sin(phi2 * (Math.PI / 180));
                                transformationMatrix[1, 2] = Math.Sin(psi * (Math.PI / 180)) * Math.Cos(phi2 * (Math.PI / 180));
                                transformationMatrix[2, 0] = Math.Cos(phi1 * (Math.PI / 180)) * Math.Sin(psi * (Math.PI / 180));
                                transformationMatrix[2, 1] = Math.Sin(phi1 * (Math.PI / 180)) * Math.Sin(psi * (Math.PI / 180));
                                transformationMatrix[2, 2] = Math.Cos(psi * (Math.PI / 180));


                                MathNet.Numerics.LinearAlgebra.Matrix<double> actStrainRateGrainAc = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
                                //for the self consistence the stress and stress rate are calculated from the last Bc if it exists
                                MathNet.Numerics.LinearAlgebra.Matrix<double> actStressGrainBc = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
                                MathNet.Numerics.LinearAlgebra.Matrix<double> actStressRateGrainBc = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

                                if (actCycle == 0)
                                {
                                    actStressGrainBc = overallStiffnesses * actSample.SimulationData[experimentIndex].StrainSFHistory[n];
                                    actStressRateGrainBc = stressRateS;
                                }
                                else
                                {
                                    ///constraintStiffness ergibt sich für alle Phasen gleichermaßen und wird aktualisiert
                                    ///instGrainTensor wird aus Orientierungen berechnet und muss aus vorherigen Schritt genommen werden
                                    ///overallStiffnesses wird pro cycle aktualisiert
                                    Tools.FourthRankTensor lastAc = actSample.PlasticTensor[phase].GetAc(constraintStiffness, grainStiffnesses[phase][grainIndexCounter], overallStiffnesses);

                                    actStrainRateGrainAc = lastAc * strainRateS;
                                    actStressRateGrainBc = grainStiffnesses[phase][grainIndexCounter] * strainRateS;
                                    for (int k = 0; k < n; k++)
                                    {
                                        actStressGrainBc += actSample.SimulationData[experimentIndex].StressRateCFOrientedHistory[phase][k][grainIndexCounter];
                                    }
                                }

                                //Spannungstensor in der aktuellen Orientierung
                                MathNet.Numerics.LinearAlgebra.Matrix<double> actStressOriented = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
                                MathNet.Numerics.LinearAlgebra.Matrix<double> actStressRateOriented = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
                                for (int i = 0; i < 3; i++)
                                {
                                    for (int j = 0; j < 3; j++)
                                    {
                                        for (int k = 0; k < 3; k++)
                                        {
                                            for (int l = 0; l < 3; l++)
                                            {
                                                //actStressOriented[i, j] += transformationMatrix[i, k] * transformationMatrix[j, l] * actStressGrainBc[k, l];
                                                actStressRateOriented[i, j] += transformationMatrix[i, k] * transformationMatrix[j, l] * actStressRateGrainBc[k, l];
                                            }
                                        }
                                    }
                                }
                                actStressOriented = actStressGrainBc + actStressRateOriented;

                                //Potentiell aktive Gleitsysteme werden ermittelt
                                List<ReflexYield> potentialActive = actSample.SimulationData[experimentIndex].YieldInformation[phase].GetPotentiallyActiveSlipSystems(actStressOriented);

                                if (potentialActive.Count != 0)
                                {
                                    List<ReflexYield> trialSystems = new List<ReflexYield>();
                                    //List<bool> checkedSystems = new List<bool>();
                                    //for (int i = 0; i < potentialActive.Count; i++)
                                    //{
                                    //    checkedSystems.Add(true);
                                    //    trialSystems.Add(potentialActive[i]);
                                    //}
                                    List<int> checkedSystems = new List<int>();
                                    for (int i = 0; i < potentialActive.Count; i++)
                                    {
                                        checkedSystems.Add(1);
                                        trialSystems.Add(potentialActive[i]);
                                    }

                                    for (int systemTrial = 0; systemTrial < Convert.ToInt32(Math.Pow(2, potentialActive.Count)); systemTrial++)
                                    {
                                        //fi einzeln
                                        List<MathNet.Numerics.LinearAlgebra.Matrix<double>> stiffnessFactorsOriented = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
                                        //setzen der kombination aus möglichen Gleitsystemen
                                        if (!invertSlipSystems)
                                        {
                                            trialSystems = GetActiveSystemCombination(potentialActive, systemTrial);
                                        }
                                        else
                                        {
                                            List<int> checkedSystemsRe = new List<int>();
                                            trialSystems = GetActiveSystemCombination(trialSystems, checkedSystems);
                                            for (int m = 0; m < checkedSystems.Count; m++)
                                            {
                                                switch (checkedSystems[m])
                                                {
                                                    case 0:
                                                        goto default;
                                                    case 1:
                                                        checkedSystemsRe.Add(1);
                                                        break;
                                                    case 2:
                                                        checkedSystemsRe.Add(2);
                                                        break;
                                                    default:
                                                        break;
                                                }
                                            }
                                            checkedSystems = checkedSystemsRe;
                                        }
                                        //
                                        //checkedSystems.Clear();
                                        //for (int i = 0; i < trialSystems.Count; i++)
                                        //{
                                        //    checkedSystems.Add(true);
                                        //}
                                        //for (int i = 0; i < trialSystems.Count; i++)
                                        //{
                                        //    checkedSystems.Add(1);
                                        //}
                                        if (trialSystems.Count != 0)
                                        {
                                            hardeningMatrix = actSample.SimulationData[experimentIndex].YieldInformation[phase].HardeningMatrixSlipSystem(trialSystems);
                                            if (useHardenningMatrix)
                                            {
                                                hardeningMatrix = actSample.SimulationData[experimentIndex].YieldInformation[phase].HardeningMatrixSlipSystem(trialSystems, actSample.SimulationData[experimentIndex]._hardenningTensor[phase]);
                                            }
                                            else
                                            {
                                                hardeningMatrix = actSample.SimulationData[experimentIndex].YieldInformation[phase].HardeningMatrixSlipSystem(trialSystems);
                                            }

                                            MathNet.Numerics.LinearAlgebra.Matrix<double> slipSystemX = actSample.SimulationData[experimentIndex].YieldInformation[phase].SlipSystemX(trialSystems, hardeningMatrix, actSample.HillTensorData[phase]);
                                            MathNet.Numerics.LinearAlgebra.Matrix<double> slipSystemY = slipSystemX.Inverse();

                                            //Shear rates
                                            List<double> shearRatesSystems = new List<double>();
                                            //shear stress change rates
                                            List<double> shearStressChangeSystems = new List<double>();
                                            //yield change rates
                                            List<double> yieldChangeSystems = new List<double>();

                                            for (int i = 0; i < trialSystems.Count; i++)
                                            {
                                                //Berechnung der instantanious Stiffness factors f^i
                                                MathNet.Numerics.LinearAlgebra.Matrix<double> compeFactorsTmp = actSample.SimulationData[experimentIndex].YieldInformation[phase].GetInstStiffnessFactors(trialSystems, actSample.HillTensorData[phase], i, slipSystemY);
                                                stiffnessFactorsOriented.Add(compeFactorsTmp);
                                            }
                                            //Lc
                                            instGrainTensor = actSample.PlasticTensor[phase].GetInstantanousGrainSiffnessTensor(trialSystems, actSample.HillTensorData[phase], stiffnessFactorsOriented);

                                            MathNet.Numerics.LinearAlgebra.Matrix<double> testTensor = instGrainTensor.GetVoigtTensor();
                                            //MathNet.Numerics.LinearAlgebra.Matrix<double> symcheck = testTensor.Transpose();
                                            //MathNet.Numerics.LinearAlgebra.Matrix<double> symdif = symcheck - testTensor;
                                            //berechung von Ac
                                            transitionGrainTensor = actSample.PlasticTensor[phase].GetAc(constraintStiffness, instGrainTensor, overallStiffnesses);

                                            //MathNet.Numerics.LinearAlgebra.Matrix<double> testTensorAc = instGrainTensor.GetVoigtTensor();
                                            //bzw Bc
                                            //Tools.FourthRankTensor Bc = actSample.PlasticTensor[phase].GetAc(constraintStiffness.InverseSC(), instGrainTensor.InverseSC(), overallStiffnesses.InverseSC());

                                            //Check, ob die ausgewählten Gleitsysteme richtig sind
                                            //berechnung der Grain Spannungsrate         actStrainOriented
                                            MathNet.Numerics.LinearAlgebra.Matrix<double> strainRateGrainComp = transitionGrainTensor * strainRateS;
                                            //actStressRateGrainBc = grainStiffnesses[phase][grainIndexCounter] * strainRateS;
                                            //MathNet.Numerics.LinearAlgebra.Matrix<double> stressRateGrain = Bc * actStressRateOriented;

                                            MathNet.Numerics.LinearAlgebra.Matrix<double> strainRateGrain = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense<double>(3, 3, 0); ;
                                            for (int i = 0; i < 3; i++)
                                            {
                                                for (int j = 0; j < 3; j++)
                                                {
                                                    for (int k = 0; k < 3; k++)
                                                    {
                                                        for (int l = 0; l < 3; l++)
                                                        {
                                                            strainRateGrain[i, j] += transformationMatrix[i, k] * transformationMatrix[j, l] * strainRateGrainComp[k, l];
                                                        }
                                                    }
                                                }
                                            }

                                            MathNet.Numerics.LinearAlgebra.Matrix<double> stressRateGrain = instGrainTensor * strainRateGrain;

                                            //MathNet.Numerics.LinearAlgebra.Matrix<double> symdif = strainRateGrainOrientedComp - strainRateGrain;

                                            //Berechnung der Yieldstressänderung
                                            //Mc wird aus dem Inversen von Lc berechnet
                                            //Tools.FourthRankTensor Mc = instGrainTensor.InverseSC();

                                            //Invertierungstestbereich
                                            //Tools.FourthRankTensor unityTest = instGrainTensor * Mc;
                                            //Tools.FourthRankTensor unity = Tools.FourthRankTensor.GetUnityTensor();

                                            //double inversionDifference = unityTest.GetDifference(unity);


                                            //MathNet.Numerics.LinearAlgebra.Matrix<double> strainRateGrain = Mc * stressRateGrain;


                                            //Berechnung der shear rates und der spannungsänderung
                                            for (int i = 0; i < trialSystems.Count; i++)
                                            {
                                                double actShearRate = 0.0;
                                                double shearstress = 0.0;
                                                MathNet.Numerics.LinearAlgebra.Matrix<double> resolvingMatrix = Tools.Calculation.GetResolvingParameter(trialSystems[i].SlipPlane, trialSystems[i].MainSlipDirection);

                                                for (int j = 0; j < 3; j++)
                                                {
                                                    for (int k = 0; k < 3; k++)
                                                    {
                                                        actShearRate += stiffnessFactorsOriented[i][j, k] * strainRateGrain[j, k];
                                                        shearstress += stressRateGrain[j, k] * resolvingMatrix[j, k];
                                                    }
                                                }

                                                shearRatesSystems.Add(actShearRate);
                                                shearStressChangeSystems.Add(shearstress);
                                            }

                                            //Berechung der yield changes
                                            for (int i = 0; i < trialSystems.Count; i++)
                                            {
                                                double yieldChangeTmp = 0.0;
                                                for (int j = 0; j < trialSystems.Count; j++)
                                                {
                                                    yieldChangeTmp += hardeningMatrix[i, j] * shearRatesSystems[j];
                                                }
                                                yieldChangeSystems.Add(yieldChangeTmp);
                                            }

                                            bool repeat = false;
                                            if (invertSlipSystems)
                                            {
                                                //Checkt ob die Scherraten positiv sind und markiert diejenigen, deren Gleitrichtung umgedreht werden muss
                                                for (int sys = 0; sys < shearRatesSystems.Count; sys++)
                                                {
                                                    //Check ob die scherraten positiv oder negativ sind
                                                    if (shearRatesSystems[sys] < 0)
                                                    {
                                                        //Falls negativ wird die Gleitrichtung umgedreht
                                                        //Falls
                                                        if (checkedSystems[sys] == 1)
                                                        {
                                                            checkedSystems[sys] = 2;
                                                        }
                                                        else if (checkedSystems[sys] == 2)
                                                        {
                                                            //trialSystems[sys].MainSlipDirection.H *= -1;
                                                            //trialSystems[sys].MainSlipDirection.K *= -1;
                                                            //trialSystems[sys].MainSlipDirection.L *= -1;
                                                            checkedSystems[sys] = 0;
                                                        }
                                                        //checkedSystems[sys] = false;
                                                        repeat = true;
                                                    }
                                                }
                                            }
                                            if (!repeat)
                                            {
                                                //Vergleich zwischen shear rates und yield changes
                                                double difference = 0.0;
                                                for (int i = 0; i < trialSystems.Count; i++)
                                                {
                                                    difference = Math.Abs(yieldChangeSystems[i] - shearStressChangeSystems[i]);
                                                }

                                                if (difference < 1)
                                                {
                                                    //Speichern der Parameter
                                                    grainInstStiffnessesOriented.Add(instGrainTensor);
                                                    grainTransitionStiffnessesOriented.Add(transitionGrainTensor);
                                                    potentialActiveGPhase.Add(trialSystems);
                                                    hardeningMatrixOriented.Add(hardeningMatrix);
                                                    grainStrainsPhase.Add(strainRateGrain);
                                                    grainStressesPhase.Add(stressRateGrain);
                                                    yieldChangePhase.Add(yieldChangeSystems);
                                                    break;
                                                }
                                                else
                                                {
                                                    //grainInstStiffnessesOriented.Add(instGrainTensor);
                                                    //grainTransitionStiffnessesOriented.Add(transitionGrainTensor);
                                                    //potentialActiveGPhase.Add(trialSystems);
                                                    //hardeningMatrixOriented.Add(hardeningMatrix);
                                                    //grainStrainsPhase.Add(strainRateGrain);
                                                    //grainStressesPhase.Add(stressRateGrain);
                                                    //yieldChangePhase.Add(yieldChangeSystems);
                                                    //break;
                                                    if (trialSystems.Count == 0)
                                                    {
                                                        //Speichern der Parameter
                                                        grainInstStiffnessesOriented.Add(instGrainTensor);
                                                        grainTransitionStiffnessesOriented.Add(transitionGrainTensor);
                                                        potentialActiveGPhase.Add(trialSystems);
                                                        hardeningMatrixOriented.Add(hardeningMatrix);
                                                        grainStrainsPhase.Add(strainRateGrain);
                                                        grainStressesPhase.Add(stressRateGrain);
                                                        yieldChangePhase.Add(yieldChangeSystems);
                                                        break;
                                                    }
                                                    //for (int sys = 0; sys < shearRatesSystems.Count; sys++)
                                                    //{
                                                    //    if (shearRatesSystems[sys] < 0)
                                                    //    {
                                                    //        checkedSystems[sys] = false;
                                                    //    }
                                                    //}
                                                }
                                            }
                                        }
                                        else
                                        {
                                            instGrainTensor = actSample.HillTensorData[phase].GetFourtRankStiffnesses();

                                            //berechung von Ac
                                            transitionGrainTensor = actSample.PlasticTensor[phase].GetAc(constraintStiffness, instGrainTensor, overallStiffnesses);
                                            //bzw Bc
                                            //Tools.FourthRankTensor Bc = actSample.PlasticTensor[phase].GetAc(constraintStiffness.InverseSC(), instGrainTensor.InverseSC(), overallStiffnesses.InverseSC());

                                            //Check, ob die ausgewählten Gleitsysteme richtig sind
                                            //berechnung der Grain Spannungsrate   actStrainOriented
                                            //MathNet.Numerics.LinearAlgebra.Matrix<double> strainRateGrainComp = transitionGrainTensor * strainRateS;

                                            MathNet.Numerics.LinearAlgebra.Matrix<double> strainRateGrainComp = transitionGrainTensor * strainRateS;
                                            //actStressRateGrainBc = grainStiffnesses[phase][grainIndexCounter] * strainRateS;
                                            //MathNet.Numerics.LinearAlgebra.Matrix<double> stressRateGrain = Bc * actStressRateOriented;

                                            MathNet.Numerics.LinearAlgebra.Matrix<double> strainRateGrain = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense<double>(3, 3, 0); ;
                                            for (int i = 0; i < 3; i++)
                                            {
                                                for (int j = 0; j < 3; j++)
                                                {
                                                    for (int k = 0; k < 3; k++)
                                                    {
                                                        for (int l = 0; l < 3; l++)
                                                        {
                                                            strainRateGrain[i, j] += transformationMatrix[i, k] * transformationMatrix[j, l] * strainRateGrainComp[k, l];
                                                        }
                                                    }
                                                }
                                            }

                                            //Berechnung der Yieldstressänderung
                                            //Mc wird aus dem Inversen von Lc berechnet
                                            //Tools.FourthRankTensor Mc = instGrainTensor.InverseSC();
                                            MathNet.Numerics.LinearAlgebra.Matrix<double> stressRateGrain = instGrainTensor * strainRateGrain;
                                            //MathNet.Numerics.LinearAlgebra.Matrix<double> strainRateGrain = Mc * stressRateGrain;

                                            //Speichern der Parameter
                                            grainInstStiffnessesOriented.Add(instGrainTensor);
                                            grainTransitionStiffnessesOriented.Add(transitionGrainTensor);
                                            potentialActiveGPhase.Add(trialSystems);
                                            hardeningMatrixOriented.Add(hardeningMatrix);
                                            grainStrainsPhase.Add(strainRateGrain);
                                            grainStressesPhase.Add(stressRateGrain);
                                            yieldChangePhase.Add(new List<double>());

                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    instGrainTensor = actSample.HillTensorData[phase].GetFourtRankStiffnesses();

                                    //berechung von Ac
                                    transitionGrainTensor = actSample.PlasticTensor[phase].GetAc(constraintStiffness, instGrainTensor, overallStiffnesses);
                                    //bzw Bc
                                    //Tools.FourthRankTensor Bc = actSample.PlasticTensor[phase].GetAc(constraintStiffness.InverseSC(), instGrainTensor.InverseSC(), overallStiffnesses.InverseSC());

                                    //Check, ob die ausgewählten Gleitsysteme richtig sind
                                    //berechnung der Grain Spannungsrate   actStrainOriented
                                    //MathNet.Numerics.LinearAlgebra.Matrix<double> strainRateGrainComp = transitionGrainTensor * strainRateS;

                                    MathNet.Numerics.LinearAlgebra.Matrix<double> strainRateGrainComp = transitionGrainTensor * strainRateS;
                                    //actStressRateGrainBc = grainStiffnesses[phase][grainIndexCounter] * strainRateS;
                                    //MathNet.Numerics.LinearAlgebra.Matrix<double> stressRateGrain = Bc * actStressRateOriented;

                                    MathNet.Numerics.LinearAlgebra.Matrix<double> strainRateGrain = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense<double>(3, 3, 0); ;
                                    for (int i = 0; i < 3; i++)
                                    {
                                        for (int j = 0; j < 3; j++)
                                        {
                                            for (int k = 0; k < 3; k++)
                                            {
                                                for (int l = 0; l < 3; l++)
                                                {
                                                    strainRateGrain[i, j] += transformationMatrix[i, k] * transformationMatrix[j, l] * strainRateGrainComp[k, l];
                                                }
                                            }
                                        }
                                    }

                                    //Berechnung der Yieldstressänderung
                                    //Mc wird aus dem Inversen von Lc berechnet
                                    //Tools.FourthRankTensor Mc = instGrainTensor.InverseSC();
                                    MathNet.Numerics.LinearAlgebra.Matrix<double> stressRateGrain = instGrainTensor * strainRateGrain;
                                    //MathNet.Numerics.LinearAlgebra.Matrix<double> strainRateGrain = Mc * stressRateGrain;

                                    //Speichern der Parameter
                                    grainInstStiffnessesOriented.Add(instGrainTensor);
                                    grainTransitionStiffnessesOriented.Add(transitionGrainTensor);
                                    potentialActiveGPhase.Add(new List<ReflexYield>());
                                    hardeningMatrixOriented.Add(hardeningMatrix);
                                    grainStrainsPhase.Add(strainRateGrain);
                                    grainStressesPhase.Add(stressRateGrain);
                                    yieldChangePhase.Add(new List<double>());
                                }

                                grainIndexCounter++;
                            }
                        }
                    }
                    //Überschreiben der Phasendaten
                    grainStiffnesses[phase] = grainInstStiffnessesOriented;
                    grainTransitionStiffnesses[phase] = grainTransitionStiffnessesOriented;
                    potentialActiveGOriented[phase] = potentialActiveGPhase;
                    grainStressesOriented[phase] = grainStressesPhase;
                    grainStrainsOriented[phase] = grainStrainsPhase;
                    hardeningMatrixList[phase] = hardeningMatrixOriented;
                    yieldChangeOriented[phase] = yieldChangePhase;

                    //Mittelung des neuen Phasenmoduls
                    if (textureActive)
                    {

                    }
                    else
                    {
                        //for(int orientation = 0; orientation < actSample.SimulationData[experimentIndex].GrainOrientations[phase].Count; orientation++)
                        //{
                        //    double phi1Angle = actSample.SimulationData[experimentIndex].GrainOrientations[phase][orientation].Phi1 * (Math.PI / 180);
                        //    double psiAngle = actSample.SimulationData[experimentIndex].GrainOrientations[phase][orientation].Psi * (Math.PI / 180);
                        //    double phi2Angle = actSample.SimulationData[experimentIndex].GrainOrientations[phase][orientation].Phi2 * (Math.PI / 180);
                        //    overallStiffnessesPhase[phase] += Tools.FourthRankTensor.InnerProduct(grainTransitionStiffnessesOriented[orientation].FrameTransformation(phi1Angle, psiAngle, phi2Angle), grainInstStiffnessesOriented[orientation].FrameTransformation(phi1Angle, psiAngle, phi2Angle));
                        //}
                        //overallStiffnessesPhase[phase] /= actSample.SimulationData[experimentIndex].GrainOrientations[phase].Count;
                        overallStiffnessesPhase[phase] = Tools.FourthRankTensor.AverageInnerProduct(grainTransitionStiffnessesOriented, grainInstStiffnessesOriented);
                        overallStiffnessesPhase[phase].SetHexagonalSymmetryCorrection();
                    }

                    //Mittelung der neuen Fließgrenze PotentialSlipSystems
                    averageYieldChange[phase] = new double[actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems.Count];

                    for (int i = 0; i < potentialActiveGOriented[phase].Count; i++)
                    {
                        for (int j = 0; j < potentialActiveGOriented[phase][i].Count; j++)
                        {
                            for (int k = 0; k < actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems.Count; k++)
                            {
                                if (potentialActiveGOriented[phase][i][j].HKLString == actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[k].HKLString && potentialActiveGOriented[phase][i][j].HKLStringSlipDirection == actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[k].HKLStringSlipDirection)
                                {
                                    //ODO: hier aufpassen wegen des Betrags
                                    averageYieldChange[phase][k] += Math.Abs(yieldChangeOriented[phase][i][j]);
                                }
                            }
                        }
                    }
                    //verlegt ans ende, weil sonst auch bei jedem Cycle härtung berechnet wird
                    //for (int k = 0; k < actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems.Count; k++)
                    //{
                    //    actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[k].YieldMainHardennedStrength += averageYieldChange[k] / potentialActiveGOriented[phase].Count;
                    //}

                }

                //Ab hier berechung der Makrodaten
                //Resetting the overall Stiffnesses

                overallStiffnesses = new Tools.FourthRankTensor();
                for (int phase = 0; phase < actSample.CrystalData.Count; phase++)
                {
                    overallStiffnesses += actSample.CrystalData[phase].PhaseFraction * overallStiffnessesPhase[phase];
                }

                double overallDifference = overallStiffnesses.GetDifference(overallStiffnessesComp);
                MathNet.Numerics.LinearAlgebra.Matrix<double> testTensor1 = overallStiffnesses.GetVoigtTensor();

                if (overallDifference < 100)
                {
                    break;
                }
                else
                {
                    overallStiffnessesComp = overallStiffnesses.Clone() as Tools.FourthRankTensor;
                }
            }

            //Abspeichern der Daten
            //All base tensor for the Grains, sample tensor are set
            //the stress states for the sample and phases will be calculated
            //Parameter Reihenfolge:
            //1. Spannung Probe (Berechnet aus der Spannungssrate der Probe)
            //2. Spannungsrate Probe (berechnet)
            //3. Dehnungsrate Probe (über overallStiffnesses gerechnet)
            //4. Dehnung Probe (Eingabe)
            //5. Bestimmung der Phasenparameter
            //Alles auf grain lvl wird nach des Konvergenz checks berechnet und gesetzt
            //6. Dehnungsraten der grains (berechnet über die transition Matrix)
            //7. Dehnung der grains (berechnet über die Dehnrate)
            //8. Berechnung der Spannungsrate im Grain (Dehnrate aus 6. - Plastischer Dehnrate --> Elastische konstanten)
            //9. Berechung der Aktuellen Spannung analog zu 7.
            //10. Aktive Gleitsysteme

            // 1
            actSample.SimulationData[experimentIndex].StressSFHistory.Add(stressS.Clone());
            //2. 
            actSample.SimulationData[experimentIndex].StressRateSFHistory.Add(stressRateS.Clone());

            //3.
            MathNet.Numerics.LinearAlgebra.Matrix<double> strainRateSample = strainRateS;
            actSample.SimulationData[experimentIndex].StrainRateSFHistory.Add(strainRateSample);

            //4.
            //MathNet.Numerics.LinearAlgebra.Matrix<double> actStrainS = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
            //if (actSample.SimulationData[experimentIndex].StrainSFHistory.Count == 0)
            //{
            //    actStrainS = strainRateSample;
            //}
            //else
            //{
            //    actStrainS = actSample.SimulationData[experimentIndex].StrainSFHistory[actSample.SimulationData[experimentIndex].StrainSFHistory.Count - 1] + strainRateSample;
            //}
            //actSample.SimulationData[experimentIndex].StrainSFHistory.Add(actStrainS.Clone());

            //5.
            for (int phase = 0; phase < actSample.CrystalData.Count; phase++)
            {
                ////8.
                actSample.SimulationData[experimentIndex].StressRateCFOrientedHistory[phase].Add(grainStressesOriented[phase]);
                ////6.
                actSample.SimulationData[experimentIndex].StrainRateCFOrientedHistory[phase].Add(grainStrainsOriented[phase]);
                ////10.
                actSample.SimulationData[experimentIndex].ActiveSystemsCFOrientedHistory[phase].Add(potentialActiveGOriented[phase]);
                ////11. Härtung berechnen
                for (int k = 0; k < actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems.Count; k++)
                {
                    actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[k].YieldMainHardennedStrength += averageYieldChange[phase][k] / potentialActiveGOriented[phase].Count;
                }
            }

            //if (correctArea)
            //{
            //    double radius = Math.Sqrt(actSample.SimulationData[experimentIndex].SampleArea / Math.PI);

            //    double ellipseA = radius * (1 + actStrainS[0, 0]);
            //    double ellipseB = radius * (1 + actStrainS[1, 1]);

            //    double newArea = Math.PI * ellipseA * ellipseB;
            //    double appliedForce = actSample.SimulationData[experimentIndex].StressSFHistory[n][2, 2] * actSample.SimulationData[experimentIndex].SampleArea;

            //    MathNet.Numerics.LinearAlgebra.Matrix<double> newStressTensor = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense<double>(3, 3, 0);
            //    newStressTensor[2, 2] = appliedForce / newArea;

            //    actSample.SimulationData[experimentIndex].StressSFHistoryAreaCorrected.Add(newStressTensor);
            //}

        }

        //Actual model now in use --> BackUp
        public static void PerformStressExperimentBackUp(Sample actSample, int experimentIndex, int n, int cycleLimit, bool textureActive, bool singleCrystalTracking, int slipCriterion, int elasticModel)
        {
            // Parameter definition
            //Potentiel aktivierte Gleitsysteme in 
            List<List<List<ReflexYield>>> potentialActiveGOriented = new List<List<List<ReflexYield>>>();
            //Stress rate for the sample
            MathNet.Numerics.LinearAlgebra.Matrix<double> stressRateS = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
            //Dehnungstensoren Tensoren guess ist einfach elastisch
            Tools.FourthRankTensor overallStiffnesses = actSample.GetSampleStiffnesses(textureActive);
            switch (elasticModel)
            {
                case 0:
                    //Reuss
                    overallStiffnesses = actSample.GetSampleStiffnesses(textureActive, actSample.ReussTensorData);
                    break;
                case 1:
                    //Hill
                    overallStiffnesses = actSample.GetSampleStiffnesses(textureActive, actSample.HillTensorData);
                    break;
                case 2:
                    //Kroener
                    overallStiffnesses = actSample.GetSampleStiffnesses(textureActive, actSample.KroenerTensorData);
                    break;
                case 3:
                    //De Wit
                    overallStiffnesses = actSample.GetSampleStiffnesses(textureActive, actSample.DeWittTensorData);
                    break;
                case 4:
                    //Matthies
                    overallStiffnesses = actSample.GetSampleStiffnesses(textureActive, actSample.GeometricHillTensorData);
                    break;
                default:
                    //Hill
                    overallStiffnesses = actSample.GetSampleStiffnesses(textureActive, actSample.HillTensorData);
                    break;
            }
            Tools.FourthRankTensor overallStiffnessesComp = overallStiffnesses.Clone() as Tools.FourthRankTensor;
            //Constraint Tensor L* Komplettes Sample
            Tools.FourthRankTensor constraintStiffness = new Tools.FourthRankTensor();

            ////Lc
            //List<List<Tools.FourthRankTensor>> grainStiffnesses = new List<List<Tools.FourthRankTensor>>();
            ////Ac
            //List<List<Tools.FourthRankTensor>> grainTransitionStiffnesses = new List<List<Tools.FourthRankTensor>>();
            //Lc, Mc, Ac, Bc, Hardenning matrix
            List<List<PlasticityTensor>> plasticTensorPhase = new List<List<PlasticityTensor>>();

            //gemittelten Phasen Nachgiebigkeiten
            List<Tools.FourthRankTensor> overallStiffnessesPhase = new List<Tools.FourthRankTensor>();
            //Grain Spannungen der Phasen und Orientierungen
            List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>> grainStressesOriented = new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>();
            //Grain Dehnungen der Phasen und Orientierungen
            List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>> grainStrainsOriented = new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>();
            //List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>> hardeningMatrixList = new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>();

            List<List<List<double>>> yieldChangeOriented = new List<List<List<double>>>();
            List<List<List<double>>> shearRate = new List<List<List<double>>>();
            List<double[]> averageYieldChange = new List<double[]>();

            //Phasen werden in den Listen definiert
            for (int phase = 0; phase < actSample.CrystalData.Count; phase++)
            {
                switch (elasticModel)
                {
                    case 0:
                        //Reuss
                        overallStiffnessesPhase.Add(actSample.ReussTensorData[phase].GetFourtRankStiffnesses());
                        break;
                    case 1:
                        //Hill
                        overallStiffnessesPhase.Add(actSample.HillTensorData[phase].GetFourtRankStiffnesses());
                        break;
                    case 2:
                        //Kroener
                        overallStiffnessesPhase.Add(actSample.KroenerTensorData[phase].GetFourtRankStiffnesses());
                        break;
                    case 3:
                        //De Wit
                        overallStiffnessesPhase.Add(actSample.DeWittTensorData[phase].GetFourtRankStiffnesses());
                        break;
                    case 4:
                        //Matthies
                        overallStiffnessesPhase.Add(actSample.GeometricHillTensorData[phase].GetFourtRankStiffnesses());
                        break;
                    default:
                        //Hill
                        overallStiffnessesPhase.Add(actSample.HillTensorData[phase].GetFourtRankStiffnesses());
                        break;
                }
                potentialActiveGOriented.Add(new List<List<ReflexYield>>());
                grainStressesOriented.Add(new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>());
                grainStrainsOriented.Add(new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>());
                //hardeningMatrixList.Add(new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>());
                shearRate.Add(new List<List<double>>());
                yieldChangeOriented.Add(new List<List<double>>());
                averageYieldChange.Add(new double[5]);
                plasticTensorPhase.Add(new List<PlasticityTensor>());
                //grainStiffnesses.Add(new List<Tools.FourthRankTensor>());
                //grainTransitionStiffnesses.Add(new List<Tools.FourthRankTensor>());
            }

            //Macroskopische Spannungsrate berechnen
            if (n == 0)
            {
                stressRateS[0, 0] += actSample.SimulationData[experimentIndex].StressSFHistory[n][0, 0];
                stressRateS[1, 0] += actSample.SimulationData[experimentIndex].StressSFHistory[n][1, 0];
                stressRateS[2, 0] += actSample.SimulationData[experimentIndex].StressSFHistory[n][2, 0];
                stressRateS[0, 1] += actSample.SimulationData[experimentIndex].StressSFHistory[n][0, 1];
                stressRateS[1, 1] += actSample.SimulationData[experimentIndex].StressSFHistory[n][1, 1];
                stressRateS[2, 1] += actSample.SimulationData[experimentIndex].StressSFHistory[n][2, 1];
                stressRateS[0, 2] += actSample.SimulationData[experimentIndex].StressSFHistory[n][0, 2];
                stressRateS[1, 2] += actSample.SimulationData[experimentIndex].StressSFHistory[n][1, 2];
                stressRateS[2, 2] += actSample.SimulationData[experimentIndex].StressSFHistory[n][2, 2];
            }
            else
            {
                stressRateS[0, 0] += actSample.SimulationData[experimentIndex].StressSFHistory[n][0, 0] - actSample.SimulationData[experimentIndex].StressSFHistory[n - 1][0, 0];
                stressRateS[1, 0] += actSample.SimulationData[experimentIndex].StressSFHistory[n][1, 0] - actSample.SimulationData[experimentIndex].StressSFHistory[n - 1][1, 0];
                stressRateS[2, 0] += actSample.SimulationData[experimentIndex].StressSFHistory[n][2, 0] - actSample.SimulationData[experimentIndex].StressSFHistory[n - 1][2, 0];
                stressRateS[0, 1] += actSample.SimulationData[experimentIndex].StressSFHistory[n][0, 1] - actSample.SimulationData[experimentIndex].StressSFHistory[n - 1][0, 1];
                stressRateS[1, 1] += actSample.SimulationData[experimentIndex].StressSFHistory[n][1, 1] - actSample.SimulationData[experimentIndex].StressSFHistory[n - 1][1, 1];
                stressRateS[2, 1] += actSample.SimulationData[experimentIndex].StressSFHistory[n][2, 1] - actSample.SimulationData[experimentIndex].StressSFHistory[n - 1][2, 1];
                stressRateS[0, 2] += actSample.SimulationData[experimentIndex].StressSFHistory[n][0, 2] - actSample.SimulationData[experimentIndex].StressSFHistory[n - 1][0, 2];
                stressRateS[1, 2] += actSample.SimulationData[experimentIndex].StressSFHistory[n][1, 2] - actSample.SimulationData[experimentIndex].StressSFHistory[n - 1][1, 2];
                stressRateS[2, 2] += actSample.SimulationData[experimentIndex].StressSFHistory[n][2, 2] - actSample.SimulationData[experimentIndex].StressSFHistory[n - 1][2, 2];
            }

            //MathNet.Numerics.LinearAlgebra.Matrix<double> stressRateS = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
            //MathNet.Numerics.LinearAlgebra.Matrix<double> stressS = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);


            //Selbstkonsistente Rechung
            for (int actCycle = 0; actCycle < cycleLimit; actCycle++)
            {
                bool allElastic = true;
                actSample.SimulationData[experimentIndex].LastActiveSystems = 0;

                Microsopic.ElasticityTensors eTmp = new Microsopic.ElasticityTensors();
                eTmp._stiffnessTensor = overallStiffnesses.GetVoigtTensor();
                eTmp.CalculateCompliances();
                constraintStiffness = actSample.PlasticTensor[0].GetConstraintStiffnessCubicIsotropic(eTmp, 2);
                actSample.SimulationData[experimentIndex].LastFailedGrains = 0;

                //Berechnungen im Krystallsystem
                for (int phase = 0; phase < actSample.CrystalData.Count; phase++)
                {
                    int grainIndexCounter = actSample.SimulationData[experimentIndex].GrainOrientations[phase].Count;
                    //Lc, Mc, Ac, Bc der aktuellen Phase
                    PlasticityTensor[] plasticTensororiented = new PlasticityTensor[grainIndexCounter];
                    //GrainSpanungen für alle orientierungen
                    MathNet.Numerics.LinearAlgebra.Matrix<double>[] grainStressesPhase = new MathNet.Numerics.LinearAlgebra.Matrix<double>[grainIndexCounter];
                    //Grain Dehnungen für alle orientierungen
                    MathNet.Numerics.LinearAlgebra.Matrix<double>[] grainStrainsPhase = new MathNet.Numerics.LinearAlgebra.Matrix<double>[grainIndexCounter];
                    //Active Gleitsysteme
                    List<ReflexYield>[] potentialActiveGPhase = new List<ReflexYield>[grainIndexCounter];
                    List<double>[] yieldChangePhase = new List<double>[grainIndexCounter];
                    List<double>[] shearRateOriented = new List<double>[grainIndexCounter];

                    // Parallelize the outer loop to partition the source array by rows.
                    //Parallel.For(0, grainIndexCounter, grainIndexTmp =>
                    for (int grainIndexTmp = 0; grainIndexTmp < grainIndexCounter; grainIndexTmp++)
                    {
                        int grainIndex = Convert.ToInt32(grainIndexTmp);
                        PlasticityTensor plasticTensorGrain = new PlasticityTensor();

                        if (grainIndex == 1000)
                        {
                            string test1 = "";
                        }
                        if (grainIndex == 2000)
                        {
                            string test1 = "";
                        }
                        //if (grainIndex == 632)
                        //{
                        //    string test1 = "";
                        //}
                        //if (grainIndex == 633)
                        //{
                        //    string test1 = "";
                        //}

                        if (singleCrystalTracking)
                        {
                            //Yield strength wird berechnet
                            List<double> hardennedYield = new List<double>();
                            for (int pS = 0; pS < actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems.Count; pS++)
                            {
                                hardennedYield.Add(0.0);
                                actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[pS].YieldMainHardennedStrength = actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[pS].YieldMainStrength;
                            }

                            if (actCycle != 0)
                            {
                                for (int k = 0; k < actSample.SimulationData[experimentIndex].YieldChangeCFHistory.Count; k++)
                                {
                                    for (int aS = 0; aS < actSample.SimulationData[experimentIndex].ActiveSystemsCFOrientedHistory[phase][k][grainIndex].Count; aS++)
                                    {
                                        for (int pS = 0; pS < actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems.Count; pS++)
                                        {
                                            if (actSample.SimulationData[experimentIndex].ActiveSystemsCFOrientedHistory[phase][k][grainIndex][aS].HKLString == actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[pS].HKLString)
                                            {
                                                //Vorfaktorberechnung für die Härtung
                                                if (actSample.SimulationData[experimentIndex].useYieldLimit)
                                                {
                                                    double hardeningSaturation = 1;
                                                    hardeningSaturation -= (actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[pS].YieldMainStrength + hardennedYield[pS]) / actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[pS].YieldLimit;
                                                    //[k] und [phase] sind mit absicht vertauscht
                                                    hardennedYield[pS] += hardeningSaturation * actSample.SimulationData[experimentIndex].YieldChangeCFHistory[k][phase][grainIndex][aS];
                                                }
                                                else
                                                {
                                                    //[k] und [phase] sind mit absicht vertauscht
                                                    hardennedYield[pS] += actSample.SimulationData[experimentIndex].YieldChangeCFHistory[k][phase][grainIndex][aS];
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            for (int pS = 0; pS < actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems.Count; pS++)
                            {
                                actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[pS].YieldMainHardennedStrength += Math.Abs(hardennedYield[pS]);

                                //if (actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[pS].YieldMainHardennedStrength > actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[pS].YieldLimit)
                                //{
                                //    actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[pS].YieldMainHardennedStrength = actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[pS].YieldLimit;
                                //}
                            }

                        }

                        //Drehen des Spannungstensor in die aktuelle Orientierung
                        MathNet.Numerics.LinearAlgebra.Matrix<double> transformationMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense<double>(3, 3, 0);

                        transformationMatrix[0, 0] = -1 * Math.Cos(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi1 * (Math.PI / 180)) * Math.Cos(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Psi * (Math.PI / 180)) * Math.Sin(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi2 * (Math.PI / 180));
                        transformationMatrix[0, 0] -= Math.Sin(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi1 * (Math.PI / 180)) * Math.Cos(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi2 * (Math.PI / 180));
                        transformationMatrix[0, 1] = -1 * Math.Cos(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Psi * (Math.PI / 180)) * Math.Sin(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi1 * (Math.PI / 180)) * Math.Sin(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi2 * (Math.PI / 180));
                        transformationMatrix[0, 1] -= Math.Cos(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi1 * (Math.PI / 180)) * Math.Cos(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi2 * (Math.PI / 180));
                        transformationMatrix[0, 2] = Math.Sin(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Psi * (Math.PI / 180)) * Math.Sin(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi2 * (Math.PI / 180));
                        transformationMatrix[1, 0] = -1 * Math.Cos(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Psi * (Math.PI / 180)) * Math.Cos(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi1 * (Math.PI / 180)) * Math.Cos(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi2 * (Math.PI / 180));
                        transformationMatrix[1, 0] -= Math.Sin(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi1 * (Math.PI / 180)) * Math.Sin(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi2 * (Math.PI / 180));
                        transformationMatrix[1, 1] = -1 * Math.Sin(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi1 * (Math.PI / 180)) * Math.Cos(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Psi * (Math.PI / 180)) * Math.Cos(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi2 * (Math.PI / 180));
                        transformationMatrix[1, 1] -= Math.Cos(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi1 * (Math.PI / 180)) * Math.Sin(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi2 * (Math.PI / 180));
                        transformationMatrix[1, 2] = Math.Sin(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Psi * (Math.PI / 180)) * Math.Cos(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi2 * (Math.PI / 180));
                        transformationMatrix[2, 0] = Math.Cos(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi1 * (Math.PI / 180)) * Math.Sin(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Psi * (Math.PI / 180));
                        transformationMatrix[2, 1] = Math.Sin(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi1 * (Math.PI / 180)) * Math.Sin(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Psi * (Math.PI / 180));
                        transformationMatrix[2, 2] = Math.Cos(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Psi * (Math.PI / 180));

                        //for the self consistence the stress and stress rate are calculated from the last Bc if it exists
                        MathNet.Numerics.LinearAlgebra.Matrix<double> actStressGrainBc = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
                        MathNet.Numerics.LinearAlgebra.Matrix<double> actStressRateGrainBc = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

                        if (actCycle == 0)
                        {
                            for (int k = 0; k < n; k++)
                            {
                                actStressGrainBc += actSample.SimulationData[experimentIndex].StressRateCFOrientedHistory[phase][k][grainIndex];
                            }
                            actStressRateGrainBc = stressRateS;
                        }
                        else
                        {
                            ///constraintStiffness ergibt sich für alle Phasen gleichermaßen und wird aktualisiert
                            ///instGrainTensor wird aus Orientierungen berechnet und muss aus vorherigen Schritt genommen werden
                            ///overallStiffnesses wird pro cycle aktualisiert
                            Tools.FourthRankTensor lastBc = actSample.PlasticTensor[phase].GetAc(constraintStiffness.InverseSC(), plasticTensorPhase[phase][grainIndex].GrainCompliance, overallStiffnesses.InverseSC());

                            //actStressGrainBc = lastBc * actSample.SimulationData[experimentIndex].StressSFHistory[n];
                            for (int k = 0; k < n; k++)
                            {
                                actStressGrainBc += actSample.SimulationData[experimentIndex].StressRateCFOrientedHistory[phase][k][grainIndex];
                            }
                            actStressRateGrainBc = lastBc * stressRateS;
                        }

                        //Spannungstensor in der aktuellen Orientierung
                        MathNet.Numerics.LinearAlgebra.Matrix<double> actStressOriented = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
                        MathNet.Numerics.LinearAlgebra.Matrix<double> actStressRateOriented = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
                        for (int i = 0; i < 3; i++)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                for (int k = 0; k < 3; k++)
                                {
                                    for (int l = 0; l < 3; l++)
                                    {
                                        //actStressOriented[i, j] += transformationMatrix[i, k] * transformationMatrix[j, l] * actStressGrainBc[k, l];
                                        actStressRateOriented[i, j] += transformationMatrix[i, k] * transformationMatrix[j, l] * actStressRateGrainBc[k, l];
                                    }
                                }
                            }
                        }
                        actStressOriented = actStressGrainBc + actStressRateOriented;


                        List<double> potentialActiveStress = actSample.SimulationData[experimentIndex].YieldInformation[phase].GetResolvedStressList(actStressOriented);

                        //Potentiell aktive Gleitsysteme werden ermittelt
                        List<ReflexYield> potentialActive = actSample.SimulationData[experimentIndex].YieldInformation[phase].GetPotentiallyActiveSlipSystems(actStressOriented, slipCriterion);
                        bool noValidCombination = true;

                        if (potentialActive.Count != 0)
                        {
                            noValidCombination = false;
                            allElastic = false;
                            //Liste mit den Testsystemen wird erstell und Indiziert
                            List<ReflexYield> trialSystems = potentialActive;
                            List<int> checkedSystems = new List<int>();
                            for (int i = 0; i < trialSystems.Count; i++)
                            {
                                checkedSystems.Add(0);
                            }

                            //Filterung von Abhängigkeiten
                            actSample.SimulationData[experimentIndex].YieldInformation[phase].CheckDependencies(potentialActive, actStressOriented, checkedSystems);

                            //Schleife für die Finale Selection der Gleitsysteme
                            while (trialSystems.Count != 0)
                            {
                                #region Direction Switch

                                //Check ob die Scherraten positiv sind
                                List<int> checkedSystemsRe = new List<int>();
                                trialSystems = GetActiveSystemCombination(potentialActive, checkedSystems);
                                if (trialSystems.Count == 0)
                                {
                                    noValidCombination = true;
                                    break;
                                }
                                //bool resetSystems = false;
                                //for (int m = 0; m < checkedSystems.Count; m++)
                                //{
                                //    switch (checkedSystems[m])
                                //    {
                                //        case 0:
                                //            goto default;
                                //        case 1:
                                //            checkedSystemsRe.Add(1);
                                //            break;
                                //        case 2:
                                //            checkedSystemsRe.Add(2);
                                //            break;
                                //        default:
                                //            resetSystems = true;
                                //            break;
                                //    }
                                //}
                                //checkedSystems = checkedSystemsRe;

                                //if (resetSystems)
                                //{
                                //    for (int m = 0; m < checkedSystems.Count; m++)
                                //    {
                                //        checkedSystems[m] = 1;
                                //    }
                                //}

                                #endregion

                                //Härtungsmatrix wird berechnet
                                plasticTensorGrain.HardeningMatrix = actSample.SimulationData[experimentIndex].YieldInformation[phase].HardeningMatrixSlipSystem(trialSystems);

                                //Shear rates
                                List<double> shearRatesSystems = new List<double>();
                                //shear stress change rates
                                List<double> shearStressChangeSystems = new List<double>();
                                //yield change rates
                                List<double> yieldChangeSystems = new List<double>();

                                //Berechnung der instantanious Stiffness factors f^i
                                switch (elasticModel)
                                {
                                    case 0:
                                        //Reuss
                                        plasticTensorGrain.ConditionX = actSample.SimulationData[experimentIndex].YieldInformation[phase].SlipSystemX(trialSystems, plasticTensorGrain.HardeningMatrix, actSample.ReussTensorData[phase]);
                                        for (int i = 0; i < trialSystems.Count; i++)
                                        {
                                            plasticTensorGrain.InstantStiffnessFactors.Add(actSample.SimulationData[experimentIndex].YieldInformation[phase].GetInstStiffnessFactors(trialSystems, actSample.ReussTensorData[phase], i, plasticTensorGrain.ConditionY));
                                        }
                                        //Lc und Mc 
                                        plasticTensorGrain.GrainStiffness = actSample.PlasticTensor[phase].GetInstantanousGrainSiffnessTensor(trialSystems, actSample.ReussTensorData[phase], plasticTensorGrain.InstantStiffnessFactors);
                                        break;
                                    case 1:
                                        //Hill
                                        plasticTensorGrain.ConditionX = actSample.SimulationData[experimentIndex].YieldInformation[phase].SlipSystemX(trialSystems, plasticTensorGrain.HardeningMatrix, actSample.HillTensorData[phase]);
                                        for (int i = 0; i < trialSystems.Count; i++)
                                        {
                                            plasticTensorGrain.InstantStiffnessFactors.Add(actSample.SimulationData[experimentIndex].YieldInformation[phase].GetInstStiffnessFactors(trialSystems, actSample.HillTensorData[phase], i, plasticTensorGrain.ConditionY));
                                        }
                                        //Lc und Mc 
                                        plasticTensorGrain.GrainStiffness = actSample.PlasticTensor[phase].GetInstantanousGrainSiffnessTensor(trialSystems, actSample.HillTensorData[phase], plasticTensorGrain.InstantStiffnessFactors);
                                        break;
                                    case 2:
                                        //Kroener
                                        plasticTensorGrain.ConditionX = actSample.SimulationData[experimentIndex].YieldInformation[phase].SlipSystemX(trialSystems, plasticTensorGrain.HardeningMatrix, actSample.KroenerTensorData[phase]);
                                        for (int i = 0; i < trialSystems.Count; i++)
                                        {
                                            plasticTensorGrain.InstantStiffnessFactors.Add(actSample.SimulationData[experimentIndex].YieldInformation[phase].GetInstStiffnessFactors(trialSystems, actSample.KroenerTensorData[phase], i, plasticTensorGrain.ConditionY));
                                        }
                                        //Lc und Mc 
                                        plasticTensorGrain.GrainStiffness = actSample.PlasticTensor[phase].GetInstantanousGrainSiffnessTensor(trialSystems, actSample.KroenerTensorData[phase], plasticTensorGrain.InstantStiffnessFactors);
                                        break;
                                    case 3:
                                        //De Wit
                                        plasticTensorGrain.ConditionX = actSample.SimulationData[experimentIndex].YieldInformation[phase].SlipSystemX(trialSystems, plasticTensorGrain.HardeningMatrix, actSample.DeWittTensorData[phase]);
                                        for (int i = 0; i < trialSystems.Count; i++)
                                        {
                                            plasticTensorGrain.InstantStiffnessFactors.Add(actSample.SimulationData[experimentIndex].YieldInformation[phase].GetInstStiffnessFactors(trialSystems, actSample.DeWittTensorData[phase], i, plasticTensorGrain.ConditionY));
                                        }
                                        //Lc und Mc 
                                        plasticTensorGrain.GrainStiffness = actSample.PlasticTensor[phase].GetInstantanousGrainSiffnessTensor(trialSystems, actSample.DeWittTensorData[phase], plasticTensorGrain.InstantStiffnessFactors);
                                        break;
                                    case 4:
                                        //Matthies
                                        plasticTensorGrain.ConditionX = actSample.SimulationData[experimentIndex].YieldInformation[phase].SlipSystemX(trialSystems, plasticTensorGrain.HardeningMatrix, actSample.GeometricHillTensorData[phase]);
                                        for (int i = 0; i < trialSystems.Count; i++)
                                        {
                                            plasticTensorGrain.InstantStiffnessFactors.Add(actSample.SimulationData[experimentIndex].YieldInformation[phase].GetInstStiffnessFactors(trialSystems, actSample.GeometricHillTensorData[phase], i, plasticTensorGrain.ConditionY));
                                        }
                                        //Lc und Mc 
                                        plasticTensorGrain.GrainStiffness = actSample.PlasticTensor[phase].GetInstantanousGrainSiffnessTensor(trialSystems, actSample.GeometricHillTensorData[phase], plasticTensorGrain.InstantStiffnessFactors);
                                        break;
                                    default:
                                        //Hill
                                        plasticTensorGrain.ConditionX = actSample.SimulationData[experimentIndex].YieldInformation[phase].SlipSystemX(trialSystems, plasticTensorGrain.HardeningMatrix, actSample.HillTensorData[phase]);
                                        for (int i = 0; i < trialSystems.Count; i++)
                                        {
                                            plasticTensorGrain.InstantStiffnessFactors.Add(actSample.SimulationData[experimentIndex].YieldInformation[phase].GetInstStiffnessFactors(trialSystems, actSample.HillTensorData[phase], i, plasticTensorGrain.ConditionY));
                                        }
                                        //Lc und Mc 
                                        plasticTensorGrain.GrainStiffness = actSample.PlasticTensor[phase].GetInstantanousGrainSiffnessTensor(trialSystems, actSample.HillTensorData[phase], plasticTensorGrain.InstantStiffnessFactors);
                                        break;
                                }


                                //InversionCheck, ob alles richtig Invertiert wurde
                                Tools.FourthRankTensor invCheckTensor = Tools.FourthRankTensor.InnerProduct(plasticTensorGrain.GrainStiffness, plasticTensorGrain.GrainCompliance);
                                double invDifference = invCheckTensor.GetDifference(Tools.FourthRankTensor.GetUnityTensor());
                                if (invDifference < 0.0001)
                                {
                                    //berechung von Ac
                                    plasticTensorGrain.GrainTransitionStiffness = actSample.PlasticTensor[phase].GetAc(constraintStiffness, plasticTensorGrain.GrainStiffness, overallStiffnesses);
                                    //bzw Bc
                                    plasticTensorGrain.GrainTransitionCompliance = actSample.PlasticTensor[phase].GetAc(constraintStiffness.InverseSC(), plasticTensorGrain.GrainCompliance, overallStiffnesses.InverseSC());

                                    MathNet.Numerics.LinearAlgebra.Matrix<double> stressRateGrainNo = plasticTensorGrain.GrainTransitionCompliance * stressRateS;
                                    //MathNet.Numerics.LinearAlgebra.Matrix<double> stressRateGrain = plasticTensorGrain.GrainTransitionCompliance * actStressOriented;
                                    MathNet.Numerics.LinearAlgebra.Matrix<double> stressRateGrain = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense<double>(3, 3, 0); ;
                                    for (int i = 0; i < 3; i++)
                                    {
                                        for (int j = 0; j < 3; j++)
                                        {
                                            for (int k = 0; k < 3; k++)
                                            {
                                                for (int l = 0; l < 3; l++)
                                                {
                                                    stressRateGrain[i, j] += transformationMatrix[i, k] * transformationMatrix[j, l] * stressRateGrainNo[k, l];
                                                }
                                            }
                                        }
                                    }

                                    MathNet.Numerics.LinearAlgebra.Matrix<double> strainRateGrain = plasticTensorGrain.GrainCompliance * stressRateGrain;

                                    //Berechnung der shear rates und der spannungsänderung
                                    for (int i = 0; i < trialSystems.Count; i++)
                                    {
                                        double actShearRate = 0.0;
                                        double shearstress = 0.0;
                                        MathNet.Numerics.LinearAlgebra.Matrix<double> resolvingMatrix = Tools.Calculation.GetResolvingParameter(trialSystems[i].SlipPlane, trialSystems[i].MainSlipDirection);

                                        for (int j = 0; j < 3; j++)
                                        {
                                            for (int k = 0; k < 3; k++)
                                            {
                                                actShearRate += plasticTensorGrain.InstantStiffnessFactors[i][j, k] * strainRateGrain[j, k];
                                                shearstress += stressRateGrain[j, k] * resolvingMatrix[j, k];
                                            }
                                        }

                                        shearRatesSystems.Add(actShearRate);
                                        shearStressChangeSystems.Add(shearstress);
                                    }

                                    bool negativeShear = false;

                                    #region direction check

                                    ////Checkt ob die Scherraten positiv sind und markiert diejenigen, deren Gleitrichtung umgedreht werden muss
                                    //for (int sys = 0; sys < shearRatesSystems.Count; sys++)
                                    //{
                                    //    //Check ob die scherraten positiv oder negativ sind
                                    //    if (shearRatesSystems[sys] < 0)
                                    //    {
                                    //        //Falls negativ wird die Gleitrichtung umgedreht
                                    //        if (checkedSystems[sys] == 1)
                                    //        {
                                    //            checkedSystems[sys] = 2;
                                    //        }
                                    //        else if (checkedSystems[sys] == 2)
                                    //        {
                                    //            checkedSystems[sys] = 0;
                                    //        }
                                    //        negativeShear = true;
                                    //    }
                                    //}

                                    #endregion

                                    if (!negativeShear)
                                    {
                                        //Berechung der yield changes
                                        for (int i = 0; i < trialSystems.Count; i++)
                                        {
                                            double yieldChangeTmp = 0.0;
                                            for (int j = 0; j < trialSystems.Count; j++)
                                            {
                                                yieldChangeTmp += plasticTensorGrain.HardeningMatrix[i, j] * shearRatesSystems[j];
                                            }
                                            yieldChangeSystems.Add(yieldChangeTmp);
                                        }

                                        //Vergleich zwischen shear rates und yield changes
                                        double difference = 0.0;
                                        double norm = 0.0;
                                        for (int i = 0; i < trialSystems.Count; i++)
                                        {
                                            difference = (Math.Abs(yieldChangeSystems[i]) - Math.Abs(shearStressChangeSystems[i])) / Math.Abs(yieldChangeSystems[i]);
                                            norm += Math.Abs(yieldChangeSystems[i]);
                                        }

                                        difference /= norm;

                                        if (difference < 1)
                                        {
                                            //Speichern der Parameter
                                            potentialActiveGPhase[grainIndex] = trialSystems;
                                            grainStrainsPhase[grainIndex] = strainRateGrain;
                                            grainStressesPhase[grainIndex] = stressRateGrain;
                                            yieldChangePhase[grainIndex] = yieldChangeSystems;

                                            plasticTensororiented[grainIndex] = plasticTensorGrain;

                                            actSample.SimulationData[experimentIndex].LastActiveSystems += trialSystems.Count;
                                            break;
                                        }
                                        else
                                        {
                                            //System mit kleinsten Shearstress wird herausgesucht
                                            int smallestIndex = actSample.SimulationData[experimentIndex].YieldInformation[phase].GetSmallesSchearStressIndex(trialSystems, actStressOriented);

                                            //System mit kleinsten Shearstress wird aussortiert
                                            for (int k = 0; k < potentialActive.Count; k++)
                                            {
                                                if (potentialActive[k].SlipPlane.HKLString == trialSystems[smallestIndex].SlipPlane.HKLString && potentialActive[k].MainSlipDirection.HKLString == trialSystems[smallestIndex].MainSlipDirection.HKLString)
                                                {
                                                    checkedSystems[k] = 3;
                                                    break;
                                                }
                                            }

                                            //Das nächste noch nicht getestete System wird aktiviert
                                            for (int k = 0; k < checkedSystems.Count; k++)
                                            {
                                                if (checkedSystems[k] == 0)
                                                {
                                                    checkedSystems[k] = 1;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    //System mit kleinsten Shearstress wird herausgesucht
                                    int smallestIndex = actSample.SimulationData[experimentIndex].YieldInformation[phase].GetSmallesSchearStressIndex(trialSystems, actStressOriented);

                                    //System mit kleinsten Shearstress wird aussortiert
                                    for (int k = 0; k < potentialActive.Count; k++)
                                    {
                                        if (potentialActive[k].SlipPlane.HKLString == trialSystems[smallestIndex].SlipPlane.HKLString && potentialActive[k].MainSlipDirection.HKLString == trialSystems[smallestIndex].MainSlipDirection.HKLString)
                                        {
                                            checkedSystems[k] = 3;
                                            break;
                                        }
                                    }

                                    //Das nächste noch nicht getestete System wird aktiviert
                                    for (int k = 0; k < checkedSystems.Count; k++)
                                    {
                                        if (checkedSystems[k] == 0)
                                        {
                                            checkedSystems[k] = 1;
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (noValidCombination)
                        {
                            actSample.SimulationData[experimentIndex].LastFailedGrains++;
                            switch (elasticModel)
                            {
                                case 0:
                                    //Reuss
                                    plasticTensorGrain.GrainStiffness = actSample.ReussTensorData[phase].GetFourtRankStiffnesses();
                                    break;
                                case 1:
                                    //Hill
                                    plasticTensorGrain.GrainStiffness = actSample.HillTensorData[phase].GetFourtRankStiffnesses();
                                    break;
                                case 2:
                                    //Kroener
                                    plasticTensorGrain.GrainStiffness = actSample.KroenerTensorData[phase].GetFourtRankStiffnesses();
                                    break;
                                case 3:
                                    //De Wit
                                    plasticTensorGrain.GrainStiffness = actSample.DeWittTensorData[phase].GetFourtRankStiffnesses();
                                    break;
                                case 4:
                                    //Matthies
                                    plasticTensorGrain.GrainStiffness = actSample.GeometricHillTensorData[phase].GetFourtRankStiffnesses();
                                    break;
                                default:
                                    //Hill
                                    plasticTensorGrain.GrainStiffness = actSample.HillTensorData[phase].GetFourtRankStiffnesses();
                                    break;
                            }

                            //berechung von Ac
                            plasticTensorGrain.GrainTransitionStiffness = actSample.PlasticTensor[phase].GetAc(constraintStiffness, plasticTensorGrain.GrainStiffness, overallStiffnesses);
                            //bzw Bc
                            plasticTensorGrain.GrainTransitionCompliance = actSample.PlasticTensor[phase].GetAc(constraintStiffness.InverseSC(), plasticTensorGrain.GrainCompliance, overallStiffnesses.InverseSC());

                            MathNet.Numerics.LinearAlgebra.Matrix<double> stressRateGrainSave = plasticTensorGrain.GrainTransitionCompliance * stressRateS;
                            MathNet.Numerics.LinearAlgebra.Matrix<double> stressRateOrientedSave = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
                            for (int i = 0; i < 3; i++)
                            {
                                for (int j = 0; j < 3; j++)
                                {
                                    for (int k = 0; k < 3; k++)
                                    {
                                        for (int l = 0; l < 3; l++)
                                        {
                                            stressRateOrientedSave[i, j] += transformationMatrix[i, k] * transformationMatrix[j, l] * stressRateGrainSave[k, l];
                                        }
                                    }
                                }
                            }
                            MathNet.Numerics.LinearAlgebra.Matrix<double> strainRateGrainSave = plasticTensorGrain.GrainCompliance * stressRateOrientedSave;

                            //Speichern der Parameter
                            potentialActiveGPhase[grainIndex] = new List<ReflexYield>();
                            grainStrainsPhase[grainIndex] = strainRateGrainSave;
                            grainStressesPhase[grainIndex] = stressRateOrientedSave;
                            yieldChangePhase[grainIndex] = new List<double>();

                            plasticTensororiented[grainIndex] = plasticTensorGrain;
                        }

                    }//); // Parallel.For

                    //Überschreiben der Phasendaten
                    plasticTensorPhase[phase] = plasticTensororiented.ToList();

                    potentialActiveGOriented[phase] = potentialActiveGPhase.ToList();
                    grainStressesOriented[phase] = grainStressesPhase.ToList();
                    grainStrainsOriented[phase] = grainStrainsPhase.ToList();
                    yieldChangeOriented[phase] = yieldChangePhase.ToList();

                    //Mittelung des neuen Phasenmoduls
                    if (textureActive)
                    {

                    }
                    else
                    {
                        overallStiffnessesPhase[phase] = Tools.FourthRankTensor.AverageInnerProduct(plasticTensorPhase[phase]);
                        overallStiffnessesPhase[phase].SetHexagonalSymmetryCorrection();
                    }

                    //Mittelung der neuen Fließgrenze PotentialSlipSystems
                    averageYieldChange[phase] = new double[actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems.Count];

                    for (int i = 0; i < potentialActiveGOriented[phase].Count; i++)
                    {
                        for (int j = 0; j < potentialActiveGOriented[phase][i].Count; j++)
                        {
                            for (int k = 0; k < actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems.Count; k++)
                            {
                                if (potentialActiveGOriented[phase][i][j].HKLString == actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[k].HKLString && potentialActiveGOriented[phase][i][j].HKLStringSlipDirection == actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[k].HKLStringSlipDirection)
                                {
                                    averageYieldChange[phase][k] += Math.Abs(yieldChangeOriented[phase][i][j]);
                                    //if (actSample.SimulationData[experimentIndex].invertSlipDirections)
                                    //{

                                    //    averageYieldChange[phase][k] += Math.Abs(yieldChangeOriented[phase][i][j]);
                                    //}
                                    //else
                                    //{
                                    //    averageYieldChange[phase][k] += yieldChangeOriented[phase][i][j];
                                    //}
                                }
                            }
                        }
                    }
                }
                //Ab hier berechung der Makrodaten
                //Resetting the overall Stiffnesses

                overallStiffnesses = new Tools.FourthRankTensor();
                for (int phase = 0; phase < actSample.CrystalData.Count; phase++)
                {
                    overallStiffnesses += actSample.CrystalData[phase].PhaseFraction * overallStiffnessesPhase[phase];
                }

                MathNet.Numerics.LinearAlgebra.Matrix<double> energyCheckMat = overallStiffnesses.GetVoigtTensor();
                bool energyCheck = true;

                //Energy check for the calculated constants
                if (energyCheckMat[3, 3] <= 0)
                {
                    energyCheck = false;
                }
                else if (energyCheckMat[0, 0] <= Math.Abs(energyCheckMat[0, 1]))
                {
                    energyCheck = false;
                }
                else if ((energyCheckMat[0, 0] + energyCheckMat[0, 1]) * energyCheckMat[2, 2] <= 0)
                {
                    energyCheck = false;
                }

                if (energyCheck)
                {
                    double overallDifference = overallStiffnesses.GetDifference(overallStiffnessesComp);

                    MathNet.Numerics.LinearAlgebra.Matrix<double> DebugComp = overallStiffnessesComp.GetVoigtTensor();
                    MathNet.Numerics.LinearAlgebra.Matrix<double> DebugDiff = DebugComp - energyCheckMat;
                    if (overallDifference < CalScec.Properties.Settings.Default.EPSCSimulationLimit)
                    {
                        break;
                    }
                    else
                    {
                        overallStiffnessesComp = overallStiffnesses.Clone() as Tools.FourthRankTensor;
                    }
                }
                else
                {
                    overallStiffnesses = overallStiffnessesComp;
                    break;
                }
            }

            //Abspeichern der Daten
            //All base tensor for the Grains, sample tensor are set
            //the stress states for the sample and phases will be calculated
            //Parameter Reihenfolge:
            //1. Spannung Probe (Berechnet aus der Spannungssrate der Probe)
            //2. Spannungsrate Probe (berechnet)
            //3. Dehnungsrate Probe (über overallStiffnesses gerechnet)
            //4. Dehnung Probe (Eingabe)
            //5. Bestimmung der Phasenparameter
            //Alles auf grain lvl wird nach des Konvergenz checks berechnet und gesetzt
            //6. Dehnungsraten der grains (berechnet über die transition Matrix)
            //7. Dehnung der grains (berechnet über die Dehnrate)
            //8. Berechnung der Spannungsrate im Grain (Dehnrate aus 6. - Plastischer Dehnrate --> Elastische konstanten)
            //9. Berechung der Aktuellen Spannung analog zu 7.
            //10. Aktive Gleitsysteme

            // 1
            //actSample.SimulationData[experimentIndex].StressSFHistory.Add(stressS.Clone());
            //2. 
            actSample.SimulationData[experimentIndex].StressRateSFHistory.Add(stressRateS.Clone());

            //3.
            MathNet.Numerics.LinearAlgebra.Matrix<double> strainRateSample = overallStiffnesses.InverseSC() * stressRateS;
            actSample.SimulationData[experimentIndex].StrainRateSFHistory.Add(strainRateSample);

            //4.
            MathNet.Numerics.LinearAlgebra.Matrix<double> actStrainS = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
            if (actSample.SimulationData[experimentIndex].StrainSFHistory.Count == 0)
            {
                actStrainS = strainRateSample;
            }
            else
            {
                actStrainS = actSample.SimulationData[experimentIndex].StrainSFHistory[actSample.SimulationData[experimentIndex].StrainSFHistory.Count - 1] + strainRateSample;
            }
            actSample.SimulationData[experimentIndex].StrainSFHistory.Add(actStrainS.Clone());


            //YieldChange yieldChangeCFHistory
            actSample.SimulationData[experimentIndex].YieldChangeCFHistory.Add(yieldChangeOriented);

            //5.
            for (int phase = 0; phase < actSample.CrystalData.Count; phase++)
            {
                ////8.
                actSample.SimulationData[experimentIndex].StressRateCFOrientedHistory[phase].Add(grainStressesOriented[phase]);
                ////6.
                actSample.SimulationData[experimentIndex].StrainRateCFOrientedHistory[phase].Add(grainStrainsOriented[phase]);
                ////10.
                actSample.SimulationData[experimentIndex].ActiveSystemsCFOrientedHistory[phase].Add(potentialActiveGOriented[phase]);
                ////11. Härtung berechnen (Anzeige)
                for (int k = 0; k < actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems.Count; k++)
                {
                    if (singleCrystalTracking)
                    {
                        actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[k].YieldMainAvgHardenning = averageYieldChange[phase][k] / potentialActiveGOriented[phase].Count;
                    }
                    else
                    {
                        actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[k].YieldMainAvgHardenning += averageYieldChange[phase][k] / potentialActiveGOriented[phase].Count;
                    }
                }
            }

            //if (correctArea)
            //{
            //    double radius = Math.Sqrt(actSample.SimulationData[experimentIndex].SampleArea / Math.PI);

            //    double ellipseA = radius * (1 + actStrainS[0, 0]);
            //    double ellipseB = radius * (1 + actStrainS[1, 1]);

            //    double newArea = Math.PI * ellipseA * ellipseB;
            //    double appliedForce = actSample.SimulationData[experimentIndex].StressSFHistory[n][2, 2] * actSample.SimulationData[experimentIndex].SampleArea;

            //    MathNet.Numerics.LinearAlgebra.Matrix<double> newStressTensor = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense<double>(3, 3, 0);
            //    newStressTensor[2, 2] = appliedForce / newArea;

            //    actSample.SimulationData[experimentIndex].StressSFHistoryAreaCorrected.Add(newStressTensor);
            //}

        }

        //Actual Model
        public static void PerformStressExperimentMultithreading(Sample actSample, int experimentIndex, int n, int cycleLimit, bool textureActive, bool singleCrystalTracking, int slipCriterion, int elasticModel)
        {
            // Parameter definition
            //Potentiel aktivierte Gleitsysteme in 
            List<List<List<ReflexYield>>> potentialActiveGOriented = new List<List<List<ReflexYield>>>();
            //Stress rate for the sample
            MathNet.Numerics.LinearAlgebra.Matrix<double> stressRateS = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
            //Dehnungstensoren Tensoren guess ist einfach elastisch
            Tools.FourthRankTensor overallStiffnesses = actSample.GetSampleStiffnesses(textureActive);
            switch (elasticModel)
            {
                case 0:
                    //Reuss
                    overallStiffnesses = actSample.GetSampleStiffnesses(textureActive, actSample.ReussTensorData);
                    break;
                case 1:
                    //Hill
                    overallStiffnesses = actSample.GetSampleStiffnesses(textureActive, actSample.HillTensorData);
                    break;
                case 2:
                    //Kroener
                    overallStiffnesses = actSample.GetSampleStiffnesses(textureActive, actSample.KroenerTensorData);
                    break;
                case 3:
                    //De Wit
                    overallStiffnesses = actSample.GetSampleStiffnesses(textureActive, actSample.DeWittTensorData);
                    break;
                case 4:
                    //Matthies
                    overallStiffnesses = actSample.GetSampleStiffnesses(textureActive, actSample.GeometricHillTensorData);
                    break;
                default:
                    //Hill
                    overallStiffnesses = actSample.GetSampleStiffnesses(textureActive, actSample.HillTensorData);
                    break;
            }
            Tools.FourthRankTensor overallStiffnessesComp = overallStiffnesses.Clone() as Tools.FourthRankTensor;
            //Constraint Tensor L* Komplettes Sample
            Tools.FourthRankTensor constraintStiffness = new Tools.FourthRankTensor();

            ////Lc
            //List<List<Tools.FourthRankTensor>> grainStiffnesses = new List<List<Tools.FourthRankTensor>>();
            ////Ac
            //List<List<Tools.FourthRankTensor>> grainTransitionStiffnesses = new List<List<Tools.FourthRankTensor>>();
            //Lc, Mc, Ac, Bc, Hardenning matrix
            List<List<PlasticityTensor>> plasticTensorPhase = new List<List<PlasticityTensor>>();

            //gemittelten Phasen Nachgiebigkeiten
            List<Tools.FourthRankTensor> overallStiffnessesPhase = new List<Tools.FourthRankTensor>();
            //Grain Spannungen der Phasen und Orientierungen
            List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>> grainStressesOriented = new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>();
            //Grain Dehnungen der Phasen und Orientierungen
            List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>> grainStrainsOriented = new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>();
            //List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>> hardeningMatrixList = new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>();

            List<List<List<double>>> yieldChangeOriented = new List<List<List<double>>>();
            List<List<List<double>>> shearRate = new List<List<List<double>>>();
            List<double[]> averageYieldChange = new List<double[]>();

            //Phasen werden in den Listen definiert
            for (int phase = 0; phase < actSample.CrystalData.Count; phase++)
            {
                switch (elasticModel)
                {
                    case 0:
                        //Reuss
                        overallStiffnessesPhase.Add(actSample.ReussTensorData[phase].GetFourtRankStiffnesses());
                        break;
                    case 1:
                        //Hill
                        overallStiffnessesPhase.Add(actSample.HillTensorData[phase].GetFourtRankStiffnesses());
                        break;
                    case 2:
                        //Kroener
                        overallStiffnessesPhase.Add(actSample.KroenerTensorData[phase].GetFourtRankStiffnesses());
                        break;
                    case 3:
                        //De Wit
                        overallStiffnessesPhase.Add(actSample.DeWittTensorData[phase].GetFourtRankStiffnesses());
                        break;
                    case 4:
                        //Matthies
                        overallStiffnessesPhase.Add(actSample.GeometricHillTensorData[phase].GetFourtRankStiffnesses());
                        break;
                    default:
                        //Hill
                        overallStiffnessesPhase.Add(actSample.HillTensorData[phase].GetFourtRankStiffnesses());
                        break;
                }
                potentialActiveGOriented.Add(new List<List<ReflexYield>>());
                grainStressesOriented.Add(new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>());
                grainStrainsOriented.Add(new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>());
                //hardeningMatrixList.Add(new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>());
                shearRate.Add(new List<List<double>>());
                yieldChangeOriented.Add(new List<List<double>>());
                averageYieldChange.Add(new double[5]);
                plasticTensorPhase.Add(new List<PlasticityTensor>());
                //grainStiffnesses.Add(new List<Tools.FourthRankTensor>());
                //grainTransitionStiffnesses.Add(new List<Tools.FourthRankTensor>());
            }

            //Macroskopische Spannungsrate berechnen
            if (n == 0)
            {
                stressRateS[0, 0] += actSample.SimulationData[experimentIndex].StressSFHistory[n][0, 0];
                stressRateS[1, 0] += actSample.SimulationData[experimentIndex].StressSFHistory[n][1, 0];
                stressRateS[2, 0] += actSample.SimulationData[experimentIndex].StressSFHistory[n][2, 0];
                stressRateS[0, 1] += actSample.SimulationData[experimentIndex].StressSFHistory[n][0, 1];
                stressRateS[1, 1] += actSample.SimulationData[experimentIndex].StressSFHistory[n][1, 1];
                stressRateS[2, 1] += actSample.SimulationData[experimentIndex].StressSFHistory[n][2, 1];
                stressRateS[0, 2] += actSample.SimulationData[experimentIndex].StressSFHistory[n][0, 2];
                stressRateS[1, 2] += actSample.SimulationData[experimentIndex].StressSFHistory[n][1, 2];
                stressRateS[2, 2] += actSample.SimulationData[experimentIndex].StressSFHistory[n][2, 2];
            }
            else
            {
                stressRateS[0, 0] += actSample.SimulationData[experimentIndex].StressSFHistory[n][0, 0] - actSample.SimulationData[experimentIndex].StressSFHistory[n - 1][0, 0];
                stressRateS[1, 0] += actSample.SimulationData[experimentIndex].StressSFHistory[n][1, 0] - actSample.SimulationData[experimentIndex].StressSFHistory[n - 1][1, 0];
                stressRateS[2, 0] += actSample.SimulationData[experimentIndex].StressSFHistory[n][2, 0] - actSample.SimulationData[experimentIndex].StressSFHistory[n - 1][2, 0];
                stressRateS[0, 1] += actSample.SimulationData[experimentIndex].StressSFHistory[n][0, 1] - actSample.SimulationData[experimentIndex].StressSFHistory[n - 1][0, 1];
                stressRateS[1, 1] += actSample.SimulationData[experimentIndex].StressSFHistory[n][1, 1] - actSample.SimulationData[experimentIndex].StressSFHistory[n - 1][1, 1];
                stressRateS[2, 1] += actSample.SimulationData[experimentIndex].StressSFHistory[n][2, 1] - actSample.SimulationData[experimentIndex].StressSFHistory[n - 1][2, 1];
                stressRateS[0, 2] += actSample.SimulationData[experimentIndex].StressSFHistory[n][0, 2] - actSample.SimulationData[experimentIndex].StressSFHistory[n - 1][0, 2];
                stressRateS[1, 2] += actSample.SimulationData[experimentIndex].StressSFHistory[n][1, 2] - actSample.SimulationData[experimentIndex].StressSFHistory[n - 1][1, 2];
                stressRateS[2, 2] += actSample.SimulationData[experimentIndex].StressSFHistory[n][2, 2] - actSample.SimulationData[experimentIndex].StressSFHistory[n - 1][2, 2];
            }

            //MathNet.Numerics.LinearAlgebra.Matrix<double> stressRateS = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
            //MathNet.Numerics.LinearAlgebra.Matrix<double> stressS = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);


            //Selbstkonsistente Rechung
            for (int actCycle = 0; actCycle < cycleLimit; actCycle++)
            {
                bool allElastic = true;
                actSample.SimulationData[experimentIndex].LastActiveSystems = 0;

                Microsopic.ElasticityTensors eTmp = new Microsopic.ElasticityTensors();
                eTmp._stiffnessTensor = overallStiffnesses.GetVoigtTensor();
                eTmp.CalculateCompliances();
                constraintStiffness = actSample.PlasticTensor[0].GetConstraintStiffnessCubicIsotropic(eTmp, 2);
                actSample.SimulationData[experimentIndex].LastFailedGrains = 0;

                //Berechnungen im Krystallsystem
                for (int phase = 0; phase < actSample.CrystalData.Count; phase++)
                {
                    int grainIndexCounter = actSample.SimulationData[experimentIndex].GrainOrientations[phase].Count;
                    //Lc, Mc, Ac, Bc der aktuellen Phase
                    PlasticityTensor[] plasticTensororiented = new PlasticityTensor[grainIndexCounter];
                    //GrainSpanungen für alle orientierungen
                    MathNet.Numerics.LinearAlgebra.Matrix<double>[] grainStressesPhase = new MathNet.Numerics.LinearAlgebra.Matrix<double>[grainIndexCounter];
                    //Grain Dehnungen für alle orientierungen
                    MathNet.Numerics.LinearAlgebra.Matrix<double>[] grainStrainsPhase = new MathNet.Numerics.LinearAlgebra.Matrix<double>[grainIndexCounter];
                    //Active Gleitsysteme
                    List<ReflexYield>[] potentialActiveGPhase = new List<ReflexYield>[grainIndexCounter];
                    List<double>[] yieldChangePhase = new List<double>[grainIndexCounter];
                    List<double>[] shearRateOriented = new List<double>[grainIndexCounter];

                    // Parallelize the outer loop to partition the source array by rows.
                    Parallel.For(0, grainIndexCounter, grainIndexTmp =>
                    //for (int grainIndexTmp = 0; grainIndexTmp < grainIndexCounter; grainIndexTmp++)
                    {
                        int grainIndex = Convert.ToInt32(grainIndexTmp);
                        PlasticityTensor plasticTensorGrain = new PlasticityTensor();

                        if (grainIndex == 1000)
                        {
                            string test1 = "";
                        }
                        if (grainIndex == 2000)
                        {
                            string test1 = "";
                        }
                        //if (grainIndex == 632)
                        //{
                        //    string test1 = "";
                        //}
                        //if (grainIndex == 633)
                        //{
                        //    string test1 = "";
                        //}

                        if (singleCrystalTracking)
                        {
                            //Yield strength wird berechnet
                            List<double> hardennedYield = new List<double>();
                            for (int pS = 0; pS < actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems.Count; pS++)
                            {
                                hardennedYield.Add(0.0);
                                actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[pS].YieldMainHardennedStrength = actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[pS].YieldMainStrength;
                            }

                            if (actCycle != 0)
                            {
                                for (int k = 0; k < actSample.SimulationData[experimentIndex].YieldChangeCFHistory.Count; k++)
                                {
                                    for (int aS = 0; aS < actSample.SimulationData[experimentIndex].ActiveSystemsCFOrientedHistory[phase][k][grainIndex].Count; aS++)
                                    {
                                        for (int pS = 0; pS < actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems.Count; pS++)
                                        {
                                            if (actSample.SimulationData[experimentIndex].ActiveSystemsCFOrientedHistory[phase][k][grainIndex][aS].HKLString == actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[pS].HKLString)
                                            {
                                                //Vorfaktorberechnung für die Härtung
                                                double hardeningSaturation = 1;
                                                hardeningSaturation -= (actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[pS].YieldMainStrength + hardennedYield[pS]) / actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[pS].YieldLimit;
                                                //[k] und [phase] sind mit absicht vertauscht
                                                hardennedYield[pS] += hardeningSaturation * actSample.SimulationData[experimentIndex].YieldChangeCFHistory[k][phase][grainIndex][aS];
                                            }
                                        }
                                    }
                                }
                            }

                            for (int pS = 0; pS < actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems.Count; pS++)
                            {
                                actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[pS].YieldMainHardennedStrength += Math.Abs(hardennedYield[pS]);

                                //if (actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[pS].YieldMainHardennedStrength > actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[pS].YieldLimit)
                                //{
                                //    actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[pS].YieldMainHardennedStrength = actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[pS].YieldLimit;
                                //}
                            }

                        }

                        //Drehen des Spannungstensor in die aktuelle Orientierung
                        MathNet.Numerics.LinearAlgebra.Matrix<double> transformationMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense<double>(3, 3, 0);

                        transformationMatrix[0, 0] = -1 * Math.Cos(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi1 * (Math.PI / 180)) * Math.Cos(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Psi * (Math.PI / 180)) * Math.Sin(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi2 * (Math.PI / 180));
                        transformationMatrix[0, 0] -= Math.Sin(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi1 * (Math.PI / 180)) * Math.Cos(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi2 * (Math.PI / 180));
                        transformationMatrix[0, 1] = -1 * Math.Cos(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Psi * (Math.PI / 180)) * Math.Sin(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi1 * (Math.PI / 180)) * Math.Sin(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi2 * (Math.PI / 180));
                        transformationMatrix[0, 1] -= Math.Cos(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi1 * (Math.PI / 180)) * Math.Cos(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi2 * (Math.PI / 180));
                        transformationMatrix[0, 2] = Math.Sin(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Psi * (Math.PI / 180)) * Math.Sin(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi2 * (Math.PI / 180));
                        transformationMatrix[1, 0] = -1 * Math.Cos(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Psi * (Math.PI / 180)) * Math.Cos(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi1 * (Math.PI / 180)) * Math.Cos(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi2 * (Math.PI / 180));
                        transformationMatrix[1, 0] -= Math.Sin(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi1 * (Math.PI / 180)) * Math.Sin(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi2 * (Math.PI / 180));
                        transformationMatrix[1, 1] = -1 * Math.Sin(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi1 * (Math.PI / 180)) * Math.Cos(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Psi * (Math.PI / 180)) * Math.Cos(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi2 * (Math.PI / 180));
                        transformationMatrix[1, 1] -= Math.Cos(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi1 * (Math.PI / 180)) * Math.Sin(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi2 * (Math.PI / 180));
                        transformationMatrix[1, 2] = Math.Sin(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Psi * (Math.PI / 180)) * Math.Cos(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi2 * (Math.PI / 180));
                        transformationMatrix[2, 0] = Math.Cos(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi1 * (Math.PI / 180)) * Math.Sin(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Psi * (Math.PI / 180));
                        transformationMatrix[2, 1] = Math.Sin(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi1 * (Math.PI / 180)) * Math.Sin(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Psi * (Math.PI / 180));
                        transformationMatrix[2, 2] = Math.Cos(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Psi * (Math.PI / 180));

                        //for the self consistence the stress and stress rate are calculated from the last Bc if it exists
                        MathNet.Numerics.LinearAlgebra.Matrix<double> actStressGrainBc = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
                        MathNet.Numerics.LinearAlgebra.Matrix<double> actStressRateGrainBc = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

                        if (actCycle == 0)
                        {
                            for (int k = 0; k < n; k++)
                            {
                                actStressGrainBc += actSample.SimulationData[experimentIndex].StressRateCFOrientedHistory[phase][k][grainIndex];
                            }
                            actStressRateGrainBc = stressRateS;
                        }
                        else
                        {
                            ///constraintStiffness ergibt sich für alle Phasen gleichermaßen und wird aktualisiert
                            ///instGrainTensor wird aus Orientierungen berechnet und muss aus vorherigen Schritt genommen werden
                            ///overallStiffnesses wird pro cycle aktualisiert
                            Tools.FourthRankTensor lastBc = actSample.PlasticTensor[phase].GetAc(constraintStiffness.InverseSC(), plasticTensorPhase[phase][grainIndex].GrainCompliance, overallStiffnesses.InverseSC());

                            //actStressGrainBc = lastBc * actSample.SimulationData[experimentIndex].StressSFHistory[n];
                            for (int k = 0; k < n; k++)
                            {
                                actStressGrainBc += actSample.SimulationData[experimentIndex].StressRateCFOrientedHistory[phase][k][grainIndex];
                            }
                            actStressRateGrainBc = lastBc * stressRateS;
                        }

                        //Spannungstensor in der aktuellen Orientierung
                        MathNet.Numerics.LinearAlgebra.Matrix<double> actStressOriented = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
                        MathNet.Numerics.LinearAlgebra.Matrix<double> actStressRateOriented = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
                        for (int i = 0; i < 3; i++)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                for (int k = 0; k < 3; k++)
                                {
                                    for (int l = 0; l < 3; l++)
                                    {
                                        //actStressOriented[i, j] += transformationMatrix[i, k] * transformationMatrix[j, l] * actStressGrainBc[k, l];
                                        actStressRateOriented[i, j] += transformationMatrix[i, k] * transformationMatrix[j, l] * actStressRateGrainBc[k, l];
                                    }
                                }
                            }
                        }
                        actStressOriented = actStressGrainBc + actStressRateOriented;


                        List<double> potentialActiveStress = actSample.SimulationData[experimentIndex].YieldInformation[phase].GetResolvedStressList(actStressOriented);

                        //Potentiell aktive Gleitsysteme werden ermittelt
                        List<ReflexYield> potentialActive = actSample.SimulationData[experimentIndex].YieldInformation[phase].GetPotentiallyActiveSlipSystems(actStressOriented, slipCriterion);
                        bool noValidCombination = true;

                        if (potentialActive.Count != 0)
                        {
                            noValidCombination = false;
                            allElastic = false;
                            //Liste mit den Testsystemen wird erstell und Indiziert
                            List<ReflexYield> trialSystems = potentialActive;
                            List<int> checkedSystems = new List<int>();
                            for (int i = 0; i < trialSystems.Count; i++)
                            {
                                checkedSystems.Add(0);
                            }

                            //Filterung von Abhängigkeiten
                            actSample.SimulationData[experimentIndex].YieldInformation[phase].CheckDependencies(potentialActive, actStressOriented, checkedSystems);

                            //Schleife für die Finale Selection der Gleitsysteme
                            while (trialSystems.Count != 0)
                            {
                                #region Direction Switch

                                //Check ob die Scherraten positiv sind
                                List<int> checkedSystemsRe = new List<int>();
                                trialSystems = GetActiveSystemCombination(potentialActive, checkedSystems);
                                if (trialSystems.Count == 0)
                                {
                                    noValidCombination = true;
                                    break;
                                }
                                //bool resetSystems = false;
                                //for (int m = 0; m < checkedSystems.Count; m++)
                                //{
                                //    switch (checkedSystems[m])
                                //    {
                                //        case 0:
                                //            goto default;
                                //        case 1:
                                //            checkedSystemsRe.Add(1);
                                //            break;
                                //        case 2:
                                //            checkedSystemsRe.Add(2);
                                //            break;
                                //        default:
                                //            resetSystems = true;
                                //            break;
                                //    }
                                //}
                                //checkedSystems = checkedSystemsRe;

                                //if (resetSystems)
                                //{
                                //    for (int m = 0; m < checkedSystems.Count; m++)
                                //    {
                                //        checkedSystems[m] = 1;
                                //    }
                                //}

                                #endregion

                                //Härtungsmatrix wird berechnet
                                plasticTensorGrain.HardeningMatrix = actSample.SimulationData[experimentIndex].YieldInformation[phase].HardeningMatrixSlipSystem(trialSystems);

                                //Shear rates
                                List<double> shearRatesSystems = new List<double>();
                                //shear stress change rates
                                List<double> shearStressChangeSystems = new List<double>();
                                //yield change rates
                                List<double> yieldChangeSystems = new List<double>();

                                //Berechnung der instantanious Stiffness factors f^i
                                switch (elasticModel)
                                {
                                    case 0:
                                        //Reuss
                                        plasticTensorGrain.ConditionX = actSample.SimulationData[experimentIndex].YieldInformation[phase].SlipSystemX(trialSystems, plasticTensorGrain.HardeningMatrix, actSample.ReussTensorData[phase]);
                                        for (int i = 0; i < trialSystems.Count; i++)
                                        {
                                            plasticTensorGrain.InstantStiffnessFactors.Add(actSample.SimulationData[experimentIndex].YieldInformation[phase].GetInstStiffnessFactors(trialSystems, actSample.ReussTensorData[phase], i, plasticTensorGrain.ConditionY));
                                        }
                                        //Lc und Mc 
                                        plasticTensorGrain.GrainStiffness = actSample.PlasticTensor[phase].GetInstantanousGrainSiffnessTensor(trialSystems, actSample.ReussTensorData[phase], plasticTensorGrain.InstantStiffnessFactors);
                                        break;
                                    case 1:
                                        //Hill
                                        plasticTensorGrain.ConditionX = actSample.SimulationData[experimentIndex].YieldInformation[phase].SlipSystemX(trialSystems, plasticTensorGrain.HardeningMatrix, actSample.HillTensorData[phase]);
                                        for (int i = 0; i < trialSystems.Count; i++)
                                        {
                                            plasticTensorGrain.InstantStiffnessFactors.Add(actSample.SimulationData[experimentIndex].YieldInformation[phase].GetInstStiffnessFactors(trialSystems, actSample.HillTensorData[phase], i, plasticTensorGrain.ConditionY));
                                        }
                                        //Lc und Mc 
                                        plasticTensorGrain.GrainStiffness = actSample.PlasticTensor[phase].GetInstantanousGrainSiffnessTensor(trialSystems, actSample.HillTensorData[phase], plasticTensorGrain.InstantStiffnessFactors);
                                        break;
                                    case 2:
                                        //Kroener
                                        plasticTensorGrain.ConditionX = actSample.SimulationData[experimentIndex].YieldInformation[phase].SlipSystemX(trialSystems, plasticTensorGrain.HardeningMatrix, actSample.KroenerTensorData[phase]);
                                        for (int i = 0; i < trialSystems.Count; i++)
                                        {
                                            plasticTensorGrain.InstantStiffnessFactors.Add(actSample.SimulationData[experimentIndex].YieldInformation[phase].GetInstStiffnessFactors(trialSystems, actSample.KroenerTensorData[phase], i, plasticTensorGrain.ConditionY));
                                        }
                                        //Lc und Mc 
                                        plasticTensorGrain.GrainStiffness = actSample.PlasticTensor[phase].GetInstantanousGrainSiffnessTensor(trialSystems, actSample.KroenerTensorData[phase], plasticTensorGrain.InstantStiffnessFactors);
                                        break;
                                    case 3:
                                        //De Wit
                                        plasticTensorGrain.ConditionX = actSample.SimulationData[experimentIndex].YieldInformation[phase].SlipSystemX(trialSystems, plasticTensorGrain.HardeningMatrix, actSample.DeWittTensorData[phase]);
                                        for (int i = 0; i < trialSystems.Count; i++)
                                        {
                                            plasticTensorGrain.InstantStiffnessFactors.Add(actSample.SimulationData[experimentIndex].YieldInformation[phase].GetInstStiffnessFactors(trialSystems, actSample.DeWittTensorData[phase], i, plasticTensorGrain.ConditionY));
                                        }
                                        //Lc und Mc 
                                        plasticTensorGrain.GrainStiffness = actSample.PlasticTensor[phase].GetInstantanousGrainSiffnessTensor(trialSystems, actSample.DeWittTensorData[phase], plasticTensorGrain.InstantStiffnessFactors);
                                        break;
                                    case 4:
                                        //Matthies
                                        plasticTensorGrain.ConditionX = actSample.SimulationData[experimentIndex].YieldInformation[phase].SlipSystemX(trialSystems, plasticTensorGrain.HardeningMatrix, actSample.GeometricHillTensorData[phase]);
                                        for (int i = 0; i < trialSystems.Count; i++)
                                        {
                                            plasticTensorGrain.InstantStiffnessFactors.Add(actSample.SimulationData[experimentIndex].YieldInformation[phase].GetInstStiffnessFactors(trialSystems, actSample.GeometricHillTensorData[phase], i, plasticTensorGrain.ConditionY));
                                        }
                                        //Lc und Mc 
                                        plasticTensorGrain.GrainStiffness = actSample.PlasticTensor[phase].GetInstantanousGrainSiffnessTensor(trialSystems, actSample.GeometricHillTensorData[phase], plasticTensorGrain.InstantStiffnessFactors);
                                        break;
                                    default:
                                        //Hill
                                        plasticTensorGrain.ConditionX = actSample.SimulationData[experimentIndex].YieldInformation[phase].SlipSystemX(trialSystems, plasticTensorGrain.HardeningMatrix, actSample.HillTensorData[phase]);
                                        for (int i = 0; i < trialSystems.Count; i++)
                                        {
                                            plasticTensorGrain.InstantStiffnessFactors.Add(actSample.SimulationData[experimentIndex].YieldInformation[phase].GetInstStiffnessFactors(trialSystems, actSample.HillTensorData[phase], i, plasticTensorGrain.ConditionY));
                                        }
                                        //Lc und Mc 
                                        plasticTensorGrain.GrainStiffness = actSample.PlasticTensor[phase].GetInstantanousGrainSiffnessTensor(trialSystems, actSample.HillTensorData[phase], plasticTensorGrain.InstantStiffnessFactors);
                                        break;
                                }


                                //InversionCheck, ob alles richtig Invertiert wurde
                                Tools.FourthRankTensor invCheckTensor = Tools.FourthRankTensor.InnerProduct(plasticTensorGrain.GrainStiffness, plasticTensorGrain.GrainCompliance);
                                double invDifference = invCheckTensor.GetDifference(Tools.FourthRankTensor.GetUnityTensor());
                                if (invDifference < 0.0001)
                                {
                                    //berechung von Ac
                                    plasticTensorGrain.GrainTransitionStiffness = actSample.PlasticTensor[phase].GetAc(constraintStiffness, plasticTensorGrain.GrainStiffness, overallStiffnesses);
                                    //bzw Bc
                                    plasticTensorGrain.GrainTransitionCompliance = actSample.PlasticTensor[phase].GetAc(constraintStiffness.InverseSC(), plasticTensorGrain.GrainCompliance, overallStiffnesses.InverseSC());

                                    MathNet.Numerics.LinearAlgebra.Matrix<double> stressRateGrainNo = plasticTensorGrain.GrainTransitionCompliance * stressRateS;
                                    //MathNet.Numerics.LinearAlgebra.Matrix<double> stressRateGrain = plasticTensorGrain.GrainTransitionCompliance * actStressOriented;
                                    MathNet.Numerics.LinearAlgebra.Matrix<double> stressRateGrain = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense<double>(3, 3, 0); ;
                                    for (int i = 0; i < 3; i++)
                                    {
                                        for (int j = 0; j < 3; j++)
                                        {
                                            for (int k = 0; k < 3; k++)
                                            {
                                                for (int l = 0; l < 3; l++)
                                                {
                                                    stressRateGrain[i, j] += transformationMatrix[i, k] * transformationMatrix[j, l] * stressRateGrainNo[k, l];
                                                }
                                            }
                                        }
                                    }

                                    MathNet.Numerics.LinearAlgebra.Matrix<double> strainRateGrain = plasticTensorGrain.GrainCompliance * stressRateGrain;

                                    //Berechnung der shear rates und der spannungsänderung
                                    for (int i = 0; i < trialSystems.Count; i++)
                                    {
                                        double actShearRate = 0.0;
                                        double shearstress = 0.0;
                                        MathNet.Numerics.LinearAlgebra.Matrix<double> resolvingMatrix = Tools.Calculation.GetResolvingParameter(trialSystems[i].SlipPlane, trialSystems[i].MainSlipDirection);

                                        for (int j = 0; j < 3; j++)
                                        {
                                            for (int k = 0; k < 3; k++)
                                            {
                                                actShearRate += plasticTensorGrain.InstantStiffnessFactors[i][j, k] * strainRateGrain[j, k];
                                                shearstress += stressRateGrain[j, k] * resolvingMatrix[j, k];
                                            }
                                        }

                                        shearRatesSystems.Add(actShearRate);
                                        shearStressChangeSystems.Add(shearstress);
                                    }

                                    bool negativeShear = false;

                                    #region direction check

                                    ////Checkt ob die Scherraten positiv sind und markiert diejenigen, deren Gleitrichtung umgedreht werden muss
                                    //for (int sys = 0; sys < shearRatesSystems.Count; sys++)
                                    //{
                                    //    //Check ob die scherraten positiv oder negativ sind
                                    //    if (shearRatesSystems[sys] < 0)
                                    //    {
                                    //        //Falls negativ wird die Gleitrichtung umgedreht
                                    //        if (checkedSystems[sys] == 1)
                                    //        {
                                    //            checkedSystems[sys] = 2;
                                    //        }
                                    //        else if (checkedSystems[sys] == 2)
                                    //        {
                                    //            checkedSystems[sys] = 0;
                                    //        }
                                    //        negativeShear = true;
                                    //    }
                                    //}

                                    #endregion

                                    if (!negativeShear)
                                    {
                                        //Berechung der yield changes
                                        for (int i = 0; i < trialSystems.Count; i++)
                                        {
                                            double yieldChangeTmp = 0.0;
                                            for (int j = 0; j < trialSystems.Count; j++)
                                            {
                                                yieldChangeTmp += plasticTensorGrain.HardeningMatrix[i, j] * shearRatesSystems[j];
                                            }
                                            yieldChangeSystems.Add(yieldChangeTmp);
                                        }

                                        //Vergleich zwischen shear rates und yield changes
                                        double difference = 0.0;
                                        double norm = 0.0;
                                        for (int i = 0; i < trialSystems.Count; i++)
                                        {
                                            difference = (Math.Abs(yieldChangeSystems[i]) - Math.Abs(shearStressChangeSystems[i])) / Math.Abs(yieldChangeSystems[i]);
                                            norm += Math.Abs(yieldChangeSystems[i]);
                                        }

                                        difference /= norm;

                                        if (difference < 1)
                                        {
                                            //Speichern der Parameter
                                            potentialActiveGPhase[grainIndex] = trialSystems;
                                            grainStrainsPhase[grainIndex] = strainRateGrain;
                                            grainStressesPhase[grainIndex] = stressRateGrain;
                                            yieldChangePhase[grainIndex] = yieldChangeSystems;

                                            plasticTensororiented[grainIndex] = plasticTensorGrain;

                                            actSample.SimulationData[experimentIndex].LastActiveSystems += trialSystems.Count;
                                            break;
                                        }
                                        else
                                        {
                                            //System mit kleinsten Shearstress wird herausgesucht
                                            int smallestIndex = actSample.SimulationData[experimentIndex].YieldInformation[phase].GetSmallesSchearStressIndex(trialSystems, actStressOriented);

                                            //System mit kleinsten Shearstress wird aussortiert
                                            for (int k = 0; k < potentialActive.Count; k++)
                                            {
                                                if (potentialActive[k].SlipPlane.HKLString == trialSystems[smallestIndex].SlipPlane.HKLString && potentialActive[k].MainSlipDirection.HKLString == trialSystems[smallestIndex].MainSlipDirection.HKLString)
                                                {
                                                    checkedSystems[k] = 3;
                                                    break;
                                                }
                                            }

                                            //Das nächste noch nicht getestete System wird aktiviert
                                            for (int k = 0; k < checkedSystems.Count; k++)
                                            {
                                                if (checkedSystems[k] == 0)
                                                {
                                                    checkedSystems[k] = 1;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    //System mit kleinsten Shearstress wird herausgesucht
                                    int smallestIndex = actSample.SimulationData[experimentIndex].YieldInformation[phase].GetSmallesSchearStressIndex(trialSystems, actStressOriented);

                                    //System mit kleinsten Shearstress wird aussortiert
                                    for (int k = 0; k < potentialActive.Count; k++)
                                    {
                                        if (potentialActive[k].SlipPlane.HKLString == trialSystems[smallestIndex].SlipPlane.HKLString && potentialActive[k].MainSlipDirection.HKLString == trialSystems[smallestIndex].MainSlipDirection.HKLString)
                                        {
                                            checkedSystems[k] = 3;
                                            break;
                                        }
                                    }

                                    //Das nächste noch nicht getestete System wird aktiviert
                                    for (int k = 0; k < checkedSystems.Count; k++)
                                    {
                                        if (checkedSystems[k] == 0)
                                        {
                                            checkedSystems[k] = 1;
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (noValidCombination)
                        {
                            actSample.SimulationData[experimentIndex].LastFailedGrains++;
                            switch (elasticModel)
                            {
                                case 0:
                                    //Reuss
                                    plasticTensorGrain.GrainStiffness = actSample.ReussTensorData[phase].GetFourtRankStiffnesses();
                                    break;
                                case 1:
                                    //Hill
                                    plasticTensorGrain.GrainStiffness = actSample.HillTensorData[phase].GetFourtRankStiffnesses();
                                    break;
                                case 2:
                                    //Kroener
                                    plasticTensorGrain.GrainStiffness = actSample.KroenerTensorData[phase].GetFourtRankStiffnesses();
                                    break;
                                case 3:
                                    //De Wit
                                    plasticTensorGrain.GrainStiffness = actSample.DeWittTensorData[phase].GetFourtRankStiffnesses();
                                    break;
                                case 4:
                                    //Matthies
                                    plasticTensorGrain.GrainStiffness = actSample.GeometricHillTensorData[phase].GetFourtRankStiffnesses();
                                    break;
                                default:
                                    //Hill
                                    plasticTensorGrain.GrainStiffness = actSample.HillTensorData[phase].GetFourtRankStiffnesses();
                                    break;
                            }

                            //berechung von Ac
                            plasticTensorGrain.GrainTransitionStiffness = actSample.PlasticTensor[phase].GetAc(constraintStiffness, plasticTensorGrain.GrainStiffness, overallStiffnesses);
                            //bzw Bc
                            plasticTensorGrain.GrainTransitionCompliance = actSample.PlasticTensor[phase].GetAc(constraintStiffness.InverseSC(), plasticTensorGrain.GrainCompliance, overallStiffnesses.InverseSC());

                            MathNet.Numerics.LinearAlgebra.Matrix<double> stressRateGrainSave = plasticTensorGrain.GrainTransitionCompliance * stressRateS;
                            MathNet.Numerics.LinearAlgebra.Matrix<double> stressRateOrientedSave = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
                            for (int i = 0; i < 3; i++)
                            {
                                for (int j = 0; j < 3; j++)
                                {
                                    for (int k = 0; k < 3; k++)
                                    {
                                        for (int l = 0; l < 3; l++)
                                        {
                                            stressRateOrientedSave[i, j] += transformationMatrix[i, k] * transformationMatrix[j, l] * stressRateGrainSave[k, l];
                                        }
                                    }
                                }
                            }
                            MathNet.Numerics.LinearAlgebra.Matrix<double> strainRateGrainSave = plasticTensorGrain.GrainCompliance * stressRateOrientedSave;

                            //Speichern der Parameter
                            potentialActiveGPhase[grainIndex] = new List<ReflexYield>();
                            grainStrainsPhase[grainIndex] = strainRateGrainSave;
                            grainStressesPhase[grainIndex] = stressRateOrientedSave;
                            yieldChangePhase[grainIndex] = new List<double>();

                            plasticTensororiented[grainIndex] = plasticTensorGrain;
                        }

                    }); // Parallel.For

                    //Überschreiben der Phasendaten
                    plasticTensorPhase[phase] = plasticTensororiented.ToList();
                    
                    potentialActiveGOriented[phase] = potentialActiveGPhase.ToList();
                    grainStressesOriented[phase] = grainStressesPhase.ToList();
                    grainStrainsOriented[phase] = grainStrainsPhase.ToList();
                    yieldChangeOriented[phase] = yieldChangePhase.ToList();

                    //Mittelung des neuen Phasenmoduls
                    if (textureActive)
                    {

                    }
                    else
                    {
                        overallStiffnessesPhase[phase] = Tools.FourthRankTensor.AverageInnerProduct(plasticTensorPhase[phase]);
                        overallStiffnessesPhase[phase].SetHexagonalSymmetryCorrection();
                    }

                    //Mittelung der neuen Fließgrenze PotentialSlipSystems
                    averageYieldChange[phase] = new double[actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems.Count];

                    for (int i = 0; i < potentialActiveGOriented[phase].Count; i++)
                    {
                        for (int j = 0; j < potentialActiveGOriented[phase][i].Count; j++)
                        {
                            for (int k = 0; k < actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems.Count; k++)
                            {
                                if (potentialActiveGOriented[phase][i][j].HKLString == actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[k].HKLString && potentialActiveGOriented[phase][i][j].HKLStringSlipDirection == actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[k].HKLStringSlipDirection)
                                {
                                    averageYieldChange[phase][k] += Math.Abs(yieldChangeOriented[phase][i][j]);
                                }
                            }
                        }
                    }
                }
                //Ab hier berechung der Makrodaten
                //Resetting the overall Stiffnesses

                overallStiffnesses = new Tools.FourthRankTensor();
                for (int phase = 0; phase < actSample.CrystalData.Count; phase++)
                {
                    overallStiffnesses += actSample.CrystalData[phase].PhaseFraction * overallStiffnessesPhase[phase];
                }

                MathNet.Numerics.LinearAlgebra.Matrix<double> energyCheckMat = overallStiffnesses.GetVoigtTensor();
                bool energyCheck = true;

                //Energy check for the calculated constants
                if (energyCheckMat[3, 3] <= 0)
                {
                    energyCheck = false;
                }
                else if (energyCheckMat[0, 0] <= Math.Abs(energyCheckMat[0, 1]))
                {
                    energyCheck = false;
                }
                else if ((energyCheckMat[0, 0] + energyCheckMat[0, 1]) * energyCheckMat[2, 2] <= 0)
                {
                    energyCheck = false;
                }

                if (energyCheck)
                {
                    double overallDifference = overallStiffnesses.GetDifference(overallStiffnessesComp);

                    MathNet.Numerics.LinearAlgebra.Matrix<double> DebugComp = overallStiffnessesComp.GetVoigtTensor();
                    MathNet.Numerics.LinearAlgebra.Matrix<double> DebugDiff = DebugComp - energyCheckMat;
                    if (overallDifference < CalScec.Properties.Settings.Default.EPSCSimulationLimit)
                    {
                        break;
                    }
                    else
                    {
                        overallStiffnessesComp = overallStiffnesses.Clone() as Tools.FourthRankTensor;
                    }
                }
                else
                {
                    overallStiffnesses = overallStiffnessesComp;
                    break;
                }
            }

            //Abspeichern der Daten
            //All base tensor for the Grains, sample tensor are set
            //the stress states for the sample and phases will be calculated
            //Parameter Reihenfolge:
            //1. Spannung Probe (Berechnet aus der Spannungssrate der Probe)
            //2. Spannungsrate Probe (berechnet)
            //3. Dehnungsrate Probe (über overallStiffnesses gerechnet)
            //4. Dehnung Probe (Eingabe)
            //5. Bestimmung der Phasenparameter
            //Alles auf grain lvl wird nach des Konvergenz checks berechnet und gesetzt
            //6. Dehnungsraten der grains (berechnet über die transition Matrix)
            //7. Dehnung der grains (berechnet über die Dehnrate)
            //8. Berechnung der Spannungsrate im Grain (Dehnrate aus 6. - Plastischer Dehnrate --> Elastische konstanten)
            //9. Berechung der Aktuellen Spannung analog zu 7.
            //10. Aktive Gleitsysteme

            // 1
            //actSample.SimulationData[experimentIndex].StressSFHistory.Add(stressS.Clone());
            //2. 
            actSample.SimulationData[experimentIndex].StressRateSFHistory.Add(stressRateS.Clone());

            //3.
            MathNet.Numerics.LinearAlgebra.Matrix<double> strainRateSample = overallStiffnesses.InverseSC() * stressRateS;
            actSample.SimulationData[experimentIndex].StrainRateSFHistory.Add(strainRateSample);

            //4.
            MathNet.Numerics.LinearAlgebra.Matrix<double> actStrainS = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
            if (actSample.SimulationData[experimentIndex].StrainSFHistory.Count == 0)
            {
                actStrainS = strainRateSample;
            }
            else
            {
                actStrainS = actSample.SimulationData[experimentIndex].StrainSFHistory[actSample.SimulationData[experimentIndex].StrainSFHistory.Count - 1] + strainRateSample;
            }
            actSample.SimulationData[experimentIndex].StrainSFHistory.Add(actStrainS.Clone());


            //YieldChange yieldChangeCFHistory
            actSample.SimulationData[experimentIndex].YieldChangeCFHistory.Add(yieldChangeOriented);
            
            //5.
            for (int phase = 0; phase < actSample.CrystalData.Count; phase++)
            {
                ////8.
                actSample.SimulationData[experimentIndex].StressRateCFOrientedHistory[phase].Add(grainStressesOriented[phase]);
                ////6.
                actSample.SimulationData[experimentIndex].StrainRateCFOrientedHistory[phase].Add(grainStrainsOriented[phase]);
                ////10.
                actSample.SimulationData[experimentIndex].ActiveSystemsCFOrientedHistory[phase].Add(potentialActiveGOriented[phase]);
                ////11. Härtung berechnen (Anzeige)
                for (int k = 0; k < actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems.Count; k++)
                {
                    if (singleCrystalTracking)
                    {
                        actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[k].YieldMainHardennedStrength = averageYieldChange[phase][k] / potentialActiveGOriented[phase].Count;
                    }
                    else
                    {
                        actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[k].YieldMainHardennedStrength += averageYieldChange[phase][k] / potentialActiveGOriented[phase].Count;
                    }
                }
            }

            //if (correctArea)
            //{
            //    double radius = Math.Sqrt(actSample.SimulationData[experimentIndex].SampleArea / Math.PI);

            //    double ellipseA = radius * (1 + actStrainS[0, 0]);
            //    double ellipseB = radius * (1 + actStrainS[1, 1]);

            //    double newArea = Math.PI * ellipseA * ellipseB;
            //    double appliedForce = actSample.SimulationData[experimentIndex].StressSFHistory[n][2, 2] * actSample.SimulationData[experimentIndex].SampleArea;

            //    MathNet.Numerics.LinearAlgebra.Matrix<double> newStressTensor = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense<double>(3, 3, 0);
            //    newStressTensor[2, 2] = appliedForce / newArea;

            //    actSample.SimulationData[experimentIndex].StressSFHistoryAreaCorrected.Add(newStressTensor);
            //}

        }

        public static void PerformStressExperiment(Sample actSample, int experimentIndex, int n, int cycleLimit, bool textureActive, bool singleCrystalTracking, int slipCriterion, int elasticModel)
        {
            // Parameter definition
            //Potentiel aktivierte Gleitsysteme in 
            List<List<List<ReflexYield>>> potentialActiveGOriented = new List<List<List<ReflexYield>>>();
            //Stress rate for the sample
            MathNet.Numerics.LinearAlgebra.Matrix<double> stressRateS = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
            //Dehnungstensoren Tensoren guess ist einfach elastisch
            Tools.FourthRankTensor overallStiffnesses = actSample.GetSampleStiffnesses(textureActive);
            switch (elasticModel)
            {
                case 0:
                    //Reuss
                    overallStiffnesses = actSample.GetSampleStiffnesses(textureActive, actSample.ReussTensorData);
                    break;
                case 1:
                    //Hill
                    overallStiffnesses = actSample.GetSampleStiffnesses(textureActive, actSample.HillTensorData);
                    break;
                case 2:
                    //Kroener
                    overallStiffnesses = actSample.GetSampleStiffnesses(textureActive, actSample.KroenerTensorData);
                    break;
                case 3:
                    //De Wit
                    overallStiffnesses = actSample.GetSampleStiffnesses(textureActive, actSample.DeWittTensorData);
                    break;
                case 4:
                    //Matthies
                    overallStiffnesses = actSample.GetSampleStiffnesses(textureActive, actSample.GeometricHillTensorData);
                    break;
                default:
                    //Hill
                    overallStiffnesses = actSample.GetSampleStiffnesses(textureActive, actSample.HillTensorData);
                    break;
            }
            Tools.FourthRankTensor overallStiffnessesComp = overallStiffnesses.Clone() as Tools.FourthRankTensor;
            //Constraint Tensor L* Komplettes Sample
            Tools.FourthRankTensor constraintStiffness = new Tools.FourthRankTensor();

            ////Lc
            //List<List<Tools.FourthRankTensor>> grainStiffnesses = new List<List<Tools.FourthRankTensor>>();
            ////Ac
            //List<List<Tools.FourthRankTensor>> grainTransitionStiffnesses = new List<List<Tools.FourthRankTensor>>();
            //Lc, Mc, Ac, Bc, Hardenning matrix
            List<List<PlasticityTensor>> plasticTensorPhase = new List<List<PlasticityTensor>>();

            //gemittelten Phasen Nachgiebigkeiten
            List<Tools.FourthRankTensor> overallStiffnessesPhase = new List<Tools.FourthRankTensor>();
            //Grain Spannungen der Phasen und Orientierungen
            List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>> grainStressesOriented = new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>();
            //Grain Dehnungen der Phasen und Orientierungen
            List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>> grainStrainsOriented = new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>();
            //List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>> hardeningMatrixList = new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>();

            List<List<List<double>>> yieldChangeOriented = new List<List<List<double>>>();
            List<List<List<double>>> shearRate = new List<List<List<double>>>();
            List<double[]> averageYieldChange = new List<double[]>();

            //Phasen werden in den Listen definiert
            for (int phase = 0; phase < actSample.CrystalData.Count; phase++)
            {
                switch (elasticModel)
                {
                    case 0:
                        //Reuss
                        overallStiffnessesPhase.Add(actSample.ReussTensorData[phase].GetFourtRankStiffnesses());
                        break;
                    case 1:
                        //Hill
                        overallStiffnessesPhase.Add(actSample.HillTensorData[phase].GetFourtRankStiffnesses());
                        break;
                    case 2:
                        //Kroener
                        overallStiffnessesPhase.Add(actSample.KroenerTensorData[phase].GetFourtRankStiffnesses());
                        break;
                    case 3:
                        //De Wit
                        overallStiffnessesPhase.Add(actSample.DeWittTensorData[phase].GetFourtRankStiffnesses());
                        break;
                    case 4:
                        //Matthies
                        overallStiffnessesPhase.Add(actSample.GeometricHillTensorData[phase].GetFourtRankStiffnesses());
                        break;
                    default:
                        //Hill
                        overallStiffnessesPhase.Add(actSample.HillTensorData[phase].GetFourtRankStiffnesses());
                        break;
                }
                potentialActiveGOriented.Add(new List<List<ReflexYield>>());
                grainStressesOriented.Add(new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>());
                grainStrainsOriented.Add(new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>());
                //hardeningMatrixList.Add(new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>());
                shearRate.Add(new List<List<double>>());
                yieldChangeOriented.Add(new List<List<double>>());
                averageYieldChange.Add(new double[5]);
                plasticTensorPhase.Add(new List<PlasticityTensor>());
                //grainStiffnesses.Add(new List<Tools.FourthRankTensor>());
                //grainTransitionStiffnesses.Add(new List<Tools.FourthRankTensor>());
            }

            //Macroskopische Spannungsrate berechnen
            if (n == 0)
            {
                stressRateS[0, 0] += actSample.SimulationData[experimentIndex].StressSFHistory[n][0, 0];
                stressRateS[1, 0] += actSample.SimulationData[experimentIndex].StressSFHistory[n][1, 0];
                stressRateS[2, 0] += actSample.SimulationData[experimentIndex].StressSFHistory[n][2, 0];
                stressRateS[0, 1] += actSample.SimulationData[experimentIndex].StressSFHistory[n][0, 1];
                stressRateS[1, 1] += actSample.SimulationData[experimentIndex].StressSFHistory[n][1, 1];
                stressRateS[2, 1] += actSample.SimulationData[experimentIndex].StressSFHistory[n][2, 1];
                stressRateS[0, 2] += actSample.SimulationData[experimentIndex].StressSFHistory[n][0, 2];
                stressRateS[1, 2] += actSample.SimulationData[experimentIndex].StressSFHistory[n][1, 2];
                stressRateS[2, 2] += actSample.SimulationData[experimentIndex].StressSFHistory[n][2, 2];
            }
            else
            {
                stressRateS[0, 0] += actSample.SimulationData[experimentIndex].StressSFHistory[n][0, 0] - actSample.SimulationData[experimentIndex].StressSFHistory[n - 1][0, 0];
                stressRateS[1, 0] += actSample.SimulationData[experimentIndex].StressSFHistory[n][1, 0] - actSample.SimulationData[experimentIndex].StressSFHistory[n - 1][1, 0];
                stressRateS[2, 0] += actSample.SimulationData[experimentIndex].StressSFHistory[n][2, 0] - actSample.SimulationData[experimentIndex].StressSFHistory[n - 1][2, 0];
                stressRateS[0, 1] += actSample.SimulationData[experimentIndex].StressSFHistory[n][0, 1] - actSample.SimulationData[experimentIndex].StressSFHistory[n - 1][0, 1];
                stressRateS[1, 1] += actSample.SimulationData[experimentIndex].StressSFHistory[n][1, 1] - actSample.SimulationData[experimentIndex].StressSFHistory[n - 1][1, 1];
                stressRateS[2, 1] += actSample.SimulationData[experimentIndex].StressSFHistory[n][2, 1] - actSample.SimulationData[experimentIndex].StressSFHistory[n - 1][2, 1];
                stressRateS[0, 2] += actSample.SimulationData[experimentIndex].StressSFHistory[n][0, 2] - actSample.SimulationData[experimentIndex].StressSFHistory[n - 1][0, 2];
                stressRateS[1, 2] += actSample.SimulationData[experimentIndex].StressSFHistory[n][1, 2] - actSample.SimulationData[experimentIndex].StressSFHistory[n - 1][1, 2];
                stressRateS[2, 2] += actSample.SimulationData[experimentIndex].StressSFHistory[n][2, 2] - actSample.SimulationData[experimentIndex].StressSFHistory[n - 1][2, 2];
            }

            //MathNet.Numerics.LinearAlgebra.Matrix<double> stressRateS = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
            //MathNet.Numerics.LinearAlgebra.Matrix<double> stressS = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);


            //Selbstkonsistente Rechung
            for (int actCycle = 0; actCycle < cycleLimit; actCycle++)
            {
                bool allElastic = true;
                actSample.SimulationData[experimentIndex].LastActiveSystems = 0;

                Microsopic.ElasticityTensors eTmp = new Microsopic.ElasticityTensors();
                eTmp._stiffnessTensor = overallStiffnesses.GetVoigtTensor();
                eTmp.CalculateCompliances();
                constraintStiffness = actSample.PlasticTensor[0].GetConstraintStiffnessCubicIsotropic(eTmp, 2);
                actSample.SimulationData[experimentIndex].LastFailedGrains = 0;

                //Berechnungen im Krystallsystem
                for (int phase = 0; phase < actSample.CrystalData.Count; phase++)
                {
                    int grainIndexCounter = actSample.SimulationData[experimentIndex].GrainOrientations[phase].Count;
                    //Lc, Mc, Ac, Bc der aktuellen Phase
                    PlasticityTensor[] plasticTensororiented = new PlasticityTensor[grainIndexCounter];
                    //GrainSpanungen für alle orientierungen
                    MathNet.Numerics.LinearAlgebra.Matrix<double>[] grainStressesPhase = new MathNet.Numerics.LinearAlgebra.Matrix<double>[grainIndexCounter];
                    //Grain Dehnungen für alle orientierungen
                    MathNet.Numerics.LinearAlgebra.Matrix<double>[] grainStrainsPhase = new MathNet.Numerics.LinearAlgebra.Matrix<double>[grainIndexCounter];
                    //Active Gleitsysteme
                    List<ReflexYield>[] potentialActiveGPhase = new List<ReflexYield>[grainIndexCounter];
                    List<double>[] yieldChangePhase = new List<double>[grainIndexCounter];
                    List<double>[] shearRateOriented = new List<double>[grainIndexCounter];

                    // Parallelize the outer loop to partition the source array by rows.
                    //Parallel.For(0, grainIndexCounter, grainIndexTmp =>
                    for (int grainIndexTmp = 0; grainIndexTmp < grainIndexCounter; grainIndexTmp++)
                    {
                        int grainIndex = Convert.ToInt32(grainIndexTmp);
                        PlasticityTensor plasticTensorGrain = new PlasticityTensor();

                        if (grainIndex == 1000)
                        {
                            string test1 = "";
                        }
                        if (grainIndex == 2000)
                        {
                            string test1 = "";
                        }
                        //if (grainIndex == 632)
                        //{
                        //    string test1 = "";
                        //}
                        //if (grainIndex == 633)
                        //{
                        //    string test1 = "";
                        //}

                        if (singleCrystalTracking)
                        {
                            //Yield strength wird berechnet
                            List<double> hardennedYield = new List<double>();
                            for (int pS = 0; pS < actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems.Count; pS++)
                            {
                                hardennedYield.Add(0.0);
                                actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[pS].YieldMainHardennedStrength = actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[pS].YieldMainStrength;
                            }

                            if (actCycle != 0)
                            {
                                for (int k = 0; k < actSample.SimulationData[experimentIndex].YieldChangeCFHistory.Count; k++)
                                {
                                    for (int aS = 0; aS < actSample.SimulationData[experimentIndex].ActiveSystemsCFOrientedHistory[phase][k][grainIndex].Count; aS++)
                                    {
                                        for (int pS = 0; pS < actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems.Count; pS++)
                                        {
                                            if (actSample.SimulationData[experimentIndex].ActiveSystemsCFOrientedHistory[phase][k][grainIndex][aS].HKLString == actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[pS].HKLString)
                                            {
                                                //Vorfaktorberechnung für die Härtung
                                                if(actSample.SimulationData[experimentIndex].useYieldLimit)
                                                {
                                                    double hardeningSaturation = 1;
                                                    hardeningSaturation -= (actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[pS].YieldMainStrength + hardennedYield[pS]) / actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[pS].YieldLimit;
                                                    //[k] und [phase] sind mit absicht vertauscht
                                                    hardennedYield[pS] += hardeningSaturation * actSample.SimulationData[experimentIndex].YieldChangeCFHistory[k][phase][grainIndex][aS];
                                                }
                                                else
                                                {
                                                    //[k] und [phase] sind mit absicht vertauscht
                                                    hardennedYield[pS] += actSample.SimulationData[experimentIndex].YieldChangeCFHistory[k][phase][grainIndex][aS];
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            for (int pS = 0; pS < actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems.Count; pS++)
                            {
                                actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[pS].YieldMainHardennedStrength += Math.Abs(hardennedYield[pS]);

                                //if (actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[pS].YieldMainHardennedStrength > actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[pS].YieldLimit)
                                //{
                                //    actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[pS].YieldMainHardennedStrength = actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[pS].YieldLimit;
                                //}
                            }

                        }

                        //Drehen des Spannungstensor in die aktuelle Orientierung
                        MathNet.Numerics.LinearAlgebra.Matrix<double> transformationMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense<double>(3, 3, 0);

                        transformationMatrix[0, 0] = -1 * Math.Cos(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi1 * (Math.PI / 180)) * Math.Cos(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Psi * (Math.PI / 180)) * Math.Sin(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi2 * (Math.PI / 180));
                        transformationMatrix[0, 0] -= Math.Sin(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi1 * (Math.PI / 180)) * Math.Cos(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi2 * (Math.PI / 180));
                        transformationMatrix[0, 1] = -1 * Math.Cos(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Psi * (Math.PI / 180)) * Math.Sin(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi1 * (Math.PI / 180)) * Math.Sin(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi2 * (Math.PI / 180));
                        transformationMatrix[0, 1] -= Math.Cos(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi1 * (Math.PI / 180)) * Math.Cos(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi2 * (Math.PI / 180));
                        transformationMatrix[0, 2] = Math.Sin(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Psi * (Math.PI / 180)) * Math.Sin(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi2 * (Math.PI / 180));
                        transformationMatrix[1, 0] = -1 * Math.Cos(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Psi * (Math.PI / 180)) * Math.Cos(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi1 * (Math.PI / 180)) * Math.Cos(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi2 * (Math.PI / 180));
                        transformationMatrix[1, 0] -= Math.Sin(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi1 * (Math.PI / 180)) * Math.Sin(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi2 * (Math.PI / 180));
                        transformationMatrix[1, 1] = -1 * Math.Sin(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi1 * (Math.PI / 180)) * Math.Cos(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Psi * (Math.PI / 180)) * Math.Cos(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi2 * (Math.PI / 180));
                        transformationMatrix[1, 1] -= Math.Cos(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi1 * (Math.PI / 180)) * Math.Sin(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi2 * (Math.PI / 180));
                        transformationMatrix[1, 2] = Math.Sin(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Psi * (Math.PI / 180)) * Math.Cos(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi2 * (Math.PI / 180));
                        transformationMatrix[2, 0] = Math.Cos(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi1 * (Math.PI / 180)) * Math.Sin(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Psi * (Math.PI / 180));
                        transformationMatrix[2, 1] = Math.Sin(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Phi1 * (Math.PI / 180)) * Math.Sin(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Psi * (Math.PI / 180));
                        transformationMatrix[2, 2] = Math.Cos(actSample.SimulationData[experimentIndex].GrainOrientations[phase][grainIndex].Psi * (Math.PI / 180));

                        //for the self consistence the stress and stress rate are calculated from the last Bc if it exists
                        MathNet.Numerics.LinearAlgebra.Matrix<double> actStressGrainBc = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
                        MathNet.Numerics.LinearAlgebra.Matrix<double> actStressRateGrainBc = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

                        if (actCycle == 0)
                        {
                            for (int k = 0; k < n; k++)
                            {
                                actStressGrainBc += actSample.SimulationData[experimentIndex].StressRateCFOrientedHistory[phase][k][grainIndex];
                            }
                            actStressRateGrainBc = stressRateS;
                        }
                        else
                        {
                            ///constraintStiffness ergibt sich für alle Phasen gleichermaßen und wird aktualisiert
                            ///instGrainTensor wird aus Orientierungen berechnet und muss aus vorherigen Schritt genommen werden
                            ///overallStiffnesses wird pro cycle aktualisiert
                            Tools.FourthRankTensor lastBc = actSample.PlasticTensor[phase].GetAc(constraintStiffness.InverseSC(), plasticTensorPhase[phase][grainIndex].GrainCompliance, overallStiffnesses.InverseSC());

                            //actStressGrainBc = lastBc * actSample.SimulationData[experimentIndex].StressSFHistory[n];
                            for (int k = 0; k < n; k++)
                            {
                                actStressGrainBc += actSample.SimulationData[experimentIndex].StressRateCFOrientedHistory[phase][k][grainIndex];
                            }
                            actStressRateGrainBc = lastBc * stressRateS;
                        }

                        //Spannungstensor in der aktuellen Orientierung
                        MathNet.Numerics.LinearAlgebra.Matrix<double> actStressOriented = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
                        MathNet.Numerics.LinearAlgebra.Matrix<double> actStressRateOriented = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
                        for (int i = 0; i < 3; i++)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                for (int k = 0; k < 3; k++)
                                {
                                    for (int l = 0; l < 3; l++)
                                    {
                                        //actStressOriented[i, j] += transformationMatrix[i, k] * transformationMatrix[j, l] * actStressGrainBc[k, l];
                                        actStressRateOriented[i, j] += transformationMatrix[i, k] * transformationMatrix[j, l] * actStressRateGrainBc[k, l];
                                    }
                                }
                            }
                        }
                        actStressOriented = actStressGrainBc + actStressRateOriented;


                        List<double> potentialActiveStress = actSample.SimulationData[experimentIndex].YieldInformation[phase].GetResolvedStressList(actStressOriented);

                        //Potentiell aktive Gleitsysteme werden ermittelt
                        List<ReflexYield> potentialActive = actSample.SimulationData[experimentIndex].YieldInformation[phase].GetPotentiallyActiveSlipSystems(actStressOriented, slipCriterion);
                        bool noValidCombination = true;

                        if (potentialActive.Count != 0)
                        {
                            noValidCombination = false;
                            allElastic = false;
                            //Liste mit den Testsystemen wird erstell und Indiziert
                            List<ReflexYield> trialSystems = potentialActive;
                            List<int> checkedSystems = new List<int>();
                            for (int i = 0; i < trialSystems.Count; i++)
                            {
                                checkedSystems.Add(0);
                            }

                            //Filterung von Abhängigkeiten
                            actSample.SimulationData[experimentIndex].YieldInformation[phase].CheckDependencies(potentialActive, actStressOriented, checkedSystems);

                            //Schleife für die Finale Selection der Gleitsysteme
                            while (trialSystems.Count != 0)
                            {
                                #region Direction Switch

                                //Check ob die Scherraten positiv sind
                                List<int> checkedSystemsRe = new List<int>();
                                trialSystems = GetActiveSystemCombination(potentialActive, checkedSystems);
                                if (trialSystems.Count == 0)
                                {
                                    noValidCombination = true;
                                    break;
                                }
                                //bool resetSystems = false;
                                //for (int m = 0; m < checkedSystems.Count; m++)
                                //{
                                //    switch (checkedSystems[m])
                                //    {
                                //        case 0:
                                //            goto default;
                                //        case 1:
                                //            checkedSystemsRe.Add(1);
                                //            break;
                                //        case 2:
                                //            checkedSystemsRe.Add(2);
                                //            break;
                                //        default:
                                //            resetSystems = true;
                                //            break;
                                //    }
                                //}
                                //checkedSystems = checkedSystemsRe;

                                //if (resetSystems)
                                //{
                                //    for (int m = 0; m < checkedSystems.Count; m++)
                                //    {
                                //        checkedSystems[m] = 1;
                                //    }
                                //}

                                #endregion

                                //Härtungsmatrix wird berechnet
                                plasticTensorGrain.HardeningMatrix = actSample.SimulationData[experimentIndex].YieldInformation[phase].HardeningMatrixSlipSystem(trialSystems);

                                //Shear rates
                                List<double> shearRatesSystems = new List<double>();
                                //shear stress change rates
                                List<double> shearStressChangeSystems = new List<double>();
                                //yield change rates
                                List<double> yieldChangeSystems = new List<double>();

                                //Berechnung der instantanious Stiffness factors f^i
                                switch (elasticModel)
                                {
                                    case 0:
                                        //Reuss
                                        plasticTensorGrain.ConditionX = actSample.SimulationData[experimentIndex].YieldInformation[phase].SlipSystemX(trialSystems, plasticTensorGrain.HardeningMatrix, actSample.ReussTensorData[phase]);
                                        for (int i = 0; i < trialSystems.Count; i++)
                                        {
                                            plasticTensorGrain.InstantStiffnessFactors.Add(actSample.SimulationData[experimentIndex].YieldInformation[phase].GetInstStiffnessFactors(trialSystems, actSample.ReussTensorData[phase], i, plasticTensorGrain.ConditionY));
                                        }
                                        //Lc und Mc 
                                        plasticTensorGrain.GrainStiffness = actSample.PlasticTensor[phase].GetInstantanousGrainSiffnessTensor(trialSystems, actSample.ReussTensorData[phase], plasticTensorGrain.InstantStiffnessFactors);
                                        break;
                                    case 1:
                                        //Hill
                                        plasticTensorGrain.ConditionX = actSample.SimulationData[experimentIndex].YieldInformation[phase].SlipSystemX(trialSystems, plasticTensorGrain.HardeningMatrix, actSample.HillTensorData[phase]);
                                        for (int i = 0; i < trialSystems.Count; i++)
                                        {
                                            plasticTensorGrain.InstantStiffnessFactors.Add(actSample.SimulationData[experimentIndex].YieldInformation[phase].GetInstStiffnessFactors(trialSystems, actSample.HillTensorData[phase], i, plasticTensorGrain.ConditionY));
                                        }
                                        //Lc und Mc 
                                        plasticTensorGrain.GrainStiffness = actSample.PlasticTensor[phase].GetInstantanousGrainSiffnessTensor(trialSystems, actSample.HillTensorData[phase], plasticTensorGrain.InstantStiffnessFactors);
                                        break;
                                    case 2:
                                        //Kroener
                                        plasticTensorGrain.ConditionX = actSample.SimulationData[experimentIndex].YieldInformation[phase].SlipSystemX(trialSystems, plasticTensorGrain.HardeningMatrix, actSample.KroenerTensorData[phase]);
                                        for (int i = 0; i < trialSystems.Count; i++)
                                        {
                                            plasticTensorGrain.InstantStiffnessFactors.Add(actSample.SimulationData[experimentIndex].YieldInformation[phase].GetInstStiffnessFactors(trialSystems, actSample.KroenerTensorData[phase], i, plasticTensorGrain.ConditionY));
                                        }
                                        //Lc und Mc 
                                        plasticTensorGrain.GrainStiffness = actSample.PlasticTensor[phase].GetInstantanousGrainSiffnessTensor(trialSystems, actSample.KroenerTensorData[phase], plasticTensorGrain.InstantStiffnessFactors);
                                        break;
                                    case 3:
                                        //De Wit
                                        plasticTensorGrain.ConditionX = actSample.SimulationData[experimentIndex].YieldInformation[phase].SlipSystemX(trialSystems, plasticTensorGrain.HardeningMatrix, actSample.DeWittTensorData[phase]);
                                        for (int i = 0; i < trialSystems.Count; i++)
                                        {
                                            plasticTensorGrain.InstantStiffnessFactors.Add(actSample.SimulationData[experimentIndex].YieldInformation[phase].GetInstStiffnessFactors(trialSystems, actSample.DeWittTensorData[phase], i, plasticTensorGrain.ConditionY));
                                        }
                                        //Lc und Mc 
                                        plasticTensorGrain.GrainStiffness = actSample.PlasticTensor[phase].GetInstantanousGrainSiffnessTensor(trialSystems, actSample.DeWittTensorData[phase], plasticTensorGrain.InstantStiffnessFactors);
                                        break;
                                    case 4:
                                        //Matthies
                                        plasticTensorGrain.ConditionX = actSample.SimulationData[experimentIndex].YieldInformation[phase].SlipSystemX(trialSystems, plasticTensorGrain.HardeningMatrix, actSample.GeometricHillTensorData[phase]);
                                        for (int i = 0; i < trialSystems.Count; i++)
                                        {
                                            plasticTensorGrain.InstantStiffnessFactors.Add(actSample.SimulationData[experimentIndex].YieldInformation[phase].GetInstStiffnessFactors(trialSystems, actSample.GeometricHillTensorData[phase], i, plasticTensorGrain.ConditionY));
                                        }
                                        //Lc und Mc 
                                        plasticTensorGrain.GrainStiffness = actSample.PlasticTensor[phase].GetInstantanousGrainSiffnessTensor(trialSystems, actSample.GeometricHillTensorData[phase], plasticTensorGrain.InstantStiffnessFactors);
                                        break;
                                    default:
                                        //Hill
                                        plasticTensorGrain.ConditionX = actSample.SimulationData[experimentIndex].YieldInformation[phase].SlipSystemX(trialSystems, plasticTensorGrain.HardeningMatrix, actSample.HillTensorData[phase]);
                                        for (int i = 0; i < trialSystems.Count; i++)
                                        {
                                            plasticTensorGrain.InstantStiffnessFactors.Add(actSample.SimulationData[experimentIndex].YieldInformation[phase].GetInstStiffnessFactors(trialSystems, actSample.HillTensorData[phase], i, plasticTensorGrain.ConditionY));
                                        }
                                        //Lc und Mc 
                                        plasticTensorGrain.GrainStiffness = actSample.PlasticTensor[phase].GetInstantanousGrainSiffnessTensor(trialSystems, actSample.HillTensorData[phase], plasticTensorGrain.InstantStiffnessFactors);
                                        break;
                                }


                                //InversionCheck, ob alles richtig Invertiert wurde
                                Tools.FourthRankTensor invCheckTensor = Tools.FourthRankTensor.InnerProduct(plasticTensorGrain.GrainStiffness, plasticTensorGrain.GrainCompliance);
                                double invDifference = invCheckTensor.GetDifference(Tools.FourthRankTensor.GetUnityTensor());
                                if (invDifference < 0.0001)
                                {
                                    //berechung von Ac
                                    plasticTensorGrain.GrainTransitionStiffness = actSample.PlasticTensor[phase].GetAc(constraintStiffness, plasticTensorGrain.GrainStiffness, overallStiffnesses);
                                    //bzw Bc
                                    plasticTensorGrain.GrainTransitionCompliance = actSample.PlasticTensor[phase].GetAc(constraintStiffness.InverseSC(), plasticTensorGrain.GrainCompliance, overallStiffnesses.InverseSC());

                                    MathNet.Numerics.LinearAlgebra.Matrix<double> stressRateGrainNo = plasticTensorGrain.GrainTransitionCompliance * stressRateS;
                                    //MathNet.Numerics.LinearAlgebra.Matrix<double> stressRateGrain = plasticTensorGrain.GrainTransitionCompliance * actStressOriented;
                                    MathNet.Numerics.LinearAlgebra.Matrix<double> stressRateGrain = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense<double>(3, 3, 0); ;
                                    for (int i = 0; i < 3; i++)
                                    {
                                        for (int j = 0; j < 3; j++)
                                        {
                                            for (int k = 0; k < 3; k++)
                                            {
                                                for (int l = 0; l < 3; l++)
                                                {
                                                    stressRateGrain[i, j] += transformationMatrix[i, k] * transformationMatrix[j, l] * stressRateGrainNo[k, l];
                                                }
                                            }
                                        }
                                    }

                                    MathNet.Numerics.LinearAlgebra.Matrix<double> strainRateGrain = plasticTensorGrain.GrainCompliance * stressRateGrain;

                                    //Berechnung der shear rates und der spannungsänderung
                                    for (int i = 0; i < trialSystems.Count; i++)
                                    {
                                        double actShearRate = 0.0;
                                        double shearstress = 0.0;
                                        MathNet.Numerics.LinearAlgebra.Matrix<double> resolvingMatrix = Tools.Calculation.GetResolvingParameter(trialSystems[i].SlipPlane, trialSystems[i].MainSlipDirection);

                                        for (int j = 0; j < 3; j++)
                                        {
                                            for (int k = 0; k < 3; k++)
                                            {
                                                actShearRate += plasticTensorGrain.InstantStiffnessFactors[i][j, k] * strainRateGrain[j, k];
                                                shearstress += stressRateGrain[j, k] * resolvingMatrix[j, k];
                                            }
                                        }

                                        shearRatesSystems.Add(actShearRate);
                                        shearStressChangeSystems.Add(shearstress);
                                    }

                                    bool negativeShear = false;

                                    #region direction check

                                    ////Checkt ob die Scherraten positiv sind und markiert diejenigen, deren Gleitrichtung umgedreht werden muss
                                    //for (int sys = 0; sys < shearRatesSystems.Count; sys++)
                                    //{
                                    //    //Check ob die scherraten positiv oder negativ sind
                                    //    if (shearRatesSystems[sys] < 0)
                                    //    {
                                    //        //Falls negativ wird die Gleitrichtung umgedreht
                                    //        if (checkedSystems[sys] == 1)
                                    //        {
                                    //            checkedSystems[sys] = 2;
                                    //        }
                                    //        else if (checkedSystems[sys] == 2)
                                    //        {
                                    //            checkedSystems[sys] = 0;
                                    //        }
                                    //        negativeShear = true;
                                    //    }
                                    //}

                                    #endregion

                                    if (!negativeShear)
                                    {
                                        //Berechung der yield changes
                                        for (int i = 0; i < trialSystems.Count; i++)
                                        {
                                            double yieldChangeTmp = 0.0;
                                            for (int j = 0; j < trialSystems.Count; j++)
                                            {
                                                yieldChangeTmp += plasticTensorGrain.HardeningMatrix[i, j] * shearRatesSystems[j];
                                            }
                                            yieldChangeSystems.Add(yieldChangeTmp);
                                        }

                                        //Vergleich zwischen shear rates und yield changes
                                        double difference = 0.0;
                                        double norm = 0.0;
                                        for (int i = 0; i < trialSystems.Count; i++)
                                        {
                                            difference = (Math.Abs(yieldChangeSystems[i]) - Math.Abs(shearStressChangeSystems[i])) / Math.Abs(yieldChangeSystems[i]);
                                            norm += Math.Abs(yieldChangeSystems[i]);
                                        }

                                        difference /= norm;

                                        if (difference < 1)
                                        {
                                            //Speichern der Parameter
                                            potentialActiveGPhase[grainIndex] = trialSystems;
                                            grainStrainsPhase[grainIndex] = strainRateGrain;
                                            grainStressesPhase[grainIndex] = stressRateGrain;
                                            yieldChangePhase[grainIndex] = yieldChangeSystems;

                                            plasticTensororiented[grainIndex] = plasticTensorGrain;

                                            actSample.SimulationData[experimentIndex].LastActiveSystems += trialSystems.Count;
                                            break;
                                        }
                                        else
                                        {
                                            //System mit kleinsten Shearstress wird herausgesucht
                                            int smallestIndex = actSample.SimulationData[experimentIndex].YieldInformation[phase].GetSmallesSchearStressIndex(trialSystems, actStressOriented);

                                            //System mit kleinsten Shearstress wird aussortiert
                                            for (int k = 0; k < potentialActive.Count; k++)
                                            {
                                                if (potentialActive[k].SlipPlane.HKLString == trialSystems[smallestIndex].SlipPlane.HKLString && potentialActive[k].MainSlipDirection.HKLString == trialSystems[smallestIndex].MainSlipDirection.HKLString)
                                                {
                                                    checkedSystems[k] = 3;
                                                    break;
                                                }
                                            }

                                            //Das nächste noch nicht getestete System wird aktiviert
                                            for (int k = 0; k < checkedSystems.Count; k++)
                                            {
                                                if (checkedSystems[k] == 0)
                                                {
                                                    checkedSystems[k] = 1;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    //System mit kleinsten Shearstress wird herausgesucht
                                    int smallestIndex = actSample.SimulationData[experimentIndex].YieldInformation[phase].GetSmallesSchearStressIndex(trialSystems, actStressOriented);

                                    //System mit kleinsten Shearstress wird aussortiert
                                    for (int k = 0; k < potentialActive.Count; k++)
                                    {
                                        if (potentialActive[k].SlipPlane.HKLString == trialSystems[smallestIndex].SlipPlane.HKLString && potentialActive[k].MainSlipDirection.HKLString == trialSystems[smallestIndex].MainSlipDirection.HKLString)
                                        {
                                            checkedSystems[k] = 3;
                                            break;
                                        }
                                    }

                                    //Das nächste noch nicht getestete System wird aktiviert
                                    for (int k = 0; k < checkedSystems.Count; k++)
                                    {
                                        if (checkedSystems[k] == 0)
                                        {
                                            checkedSystems[k] = 1;
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (noValidCombination)
                        {
                            actSample.SimulationData[experimentIndex].LastFailedGrains++;
                            switch (elasticModel)
                            {
                                case 0:
                                    //Reuss
                                    plasticTensorGrain.GrainStiffness = actSample.ReussTensorData[phase].GetFourtRankStiffnesses();
                                    break;
                                case 1:
                                    //Hill
                                    plasticTensorGrain.GrainStiffness = actSample.HillTensorData[phase].GetFourtRankStiffnesses();
                                    break;
                                case 2:
                                    //Kroener
                                    plasticTensorGrain.GrainStiffness = actSample.KroenerTensorData[phase].GetFourtRankStiffnesses();
                                    break;
                                case 3:
                                    //De Wit
                                    plasticTensorGrain.GrainStiffness = actSample.DeWittTensorData[phase].GetFourtRankStiffnesses();
                                    break;
                                case 4:
                                    //Matthies
                                    plasticTensorGrain.GrainStiffness = actSample.GeometricHillTensorData[phase].GetFourtRankStiffnesses();
                                    break;
                                default:
                                    //Hill
                                    plasticTensorGrain.GrainStiffness = actSample.HillTensorData[phase].GetFourtRankStiffnesses();
                                    break;
                            }

                            //berechung von Ac
                            plasticTensorGrain.GrainTransitionStiffness = actSample.PlasticTensor[phase].GetAc(constraintStiffness, plasticTensorGrain.GrainStiffness, overallStiffnesses);
                            //bzw Bc
                            plasticTensorGrain.GrainTransitionCompliance = actSample.PlasticTensor[phase].GetAc(constraintStiffness.InverseSC(), plasticTensorGrain.GrainCompliance, overallStiffnesses.InverseSC());

                            MathNet.Numerics.LinearAlgebra.Matrix<double> stressRateGrainSave = plasticTensorGrain.GrainTransitionCompliance * stressRateS;
                            MathNet.Numerics.LinearAlgebra.Matrix<double> stressRateOrientedSave = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
                            for (int i = 0; i < 3; i++)
                            {
                                for (int j = 0; j < 3; j++)
                                {
                                    for (int k = 0; k < 3; k++)
                                    {
                                        for (int l = 0; l < 3; l++)
                                        {
                                            stressRateOrientedSave[i, j] += transformationMatrix[i, k] * transformationMatrix[j, l] * stressRateGrainSave[k, l];
                                        }
                                    }
                                }
                            }
                            MathNet.Numerics.LinearAlgebra.Matrix<double> strainRateGrainSave = plasticTensorGrain.GrainCompliance * stressRateOrientedSave;

                            //Speichern der Parameter
                            potentialActiveGPhase[grainIndex] = new List<ReflexYield>();
                            grainStrainsPhase[grainIndex] = strainRateGrainSave;
                            grainStressesPhase[grainIndex] = stressRateOrientedSave;
                            yieldChangePhase[grainIndex] = new List<double>();

                            plasticTensororiented[grainIndex] = plasticTensorGrain;
                        }

                    }//); // Parallel.For

                    //Überschreiben der Phasendaten
                    plasticTensorPhase[phase] = plasticTensororiented.ToList();

                    potentialActiveGOriented[phase] = potentialActiveGPhase.ToList();
                    grainStressesOriented[phase] = grainStressesPhase.ToList();
                    grainStrainsOriented[phase] = grainStrainsPhase.ToList();
                    yieldChangeOriented[phase] = yieldChangePhase.ToList();

                    //Mittelung des neuen Phasenmoduls
                    if (textureActive)
                    {
                        overallStiffnessesPhase[phase] = Tools.FourthRankTensor.AverageInnerProduct(plasticTensorPhase[phase], actSample.SimulationData[experimentIndex].GrainOrientations[phase]);
                        overallStiffnessesPhase[phase].SetHexagonalSymmetryCorrection();
                    }
                    else
                    {
                        overallStiffnessesPhase[phase] = Tools.FourthRankTensor.AverageInnerProduct(plasticTensorPhase[phase]);
                        overallStiffnessesPhase[phase].SetHexagonalSymmetryCorrection();
                    }

                    //Mittelung der neuen Fließgrenze PotentialSlipSystems
                    averageYieldChange[phase] = new double[actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems.Count];

                    for (int i = 0; i < potentialActiveGOriented[phase].Count; i++)
                    {
                        for (int j = 0; j < potentialActiveGOriented[phase][i].Count; j++)
                        {
                            for (int k = 0; k < actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems.Count; k++)
                            {
                                if (potentialActiveGOriented[phase][i][j].HKLString == actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[k].HKLString && potentialActiveGOriented[phase][i][j].HKLStringSlipDirection == actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[k].HKLStringSlipDirection)
                                {
                                    averageYieldChange[phase][k] += Math.Abs(yieldChangeOriented[phase][i][j]);
                                    //if (actSample.SimulationData[experimentIndex].invertSlipDirections)
                                    //{

                                    //    averageYieldChange[phase][k] += Math.Abs(yieldChangeOriented[phase][i][j]);
                                    //}
                                    //else
                                    //{
                                    //    averageYieldChange[phase][k] += yieldChangeOriented[phase][i][j];
                                    //}
                                }
                            }
                        }
                    }
                }
                //Ab hier berechung der Makrodaten
                //Resetting the overall Stiffnesses

                overallStiffnesses = new Tools.FourthRankTensor();
                for (int phase = 0; phase < actSample.CrystalData.Count; phase++)
                {
                    overallStiffnesses += actSample.CrystalData[phase].PhaseFraction * overallStiffnessesPhase[phase];
                }

                MathNet.Numerics.LinearAlgebra.Matrix<double> energyCheckMat = overallStiffnesses.GetVoigtTensor();
                bool energyCheck = true;

                //Energy check for the calculated constants
                if (energyCheckMat[3, 3] <= 0)
                {
                    energyCheck = false;
                }
                else if (energyCheckMat[0, 0] <= Math.Abs(energyCheckMat[0, 1]))
                {
                    energyCheck = false;
                }
                else if ((energyCheckMat[0, 0] + energyCheckMat[0, 1]) * energyCheckMat[2, 2] <= 0)
                {
                    energyCheck = false;
                }

                if (energyCheck)
                {
                    double overallDifference = overallStiffnesses.GetDifference(overallStiffnessesComp);

                    MathNet.Numerics.LinearAlgebra.Matrix<double> DebugComp = overallStiffnessesComp.GetVoigtTensor();
                    MathNet.Numerics.LinearAlgebra.Matrix<double> DebugDiff = DebugComp - energyCheckMat;
                    if (overallDifference < CalScec.Properties.Settings.Default.EPSCSimulationLimit)
                    {
                        break;
                    }
                    else
                    {
                        overallStiffnessesComp = overallStiffnesses.Clone() as Tools.FourthRankTensor;
                    }
                }
                else
                {
                    overallStiffnesses = overallStiffnessesComp;
                    break;
                }
            }

            //Abspeichern der Daten
            //All base tensor for the Grains, sample tensor are set
            //the stress states for the sample and phases will be calculated
            //Parameter Reihenfolge:
            //1. Spannung Probe (Berechnet aus der Spannungssrate der Probe)
            //2. Spannungsrate Probe (berechnet)
            //3. Dehnungsrate Probe (über overallStiffnesses gerechnet)
            //4. Dehnung Probe (Eingabe)
            //5. Bestimmung der Phasenparameter
            //Alles auf grain lvl wird nach des Konvergenz checks berechnet und gesetzt
            //6. Dehnungsraten der grains (berechnet über die transition Matrix)
            //7. Dehnung der grains (berechnet über die Dehnrate)
            //8. Berechnung der Spannungsrate im Grain (Dehnrate aus 6. - Plastischer Dehnrate --> Elastische konstanten)
            //9. Berechung der Aktuellen Spannung analog zu 7.
            //10. Aktive Gleitsysteme

            // 1
            //actSample.SimulationData[experimentIndex].StressSFHistory.Add(stressS.Clone());
            //2. 
            actSample.SimulationData[experimentIndex].StressRateSFHistory.Add(stressRateS.Clone());

            //3.
            MathNet.Numerics.LinearAlgebra.Matrix<double> strainRateSample = overallStiffnesses.InverseSC() * stressRateS;
            actSample.SimulationData[experimentIndex].StrainRateSFHistory.Add(strainRateSample);

            //4.
            MathNet.Numerics.LinearAlgebra.Matrix<double> actStrainS = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
            if (actSample.SimulationData[experimentIndex].StrainSFHistory.Count == 0)
            {
                actStrainS = strainRateSample;
            }
            else
            {
                actStrainS = actSample.SimulationData[experimentIndex].StrainSFHistory[actSample.SimulationData[experimentIndex].StrainSFHistory.Count - 1] + strainRateSample;
            }
            actSample.SimulationData[experimentIndex].StrainSFHistory.Add(actStrainS.Clone());


            //YieldChange yieldChangeCFHistory
            actSample.SimulationData[experimentIndex].YieldChangeCFHistory.Add(yieldChangeOriented);

            //5.
            for (int phase = 0; phase < actSample.CrystalData.Count; phase++)
            {
                ////8.
                actSample.SimulationData[experimentIndex].StressRateCFOrientedHistory[phase].Add(grainStressesOriented[phase]);
                ////6.
                actSample.SimulationData[experimentIndex].StrainRateCFOrientedHistory[phase].Add(grainStrainsOriented[phase]);
                ////10.
                actSample.SimulationData[experimentIndex].ActiveSystemsCFOrientedHistory[phase].Add(potentialActiveGOriented[phase]);
                ////11. Härtung berechnen (Anzeige)
                for (int k = 0; k < actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems.Count; k++)
                {
                    if (singleCrystalTracking)
                    {
                        actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[k].YieldMainAvgHardenning = averageYieldChange[phase][k] / potentialActiveGOriented[phase].Count;
                    }
                    else
                    {
                        actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[k].YieldMainAvgHardenning += averageYieldChange[phase][k] / potentialActiveGOriented[phase].Count;
                    }
                }
            }

            //if (correctArea)
            //{
            //    double radius = Math.Sqrt(actSample.SimulationData[experimentIndex].SampleArea / Math.PI);

            //    double ellipseA = radius * (1 + actStrainS[0, 0]);
            //    double ellipseB = radius * (1 + actStrainS[1, 1]);

            //    double newArea = Math.PI * ellipseA * ellipseB;
            //    double appliedForce = actSample.SimulationData[experimentIndex].StressSFHistory[n][2, 2] * actSample.SimulationData[experimentIndex].SampleArea;

            //    MathNet.Numerics.LinearAlgebra.Matrix<double> newStressTensor = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense<double>(3, 3, 0);
            //    newStressTensor[2, 2] = appliedForce / newArea;

            //    actSample.SimulationData[experimentIndex].StressSFHistoryAreaCorrected.Add(newStressTensor);
            //}

        }
        



        public static List<ReflexYield> GetActiveSystemCombination(List<ReflexYield> potentialSystems, long combination)
        {
            //List<List<bool>> indexList = new List<List<bool>>();
            //for (int n = Convert.ToInt32(Math.Pow(2, potentialSystems.Count)) - 1; n >= 0; n--)
            //{
            //    byte[] val = BitConverter.GetBytes(n);
            //    BitArray t = new BitArray(val);
            //    bool[] forIndex = new bool[32];
            //    t.CopyTo(forIndex, 0);
            //    if (!BitConverter.IsLittleEndian)
            //    {
            //        Array.Reverse(forIndex); //IMPORTANT!
            //    }
            //    indexList.Add(forIndex.ToList());
            //}

            //List<ReflexYield> ret = new List<ReflexYield>();
            //if (indexList.Count > combination)
            //{
            //    for (int m = 0; m < potentialSystems.Count; m++)
            //    {
            //        if (indexList[combination][m])
            //        {
            //            ret.Add(potentialSystems[m]);
            //        }
            //    }
            //}
            List<ReflexYield> ret = new List<ReflexYield>();

            long index = Convert.ToInt64(Math.Pow(2, potentialSystems.Count)) - 1 - combination;
            byte[] val = BitConverter.GetBytes(index);
            BitArray t = new BitArray(val);
            bool[] forIndex = new bool[64];
            t.CopyTo(forIndex, 0);
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(forIndex); //IMPORTANT!
            }

            for (int m = 0; m < potentialSystems.Count; m++)
            {
                if (forIndex[m])
                {
                    ret.Add(potentialSystems[m]);
                }
            }

            return ret;
        }
        public static List<ReflexYield> GetActiveSystemCombination(List<ReflexYield> potentialSystems, List<bool> combination)
        {
            List<ReflexYield> ret = new List<ReflexYield>();

            for (int m = 0; m < combination.Count; m++)
            {
                if (combination[m])
                {
                    ret.Add(potentialSystems[m]);
                }
            }

            return ret;
        }
        public static List<ReflexYield> GetActiveSystemCombination(List<ReflexYield> potentialSystems, List<int> combination)
        {
            List<ReflexYield> ret = new List<ReflexYield>();
            for (int m = 0; m < combination.Count; m++)
            {
                switch(combination[m])
                {
                    case 0:
                        goto default;
                    case 3:
                        goto default;
                    case 1:
                        ret.Add(potentialSystems[m]);
                        break;
                    case 2:
                        potentialSystems[m].MainSlipDirection.H *= -1;
                        potentialSystems[m].MainSlipDirection.K *= -1;
                        potentialSystems[m].MainSlipDirection.L *= -1;
                        ret.Add(potentialSystems[m]);
                        break;
                    default:
                        break;
                }
            }
            return ret;
        }


        public static void PerfomStandardExperimentStep(Sample actSample, bool textureActive, int experimentIndex, int n, MathNet.Numerics.LinearAlgebra.Matrix<double> actStrainS, List<MathNet.Numerics.LinearAlgebra.Matrix<double>> actStrainG, List<MathNet.Numerics.LinearAlgebra.Matrix<double>> actStressG, bool useHardenningMatrix, bool correctArea)
        {
            //setzen der einzelnen Parameter
            List<List<ReflexYield>> potentialActiveG = new List<List<ReflexYield>>();

            //Spannungsrate berechnen der angelegten Probenspannungsrate
            MathNet.Numerics.LinearAlgebra.Matrix<double> stressRateS = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
            if (n == 0)
            {
                stressRateS[0, 0] += actSample.SimulationData[experimentIndex].StressSFHistory[n][0, 0];
                stressRateS[1, 0] += actSample.SimulationData[experimentIndex].StressSFHistory[n][1, 0];
                stressRateS[2, 0] += actSample.SimulationData[experimentIndex].StressSFHistory[n][2, 0];
                stressRateS[0, 1] += actSample.SimulationData[experimentIndex].StressSFHistory[n][0, 1];
                stressRateS[1, 1] += actSample.SimulationData[experimentIndex].StressSFHistory[n][1, 1];
                stressRateS[2, 1] += actSample.SimulationData[experimentIndex].StressSFHistory[n][2, 1];
                stressRateS[0, 2] += actSample.SimulationData[experimentIndex].StressSFHistory[n][0, 2];
                stressRateS[1, 2] += actSample.SimulationData[experimentIndex].StressSFHistory[n][1, 2];
                stressRateS[2, 2] += actSample.SimulationData[experimentIndex].StressSFHistory[n][2, 2];

                if (correctArea)
                {
                    actSample.SimulationData[experimentIndex].StressSFHistoryAreaCorrected.Add(actSample.SimulationData[experimentIndex].StressSFHistory[n].Clone());
                }
            }
            else
            {
                if (correctArea)
                {
                    stressRateS[0, 0] += actSample.SimulationData[experimentIndex].StressSFHistoryAreaCorrected[n][0, 0] - actSample.SimulationData[experimentIndex].StressSFHistoryAreaCorrected[n - 1][0, 0];
                    stressRateS[1, 0] += actSample.SimulationData[experimentIndex].StressSFHistoryAreaCorrected[n][1, 0] - actSample.SimulationData[experimentIndex].StressSFHistoryAreaCorrected[n - 1][1, 0];
                    stressRateS[2, 0] += actSample.SimulationData[experimentIndex].StressSFHistoryAreaCorrected[n][2, 0] - actSample.SimulationData[experimentIndex].StressSFHistoryAreaCorrected[n - 1][2, 0];
                    stressRateS[0, 1] += actSample.SimulationData[experimentIndex].StressSFHistoryAreaCorrected[n][0, 1] - actSample.SimulationData[experimentIndex].StressSFHistoryAreaCorrected[n - 1][0, 1];
                    stressRateS[1, 1] += actSample.SimulationData[experimentIndex].StressSFHistoryAreaCorrected[n][1, 1] - actSample.SimulationData[experimentIndex].StressSFHistoryAreaCorrected[n - 1][1, 1];
                    stressRateS[2, 1] += actSample.SimulationData[experimentIndex].StressSFHistoryAreaCorrected[n][2, 1] - actSample.SimulationData[experimentIndex].StressSFHistoryAreaCorrected[n - 1][2, 1];
                    stressRateS[0, 2] += actSample.SimulationData[experimentIndex].StressSFHistoryAreaCorrected[n][0, 2] - actSample.SimulationData[experimentIndex].StressSFHistoryAreaCorrected[n - 1][0, 2];
                    stressRateS[1, 2] += actSample.SimulationData[experimentIndex].StressSFHistoryAreaCorrected[n][1, 2] - actSample.SimulationData[experimentIndex].StressSFHistoryAreaCorrected[n - 1][1, 2];
                    stressRateS[2, 2] += actSample.SimulationData[experimentIndex].StressSFHistoryAreaCorrected[n][2, 2] - actSample.SimulationData[experimentIndex].StressSFHistoryAreaCorrected[n - 1][2, 2];
                }
                else
                {
                    stressRateS[0, 0] += actSample.SimulationData[experimentIndex].StressSFHistory[n][0, 0] - actSample.SimulationData[experimentIndex].StressSFHistory[n - 1][0, 0];
                    stressRateS[1, 0] += actSample.SimulationData[experimentIndex].StressSFHistory[n][1, 0] - actSample.SimulationData[experimentIndex].StressSFHistory[n - 1][1, 0];
                    stressRateS[2, 0] += actSample.SimulationData[experimentIndex].StressSFHistory[n][2, 0] - actSample.SimulationData[experimentIndex].StressSFHistory[n - 1][2, 0];
                    stressRateS[0, 1] += actSample.SimulationData[experimentIndex].StressSFHistory[n][0, 1] - actSample.SimulationData[experimentIndex].StressSFHistory[n - 1][0, 1];
                    stressRateS[1, 1] += actSample.SimulationData[experimentIndex].StressSFHistory[n][1, 1] - actSample.SimulationData[experimentIndex].StressSFHistory[n - 1][1, 1];
                    stressRateS[2, 1] += actSample.SimulationData[experimentIndex].StressSFHistory[n][2, 1] - actSample.SimulationData[experimentIndex].StressSFHistory[n - 1][2, 1];
                    stressRateS[0, 2] += actSample.SimulationData[experimentIndex].StressSFHistory[n][0, 2] - actSample.SimulationData[experimentIndex].StressSFHistory[n - 1][0, 2];
                    stressRateS[1, 2] += actSample.SimulationData[experimentIndex].StressSFHistory[n][1, 2] - actSample.SimulationData[experimentIndex].StressSFHistory[n - 1][1, 2];
                    stressRateS[2, 2] += actSample.SimulationData[experimentIndex].StressSFHistory[n][2, 2] - actSample.SimulationData[experimentIndex].StressSFHistory[n - 1][2, 2];
                }
            }

            //Vorbereitung der Daten für die Spannungsrate in grains ausgerichtet nach der Probenorientierung
            List<MathNet.Numerics.LinearAlgebra.Matrix<double>> actStressRateG = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
            List<MathNet.Numerics.LinearAlgebra.Matrix<double>> actStressRateGA = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

            //Vorbereitung der Dehnraten im elastischen und Plastischen Bereich
            List<MathNet.Numerics.LinearAlgebra.Matrix<double>> actStrainRatePlG = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

            //Elastische Tensoren
            Tools.FourthRankTensor overallStiffnesses = actSample.GetSampleStiffnesses(textureActive);
            Tools.FourthRankTensor overallStiffnessesComp = new Tools.FourthRankTensor();
            //Tools.FourthRankTensor Stiffnesses = eT.GetFourtRankStiffnesses();

            List<Tools.FourthRankTensor> constraintStiffness = new List<Tools.FourthRankTensor>();
            List<List<Tools.FourthRankTensor>> grainStiffnesses = new List<List<Tools.FourthRankTensor>>();
            List<List<Tools.FourthRankTensor>> grainTransitionStiffnesses = new List<List<Tools.FourthRankTensor>>();

            //gemittelten Phasen nachgiebigkeiten werden erstellt
            List<Tools.FourthRankTensor> overallStiffnessesPhase = new List<Tools.FourthRankTensor>();

            //fi
            List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>> stiffnessFactors = new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>();
            List<List <List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>> stiffnessFactorsOrientedAll = new List<List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>>();
            List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>> hardeningMatrixList = new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>();

            //Ausfüllen der Grain parameter für die verschiedenen Phasen
            for (int phase = 0; phase < actSample.CrystalData.Count; phase++)
            {
                actStrainRatePlG.Add(MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0));

                actStressRateG.Add(MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0));

                overallStiffnessesPhase.Add(new Tools.FourthRankTensor());
                constraintStiffness.Add(new Tools.FourthRankTensor());
                grainStiffnesses.Add(new List<Tools.FourthRankTensor>());
                grainTransitionStiffnesses.Add(new List<Tools.FourthRankTensor>());

                potentialActiveG.Add(new List<ReflexYield>());

                stiffnessFactors.Add(new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>());
                stiffnessFactorsOrientedAll.Add(new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>());
                hardeningMatrixList.Add(new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>());
            }

            bool sCCompletted = false;
            int actCircle = 0;

            while (!sCCompletted)
            {
                //Berechnungen im Krystallsystem
                for (int phase = 0; phase < actSample.CrystalData.Count; phase++)
                {
                    double maxPhi1 = 360;
                    double maxPsi = 360;
                    double maxPhi2 = 360;

                    //The constrain Tensor L* for cubic isotropic materials

                    Tools.FourthRankTensor constraintTensor = actSample.PlasticTensor[phase].GetConstraintStiffnessCubicIsotropic(actSample.HillTensorData[phase], 2);

                    if (actSample.CrystalData[phase].SymmetryGroupID == 225 || actSample.CrystalData[phase].SymmetryGroupID == 229)
                    {
                        maxPhi1 = 45;
                        maxPsi = 45;
                        maxPhi2 = 45;

                        //The constrain Tensor L* for cubic isotropic materials
                        constraintTensor = actSample.PlasticTensor[phase].GetConstraintStiffnessCubicIsotropic(actSample.HillTensorData[phase], 2);
                    }
                    if (actSample.CrystalData[phase].SymmetryGroupID == 194)
                    {
                        maxPhi1 = 60;
                        maxPsi = 90;
                        maxPhi2 = 60;
                    }

                    constraintStiffness[phase] = constraintTensor;

                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> StressCFPhase = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

                    List<Tools.FourthRankTensor> grainInstStiffnessesOriented = new List<Tools.FourthRankTensor>();
                    List<Tools.FourthRankTensor> grainTransitionStiffnessesOriented = new List<Tools.FourthRankTensor>();

                    List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>> stiffnessFactorsOrientedPhase = new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>();

                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> hardeningMatrixOriented = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

                    for (double phi1 = 0.0; phi1 < maxPhi1; phi1 += 5.0)
                    {
                        for (double psi = 0.0; psi < maxPsi; psi += 5.0)
                        {
                            for (double phi2 = 0.0; phi2 < maxPhi2; phi2 += 5.0)
                            {
                                MathNet.Numerics.LinearAlgebra.Matrix<double> transformationMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense<double>(3, 3, 0);

                                transformationMatrix[0, 0] = -1 * Math.Cos(phi1) * Math.Cos(psi) * Math.Sin(phi2);
                                transformationMatrix[0, 0] -= Math.Sin(phi1) * Math.Cos(phi2);

                                transformationMatrix[0, 1] = -1 * Math.Cos(psi) * Math.Sin(phi1) * Math.Sin(phi2);
                                transformationMatrix[0, 1] -= Math.Cos(phi1) * Math.Cos(phi2);

                                transformationMatrix[0, 2] = Math.Sin(psi) * Math.Sin(phi2);

                                transformationMatrix[1, 0] = -1 * Math.Cos(psi) * Math.Cos(phi1) * Math.Cos(phi2);
                                transformationMatrix[1, 0] -= Math.Sin(phi1) * Math.Sin(phi2);

                                transformationMatrix[1, 1] = -1 * Math.Sin(phi1) * Math.Cos(psi) * Math.Cos(phi2);
                                transformationMatrix[1, 1] -= Math.Cos(phi1) * Math.Sin(phi2);

                                transformationMatrix[1, 2] = Math.Sin(psi) * Math.Cos(phi2);

                                transformationMatrix[2, 0] = Math.Cos(phi1) * Math.Sin(psi);

                                transformationMatrix[2, 1] = Math.Sin(phi1) * Math.Sin(psi);

                                transformationMatrix[2, 2] = Math.Cos(psi);

                                MathNet.Numerics.LinearAlgebra.Matrix<double> actStressOriented = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

                                for (int i = 0; i < 3; i++)
                                {
                                    for (int j = 0; j < 3; j++)
                                    {
                                        for (int k = 0; k < 3; k++)
                                        {
                                            for (int l = 0; l < 3; l++)
                                            {
                                                actStressOriented[i, j] += transformationMatrix[i, k] * transformationMatrix[j, l] * actSample.SimulationData[experimentIndex].StressSFHistory[n][k, l];
                                            }
                                        }
                                    }
                                }

                                //Potentiell active Gleitsysteme werden ermittelt
                                List<ReflexYield> potentialActive = actSample.SimulationData[experimentIndex].YieldInformation[phase].GetPotentiallyActiveSlipSystems(actStressOriented);

                                if (phi1 == 0.0 && psi == 0.0 && phi2 == 0.0)
                                {
                                    potentialActiveG[phase] = potentialActive;
                                }



                                //statistik parameter werden gesetzt
                                double activeSystems = 0.0;
                                double totalShearRate = 0.0;

                                List<MathNet.Numerics.LinearAlgebra.Matrix<double>> stiffnessFactorsOriented = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

                                //Berechnung von Lc for die spätere selbstkonsistente Berechnung von L und Ac bzw Bc
                                Tools.FourthRankTensor instGrainTensor = new Tools.FourthRankTensor();

                                if (potentialActive.Count != 0)
                                {
                                    //Hardening matrix ij der Gleitsysteme wird erstellt auf basis der des Richtungshärtungstensors
                                    MathNet.Numerics.LinearAlgebra.Matrix<double> hardeningMatrix = actSample.SimulationData[experimentIndex].YieldInformation[phase].HardeningMatrixSlipSystem(potentialActive, actSample.SimulationData[experimentIndex]._hardenningTensor[phase]);
                                    if (useHardenningMatrix)
                                    {
                                        hardeningMatrix = actSample.SimulationData[experimentIndex].YieldInformation[phase].HardeningMatrixSlipSystem(potentialActive, actSample.SimulationData[experimentIndex]._hardenningTensor[phase]);
                                    }
                                    else
                                    {
                                        hardeningMatrix = actSample.SimulationData[experimentIndex].YieldInformation[phase].HardeningMatrixSlipSystem(potentialActive);
                                    }

                                    MathNet.Numerics.LinearAlgebra.Matrix<double> slipSystemX = actSample.SimulationData[experimentIndex].YieldInformation[phase].SlipSystemX(potentialActive, hardeningMatrix, actSample.HillTensorData[phase]);
                                    MathNet.Numerics.LinearAlgebra.Matrix<double> slipSystemY = slipSystemX.Inverse();

                                    //hardenning der einzelnen Reflexe wird berechnet
                                    //actSample.PlasticTensor[phase].YieldSurfaceData.SetHardenning(potentialActive, hardeningMatrix);
                                    hardeningMatrixOriented.Add(hardeningMatrix);
                                    //Shear rates werden berechnet
                                    for (int i = 0; i < potentialActive.Count; i++)
                                    {
                                        //Berechnung der instantanious Stiffness factors f^i
                                        MathNet.Numerics.LinearAlgebra.Matrix<double> compeFactorsTmp = actSample.SimulationData[experimentIndex].YieldInformation[phase].GetInstStiffnessFactors(potentialActive, actSample.HillTensorData[phase], i, slipSystemY);
                                        stiffnessFactorsOriented.Add(compeFactorsTmp);

                                        //double shearRate = actSample.PlasticTensor[phase].YieldSurfaceData.GetShearRate(compeFactorsTmp, actSample.SimulationData[experimentIndex].StrainRateCFHistory[phase][n]);
                                        //MathNet.Numerics.LinearAlgebra.Matrix<double> alphaAct = potentialActive[i].GetResolvingParameter();
                                        //potentialActive[i].ActShearRate = shearRate;

                                        //if (shearRate >= 0)
                                        //{
                                        //    actStrainRatePlG[phase][0, 0] += shearRate * alphaAct[0, 0];
                                        //    actStrainRatePlG[phase][1, 0] += shearRate * alphaAct[1, 0];
                                        //    actStrainRatePlG[phase][2, 0] += shearRate * alphaAct[2, 0];
                                        //    actStrainRatePlG[phase][0, 1] += shearRate * alphaAct[0, 1];
                                        //    actStrainRatePlG[phase][1, 1] += shearRate * alphaAct[1, 1];
                                        //    actStrainRatePlG[phase][2, 1] += shearRate * alphaAct[2, 1];
                                        //    actStrainRatePlG[phase][0, 2] += shearRate * alphaAct[0, 2];
                                        //    actStrainRatePlG[phase][1, 2] += shearRate * alphaAct[1, 2];
                                        //    actStrainRatePlG[phase][2, 2] += shearRate * alphaAct[2, 2];

                                        //    totalShearRate += shearRate;
                                        //}

                                        activeSystems += potentialActive[i].DirectionMainMultiplizity * potentialActive[i].PlainMainMultiplizity;

                                    }

                                    instGrainTensor = actSample.PlasticTensor[phase].GetInstantanousGrainSiffnessTensor(potentialActive, actSample.HillTensorData[phase], stiffnessFactorsOriented);
                                }
                                else
                                {
                                    instGrainTensor = actSample.HillTensorData[phase].GetFourtRankStiffnesses();
                                }

                                stiffnessFactorsOrientedPhase.Add(stiffnessFactorsOriented);
                                //if (phi1 == 0.0 && psi == 0.0 && phi2 == 0.0)
                                //{
                                //    stiffnessFactors[phase] = stiffnessFactorsOriented;
                                //}
                                grainInstStiffnessesOriented.Add(instGrainTensor);

                                Tools.FourthRankTensor transitionGrainTensor = actSample.PlasticTensor[phase].GetAc(constraintTensor, instGrainTensor, overallStiffnesses);
                                grainTransitionStiffnessesOriented.Add(transitionGrainTensor);
                            }
                        }
                    }

                    stiffnessFactorsOrientedAll[phase] = stiffnessFactorsOrientedPhase;

                    if (textureActive)
                    {

                    }
                    else
                    {
                        overallStiffnessesPhase[phase] = Tools.FourthRankTensor.AverageInnerProduct(grainTransitionStiffnessesOriented, grainInstStiffnessesOriented);
                    }

                    hardeningMatrixList[phase] = hardeningMatrixOriented;
                    //setzen von Lc in den Listen
                    grainStiffnesses[phase] = grainInstStiffnessesOriented;
                    //setzen von Ac in den Listen
                    grainTransitionStiffnesses[phase] = grainTransitionStiffnessesOriented;
                }

                //Resetting the overall Stiffnesses
                overallStiffnessesComp = overallStiffnesses.Clone() as Tools.FourthRankTensor;

                overallStiffnesses = new Tools.FourthRankTensor();
                for (int phase = 0; phase < actSample.CrystalData.Count; phase++)
                {
                    overallStiffnesses += actSample.CrystalData[phase].PhaseFraction * overallStiffnessesPhase[phase];
                }

                //Checking if self consistent is finished already and meets the all the requirements

                //First check stress and strain rates 
                MathNet.Numerics.LinearAlgebra.Matrix<double> strainRateSampleTest = overallStiffnesses.Inverse() * stressRateS;

                //Setting grain conversions for stress rates (Bc)
                //List<Tools.FourthRankTensor> BcTest = new List<Tools.FourthRankTensor>();

                List<MathNet.Numerics.LinearAlgebra.Matrix<double>> strainRateGrainTest = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
                List<MathNet.Numerics.LinearAlgebra.Matrix<double>> stressRateGrainTest = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
                List<MathNet.Numerics.LinearAlgebra.Matrix<double>> ConvertedStressRateGrainTest = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

                double difCheck = 0.0;

                for (int phase = 0; phase < actSample.CrystalData.Count; phase++)
                {
                    Tools.FourthRankTensor BcTest = actSample.PlasticTensor[phase].GetBc(grainTransitionStiffnesses[phase][0], grainStiffnesses[phase][0], overallStiffnesses);
                    MathNet.Numerics.LinearAlgebra.Matrix<double> strainRateGrainTestTmp = grainTransitionStiffnesses[phase][0] * strainRateSampleTest;

                    strainRateGrainTest.Add(strainRateGrainTestTmp);
                    stressRateGrainTest.Add(BcTest * stressRateS);

                    ConvertedStressRateGrainTest.Add(grainStiffnesses[phase][0] * strainRateGrainTestTmp);
                }

                List<List<double>> yieldChange = new List<List<double>>();
                List<List<double>> yieldChangeComp = new List<List<double>>();

                for (int phase = 0; phase < actSample.CrystalData.Count; phase++)
                {
                    difCheck += GetTensorDifference(ConvertedStressRateGrainTest[phase], stressRateGrainTest[phase]);

                    List<double> yieldChangeTmp = new List<double>();
                    List<double> yieldChangeCompTmp = new List<double>();

                    yieldChange.Add(yieldChangeTmp);
                    yieldChangeComp.Add(yieldChangeCompTmp);
                }

                if (difCheck < 1)
                {
                    difCheck = 0.0;
                    int PotCounter = 0;

                    for (int phase = 0; phase < actSample.CrystalData.Count; phase++)
                    {
                        List<double> shearRates = new List<double>();

                        yieldChange[phase].Clear();
                        yieldChangeComp[phase].Clear();

                        for (int pot = 0; pot < potentialActiveG[phase].Count; pot++)
                        {
                            double shearRateTmp = actSample.SimulationData[experimentIndex].YieldInformation[phase].GetShearRate(stiffnessFactors[phase][pot], strainRateGrainTest[phase]);

                            shearRates.Add(shearRateTmp);

                            MathNet.Numerics.LinearAlgebra.Matrix<double> alphaAct = potentialActiveG[phase][pot].GetResolvingParameter();

                            double yieldChangeTmp = 0.0;
                            double yieldChangeCompTmp = actSample.SimulationData[experimentIndex].YieldInformation[phase].GetShearRate(stressRateGrainTest[phase], alphaAct); ;

                            for (int pot2 = 0; pot2 < hardeningMatrixList[phase][0].ColumnCount; pot2++)
                            {
                                yieldChangeTmp += hardeningMatrixList[phase][0][pot, pot2] * shearRateTmp;
                            }

                            yieldChange[phase].Add(yieldChangeTmp);
                            yieldChangeComp[phase].Add(yieldChangeCompTmp);
                        }

                        for (int pot = 0; pot < yieldChange[phase].Count; pot++)
                        {
                            difCheck = Math.Abs(yieldChange[phase][pot] - yieldChangeComp[phase][pot]) / Math.Abs(yieldChange[phase][pot]);
                            PotCounter++;
                        }
                    }

                    if (PotCounter != 0)
                    {
                        if ((difCheck / PotCounter) < 1)
                        {
                            for (int phase = 0; phase < actSample.CrystalData.Count; phase++)
                            {
                                for (int pot = 0; pot < yieldChange[phase].Count; pot++)
                                {
                                    potentialActiveG[phase][pot].YieldMainHardennedStrength += yieldChange[phase][pot];
                                }
                            }
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }

                }
                if (actCircle > 5)
                {
                    break;
                }

                actCircle++;
            }

            //All base tensor for the Grains, sample tensor are set
            //the stress states for the sample and phases will be calculated
            //Parameter Reihenfolge:
            //1. Spannung Probe (Eingabe)
            //2. Spannungsrate Probe (berechnet über Eingabe)
            //3. Dehnungsrate Probe (über overallStiffnesses gerechnet)
            //4. Dehnung Probe (Berechnet aus der Dehnungsrate der Probe)
            //5. Bestimmung der Phasenparameter
            //6. Dehnungsraten der grains (berechnet über die transition Matrix)
            //7. Dehnung der grains (berechnet über die Dehnrate)
            //8. Berechnung der Spannungsrate im Grain (Dehnrate aus 6. - Plastischer Dehnrate --> Elastische konstanten)
            //9. Berechung der Aktuellen Spannung analog zu 7.
            //10. Aktive Gleitsysteme


            //public List<MathNet.Numerics.LinearAlgebra.Matrix<double>> HardeningSFHistory = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

            //public List<double> ShearRateSFHistory = new List<double>();
            //public List<double> ActiveSystemsSFHistory = new List<double>();

            //public List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>> StressCFHistory = new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>();

            //public List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>> HardeningCFHistory = new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>();
            //public List<List<double>> ShearRateCFHistory = new List<List<double>>();

            //2. 
            actSample.SimulationData[experimentIndex].StressRateSFHistory.Add(stressRateS.Clone());

            //3.
            MathNet.Numerics.LinearAlgebra.Matrix<double> strainRateSample = overallStiffnesses.Inverse() * stressRateS;
            actSample.SimulationData[experimentIndex].StrainRateSFHistory.Add(strainRateSample);

            //4.
            actStrainS += strainRateSample;
            actSample.SimulationData[experimentIndex].StrainSFHistory.Add(actStrainS.Clone());

            //5.
            for (int phase = 0; phase < actSample.CrystalData.Count; phase++)
            {
                //6.
                MathNet.Numerics.LinearAlgebra.Matrix<double> strainRateG = grainTransitionStiffnesses[phase][0] * strainRateSample;
                actSample.SimulationData[experimentIndex].StrainRateCFHistory[phase].Add(strainRateG);

                //7.
                actStrainG[phase] += strainRateG;
                actSample.SimulationData[experimentIndex].StrainCFHistory[phase].Add(actStrainG[phase].Clone());

                //8.
                MathNet.Numerics.LinearAlgebra.Matrix<double> elasticStrainRateG = strainRateG - actStrainRatePlG[phase];
                MathNet.Numerics.LinearAlgebra.Matrix<double> stressRateG = overallStiffnessesPhase[phase] * elasticStrainRateG;
                actSample.SimulationData[experimentIndex].StressRateCFHistory[phase].Add(stressRateG);

                //9.
                actStressG[phase] += stressRateG;
                actSample.SimulationData[experimentIndex].StressCFHistory[phase].Add(actStressG[phase].Clone());

                //10.
                actSample.SimulationData[experimentIndex].ActiveSystemsCFHistory[phase].Add(potentialActiveG[phase]);

            }

            if (correctArea)
            {
                double radius = Math.Sqrt(actSample.SimulationData[experimentIndex].SampleArea / Math.PI);

                double ellipseA = radius * (1 + actStrainS[0, 0]);
                double ellipseB = radius * (1 + actStrainS[1, 1]);

                double newArea = Math.PI * ellipseA * ellipseB;
                double appliedForce = actSample.SimulationData[experimentIndex].StressSFHistory[n][2, 2] * actSample.SimulationData[experimentIndex].SampleArea;

                MathNet.Numerics.LinearAlgebra.Matrix<double> newStressTensor = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense<double>(3, 3, 0);
                newStressTensor[2, 2] = appliedForce / newArea;

                actSample.SimulationData[experimentIndex].StressSFHistoryAreaCorrected.Add(newStressTensor);
            }
        }

        public static void PerfomStandardExperimentStepAllGRain(Sample actSample, bool textureActive, int experimentIndex, int n, MathNet.Numerics.LinearAlgebra.Matrix<double> actStrainS, List<MathNet.Numerics.LinearAlgebra.Matrix<double>> actStrainG, List<MathNet.Numerics.LinearAlgebra.Matrix<double>> actStressG, bool useHardenningMatrix, bool correctArea)
        {
            //setzen der einzelnen Parameter
            List<List<ReflexYield>> potentialActiveG = new List<List<ReflexYield>>();
            List<List<List<ReflexYield>>> potentialActiveGOriented = new List<List<List<ReflexYield>>>();

            //Spannungsrate berechnen der angelegten Probenspannungsrate
            MathNet.Numerics.LinearAlgebra.Matrix<double> stressRateS = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
            if (n == 0)
            {
                stressRateS[0, 0] += actSample.SimulationData[experimentIndex].StressSFHistory[n][0, 0];
                stressRateS[1, 0] += actSample.SimulationData[experimentIndex].StressSFHistory[n][1, 0];
                stressRateS[2, 0] += actSample.SimulationData[experimentIndex].StressSFHistory[n][2, 0];
                stressRateS[0, 1] += actSample.SimulationData[experimentIndex].StressSFHistory[n][0, 1];
                stressRateS[1, 1] += actSample.SimulationData[experimentIndex].StressSFHistory[n][1, 1];
                stressRateS[2, 1] += actSample.SimulationData[experimentIndex].StressSFHistory[n][2, 1];
                stressRateS[0, 2] += actSample.SimulationData[experimentIndex].StressSFHistory[n][0, 2];
                stressRateS[1, 2] += actSample.SimulationData[experimentIndex].StressSFHistory[n][1, 2];
                stressRateS[2, 2] += actSample.SimulationData[experimentIndex].StressSFHistory[n][2, 2];

                if (correctArea)
                {
                    actSample.SimulationData[experimentIndex].StressSFHistoryAreaCorrected.Add(actSample.SimulationData[experimentIndex].StressSFHistory[n].Clone());
                }
            }
            else
            {
                if (correctArea)
                {
                    stressRateS[0, 0] += actSample.SimulationData[experimentIndex].StressSFHistoryAreaCorrected[n][0, 0] - actSample.SimulationData[experimentIndex].StressSFHistoryAreaCorrected[n - 1][0, 0];
                    stressRateS[1, 0] += actSample.SimulationData[experimentIndex].StressSFHistoryAreaCorrected[n][1, 0] - actSample.SimulationData[experimentIndex].StressSFHistoryAreaCorrected[n - 1][1, 0];
                    stressRateS[2, 0] += actSample.SimulationData[experimentIndex].StressSFHistoryAreaCorrected[n][2, 0] - actSample.SimulationData[experimentIndex].StressSFHistoryAreaCorrected[n - 1][2, 0];
                    stressRateS[0, 1] += actSample.SimulationData[experimentIndex].StressSFHistoryAreaCorrected[n][0, 1] - actSample.SimulationData[experimentIndex].StressSFHistoryAreaCorrected[n - 1][0, 1];
                    stressRateS[1, 1] += actSample.SimulationData[experimentIndex].StressSFHistoryAreaCorrected[n][1, 1] - actSample.SimulationData[experimentIndex].StressSFHistoryAreaCorrected[n - 1][1, 1];
                    stressRateS[2, 1] += actSample.SimulationData[experimentIndex].StressSFHistoryAreaCorrected[n][2, 1] - actSample.SimulationData[experimentIndex].StressSFHistoryAreaCorrected[n - 1][2, 1];
                    stressRateS[0, 2] += actSample.SimulationData[experimentIndex].StressSFHistoryAreaCorrected[n][0, 2] - actSample.SimulationData[experimentIndex].StressSFHistoryAreaCorrected[n - 1][0, 2];
                    stressRateS[1, 2] += actSample.SimulationData[experimentIndex].StressSFHistoryAreaCorrected[n][1, 2] - actSample.SimulationData[experimentIndex].StressSFHistoryAreaCorrected[n - 1][1, 2];
                    stressRateS[2, 2] += actSample.SimulationData[experimentIndex].StressSFHistoryAreaCorrected[n][2, 2] - actSample.SimulationData[experimentIndex].StressSFHistoryAreaCorrected[n - 1][2, 2];
                }
                else
                {
                    stressRateS[0, 0] += actSample.SimulationData[experimentIndex].StressSFHistory[n][0, 0] - actSample.SimulationData[experimentIndex].StressSFHistory[n - 1][0, 0];
                    stressRateS[1, 0] += actSample.SimulationData[experimentIndex].StressSFHistory[n][1, 0] - actSample.SimulationData[experimentIndex].StressSFHistory[n - 1][1, 0];
                    stressRateS[2, 0] += actSample.SimulationData[experimentIndex].StressSFHistory[n][2, 0] - actSample.SimulationData[experimentIndex].StressSFHistory[n - 1][2, 0];
                    stressRateS[0, 1] += actSample.SimulationData[experimentIndex].StressSFHistory[n][0, 1] - actSample.SimulationData[experimentIndex].StressSFHistory[n - 1][0, 1];
                    stressRateS[1, 1] += actSample.SimulationData[experimentIndex].StressSFHistory[n][1, 1] - actSample.SimulationData[experimentIndex].StressSFHistory[n - 1][1, 1];
                    stressRateS[2, 1] += actSample.SimulationData[experimentIndex].StressSFHistory[n][2, 1] - actSample.SimulationData[experimentIndex].StressSFHistory[n - 1][2, 1];
                    stressRateS[0, 2] += actSample.SimulationData[experimentIndex].StressSFHistory[n][0, 2] - actSample.SimulationData[experimentIndex].StressSFHistory[n - 1][0, 2];
                    stressRateS[1, 2] += actSample.SimulationData[experimentIndex].StressSFHistory[n][1, 2] - actSample.SimulationData[experimentIndex].StressSFHistory[n - 1][1, 2];
                    stressRateS[2, 2] += actSample.SimulationData[experimentIndex].StressSFHistory[n][2, 2] - actSample.SimulationData[experimentIndex].StressSFHistory[n - 1][2, 2];
                }
            }

            //Vorbereitung der Daten für die Spannungsrate in grains ausgerichtet nach der Probenorientierung
            List<MathNet.Numerics.LinearAlgebra.Matrix<double>> actStressRateG = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
            List<MathNet.Numerics.LinearAlgebra.Matrix<double>> actStressRateGA = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

            //Vorbereitung der Dehnraten im elastischen und Plastischen Bereich
            List<MathNet.Numerics.LinearAlgebra.Matrix<double>> actStrainRatePlG = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

            //Elastische Tensoren
            Tools.FourthRankTensor overallStiffnesses = actSample.GetSampleStiffnesses(textureActive);
            Tools.FourthRankTensor overallStiffnessesComp = new Tools.FourthRankTensor();
            //Tools.FourthRankTensor Stiffnesses = eT.GetFourtRankStiffnesses();

            List<Tools.FourthRankTensor> constraintStiffness = new List<Tools.FourthRankTensor>();
            List<List<Tools.FourthRankTensor>> grainStiffnesses = new List<List<Tools.FourthRankTensor>>();
            List<List<Tools.FourthRankTensor>> grainTransitionStiffnesses = new List<List<Tools.FourthRankTensor>>();

            //gemittelten Phasen nachgiebigkeiten werden erstellt
            List<Tools.FourthRankTensor> overallStiffnessesPhase = new List<Tools.FourthRankTensor>();

            //fi
            List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>> stiffnessFactors = new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>();
            List<List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>> stiffnessFactorsOrientedAll = new List<List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>>();
            List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>> hardeningMatrixList = new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>();

            //Ausfüllen der Grain parameter für die verschiedenen Phasen
            for (int phase = 0; phase < actSample.CrystalData.Count; phase++)
            {
                actStrainRatePlG.Add(MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0));

                actStressRateG.Add(MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0));

                overallStiffnessesPhase.Add(new Tools.FourthRankTensor());
                constraintStiffness.Add(new Tools.FourthRankTensor());
                grainStiffnesses.Add(new List<Tools.FourthRankTensor>());
                grainTransitionStiffnesses.Add(new List<Tools.FourthRankTensor>());

                potentialActiveG.Add(new List<ReflexYield>());
                potentialActiveGOriented.Add(new List<List<ReflexYield>>());

                stiffnessFactors.Add(new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>());
                stiffnessFactorsOrientedAll.Add(new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>());
                hardeningMatrixList.Add(new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>());
            }

            bool sCCompletted = false;
            int actCircle = 0;

            while (!sCCompletted)
            {
                //Berechnungen im Krystallsystem
                for (int phase = 0; phase < actSample.CrystalData.Count; phase++)
                {
                    double maxPhi1 = 360;
                    double maxPsi = 360;
                    double maxPhi2 = 360;


                    //The constrain Tensor L* for cubic isotropic materials
                    Microsopic.ElasticityTensors eTmp = new Microsopic.ElasticityTensors();
                    eTmp._stiffnessTensor = overallStiffnesses.GetVoigtTensor();
                    eTmp.CalculateCompliances();
                    Tools.FourthRankTensor constraintTensor = actSample.PlasticTensor[phase].GetConstraintStiffnessCubicIsotropic(eTmp, 2);
                    //Tools.FourthRankTensor constraintTensor = actSample.PlasticTensor[phase].GetConstraintStiffnessCubicIsotropic(actSample.HillTensorData[phase], 2);

                    if (actSample.CrystalData[phase].SymmetryGroupID == 225 || actSample.CrystalData[phase].SymmetryGroupID == 229)
                    {
                        maxPhi1 = 45;
                        maxPsi = 45;
                        maxPhi2 = 45;

                        //The constrain Tensor L* for cubic isotropic materials
                        constraintTensor = actSample.PlasticTensor[phase].GetConstraintStiffnessCubicIsotropic(eTmp, 2);
                        //constraintTensor = actSample.PlasticTensor[phase].GetConstraintStiffnessCubicIsotropic(actSample.HillTensorData[phase], 2);
                    }
                    if (actSample.CrystalData[phase].SymmetryGroupID == 194)
                    {
                        maxPhi1 = 60;
                        maxPsi = 90;
                        maxPhi2 = 60;
                    }

                    if (actSample.SimulationData[experimentIndex].GrainOrientations[phase].Count == 0)
                    {
                        for (double phi1 = 0.0; phi1 < maxPhi1; phi1 += 5.0)
                        {
                            for (double psi = 0.0; psi < maxPsi; psi += 5.0)
                            {
                                for (double phi2 = 0.0; phi2 < maxPhi2; phi2 += 5.0)
                                {
                                    actSample.SimulationData[experimentIndex].GrainOrientations[phase].Add(new GrainOrientationParameter(phi1, psi, phi2));
                                }
                            }
                        }
                    }

                    constraintStiffness[phase] = constraintTensor;

                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> StressCFPhase = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

                    List<Tools.FourthRankTensor> grainInstStiffnessesOriented = new List<Tools.FourthRankTensor>();
                    List<Tools.FourthRankTensor> grainTransitionStiffnessesOriented = new List<Tools.FourthRankTensor>();

                    List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>> stiffnessFactorsOrientedPhase = new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>();
                    List<List<ReflexYield>> potentialActiveGPhase = new List<List<ReflexYield>>();

                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> hardeningMatrixOriented = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

                    for (double phi1 = 0.0; phi1 < maxPhi1; phi1 += 5.0)
                    {
                        for (double psi = 0.0; psi < maxPsi; psi += 5.0)
                        {
                            for (double phi2 = 0.0; phi2 < maxPhi2; phi2 += 5.0)
                            {
                                
                                MathNet.Numerics.LinearAlgebra.Matrix<double> transformationMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense<double>(3, 3, 0);

                                transformationMatrix[0, 0] = -1 * Math.Cos(phi1) * Math.Cos(psi) * Math.Sin(phi2);
                                transformationMatrix[0, 0] -= Math.Sin(phi1) * Math.Cos(phi2);

                                transformationMatrix[0, 1] = -1 * Math.Cos(psi) * Math.Sin(phi1) * Math.Sin(phi2);
                                transformationMatrix[0, 1] -= Math.Cos(phi1) * Math.Cos(phi2);

                                transformationMatrix[0, 2] = Math.Sin(psi) * Math.Sin(phi2);

                                transformationMatrix[1, 0] = -1 * Math.Cos(psi) * Math.Cos(phi1) * Math.Cos(phi2);
                                transformationMatrix[1, 0] -= Math.Sin(phi1) * Math.Sin(phi2);

                                transformationMatrix[1, 1] = -1 * Math.Sin(phi1) * Math.Cos(psi) * Math.Cos(phi2);
                                transformationMatrix[1, 1] -= Math.Cos(phi1) * Math.Sin(phi2);

                                transformationMatrix[1, 2] = Math.Sin(psi) * Math.Cos(phi2);

                                transformationMatrix[2, 0] = Math.Cos(phi1) * Math.Sin(psi);

                                transformationMatrix[2, 1] = Math.Sin(phi1) * Math.Sin(psi);

                                transformationMatrix[2, 2] = Math.Cos(psi);

                                MathNet.Numerics.LinearAlgebra.Matrix<double> actStressOriented = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

                                for (int i = 0; i < 3; i++)
                                {
                                    for (int j = 0; j < 3; j++)
                                    {
                                        for (int k = 0; k < 3; k++)
                                        {
                                            for (int l = 0; l < 3; l++)
                                            {
                                                actStressOriented[i, j] += transformationMatrix[i, k] * transformationMatrix[j, l] * actSample.SimulationData[experimentIndex].StressSFHistory[n][k, l];
                                            }
                                        }
                                    }
                                }

                                //Potentiell active Gleitsysteme werden ermittelt
                                List<ReflexYield> potentialActive = actSample.SimulationData[experimentIndex].YieldInformation[phase].GetPotentiallyActiveSlipSystems(actStressOriented);

                                potentialActiveGPhase.Add(potentialActive);
                                //if (phi1 == 0.0 && psi == 0.0 && phi2 == 0.0)
                                //{
                                //    potentialActiveG[phase] = potentialActive;
                                //}



                                //statistik parameter werden gesetzt
                                double activeSystems = 0.0;
                                double totalShearRate = 0.0;

                                List<MathNet.Numerics.LinearAlgebra.Matrix<double>> stiffnessFactorsOriented = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

                                //Berechnung von Lc for die spätere selbstkonsistente Berechnung von L und Ac bzw Bc
                                Tools.FourthRankTensor instGrainTensor = new Tools.FourthRankTensor();

                                if (potentialActive.Count != 0)
                                {
                                    //Hardening matrix ij der Gleitsysteme wird erstellt auf basis der des Richtungshärtungstensors
                                    MathNet.Numerics.LinearAlgebra.Matrix<double> hardeningMatrix = actSample.SimulationData[experimentIndex].YieldInformation[phase].HardeningMatrixSlipSystem(potentialActive, actSample.SimulationData[experimentIndex]._hardenningTensor[phase]);
                                    if (useHardenningMatrix)
                                    {
                                        hardeningMatrix = actSample.SimulationData[experimentIndex].YieldInformation[phase].HardeningMatrixSlipSystem(potentialActive, actSample.SimulationData[experimentIndex]._hardenningTensor[phase]);
                                    }
                                    else
                                    {
                                        hardeningMatrix = actSample.SimulationData[experimentIndex].YieldInformation[phase].HardeningMatrixSlipSystem(potentialActive);
                                    }

                                    MathNet.Numerics.LinearAlgebra.Matrix<double> slipSystemX = actSample.SimulationData[experimentIndex].YieldInformation[phase].SlipSystemX(potentialActive, hardeningMatrix, actSample.HillTensorData[phase]);
                                    MathNet.Numerics.LinearAlgebra.Matrix<double> slipSystemY = slipSystemX.Inverse();

                                    //hardenning der einzelnen Reflexe wird berechnet
                                    //actSample.PlasticTensor[phase].YieldSurfaceData.SetHardenning(potentialActive, hardeningMatrix);
                                    hardeningMatrixOriented.Add(hardeningMatrix);
                                    //Shear rates werden berechnet
                                    for (int i = 0; i < potentialActive.Count; i++)
                                    {
                                        //Berechnung der instantanious Stiffness factors f^i
                                        MathNet.Numerics.LinearAlgebra.Matrix<double> compeFactorsTmp = actSample.SimulationData[experimentIndex].YieldInformation[phase].GetInstStiffnessFactors(potentialActive, actSample.HillTensorData[phase], i, slipSystemY);
                                        stiffnessFactorsOriented.Add(compeFactorsTmp);

                                        //double shearRate = actSample.PlasticTensor[phase].YieldSurfaceData.GetShearRate(compeFactorsTmp, actSample.SimulationData[experimentIndex].StrainRateCFHistory[phase][n]);
                                        //MathNet.Numerics.LinearAlgebra.Matrix<double> alphaAct = potentialActive[i].GetResolvingParameter();
                                        //potentialActive[i].ActShearRate = shearRate;

                                        //if (shearRate >= 0)
                                        //{
                                        //    actStrainRatePlG[phase][0, 0] += shearRate * alphaAct[0, 0];
                                        //    actStrainRatePlG[phase][1, 0] += shearRate * alphaAct[1, 0];
                                        //    actStrainRatePlG[phase][2, 0] += shearRate * alphaAct[2, 0];
                                        //    actStrainRatePlG[phase][0, 1] += shearRate * alphaAct[0, 1];
                                        //    actStrainRatePlG[phase][1, 1] += shearRate * alphaAct[1, 1];
                                        //    actStrainRatePlG[phase][2, 1] += shearRate * alphaAct[2, 1];
                                        //    actStrainRatePlG[phase][0, 2] += shearRate * alphaAct[0, 2];
                                        //    actStrainRatePlG[phase][1, 2] += shearRate * alphaAct[1, 2];
                                        //    actStrainRatePlG[phase][2, 2] += shearRate * alphaAct[2, 2];

                                        //    totalShearRate += shearRate;
                                        //}

                                        activeSystems += potentialActive[i].DirectionMainMultiplizity * potentialActive[i].PlainMainMultiplizity;

                                    }

                                    instGrainTensor = actSample.PlasticTensor[phase].GetInstantanousGrainSiffnessTensor(potentialActive, actSample.HillTensorData[phase], stiffnessFactorsOriented);
                                }
                                else
                                {
                                    MathNet.Numerics.LinearAlgebra.Matrix<double> emptyHardenning = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(1, 1, 0.0);
                                    hardeningMatrixOriented.Add(emptyHardenning);

                                    instGrainTensor = actSample.HillTensorData[phase].GetFourtRankStiffnesses();
                                }

                                stiffnessFactorsOrientedPhase.Add(stiffnessFactorsOriented);
                                //if (phi1 == 0.0 && psi == 0.0 && phi2 == 0.0)
                                //{
                                //    stiffnessFactors[phase] = stiffnessFactorsOriented;
                                //}
                                grainInstStiffnessesOriented.Add(instGrainTensor);

                                Tools.FourthRankTensor transitionGrainTensor = actSample.PlasticTensor[phase].GetAc(constraintTensor, instGrainTensor, overallStiffnesses);
                                grainTransitionStiffnessesOriented.Add(transitionGrainTensor);
                            }
                        }
                    }

                    stiffnessFactorsOrientedAll[phase] = stiffnessFactorsOrientedPhase;
                    potentialActiveGOriented[phase] = potentialActiveGPhase;

                    if (textureActive)
                    {

                    }
                    else
                    {
                        overallStiffnessesPhase[phase] = Tools.FourthRankTensor.AverageInnerProduct(grainTransitionStiffnessesOriented, grainInstStiffnessesOriented);
                    }

                    hardeningMatrixList[phase] = hardeningMatrixOriented;
                    //setzen von Lc in den Listen
                    grainStiffnesses[phase] = grainInstStiffnessesOriented;
                    //setzen von Ac in den Listen
                    grainTransitionStiffnesses[phase] = grainTransitionStiffnessesOriented;
                }

                //Resetting the overall Stiffnesses
                overallStiffnessesComp = overallStiffnesses.Clone() as Tools.FourthRankTensor;

                overallStiffnesses = new Tools.FourthRankTensor();
                for (int phase = 0; phase < actSample.CrystalData.Count; phase++)
                {
                    overallStiffnesses += actSample.CrystalData[phase].PhaseFraction * overallStiffnessesPhase[phase];
                }

                //Checking if self consistent is finished already and meets the all the requirements

                //First check stress and strain rates 
                MathNet.Numerics.LinearAlgebra.Matrix<double> strainRateSampleTest = overallStiffnesses.Inverse() * stressRateS;

                //Setting grain conversions for stress rates (Bc)
                //List<Tools.FourthRankTensor> BcTest = new List<Tools.FourthRankTensor>();

                List<MathNet.Numerics.LinearAlgebra.Matrix<double>> strainRateGrainTest = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
                List<MathNet.Numerics.LinearAlgebra.Matrix<double>> stressRateGrainTest = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
                List<MathNet.Numerics.LinearAlgebra.Matrix<double>> ConvertedStressRateGrainTest = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

                double difCheck = 0.0;

                for (int phase = 0; phase < actSample.CrystalData.Count; phase++)
                {
                    Tools.FourthRankTensor BcTest = actSample.PlasticTensor[phase].GetBc(grainTransitionStiffnesses[phase][0], grainStiffnesses[phase][0], overallStiffnesses);
                    MathNet.Numerics.LinearAlgebra.Matrix<double> strainRateGrainTestTmp = grainTransitionStiffnesses[phase][0] * strainRateSampleTest;

                    strainRateGrainTest.Add(strainRateGrainTestTmp);
                    stressRateGrainTest.Add(BcTest * stressRateS);

                    ConvertedStressRateGrainTest.Add(grainStiffnesses[phase][0] * strainRateGrainTestTmp);
                }

                List<List<double>> yieldChange = new List<List<double>>();
                List<List<double>> yieldChangeComp = new List<List<double>>();
                List<List<List<double>>> yieldChangeOriented = new List<List<List<double>>>();
                List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>> strainRateGrainOriented = new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>();
                List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>> stressRateGrainOriented = new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>();

                for (int phase = 0; phase < actSample.CrystalData.Count; phase++)
                {
                    difCheck += GetTensorDifference(ConvertedStressRateGrainTest[phase], stressRateGrainTest[phase]);

                    List<double> yieldChangeTmp = new List<double>();
                    List<double> yieldChangeCompTmp = new List<double>();
                    List<List<double>> yieldChangeOrientedTmp = new List<List<double>>();
                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> strainRateGrainOrientedPhaseTmp = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
                    List<MathNet.Numerics.LinearAlgebra.Matrix<double>> stressRateGrainOrientedPhaseTmp = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

                    yieldChange.Add(yieldChangeTmp);
                    yieldChangeComp.Add(yieldChangeCompTmp);
                    yieldChangeOriented.Add(yieldChangeOrientedTmp);
                    strainRateGrainOriented.Add(strainRateGrainOrientedPhaseTmp);
                    stressRateGrainOriented.Add(stressRateGrainOrientedPhaseTmp);
                }

                //Achtung hier ist gerade ein Testlauf am kommen. L werd min 5 mal berechnet
                if (difCheck < 1)
                {
                    difCheck = 0.0;
                    int PotCounter = 0;
                    
                    for (int phase = 0; phase < actSample.CrystalData.Count; phase++)
                    {
                        List<MathNet.Numerics.LinearAlgebra.Matrix<double>> strainRateGrainOrientedPhase = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
                        List<MathNet.Numerics.LinearAlgebra.Matrix<double>> stressRateGrainOrientedPhase = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
                        List<List<double>> yieldChangeOrientedPhase = new List<List<double>>();

                        for (int orientationIndex = 0; orientationIndex < grainTransitionStiffnesses[phase].Count; orientationIndex++)
                        {
                            yieldChange[phase].Clear();
                            yieldChangeComp[phase].Clear();

                            //List<double> shearRates = new List<double>();
                            MathNet.Numerics.LinearAlgebra.Matrix<double> strainRateGrainTmp = grainTransitionStiffnesses[phase][orientationIndex] * strainRateSampleTest;
                            strainRateGrainOrientedPhase.Add(strainRateGrainTmp);
                            
                            Tools.FourthRankTensor BcTest = actSample.PlasticTensor[phase].GetBc(grainTransitionStiffnesses[phase][orientationIndex], grainStiffnesses[phase][orientationIndex], overallStiffnesses);
                            MathNet.Numerics.LinearAlgebra.Matrix<double> stressRateGrainTmp = BcTest * stressRateS;
                            stressRateGrainOrientedPhase.Add(stressRateGrainTmp);

                            for (int pot = 0; pot < potentialActiveGOriented[phase][orientationIndex].Count; pot++)
                            {
                                double shearRateTmp = actSample.SimulationData[experimentIndex].YieldInformation[phase].GetShearRate(stiffnessFactorsOrientedAll[phase][orientationIndex][pot], strainRateGrainTmp);

                                //shearRates.Add(shearRateTmp);

                                MathNet.Numerics.LinearAlgebra.Matrix<double> alphaAct = potentialActiveGOriented[phase][orientationIndex][pot].GetResolvingParameter();

                                double yieldChangeTmp = 0.0;
                                double yieldChangeCompTmp = actSample.SimulationData[experimentIndex].YieldInformation[phase].GetShearRate(stressRateGrainTmp, alphaAct); ;

                                for (int pot2 = 0; pot2 < hardeningMatrixList[phase][orientationIndex].ColumnCount; pot2++)
                                {
                                    yieldChangeTmp += hardeningMatrixList[phase][orientationIndex][pot, pot2] * Math.Abs(shearRateTmp);
                                }

                                yieldChange[phase].Add(yieldChangeTmp);
                                yieldChangeComp[phase].Add(yieldChangeCompTmp);
                            }

                            List<double> yieldChangeOrTmp = new List<double>();
                            for (int pot = 0; pot < yieldChange[phase].Count; pot++)
                            {
                                difCheck = Math.Abs(yieldChange[phase][pot] - yieldChangeComp[phase][pot]) / Math.Abs(yieldChange[phase][pot]);
                                yieldChangeOrTmp.Add(yieldChange[phase][pot]);
                                PotCounter++;
                            }
                            yieldChangeOrientedPhase.Add(yieldChangeOrTmp);
                        }

                        strainRateGrainOriented[phase] = strainRateGrainOrientedPhase;
                        stressRateGrainOriented[phase] = stressRateGrainOrientedPhase;
                        yieldChangeOriented[phase] = yieldChangeOrientedPhase;
                    }
                    
                    if (PotCounter != 0)
                    {
                        if ((difCheck / PotCounter) < 1 && actCircle > 4)
                        {
                            for (int phase = 0; phase < actSample.CrystalData.Count; phase++)
                            {
                                List<double> yieldChangeAverage = new List<double>();
                                //List<int> averageCounter = new List<int>();
                                for (int allSystems = 0; allSystems < actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems.Count; allSystems++)
                                {
                                    yieldChangeAverage.Add(0.0);
                                    //averageCounter.Add(0);
                                }

                                for (int orientations = 0; orientations < yieldChangeOriented[phase].Count; orientations++)
                                {
                                    for (int pot = 0; pot < yieldChangeOriented[phase][orientations].Count; pot++)
                                    {
                                        for(int refIndex = 0; refIndex < actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems.Count; refIndex++)
                                        {
                                            if(potentialActiveGOriented[phase][orientations][pot].HKLString == actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[refIndex].HKLString)
                                            {
                                                yieldChangeAverage[refIndex] += yieldChangeOriented[phase][orientations][pot];
                                                //averageCounter[refIndex]++;
                                            }
                                        }
                                        //potentialActiveG[phase][pot].YieldMainHardennedStrength += yieldChange[phase][pot];
                                    }
                                }

                                for(int slipIndex = 0; slipIndex < yieldChangeAverage.Count; slipIndex++)
                                {
                                    actSample.SimulationData[experimentIndex].YieldInformation[phase].PotentialSlipSystems[slipIndex].YieldMainHardennedStrength += yieldChangeAverage[slipIndex] / actSample.SimulationData[experimentIndex].GrainOrientations.Count;
                                }

                                //Datasetting für Graindaten
                                actSample.SimulationData[experimentIndex].StressRateCFOrientedHistory[phase].Add(stressRateGrainOriented[phase]);
                                actSample.SimulationData[experimentIndex].StrainRateCFOrientedHistory[phase].Add(strainRateGrainOriented[phase]);
                                actSample.SimulationData[experimentIndex].ActiveSystemsCFOrientedHistory[phase].Add(potentialActiveGOriented[phase]);
                            }
                            
                            break;
                        }
                    }
                    else
                    {
                        for (int phase = 0; phase < actSample.CrystalData.Count; phase++)
                        {
                            //Datasetting für Graindaten
                            actSample.SimulationData[experimentIndex].StressRateCFOrientedHistory[phase].Add(stressRateGrainOriented[phase]);
                            actSample.SimulationData[experimentIndex].StrainRateCFOrientedHistory[phase].Add(strainRateGrainOriented[phase]);
                            actSample.SimulationData[experimentIndex].ActiveSystemsCFOrientedHistory[phase].Add(potentialActiveGOriented[phase]);
                        }
                        break;
                    }

                }
                if (actCircle > 7)
                {
                    for (int phase = 0; phase < actSample.CrystalData.Count; phase++)
                    {
                        //Datasetting für Graindaten
                        actSample.SimulationData[experimentIndex].StressRateCFOrientedHistory[phase].Add(stressRateGrainOriented[phase]);
                        actSample.SimulationData[experimentIndex].StrainRateCFOrientedHistory[phase].Add(strainRateGrainOriented[phase]);
                        actSample.SimulationData[experimentIndex].ActiveSystemsCFOrientedHistory[phase].Add(potentialActiveGOriented[phase]);
                    }
                    break;
                }

                actCircle++;
            }

            //All base tensor for the Grains, sample tensor are set
            //the stress states for the sample and phases will be calculated
            //Parameter Reihenfolge:
            //1. Spannung Probe (Eingabe)
            //2. Spannungsrate Probe (berechnet über Eingabe)
            //3. Dehnungsrate Probe (über overallStiffnesses gerechnet)
            //4. Dehnung Probe (Berechnet aus der Dehnungsrate der Probe)
            //5. Bestimmung der Phasenparameter
            //Alles auf grain lvl wird nach des Konvergenz checks berechnet und gesetzt
            //6. Dehnungsraten der grains (berechnet über die transition Matrix)
            //7. Dehnung der grains (berechnet über die Dehnrate)
            //8. Berechnung der Spannungsrate im Grain (Dehnrate aus 6. - Plastischer Dehnrate --> Elastische konstanten)
            //9. Berechung der Aktuellen Spannung analog zu 7.
            //10. Aktive Gleitsysteme


            //public List<MathNet.Numerics.LinearAlgebra.Matrix<double>> HardeningSFHistory = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

            //public List<double> ShearRateSFHistory = new List<double>();
            //public List<double> ActiveSystemsSFHistory = new List<double>();

            //public List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>> StressCFHistory = new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>();

            //public List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>> HardeningCFHistory = new List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>();
            //public List<List<double>> ShearRateCFHistory = new List<List<double>>();

            //2. 
            actSample.SimulationData[experimentIndex].StressRateSFHistory.Add(stressRateS.Clone());

            //3.
            MathNet.Numerics.LinearAlgebra.Matrix<double> strainRateSample = overallStiffnesses.Inverse() * stressRateS;
            actSample.SimulationData[experimentIndex].StrainRateSFHistory.Add(strainRateSample);

            //4.

            if (actSample.SimulationData[experimentIndex].StrainSFHistory.Count == 0)
            {
                actStrainS = strainRateSample;
            }
            else
            {
                actStrainS = actSample.SimulationData[experimentIndex].StrainSFHistory[actSample.SimulationData[experimentIndex].StrainSFHistory.Count - 1] + strainRateSample;
            }
            actSample.SimulationData[experimentIndex].StrainSFHistory.Add(actStrainS.Clone());

            //5.
            //for (int phase = 0; phase < actSample.CrystalData.Count; phase++)
            //{
            //    ////6.
            //    //MathNet.Numerics.LinearAlgebra.Matrix<double> strainRateG = grainTransitionStiffnesses[phase][0] * strainRateSample;
            //    //actSample.SimulationData[experimentIndex].StrainRateCFHistory[phase].Add(strainRateG);

            //    ////7.
            //    //actStrainG[phase] += strainRateG;
            //    //actSample.SimulationData[experimentIndex].StrainCFHistory[phase].Add(actStrainG[phase].Clone());

            //    ////8.
            //    //MathNet.Numerics.LinearAlgebra.Matrix<double> elasticStrainRateG = strainRateG - actStrainRatePlG[phase];
            //    //MathNet.Numerics.LinearAlgebra.Matrix<double> stressRateG = overallStiffnessesPhase[phase] * elasticStrainRateG;
            //    //actSample.SimulationData[experimentIndex].StressRateCFHistory[phase].Add(stressRateG);

            //    ////9.
            //    //actStressG[phase] += stressRateG;
            //    //actSample.SimulationData[experimentIndex].StressCFHistory[phase].Add(actStressG[phase].Clone());

            //    ////10.
            //    //actSample.SimulationData[experimentIndex].ActiveSystemsCFHistory[phase].Add(potentialActiveG[phase]);

            //}

            if (correctArea)
            {
                double radius = Math.Sqrt(actSample.SimulationData[experimentIndex].SampleArea / Math.PI);

                double ellipseA = radius * (1 + actStrainS[0, 0]);
                double ellipseB = radius * (1 + actStrainS[1, 1]);

                double newArea = Math.PI * ellipseA * ellipseB;
                double appliedForce = actSample.SimulationData[experimentIndex].StressSFHistory[n][2, 2] * actSample.SimulationData[experimentIndex].SampleArea;

                MathNet.Numerics.LinearAlgebra.Matrix<double> newStressTensor = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense<double>(3, 3, 0);
                newStressTensor[2, 2] = appliedForce / newArea;

                actSample.SimulationData[experimentIndex].StressSFHistoryAreaCorrected.Add(newStressTensor);
            }
        }

        #endregion

        public static double GetTensorDifference(MathNet.Numerics.LinearAlgebra.Matrix<double> mat1, MathNet.Numerics.LinearAlgebra.Matrix<double>  mat2)
        {
            double ret = 0.0;

            for(int n = 0; n < mat1.RowCount; n++)
            {
                for (int i = 0; i < mat1.ColumnCount; i++)
                {
                    double val = Math.Abs(mat1[n, i] - mat2[n, i]);
                    if(mat1[n, i] != 0.0)
                    {
                        val /= Math.Abs(mat1[n, i]);
                    }
                    ret += val;
                }
            }

            ret /= (mat1.RowCount * mat1.ColumnCount);

            return ret;
        }
        

    }
}
