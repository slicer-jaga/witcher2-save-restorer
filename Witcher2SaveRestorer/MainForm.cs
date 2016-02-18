using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Witcher2SaveRestorer
{
    public partial class frmMain : Form
    {
        private ResourceManager loc = new ResourceManager("Witcher2SaveRestorer.strings", Assembly.GetExecutingAssembly());

        public frmMain()
        {            
            InitializeComponent();            
        }

        private SaveRestorer Restorer = new SaveRestorer();

        private void Log(string Msg)
        {
            txtLog.Text = DateTime.Now.ToString() + ": " + Msg + Environment.NewLine + txtLog.Text;
        }

        private void DoChange(object sender, object e)
        {
            btnRestore.Enabled = Restorer.Loaded && (Restorer.Data.Count > 1);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            try
            {                
                Restorer.CheckAccess();                
            }
            catch (Exception ex)
            {                
                MessageBox.Show(ex.Message, loc.GetString("strError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();                
            }
                        
            Restorer.OnChange += new EventHandler(DoChange);
            try
            {
                Restorer.Load();                
                tmBackup.Enabled = Restorer.Loaded;
                Log(loc.GetString("strRunningSuccess"));                
            }
            catch (Exception ex)
            {
                Log(ex.Message);
                MessageBox.Show(loc.GetString("strCantLoadData") + ex.Message, loc.GetString("strError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            DoChange(null, null);
        }

        private void Check()
        {
            try
            {
                if (Restorer.Check())
                {
                    Log(loc.GetString("strSavingSuccess"));
                }
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Restorer.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(loc.GetString("strCantSaveData") + ex.Message, loc.GetString("strError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(loc.GetString("strRestoreAsk"), loc.GetString("strConfirm"), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                if (Restorer.Restore())
                {
                    Log(loc.GetString("strRestoringSuccess"));
                    MessageBox.Show(loc.GetString("strRestoringSuccessMsg"), loc.GetString("strInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Log(ex.Message);
                MessageBox.Show(ex.Message, loc.GetString("strError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }            
        }

        private void tmBackup_Tick(object sender, EventArgs e)
        {
            Check();
        }

        private void linkAuthor_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("mailto:adsoft@nm.ru?subject=Wither 2 Save Restorer");
        }

        private void linkLicense_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.gnu.org/licenses/gpl.html");
        }

    }
}
