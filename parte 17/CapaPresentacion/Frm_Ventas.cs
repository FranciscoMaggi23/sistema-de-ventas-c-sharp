using CapaEntidad;
using CapaNegocio;
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
    public partial class Frm_Ventas : Form
    {
        private Usuario _usuario;

        public Frm_Ventas(Usuario ousuario = null)
        {
            _usuario = ousuario;
            InitializeComponent();
        }

        private void Frm_Ventas_Load(object sender, EventArgs e)
        {
            cboTipoDoc.Items.Add(new OpcionCombo() { Valor = "Boleta", Texto = "Boleta" });
            cboTipoDoc.Items.Add(new OpcionCombo() { Valor = "Factura", Texto = "Factura" });
            cboTipoDoc.DisplayMember = "Texto";
            cboTipoDoc.ValueMember = "Valor";
            cboTipoDoc.SelectedIndex = 0;

            txtFecha.Text = DateTime.Now.ToString("dd/MM/yyyy");

            txtIdProducto.Text = "0";
            txtCambio.Text = "";
            txtPagaCon.Text = "";
            txtTotalPagar.Text = "0";



        }

        private void btnBuscarProveedor_Click(object sender, EventArgs e)
        {
            using (var modal = new MdCliente())
            {
                var result = modal.ShowDialog();

                if (result == DialogResult.OK)
                {
                    txtNumDoc.Text = modal._Cliente.Documento.ToString();
                    txtNombreCompleto.Text = modal._Cliente.NombreCompleto;
                    txtCodProducto.Select();
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
                    txtPrecio.Text = modal._Producto.PrecioVenta.ToString("0.00");
                    txtStock.Text = modal._Producto.Stock.ToString();
                    txtcantidad.Select();

                }
                else
                    txtCodProducto.Select();
            }
        }

        private void txtCodProducto_KeyDown(object sender, KeyEventArgs e)
        {
            //si teclea enter entonces...
            if (e.KeyData == Keys.Enter)
            {
                //utilizamos el where para hacer una busqueda mas precisa con con SQL
                Producto oProducto = new CN_Producto().Listar().Where(p => p.Codigo == txtCodProducto.Text && p.estado == true).FirstOrDefault();

                if (oProducto != null)
                {
                    txtCodProducto.BackColor = System.Drawing.Color.Honeydew;
                    txtIdProducto.Text = oProducto.IdProducto.ToString();
                    txtProducto.Text = oProducto.Nombre;
                    txtPrecio.Text = oProducto.PrecioVenta.ToString("0.00");
                    txtStock.Text = oProducto.Stock.ToString();
                    txtcantidad.Select();
                }
                else
                {
                    txtCodProducto.BackColor = System.Drawing.Color.MistyRose;
                    txtIdProducto.Text = "0";
                    txtProducto.Text = "";
                    txtPrecio.Text = "";
                    txtStock.Text="";
                    txtcantidad.Value = 1;
                }
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            decimal precio = 0;
            // Verificamos el formato del número ingresado
            string precioText = txtPrecio.Text.Replace('.', ',');
            bool producto_existe = false;

            if (int.Parse(txtIdProducto.Text) == 0)
            {
                MessageBox.Show("Debe seleccionar un producto", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            //validamos el formato del precio
            if (!decimal.TryParse(precioText, out precio))
            {
                MessageBox.Show("Precio - Formato moneda incorrecto", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtPrecio.Select();
                return;
            }

            //validamos la cantidad del stock ingresado
            if (Convert.ToInt32(txtStock.Text) < Convert.ToInt32(txtcantidad.Value.ToString()))
            {
                MessageBox.Show("La cantidad no puede ser mayor al stock", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtPrecio.Select();
                return;
            }

            //validamos si existe el producto dentro de nuestro data grid view
            foreach (DataGridViewRow fila in dgvdataVenta.Rows)
            {
                // Verifica si la celda no es nula y luego compara
                if (fila.Cells["IdProducto"].Value.ToString() == txtIdProducto.Text)
                {
                    producto_existe = true;
                    break;
                }
            }

            if (!producto_existe)
            {
                dgvdataVenta.Rows.Add(new object[]
                {
                    txtIdProducto.Text,
                    txtProducto.Text,
                    precio.ToString("0.00"),
                    txtcantidad.Value.ToString(),
                    (txtcantidad.Value * precio).ToString("0.00")
                });
                calcularTotal();
                limpiarProducto();
                txtCodProducto.Select();
            }
        }
        private void calcularTotal()
        {
            decimal total = 0;
            //validamos si hay registros en nuestra grilla
            if (dgvdataVenta.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvdataVenta.Rows)
                {
                    total += Convert.ToDecimal(row.Cells["SubTotal"].Value.ToString());
                }
            }
            txtTotalPagar.Text = total.ToString("0.00");
        }
        private void limpiarProducto()
        {
            txtIdProducto.Text = "0";
            txtCodProducto.Text = "";
            txtCodProducto.BackColor = System.Drawing.Color.White;
            txtProducto.Text = "";
            txtPrecio.Text = "";
            txtStock.Text = "";
            txtcantidad.Value = 1;
        }

        private void dgvdataVenta_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            if (e.ColumnIndex == 6)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);
                //ancho de la imagen. aca uso el icono checkAzul que es mucho mas chico que la imagen "deleteicon"
                //de esta forma toma el tamaño de este icono
                var w = Properties.Resources.checkAzul.Width;
                var h = Properties.Resources.deleteIcon.Height;
                //limite del lado de la izquierda.    le restamos el ancho de nuestra imagen /2
                var x = e.CellBounds.Left + (e.CellBounds.Width - w) / 2;
                var y = e.CellBounds.Top + (e.CellBounds.Height - h) / 2;
                //aca dibujamos el icono "deleteicon" pero con el tamaño de checkAzul
                e.Graphics.DrawImage(Properties.Resources.deleteIcon, new Rectangle(x, y, w, h));
                e.Handled = true;
            }
        }

        private void dgvdataVenta_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //si se ha echo click en el boton seleccionar:
            if (dgvdataVenta.Columns[e.ColumnIndex].Name == "btnEliminar")
            {
                //almacenamos el indice de la fila q ha sido seleccionada
                int indice = e.RowIndex;

                if (indice >= 0)
                {
                    dgvdataVenta.Rows.RemoveAt(indice);
                    calcularTotal();
                }
            }
        }

        private void txtPrecio_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                //si el campo esta vacio o el primer caracter es un punto entonces...
                if (txtPrecio.Text.Trim().Length == 0 && e.KeyChar.ToString() == ".")
                {
                    //mi controlador que permite que se escriba o no, se activara entonces no se podra escribir
                    e.Handled = true;
                }
                else
                {
                    //si esta borrando o esta ecribiendo un punto...
                    if (char.IsControl(e.KeyChar) || e.KeyChar.ToString() == ".")
                    {
                        //entonces mi controlador sera falso
                        e.Handled = false;
                    }
                    else
                    {
                        e.Handled = true;
                    }
                }

            }
        }

        private void txtPagaCon_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                //si el campo esta vacio o el primer caracter es un punto entonces...
                if (txtPagaCon.Text.Trim().Length == 0 && e.KeyChar.ToString() == ".")
                {
                    //mi controlador que permite que se escriba o no, se activara entonces no se podra escribir
                    e.Handled = true;
                }
                else
                {
                    //si esta borrando o esta ecribiendo un punto...
                    if (char.IsControl(e.KeyChar) || e.KeyChar.ToString() == ".")
                    {
                        //entonces mi controlador sera falso
                        e.Handled = false;
                    }
                    else
                    {
                        e.Handled = true;
                    }
                }

            }
        }
        private void calcularCambio()
        {
            if(txtTotalPagar.Text.Trim() == "")
            {
                MessageBox.Show("No existen productos en la venta", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            decimal pagacon;
            decimal total = Convert.ToDecimal(txtTotalPagar.Text);

            if (txtPagaCon.Text.Trim() == "")
            {
                txtPagaCon.Text = "0";
            }
            if(decimal.TryParse(txtPagaCon.Text.Trim(), out pagacon))
            {
                if (pagacon < total)
                {
                    txtCambio.Text = "0.00";
                }
                else
                {
                    decimal cambio = pagacon - total;
                    txtCambio.Text = cambio.ToString("0.00");
                }
            }
        }

        private void txtPagaCon_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Enter)
            {
                calcularCambio();
            }
        }
    }
}
