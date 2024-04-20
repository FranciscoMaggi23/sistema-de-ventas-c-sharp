using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaEntidad;
using FontAwesome.Sharp;
using CapaNegocio;

namespace CapaPresentacion
{
    public partial class Inicio : Form
    {
        private static Usuario usuarioActual;
        private static IconMenuItem MenuActivo = null;
        private static Form FormularioActivo = null;
        public Inicio(Usuario objusuario=null)
        {
            if(objusuario==null)
            {
                usuarioActual = new Usuario(){ NombreCompleto = "pepe", IdUsuario = 1 };
            }
            else
                usuarioActual = objusuario;


            //usuarioActual = objusuario;
            InitializeComponent();
        }

        private void Inicio_Load(object sender, EventArgs e)
        {
            List<Permiso> ListaPermiso = new CN_Permiso().Listar(usuarioActual.IdUsuario);

            foreach(IconMenuItem iconmenu in menu.Items)
            {
                bool encontrado = ListaPermiso.Any(m => m.NombreMenu == iconmenu.Name);

                if (encontrado == false)
                {
                    iconmenu.Visible = false;
                }

            }
            labelIdUsuario.Text = usuarioActual.NombreCompleto;
        }

        private void AbrirFormulario(IconMenuItem menu, Form formulario)
        {
            if (MenuActivo != null)
            {
                MenuActivo.BackColor = Color.White;
            }
            menu.BackColor = Color.Silver;
            MenuActivo = menu;
            if (FormularioActivo != null)
            {
                FormularioActivo.Close();
            }
            //configuramos el formulario
            FormularioActivo = formulario;
            formulario.TopLevel = false;
            formulario.FormBorderStyle = FormBorderStyle.None;
            formulario.Dock = DockStyle.Fill;
            formulario.BackColor = Color.SteelBlue;
            //agregamos el formulario al contenedor
            contenedor.Controls.Add(formulario);
            formulario.Show();  

        }

        private void menuUsuario_Click(object sender, EventArgs e)
        {
            AbrirFormulario((IconMenuItem)sender, new Frm_Usuario());
        }

        private void subMenuCategoria_Click(object sender, EventArgs e)
        {
            AbrirFormulario(menuMantenedor, new Frm_Categoria());

        }

        private void subMenuProductos_Click(object sender, EventArgs e)
        {
            AbrirFormulario(menuMantenedor, new Frm_Productos());
        }

        private void subMenuRegistrarVenta_Click(object sender, EventArgs e)
        {
            AbrirFormulario(menuVentas, new Frm_Ventas(usuarioActual));
        }

        private void subMenuVerDetalleVenta_Click(object sender, EventArgs e)
        {
            AbrirFormulario(menuVentas, new Frm_DetalleVenta());
        }
        private void subMenuRegistrarCompra_Click(object sender, EventArgs e)
        {
            AbrirFormulario(menuVentas, new Frm_Compras(usuarioActual));
        }
        private void subMenuVerDetalleCompra_Click(object sender, EventArgs e)
        {
            AbrirFormulario(menuCompras, new Frm_DetalleCompra());
        }

        private void menuClientes_Click(object sender, EventArgs e)
        {
            AbrirFormulario(menuClientes, new Frm_Clientes());
        }

        private void menuProveedores_Click(object sender, EventArgs e)
        {
            AbrirFormulario(menuProveedores, new Frm_Proveedores());

        }

        private void menu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void subMenuNegocio_Click(object sender, EventArgs e)
        {
            AbrirFormulario(menuMantenedor, new Frm_Negocio());
        }

        private void subMenuReporteCompra_Click(object sender, EventArgs e)
        {
            AbrirFormulario(menuReportes, new FrmReporteCompras());
        }

        private void subMenuReporteVenta_Click(object sender, EventArgs e)
        {
            AbrirFormulario(menuReportes, new FrmReporteVenta());
        }
    }
}
