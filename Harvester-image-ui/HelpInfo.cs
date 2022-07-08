using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Harvester_image_ui
{
    public partial class HelpInfo : Form
    {
        public HelpInfo()
        {
            InitializeComponent();
        }

        private void ShowImageExplanationButton_Click(object sender, EventArgs e)
        {
            HelpImages him = new HelpImages();
            him.Show();
        }
    }
}
