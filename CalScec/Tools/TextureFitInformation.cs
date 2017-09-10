using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.Tools
{
    public struct TextureFitInformation
    {
        public string ModelName;

        public DateTime TimeAdded;

        public TextureFitInformation(string modelName, DateTime timeAdded)
        {
            this.ModelName = modelName;
            this.TimeAdded = timeAdded;
        }

        public TextureFitInformation(string modelName)
        {
            this.ModelName = modelName;
            this.TimeAdded = DateTime.Now;
        }
    }
}
