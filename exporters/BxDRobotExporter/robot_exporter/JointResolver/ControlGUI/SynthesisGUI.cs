using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using EditorsLibrary;
using OGLViewer;

public delegate bool ValidationAction(RigidNode_Base baseNode, out string message);

[GuidAttribute("ec18f8d4-c13e-4c86-8148-7414efb6e1e2")]
public partial class SynthesisGUI : Form
{
    private ExporterLogger logger;
    public static SynthesisGUI Instance;
    public static bool LiteMode = true;

    public static ExporterSettingsForm.PluginSettingsValues ExporterSettings;
    public static ViewerSettingsForm.ViewerSettingsValues ViewerSettings;

    public Form BXDAViewerPaneForm = new Form
    {
        FormBorderStyle = FormBorderStyle.None
    };
    public Form JointPaneForm = new Form
    {
        FormBorderStyle = FormBorderStyle.None
    };

    public RigidNode_Base SkeletonBase = null;
    public List<BXDAMesh> Meshes = null;

    private ExporterForm exporter;

    private LiteExporterForm liteExporter;

    /// <summary>
    /// The last path that was saved to/opened from
    /// </summary>
    private string lastDirPath = null;

    static SynthesisGUI()
    {
        ExporterSettings = ExporterSettingsForm.GetDefaultSettings();
        ViewerSettings = ViewerSettingsForm.GetDefaultSettings();
    }

    public SynthesisGUI(ExporterLogger logger, bool MakeOwners = false)
    {
        this.logger = logger;
        InitializeComponent();

        Instance = this;


        bxdaEditorPane1.Units = ViewerSettings.modelUnits;
        BXDAViewerPaneForm.Controls.Add(bxdaEditorPane1);
        if (MakeOwners) BXDAViewerPaneForm.Owner = this;
        BXDAViewerPaneForm.FormClosing += Generic_FormClosing;

        JointPaneForm.Controls.Add(jointEditorPane1);
        if (MakeOwners) JointPaneForm.Owner = this;
        JointPaneForm.FormClosing += Generic_FormClosing;


        RigidNode_Base.NODE_FACTORY = delegate (Guid guid)
        {
            return new OGL_RigidNode(guid);
        };

        fileNew.Click += new System.EventHandler(delegate (object sender, System.EventArgs e)
        {
            SetNew();
        });
        fileLoad.Click += FileLoad_OnClick;
        fileOpen.Click += new System.EventHandler(delegate (object sender, System.EventArgs e)
        {
            OpenExisting();
        });
        fileSave.Click += new System.EventHandler(delegate (object sender, System.EventArgs e)
        {
            SaveRobot(false);
        });
        fileSaveAs.Click += new System.EventHandler(delegate (object sender, System.EventArgs e)
        {
            SaveRobot(true);
        });
        fileExit.Click += new System.EventHandler(delegate (object sender, System.EventArgs e)
        {
            Close();
        });

        settingsExporter.Click += SettingsExporter_OnClick;
        settingsViewer.Click += SettingsViewer_OnClick;

        helpAbout.Click += new System.EventHandler(delegate (object sender, System.EventArgs e)
        {
            AboutDialog about = new AboutDialog();
            about.ShowDialog(this);
        });

        this.Shown += SynthesisGUI_Shown;

        this.FormClosing += new FormClosingEventHandler(delegate (object sender, FormClosingEventArgs e)
        {
            if (SkeletonBase != null && !WarnUnsaved()) e.Cancel = true;
            InventorManager.ReleaseInventor();
        });

        jointEditorPane1.ModifiedJoint += delegate (List<RigidNode_Base> nodes)
        {

            if (nodes == null || nodes.Count == 0) return;

            foreach (RigidNode_Base node in nodes)
            {
                if (node.GetSkeletalJoint() != null && node.GetSkeletalJoint().cDriver != null &&
                    node.GetSkeletalJoint().cDriver.GetInfo<WheelDriverMeta>() != null &&
                    node.GetSkeletalJoint().cDriver.GetInfo<WheelDriverMeta>().radius == 0 &&
                    node is OGL_RigidNode)
                {
                    float radius;
                    float width;
                    BXDVector3 center;
                    (node as OGL_RigidNode).GetWheelInfo(out radius, out width, out center);

                    WheelDriverMeta wheelDriver = node.GetSkeletalJoint().cDriver.GetInfo<WheelDriverMeta>();
                    wheelDriver.center = center;
                    wheelDriver.radius = radius;
                    wheelDriver.width = width;
                    node.GetSkeletalJoint().cDriver.AddInfo(wheelDriver);

                }
            }
        };

        jointEditorPane1.SelectedJoint += bxdaEditorPane1.SelectJoints;

        bxdaEditorPane1.NodeSelected += (BXDAMesh mesh) =>
            {
                List<RigidNode_Base> nodes = new List<RigidNode_Base>();
                SkeletonBase.ListAllNodes(nodes);

                jointEditorPane1.AddSelection(nodes[Meshes.IndexOf(mesh)], true);
            };
    }

