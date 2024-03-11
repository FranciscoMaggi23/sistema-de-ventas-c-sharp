using CapaEntidad;
using CapaPresentacion.Modales;
using CapaPresentacion.Utilidades;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapaPresentacion
{
    public partial class Frm_Compras : Form
    {
        //variable que almacenara el usuario logeado del formulario de compras
        private Usuario _usuario;

        public Frm_Compras(Usuario ousuario = null)
        {
            _usuario = ousuario;
            InitializeComponent();
        }

        private void Frm_Compras_Load(object sender, EventArgs e)
        {

            cboTipoDoc.Items.Add(new OpcionCombo() { Valor = "Boleta", Texto = "Boleta" });
            cboTipoDoc.Items.Add(new OpcionCombo() { Valor = "Factura", Texto = "Factura" });
            cboTipoDoc.DisplayMember = "Texto";
            cboTipoDoc.ValueMember = "Valor";
            cboTipoDoc.SelectedIndex = 0;

            txtFecha.Text = DateTime.Now.ToString("dd/MM/yyyy");

            txtIdProducto.Text = "0";
            txtIdProveedor.Text = "0";



        }

        private void btnBuscarProveedor_Click(object sender, EventArgs e)
        {
            using (var modal = new mdProveedor())
            {
                var result = modal.ShowDialog();

                if(result == DialogResult.OK)
                {

                    txtIdProveedor.Text = modal._Proveedor.IdProveedor.ToString();
                    txtNumDoc.Text = modal._Proveedor.Documento;
                    txtRazonSocial.Text = modal._Proveedor.RazonSocial;

                }
                else
                    txtNumDoc.Select();
            }
        }

        private void btnBuscarProducto_Click(object sender, EventArgs e)
        {
            using (var modal = new mdProductos())
            {
                var result = modal.ShowDialog();

                if (result == DialogResult.OK)
                {

                    txtIdProducto.Text = modal._Producto.IdProducto.ToString();
                    txtCodProducto.Text = modal._Producto.Codigo;
                    txtProducto.Text = modal._Producto.Nombre;
                    txtPrecioCompra.Select();   

                }
                else
                    txtCodProducto.Select();
            }
        }
    }
}
