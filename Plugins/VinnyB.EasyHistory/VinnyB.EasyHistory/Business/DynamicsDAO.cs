using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinnyB.EasyHistory.Extensions;
using VinnyB.EasyHistory.Models;

namespace VinnyB.EasyHistory.Business
{
    public class DynamicsDAO
    {
        private IOrganizationService Service;

        public DynamicsDAO(IOrganizationService service) { this.Service = service; }

        /// <summary>
        /// Retrieve all labels of OptionSet
        /// </summary>
        /// <param name="entityName">Entity Logical Name</param>
        /// <param name="attributeName">Attribute Logical Name</param>
        /// <returns>OptionMetadataCollection</returns>
        public OptionMetadataCollection GetOptionsSetTextOnValue(string entityName, string attributeName)
        {
            RetrieveAttributeRequest retrieveAttributeRequest = new RetrieveAttributeRequest
            {
                EntityLogicalName = entityName,
                LogicalName = attributeName,
                RetrieveAsIfPublished = true
            };
            RetrieveAttributeResponse retrieveAttributeResponse = (RetrieveAttributeResponse)Service.Execute(retrieveAttributeRequest);
            PicklistAttributeMetadata attributeMetadata = (PicklistAttributeMetadata)retrieveAttributeResponse?.AttributeMetadata;
            if (attributeMetadata == null) return null;
            return attributeMetadata?.OptionSet?.Options;
        }

        /// <summary>
        /// Retrieve all labels of Booleans
        /// </summary>
        /// <param name="entityName">Entity Logical Name</param>
        /// <param name="attributeName">Attribute Logical Name</param>
        /// <returns>OptionMetadataCollection</returns>
        public BooleanOptionSetMetadata GetBooleanTextOnValue(string entityName, string attributeName)
        {
            RetrieveAttributeRequest retrieveAttributeRequest = new RetrieveAttributeRequest
            {
                EntityLogicalName = entityName,
                LogicalName = attributeName,
                RetrieveAsIfPublished = true
            };
            RetrieveAttributeResponse retrieveAttributeResponse = (RetrieveAttributeResponse)Service.Execute(retrieveAttributeRequest);
            BooleanAttributeMetadata attributeMetadata = (BooleanAttributeMetadata)retrieveAttributeResponse?.AttributeMetadata;
            if (attributeMetadata == null) return null;
            return attributeMetadata?.OptionSet;
        }

        

        /// <summary>
        /// Retrieve attribute audit 
        /// </summary>
        /// <param name="entityLogicalName"></param>
        /// <param name="attributeLogicalName"></param>
        /// <param name="recordId"></param>
        /// <returns></returns>
        public List<AuditDetailModel> RetrieveAuditHistory(string entityLogicalName, string attributeLogicalName, Guid recordId)
        {
            //Return
            List<AuditDetailModel> audits = new List<AuditDetailModel>();

            //Make a request to retrieve the audit history
            RetrieveAttributeChangeHistoryRequest request = new RetrieveAttributeChangeHistoryRequest()
            {
                Target = new EntityReference(entityLogicalName, recordId),
                AttributeLogicalName = attributeLogicalName
            };
            //Execute the request
            RetrieveAttributeChangeHistoryResponse response = (RetrieveAttributeChangeHistoryResponse)Service.Execute(request);

            //If the audit return histories
            if (response.AuditDetailCollection != null && response.AuditDetailCollection.AuditDetails != null)
            {
                //Filter by changes
                foreach (AuditDetail auditDetail in response.AuditDetailCollection.AuditDetails.Where(w => w.GetType() == typeof(AttributeAuditDetail)))
                {
                    AttributeAuditDetail attributeDetail = (AttributeAuditDetail)auditDetail;
                    audits.Add(attributeDetail.ToEasyHistoryComponent(attributeLogicalName));
                }
            }

            return audits;
        }
    }
}
