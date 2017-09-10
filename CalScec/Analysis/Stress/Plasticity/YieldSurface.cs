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
        List<ReflexYield> ReflexYieldData = new List<ReflexYield>();

        public double GetYieldStrenght(DataManagment.CrystalData.HKLReflex reflex)
        {
            for(int n = 0; n < ReflexYieldData.Count; n++)
            {
                if(reflex.H == ReflexYieldData[n].Reflex.H && reflex.K == ReflexYieldData[n].Reflex.K && reflex.L == ReflexYieldData[n].Reflex.L)
                {
                    if (_useAnisotropy)
                    {
                        return ReflexYieldData[n].YieldStrength;
                    }
                    else
                    {
                        return ReflexYieldData[n].YieldStrength;
                    }
                }
            }

            return -1;
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
            double systemYield = this.GetYieldStrenght(reflex);

            double shearForce = Math.Pow(aStress[0] - aStress[1], 2);
            shearForce += Math.Pow(aStress[1] - aStress[2], 2);
            shearForce += Math.Pow(aStress[0] - aStress[2], 2);
            shearForce += Math.Pow(aStress[3], 2);
            shearForce += Math.Pow(aStress[4], 2);
            shearForce += Math.Pow(aStress[5], 2);
            shearForce = Math.Sqrt((1 / 6) * shearForce);

            if((systemYield - shearForce) > 0)
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
            double systemYield = this.GetYieldStrenght(reflex);

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
    }
}
