using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CalScec.DataManagment.Files.SCEC;

namespace CalScec.DataManagment.Files.Simulation
{
    [Serializable]
    public class SimBody
    {
        public int FileIndex;
        public bool Symmetric = true;
        public string SavePath = "";

        public List<MathNet.Numerics.LinearAlgebra.Matrix<double>> StressRateSFHistory = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
        public List<MathNet.Numerics.LinearAlgebra.Matrix<double>> StrainRateSFHistory = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();

        //[phase][Step][grainIndex]
        public List<List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>> StressRateCFOrientedHistory = new List<List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>>();
        public List<List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>> StrainRateCFOrientedHistory = new List<List<List<MathNet.Numerics.LinearAlgebra.Matrix<double>>>>();

        //Achtung Speichereihenfolge ist bsonders [step][phase][grain][system]
        public List<List<List<List<double>>>> YieldChangeCFHistory = new List<List<List<List<double>>>>();

        /// <summary>
        /// Phase -> ExperimentalStep -> Orientation -> active Slipsystems
        /// </summary>
        public List<List<List<List<ReflexYieldInformation>>>> ActiveSystemsCFOrientedHistory = new List<List<List<List<ReflexYieldInformation>>>>();

        public SimBody(int fileIndex, bool symmetric)
        {
            this.FileIndex = fileIndex;
            this.Symmetric = symmetric;
        }
    }
}
