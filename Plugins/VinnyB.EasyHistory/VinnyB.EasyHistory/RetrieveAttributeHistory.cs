using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinnyB.EasyHistory.Extensions;
using VinnyB.EasyHistory.Models;

namespace VinnyB.EasyHistory
{
    public class RetrieveAttributeHistory : CodeActivity
    {
        [Input("EntityLogicalName")]
        public InArgument<string> EntityLogicalName { get; set; }

        [Input("EntityId")]
        public InArgument<string> EntityId { get; set; }

        [Input("AttributeLogicalName")]
        public InArgument<string> AttributeLogicalName { get; set; }

        [Output("History")]
        public OutArgument<string> History { get; set; }

        [Output("ChangesCount")]
        public OutArgument<int> ChangesCount { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            if (context == null) { throw new InvalidPluginExecutionException("Context not found!"); };

            var workflowContext = context.GetExtension<IWorkflowContext>();
            var serviceFactory = context.GetExtension<IOrganizationServiceFactory>();
            var orgService = serviceFactory.CreateOrganizationService(workflowContext.UserId);

            Guid recordId = Guid.Empty;
            if (Guid.TryParse(EntityId.Get<string>(context), out recordId))
            {
                //Make a request to retrieve the audit history
                RetrieveAttributeChangeHistoryRequest request = new RetrieveAttributeChangeHistoryRequest()
                {
                    Target = new EntityReference(EntityLogicalName.Get<string>(context), recordId),
                    AttributeLogicalName = AttributeLogicalName.Get<string>(context)
                };
                //Execute the request
                RetrieveAttributeChangeHistoryResponse response = (RetrieveAttributeChangeHistoryResponse)orgService.Execute(request);

                //If the audit return histories
                if (response.AuditDetailCollection != null && response.AuditDetailCollection.AuditDetails != null)
                {
                    //List do return
                    List<AuditDetailModel> audits = new List<AuditDetailModel>();

                    //Filter by changes
                    foreach (AuditDetail auditDetail in response.AuditDetailCollection.AuditDetails.Where(w => w.GetType() == typeof(AttributeAuditDetail)))
                    {
                        AttributeAuditDetail attributeDetail = (AttributeAuditDetail)auditDetail;
                        audits.Add(attributeDetail.ToEasyHistoryComponent());
                    }

                    //Return values
                    ChangesCount.Set(context, response.AuditDetailCollection.AuditDetails.Where(w => w.GetType() == typeof(AttributeAuditDetail)).Count());
                    History.Set(context, audits.ToJSON());
                }
                else
                {
                    //Error
                    History.Set(context, $"{EntityId.Get<string>(context)} isn't a Guid");
                }
            }
        }
    }
}
