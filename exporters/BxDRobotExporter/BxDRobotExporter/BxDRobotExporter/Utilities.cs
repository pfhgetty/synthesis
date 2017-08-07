using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventor;
using System.Windows.Forms;
using System.Drawing;

namespace BxDRobotExporter
{
    internal static class Utilities
    {
        //TODO: Maybe make less stuff static. Or just make it a singleton. 
        static internal SynthesisGUI GUI;
        static DockableWindow EmbededViewer;
        static DockableWindow EmbededJointPane;
        static DockableWindow EmbededBxDViewer;

        /// <summary>
        /// Creates a dockable window containing all of the components of the SynthesisGUI object
        /// </summary>
        /// <param name="app"></param>
        public static void CreateDockableWindows(Inventor.Application app)
        {
            try
            {
                IntPtr[] children = CreateChildDialog();

                UserInterfaceManager uiMan = app.UserInterfaceManager;
                EmbededViewer = uiMan.DockableWindows.Add(Guid.NewGuid().ToString(), "BxD:RobotExporter:EmbededLegacy0", "Robot Viewer");
                EmbededJointPane = uiMan.DockableWindows.Add(Guid.NewGuid().ToString(), "BxD:RobotExporter:EmbededLegacy1", "Robot Joint Editor");
                EmbededBxDViewer = uiMan.DockableWindows.Add(Guid.NewGuid().ToString(), "BxD:RobotExporter:EmbededLegacy2", "Robot BxD Viewer");

                #region EmbededViewer
                if (EmbededViewer.IsCustomized)
                    EmbededViewer.DockingState = DockingStateEnum.kDockLastKnown;
                else
                    EmbededViewer.DockingState = DockingStateEnum.kDockBottom;
                EmbededViewer.ShowVisibilityCheckBox = true;
                EmbededViewer.Visible = true;
                EmbededViewer.ShowTitleBar = false;
                EmbededViewer.AddChild(children[0]);
                #endregion

                #region EmbededJointPane
                if (EmbededJointPane.IsCustomized)
                    EmbededJointPane.DockingState = DockingStateEnum.kDockLastKnown;
                else
                    EmbededJointPane.DockingState = DockingStateEnum.kDockBottom;
                EmbededJointPane.ShowVisibilityCheckBox = true;
                EmbededJointPane.Visible = true;
                EmbededJointPane.ShowTitleBar = false;
                EmbededJointPane.AddChild(children[1]);
                #endregion

                #region EmbededBxDViewer
                if (EmbededJointPane.IsCustomized)
                    EmbededJointPane.DockingState = DockingStateEnum.kDockLastKnown;
                else
                    EmbededJointPane.DockingState = DockingStateEnum.kDockBottom;
                EmbededBxDViewer.ShowVisibilityCheckBox = true;
                EmbededBxDViewer.ShowTitleBar = false;
                EmbededBxDViewer.SetMinimumSize(100, 100);
                EmbededBxDViewer.Visible = true;

                EmbededBxDViewer.AddChild(children[2]);
                #endregion
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private static IntPtr[] CreateChildDialog()
        {
            try
            {
                GUI = new SynthesisGUI();
                GUI.Opacity = 0.00d;
                GUI.Show();
                GUI.Opacity = 1.00d;

                return new IntPtr[] { GUI.robotViewer1.Handle, GUI.JointPaneForm.Handle, GUI.ViewerPaneForm.Handle };
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                throw;
            }
        }

        /// <summary>
        /// Disposes of the dockable windows.
        /// </summary>
        public static void DisposeDockableWindows()
        {
            if (EmbededViewer != null && EmbededJointPane != null && EmbededBxDViewer != null)
            {
                EmbededViewer.Visible = false;
                EmbededViewer.Delete();

                EmbededJointPane.Visible = false;
                EmbededJointPane.Delete();

                EmbededBxDViewer.Visible = false;
                EmbededBxDViewer.Delete(); 
            }
        }

        public static void HideDockableWindows()
        {
            if (EmbededViewer != null && EmbededJointPane != null && EmbededBxDViewer != null)
            {
                EmbededViewer.Visible = false;
                EmbededJointPane.Visible = false;
                EmbededBxDViewer.Visible = false;
            }
        }
        public static void ShowDockableWindows()
        {
            if (EmbededViewer != null && EmbededJointPane != null && EmbededBxDViewer != null)
            {
                EmbededViewer.Visible = true;
                EmbededJointPane.Visible = true;
                EmbededBxDViewer.Visible = true;
            }
        }
    }
}
