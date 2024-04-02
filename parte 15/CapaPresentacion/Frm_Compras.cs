using CapaEntidad;
using CapaNegocio;
using CapaPresentacion.Modales;
using CapaPresentacion.Utilidades;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
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

        private void txtCodProducto_KeyDown(object sender, KeyEventArgs e)
        {
            //si teclea enter entonces...
            if(e.KeyData == Keys.Enter)
            {
                //utilizamos el where para hacer una busqueda mas precisa con con SQL
                Producto oProducto = new CN_Producto().Listar().Where(p => p.Codigo == txtCodProducto.Text && p.estado == true).FirstOrDefault();    

                if(oProducto != null)
                {
                    txtCodProducto.BackColor = System.Drawing.Color.Honeydew;
                    txtIdProducto.Text = oProducto.IdProducto.ToString();
                    txtProducto.Text = oProducto.Nombre;
                    txtPrecioCompra.Select();
                }
                else
                {
                    txtCodProducto.BackColor = System.Drawing.Color.MistyRose;
                    txtIdProducto.Text = "0";
                    txtProducto.Text = "";
                }
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            decimal preciocompra = 0;
            decimal precioventa = 0;
            // Verificamos el formato del número ingresado
            string precioCompraText = txtPrecioCompra.Text.Replace('.', ',');
            string precioVentaText = txtPrecioVenta.Text.Replace('.', ',');
            bool producto_existe = false;

            if(int.Parse(txtIdProducto.Text) == 0)
            {
                MessageBox.Show("debe seleccionar un producto", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            //validamos el formato del precio de compra
            if(!decimal.TryParse(precioCompraText, out preciocompra))
            {
                MessageBox.Show("Precio compra - Formato moneda incorrecto", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtPrecioCompra.Select();
                return;
            }

            //validamos el formato del precio de venta
            if (!decimal.TryParse(precioVentaText, out precioventa))
            {
                MessageBox.Show("Precio venta - Formato moneda incorrecto", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtPrecioVenta .Select();
                return;
            }

            //validamos si existe el producto dentro de nuestro data grid view
            foreach (DataGridViewRow fila in dgvdata.Rows)
            {
                // Verifica si la celda no es nula y luego compara
                if (fila.Cells["IdProducto"].Value != null && fila.Cells["IdProducto"].Value.ToString() == txtIdProducto.Text)
                {
                    producto_existe = true;
                    break;
                }
            }

            if (!producto_existe)
            {
                dgvdata.Rows.Add(new object[]
                {
                    txtIdProducto.Text,
                    txtProducto.Text,
                    preciocompra.ToString("0.00"),
                    precioventa.ToString("0.00"),
                    txtcantidad.Value.ToString(),
                    (txtcantidad.Value * preciocompra).ToString("0.00")
                });
                calcularTotal();
                limpiarProducto();
                txtCodProducto.Select();
            }
        }
        private void limpiarProducto()
        {
            txtIdProducto.Text = "0";
            txtCodProducto.Text = "";
            txtCodProducto.BackColor = System.Drawing.Color.White;
            txtProducto.Text = "";
            txtPrecioCompra.Text = "";
            txtPrecioVenta.Text = "";
            txtcantidad.Value = 1;
        }

        private void calcularTotal()
        {
            decimal total=0;
            //validamos si hay registros en nuestra grilla
            if(dgvdata.Rows.Count>0) 
            {
                foreach(DataGridViewRow row in dgvdata.Rows)
                {
                    total += Convert.ToDecimal(row.Cells["SubTotal"].Value.ToString());
                }
            }
            txtTotalPagar.Text = total.ToString("0.00");
        }

        private void dgvdata_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            if (e.ColumnIndex == 6)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);
                //ancho de la imagen. aca uso el icono checkAzul que es mucho mas chico que la imagen "deleteicon"
                //de esta forma toma el tamaño de este icono
                var w = Properties.Resources.checkAzul.Width;
                var h = Properties.Resources.checkAzul.Height;
                //limite del lado de la izquierda.    le restamos el ancho de nuestra imagen /2
                var x = e.CellBounds.Left + (e.CellBounds.Width - w) / 2;
                var y = e.CellBounds.Top + (e.CellBounds.Height - h) / 2;
                //aca dibujamos el icono "deleteicon" pero con el tamaño de checkAzul
                e.Graphics.DrawImage(Properties.Resources.deleteIcon, new Rectangle(x, y, w, h));
                e.Handled = true;
            }
        }

        private void dgvdata_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //si se ha echo click en el boton seleccionar:
            if (dgvdata.Columns[e.ColumnIndex].Name == "btnEliminar")
            {
                //almacenamos el indice de la fila q ha sido seleccionada
                int indice = e.RowIndex;

                if (indice >= 0)
                {
                    dgvdata.Rows.RemoveAt(indice);
                    calcularTotal();
                    

                    

                }
            }
        }

        private void txtPrecioCompra_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                //si el campo esta vacio o el primer caracter es un punto entonces...
                if(txtPrecioCompra.Text.Trim().Length == 0 && e.KeyChar.ToString() == ".")
                {
                    //mi controlador que permite que se escriba o no, se activara entonces no se podra escribir
                    e.Handled=true;
                }
                else
                {
                    //si esta borrando o esta ecribiendo un punto...
                    if(char.IsControl(e.KeyChar) || e.KeyChar.ToString() == ".") 
                    {
                        //entonces mi controlador sera falso
                        e.Handled=false;
                    }
                    else
                    {
                        e.Handled = true;
                    }
                }

            }
        }

        private void txtPrecioVenta_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                //si el campo esta vacio o el primer caracter es un punto entonces...
                if (txtPrecioVenta.Text.Trim().Length == 0 && e.KeyChar.ToString() == ".")
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

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            if(Convert.ToInt32(txtIdProveedor.Text) == 0)
            {
                MessageBox.Show("Debe seleccionar un proveedor", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            //si la cantidad de filas que hay es menor a 1
            if(dgvdata.Rows.Count < 1) 
            {
                MessageBox.Show("Debe seleccionar un proveedor", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            DataTable detalleCompra = new DataTable();

            detalleCompra.Columns.Add("IdProducto", typeof(int));
            detalleCompra.Columns.Add("PrecioCompra", typeof(decimal));
            detalleCompra.Columns.Add("PrecioVenta", typeof(decimal));
            detalleCompra.Columns.Add("Cantidad", typeof(int));
            detalleCompra.Columns.Add("MontoTotal", typeof(decimal));

            //registramos todos los productos en el data table. leemos cada fila de nuesto datagridview
            foreach(DataGridViewRow row in dgvdata.Rows)
            {
                detalleCompra.Rows.Add(
                    new object[]
                    {
                        Convert.ToInt32(row.Cells["IdProducto"].Value.ToString()),
                        row.Cells["PrecioCompra"].Value.ToString(),
                        row.Cells["PrecioVenta"].Value.ToString(),
                        row.Cells["Cantidad"].Value.ToString(),
                        row.Cells["SubTotal"].Value.ToString(),
                    });    
            }
            int idCorrelativo = new CN_Compra().ObtenerCorrelativo();
            string numeroDocumento = string.Format("{0:00000}", idCorrelativo);

            Compra oCompra = new Compra()
            {
                oUsuario = new Usuario() {IdUsuario = _usuario.IdUsuario},
                oProveedor = new Proveedor() {IdProveedor = Convert.ToInt32(txtIdProveedor.Text) },
                TipoDocumento = ((OpcionCombo)cboTipoDoc.SelectedItem).Texto,
                NumeroDocumento = numeroDocumento,
                MontoTotal = Convert.ToDecimal(txtTotalPagar.Text),

            };
            string mensaje = string.Empty;
            bool respuesta = new CN_Compra().Registrar(oCompra,detalleCompra, out mensaje);

            if (respuesta)
            {
                var result = MessageBox.Show("Numero de compra generada:\n" + numeroDocumento + "\n\n¿Desea copiar al portapapeles?"
                    ,"Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if(result == DialogResult.Yes)
                    Clipboard.SetText(numeroDocumento);

                txtIdProveedor.Text = "0";
                txtNumDoc.Text = "";
                txtRazonSocial.Text = "";
                dgvdata.Rows.Clear();
                calcularTotal();

            }
            else
            {
                MessageBox.Show(mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }
    }
}
