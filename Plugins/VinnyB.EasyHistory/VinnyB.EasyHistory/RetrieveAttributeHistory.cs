using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinnyB.EasyHistory.Business;
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
                DynamicsDAO dynamicsDAO = new DynamicsDAO(orgService);
                List<AuditDetailModel> audits = dynamicsDAO.RetrieveAuditHistory(EntityLogicalName.Get<string>(context), AttributeLogicalName.Get<string>(context), recordId);
                if (audits.Count > 0)
                {
                    if (audits.Where(w => w.Type == "optionsetvalue").ToList().Count > 0) //This is necessary because the audit log, capture all changes in attributes including 'clear'
                    {
                        OptionMetadataCollection optionMetadatas = dynamicsDAO.GetOptionsSetTextOnValue(EntityLogicalName.Get<string>(context), AttributeLogicalName.Get<string>(context));
                        audits.ReplaceOptionSetValuesByLabels(optionMetadatas);
                    }
                    else if(audits.Where(w => w.Type == "boolean").ToList().Count > 0)
                    {
                        BooleanOptionSetMetadata booleanOptionSetMetadata = dynamicsDAO.GetBooleanTextOnValue(EntityLogicalName.Get<string>(context), AttributeLogicalName.Get<string>(context));
                        audits.ReplaceBooleanValuesByLabels(booleanOptionSetMetadata);
                    }
                }
                ChangesCount.Set(context, audits.Count);
                History.Set(context, audits.ToJSON());
            }
        }
    }
}
