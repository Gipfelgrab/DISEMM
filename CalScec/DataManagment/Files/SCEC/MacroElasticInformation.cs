using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.DataManagment.Files.SCEC
{
    [Serializable]
    public class MacroElasticInformation : List<PeakStressInformation>
    {
        public Analysis.Fitting.LinearFunction FittingFunction;

        public MacroElasticInformation(Analysis.Stress.Macroskopic.Elasticity E)
        {
            if (E.FittingFunction != null)
            {
                this.FittingFunction = E.FittingFunction.Clone() as Analysis.Fitting.LinearFunction;
            }
            for(int n = 0; n < E.Count; n++)
            {
                this.Add(new PeakStressInformation(E[n]));
            }
        }
    }

    [Serializable]
    public struct PeakStressInformation
    {
        public double Stress;
        public double PsiAngle;

        public PeakFunctionInformation DPeak;

        public PeakStressInformation(Analysis.Stress.Macroskopic.PeakStressAssociation PSA)
        {
            this.Stress = PSA.Stress;
            this.PsiAngle = PSA.PsiAngle;

            DPeak = new PeakFunctionInformation(PSA.DPeak);
        }
    }
}
