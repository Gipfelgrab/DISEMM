using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.Analysis.Stress.Plasticity
{
    public class YieldSurface
    {
        public bool _useAnisotropy = false;
        public List<ReflexYield> ReflexYieldData = new List<ReflexYield>();
        public List<ReflexYield> PotentialSlipSystems = new List<ReflexYield>();
        public  DataManagment.CrystalData.CODData CrystalData;

        public YieldSurface(DataManagment.CrystalData.CODData crystalData)
        {
            for(int n = 0; n < crystalData.HKLList.Count; n++)
            {
                ReflexYield RYTmp = new ReflexYield(crystalData.HKLList[n], crystalData);
                this.CrystalData = crystalData;
                this.ReflexYieldData.Add(RYTmp);
            }

            #region Slip direction

            string RessourcePath = System.IO.Path.Combine(Environment.CurrentDirectory, @"Res\ReflectionConditions\SlipSystems.xml");
            using (System.IO.StreamReader XMLResStream = new System.IO.StreamReader(RessourcePath))
            {
                using (System.Xml.XmlTextReader ReflexReader = new System.Xml.XmlTextReader(XMLResStream))
                {
                    int conditionFailed = 0;
                    while (ReflexReader.Read())
                    {
                        int firstConditionFailed = 0;
                        if (ReflexReader.NodeType == System.Xml.XmlNodeType.Element)
                        {
                            if (ReflexReader.Name == "SlipSystem")
                            {

                                bool systemReadAktive = true;
                                while (systemReadAktive)
                                {
                                    ReflexReader.Read();

                                    if (ReflexReader.NodeType == System.Xml.XmlNodeType.Element)
                                    {
                                        ReflexYield RYTmp = new ReflexYield();
                                        switch (ReflexReader.Name)
                                        {
                                            case ("SpaceGroup"):
                                                ReflexReader.Read();
                                                if (ReflexReader.NodeType == System.Xml.XmlNodeType.Text)
                                                {
                                                    if (!(crystalData.SymmetryGroup == ReflexReader.Value))
                                                    {
                                                        firstConditionFailed++;
                                                    }
                                                }
                                                break;
                                            case ("SpaceGroupId"):
                                                ReflexReader.Read();
                                                if (ReflexReader.NodeType == System.Xml.XmlNodeType.Text)
                                                {
                                                    if (!(crystalData.SymmetryGroupID == Convert.ToInt32(ReflexReader.Value)))
                                                    {
                                                        firstConditionFailed++;
                                                    }
                                                }
                                                break;
                                            case ("SlipPlain"):
                                                ReflexReader.Read();
                                                if (ReflexReader.NodeType == System.Xml.XmlNodeType.Text)
                                                {
                                                    firstConditionFailed = 0;
                                                    string PlainString = ReflexReader.Value;

                                                    int PH = Convert.ToInt32(PlainString[0].ToString());
                                                    int PK = Convert.ToInt32(PlainString[1].ToString());
                                                    int PL = Convert.ToInt32(PlainString[2].ToString());

                                                    DataManagment.CrystalData.HKLReflex slipPlane = new DataManagment.CrystalData.HKLReflex(PH, PK, PL, 1);
                                                    RYTmp.SlipPlane = slipPlane;
                                                    bool InnerSystemRead = true;
                                                    while (InnerSystemRead)
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

                                                                        RYTmp.MainSlipDirection = new DataManagment.CrystalData.HKLReflex(SlipH, SlipK, SlipL, Tools.Calculation.CalculateHKLDistance(SlipH, SlipK, SlipL, crystalData));
                                                                    }
                                                                    break;
                                                                case ("PlainCount"):
                                                                    ReflexReader.Read();
                                                                    if (ReflexReader.NodeType == System.Xml.XmlNodeType.Text)
                                                                    {
                                                                        RYTmp.PlainMainMultiplizity = Convert.ToInt32(ReflexReader.Value);
                                                                    }
                                                                    break;
                                                                case ("DirectionCount"):
                                                                    ReflexReader.Read();
                                                                    if (ReflexReader.NodeType == System.Xml.XmlNodeType.Text)
                                                                    {
                                                                        RYTmp.DirectionMainMultiplizity = Convert.ToInt32(ReflexReader.Value);
                                                                    }
                                                                    InnerSystemRead = false;
                                                                    this.PotentialSlipSystems.Add(RYTmp);
                                                                    break;
                                                            }
                                                        }
                                                    }
                                                }
                                                break;
                                        }
                                        if(firstConditionFailed > 1)
                                        {
                                            firstConditionFailed = 0;
                                            break;
                                        }
                                    }

                                    if (conditionFailed == 150)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        conditionFailed++;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            #endregion
        }

        public double GetMainYieldStrenght(DataManagment.CrystalData.HKLReflex reflex)
        {
            for(int n = 0; n < ReflexYieldData.Count; n++)
            {
                if(reflex.H == ReflexYieldData[n].SlipPlane.H && reflex.K == ReflexYieldData[n].SlipPlane.K && reflex.L == ReflexYieldData[n].SlipPlane.L)
                {
                    if (_useAnisotropy)
                    {
                        return ReflexYieldData[n].YieldMainStrength;
                    }
                    else
                    {
                        return ReflexYieldData[n].YieldMainStrength;
                    }
                }
            }

            return -1;
        }

        public List<ReflexYield> GetPotentiallyActiveSlipSystems(MathNet.Numerics.LinearAlgebra.Vector<double> aStress)
        {
            List<ReflexYield> Ret = new List<ReflexYield>();

            for (int n = 0; n < this.PotentialSlipSystems.Count; n++)
            {
                if(this.CheckForMise(this.PotentialSlipSystems[n].SlipPlane, aStress))
                {
                    Ret.Add(this.PotentialSlipSystems[n]);
                }
            }

            return Ret;
        }

        public double Shearforce(MathNet.Numerics.LinearAlgebra.Vector<double> aStress)
        {
            double shearForce = Math.Pow(aStress[0] - aStress[1], 2);
            shearForce += Math.Pow(aStress[1] - aStress[2], 2);
            shearForce += Math.Pow(aStress[0] - aStress[2], 2);
            shearForce += Math.Pow(aStress[3], 2);
            shearForce += Math.Pow(aStress[4], 2);
            shearForce += Math.Pow(aStress[5], 2);
            shearForce = Math.Sqrt((1 / 6) * shearForce);

            return shearForce;
        }

        public double Shearforce(MathNet.Numerics.LinearAlgebra.Matrix<double> aStress)
        {
            double shearForce = Math.Pow(aStress[0, 0] - aStress[1, 1], 2);
            shearForce += Math.Pow(aStress[1, 1] - aStress[2, 2], 2);
            shearForce += Math.Pow(aStress[0, 0] - aStress[2, 2], 2);
            shearForce += Math.Pow(aStress[1 , 2], 2);
            shearForce += Math.Pow(aStress[0, 2], 2);
            shearForce += Math.Pow(aStress[0, 1], 2);
            shearForce = Math.Sqrt((1 / 6) * shearForce);

            return shearForce;
        }

        /// <summary>
        /// Checks if the yield critereon after Mise holds
        /// </summary>
        /// <param name="reflex"></param>
        /// <param name="aStress">
        /// [0]: 11
        /// [1]: 22
        /// [2]: 33
        /// [3]: 23
        /// [4]: 13
        /// [5]: 12
        /// </param>
        /// <returns>false: systemYield > shearForce, true slip system is active</returns>
        public bool CheckForMise(DataManagment.CrystalData.HKLReflex reflex, MathNet.Numerics.LinearAlgebra.Vector<double> aStress)
        {
            double systemYield = this.GetMainYieldStrenght(reflex);

            double shearForcet = Math.Pow(aStress[0] - aStress[1], 2);
            shearForcet += Math.Pow(aStress[1] - aStress[2], 2);
            shearForcet += Math.Pow(aStress[0] - aStress[2], 2);
            shearForcet += Math.Pow(aStress[3], 2);
            shearForcet += Math.Pow(aStress[4], 2);
            shearForcet += Math.Pow(aStress[5], 2);
            double shearForce = Math.Sqrt((1.0 / 6.0) * shearForcet);

            if((systemYield - shearForce) > 0.0)
            {
                return false;
            }
            else
            {
                return true;
            }
            
        }

        /// <summary>
        /// Checks if the yield critereon after Tresca holds
        /// </summary>
        /// <param name="reflex"></param>
        /// <param name="aStress">
        /// [0]: 11
        /// [1]: 22
        /// [2]: 33
        /// [3]: 23
        /// [4]: 13
        /// [5]: 12
        /// </param>
        /// <returns>false: systemYield > shearForce, true slip system is active</returns>
        public bool CheckForTresca(DataManagment.CrystalData.HKLReflex reflex, MathNet.Numerics.LinearAlgebra.Vector<double> aStress)
        {
            double systemYield = this.GetMainYieldStrenght(reflex);

            double maxPrincipleStress = 0;
            double minPrincipleStress = Double.MaxValue;
            for ( int n = 0; n < 2; n++)
            {
                if(aStress[n] > maxPrincipleStress)
                {
                    maxPrincipleStress = aStress[n];
                }
            }
            for (int n = 0; n < 2; n++)
            {
                if (aStress[n] < maxPrincipleStress)
                {
                    maxPrincipleStress = aStress[n];
                }
            }

            double shearForce = (maxPrincipleStress - minPrincipleStress) / 2.0;

            if ((systemYield - shearForce) > 0)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        public MathNet.Numerics.LinearAlgebra.Matrix<double> HardeningMatrixSlipSystem(List<ReflexYield> potActiveSystems, MathNet.Numerics.LinearAlgebra.Matrix<double> hardeningMatrix)
        {
            MathNet.Numerics.LinearAlgebra.Matrix<double> ret = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(potActiveSystems.Count, potActiveSystems.Count, 0.0);
            for (int n = 0; n < potActiveSystems.Count; n++)
            {
                for (int i = 0; i < potActiveSystems.Count; i++)
                {
                    MathNet.Numerics.LinearAlgebra.Vector<double> directionHardeningN = hardeningMatrix * potActiveSystems[n].MainSlipDirection.Direction;
                    MathNet.Numerics.LinearAlgebra.Vector<double> directionHardeningI = hardeningMatrix * potActiveSystems[i].MainSlipDirection.Direction;
                    ret[n, i] = directionHardeningN * directionHardeningI;
                }
            }

            return ret;
        }

        public MathNet.Numerics.LinearAlgebra.Matrix<double> SlipSystemX(List<ReflexYield> potActiveSystems, MathNet.Numerics.LinearAlgebra.Matrix<double> hardeningMatrix, Microsopic.ElasticityTensors eT)
        {
            MathNet.Numerics.LinearAlgebra.Matrix<double> ret = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(potActiveSystems.Count, potActiveSystems.Count, 0.0);

            for (int n = 0; n < potActiveSystems.Count; n++)
            {
                for (int i = 0; i < potActiveSystems.Count; i++)
                {
                    MathNet.Numerics.LinearAlgebra.Vector<double> directionHardeningN = hardeningMatrix * potActiveSystems[n].MainSlipDirection.Direction;
                    MathNet.Numerics.LinearAlgebra.Vector<double> directionHardeningI = hardeningMatrix * potActiveSystems[i].MainSlipDirection.Direction;
                    ret[n, i] = directionHardeningN * directionHardeningI;

                    DiffractionOrientation.OrientationMatrix oMN = new DiffractionOrientation.OrientationMatrix();
                    DiffractionOrientation.OrientationMatrix oMI = new DiffractionOrientation.OrientationMatrix();

                    if (this.CrystalData.SymmetryGroupID == 168)
                    {
                        oMN.AsMainAxisTransformation(potActiveSystems[n].MainSlipDirection.H, potActiveSystems[n].MainSlipDirection.K, potActiveSystems[n].MainSlipDirection.L, 1);
                    }
                    else
                    {
                        oMN.AsMainAxisTransformation(potActiveSystems[n].MainSlipDirection.H, potActiveSystems[n].MainSlipDirection.K, potActiveSystems[n].MainSlipDirection.L, 0);
                    }
                    if (this.CrystalData.SymmetryGroupID == 168)
                    {
                        oMI.AsMainAxisTransformation(potActiveSystems[i].MainSlipDirection.H, potActiveSystems[n].MainSlipDirection.K, potActiveSystems[n].MainSlipDirection.L, 1);
                    }
                    else
                    {
                        oMI.AsMainAxisTransformation(potActiveSystems[i].MainSlipDirection.H, potActiveSystems[n].MainSlipDirection.K, potActiveSystems[n].MainSlipDirection.L, 0);
                    }

                    Tools.FourthRankTensor compliances = new Tools.FourthRankTensor(eT._complianceTensor);
                    double hV = compliances.InnerTransormation(oMN.OM, oMI.OM);

                    ret[n, i] += hV;
                }
            }

            return ret;
        }

        public MathNet.Numerics.LinearAlgebra.Matrix<double> GetInstComplianceFactors(List<ReflexYield> potActiveSystems, MathNet.Numerics.LinearAlgebra.Matrix<double> hardeningMatrix, Microsopic.ElasticityTensors eT, int slipsSystemIndex)
        {
            MathNet.Numerics.LinearAlgebra.Matrix<double> ret = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);
            MathNet.Numerics.LinearAlgebra.Matrix<double> slipSystemX = this.SlipSystemX(potActiveSystems, hardeningMatrix, eT);
            MathNet.Numerics.LinearAlgebra.Matrix<double> slipSystemY = slipSystemX.Inverse();
            MathNet.Numerics.LinearAlgebra.Matrix<double> hardMatrix = this.HardeningMatrixSlipSystem(potActiveSystems, hardeningMatrix);

            Tools.FourthRankTensor compliances = eT.GetFourtRankCompliances();
            
            for (int n = 0; n < potActiveSystems.Count; n++)
            {
                MathNet.Numerics.LinearAlgebra.Matrix<double> alpha = potActiveSystems[n].GetResolvingParameterMain();

                ret += hardMatrix[slipsSystemIndex, n] * (compliances * alpha);
            }

            return ret;
        }

        public double GetShearRate(MathNet.Numerics.LinearAlgebra.Matrix<double> iCF, MathNet.Numerics.LinearAlgebra.Matrix<double> grainStrain)
        {
            double ret = 0.0;

            for(int n = 0; n < 3; n++)
            {
                for (int i = 0; i < 3; i++)
                {
                    ret += iCF[n, i] * grainStrain[n, i];
                }
            }

            return ret;
        }

        #region Data display
        
        //public Pattern.Counts MacroStrainOverMacroStrainData(double psiAngle)
        //{
        //    Pattern.Counts Ret = new Pattern.Counts();

        //    for (int g = 0; g < this.ReflexYieldData.Count; g++)
        //    {
        //        for (int n = 0; n < this.ReflexYieldData[g].PeakData.Count; n++)
        //        {
        //            if (Math.Abs(psiAngle - this.ReflexYieldData[g].PeakData[n][0].PsiAngle) < 2.5)
        //            {
        //                for (int i = 0; i < this.ReflexYieldData[g].PeakData[n].Count; i++)
        //                {
        //                    if (g == 0)
        //                    {
        //                        double[] CTmp = { this.ReflexYieldData[g].PeakData[n][i].MacroskopicStrain, this.ReflexYieldData[g].PeakData[n][i].MacroskopicStrain, (this.ReflexYieldData[g].PeakData[n][i].Stress * Math.Cos(this.ReflexYieldData[g].PeakData[n][i].PsiAngle) * Math.Cos(this.ReflexYieldData[g].PeakData[n][i].MainSlipDirectionAngle)) };

        //                        Ret.Add(CTmp);
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return Ret;
        //}
        //public Pattern.Counts MacroStrainOverMicroStrainData(double psiAngle)
        //{
        //    Pattern.Counts Ret = new Pattern.Counts();

        //    for (int g = 0; g < this.ReflexYieldData.Count; g++)
        //    {
        //        for (int n = 0; n < this.ReflexYieldData[g].PeakData.Count; n++)
        //        {
        //            if (Math.Abs(psiAngle - this.ReflexYieldData[g].PeakData[n][0].PsiAngle) < 2.5)
        //            {
        //                for (int i = 0; i < this.ReflexYieldData[g].PeakData[n].Count; i++)
        //                {
        //                    if (g == 0)
        //                    {
        //                        double[] CTmp = { this.ReflexYieldData[g].PeakData[n][i].Strain, this.ReflexYieldData[g].PeakData[n][i].MacroskopicStrain, (this.ReflexYieldData[g].PeakData[n][i].Stress * Math.Cos(this.ReflexYieldData[g].PeakData[n][i].PsiAngle) * Math.Cos(this.ReflexYieldData[g].PeakData[n][i].MainSlipDirectionAngle)) };

        //                        Ret.Add(CTmp);
        //                    }
        //                    else
        //                    {
        //                        Ret[i][0] += this.ReflexYieldData[g].PeakData[n][i].Strain;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return Ret;
        //}
        //public Pattern.Counts MacroStrainDataOverStress(double psiAngle)
        //{
        //    Pattern.Counts Ret = new Pattern.Counts();

        //    for (int g = 0; g < this.ReflexYieldData.Count; g++)
        //    {
        //        for (int n = 0; n < this.ReflexYieldData[g].PeakData.Count; n++)
        //        {
        //            if (Math.Abs(psiAngle - this.ReflexYieldData[g].PeakData[n][0].PsiAngle) < 5.0)
        //            {
        //                for (int i = 0; i < this.ReflexYieldData[g].PeakData[n].Count; i++)
        //                {
        //                    if (g == 0)
        //                    {
        //                        double[] CTmp = { (this.ReflexYieldData[g].PeakData[n][i].Stress), this.ReflexYieldData[g].PeakData[n][i].MacroskopicStrain, this.ReflexYieldData[g].PeakData[n][i].MacroskopicStrain };

        //                        Ret.Add(CTmp);
        //                    }
        //                    else
        //                    {
                                
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return Ret;
        //}
        //public Pattern.Counts MacroStrainDataOverPsiAdjustedStress(double psiAngle)
        //{
        //    Pattern.Counts Ret = new Pattern.Counts();

        //    for (int g = 0; g < this.ReflexYieldData.Count; g++)
        //    {
        //        for (int n = 0; n < this.ReflexYieldData[g].PeakData.Count; n++)
        //        {
        //            if (Math.Abs(psiAngle - this.ReflexYieldData[g].PeakData[n][0].PsiAngle) < 5.0)
        //            {
        //                for (int i = 0; i < this.ReflexYieldData[g].PeakData[n].Count; i++)
        //                {
        //                    if (g == 0)
        //                    {
        //                        double[] CTmp = { (this.ReflexYieldData[g].PeakData[n][i].Stress * Math.Cos(this.ReflexYieldData[g].PeakData[n][i].PsiAngle)), this.ReflexYieldData[g].PeakData[n][i].MacroskopicStrain, this.ReflexYieldData[g].PeakData[n][i].MacroskopicStrain };

        //                        Ret.Add(CTmp);
        //                    }
        //                    else
        //                    {

        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return Ret;
        //}
        //public Pattern.Counts MacroStrainDataOverPlainAdjustedStress(double psiAngle)
        //{
        //    Pattern.Counts Ret = new Pattern.Counts();

        //    for (int g = 0; g < this.ReflexYieldData.Count; g++)
        //    {
        //        for (int n = 0; n < this.ReflexYieldData[g].PeakData.Count; n++)
        //        {
        //            if (Math.Abs(psiAngle - this.ReflexYieldData[g].PeakData[n][0].PsiAngle) < 5.0)
        //            {
        //                for (int i = 0; i < this.ReflexYieldData[g].PeakData[n].Count; i++)
        //                {
        //                    if (g == 0)
        //                    {
        //                        double[] CTmp = { (this.ReflexYieldData[g].PeakData[n][i].Stress * Math.Cos(this.ReflexYieldData[g].PeakData[n][i].PsiAngle) * Math.Cos(this.ReflexYieldData[g].PeakData[n][i].MainSlipDirectionAngle)), this.ReflexYieldData[g].PeakData[n][i].MacroskopicStrain, this.ReflexYieldData[g].PeakData[n][i].MacroskopicStrain };

        //                        Ret.Add(CTmp);
        //                    }
        //                    else
        //                    {
        //                        Ret[i][0] += (this.ReflexYieldData[g].PeakData[n][i].Stress * Math.Cos(this.ReflexYieldData[g].PeakData[n][i].PsiAngle) * Math.Cos(this.ReflexYieldData[g].PeakData[n][i].MainSlipDirectionAngle));
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return Ret;
        //}

        //public Pattern.Counts MicroStrainOverMacroStrainData(double psiAngle)
        //{
        //    Pattern.Counts Ret = new Pattern.Counts();
        //    for (int g = 0; g < this.ReflexYieldData.Count; g++)
        //    {
        //        for (int n = 0; n < this.ReflexYieldData[g].PeakData.Count; n++)
        //        {
        //            if (Math.Abs(psiAngle - this.ReflexYieldData[g].PeakData[n][0].PsiAngle) < 5.0)
        //            {
        //                for (int i = 0; i < this.ReflexYieldData[g].PeakData[n].Count; i++)
        //                {
        //                    if (g == 0)
        //                    {
        //                        double[] CTmp = { this.ReflexYieldData[g].PeakData[n][i].MacroskopicStrain, this.ReflexYieldData[g].PeakData[n][i].Strain, (this.ReflexYieldData[g].PeakData[n][i].Stress * Math.Cos(this.ReflexYieldData[g].PeakData[n][i].PsiAngle) * Math.Cos(this.ReflexYieldData[g].PeakData[n][i].MainSlipDirectionAngle)) };

        //                        Ret.Add(CTmp);
        //                    }
        //                    else
        //                    {
        //                        if (Ret.Count > i)
        //                        {
        //                            Ret[i][1] += this.ReflexYieldData[g].PeakData[n][i].Strain;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return Ret;
        //}
        //public Pattern.Counts MicroStrainOverMicroStrainData(double psiAngle)
        //{
        //    Pattern.Counts Ret = new Pattern.Counts();
        //    for (int g = 0; g < this.ReflexYieldData.Count; g++)
        //    {
        //        for (int n = 0; n < this.ReflexYieldData[g].PeakData.Count; n++)
        //        {
        //            if (Math.Abs(psiAngle - this.ReflexYieldData[g].PeakData[n][0].PsiAngle) < 5.0)
        //            {
        //                for (int i = 0; i < this.ReflexYieldData[g].PeakData[n].Count; i++)
        //                {
        //                    if (g == 0)
        //                    {
        //                        double[] CTmp = { this.ReflexYieldData[g].PeakData[n][i].Strain, this.ReflexYieldData[g].PeakData[n][i].Strain, (this.ReflexYieldData[g].PeakData[n][i].Stress * Math.Cos(this.ReflexYieldData[g].PeakData[n][i].PsiAngle) * Math.Cos(this.ReflexYieldData[g].PeakData[n][i].MainSlipDirectionAngle)) };

        //                        Ret.Add(CTmp);
        //                    }
        //                    else
        //                    {
        //                        Ret[i][0] += this.ReflexYieldData[g].PeakData[n][i].Strain;
        //                        Ret[i][1] += this.ReflexYieldData[g].PeakData[n][i].Strain;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return Ret;
        //}
        //public Pattern.Counts MicroStrainDataOverStress(double psiAngle)
        //{
        //    Pattern.Counts Ret = new Pattern.Counts();

        //    for (int g = 0; g < this.ReflexYieldData.Count; g++)
        //    {
        //        for (int n = 0; n < this.ReflexYieldData[g].PeakData.Count; n++)
        //        {
        //            if (Math.Abs(psiAngle - this.ReflexYieldData[g].PeakData[n][0].PsiAngle) < 5.0)
        //            {
        //                for (int i = 0; i < this.ReflexYieldData[g].PeakData[n].Count; i++)
        //                {
        //                    if (g == 0)
        //                    {
        //                        double[] CTmp = { (this.ReflexYieldData[g].PeakData[n][i].Stress), this.ReflexYieldData[g].PeakData[n][i].Strain, this.ReflexYieldData[g].PeakData[n][i].MacroskopicStrain };

        //                        Ret.Add(CTmp);
        //                    }
        //                    else
        //                    {
        //                        Ret[i][1] += this.ReflexYieldData[g].PeakData[n][i].Strain;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return Ret;
        //}
        //public Pattern.Counts MicroStrainDataOverPsiAdjustedStress(double psiAngle)
        //{
        //    Pattern.Counts Ret = new Pattern.Counts();

        //    for (int g = 0; g < this.ReflexYieldData.Count; g++)
        //    {
        //        for (int n = 0; n < this.ReflexYieldData[g].PeakData.Count; n++)
        //        {
        //            if (Math.Abs(psiAngle - this.ReflexYieldData[g].PeakData[n][0].PsiAngle) < 5.0)
        //            {
        //                for (int i = 0; i < this.ReflexYieldData[g].PeakData[n].Count; i++)
        //                {
        //                    if (g == 0)
        //                    {
        //                        double[] CTmp = { (this.ReflexYieldData[g].PeakData[n][i].Stress * Math.Cos(this.ReflexYieldData[g].PeakData[n][i].PsiAngle)), this.ReflexYieldData[g].PeakData[n][i].Strain, this.ReflexYieldData[g].PeakData[n][i].MacroskopicStrain };

        //                        Ret.Add(CTmp);
        //                    }
        //                    else
        //                    {
        //                        Ret[i][1] += this.ReflexYieldData[g].PeakData[n][i].Strain;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return Ret;
        //}
        //public Pattern.Counts MicroStrainDataOverPlainAdjustedStress(double psiAngle)
        //{
        //    Pattern.Counts Ret = new Pattern.Counts();

        //    for (int g = 0; g < this.ReflexYieldData.Count; g++)
        //    {
        //        for (int n = 0; n < this.ReflexYieldData[g].PeakData.Count; n++)
        //        {
        //            if (Math.Abs(psiAngle - this.ReflexYieldData[g].PeakData[n][0].PsiAngle) < 5.0)
        //            {
        //                for (int i = 0; i < this.ReflexYieldData[g].PeakData[n].Count; i++)
        //                {
        //                    if (g == 0)
        //                    {
        //                        double[] CTmp = { (this.ReflexYieldData[g].PeakData[n][i].Stress * Math.Cos(this.ReflexYieldData[g].PeakData[n][i].PsiAngle) * Math.Cos(this.ReflexYieldData[g].PeakData[n][i].MainSlipDirectionAngle)), this.ReflexYieldData[g].PeakData[n][i].Strain, this.ReflexYieldData[g].PeakData[n][i].MacroskopicStrain };

        //                        Ret.Add(CTmp);
        //                    }
        //                    else
        //                    {
        //                        Ret[i][0] += (this.ReflexYieldData[g].PeakData[n][i].Stress * Math.Cos(this.ReflexYieldData[g].PeakData[n][i].PsiAngle) * Math.Cos(this.ReflexYieldData[g].PeakData[n][i].MainSlipDirectionAngle));
        //                        Ret[i][1] += this.ReflexYieldData[g].PeakData[n][i].Strain;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return Ret;
        //}

        //public Pattern.Counts StressOverMacroStrainData(double psiAngle)
        //{
        //    Pattern.Counts Ret = new Pattern.Counts();

        //    for (int g = 0; g < this.ReflexYieldData.Count; g++)
        //    {
        //        for (int n = 0; n < this.ReflexYieldData[g].PeakData.Count; n++)
        //        {
        //            if (Math.Abs(psiAngle - this.ReflexYieldData[g].PeakData[n][0].PsiAngle) < 5.0)
        //            {
        //                for (int i = 0; i < this.ReflexYieldData[g].PeakData[n].Count; i++)
        //                {
        //                    if (g == 0)
        //                    {
        //                        double[] CTmp = { this.ReflexYieldData[g].PeakData[n][i].MacroskopicStrain, (this.ReflexYieldData[g].PeakData[n][i].Stress), this.ReflexYieldData[g].PeakData[n][i].MacroskopicStrain };

        //                        Ret.Add(CTmp);
        //                    }
        //                    else
        //                    {
                                
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return Ret;
        //}
        //public Pattern.Counts StressOverMicroStrainData(double psiAngle)
        //{
        //    Pattern.Counts Ret = new Pattern.Counts();

        //    for (int g = 0; g < this.ReflexYieldData.Count; g++)
        //    {
        //        for (int n = 0; n < this.ReflexYieldData[g].PeakData.Count; n++)
        //        {
        //            if (Math.Abs(psiAngle - this.ReflexYieldData[g].PeakData[n][0].PsiAngle) < 5.0)
        //            {
        //                for (int i = 0; i < this.ReflexYieldData[g].PeakData[n].Count; i++)
        //                {
        //                    if (g == 0)
        //                    {
        //                        double[] CTmp = { this.ReflexYieldData[g].PeakData[n][i].Strain, (this.ReflexYieldData[g].PeakData[n][i].Stress), this.ReflexYieldData[g].PeakData[n][i].MacroskopicStrain };

        //                        Ret.Add(CTmp);
        //                    }
        //                    else
        //                    {
        //                        Ret[i][0] += this.ReflexYieldData[g].PeakData[n][i].Strain;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return Ret;
        //}
        //public Pattern.Counts StressOverStressData(double psiAngle)
        //{
        //    Pattern.Counts Ret = new Pattern.Counts();

        //    for (int g = 0; g < this.ReflexYieldData.Count; g++)
        //    {
        //        for (int n = 0; n < this.ReflexYieldData[g].PeakData.Count; n++)
        //        {
        //            if (Math.Abs(psiAngle - this.ReflexYieldData[g].PeakData[n][0].PsiAngle) < 5.0)
        //            {
        //                for (int i = 0; i < this.ReflexYieldData[g].PeakData[n].Count; i++)
        //                {
        //                    if (g == 0)
        //                    {
        //                        double[] CTmp = { this.ReflexYieldData[g].PeakData[n][i].Stress, (this.ReflexYieldData[g].PeakData[n][i].Stress), this.ReflexYieldData[g].PeakData[n][i].MacroskopicStrain };

        //                        Ret.Add(CTmp);
        //                    }
        //                    else
        //                    {

        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return Ret;
        //}
        //public Pattern.Counts StressOverPsiAdjustedStressData(double psiAngle)
        //{
        //    Pattern.Counts Ret = new Pattern.Counts();

        //    for (int g = 0; g < this.ReflexYieldData.Count; g++)
        //    {
        //        for (int n = 0; n < this.ReflexYieldData[g].PeakData.Count; n++)
        //        {
        //            if (Math.Abs(psiAngle - this.ReflexYieldData[g].PeakData[n][0].PsiAngle) < 5.0)
        //            {
        //                for (int i = 0; i < this.ReflexYieldData[g].PeakData[n].Count; i++)
        //                {
        //                    if (g == 0)
        //                    {
        //                        double[] CTmp = { (this.ReflexYieldData[g].PeakData[n][i].Stress * Math.Cos(this.ReflexYieldData[g].PeakData[n][i].PsiAngle)), (this.ReflexYieldData[g].PeakData[n][i].Stress), this.ReflexYieldData[g].PeakData[n][i].MacroskopicStrain };

        //                        Ret.Add(CTmp);
        //                    }
        //                    else
        //                    {

        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return Ret;
        //}
        //public Pattern.Counts StressOverPlainAdjustedStressData(double psiAngle)
        //{
        //    Pattern.Counts Ret = new Pattern.Counts();

        //    for (int g = 0; g < this.ReflexYieldData.Count; g++)
        //    {
        //        for (int n = 0; n < this.ReflexYieldData[g].PeakData.Count; n++)
        //        {
        //            if (Math.Abs(psiAngle - this.ReflexYieldData[g].PeakData[n][0].PsiAngle) < 5.0)
        //            {
        //                for (int i = 0; i < this.ReflexYieldData[g].PeakData[n].Count; i++)
        //                {
        //                    if (g == 0)
        //                    {
        //                        double[] CTmp = { (this.ReflexYieldData[g].PeakData[n][i].Stress * Math.Cos(this.ReflexYieldData[g].PeakData[n][i].PsiAngle) * Math.Cos(this.ReflexYieldData[g].PeakData[n][i].MainSlipDirectionAngle)), (this.ReflexYieldData[g].PeakData[n][i].Stress), this.ReflexYieldData[g].PeakData[n][i].MacroskopicStrain };

        //                        Ret.Add(CTmp);
        //                    }
        //                    else
        //                    {
        //                        Ret[i][0] += (this.ReflexYieldData[g].PeakData[n][i].Stress * Math.Cos(this.ReflexYieldData[g].PeakData[n][i].PsiAngle) * Math.Cos(this.ReflexYieldData[g].PeakData[n][i].MainSlipDirectionAngle));
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return Ret;
        //}


        //public Pattern.Counts PlainAdjustedStressOverMicroStrainData(double psiAngle)
        //{
        //    Pattern.Counts Ret = new Pattern.Counts();

        //    for (int g = 0; g < this.ReflexYieldData.Count; g++)
        //    {
        //        for (int n = 0; n < this.ReflexYieldData[g].PeakData.Count; n++)
        //        {
        //            if (Math.Abs(psiAngle - this.ReflexYieldData[g].PeakData[n][0].PsiAngle) < 5.0)
        //            {
        //                for (int i = 0; i < this.ReflexYieldData[g].PeakData[n].Count; i++)
        //                {
        //                    if (g == 0)
        //                    {
        //                        double[] CTmp = { this.ReflexYieldData[g].PeakData[n][i].Strain, (this.ReflexYieldData[g].PeakData[n][i].Stress * Math.Cos(this.ReflexYieldData[g].PeakData[n][i].PsiAngle) * Math.Cos(this.ReflexYieldData[g].PeakData[n][i].MainSlipDirectionAngle)), this.ReflexYieldData[g].PeakData[n][i].MacroskopicStrain };

        //                        Ret.Add(CTmp);
        //                    }
        //                    else
        //                    {
        //                        Ret[i][0] += this.ReflexYieldData[g].PeakData[n][i].Strain;
        //                        Ret[i][1] += (this.ReflexYieldData[g].PeakData[n][i].Stress * Math.Cos(this.ReflexYieldData[g].PeakData[n][i].PsiAngle) * Math.Cos(this.ReflexYieldData[g].PeakData[n][i].MainSlipDirectionAngle));
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return Ret;
        //}
        //public Pattern.Counts PsiAdjustedStressOverMicroStrainData(double psiAngle)
        //{
        //    Pattern.Counts Ret = new Pattern.Counts();

        //    for (int g = 0; g < this.ReflexYieldData.Count; g++)
        //    {
        //        for (int n = 0; n < this.ReflexYieldData[g].PeakData.Count; n++)
        //        {
        //            if (Math.Abs(psiAngle - this.ReflexYieldData[g].PeakData[n][0].PsiAngle) < 5.0)
        //            {
        //                for (int i = 0; i < this.ReflexYieldData[g].PeakData[n].Count; i++)
        //                {
        //                    if (g == 0)
        //                    {
        //                        double[] CTmp = { this.ReflexYieldData[g].PeakData[n][i].Strain, (this.ReflexYieldData[g].PeakData[n][i].Stress * Math.Cos(this.ReflexYieldData[g].PeakData[n][i].PsiAngle)), this.ReflexYieldData[g].PeakData[n][i].MacroskopicStrain };

        //                        Ret.Add(CTmp);
        //                    }
        //                    else
        //                    {
        //                        Ret[i][0] += this.ReflexYieldData[g].PeakData[n][i].Strain;
        //                        Ret[i][1] += (this.ReflexYieldData[g].PeakData[n][i].Stress * Math.Cos(this.ReflexYieldData[g].PeakData[n][i].PsiAngle));
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return Ret;
        //}

        //public Pattern.Counts PlainAdjustedStressOverMacroStrainData(double psiAngle)
        //{
        //    Pattern.Counts Ret = new Pattern.Counts();

        //    for (int g = 0; g < this.ReflexYieldData.Count; g++)
        //    {
        //        for (int n = 0; n < this.ReflexYieldData[g].PeakData.Count; n++)
        //        {
        //            if (Math.Abs(psiAngle - this.ReflexYieldData[g].PeakData[n][0].PsiAngle) < 5.0)
        //            {
        //                for (int i = 0; i < this.ReflexYieldData[g].PeakData[n].Count; i++)
        //                {
        //                    if (g == 0)
        //                    {
        //                        double[] CTmp = { this.ReflexYieldData[g].PeakData[n][i].MacroskopicStrain, (this.ReflexYieldData[g].PeakData[n][i].Stress * Math.Cos(this.ReflexYieldData[g].PeakData[n][i].PsiAngle) * Math.Cos(this.ReflexYieldData[g].PeakData[n][i].MainSlipDirectionAngle)), this.ReflexYieldData[g].PeakData[n][i].MacroskopicStrain };

        //                        Ret.Add(CTmp);
        //                    }
        //                    else
        //                    {
        //                        Ret[i][1] += (this.ReflexYieldData[g].PeakData[n][i].Stress * Math.Cos(this.ReflexYieldData[g].PeakData[n][i].PsiAngle) * Math.Cos(this.ReflexYieldData[g].PeakData[n][i].MainSlipDirectionAngle));
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return Ret;
        //}
        //public Pattern.Counts PsiAdjustedStressOverMacroStrainData(double psiAngle)
        //{
        //    Pattern.Counts Ret = new Pattern.Counts();

        //    for (int g = 0; g < this.ReflexYieldData.Count; g++)
        //    {
        //        for (int n = 0; n < this.ReflexYieldData[g].PeakData.Count; n++)
        //        {
        //            if (Math.Abs(psiAngle - this.ReflexYieldData[g].PeakData[n][0].PsiAngle) < 5.0)
        //            {
        //                for (int i = 0; i < this.ReflexYieldData[g].PeakData[n].Count; i++)
        //                {
        //                    if (g == 0)
        //                    {
        //                        double[] CTmp = { this.ReflexYieldData[g].PeakData[n][i].MacroskopicStrain, (this.ReflexYieldData[g].PeakData[n][i].Stress * Math.Cos(this.ReflexYieldData[g].PeakData[n][i].PsiAngle)), this.ReflexYieldData[g].PeakData[n][i].MacroskopicStrain };

        //                        Ret.Add(CTmp);
        //                    }
        //                    else
        //                    {
        //                        Ret[i][1] += (this.ReflexYieldData[g].PeakData[n][i].Stress * Math.Cos(this.ReflexYieldData[g].PeakData[n][i].PsiAngle));
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return Ret;
        //}

        #endregion
    }
}
