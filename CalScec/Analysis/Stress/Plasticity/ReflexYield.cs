using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.Analysis.Stress.Plasticity
{
    public class ReflexYield
    {
        public DataManagment.CrystalData.HKLReflex Reflex;
        private double _yieldStrength = -1;
        public double YieldStrength
        {
            get
            {
                return this._yieldStrength;
            }
            set
            {
                this._yieldStrength = value;
            }
        }

        List<Analysis.Peaks.DiffractionPeak> PeakData = new List<Peaks.DiffractionPeak>();
        List<bool> ElasticRegime = new List<bool>();
        List<double> MacroskopicStrain = new List<double>();
        List<double> AppliedForce = new List<double>();
        List<double> AppliedStress = new List<double>();
    }
}
