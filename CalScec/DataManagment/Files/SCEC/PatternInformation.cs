using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.DataManagment.Files.SCEC
{
    [Serializable]
    public class PatternInformation
    {
        #region Version data

        private int _patternVersion = CalScec.Properties.Settings.Default.PatternVersion;
        public int PatternVersion
        {
            get
            {
                return this._patternVersion;
            }
        }

        #endregion

        #region General information
        
        public int Id;

        public string Name;
        public string Path;
        public double PsiAngle;
        public double PhiAngle;
        public double Stress;
        public double Force;

        public Pattern.Counts PatternCounts;

        public List<PeakFunctionInformation> FoundPeaks = new List<PeakFunctionInformation>();
        public List<PeakRegionInformation> PeakRegions = new List<PeakRegionInformation>();

        #endregion

        public PatternInformation(Pattern.DiffractionPattern DP)
        {
            this.Id = DP.Id;
            this.Name = DP.Name;
            this.Path = DP.Path;
            this.PsiAngle = DP.PsiAngle;
            this.PhiAngle = DP.PhiAngle;
            this.Stress = DP.Stress;
            this.Force = DP.Force;

            this.PatternCounts = DP.PatternCounts.Clone() as Pattern.Counts;

            for(int n = 0; n < DP.FoundPeaks.Count; n++)
            {
                this.FoundPeaks.Add(new PeakFunctionInformation(DP.FoundPeaks[n]));
            }
            for (int n = 0; n < DP.PeakRegions.Count; n++)
            {
                this.PeakRegions.Add(new PeakRegionInformation(DP.PeakRegions[n]));
            }
        }

        public Pattern.DiffractionPattern GetDiffractionPattern()
        {
            Pattern.DiffractionPattern Ret = new Pattern.DiffractionPattern(this.Id);

            Ret.Name = this.Name;
            Ret.PsiAngle = this.PsiAngle;
            Ret.PhiAngle = this.PhiAngle;
            Ret.Stress = this.Stress;
            Ret.Force = this.Force;

            Ret.PatternCounts = this.PatternCounts.Clone() as Pattern.Counts;

            for(int n = 0; n < this.FoundPeaks.Count; n++)
            {
                Ret.FoundPeaks.Add(new Analysis.Peaks.DiffractionPeak(this.FoundPeaks[n]));
            }

            for(int n = 0; n < this.PeakRegions.Count; n++)
            {
                Analysis.Peaks.Functions.PeakRegionFunction RegionTmp = new Analysis.Peaks.Functions.PeakRegionFunction(this.PeakRegions[n]);

                RegionTmp.Clear();

                for(int i = 0; i < this.PeakRegions[n].Count; i++)
                {
                    for(int j = 0; j < Ret.FoundPeaks.Count; j++)
                    {
                        if(this.PeakRegions[n][i] == Ret.FoundPeaks[j].PeakId)
                        {
                            RegionTmp.Add(Ret.FoundPeaks[j].PFunction);
                            break;
                        }
                    }
                }

                Ret.PeakRegions.Add(RegionTmp);
            }

            Ret.SetPeakAssociations();
            return Ret;
        }
    }
}
