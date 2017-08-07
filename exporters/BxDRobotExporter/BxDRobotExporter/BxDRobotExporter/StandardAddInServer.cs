using System;
using System.Runtime.InteropServices;
using Inventor;
using Microsoft.Win32;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Collections;
using System.Xml;
using System.Timers;
using System.Threading;
using System.Security.Permissions;
using ExportProcess;

namespace BxDRobotExporter
{
    /// <summary>
    /// This is the primary AddIn Server class that implements the ApplicationAddInServer interface
    /// that all Inventor AddIns are required to implement. The communication between Inventor and
    /// the AddIn is via the methods on this interface.
    /// </summary>


    //TLDR exports the robot to the simulator


    [Guid("0c9a07ad-2768-4a62-950a-b5e33b88e4a3")]
    public partial class StandardAddInServer : Inventor.ApplicationAddInServer
    {

        #region data
        public static Inventor.Application MainApplication;

        Document oDoc;

        BrowserPanes oPanes;

        Boolean inExportView;

        int jointNumber;

        Boolean Rotating;

        EnvironmentManager envMan;
        static Inventor.ButtonDefinition StartExport;
        static Inventor.ButtonDefinition ExportRobot;
        static Inventor.ButtonDefinition CancelExport;
        static Inventor.ButtonDefinition SelectJointInsideJoint;
        static Inventor.ButtonDefinition EditDrivers;
        static Inventor.ButtonDefinition EditLimits;
        static Inventor.ButtonDefinition Test;

        static Inventor.BrowserPane oPane;

        static HighlightSet oSet;

        Inventor.ComboBoxDefinitionSink_OnSelectEventHandler JointsComboBox_OnSelectEventDelegate;
        Inventor.ComboBoxDefinitionSink_OnSelectEventHandler LimitsComboBox_OnSelectEventDelegate;
        Inventor.UserInputEventsSink_OnSelectEventHandler click_OnSelectEventDelegate;
        Inventor.UserInterfaceEventsSink_OnEnvironmentChangeEventHandler enviroment_OnChangeEventDelegate;

        ConfigureJointForm ConfigureJoint;
        static bool DoWork;

        EditLimits EditLimitsForm;

        UserControl1 control;

        public string m_ClientId;

        static private ComboBoxDefinition JointsComboBox;
        static private ComboBoxDefinition LimitsComboBox;

        ArrayList joints;

        static Document nativeDoc;

        JointData selectedJointData;

        Inventor.Ribbon PartRibbon;

        Inventor.Environment ExporterEnv;

        public Object z;
        public Object v;

        public static String pathToSaveTo;
        Inventor.RibbonPanel PartPanel;
        Inventor.RibbonPanel PartPanel2;
        Inventor.RibbonPanel PartPanel3;
        Inventor.RibbonPanel ModelControls;

        Random Rand;
        JointData AssemblyJoint;
        String AddInCLSIDString;

        ClientNodeResource oRsc;

        int i;
        UserInputEvents UIEvent;

        static ArrayList selectedJoints;

        static ArrayList jointList;
        #endregion

        public StandardAddInServer()
        {
        }

        #region ApplicationAddInServer Members

        public void Activate(Inventor.ApplicationAddInSite addInSiteObject, bool firstTime)
        {
            //try
            //{
            //    // This method is called by Inventor when it loads the addin.
            //    // The AddInSiteObject provides access to the Inventor Application object.
            //    // The FirstTime flag indicates if the addin is loaded for the first time.

            //    // Initialize AddIn members.


            //    // TODO: Add ApplicationAddInServer.Activate implementation.
            //    // e.g. event initialization, command creation etc.
            //    GuidAttribute addInCLSID;
            //    addInCLSID = (GuidAttribute)GuidAttribute.GetCustomAttribute(typeof(StandardAddInServer), typeof(GuidAttribute));
            //    AddInCLSIDString = "{" + addInCLSID.Value + "}";
            //    m_ClientId = "0c9a07ad-2768-4a62-950a-b5e33b88e4a3";
            //    inExportView = false;
            //    control = new UserControl1();
            //    MainApplication = addInSiteObject.Application;
            //    jointNumber = 1;

            //    DoWork = false;

            //    closing = false;

            //    selectedJoints = new ArrayList();

            //    Rotating = true;

            //    Rand = new Random();

            //    ConfigureJoint = new ConfigureJointForm();
            //    EditLimitsForm = new EditLimits();
            //    if(firstTime)
            //    {
            //        AddParallelEnvironment();
            //    }
            //    UIEvent = MainApplication.CommandManager.UserInputEvents;
            //    click_OnSelectEventDelegate = new UserInputEventsSink_OnSelectEventHandler(UIEvents_OnSelect);
            //    UIEvent.OnSelect += click_OnSelectEventDelegate;
            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show(e.ToString());
            //}
            ActivateDockable(addInSiteObject, firstTime);
        }

