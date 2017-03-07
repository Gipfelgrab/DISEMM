///////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////Im Gedenken an Tepi//////////////////////////////////////
//////////////////////Das Leben ist wie eine Reise in totaler Dunkelheit://////////////////////
/////Man weiß wie wo der nächste Schritt hinführt, aber jeder findet irgendwann das Licht//////
///////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.Analysis.Peaks.Functions
{
    [Serializable]
    public class Constraints : List<double>, ICloneable
    {
        #region Parameter

        public bool AngleConstraintActiv;
        public double AngleConstraint
        {
            get
            {
                return this[0];
            }
            set
            {
                this[0] = value;
            }
        }

        public bool SigmaConstraintActiv;
        public double SigmaConstraint
        {
            get
            {
                return this[1];
            }
            set
            {
                this[1] = value;
            }
        }

        public bool LorentzRatioConstraintActiv;
        public double LorentzRatioConstraint
        {
            get
            {
                return this[2];
            }
            set
            {
                this[2] = value;
            }
        }

        #endregion

        public Constraints()
        {
            this.Add(CalScec.Properties.Settings.Default.FittingAngleConstraint);
            this.Add(CalScec.Properties.Settings.Default.FittingSigmaConstraint);
            this.Add(CalScec.Properties.Settings.Default.FittingLorentzRatioConstraint);

            AngleConstraintActiv = true;
            SigmaConstraintActiv = true;
            LorentzRatioConstraintActiv = true;
        }

        public Constraints(double angleConstraint)
        {
            this.Add(angleConstraint);
            this.Add(CalScec.Properties.Settings.Default.FittingSigmaConstraint);
            this.Add(CalScec.Properties.Settings.Default.FittingLorentzRatioConstraint);

            AngleConstraintActiv = true;
            SigmaConstraintActiv = true;
            LorentzRatioConstraintActiv = true;
        }

        public Constraints(double angleConstraint, double sigmaConstraint)
        {
            this.Add(angleConstraint);
            this.Add(sigmaConstraint);
            this.Add(CalScec.Properties.Settings.Default.FittingLorentzRatioConstraint);

            AngleConstraintActiv = true;
            SigmaConstraintActiv = true;
            LorentzRatioConstraintActiv = true;
        }

        public Constraints(double angleConstraint, double sigmaConstraint, double lorentzRatioConstraint)
        {
            this.Add(angleConstraint);
            this.Add(sigmaConstraint);
            this.Add(lorentzRatioConstraint);

            AngleConstraintActiv = true;
            SigmaConstraintActiv = true;
            LorentzRatioConstraintActiv = true;
        }

        public Constraints(double[] constraints)
        {
            this.Add(constraints[0]);
            this.Add(constraints[1]);
            this.Add(constraints[2]);

            AngleConstraintActiv = true;
            SigmaConstraintActiv = true;
            LorentzRatioConstraintActiv = true;
        }

        #region Cloning

        public object Clone()
        {
            double angleTmp = this[0];
            double sigmaTmp = this[1];
            double lorentzRationTmp = this[2];

            return new Constraints(angleTmp, sigmaTmp, lorentzRationTmp);
        }

        #endregion
    }
}
