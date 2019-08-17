export class MyPropertyHelper {
        public AttributeLogicalName(property : ComponentFramework.PropertyTypes.Property) : string {
        let AttributeLogicalName : string = "";
        switch(property.type)
        {
            case "Whole.None":
                    AttributeLogicalName = (property as ComponentFramework.PropertyTypes.WholeNumberProperty).attributes!.LogicalName;
            break;

            case "TwoOptions":
                    AttributeLogicalName = (property as ComponentFramework.PropertyTypes.TwoOptionsProperty).attributes!.LogicalName;
            break;

            case "DateAndTime.DateOnly":
            case "DateAndTime.DateAndTime":
                    AttributeLogicalName = (property as ComponentFramework.PropertyTypes.DateTimeProperty).attributes!.LogicalName;
            break;

            case "Decimal":
            case "Currency":
                    AttributeLogicalName = (property as ComponentFramework.PropertyTypes.DecimalNumberProperty).attributes!.LogicalName;
            break;

            case "FP":
                    AttributeLogicalName = (property as ComponentFramework.PropertyTypes.FloatingNumberProperty).attributes!.LogicalName;
            break;

            case "Multiple":
                    AttributeLogicalName = (property as ComponentFramework.PropertyTypes.MultiSelectOptionSetProperty).attributes!.LogicalName;
            break;

            case "OptionSet":
                    AttributeLogicalName = (property as ComponentFramework.PropertyTypes.OptionSetProperty).attributes!.LogicalName;
            break;

            case "SingleLine.Email":
            case "SingleLine.Text":
            case "SingleLine.TextArea":
            case "SingleLine.URL":
            case "SingleLine.Ticker":
            case "SingleLine.Phone":
                    AttributeLogicalName = (property as ComponentFramework.PropertyTypes.StringProperty).attributes!.LogicalName;
            break;

            default:
            break;
        }

        return AttributeLogicalName;
    }

    public AttributeDisplayName(property : ComponentFramework.PropertyTypes.Property) : string {
        let AttributeDisplayName : string = "";
        switch(property.type)
        {
            case "Whole.None":
                    AttributeDisplayName = (property as ComponentFramework.PropertyTypes.WholeNumberProperty).attributes!.DisplayName;
            break;

            case "TwoOptions":
                    AttributeDisplayName = (property as ComponentFramework.PropertyTypes.TwoOptionsProperty).attributes!.DisplayName;
            break;

            case "DateAndTime.DateOnly":
            case "DateAndTime.DateAndTime":
                        AttributeDisplayName = (property as ComponentFramework.PropertyTypes.DateTimeProperty).attributes!.DisplayName;
            break;

            case "Decimal":
            case "Currency":
                        AttributeDisplayName = (property as ComponentFramework.PropertyTypes.DecimalNumberProperty).attributes!.DisplayName;
            break;

            case "FP":
                        AttributeDisplayName = (property as ComponentFramework.PropertyTypes.FloatingNumberProperty).attributes!.DisplayName;
            break;

            case "Multiple":
                        AttributeDisplayName = (property as ComponentFramework.PropertyTypes.MultiSelectOptionSetProperty).attributes!.DisplayName;
            break;

            case "OptionSet":
                        AttributeDisplayName = (property as ComponentFramework.PropertyTypes.OptionSetProperty).attributes!.DisplayName;
            break;

            case "SingleLine.Email":
            case "SingleLine.Text":
            case "SingleLine.TextArea":
            case "SingleLine.URL":
            case "SingleLine.Ticker":
            case "SingleLine.Phone":
                        AttributeDisplayName = (property as ComponentFramework.PropertyTypes.StringProperty).attributes!.DisplayName;
            break;

            default:
            break;
        }

        return AttributeDisplayName;
    }
}