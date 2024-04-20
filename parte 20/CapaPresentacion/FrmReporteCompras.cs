using CapaEntidad;
using CapaNegocio;
using CapaPresentacion.Utilidades;
using ClosedXML.Excel;
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
    public partial class FrmReporteCompras : Form
    {

        public FrmReporteCompras()
        {
            InitializeComponent();
        }

        private void FrmReporteCompras_Load(object sender, EventArgs e)
        {
            List<Proveedor> lista = new CN_Proveedor().Listar();

            cboProveedor.Items.Add(new OpcionCombo() { Valor = 0, Texto = "TODOS" });
            foreach(Proveedor item in lista)
            {
                cboProveedor.Items.Add(new OpcionCombo() { Valor = item.IdProveedor, Texto = item.RazonSocial });
            }
            cboProveedor.DisplayMember = "Texto";
            cboProveedor.ValueMember = "Valor";
            cboProveedor.SelectedIndex = 0;

            foreach(DataGridViewColumn column in dgvDataReporte.Columns)
            {
                cboBusqueda.Items.Add(new OpcionCombo() { Valor = column.Name, Texto = column.HeaderText });
            }
            cboBusqueda.DisplayMember = "Texto";
            cboBusqueda.ValueMember = "Valor";
            cboBusqueda.SelectedIndex = 0;

        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            string columnaFiltro = ((OpcionCombo)cboBusqueda.SelectedItem).Valor.ToString();

            //si existen filas en nuestra grilla
            if (dgvDataReporte.Rows.Count > 0)
            {
                //recorremos cada fila de la grilla 
                foreach (DataGridViewRow row in dgvDataReporte.Rows)
                {
                    //filtramos: de la columna seleccionada, retorna el valor, limpia los espacios del principio y del final,
                    // pasamo a mayusculas, tiene que contener lo que tiene la caja de texto(esto tambien lo pasamos a mayuscula
                    // y eliminamos espacios)
                    if (row.Cells[columnaFiltro].Value.ToString().Trim().ToUpper().Contains(txtBusqueda.Text.Trim().ToUpper()))
                        row.Visible = true;
                    else
                        row.Visible = false;
                }
            }

        }

        private void btnBuscarProveedor_Click(object sender, EventArgs e)
        {
            int idproveedor = Convert.ToInt32(((OpcionCombo)cboProveedor.SelectedItem).Valor.ToString());

            List<ReporteCompra> lista = new List<ReporteCompra>();


            lista = new CN_Reporte().Compra(
                txtFechaInicio.Value.ToString(),
                txtFechaFin.Value.ToString(),
                idproveedor
                );

            dgvDataReporte.Rows.Clear();
            foreach (ReporteCompra rc in lista)
            {
                dgvDataReporte.Rows.Add(new object[]
                {
                    rc.FechaRegistro,
                    rc.TipoDocumento,
                    rc.NumeroDocumento,
                    rc.MontoTotal,
                    rc.UsuarioRegistrado,
                    rc.DocumentoProveedor,
                    rc.RazonSocial,
                    rc.CodigoProducto,
                    rc.NombreProducto,
                    rc.Categoria,
                    rc.PrecioCompra,
                    rc.PrecioVenta,
                    rc.Cantidad,
                    rc.SubTotal
                });

            }
        }

        private void btnExel_Click(object sender, EventArgs e)
        {
            //si no hay registros en el data grid view no nos genera el excel
            if (dgvDataReporte.Rows.Count < 1)
            {
                MessageBox.Show("no hay datos para exportar", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                //si hay filas para exportar, insertamos todos los datos en el datatable
                DataTable dt = new DataTable();

                //insertamos columnas
                foreach (DataGridViewColumn columna in dgvDataReporte.Columns)
                {
                    dt.Columns.Add(columna.HeaderText, typeof(string));
                }

                //insertamos filas
                foreach (DataGridViewRow row in dgvDataReporte.Rows)
                {
                    //si la fila es visible
                    if (row.Visible)
                    {
                        dt.Rows.Add(new object[]
                        {
                            row.Cells[0].Value.ToString(),
                            row.Cells[1].Value.ToString(),
                            row.Cells[2].Value.ToString(),
                            row.Cells[3].Value.ToString(),
                            row.Cells[4].Value.ToString(),
                            row.Cells[5].Value.ToString(),
                            row.Cells[6].Value.ToString(),
                            row.Cells[7].Value.ToString(),
                            row.Cells[8].Value.ToString(),
                            row.Cells[9].Value.ToString(),
                            row.Cells[10].Value.ToString(),
                            row.Cells[11].Value.ToString(),
                            row.Cells[12].Value.ToString(),
                            row.Cells[13].Value.ToString(),

                        });
                    }
                }
                //
                SaveFileDialog savefile = new SaveFileDialog();
                //nombre con el que se va a guardar nuestro archivo excel     
                savefile.FileName = string.Format("ReporteCompras_{0}.xlsx", DateTime.Now.ToString("ddMMyyyyHHmmss"));
                //filtro para que se muestren solo archivos del mismo tipo(extension xlsx)
                savefile.Filter = "Excel Files | *.xlsx";

                if (savefile.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        //creamos el archivo excel
                        XLWorkbook wb = new XLWorkbook();
                        //agregamos una hoja           nombre de la hoja
                        var hoja = wb.Worksheets.Add(dt, "Informe");
                        //ajustamos el ancho de las columnas segun el valor que tengan
                        hoja.ColumnsUsed().AdjustToContents();
                        //guardamos con la ruta que seleccionamos
                        wb.SaveAs(savefile.FileName);
                        MessageBox.Show("Reporte Generado", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch
                    {
                        MessageBox.Show("Error al generar el Reporte", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }

            }
        }

        private void btnLimpiarBuscador_Click(object sender, EventArgs e)
        {
            txtBusqueda.Text = "";
            foreach (DataGridViewRow row in dgvDataReporte.Rows)
            {
                row.Visible = true;
            }
        }
    }
}
