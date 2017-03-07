using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.Analysis.Stress.Microsopic
{
    public class ElasticityTensors : ICloneable
    {
        #region Parameters

        public MathNet.Numerics.LinearAlgebra.Matrix<double> _stiffnessTensor = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(6, 6, 0.0);
        public MathNet.Numerics.LinearAlgebra.Matrix<double> _stiffnessTensorError = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(6, 6, 0.0);

        public MathNet.Numerics.LinearAlgebra.Matrix<double> _complianceTensor = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(6, 6, 0.0);
        public MathNet.Numerics.LinearAlgebra.Matrix<double> _complianceTensorError = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(6, 6, 0.0);

        private int _symmetry;
        public string Symmetry
        {
            get
            {
                switch(this._symmetry)
                {
                    case 1:
                        return "cubic";
                    case 2:
                        return "hexagonal";
                    case 3:
                        return "tetragonal type 1";
                    case 4:
                        return "tetragonal type 2";
                    case 5:
                        return "trigonal type 1";
                    case 6:
                        return "trigonal type 2";
                    case 7:
                        return "rhombic";
                    case 8:
                        return "monoclinic";
                    case 9:
                        return "triclinic";
                    default:
                        return "triclinic";
                }
            }
            set
            {
                switch(value)
                {
                    case "cubic":
                        this._symmetry = 1;
                        break;
                    case "hexagonal":
                        this._symmetry = 2;
                        break;
                    case "tetragonal type 1":
                        this._symmetry = 3;
                        break;
                    case "tetragonal type 2":
                        this._symmetry = 4;
                        break;
                    case "trigonal type 1":
                        this._symmetry = 5;
                        break;
                    case "trigonal type 2":
                        this._symmetry = 6;
                        break;
                    case "rhombic":
                        this._symmetry = 7;
                        break;
                    case "monoclinic":
                        this._symmetry = 8;
                        break;
                    case "triclinic":
                        this._symmetry = 9;
                        break;
                    default:
                        this._symmetry = 9;
                        break;
                }
            }
        }

        public DataManagment.CrystalData.CODData GetPhaseInformation
        {
            get
            {
                if(this.DiffractionConstants.Count > 0)
                {
                    return DiffractionConstants[0].PhaseInformation;
                }
                else
                {
                    return new DataManagment.CrystalData.CODData();
                }
            }
        }

        public List<REK> DiffractionConstants = new List<REK>();

        public bool IsIsotropic = false;
        public bool FitConverged = false;

        #region Voigt Matrix components

        #region Stiffness matrix components

        public double C11
        {
            get
            {
                return this._stiffnessTensor[0, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._stiffnessTensor[0, 0] = value;
                        this._stiffnessTensor[1, 1] = value;
                        this._stiffnessTensor[2, 2] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 0.5 * (this._stiffnessTensor[0, 0] - this._stiffnessTensor[0, 1]);
                            this._stiffnessTensor[3, 3] = IsoValue;
                            this._stiffnessTensor[4, 4] = IsoValue;
                            this._stiffnessTensor[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        double IsoValue1 = 0.5 * (value - this._stiffnessTensor[0, 1]);
                        this._stiffnessTensor[5, 5] = IsoValue1;
                        goto case 3;
                    case 3:
                        this._stiffnessTensor[0, 0] = value;
                        this._stiffnessTensor[1, 1] = value;
                        break;
                    case 4:
                        goto case 3;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._stiffnessTensor[0, 0] = value;
                        break;
                }
            }
        }
        public double C11Error
        {
            get
            {
                return this._stiffnessTensorError[0, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._stiffnessTensorError[0, 0] = value;
                        this._stiffnessTensorError[1, 1] = value;
                        this._stiffnessTensorError[2, 2] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 0.5 * (this._stiffnessTensorError[0, 0] - this._stiffnessTensorError[0, 1]);
                            this._stiffnessTensorError[3, 3] = IsoValue;
                            this._stiffnessTensorError[4, 4] = IsoValue;
                            this._stiffnessTensorError[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        double IsoValue1 = 0.5 * (value - this._stiffnessTensorError[0, 1]);
                        this._stiffnessTensorError[5, 5] = IsoValue1;
                        goto case 3;
                    case 3:
                        this._stiffnessTensorError[0, 0] = value;
                        this._stiffnessTensorError[1, 1] = value;
                        break;
                    case 4:
                        goto case 3;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._stiffnessTensorError[0, 0] = value;
                        break;
                }
            }
        }

        public double C12
        {
            get
            {
                return this._stiffnessTensor[0, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._stiffnessTensor[0, 1] = value;
                        this._stiffnessTensor[0, 2] = value;
                        this._stiffnessTensor[1, 2] = value;
                        this._stiffnessTensor[1, 0] = value;
                        this._stiffnessTensor[2, 0] = value;
                        this._stiffnessTensor[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 0.5 * (this._stiffnessTensor[0, 0] - this._stiffnessTensor[0, 1]);
                            this._stiffnessTensor[3, 3] = IsoValue;
                            this._stiffnessTensor[4, 4] = IsoValue;
                            this._stiffnessTensor[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        double IsoValue1 = 0.5 * (this._stiffnessTensor[0, 0] - value);
                        this._stiffnessTensor[5, 5] = IsoValue1;
                        goto default;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._stiffnessTensor[0, 1] = value;
                        this._stiffnessTensor[1, 0] = value;
                        break;
                }
            }
        }
        public double C12Error
        {
            get
            {
                return this._stiffnessTensorError[0, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._stiffnessTensorError[0, 1] = value;
                        this._stiffnessTensorError[0, 2] = value;
                        this._stiffnessTensorError[1, 2] = value;
                        this._stiffnessTensorError[1, 0] = value;
                        this._stiffnessTensorError[2, 0] = value;
                        this._stiffnessTensorError[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 0.5 * (this._stiffnessTensorError[0, 0] - this._stiffnessTensorError[0, 1]);
                            this._stiffnessTensorError[3, 3] = IsoValue;
                            this._stiffnessTensorError[4, 4] = IsoValue;
                            this._stiffnessTensorError[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        double IsoValue1 = 0.5 * (this._stiffnessTensorError[0, 0] - value);
                        this._stiffnessTensorError[5, 5] = IsoValue1;
                        goto default;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._stiffnessTensorError[0, 1] = value;
                        this._stiffnessTensorError[1, 0] = value;
                        break;
                }
            }
        }

        public double C13
        {
            get
            {
                return this._stiffnessTensor[0, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._stiffnessTensor[0, 1] = value;
                        this._stiffnessTensor[0, 2] = value;
                        this._stiffnessTensor[1, 2] = value;
                        this._stiffnessTensor[1, 0] = value;
                        this._stiffnessTensor[2, 0] = value;
                        this._stiffnessTensor[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 0.5 * (this._stiffnessTensor[0, 0] - this._stiffnessTensor[0, 1]);
                            this._stiffnessTensor[3, 3] = IsoValue;
                            this._stiffnessTensor[4, 4] = IsoValue;
                            this._stiffnessTensor[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        this._stiffnessTensor[0, 2] = value;
                        this._stiffnessTensor[1, 2] = value;
                        this._stiffnessTensor[2, 0] = value;
                        this._stiffnessTensor[2, 1] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._stiffnessTensor[0, 2] = value;
                        this._stiffnessTensor[2, 0] = value;
                        break;
                }
            }
        }
        public double C13Error
        {
            get
            {
                return this._stiffnessTensorError[0, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._stiffnessTensorError[0, 1] = value;
                        this._stiffnessTensorError[0, 2] = value;
                        this._stiffnessTensorError[1, 2] = value;
                        this._stiffnessTensorError[1, 0] = value;
                        this._stiffnessTensorError[2, 0] = value;
                        this._stiffnessTensorError[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 0.5 * (this._stiffnessTensorError[0, 0] - this._stiffnessTensorError[0, 1]);
                            this._stiffnessTensorError[3, 3] = IsoValue;
                            this._stiffnessTensorError[4, 4] = IsoValue;
                            this._stiffnessTensorError[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        this._stiffnessTensorError[0, 2] = value;
                        this._stiffnessTensorError[1, 2] = value;
                        this._stiffnessTensorError[2, 0] = value;
                        this._stiffnessTensorError[2, 1] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._stiffnessTensorError[0, 2] = value;
                        this._stiffnessTensorError[2, 0] = value;
                        break;
                }
            }
        }

        public double C14
        {
            get
            {
                return this._stiffnessTensor[0, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._stiffnessTensor[0, 3] = value;
                        this._stiffnessTensor[1, 3] = -1 * value;
                        this._stiffnessTensor[4, 5] = value;
                        this._stiffnessTensor[3, 0] = value;
                        this._stiffnessTensor[3, 1] = -1 * value;
                        this._stiffnessTensor[5, 4] = value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._stiffnessTensor[0, 3] = value;
                        this._stiffnessTensor[3, 0] = value;
                        break;
                    default:
                        this._stiffnessTensor[0, 3] = 0;
                        this._stiffnessTensor[3, 0] = 0;
                        break;
                }
            }
        }
        public double C14Error
        {
            get
            {
                return this._stiffnessTensorError[0, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._stiffnessTensorError[0, 3] = value;
                        this._stiffnessTensorError[1, 3] = -1 * value;
                        this._stiffnessTensorError[4, 5] = value;
                        this._stiffnessTensorError[3, 0] = value;
                        this._stiffnessTensorError[3, 1] = -1 * value;
                        this._stiffnessTensorError[5, 4] = value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._stiffnessTensorError[0, 3] = value;
                        this._stiffnessTensorError[3, 0] = value;
                        break;
                    default:
                        this._stiffnessTensorError[0, 3] = 0;
                        this._stiffnessTensorError[3, 0] = 0;
                        break;
                }
            }
        }

        public double C15
        {
            get
            {
                return this._stiffnessTensor[0, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._stiffnessTensor[0, 4] = value;
                        this._stiffnessTensor[1, 4] = -1 * value;
                        this._stiffnessTensor[3, 5] = -1 * value;
                        this._stiffnessTensor[4, 0] = value;
                        this._stiffnessTensor[4, 1] = -1 * value;
                        this._stiffnessTensor[5, 3] = -1 * value;
                        break;
                    case 9:
                        this._stiffnessTensor[0, 4] = value;
                        this._stiffnessTensor[4, 0] = value;
                        break;
                    default:
                        this._stiffnessTensor[0, 4] = 0;
                        this._stiffnessTensor[4, 0] = 0;
                        break;
                }
            }
        }
        public double C15Error
        {
            get
            {
                return this._stiffnessTensorError[0, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._stiffnessTensorError[0, 4] = value;
                        this._stiffnessTensorError[1, 4] = -1 * value;
                        this._stiffnessTensorError[3, 5] = -1 * value;
                        this._stiffnessTensorError[4, 0] = value;
                        this._stiffnessTensorError[4, 1] = -1 * value;
                        this._stiffnessTensorError[5, 3] = -1 * value;
                        break;
                    case 9:
                        this._stiffnessTensorError[0, 4] = value;
                        this._stiffnessTensorError[4, 0] = value;
                        break;
                    default:
                        this._stiffnessTensorError[0, 4] = 0;
                        this._stiffnessTensorError[4, 0] = 0;
                        break;
                }
            }
        }

        public double C16
        {
            get
            {
                return this._stiffnessTensor[0, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 4:
                        this._stiffnessTensor[0, 5] = value;
                        this._stiffnessTensor[1, 5] = -1 * value;
                        this._stiffnessTensor[5, 0] = value;
                        this._stiffnessTensor[5, 1] = -1 * value;
                        break;
                    case 8:
                        this._stiffnessTensor[0, 5] = value;
                        this._stiffnessTensor[5, 0] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._stiffnessTensor[0, 5] = 0;
                        this._stiffnessTensor[5, 0] = 0;
                        break;
                }
            }
        }
        public double C16Error
        {
            get
            {
                return this._stiffnessTensorError[0, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 4:
                        this._stiffnessTensorError[0, 5] = value;
                        this._stiffnessTensorError[1, 5] = -1 * value;
                        this._stiffnessTensorError[5, 0] = value;
                        this._stiffnessTensorError[5, 1] = -1 * value;
                        break;
                    case 8:
                        this._stiffnessTensorError[0, 5] = value;
                        this._stiffnessTensorError[5, 0] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._stiffnessTensorError[0, 5] = 0;
                        this._stiffnessTensorError[5, 0] = 0;
                        break;
                }
            }
        }

        public double C21
        {
            get
            {
                return this._stiffnessTensor[1, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._stiffnessTensor[0, 1] = value;
                        this._stiffnessTensor[0, 2] = value;
                        this._stiffnessTensor[1, 2] = value;
                        this._stiffnessTensor[1, 0] = value;
                        this._stiffnessTensor[2, 0] = value;
                        this._stiffnessTensor[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 0.5 * (this._stiffnessTensor[0, 0] - this._stiffnessTensor[0, 1]);
                            this._stiffnessTensor[3, 3] = IsoValue;
                            this._stiffnessTensor[4, 4] = IsoValue;
                            this._stiffnessTensor[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        double IsoValue1 = 0.5 * (this._stiffnessTensor[0, 0] - value);
                        this._stiffnessTensor[5, 5] = IsoValue1;
                        goto default;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._stiffnessTensor[0, 1] = value;
                        this._stiffnessTensor[1, 0] = value;
                        break;
                }
            }
        }
        public double C21Error
        {
            get
            {
                return this._stiffnessTensorError[1, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._stiffnessTensorError[0, 1] = value;
                        this._stiffnessTensorError[0, 2] = value;
                        this._stiffnessTensorError[1, 2] = value;
                        this._stiffnessTensorError[1, 0] = value;
                        this._stiffnessTensorError[2, 0] = value;
                        this._stiffnessTensorError[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 0.5 * (this._stiffnessTensorError[0, 0] - this._stiffnessTensorError[0, 1]);
                            this._stiffnessTensorError[3, 3] = IsoValue;
                            this._stiffnessTensorError[4, 4] = IsoValue;
                            this._stiffnessTensorError[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        double IsoValue1 = 0.5 * (this._stiffnessTensorError[0, 0] - value);
                        this._stiffnessTensorError[5, 5] = IsoValue1;
                        goto default;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._stiffnessTensorError[0, 1] = value;
                        this._stiffnessTensorError[1, 0] = value;
                        break;
                }
            }
        }

        public double C22
        {
            get
            {
                return this._stiffnessTensor[1, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._stiffnessTensor[0, 0] = value;
                        this._stiffnessTensor[1, 1] = value;
                        this._stiffnessTensor[2, 2] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 0.5 * (this._stiffnessTensor[0, 0] - this._stiffnessTensor[0, 1]);
                            this._stiffnessTensor[3, 3] = IsoValue;
                            this._stiffnessTensor[4, 4] = IsoValue;
                            this._stiffnessTensor[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        double IsoValue1 = 0.5 * (value - this._stiffnessTensor[0, 1]);
                        this._stiffnessTensor[5, 5] = IsoValue1;
                        goto case 3;
                    case 3:
                        this._stiffnessTensor[0, 0] = value;
                        this._stiffnessTensor[1, 1] = value;
                        break;
                    case 4:
                        goto case 3;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._stiffnessTensor[1, 1] = value;
                        break;
                }
            }
        }
        public double C22Error
        {
            get
            {
                return this._stiffnessTensorError[1, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._stiffnessTensorError[0, 0] = value;
                        this._stiffnessTensorError[1, 1] = value;
                        this._stiffnessTensorError[2, 2] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 0.5 * (this._stiffnessTensorError[0, 0] - this._stiffnessTensorError[0, 1]);
                            this._stiffnessTensorError[3, 3] = IsoValue;
                            this._stiffnessTensorError[4, 4] = IsoValue;
                            this._stiffnessTensorError[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        double IsoValue1 = 0.5 * (value - this._stiffnessTensorError[0, 1]);
                        this._stiffnessTensorError[5, 5] = IsoValue1;
                        goto case 3;
                    case 3:
                        this._stiffnessTensorError[0, 0] = value;
                        this._stiffnessTensorError[1, 1] = value;
                        break;
                    case 4:
                        goto case 3;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._stiffnessTensorError[1, 1] = value;
                        break;
                }
            }
        }

        public double C23
        {
            get
            {
                return this._stiffnessTensor[1, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._stiffnessTensor[0, 1] = value;
                        this._stiffnessTensor[0, 2] = value;
                        this._stiffnessTensor[1, 2] = value;
                        this._stiffnessTensor[1, 0] = value;
                        this._stiffnessTensor[2, 0] = value;
                        this._stiffnessTensor[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 0.5 * (this._stiffnessTensor[0, 0] - this._stiffnessTensor[0, 1]);
                            this._stiffnessTensor[3, 3] = IsoValue;
                            this._stiffnessTensor[4, 4] = IsoValue;
                            this._stiffnessTensor[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        this._stiffnessTensor[0, 2] = value;
                        this._stiffnessTensor[1, 2] = value;
                        this._stiffnessTensor[2, 0] = value;
                        this._stiffnessTensor[2, 1] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._stiffnessTensor[1, 2] = value;
                        this._stiffnessTensor[2, 1] = value;
                        break;
                }
            }
        }
        public double C23Error
        {
            get
            {
                return this._stiffnessTensorError[1, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._stiffnessTensorError[0, 1] = value;
                        this._stiffnessTensorError[0, 2] = value;
                        this._stiffnessTensorError[1, 2] = value;
                        this._stiffnessTensorError[1, 0] = value;
                        this._stiffnessTensorError[2, 0] = value;
                        this._stiffnessTensorError[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 0.5 * (this._stiffnessTensorError[0, 0] - this._stiffnessTensorError[0, 1]);
                            this._stiffnessTensorError[3, 3] = IsoValue;
                            this._stiffnessTensorError[4, 4] = IsoValue;
                            this._stiffnessTensorError[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        this._stiffnessTensorError[0, 2] = value;
                        this._stiffnessTensorError[1, 2] = value;
                        this._stiffnessTensorError[2, 0] = value;
                        this._stiffnessTensorError[2, 1] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._stiffnessTensorError[1, 2] = value;
                        this._stiffnessTensorError[2, 1] = value;
                        break;
                }
            }
        }

        public double C24
        {
            get
            {
                return this._stiffnessTensor[1, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._stiffnessTensor[0, 3] = -1 * value;
                        this._stiffnessTensor[1, 3] = value;
                        this._stiffnessTensor[4, 5] = -1 * value;
                        this._stiffnessTensor[3, 0] = -1 * value;
                        this._stiffnessTensor[3, 1] = value;
                        this._stiffnessTensor[5, 4] = -1 * value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._stiffnessTensor[1, 3] = value;
                        this._stiffnessTensor[3, 1] = value;
                        break;
                    default:
                        this._stiffnessTensor[1, 3] = 0;
                        this._stiffnessTensor[3, 1] = 0;
                        break;
                }
            }
        }
        public double C24Error
        {
            get
            {
                return this._stiffnessTensorError[1, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._stiffnessTensorError[0, 3] = -1 * value;
                        this._stiffnessTensorError[1, 3] = value;
                        this._stiffnessTensorError[4, 5] = -1 * value;
                        this._stiffnessTensorError[3, 0] = -1 * value;
                        this._stiffnessTensorError[3, 1] = value;
                        this._stiffnessTensorError[5, 4] = -1 * value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._stiffnessTensorError[1, 3] = value;
                        this._stiffnessTensorError[3, 1] = value;
                        break;
                    default:
                        this._stiffnessTensorError[1, 3] = 0;
                        this._stiffnessTensorError[3, 1] = 0;
                        break;
                }
            }
        }

        public double C25
        {
            get
            {
                return this._stiffnessTensor[1, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._stiffnessTensor[0, 4] = -1 * value;
                        this._stiffnessTensor[1, 4] = value;
                        this._stiffnessTensor[3, 5] = value;
                        this._stiffnessTensor[4, 0] = -1 * value;
                        this._stiffnessTensor[4, 1] = value;
                        this._stiffnessTensor[5, 3] = value;
                        break;
                    case 9:
                        this._stiffnessTensor[1, 4] = value;
                        this._stiffnessTensor[4, 1] = value;
                        break;
                    default:
                        this._stiffnessTensor[1, 4] = 0;
                        this._stiffnessTensor[4, 1] = 0;
                        break;
                }
            }
        }
        public double C25Error
        {
            get
            {
                return this._stiffnessTensorError[1, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._stiffnessTensorError[0, 4] = -1 * value;
                        this._stiffnessTensorError[1, 4] = value;
                        this._stiffnessTensorError[3, 5] = value;
                        this._stiffnessTensorError[4, 0] = -1 * value;
                        this._stiffnessTensorError[4, 1] = value;
                        this._stiffnessTensorError[5, 3] = value;
                        break;
                    case 9:
                        this._stiffnessTensorError[1, 4] = value;
                        this._stiffnessTensorError[4, 1] = value;
                        break;
                    default:
                        this._stiffnessTensorError[1, 4] = 0;
                        this._stiffnessTensorError[4, 1] = 0;
                        break;
                }
            }
        }

        public double C26
        {
            get
            {
                return this._stiffnessTensor[1, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 4:
                        this._stiffnessTensor[0, 5] = -1 * value;
                        this._stiffnessTensor[1, 5] = value;
                        this._stiffnessTensor[5, 0] = -1 * value;
                        this._stiffnessTensor[5, 1] = value;
                        break;
                    case 8:
                        this._stiffnessTensor[1, 5] = value;
                        this._stiffnessTensor[5, 1] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._stiffnessTensor[1, 5] = 0;
                        this._stiffnessTensor[5, 1] = 0;
                        break;
                }
            }
        }
        public double C26Error
        {
            get
            {
                return this._stiffnessTensorError[1, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 4:
                        this._stiffnessTensorError[0, 5] = -1 * value;
                        this._stiffnessTensorError[1, 5] = value;
                        this._stiffnessTensorError[5, 0] = -1 * value;
                        this._stiffnessTensorError[5, 1] = value;
                        break;
                    case 8:
                        this._stiffnessTensorError[1, 5] = value;
                        this._stiffnessTensorError[5, 1] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._stiffnessTensorError[1, 5] = 0;
                        this._stiffnessTensorError[5, 1] = 0;
                        break;
                }
            }
        }

        public double C31
        {
            get
            {
                return this._stiffnessTensor[2, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._stiffnessTensor[0, 1] = value;
                        this._stiffnessTensor[0, 2] = value;
                        this._stiffnessTensor[1, 2] = value;
                        this._stiffnessTensor[1, 0] = value;
                        this._stiffnessTensor[2, 0] = value;
                        this._stiffnessTensor[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 0.5 * (this._stiffnessTensor[0, 0] - this._stiffnessTensor[0, 1]);
                            this._stiffnessTensor[3, 3] = IsoValue;
                            this._stiffnessTensor[4, 4] = IsoValue;
                            this._stiffnessTensor[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        this._stiffnessTensor[0, 2] = value;
                        this._stiffnessTensor[1, 2] = value;
                        this._stiffnessTensor[2, 0] = value;
                        this._stiffnessTensor[2, 1] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._stiffnessTensor[0, 2] = value;
                        this._stiffnessTensor[2, 0] = value;
                        break;
                }
            }
        }
        public double C31Error
        {
            get
            {
                return this._stiffnessTensorError[2, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._stiffnessTensorError[0, 1] = value;
                        this._stiffnessTensorError[0, 2] = value;
                        this._stiffnessTensorError[1, 2] = value;
                        this._stiffnessTensorError[1, 0] = value;
                        this._stiffnessTensorError[2, 0] = value;
                        this._stiffnessTensorError[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 0.5 * (this._stiffnessTensorError[0, 0] - this._stiffnessTensorError[0, 1]);
                            this._stiffnessTensorError[3, 3] = IsoValue;
                            this._stiffnessTensorError[4, 4] = IsoValue;
                            this._stiffnessTensorError[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        this._stiffnessTensorError[0, 2] = value;
                        this._stiffnessTensorError[1, 2] = value;
                        this._stiffnessTensorError[2, 0] = value;
                        this._stiffnessTensorError[2, 1] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._stiffnessTensorError[0, 2] = value;
                        this._stiffnessTensorError[2, 0] = value;
                        break;
                }
            }
        }

        public double C32
        {
            get
            {
                return this._stiffnessTensor[2, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._stiffnessTensor[0, 1] = value;
                        this._stiffnessTensor[0, 2] = value;
                        this._stiffnessTensor[1, 2] = value;
                        this._stiffnessTensor[1, 0] = value;
                        this._stiffnessTensor[2, 0] = value;
                        this._stiffnessTensor[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 0.5 * (this._stiffnessTensor[0, 0] - this._stiffnessTensor[0, 1]);
                            this._stiffnessTensor[3, 3] = IsoValue;
                            this._stiffnessTensor[4, 4] = IsoValue;
                            this._stiffnessTensor[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        this._stiffnessTensor[0, 2] = value;
                        this._stiffnessTensor[1, 2] = value;
                        this._stiffnessTensor[2, 0] = value;
                        this._stiffnessTensor[2, 1] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._stiffnessTensor[1, 2] = value;
                        this._stiffnessTensor[2, 1] = value;
                        break;
                }
            }
        }
        public double C32Error
        {
            get
            {
                return this._stiffnessTensorError[2, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._stiffnessTensorError[0, 1] = value;
                        this._stiffnessTensorError[0, 2] = value;
                        this._stiffnessTensorError[1, 2] = value;
                        this._stiffnessTensorError[1, 0] = value;
                        this._stiffnessTensorError[2, 0] = value;
                        this._stiffnessTensorError[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 0.5 * (this._stiffnessTensorError[0, 0] - this._stiffnessTensorError[0, 1]);
                            this._stiffnessTensorError[3, 3] = IsoValue;
                            this._stiffnessTensorError[4, 4] = IsoValue;
                            this._stiffnessTensorError[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        this._stiffnessTensorError[0, 2] = value;
                        this._stiffnessTensorError[1, 2] = value;
                        this._stiffnessTensorError[2, 0] = value;
                        this._stiffnessTensorError[2, 1] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._stiffnessTensorError[1, 2] = value;
                        this._stiffnessTensorError[2, 1] = value;
                        break;
                }
            }
        }

        public double C33
        {
            get
            {
                return this._stiffnessTensor[2, 2];
            }
            set
            {
                switch(this._symmetry)
                {
                    case 1:
                        this._stiffnessTensor[0, 0] = value;
                        this._stiffnessTensor[1, 1] = value;
                        this._stiffnessTensor[2, 2] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 0.5 * (this._stiffnessTensor[0, 0] - this._stiffnessTensor[0, 1]);
                            this._stiffnessTensor[3, 3] = IsoValue;
                            this._stiffnessTensor[4, 4] = IsoValue;
                            this._stiffnessTensor[5, 5] = IsoValue;
                        }
                        break;
                    default:
                        this._stiffnessTensor[2, 2] = value;
                        break;
                }
            }
        }
        public double C33Error
        {
            get
            {
                return this._stiffnessTensorError[2, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._stiffnessTensorError[0, 0] = value;
                        this._stiffnessTensorError[1, 1] = value;
                        this._stiffnessTensorError[2, 2] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 0.5 * (this._stiffnessTensorError[0, 0] - this._stiffnessTensorError[0, 1]);
                            this._stiffnessTensorError[3, 3] = IsoValue;
                            this._stiffnessTensorError[4, 4] = IsoValue;
                            this._stiffnessTensorError[5, 5] = IsoValue;
                        }
                        break;
                    default:
                        this._stiffnessTensorError[2, 2] = value;
                        break;
                }
            }
        }

        public double C34
        {
            get
            {
                return this._stiffnessTensor[2, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 9:
                        this._stiffnessTensor[2, 3] = value;
                        this._stiffnessTensor[3, 2] = value;
                        break;
                    default:
                        this._stiffnessTensor[2, 3] = 0;
                        this._stiffnessTensor[3, 2] = 0;
                        break;
                }
            }
        }
        public double C34Error
        {
            get
            {
                return this._stiffnessTensorError[2, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 9:
                        this._stiffnessTensorError[2, 3] = value;
                        this._stiffnessTensorError[3, 2] = value;
                        break;
                    default:
                        this._stiffnessTensorError[2, 3] = 0;
                        this._stiffnessTensorError[3, 2] = 0;
                        break;
                }
            }
        }

        public double C35
        {
            get
            {
                return this._stiffnessTensor[2, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 9:
                        this._stiffnessTensor[2, 4] = value;
                        this._stiffnessTensor[4, 2] = value;
                        break;
                    default:
                        this._stiffnessTensor[2, 4] = 0;
                        this._stiffnessTensor[4, 2] = 0;
                        break;
                }
            }
        }
        public double C35Error
        {
            get
            {
                return this._stiffnessTensorError[2, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 9:
                        this._stiffnessTensorError[2, 4] = value;
                        this._stiffnessTensorError[4, 2] = value;
                        break;
                    default:
                        this._stiffnessTensorError[2, 4] = 0;
                        this._stiffnessTensorError[4, 2] = 0;
                        break;
                }
            }
        }

        public double C36
        {
            get
            {
                return this._stiffnessTensor[2, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 8:
                        this._stiffnessTensor[2, 5] = value;
                        this._stiffnessTensor[5, 2] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._stiffnessTensor[2, 5] = 0;
                        this._stiffnessTensor[5, 2] = 0;
                        break;
                }
            }
        }
        public double C36Error
        {
            get
            {
                return this._stiffnessTensorError[2, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 8:
                        this._stiffnessTensorError[2, 5] = value;
                        this._stiffnessTensorError[5, 2] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._stiffnessTensorError[2, 5] = 0;
                        this._stiffnessTensorError[5, 2] = 0;
                        break;
                }
            }
        }

        public double C41
        {
            get
            {
                return this._stiffnessTensor[3, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._stiffnessTensor[0, 3] = value;
                        this._stiffnessTensor[1, 3] = -1 * value;
                        this._stiffnessTensor[4, 5] = value;
                        this._stiffnessTensor[3, 0] = value;
                        this._stiffnessTensor[3, 1] = -1 * value;
                        this._stiffnessTensor[5, 4] = value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._stiffnessTensor[0, 3] = value;
                        this._stiffnessTensor[3, 0] = value;
                        break;
                    default:
                        this._stiffnessTensor[0, 3] = 0;
                        this._stiffnessTensor[3, 0] = 0;
                        break;
                }
            }
        }
        public double C41Error
        {
            get
            {
                return this._stiffnessTensorError[3, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._stiffnessTensorError[0, 3] = value;
                        this._stiffnessTensorError[1, 3] = -1 * value;
                        this._stiffnessTensorError[4, 5] = value;
                        this._stiffnessTensorError[3, 0] = value;
                        this._stiffnessTensorError[3, 1] = -1 * value;
                        this._stiffnessTensorError[5, 4] = value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._stiffnessTensorError[0, 3] = value;
                        this._stiffnessTensorError[3, 0] = value;
                        break;
                    default:
                        this._stiffnessTensorError[0, 3] = 0;
                        this._stiffnessTensorError[3, 0] = 0;
                        break;
                }
            }
        }

        public double C42
        {
            get
            {
                return this._stiffnessTensor[3, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._stiffnessTensor[0, 3] = -1 * value;
                        this._stiffnessTensor[1, 3] = value;
                        this._stiffnessTensor[4, 5] = -1 * value;
                        this._stiffnessTensor[3, 0] = -1 * value;
                        this._stiffnessTensor[3, 1] = value;
                        this._stiffnessTensor[5, 4] = -1 * value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._stiffnessTensor[1, 3] = value;
                        this._stiffnessTensor[3, 1] = value;
                        break;
                    default:
                        this._stiffnessTensor[1, 3] = 0;
                        this._stiffnessTensor[3, 1] = 0;
                        break;
                }
            }
        }
        public double C42Error
        {
            get
            {
                return this._stiffnessTensorError[3, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._stiffnessTensorError[0, 3] = -1 * value;
                        this._stiffnessTensorError[1, 3] = value;
                        this._stiffnessTensorError[4, 5] = -1 * value;
                        this._stiffnessTensorError[3, 0] = -1 * value;
                        this._stiffnessTensorError[3, 1] = value;
                        this._stiffnessTensorError[5, 4] = -1 * value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._stiffnessTensorError[1, 3] = value;
                        this._stiffnessTensorError[3, 1] = value;
                        break;
                    default:
                        this._stiffnessTensorError[1, 3] = 0;
                        this._stiffnessTensorError[3, 1] = 0;
                        break;
                }
            }
        }

        public double C43
        {
            get
            {
                return this._stiffnessTensor[3, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 9:
                        this._stiffnessTensor[2, 3] = value;
                        this._stiffnessTensor[3, 2] = value;
                        break;
                    default:
                        this._stiffnessTensor[2, 3] = 0;
                        this._stiffnessTensor[3, 2] = 0;
                        break;
                }
            }
        }
        public double C43Error
        {
            get
            {
                return this._stiffnessTensorError[3, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 9:
                        this._stiffnessTensorError[2, 3] = value;
                        this._stiffnessTensorError[3, 2] = value;
                        break;
                    default:
                        this._stiffnessTensorError[2, 3] = 0;
                        this._stiffnessTensorError[3, 2] = 0;
                        break;
                }
            }
        }

        public double C44
        {
            get
            {
                return this._stiffnessTensor[3, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        if(!this.IsIsotropic)
                        {
                            this._stiffnessTensor[3, 3] = value;
                            this._stiffnessTensor[4, 4] = value;
                            this._stiffnessTensor[5, 5] = value;
                        }
                        break;
                    case 2:
                        this._stiffnessTensor[3, 3] = value;
                        this._stiffnessTensor[4, 4] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._stiffnessTensor[3, 3] = value;
                        break;
                }
            }
        }
        public double C44Error
        {
            get
            {
                return this._stiffnessTensorError[3, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        if (!this.IsIsotropic)
                        {
                            this._stiffnessTensorError[3, 3] = value;
                            this._stiffnessTensorError[4, 4] = value;
                            this._stiffnessTensorError[5, 5] = value;
                        }
                        break;
                    case 2:
                        this._stiffnessTensorError[3, 3] = value;
                        this._stiffnessTensorError[4, 4] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._stiffnessTensorError[3, 3] = value;
                        break;
                }
            }
        }

        public double C45
        {
            get
            {
                return this._stiffnessTensor[3, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 8:
                        this._stiffnessTensor[3, 4] = value;
                        this._stiffnessTensor[4, 3] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._stiffnessTensor[3, 4] = 0;
                        this._stiffnessTensor[4, 3] = 0;
                        break;
                }
            }
        }
        public double C45Error
        {
            get
            {
                return this._stiffnessTensorError[3, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 8:
                        this._stiffnessTensorError[3, 4] = value;
                        this._stiffnessTensorError[4, 3] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._stiffnessTensorError[3, 4] = 0;
                        this._stiffnessTensorError[4, 3] = 0;
                        break;
                }
            }
        }

        public double C46
        {
            get
            {
                return this._stiffnessTensor[3, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._stiffnessTensor[0, 4] = -1 * value;
                        this._stiffnessTensor[1, 4] = value;
                        this._stiffnessTensor[3, 5] = value;
                        this._stiffnessTensor[4, 0] = -1 * value;
                        this._stiffnessTensor[4, 1] = value;
                        this._stiffnessTensor[5, 3] = value;
                        break;
                    case 9:
                        this._stiffnessTensor[3, 5] = value;
                        this._stiffnessTensor[5, 3] = value;
                        break;
                    default:
                        this._stiffnessTensor[3, 5] = 0;
                        this._stiffnessTensor[5, 3] = 0;
                        break;
                }
            }
        }
        public double C46Error
        {
            get
            {
                return this._stiffnessTensorError[3, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._stiffnessTensorError[0, 4] = -1 * value;
                        this._stiffnessTensorError[1, 4] = value;
                        this._stiffnessTensorError[3, 5] = value;
                        this._stiffnessTensorError[4, 0] = -1 * value;
                        this._stiffnessTensorError[4, 1] = value;
                        this._stiffnessTensorError[5, 3] = value;
                        break;
                    case 9:
                        this._stiffnessTensorError[3, 5] = value;
                        this._stiffnessTensorError[5, 3] = value;
                        break;
                    default:
                        this._stiffnessTensorError[3, 5] = 0;
                        this._stiffnessTensorError[5, 3] = 0;
                        break;
                }
            }
        }

        public double C51
        {
            get
            {
                return this._stiffnessTensor[4, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._stiffnessTensor[0, 4] = value;
                        this._stiffnessTensor[1, 4] = -1 * value;
                        this._stiffnessTensor[3, 5] = -1 * value;
                        this._stiffnessTensor[4, 0] = value;
                        this._stiffnessTensor[4, 1] = -1 * value;
                        this._stiffnessTensor[5, 3] = -1 * value;
                        break;
                    case 9:
                        this._stiffnessTensor[0, 4] = value;
                        this._stiffnessTensor[4, 0] = value;
                        break;
                    default:
                        this._stiffnessTensor[0, 4] = 0;
                        this._stiffnessTensor[4, 0] = 0;
                        break;
                }
            }
        }
        public double C51Error
        {
            get
            {
                return this._stiffnessTensorError[4, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._stiffnessTensorError[0, 4] = value;
                        this._stiffnessTensorError[1, 4] = -1 * value;
                        this._stiffnessTensorError[3, 5] = -1 * value;
                        this._stiffnessTensorError[4, 0] = value;
                        this._stiffnessTensorError[4, 1] = -1 * value;
                        this._stiffnessTensorError[5, 3] = -1 * value;
                        break;
                    case 9:
                        this._stiffnessTensorError[0, 4] = value;
                        this._stiffnessTensorError[4, 0] = value;
                        break;
                    default:
                        this._stiffnessTensorError[0, 4] = 0;
                        this._stiffnessTensorError[4, 0] = 0;
                        break;
                }
            }
        }

        public double C52
        {
            get
            {
                return this._stiffnessTensor[4, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._stiffnessTensor[0, 4] = -1 * value;
                        this._stiffnessTensor[1, 4] = value;
                        this._stiffnessTensor[3, 5] = value;
                        this._stiffnessTensor[4, 0] = -1 * value;
                        this._stiffnessTensor[4, 1] = value;
                        this._stiffnessTensor[5, 3] = value;
                        break;
                    case 9:
                        this._stiffnessTensor[1, 4] = value;
                        this._stiffnessTensor[4, 1] = value;
                        break;
                    default:
                        this._stiffnessTensor[1, 4] = 0;
                        this._stiffnessTensor[4, 1] = 0;
                        break;
                }
            }
        }
        public double C52Error
        {
            get
            {
                return this._stiffnessTensorError[4, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._stiffnessTensorError[0, 4] = -1 * value;
                        this._stiffnessTensorError[1, 4] = value;
                        this._stiffnessTensorError[3, 5] = value;
                        this._stiffnessTensorError[4, 0] = -1 * value;
                        this._stiffnessTensorError[4, 1] = value;
                        this._stiffnessTensorError[5, 3] = value;
                        break;
                    case 9:
                        this._stiffnessTensorError[1, 4] = value;
                        this._stiffnessTensorError[4, 1] = value;
                        break;
                    default:
                        this._stiffnessTensorError[1, 4] = 0;
                        this._stiffnessTensorError[4, 1] = 0;
                        break;
                }
            }
        }

        public double C53
        {
            get
            {
                return this._stiffnessTensor[4, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 9:
                        this._stiffnessTensor[2, 4] = value;
                        this._stiffnessTensor[4, 2] = value;
                        break;
                    default:
                        this._stiffnessTensor[2, 4] = 0;
                        this._stiffnessTensor[4, 2] = 0;
                        break;
                }
            }
        }
        public double C53Error
        {
            get
            {
                return this._stiffnessTensorError[4, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 9:
                        this._stiffnessTensorError[2, 4] = value;
                        this._stiffnessTensorError[4, 2] = value;
                        break;
                    default:
                        this._stiffnessTensorError[2, 4] = 0;
                        this._stiffnessTensorError[4, 2] = 0;
                        break;
                }
            }
        }

        public double C54
        {
            get
            {
                return this._stiffnessTensor[4, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 8:
                        this._stiffnessTensor[3, 4] = value;
                        this._stiffnessTensor[4, 3] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._stiffnessTensor[3, 4] = 0;
                        this._stiffnessTensor[4, 3] = 0;
                        break;
                }
            }
        }
        public double C54Error
        {
            get
            {
                return this._stiffnessTensorError[4, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 8:
                        this._stiffnessTensorError[3, 4] = value;
                        this._stiffnessTensorError[4, 3] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._stiffnessTensorError[3, 4] = 0;
                        this._stiffnessTensorError[4, 3] = 0;
                        break;
                }
            }
        }

        public double C55
        {
            get
            {
                return this._stiffnessTensor[4, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        if (!this.IsIsotropic)
                        {
                            this._stiffnessTensor[3, 3] = value;
                            this._stiffnessTensor[4, 4] = value;
                            this._stiffnessTensor[5, 5] = value;
                        }
                        break;
                    case 2:
                        this._stiffnessTensor[3, 3] = value;
                        this._stiffnessTensor[4, 4] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._stiffnessTensor[4, 4] = value;
                        break;
                }
            }
        }
        public double C55Error
        {
            get
            {
                return this._stiffnessTensorError[4, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        if (!this.IsIsotropic)
                        {
                            this._stiffnessTensorError[3, 3] = value;
                            this._stiffnessTensorError[4, 4] = value;
                            this._stiffnessTensorError[5, 5] = value;
                        }
                        break;
                    case 2:
                        this._stiffnessTensorError[3, 3] = value;
                        this._stiffnessTensorError[4, 4] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._stiffnessTensorError[4, 4] = value;
                        break;
                }
            }
        }

        public double C56
        {
            get
            {
                return this._stiffnessTensor[4, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._stiffnessTensor[0, 3] = value;
                        this._stiffnessTensor[1, 3] = -1 * value;
                        this._stiffnessTensor[4, 5] = value;
                        this._stiffnessTensor[3, 0] = value;
                        this._stiffnessTensor[3, 1] = -1 * value;
                        this._stiffnessTensor[5, 4] = value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._stiffnessTensor[4, 5] = value;
                        this._stiffnessTensor[5, 4] = value;
                        break;
                    default:
                        this._stiffnessTensor[4, 5] = 0;
                        this._stiffnessTensor[5, 4] = 0;
                        break;
                }
            }
        }
        public double C56Error
        {
            get
            {
                return this._stiffnessTensorError[4, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._stiffnessTensorError[0, 3] = value;
                        this._stiffnessTensorError[1, 3] = -1 * value;
                        this._stiffnessTensorError[4, 5] = value;
                        this._stiffnessTensorError[3, 0] = value;
                        this._stiffnessTensorError[3, 1] = -1 * value;
                        this._stiffnessTensorError[5, 4] = value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._stiffnessTensorError[4, 5] = value;
                        this._stiffnessTensorError[5, 4] = value;
                        break;
                    default:
                        this._stiffnessTensorError[4, 5] = 0;
                        this._stiffnessTensorError[5, 4] = 0;
                        break;
                }
            }
        }

        public double C61
        {
            get
            {
                return this._stiffnessTensor[5, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 4:
                        this._stiffnessTensor[0, 5] = value;
                        this._stiffnessTensor[1, 5] = -1 * value;
                        this._stiffnessTensor[5, 0] = value;
                        this._stiffnessTensor[5, 1] = -1 * value;
                        break;
                    case 8:
                        this._stiffnessTensor[0, 5] = value;
                        this._stiffnessTensor[5, 0] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._stiffnessTensor[0, 5] = 0;
                        this._stiffnessTensor[5, 0] = 0;
                        break;
                }
            }
        }
        public double C61Error
        {
            get
            {
                return this._stiffnessTensorError[5, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 4:
                        this._stiffnessTensorError[0, 5] = value;
                        this._stiffnessTensorError[1, 5] = -1 * value;
                        this._stiffnessTensorError[5, 0] = value;
                        this._stiffnessTensorError[5, 1] = -1 * value;
                        break;
                    case 8:
                        this._stiffnessTensorError[0, 5] = value;
                        this._stiffnessTensorError[5, 0] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._stiffnessTensorError[0, 5] = 0;
                        this._stiffnessTensorError[5, 0] = 0;
                        break;
                }
            }
        }

        public double C62
        {
            get
            {
                return this._stiffnessTensor[5, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 4:
                        this._stiffnessTensor[0, 5] = -1 * value;
                        this._stiffnessTensor[1, 5] = value;
                        this._stiffnessTensor[5, 0] = -1 * value;
                        this._stiffnessTensor[5, 1] = value;
                        break;
                    case 8:
                        this._stiffnessTensor[1, 5] = value;
                        this._stiffnessTensor[5, 1] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._stiffnessTensor[1, 5] = 0;
                        this._stiffnessTensor[5, 1] = 0;
                        break;
                }
            }
        }
        public double C62Error
        {
            get
            {
                return this._stiffnessTensorError[5, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 4:
                        this._stiffnessTensorError[0, 5] = -1 * value;
                        this._stiffnessTensorError[1, 5] = value;
                        this._stiffnessTensorError[5, 0] = -1 * value;
                        this._stiffnessTensorError[5, 1] = value;
                        break;
                    case 8:
                        this._stiffnessTensorError[1, 5] = value;
                        this._stiffnessTensorError[5, 1] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._stiffnessTensorError[1, 5] = 0;
                        this._stiffnessTensorError[5, 1] = 0;
                        break;
                }
            }
        }

        public double C63
        {
            get
            {
                return this._stiffnessTensor[5, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 8:
                        this._stiffnessTensor[2, 5] = value;
                        this._stiffnessTensor[5, 2] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._stiffnessTensor[2, 5] = 0;
                        this._stiffnessTensor[5, 2] = 0;
                        break;
                }
            }
        }
        public double C63Error
        {
            get
            {
                return this._stiffnessTensorError[5, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 8:
                        this._stiffnessTensorError[2, 5] = value;
                        this._stiffnessTensorError[5, 2] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._stiffnessTensorError[2, 5] = 0;
                        this._stiffnessTensorError[5, 2] = 0;
                        break;
                }
            }
        }

        public double C64
        {
            get
            {
                return this._stiffnessTensor[5, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._stiffnessTensor[0, 4] = -1 * value;
                        this._stiffnessTensor[1, 4] = value;
                        this._stiffnessTensor[3, 5] = value;
                        this._stiffnessTensor[4, 0] = -1 * value;
                        this._stiffnessTensor[4, 1] = value;
                        this._stiffnessTensor[5, 3] = value;
                        break;
                    case 9:
                        this._stiffnessTensor[3, 5] = value;
                        this._stiffnessTensor[5, 3] = value;
                        break;
                    default:
                        this._stiffnessTensor[3, 5] = 0;
                        this._stiffnessTensor[5, 3] = 0;
                        break;
                }
            }
        }
        public double C64Error
        {
            get
            {
                return this._stiffnessTensorError[5, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._stiffnessTensorError[0, 4] = -1 * value;
                        this._stiffnessTensorError[1, 4] = value;
                        this._stiffnessTensorError[3, 5] = value;
                        this._stiffnessTensorError[4, 0] = -1 * value;
                        this._stiffnessTensorError[4, 1] = value;
                        this._stiffnessTensorError[5, 3] = value;
                        break;
                    case 9:
                        this._stiffnessTensorError[3, 5] = value;
                        this._stiffnessTensorError[5, 3] = value;
                        break;
                    default:
                        this._stiffnessTensorError[3, 5] = 0;
                        this._stiffnessTensorError[5, 3] = 0;
                        break;
                }
            }
        }

        public double C65
        {
            get
            {
                return this._stiffnessTensor[5, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._stiffnessTensor[0, 3] = value;
                        this._stiffnessTensor[1, 3] = -1 * value;
                        this._stiffnessTensor[4, 5] = value;
                        this._stiffnessTensor[3, 0] = value;
                        this._stiffnessTensor[3, 1] = -1 * value;
                        this._stiffnessTensor[5, 4] = value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._stiffnessTensor[4, 5] = value;
                        this._stiffnessTensor[5, 4] = value;
                        break;
                    default:
                        this._stiffnessTensor[4, 5] = 0;
                        this._stiffnessTensor[5, 4] = 0;
                        break;
                }
            }
        }
        public double C65Error
        {
            get
            {
                return this._stiffnessTensorError[5, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._stiffnessTensorError[0, 3] = value;
                        this._stiffnessTensorError[1, 3] = -1 * value;
                        this._stiffnessTensorError[4, 5] = value;
                        this._stiffnessTensorError[3, 0] = value;
                        this._stiffnessTensorError[3, 1] = -1 * value;
                        this._stiffnessTensorError[5, 4] = value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._stiffnessTensorError[4, 5] = value;
                        this._stiffnessTensorError[5, 4] = value;
                        break;
                    default:
                        this._stiffnessTensorError[4, 5] = 0;
                        this._stiffnessTensorError[5, 4] = 0;
                        break;
                }
            }
        }

        public double C66
        {
            get
            {
                return this._stiffnessTensor[5, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        if (!this.IsIsotropic)
                        {
                            this._stiffnessTensor[3, 3] = value;
                            this._stiffnessTensor[4, 4] = value;
                            this._stiffnessTensor[5, 5] = value;
                        }
                        break;
                    case 2:
                        break;
                    case 5:
                        break;
                    case 6:
                        break;
                    default:
                        this._stiffnessTensor[5, 5] = value;
                        break;
                }
            }
        }
        public double C66Error
        {
            get
            {
                return this._stiffnessTensorError[5, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        if (!this.IsIsotropic)
                        {
                            this._stiffnessTensorError[3, 3] = value;
                            this._stiffnessTensorError[4, 4] = value;
                            this._stiffnessTensorError[5, 5] = value;
                        }
                        break;
                    case 2:
                        break;
                    case 5:
                        break;
                    case 6:
                        break;
                    default:
                        this._stiffnessTensorError[5, 5] = value;
                        break;
                }
            }
        }

        #endregion

        #region Compliance matrix components

        public double S11
        {
            get
            {
                return this._complianceTensor[0, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._complianceTensor[0, 0] = value;
                        this._complianceTensor[1, 1] = value;
                        this._complianceTensor[2, 2] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 2 * (this._complianceTensor[0, 0] - this._complianceTensor[0, 1]);
                            this._complianceTensor[3, 3] = IsoValue;
                            this._complianceTensor[4, 4] = IsoValue;
                            this._complianceTensor[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        double IsoValue1 = 2 * (value - this._complianceTensor[0, 1]);
                        this._complianceTensor[5, 5] = IsoValue1;
                        goto case 3;
                    case 3:
                        this._complianceTensor[0, 0] = value;
                        this._complianceTensor[1, 1] = value;
                        break;
                    case 4:
                        goto case 3;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._complianceTensor[0, 0] = value;
                        break;
                }
            }
        }
        public double S11Error
        {
            get
            {
                return this._complianceTensorError[0, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._complianceTensorError[0, 0] = value;
                        this._complianceTensorError[1, 1] = value;
                        this._complianceTensorError[2, 2] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 2 * (this._complianceTensorError[0, 0] - this._complianceTensorError[0, 1]);
                            this._complianceTensorError[3, 3] = IsoValue;
                            this._complianceTensorError[4, 4] = IsoValue;
                            this._complianceTensorError[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        double IsoValue1 = 2 * (value - this._complianceTensorError[0, 1]);
                        this._complianceTensorError[5, 5] = IsoValue1;
                        goto case 3;
                    case 3:
                        this._complianceTensorError[0, 0] = value;
                        this._complianceTensorError[1, 1] = value;
                        break;
                    case 4:
                        goto case 3;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._complianceTensorError[0, 0] = value;
                        break;
                }
            }
        }

        public double S12
        {
            get
            {
                return this._complianceTensor[0, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._complianceTensor[0, 1] = value;
                        this._complianceTensor[0, 2] = value;
                        this._complianceTensor[1, 2] = value;
                        this._complianceTensor[1, 0] = value;
                        this._complianceTensor[2, 0] = value;
                        this._complianceTensor[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 2 * (this._complianceTensor[0, 0] - this._complianceTensor[0, 1]);
                            this._complianceTensor[3, 3] = IsoValue;
                            this._complianceTensor[4, 4] = IsoValue;
                            this._complianceTensor[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        double IsoValue1 = 2 * (this._complianceTensor[0, 0] - value);
                        this._complianceTensor[5, 5] = IsoValue1;
                        goto default;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._complianceTensor[0, 1] = value;
                        this._complianceTensor[1, 0] = value;
                        break;
                }
            }
        }
        public double S12Error
        {
            get
            {
                return this._complianceTensorError[0, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._complianceTensorError[0, 1] = value;
                        this._complianceTensorError[0, 2] = value;
                        this._complianceTensorError[1, 2] = value;
                        this._complianceTensorError[1, 0] = value;
                        this._complianceTensorError[2, 0] = value;
                        this._complianceTensorError[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 2 * (this._complianceTensorError[0, 0] - this._complianceTensorError[0, 1]);
                            this._complianceTensorError[3, 3] = IsoValue;
                            this._complianceTensorError[4, 4] = IsoValue;
                            this._complianceTensorError[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        double IsoValue1 = 2 * (this._complianceTensorError[0, 0] - value);
                        this._complianceTensorError[5, 5] = IsoValue1;
                        goto default;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._complianceTensorError[0, 1] = value;
                        this._complianceTensorError[1, 0] = value;
                        break;
                }
            }
        }

        public double S13
        {
            get
            {
                return this._complianceTensor[0, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._complianceTensor[0, 1] = value;
                        this._complianceTensor[0, 2] = value;
                        this._complianceTensor[1, 2] = value;
                        this._complianceTensor[1, 0] = value;
                        this._complianceTensor[2, 0] = value;
                        this._complianceTensor[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 2 * (this._complianceTensor[0, 0] - this._complianceTensor[0, 1]);
                            this._complianceTensor[3, 3] = IsoValue;
                            this._complianceTensor[4, 4] = IsoValue;
                            this._complianceTensor[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        this._complianceTensor[0, 2] = value;
                        this._complianceTensor[1, 2] = value;
                        this._complianceTensor[2, 0] = value;
                        this._complianceTensor[2, 1] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._complianceTensor[0, 2] = value;
                        this._complianceTensor[2, 0] = value;
                        break;
                }
            }
        }
        public double S13Error
        {
            get
            {
                return this._complianceTensorError[0, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._complianceTensorError[0, 1] = value;
                        this._complianceTensorError[0, 2] = value;
                        this._complianceTensorError[1, 2] = value;
                        this._complianceTensorError[1, 0] = value;
                        this._complianceTensorError[2, 0] = value;
                        this._complianceTensorError[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 2 * (this._complianceTensorError[0, 0] - this._complianceTensorError[0, 1]);
                            this._complianceTensorError[3, 3] = IsoValue;
                            this._complianceTensorError[4, 4] = IsoValue;
                            this._complianceTensorError[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        this._complianceTensorError[0, 2] = value;
                        this._complianceTensorError[1, 2] = value;
                        this._complianceTensorError[2, 0] = value;
                        this._complianceTensorError[2, 1] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._complianceTensorError[0, 2] = value;
                        this._complianceTensorError[2, 0] = value;
                        break;
                }
            }
        }

        public double S14
        {
            get
            {
                return this._complianceTensor[0, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._complianceTensor[0, 3] = value;
                        this._complianceTensor[1, 3] = -1 * value;
                        this._complianceTensor[4, 5] = 0.5 * value;
                        this._complianceTensor[3, 0] = value;
                        this._complianceTensor[3, 1] = -1 * value;
                        this._complianceTensor[5, 4] = 0.5 * value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._complianceTensor[0, 3] = value;
                        this._complianceTensor[3, 0] = value;
                        break;
                    default:
                        this._complianceTensor[0, 3] = 0;
                        this._complianceTensor[3, 0] = 0;
                        break;
                }
            }
        }
        public double S14Error
        {
            get
            {
                return this._complianceTensorError[0, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._complianceTensorError[0, 3] = value;
                        this._complianceTensorError[1, 3] = -1 * value;
                        this._complianceTensorError[4, 5] = 0.5 * value;
                        this._complianceTensorError[3, 0] = value;
                        this._complianceTensorError[3, 1] = -1 * value;
                        this._complianceTensorError[5, 4] = 0.5 * value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._complianceTensorError[0, 3] = value;
                        this._complianceTensorError[3, 0] = value;
                        break;
                    default:
                        this._complianceTensorError[0, 3] = 0;
                        this._complianceTensorError[3, 0] = 0;
                        break;
                }
            }
        }

        public double S15
        {
            get
            {
                return this._complianceTensor[0, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._complianceTensor[0, 4] = value;
                        this._complianceTensor[1, 4] = -1 * value;
                        this._complianceTensor[3, 5] = -0.5 * value;
                        this._complianceTensor[4, 0] = value;
                        this._complianceTensor[4, 1] = -1 * value;
                        this._complianceTensor[5, 3] = -0.5 * value;
                        break;
                    case 9:
                        this._complianceTensor[0, 4] = value;
                        this._complianceTensor[4, 0] = value;
                        break;
                    default:
                        this._complianceTensor[0, 4] = 0;
                        this._complianceTensor[4, 0] = 0;
                        break;
                }
            }
        }
        public double S15Error
        {
            get
            {
                return this._complianceTensorError[0, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._complianceTensorError[0, 4] = value;
                        this._complianceTensorError[1, 4] = -1 * value;
                        this._complianceTensorError[3, 5] = -0.5 * value;
                        this._complianceTensorError[4, 0] = value;
                        this._complianceTensorError[4, 1] = -1 * value;
                        this._complianceTensorError[5, 3] = -0.5 * value;
                        break;
                    case 9:
                        this._complianceTensorError[0, 4] = value;
                        this._complianceTensorError[4, 0] = value;
                        break;
                    default:
                        this._complianceTensorError[0, 4] = 0;
                        this._complianceTensorError[4, 0] = 0;
                        break;
                }
            }
        }

        public double S16
        {
            get
            {
                return this._complianceTensor[0, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 4:
                        this._complianceTensor[0, 5] = value;
                        this._complianceTensor[1, 5] = -1 * value;
                        this._complianceTensor[5, 0] = value;
                        this._complianceTensor[5, 1] = -1 * value;
                        break;
                    case 8:
                        this._complianceTensor[0, 5] = value;
                        this._complianceTensor[5, 0] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._complianceTensor[0, 5] = 0;
                        this._complianceTensor[5, 0] = 0;
                        break;
                }
            }
        }
        public double S16Error
        {
            get
            {
                return this._complianceTensorError[0, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 4:
                        this._complianceTensorError[0, 5] = value;
                        this._complianceTensorError[1, 5] = -1 * value;
                        this._complianceTensorError[5, 0] = value;
                        this._complianceTensorError[5, 1] = -1 * value;
                        break;
                    case 8:
                        this._complianceTensorError[0, 5] = value;
                        this._complianceTensorError[5, 0] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._complianceTensorError[0, 5] = 0;
                        this._complianceTensorError[5, 0] = 0;
                        break;
                }
            }
        }

        public double S21
        {
            get
            {
                return this._complianceTensor[1, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._complianceTensor[0, 1] = value;
                        this._complianceTensor[0, 2] = value;
                        this._complianceTensor[1, 2] = value;
                        this._complianceTensor[1, 0] = value;
                        this._complianceTensor[2, 0] = value;
                        this._complianceTensor[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 2 * (this._complianceTensor[0, 0] - this._complianceTensor[0, 1]);
                            this._complianceTensor[3, 3] = IsoValue;
                            this._complianceTensor[4, 4] = IsoValue;
                            this._complianceTensor[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        double IsoValue1 = 2 * (this._complianceTensor[0, 0] - value);
                        this._complianceTensor[5, 5] = IsoValue1;
                        goto default;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._complianceTensor[0, 1] = value;
                        this._complianceTensor[1, 0] = value;
                        break;
                }
            }
        }
        public double S21Error
        {
            get
            {
                return this._complianceTensorError[1, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._complianceTensorError[0, 1] = value;
                        this._complianceTensorError[0, 2] = value;
                        this._complianceTensorError[1, 2] = value;
                        this._complianceTensorError[1, 0] = value;
                        this._complianceTensorError[2, 0] = value;
                        this._complianceTensorError[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 2 * (this._complianceTensorError[0, 0] - this._complianceTensorError[0, 1]);
                            this._complianceTensorError[3, 3] = IsoValue;
                            this._complianceTensorError[4, 4] = IsoValue;
                            this._complianceTensorError[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        double IsoValue1 = 2 * (this._complianceTensorError[0, 0] - value);
                        this._complianceTensorError[5, 5] = IsoValue1;
                        goto default;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._complianceTensorError[0, 1] = value;
                        this._complianceTensorError[1, 0] = value;
                        break;
                }
            }
        }

        public double S22
        {
            get
            {
                return this._complianceTensor[1, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._complianceTensor[0, 0] = value;
                        this._complianceTensor[1, 1] = value;
                        this._complianceTensor[2, 2] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 2 * (this._complianceTensor[0, 0] - this._complianceTensor[0, 1]);
                            this._complianceTensor[3, 3] = IsoValue;
                            this._complianceTensor[4, 4] = IsoValue;
                            this._complianceTensor[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        double IsoValue1 = 2 * (value - this._complianceTensor[0, 1]);
                        this._complianceTensor[5, 5] = IsoValue1;
                        goto case 3;
                    case 3:
                        this._complianceTensor[0, 0] = value;
                        this._complianceTensor[1, 1] = value;
                        break;
                    case 4:
                        goto case 3;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._complianceTensor[1, 1] = value;
                        break;
                }
            }
        }
        public double S22Error
        {
            get
            {
                return this._complianceTensorError[1, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._complianceTensorError[0, 0] = value;
                        this._complianceTensorError[1, 1] = value;
                        this._complianceTensorError[2, 2] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 2 * (this._complianceTensorError[0, 0] - this._complianceTensorError[0, 1]);
                            this._complianceTensorError[3, 3] = IsoValue;
                            this._complianceTensorError[4, 4] = IsoValue;
                            this._complianceTensorError[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        double IsoValue1 = 2 * (value - this._complianceTensorError[0, 1]);
                        this._complianceTensorError[5, 5] = IsoValue1;
                        goto case 3;
                    case 3:
                        this._complianceTensorError[0, 0] = value;
                        this._complianceTensorError[1, 1] = value;
                        break;
                    case 4:
                        goto case 3;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._complianceTensorError[1, 1] = value;
                        break;
                }
            }
        }

        public double S23
        {
            get
            {
                return this._complianceTensor[1, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._complianceTensor[0, 1] = value;
                        this._complianceTensor[0, 2] = value;
                        this._complianceTensor[1, 2] = value;
                        this._complianceTensor[1, 0] = value;
                        this._complianceTensor[2, 0] = value;
                        this._complianceTensor[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 2 * (this._complianceTensor[0, 0] - this._complianceTensor[0, 1]);
                            this._complianceTensor[3, 3] = IsoValue;
                            this._complianceTensor[4, 4] = IsoValue;
                            this._complianceTensor[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        this._complianceTensor[0, 2] = value;
                        this._complianceTensor[1, 2] = value;
                        this._complianceTensor[2, 0] = value;
                        this._complianceTensor[2, 1] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._complianceTensor[1, 2] = value;
                        this._complianceTensor[2, 1] = value;
                        break;
                }
            }
        }
        public double S23Error
        {
            get
            {
                return this._complianceTensorError[1, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._complianceTensorError[0, 1] = value;
                        this._complianceTensorError[0, 2] = value;
                        this._complianceTensorError[1, 2] = value;
                        this._complianceTensorError[1, 0] = value;
                        this._complianceTensorError[2, 0] = value;
                        this._complianceTensorError[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 2 * (this._complianceTensorError[0, 0] - this._complianceTensorError[0, 1]);
                            this._complianceTensorError[3, 3] = IsoValue;
                            this._complianceTensorError[4, 4] = IsoValue;
                            this._complianceTensorError[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        this._complianceTensorError[0, 2] = value;
                        this._complianceTensorError[1, 2] = value;
                        this._complianceTensorError[2, 0] = value;
                        this._complianceTensorError[2, 1] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._complianceTensorError[1, 2] = value;
                        this._complianceTensorError[2, 1] = value;
                        break;
                }
            }
        }

        public double S24
        {
            get
            {
                return this._complianceTensor[1, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._complianceTensor[0, 3] = -1 * value;
                        this._complianceTensor[1, 3] = value;
                        this._complianceTensor[4, 5] = -0.5 * value;
                        this._complianceTensor[3, 0] = -1 * value;
                        this._complianceTensor[3, 1] = value;
                        this._complianceTensor[5, 4] = -0.5 * value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._complianceTensor[1, 3] = value;
                        this._complianceTensor[3, 1] = value;
                        break;
                    default:
                        this._complianceTensor[1, 3] = 0;
                        this._complianceTensor[3, 1] = 0;
                        break;
                }
            }
        }
        public double S24Error
        {
            get
            {
                return this._complianceTensorError[1, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._complianceTensorError[0, 3] = -1 * value;
                        this._complianceTensorError[1, 3] = value;
                        this._complianceTensorError[4, 5] = -0.5 * value;
                        this._complianceTensorError[3, 0] = -1 * value;
                        this._complianceTensorError[3, 1] = value;
                        this._complianceTensorError[5, 4] = -0.5 * value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._complianceTensorError[1, 3] = value;
                        this._complianceTensorError[3, 1] = value;
                        break;
                    default:
                        this._complianceTensorError[1, 3] = 0;
                        this._complianceTensorError[3, 1] = 0;
                        break;
                }
            }
        }

        public double S25
        {
            get
            {
                return this._complianceTensor[1, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._complianceTensor[0, 4] = -1 * value;
                        this._complianceTensor[1, 4] = value;
                        this._complianceTensor[3, 5] = 0.5 * value;
                        this._complianceTensor[4, 0] = -1 * value;
                        this._complianceTensor[4, 1] = value;
                        this._complianceTensor[5, 3] = 0.5 * value;
                        break;
                    case 9:
                        this._complianceTensor[1, 4] = value;
                        this._complianceTensor[4, 1] = value;
                        break;
                    default:
                        this._complianceTensor[1, 4] = 0;
                        this._complianceTensor[4, 1] = 0;
                        break;
                }
            }
        }
        public double S25Error
        {
            get
            {
                return this._complianceTensorError[1, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._complianceTensorError[0, 4] = -1 * value;
                        this._complianceTensorError[1, 4] = value;
                        this._complianceTensorError[3, 5] = 0.5 * value;
                        this._complianceTensorError[4, 0] = -1 * value;
                        this._complianceTensorError[4, 1] = value;
                        this._complianceTensorError[5, 3] = 0.5 * value;
                        break;
                    case 9:
                        this._complianceTensorError[1, 4] = value;
                        this._complianceTensorError[4, 1] = value;
                        break;
                    default:
                        this._complianceTensorError[1, 4] = 0;
                        this._complianceTensorError[4, 1] = 0;
                        break;
                }
            }
        }

        public double S26
        {
            get
            {
                return this._complianceTensor[1, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 4:
                        this._complianceTensor[0, 5] = -1 * value;
                        this._complianceTensor[1, 5] = value;
                        this._complianceTensor[5, 0] = -1 * value;
                        this._complianceTensor[5, 1] = value;
                        break;
                    case 8:
                        this._complianceTensor[1, 5] = value;
                        this._complianceTensor[5, 1] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._complianceTensor[1, 5] = 0;
                        this._complianceTensor[5, 1] = 0;
                        break;
                }
            }
        }
        public double S26Error
        {
            get
            {
                return this._complianceTensorError[1, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 4:
                        this._complianceTensorError[0, 5] = -1 * value;
                        this._complianceTensorError[1, 5] = value;
                        this._complianceTensorError[5, 0] = -1 * value;
                        this._complianceTensorError[5, 1] = value;
                        break;
                    case 8:
                        this._complianceTensorError[1, 5] = value;
                        this._complianceTensorError[5, 1] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._complianceTensorError[1, 5] = 0;
                        this._complianceTensorError[5, 1] = 0;
                        break;
                }
            }
        }

        public double S31
        {
            get
            {
                return this._complianceTensor[2, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._complianceTensor[0, 1] = value;
                        this._complianceTensor[0, 2] = value;
                        this._complianceTensor[1, 2] = value;
                        this._complianceTensor[1, 0] = value;
                        this._complianceTensor[2, 0] = value;
                        this._complianceTensor[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 2 * (this._complianceTensor[0, 0] - this._complianceTensor[0, 1]);
                            this._complianceTensor[3, 3] = IsoValue;
                            this._complianceTensor[4, 4] = IsoValue;
                            this._complianceTensor[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        this._complianceTensor[0, 2] = value;
                        this._complianceTensor[1, 2] = value;
                        this._complianceTensor[2, 0] = value;
                        this._complianceTensor[2, 1] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._complianceTensor[0, 2] = value;
                        this._complianceTensor[2, 0] = value;
                        break;
                }
            }
        }
        public double S31Error
        {
            get
            {
                return this._complianceTensorError[2, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._complianceTensorError[0, 1] = value;
                        this._complianceTensorError[0, 2] = value;
                        this._complianceTensorError[1, 2] = value;
                        this._complianceTensorError[1, 0] = value;
                        this._complianceTensorError[2, 0] = value;
                        this._complianceTensorError[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 2 * (this._complianceTensorError[0, 0] - this._complianceTensorError[0, 1]);
                            this._complianceTensorError[3, 3] = IsoValue;
                            this._complianceTensorError[4, 4] = IsoValue;
                            this._complianceTensorError[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        this._complianceTensorError[0, 2] = value;
                        this._complianceTensorError[1, 2] = value;
                        this._complianceTensorError[2, 0] = value;
                        this._complianceTensorError[2, 1] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._complianceTensorError[0, 2] = value;
                        this._complianceTensorError[2, 0] = value;
                        break;
                }
            }
        }

        public double S32
        {
            get
            {
                return this._complianceTensor[2, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._complianceTensor[0, 1] = value;
                        this._complianceTensor[0, 2] = value;
                        this._complianceTensor[1, 2] = value;
                        this._complianceTensor[1, 0] = value;
                        this._complianceTensor[2, 0] = value;
                        this._complianceTensor[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 2 * (this._complianceTensor[0, 0] - this._complianceTensor[0, 1]);
                            this._complianceTensor[3, 3] = IsoValue;
                            this._complianceTensor[4, 4] = IsoValue;
                            this._complianceTensor[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        this._complianceTensor[0, 2] = value;
                        this._complianceTensor[1, 2] = value;
                        this._complianceTensor[2, 0] = value;
                        this._complianceTensor[2, 1] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._complianceTensor[1, 2] = value;
                        this._complianceTensor[2, 1] = value;
                        break;
                }
            }
        }
        public double S32Error
        {
            get
            {
                return this._complianceTensorError[2, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._complianceTensorError[0, 1] = value;
                        this._complianceTensorError[0, 2] = value;
                        this._complianceTensorError[1, 2] = value;
                        this._complianceTensorError[1, 0] = value;
                        this._complianceTensorError[2, 0] = value;
                        this._complianceTensorError[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 2 * (this._complianceTensorError[0, 0] - this._complianceTensorError[0, 1]);
                            this._complianceTensorError[3, 3] = IsoValue;
                            this._complianceTensorError[4, 4] = IsoValue;
                            this._complianceTensorError[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        this._complianceTensorError[0, 2] = value;
                        this._complianceTensorError[1, 2] = value;
                        this._complianceTensorError[2, 0] = value;
                        this._complianceTensorError[2, 1] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._complianceTensorError[1, 2] = value;
                        this._complianceTensorError[2, 1] = value;
                        break;
                }
            }
        }

        public double S33
        {
            get
            {
                return this._complianceTensor[2, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._complianceTensor[0, 0] = value;
                        this._complianceTensor[1, 1] = value;
                        this._complianceTensor[2, 2] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 2 * (this._complianceTensor[0, 0] - this._complianceTensor[0, 1]);
                            this._complianceTensor[3, 3] = IsoValue;
                            this._complianceTensor[4, 4] = IsoValue;
                            this._complianceTensor[5, 5] = IsoValue;
                        }
                        break;
                    default:
                        this._complianceTensor[2, 2] = value;
                        break;
                }
            }
        }
        public double S33Error
        {
            get
            {
                return this._complianceTensorError[2, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._complianceTensorError[0, 0] = value;
                        this._complianceTensorError[1, 1] = value;
                        this._complianceTensorError[2, 2] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 2 * (this._complianceTensorError[0, 0] - this._complianceTensorError[0, 1]);
                            this._complianceTensorError[3, 3] = IsoValue;
                            this._complianceTensorError[4, 4] = IsoValue;
                            this._complianceTensorError[5, 5] = IsoValue;
                        }
                        break;
                    default:
                        this._complianceTensorError[2, 2] = value;
                        break;
                }
            }
        }

        public double S34
        {
            get
            {
                return this._complianceTensor[2, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 9:
                        this._complianceTensor[2, 3] = value;
                        this._complianceTensor[3, 2] = value;
                        break;
                    default:
                        this._complianceTensor[2, 3] = 0;
                        this._complianceTensor[3, 2] = 0;
                        break;
                }
            }
        }
        public double S34Error
        {
            get
            {
                return this._complianceTensorError[2, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 9:
                        this._complianceTensorError[2, 3] = value;
                        this._complianceTensorError[3, 2] = value;
                        break;
                    default:
                        this._complianceTensorError[2, 3] = 0;
                        this._complianceTensorError[3, 2] = 0;
                        break;
                }
            }
        }

        public double S35
        {
            get
            {
                return this._complianceTensor[2, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 9:
                        this._complianceTensor[2, 4] = value;
                        this._complianceTensor[4, 2] = value;
                        break;
                    default:
                        this._complianceTensor[2, 4] = 0;
                        this._complianceTensor[4, 2] = 0;
                        break;
                }
            }
        }
        public double S35Error
        {
            get
            {
                return this._complianceTensorError[2, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 9:
                        this._complianceTensorError[2, 4] = value;
                        this._complianceTensorError[4, 2] = value;
                        break;
                    default:
                        this._complianceTensorError[2, 4] = 0;
                        this._complianceTensorError[4, 2] = 0;
                        break;
                }
            }
        }

        public double S36
        {
            get
            {
                return this._complianceTensor[2, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 8:
                        this._complianceTensor[2, 5] = value;
                        this._complianceTensor[5, 2] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._complianceTensor[2, 5] = 0;
                        this._complianceTensor[5, 2] = 0;
                        break;
                }
            }
        }
        public double S36Error
        {
            get
            {
                return this._complianceTensorError[2, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 8:
                        this._complianceTensorError[2, 5] = value;
                        this._complianceTensorError[5, 2] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._complianceTensorError[2, 5] = 0;
                        this._complianceTensorError[5, 2] = 0;
                        break;
                }
            }
        }

        public double S41
        {
            get
            {
                return this._complianceTensor[3, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._complianceTensor[0, 3] = value;
                        this._complianceTensor[1, 3] = -1 * value;
                        this._complianceTensor[4, 5] = 0.5 * value;
                        this._complianceTensor[3, 0] = value;
                        this._complianceTensor[3, 1] = -1 * value;
                        this._complianceTensor[5, 4] = 0.5 * value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._complianceTensor[0, 3] = value;
                        this._complianceTensor[3, 0] = value;
                        break;
                    default:
                        this._complianceTensor[0, 3] = 0;
                        this._complianceTensor[3, 0] = 0;
                        break;
                }
            }
        }
        public double S41Error
        {
            get
            {
                return this._complianceTensorError[3, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._complianceTensorError[0, 3] = value;
                        this._complianceTensorError[1, 3] = -1 * value;
                        this._complianceTensorError[4, 5] = 0.5 * value;
                        this._complianceTensorError[3, 0] = value;
                        this._complianceTensorError[3, 1] = -1 * value;
                        this._complianceTensorError[5, 4] = 0.5 * value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._complianceTensorError[0, 3] = value;
                        this._complianceTensorError[3, 0] = value;
                        break;
                    default:
                        this._complianceTensorError[0, 3] = 0;
                        this._complianceTensorError[3, 0] = 0;
                        break;
                }
            }
        }

        public double S42
        {
            get
            {
                return this._complianceTensor[3, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._complianceTensor[0, 3] = -1 * value;
                        this._complianceTensor[1, 3] = value;
                        this._complianceTensor[4, 5] = -0.5 * value;
                        this._complianceTensor[3, 0] = -1 * value;
                        this._complianceTensor[3, 1] = value;
                        this._complianceTensor[5, 4] = -0.5 * value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._complianceTensor[1, 3] = value;
                        this._complianceTensor[3, 1] = value;
                        break;
                    default:
                        this._complianceTensor[1, 3] = 0;
                        this._complianceTensor[3, 1] = 0;
                        break;
                }
            }
        }
        public double S42Error
        {
            get
            {
                return this._complianceTensorError[3, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._complianceTensorError[0, 3] = -1 * value;
                        this._complianceTensorError[1, 3] = value;
                        this._complianceTensorError[4, 5] = -0.5 * value;
                        this._complianceTensorError[3, 0] = -1 * value;
                        this._complianceTensorError[3, 1] = value;
                        this._complianceTensorError[5, 4] = -0.5 * value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._complianceTensorError[1, 3] = value;
                        this._complianceTensorError[3, 1] = value;
                        break;
                    default:
                        this._complianceTensorError[1, 3] = 0;
                        this._complianceTensorError[3, 1] = 0;
                        break;
                }
            }
        }

        public double S43
        {
            get
            {
                return this._complianceTensor[3, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 9:
                        this._complianceTensor[2, 3] = value;
                        this._complianceTensor[3, 2] = value;
                        break;
                    default:
                        this._complianceTensor[2, 3] = 0;
                        this._complianceTensor[3, 2] = 0;
                        break;
                }
            }
        }
        public double S43Error
        {
            get
            {
                return this._complianceTensorError[3, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 9:
                        this._complianceTensorError[2, 3] = value;
                        this._complianceTensorError[3, 2] = value;
                        break;
                    default:
                        this._complianceTensorError[2, 3] = 0;
                        this._complianceTensorError[3, 2] = 0;
                        break;
                }
            }
        }

        public double S44
        {
            get
            {
                return this._complianceTensor[3, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        if (!this.IsIsotropic)
                        {
                            this._complianceTensor[3, 3] = value;
                            this._complianceTensor[4, 4] = value;
                            this._complianceTensor[5, 5] = value;
                        }
                        break;
                    case 2:
                        this._complianceTensor[3, 3] = value;
                        this._complianceTensor[4, 4] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._complianceTensor[3, 3] = value;
                        break;
                }
            }
        }
        public double S44Error
        {
            get
            {
                return this._complianceTensorError[3, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        if (!this.IsIsotropic)
                        {
                            this._complianceTensorError[3, 3] = value;
                            this._complianceTensorError[4, 4] = value;
                            this._complianceTensorError[5, 5] = value;
                        }
                        break;
                    case 2:
                        this._complianceTensorError[3, 3] = value;
                        this._complianceTensorError[4, 4] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._complianceTensorError[3, 3] = value;
                        break;
                }
            }
        }

        public double S45
        {
            get
            {
                return this._complianceTensor[3, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 8:
                        this._complianceTensor[3, 4] = value;
                        this._complianceTensor[4, 3] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._complianceTensor[3, 4] = 0;
                        this._complianceTensor[4, 3] = 0;
                        break;
                }
            }
        }
        public double S45Error
        {
            get
            {
                return this._complianceTensorError[3, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 8:
                        this._complianceTensorError[3, 4] = value;
                        this._complianceTensorError[4, 3] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._complianceTensorError[3, 4] = 0;
                        this._complianceTensorError[4, 3] = 0;
                        break;
                }
            }
        }

        public double S46
        {
            get
            {
                return this._complianceTensor[3, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._complianceTensor[0, 4] = -2 * value;
                        this._complianceTensor[1, 4] = 2 * value;
                        this._complianceTensor[3, 5] = value;
                        this._complianceTensor[4, 0] = -2 * value;
                        this._complianceTensor[4, 1] = 2 * value;
                        this._complianceTensor[5, 3] = value;
                        break;
                    case 9:
                        this._complianceTensor[3, 5] = value;
                        this._complianceTensor[5, 3] = value;
                        break;
                    default:
                        this._complianceTensor[3, 5] = 0;
                        this._complianceTensor[5, 3] = 0;
                        break;
                }
            }
        }
        public double S46Error
        {
            get
            {
                return this._complianceTensorError[3, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._complianceTensorError[0, 4] = -2 * value;
                        this._complianceTensorError[1, 4] = 2 * value;
                        this._complianceTensorError[3, 5] = value;
                        this._complianceTensorError[4, 0] = -2 * value;
                        this._complianceTensorError[4, 1] = 2 * value;
                        this._complianceTensorError[5, 3] = value;
                        break;
                    case 9:
                        this._complianceTensorError[3, 5] = value;
                        this._complianceTensorError[5, 3] = value;
                        break;
                    default:
                        this._complianceTensorError[3, 5] = 0;
                        this._complianceTensorError[5, 3] = 0;
                        break;
                }
            }
        }

        public double S51
        {
            get
            {
                return this._complianceTensor[4, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._complianceTensor[0, 4] = value;
                        this._complianceTensor[1, 4] = -1 * value;
                        this._complianceTensor[3, 5] = -0.5 * value;
                        this._complianceTensor[4, 0] = value;
                        this._complianceTensor[4, 1] = -1 * value;
                        this._complianceTensor[5, 3] = -0.5 * value;
                        break;
                    case 9:
                        this._complianceTensor[0, 4] = value;
                        this._complianceTensor[4, 0] = value;
                        break;
                    default:
                        this._complianceTensor[0, 4] = 0;
                        this._complianceTensor[4, 0] = 0;
                        break;
                }
            }
        }
        public double S51Error
        {
            get
            {
                return this._complianceTensorError[4, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._complianceTensorError[0, 4] = value;
                        this._complianceTensorError[1, 4] = -1 * value;
                        this._complianceTensorError[3, 5] = -0.5 * value;
                        this._complianceTensorError[4, 0] = value;
                        this._complianceTensorError[4, 1] = -1 * value;
                        this._complianceTensorError[5, 3] = -0.5 * value;
                        break;
                    case 9:
                        this._complianceTensorError[0, 4] = value;
                        this._complianceTensorError[4, 0] = value;
                        break;
                    default:
                        this._complianceTensorError[0, 4] = 0;
                        this._complianceTensorError[4, 0] = 0;
                        break;
                }
            }
        }

        public double S52
        {
            get
            {
                return this._complianceTensor[4, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._complianceTensor[0, 4] = -1 * value;
                        this._complianceTensor[1, 4] = value;
                        this._complianceTensor[3, 5] = 0.5 * value;
                        this._complianceTensor[4, 0] = -1 * value;
                        this._complianceTensor[4, 1] = value;
                        this._complianceTensor[5, 3] = 0.5 * value;
                        break;
                    case 9:
                        this._complianceTensor[1, 4] = value;
                        this._complianceTensor[4, 1] = value;
                        break;
                    default:
                        this._complianceTensor[1, 4] = 0;
                        this._complianceTensor[4, 1] = 0;
                        break;
                }
            }
        }
        public double S52Error
        {
            get
            {
                return this._complianceTensorError[4, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._complianceTensorError[0, 4] = -1 * value;
                        this._complianceTensorError[1, 4] = value;
                        this._complianceTensorError[3, 5] = 0.5 * value;
                        this._complianceTensorError[4, 0] = -1 * value;
                        this._complianceTensorError[4, 1] = value;
                        this._complianceTensorError[5, 3] = 0.5 * value;
                        break;
                    case 9:
                        this._complianceTensorError[1, 4] = value;
                        this._complianceTensorError[4, 1] = value;
                        break;
                    default:
                        this._complianceTensorError[1, 4] = 0;
                        this._complianceTensorError[4, 1] = 0;
                        break;
                }
            }
        }

        public double S53
        {
            get
            {
                return this._complianceTensor[4, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 9:
                        this._complianceTensor[2, 4] = value;
                        this._complianceTensor[4, 2] = value;
                        break;
                    default:
                        this._complianceTensor[2, 4] = 0;
                        this._complianceTensor[4, 2] = 0;
                        break;
                }
            }
        }
        public double S53Error
        {
            get
            {
                return this._complianceTensorError[4, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 9:
                        this._complianceTensorError[2, 4] = value;
                        this._complianceTensorError[4, 2] = value;
                        break;
                    default:
                        this._complianceTensorError[2, 4] = 0;
                        this._complianceTensorError[4, 2] = 0;
                        break;
                }
            }
        }

        public double S54
        {
            get
            {
                return this._complianceTensor[4, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 8:
                        this._complianceTensor[3, 4] = value;
                        this._complianceTensor[4, 3] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._complianceTensor[3, 4] = 0;
                        this._complianceTensor[4, 3] = 0;
                        break;
                }
            }
        }
        public double S54Error
        {
            get
            {
                return this._complianceTensorError[4, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 8:
                        this._complianceTensorError[3, 4] = value;
                        this._complianceTensorError[4, 3] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._complianceTensorError[3, 4] = 0;
                        this._complianceTensorError[4, 3] = 0;
                        break;
                }
            }
        }

        public double S55
        {
            get
            {
                return this._complianceTensor[4, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        if (!this.IsIsotropic)
                        {
                            this._complianceTensor[3, 3] = value;
                            this._complianceTensor[4, 4] = value;
                            this._complianceTensor[5, 5] = value;
                        }
                        break;
                    case 2:
                        this._complianceTensor[3, 3] = value;
                        this._complianceTensor[4, 4] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._complianceTensor[4, 4] = value;
                        break;
                }
            }
        }
        public double S55Error
        {
            get
            {
                return this._complianceTensorError[4, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        if (!this.IsIsotropic)
                        {
                            this._complianceTensorError[3, 3] = value;
                            this._complianceTensorError[4, 4] = value;
                            this._complianceTensorError[5, 5] = value;
                        }
                        break;
                    case 2:
                        this._complianceTensorError[3, 3] = value;
                        this._complianceTensorError[4, 4] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._complianceTensorError[4, 4] = value;
                        break;
                }
            }
        }

        public double S56
        {
            get
            {
                return this._complianceTensor[4, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._complianceTensor[0, 3] = 2 * value;
                        this._complianceTensor[1, 3] = -2 * value;
                        this._complianceTensor[4, 5] = value;
                        this._complianceTensor[3, 0] = 2 * value;
                        this._complianceTensor[3, 1] = -2 * value;
                        this._complianceTensor[5, 4] = value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._complianceTensor[4, 5] = value;
                        this._complianceTensor[5, 4] = value;
                        break;
                    default:
                        this._complianceTensor[4, 5] = 0;
                        this._complianceTensor[5, 4] = 0;
                        break;
                }
            }
        }
        public double S56Error
        {
            get
            {
                return this._complianceTensorError[4, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._complianceTensorError[0, 3] = 2 * value;
                        this._complianceTensorError[1, 3] = -2 * value;
                        this._complianceTensorError[4, 5] = value;
                        this._complianceTensorError[3, 0] = 2 * value;
                        this._complianceTensorError[3, 1] = -2 * value;
                        this._complianceTensorError[5, 4] = value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._complianceTensorError[4, 5] = value;
                        this._complianceTensorError[5, 4] = value;
                        break;
                    default:
                        this._complianceTensorError[4, 5] = 0;
                        this._complianceTensorError[5, 4] = 0;
                        break;
                }
            }
        }

        public double S61
        {
            get
            {
                return this._complianceTensor[5, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 4:
                        this._complianceTensor[0, 5] = value;
                        this._complianceTensor[1, 5] = -1 * value;
                        this._complianceTensor[5, 0] = value;
                        this._complianceTensor[5, 1] = -1 * value;
                        break;
                    case 8:
                        this._complianceTensor[0, 5] = value;
                        this._complianceTensor[5, 0] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._complianceTensor[0, 5] = 0;
                        this._complianceTensor[5, 0] = 0;
                        break;
                }
            }
        }
        public double S61Error
        {
            get
            {
                return this._complianceTensorError[5, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 4:
                        this._complianceTensorError[0, 5] = value;
                        this._complianceTensorError[1, 5] = -1 * value;
                        this._complianceTensorError[5, 0] = value;
                        this._complianceTensorError[5, 1] = -1 * value;
                        break;
                    case 8:
                        this._complianceTensorError[0, 5] = value;
                        this._complianceTensorError[5, 0] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._complianceTensorError[0, 5] = 0;
                        this._complianceTensorError[5, 0] = 0;
                        break;
                }
            }
        }

        public double S62
        {
            get
            {
                return this._complianceTensor[5, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 4:
                        this._complianceTensor[0, 5] = -1 * value;
                        this._complianceTensor[1, 5] = value;
                        this._complianceTensor[5, 0] = -1 * value;
                        this._complianceTensor[5, 1] = value;
                        break;
                    case 8:
                        this._complianceTensor[1, 5] = value;
                        this._complianceTensor[5, 1] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._complianceTensor[1, 5] = 0;
                        this._complianceTensor[5, 1] = 0;
                        break;
                }
            }
        }
        public double S62Error
        {
            get
            {
                return this._complianceTensorError[5, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 4:
                        this._complianceTensorError[0, 5] = -1 * value;
                        this._complianceTensorError[1, 5] = value;
                        this._complianceTensorError[5, 0] = -1 * value;
                        this._complianceTensorError[5, 1] = value;
                        break;
                    case 8:
                        this._complianceTensorError[1, 5] = value;
                        this._complianceTensorError[5, 1] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._complianceTensorError[1, 5] = 0;
                        this._complianceTensorError[5, 1] = 0;
                        break;
                }
            }
        }

        public double S63
        {
            get
            {
                return this._complianceTensor[5, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 8:
                        this._complianceTensor[2, 5] = value;
                        this._complianceTensor[5, 2] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._complianceTensor[2, 5] = 0;
                        this._complianceTensor[5, 2] = 0;
                        break;
                }
            }
        }
        public double S63Error
        {
            get
            {
                return this._complianceTensorError[5, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 8:
                        this._complianceTensorError[2, 5] = value;
                        this._complianceTensorError[5, 2] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._complianceTensorError[2, 5] = 0;
                        this._complianceTensorError[5, 2] = 0;
                        break;
                }
            }
        }

        public double S64
        {
            get
            {
                return this._complianceTensor[5, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._complianceTensor[0, 4] = -2 * value;
                        this._complianceTensor[1, 4] = 2 * value;
                        this._complianceTensor[3, 5] = value;
                        this._complianceTensor[4, 0] = -2 * value;
                        this._complianceTensor[4, 1] = 2 * value;
                        this._complianceTensor[5, 3] = value;
                        break;
                    case 9:
                        this._complianceTensor[3, 5] = value;
                        this._complianceTensor[5, 3] = value;
                        break;
                    default:
                        this._complianceTensor[3, 5] = 0;
                        this._complianceTensor[5, 3] = 0;
                        break;
                }
            }
        }
        public double S64Error
        {
            get
            {
                return this._complianceTensorError[5, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._complianceTensorError[0, 4] = -2 * value;
                        this._complianceTensorError[1, 4] = 2 * value;
                        this._complianceTensorError[3, 5] = value;
                        this._complianceTensorError[4, 0] = -2 * value;
                        this._complianceTensorError[4, 1] = 2 * value;
                        this._complianceTensorError[5, 3] = value;
                        break;
                    case 9:
                        this._complianceTensorError[3, 5] = value;
                        this._complianceTensorError[5, 3] = value;
                        break;
                    default:
                        this._complianceTensorError[3, 5] = 0;
                        this._complianceTensorError[5, 3] = 0;
                        break;
                }
            }
        }

        public double S65
        {
            get
            {
                return this._complianceTensor[5, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._complianceTensor[0, 3] = 2 * value;
                        this._complianceTensor[1, 3] = -2 * value;
                        this._complianceTensor[4, 5] = value;
                        this._complianceTensor[3, 0] = 2 * value;
                        this._complianceTensor[3, 1] = -2 * value;
                        this._complianceTensor[5, 4] = value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._complianceTensor[4, 5] = value;
                        this._complianceTensor[5, 4] = value;
                        break;
                    default:
                        this._complianceTensor[4, 5] = 0;
                        this._complianceTensor[5, 4] = 0;
                        break;
                }
            }
        }
        public double S65Error
        {
            get
            {
                return this._complianceTensorError[5, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._complianceTensorError[0, 3] = 2 * value;
                        this._complianceTensorError[1, 3] = -2 * value;
                        this._complianceTensorError[4, 5] = value;
                        this._complianceTensorError[3, 0] = 2 * value;
                        this._complianceTensorError[3, 1] = -2 * value;
                        this._complianceTensorError[5, 4] = value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._complianceTensorError[4, 5] = value;
                        this._complianceTensorError[5, 4] = value;
                        break;
                    default:
                        this._complianceTensorError[4, 5] = 0;
                        this._complianceTensorError[5, 4] = 0;
                        break;
                }
            }
        }

        public double S66
        {
            get
            {
                return this._complianceTensor[5, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        if (!this.IsIsotropic)
                        {
                            this._complianceTensor[3, 3] = value;
                            this._complianceTensor[4, 4] = value;
                            this._complianceTensor[5, 5] = value;
                        }
                        break;
                    case 2:
                        break;
                    case 5:
                        break;
                    case 6:
                        break;
                    default:
                        this._complianceTensor[5, 5] = value;
                        break;
                }
            }
        }
        public double S66Error
        {
            get
            {
                return this._complianceTensorError[5, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        if (!this.IsIsotropic)
                        {
                            this._complianceTensorError[3, 3] = value;
                            this._complianceTensorError[4, 4] = value;
                            this._complianceTensorError[5, 5] = value;
                        }
                        break;
                    case 2:
                        break;
                    case 5:
                        break;
                    case 6:
                        break;
                    default:
                        this._complianceTensorError[5, 5] = value;
                        break;
                }
            }
        }

        #endregion

        #endregion

        #endregion

        #region Calculation

        public void CalculateCompliances()
        {
            MathNet.Numerics.LinearAlgebra.Matrix<double> Inverted = this._stiffnessTensor.Inverse();

            switch (this.Symmetry)
            {
                case "cubic":
                    if(IsIsotropic)
                    {
                        this.S11 = Inverted[0, 0];
                        this.S12 = Inverted[0, 1];
                    }
                    else
                    {
                        this.S11 = Inverted[0, 0];
                        this.S12 = Inverted[0, 1];
                        this.S44 = 4 * Inverted[3, 3];
                    }
                    break;
                case "hexagonal":
                    this.S11 = Inverted[0, 0];
                    this.S33 = Inverted[2, 2];
                    this.S12 = Inverted[0, 1];
                    this.S13 = Inverted[0, 2];
                    this.S44 = 4 * Inverted[3, 3];
                    break;
                case "tetragonal type 1":
                    this.S11 = Inverted[0, 0];
                    this.S33 = Inverted[2, 2];
                    this.S12 = Inverted[0, 1];
                    this.S13 = Inverted[0, 2];
                    this.S44 = 4 * Inverted[3, 3];
                    this.S66 = 4 * Inverted[5, 5];
                    break;
                case "tetragonal type 2":
                    this.S11 = Inverted[0, 0];
                    this.S33 = Inverted[2, 2];
                    this.S12 = Inverted[0, 1];
                    this.S13 = Inverted[0, 2];
                    this.S16 = 2 * Inverted[0, 5];
                    this.S44 = 4 * Inverted[3, 3];
                    this.S66 = 4 * Inverted[5, 5];
                    break;
                case "trigonal type 1":
                    this.S11 = Inverted[0, 0];
                    this.S33 = Inverted[2, 2];
                    this.S12 = Inverted[0, 1];
                    this.S13 = Inverted[0, 2];
                    this.S14 = 2 * Inverted[0, 3];
                    this.S44 = 4 * Inverted[3, 3];
                    break;
                case "trigonal type 2":
                    this.S11 = Inverted[0, 0];
                    this.S33 = Inverted[2, 2];
                    this.S12 = Inverted[0, 1];
                    this.S13 = Inverted[0, 2];
                    this.S14 = 2 * Inverted[0, 3];
                    this.S15 = 2 * Inverted[0, 4];
                    this.S44 = 4 * Inverted[3, 3];
                    break;
                case "rhombic":
                    this.S11 = Inverted[0, 0];
                    this.S22 = Inverted[1, 1];
                    this.S33 = Inverted[2, 2];
                    this.S12 = Inverted[0, 1];
                    this.S13 = Inverted[0, 2];
                    this.S23 = Inverted[1, 2];
                    this.S44 = 4 * Inverted[3, 3];
                    this.S55 = 4 * Inverted[4, 4];
                    this.S66 = 4 * Inverted[5, 5];
                    break;
                case "monoclinic":
                    this.S11 = Inverted[0, 0];
                    this.S22 = Inverted[1, 1];
                    this.S33 = Inverted[2, 2];
                    this.S12 = Inverted[0, 1];
                    this.S13 = Inverted[0, 2];
                    this.S16 = 2 * Inverted[0, 5];
                    this.S23 = Inverted[1, 2];
                    this.S26 = 2 * Inverted[1, 5];
                    this.S36 = 2 * Inverted[2, 5];
                    this.S44 = 4 * Inverted[3, 3];
                    this.S45 = 4 * Inverted[3, 4];
                    this.S55 = 4 * Inverted[4, 4];
                    this.S66 = 4 * Inverted[5, 5];
                    break;
                case "triclinic":
                    this.S11 = Inverted[0, 0];
                    this.S12 = Inverted[0, 1];
                    this.S13 = Inverted[0, 2];
                    this.S14 = 2 * Inverted[0, 3];
                    this.S15 = 2 * Inverted[0, 4];
                    this.S16 = 2 * Inverted[0, 5];
                    this.S22 = Inverted[1, 1];
                    this.S23 = Inverted[1, 2];
                    this.S24 = 2 * Inverted[1, 3];
                    this.S25 = 2 * Inverted[1, 4];
                    this.S26 = 2 * Inverted[1, 5];
                    this.S33 = Inverted[2, 2];
                    this.S34 = 2 * Inverted[2, 3];
                    this.S35 = 2 * Inverted[2, 4];
                    this.S36 = 2 * Inverted[2, 5];
                    this.S44 = 4 * Inverted[3, 3];
                    this.S45 = 4 * Inverted[3, 4];
                    this.S46 = 4 * Inverted[3, 5];
                    this.S55 = 4 * Inverted[4, 4];
                    this.S56 = 4 * Inverted[4, 5];
                    this.S66 = 4 * Inverted[5, 5];
                    break;
                default:
                    this.S11 = Inverted[0, 0];
                    this.S12 = Inverted[0, 1];
                    this.S13 = Inverted[0, 2];
                    this.S14 = 2 * Inverted[0, 3];
                    this.S15 = 2 * Inverted[0, 4];
                    this.S16 = 2 * Inverted[0, 5];
                    this.S22 = Inverted[1, 1];
                    this.S23 = Inverted[1, 2];
                    this.S24 = 2 * Inverted[1, 3];
                    this.S25 = 2 * Inverted[1, 4];
                    this.S26 = 2 * Inverted[1, 5];
                    this.S33 = Inverted[2, 2];
                    this.S34 = 2 * Inverted[2, 3];
                    this.S35 = 2 * Inverted[2, 4];
                    this.S36 = 2 * Inverted[2, 5];
                    this.S44 = 4 * Inverted[3, 3];
                    this.S45 = 4 * Inverted[3, 4];
                    this.S46 = 4 * Inverted[3, 5];
                    this.S55 = 4 * Inverted[4, 4];
                    this.S56 = 4 * Inverted[4, 5];
                    this.S66 = 4 * Inverted[5, 5];
                    break;
            }
        }

        public void CalculateStiffnesses()
        {
            MathNet.Numerics.LinearAlgebra.Matrix<double> Inverted = createStiffnessMatrix().Inverse();

            switch (this.Symmetry)
            {
                case "cubic":
                    if (IsIsotropic)
                    {
                        this.C11 = Inverted[0, 0];
                        this.C12 = Inverted[0, 1];
                    }
                    else
                    {
                        this.C11 = Inverted[0, 0];
                        this.C12 = Inverted[0, 1];
                        this.C44 = Inverted[3, 3];
                    }
                    break;
                case "hexagonal":
                    this.C11 = Inverted[0, 0];
                    this.C33 = Inverted[2, 2];
                    this.C12 = Inverted[0, 1];
                    this.C13 = Inverted[0, 2];
                    this.C44 = Inverted[3, 3];
                    break;
                case "tetragonal type 1":
                    this.C11 = Inverted[0, 0];
                    this.C33 = Inverted[2, 2];
                    this.C12 = Inverted[0, 1];
                    this.C13 = Inverted[0, 2];
                    this.C44 = Inverted[3, 3];
                    this.C66 = Inverted[5, 5];
                    break;
                case "tetragonal type 2":
                    this.C11 = Inverted[0, 0];
                    this.C33 = Inverted[2, 2];
                    this.C12 = Inverted[0, 1];
                    this.C13 = Inverted[0, 2];
                    this.C16 = Inverted[0, 5];
                    this.C44 = Inverted[3, 3];
                    this.C66 = Inverted[5, 5];
                    break;
                case "trigonal type 1":
                    this.C11 = Inverted[0, 0];
                    this.C33 = Inverted[2, 2];
                    this.C12 = Inverted[0, 1];
                    this.C13 = Inverted[0, 2];
                    this.C14 = Inverted[0, 3];
                    this.C44 = Inverted[3, 3];
                    break;
                case "trigonal type 2":
                    this.C11 = Inverted[0, 0];
                    this.C33 = Inverted[2, 2];
                    this.C12 = Inverted[0, 1];
                    this.C13 = Inverted[0, 2];
                    this.C14 = Inverted[0, 3];
                    this.C15 = Inverted[0, 4];
                    this.C44 = Inverted[3, 3];
                    break;
                case "rhombic":
                    this.C11 = Inverted[0, 0];
                    this.C22 = Inverted[1, 1];
                    this.C33 = Inverted[2, 2];
                    this.C12 = Inverted[0, 1];
                    this.C13 = Inverted[0, 2];
                    this.C23 = Inverted[1, 2];
                    this.C44 = Inverted[3, 3];
                    this.C55 = Inverted[4, 4];
                    this.C66 = Inverted[5, 5];
                    break;
                case "monoclinic":
                    this.C11 = Inverted[0, 0];
                    this.C22 = Inverted[1, 1];
                    this.C33 = Inverted[2, 2];
                    this.C12 = Inverted[0, 1];
                    this.C13 = Inverted[0, 2];
                    this.C16 = Inverted[0, 5];
                    this.C23 = Inverted[1, 2];
                    this.C26 = Inverted[1, 5];
                    this.C36 = Inverted[2, 5];
                    this.C44 = Inverted[3, 3];
                    this.C45 = Inverted[3, 4];
                    this.C55 = Inverted[4, 4];
                    this.C66 = Inverted[5, 5];
                    break;
                case "triclinic":
                    this.C11 = Inverted[0, 0];
                    this.C12 = Inverted[0, 1];
                    this.C13 = Inverted[0, 2];
                    this.C14 = Inverted[0, 3];
                    this.C15 = Inverted[0, 4];
                    this.C16 = Inverted[0, 5];
                    this.C22 = Inverted[1, 1];
                    this.C23 = Inverted[1, 2];
                    this.C24 = Inverted[1, 3];
                    this.C25 = Inverted[1, 4];
                    this.C26 = Inverted[1, 5];
                    this.C33 = Inverted[2, 2];
                    this.C34 = Inverted[2, 3];
                    this.C35 = Inverted[2, 4];
                    this.C36 = Inverted[2, 5];
                    this.C44 = Inverted[3, 3];
                    this.C45 = Inverted[3, 4];
                    this.C46 = Inverted[3, 5];
                    this.C55 = Inverted[4, 4];
                    this.C56 = Inverted[4, 5];
                    this.C66 = Inverted[5, 5];
                    break;
                default:
                    this.C11 = Inverted[0, 0];
                    this.C12 = Inverted[0, 1];
                    this.C13 = Inverted[0, 2];
                    this.C14 = Inverted[0, 3];
                    this.C15 = Inverted[0, 4];
                    this.C16 = Inverted[0, 5];
                    this.C22 = Inverted[1, 1];
                    this.C23 = Inverted[1, 2];
                    this.C24 = Inverted[1, 3];
                    this.C25 = Inverted[1, 4];
                    this.C26 = Inverted[1, 5];
                    this.C33 = Inverted[2, 2];
                    this.C34 = Inverted[2, 3];
                    this.C35 = Inverted[2, 4];
                    this.C36 = Inverted[2, 5];
                    this.C44 = Inverted[3, 3];
                    this.C45 = Inverted[3, 4];
                    this.C46 = Inverted[3, 5];
                    this.C55 = Inverted[4, 4];
                    this.C56 = Inverted[4, 5];
                    this.C66 = Inverted[5, 5];
                    break;
            }
        }

        public MathNet.Numerics.LinearAlgebra.Matrix<double> createStiffnessMatrix()
        {
            MathNet.Numerics.LinearAlgebra.Matrix<double> Ret = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(6, 6, 0.0);

            Ret[0, 0] = this.S11;
            Ret[0, 1] = this.S12;
            Ret[0, 2] = this.S13;
            Ret[0, 3] = 0.5 * this.S14;
            Ret[0, 4] = 0.5 * this.S15;
            Ret[0, 5] = 0.5 * this.S16;

            Ret[1, 0] = this.S21;
            Ret[1, 1] = this.S22;
            Ret[1, 2] = this.S23;
            Ret[1, 3] = 0.5 * this.S24;
            Ret[1, 4] = 0.5 * this.S25;
            Ret[1, 5] = 0.5 * this.S26;

            Ret[2, 0] = this.S31;
            Ret[2, 1] = this.S32;
            Ret[2, 2] = this.S33;
            Ret[2, 3] = 0.5 * this.S34;
            Ret[2, 4] = 0.5 * this.S35;
            Ret[2, 5] = 0.5 * this.S36;

            Ret[3, 0] = 0.5 * this.S41;
            Ret[3, 1] = 0.5 * this.S42;
            Ret[3, 2] = 0.5 * this.S43;
            Ret[3, 3] = 0.25 * this.S44;
            Ret[3, 4] = 0.25 * this.S45;
            Ret[3, 5] = 0.25 * this.S46;

            Ret[4, 0] = 0.5 * this.S51;
            Ret[4, 1] = 0.5 * this.S52;
            Ret[4, 2] = 0.5 * this.S53;
            Ret[4, 3] = 0.25 * this.S54;
            Ret[4, 4] = 0.25 * this.S55;
            Ret[4, 5] = 0.25 * this.S56;

            Ret[5, 0] = 0.5 * this.S61;
            Ret[5, 1] = 0.5 * this.S62;
            Ret[5, 2] = 0.5 * this.S63;
            Ret[5, 3] = 0.25 * this.S64;
            Ret[5, 4] = 0.25 * this.S65;
            Ret[5, 5] = 0.25 * this.S66;

            return Ret;
        }

        public void SetErrors(double percentage)
        {
            for(int n = 0; n < 6; n++)
            {
                for(int i = 0; i < 6; i++)
                {
                    if(this._stiffnessTensorError[n, i] == 0.0)
                    {
                        this._stiffnessTensorError[n, i] = percentage * this._stiffnessTensor[n, i];
                    }
                    if (this._complianceTensorError[n, i] == 0.0)
                    {
                        this._complianceTensorError[n, i] = percentage * this._complianceTensor[n, i];
                    }
                }
            }
        }

        #endregion

        #region Fitting

        #region Voigt

        public void FitVoigt(bool classicCalculation)
        {
            switch (this.Symmetry)
            {
                case "cubic":
                    if(this.IsIsotropic)
                    {
                        this.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorVoigtCubicIsotrope(this, classicCalculation);
                    }
                    else
                    {
                        this.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorVoigtCubic(this, classicCalculation);
                    }
                    break;
                case "hexagonal":
                    this.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorVoigtType1(this, classicCalculation);
                    break;
                case "tetragonal type 1":
                    this.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorVoigtType2(this, classicCalculation);
                    break;
                case "tetragonal type 2":
                    this.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorVoigtType2(this, classicCalculation);
                    break;
                case "trigonal type 1":
                    this.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorVoigtType1(this, classicCalculation);
                    break;
                case "trigonal type 2":
                    this.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorVoigtType1(this, classicCalculation);
                    break;
                case "rhombic":
                    this.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorVoigtType3(this, classicCalculation);
                    break;
                case "monoclinic":
                    this.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorVoigtType3(this, classicCalculation);
                    break;
                case "triclinic":
                    this.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorVoigtType3(this, classicCalculation);
                    break;
                default:
                    this.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorVoigtType3(this, classicCalculation);
                    break;
            }

            this.CalculateCompliances();
        }

        #endregion

        #region Reuss

        public void FitReuss(bool classicCalculation)
        {
            switch (this.Symmetry)
            {
                case "cubic":
                    this.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorReussCubic(this, classicCalculation);
                    break;
                case "hexagonal":
                    this.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorReussHexagonal(this, classicCalculation);
                    break;
                case "tetragonal type 1":
                    
                    break;
                case "tetragonal type 2":
                    
                    break;
                case "trigonal type 1":
                    
                    break;
                case "trigonal type 2":
                    
                    break;
                case "rhombic":
                    
                    break;
                case "monoclinic":
                    
                    break;
                case "triclinic":
                    
                    break;
                default:
                    
                    break;
            }

            this.CalculateStiffnesses();
        }

        #endregion

        #endregion

        #region REK Calculation

        #region Voigt

        #region Cubic Isotrope

        public double S1VoigtCubicIsotrope()
        {
            double Ret = -5 * this.C12;

            double N = 3 * Math.Pow(this.C11, 2);
            N += this.C11 * this.C12;
            N -= 10 * Math.Pow(this.C12, 2);

            Ret /= N;

            return Ret;
        }

        public double HS2VoigtCubicIsotrope()
        {
            double Ret = 5 / 4;

            double Sum1 = this.C11 - this.C12;

            Ret /= Sum1;

            return Ret;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C12
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorVoigtCubicIsotropeClassic(double Lambda)
        {
            //[0][0] Aclivity
            //[1][1] Constant
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(2, 2, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(2);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeC11S1VoigtCubicIsotrope() * this.FirstDerivativeC11S1VoigtCubicIsotrope() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 0] += (this.FirstDerivativeC11HS2VoigtCubicIsotrope() * this.FirstDerivativeC11HS2VoigtCubicIsotrope() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeC12S1VoigtCubicIsotrope() * this.FirstDerivativeC12S1VoigtCubicIsotrope() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 1] += (this.FirstDerivativeC12HS2VoigtCubicIsotrope() * this.FirstDerivativeC12HS2VoigtCubicIsotrope() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC12S1VoigtCubicIsotrope() * this.FirstDerivativeC11S1VoigtCubicIsotrope() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC12HS2VoigtCubicIsotrope() * this.FirstDerivativeC11HS2VoigtCubicIsotrope() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC12S1VoigtCubicIsotrope() * this.FirstDerivativeC11S1VoigtCubicIsotrope() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC12HS2VoigtCubicIsotrope() * this.FirstDerivativeC11HS2VoigtCubicIsotrope() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                #endregion

                #region Vector build

                SolutionVector[0] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtCubicIsotrope()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC11S1VoigtCubicIsotrope();
                SolutionVector[0] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtCubicIsotrope()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC11HS2VoigtCubicIsotrope();
                SolutionVector[1] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtCubicIsotrope()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC12S1VoigtCubicIsotrope();
                SolutionVector[1] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtCubicIsotrope()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC12HS2VoigtCubicIsotrope();

                #endregion
            }

            for (int n = 0; n < 2; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(2);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C12
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorVoigtCubicIsotropeMacroscopic(double Lambda)
        {
            //[0][0] Aclivity
            //[1][1] Constant
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(2, 2, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(2);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeC11S1VoigtCubicIsotrope() * this.FirstDerivativeC11S1VoigtCubicIsotrope() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 0] += (this.FirstDerivativeC11HS2VoigtCubicIsotrope() * this.FirstDerivativeC11HS2VoigtCubicIsotrope() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeC12S1VoigtCubicIsotrope() * this.FirstDerivativeC12S1VoigtCubicIsotrope() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 1] += (this.FirstDerivativeC12HS2VoigtCubicIsotrope() * this.FirstDerivativeC12HS2VoigtCubicIsotrope() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC12S1VoigtCubicIsotrope() * this.FirstDerivativeC11S1VoigtCubicIsotrope() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC12HS2VoigtCubicIsotrope() * this.FirstDerivativeC11HS2VoigtCubicIsotrope() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC12S1VoigtCubicIsotrope() * this.FirstDerivativeC11S1VoigtCubicIsotrope() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC12HS2VoigtCubicIsotrope() * this.FirstDerivativeC11HS2VoigtCubicIsotrope() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                #endregion

                #region Vector build

                SolutionVector[0] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtCubicIsotrope()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC11S1VoigtCubicIsotrope();
                SolutionVector[0] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtCubicIsotrope()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC11HS2VoigtCubicIsotrope();
                SolutionVector[1] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtCubicIsotrope()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC12S1VoigtCubicIsotrope();
                SolutionVector[1] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtCubicIsotrope()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC12HS2VoigtCubicIsotrope();

                #endregion
            }

            for (int n = 0; n < 2; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(2);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        #region First derivative

        public double FirstDerivativeC11S1VoigtCubicIsotrope()
        {
            double Ret = 5 * this.C12;
            Ret *= (6 * this.C11) + this.C12;

            double Sum = 3 * Math.Pow(this.C12, 2);
            Sum -= 10 * Math.Pow(this.C12, 2);
            Sum += this.C12 * this.C11;

            Ret /= Math.Pow(Sum, 2);

            return Ret;
        }

        public double FirstDerivativeC12S1VoigtCubicIsotrope()
        {
            double Ret = (3 * Math.Pow(this.C11, 2)) + (10 * Math.Pow(this.C12, 2));
            Ret *= -5;

            double Sum = 3 * Math.Pow(this.C12, 2);
            Sum -= 10 * Math.Pow(this.C12, 2);
            Sum += this.C12 * this.C11;

            Ret /= Math.Pow(Sum, 2);

            return Ret;
        }

        public double FirstDerivativeC11HS2VoigtCubicIsotrope()
        {
            double Ret = -5;

            double Sum = 4 * Math.Pow(this.C11 - this.C12, 2);

            Ret /= Sum;

            return Ret;
        }

        public double FirstDerivativeC12HS2VoigtCubicIsotrope()
        {
            double Ret = 5;

            double Sum = 4 * Math.Pow(this.C11 - this.C12, 2);

            Ret /= Sum;

            return Ret;
        }

        #endregion

        #endregion

        #region Cubic

        public double S1VoigtCubic()
        {
            double Ret = this.C11;
            Ret += 4 * this.C12;
            Ret -= 2 * this.C44;
            Ret *= -0.5;

            double N = this.C11 - this.C12;
            N += 3 * this.C44;

            N *= ((2 * this.C12) + this.C11);

            Ret /= N;

            return Ret;
        }

        public double HS2VoigtCubic()
        {
            double Ret = 5 / 2;

            double Sum = this.C11 - this.C12;
            Sum += 3 * this.C44;

            Ret /= Sum;

            return Ret;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C12
        ///[2] C44
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorVoigtCubicClassic(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeC11S1VoigtCubic() * this.FirstDerivativeC11S1VoigtCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 0] += (this.FirstDerivativeC11HS2VoigtCubic() * this.FirstDerivativeC11HS2VoigtCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeC12S1VoigtCubic() * this.FirstDerivativeC12S1VoigtCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 1] += (this.FirstDerivativeC12HS2VoigtCubic() * this.FirstDerivativeC12HS2VoigtCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC12S1VoigtCubic() * this.FirstDerivativeC11S1VoigtCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC12HS2VoigtCubic() * this.FirstDerivativeC11HS2VoigtCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC12S1VoigtCubic() * this.FirstDerivativeC11S1VoigtCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC12HS2VoigtCubic() * this.FirstDerivativeC11HS2VoigtCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeC44S1VoigtCubic() * this.FirstDerivativeC44S1VoigtCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 2] += (this.FirstDerivativeC44HS2VoigtCubic() * this.FirstDerivativeC44HS2VoigtCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeC44S1VoigtCubic() * this.FirstDerivativeC11S1VoigtCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeC44HS2VoigtCubic() * this.FirstDerivativeC11HS2VoigtCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeC44S1VoigtCubic() * this.FirstDerivativeC11S1VoigtCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeC44HS2VoigtCubic() * this.FirstDerivativeC11HS2VoigtCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeC44S1VoigtCubic() * this.FirstDerivativeC12S1VoigtCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeC44HS2VoigtCubic() * this.FirstDerivativeC12HS2VoigtCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeC44S1VoigtCubic() * this.FirstDerivativeC12S1VoigtCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeC44HS2VoigtCubic() * this.FirstDerivativeC12HS2VoigtCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                #endregion

                #region Vector build

                SolutionVector[0] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtCubic()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC11S1VoigtCubic();
                SolutionVector[0] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtCubic()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC11HS2VoigtCubic();
                SolutionVector[1] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtCubic()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC12S1VoigtCubic();
                SolutionVector[1] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtCubic()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC12HS2VoigtCubic();
                SolutionVector[2] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtCubic()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC44S1VoigtCubic();
                SolutionVector[2] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtCubic()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC44HS2VoigtCubic();

                #endregion
            }

            for (int n = 0; n < 3; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C12
        ///[2] C44
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorVoigtCubicMacroscopic(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeC11S1VoigtCubic() * this.FirstDerivativeC11S1VoigtCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 0] += (this.FirstDerivativeC11HS2VoigtCubic() * this.FirstDerivativeC11HS2VoigtCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeC12S1VoigtCubic() * this.FirstDerivativeC12S1VoigtCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 1] += (this.FirstDerivativeC12HS2VoigtCubic() * this.FirstDerivativeC12HS2VoigtCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC12S1VoigtCubic() * this.FirstDerivativeC11S1VoigtCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC12HS2VoigtCubic() * this.FirstDerivativeC11HS2VoigtCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC12S1VoigtCubic() * this.FirstDerivativeC11S1VoigtCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC12HS2VoigtCubic() * this.FirstDerivativeC11HS2VoigtCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeC44S1VoigtCubic() * this.FirstDerivativeC44S1VoigtCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 2] += (this.FirstDerivativeC44HS2VoigtCubic() * this.FirstDerivativeC44HS2VoigtCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeC44S1VoigtCubic() * this.FirstDerivativeC11S1VoigtCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeC44HS2VoigtCubic() * this.FirstDerivativeC11HS2VoigtCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeC44S1VoigtCubic() * this.FirstDerivativeC11S1VoigtCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeC44HS2VoigtCubic() * this.FirstDerivativeC11HS2VoigtCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeC44S1VoigtCubic() * this.FirstDerivativeC12S1VoigtCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeC44HS2VoigtCubic() * this.FirstDerivativeC12HS2VoigtCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeC44S1VoigtCubic() * this.FirstDerivativeC12S1VoigtCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeC44HS2VoigtCubic() * this.FirstDerivativeC12HS2VoigtCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                #endregion

                #region Vector build

                SolutionVector[0] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtCubic()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC11S1VoigtCubic();
                SolutionVector[0] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtCubic()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC11HS2VoigtCubic();
                SolutionVector[1] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtCubic()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC12S1VoigtCubic();
                SolutionVector[1] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtCubic()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC12HS2VoigtCubic();
                SolutionVector[2] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtCubic()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC44S1VoigtCubic();
                SolutionVector[2] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtCubic()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC44HS2VoigtCubic();

                #endregion
            }

            for (int n = 0; n < 3; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        #region First derivative

        public double FirstDerivativeC11S1VoigtCubic()
        {
            double Ret = Math.Pow(this.C11, 2);
            Ret += 6 * Math.Pow(this.C12, 2);
            Ret -= 6 * Math.Pow(this.C44, 2);
            Ret += 8 * this.C11 * this.C12;
            Ret -= 4 * this.C11 * this.C44;
            Ret += 4 * this.C12 * this.C44;

            double N = Math.Pow(this.C11 + (2 * this.C12), 2) * Math.Pow(this.C11 - this.C12 + (3 * this.C44), 2);

            Ret /= 2 * N;

            return Ret;
        }

        public double FirstDerivativeC12S1VoigtCubic()
        {
            double Ret = -3 * Math.Pow(this.C11, 2);
            Ret -= 8 * Math.Pow(this.C12, 2);
            Ret -= 12 * Math.Pow(this.C44, 2);
            Ret += 4 * this.C11 * this.C12;
            Ret -= 10 * this.C11 * this.C44;
            Ret += 8 * this.C12 * this.C44;

            double N = Math.Pow(this.C11 + (2 * this.C12), 2) * Math.Pow(this.C11 - this.C12 + (3 * this.C44), 2);

            Ret /= 2 * N;

            return Ret;
        }

        public double FirstDerivativeC44S1VoigtCubic()
        {
            double Ret = 5 / 2;
            Ret /= Math.Pow(this.C11 - this.C12 + (3.0 * this.C44), 2);

            return Ret;
        }

        public double FirstDerivativeC11HS2VoigtCubic()
        {
            double Ret = -5;

            double Sum = this.C11 - this.C12 + (3 * this.C44);

            Ret /= 2 * Math.Pow(Sum, 2);

            return Ret;
        }

        public double FirstDerivativeC12HS2VoigtCubic()
        {
            double Ret = 5;

            double Sum = this.C11 - this.C12 + (3 * this.C44);

            Ret /= 2 * Math.Pow(Sum, 2);

            return Ret;
        }

        public double FirstDerivativeC44HS2VoigtCubic()
        {
            double Ret = 15;

            double Sum = this.C11 - this.C12 + (3 * this.C44);

            Ret /= 2 * Math.Pow(Sum, 2);

            return Ret;
        }

        #endregion

        #endregion

        #region Type1 (Hexagonal, Trigonal 1 and 2)

        public double S1VoigtType1()
        {
            double Ret = this.C11 + this.C33;
            Ret += 5 * this.C12;
            Ret += 8 * this.C13;
            Ret -= 4 * this.C44;
            Ret *= -3 / 2;

            double Sum = (7 / 2) * this.C11;
            Sum -= (5 / 2) * this.C12;
            Sum -= 2 * this.C13;
            Sum += this.C33;
            Sum += 6 * this.C44;

            double Sum1 = 2 * this.C11;
            Sum1 += this.C33;
            Sum1 += 2 * this.C12;
            Sum1 += 4 * this.C13;

            Sum *= Sum1;

            Ret /= Sum;

            return Ret;
        }

        public double HS2VoigtType1()
        {
            double Ret = 15;

            double Sum = 7 * this.C11;
            Sum += 2 * this.C33;
            Sum -= 5 * this.C12;
            Sum -= 4 * this.C13;
            Sum += 12 * this.C44;

            Ret /= Sum;

            return Ret;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C33
        ///[2] C12
        ///[3] C13
        ///[4] C44
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorVoigtType1Classic(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(5, 5, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(5);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeC11S1VoigtType1() * this.FirstDerivativeC11S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 0] += (this.FirstDerivativeC11HS2VoigtType1() * this.FirstDerivativeC11HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeC33S1VoigtType1() * this.FirstDerivativeC33S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 1] += (this.FirstDerivativeC33HS2VoigtType1() * this.FirstDerivativeC33HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC33S1VoigtType1() * this.FirstDerivativeC11S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC33HS2VoigtType1() * this.FirstDerivativeC11HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC33S1VoigtType1() * this.FirstDerivativeC11S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC33HS2VoigtType1() * this.FirstDerivativeC11HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeC12S1VoigtType1() * this.FirstDerivativeC12S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 2] += (this.FirstDerivativeC12HS2VoigtType1() * this.FirstDerivativeC12HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeC12S1VoigtType1() * this.FirstDerivativeC11S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeC12HS2VoigtType1() * this.FirstDerivativeC11HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeC12S1VoigtType1() * this.FirstDerivativeC11S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeC12HS2VoigtType1() * this.FirstDerivativeC11HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeC12S1VoigtType1() * this.FirstDerivativeC33S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeC12HS2VoigtType1() * this.FirstDerivativeC33HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeC12S1VoigtType1() * this.FirstDerivativeC33S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeC12HS2VoigtType1() * this.FirstDerivativeC33HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[3, 3] += (this.FirstDerivativeC13S1VoigtType1() * this.FirstDerivativeC13S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 3] += (this.FirstDerivativeC13HS2VoigtType1() * this.FirstDerivativeC13HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeC13S1VoigtType1() * this.FirstDerivativeC11S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeC13HS2VoigtType1() * this.FirstDerivativeC11HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeC13S1VoigtType1() * this.FirstDerivativeC11S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeC13HS2VoigtType1() * this.FirstDerivativeC11HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeC13S1VoigtType1() * this.FirstDerivativeC33S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeC13HS2VoigtType1() * this.FirstDerivativeC33HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeC13S1VoigtType1() * this.FirstDerivativeC33S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeC13HS2VoigtType1() * this.FirstDerivativeC33HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeC13S1VoigtType1() * this.FirstDerivativeC12S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeC13HS2VoigtType1() * this.FirstDerivativeC12HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeC13S1VoigtType1() * this.FirstDerivativeC12S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeC13HS2VoigtType1() * this.FirstDerivativeC12HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[4, 4] += (this.FirstDerivativeC44S1VoigtType1() * this.FirstDerivativeC44S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 4] += (this.FirstDerivativeC44HS2VoigtType1() * this.FirstDerivativeC44HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeC44S1VoigtType1() * this.FirstDerivativeC11S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeC44HS2VoigtType1() * this.FirstDerivativeC11HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeC44S1VoigtType1() * this.FirstDerivativeC11S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeC44HS2VoigtType1() * this.FirstDerivativeC11HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeC44S1VoigtType1() * this.FirstDerivativeC33S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeC44HS2VoigtType1() * this.FirstDerivativeC33HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeC44S1VoigtType1() * this.FirstDerivativeC33S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeC44HS2VoigtType1() * this.FirstDerivativeC33HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeC44S1VoigtType1() * this.FirstDerivativeC12S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeC44HS2VoigtType1() * this.FirstDerivativeC12HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeC44S1VoigtType1() * this.FirstDerivativeC12S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeC44HS2VoigtType1() * this.FirstDerivativeC12HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeC44S1VoigtType1() * this.FirstDerivativeC13S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeC44HS2VoigtType1() * this.FirstDerivativeC13HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeC44S1VoigtType1() * this.FirstDerivativeC13S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeC44HS2VoigtType1() * this.FirstDerivativeC13HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                #endregion

                #region Vector build

                SolutionVector[0] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtType1()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC11S1VoigtType1();
                SolutionVector[0] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtType1()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC11HS2VoigtType1();
                SolutionVector[1] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtType1()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC33S1VoigtType1();
                SolutionVector[1] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtType1()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC33HS2VoigtType1();
                SolutionVector[2] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtType1()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC12S1VoigtType1();
                SolutionVector[2] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtType1()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC12HS2VoigtType1();
                SolutionVector[3] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtType1()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC13S1VoigtType1();
                SolutionVector[3] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtType1()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC13HS2VoigtType1();
                SolutionVector[4] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtType1()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC44S1VoigtType1();
                SolutionVector[4] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtType1()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC44HS2VoigtType1();

                #endregion
            }

            for (int n = 0; n < 5; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(5);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C33
        ///[2] C12
        ///[3] C13
        ///[4] C44
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorVoigtType1Macroscopic(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(5, 5, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(5);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeC11S1VoigtType1() * this.FirstDerivativeC11S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 0] += (this.FirstDerivativeC11HS2VoigtType1() * this.FirstDerivativeC11HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeC33S1VoigtType1() * this.FirstDerivativeC33S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 1] += (this.FirstDerivativeC33HS2VoigtType1() * this.FirstDerivativeC33HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC33S1VoigtType1() * this.FirstDerivativeC11S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC33HS2VoigtType1() * this.FirstDerivativeC11HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC33S1VoigtType1() * this.FirstDerivativeC11S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC33HS2VoigtType1() * this.FirstDerivativeC11HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeC12S1VoigtType1() * this.FirstDerivativeC12S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 2] += (this.FirstDerivativeC12HS2VoigtType1() * this.FirstDerivativeC12HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeC12S1VoigtType1() * this.FirstDerivativeC11S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeC12HS2VoigtType1() * this.FirstDerivativeC11HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeC12S1VoigtType1() * this.FirstDerivativeC11S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeC12HS2VoigtType1() * this.FirstDerivativeC11HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeC12S1VoigtType1() * this.FirstDerivativeC33S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeC12HS2VoigtType1() * this.FirstDerivativeC33HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeC12S1VoigtType1() * this.FirstDerivativeC33S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeC12HS2VoigtType1() * this.FirstDerivativeC33HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[3, 3] += (this.FirstDerivativeC13S1VoigtType1() * this.FirstDerivativeC13S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 3] += (this.FirstDerivativeC13HS2VoigtType1() * this.FirstDerivativeC13HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeC13S1VoigtType1() * this.FirstDerivativeC11S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeC13HS2VoigtType1() * this.FirstDerivativeC11HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeC13S1VoigtType1() * this.FirstDerivativeC11S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeC13HS2VoigtType1() * this.FirstDerivativeC11HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeC13S1VoigtType1() * this.FirstDerivativeC33S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeC13HS2VoigtType1() * this.FirstDerivativeC33HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeC13S1VoigtType1() * this.FirstDerivativeC33S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeC13HS2VoigtType1() * this.FirstDerivativeC33HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeC13S1VoigtType1() * this.FirstDerivativeC12S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeC13HS2VoigtType1() * this.FirstDerivativeC12HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeC13S1VoigtType1() * this.FirstDerivativeC12S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeC13HS2VoigtType1() * this.FirstDerivativeC12HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[4, 4] += (this.FirstDerivativeC44S1VoigtType1() * this.FirstDerivativeC44S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 4] += (this.FirstDerivativeC44HS2VoigtType1() * this.FirstDerivativeC44HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeC44S1VoigtType1() * this.FirstDerivativeC11S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeC44HS2VoigtType1() * this.FirstDerivativeC11HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeC44S1VoigtType1() * this.FirstDerivativeC11S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeC44HS2VoigtType1() * this.FirstDerivativeC11HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeC44S1VoigtType1() * this.FirstDerivativeC33S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeC44HS2VoigtType1() * this.FirstDerivativeC33HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeC44S1VoigtType1() * this.FirstDerivativeC33S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeC44HS2VoigtType1() * this.FirstDerivativeC33HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeC44S1VoigtType1() * this.FirstDerivativeC12S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeC44HS2VoigtType1() * this.FirstDerivativeC12HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeC44S1VoigtType1() * this.FirstDerivativeC12S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeC44HS2VoigtType1() * this.FirstDerivativeC12HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeC44S1VoigtType1() * this.FirstDerivativeC13S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeC44HS2VoigtType1() * this.FirstDerivativeC13HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeC44S1VoigtType1() * this.FirstDerivativeC13S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeC44HS2VoigtType1() * this.FirstDerivativeC13HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                #endregion

                #region Vector build

                SolutionVector[0] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtType1()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC11S1VoigtType1();
                SolutionVector[0] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtType1()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC11HS2VoigtType1();
                SolutionVector[1] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtType1()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC33S1VoigtType1();
                SolutionVector[1] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtType1()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC33HS2VoigtType1();
                SolutionVector[2] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtType1()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC12S1VoigtType1();
                SolutionVector[2] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtType1()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC12HS2VoigtType1();
                SolutionVector[3] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtType1()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC13S1VoigtType1();
                SolutionVector[3] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtType1()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC13HS2VoigtType1();
                SolutionVector[4] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtType1()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC44S1VoigtType1();
                SolutionVector[4] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtType1()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC44HS2VoigtType1();

                #endregion
            }

            for (int n = 0; n < 5; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(5);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        #region First derivative

        public double FirstDerivativeC11S1VoigtType1()
        {
            double Ret = 14 * Math.Pow(this.C11, 2);
            Ret += 30 * Math.Pow(this.C12, 2);
            Ret += 176 * Math.Pow(this.C13, 2);
            Ret += 9 * Math.Pow(this.C33, 2);
            Ret -= 30 * Math.Pow(this.C44, 2);
            Ret += 104 * this.C13 * this.C33;
            Ret += 64 * this.C13 * this.C44;
            Ret += 32 * this.C33 * this.C44;

            double Sum = (5 * this.C12) + (8 * this.C13) + this.C33 - (4 * this.C44);

            Ret += 28 * this.C11 * Sum;

            Sum = (8 * this.C13) + (3 * this.C33) + (4 * this.C44);

            Ret += 20 * this.C12 * Sum;
            Ret *= 3;

            double N = 4 * Math.Pow((2 * this.C11) + (2 * this.C12) + (4 * this.C13) + this.C33, 2);

            Sum = (7 * this.C11) - (5 * this.C12);
            Sum -= (4 * this.C13);
            Sum += 2 * this.C33;
            Sum += 12 * this.C44;

            N *= Math.Pow(Sum, 2);

            Ret /= N;

            return Ret;
        }

        public double FirstDerivativeC33S1VoigtType1()
        {
            double Ret = -3 * Math.Pow(this.C11, 2);
            Ret += 5 * Math.Pow(this.C12, 2);
            Ret += 48 * Math.Pow(this.C13, 2);
            Ret += 2 * Math.Pow(this.C33, 2);
            Ret -= 48 * Math.Pow(this.C44, 2);
            Ret += 32 * this.C13 * this.C33;
            Ret += 32 * this.C13 * this.C44;
            Ret -= 16 * this.C33 * this.C44;

            double Sum = (50 * this.C12) + (72 * this.C13) + (4 * this.C33) - (56 * this.C44);

            Ret += this.C11 * Sum;

            Sum = (2 * this.C13) + this.C33 + (2 * this.C44);

            Ret += 20 * this.C12 * Sum;
            Ret *= 3;

            double N = 4 * Math.Pow((2 * this.C11) + (2 * this.C12) + (4 * this.C13) + this.C33, 2);

            Sum = (7 * this.C11) - (5 * this.C12);
            Sum -= (4 * this.C13);
            Sum += 2 * this.C33;
            Sum += 12 * this.C44;

            N *= Math.Pow(Sum, 2);

            Ret /= N;

            return Ret;
        }

        public double FirstDerivativeC12S1VoigtType1()
        {
            double Ret = 66 * Math.Pow(this.C11, 2);
            Ret += 50 * Math.Pow(this.C12, 2);
            Ret += 144 * Math.Pow(this.C13, 2);
            Ret += 11 * Math.Pow(this.C33, 2);
            Ret += 96 * Math.Pow(this.C44, 2);
            Ret += 56 * this.C13 * this.C33;
            Ret -= 64 * this.C13 * this.C44;
            Ret += 32 * this.C33 * this.C44;

            double Sum = (5 * this.C12) + (24 * this.C13) + (13 * this.C33) + (28 * this.C44);

            Ret += 4 * this.C11 * Sum;

            Sum = (8 * this.C13) + this.C33 - (4 * this.C44);

            Ret += 20 * this.C12 * Sum;
            Ret *= -3;

            double N = 4 * Math.Pow((2 * this.C11) + (2 * this.C12) + (4 * this.C13) + this.C33, 2);

            Sum = (7 * this.C11) - (5 * this.C12);
            Sum -= (4 * this.C13);
            Sum += 2 * this.C33;
            Sum += 12 * this.C44;

            N *= Math.Pow(Sum, 2);

            Ret /= N;

            return Ret;
        }

        public double FirstDerivativeC13S1VoigtType1()
        {
            double Ret = 23 * Math.Pow(this.C11, 2);
            Ret += 15 * Math.Pow(this.C12, 2);
            Ret += 32 * Math.Pow(this.C13, 2);
            Ret += 3 * Math.Pow(this.C33, 2);
            Ret += 48 * Math.Pow(this.C44, 2);
            Ret += 8 * this.C13 * this.C33;
            Ret -= 32 * this.C13 * this.C44;
            Ret += 16 * this.C33 * this.C44;

            double Sum = (-10 * this.C12) + (8 * this.C13) + (16 * this.C33) + (56 * this.C44);

            Ret += this.C11 * Sum;

            Sum = this.C13 + this.C33 - this.C44;

            Ret += 40 * this.C12 * Sum;
            Ret *= -3;

            double N = Math.Pow((2 * this.C11) + (2 * this.C12) + (4 * this.C13) + this.C33, 2);

            Sum = (7 * this.C11) - (5 * this.C12);
            Sum -= (4 * this.C13);
            Sum += 2 * this.C33;
            Sum += 12 * this.C44;

            N *= Math.Pow(Sum, 2);

            Ret /= N;

            return Ret;
        }

        public double FirstDerivativeC44S1VoigtType1()
        {
            double Ret = 15;

            double N = (7 * this.C11) - (5 * this.C12) - (4 * this.C13) + (2 * this.C33) + (12 * this.C44);

            Ret /= Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeC11HS2VoigtType1()
        {
            double Ret = -105;

            double Sum = 7 * this.C11;
            Sum -= 5 * this.C12;
            Sum -= 4 * this.C13;
            Sum += 2 * this.C33;
            Sum += 12 * this.C44;

            Ret /= Math.Pow(Sum, 2);

            return Ret;
        }

        public double FirstDerivativeC33HS2VoigtType1()
        {
            double Ret = 30;

            double Sum = 7 * this.C11;
            Sum -= 5 * this.C12;
            Sum -= 4 * this.C13;
            Sum += 2 * this.C33;
            Sum += 12 * this.C44;

            Ret /= Math.Pow(Sum, 2);

            return Ret;
        }

        public double FirstDerivativeC12HS2VoigtType1()
        {
            double Ret = 75;

            double Sum = 7 * this.C11;
            Sum -= 5 * this.C12;
            Sum -= 4 * this.C13;
            Sum += 2 * this.C33;
            Sum += 12 * this.C44;

            Ret /= Math.Pow(Sum, 2);

            return Ret;
        }

        public double FirstDerivativeC13HS2VoigtType1()
        {
            double Ret = 60;

            double Sum = 7 * this.C11;
            Sum -= 5 * this.C12;
            Sum -= 4 * this.C13;
            Sum += 2 * this.C33;
            Sum += 12 * this.C44;

            Ret /= Math.Pow(Sum, 2);

            return Ret;
        }

        public double FirstDerivativeC44HS2VoigtType1()
        {
            double Ret = -180;

            double Sum = 7 * this.C11;
            Sum -= 5 * this.C12;
            Sum -= 4 * this.C13;
            Sum += 2 * this.C33;
            Sum += 12 * this.C44;

            Ret /= Math.Pow(Sum, 2);

            return Ret;
        }

        #endregion

        #endregion

        #region Type 2 (Tetragonal 1 and 2)

        public double S1VoigtType2()
        {
            double Ret = 2 * this.C11;
            Ret += this.C33;
            Ret += 4 * this.C12;
            Ret += 8 * this.C13;
            Ret -= 4 * this.C44;
            Ret -= 2 * this.C66;
            Ret *= -3 / 2;

            double Sum = 2 * this.C11;
            Sum += this.C33 - this.C12;
            Sum -= 2 * this.C13;
            Sum += 6 * this.C44;
            Sum += 3 * this.C66;

            double Sum1 = 2 * this.C11;
            Sum1 += this.C33;
            Sum1 += 2 * this.C12;
            Sum1 += 4 * this.C13;

            Sum *= Sum1;

            Ret /= Sum;

            return Ret;
        }

        public double HS2VoigtType2()
        {
            double Sum = 4 * this.C11;
            Sum += 2 * (this.C33 - this.C12);
            Sum -= 4 * this.C13;
            Sum += 12 * this.C44;
            Sum += 6 * this.C66;

            double Ret = 15 / Sum;

            return Ret;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C33
        ///[2] C12
        ///[3] C13
        ///[4] C44
        ///[5] C66
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorVoigtType2Classic(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(6, 6, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(6);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeC11S1VoigtType2() * this.FirstDerivativeC11S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 0] += (this.FirstDerivativeC11HS2VoigtType2() * this.FirstDerivativeC11HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeC33S1VoigtType2() * this.FirstDerivativeC33S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 1] += (this.FirstDerivativeC33HS2VoigtType2() * this.FirstDerivativeC33HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC33S1VoigtType2() * this.FirstDerivativeC11S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC33HS2VoigtType2() * this.FirstDerivativeC11HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC33S1VoigtType2() * this.FirstDerivativeC11S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC33HS2VoigtType2() * this.FirstDerivativeC11HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeC12S1VoigtType2() * this.FirstDerivativeC12S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 2] += (this.FirstDerivativeC12HS2VoigtType2() * this.FirstDerivativeC12HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeC12S1VoigtType2() * this.FirstDerivativeC11S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeC12HS2VoigtType2() * this.FirstDerivativeC11HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeC12S1VoigtType2() * this.FirstDerivativeC11S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeC12HS2VoigtType2() * this.FirstDerivativeC11HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeC12S1VoigtType2() * this.FirstDerivativeC33S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeC12HS2VoigtType2() * this.FirstDerivativeC33HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeC12S1VoigtType2() * this.FirstDerivativeC33S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeC12HS2VoigtType2() * this.FirstDerivativeC33HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[3, 3] += (this.FirstDerivativeC13S1VoigtType2() * this.FirstDerivativeC13S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 3] += (this.FirstDerivativeC13HS2VoigtType2() * this.FirstDerivativeC13HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeC13S1VoigtType2() * this.FirstDerivativeC11S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeC13HS2VoigtType2() * this.FirstDerivativeC11HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeC13S1VoigtType2() * this.FirstDerivativeC11S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeC13HS2VoigtType2() * this.FirstDerivativeC11HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeC13S1VoigtType2() * this.FirstDerivativeC33S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeC13HS2VoigtType2() * this.FirstDerivativeC33HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeC13S1VoigtType2() * this.FirstDerivativeC33S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeC13HS2VoigtType2() * this.FirstDerivativeC33HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeC13S1VoigtType2() * this.FirstDerivativeC12S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeC13HS2VoigtType2() * this.FirstDerivativeC12HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeC13S1VoigtType2() * this.FirstDerivativeC12S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeC13HS2VoigtType2() * this.FirstDerivativeC12HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[4, 4] += (this.FirstDerivativeC44S1VoigtType2() * this.FirstDerivativeC44S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 4] += (this.FirstDerivativeC44HS2VoigtType2() * this.FirstDerivativeC44HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeC44S1VoigtType2() * this.FirstDerivativeC11S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeC44HS2VoigtType2() * this.FirstDerivativeC11HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeC44S1VoigtType2() * this.FirstDerivativeC11S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeC44HS2VoigtType2() * this.FirstDerivativeC11HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeC44S1VoigtType2() * this.FirstDerivativeC33S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeC44HS2VoigtType2() * this.FirstDerivativeC33HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeC44S1VoigtType2() * this.FirstDerivativeC33S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeC44HS2VoigtType2() * this.FirstDerivativeC33HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeC44S1VoigtType2() * this.FirstDerivativeC12S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeC44HS2VoigtType2() * this.FirstDerivativeC12HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeC44S1VoigtType2() * this.FirstDerivativeC12S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeC44HS2VoigtType2() * this.FirstDerivativeC12HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeC44S1VoigtType2() * this.FirstDerivativeC13S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeC44HS2VoigtType2() * this.FirstDerivativeC13HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeC44S1VoigtType2() * this.FirstDerivativeC13S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeC44HS2VoigtType2() * this.FirstDerivativeC13HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[5, 5] += (this.FirstDerivativeC66S1VoigtType2() * this.FirstDerivativeC66S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[5, 5] += (this.FirstDerivativeC66HS2VoigtType2() * this.FirstDerivativeC66HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 5] += (this.FirstDerivativeC66S1VoigtType2() * this.FirstDerivativeC11S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 5] += (this.FirstDerivativeC66HS2VoigtType2() * this.FirstDerivativeC11HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[5, 0] += (this.FirstDerivativeC66S1VoigtType2() * this.FirstDerivativeC11S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[5, 0] += (this.FirstDerivativeC66HS2VoigtType2() * this.FirstDerivativeC11HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 5] += (this.FirstDerivativeC66S1VoigtType2() * this.FirstDerivativeC33S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 5] += (this.FirstDerivativeC66HS2VoigtType2() * this.FirstDerivativeC33HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[5, 1] += (this.FirstDerivativeC66S1VoigtType2() * this.FirstDerivativeC33S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[5, 1] += (this.FirstDerivativeC66HS2VoigtType2() * this.FirstDerivativeC33HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 5] += (this.FirstDerivativeC66S1VoigtType2() * this.FirstDerivativeC12S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 5] += (this.FirstDerivativeC66HS2VoigtType2() * this.FirstDerivativeC12HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[5, 2] += (this.FirstDerivativeC66S1VoigtType2() * this.FirstDerivativeC12S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[5, 2] += (this.FirstDerivativeC66HS2VoigtType2() * this.FirstDerivativeC12HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 5] += (this.FirstDerivativeC66S1VoigtType2() * this.FirstDerivativeC13S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 5] += (this.FirstDerivativeC66HS2VoigtType2() * this.FirstDerivativeC13HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[5, 3] += (this.FirstDerivativeC66S1VoigtType2() * this.FirstDerivativeC13S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[5, 3] += (this.FirstDerivativeC66HS2VoigtType2() * this.FirstDerivativeC13HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 5] += (this.FirstDerivativeC66S1VoigtType2() * this.FirstDerivativeC44S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 5] += (this.FirstDerivativeC66HS2VoigtType2() * this.FirstDerivativeC44HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[5, 4] += (this.FirstDerivativeC66S1VoigtType2() * this.FirstDerivativeC44S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[5, 4] += (this.FirstDerivativeC66HS2VoigtType2() * this.FirstDerivativeC44HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                #endregion

                #region Vector build

                SolutionVector[0] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtType2()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC11S1VoigtType2();
                SolutionVector[0] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtType2()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC11HS2VoigtType2();
                SolutionVector[1] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtType2()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC33S1VoigtType2();
                SolutionVector[1] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtType2()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC33HS2VoigtType2();
                SolutionVector[2] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtType2()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC12S1VoigtType2();
                SolutionVector[2] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtType2()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC12HS2VoigtType2();
                SolutionVector[3] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtType2()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC13S1VoigtType2();
                SolutionVector[3] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtType2()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC13HS2VoigtType2();
                SolutionVector[4] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtType2()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC44S1VoigtType2();
                SolutionVector[4] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtType2()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC44HS2VoigtType2();
                SolutionVector[5] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtType2()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC66S1VoigtType2();
                SolutionVector[5] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtType2()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC66HS2VoigtType2();

                #endregion
            }

            for (int n = 0; n < 6; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(6);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C33
        ///[2] C12
        ///[3] C13
        ///[4] C44
        ///[5] C66
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorVoigtType2Macroscopic(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(6, 6, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(6);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeC11S1VoigtType2() * this.FirstDerivativeC11S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 0] += (this.FirstDerivativeC11HS2VoigtType2() * this.FirstDerivativeC11HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeC33S1VoigtType2() * this.FirstDerivativeC33S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 1] += (this.FirstDerivativeC33HS2VoigtType2() * this.FirstDerivativeC33HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC33S1VoigtType2() * this.FirstDerivativeC11S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC33HS2VoigtType2() * this.FirstDerivativeC11HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC33S1VoigtType2() * this.FirstDerivativeC11S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC33HS2VoigtType2() * this.FirstDerivativeC11HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeC12S1VoigtType2() * this.FirstDerivativeC12S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 2] += (this.FirstDerivativeC12HS2VoigtType2() * this.FirstDerivativeC12HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeC12S1VoigtType2() * this.FirstDerivativeC11S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeC12HS2VoigtType2() * this.FirstDerivativeC11HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeC12S1VoigtType2() * this.FirstDerivativeC11S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeC12HS2VoigtType2() * this.FirstDerivativeC11HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeC12S1VoigtType2() * this.FirstDerivativeC33S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeC12HS2VoigtType2() * this.FirstDerivativeC33HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeC12S1VoigtType2() * this.FirstDerivativeC33S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeC12HS2VoigtType2() * this.FirstDerivativeC33HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[3, 3] += (this.FirstDerivativeC13S1VoigtType2() * this.FirstDerivativeC13S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 3] += (this.FirstDerivativeC13HS2VoigtType2() * this.FirstDerivativeC13HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeC13S1VoigtType2() * this.FirstDerivativeC11S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeC13HS2VoigtType2() * this.FirstDerivativeC11HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeC13S1VoigtType2() * this.FirstDerivativeC11S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeC13HS2VoigtType2() * this.FirstDerivativeC11HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeC13S1VoigtType2() * this.FirstDerivativeC33S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeC13HS2VoigtType2() * this.FirstDerivativeC33HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeC13S1VoigtType2() * this.FirstDerivativeC33S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeC13HS2VoigtType2() * this.FirstDerivativeC33HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeC13S1VoigtType2() * this.FirstDerivativeC12S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeC13HS2VoigtType2() * this.FirstDerivativeC12HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeC13S1VoigtType2() * this.FirstDerivativeC12S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeC13HS2VoigtType2() * this.FirstDerivativeC12HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[4, 4] += (this.FirstDerivativeC44S1VoigtType2() * this.FirstDerivativeC44S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 4] += (this.FirstDerivativeC44HS2VoigtType2() * this.FirstDerivativeC44HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeC44S1VoigtType2() * this.FirstDerivativeC11S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeC44HS2VoigtType2() * this.FirstDerivativeC11HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeC44S1VoigtType2() * this.FirstDerivativeC11S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeC44HS2VoigtType2() * this.FirstDerivativeC11HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeC44S1VoigtType2() * this.FirstDerivativeC33S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeC44HS2VoigtType2() * this.FirstDerivativeC33HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeC44S1VoigtType2() * this.FirstDerivativeC33S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeC44HS2VoigtType2() * this.FirstDerivativeC33HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeC44S1VoigtType2() * this.FirstDerivativeC12S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeC44HS2VoigtType2() * this.FirstDerivativeC12HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeC44S1VoigtType2() * this.FirstDerivativeC12S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeC44HS2VoigtType2() * this.FirstDerivativeC12HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeC44S1VoigtType2() * this.FirstDerivativeC13S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeC44HS2VoigtType2() * this.FirstDerivativeC13HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeC44S1VoigtType2() * this.FirstDerivativeC13S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeC44HS2VoigtType2() * this.FirstDerivativeC13HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[5, 5] += (this.FirstDerivativeC66S1VoigtType2() * this.FirstDerivativeC66S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[5, 5] += (this.FirstDerivativeC66HS2VoigtType2() * this.FirstDerivativeC66HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 5] += (this.FirstDerivativeC66S1VoigtType2() * this.FirstDerivativeC11S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 5] += (this.FirstDerivativeC66HS2VoigtType2() * this.FirstDerivativeC11HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[5, 0] += (this.FirstDerivativeC66S1VoigtType2() * this.FirstDerivativeC11S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[5, 0] += (this.FirstDerivativeC66HS2VoigtType2() * this.FirstDerivativeC11HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 5] += (this.FirstDerivativeC66S1VoigtType2() * this.FirstDerivativeC33S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 5] += (this.FirstDerivativeC66HS2VoigtType2() * this.FirstDerivativeC33HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[5, 1] += (this.FirstDerivativeC66S1VoigtType2() * this.FirstDerivativeC33S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[5, 1] += (this.FirstDerivativeC66HS2VoigtType2() * this.FirstDerivativeC33HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 5] += (this.FirstDerivativeC66S1VoigtType2() * this.FirstDerivativeC12S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 5] += (this.FirstDerivativeC66HS2VoigtType2() * this.FirstDerivativeC12HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[5, 2] += (this.FirstDerivativeC66S1VoigtType2() * this.FirstDerivativeC12S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[5, 2] += (this.FirstDerivativeC66HS2VoigtType2() * this.FirstDerivativeC12HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 5] += (this.FirstDerivativeC66S1VoigtType2() * this.FirstDerivativeC13S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 5] += (this.FirstDerivativeC66HS2VoigtType2() * this.FirstDerivativeC13HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[5, 3] += (this.FirstDerivativeC66S1VoigtType2() * this.FirstDerivativeC13S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[5, 3] += (this.FirstDerivativeC66HS2VoigtType2() * this.FirstDerivativeC13HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 5] += (this.FirstDerivativeC66S1VoigtType2() * this.FirstDerivativeC44S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 5] += (this.FirstDerivativeC66HS2VoigtType2() * this.FirstDerivativeC44HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[5, 4] += (this.FirstDerivativeC66S1VoigtType2() * this.FirstDerivativeC44S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[5, 4] += (this.FirstDerivativeC66HS2VoigtType2() * this.FirstDerivativeC44HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                #endregion

                #region Vector build

                SolutionVector[0] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtType2()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC11S1VoigtType2();
                SolutionVector[0] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtType2()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC11HS2VoigtType2();
                SolutionVector[1] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtType2()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC33S1VoigtType2();
                SolutionVector[1] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtType2()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC33HS2VoigtType2();
                SolutionVector[2] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtType2()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC12S1VoigtType2();
                SolutionVector[2] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtType2()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC12HS2VoigtType2();
                SolutionVector[3] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtType2()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC13S1VoigtType2();
                SolutionVector[3] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtType2()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC13HS2VoigtType2();
                SolutionVector[4] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtType2()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC44S1VoigtType2();
                SolutionVector[4] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtType2()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC44HS2VoigtType2();
                SolutionVector[5] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtType2()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC66S1VoigtType2();
                SolutionVector[5] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtType2()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC66HS2VoigtType2();

                #endregion
            }

            for (int n = 0; n < 6; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(6);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        #region First derivative

        public double FirstDerivativeC11S1VoigtType2()
        {
            double Ret = 4 * Math.Pow(this.C11, 2);
            Ret += 6 * Math.Pow(this.C12, 2);
            Ret += 24 * Math.Pow(this.C13, 2);
            Ret += 16 * this.C13 * this.C33;
            Ret += Math.Pow(this.C33, 2);
            Ret += 16 * this.C13 * this.C44;
            Ret -= 8 * this.C44 * this.C33;
            Ret -= 24 * Math.Pow(this.C44, 2);
            Ret += 8 * this.C13 * this.C66;
            Ret -= 4 * this.C33 * this.C66;
            Ret -= 24 * this.C44 * this.C66;
            Ret -= 6 * Math.Pow(this.C66, 2);

            double Sum = 4 * this.C12;
            Sum += 8 * this.C13;
            Sum += this.C33;
            Sum -= 4 * this.C44;
            Sum -= 2 * this.C66;

            Ret += 4 * this.C11 * Sum;

            Sum = 6 * this.C13;
            Sum += 2 * this.C33;
            Sum += 2 * this.C44;
            Sum += this.C66;

            Ret += 4 * this.C12 * Sum;
            Ret *= 3;

            double SumN = 2 * this.C11;
            SumN += 2 * this.C12;
            SumN += 4 * this.C13;
            SumN += this.C33;

            double N = Math.Pow(SumN, 2);

            SumN = 2 * this.C11;
            SumN -= this.C12;
            SumN -= 2 * this.C13;
            SumN += this.C33;
            SumN += 6 * this.C44;
            SumN += 3 * this.C66;

            N *= Math.Pow(SumN, 2);

            Ret /= N;

            return Ret;
        }

        public double FirstDerivativeC33S1VoigtType2()
        {
            double Ret = 4 * Math.Pow(this.C11, 2);
            Ret += 6 * Math.Pow(this.C12, 2);
            Ret += 24 * Math.Pow(this.C13, 2);
            Ret += 16 * this.C13 * this.C33;
            Ret += Math.Pow(this.C33, 2);
            Ret += 16 * this.C13 * this.C44;
            Ret -= 8 * this.C44 * this.C33;
            Ret -= 24 * Math.Pow(this.C44, 2);
            Ret += 8 * this.C13 * this.C66;
            Ret -= 4 * this.C33 * this.C66;
            Ret -= 24 * this.C44 * this.C66;
            Ret -= 6 * Math.Pow(this.C66, 2);

            double Sum = 4 * this.C12;
            Sum += 8 * this.C13;
            Sum += this.C33;
            Sum -= 4 * this.C44;
            Sum -= 2 * this.C66;

            Ret += 4 * this.C11 * Sum;

            Sum = 6 * this.C13;
            Sum += 2 * this.C33;
            Sum += 2 * this.C44;
            Sum += this.C66;

            Ret += 4 * this.C12 * Sum;
            Ret *= 3;

            double SumN = 2 * this.C11;
            SumN += 2 * this.C12;
            SumN += 4 * this.C13;
            SumN += this.C33;

            double N = Math.Pow(SumN, 2);

            SumN = 2 * this.C11;
            SumN -= this.C12;
            SumN -= 2 * this.C13;
            SumN += this.C33;
            SumN += 6 * this.C44;
            SumN += 3 * this.C66;

            N *= Math.Pow(SumN, 2);

            Ret /= 2 * N;

            return Ret;
        }

        public double FirstDerivativeC12S1VoigtType2()
        {
            double Ret = 12 * Math.Pow(this.C11, 2);
            Ret += 8 * Math.Pow(this.C12, 2);
            Ret += 32 * Math.Pow(this.C13, 2);
            Ret += 8 * this.C13 * this.C33;
            Ret += 3 * Math.Pow(this.C33, 2);
            Ret -= 32 * this.C13 * this.C44;
            Ret += 16 * this.C44 * this.C33;
            Ret += 48 * Math.Pow(this.C44, 2);
            Ret -= 16 * this.C13 * this.C66;
            Ret += 8 * this.C33 * this.C66;
            Ret += 48 * this.C44 * this.C66;
            Ret += 12 * Math.Pow(this.C66, 2);

            double Sum = 2 * this.C12;
            Sum += 4 * this.C13;
            Sum += 3 * this.C33;
            Sum += 8 * this.C44;
            Sum += 4 * this.C66;

            Ret += 4 * this.C11 * Sum;

            Sum = 8 * this.C13;
            Sum += this.C33;
            Sum -= 4 * this.C44;
            Sum -= 2 * this.C66;

            Ret += 4 * this.C12 * Sum;
            Ret *= -3;

            double SumN = 2 * this.C11;
            SumN += 2 * this.C12;
            SumN += 4 * this.C13;
            SumN += this.C33;

            double N = Math.Pow(SumN, 2);

            SumN = 2 * this.C11;
            SumN -= this.C12;
            SumN -= 2 * this.C13;
            SumN += this.C33;
            SumN += 6 * this.C44;
            SumN += 3 * this.C66;

            N *= Math.Pow(SumN, 2);

            Ret /= 2 * N;

            return Ret;
        }

        public double FirstDerivativeC13S1VoigtType2()
        {
            double Ret = 12 * Math.Pow(this.C11, 2);
            Ret += 8 * Math.Pow(this.C12, 2);
            Ret += 32 * Math.Pow(this.C13, 2);
            Ret += 8 * this.C13 * this.C33;
            Ret += 3 * Math.Pow(this.C33, 2);
            Ret -= 32 * this.C13 * this.C44;
            Ret += 16 * this.C44 * this.C33;
            Ret += 48 * Math.Pow(this.C44, 2);
            Ret -= 16 * this.C13 * this.C66;
            Ret += 8 * this.C33 * this.C66;
            Ret += 48 * this.C44 * this.C66;
            Ret += 12 * Math.Pow(this.C66, 2);

            double Sum = 2 * this.C12;
            Sum += 4 * this.C13;
            Sum += 3 * this.C33;
            Sum += 8 * this.C44;
            Sum += 4 * this.C66;

            Ret += 4 * this.C11 * Sum;

            Sum = 8 * this.C13;
            Sum += this.C33;
            Sum -= 4 * this.C44;
            Sum -= 2 * this.C66;

            Ret += 4 * this.C12 * Sum;
            Ret *= -3;

            double SumN = 2 * this.C11;
            SumN += 2 * this.C12;
            SumN += 4 * this.C13;
            SumN += this.C33;

            double N = Math.Pow(SumN, 2);

            SumN = 2 * this.C11;
            SumN -= this.C12;
            SumN -= 2 * this.C13;
            SumN += this.C33;
            SumN += 6 * this.C44;
            SumN += 3 * this.C66;

            N *= Math.Pow(SumN, 2);

            Ret /= N;

            return Ret;
        }

        public double FirstDerivativeC44S1VoigtType2()
        {
            double Ret = 15;

            double N = 2 * this.C11;
            N -= this.C12;
            N -= 2 * this.C13;
            N += this.C33;
            N += 6 * this.C44;
            N += 3 * this.C66;

            Ret /= Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeC66S1VoigtType2()
        {
            double Ret = 15;

            double N = 2 * this.C11;
            N -= this.C12;
            N -= 2 * this.C13;
            N += this.C33;
            N += 6 * this.C44;
            N += 3 * this.C66;

            Ret /= 2 * Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeC11HS2VoigtType2()
        {
            double Ret = -60;

            double N = 4 * this.C11;
            N -= 2 * this.C12;
            N -= 4 * this.C13;
            N += 2 * this.C33;
            N += 12 * this.C44;
            N += 6 * this.C66;

            Ret /= Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeC33HS2VoigtType2()
        {
            double Ret = -30;

            double N = 4 * this.C11;
            N -= 2 * this.C12;
            N -= 4 * this.C13;
            N += 2 * this.C33;
            N += 12 * this.C44;
            N += 6 * this.C66;

            Ret /= Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeC12HS2VoigtType2()
        {
            double Ret = 30;

            double N = 4 * this.C11;
            N -= 2 * this.C12;
            N -= 4 * this.C13;
            N += 2 * this.C33;
            N += 12 * this.C44;
            N += 6 * this.C66;

            Ret /= Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeC13HS2VoigtType2()
        {
            double Ret = 60;

            double N = 4 * this.C11;
            N -= 2 * this.C12;
            N -= 4 * this.C13;
            N += 2 * this.C33;
            N += 12 * this.C44;
            N += 6 * this.C66;

            Ret /= Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeC44HS2VoigtType2()
        {
            double Ret = -180;

            double N = 4 * this.C11;
            N -= 2 * this.C12;
            N -= 4 * this.C13;
            N += 2 * this.C33;
            N += 12 * this.C44;
            N += 6 * this.C66;

            Ret /= Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeC66HS2VoigtType2()
        {
            double Ret = -90;

            double N = 4 * this.C11;
            N -= 2 * this.C12;
            N -= 4 * this.C13;
            N += 2 * this.C33;
            N += 12 * this.C44;
            N += 6 * this.C66;

            Ret /= Math.Pow(N, 2);

            return Ret;
        }

        #endregion

        #endregion

        #region Type 3 (rhombic, monoclinic, triclinic)

        public double S1VoigtType3()
        {
            double Ret = this.C11 + this.C22 + this.C33;
            Ret += 4 * (this.C12 + this.C13 + this.C23);
            Ret -= 2 * (this.C44 + this.C55 + this.C66);

            Ret *= -3 / 2;

            double Sum = this.C11 + this.C22 + this.C33;
            Sum -= this.C12 + this.C13 + this.C23;
            Sum += 3 * (this.C44 + this.C55 + this.C66);

            double Sum1 = this.C11 + this.C22 + this.C33;
            Sum1 += 2 * (this.C12 + this.C13 + this.C23);

            Sum *= Sum1;

            Ret /= Sum;

            return Ret;
        }

        public double HS2VoigtType3()
        {
            double Sum = 2 * (this.C11 + this.C22 + this.C33);
            Sum -= 2 * (this.C12 + this.C13 + this.C23);
            Sum += 6 * (this.C44 + this.C55 + this.C66);

            double Ret = 15 / Sum;

            return Ret;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C22
        ///[2] C33
        ///[3] C12
        ///[4] C13
        ///[5] C23
        ///[6] C44
        ///[7] C55
        ///[8] C66
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorVoigtType3Classic(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(9, 9, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(9);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeC11S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 0] += (this.FirstDerivativeC11HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeC22S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 1] += (this.FirstDerivativeC22HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC22S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC22HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC22S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC22HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeC33S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 2] += (this.FirstDerivativeC33HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeC33S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeC33HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeC33S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeC33HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeC33S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeC33HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeC33S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeC33HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[3, 3] += (this.FirstDerivativeC12S1VoigtType3() * this.FirstDerivativeC12S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 3] += (this.FirstDerivativeC12HS2VoigtType3() * this.FirstDerivativeC12HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeC12S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeC12HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeC12S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeC12HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeC12S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeC12HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeC12S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeC12HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeC12S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeC12HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeC12S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeC12HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[4, 4] += (this.FirstDerivativeC13S1VoigtType3() * this.FirstDerivativeC13S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 4] += (this.FirstDerivativeC13HS2VoigtType3() * this.FirstDerivativeC13HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeC13S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeC13HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeC13S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeC13HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeC13S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeC13HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeC13S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeC13HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeC13S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeC13HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeC13S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeC13HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeC13S1VoigtType3() * this.FirstDerivativeC12S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeC13HS2VoigtType3() * this.FirstDerivativeC12HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeC13S1VoigtType3() * this.FirstDerivativeC12S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeC13HS2VoigtType3() * this.FirstDerivativeC12HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[5, 5] += (this.FirstDerivativeC23S1VoigtType3() * this.FirstDerivativeC23S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[5, 5] += (this.FirstDerivativeC23HS2VoigtType3() * this.FirstDerivativeC23HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 5] += (this.FirstDerivativeC23S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 5] += (this.FirstDerivativeC23HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[5, 0] += (this.FirstDerivativeC23S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[5, 0] += (this.FirstDerivativeC23HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 5] += (this.FirstDerivativeC23S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 5] += (this.FirstDerivativeC23HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[5, 1] += (this.FirstDerivativeC23S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[5, 1] += (this.FirstDerivativeC23HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 5] += (this.FirstDerivativeC23S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 5] += (this.FirstDerivativeC23HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[5, 2] += (this.FirstDerivativeC23S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[5, 2] += (this.FirstDerivativeC23HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 5] += (this.FirstDerivativeC23S1VoigtType3() * this.FirstDerivativeC12S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 5] += (this.FirstDerivativeC23HS2VoigtType3() * this.FirstDerivativeC12HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[5, 3] += (this.FirstDerivativeC23S1VoigtType3() * this.FirstDerivativeC12S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[5, 3] += (this.FirstDerivativeC23HS2VoigtType3() * this.FirstDerivativeC12HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 5] += (this.FirstDerivativeC23S1VoigtType3() * this.FirstDerivativeC13S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 5] += (this.FirstDerivativeC23HS2VoigtType3() * this.FirstDerivativeC13HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[5, 4] += (this.FirstDerivativeC23S1VoigtType3() * this.FirstDerivativeC13S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[5, 4] += (this.FirstDerivativeC23HS2VoigtType3() * this.FirstDerivativeC13HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[6, 6] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC44S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[6, 6] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC44HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 6] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 6] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[6, 0] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[6, 0] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 6] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 6] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[6, 1] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[6, 1] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 6] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 6] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[6, 2] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[6, 2] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 6] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC12S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 6] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC12HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[6, 3] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC12S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[6, 3] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC12HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 6] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC13S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 6] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC13HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[6, 4] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC13S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[6, 4] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC13HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[5, 6] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC23S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[5, 6] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC23HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[6, 5] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC23S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[6, 5] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC23HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[7, 7] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC55S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[7, 7] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC55HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 7] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 7] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[7, 0] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[7, 0] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 7] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 7] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[7, 1] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[7, 1] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 7] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 7] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[7, 2] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[7, 2] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 7] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC12S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 7] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC12HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[7, 3] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC12S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[7, 3] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC12HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 7] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC13S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 7] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC13HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[7, 4] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC13S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[7, 4] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC13HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[5, 7] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC23S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[5, 7] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC23HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[7, 5] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC23S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[7, 5] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC23HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[6, 7] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC44S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[6, 7] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC44HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[7, 6] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC44S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[7, 6] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC44HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[8, 8] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC66S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[8, 8] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC66HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 8] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 8] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[8, 0] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[8, 0] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 8] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 8] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[8, 1] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[8, 1] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 8] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 8] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[8, 2] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[8, 2] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 8] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC12S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 8] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC12HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[8, 3] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC12S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[8, 3] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC12HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 8] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC13S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 8] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC13HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[8, 4] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC13S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[8, 4] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC13HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[5, 8] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC23S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[5, 8] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC23HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[8, 5] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC23S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[8, 5] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC23HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[6, 8] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC44S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[6, 8] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC44HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[8, 6] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC44S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[8, 6] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC44HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[7, 8] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC55S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[7, 8] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC55HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[8, 7] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC55S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[8, 7] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC55HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                #endregion

                #region Vector build

                SolutionVector[0] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtType3()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC11S1VoigtType3();
                SolutionVector[0] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtType3()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC11HS2VoigtType3();
                SolutionVector[1] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtType3()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC22S1VoigtType3();
                SolutionVector[1] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtType3()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC22HS2VoigtType3();
                SolutionVector[2] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtType3()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC33S1VoigtType3();
                SolutionVector[2] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtType3()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC33HS2VoigtType3();
                SolutionVector[3] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtType3()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC12S1VoigtType3();
                SolutionVector[3] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtType3()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC12HS2VoigtType3();
                SolutionVector[4] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtType3()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC13S1VoigtType3();
                SolutionVector[4] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtType3()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC13HS2VoigtType3();
                SolutionVector[5] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtType3()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC23S1VoigtType3();
                SolutionVector[5] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtType3()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC23HS2VoigtType3();
                SolutionVector[6] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtType3()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC44S1VoigtType3();
                SolutionVector[6] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtType3()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC44HS2VoigtType3();
                SolutionVector[7] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtType3()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC55S1VoigtType3();
                SolutionVector[7] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtType3()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC55HS2VoigtType3();
                SolutionVector[8] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtType3()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC66S1VoigtType3();
                SolutionVector[8] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtType3()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC66HS2VoigtType3();

                #endregion
            }

            for (int n = 0; n < 9; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(9);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C22
        ///[2] C33
        ///[3] C12
        ///[4] C13
        ///[5] C23
        ///[6] C44
        ///[7] C55
        ///[8] C66
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorVoigtType3Macroscopic(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(9, 9, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(9);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeC11S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 0] += (this.FirstDerivativeC11HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeC22S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 1] += (this.FirstDerivativeC22HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC22S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC22HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC22S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC22HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeC33S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 2] += (this.FirstDerivativeC33HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeC33S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeC33HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeC33S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeC33HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeC33S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeC33HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeC33S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeC33HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[3, 3] += (this.FirstDerivativeC12S1VoigtType3() * this.FirstDerivativeC12S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 3] += (this.FirstDerivativeC12HS2VoigtType3() * this.FirstDerivativeC12HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeC12S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeC12HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeC12S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeC12HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeC12S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeC12HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeC12S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeC12HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeC12S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeC12HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeC12S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeC12HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[4, 4] += (this.FirstDerivativeC13S1VoigtType3() * this.FirstDerivativeC13S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 4] += (this.FirstDerivativeC13HS2VoigtType3() * this.FirstDerivativeC13HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeC13S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeC13HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeC13S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeC13HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeC13S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeC13HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeC13S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeC13HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeC13S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeC13HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeC13S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeC13HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeC13S1VoigtType3() * this.FirstDerivativeC12S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeC13HS2VoigtType3() * this.FirstDerivativeC12HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeC13S1VoigtType3() * this.FirstDerivativeC12S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeC13HS2VoigtType3() * this.FirstDerivativeC12HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[5, 5] += (this.FirstDerivativeC23S1VoigtType3() * this.FirstDerivativeC23S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[5, 5] += (this.FirstDerivativeC23HS2VoigtType3() * this.FirstDerivativeC23HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 5] += (this.FirstDerivativeC23S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 5] += (this.FirstDerivativeC23HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[5, 0] += (this.FirstDerivativeC23S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[5, 0] += (this.FirstDerivativeC23HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 5] += (this.FirstDerivativeC23S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 5] += (this.FirstDerivativeC23HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[5, 1] += (this.FirstDerivativeC23S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[5, 1] += (this.FirstDerivativeC23HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 5] += (this.FirstDerivativeC23S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 5] += (this.FirstDerivativeC23HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[5, 2] += (this.FirstDerivativeC23S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[5, 2] += (this.FirstDerivativeC23HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 5] += (this.FirstDerivativeC23S1VoigtType3() * this.FirstDerivativeC12S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 5] += (this.FirstDerivativeC23HS2VoigtType3() * this.FirstDerivativeC12HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[5, 3] += (this.FirstDerivativeC23S1VoigtType3() * this.FirstDerivativeC12S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[5, 3] += (this.FirstDerivativeC23HS2VoigtType3() * this.FirstDerivativeC12HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 5] += (this.FirstDerivativeC23S1VoigtType3() * this.FirstDerivativeC13S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 5] += (this.FirstDerivativeC23HS2VoigtType3() * this.FirstDerivativeC13HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[5, 4] += (this.FirstDerivativeC23S1VoigtType3() * this.FirstDerivativeC13S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[5, 4] += (this.FirstDerivativeC23HS2VoigtType3() * this.FirstDerivativeC13HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[6, 6] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC44S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[6, 6] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC44HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 6] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 6] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[6, 0] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[6, 0] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 6] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 6] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[6, 1] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[6, 1] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 6] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 6] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[6, 2] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[6, 2] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 6] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC12S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 6] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC12HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[6, 3] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC12S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[6, 3] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC12HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 6] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC13S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 6] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC13HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[6, 4] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC13S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[6, 4] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC13HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[5, 6] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC23S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[5, 6] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC23HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[6, 5] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC23S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[6, 5] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC23HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[7, 7] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC55S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[7, 7] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC55HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 7] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 7] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[7, 0] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[7, 0] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 7] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 7] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[7, 1] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[7, 1] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 7] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 7] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[7, 2] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[7, 2] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 7] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC12S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 7] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC12HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[7, 3] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC12S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[7, 3] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC12HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 7] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC13S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 7] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC13HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[7, 4] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC13S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[7, 4] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC13HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[5, 7] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC23S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[5, 7] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC23HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[7, 5] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC23S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[7, 5] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC23HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[6, 7] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC44S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[6, 7] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC44HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[7, 6] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC44S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[7, 6] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC44HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[8, 8] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC66S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[8, 8] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC66HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 8] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 8] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[8, 0] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[8, 0] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 8] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 8] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[8, 1] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[8, 1] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 8] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 8] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[8, 2] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[8, 2] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 8] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC12S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 8] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC12HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[8, 3] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC12S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[8, 3] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC12HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 8] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC13S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 8] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC13HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[8, 4] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC13S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[8, 4] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC13HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[5, 8] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC23S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[5, 8] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC23HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[8, 5] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC23S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[8, 5] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC23HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[6, 8] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC44S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[6, 8] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC44HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[8, 6] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC44S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[8, 6] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC44HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[7, 8] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC55S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[7, 8] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC55HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[8, 7] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC55S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[8, 7] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC55HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                #endregion

                #region Vector build

                SolutionVector[0] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtType3()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC11S1VoigtType3();
                SolutionVector[0] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtType3()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC11HS2VoigtType3();
                SolutionVector[1] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtType3()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC22S1VoigtType3();
                SolutionVector[1] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtType3()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC22HS2VoigtType3();
                SolutionVector[2] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtType3()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC33S1VoigtType3();
                SolutionVector[2] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtType3()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC33HS2VoigtType3();
                SolutionVector[3] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtType3()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC12S1VoigtType3();
                SolutionVector[3] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtType3()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC12HS2VoigtType3();
                SolutionVector[4] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtType3()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC13S1VoigtType3();
                SolutionVector[4] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtType3()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC13HS2VoigtType3();
                SolutionVector[5] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtType3()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC23S1VoigtType3();
                SolutionVector[5] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtType3()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC23HS2VoigtType3();
                SolutionVector[6] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtType3()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC44S1VoigtType3();
                SolutionVector[6] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtType3()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC44HS2VoigtType3();
                SolutionVector[7] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtType3()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC55S1VoigtType3();
                SolutionVector[7] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtType3()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC55HS2VoigtType3();
                SolutionVector[8] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtType3()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC66S1VoigtType3();
                SolutionVector[8] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtType3()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC66HS2VoigtType3();

                #endregion
            }

            for (int n = 0; n < 9; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(9);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        #region First derivative

        public double FirstDerivativeC11S1VoigtType3()
        {
            double Sum = this.C11;
            Sum += 2 * this.C12;
            Sum += 2 * this.C13;
            Sum += this.C22;
            Sum += 2 * this.C23;
            Sum += this.C33;

            double Mult = this.C11;
            Mult += 4 * this.C12;
            Mult += 4 * this.C13;
            Mult += this.C22;
            Mult += 4 * this.C23;
            Mult += this.C33;
            Mult -= 2 * this.C44;
            Mult -= 2 * this.C55;
            Mult -= 2 * this.C66;

            double Ret = -Sum * Mult; ;

            Sum = this.C11;
            Sum += 2 * this.C12;
            Sum += 2 * this.C13;
            Sum += this.C22;
            Sum += 2 * this.C23;
            Sum += this.C33;

            Mult = this.C11;
            Mult -= this.C12;
            Mult -= this.C13;
            Mult += this.C22;
            Mult -= this.C23;
            Mult += this.C33;
            Mult += 3 * this.C44;
            Mult += 3 * this.C55;
            Mult += 3 * this.C66;

            Ret += Sum * Mult;

            Sum = this.C11;
            Sum += 4 * this.C12;
            Sum += 4 * this.C13;
            Sum += this.C22;
            Sum += 4 * this.C23;
            Sum += this.C33;
            Sum -= 2 * this.C44;
            Sum -= 2 * this.C55;
            Sum -= 2 * this.C66;

            Mult = this.C11;
            Mult -= this.C12;
            Mult -= this.C13;
            Mult += this.C22;
            Mult -= this.C23;
            Mult += this.C33;
            Mult += 3 * this.C44;
            Mult += 3 * this.C55;
            Mult += 3 * this.C66;

            Ret -= Sum * Mult;

            Sum = this.C11;
            Sum += 2 * this.C12;
            Sum += 2 * this.C13;
            Sum += this.C22;
            Sum += 2 * this.C23;
            Sum += this.C33;

            Mult = this.C11;
            Mult -= this.C12;
            Mult -= this.C13;
            Mult += this.C22;
            Mult -= this.C23;
            Mult += this.C33;
            Mult += 3 * this.C44;
            Mult += 3 * this.C55;
            Mult += 3 * this.C66;

            Ret /= Math.Pow(Sum, 2) * Math.Pow(Mult, 2);

            return Ret;
        }

        public double FirstDerivativeC22S1VoigtType3()
        {
            double Sum = this.C11;
            Sum += 2 * this.C12;
            Sum += 2 * this.C13;
            Sum += this.C22;
            Sum += 2 * this.C23;
            Sum += this.C33;

            double Mult = this.C11;
            Mult += 4 * this.C12;
            Mult += 4 * this.C13;
            Mult += this.C22;
            Mult += 4 * this.C23;
            Mult += this.C33;
            Mult -= 2 * this.C44;
            Mult -= 2 * this.C55;
            Mult -= 2 * this.C66;

            double Ret = -Sum * Mult; ;

            Sum = this.C11;
            Sum += 2 * this.C12;
            Sum += 2 * this.C13;
            Sum += this.C22;
            Sum += 2 * this.C23;
            Sum += this.C33;

            Mult = this.C11;
            Mult -= this.C12;
            Mult -= this.C13;
            Mult += this.C22;
            Mult -= this.C23;
            Mult += this.C33;
            Mult += 3 * this.C44;
            Mult += 3 * this.C55;
            Mult += 3 * this.C66;

            Ret += Sum * Mult;

            Sum = this.C11;
            Sum += 4 * this.C12;
            Sum += 4 * this.C13;
            Sum += this.C22;
            Sum += 4 * this.C23;
            Sum += this.C33;
            Sum -= 2 * this.C44;
            Sum -= 2 * this.C55;
            Sum -= 2 * this.C66;

            Mult = this.C11;
            Mult -= this.C12;
            Mult -= this.C13;
            Mult += this.C22;
            Mult -= this.C23;
            Mult += this.C33;
            Mult += 3 * this.C44;
            Mult += 3 * this.C55;
            Mult += 3 * this.C66;

            Ret -= Sum * Mult;

            Sum = this.C11;
            Sum += 2 * this.C12;
            Sum += 2 * this.C13;
            Sum += this.C22;
            Sum += 2 * this.C23;
            Sum += this.C33;

            Mult = this.C11;
            Mult -= this.C12;
            Mult -= this.C13;
            Mult += this.C22;
            Mult -= this.C23;
            Mult += this.C33;
            Mult += 3 * this.C44;
            Mult += 3 * this.C55;
            Mult += 3 * this.C66;

            Ret /= Math.Pow(Sum, 2) * Math.Pow(Mult, 2);

            return Ret;
        }

        public double FirstDerivativeC33S1VoigtType3()
        {
            double Sum = this.C11;
            Sum += 2 * this.C12;
            Sum += 2 * this.C13;
            Sum += this.C22;
            Sum += 2 * this.C23;
            Sum += this.C33;

            double Mult = this.C11;
            Mult += 4 * this.C12;
            Mult += 4 * this.C13;
            Mult += this.C22;
            Mult += 4 * this.C23;
            Mult += this.C33;
            Mult -= 2 * this.C44;
            Mult -= 2 * this.C55;
            Mult -= 2 * this.C66;

            double Ret = -Sum * Mult; ;

            Sum = this.C11;
            Sum += 2 * this.C12;
            Sum += 2 * this.C13;
            Sum += this.C22;
            Sum += 2 * this.C23;
            Sum += this.C33;

            Mult = this.C11;
            Mult -= this.C12;
            Mult -= this.C13;
            Mult += this.C22;
            Mult -= this.C23;
            Mult += this.C33;
            Mult += 3 * this.C44;
            Mult += 3 * this.C55;
            Mult += 3 * this.C66;

            Ret += Sum * Mult;

            Sum = this.C11;
            Sum += 4 * this.C12;
            Sum += 4 * this.C13;
            Sum += this.C22;
            Sum += 4 * this.C23;
            Sum += this.C33;
            Sum -= 2 * this.C44;
            Sum -= 2 * this.C55;
            Sum -= 2 * this.C66;

            Mult = this.C11;
            Mult -= this.C12;
            Mult -= this.C13;
            Mult += this.C22;
            Mult -= this.C23;
            Mult += this.C33;
            Mult += 3 * this.C44;
            Mult += 3 * this.C55;
            Mult += 3 * this.C66;

            Ret -= Sum * Mult;

            Sum = this.C11;
            Sum += 2 * this.C12;
            Sum += 2 * this.C13;
            Sum += this.C22;
            Sum += 2 * this.C23;
            Sum += this.C33;

            Mult = this.C11;
            Mult -= this.C12;
            Mult -= this.C13;
            Mult += this.C22;
            Mult -= this.C23;
            Mult += this.C33;
            Mult += 3 * this.C44;
            Mult += 3 * this.C55;
            Mult += 3 * this.C66;

            Ret /= Math.Pow(Sum, 2) * Math.Pow(Mult, 2);

            return Ret;
        }

        public double FirstDerivativeC12S1VoigtType3()
        {
            double Sum = this.C11;
            Sum += 2 * this.C12;
            Sum += 2 * this.C13;
            Sum += this.C22;
            Sum += 2 * this.C23;
            Sum += this.C33;

            double Mult = this.C11;
            Mult += 4 * this.C12;
            Mult += 4 * this.C13;
            Mult += this.C22;
            Mult += 4 * this.C23;
            Mult += this.C33;
            Mult -= 2 * this.C44;
            Mult -= 2 * this.C55;
            Mult -= 2 * this.C66;

            double Ret = Sum * Mult; ;

            Sum = this.C11;
            Sum += 2 * this.C12;
            Sum += 2 * this.C13;
            Sum += this.C22;
            Sum += 2 * this.C23;
            Sum += this.C33;

            Mult = this.C11;
            Mult -= this.C12;
            Mult -= this.C13;
            Mult += this.C22;
            Mult -= this.C23;
            Mult += this.C33;
            Mult += 3 * this.C44;
            Mult += 3 * this.C55;
            Mult += 3 * this.C66;

            Ret += 4 * Sum * Mult;

            Sum = this.C11;
            Sum += 4 * this.C12;
            Sum += 4 * this.C13;
            Sum += this.C22;
            Sum += 4 * this.C23;
            Sum += this.C33;
            Sum -= 2 * this.C44;
            Sum -= 2 * this.C55;
            Sum -= 2 * this.C66;

            Mult = this.C11;
            Mult -= this.C12;
            Mult -= this.C13;
            Mult += this.C22;
            Mult -= this.C23;
            Mult += this.C33;
            Mult += 3 * this.C44;
            Mult += 3 * this.C55;
            Mult += 3 * this.C66;

            Ret -= 2 * Sum * Mult;

            Sum = this.C11;
            Sum += 2 * this.C12;
            Sum += 2 * this.C13;
            Sum += this.C22;
            Sum += 2 * this.C23;
            Sum += this.C33;

            Mult = this.C11;
            Mult -= this.C12;
            Mult -= this.C13;
            Mult += this.C22;
            Mult -= this.C23;
            Mult += this.C33;
            Mult += 3 * this.C44;
            Mult += 3 * this.C55;
            Mult += 3 * this.C66;

            Ret /= Math.Pow(Sum, 2) * Math.Pow(Mult, 2);

            return Ret;
        }

        public double FirstDerivativeC13S1VoigtType3()
        {
            double Sum = this.C11;
            Sum += 2 * this.C12;
            Sum += 2 * this.C13;
            Sum += this.C22;
            Sum += 2 * this.C23;
            Sum += this.C33;

            double Mult = this.C11;
            Mult += 4 * this.C12;
            Mult += 4 * this.C13;
            Mult += this.C22;
            Mult += 4 * this.C23;
            Mult += this.C33;
            Mult -= 2 * this.C44;
            Mult -= 2 * this.C55;
            Mult -= 2 * this.C66;

            double Ret = Sum * Mult; ;

            Sum = this.C11;
            Sum += 2 * this.C12;
            Sum += 2 * this.C13;
            Sum += this.C22;
            Sum += 2 * this.C23;
            Sum += this.C33;

            Mult = this.C11;
            Mult -= this.C12;
            Mult -= this.C13;
            Mult += this.C22;
            Mult -= this.C23;
            Mult += this.C33;
            Mult += 3 * this.C44;
            Mult += 3 * this.C55;
            Mult += 3 * this.C66;

            Ret += 4 * Sum * Mult;

            Sum = this.C11;
            Sum += 4 * this.C12;
            Sum += 4 * this.C13;
            Sum += this.C22;
            Sum += 4 * this.C23;
            Sum += this.C33;
            Sum -= 2 * this.C44;
            Sum -= 2 * this.C55;
            Sum -= 2 * this.C66;

            Mult = this.C11;
            Mult -= this.C12;
            Mult -= this.C13;
            Mult += this.C22;
            Mult -= this.C23;
            Mult += this.C33;
            Mult += 3 * this.C44;
            Mult += 3 * this.C55;
            Mult += 3 * this.C66;

            Ret -= 2 * Sum * Mult;

            Sum = this.C11;
            Sum += 2 * this.C12;
            Sum += 2 * this.C13;
            Sum += this.C22;
            Sum += 2 * this.C23;
            Sum += this.C33;

            Mult = this.C11;
            Mult -= this.C12;
            Mult -= this.C13;
            Mult += this.C22;
            Mult -= this.C23;
            Mult += this.C33;
            Mult += 3 * this.C44;
            Mult += 3 * this.C55;
            Mult += 3 * this.C66;

            Ret /= Math.Pow(Sum, 2) * Math.Pow(Mult, 2);

            return Ret;
        }

        public double FirstDerivativeC23S1VoigtType3()
        {
            double Sum = this.C11;
            Sum += 2 * this.C12;
            Sum += 2 * this.C13;
            Sum += this.C22;
            Sum += 2 * this.C23;
            Sum += this.C33;

            double Mult = this.C11;
            Mult += 4 * this.C12;
            Mult += 4 * this.C13;
            Mult += this.C22;
            Mult += 4 * this.C23;
            Mult += this.C33;
            Mult -= 2 * this.C44;
            Mult -= 2 * this.C55;
            Mult -= 2 * this.C66;

            double Ret = Sum * Mult; ;

            Sum = this.C11;
            Sum += 2 * this.C12;
            Sum += 2 * this.C13;
            Sum += this.C22;
            Sum += 2 * this.C23;
            Sum += this.C33;

            Mult = this.C11;
            Mult -= this.C12;
            Mult -= this.C13;
            Mult += this.C22;
            Mult -= this.C23;
            Mult += this.C33;
            Mult += 3 * this.C44;
            Mult += 3 * this.C55;
            Mult += 3 * this.C66;

            Ret += 4 * Sum * Mult;

            Sum = this.C11;
            Sum += 4 * this.C12;
            Sum += 4 * this.C13;
            Sum += this.C22;
            Sum += 4 * this.C23;
            Sum += this.C33;
            Sum -= 2 * this.C44;
            Sum -= 2 * this.C55;
            Sum -= 2 * this.C66;

            Mult = this.C11;
            Mult -= this.C12;
            Mult -= this.C13;
            Mult += this.C22;
            Mult -= this.C23;
            Mult += this.C33;
            Mult += 3 * this.C44;
            Mult += 3 * this.C55;
            Mult += 3 * this.C66;

            Ret -= 2 * Sum * Mult;

            Sum = this.C11;
            Sum += 2 * this.C12;
            Sum += 2 * this.C13;
            Sum += this.C22;
            Sum += 2 * this.C23;
            Sum += this.C33;

            Mult = this.C11;
            Mult -= this.C12;
            Mult -= this.C13;
            Mult += this.C22;
            Mult -= this.C23;
            Mult += this.C33;
            Mult += 3 * this.C44;
            Mult += 3 * this.C55;
            Mult += 3 * this.C66;

            Ret /= Math.Pow(Sum, 2) * Math.Pow(Mult, 2);

            return Ret;
        }

        public double FirstDerivativeC44S1VoigtType3()
        {
            double Ret = -5;

            double N = this.C11;
            N -= this.C12;
            N -= this.C13;
            N += this.C22;
            N -= this.C23;
            N += this.C33;
            N += 3 * this.C44;
            N += 3 * this.C55;
            N += 3 * this.C66;

            Ret /= Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeC55S1VoigtType3()
        {
            double Ret = -5;

            double N = this.C11;
            N -= this.C12;
            N -= this.C13;
            N += this.C22;
            N -= this.C23;
            N += this.C33;
            N += 3 * this.C44;
            N += 3 * this.C55;
            N += 3 * this.C66;

            Ret /= Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeC66S1VoigtType3()
        {
            double Ret = -5;

            double N = this.C11;
            N -= this.C12;
            N -= this.C13;
            N += this.C22;
            N -= this.C23;
            N += this.C33;
            N += 3 * this.C44;
            N += 3 * this.C55;
            N += 3 * this.C66;

            Ret /= Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeC11HS2VoigtType3()
        {
            double Ret = -15;

            double N = this.C11 - this.C12 - this.C13 + this.C22 - this.C23 + this.C33 + (3 * this.C44) + (3 * this.C55) + (3 * this.C66);

            Ret /= 2 * Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeC22HS2VoigtType3()
        {
            double Ret = -15;

            double N = this.C11 - this.C12 - this.C13 + this.C22 - this.C23 + this.C33 + (3 * this.C44) + (3 * this.C55) + (3 * this.C66);

            Ret /= 2 * Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeC33HS2VoigtType3()
        {
            double Ret = -15;

            double N = this.C11 - this.C12 - this.C13 + this.C22 - this.C23 + this.C33 + (3 * this.C44) + (3 * this.C55) + (3 * this.C66);

            Ret /= 2 * Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeC12HS2VoigtType3()
        {
            double Ret = 15;

            double N = this.C11 - this.C12 - this.C13 + this.C22 - this.C23 + this.C33 + (3 * this.C44) + (3 * this.C55) + (3 * this.C66);

            Ret /= 2 * Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeC13HS2VoigtType3()
        {
            double Ret = 15;

            double N = this.C11 - this.C12 - this.C13 + this.C22 - this.C23 + this.C33 + (3 * this.C44) + (3 * this.C55) + (3 * this.C66);

            Ret /= 2 * Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeC23HS2VoigtType3()
        {
            double Ret = 15;

            double N = this.C11 - this.C12 - this.C13 + this.C22 - this.C23 + this.C33 + (3 * this.C44) + (3 * this.C55) + (3 * this.C66);

            Ret /= 2 * Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeC44HS2VoigtType3()
        {
            double Ret = -45;

            double N = this.C11 - this.C12 - this.C13 + this.C22 - this.C23 + this.C33 + (3 * this.C44) + (3 * this.C55) + (3 * this.C66);

            Ret /= 2 * Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeC55HS2VoigtType3()
        {
            double Ret = -45;

            double N = this.C11 - this.C12 - this.C13 + this.C22 - this.C23 + this.C33 + (3 * this.C44) + (3 * this.C55) + (3 * this.C66);

            Ret /= 2 * Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeC66HS2VoigtType3()
        {
            double Ret = -45;

            double N = this.C11 - this.C12 - this.C13 + this.C22 - this.C23 + this.C33 + (3 * this.C44) + (3 * this.C55) + (3 * this.C66);

            Ret /= 2 * Math.Pow(N, 2);

            return Ret;
        }

        #endregion

        #endregion

        #endregion

        #region Reuss

        #region Cubic

        public double S1ReussCubic(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Sum = Math.Pow(hKL.K, 2) * Math.Pow(hKL.L, 2);
            Sum += Math.Pow(hKL.H, 2) * Math.Pow(hKL.L, 2);
            Sum += Math.Pow(hKL.H, 2) * Math.Pow(hKL.K, 2);
            Sum *= (this.S11 - this.S44) / 2;

            double Sum1 = Math.Pow(hKL.H, 4) + Math.Pow(hKL.K, 4) + Math.Pow(hKL.L, 4);
            Sum1 += Math.Pow(hKL.K, 2) * Math.Pow(hKL.L, 2);
            Sum1 += Math.Pow(hKL.H, 2) * Math.Pow(hKL.L, 2);
            Sum1 += Math.Pow(hKL.H, 2) * Math.Pow(hKL.K, 2);
            Sum1 *= this.S12;

            double N = Math.Pow(Math.Pow(hKL.H, 2) + Math.Pow(hKL.K, 2) + Math.Pow(hKL.L, 2), 2);

            double Ret = Sum + Sum1;
            Ret /= N;

            return Ret;
        }

        public double HS2ReussCubic(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Sum = Math.Pow(hKL.H, 4) + Math.Pow(hKL.K, 4) + Math.Pow(hKL.L, 4);
            Sum *= this.S11;

            double Sum1 = Math.Pow(hKL.K, 2) * Math.Pow(hKL.L, 2);
            Sum1 += Math.Pow(hKL.H, 2) * Math.Pow(hKL.L, 2);
            Sum1 += Math.Pow(hKL.H, 2) * Math.Pow(hKL.K, 2);
            Sum1 *= (2 * this.S11) - this.S44;

            double N = Math.Pow(Math.Pow(hKL.H, 2) + Math.Pow(hKL.K, 2) + Math.Pow(hKL.L, 2), 2);

            double Tmp = Sum + Sum1;
            Tmp /= N;

            double Ret = Tmp - this.S1ReussCubic(hKL);

            return Ret;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C12
        ///[2] C44
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorReussCubicClassic(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeS11S1ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 0] += (this.FirstDerivativeS11HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeS12S1ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 1] += (this.FirstDerivativeS12HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeS12S1ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeS12HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeS12S1ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeS12HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeS44S1ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44S1ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 2] += (this.FirstDerivativeS44HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeS44S1ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeS44HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeS44S1ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeS44HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeS44S1ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeS44HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeS44S1ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeS44HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                #endregion

                #region Vector build

                SolutionVector[0] += ((this.DiffractionConstants[n].ClassicS1 - this.S1ReussCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeS11S1ReussCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[0] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2ReussCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeS11HS2ReussCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[1] += ((this.DiffractionConstants[n].ClassicS1 - this.S1ReussCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeS12S1ReussCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[1] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2ReussCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeS12HS2ReussCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[2] += ((this.DiffractionConstants[n].ClassicS1 - this.S1ReussCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeS44S1ReussCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[2] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2ReussCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeS44HS2ReussCubic(this.DiffractionConstants[n].UsedReflex);

                #endregion
            }

            for (int n = 0; n < 3; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C12
        ///[2] C44
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorReussCubicMacroscopic(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeS11S1ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 0] += (this.FirstDerivativeS11HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeS12S1ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 1] += (this.FirstDerivativeS12HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeS12S1ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeS12HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeS12S1ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeS12HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeS44S1ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44S1ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 2] += (this.FirstDerivativeS44HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeS44S1ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeS44HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeS44S1ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeS44HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeS44S1ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeS44HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeS44S1ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeS44HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                #endregion

                #region Vector build

                SolutionVector[0] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1ReussCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeS11S1ReussCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[0] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2ReussCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeS11HS2ReussCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[1] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1ReussCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeS12S1ReussCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[1] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2ReussCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeS12HS2ReussCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[2] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1ReussCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeS44S1ReussCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[2] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2ReussCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeS44HS2ReussCubic(this.DiffractionConstants[n].UsedReflex);

                #endregion
            }

            for (int n = 0; n < 3; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        #region First derivative

        public double FirstDerivativeS11S1ReussCubic(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = Math.Pow(hKL.K, 2) * Math.Pow(hKL.L, 2);
            Ret += Math.Pow(hKL.H, 2) * Math.Pow(hKL.L, 2);
            Ret += Math.Pow(hKL.H, 2) * Math.Pow(hKL.K, 2);

            double N = Math.Pow(Math.Pow(hKL.H, 2) + Math.Pow(hKL.K, 2) + Math.Pow(hKL.L, 2), 2);

            Ret /= 2 * N;

            return Ret;
        }

        public double FirstDerivativeS12S1ReussCubic(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = Math.Pow(hKL.H, 4) + Math.Pow(hKL.K, 4) + Math.Pow(hKL.L, 4);
            Ret += Math.Pow(hKL.K, 2) * Math.Pow(hKL.L, 2);
            Ret += Math.Pow(hKL.H, 2) * Math.Pow(hKL.L, 2);
            Ret += Math.Pow(hKL.H, 2) * Math.Pow(hKL.K, 2);

            double N = Math.Pow(Math.Pow(hKL.H, 2) + Math.Pow(hKL.K, 2) + Math.Pow(hKL.L, 2), 2);

            Ret /= N;

            return Ret;
        }

        public double FirstDerivativeS44S1ReussCubic(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = -Math.Pow(hKL.K, 2) * Math.Pow(hKL.L, 2);
            Ret += Math.Pow(hKL.H, 2) * Math.Pow(hKL.L, 2);
            Ret += Math.Pow(hKL.H, 2) * Math.Pow(hKL.K, 2);

            double N = Math.Pow(Math.Pow(hKL.H, 2) + Math.Pow(hKL.K, 2) + Math.Pow(hKL.L, 2), 2);

            Ret /= 2 * N;

            return Ret;
        }

        public double FirstDerivativeS11HS2ReussCubic(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Sum = Math.Pow(hKL.H, 4) + Math.Pow(hKL.K, 4) + Math.Pow(hKL.L, 4);

            double N = Math.Pow(Math.Pow(hKL.H, 2) + Math.Pow(hKL.K, 2) + Math.Pow(hKL.L, 2), 2);

            double Ret = Sum / N;
            Ret -= this.FirstDerivativeS11S1ReussCubic(hKL);

            return Ret;
        }

        public double FirstDerivativeS12HS2ReussCubic(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Sum = Math.Pow(hKL.K, 2) * Math.Pow(hKL.L, 2);
            Sum += Math.Pow(hKL.H, 2) * Math.Pow(hKL.L, 2);
            Sum += Math.Pow(hKL.H, 2) * Math.Pow(hKL.K, 2);
            Sum *= 2;

            double N = Math.Pow(Math.Pow(hKL.H, 2) + Math.Pow(hKL.K, 2) + Math.Pow(hKL.L, 2), 2);

            double Ret = Sum / N;
            Ret -= FirstDerivativeS12S1ReussCubic(hKL);

            return Ret;
        }

        public double FirstDerivativeS44HS2ReussCubic(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Sum = Math.Pow(hKL.K, 2) * Math.Pow(hKL.L, 2);
            Sum += Math.Pow(hKL.H, 2) * Math.Pow(hKL.L, 2);
            Sum += Math.Pow(hKL.H, 2) * Math.Pow(hKL.K, 2);

            double N = Math.Pow(Math.Pow(hKL.H, 2) + Math.Pow(hKL.K, 2) + Math.Pow(hKL.L, 2), 2);

            double Ret = Sum / N;
            Ret -= FirstDerivativeS44S1ReussCubic(hKL);

            return Ret;
        }

        #endregion

        #endregion

        #region Hexagonal

        public double S1ReussHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = Math.Pow(hKL.H, 2) + (hKL.H * hKL.K) + Math.Pow(hKL.K, 2);
            Ret *= 6 * Math.Pow(hKL.L, 2);
            Ret *= this.S11 + this.S22 - this.S44;

            double Sum = Math.Pow(hKL.H, 2) + (hKL.H * hKL.K) + Math.Pow(hKL.K, 2);
            Sum *= ( 4 * Math.Pow(hKL.H, 2)) + (4 * (hKL.H * hKL.K)) + ( 4 * Math.Pow(hKL.K, 2)) + (3 * Math.Pow(hKL.L, 2));

            Ret += 2 * this.S12 * Sum;

            Sum = 8 * Math.Pow(hKL.H, 4);
            Sum += 16 * Math.Pow(hKL.H, 3) * hKL.K;
            Sum += 24 * Math.Pow(hKL.H, 2) * Math.Pow(hKL.K, 2);
            Sum += 16 * hKL.H * Math.Pow(hKL.K, 3);
            Sum += 8 * Math.Pow(hKL.K, 4);
            Sum += 6 * Math.Pow(hKL.H, 2) * Math.Pow(hKL.L, 2);
            Sum += 6 * hKL.H * hKL.K * Math.Pow(hKL.L, 2);
            Sum += 6 * Math.Pow(hKL.K, 2) * Math.Pow(hKL.L, 2);
            Sum += 9 * Math.Pow(hKL.L, 4);

            Ret += Sum * this.S13;

            double N = 4 * Math.Pow(hKL.H, 2);
            N += 4 * Math.Pow(hKL.K, 2);
            N += 4 * Math.Pow(hKL.L, 2);
            N += 4 * hKL.H * hKL.K;

            Ret /= Math.Pow(N, 2);

            return Ret;
        }

        public double HS2ReussHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = Math.Pow(Math.Pow(hKL.H, 2) + (hKL.H * hKL.K) + Math.Pow(hKL.K, 2), 2);
            Ret *= 16 * this.S11;
            Ret += 9 * Math.Pow(hKL.L, 4) * this.S33;

            double Sum = Math.Pow(hKL.H, 2) + (hKL.H * hKL.K) + Math.Pow(hKL.K, 2);
            Sum *= 12 * Math.Pow(hKL.L, 2);
            Sum *= this.S13 + this.S44;

            Ret += Sum;

            double N = 4 * Math.Pow(hKL.H, 2);
            N += 4 * Math.Pow(hKL.K, 2);
            N += 4 * Math.Pow(hKL.L, 2);
            N += 4 * hKL.H * hKL.K;

            Ret /= Math.Pow(N, 2);

            Ret -= this.S1ReussHexagonal(hKL);

            return Ret;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C33
        ///[2] C12
        ///[3] C13
        ///[4] C44
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorReussHexagonalClassic(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(5, 5, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(5);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 0] += (this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 1] += (this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 2] += (this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[3, 3] += (this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 3] += (this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                
                HessianMatrix[4, 4] += (this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 4] += (this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                #endregion

                #region Vector build

                SolutionVector[0] += ((this.DiffractionConstants[n].ClassicS1 - this.S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[0] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[1] += ((this.DiffractionConstants[n].ClassicS1 - this.S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[1] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[2] += ((this.DiffractionConstants[n].ClassicS1 - this.S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[2] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[3] += ((this.DiffractionConstants[n].ClassicS1 - this.S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[3] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[4] += ((this.DiffractionConstants[n].ClassicS1 - this.S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[4] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex);

                #endregion
            }

            for (int n = 0; n < 5; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(5);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C33
        ///[2] C12
        ///[3] C13
        ///[4] C44
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorReussHexagonalMacroscopic(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(5, 5, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(5);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 0] += (this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 1] += (this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 2] += (this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[3, 3] += (this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 3] += (this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[4, 4] += (this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 4] += (this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                #endregion

                #region Vector build

                SolutionVector[0] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[0] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[1] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[1] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[2] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[2] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[3] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[3] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[4] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[4] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex);

                #endregion
            }

            for (int n = 0; n < 5; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(5);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        #region First derivative

        public double FirstDerivativeS11S1ReussHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = Math.Pow(hKL.H, 2) + (hKL.H * hKL.K) + Math.Pow(hKL.K, 2);
            Ret *= 6 * Math.Pow(hKL.L, 2);

            double N = 4 * Math.Pow(hKL.H, 2);
            N += 4 * Math.Pow(hKL.K, 2);
            N += 4 * Math.Pow(hKL.L, 2);
            N += 4 * hKL.H * hKL.K;

            Ret /= Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeS33S1ReussHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = Math.Pow(hKL.H, 2) + (hKL.H * hKL.K) + Math.Pow(hKL.K, 2);
            Ret *= 6 * Math.Pow(hKL.L, 2);

            double N = 4 * Math.Pow(hKL.H, 2);
            N += 4 * Math.Pow(hKL.K, 2);
            N += 4 * Math.Pow(hKL.L, 2);
            N += 4 * hKL.H * hKL.K;

            Ret /= Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeS44S1ReussHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = Math.Pow(hKL.H, 2) + (hKL.H * hKL.K) + Math.Pow(hKL.K, 2);
            Ret *= -6 * Math.Pow(hKL.L, 2);

            double N = 4 * Math.Pow(hKL.H, 2);
            N += 4 * Math.Pow(hKL.K, 2);
            N += 4 * Math.Pow(hKL.L, 2);
            N += 4 * hKL.H * hKL.K;

            Ret /= Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeS12S1ReussHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = Math.Pow(hKL.H, 2) + (hKL.H * hKL.K) + Math.Pow(hKL.K, 2);
            Ret *= (4 * Math.Pow(hKL.H, 2)) + (4 * (hKL.H * hKL.K)) + (4 * Math.Pow(hKL.K, 2)) + (3 * Math.Pow(hKL.L, 2));
            Ret *= 2;

            double N = 4 * Math.Pow(hKL.H, 2);
            N += 4 * Math.Pow(hKL.K, 2);
            N += 4 * Math.Pow(hKL.L, 2);
            N += 4 * hKL.H * hKL.K;

            Ret /= Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeS13S1ReussHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = 8 * Math.Pow(hKL.H, 4);
            Ret += 16 * Math.Pow(hKL.H, 3) * hKL.K;
            Ret += 24 * Math.Pow(hKL.H, 2) * Math.Pow(hKL.K, 2);
            Ret += 16 * hKL.H * Math.Pow(hKL.K, 3);
            Ret += 8 * Math.Pow(hKL.K, 4);
            Ret += 6 * Math.Pow(hKL.H, 2) * Math.Pow(hKL.L, 2);
            Ret += 6 * hKL.H * hKL.K * Math.Pow(hKL.L, 2);
            Ret += 6 * Math.Pow(hKL.K, 2) * Math.Pow(hKL.L, 2);
            Ret += 9 * Math.Pow(hKL.L, 4);

            double N = 4 * Math.Pow(hKL.H, 2);
            N += 4 * Math.Pow(hKL.K, 2);
            N += 4 * Math.Pow(hKL.L, 2);
            N += 4 * hKL.H * hKL.K;

            Ret /= Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeS11HS2ReussHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = Math.Pow(Math.Pow(hKL.H, 2) + (hKL.H * hKL.K) + Math.Pow(hKL.K, 2), 2);
            Ret *= 16;

            double N = 4 * Math.Pow(hKL.H, 2);
            N += 4 * Math.Pow(hKL.K, 2);
            N += 4 * Math.Pow(hKL.L, 2);
            N += 4 * hKL.H * hKL.K;

            Ret /= Math.Pow(N, 2);

            Ret -= FirstDerivativeS11S1ReussHexagonal(hKL);

            return Ret;
        }

        public double FirstDerivativeS33HS2ReussHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = 9 * Math.Pow(hKL.L, 4);

            double N = 4 * Math.Pow(hKL.H, 2);
            N += 4 * Math.Pow(hKL.K, 2);
            N += 4 * Math.Pow(hKL.L, 2);
            N += 4 * hKL.H * hKL.K;

            Ret /= Math.Pow(N, 2);

            Ret -= FirstDerivativeS33S1ReussHexagonal(hKL);

            return Ret;
        }

        public double FirstDerivativeS12HS2ReussHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = -1 * FirstDerivativeS12S1ReussHexagonal(hKL);

            return Ret;
        }

        public double FirstDerivativeS13HS2ReussHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = Math.Pow(hKL.H, 2) + (hKL.H * hKL.K) + Math.Pow(hKL.K, 2);
            Ret *= 12 * Math.Pow(hKL.L, 2);

            double N = 4 * Math.Pow(hKL.H, 2);
            N += 4 * Math.Pow(hKL.K, 2);
            N += 4 * Math.Pow(hKL.L, 2);
            N += 4 * hKL.H * hKL.K;

            Ret /= Math.Pow(N, 2);

            Ret -= FirstDerivativeS13S1ReussHexagonal(hKL);

            return Ret;
        }

        public double FirstDerivativeS44HS2ReussHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = Math.Pow(hKL.H, 2) + (hKL.H * hKL.K) + Math.Pow(hKL.K, 2);
            Ret *= 12 * Math.Pow(hKL.L, 2);

            double N = 4 * Math.Pow(hKL.H, 2);
            N += 4 * Math.Pow(hKL.K, 2);
            N += 4 * Math.Pow(hKL.L, 2);
            N += 4 * hKL.H * hKL.K;

            Ret /= Math.Pow(N, 2);

            Ret -= FirstDerivativeS44S1ReussHexagonal(hKL);

            return Ret;
        }

        #endregion

        #endregion

        #endregion

        #endregion

        #region Cloning

        public object Clone()
        {
            ElasticityTensors Ret = new ElasticityTensors();

            Ret._complianceTensor = this._complianceTensor.Clone();
            Ret._stiffnessTensor = this._stiffnessTensor.Clone();

            Ret._complianceTensorError = this._complianceTensorError.Clone();
            Ret._stiffnessTensorError = this._stiffnessTensorError.Clone();

            Ret._symmetry = this._symmetry;

            Ret.IsIsotropic = this.IsIsotropic;

            Ret.DiffractionConstants = new List<REK>();

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                Ret.DiffractionConstants.Add(this.DiffractionConstants[n].Clone() as REK);
            }

            return Ret;
        }

        #endregion

    }
}
