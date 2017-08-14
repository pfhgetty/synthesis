using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace EditorsLibrary
{
    public delegate void SettingsEvent(Color Child, Color Parent, bool IsParentHighlight);

    public partial class ExporterSettingsForm : Form
    {
        /// <summary>
        /// The local copy of the setting values
        /// </summary>
        public static PluginSettingsValues values = GetDefaultSettings();

        public ExporterSettingsForm(PluginSettingsValues defaultValues)
        {
            InitializeComponent();

            values = defaultValues;
            LoadValues();

            buttonOK.Click += delegate(object sender, EventArgs e)
            {
                SaveValues();
                Close();
            };

            buttonCancel.Click += delegate(object sender, EventArgs e)
            {
                Close();
            };
        }

        public ExporterSettingsForm()
        {
            InitializeComponent();

            LoadValues();

            buttonOK.Click += delegate (object sender, EventArgs e)
            {
                SaveValues();
                Close();
            };

            buttonCancel.Click += delegate (object sender, EventArgs e)
            {
                Close();
            };

        }

        /// <summary>
        /// Load values into the form
        /// </summary>
        private void LoadValues()
        {
            ChildHighlight.BackColor = values.InventorChildColor;
            ParentHighlight.BackColor = values.InventorParentColor;
            HighlightParentsCheckBox.Checked = values.InventorHighlightParent;
            if (!values.InventorHighlightParent)
                ParentLabel.ForeColor = Color.Gray;
            HighlightParentsCheckBox_CheckedChanged(null, null);
        }
        
        /// <summary>
        /// Save the form's values in a <see cref="PluginSettingsValues"/> structure
        /// </summary>
        private void SaveValues()
        {
            values.InventorChildColor = ChildHighlight.BackColor;
            values.InventorParentColor = ParentHighlight.BackColor;
            values.InventorHighlightParent = HighlightParentsCheckBox.Checked;
            values.OnSettingsChanged(ChildHighlight.BackColor, ParentHighlight.BackColor, HighlightParentsCheckBox.Checked);
        }

        /// <summary>
        /// Get the default values for the <see cref="PluginSettingsValues"/> structure
        /// </summary>
        /// <returns>Default values for the <see cref="Exporter"/></returns>
        public static PluginSettingsValues GetDefaultSettings()
        {
           return new PluginSettingsValues()
            {
                InventorParentColor = Color.FromArgb(255, 125, 0, 255),
                InventorChildColor = Color.FromArgb(255, 0, 125, 255),
                InventorHighlightParent = false
            };
        }

        /// <summary>
        /// The struct that stores settings for the <see cref="Exporter"/>
        /// </summary>
        public struct PluginSettingsValues
        {
            [XmlIgnore]
            public Color InventorParentColor;
            [XmlIgnore]
            public Color InventorChildColor;

            [XmlElement("InventorParentColor")]
            private int InventorParentColorARGB
            {
                get
                {
                    return InventorParentColor.ToArgb();
                }
                set
                {
                    InventorParentColor = Color.FromArgb(value);
                }
            }
            [XmlElement("InventorChildColor")]
            private int InventorChildColorARGB
            {
                get
                {
                    return InventorChildColor.ToArgb();
                }
                set
                {
                    InventorChildColor = Color.FromArgb(value);
                }
            }


            public bool InventorHighlightParent;

            public static event SettingsEvent SettingsChanged;
            internal void OnSettingsChanged(Color Child, Color Parent, bool IsParentHighlight)
            {
                SettingsChanged.Invoke(Child, Parent, IsParentHighlight);
            }
        }

        /// <summary>
        /// Handles the 'Highlight Parents' checkbox being changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HighlightParentsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ParentHighlight.Enabled = HighlightParentsCheckBox.Checked;
            if (ParentHighlight.Enabled)
                ParentLabel.ForeColor = Color.Black;
            else
                ParentLabel.ForeColor = Color.Gray;
        }

        /// <summary>
        /// Sets the <see cref="Color"/> of the , and by extension the <see cref="PluginSettingsValues"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChildHighlight_Click(object sender, EventArgs e)
        {
            ColorDialog colorChoose = new ColorDialog();
            colorChoose.ShowDialog();
            ChildHighlight.BackColor = colorChoose.Color;
        }

        /// <summary>
        /// Sets the <see cref="Color"/> of the background, and by extension the <see cref="PluginSettingsValues"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ParentHighlight_Click(object sender, EventArgs e)
        {
            ColorDialog colorChoose = new ColorDialog();
            colorChoose.ShowDialog();
            ParentHighlight.BackColor = colorChoose.Color;
        }
    }
}
