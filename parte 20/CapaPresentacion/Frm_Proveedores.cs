using CapaEntidad;
using CapaNegocio;
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
    public partial class Frm_Proveedores : Form
    {
        public Frm_Proveedores()
        {
            InitializeComponent();
        }

        private void Frm_Proveedores_Load(object sender, EventArgs e)
        {
            //cargamos el desplegable "Estado"
            cboEstado.Items.Add(new OpcionCombo() { Valor = 1, Texto = "Activo" });
            cboEstado.Items.Add(new OpcionCombo() { Valor = 0, Texto = "No Activo" });
            cboEstado.DisplayMember = "Texto";
            cboEstado.ValueMember = "Valor";
            cboEstado.SelectedIndex = 0;


            foreach (DataGridViewColumn columna in dgvData.Columns)
            {
                if (columna.Visible == true && columna.Name != "BtnSeleccionar")
                {
                    cboBusqueda.Items.Add(new OpcionCombo() { Valor = columna.Name, Texto = columna.HeaderText });
                }
            }

            cboBusqueda.DisplayMember = "Texto";
            cboBusqueda.ValueMember = "Valor";
            cboBusqueda.SelectedIndex = 0;

            List<Proveedor> lista = new CN_Proveedor().Listar();
            foreach (Proveedor item in lista)
            {
                dgvData.Rows.Add(new object[] { "",
                    item.IdProveedor,
                    item.Documento,
                    item.RazonSocial,
                    item.Correo,
                    item.Telefono,
                    item.Estado == true ? 1 : 0,
                    item.Estado == true ? "Activo" : "No Activo"
                });
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string mensaje = string.Empty;

            //cargamos el objeto "objproveedor" con los textos ingresados por el usuario
            Proveedor objProveedor = new Proveedor()
            {
                IdProveedor = Convert.ToInt32(txtId.Text),
                Documento = txtDocumento.Text,
                RazonSocial = txtRazonSocial.Text,
                Correo= txtCorreo.Text,
                Telefono = txtTelefono.Text,
                Estado = Convert.ToInt32(((OpcionCombo)cboEstado.SelectedItem).Valor) == 1 ? true : false
            };

            //cuando va a registrar y cuando va a editar:
            //el txtId siempre es 0 salvo cuando seleccionamos un usuario de la grilla

            //tenemos un Proveedor nuevo
            if (objProveedor.IdProveedor == 0)
            {
                //                                                      salida
                int idgenerado = new CN_Proveedor().Registrar(objProveedor, out mensaje);

                if (idgenerado != 0)
                {
                    //agregamos una nueva fila a la grilla, le pasamos un objeto y completamos segun el orden de las columnas.
                    //para los desplegables seleccionamos el objeto, lo convertimos a tipo de dato q necesitamos(object) y accedemos a su valor
                    dgvData.Rows.Add(new object[] {"",idgenerado,
                        txtDocumento.Text,
                        txtRazonSocial.Text,
                        txtCorreo.Text,
                        txtTelefono.Text,
                    ((OpcionCombo)cboEstado.SelectedItem).Valor.ToString(),
                    ((OpcionCombo)cboEstado.SelectedItem).Texto.ToString(),

                    });

                    Limpiar();
                }
                else
                {
                    MessageBox.Show(mensaje);
                }
            }
            else
            {
                // si el idproveedor no es cero entonces vamos a editar
                bool resultado = new CN_Proveedor().Editar(objProveedor, out mensaje);

                if (resultado)
                {
                    //para editar la fila necesitamos el numero de la fila.
                    //para eso tenemos el txtindice

                    // fila de tipo datagridview.  en el corchete le pasamos el indice de la fila q queremos
                    DataGridViewRow row = dgvData.Rows[Convert.ToInt32(txtIndice.Text)];
                    //le pasamos la columna que queremos editar
                    row.Cells["Id"].Value = txtId.Text;
                    row.Cells["Documento"].Value = txtDocumento.Text;
                    row.Cells["RazonSocial"].Value = txtRazonSocial.Text;
                    row.Cells["Correo"].Value = txtCorreo.Text;
                    row.Cells["Telefono"].Value = txtTelefono.Text;
                    row.Cells["EstadoValor"].Value = ((OpcionCombo)cboEstado.SelectedItem).Valor.ToString();
                    row.Cells["Estado"].Value = ((OpcionCombo)cboEstado.SelectedItem).Texto.ToString();

                    Limpiar();
                }
                else
                {
                    MessageBox.Show(mensaje);
                }

            }
        }

        private void Limpiar()
        {
            txtIndice.Text = "-1";
            txtId.Text = "0";
            txtDocumento.Text = "";
            txtRazonSocial.Text = "";
            txtCorreo.Text = "";
            txtTelefono.Text = "";
            cboEstado.SelectedIndex = 0;

            //el puntero se situa en el textbox documento dsp de cargar algun dato
            txtDocumento.Select();

        }

        private void dgvData_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            if (e.ColumnIndex == 0)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);
                //ancho de la imagen. aca uso el icono delete que es mucho mas chico que la imagen "checkAzul"
                //de esta forma toma el tamaño de este icono
                var w = Properties.Resources.checkAzul.Width;
                var h = Properties.Resources.checkAzul.Height;
                //limite del lado de la izquierda.    le restamos el ancho de nuestra imagen /2
                var x = e.CellBounds.Left + (e.CellBounds.Width - w) / 2;
                var y = e.CellBounds.Top + (e.CellBounds.Height - h) / 2;
                //aca dibujamos el icono "checkAzul" pero con el tamaño de delete icon
                e.Graphics.DrawImage(Properties.Resources.checkAzul, new Rectangle(x, y, w, h));
                e.Handled = true;
            }
        }

        private void dgvData_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //si se ha echo click en el boton seleccionar:
            if (dgvData.Columns[e.ColumnIndex].Name == "BtnSeleccionar")
            {
                //almacenamos el indice de la fila q ha sido seleccionada
                int indice = e.RowIndex;

                if (indice >= 0)
                {

                    txtIndice.Text = indice.ToString();
                    //nos quedamos con los datos de la columna especifica de esa fila que estamos seleccionando. Le pasamos el indice y el nombre de la columna
                    txtId.Text = dgvData.Rows[indice].Cells["Id"].Value.ToString();
                    txtDocumento.Text = dgvData.Rows[indice].Cells["Documento"].Value.ToString();
                    txtRazonSocial.Text = dgvData.Rows[indice].Cells["RazonSocial"].Value.ToString();
                    txtCorreo.Text = dgvData.Rows[indice].Cells["Correo"].Value.ToString();
                    txtTelefono.Text = dgvData.Rows[indice].Cells["Telefono"].Value.ToString();

                    foreach (OpcionCombo oc in cboEstado.Items)
                    {
                        if (Convert.ToInt32(oc.Valor) == Convert.ToInt32(dgvData.Rows[indice].Cells["EstadoValor"].Value))
                        {
                            int indice_combo = cboEstado.Items.IndexOf(oc);
                            cboEstado.SelectedIndex = indice_combo;
                            break;

                        }
                    }

                }
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txtId.Text) != 0)
            {
                //mensaje; Titulo del mensaje; tipo de botones q quiero q tenga el mensaje(si,no); icono del mensaje;  si presiona "yes" entonces....                        
                if (MessageBox.Show("¿desea eliminar este Proveedor?", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string mensaje = string.Empty;
                    Proveedor objProveedor = new Proveedor()
                    {
                        IdProveedor = Convert.ToInt32(txtId.Text)
                    };

                    bool resultado = new CN_Proveedor().Eliminar(objProveedor, out mensaje);

                    if (resultado)
                    {
                        dgvData.Rows.RemoveAt(Convert.ToInt32(txtIndice.Text));
                    }
                    else
                    {
                        MessageBox.Show(mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            string columnaFiltro = ((OpcionCombo)cboBusqueda.SelectedItem).Valor.ToString();

            //si existen filas en nuestra grilla
            if (dgvData.Rows.Count > 0)
            {
                //recorremos cada fila de la grilla 
                foreach (DataGridViewRow row in dgvData.Rows)
                {
                    //filtramos: de la columna seleccionada, retorna el valor, limpia los espacios del principio y del final,
                    // pasalo a mayusculas, tiene que contener lo que tiene la caja de texto(esto tambien lo pasamos a mayuscula
                    // y eliminamos espacios)
                    if (row.Cells[columnaFiltro].Value.ToString().Trim().ToUpper().Contains(txtBusqueda.Text.Trim().ToUpper()))
                        row.Visible = true;
                    else
                        row.Visible = false;
                }
            }
        }

        private void btnLimpiarBuscador_Click(object sender, EventArgs e)
        {
            txtBusqueda.Text = "";
            foreach (DataGridViewRow row in dgvData.Rows)
            {
                row.Visible = true;
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            Limpiar();
        }
    }
}
