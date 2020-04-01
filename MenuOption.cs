using System.ComponentModel;

namespace PresentationModel.Controls
{
    public enum MenuOption
    {
        // File -> Import Menu Options
        [Description("MenuImportRiskManagementDataFromExcel")]
        ImportRisksExcel,
        [Description("MenuImportAuditManagementDataFromExcel")]
        ImportAuditsExcel,
        // File -> Export Menu Options
        [Description("MenuExportRiskManagementDataToExcel")]
        ExportRisksExcel,
        [Description("MenuExportAuditManagementDataToExcel")]
        ExportAuditsExcel,
        // Edit Menu Options
        [Description("MenuBulkUpdateResponse")]
        BulkUpdateResponse,
        [Description("MenuBulkUpdateRisk")]
        BulkUpdateRisk,
        [Description("MenuDeleteSelectedRecords")]
        DeleteSelectedRecords,
        [Description("MenuViewRiskVisualiserView")]
        RiskVisualiserOption,
        [Description("MenuConvertToRiskType")]
        ConvertToOption
    }

    public enum ContextMenuOption
    {
        [Description("LinkSelectedToSummary")]
        LinkSelectedToSummary,
        [Description("LinkDetailRecordsToRisk")]
        LinkDetailRecordTo,
        [Description("UnlinkDetailRisk")]
        UnlinkDetailRecord,
        [Description("UnlinkSummaryRisk")]
        UnlinkSummaryRecord,
        [Description("CopySelected")]
        CopySelected,
        [Description("PasteSelectedLinkedHere")]
        PasteLinkedHere,
        [Description("UnlinkMasterRisk")]
        UnlinkMasterRecord,
        [Description("UnlinkCopyRisk")]
        UnlinkCopyRecord
    }
}