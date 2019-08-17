using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using VinnyB.EasyHistory.Models;

namespace VinnyB.EasyHistory.Extensions
{
    public static class AttributeAuditDetailExtension
    {
        /// <summary>
        /// Convert the Microsoft.Crm.Sdk.Messages.AttributeAuditDetail to custom VinnyB.EasyHistory.Models.AuditDetailModel
        /// </summary>
        /// <param name="_this">AttributeAuditDetail</param>
        /// <returns>AuditDetailModel</returns>
        public static AuditDetailModel ToEasyHistoryComponent(this AttributeAuditDetail _this)
        {
            EntityReference user = _this.AuditRecord.GetAttributeValue<EntityReference>("userid");
            if (user == null || String.IsNullOrEmpty(user.Name))
            {
                user = new EntityReference();
                user.Name = "N/A";
            }
            DateTime date = _this.AuditRecord.GetAttributeValue<DateTime>("createdon");
            string value = _this.OldValue.GetAttributeValue<string>("accountnumber");

            AuditDetailModel auditDetailModel = new AuditDetailModel(user.Name, date.ToString("MM-dd-yyyy h:mm tt"), value);

            return auditDetailModel;
        }

        /// <summary>
        /// Return a JSON based on AuditDetailModels order by (desc) ModifiedOn 
        /// </summary>
        /// <param name="_this">List<AuditDetailModel></param>
        /// <returns>string</returns>
        public static string ToJSON(this List<AuditDetailModel> _this)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(_this.GetType());
                serializer.WriteObject(memoryStream, _this);
                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }
    }
}
