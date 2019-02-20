using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.DataManagment.Files.SCEC
{
    [Serializable]
    public class YieldSurfaceInformation
    {
        public List<ReflexYieldInformation> ReflexYieldData = new List<ReflexYieldInformation>();
        public List<ReflexYieldInformation> PotentialSlipSystems = new List<ReflexYieldInformation>();

        private DataManagment.CrystalData.CODData CrystalData;

        public YieldSurfaceInformation(Analysis.Stress.Plasticity.YieldSurface yS)
        {
            this.CrystalData = yS.CrystalData;

            for(int n = 0; n < yS.ReflexYieldData.Count; n++)
            {
                ReflexYieldInformation rYITmp = new ReflexYieldInformation(yS.ReflexYieldData[n]);
                this.ReflexYieldData.Add(rYITmp);
            }
            for (int n = 0; n < yS.PotentialSlipSystems.Count; n++)
            {
                ReflexYieldInformation rYITmp = new ReflexYieldInformation(yS.PotentialSlipSystems[n]);
                this.PotentialSlipSystems.Add(rYITmp);
            }
        }


        public Analysis.Stress.Plasticity.YieldSurface GetYieldSurface()
        {
            Analysis.Stress.Plasticity.YieldSurface ret = new Analysis.Stress.Plasticity.YieldSurface(this.CrystalData);
            ret.CrystalData = this.CrystalData;

            for (int n = 0; n < this.ReflexYieldData.Count; n++)
            {
                Analysis.Stress.Plasticity.ReflexYield rYITmp = this.ReflexYieldData[n].GetReflexYield();
                ret.ReflexYieldData.Add(rYITmp);
            }
            for (int n = 0; n < this.PotentialSlipSystems.Count; n++)
            {
                Analysis.Stress.Plasticity.ReflexYield rYITmp = this.ReflexYieldData[n].GetReflexYield();
                ret.PotentialSlipSystems.Add(rYITmp);
            }

            return ret;
        }

    }
}
