using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.Analysis
{
    public class Sample : ICloneable
    {
        private string _name;
        public string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                this._name = value;
            }
        }

        private double _area;
        public double Area
        {
            get
            {
                return this._area;
            }
            set
            {
                this._area = value;
            }
        }

        private int _actualPatternId;
        public int ActualPatterId
        {
            get
            {
                this._actualPatternId++;
                return this._actualPatternId;
            }
        }

        public List<DataManagment.CrystalData.CODData> CrystalData = new List<DataManagment.CrystalData.CODData>();

        public List<Pattern.DiffractionPattern> DiffractionPatterns = new List<Pattern.DiffractionPattern>();

        #region Macro elastic calculations

        public List<Stress.Macroskopic.Elasticity> MacroElasticData = new List<Stress.Macroskopic.Elasticity>();

        public List<Tools.BulkElasticPhaseEvaluations> AveragedEModulStandard()
        {
            List<Tools.BulkElasticPhaseEvaluations> Ret = new List<Tools.BulkElasticPhaseEvaluations>();

            for (int i = 0; i < this.CrystalData.Count; i++)
            {
                Tools.BulkElasticPhaseEvaluations Tmp = new Tools.BulkElasticPhaseEvaluations();
                Tmp.HKLPase = this.CrystalData[i].SymmetryGroup;

                int count = 0;
                double ret = 0.0;

                for (int n = 0; n < MacroElasticData.Count; n++)
                {
                    if (MacroElasticData[n][0].DPeak.AssociatedCrystalData.SymmetryGroup == this.CrystalData[i].SymmetryGroup)
                    {
                        if (MacroElasticData[n][0].PsiAngle == 0)
                        {
                            count++;
                            ret += MacroElasticData[n].EModul;
                        }
                    }
                }
                if (count == 0)
                {
                    count = 1;
                }
            
                Tmp.PsiAngle = 0;
                Tmp.BulkElasticity = ret / count;

                ret = 0.0;
                for (int n = 0; n < MacroElasticData.Count; n++)
                {
                    if (MacroElasticData[n][0].DPeak.AssociatedCrystalData.SymmetryGroup == this.CrystalData[i].SymmetryGroup)
                    {
                        if (MacroElasticData[n][0].PsiAngle == 0)
                        {
                            ret += Math.Pow(Tmp.BulkElasticity - MacroElasticData[n].EModul, 2);

                        }
                    }
                }

                Tmp.BulkElasticityError = Math.Sqrt(ret / MacroElasticData.Count);

                Ret.Add(Tmp);
            }

            return Ret;
        }

        public List<Tools.BulkElasticPhaseEvaluations> AveragedPossionNumberStandard()
        {
            List<Tools.BulkElasticPhaseEvaluations> Ret = new List<Tools.BulkElasticPhaseEvaluations>();

            for (int i = 0; i < this.CrystalData.Count; i++)
            {
                Tools.BulkElasticPhaseEvaluations Tmp = new Tools.BulkElasticPhaseEvaluations();
                Tmp.HKLPase = this.CrystalData[i].SymmetryGroup;

                int count = 0;
                double ret = 0.0;

                for (int n = 0; n < MacroElasticData.Count; n++)
                {
                    if (MacroElasticData[n][0].DPeak.AssociatedCrystalData.SymmetryGroup == this.CrystalData[i].SymmetryGroup)
                    {
                        if (MacroElasticData[n][0].PsiAngle == 90)
                        {
                            count++;
                            ret += MacroElasticData[n].EModul;
                        }
                    }
                }
                if (count == 0)
                {
                    count = 1;
                }

                Tmp.PsiAngle = 90;
                Tmp.BulkElasticity = ret / count;

                ret = 0.0;

                for (int n = 0; n < MacroElasticData.Count; n++)
                {
                    if (MacroElasticData[n][0].DPeak.AssociatedCrystalData.SymmetryGroup == this.CrystalData[i].SymmetryGroup)
                    {
                        if (MacroElasticData[n][0].PsiAngle == 90)
                        {
                            ret += Math.Pow(Tmp.BulkElasticity - MacroElasticData[n].EModul, 2);

                        }
                    }
                }

                Tmp.BulkElasticityError = Math.Sqrt(ret / MacroElasticData.Count);

                Ret.Add(Tmp);
            }

            return Ret;
        }

        public List<Tools.BulkElasticPhaseEvaluations> AveragedEModulWeighted()
        {
            List<Tools.BulkElasticPhaseEvaluations> Ret = new List<Tools.BulkElasticPhaseEvaluations>();

            for (int i = 0; i < this.CrystalData.Count; i++)
            {
                Tools.BulkElasticPhaseEvaluations Tmp = new Tools.BulkElasticPhaseEvaluations();
                Tmp.HKLPase = this.CrystalData[i].SymmetryGroup;

                double TotalWeight = 0;
                double ret = 0.0;

                for (int n = 0; n < MacroElasticData.Count; n++)
                {
                    if (MacroElasticData[n][0].DPeak.AssociatedCrystalData.SymmetryGroup == this.CrystalData[i].SymmetryGroup)
                    {
                        if (MacroElasticData[n][0].PsiAngle == 0)
                        {
                            TotalWeight += MacroElasticData[n].EModulError;
                            ret += MacroElasticData[n].EModul / MacroElasticData[n].EModulError;
                        }
                    }
                }

                Tmp.PsiAngle = 0;
                Tmp.BulkElasticity = ret / TotalWeight;
                Tmp.BulkElasticityError = 0.0;

                Ret.Add(Tmp);
            }

            return Ret;
        }

        public List<Tools.BulkElasticPhaseEvaluations> AveragedPossionNumberWeighted()
        {
            List<Tools.BulkElasticPhaseEvaluations> Ret = new List<Tools.BulkElasticPhaseEvaluations>();

            for (int i = 0; i < this.CrystalData.Count; i++)
            {
                Tools.BulkElasticPhaseEvaluations Tmp = new Tools.BulkElasticPhaseEvaluations();
                Tmp.HKLPase = this.CrystalData[i].SymmetryGroup;

                double TotalWeight = 0;
                double ret = 0.0;

                for (int n = 0; n < MacroElasticData.Count; n++)
                {
                    if (MacroElasticData[n][0].DPeak.AssociatedCrystalData.SymmetryGroup == this.CrystalData[i].SymmetryGroup)
                    {
                        if (MacroElasticData[n][0].PsiAngle == 90)
                        {
                            TotalWeight += MacroElasticData[n].EModulError;
                            ret += MacroElasticData[n].EModul / MacroElasticData[n].EModulError;
                        }
                    }
                }

                Tmp.PsiAngle = 90;
                Tmp.BulkElasticity = ret / TotalWeight;
                Tmp.BulkElasticityError = 0.0;

                Ret.Add(Tmp);
            }

            return Ret;
        }

        public List<Analysis.Stress.Macroskopic.PeakStressAssociation> GetSinPsyData()
        {
            List<Analysis.Stress.Macroskopic.PeakStressAssociation> ret = new List<Stress.Macroskopic.PeakStressAssociation>();

            for(int n = 0; n < this.DiffractionPatterns.Count; n++)
            {
                for(int i = 0; i < this.DiffractionPatterns[n].FoundPeaks.Count; i++)
                {
                    ret.Add(new Stress.Macroskopic.PeakStressAssociation(this.DiffractionPatterns[n].Stress, this.DiffractionPatterns[n].PsiAngle, this.DiffractionPatterns[n].FoundPeaks[i]));
                }
            }

            return ret;
        }

        #endregion

        #region Micro elastic calculation

        public List<List<Stress.Microsopic.REK>> DiffractionConstants = new List<List<Stress.Microsopic.REK>>();

        public List<Stress.Microsopic.ElasticityTensors> VoigtTensorData = new List<Stress.Microsopic.ElasticityTensors>();
        public List<Stress.Microsopic.ElasticityTensors> ReussTensorData = new List<Stress.Microsopic.ElasticityTensors>();
        public List<Stress.Microsopic.ElasticityTensors> HillTensorData = new List<Stress.Microsopic.ElasticityTensors>();

        public void SetHillTensorData(int phase)
        {
            if (VoigtTensorData[phase].Symmetry == ReussTensorData[phase].Symmetry)
            {
                this.HillTensorData[phase] = new Stress.Microsopic.ElasticityTensors();
                this.HillTensorData[phase].Symmetry = VoigtTensorData[phase].Symmetry;
                this.HillTensorData[phase].IsIsotropic = VoigtTensorData[phase].IsIsotropic;
                this.HillTensorData[phase].FitConverged = VoigtTensorData[phase].FitConverged;

                switch (this.VoigtTensorData[phase].Symmetry)
                {
                    case "cubic":
                        if (VoigtTensorData[phase].IsIsotropic)
                        {
                            this.HillTensorData[phase].C11 = (this.VoigtTensorData[phase].C11 + this.ReussTensorData[phase].C11) / 2;
                            this.HillTensorData[phase].C12 = (this.VoigtTensorData[phase].C12 + this.ReussTensorData[phase].C12) / 2;
                        }
                        else
                        {
                            this.HillTensorData[phase].C11 = (this.VoigtTensorData[phase].C11 + this.ReussTensorData[phase].C11) / 2;
                            this.HillTensorData[phase].C12 = (this.VoigtTensorData[phase].C12 + this.ReussTensorData[phase].C12) / 2;
                            this.HillTensorData[phase].C44 = (this.VoigtTensorData[phase].C44 + this.ReussTensorData[phase].C44) / 2;
                        }
                        break;
                    case "hexagonal":
                        this.HillTensorData[phase].C11 = (this.VoigtTensorData[phase].C11 + this.ReussTensorData[phase].C11) / 2;
                        this.HillTensorData[phase].C33 = (this.VoigtTensorData[phase].C33 + this.ReussTensorData[phase].C33) / 2;
                        this.HillTensorData[phase].C12 = (this.VoigtTensorData[phase].C12 + this.ReussTensorData[phase].C12) / 2;
                        this.HillTensorData[phase].C13 = (this.VoigtTensorData[phase].C13 + this.ReussTensorData[phase].C13) / 2;
                        this.HillTensorData[phase].C44 = (this.VoigtTensorData[phase].C44 + this.ReussTensorData[phase].C44) / 2;
                        break;
                    case "tetragonal type 1":
                        this.HillTensorData[phase].C11 = (this.VoigtTensorData[phase].C11 + this.ReussTensorData[phase].C11) / 2;
                        this.HillTensorData[phase].C33 = (this.VoigtTensorData[phase].C33 + this.ReussTensorData[phase].C33) / 2;
                        this.HillTensorData[phase].C12 = (this.VoigtTensorData[phase].C12 + this.ReussTensorData[phase].C12) / 2;
                        this.HillTensorData[phase].C13 = (this.VoigtTensorData[phase].C13 + this.ReussTensorData[phase].C13) / 2;
                        this.HillTensorData[phase].C44 = (this.VoigtTensorData[phase].C44 + this.ReussTensorData[phase].C44) / 2;
                        this.HillTensorData[phase].C66 = (this.VoigtTensorData[phase].C66 + this.ReussTensorData[phase].C66) / 2;
                        break;
                    case "tetragonal type 2":
                        this.HillTensorData[phase].C11 = (this.VoigtTensorData[phase].C11 + this.ReussTensorData[phase].C11) / 2;
                        this.HillTensorData[phase].C33 = (this.VoigtTensorData[phase].C33 + this.ReussTensorData[phase].C33) / 2;
                        this.HillTensorData[phase].C12 = (this.VoigtTensorData[phase].C12 + this.ReussTensorData[phase].C12) / 2;
                        this.HillTensorData[phase].C13 = (this.VoigtTensorData[phase].C13 + this.ReussTensorData[phase].C13) / 2;
                        this.HillTensorData[phase].C16 = (this.VoigtTensorData[phase].C16 + this.ReussTensorData[phase].C16) / 2;
                        this.HillTensorData[phase].C44 = (this.VoigtTensorData[phase].C44 + this.ReussTensorData[phase].C44) / 2;
                        this.HillTensorData[phase].C66 = (this.VoigtTensorData[phase].C66 + this.ReussTensorData[phase].C66) / 2;
                        break;
                    case "trigonal type 1":
                        this.HillTensorData[phase].C11 = (this.VoigtTensorData[phase].C11 + this.ReussTensorData[phase].C11) / 2;
                        this.HillTensorData[phase].C33 = (this.VoigtTensorData[phase].C33 + this.ReussTensorData[phase].C33) / 2;
                        this.HillTensorData[phase].C12 = (this.VoigtTensorData[phase].C12 + this.ReussTensorData[phase].C12) / 2;
                        this.HillTensorData[phase].C13 = (this.VoigtTensorData[phase].C13 + this.ReussTensorData[phase].C13) / 2;
                        this.HillTensorData[phase].C14 = (this.VoigtTensorData[phase].C14 + this.ReussTensorData[phase].C14) / 2;
                        this.HillTensorData[phase].C44 = (this.VoigtTensorData[phase].C44 + this.ReussTensorData[phase].C44) / 2;
                        break;
                    case "trigonal type 2":
                        this.HillTensorData[phase].C11 = (this.VoigtTensorData[phase].C11 + this.ReussTensorData[phase].C11) / 2;
                        this.HillTensorData[phase].C33 = (this.VoigtTensorData[phase].C33 + this.ReussTensorData[phase].C33) / 2;
                        this.HillTensorData[phase].C12 = (this.VoigtTensorData[phase].C12 + this.ReussTensorData[phase].C12) / 2;
                        this.HillTensorData[phase].C13 = (this.VoigtTensorData[phase].C13 + this.ReussTensorData[phase].C13) / 2;
                        this.HillTensorData[phase].C14 = (this.VoigtTensorData[phase].C14 + this.ReussTensorData[phase].C14) / 2;
                        this.HillTensorData[phase].C15 = (this.VoigtTensorData[phase].C15 + this.ReussTensorData[phase].C15) / 2;
                        this.HillTensorData[phase].C44 = (this.VoigtTensorData[phase].C44 + this.ReussTensorData[phase].C44) / 2;
                        break;
                    case "rhombic":
                        this.HillTensorData[phase].C11 = (this.VoigtTensorData[phase].C11 + this.ReussTensorData[phase].C11) / 2;
                        this.HillTensorData[phase].C22 = (this.VoigtTensorData[phase].C22 + this.ReussTensorData[phase].C22) / 2;
                        this.HillTensorData[phase].C33 = (this.VoigtTensorData[phase].C33 + this.ReussTensorData[phase].C33) / 2;
                        this.HillTensorData[phase].C12 = (this.VoigtTensorData[phase].C12 + this.ReussTensorData[phase].C12) / 2;
                        this.HillTensorData[phase].C13 = (this.VoigtTensorData[phase].C13 + this.ReussTensorData[phase].C13) / 2;
                        this.HillTensorData[phase].C23 = (this.VoigtTensorData[phase].C23 + this.ReussTensorData[phase].C23) / 2;
                        this.HillTensorData[phase].C44 = (this.VoigtTensorData[phase].C44 + this.ReussTensorData[phase].C44) / 2;
                        this.HillTensorData[phase].C55 = (this.VoigtTensorData[phase].C55 + this.ReussTensorData[phase].C55) / 2;
                        this.HillTensorData[phase].C66 = (this.VoigtTensorData[phase].C66 + this.ReussTensorData[phase].C66) / 2;
                        break;
                    case "monoclinic":
                        this.HillTensorData[phase].C11 = (this.VoigtTensorData[phase].C11 + this.ReussTensorData[phase].C11) / 2;
                        this.HillTensorData[phase].C22 = (this.VoigtTensorData[phase].C22 + this.ReussTensorData[phase].C22) / 2;
                        this.HillTensorData[phase].C33 = (this.VoigtTensorData[phase].C33 + this.ReussTensorData[phase].C33) / 2;
                        this.HillTensorData[phase].C12 = (this.VoigtTensorData[phase].C12 + this.ReussTensorData[phase].C12) / 2;
                        this.HillTensorData[phase].C13 = (this.VoigtTensorData[phase].C13 + this.ReussTensorData[phase].C13) / 2;
                        this.HillTensorData[phase].C16 = (this.VoigtTensorData[phase].C16 + this.ReussTensorData[phase].C16) / 2;
                        this.HillTensorData[phase].C23 = (this.VoigtTensorData[phase].C23 + this.ReussTensorData[phase].C23) / 2;
                        this.HillTensorData[phase].C26 = (this.VoigtTensorData[phase].C26 + this.ReussTensorData[phase].C26) / 2;
                        this.HillTensorData[phase].C36 = (this.VoigtTensorData[phase].C36 + this.ReussTensorData[phase].C36) / 2;
                        this.HillTensorData[phase].C44 = (this.VoigtTensorData[phase].C44 + this.ReussTensorData[phase].C44) / 2;
                        this.HillTensorData[phase].C45 = (this.VoigtTensorData[phase].C45 + this.ReussTensorData[phase].C45) / 2;
                        this.HillTensorData[phase].C55 = (this.VoigtTensorData[phase].C55 + this.ReussTensorData[phase].C55) / 2;
                        this.HillTensorData[phase].C66 = (this.VoigtTensorData[phase].C66 + this.ReussTensorData[phase].C66) / 2;
                        break;
                    case "triclinic":
                        this.HillTensorData[phase].C11 = (this.VoigtTensorData[phase].C11 + this.ReussTensorData[phase].C11) / 2;
                        this.HillTensorData[phase].C12 = (this.VoigtTensorData[phase].C12 + this.ReussTensorData[phase].C12) / 2;
                        this.HillTensorData[phase].C13 = (this.VoigtTensorData[phase].C13 + this.ReussTensorData[phase].C13) / 2;
                        this.HillTensorData[phase].C14 = (this.VoigtTensorData[phase].C14 + this.ReussTensorData[phase].C14) / 2;
                        this.HillTensorData[phase].C15 = (this.VoigtTensorData[phase].C15 + this.ReussTensorData[phase].C15) / 2;
                        this.HillTensorData[phase].C16 = (this.VoigtTensorData[phase].C16 + this.ReussTensorData[phase].C16) / 2;
                        this.HillTensorData[phase].C22 = (this.VoigtTensorData[phase].C22 + this.ReussTensorData[phase].C22) / 2;
                        this.HillTensorData[phase].C23 = (this.VoigtTensorData[phase].C23 + this.ReussTensorData[phase].C23) / 2;
                        this.HillTensorData[phase].C24 = (this.VoigtTensorData[phase].C24 + this.ReussTensorData[phase].C24) / 2;
                        this.HillTensorData[phase].C25 = (this.VoigtTensorData[phase].C25 + this.ReussTensorData[phase].C25) / 2;
                        this.HillTensorData[phase].C26 = (this.VoigtTensorData[phase].C26 + this.ReussTensorData[phase].C26) / 2;
                        this.HillTensorData[phase].C33 = (this.VoigtTensorData[phase].C33 + this.ReussTensorData[phase].C33) / 2;
                        this.HillTensorData[phase].C34 = (this.VoigtTensorData[phase].C34 + this.ReussTensorData[phase].C34) / 2;
                        this.HillTensorData[phase].C35 = (this.VoigtTensorData[phase].C35 + this.ReussTensorData[phase].C35) / 2;
                        this.HillTensorData[phase].C36 = (this.VoigtTensorData[phase].C36 + this.ReussTensorData[phase].C36) / 2;
                        this.HillTensorData[phase].C44 = (this.VoigtTensorData[phase].C44 + this.ReussTensorData[phase].C44) / 2;
                        this.HillTensorData[phase].C45 = (this.VoigtTensorData[phase].C45 + this.ReussTensorData[phase].C45) / 2;
                        this.HillTensorData[phase].C46 = (this.VoigtTensorData[phase].C46 + this.ReussTensorData[phase].C46) / 2;
                        this.HillTensorData[phase].C55 = (this.VoigtTensorData[phase].C55 + this.ReussTensorData[phase].C55) / 2;
                        this.HillTensorData[phase].C56 = (this.VoigtTensorData[phase].C56 + this.ReussTensorData[phase].C56) / 2;
                        this.HillTensorData[phase].C66 = (this.VoigtTensorData[phase].C66 + this.ReussTensorData[phase].C66) / 2;
                        break;
                    default:
                        this.HillTensorData[phase].C11 = (this.VoigtTensorData[phase].C11 + this.ReussTensorData[phase].C11) / 2;
                        this.HillTensorData[phase].C12 = (this.VoigtTensorData[phase].C12 + this.ReussTensorData[phase].C12) / 2;
                        this.HillTensorData[phase].C13 = (this.VoigtTensorData[phase].C13 + this.ReussTensorData[phase].C13) / 2;
                        this.HillTensorData[phase].C14 = (this.VoigtTensorData[phase].C14 + this.ReussTensorData[phase].C14) / 2;
                        this.HillTensorData[phase].C15 = (this.VoigtTensorData[phase].C15 + this.ReussTensorData[phase].C15) / 2;
                        this.HillTensorData[phase].C16 = (this.VoigtTensorData[phase].C16 + this.ReussTensorData[phase].C16) / 2;
                        this.HillTensorData[phase].C22 = (this.VoigtTensorData[phase].C22 + this.ReussTensorData[phase].C22) / 2;
                        this.HillTensorData[phase].C23 = (this.VoigtTensorData[phase].C23 + this.ReussTensorData[phase].C23) / 2;
                        this.HillTensorData[phase].C24 = (this.VoigtTensorData[phase].C24 + this.ReussTensorData[phase].C24) / 2;
                        this.HillTensorData[phase].C25 = (this.VoigtTensorData[phase].C25 + this.ReussTensorData[phase].C25) / 2;
                        this.HillTensorData[phase].C26 = (this.VoigtTensorData[phase].C26 + this.ReussTensorData[phase].C26) / 2;
                        this.HillTensorData[phase].C33 = (this.VoigtTensorData[phase].C33 + this.ReussTensorData[phase].C33) / 2;
                        this.HillTensorData[phase].C34 = (this.VoigtTensorData[phase].C34 + this.ReussTensorData[phase].C34) / 2;
                        this.HillTensorData[phase].C35 = (this.VoigtTensorData[phase].C35 + this.ReussTensorData[phase].C35) / 2;
                        this.HillTensorData[phase].C36 = (this.VoigtTensorData[phase].C36 + this.ReussTensorData[phase].C36) / 2;
                        this.HillTensorData[phase].C44 = (this.VoigtTensorData[phase].C44 + this.ReussTensorData[phase].C44) / 2;
                        this.HillTensorData[phase].C45 = (this.VoigtTensorData[phase].C45 + this.ReussTensorData[phase].C45) / 2;
                        this.HillTensorData[phase].C46 = (this.VoigtTensorData[phase].C46 + this.ReussTensorData[phase].C46) / 2;
                        this.HillTensorData[phase].C55 = (this.VoigtTensorData[phase].C55 + this.ReussTensorData[phase].C55) / 2;
                        this.HillTensorData[phase].C56 = (this.VoigtTensorData[phase].C56 + this.ReussTensorData[phase].C56) / 2;
                        this.HillTensorData[phase].C66 = (this.VoigtTensorData[phase].C66 + this.ReussTensorData[phase].C66) / 2;
                        break;
                }

                this.HillTensorData[phase].CalculateCompliances();
            }
        }

        #endregion

        public Sample()
        {
            this._name = "Default";
            this._actualPatternId = 0;
        }

        public Sample(int PatternId)
        {
            this._name = "Default";
            this._actualPatternId = PatternId;
        }



        #region IClonable

        public object Clone()
        {
            Sample Ret = new Sample();

            Ret.Name = this.Name;
            Ret._area = this._area;

            foreach(Pattern.DiffractionPattern DP in this.DiffractionPatterns)
            {
                Ret.DiffractionPatterns.Add((Pattern.DiffractionPattern)DP.Clone());
            }
            foreach(DataManagment.CrystalData.CODData CD in this.CrystalData)
            {
                Ret.CrystalData.Add(new DataManagment.CrystalData.CODData(CD));
            }
            foreach (Stress.Macroskopic.Elasticity ME in this.MacroElasticData)
            {
                Ret.MacroElasticData.Add(ME.Clone() as Stress.Macroskopic.Elasticity);
            }

            return Ret;
        }

        #endregion
    }
}
