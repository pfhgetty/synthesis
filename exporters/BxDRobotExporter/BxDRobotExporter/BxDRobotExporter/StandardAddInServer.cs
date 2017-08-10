using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventor;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using EditorsLibrary;
using System.Runtime.InteropServices;

namespace BxDRobotExporter
{
    /// <summary>
    /// With any luck i'll slowly replace StandardAddInServer.cs with this and just rename it and refactor the place holder functions to be the actual ApplicationAddInServer Methods instead of just calling them from there
    /// </summary>
    public partial class StandardAddInServer : Inventor.ApplicationAddInServer
    {
        enum JointNodeType
        {
            kParentNode,
            kChildNode
        }


        #region Variables 
        //(Variables that are defined in StandardAddinServer.cs but used here are named in comments)
        //ExporterEnv
        public static Inventor.Application MainApplication;

        AssemblyDocument AsmDocument;
        Inventor.Environment ExporterEnv;
        bool EnvironmentEnabled = false;

        //Makes sure that the application doesn't create a bunch of dockable windows. Nobody wants that crap.
        bool HiddenExporter = false;

        //Ribbon Pannels
        RibbonPanel FilePanel;
        RibbonPanel ExportPanel;
        RibbonPanel SettingsPanel;
        RibbonPanel HelpPanel;


        //Buttons
        ButtonDefinition LoadExportedRobotButton;
        ButtonDefinition ExportMeshesButton;
        ButtonDefinition ExportJointsButton;
        ButtonDefinition ExporterSettingsButton;
        ButtonDefinition HelpButton;

        //Highlighting
        HighlightSet ChildHighlight;
        bool IsParentHighlight = false;
        HighlightSet ParentHighlight;

        #region DEBUG
#if DEBUG
        RibbonPanel DebugPanel;
        ButtonDefinition DebugButton;
#endif
        #endregion
        #endregion