    private void Generic_FormClosing(object sender, FormClosingEventArgs e)
    {
        foreach (Form f in OwnedForms)
        {
            if(f.Visible)
                f.Close();
        }
        Close();
    }

    private void SynthesisGUI_Shown(object sender, EventArgs e)
    {
        Hide();
        BXDAViewerPaneForm.Show();
        JointPaneForm.Show();
    }

    public void SetNew()
    {
        if (SkeletonBase == null || !WarnUnsaved()) return;

        SkeletonBase = null;
        Meshes = null;

        ReloadPanels();
    }

    /// <summary>
    /// Export a robot from Inventor
    /// </summary>
    public bool ExportMeshes()
    {
        if (SkeletonBase != null && !WarnUnsaved()) return false;

        try
        {

            var exporterThread = new Thread(() =>
            {
                if (!LiteMode)
                {
                    exporter = new ExporterForm(ExporterSettings);
                    exporter.ShowDialog(); 
                }
                else
                {
                    liteExporter = new LiteExporterForm(logger);
                    liteExporter.ShowDialog();
                }
            });

            exporterThread.SetApartmentState(ApartmentState.STA);
            exporterThread.Start();

            exporterThread.Join();

            GC.Collect();
        }
        catch (InvalidComObjectException)
        {
        }
        catch (TaskCanceledException)
        {
            return false;
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
            return false;
        }

        List<RigidNode_Base> nodes = SkeletonBase.ListAllNodes();
        for (int i = 0; i < Meshes.Count; i++)
        {
            ((OGL_RigidNode)nodes[i]).loadMeshes(Meshes[i]);
        }
        ReloadPanels();
        return true;
    }

    /// <summary>
    /// Open a previously exported robot. 
    /// </summary>
    /// <param name="validate">If it is not null, this will validate the open inventor assembly.</param>
    public void OpenExisting()
    {
        if (SkeletonBase != null && !WarnUnsaved()) return;

        string dirPath = OpenFolderPath();

        if (dirPath == null) return;

        try
        {
            List<RigidNode_Base> nodes = new List<RigidNode_Base>();
            SkeletonBase = BXDJSkeleton.ReadSkeleton(dirPath + "\\skeleton.bxdj");

            SkeletonBase.ListAllNodes(nodes);

            Meshes = new List<BXDAMesh>();

            foreach (RigidNode_Base n in nodes)
            {
                BXDAMesh mesh = new BXDAMesh();
                mesh.ReadFromFile(dirPath + "\\" + n.ModelFileName);

                if (!n.GUID.Equals(mesh.GUID))
                {
                    MessageBox.Show(n.ModelFileName + " has been modified.", "Could not load mesh.");
                    return;
                }

                Meshes.Add(mesh);
            }
        }
        catch (Exception e)
        {
            MessageBox.Show(e.ToString());
        }

        lastDirPath = dirPath;

        ReloadPanels();
    }

