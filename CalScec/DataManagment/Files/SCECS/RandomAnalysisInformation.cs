using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.DataManagment.Files.SCECS
{
    [Serializable]
    public class RandomAnalysisInformation
    {
        #region Parameters

        public string _name = "";

        public List<SCEC.REKInformation> DiffractionConstants = new List<SCEC.REKInformation>();
        public DataManagment.CrystalData.CODData CrystalInfo;

        public SCEC.ElasticityTensorInformation InvestigatedTensor;

        public List<SCEC.ElasticityTensorInformation> TensorResults;

        /// <summary>
        /// [0]:C11
        /// [1]:C22
        /// [2]:C33
        /// [3]:C44
        /// [4]:C55
        /// [5]:C66
        /// [6]:C12
        /// [7]:C13
        /// [8]:C23
        /// [9]:C45
        /// [10]:C16
        /// [11]:C26
        /// [12]:C36
        /// </summary>
        public bool[] UsedConstants = { false, false, false, false, false, false, false, false, false, false, false, false, false };

        /// <summary>
        /// List with borders ind the same order as UsedConstants
        /// [0]: Lower border (Total value)
        /// [1]: Upper border (Total value)
        /// [2]: Stepwidth (equal distribution) or Total number of points (random distribution)
        /// </summary>
        public double[][] ConstantBorders;

        public bool ClassicREKCaluclation = false;

        #endregion

        public RandomAnalysisInformation(Analysis.MC.RandomAnalysis RA)
        {
            this._name = RA.Name;
            this.InvestigatedTensor = new SCEC.ElasticityTensorInformation(RA.InvestigatedTensor);
            this.UsedConstants = RA.UsedConstants;
            this.ConstantBorders = RA.ConstantBorders;
            this.ClassicREKCaluclation = RA.ClassicREKCaluclation;

            TensorResults = new List<SCEC.ElasticityTensorInformation>();
            for (int n = 0; n < RA.TensorResults.Count; n++)
            {
                TensorResults.Add(new SCEC.ElasticityTensorInformation(RA.TensorResults[n]));
            }

            this.DiffractionConstants = new List<SCEC.REKInformation>();
            for(int n = 0; n < RA.InvestigatedTensor.DiffractionConstants.Count; n++)
            {
                this.DiffractionConstants.Add(new SCEC.REKInformation(RA.InvestigatedTensor.DiffractionConstants[n]));
            }
            this.CrystalInfo = RA.InvestigatedTensor.DiffractionConstants[0].PhaseInformation;
        }

        public Analysis.MC.RandomAnalysis GetRandomAnalysis()
        {
            Analysis.MC.RandomAnalysis Ret = new Analysis.MC.RandomAnalysis(this.InvestigatedTensor.GetElasticityTensor());
            Ret.Name = this._name;

            Ret.InvestigatedTensor = this.InvestigatedTensor.GetElasticityTensor();
            for(int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                Ret.InvestigatedTensor.DiffractionConstants.Add(this.DiffractionConstants[n].GetREK(this.CrystalInfo));
            }

            Ret.UsedConstants = this.UsedConstants;
            Ret.ConstantBorders = this.ConstantBorders;
            Ret.ClassicREKCaluclation = this.ClassicREKCaluclation;

            for (int n = 0; n < this.TensorResults.Count; n++)
            {
                Analysis.Stress.Microsopic.ElasticityTensors ETTmp = this.TensorResults[n].GetElasticityTensor();
                for (int i = 0; i < Ret.InvestigatedTensor.DiffractionConstants.Count; i++)
                {
                    ETTmp.DiffractionConstants.Add(Ret.InvestigatedTensor.DiffractionConstants[i]);
                }
                Ret.TensorResults.Add(ETTmp);
            }

            return Ret;
        }
    }
}