        public void Deactivate()
        {
            // This method is called by Inventor when the AddIn is unloaded.
            // The AddIn will be unloaded either manually by the user or
            // when the Inventor session is terminated

            // TODO: Add ApplicationAddInServer.Deactivate implementation

            // Release objects.
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

        bool closing;
        public void OnEnvironmentChange(Inventor.Environment environment, EnvironmentStateEnum EnvironmentState, EventTimingEnum BeforeOrAfter, NameValueMap Context, out HandlingCodeEnum HandlingCode)
        {
            if (environment.Equals(ExporterEnv) && EnvironmentState.Equals(EnvironmentStateEnum.kActivateEnvironmentState) && !closing)
            {
                closing = true;
                StartExport_OnExecute(null);
            }
            else if (environment.Equals(ExporterEnv) && EnvironmentState.Equals(EnvironmentStateEnum.kTerminateEnvironmentState) && closing)
            {
                closing = false;
                CancelExport_OnExecute(null);
            }
            HandlingCode = HandlingCodeEnum.kEventNotHandled;
        }
        public void SelectJointInsideJoint_OnExecute(Inventor.NameValueMap Context)
        {
            try
            {
                ComponentOccurrence joint;
                AssemblyDocument asmDoc = (AssemblyDocument)
                    MainApplication.ActiveDocument;
                joint = (ComponentOccurrence)MainApplication.CommandManager.Pick
                    (SelectionFilterEnum.kAssemblyLeafOccurrenceFilter, "Select an assembly to add");
                foreach (JointData j in jointList)
                {
                    if (joint.Equals(j.jointOfType.AffectedOccurrenceOne) || joint.Equals(j.jointOfType.AffectedOccurrenceTwo))
                    {
                        foreach (BrowserNode node in oPane.TopNode.BrowserNodes)
                        {
                            if (j.same(node.BrowserNodeDefinition))
                            {
                                node.DoSelect();
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
        }
        //starts the exporter
        public void StartExport_OnExecute(Inventor.NameValueMap Context)
        {
            try
            {
                if (!inExportView)
                {
                    Utilities.CreateDockableWindows(MainApplication);
                    nativeDoc = MainApplication.ActiveDocument;
                    BrowserNodeDefinition def = null;
                    inExportView = true;
                    JointsComboBox.Enabled = true;
                    LimitsComboBox.Enabled = true;
                    ExportRobot.Enabled = true;//set buttons to proper state
                    StartExport.Enabled = false;
                    CancelExport.Enabled = true;
                    EditDrivers.Enabled = true;
                    EditLimits.Enabled = true;
                    SelectJointInsideJoint.Enabled = true;
                    AssemblyDocument asmDoc = (AssemblyDocument)MainApplication.ActiveDocument;
                    BrowserNodeDefinition oDef;
                    jointList = new ArrayList();
                    joints = new ArrayList();
                    i = 1;
                    oDoc = MainApplication.ActiveDocument;
                    oPanes = oDoc.BrowserPanes;
                    ObjectCollection oOccurrenceNodes;
                    oOccurrenceNodes = MainApplication.TransientObjects.CreateObjectCollection();
                    JointsComboBox.Enabled = false;
                    LimitsComboBox.Enabled = false;
                    EditDrivers.Enabled = false;
                    EditLimits.Enabled = false;
                    envMan = ((AssemblyDocument)MainApplication.ActiveDocument).EnvironmentManager;
                    try
                    {// if no browser pane previously created then create a new one
                        ClientNodeResources oRscs = oPanes.ClientNodeResources;
                        oRsc = oRscs.Add(m_ClientId, 1, null);
                        oDef = (BrowserNodeDefinition)oPanes.CreateBrowserNodeDefinition("Joints", 3, null);
                        oPane = oPanes.AddTreeBrowserPane("Select Joints", m_ClientId, oDef);
                        oPane.Activate();
                    }
                    catch (Exception)
                    {
                        bool found = false;
                        foreach (BrowserPane pane in oPanes)
                        {
                            if (pane.Name.Equals("Select Joints"))
                            {

                                oPane = pane;
                                foreach (BrowserFolder f in oPane.TopNode.BrowserFolders)
                                {
                                    f.Delete();
                                }
                                foreach (BrowserNode f in oPane.TopNode.BrowserNodes)
                                {
                                    f.Delete();
                                }
                                oPane.Visible = true;
                                oPane.Activate();// is there is already a pane then use that
                                found = true;
                            }
                        }
                        if (!found)
                        {
                            oDef = (BrowserNodeDefinition)oPanes.CreateBrowserNodeDefinition("Joints", 3, oRsc);// if the pane was created but the node wasnt then init a node 
                            oPane = oPanes.AddTreeBrowserPane("Select Joints", m_ClientId, oDef);
                        }
                    }

                    oSet = oDoc.CreateHighlightSet();
                    oSet.Color = MainApplication.TransientObjects.CreateColor(125, 0, 255);

                    ReadSave();
                    foreach (ComponentOccurrence c in ((AssemblyDocument)MainApplication.ActiveDocument).ComponentDefinition.Occurrences)
                    {
                        foreach (AssemblyJoint j in c.Joints)
                        {// look at all joints inside of the main doc
                            bool found = false;
                            foreach (JointData d in jointList)
                            {// looks at all joints in the joint data to check for duplicates
                                if (d.equals(j))
                                {
                                    found = true;
                                }
                            }
                            if (!found)
                            {// if there isn't a duplicate then add part to browser folder
                                int th = Rand.Next();
                                ClientNodeResources oNodeRescs;
                                ClientNodeResource oRes = null;
                                oNodeRescs = oPanes.ClientNodeResources;
                                try
                                {
                                    oRes = oNodeRescs.Add("MYID", 1, null);
                                }
                                catch (Exception)
                                {
                                    oRes = oPanes.ClientNodeResources.ItemById("MYID", 1);
                                }
                                def = (BrowserNodeDefinition)oPanes.CreateBrowserNodeDefinition("Joint " + jointNumber.ToString(), th, oRes);
                                oPane.TopNode.AddChild(def);
                                joints.Add(j.AffectedOccurrenceOne);
                                joints.Add(j.AffectedOccurrenceTwo);
                                i++;
                                AssemblyJoint = new JointData(j, "Joint " + jointNumber.ToString());
                                try
                                {
                                    try
                                    {
                                        AssemblyJoint.LowerLim = ((ModelParameter)j.Definition.AngularPositionStartLimit).ModelValue;
                                        AssemblyJoint.UpperLim = ((ModelParameter)j.Definition.AngularPositionEndLimit).ModelValue;
                                        AssemblyJoint.HasLimits = true;
                                    }
                                    catch (Exception)
                                    {
                                        AssemblyJoint.LowerLim = ((ModelParameter)j.Definition.LinearPositionStartLimit).ModelValue;
                                        AssemblyJoint.UpperLim = ((ModelParameter)j.Definition.LinearPositionEndLimit).ModelValue;
                                        AssemblyJoint.HasLimits = true;
                                    }
                                }
                                catch (Exception)
                                {

                                }
                                jointNumber++;
                                jointList.Add(AssemblyJoint);// add new joint data to the array
                            }
                        }
                        if (c.SubOccurrences.Count > 0)
                        {// if there are parts/ assemblies inside the assembly then look at it for joints
                            foreach (ComponentOccurrence v in c.SubOccurrences)
                            {
                                // FindSubOccurences(v);
                            }
                        }
                    }
                    Boolean contains = false;
                    foreach (ComponentOccurrence c in asmDoc.ComponentDefinition.Occurrences)
                    {// looks at all parts/ assemblies in the main assembly
                        contains = false;
                        foreach (ComponentOccurrence j in joints)
                        {
                            if ((j.Equals(c)))
                            {// checks is the part/ assembly is in a joint
                                contains = true;
                            }
                        }
                        if (!contains)
                        {// if the assembly/ part isn't part of a joint then hide it
                            c.Enabled = false;
                        }
                    }
                    TimerWatch();
                }
                else
                {
                    MessageBox.Show("Please close out of the robot exporter in the other assembly");
                }
            }
            catch (Exception e)
            {

                MessageBox.Show(e.ToString());
            }
        }
        public void AddParallelEnvironment()
        {
            try
            {

                #region Image Loading
                stdole.IPictureDisp startExporterIconSmall = PictureDispConverter.ToIPictureDisp(new Bitmap(BxDRobotExporter.Resource.StartRobotExporter16));
                stdole.IPictureDisp startExporterIconLarge = PictureDispConverter.ToIPictureDisp(new Bitmap(BxDRobotExporter.Resource.StartRobotExporter32));

                stdole.IPictureDisp exportRobotIconSmall = PictureDispConverter.ToIPictureDisp(new Bitmap(BxDRobotExporter.Resource.ExportRobot16));
                stdole.IPictureDisp exportRobotIconLarge = PictureDispConverter.ToIPictureDisp(new Bitmap(BxDRobotExporter.Resource.ExportRobot32));

                stdole.IPictureDisp SelectJointInsideJointIconSmall = PictureDispConverter.ToIPictureDisp(new Bitmap(BxDRobotExporter.Resource.SelectJointInsideJoint16));
                stdole.IPictureDisp SelectJointInsideJointIconLarge = PictureDispConverter.ToIPictureDisp(new Bitmap(BxDRobotExporter.Resource.SelectJointInsideJoint32));

                stdole.IPictureDisp EditDriversIconSmall = PictureDispConverter.ToIPictureDisp(new Bitmap(BxDRobotExporter.Resource.EditDrivers16));
                stdole.IPictureDisp EditDriversIconLarge = PictureDispConverter.ToIPictureDisp(new Bitmap(BxDRobotExporter.Resource.EditDrivers32));

                stdole.IPictureDisp EditLimitsIconSmall = PictureDispConverter.ToIPictureDisp(new Bitmap(BxDRobotExporter.Resource.EditLimits16));
                stdole.IPictureDisp EditLimitsIconLarge = PictureDispConverter.ToIPictureDisp(new Bitmap(BxDRobotExporter.Resource.EditLimits32));

                stdole.IPictureDisp TTDriverDropdown = PictureDispConverter.ToIPictureDisp(new Bitmap(BxDRobotExporter.Resource.TTDriverDropdown));
                stdole.IPictureDisp TTEditDrivers = PictureDispConverter.ToIPictureDisp(new Bitmap(BxDRobotExporter.Resource.TTEditDrivers));
                stdole.IPictureDisp TTLimitsDropdown = PictureDispConverter.ToIPictureDisp(new Bitmap(BxDRobotExporter.Resource.TTLimitsDropdown));
                stdole.IPictureDisp TTEditLimits = PictureDispConverter.ToIPictureDisp(new Bitmap(BxDRobotExporter.Resource.TTEditLimits));
                stdole.IPictureDisp TTExportRobot = PictureDispConverter.ToIPictureDisp(new Bitmap(BxDRobotExporter.Resource.TTExportRobot));
                #endregion

                // Get the Environments collection
                Environments oEnvironments = MainApplication.UserInterfaceManager.Environments;

                // Create a new environment
                ExporterEnv = oEnvironments.Add("Robot Exporter", "BxD:RobotExporter:Environment", null, startExporterIconSmall, startExporterIconLarge);

                #region Making Controls
                // Get the ribbon associated with the assembly environment
                Ribbon oAssemblyRibbon = MainApplication.UserInterfaceManager.Ribbons["Assembly"];

                // Create contextual tabs and panels within them
                RibbonTab oContextualTabOne = oAssemblyRibbon.RibbonTabs.Add("Robot Exporter", "BxD:RobotExporter:RibbonTab", "ClientId123", "", false, true);

                ControlDefinitions controlDefs = MainApplication.CommandManager.ControlDefinitions;

                // Get the assembly ribbon.
                PartRibbon = MainApplication.UserInterfaceManager.Ribbons["Assembly"];

                PartPanel = oContextualTabOne.RibbonPanels.Add("Joints", "BxD:RobotExporter:Joints", "{55e5c0be-2fa4-4c95-a1f6-4782ea7a3258}");
                PartPanel2 = oContextualTabOne.RibbonPanels.Add("Limits", "BxD:RobotExporter:Limits", "{55e5c0be-2fa4-4c95-a1f6-4782ea7a3258}");
                PartPanel3 = oContextualTabOne.RibbonPanels.Add("Exporter Control", "BxD:RobotExporter:ExporterControl", "{55e5c0be-2fa4-4c95-a1f6-4782ea7a3258}", "BxD:RobotExporter:Limits");
                ModelControls = oContextualTabOne.RibbonPanels.Add("Model Controls", "BxD:RobotExporter:ModelControls", "{55e5c0be-2fa4-4c95-a1f6-4782ea7a3258}");
                PartPanel.Reposition("BxD:RobotExporter:ModelControls", false);
                PartPanel2.Reposition("BxD:RobotExporter:Joints", false);
                PartPanel3.Reposition("BxD:RobotExporter:Limits", false);

                EditDrivers = controlDefs.AddButtonDefinition("Edit Drivers", "BxD:RobotExporter:EditDrivers", CommandTypesEnum.kNonShapeEditCmdType, m_ClientId, null, null, EditDriversIconSmall, EditDriversIconLarge);
                EditDrivers.OnExecute += new ButtonDefinitionSink_OnExecuteEventHandler(EditDrivers_OnExecute);
                ToolTip(EditDrivers, "Edit properties of a joint.",
                    "Select a joint so that it is highlighted blue, and then click \"Edit Drivers.\" Reconfigure joint properties including port number and type, joint friction, wheel type and gear ratio." +
                    " \"Joint friction\" is the friction between the two parts that make up joint, with 0 as none and 100 as extreme.",
                    TTEditDrivers, "Edit Driver");

                EditLimits = controlDefs.AddButtonDefinition("Edit Limits", "BxD:RobotExporter:editLimits", CommandTypesEnum.kNonShapeEditCmdType, m_ClientId, null, null, EditLimitsIconSmall, EditLimitsIconLarge);
                EditLimits.OnExecute += new ButtonDefinitionSink_OnExecuteEventHandler(EditLimits_OnExecute);
                ToolTip(EditLimits, "Edit rotational limits of a joint.",
                    "If a joint has limits, reassigns the lower and upper rotation of a joint in radians.",
                    TTEditLimits, "Edit Limits");

                SelectJointInsideJoint = controlDefs.AddButtonDefinition("Select Joint \n Inside a Subassembly", "BxD:RobotExporter:SelectJointInsideaJoint", CommandTypesEnum.kNonShapeEditCmdType, m_ClientId, null, null, SelectJointInsideJointIconSmall, SelectJointInsideJointIconLarge);
                SelectJointInsideJoint.OnExecute += new ButtonDefinitionSink_OnExecuteEventHandler(SelectJointInsideJoint_OnExecute);

                StartExport = controlDefs.AddButtonDefinition("Start Exporter", "BxD:RobotExporter:StartExporter", CommandTypesEnum.kNonShapeEditCmdType, m_ClientId, null, null);
                StartExport.OnExecute += new ButtonDefinitionSink_OnExecuteEventHandler(StartExport_OnExecute);

                ExportRobot = controlDefs.AddButtonDefinition("Export Robot", "BxD:RobotExporter:ExportRobot", CommandTypesEnum.kNonShapeEditCmdType, m_ClientId, null, null, exportRobotIconSmall, exportRobotIconLarge);
                ExportRobot.OnExecute += new ButtonDefinitionSink_OnExecuteEventHandler(ExportRobot_OnExecute);
                ToolTip(ExportRobot, "Export the robot for use in the Synthesis Simulation.",
                    "After assigning all joints, joint properties and limits, export the robot. The robot will be saved to Documents/Synthesis/Robots and can be accessed through Synthesis.",
                    TTExportRobot, "Export the robot");

                CancelExport = controlDefs.AddButtonDefinition("Cancel Export", "BxD:RobotExporter:CancelExport", CommandTypesEnum.kNonShapeEditCmdType, m_ClientId, null, null);
                CancelExport.OnExecute += new ButtonDefinitionSink_OnExecuteEventHandler(CancelExport_OnExecute);

                Test = controlDefs.AddButtonDefinition("test", "BxD:RobotExporter:test", CommandTypesEnum.kNonShapeEditCmdType, m_ClientId, null, null);
                Test.OnExecute += new ButtonDefinitionSink_OnExecuteEventHandler(Test_OnExecute);

                JointsComboBox = MainApplication.CommandManager.ControlDefinitions.AddComboBoxDefinition("Driver", "Autodesk:SimpleAddIn:Driver", CommandTypesEnum.kShapeEditCmdType, 100, AddInCLSIDString, "Driver", "Driver", Type.Missing, Type.Missing, ButtonDisplayEnum.kDisplayTextInLearningMode);
                ToolTip(JointsComboBox, "Assign a driver type and properties to a joint.",
                   @"Select a joint so that it is highlighted blue. Then, select the driver type from this dropdown menu. Configure joint properties including port number and type, joint friction, wheel type and gear ratio. “Joint friction” is the friction between the two parts that make up joint, with 0 as none and 100 as extreme.

If you do not have any joints, create some by navigating to Assemble> Joint. A joint is any component that moves, such as a wheel or arm.",
                    TTDriverDropdown, "Select Driver");

                LimitsComboBox = MainApplication.CommandManager.ControlDefinitions.AddComboBoxDefinition("Has Limits", "Autodesk:SimpleAddIn:HasLimits", CommandTypesEnum.kShapeEditCmdType, 100, AddInCLSIDString, "Has Limits", "Has Limits", Type.Missing, Type.Missing, ButtonDisplayEnum.kDisplayTextInLearningMode);
                ToolTip(LimitsComboBox, "Select whether a joint has a limit",
                    @"Joints that can freely rotate, such as wheels, have no limits. Joints that cannot rotate freely, such as an arm, have limits.
 
Select a joint so that it is highlighted blue. Then, select whether it has limits or no limits from this dropdown list. If there are limits, assign the lower and upper rotational limits in radians.",
                    TTLimitsDropdown, "Select Limit");
                #endregion


                #region Cleaning Up
                //add some initial items to the comboboxes
                JointsComboBox.AddItem("No Driver", 0);
                JointsComboBox.AddItem("Motor", 0);
                JointsComboBox.AddItem("Servo", 0);
                JointsComboBox.AddItem("Bumper Pneumatic", 0);
                JointsComboBox.AddItem("Relay Pneumatic", 0);
                JointsComboBox.AddItem("Worm Screw", 0);
                JointsComboBox.AddItem("Dual Motor", 0);
                JointsComboBox.ListIndex = 1;
                JointsComboBox.ToolTipText = JointsComboBox.Text;
                JointsComboBox.DescriptionText = "Slot width: " + JointsComboBox.Text;

                JointsComboBox_OnSelectEventDelegate = new ComboBoxDefinitionSink_OnSelectEventHandler(JointsComboBox_OnSelect);
                JointsComboBox.OnSelect += JointsComboBox_OnSelectEventDelegate;
                PartPanel.CommandControls.AddComboBox(JointsComboBox);

                LimitsComboBox.AddItem("No Limits", 0);
                LimitsComboBox.AddItem("Limits", 0);
                LimitsComboBox.ListIndex = 1;
                LimitsComboBox.ToolTipText = JointsComboBox.Text;
                LimitsComboBox.DescriptionText = "Slot width: " + JointsComboBox.Text;

                PartPanel.CommandControls.AddButton(EditDrivers, true, true);
                LimitsComboBox_OnSelectEventDelegate = new ComboBoxDefinitionSink_OnSelectEventHandler(LimitsComboBox_OnSelect);
                LimitsComboBox.OnSelect += LimitsComboBox_OnSelectEventDelegate;
                PartPanel2.CommandControls.AddComboBox(LimitsComboBox);
                PartPanel2.CommandControls.AddButton(EditLimits, true, true);
                PartPanel3.CommandControls.AddButton(ExportRobot, true, true);
                // modelControls.CommandControls.AddButton(selectJointInsideJoint, true, true);

                JointsComboBox.Enabled = false;
                LimitsComboBox.Enabled = false;
                EditDrivers.Enabled = false;
                EditLimits.Enabled = false;
                ExportRobot.Enabled = false;
                StartExport.Enabled = true;
                CancelExport.Enabled = false;
                SelectJointInsideJoint.Enabled = false;

                UserInterfaceEvents UIEvents = MainApplication.UserInterfaceManager.UserInterfaceEvents;

                enviroment_OnChangeEventDelegate = new UserInterfaceEventsSink_OnEnvironmentChangeEventHandler(OnEnvironmentChange);
                UIEvents.OnEnvironmentChange += enviroment_OnChangeEventDelegate;

                // Make the "SomeAnalysis" tab default for the environment
                ExporterEnv.DefaultRibbonTab = "BxD:RobotExporter:RibbonTab";

                // Get the collection of parallel environments and add the new environment
                EnvironmentList oParEnvs = MainApplication.UserInterfaceManager.ParallelEnvironments;

                oParEnvs.Add(ExporterEnv);

                // Make the new parallel environment available only within the assembly environment
                // A ControlDefinition is automatically created when an environment is added to the
                // parallel environments list. The internal name of the definition is the same as
                // the internal name of the environment.
                ControlDefinition oParallelEnvButton = MainApplication.CommandManager.ControlDefinitions["BxD:RobotExporter:Environment"];

                ExporterEnv.DisabledCommandList.Add(oParallelEnvButton); 
                #endregion
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        //Sets the tool tips for a button
        public void ToolTip(ButtonDefinition button, String description, String expandedDescription, stdole.IPictureDisp picture, String title)
        {
            button.ProgressiveToolTip.Description = description;
            button.ProgressiveToolTip.ExpandedDescription = expandedDescription;
            button.ProgressiveToolTip.Image = picture;
            button.ProgressiveToolTip.IsProgressive = true;
            button.ProgressiveToolTip.Title = title;
        }

        //Sets the tool tips without the picture
        public void ToolTip(ButtonDefinition button, String description, String expandedDescription, String title)
        {
            button.ProgressiveToolTip.Description = description;
            button.ProgressiveToolTip.ExpandedDescription = expandedDescription;
            button.ProgressiveToolTip.IsProgressive = true;
            button.ProgressiveToolTip.Title = title;
        }

        //Sets the tool tips for a ComboBox
        public void ToolTip(ComboBoxDefinition comboBox, String description, String expandedDescription, stdole.IPictureDisp picture, String title)
        {
            comboBox.ProgressiveToolTip.Description = description;
            comboBox.ProgressiveToolTip.ExpandedDescription = expandedDescription;
            comboBox.ProgressiveToolTip.Image = picture;
            comboBox.ProgressiveToolTip.IsProgressive = true;
            comboBox.ProgressiveToolTip.Title = title;
        }

        //reacts to a select event
        private void UIEvents_OnSelect(ObjectsEnumerator JustSelectedEntities, ref ObjectCollection MoreSelectedEntities, SelectionDeviceEnum SelectionDevice, Inventor.Point ModelPosition, Point2d ViewPosition, Inventor.View View)
        {
            try
            {
                if (inExportView)
                {
                    if (JustSelectedEntities.Count == 1)
                    {
                        if (SelectionDevice == SelectionDeviceEnum.kGraphicsWindowSelection && inExportView)
                        {// if the selection is from the graphical interface and the exporter is active
                            foreach (Object sel in JustSelectedEntities)
                            {//looks at all things selected
                                if (sel is ComponentOccurrence)
                                {// react only if sel is a part/ assembly
                                    foreach (JointData joint in jointList)
                                    {// looks at all the groups of parts
                                        if (((ComponentOccurrence)sel).Equals(joint.jointOfType.AffectedOccurrenceOne)
                                                || ((ComponentOccurrence)sel).Equals(joint.jointOfType.AffectedOccurrenceTwo))
                                        {// if the occurence is contained by anyof the groups then react
                                            foreach (BrowserNode n in oPane.TopNode.BrowserNodes)
                                            {// looks at all the browser nodes in the top node
                                                if (n.BrowserNodeDefinition.Label.Equals(joint.Name))
                                                {// if the browsernode is the same as the types node then react
                                                    n.DoSelect();// select the proper node
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (SelectionDevice == SelectionDeviceEnum.kBrowserSelection && inExportView)
                        {// if the selection is from the browser and the exporter is active, cool feature is that browsernode.DoSelect() calls this so I do all the reactions in here
                            foreach (Object sel in JustSelectedEntities)
                            {//looks at all things selected
                                if (sel is BrowserNodeDefinition)
                                {// react only if sel is a browsernodedef
                                    foreach (JointData joint in jointList)
                                    {// looks at all the groups of parts
                                        if (joint.same(((BrowserNodeDefinition)sel)))
                                        {// if the browsernode is the same as a the joint's node
                                            MainApplication.ActiveDocument.SelectSet.Clear();
                                            oSet.Clear();// clear the highlight set to add a new group to the set
                                            selectedJoints.Clear();
                                            oSet.AddItem(joint.jointOfType.AffectedOccurrenceOne);
                                            oSet.AddItem(joint.jointOfType.AffectedOccurrenceTwo);
                                            selectedJoints.Add(joint);
                                            EditDrivers.Enabled = true;
                                            JointsComboBox.Enabled = true;
                                            LimitsComboBox.Enabled = true;
                                            EditLimits.Enabled = true;
                                            if (((JointData)selectedJoints[0]).jointOfType.Definition.JointType == AssemblyJointTypeEnum.kCylindricalJointType ||
                                                    ((JointData)selectedJoints[0]).jointOfType.Definition.JointType == AssemblyJointTypeEnum.kSlideJointType)
                                            {// if the assembly joint is linear
                                                JointTypeLinear();
                                            }
                                            else
                                            {// set the combo box choices to rotating
                                                JointTypeRotating();
                                            }
                                            SwitchSelectedJoint(((JointData)selectedJoints[0]).Driver);// set selected joint type in the combo box to the correct one
                                            SwitchSelectedLimit(((JointData)selectedJoints[0]).HasLimits);// set selected limit choice in the combo box to the correct one
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        ObjectCollection Obj = MainApplication.TransientObjects.CreateObjectCollection();
                        oSet.Clear();
                        if (inExportView)
                        {
                            foreach (Object o in JustSelectedEntities)
                            {
                                if (o is BrowserNodeDefinition)
                                {
                                    foreach (JointData joint in jointList)
                                    {
                                        if (joint.same(((BrowserNodeDefinition)o)))
                                        {
                                            if (selectedJoints.Count > 0)
                                            {
                                                if (!selectedJoints.Contains(joint))
                                                {
                                                    if (((JointData)selectedJoints[0]).Rotating == joint.Rotating)
                                                    {
                                                        Obj.Add((BrowserNodeDefinition)o);
                                                        selectedJoints.Add(joint);
                                                        oSet.AddItem(joint.jointOfType.AffectedOccurrenceOne);
                                                        oSet.AddItem(joint.jointOfType.AffectedOccurrenceTwo);
                                                        EditDrivers.Enabled = true;
                                                        JointsComboBox.Enabled = true;
                                                        LimitsComboBox.Enabled = true;
                                                        EditLimits.Enabled = true;
                                                    }
                                                    else
                                                    {
                                                        MainApplication.ActiveDocument.SelectSet.Remove(((BrowserNodeDefinition)o));
                                                        MessageBox.Show("Error, the selected joint type is incorrect for the rest of the selected joints");
                                                    }
                                                }
                                                else
                                                {
                                                    Obj.Add((BrowserNodeDefinition)o);
                                                    oSet.AddItem(joint.jointOfType.AffectedOccurrenceOne);
                                                    oSet.AddItem(joint.jointOfType.AffectedOccurrenceTwo);
                                                }
                                                if (((JointData)selectedJoints[0]).jointOfType.Definition.JointType == AssemblyJointTypeEnum.kCylindricalJointType ||
                                                                ((JointData)selectedJoints[0]).jointOfType.Definition.JointType == AssemblyJointTypeEnum.kSlideJointType)
                                                {// if the assembly joint is linear
                                                    JointTypeLinear();
                                                }
                                                else
                                                {// set the combo box choices to rotating
                                                    JointTypeRotating();
                                                }
                                                SwitchSelectedJoint(((JointData)selectedJoints[0]).Driver);// set selected joint type in the combo box to the correct one
                                                SwitchSelectedLimit(((JointData)selectedJoints[0]).HasLimits);// set selected limit choice in the combo box to the correct one
                                            }
                                            else
                                            {
                                                selectedJoints.Add(joint);
                                                Obj.Add((BrowserNodeDefinition)o);
                                                oSet.AddItem(joint.jointOfType.AffectedOccurrenceOne);
                                                oSet.AddItem(joint.jointOfType.AffectedOccurrenceTwo);
                                                EditDrivers.Enabled = true;
                                                JointsComboBox.Enabled = true;
                                                LimitsComboBox.Enabled = true;
                                                EditLimits.Enabled = true;
                                                if (((JointData)selectedJoints[0]).jointOfType.Definition.JointType == AssemblyJointTypeEnum.kCylindricalJointType ||
                                                                ((JointData)selectedJoints[0]).jointOfType.Definition.JointType == AssemblyJointTypeEnum.kSlideJointType)
                                                {// if the assembly joint is linear
                                                    JointTypeLinear();
                                                }
                                                else
                                                {// set the combo box choices to rotating
                                                    JointTypeRotating();
                                                }
                                                SwitchSelectedJoint(((JointData)selectedJoints[0]).Driver);// set selected joint type in the combo box to the correct one
                                                SwitchSelectedLimit(((JointData)selectedJoints[0]).HasLimits);// set selected limit choice in the combo box to the correct one
                                            }
                                        }
                                    }
                                }
                                else if (o is ComponentOccurrence)
                                {
                                    foreach (JointData joint in jointList)
                                    {
                                        if ((o).Equals(joint.jointOfType.AffectedOccurrenceOne) || o.Equals(joint.jointOfType.AffectedOccurrenceTwo))
                                        {
                                            foreach (BrowserNode node in oPane.TopNode.BrowserNodes)
                                            {
                                                if (joint.same(node.BrowserNodeDefinition))
                                                {
                                                    if (selectedJoints.Count > 0)
                                                    {
                                                        if (!selectedJoints.Contains(joint))
                                                        {
                                                            if (((JointData)selectedJoints[0]).Rotating == joint.Rotating)
                                                            {
                                                                Obj.Add(node.BrowserNodeDefinition);
                                                                selectedJoints.Add(joint);
                                                                oSet.AddItem(joint.jointOfType.AffectedOccurrenceOne);
                                                                oSet.AddItem(joint.jointOfType.AffectedOccurrenceTwo);
                                                                EditDrivers.Enabled = true;
                                                                JointsComboBox.Enabled = true;
                                                                LimitsComboBox.Enabled = true;
                                                                EditLimits.Enabled = true;
                                                            }
                                                            else
                                                            {
                                                                MainApplication.ActiveDocument.SelectSet.Remove(((ComponentOccurrence)o));
                                                                MessageBox.Show("Error, the selected joint type is incorrect for the rest of the selected joints");

                                                            }
                                                        }
                                                        else
                                                        {
                                                            Obj.Add(node.BrowserNodeDefinition);
                                                            oSet.AddItem(joint.jointOfType.AffectedOccurrenceOne);
                                                            oSet.AddItem(joint.jointOfType.AffectedOccurrenceTwo);
                                                        }
                                                        if (((JointData)selectedJoints[0]).jointOfType.Definition.JointType == AssemblyJointTypeEnum.kCylindricalJointType ||
                                                                ((JointData)selectedJoints[0]).jointOfType.Definition.JointType == AssemblyJointTypeEnum.kSlideJointType)
                                                        {// if the assembly joint is linear
                                                            JointTypeLinear();
                                                        }
                                                        else
                                                        {// set the combo box choices to rotating
                                                            JointTypeRotating();
                                                        }
                                                        SwitchSelectedJoint(((JointData)selectedJoints[0]).Driver);// set selected joint type in the combo box to the correct one
                                                        SwitchSelectedLimit(((JointData)selectedJoints[0]).HasLimits);// set selected limit choice in the combo box to the correct one
                                                    }
                                                    else
                                                    {
                                                        selectedJoints.Add(joint);
                                                        Obj.Add(node.BrowserNodeDefinition);
                                                        oSet.AddItem(joint.jointOfType.AffectedOccurrenceOne);
                                                        oSet.AddItem(joint.jointOfType.AffectedOccurrenceTwo);
                                                        EditDrivers.Enabled = true;
                                                        JointsComboBox.Enabled = true;
                                                        LimitsComboBox.Enabled = true;
                                                        EditLimits.Enabled = true;
                                                        if (((JointData)selectedJoints[0]).jointOfType.Definition.JointType == AssemblyJointTypeEnum.kCylindricalJointType ||
                                                                ((JointData)selectedJoints[0]).jointOfType.Definition.JointType == AssemblyJointTypeEnum.kSlideJointType)
                                                        {// if the assembly joint is linear
                                                            JointTypeLinear();
                                                        }
                                                        else
                                                        {// set the combo box choices to rotating
                                                            JointTypeRotating();
                                                        }
                                                        SwitchSelectedJoint(((JointData)selectedJoints[0]).Driver);// set selected joint type in the combo box to the correct one
                                                        SwitchSelectedLimit(((JointData)selectedJoints[0]).HasLimits);// set selected limit choice in the combo box to the correct one
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        MainApplication.ActiveDocument.SelectSet.SelectMultiple(Obj);
                        //m_inventorApplication.ActiveDocument.SelectSet.SelectMultiple(Obj);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        // switch the selected 
        public void SwitchSelectedLimit(bool HasLimits)
        {
            DoWork = false;// make sure the limit combo box reactor doesn't react
            if (HasLimits)
            {  // if the joint data has limits then change the combo box selection
                LimitsComboBox.ListIndex = 2;
            }
            else
            { // if the joint data doesn't have limits then change the combo box selection
                LimitsComboBox.ListIndex = 1;
            }
            DoWork = true;// reanable the limits combo box reactor
        }
        // switch selection in combo box to the correct choice
        private void SwitchSelectedJoint(DriveTypes driver)
        {//  gets passed the DriveType of the selected joints
            DoWork = false;// disable the combo box selection reactor
            if (Rotating)
            {// if rotating read from Driver and select proper choice
                if (driver == DriveTypes.Motor)
                {
                    JointsComboBox.ListIndex = 2;
                }
                else if (driver == DriveTypes.Servo)
                {
                    JointsComboBox.ListIndex = 3;
                }
                else if (driver == DriveTypes.BumperPnuematic)
                {
                    JointsComboBox.ListIndex = 4;
                }
                else if (driver == DriveTypes.RelayPneumatic)
                {
                    JointsComboBox.ListIndex = 5;
                }
                else if (driver == DriveTypes.WormScrew)
                {
                    JointsComboBox.ListIndex = 6;
                }
                else if (driver == DriveTypes.DualMotor)
                {
                    JointsComboBox.ListIndex = 7;
                }
                else
                {
                    JointsComboBox.ListIndex = 1;
                }
            }
            else
            {// if linear read from Driver and select proper choice
                if (driver == DriveTypes.Elevator)
                {
                    JointsComboBox.ListIndex = 2;
                }
                else if (driver == DriveTypes.BumperPnuematic)
                {
                    JointsComboBox.ListIndex = 3;
                }
                else if (driver == DriveTypes.RelayPneumatic)
                {
                    JointsComboBox.ListIndex = 4;
                }
                else if (driver == DriveTypes.WormScrew)
                {
                    JointsComboBox.ListIndex = 5;
                }
                else
                {
                    JointsComboBox.ListIndex = 1;
                }
            }
            DoWork = true;// reenable the combo box selection reactor
        }
        // if the joint is rotating then set the proper combo box choices
        private void JointTypeRotating()
        {
            try
            {
                DoWork = false; // tells the reactor method to ignore the selection changes
                Rotating = true; // sets the joint of joint to rotating
                JointsComboBox.Clear();
                JointsComboBox.AddItem("No Driver", 0);
                JointsComboBox.AddItem("Motor", 0);
                JointsComboBox.AddItem("Servo", 0);
                JointsComboBox.AddItem("Bumper Pneumatic", 0);
                JointsComboBox.AddItem("Relay Pneumatic", 0);
                JointsComboBox.AddItem("Worm Screw", 0);
                JointsComboBox.AddItem("Dual Motor", 0);
                JointsComboBox.ListIndex = 1;
                JointsComboBox.ToolTipText = JointsComboBox.Text;
                JointsComboBox.DescriptionText = "Slot width: " + JointsComboBox.Text;
                DoWork = true;// reenables the combo box reactor method
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        // if the joint is linear then set the proper combo box choices
        private void JointTypeLinear()
        {
            DoWork = false; // tells the reactor method to ignore the selection changes
            Rotating = false; // sets the joint of joint to not rotating
            JointsComboBox.Clear();
            JointsComboBox.AddItem("No Driver", 0);
            JointsComboBox.AddItem("Elevator", 0);
            JointsComboBox.AddItem("Bumper Pneumatic", 0);
            JointsComboBox.AddItem("Relay Pneumatic", 0);
            JointsComboBox.AddItem("Worm Screw", 0);
            JointsComboBox.ListIndex = 1;
            JointsComboBox.ToolTipText = JointsComboBox.Text;
            JointsComboBox.DescriptionText = "Slot width: " + JointsComboBox.Text;
            DoWork = true; // reenables the combo box reactor method
        }
        //BrowserNodeDefinition def;
        private void ReadSave()
        {
            BrowserNodeDefinition def;
            object other;
            object context;
            object resultObj;
            byte[] refObj;
            try
            {
                PropertySets sets = MainApplication.ActiveDocument.PropertySets;
                other = null;
                context = null;
                JointData j;
                int k = 0;
                resultObj = null;
                foreach (Inventor.PropertySet p in sets)
                {
                    if (p.DisplayName.Equals("Number of Joints"))
                    {
                        jointNumber = (int)p.ItemByPropId[2].Value;
                        k = (int)p.ItemByPropId[2].Value;
                    }
                }
                for (int n = 0; n <= jointNumber; n++)
                {
                    foreach (Inventor.PropertySet p in sets)
                    {
                        if (p.Name.Equals("Joint " + n.ToString()))
                        {
                            resultObj = null;
                            context = null;
                            other = null;
                            refObj = new byte[0];
                            MainApplication.ActiveDocument.ReferenceKeyManager.
                                    StringToKey(((String)p.ItemByPropId[2].Value), ref refObj);
                            if (MainApplication.ActiveDocument.ReferenceKeyManager.CanBindKeyToObject(refObj, 0, out resultObj, out context))
                            {
                                object obje = MainApplication.ActiveDocument.ReferenceKeyManager.
                                        BindKeyToObject(refObj, 0, out other);
                                int th = Rand.Next();
                                ClientNodeResources oNodeRescs;
                                ClientNodeResource oRes = null;
                                oNodeRescs = oPanes.ClientNodeResources;
                                try
                                {
                                    oRes = oNodeRescs.Add("MYID", 1, null);
                                }
                                catch (Exception)
                                {
                                    oRes = oPanes.ClientNodeResources.ItemById("MYID", 1);
                                }
                                try
                                {
                                    def = (BrowserNodeDefinition)oPanes.CreateBrowserNodeDefinition("Joint " + n.ToString(), th, oRes);
                                    // ((BrowserFolder)def).AllowRename = false;
                                    oPane.TopNode.AddChild(def);
                                    joints.Add(((AssemblyJoint)obje).AffectedOccurrenceOne);
                                    joints.Add(((AssemblyJoint)obje).AffectedOccurrenceTwo);
                                    k++;
                                    j = new JointData(((AssemblyJoint)obje), ((String)p.ItemByPropId[27].Value));
                                    jointList.Add(j);
                                    j.RefKey = (String)p.ItemByPropId[2].Value;
                                    j.Driver = (DriveTypes)p.ItemByPropId[3].Value;
                                    j.Wheel = (WheelType)p.ItemByPropId[4].Value;
                                    j.Friction = (FrictionLevel)p.ItemByPropId[5].Value;
                                    j.Diameter = (InternalDiameter)p.ItemByPropId[6].Value;
                                    j.Pressure = (Pressure)p.ItemByPropId[7].Value;
                                    j.Stages = (Stages)p.ItemByPropId[8].Value;
                                    j.PWMport = (double)p.ItemByPropId[9].Value;
                                    j.PWMport2 = (double)p.ItemByPropId[10].Value;
                                    j.CANport = (double)p.ItemByPropId[11].Value;
                                    j.CANport2 = (double)p.ItemByPropId[12].Value;
                                    j.DriveWheel = (bool)p.ItemByPropId[13].Value;
                                    j.PWM = (bool)p.ItemByPropId[14].Value;
                                    j.InputGear = (double)p.ItemByPropId[15].Value;
                                    j.OutputGear = (double)p.ItemByPropId[16].Value;
                                    j.SolenoidPortA = (double)p.ItemByPropId[17].Value;
                                    j.SolenoidPortB = (double)p.ItemByPropId[18].Value;
                                    j.RelayPort = (double)p.ItemByPropId[19].Value;
                                    j.HasBrake = (bool)p.ItemByPropId[20].Value;
                                    j.BrakePortA = (double)p.ItemByPropId[21].Value;
                                    j.BrakePortB = (double)p.ItemByPropId[22].Value;
                                    j.UpperLim = (double)p.ItemByPropId[23].Value;
                                    j.LowerLim = (double)p.ItemByPropId[24].Value;
                                    j.HasLimits = (bool)p.ItemByPropId[25].Value;
                                    j.Rotating = (bool)p.ItemByPropId[26].Value;
                                    j.HasJointFriction = (bool)p.ItemByPropId[28].Value;
                                    j.JointFrictionLevel = (double)p.ItemByPropId[29].Value;
                                }
                                catch (Exception)
                                {

                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void WriteNumJoints()
        {
            PropertySets sets = MainApplication.ActiveDocument.PropertySets;
            Inventor.PropertySet set = null;
            try
            {
                set = sets.Add("Number of Joints");
            }
            catch (Exception)
            {
                foreach (Inventor.PropertySet s in sets)
                {
                    if (s.DisplayName.Equals("Number of Joints"))
                    {
                        set = s;
                    }
                }
            }
            try
            {
                set.Add(jointNumber, "Number of joints", 2);
            }
            catch (Exception)
            {
                set.ItemByPropId[2].Value = jointNumber;
            }
        }

        private void WriteSave(JointData j)
        {
            PropertySets sets = MainApplication.ActiveDocument.PropertySets;
            Inventor.PropertySet set = null;
            try
            {
                set = sets.Add(j.Name);
            }
            catch (Exception)
            {
                foreach (Inventor.PropertySet s in sets)
                {
                    if (s.DisplayName.Equals(j.Name))
                    {
                        set = s;
                    }
                }
            }
            try
            {
                try
                {
                    set.Add(j.RefKey, "RefKey", 2);
                }
                catch (Exception)
                {
                    set.ItemByPropId[2].Value = j.RefKey;
                }
                try
                {
                    set.Add(j.Driver, "Driver", 3);
                }
                catch (Exception)
                {
                    set.ItemByPropId[3].Value = j.Driver;
                }
                try
                {
                    set.Add(j.Wheel, "Wheel", 4);
                }
                catch (Exception)
                {
                    set.ItemByPropId[4].Value = j.Wheel;
                }
                try
                {
                    set.Add(j.Friction, "Friction", 5);
                }
                catch (Exception)
                {
                    set.ItemByPropId[5].Value = j.Friction;
                }
                try
                {
                    set.Add(j.Diameter, "Diameter", 6);
                }
                catch (Exception)
                {
                    set.ItemByPropId[6].Value = j.Diameter;
                }
                try
                {
                    set.Add(j.Pressure, "Pressure", 7);
                }
                catch (Exception)
                {
                    set.ItemByPropId[7].Value = j.Pressure;
                }
                try
                {
                    set.Add(j.Stages, "Stages", 8);
                }
                catch (Exception)
                {
                    set.ItemByPropId[8].Value = j.Stages;
                }
                try
                {
                    set.Add(j.PWMport, "PWMport", 9);
                }
                catch (Exception)
                {
                    set.ItemByPropId[9].Value = j.PWMport;
                }
                try
                {
                    set.Add(j.PWMport2, "PWMport2", 10);
                }
                catch (Exception)
                {
                    set.ItemByPropId[10].Value = j.PWMport2;
                }
                try
                {
                    set.Add(j.CANport, "CANport", 11);
                }
                catch (Exception)
                {
                    set.ItemByPropId[11].Value = j.CANport;
                }
                try
                {
                    set.Add(j.CANport2, "CANport2", 12);
                }
                catch (Exception)
                {
                    set.ItemByPropId[12].Value = j.CANport2;
                }
                try
                {
                    set.Add(j.DriveWheel, "DriveWheel", 13);
                }
                catch (Exception)
                {
                    set.ItemByPropId[13].Value = j.DriveWheel;
                }
                try
                {
                    set.Add(j.PWM, "PWM", 14);
                }
                catch (Exception)
                {
                    set.ItemByPropId[14].Value = j.PWM;
                }
                try
                {
                    set.Add(j.InputGear, "InputGear", 15);
                }
                catch (Exception)
                {
                    set.ItemByPropId[15].Value = j.InputGear;
                }
                try
                {
                    set.Add(j.OutputGear, "OutputGear", 16);
                }
                catch (Exception)
                {
                    set.ItemByPropId[16].Value = j.OutputGear;
                }
                try
                {
                    set.Add(j.SolenoidPortA, "SolenoidPortA", 17);
                }
                catch (Exception)
                {
                    set.ItemByPropId[17].Value = j.SolenoidPortA;
                }
                try
                {
                    set.Add(j.SolenoidPortB, "SolenoidPortB", 18);
                }
                catch (Exception)
                {
                    set.ItemByPropId[18].Value = j.SolenoidPortB;
                }
                try
                {
                    set.Add(j.RelayPort, "RelayPort", 19);
                }
                catch (Exception)
                {
                    set.ItemByPropId[19].Value = j.RelayPort;
                }
                try
                {
                    set.Add(j.HasBrake, "HasBrake", 20);
                }
                catch (Exception)
                {
                    set.ItemByPropId[20].Value = j.HasBrake;
                }
                try
                {
                    set.Add(j.BrakePortA, "BrakePortA", 21);
                }
                catch (Exception)
                {
                    set.ItemByPropId[21].Value = j.BrakePortA;
                }
                try
                {
                    set.Add(j.BrakePortB, "BrakePortB", 22);
                }
                catch (Exception)
                {
                    set.ItemByPropId[22].Value = j.BrakePortB;
                }
                try
                {
                    set.Add(j.UpperLim, "UpperLim", 23);
                }
                catch (Exception)
                {
                    set.ItemByPropId[23].Value = j.UpperLim;
                }
                try
                {
                    set.Add(j.LowerLim, "LowerLim", 24);
                }
                catch (Exception)
                {
                    set.ItemByPropId[24].Value = j.LowerLim;
                }
                try
                {
                    set.Add(j.HasLimits, "HasLimits", 25);
                }
                catch (Exception)
                {
                    set.ItemByPropId[25].Value = j.HasLimits;
                }
                try
                {
                    set.Add(j.Rotating, "Rotating", 26);
                }
                catch (Exception)
                {
                    set.ItemByPropId[26].Value = j.Rotating;
                }
                try
                {
                    set.Add(j.Name, "Name", 27);
                }
                catch (Exception)
                {
                    set.ItemByPropId[27].Value = j.Name;
                }
                try
                {
                    set.Add(j.HasJointFriction, "HasJointFriction", 28);
                }
                catch (Exception)
                {
                    set.ItemByPropId[28].Value = j.HasJointFriction;
                }
                try
                {
                    set.Add(j.JointFrictionLevel, "JointFrictionLevel", 29);
                }
                catch (Exception)
                {
                    set.ItemByPropId[29].Value = j.JointFrictionLevel;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        //test button for doing experimental things
        public void Test_OnExecute(Inventor.NameValueMap Context)
        {
            ComponentOccurrence joint;
            AssemblyDocument asmDoc = (AssemblyDocument)
                MainApplication.ActiveDocument;
            joint = (ComponentOccurrence)MainApplication.CommandManager.Pick
                (SelectionFilterEnum.kAssemblyLeafOccurrenceFilter, "Select an assembly to add");
        }
        // looks at subcomponents for joints
        public void HideInside(ComponentOccurrence c)
        {
            bool contains = false;
            foreach (ComponentOccurrence j in joints)// looks at all parts/ assemblies inside of the joints list
            {

                if ((j.Equals(c)))
                {
                    contains = true;// if the selected part/ assembly is a part of a joint then don't disable it 
                }
            }
            if (!contains)
            {// if the part/ assembly isn't a part of a joint the disable it
                c.Enabled = false;
            }
            if (c.SubOccurrences.Count > 0)
            {
                foreach (ComponentOccurrence v in c.SubOccurrences)
                {
                    HideInside(v);// if the assembly has parts/ assemblies then look at those
                }
            }
        }
        // looks at sub assembly for joints
        public void FindSubOccurences(ComponentOccurrence occ)
        {
            BrowserNodeDefinition def;
            try
            {
                foreach (AssemblyJoint j in occ.Joints)
                {// look at all joints inside of the main doc
                    bool found = false;
                    foreach (JointData d in jointList)
                    {// looks at all joints in the joint data to check for duplicates
                        if (d.equals(j))
                        {
                            found = true;
                        }
                    }
                    if (!found)
                    {// if there isn't a duplicate then add part to browser folder
                        int th = Rand.Next();
                        ClientNodeResources oNodeRescs;
                        ClientNodeResource oRes = null;
                        oNodeRescs = oPanes.ClientNodeResources;
                        try
                        {
                            oRes = oNodeRescs.Add("MYID", 1, null);
                        }
                        catch (Exception)
                        {
                            oRes = oPanes.ClientNodeResources.ItemById("MYID", 1);
                        }
                        def = (BrowserNodeDefinition)oPanes.CreateBrowserNodeDefinition("Joint " + jointNumber.ToString(), th, oRes);
                        oPane.TopNode.AddChild(def);
                        joints.Add(j.AffectedOccurrenceOne);
                        joints.Add(j.AffectedOccurrenceTwo);
                        i++;
                        AssemblyJoint = new JointData(j, "Joint " + jointNumber.ToString());
                        jointNumber++;
                        jointList.Add(AssemblyJoint);// add new joint data to the array
                    }
                }
                if (occ.SubOccurrences.Count > 0)
                {// if there are parts/ assemblies inside the assembly then look at it for joints
                    foreach (ComponentOccurrence v in occ.SubOccurrences)
                    {
                        FindSubOccurences(v);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        // exports the robot
        public void ExportRobot_OnExecute(Inventor.NameValueMap Context)
        {
            Utilities.CreateDockableWindows(MainApplication);
            //if (jointList.Count == 0)
            //{
            //    MessageBox.Show("ERROR: No Joints Defined!");
            //    return;
            //}

            //try
            //{
            //    control.saveFile();// save the file
            //    envMan.SetCurrentEnvironment(envMan.BaseEnvironment);
            //    RobotSaver exporter = new RobotSaver(MainApplication, jointList);
            //    exporter.beginExport();
            //}
            //catch (Exception e)
            //{

            //}
        }
        // cancels the export
        static bool rightDoc;

        private void TimerWatch()
        {
            try
            {
                System.Timers.Timer aTimer = new System.Timers.Timer();
                aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                aTimer.Interval = 250;
                aTimer.AutoReset = true;
                aTimer.Enabled = true;
                rightDoc = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        static bool found;
        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            found = false;
            foreach (BrowserNode node in oPane.TopNode.BrowserNodes)
            {// looks through all the nodes under the top node
                if (node.Selected)
                {// if the node is seleted
                    foreach (JointData t in jointList)
                    {// looks at all the groups in the fieldtypes
                        if (t.same(node.BrowserNodeDefinition))
                        {// if t is part of that browser node
                            found = true;// tell the program it found the 
                        }
                    }
                }
            }
            if (!found)
            {
                foreach (BrowserNode node in oPane.TopNode.BrowserNodes)
                {// looks through all the nodes under the top node
                    if (node.Selected)
                    {// if the node is seleted
                        foreach (JointData t in jointList)
                        {// looks at all the groups in the fieldtypes
                            if (t.same(node.BrowserNodeDefinition))
                            {// if t is part of that browser node
                                found = true;// tell the program it found the 
                            }
                        }
                    }
                }
                if (!found)
                {
                    oSet.Clear();
                    selectedJoints.Clear();
                    DoWork = false;
                    JointsComboBox.ListIndex = 1;
                    LimitsComboBox.ListIndex = 1;
                    DoWork = true;
                    JointsComboBox.Enabled = false;
                    LimitsComboBox.Enabled = false;
                    EditDrivers.Enabled = false;
                    EditLimits.Enabled = false;
                }
            }
            if (!MainApplication.ActiveDocument.InternalName.Equals(nativeDoc.InternalName))
            {
                if (rightDoc)
                {
                    rightDoc = false;

                    JointsComboBox.Enabled = false;
                    LimitsComboBox.Enabled = false;
                    ExportRobot.Enabled = false;// change buttons the proper state
                    StartExport.Enabled = true;
                    CancelExport.Enabled = false;
                    EditDrivers.Enabled = false;
                    EditLimits.Enabled = false;
                    SelectJointInsideJoint.Enabled = false;

                    oPane.Visible = false;// Hide the browser pane
                }
            }
            else
            {
                if (!rightDoc)
                {
                    rightDoc = true;

                    JointsComboBox.Enabled = true;
                    LimitsComboBox.Enabled = true;
                    ExportRobot.Enabled = true;// change buttons the proper state
                    StartExport.Enabled = false;
                    CancelExport.Enabled = true;
                    EditDrivers.Enabled = true;
                    EditLimits.Enabled = true;
                    SelectJointInsideJoint.Enabled = true;

                    oPane.Visible = true;// Hide the browser pane
                    oPane.Activate();
                }

            }
        }
        public void CancelExport_OnExecute(Inventor.NameValueMap Context)
        {
            try
            {
                SwitchSelectedJoint(DriveTypes.NoDriver);// change combo box selections to default
                SwitchSelectedLimit(false);
                inExportView = false;// exit export view
                AssemblyDocument asmDoc = (AssemblyDocument)MainApplication.ActiveDocument;
                foreach (ComponentOccurrence c in asmDoc.ComponentDefinition.Occurrences)
                {
                    c.Enabled = true;
                }
                JointsComboBox.Enabled = false;
                LimitsComboBox.Enabled = false;
                ExportRobot.Enabled = false;// change buttons the proper state
                StartExport.Enabled = true;
                CancelExport.Enabled = false;
                EditDrivers.Enabled = false;
                EditLimits.Enabled = false;
                SelectJointInsideJoint.Enabled = false;

                oPane.Visible = false;// Hide the browser pane
                WriteNumJoints();

                foreach (JointData l in jointList)
                {
                    WriteSave(l);
                }
                jointList = new ArrayList();// clear jointList
                foreach (BrowserNode folder in oPane.TopNode.BrowserNodes)
                {
                    folder.Delete();// delete the folders
                }
                MainApplication.ActiveDocument.Save();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        // passes call to the joints combo box, allows for the windows form to be activated without reselecting in the combo box
        public void EditDrivers_OnExecute(Inventor.NameValueMap Context)
        {
            JointsComboBox_OnSelect(Context);
        }
        // passes call to the limits combo box, allows for the windows form to be activated without reselecting in the combo box
        public void EditLimits_OnExecute(Inventor.NameValueMap Context)
        {
            LimitsComboBox_OnSelect(Context);
        }

        private void JointsComboBox_OnSelect(NameValueMap context)
        {
            try
            {
                if (DoWork)
                {
                    if (selectedJoints.Count > 0)
                    {
                        selectedJointData = (JointData)selectedJoints[0];
                        try
                        {
                            ConfigureJoint.readFromData(selectedJointData);
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.ToString());
                        }
                        if (Rotating)
                        {
                            if (JointsComboBox.Text.Equals("Motor"))
                            {
                                selectedJointData.Driver = DriveTypes.Motor;
                                ConfigureJoint.MotorChosen();
                                ConfigureJoint.readFromData(selectedJointData);
                                ConfigureJoint.ShowDialog();
                            }
                            else if (JointsComboBox.Text.Equals("Servo"))
                            {
                                selectedJointData.Driver = DriveTypes.Servo;
                                ConfigureJoint.ServoChosen();
                                ConfigureJoint.readFromData(selectedJointData);
                                ConfigureJoint.ShowDialog();
                            }
                            else if (JointsComboBox.Text.Equals("Bumper Pneumatic"))
                            {
                                selectedJointData.Driver = DriveTypes.BumperPnuematic;
                                ConfigureJoint.BumperPneumaticChosen();
                                ConfigureJoint.ShowDialog();
                            }
                            else if (JointsComboBox.Text.Equals("Relay Pneumatic"))
                            {
                                selectedJointData.Driver = DriveTypes.RelayPneumatic;
                                ConfigureJoint.RelayPneumaticChosen();
                                ConfigureJoint.ShowDialog();
                            }
                            else if (JointsComboBox.Text.Equals("Worm Screw"))
                            {
                                selectedJointData.Driver = DriveTypes.WormScrew;
                                ConfigureJoint.WormScrewChosen();
                                ConfigureJoint.ShowDialog();
                            }
                            else if (JointsComboBox.Text.Equals("Dual Motor"))
                            {
                                selectedJointData.Driver = DriveTypes.DualMotor;
                                ConfigureJoint.DualMotorChosen();
                                ConfigureJoint.ShowDialog();
                            }
                            else
                            {
                                selectedJointData.Driver = DriveTypes.NoDriver;
                            }
                        }
                        else
                        {
                            if (JointsComboBox.Text.Equals("Elevator"))
                            {
                                selectedJointData.Driver = DriveTypes.Elevator;
                                ConfigureJoint.ElevatorChosen();
                                ConfigureJoint.ShowDialog();
                            }
                            else if (JointsComboBox.Text.Equals("Bumper Pneumatic"))
                            {
                                selectedJointData.Driver = DriveTypes.BumperPnuematic;
                                ConfigureJoint.BumperPneumaticChosen();
                                ConfigureJoint.ShowDialog();
                            }
                            else if (JointsComboBox.Text.Equals("Relay Pneumatic"))
                            {
                                selectedJointData.Driver = DriveTypes.RelayPneumatic;
                                ConfigureJoint.RelayPneumaticChosen();
                                ConfigureJoint.ShowDialog();
                            }
                            else if (JointsComboBox.Text.Equals("Worm Screw"))
                            {
                                selectedJointData.Driver = DriveTypes.WormScrew;
                                ConfigureJoint.WormScrewChosen();
                                ConfigureJoint.ShowDialog();
                            }
                            else
                            {
                                selectedJointData.Driver = DriveTypes.NoDriver;
                            }
                        }
                        for (int i = 0; i < selectedJoints.Count; i++)
                        {
                            selectedJointData.copyTo((JointData)selectedJoints[i]);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please select a joint to edit");
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        // reacts to the limits combobox being selected
        public void LimitsComboBox_OnSelect(Inventor.NameValueMap Context)
        {
            if (DoWork)
            {// if the select event shuold be reacted to
                if (selectedJoints.Count > 0)
                {
                    selectedJointData = (JointData)selectedJoints[0];
                    try
                    {
                        EditLimitsForm.readFromData(selectedJointData);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                    }
                    if (LimitsComboBox.Text.Equals("Limits"))// if limits is selected then set the selected joint gets the correct data
                    {
                        selectedJointData.HasLimits = true;
                        EditLimitsForm.ShowDialog();// show windows form
                    }
                    else
                    {
                        selectedJointData.HasLimits = false;
                    }
                }
                else
                {
                    MessageBox.Show("Please select a joint to edit");
                }
            }
        }
    }
    internal class AxHostConverter : AxHost
    {
        private AxHostConverter()
            : base("")
        {
        }


        public static stdole.IPictureDisp ImageToPictureDisp(Image image)
        {
            return (stdole.IPictureDisp)GetIPictureDispFromPicture(image);
        }


        public static Image PictureDispToImage(stdole.IPictureDisp pictureDisp)
        {
            return GetPictureFromIPicture(pictureDisp);
        }
    }
}
