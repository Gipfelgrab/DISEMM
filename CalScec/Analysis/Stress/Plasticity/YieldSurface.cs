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

                                    if (conditionFailed == 1500)
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

        //public double GetMainYieldStrenght(DataManagment.CrystalData.HKLReflex reflex)
        //{
        //    for(int n = 0; n < ReflexYieldData.Count; n++)
        //    {
        //        if(reflex.H == ReflexYieldData[n].SlipPlane.H && reflex.K == ReflexYieldData[n].SlipPlane.K && reflex.L == ReflexYieldData[n].SlipPlane.L)
        //        {
        //            if (_useAnisotropy)
        //            {
        //                return ReflexYieldData[n].YieldMainStrength;
        //            }
        //            else
        //            {
        //                return ReflexYieldData[n].YieldMainStrength;
        //            }
        //        }
        //    }

        //    return -1;
        //}

        //public List<ReflexYield> GetPotentiallyActiveSlipSystems(MathNet.Numerics.LinearAlgebra.Vector<double> aStress)
        //{
        //    List<ReflexYield> Ret = new List<ReflexYield>();

        //    for (int n = 0; n < this.PotentialSlipSystems.Count; n++)
        //    {
        //        if(this.CheckForMise(this.PotentialSlipSystems[n], aStress))
        //        {
        //            Ret.Add(this.PotentialSlipSystems[n]);
        //        }
        //    }

        //    return Ret;
        //}

        public List<double> GetResolvedStressList(MathNet.Numerics.LinearAlgebra.Matrix<double> aStress)
        {
            List<double> ret = new List<double>();
            for(int n = 0; n < this.PotentialSlipSystems.Count; n++)
            {
                double shearstress = this.ShearStress(this.PotentialSlipSystems[n], aStress);
                ret.Add(shearstress);
            }

            return ret;
        }

        public List<ReflexYield> GetPotentiallyActiveSlipSystems(MathNet.Numerics.LinearAlgebra.Matrix<double> aStress, int criterion)
        {
            List<ReflexYield> Ret = new List<ReflexYield>();

            for (int n = 0; n < this.PotentialSlipSystems.Count; n++)
            {
                if (this.PotentialSlipSystems[n].ActiveSystem == 1)
                {
                    switch (criterion)
                    {
                        case 0:
                            if(this.CheckStandard(this.PotentialSlipSystems[n], aStress))
                            {
                                Ret.Add(this.PotentialSlipSystems[n]);
                            }
                            break;
                        case 1:
                            if (this.CheckForMise(this.PotentialSlipSystems[n], aStress))
                            {
                                Ret.Add(this.PotentialSlipSystems[n]);
                            }
                            break;
                        case 2:
                            if (this.CheckForTresca(this.PotentialSlipSystems[n], aStress))
                            {
                                Ret.Add(this.PotentialSlipSystems[n]);
                            }
                            break;
                        default:
                            if (this.CheckStandard(this.PotentialSlipSystems[n], aStress))
                            {
                                Ret.Add(this.PotentialSlipSystems[n]);
                            }
                            break;
                    }
                }
            }

            return Ret;
        }

        /// <summary>
        /// Checks if the yield critereon after Mise holds
        /// </summary>
        /// <param name="reflex"></param>
        /// <param name="aStress"></param>
        /// <returns>false: systemYield > shearForce, true slip system is active</returns>
        public bool CheckForMise(ReflexYield reflex, MathNet.Numerics.LinearAlgebra.Matrix<double> aStress)
        {
            double shearForce = this.Shearforce(aStress);

            if ((reflex.YieldMainHardennedStrength - shearForce) > 0.0)
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
        /// <param name="aStress"></param>
        /// <returns>false: systemYield > shearForce, true slip system is active</returns>
        public bool CheckForTresca(ReflexYield reflex, MathNet.Numerics.LinearAlgebra.Matrix<double> aStress)
        {
            double shearForce = this.PrincibleStress(aStress);

            if ((reflex.YieldMainHardennedStrength - shearForce) > 0)
            {
                return false;
            }
            else
            {
                return true;
            }

        }
        public bool CheckStandard(ReflexYield reflex, MathNet.Numerics.LinearAlgebra.Matrix<double> aStress)
        {
            double shearstress = this.ShearStress(reflex, aStress);

            if (Math.Abs(shearstress) > reflex.YieldMainHardennedStrength)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public double Shearforce(MathNet.Numerics.LinearAlgebra.Matrix<double> aStress)
        {
            double shearForce = Math.Pow(aStress[0, 0] - aStress[1, 1], 2);
            shearForce += Math.Pow(aStress[1, 1] - aStress[2, 2], 2);
            shearForce += Math.Pow(aStress[0, 0] - aStress[2, 2], 2);
            shearForce += Math.Pow(aStress[1, 2], 2);
            shearForce += Math.Pow(aStress[0, 2], 2);
            shearForce += Math.Pow(aStress[0, 1], 2);
            shearForce = Math.Sqrt((1 / 6) * shearForce);

            return shearForce;
        }
        public double ShearStress(ReflexYield reflex, MathNet.Numerics.LinearAlgebra.Matrix<double> aStress)
        {
            MathNet.Numerics.LinearAlgebra.Matrix<double> resolvingMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

            if (this.CrystalData.SymmetryGroupID == 194)
            {
                //For TEsting
                resolvingMatrix = Tools.Calculation.GetResolvingParameterMainHex(reflex.SlipPlane, reflex.MainSlipDirection, this.CrystalData.A, this.CrystalData.C);

                //resolvingMatrix = Tools.Calculation.GetResolvingParameter(reflex.SlipPlane, reflex.MainSlipDirection);

            }
            else
            {
                resolvingMatrix = Tools.Calculation.GetResolvingParameter(reflex.SlipPlane, reflex.MainSlipDirection);
            }
            //resolvingMatrix = Tools.Calculation.GetResolvingParameter(reflex.SlipPlane, reflex.MainSlipDirection);

            double ret = 0;

            for (int n = 0; n < 3; n++)
            {
                for (int m = 0; m < 3; m++)
                {
                    ret += aStress[n, m] * resolvingMatrix[n, m];
                }
            }

            return ret;
        }
        public double PrincibleStress(MathNet.Numerics.LinearAlgebra.Matrix<double> aStress)
        {
            double maxPrincipleStress = 0;
            double minPrincipleStress = Double.MaxValue;
            for (int n = 0; n < 3; n++)
            {
                if (aStress[n, n] > maxPrincipleStress)
                {
                    maxPrincipleStress = aStress[n, n];
                }
            }
            for (int n = 0; n < 3; n++)
            {
                if (aStress[n, n] < maxPrincipleStress)
                {
                    maxPrincipleStress = aStress[n, n];
                }
            }

            double shearForce = (maxPrincipleStress - minPrincipleStress) / 2.0;
            return shearForce;
        }

        public List<ReflexYield> CheckDependencies(List<ReflexYield> potSystems, MathNet.Numerics.LinearAlgebra.Matrix<double> aStress)
        {
            List<ReflexYield> ret = new List<ReflexYield>();
            
            for (int n = 0; n < potSystems.Count; n++)
            {
                MathNet.Numerics.LinearAlgebra.Matrix<double> selectedSystemNomStrain = this.GetNominalStrain(potSystems[n]);

                bool repl = false;

                if(n != 0)
                {
                    int limit = ret.Count;
                    for (int i = 0; i < limit; i++)
                    {
                        MathNet.Numerics.LinearAlgebra.Matrix<double> comNomStrain = this.GetNominalStrain(ret[i]);

                        MathNet.Numerics.LinearAlgebra.Matrix<double> diff = selectedSystemNomStrain - comNomStrain;
                        double diffNorm = Math.Sqrt(Math.Pow(diff[0, 0], 2) + Math.Pow(diff[1, 0], 2) + Math.Pow(diff[2, 0], 2) + Math.Pow(diff[1, 1], 2) + Math.Pow(diff[1, 2], 2) + Math.Pow(diff[2, 2], 2));

                        diffNorm /= 6.0;

                        if(diffNorm < 0.00001)
                        {
                            repl = true;
                            break;
                        }
                        //else
                        //{
                        //    ret[i].MainSlipDirection.H *= -1;
                        //    ret[i].MainSlipDirection.K *= -1;
                        //    ret[i].MainSlipDirection.L *= -1;

                        //    comNomStrain = this.GetNominalStrain(ret[i]);
                        //    diff = selectedSystemNomStrain - comNomStrain;
                        //    diffNorm = Math.Sqrt(Math.Pow(diff[0, 0], 2) + Math.Pow(diff[1, 0], 2) + Math.Pow(diff[2, 0], 2) + Math.Pow(diff[1, 1], 2) + Math.Pow(diff[1, 2], 2) + Math.Pow(diff[2, 2], 2));
                        //    diffNorm /= 6.0;
                        //    if (diffNorm < 0.00001)
                        //    {
                        //        repl = true;
                        //        ret[i].MainSlipDirection.H *= -1;
                        //        ret[i].MainSlipDirection.K *= -1;
                        //        ret[i].MainSlipDirection.L *= -1;
                        //        break;
                        //    }

                        //    ret[i].MainSlipDirection.H *= -1;
                        //    ret[i].MainSlipDirection.K *= -1;
                        //    ret[i].MainSlipDirection.L *= -1;
                        //}
                    }
                }

                if(!repl)
                {
                    ret.Add(potSystems[n]);
                }
            }
            
            if(ret.Count > 5)
            {
                List<ReflexYield> retFinal = new List<ReflexYield>();
                int index = 5;

                for(int n = 0; n < ret.Count; n++)
                {
                    //Die Auswahl der 5 größten Gleitsysteme wird über ein rankingsystem durchgeführt. 
                    //Die Reihenfolge der Gleitsysteme bleibt dadurch unverändert
                    //Jedes Gleitsystem hat am Anfang den höchsten Rang
                    //Positive Scherraten werden bevorzugt behandelt
                    //Danach kommtes nur noch darauf an welcher der größte Wert ist
                    int rank = ret.Count;
                    double shearStressN = this.ShearStress(ret[n], aStress);
                    if (shearStressN >= 0)
                    {
                        for (int i = 0; i < ret.Count; i++)
                        {
                            double shearStressI = this.ShearStress(ret[i], aStress);

                            //Ein Gleitsystem steigt im Rang auf sobald der shearstress größer oder gleich der anderen übrigen ist.
                            if (shearStressN >= shearStressI)
                            {
                                rank--;
                            }

                            //Wird der Rang klein genug wird das Gleitsystem der neuen Rückgabe wert übergeben.
                            if (rank < 5)
                            {
                                retFinal.Add(ret[n]);
                                index--;
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < ret.Count; i++)
                        {
                            double shearStressI = this.ShearStress(ret[i], aStress);

                            //Ein Gleitsystem steigt im Rang auf sobald der shearstress größer oder gleich der anderen übrigen ist.
                            if (shearStressI <= 0)
                            {
                                if (shearStressN <= shearStressI)
                                {
                                    rank--;
                                }
                            }

                            //Wird der Rang klein genug wird das Gleitsystem der neuen Rückgabe wert übergeben.
                            if (rank < 5)
                            {
                                retFinal.Add(ret[n]);
                                index--;
                                break;
                            }
                        }
                    }

                    if(index == 0)
                    {
                        break;
                    }
                }

                return retFinal;
            }
            else
            {
                return ret;
            }
        }

        public void CheckDependencies(List<ReflexYield> potSystems, MathNet.Numerics.LinearAlgebra.Matrix<double> aStress, List<int> checkedSystems)
        {
            List<int> activeIndex = new List<int>();

            for (int n = 0; n < potSystems.Count; n++)
            {
                MathNet.Numerics.LinearAlgebra.Matrix<double> selectedSystemNomStrain = this.GetNominalStrain(potSystems[n]);

                bool repl = false;
                if (n != 0)
                {
                    int limit = activeIndex.Count;
                    for (int i = 0; i < limit; i++)
                    {
                        MathNet.Numerics.LinearAlgebra.Matrix<double> comNomStrain = this.GetNominalStrain(potSystems[activeIndex[i]]);

                        MathNet.Numerics.LinearAlgebra.Matrix<double> diff = selectedSystemNomStrain - comNomStrain;
                        double diffNorm = Math.Sqrt(Math.Pow(diff[0, 0], 2) + Math.Pow(diff[1, 0], 2) + Math.Pow(diff[2, 0], 2) + Math.Pow(diff[1, 1], 2) + Math.Pow(diff[1, 2], 2) + Math.Pow(diff[2, 2], 2));

                        diffNorm /= 6.0;

                        if (diffNorm < 0.00001)
                        {
                            repl = true;
                            break;
                        }

                        #region direction check
                        //else
                        //{
                        //    ret[i].MainSlipDirection.H *= -1;
                        //    ret[i].MainSlipDirection.K *= -1;
                        //    ret[i].MainSlipDirection.L *= -1;

                        //    comNomStrain = this.GetNominalStrain(ret[i]);
                        //    diff = selectedSystemNomStrain - comNomStrain;
                        //    diffNorm = Math.Sqrt(Math.Pow(diff[0, 0], 2) + Math.Pow(diff[1, 0], 2) + Math.Pow(diff[2, 0], 2) + Math.Pow(diff[1, 1], 2) + Math.Pow(diff[1, 2], 2) + Math.Pow(diff[2, 2], 2));
                        //    diffNorm /= 6.0;
                        //    if (diffNorm < 0.00001)
                        //    {
                        //        repl = true;
                        //        ret[i].MainSlipDirection.H *= -1;
                        //        ret[i].MainSlipDirection.K *= -1;
                        //        ret[i].MainSlipDirection.L *= -1;
                        //        break;
                        //    }

                        //    ret[i].MainSlipDirection.H *= -1;
                        //    ret[i].MainSlipDirection.K *= -1;
                        //    ret[i].MainSlipDirection.L *= -1;
                        //}
                        #endregion
                    }
                }

                if (!repl)
                {
                    activeIndex.Add(n);
                    //ret.Add(potSystems[n]);
                }
            }
            if (this.CrystalData.SymmetryGroupID == 229)
            {
                bool fistFamilyactive = false;
                bool secondFamilyactive = false;

                for (int n = 0; n < activeIndex.Count; n++)
                {
                    int sum = Math.Abs(potSystems[activeIndex[n]].SlipPlane.H) + Math.Abs(potSystems[activeIndex[n]].SlipPlane.K) + Math.Abs(potSystems[activeIndex[n]].SlipPlane.L);
                    if (sum == 2)
                    {
                        fistFamilyactive = true;
                    }
                    if (sum == 4)
                    {
                        secondFamilyactive = true;
                    }

                    if (fistFamilyactive && secondFamilyactive)
                    {
                        List<int> bccSelection = new List<int>();
                        for (int i = 0; i < activeIndex.Count; i++)
                        {
                            int sum1 = Math.Abs(potSystems[activeIndex[i]].SlipPlane.H) + Math.Abs(potSystems[activeIndex[i]].SlipPlane.K) + Math.Abs(potSystems[activeIndex[i]].SlipPlane.L);

                            if (sum1 != 6)
                            {
                                bccSelection.Add(activeIndex[i]);
                            }
                        }

                        activeIndex = bccSelection;
                        break;
                    }
                }
            }

            if (activeIndex.Count > 5)
            {
                int index = 5;

                for (int n = 0; n < activeIndex.Count; n++)
                {
                    //Die Auswahl der 5 größten Gleitsysteme wird über ein rankingsystem durchgeführt. 
                    //Die Reihenfolge der Gleitsysteme bleibt dadurch unverändert
                    //Jedes Gleitsystem hat am Anfang den höchsten Rang
                    //Positive Scherraten werden bevorzugt behandelt
                    //Danach kommtes nur noch darauf an welcher der größte Wert ist
                    int rank = activeIndex.Count;
                    double shearStressN = this.ShearStress(potSystems[activeIndex[n]], aStress);
                    if (shearStressN >= 0)
                    {
                        for (int i = 0; i < activeIndex.Count; i++)
                        {
                            double shearStressI = this.ShearStress(potSystems[activeIndex[i]], aStress);

                            //Ein Gleitsystem steigt im Rang auf sobald der shearstress größer oder gleich der anderen übrigen ist.
                            if (shearStressN >= shearStressI)
                            {
                                rank--;
                            }

                            //Wird der Rang klein genug wird das Gleitsystem der neuen Rückgabe wert übergeben.
                            if (rank < 5)
                            {
                                checkedSystems[activeIndex[n]] = 1;
                                index--;
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < activeIndex.Count; i++)
                        {
                            double shearStressI = this.ShearStress(potSystems[activeIndex[i]], aStress);

                            //Ein Gleitsystem steigt im Rang auf sobald der shearstress größer oder gleich der anderen übrigen ist.
                            if (shearStressI <= 0)
                            {
                                if (shearStressN <= shearStressI)
                                {
                                    rank--;
                                }
                            }

                            //Wird der Rang klein genug wird das Gleitsystem der neuen Rückgabe wert übergeben.
                            if (rank < 5)
                            {
                                checkedSystems[activeIndex[n]] = 1;
                                index--;
                                break;
                            }
                        }
                    }

                    if (index == 0)
                    {
                        break;
                    }
                }
            }
            else
            {
                for(int n = 0; n < activeIndex.Count; n++)
                {
                    checkedSystems[activeIndex[n]] = 1;
                }
            }
        }

        public MathNet.Numerics.LinearAlgebra.Matrix<double> GetNominalStrain(ReflexYield potSystem)
        {
            MathNet.Numerics.LinearAlgebra.Matrix<double> ret = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

            double normDirection = Math.Sqrt(Math.Pow(potSystem.MainSlipDirection.H, 2) + Math.Pow(potSystem.MainSlipDirection.K, 2) + Math.Pow(potSystem.MainSlipDirection.L, 2));
            double normPlane = Math.Sqrt(Math.Pow(potSystem.SlipPlane.H, 2) + Math.Pow(potSystem.SlipPlane.K, 2) + Math.Pow(potSystem.SlipPlane.L, 2));

            ret[0, 0] = (potSystem.SlipPlane.H / normPlane) * (potSystem.MainSlipDirection.H / normDirection);
            ret[0, 1] = 0.5 * (((potSystem.SlipPlane.H / normPlane) * (potSystem.MainSlipDirection.K / normDirection)) + ((potSystem.SlipPlane.K / normPlane) * (potSystem.MainSlipDirection.H / normDirection)));
            ret[0, 2] = 0.5 * (((potSystem.SlipPlane.H / normPlane) * (potSystem.MainSlipDirection.L / normDirection)) + ((potSystem.SlipPlane.L / normPlane) * (potSystem.MainSlipDirection.H / normDirection)));

            ret[1, 0] = 0.5 * (((potSystem.SlipPlane.H / normPlane) * (potSystem.MainSlipDirection.K / normDirection)) + ((potSystem.SlipPlane.K / normPlane) * (potSystem.MainSlipDirection.H / normDirection)));
            ret[1, 1] = (potSystem.SlipPlane.K / normPlane) * (potSystem.MainSlipDirection.K / normDirection);
            ret[1, 2] = 0.5 * (((potSystem.SlipPlane.K / normPlane) * (potSystem.MainSlipDirection.L / normDirection)) + ((potSystem.SlipPlane.L / normPlane) * (potSystem.MainSlipDirection.K / normDirection)));

            ret[2, 0] = 0.5 * (((potSystem.SlipPlane.H / normPlane) * (potSystem.MainSlipDirection.L / normDirection)) + ((potSystem.SlipPlane.L / normPlane) * (potSystem.MainSlipDirection.H / normDirection)));
            ret[2, 1] = 0.5 * (((potSystem.SlipPlane.K / normPlane) * (potSystem.MainSlipDirection.L / normDirection)) + ((potSystem.SlipPlane.L / normPlane) * (potSystem.MainSlipDirection.K / normDirection)));
            ret[2, 2] = (potSystem.SlipPlane.L / normPlane) * (potSystem.MainSlipDirection.L / normDirection);

            return ret;
        }

        public int GetSmallesSchearStressIndex(List<ReflexYield> reflexList, MathNet.Numerics.LinearAlgebra.Matrix<double> aStress)
        {
            double sValue = 5000;
            int ret = 0;

            for(int n = 0; n < reflexList.Count; n++)
            {
                if (sValue > Math.Abs(ShearStress(reflexList[n], aStress)))
                {
                    ret = n;
                    sValue = Math.Abs(ShearStress(reflexList[n], aStress));
                }
            }

            return ret;
        }


        public List<ReflexYield> GetPotentiallyActiveSlipSystems(MathNet.Numerics.LinearAlgebra.Matrix<double> aStress)
        {
            List<ReflexYield> Ret = new List<ReflexYield>();

            for (int n = 0; n < this.PotentialSlipSystems.Count; n++)
            {
                if (this.PotentialSlipSystems[n].ActiveSystem == 1 && this.CheckStandard(this.PotentialSlipSystems[n], aStress))
                {
                    Ret.Add(this.PotentialSlipSystems[n]);
                }
            }

            return Ret;
        }

        public List<ReflexYield> GetSlipSystemsFamily(ReflexYield family)
        {
            List<ReflexYield> familySystems = new List<ReflexYield>();

            if(CrystalData.SymmetryGroupID == 225)
            {
                familySystems = PotentialSlipSystems;
            }
            else if (CrystalData.SymmetryGroupID == 229)
            {
                int indexCount = Convert.ToInt32(Math.Abs(family.SlipPlane.H)) + Convert.ToInt32(Math.Abs(family.SlipPlane.K)) + Convert.ToInt32(Math.Abs(family.SlipPlane.L));

                for (int n = 0; n < this.PotentialSlipSystems.Count; n++)
                {
                    int familyIndexCount = Convert.ToInt32(Math.Abs(this.PotentialSlipSystems[n].SlipPlane.H)) + Convert.ToInt32(Math.Abs(this.PotentialSlipSystems[n].SlipPlane.K)) + Convert.ToInt32(Math.Abs(this.PotentialSlipSystems[n].SlipPlane.L));
                    if(indexCount == familyIndexCount)
                    {
                        familySystems.Add(this.PotentialSlipSystems[n]);
                    }
                }
            }

            return familySystems;
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
        
        public MathNet.Numerics.LinearAlgebra.Matrix<double> HardeningMatrixSlipSystem(List<ReflexYield> potActiveSystems, MathNet.Numerics.LinearAlgebra.Matrix<double> hardeningMatrix)
        {
            MathNet.Numerics.LinearAlgebra.Matrix<double> ret = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(potActiveSystems.Count, potActiveSystems.Count, 0.0);
            for (int n = 0; n < potActiveSystems.Count; n++)
            {
                for (int i = 0; i < potActiveSystems.Count; i++)
                {
                    MathNet.Numerics.LinearAlgebra.Vector<double> directionHardeningN = hardeningMatrix * potActiveSystems[n].MainSlipDirection.DirectionNorm;
                    ret[n, i] = potActiveSystems[i].MainSlipDirection.DirectionNorm * directionHardeningN;
                }
            }

            return ret;
        }

        public MathNet.Numerics.LinearAlgebra.Matrix<double> HardeningMatrixSlipSystem(List<ReflexYield> potActiveSystems)
        {
            MathNet.Numerics.LinearAlgebra.Matrix<double> ret = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(potActiveSystems.Count, potActiveSystems.Count, 0.0);
            for (int n = 0; n < potActiveSystems.Count; n++)
            {
                ret[n, n] = potActiveSystems[n].YieldHardenning;
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
                    //MathNet.Numerics.LinearAlgebra.Vector<double> directionHardeningN = hardeningMatrix * potActiveSystems[n].MainSlipDirection.DirectionNorm;
                    ret[n, i] = hardeningMatrix[n, i];

                    MathNet.Numerics.LinearAlgebra.Matrix<double> resN = Tools.Calculation.GetResolvingParameter(potActiveSystems[n].SlipPlane, potActiveSystems[n].MainSlipDirection);
                    MathNet.Numerics.LinearAlgebra.Matrix<double> resI = Tools.Calculation.GetResolvingParameter(potActiveSystems[i].SlipPlane, potActiveSystems[i].MainSlipDirection);

                    //DiffractionOrientation.OrientationMatrix oMN = new DiffractionOrientation.OrientationMatrix();
                    //DiffractionOrientation.OrientationMatrix oMI = new DiffractionOrientation.OrientationMatrix();

                    //if (this.CrystalData.SymmetryGroupID == 194)
                    //{
                    //    oMN.AsMainAxisTransformation(potActiveSystems[n].MainSlipDirection.H, potActiveSystems[n].MainSlipDirection.K, potActiveSystems[n].MainSlipDirection.L, 1);
                    //}
                    //else
                    //{
                    //    oMN.AsMainAxisTransformation(potActiveSystems[n].MainSlipDirection.H, potActiveSystems[n].MainSlipDirection.K, potActiveSystems[n].MainSlipDirection.L, 0);
                    //}
                    //if (this.CrystalData.SymmetryGroupID == 194)
                    //{
                    //    oMI.AsMainAxisTransformation(potActiveSystems[i].MainSlipDirection.H, potActiveSystems[i].MainSlipDirection.K, potActiveSystems[i].MainSlipDirection.L, 1);
                    //}
                    //else
                    //{
                    //    oMI.AsMainAxisTransformation(potActiveSystems[i].MainSlipDirection.H, potActiveSystems[i].MainSlipDirection.K, potActiveSystems[i].MainSlipDirection.L, 0);
                    //}

                    Tools.FourthRankTensor stiffnesses = eT.GetFourtRankStiffnesses();
                    double hV = stiffnesses.InnerTransormation(resN, resI);

                    ret[n, i] += hV;
                }
            }

            return ret;
        }
        
        public MathNet.Numerics.LinearAlgebra.Matrix<double> GetInstStiffnessFactors(List<ReflexYield> potActiveSystems, Microsopic.ElasticityTensors eT, int slipsSystemIndex, MathNet.Numerics.LinearAlgebra.Matrix<double> slipSystemY)
        {
            MathNet.Numerics.LinearAlgebra.Matrix<double> ret = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

            Tools.FourthRankTensor stiffnesses = eT.GetFourtRankStiffnesses();
            
            for (int n = 0; n < potActiveSystems.Count; n++)
            {
                MathNet.Numerics.LinearAlgebra.Matrix<double> alpha = potActiveSystems[n].GetResolvingParameterMain();

                ret += slipSystemY[slipsSystemIndex, n] * (stiffnesses * alpha);
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
        
        //public MathNet.Numerics.LinearAlgebra.Vector<double> GetShearRate(List<ReflexYield> potActiveSystems, List<MathNet.Numerics.LinearAlgebra.Matrix<double>> iCFList, MathNet.Numerics.LinearAlgebra.Matrix<double> grainStrain)
        //{
        //    MathNet.Numerics.LinearAlgebra.Vector<double> ret = MathNet.Numerics.LinearAlgebra.CreateVector.Dense(potActiveSystems.Count, 0.0);

        //    for (int n = 0; n < 3; n++)
        //    {
        //        for (int i = 0; i < 3; i++)
        //        {
        //            ret += iCF[n, i] * grainStrain[n, i];
        //        }
        //    }

        //    return ret;
        //}

        public void SetHardenning(List<ReflexYield> potActiveSystems, MathNet.Numerics.LinearAlgebra.Matrix<double> hardeningMatrix)
        {
            MathNet.Numerics.LinearAlgebra.Vector<double> systemVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense(potActiveSystems.Count, 1.0);

            MathNet.Numerics.LinearAlgebra.Vector<double> ret = hardeningMatrix * systemVector;
            
            for( int n = 0; n < potActiveSystems.Count; n++)
            {
                potActiveSystems[n].YieldMainHardennedStrength += ret[n];
            }
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
