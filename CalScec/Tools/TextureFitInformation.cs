using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.Tools
{
    public class TextureFitInformation
    {
        
        private string _modelName;
        private int _lMATrial;
        private DateTime _timeAdded;

        public int iD;
        public string ModelName
        {
            get
            {
                return this._modelName;
            }
            set
            {
                this._modelName = value;
            }
        }
        public int LMATrial
        {
            get
            {
                return this._lMATrial;
            }
            set
            {
                this._lMATrial = value;
            }
        }
        public DateTime TimeAdded
        {
            get
            {
                return this._timeAdded;
            }
            set
            {
                this._timeAdded = value;
            }
        }

        public TextureFitInformation(string modelName, DateTime timeAdded)
        {
            this._modelName = modelName;
            this._timeAdded = timeAdded;
            this._lMATrial = 0;
            this.iD = 0;
        }

        public TextureFitInformation(string modelName)
        {
            this._modelName = modelName;
            this._timeAdded = DateTime.Now;
            this._lMATrial = 0;
            this.iD = 0;
        }

        public void AddTrial()
        {
            this._lMATrial++;
            string Test = "";
        }
    }
}
