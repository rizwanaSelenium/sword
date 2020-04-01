using OpenQA.Selenium;

namespace PresentationModel.Controls
{
    public class DesktopGridLocators
    {
        private static string _recordType;

        public DesktopGridLocators(string recordType)
        {
            _recordType = recordType;
        }

        public string RecordCheckbox { get; } = "td input[type='checkbox']";
        public string GridRow { get; } = "tr.grid-row";

        public string TitleColumnLocator
        {
            get
            {
                _recordType = _recordType.ToLower();
                if (_recordType == "evaluation" || _recordType == "deficiency" || _recordType == "audit" ||  _recordType == "finding" || _recordType == "audit action")
                {
                    return "td[id$='_Title']";
                }
                if (_recordType == "risk" || _recordType == "plan" || _recordType == "response" || _recordType == "incident" || _recordType == "incident response" || _recordType == "alert" || _recordType == "document vault")
                {
                    return "td[id$='_Name']";
                }
                throw new WebDriverException("TitleColumnLocator - Unknown value passed in.");
            }
        }

        public string IdColumnLocator
        {
            get
            {
                switch (_recordType.ToLower())
                {
                    case "risk":
                        return "td[id$='_CustomRiskRef']";
                    case "plan":
                        return "td[id$='_CustomPlanRef']";
                    case "response":
                        return "td[id$='_CustomResponseRef']";
                    case "evaluation":
                        return "td[id$='_CustomEvaluationRef']";
                    case "deficiency":
                        return "td[id$='_CustomDeficiencyRef']";
                    case "incident":
                        return "td[id$='_CustomIncidentRef']";
                    case "incident response":
                        return "td[id$='_CustomResponseRef']";
                    case "audit":
                        return "td[id$='_CustomAuditRef']";
                    case "finding":
                        return "td[id$='_CustomFindingRef']";
                    case "audit action":
                        return "td[id$='_CustomAuditActionRef']";
                    case "alert":
                        return "td[id$='_AlertId']";
                    case "document vault":
                        return "td[id$='_DocumentIdItemId']";
                }
                throw new WebDriverException("NameColumnLocator - Unknown value passed in.");
            }
        }

        public string DeleteSelectedLocator
        {
            get
            {
                switch (_recordType.ToLower())
                {
                    case "risk":
                        return "DeleteSelected";
                    case "plan":
                        return "DeleteSelectedPlans";
                    case "response":
                        return "DeleteSelectedResponses";
                    case "evaluation":
                        return "DeleteSelectedEvaluations";
                }
                throw new WebDriverException("NameColumnLocator - Unknown value passed in.");
            }
        }

        public string DesktopGridId
        {
            get
            {
                switch (_recordType.ToLower())
                {
                    case "risk":
                        return "td[id$='_CustomRiskRef']";
                    case "plan":
                        return "td[id$='_CustomPlanRef']";
                    case "response":
                        return "td[id$='_CustomResponseRef']";
                    case "evaluation":
                        return "td[id$='_CustomEvaluationRef']";
                    case "deficiency":
                        return "td[id$='_CustomDeficiencyRef']";
                    case "incident":
                        return "td[id$='_CustomIncidentRef']";
                    case "incident response":
                        return "td[id$='_CustomResponseRef']";
                    case "audit":
                        return "td[id$='_CustomAuditRef']";
                    case "finding":
                        return "td[id$='_CustomFindingRef']";
                    case "audit action":
                        return "td[id$='_CustomAuditActionRef']";
                    case "alert":
                        return "td[id$='_AlertId']";
                    case "document vault":
                        return "td[id$='_DocumentIdItemId']";
                }
                throw new WebDriverException("Unknown grid type please check implemented.");
            }
        }

        public string RelatedGridSelector (string relatedGrid)
        {
            switch (relatedGrid.ToLower())
            {
                case "risk":
                    return "td[id$='_NumberOfRisks']";
                case "impact":
                    return "td[id$='_NumberOfImpacts']";
                case "plan":
                    return "td[id$='_NumberOfPlans']"; 
                case "response":
                    return "td[id$='_NumberOfResponses']"; 
                case "evaluation":
                    return "td[id$='_NumberOfEvaluations']";
                case "deficiency":
                    return "td[id$='_NumberOfDeficiencies']"; 
                case "documents":
                    return "td[id$='_NumberOfDocuments']"; 
                case "audits":
                    return "td[id$='_NumberOfAudits']"; 
                case "incidents":
                    return "td[id$='_NumberOfIncidents']"; 
            }
            throw new WebDriverException("RelatedGridSelector - Unknown value passed in.");
        }

        public string GridCellSelector(string cellToClick)
        {
            switch (cellToClick.ToLower())
            {
                case "current risk level":
                case "current assessment score":
                    return "td[id$='_CurrentAssessmentScore']";
                case "target risk level":
                case "target assessment score":
                    return "td[id$='_TargetAssessmentScore']";
            }
            throw new WebDriverException("GridCellSelector - Unknown value passed in.");
        }

    }
}
