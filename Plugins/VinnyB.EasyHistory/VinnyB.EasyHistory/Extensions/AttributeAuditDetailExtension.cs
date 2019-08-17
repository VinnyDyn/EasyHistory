using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
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
        public static AuditDetailModel ToEasyHistoryComponent(this AttributeAuditDetail _this, string _attributeLogicalName)
        {
            EntityReference user = _this.AuditRecord.GetAttributeValue<EntityReference>("userid");
            if (user == null || String.IsNullOrEmpty(user.Name))
            {
                user = new EntityReference();
                user.Name = "N/A";
            }
            DateTime date = _this.AuditRecord.GetAttributeValue<DateTime>("createdon");
            string value = string.Empty;
            Type attributeType = null;
            if (_this.OldValue.Contains(_attributeLogicalName) && _this.OldValue.Attributes[_attributeLogicalName] != null)
            {
                attributeType = _this.OldValue.Attributes[_attributeLogicalName].GetType();
                value = RetrieveAttributeValue(attributeType, _this.OldValue, _attributeLogicalName);
            }
            else
                value = string.Empty;

            AuditDetailModel auditDetailModel = new AuditDetailModel(user.Name, date.ToString("MM-dd-yyyy hh:mm tt"), value, attributeType != null ? attributeType.Name : string.Empty);

            return auditDetailModel;
        }

        private static string RetrieveAttributeValue(Type _type, Entity _oldValue, string _attributeLogicalName)
        {
            //Return value
            string value = string.Empty;

            //Define how can return the value
            switch (_type.Name.ToLower())
            {
                case "string":
                    value = _oldValue.GetAttributeValue<string>(_attributeLogicalName);
                    break;

                case "boolean":
                    bool boolean = _oldValue.GetAttributeValue<bool>(_attributeLogicalName);
                    value = boolean ? "1" : "0"; //Necessary to take the label
                    break;

                case "int32":
                    value = _oldValue.GetAttributeValue<Int32>(_attributeLogicalName).ToString();
                    break;

                case "decimal":
                    value = _oldValue.GetAttributeValue<decimal>(_attributeLogicalName).ToString();
                    break;

                case "double":
                    value = _oldValue.GetAttributeValue<double>(_attributeLogicalName).ToString();
                    break;

                case "money":
                    value = _oldValue.GetAttributeValue<Money>(_attributeLogicalName).Value.ToString();
                    break;

                case "optionsetvalue":
                    value = _oldValue.GetAttributeValue<OptionSetValue>(_attributeLogicalName).Value.ToString();
                    break;

                case "datetime":
                    value = _oldValue.GetAttributeValue<DateTime>(_attributeLogicalName).ToString("MM-dd-yyyy hh:mm tt");
                    break;
            }
            return value;
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

        /// <summary>
        /// Replace OptionSetValue (int) by labes (string)
        /// </summary>
        /// <param name="_this">List<AuditDetailModel></param>
        public static void ReplaceOptionSetValuesByLabels(this List<AuditDetailModel> _this, OptionMetadataCollection optionMetadataCollection)
        {
            //All audits histories where the attribute has value
            foreach (var audit in _this.Where(w => !String.IsNullOrEmpty(w.Value)))
            {
                var currentOption = optionMetadataCollection?.FirstOrDefault(x => x.Value == Convert.ToInt32(audit.Value));
                audit.Value = currentOption?.Label?.UserLocalizedLabel?.Label != null ? currentOption.Label.UserLocalizedLabel.Label : string.Empty;
            }
        }

        /// <summary>
        /// Replace Boolean (int) by labes (string)
        /// </summary>
        /// <param name="_this">List<AuditDetailModel></param>
        public static void ReplaceBooleanValuesByLabels(this List<AuditDetailModel> _this, BooleanOptionSetMetadata booleanOptionSetMetadata)
        {
            //All audits histories where the attribute has value
            foreach (var audit in _this.Where(w => !String.IsNullOrEmpty(w.Value)))
            {
                if (audit.Value == "1")
                    audit.Value = booleanOptionSetMetadata.TrueOption?.Label?.UserLocalizedLabel?.Label != null ? booleanOptionSetMetadata.TrueOption.Label.UserLocalizedLabel.Label : string.Empty;
                else
                    audit.Value = booleanOptionSetMetadata.FalseOption?.Label?.UserLocalizedLabel?.Label != null ? booleanOptionSetMetadata.FalseOption.Label.UserLocalizedLabel.Label : string.Empty;
            }
        }
    }
}