    public bool OpenExisting(ValidationAction validate)
    {
        if (SkeletonBase != null && !WarnUnsaved()) return false;

        string dirPath = OpenFolderPath();

        if (dirPath == null) return false;

        try
        {
            List<RigidNode_Base> nodes = new List<RigidNode_Base>();
            SkeletonBase = BXDJSkeleton.ReadSkeleton(dirPath + "\\skeleton.bxdj");

            if (validate != null)
            {
                if (!validate(SkeletonBase, out string message))
                {
                    while (true)
                    {
                        DialogResult result = MessageBox.Show(message, "Assembly Validation", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        if (result == DialogResult.Retry)
                            continue;
                        if (result == DialogResult.Abort)
                        {
                            return false;
                        }
                        break;
                    }
                }
                #region DEBUG
#if DEBUG
                else
                {
                    MessageBox.Show(message);
                }
#endif 
                #endregion
            }

            SkeletonBase.ListAllNodes(nodes);

            Meshes = new List<BXDAMesh>();

            foreach (RigidNode_Base n in nodes)
            {
                BXDAMesh mesh = new BXDAMesh();
                mesh.ReadFromFile(dirPath + "\\" + n.ModelFileName);

                if (!n.GUID.Equals(mesh.GUID))
                {
                    MessageBox.Show(n.ModelFileName + " has been modified.", "Could not load mesh.");
                    return false;
                }

                Meshes.Add(mesh);
            }
        }
        catch (Exception e)
        {
            MessageBox.Show(e.ToString());
        }

        lastDirPath = dirPath;

        ReloadPanels();
        return true;
    }

    /// <summary>
    /// Save all changes to an open robot
    /// </summary>
    /// <param name="isSaveAs">If this is a "Save As" operation</param>
    /// <returns>If the save operation succeeded</returns>
    public bool SaveRobot(bool isSaveAs = false, bool previewRobot = false)
    {
        if (SkeletonBase == null || Meshes == null) return false;

        string dirPath = lastDirPath;

        if (dirPath == null || isSaveAs) dirPath = OpenFolderPath();
        if (dirPath == null) return false;

        if (File.Exists(dirPath + "\\skeleton.bxdj"))
        {
            if (dirPath != lastDirPath && !WarnOverwrite()) return false;
        }

        try
        {
            BXDJSkeleton.WriteSkeleton(dirPath + "\\skeleton.bxdj", SkeletonBase);

            for (int i = 0; i < Meshes.Count; i++)
            {
                Meshes[i].WriteToFile(dirPath + "\\node_" + i + ".bxda");
            }

            /*
             * The commented code below is for testing purposes.
             * To determine if the reading/writing process runs
             * without loss of data, compare the text of skeleton.bxdj
             * and skeleton2.bxdj. If they are equal, no data was lost.
             */

            /** /
            RigidNode_Base testRigidNode = BXDJSkeleton.ReadSkeleton(dirPath + "\\skeleton.bxdj");

            BXDJSkeleton.WriteSkeleton(dirPath + "\\skeleton2.bxdj", testRigidNode);
            /**/
        }
        catch (Exception e)
        {
            MessageBox.Show("Couldn't save robot \n" + e.Message);
            return false;
        }

        MessageBox.Show("Saved!");

        lastDirPath = dirPath;

        if(previewRobot)
        {
            Process.Start(Utilities.VIEWER_PATH, "-path " + lastDirPath);
        }

        return true;
    }

    /// <summary>
    /// Reset the <see cref="ExporterProgressWindow"/> progress bar
    /// </summary>
    public void ExporterReset()
    {
        exporter.ResetProgress();
    }

    public void ExporterOverallReset()
    {
        exporter.ResetOverall();
    }

    /// <summary>
    /// Set the length of the <see cref="ExporterProgressWindow"/> progress bar
    /// </summary>
    /// <param name="percentLength">The length of the bar in percentage points (0%-100%)</param>
    public void ExporterSetProgress(double percentLength)
    {
        exporter.AddProgress((int)Math.Floor(percentLength) - exporter.GetProgress());
    }

    /// <summary>
    /// Set the <see cref="ExporterProgressWindow"/> text after "Progress:"
    /// </summary>
    /// <param name="text">The text to add</param>
    public void ExporterSetSubText(string text)
    {
        exporter.SetProgressText(text);
    }

    public void ExporterSetMeshes(int num)
    {
        exporter.SetNumMeshes(num);
    }

    public void ExporterStepOverall()
    {
        exporter.AddOverallStep();
    }

    public void ExporterSetOverallText(string text)
    {
        exporter.SetOverallText(text);
    }

    /// <summary>
    /// Get the desired folder to open from or save to
    /// </summary>
    /// <returns>The full path of the selected folder</returns>
    private string OpenFolderPath()
    {
        string dirPath = null;

        var dialogThread = new Thread(() =>
        {
            FolderBrowserDialog openDialog = new FolderBrowserDialog()
            {
                ShowNewFolderButton = true,
                Description = "Choose Robot Folder"
            };
            DialogResult openResult = openDialog.ShowDialog();

            if (openResult == DialogResult.OK) dirPath = openDialog.SelectedPath;
        });

        dialogThread.SetApartmentState(ApartmentState.STA);
        dialogThread.Start();
        dialogThread.Join();

        return dirPath;
    }

    /// <summary>
    /// Warn the user that they are about to overwrite existing data
    /// </summary>
    /// <returns>Whether the user wishes to overwrite the data</returns>
    private bool WarnOverwrite()
    {
        DialogResult overwriteResult = MessageBox.Show("Really overwrite existing robot?", "Overwrite Warning", MessageBoxButtons.YesNo);

        if (overwriteResult == DialogResult.Yes) return true;
        else return false;
    }

    /// <summary>
    /// Warn the user that they are about to exit without unsaved work
    /// </summary>
    /// <returns>Whether the user wishes to continue without saving</returns>
    public bool WarnUnsaved()
    {
        DialogResult saveResult = MessageBox.Show("Do you want to save your work?", "Save", MessageBoxButtons.YesNoCancel);

        if (saveResult == DialogResult.Yes)
        {
            return SaveRobot(false);
        }
        else if (saveResult == DialogResult.No)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Reload all panels with newly loaded robot data
    /// </summary>
    private void ReloadPanels()
    {
        jointEditorPane1.SetSkeleton(SkeletonBase);
        bxdaEditorPane1.loadModel(Meshes);
    }

    protected override void OnResize(EventArgs e)
    {
        SuspendLayout();

        base.OnResize(e);
        splitContainer1.Height = ClientSize.Height - 27;

        ResumeLayout();
    }

    private void HelpTutorials_Click(object sender, EventArgs e)
    {
        Process.Start("http://bxd.autodesk.com/tutorial-robot.html");
    }

    public void SettingsExporter_OnClick(object sender, System.EventArgs e)
    {
        //TODO: Implement Value saving and loading
        ExporterSettingsForm eSettingsForm = new ExporterSettingsForm();

        eSettingsForm.ShowDialog(this);

        //BXDSettings.Instance.AddSettingsObject("Plugin Settings", ExporterSettingsForm.values);
        ExporterSettings = ExporterSettingsForm.values;
    }

    public void SettingsViewer_OnClick(object sender, System.EventArgs e)
    {
        //var defaultValues = BXDSettings.Instance.GetSettingsObject("Viewer Settings");

        //ViewerSettingsForm vSettingsForm = new ViewerSettingsForm((defaultValues != null) ? (ViewerSettingsForm.ViewerSettingsValues)defaultValues :
        //                                                                        ViewerSettingsForm.GetDefaultSettings());

        //vSettingsForm.ShowDialog(this);

        //BXDSettings.Instance.AddSettingsObject("Viewer Settings", vSettingsForm.values);
        //ViewerSettings = vSettingsForm.values;

        //robotViewer1.LoadSettings(ViewerSettings);
        //bxdaEditorPane1.Units = ViewerSettings.modelUnits;
    }

    public void FileLoad_OnClick(object sender, System.EventArgs e)
    {
        ExportMeshes();
    }

    public void PreviewRobot()
    {
        if(lastDirPath != null)
        {
            Process.Start(Utilities.VIEWER_PATH, "-path " + lastDirPath);
        }
    }
}