        #region ApplicationAddInServer Methods
        /// <summary>
        /// Called when the <see cref="StandardAddInServer"/> is being loaded
        /// </summary>
        /// <param name="AddInSiteObject"></param>
        /// <param name="FirstTime"></param>
        public void Activate(ApplicationAddInSite AddInSiteObject, bool FirstTime)
        {
            MainApplication = AddInSiteObject.Application; //Gets the application object, which is used in many different ways throughout this whole process
            string ClientID = "{0c9a07ad-2768-4a62-950a-b5e33b88e4a3}"; //TBH I don't really know why this is a GUID but whatever.
            #region Add Parallel Environment
            #region Load Images
            stdole.IPictureDisp StartExporterIconSmall = PictureDispConverter.ToIPictureDisp(new Bitmap(Resource.StartRobotExporter16));
            stdole.IPictureDisp StartExporterIconLarge = PictureDispConverter.ToIPictureDisp(new Bitmap(Resource.StartRobotExporter32));

            stdole.IPictureDisp ExportMeshesIconSmall = PictureDispConverter.ToIPictureDisp(new Bitmap(Resource.ExportMeshes16));
            stdole.IPictureDisp ExportMeshesIconLarge = PictureDispConverter.ToIPictureDisp(new Bitmap(Resource.ExportMeshes32));

            stdole.IPictureDisp ExportJointsIconSmall = PictureDispConverter.ToIPictureDisp(new Bitmap(Resource.ExportJoints16));
            stdole.IPictureDisp ExportJointsIconLarge = PictureDispConverter.ToIPictureDisp(new Bitmap(Resource.ExportJoints32));

            stdole.IPictureDisp ExporterSettingsIconSmall = PictureDispConverter.ToIPictureDisp(new Bitmap(Resource.ExporterSettings16));
            stdole.IPictureDisp ExporterSettingsIconLarge = PictureDispConverter.ToIPictureDisp(new Bitmap(Resource.ExporterSettings32));

            stdole.IPictureDisp HelpButtonIconSmall = PictureDispConverter.ToIPictureDisp(new Bitmap(Resource.Help16));
            stdole.IPictureDisp HelpButtonIconLarge = PictureDispConverter.ToIPictureDisp(new Bitmap(Resource.Help32));

            stdole.IPictureDisp LoadExportedRobotIconSmall = PictureDispConverter.ToIPictureDisp(new Bitmap(Resource.LoadRobot16));
            stdole.IPictureDisp LoadExportedRobotIconLarge = PictureDispConverter.ToIPictureDisp(new Bitmap(Resource.LoadRobot32));

            #region DEBUG
#if DEBUG
            stdole.IPictureDisp DebugButtonSmall = PictureDispConverter.ToIPictureDisp(new Bitmap(Resource.ViewerSettings16));
            stdole.IPictureDisp DebugButtonLarge = PictureDispConverter.ToIPictureDisp(new Bitmap(Resource.ViewerSettings32));
#endif
            #endregion

            #endregion
            #region UI Creation
            Environments environments = MainApplication.UserInterfaceManager.Environments;

            try
            {
                ExporterEnv = environments.Add("Robot Exporter", "BxD:RobotExporter:Environment", null, StartExporterIconSmall, StartExporterIconLarge);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                throw;
            }

            Ribbon assemblyRibbon = MainApplication.UserInterfaceManager.Ribbons["Assembly"];
            RibbonTab ExporterTab = assemblyRibbon.RibbonTabs.Add("Robot Exporter", "BxD:RobotExporter:RobotExporterTab", ClientID, "", false, true);

            ControlDefinitions ControlDefs = MainApplication.CommandManager.ControlDefinitions;

            FilePanel = ExporterTab.RibbonPanels.Add("File", "BxD:RobotExporter:FilePanel", ClientID);
            ExportPanel = ExporterTab.RibbonPanels.Add("Export", "BxD:RobotExporter:ExportPanel", ClientID);
            SettingsPanel = ExporterTab.RibbonPanels.Add("Settings", "BxD:RobotExporter:SettingsPanel", ClientID);
            HelpPanel = ExporterTab.RibbonPanels.Add("Help", "BxD:RobotExporter:HelpPanel", ClientID);

            //Load Exported Robot
            LoadExportedRobotButton = ControlDefs.AddButtonDefinition("Load Exported Robot", "BxD:RobotExporter:LoadExportedRobot", CommandTypesEnum.kNonShapeEditCmdType, ClientID, null, null, LoadExportedRobotIconSmall, LoadExportedRobotIconLarge);
            LoadExportedRobotButton.OnExecute += LoadExportedRobotButton_OnExecute;
            LoadExportedRobotButton.OnHelp += _OnHelp;
            FilePanel.CommandControls.AddButton(LoadExportedRobotButton, true);

            //Export Meshes
            ExportMeshesButton = ControlDefs.AddButtonDefinition("Export Meshes", "BxD:RobotExporter:ExportMeshes", CommandTypesEnum.kNonShapeEditCmdType, ClientID, null, null, ExportMeshesIconSmall, ExportMeshesIconLarge);
            ExportMeshesButton.OnExecute += ExportMeshes_OnExecute;
            ExportMeshesButton.OnHelp += _OnHelp;
            ExportPanel.CommandControls.AddButton(ExportMeshesButton, true);

            //Export Joints
            ExportJointsButton = ControlDefs.AddButtonDefinition("Export Joints", "BxD:RobotExporter:ExportJoints", CommandTypesEnum.kNonShapeEditCmdType, ClientID, null, null, ExportJointsIconSmall, ExportJointsIconLarge);
            ExportJointsButton.OnExecute += ExportJointsButton_OnExecute;
            ExportMeshesButton.OnHelp += _OnHelp;
            ExportPanel.CommandControls.AddButton(ExportJointsButton, true);

            //Exporter Settings
            ExporterSettingsButton = ControlDefs.AddButtonDefinition("Exporter Settings", "BxD:RobotExporter:ExporterSettings", CommandTypesEnum.kNonShapeEditCmdType, ClientID, null, null, ExporterSettingsIconLarge, ExporterSettingsIconLarge);
            ExporterSettingsButton.OnExecute += ExporterSettings_OnExecute;
            ExporterSettingsButton.OnHelp += _OnHelp;
            SettingsPanel.CommandControls.AddButton(ExporterSettingsButton, true);

            //Help Button
            HelpButton = ControlDefs.AddButtonDefinition("Help", "BxD:RobotExporter:Help", CommandTypesEnum.kNonShapeEditCmdType, ClientID, null, null, HelpButtonIconSmall, HelpButtonIconLarge);
            HelpButton.OnExecute += HelpButton_OnExecute;
            HelpButton.OnHelp += _OnHelp;
            HelpPanel.CommandControls.AddButton(HelpButton, true);

            #region DEBUG
#if DEBUG
            DebugPanel = ExporterTab.RibbonPanels.Add("Debug", "BxD:RobotExporter:DebugPanel", ClientID);
            DebugButton = ControlDefs.AddButtonDefinition("Debug", "BxD:RobotExporter:DebugButton", CommandTypesEnum.kNonShapeEditCmdType, ClientID, null, null, DebugButtonSmall, DebugButtonLarge);
            DebugButton.OnExecute += delegate (NameValueMap context)
            {
                Forms.DebugHighlightForm dhf = new Forms.DebugHighlightForm();
                if (dhf.ShowDialog(out string ComponentName) == DialogResult.OK)
                {
                    SelectNode(ComponentName, JointNodeType.kChildNode);
                }
            };
            DebugPanel.CommandControls.AddButton(DebugButton, true);
#endif
            #endregion

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
            #endregion
        }

