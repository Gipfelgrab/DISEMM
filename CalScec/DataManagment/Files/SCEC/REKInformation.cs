using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.DataManagment.Files.SCEC
{
    [Serializable]
    public class REKInformation
    {
        public int CODDataId;

        public DataManagment.CrystalData.HKLReflex UsedReflex;

        public List<PeakStressInformation> ElasticStressInformation;

        public bool ClassicREKFittingConverged;
        public Analysis.Fitting.LinearFunction ClassicFittingFunction;

        public double _strainFreeD0;

        public MacroElasticInformation LongitudionalElasticity;
        public MacroElasticInformation TransversalElasticity;

        public REKInformation(Analysis.Stress.Microsopic.REK rEK)
        {
            this.CODDataId = rEK.PhaseInformation.Id;
            this.UsedReflex = new CrystalData.HKLReflex(rEK.UsedReflex.H, rEK.UsedReflex.K, rEK.UsedReflex.L, rEK.UsedReflex.Distance);

            this.ElasticStressInformation = new List<PeakStressInformation>();
            for (int n = 0; n < rEK.ElasticStressData.Count; n++)
            {
                this.ElasticStressInformation.Add(new PeakStressInformation(rEK.ElasticStressData[n]));
            }

            this.ClassicREKFittingConverged = rEK.ClassicREKFittingConverged;
            this.ClassicFittingFunction = rEK.ClassicFittingFunction.Clone() as Analysis.Fitting.LinearFunction;

            this._strainFreeD0 = rEK.StrainFreeD0;

            if (rEK.LongitudionalElasticity != null)
            {
                this.LongitudionalElasticity = new MacroElasticInformation(rEK.LongitudionalElasticity);
            }
            if (rEK.TransversalElasticity != null)
            {
                this.TransversalElasticity = new MacroElasticInformation(rEK.TransversalElasticity);
            }
        }

        public Analysis.Stress.Microsopic.REK GetREK(DataManagment.CrystalData.CODData phaseInformation)
        {
            Analysis.Stress.Microsopic.REK Ret = new Analysis.Stress.Microsopic.REK(phaseInformation, new CrystalData.HKLReflex(this.UsedReflex.H, this.UsedReflex.K, this.UsedReflex.L, this.UsedReflex.Distance), this._strainFreeD0);

            for(int n = 0; n < this.ElasticStressInformation.Count; n++)
            {
                Ret.ElasticStressData.Add(new Analysis.Stress.Macroskopic.PeakStressAssociation(this.ElasticStressInformation[n]));
            }

            Ret.ClassicREKFittingConverged = this.ClassicREKFittingConverged;
            Ret.ClassicFittingFunction = this.ClassicFittingFunction.Clone() as Analysis.Fitting.LinearFunction;

            Ret.LongitudionalElasticity = new Analysis.Stress.Macroskopic.Elasticity(this.LongitudionalElasticity);
            Ret.TransversalElasticity = new Analysis.Stress.Macroskopic.Elasticity(this.TransversalElasticity);

            Ret.SetClassicDeviation();

            return Ret;
        }
    }
}
