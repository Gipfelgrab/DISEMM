using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.Pattern
{
    public class DiffractionPattern : ICloneable
    {

        #region Parameters

        private int _id = 0;
        public int Id
        {
            get
            {
                return this._id;
            }
        }

        private string _name;
        public string Name
        {
            get
            {
                if (this._name == "")
                {
                    return this._path;
                }
                else
                {
                    return this._name;
                }
            }
            set
            {
                this._name = value;
            }
        }

        private string _path;
        public string Path
        {
            get
            {
                return this._path;
            }
        }

        private double _psiAngle;
        public double PsiAngle
        {
            get
            {
                return this._psiAngle;
            }
            set
            {
                this._psiAngle = value;
            }
        }

        private double _phiAngle;
        public double PhiAngle
        {
            get
            {
                return this._phiAngle;
            }
            set
            {
                this._phiAngle = value;
            }
        }

        private double _stress;
        public double Stress
        {
            get
            {
                return this._stress;
            }
            set
            {
                this._stress = value;
            }
        }

        private double _force;
        public double Force
        {
            get
            {
                return this._force;
            }
            set
            {
                this._force = value;
            }
        }

        public Counts PatternCounts;

        public List<Analysis.Peaks.DiffractionPeak> FoundPeaks = new List<Analysis.Peaks.DiffractionPeak>();
        public List<Analysis.Peaks.Functions.PeakRegionFunction> PeakRegions = new List<Analysis.Peaks.Functions.PeakRegionFunction>();

        #endregion

        public DiffractionPattern(string FilePath, int id)
        {
            this._id = id;
            this._path = FilePath;

            this._name = System.IO.Path.GetFileName(FilePath);
            this.PatternCounts = new Counts();

            string[] PatternFileLines = System.IO.File.ReadLines(FilePath).ToArray();
            foreach(string line in PatternFileLines)
            {
                if(line[0] != '#' && line[0] != '%' && line[0] != '/')
                {
                    string[] Parts = line.Split(' ');

                    List<double> ActCount = new List<double>();
                    foreach(string s in Parts)
                    {
                        if(s != "")
                        {
                            ActCount.Add(Convert.ToDouble(s));
                        }
                    }

                    this.PatternCounts.Add(ActCount.ToArray());
                }
            }
        }

        public DiffractionPattern(int id)
        {
            this._id = id;
        }

        public void SetPeakAssociations()
        {
            for (int n = 0; n < this.PeakRegions.Count; n++)
            {
                this.PeakRegions[n].AssociatedPatternName = this.Name;
            }
            for (int n = 0; n < this.FoundPeaks.Count; n++)
            {
                this.FoundPeaks[n].AssociatedPatternName = this.Name;
            }
        }

        #region Calculations

        #region Peak regions

        public void SetRegions()
        {
            this.PeakRegions = new List<Analysis.Peaks.Functions.PeakRegionFunction>();
            FoundPeaks.Sort((A, B) => (1) * (A.PFunction.Angle).CompareTo(B.PFunction.Angle));

            for(int n = 0; n < this.FoundPeaks.Count;)
            {
                if( n + 1 < this.FoundPeaks.Count)
                {
                    for (int i = 1; n + i < this.FoundPeaks.Count + 1; i++)
                    {
                        if(n + i == this.FoundPeaks.Count)
                        {
                            Pattern.Counts fittingData = this.PatternCounts.GetRange(this.FoundPeaks[n].PFunction.Angle - (this.FoundPeaks[n].PFunction.FWHM * CalScec.Properties.Settings.Default.PeakWidthFWHM), this.FoundPeaks[n + (i - 1)].PFunction.Angle + (this.FoundPeaks[n + (i - 1)].PFunction.FWHM * CalScec.Properties.Settings.Default.PeakWidthFWHM));

                            List<Analysis.Peaks.Functions.PeakFunction> ForAdd = new List<Analysis.Peaks.Functions.PeakFunction>();
                            foreach (Analysis.Peaks.DiffractionPeak DP in FoundPeaks.GetRange(n, i))
                            {
                                ForAdd.Add(DP.PFunction);
                            }

                            PeakRegions.Add(new Analysis.Peaks.Functions.PeakRegionFunction(this.FoundPeaks[n].PFunction.ConstantBackground, fittingData, ForAdd));
                            n += i;
                            break;
                        }

                        double PeakDistance = Math.Abs(this.FoundPeaks[n + (i - 1)].PFunction.Angle - this.FoundPeaks[n + i].PFunction.Angle);
                        if(PeakDistance > 2 * this.FoundPeaks[n].PFunction.FWHM * CalScec.Properties.Settings.Default.PeakWidthFWHM)
                        {
                            if(i == 1)
                            {
                                PeakRegions.Add(new Analysis.Peaks.Functions.PeakRegionFunction(this.FoundPeaks[n].PFunction.ConstantBackground, this.FoundPeaks[n].PFunction.FittingCounts, FoundPeaks[n].PFunction));
                                n++;
                                break;
                            }
                            else
                            {
                                Pattern.Counts fittingData = this.PatternCounts.GetRange(this.FoundPeaks[n].PFunction.Angle - (this.FoundPeaks[n].PFunction.FWHM * CalScec.Properties.Settings.Default.PeakWidthFWHM), this.FoundPeaks[n + (i - 1)].PFunction.Angle + (this.FoundPeaks[n + (i - 1)].PFunction.FWHM * CalScec.Properties.Settings.Default.PeakWidthFWHM));

                                List<Analysis.Peaks.Functions.PeakFunction> ForAdd = new List<Analysis.Peaks.Functions.PeakFunction>();
                                foreach(Analysis.Peaks.DiffractionPeak DP in FoundPeaks.GetRange(n, i))
                                {
                                    ForAdd.Add(DP.PFunction);
                                }

                                PeakRegions.Add(new Analysis.Peaks.Functions.PeakRegionFunction(this.FoundPeaks[n].PFunction.ConstantBackground, fittingData, ForAdd));
                                n += i;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    PeakRegions.Add(new Analysis.Peaks.Functions.PeakRegionFunction(this.FoundPeaks[n].PFunction.ConstantBackground, this.FoundPeaks[n].PFunction.FittingCounts, FoundPeaks[n].PFunction));
                    n++;
                }
            }

            for(int n = 0; n < this.PeakRegions.Count; n++)
            {
                this.PeakRegions[n].AssociatedPatternName = this.Name;
            }
        }

        #endregion

        #endregion

        #region IClonable

        public object Clone()
        {
            DiffractionPattern Ret = new DiffractionPattern(this._id);
            
            Ret._name = this._name;
            Ret._path = this._path;
            Ret._psiAngle = this._psiAngle;
            Ret._phiAngle = this._phiAngle;
            Ret._stress = this._stress;
            Ret._force = this._force;

            Ret.PatternCounts = (Counts)this.PatternCounts.Clone();

            foreach(Analysis.Peaks.DiffractionPeak DP in this.FoundPeaks)
            {
                Ret.FoundPeaks.Add(DP.Clone() as Analysis.Peaks.DiffractionPeak);
            }
            foreach (Analysis.Peaks.Functions.PeakRegionFunction PR in this.PeakRegions)
            {
                Ret.PeakRegions.Add(PR.Clone() as Analysis.Peaks.Functions.PeakRegionFunction);
            }

            return Ret;
        }

        #endregion
    }
}
