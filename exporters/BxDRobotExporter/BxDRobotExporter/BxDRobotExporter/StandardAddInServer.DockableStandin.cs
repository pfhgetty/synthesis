using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventor;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace BxDRobotExporter
{
    /// <summary>
    /// With any luck i'll slowly replace StandardAddInServer.cs with this and just rename it and refactor the place holder functions to be the actual ApplicationAddInServer Methods instead of just calling them from there
    /// </summary>
    public partial class StandardAddInServer : Inventor.ApplicationAddInServer
    {

        #region Variables 
        //(Variables that are defined in StandardAddinServer.cs but used here are named in comments)
        //ExporterEnv
        //MainApplication
        bool EnvironmentEnabled = false;

        //Makes sure that the application doesn't create a bunch of dockable windows. Nobody wants that crap.
        bool FirstRun = true;
        bool HiddenExporter = false;

        RibbonPanel FilePanel;
        RibbonPanel ExportPanel;
        RibbonPanel SettingsPanel;
        RibbonPanel HelpPanel;

        ButtonDefinition LoadFromInventorButton;
        ButtonDefinition ExportMeshesButton;
        ButtonDefinition ExporterSettingsButton;
        ButtonDefinition ViewerSettingsButton;
        ButtonDefinition HelpButton;

        AssemblyDocument AsmDocument;

        #endregion

        public void ActivateDockable(Inventor.ApplicationAddInSite AddInSiteObject, bool FirstTime)
        {
            MainApplication = AddInSiteObject.Application; //Gets the application object, which is used in many different ways throughout this whole process
            string ClientID = "{0c9a07ad-2768-4a62-950a-b5e33b88e4a3}"; //TBH I don't really know why this is a GUID but whatever.
            #region Load Images
            stdole.IPictureDisp startExporterIconSmall = PictureDispConverter.ToIPictureDisp(new Bitmap(BxDRobotExporter.Resource.StartRobotExporter16));
            stdole.IPictureDisp startExporterIconLarge = PictureDispConverter.ToIPictureDisp(new Bitmap(BxDRobotExporter.Resource.StartRobotExporter32));

            stdole.IPictureDisp LoadFromInventorIconSmall = PictureDispConverter.ToIPictureDisp(new Bitmap(Resource.StartRobotExporter16)); //Placeholder
            stdole.IPictureDisp LoadFromInventorIconLarge = PictureDispConverter.ToIPictureDisp(new Bitmap(Resource.StartRobotExporter32)); //Placeholder

            stdole.IPictureDisp ExportMeshesIconSmall = PictureDispConverter.ToIPictureDisp(new Bitmap(Resource.ExportRobot16)); //Placeholder
            stdole.IPictureDisp ExportMeshesIconLarge = PictureDispConverter.ToIPictureDisp(new Bitmap(Resource.ExportRobot32)); //Placeholder

            stdole.IPictureDisp ExporterSettingsIconSmall = PictureDispConverter.ToIPictureDisp(new Bitmap(Resource.EditDrivers16)); //Placeholder
            stdole.IPictureDisp ExporterSettingsIconLarge = PictureDispConverter.ToIPictureDisp(new Bitmap(Resource.EditDrivers32)); //Placeholder

            stdole.IPictureDisp ViewerSettingsIconSmall = PictureDispConverter.ToIPictureDisp(new Bitmap(Resource.EditDrivers32)); //Placeholder
            stdole.IPictureDisp ViewerSettingsIconLarge = PictureDispConverter.ToIPictureDisp(new Bitmap(Resource.EditDrivers32)); //Placeholder
            #endregion
            #region UI Creation
            Environments environments = MainApplication.UserInterfaceManager.Environments;
            ExporterEnv = environments.Add("Robot Exporter", "BxD:RobotExporter:Environment", null, startExporterIconSmall, startExporterIconLarge);

            Ribbon assemblyRibbon = MainApplication.UserInterfaceManager.Ribbons["Assembly"];
            RibbonTab ExporterTab = assemblyRibbon.RibbonTabs.Add("Robot Exporter", "BxD:RobotExporter:RobotExporterTab", ClientID, "", false, true);

            ControlDefinitions ControlDefs = MainApplication.CommandManager.ControlDefinitions;

            FilePanel = ExporterTab.RibbonPanels.Add("File", "BxD:RobotExporter:FilePanel", ClientID);
            ExportPanel = ExporterTab.RibbonPanels.Add("Export", "BxD:RobotExporter:ExportPanel", ClientID);
            SettingsPanel = ExporterTab.RibbonPanels.Add("Settings", "BxD:RobotExporter:SettingsPanel", ClientID);
            HelpPanel = ExporterTab.RibbonPanels.Add("Help", "BxD:RobotExporter:HelpPanel", ClientID);

            //Load From Inventor
            LoadFromInventorButton = ControlDefs.AddButtonDefinition("Open Exported Mesh", "BxD:RobotExporter:OpenExportedMesh", CommandTypesEnum.kNonShapeEditCmdType, ClientID, null, null, LoadFromInventorIconSmall, LoadFromInventorIconLarge);
            LoadFromInventorButton.OnExecute += OpenExportedMesh_OnExecute;
            LoadFromInventorButton.OnHelp += _OnHelp;
            FilePanel.CommandControls.AddButton(LoadFromInventorButton, true);

            //Export Meshes
            ExportMeshesButton = ControlDefs.AddButtonDefinition("Export Meshes", "BxD:RobotExporter:ExportMeshes", CommandTypesEnum.kNonShapeEditCmdType, ClientID, null, null, ExportMeshesIconSmall, ExportMeshesIconLarge);
            ExportMeshesButton.OnExecute += ExportMeshes_OnExecute;
            ExportMeshesButton.OnHelp += _OnHelp;
            ExportPanel.CommandControls.AddButton(ExportMeshesButton, true);

            //Exporter Settings
            ExporterSettingsButton = ControlDefs.AddButtonDefinition("Exporter Settings", "BxD:RobotExporter:ExporterSettings", CommandTypesEnum.kNonShapeEditCmdType, ClientID, null, null, ExporterSettingsIconLarge, ExporterSettingsIconLarge);
            ExporterSettingsButton.OnExecute += ExporterSettings_OnExecute;
            ExporterSettingsButton.OnHelp += _OnHelp;
            SettingsPanel.CommandControls.AddButton(ExporterSettingsButton, true);

            //Viewer Settings
            ViewerSettingsButton = ControlDefs.AddButtonDefinition("Viewer Settings", "BxD:RobotExporter:ViewerSettings", CommandTypesEnum.kNonShapeEditCmdType, ClientID, null, null, ViewerSettingsIconSmall, ViewerSettingsIconLarge);
            ViewerSettingsButton.OnExecute += ViewerSettings_OnExecute;
            ViewerSettingsButton.OnHelp += _OnHelp;
            SettingsPanel.CommandControls.AddButton(ViewerSettingsButton, true);

            //Help Button
            HelpButton = ControlDefs.AddButtonDefinition("Help", "BxD:RobotExporter:Help", CommandTypesEnum.kNonShapeEditCmdType, ClientID, null, null, ViewerSettingsIconSmall, ViewerSettingsIconLarge);
            HelpButton.OnExecute += HelpButton_OnExecute;
            HelpButton.OnHelp += _OnHelp;
            HelpPanel.CommandControls.AddButton(HelpButton, true);
            #endregion
            #region Final Environment Setup
            ExporterEnv.DefaultRibbonTab = "BxD:RobotExporter:RobotExporterTab";
            MainApplication.UserInterfaceManager.ParallelEnvironments.Add(ExporterEnv);
            ExporterEnv.DisabledCommandList.Add(MainApplication.CommandManager.ControlDefinitions["BxD:RobotExporter:Environment"]);
            #endregion
            #region Event Handler Assignment
            UserInterfaceEvents UIEvents = MainApplication.UserInterfaceManager.UserInterfaceEvents;
            UIEvents.OnEnvironmentChange += UIEvents_OnEnvironmentChange;
            MainApplication.ApplicationEvents.OnActivateDocument += ApplicationEvents_OnActivateDocument;
            MainApplication.ApplicationEvents.OnDeactivateDocument += ApplicationEvents_OnDeactivateDocument;
            #endregion
        }

        private void ApplicationEvents_OnDeactivateDocument(_Document DocumentObject, EventTimingEnum BeforeOrAfter, NameValueMap Context, out HandlingCodeEnum HandlingCode)
        {
            if(BeforeOrAfter == EventTimingEnum.kBefore && EnvironmentEnabled)
            {
                Utilities.HideDockableWindows();
                HiddenExporter = true;
            }
            HandlingCode = HandlingCodeEnum.kEventNotHandled;
        }

        /// <summary>
        /// Disables the environment button if you aren't in an assembly document.
        /// </summary>
        /// <param name="DocumentObject"></param>
        /// <param name="BeforeOrAfter"></param>
        /// <param name="Context"></param>
        /// <param name="HandlingCode"></param>
        private void ApplicationEvents_OnActivateDocument(_Document DocumentObject, EventTimingEnum BeforeOrAfter, NameValueMap Context, out HandlingCodeEnum HandlingCode)
        {
            if (DocumentObject is PartDocument Part)
            {
                Part.DisabledCommandList.Add(MainApplication.CommandManager.ControlDefinitions["BxD:RobotExporter:Environment"]);
            }
            else if (DocumentObject is PresentationDocument Presentation)
            {
                Presentation.DisabledCommandList.Add(MainApplication.CommandManager.ControlDefinitions["BxD:RobotExporter:Environment"]);
            }
            else if (DocumentObject is DrawingDocument Drawing)
            {
                Drawing.DisabledCommandList.Add(MainApplication.CommandManager.ControlDefinitions["BxD:RobotExporter:Environment"]);
            }
            else if (DocumentObject.Equals(AsmDocument) && HiddenExporter)
            {
                Utilities.ShowDockableWindows();
                HiddenExporter = false;
            }
            HandlingCode = HandlingCodeEnum.kEventNotHandled;
        }

        /// <summary>
        /// Checks to make sure that you are in an assembly document and then readies for environment changing
        /// </summary>
        /// <param name="Environment"></param>
        /// <param name="EnvironmentState"></param>
        /// <param name="BeforeOrAfter"></param>
        /// <param name="Context"></param>
        /// <param name="HandlingCode"></param>
        private void UIEvents_OnEnvironmentChange(Inventor.Environment Environment, EnvironmentStateEnum EnvironmentState, EventTimingEnum BeforeOrAfter, NameValueMap Context, out HandlingCodeEnum HandlingCode)
        {
            if(Environment.Equals(ExporterEnv) && EnvironmentState == EnvironmentStateEnum.kActivateEnvironmentState && !EnvironmentEnabled && BeforeOrAfter == EventTimingEnum.kBefore)
            {
                ToggleEnvironment();
            }
            else if (Environment.Equals(ExporterEnv) && EnvironmentState == EnvironmentStateEnum.kTerminateEnvironmentState && EnvironmentEnabled && BeforeOrAfter == EventTimingEnum.kBefore)
            {
                ToggleEnvironment();
            }
            HandlingCode = HandlingCodeEnum.kEventNotHandled;
        }

        /// <summary>
        /// Also opens the help page
        /// </summary>
        private void HelpButton_OnExecute(NameValueMap Context)
        {
            Process.Start("http://bxd.autodesk.com/tutorials.html");
        }

        private void ViewerSettings_OnExecute(NameValueMap Context)
        {
            Utilities.GUI.SettingsViewer_OnClick(this, null);
        }

        private void ExporterSettings_OnExecute(NameValueMap Context)
        {
            Utilities.GUI.SettingsExporter_OnClick(this, null);
        }

        private void ExportMeshes_OnExecute(NameValueMap Context)
        {
            Utilities.GUI.FileLoad_OnClick(null, null);
        }

        private void OpenExportedMesh_OnExecute(NameValueMap Context)
        {
            Utilities.GUI.OpenExisting();
        }

        /// <summary>
        /// Opens the help page on bxd.autodesk.com. This is the callback used for all OnHelp events.
        /// </summary>
        /// <param name="Context"></param>
        /// <param name="HandlingCode"></param>
        private void _OnHelp(NameValueMap Context, out HandlingCodeEnum HandlingCode)
        {
            Process.Start("http://bxd.autodesk.com/tutorials.html");
            HandlingCode = HandlingCodeEnum.kEventHandled;
        }

        /// <summary>
        /// Enables or disables the environment
        /// </summary>
        /// <remarks>
        /// calls StartExporter and EndExporter
        /// </remarks>
        public void ToggleEnvironment()
        {
            if(EnvironmentEnabled)
            {
                EnvironmentEnabled = false;
                EndExporter();
            }
            else 
            {
                EnvironmentEnabled = true;
                StartExporter();
            }
        }

        private void StartExporter()
        {
            AsmDocument = (AssemblyDocument)MainApplication.ActiveDocument;

            Utilities.CreateDockableWindows(MainApplication);
        }
        private void EndExporter()
        {
            Utilities.DisposeDockableWindows();
        }
    }
}
