using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.DataManagment.Files.SCEC
{
    [Serializable]
    public class ReflexYieldInformation
    {
        public CrystalData.HKLReflex SlipPlane;

        public CrystalData.HKLReflex MainSlipDirection;
        public CrystalData.HKLReflex SecondarySlipDirection;

        private double _yieldMainStrength = -1;
        private int _plainMainMultiplizity;

        public DiffractionOrientation.OrientationMatrix MeasurementCrystalTransformation = new DiffractionOrientation.OrientationMatrix();

        public List<List<PeakStressInformation>> PeakData = new List<List<PeakStressInformation>>();

        public ReflexYieldInformation(Analysis.Stress.Plasticity.ReflexYield rY)
        {
            this.SlipPlane = rY.SlipPlane;
            this.MainSlipDirection = rY.MainSlipDirection;
            this.SecondarySlipDirection = rY.SecondarySlipDirection;
            this.MeasurementCrystalTransformation = rY.MeasurementCrystalTransformation;
            this._yieldMainStrength = rY.YieldMainStrength;
            this._plainMainMultiplizity = rY.PlainMainMultiplizity;

            for(int n = 0; n < rY.PeakData.Count; n++)
            {
                List<PeakStressInformation> pSIList = new List<PeakStressInformation>();
                for (int i = 0; i < rY.PeakData[n].Count; i++)
                {
                    PeakStressInformation pSITmp = new PeakStressInformation(rY.PeakData[n][i]);
                    pSIList.Add(pSITmp);
                }
                PeakData.Add(pSIList);
            }
        }

        public Analysis.Stress.Plasticity.ReflexYield GetReflexYield()
        {
            Analysis.Stress.Plasticity.ReflexYield ret = new Analysis.Stress.Plasticity.ReflexYield();

            ret.SlipPlane = this.SlipPlane;
            ret.MainSlipDirection = this.MainSlipDirection;
            ret.SecondarySlipDirection = this.SecondarySlipDirection;
            ret.MeasurementCrystalTransformation = this.MeasurementCrystalTransformation;
            ret.YieldMainStrength = this._yieldMainStrength;
            ret.PlainMainMultiplizity = this._plainMainMultiplizity;

            for (int n = 0; n < this.PeakData.Count; n++)
            {
                List<Analysis.Stress.Macroskopic.PeakStressAssociation> pSIList = new List<Analysis.Stress.Macroskopic.PeakStressAssociation>();
                for (int i = 0; i < this.PeakData[n].Count; i++)
                {
                    Analysis.Stress.Macroskopic.PeakStressAssociation pSITmp = new Analysis.Stress.Macroskopic.PeakStressAssociation(this.PeakData[n][i]);
                    pSIList.Add(pSITmp);
                }
                ret.PeakData.Add(pSIList);
            }

            return ret;
        }
    }
}
