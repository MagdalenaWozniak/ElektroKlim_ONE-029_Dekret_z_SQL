using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DekretZSql
{
    public partial class Pasek : Form
    {
        public Pasek()
        {
            try
            {
                InitializeComponent();
            }
            catch(Exception e)
            {
                MessageBox.Show("Błąd paska: " + e, "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
