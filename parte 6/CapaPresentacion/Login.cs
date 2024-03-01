using CapaEntidad;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaNegocio;


namespace CapaPresentacion
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            List<Usuario> test = new CN_Usuario().Listar();

            //de la lista devuelve el primer usuario donde el documento y la clave coincidan con lo escrito en los textbox del login
            Usuario ousuario = new CN_Usuario().Listar().Where(u => u.Documento == txtDocumento.Text && u.Clave == txtClave.Text).FirstOrDefault();
            ;
            if (ousuario != null)
            {
                Inicio form = new Inicio(ousuario);
                form.Show();
                this.Hide();
                //cuando se este cerrrando el formulario, abrimos el form Login
                form.FormClosing += frm_Closing;
            }
            else
            {
                MessageBox.Show("no se encontro el usuario","Mensaje",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
            }


        }

        private void frm_Closing(object sender, FormClosingEventArgs e)
        {
            this.Show();
        }
    }
}
