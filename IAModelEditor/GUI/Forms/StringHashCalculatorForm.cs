using AriaLibrary.Helpers;
using Ookii.Dialogs.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IAModelEditor.GUI.Forms
{
    public partial class StringHashCalculatorForm : Form
    {
        public StringHashCalculatorForm()
        {
            InitializeComponent();
        }

        public void button1_Click(object sender, EventArgs e)
        {
            uint hash = StringHelper.GetStringHash(textBox1.Text);
            label1.Text = $"{hash}";
            label2.Text = $"{hash:X8}";
        }
    }
}
