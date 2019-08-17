import { IInputs, IOutputs } from "./generated/ManifestTypes";
import { ActionContract } from "./ActionContract";
import { ActionResponse, AuditDetailModel } from "./ActionResponse";
import { MyPropertyHelper } from "./MyPropertyHelper";

export class EasyHistory implements ComponentFramework.StandardControl<IInputs, IOutputs> {

	private _attributeLogicalName : string;
	private _attributeDisplayName : string;
	private _container: HTMLDivElement;

	private _historyButton: HTMLButtonElement;
	private _historyContainer: HTMLDivElement;

	private _context: ComponentFramework.Context<IInputs>;
	private _notifyOutputChanged: () => void;
	private _historyDiv: HTMLDivElement;
	private _refreshData: EventListenerOrEventListenerObject;
	private _myPropertyHelper : MyPropertyHelper;

	/**
	 * Empty constructor.
	 */
	constructor()
	{
		this._myPropertyHelper = new MyPropertyHelper();
	}

	/**
	 * Used to initialize the control instance. Controls can kick off remote server calls and other initialization actions here.
	 * Data-set values are not initialized here, use updateView.
	 * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to property names defined in the manifest, as well as utility functions.
	 * @param notifyOutputChanged A callback method to alert the framework that the control has new outputs ready to be retrieved asynchronously.
	 * @param state A piece of data that persists in one session for a single user. Can be set at any point in a controls life cycle by calling 'setControlState' in the Mode interface.
	 * @param container If a control is marked control-type='starndard', it will receive an empty div element within which it can render its content.
	 */
	public init(context: ComponentFramework.Context<IInputs>, notifyOutputChanged: () => void, state: ComponentFramework.Dictionary, container:HTMLDivElement)
	{
		//Context
		this._context = context;
		this._notifyOutputChanged = notifyOutputChanged;

		//Atribute
		this._attributeLogicalName = this._myPropertyHelper.AttributeLogicalName(this._context.parameters.attribute);
		this._attributeDisplayName = this._myPropertyHelper.AttributeDisplayName(this._context.parameters.attribute);

		//Core
		this._container = document.createElement("div");
		this._container.setAttribute("class", "Container");
		container.append(this._container); //Main
		
		//Core.Button.Input
		this._historyButton = document.createElement("button");
		this._historyButton.setAttribute("type", "button");
		this._historyButton.setAttribute("class", "Retrieve");
		this._historyButton.innerText = "Show history of " + this._attributeDisplayName;
		this._historyButton.addEventListener("click", this.ShowHistory.bind(this));
		this._container.append(this._historyButton);

		//Core.History
		this._historyContainer = document.createElement("div");
		this._historyContainer.setAttribute("class", "HiddenHistory");
		this._historyButton.append(this._historyContainer);
		}

	public ShowHistory()
	{
		if(this._historyContainer.className == "HiddenHistory")
		{
			//Get Attribute History
			this.RetrieveHistory();
			//this.RetrieveHistoryMOC();
			this._historyContainer.setAttribute("class", "VisibleHistory");
		}
		else
		{
			this.ClearHistory();
			this._historyContainer.setAttribute("class", "HiddenHistory");
		}
	}

	public RetrieveHistoryMOC()
	{
		let A = this.CreateHistoryParamagraph("A", "8/12/2019 5:26:11 PM", "Vinicius Basile");
		let B = this.CreateHistoryParamagraph("B", "8/12/2019 5:26:11 PM", "Vinicius Basile");
		let C = this.CreateHistoryParamagraph("C", "8/12/2019 5:26:11 PM", "Vinicius Basile");

		this.AddHistoryRecord(A);
		this.AddHistoryRecord(B);
		this.AddHistoryRecord(C);
	}

	public RetrieveHistory()
	{
		let request = new ActionContract();
		request.EntityLogicalName = Xrm.Page.data.entity.getEntityName(); 
		request.EntityId = Xrm.Page.data.entity.getId().replace("{","").replace("}","");
		request.AttributeLogicalName = this._attributeLogicalName;
		request.getMetadata = function() {
			return {
				boundParameter: null,
				parameterTypes: {
					"EntityLogicalName": {
						"typeName": "Edm.String",
						"structuralProperty": 1
					},
					"EntityId": {
						"typeName": "Edm.String",
						"structuralProperty": 1
					},
					"AttributeLogicalName": {
						"typeName": "Edm.String",
						"structuralProperty": 1
					}
				},
				operationType: 0,
				operationName: "vnb_RetrieveAttributeHistory"
			};
		};

		const self = this;
		Xrm.WebApi.online.execute(request).then(
			function (result) {
				if (result.ok) {
					self.FetchStream(self, result.body);
				}
			},
			function (error) {
				Xrm.Utility.alertDialog(error.message, function(){});
			}
		);
	}

	public FetchStream(caller : EasyHistory, stream : ReadableStream) : void {
		const reader = stream.getReader();
		let text : string;
		text = "";
		reader.read().then(function processText({ done, value }) : void {  
			
			if(done)
			{
				let content: ActionResponse = JSON.parse(text);
				let historyDetails: AuditDetailModel[] = JSON.parse(content.History);
				for (let historyDetail of historyDetails) {
					caller.AddHistoryRecord(caller.CreateHistoryParamagraph(historyDetail.Value, historyDetail.ModifiedOn, historyDetail.User));
				}
				return;
			}
			
			if(value)
				text += new TextDecoder("utf-8").decode(value);
				reader.read().then(processText);
		});
	}

	public CreateHistoryParamagraph(historyValue: string, modifiedOn: string, userName: string) : HTMLDivElement
	{
		let div : HTMLDivElement;
		div = document.createElement("div");
		div.setAttribute("class","HistoryRecord");

		let elementHistory : HTMLParagraphElement;
		elementHistory = document.createElement("p");
		elementHistory.setAttribute("class", "HistoryValue");
		elementHistory.innerHTML = historyValue;
		div.append(elementHistory);

		let elementDetail : HTMLParagraphElement;
		elementDetail = document.createElement("p");
		elementDetail.setAttribute("class", "HistoryDetail");
		elementDetail.innerHTML = userName + ": " + modifiedOn;
		div.append(elementDetail);

		return div;
	}

	public AddHistoryRecord(element : HTMLParagraphElement)
	{
		this._historyContainer.appendChild(element);
	}

	public ClearHistory()
	{
		while (this._historyContainer.firstChild) {
			this._historyContainer.removeChild(this._historyContainer.firstChild);
		}
	}

	/**
	 * Called when any value in the property bag has changed. This includes field values, data-sets, global values such as container height and width, offline status, control metadata values such as label, visible, etc.
	 * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to names defined in the manifest, as well as utility functions
	 */
	public updateView(context: ComponentFramework.Context<IInputs>): void
	{
	}

	/** 
	 * It is called by the framework prior to a control receiving new data. 
	 * @returns an object based on nomenclature defined in manifest, expecting object[s] for property marked as “bound” or “output”
	 */
	public getOutputs(): IOutputs
	{
		return {};
	}

	/** 
	 * Called when the control is to be removed from the DOM tree. Controls should use this call for cleanup.
	 * i.e. cancelling any pending remote calls, removing listeners, etc.
	 */
	public destroy(): void
	{
		// Add code to cleanup control if necessary
	}
}
