using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Tenorio_Activity1
{
    public static class Prompt
    {
        public static string ShowDialog(string text, string caption)
        {
            var form = new Form
            {
                Width = 420,
                Height = 160,
                Text = caption,
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var lbl = new Label { Text = text, Left = 15, Top = 20, Width = 380 };
            var txt = new TextBox { Left = 15, Top = 50, Width = 380, UseSystemPasswordChar = true };
            var btnOk = new Button { Text = "OK", Left = 220, Top = 85, Width = 80, DialogResult = DialogResult.OK };
            var btnCancel = new Button { Text = "Cancel", Left = 310, Top = 85, Width = 80, DialogResult = DialogResult.Cancel };

            form.Controls.Add(lbl);
            form.Controls.Add(txt);
            form.Controls.Add(btnOk);
            form.Controls.Add(btnCancel);
            form.AcceptButton = btnOk;
            form.CancelButton = btnCancel;

            return form.ShowDialog() == DialogResult.OK ? txt.Text : null;
        }
    }
}
