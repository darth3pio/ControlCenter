using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BF2Statistics.MedalData
{
    class SelectItem
    {
        public string Name = "";
        public string Value = "";

        public SelectItem(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        public override string ToString()
        {
            return this.Value;
        }
    }
}
