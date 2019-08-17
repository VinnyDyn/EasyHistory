using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace VinnyB.EasyHistory.Models
{
    [DataContract]
    public class AuditDetailModel
    {
        public AuditDetailModel(string user, string modifiedOn, string value, string type)
        {
            this.User = user;
            this.ModifiedOn = modifiedOn;
            this.Value = value;
            this.Type = type.ToLower();
        }

        [DataMember]
        public string User { get; set; }
        [DataMember]
        public string ModifiedOn { get; set; }
        [DataMember]
        public string Value { get; set; }
        [DataMember]
        public string Type { get; set; }
    }
}
