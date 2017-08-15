using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventor;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Diagnostics;

namespace BxDRobotExporter
{
    internal static class Utilities
    {
        //TODO: Maybe make less stuff static. Or just make it a singleton. 
        static internal SynthesisGUI GUI;
        static DockableWindow EmbededJointPane;
        static DockableWindow EmbededBxDViewer;


        /// <summary>
        /// Creates a dockable window containing all of the components of the SynthesisGUI object
        /// </summary>
        /// <param name="app"></param>
        public static void CreateDockableWindows(Inventor.Application app)
        {
            IntPtr[] children = CreateChildDialog();

            UserInterfaceManager uiMan = app.UserInterfaceManager;
            EmbededJointPane = uiMan.DockableWindows.Add(Guid.NewGuid().ToString(), "BxD:RobotExporter:JointEditor", "Robot Joint Editor");
            EmbededBxDViewer = uiMan.DockableWindows.Add(Guid.NewGuid().ToString(), "BxD:RobotExporter:BxDViewer", "Robot BxD Viewer");

            #region EmbededJointPane
            EmbededJointPane.DockingState = DockingStateEnum.kDockBottom;
            EmbededJointPane.Height = 250;
            EmbededJointPane.ShowVisibilityCheckBox = true;
            EmbededJointPane.ShowTitleBar = true;
            EmbededJointPane.AddChild(children[0]);
            #endregion

            #region EmbededBxDViewer
            EmbededBxDViewer.DockingState = DockingStateEnum.kDockRight;
            EmbededBxDViewer.Width = uiMan.DockableWindows["model"].Width;
            EmbededBxDViewer.ShowVisibilityCheckBox = true;
            EmbededBxDViewer.ShowTitleBar = true;
            EmbededBxDViewer.AddChild(children[1]);
            #endregion

            EmbededBxDViewer.Visible = true;
            EmbededJointPane.Visible = true;
        }

        private static IntPtr[] CreateChildDialog()
        {
            try
            {
                GUI = new SynthesisGUI(StandardAddInServer.Instance.Logger)
                {
                    Opacity = 0.00d
                };
                GUI.Show();
                GUI.Hide();
                GUI.Opacity = 1.00d;

                return new IntPtr[] { GUI.JointPaneForm.Handle, GUI.BXDAViewerPaneForm.Handle };
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
            if (/*EmbededViewer != null && */EmbededJointPane != null && EmbededBxDViewer != null)
            {
                //EmbededViewer.Visible = false;
                //EmbededViewer.Delete();

                EmbededJointPane.Visible = false;
                EmbededJointPane.Delete();

                EmbededBxDViewer.Visible = false;
                EmbededBxDViewer.Delete();
            }
        }

        /// <summary>
        /// Hides the dockable windows. Used when switching documents. Called in <see cref="StandardAddInServer.ApplicationEvents_OnDeactivateDocument(_Document, EventTimingEnum, NameValueMap, out HandlingCodeEnum)"/>.
        /// </summary>
        public static void HideDockableWindows()
        {
            if (/*EmbededViewer != null &&*/ EmbededJointPane != null && EmbededBxDViewer != null)
            {
                //EmbededViewer.Visible = false;
                EmbededJointPane.Visible = false;
                EmbededBxDViewer.Visible = false;
            }
        }

        /// <summary>
        /// Shows the dockable windows again when assembly document is switched back to. Called in <see cref="StandardAddInServer.ApplicationEvents_OnActivateDocument(_Document, EventTimingEnum, NameValueMap, out HandlingCodeEnum)"/>.
        /// </summary>
        public static void ShowDockableWindows()
        {
            if (/*EmbededViewer != null && */EmbededJointPane != null && EmbededBxDViewer != null)
            {
                //EmbededViewer.Visible = true;
                EmbededJointPane.Visible = true;
                EmbededBxDViewer.Visible = true;
            }
        }

        /// <summary>
        /// Converts from a <see cref="System.Drawing.Color"/> to an <see cref="Inventor.Color"/>
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Inventor.Color GetInventorColor(System.Drawing.Color color)
        {
            return StandardAddInServer.Instance.MainApplication.TransientObjects.CreateColor(color.R, color.G, color.B);
        }
    }
}
