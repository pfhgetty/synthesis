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
    /// This is where the magic happens. All top-level event handling, UI creation, and inventor communication is handled here.
    /// </summary>
    public class StandardAddInServer : ApplicationAddInServer
    {
        #region Variables 

        public static StandardAddInServer Instance;
        public ExporterLogger Logger = new ExporterLogger();

        public Inventor.Application MainApplication;

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
        ButtonDefinition SelectionTestButton;
        ButtonDefinition UITestButton;
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

            #region Setup New Environment and Ribbon
            Environments environments = MainApplication.UserInterfaceManager.Environments;
            ExporterEnv = environments.Add("Robot Exporter", "BxD:RobotExporter:Environment", null, StartExporterIconSmall, StartExporterIconLarge);

            Ribbon assemblyRibbon = MainApplication.UserInterfaceManager.Ribbons["Assembly"];
            RibbonTab ExporterTab = assemblyRibbon.RibbonTabs.Add("Robot Exporter", "BxD:RobotExporter:RobotExporterTab", ClientID, "", false, true);

            ControlDefinitions ControlDefs = MainApplication.CommandManager.ControlDefinitions;

            FilePanel = ExporterTab.RibbonPanels.Add("File", "BxD:RobotExporter:FilePanel", ClientID);
            ExportPanel = ExporterTab.RibbonPanels.Add("Export", "BxD:RobotExporter:ExportPanel", ClientID);
            SettingsPanel = ExporterTab.RibbonPanels.Add("Settings", "BxD:RobotExporter:SettingsPanel", ClientID);
            HelpPanel = ExporterTab.RibbonPanels.Add("Help", "BxD:RobotExporter:HelpPanel", ClientID);
            #endregion

            #region Setup Buttons
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
            //Selection Test
            SelectionTestButton = ControlDefs.AddButtonDefinition("Selection Test", "BxD:RobotExporter:SelectionTestButton", CommandTypesEnum.kNonShapeEditCmdType, ClientID, null, null, DebugButtonSmall, DebugButtonLarge);
            SelectionTestButton.OnExecute += delegate (NameValueMap context)
            {
                Forms.DebugHighlightForm dhf = new Forms.DebugHighlightForm();
                if (dhf.ShowDialog(out string ComponentName) == DialogResult.OK)
                {
                    SelectNode(ComponentName, JointNodeTypeEnum.kChildNode);
                }
            };
            DebugPanel.CommandControls.AddButton(SelectionTestButton, true);
            //UI Test
            UITestButton = ControlDefs.AddButtonDefinition("UI Test", "BxD:RobotExporter:UITestButton", CommandTypesEnum.kNonShapeEditCmdType, ClientID, null, null, DebugButtonSmall, DebugButtonLarge);
            UITestButton.OnExecute += delegate (NameValueMap context)
            {
                LiteExporterForm ExportLite = new LiteExporterForm(Logger);
                ExportLite.ShowDialog();
            };
            DebugPanel.CommandControls.AddButton(UITestButton, true);
#endif
            #endregion 
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

            Instance = this;
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

        #region Environment Switching
        /// <summary>
        /// Enables or disables the <see cref="Inventor.Environment"/>
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
                Logger.DisposeWriter();
            }
            else
            {
                EnvironmentEnabled = true;
                StartExporter();
                #region DEBUG SWITCH
#if DEBUG
                Logger = new ExporterLogger(ExporterLogger.LoggerMode.Precise);
#else
            Logger = new ExporterLogger();
#endif
                #endregion

            }
        }

        /// <summary>
        /// Gets the assembly document and makes the <see cref="DockableWindows"/>
        /// </summary>
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

        /// <summary>
        /// Disposes of some COM objects and exits the environment
        /// </summary>
        private void EndExporter()
        {
            AsmDocument = null;
            Utilities.DisposeDockableWindows();
            ChildHighlight = null;
        } 
        #endregion

        #region Event Callbacks and Button Commands
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
            Process.Start("http://bxd.autodesk.com/tutorial-robot.html");
        }

        /// <summary>
        /// Opens the <see cref="ExporterSettingsForm"/> form to allow the user to customize their experience
        /// </summary>
        /// <param name="Context"></param>
        private void ExporterSettings_OnExecute(NameValueMap Context)
        {
            Utilities.GUI.SettingsExporter_OnClick(this, null);
        }

        /// <summary>
        /// Opens the <see cref="LiteExporterForm"/> through <see cref="Utilities.GUI"/>
        /// </summary>
        /// <param name="Context"></param>
        private void ExportMeshes_OnExecute(NameValueMap Context)
        {
            if(Utilities.GUI.SkeletonBase != null)
            {
                Utilities.GUI.SetNew();
            }
            Utilities.GUI.ExportMeshes();
        }

        /// <summary>
        /// Opens a <see cref="FolderBrowserDialog"/> and prompts the user to select a robot folder. 
        /// Note: soon this should be replaced with an <see cref="OpenFileDialog"/> when the old format is merged into one file.
        /// </summary>
        /// <param name="Context"></param>
        private void LoadExportedRobotButton_OnExecute(NameValueMap Context)
        {
            Utilities.GUI.OpenExisting(ValidateAssembly);
        }

        /// <summary>
        /// Opens a <see cref="FolderBrowserDialog"/> and prompts the user to select the folder where they want their robot to be saved.
        /// Note: soon this should be replaced with an <see cref="OpenFileDialog"/> when the old format is merged into one file.
        /// </summary>
        /// <param name="Context"></param>
        private void ExportJointsButton_OnExecute(NameValueMap Context)
        {
            if(Utilities.GUI.SkeletonBase == null)
            {
                MessageBox.Show("Please load or generate meshes before exporting joints");
                return;
            }
            Utilities.GUI.SaveRobot(true);
        }

        /// <summary>
        /// Opens the help page on bxd.autodesk.com. This is the callback used for all OnHelp events.
        /// </summary>
        /// <param name="Context"></param>
        /// <param name="HandlingCode"></param>
        private void _OnHelp(NameValueMap Context, out HandlingCodeEnum HandlingCode)
        {
            Process.Start("http://bxd.autodesk.com/tutorial-robot.html");
            HandlingCode = HandlingCodeEnum.kEventHandled;
        }

        /// <summary>
        /// Selects all the parts in inventor associated with the given joint or joints.
        /// </summary>
        /// <param name="nodes"></param>
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
                SelectNode(node.GetModelID().Substring(0, node.GetModelID().Length - 3), JointNodeTypeEnum.kChildNode);
                if (node.GetParent() != null && IsParentHighlight)
                {
                    string[] Nodes = node.GetParent().ModelFullID.Split(new char[] { '-', '_', '-' });
                    foreach(string name in Nodes)
                    {
                        SelectNode(name, JointNodeTypeEnum.kParentNode);
                    }
                }
            }

        }

        /// <summary>
        /// Called when the user presses 'OK' in the settings menu
        /// </summary>
        /// <param name="Child"></param>
        /// <param name="Parent"></param>
        /// <param name="IsParentHighlight"></param>
        private void ExporterSettings_SettingsChanged(System.Drawing.Color Child, System.Drawing.Color Parent, bool IsParentHighlight)
        {
            ChildHighlight.Color = Utilities.GetInventorColor(Child);
            ParentHighlight.Color = Utilities.GetInventorColor(Parent);
            this.IsParentHighlight = IsParentHighlight;
        }
        #endregion

        #region Miscellaneous Methods
        /// <summary>
        /// Checks if a baseNode matches up with the assembly. Passed as a <see cref="ValidationAction"/> to
        /// </summary>
        /// <param name="baseNode"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private bool ValidateAssembly(RigidNode_Base baseNode, out string message)
        {
            int ValidationCount = 0;
            int FailedCount = 0;
            List<RigidNode_Base> nodes = baseNode.ListAllNodes();
            foreach (RigidNode_Base node in nodes)
            {
                bool FailedValidation = false;
                foreach (string componentName in node.ModelFullID.Split(new string[] { "-_-" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (!CheckForOccurrence(componentName))
                    {
                        FailedCount++;
                        FailedValidation = true;
                    }
                }
                if (!FailedValidation)
                {
                    ValidationCount++;
                }
            }
            if (ValidationCount == nodes.Count)
            {
                message = string.Format("The assembly validated successfully. {0} / {1} nodes checked out.", ValidationCount, nodes.Count);
                return true;
            }
            else
            {
                message = string.Format("The assembly failed to validate. {0} / {1} nodes checked out. {2} parts/assemblies were not found.", ValidationCount, nodes.Count, FailedCount);
                return false;
            }
        }

        private bool CheckForOccurrence(string name)
        {
            foreach (ComponentOccurrence component in AsmDocument.ComponentDefinition.Occurrences)
            {
                if (component.Name == name)
                    return true;
            }
            return false;
        }

        private void SelectNode(string Name, JointNodeTypeEnum jointNodeType)
        {
            switch (jointNodeType)
            {
                case JointNodeTypeEnum.kParentNode:
                    foreach (ComponentOccurrence Occ in AsmDocument.ComponentDefinition.Occurrences)
                    {
                        if (Occ.Name == Name)
                        {
                            ParentHighlight.AddItem(Occ);
                        }
                    }
                    break;
                case JointNodeTypeEnum.kChildNode:
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
        #endregion
    }
}