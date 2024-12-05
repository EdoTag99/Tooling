using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ToolingLib.Models
{
    [System.SerializableAttribute()]
    [DataContract(Name = nameof(PressBar), Namespace = "")]
    public class PressBar
    {
        [DataMember(Name = "PressID", IsRequired = true)]
        public int PressID { get; set; }

        [DataMember(Name = "Toolbar", IsRequired = true)]
        public int Toolbar { get; set; }

        public PressBar(int PressID, int Toolbar)
        {
            this.PressID = PressID;
            this.Toolbar = Toolbar;
        }
    }
}