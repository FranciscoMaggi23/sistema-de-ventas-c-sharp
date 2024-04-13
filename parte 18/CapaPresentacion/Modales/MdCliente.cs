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

namespace CapaPresentacion.Modales
{
    public partial class MdCliente : Form
    {
        public Cliente _Cliente {  get; set; }
        public MdCliente()
        {
            InitializeComponent();
        }

        private void MdCliente_Load(object sender, EventArgs e)
        {
            foreach (DataGridViewColumn columna in dgvData.Columns)
            {
                    cboBusqueda.Items.Add(new OpcionCombo() { Valor = columna.Name, Texto = columna.HeaderText });
            }

            cboBusqueda.DisplayMember = "Texto";
            cboBusqueda.ValueMember = "Valor";
            cboBusqueda.SelectedIndex = 0;

            List<Cliente> listaProducto = new CN_Cliente().Listar();

            foreach (Cliente item in listaProducto)
            {
                if (item.Estado)
                {
                    dgvData.Rows.Add(new object[] {item.Documento,
                    item.NombreCompleto});
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

        private void dgvData_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //guardamos el indice de la fila y la columna seleccionada
            int iRow = e.RowIndex;
            int iColum = e.ColumnIndex;

            if (iRow >= 0 && iColum >= 0)
            {
                _Cliente = new Cliente()
                {
                    Documento = dgvData.Rows[iRow].Cells["NumeroDocumento"].Value.ToString(),
                    NombreCompleto = dgvData.Rows[iRow].Cells["NombreCompleto"].Value.ToString(),
                };
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