        /// <summary>
        /// Called when the <see cref="StandardAddInServer"/> is being unloaded
        /// </summary>
        public void Deactivate()
        {
            Marshal.ReleaseComObject(MainApplication);
            MainApplication = null;

            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        public void ExecuteCommand(int commandID)
        {
            // Note:this method is now obsolete, you should use the 
            // ControlDefinition functionality for implementing commands.
        }

        public object Automation
        {
            // This property is provided to allow the AddIn to expose an API 
            // of its own to other programs. Typically, this  would be done by
            // implementing the AddIn's API interface in a class and returning 
            // that class object through this property.

            get
            {
                // TODO: Add ApplicationAddInServer.Automation getter implementation
                return null;
            }
        }
        #endregion

        /// <summary>
        /// Enables or disables the environment
        /// </summary>
        /// <remarks>
        /// calls StartExporter and EndExporter
        /// </remarks>
        public void ToggleEnvironment()
        {
            if (EnvironmentEnabled)
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
            //Gets the assembly document and creates dockable windows
            AsmDocument = (AssemblyDocument)MainApplication.ActiveDocument;
            Utilities.CreateDockableWindows(MainApplication);
            ChildHighlight = AsmDocument.CreateHighlightSet();
            ChildHighlight.Color = Utilities.GetInventorColor(SynthesisGUI.ExporterSettings.InventorChildColor);
            ParentHighlight = AsmDocument.CreateHighlightSet();
            ParentHighlight.Color = Utilities.GetInventorColor(SynthesisGUI.ExporterSettings.InventorParentColor);

            //Sets up events for selecting and deselecting parts in inventor
            Utilities.GUI.jointEditorPane1.SelectedJoint += JointEditorPane_SelectedJoint;
            ExporterSettingsForm.PluginSettingsValues.SettingsChanged += ExporterSettings_SettingsChanged;
        }


        private void EndExporter()
        {
            AsmDocument = null;
            Utilities.DisposeDockableWindows();
            ChildHighlight = null;
        }

        private void SelectNode(string Name, JointNodeType jointNodeType)
        {
            switch (jointNodeType)
            {
                case JointNodeType.kParentNode:
                    foreach (ComponentOccurrence Occ in AsmDocument.ComponentDefinition.Occurrences)
                    {
                        if (Occ.Name == Name)
                        {
                            ParentHighlight.AddItem(Occ);
                        }
                    }
                    break;
                case JointNodeType.kChildNode:
                    foreach (ComponentOccurrence Occ in AsmDocument.ComponentDefinition.Occurrences)
                    {
                        if (Occ.Name == Name)
                        {
                            ChildHighlight.AddItem(Occ);
                        }
                    }
                    break;
            }


        }

        #region Event Callbacks
        /// <summary>
        /// Makes the dockable windows invisible when the document switches. This avoids data loss.
        /// </summary>
        /// <param name="DocumentObject"></param>
        /// <param name="BeforeOrAfter"></param>
        /// <param name="Context"></param>
        /// <param name="HandlingCode"></param>
        private void ApplicationEvents_OnDeactivateDocument(_Document DocumentObject, EventTimingEnum BeforeOrAfter, NameValueMap Context, out HandlingCodeEnum HandlingCode)
        {
            if (BeforeOrAfter == EventTimingEnum.kBefore && EnvironmentEnabled)
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
            if (Environment.Equals(ExporterEnv) && EnvironmentState == EnvironmentStateEnum.kActivateEnvironmentState && !EnvironmentEnabled && BeforeOrAfter == EventTimingEnum.kBefore)
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

        /// <summary>
        /// Opens the <see cref="ExporterSettingsForm"/> form to allow the user to customize their experience
        /// </summary>
        /// <param name="Context"></param>
        private void ExporterSettings_OnExecute(NameValueMap Context)
        {
            Utilities.GUI.SettingsExporter_OnClick(this, null);
        }


        private void ExportMeshes_OnExecute(NameValueMap Context)
        {
            Utilities.GUI.LoadFromInventor();
        }

        private void LoadExportedRobotButton_OnExecute(NameValueMap Context)
        {
            Utilities.GUI.OpenExisting();
        }

        private void ExportJointsButton_OnExecute(NameValueMap Context)
        {
            Utilities.GUI.SaveRobot(true);
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

        private void JointEditorPane_SelectedJoint(List<RigidNode_Base> nodes)
        {
            ChildHighlight.Clear();
            ParentHighlight.Clear();
            if (nodes == null)
            {
                return;
            }
            foreach(RigidNode_Base node in nodes)
            {
                SelectNode(node.GetModelID().Substring(0, node.GetModelID().Length - 3), JointNodeType.kChildNode);
                if (node.GetParent() != null && IsParentHighlight)
                {
                    string[] Nodes = node.GetParent().ModelFullID.Split(new char[] { '-', '_', '-' });
                    foreach(string name in Nodes)
                    {
                        SelectNode(name, JointNodeType.kParentNode);
                    }
                }
            }

        }

        private void ExporterSettings_SettingsChanged(System.Drawing.Color Child, System.Drawing.Color Parent, bool IsParentHighlight)
        {
            ChildHighlight.Color = Utilities.GetInventorColor(Child);
            ParentHighlight.Color = Utilities.GetInventorColor(Parent);
            this.IsParentHighlight = IsParentHighlight;
        }
        #endregion




    }
}