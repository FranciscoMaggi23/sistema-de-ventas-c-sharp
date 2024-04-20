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
using System.IO;
using DocumentFormat.OpenXml.Wordprocessing;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;


namespace CapaPresentacion
{
    public partial class Frm_DetalleCompra : Form
    {
        public Frm_DetalleCompra()
        {
            InitializeComponent();
        }

        private void btnBuscarProveedor_Click(object sender, EventArgs e)
        {
            Compra oCompra = new CN_Compra().ObtenerCompra(txtBusqueda.Text);
            if(oCompra.IdCompra != 0)
            {
                txtNumDocHidden.Text = oCompra.NumeroDocumento;

                txtFecha.Text = oCompra.FechaRegistro;
                txtTipoDoc.Text = oCompra.TipoDocumento;
                txtUsuario.Text = oCompra.oUsuario.NombreCompleto;
                txtDocProveedor.Text = oCompra.oProveedor.Documento;
                txtRazonSocial.Text = oCompra.oProveedor.RazonSocial;

                dgvData.Rows.Clear();
                foreach(Detalle_Compra dc in oCompra.oDetalleCompra)
                {
                    dgvData.Rows.Add(new object[] { dc.oProducto.Nombre, dc.PrecioCompra, dc.Cantidad, dc.MontoTotal });
                }
                txtMontoTotal.Text = oCompra.MontoTotal.ToString("0.00");

            }
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            txtFecha.Text = "";
            txtTipoDoc.Text = "";
            txtUsuario.Text = "";
            txtDocProveedor.Text = "";
            txtRazonSocial.Text = "";

            dgvData.Rows.Clear();
            txtMontoTotal.Text="0.00";
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            if(txtTipoDoc.Text == "")
            {
                MessageBox.Show("No se encontraron resultados","Mensaje",MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            //lectura del texto HTML
            //convertimos la plantilla en un texto y la almacenamos en la variable
            string Texto_HTML = Properties.Resources.PlantillaCompra.ToString();
            Negocio oDatos = new CN_Negocio().ObtenerDatos();

            //reemplazamos textos de ciertas partes de la plantilla por los textos cargados
            Texto_HTML = Texto_HTML.Replace("@nombrenegocio", oDatos.Nombre.ToUpper());
            Texto_HTML = Texto_HTML.Replace("@docnegocio", oDatos.RUC);
            Texto_HTML = Texto_HTML.Replace("@direcnegocio", oDatos.Direccion);

            Texto_HTML = Texto_HTML.Replace("@tipodocumento", txtTipoDoc.Text.ToUpper());
            Texto_HTML = Texto_HTML.Replace("@numerodocumento", txtNumDocHidden.Text);

            Texto_HTML = Texto_HTML.Replace("@docproveedor", txtDocProveedor.Text);
            Texto_HTML = Texto_HTML.Replace("@nombreproveedor", txtRazonSocial.Text);
            Texto_HTML = Texto_HTML.Replace("@fecharegistro", txtFecha.Text);
            Texto_HTML = Texto_HTML.Replace("@usuarioregistro", txtUsuario.Text);

            //sector de la lista
            string filas = string.Empty;
            foreach(DataGridViewRow row in dgvData.Rows)
            {
                filas += "<tr>";
                filas += "<td>" + row.Cells["Producto"].Value.ToString() + "</td>";
                filas += "<td>" + row.Cells["PrecioCompra"].Value.ToString() + "</td>";
                filas += "<td>" + row.Cells["Cantidad"].Value.ToString() + "</td>";
                filas += "<td>" + row.Cells["SubTotal"].Value.ToString() + "</td>";
                filas += "</tr>";
            }
            Texto_HTML = Texto_HTML.Replace("@filas", filas);
            Texto_HTML = Texto_HTML.Replace("@montototal", txtMontoTotal.Text);

            //abrimos la ventana de dialogo para guardar el archivo
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.FileName = string.Format("Compra_{0}.pdf", txtNumDocHidden.Text);
            saveFile.Filter = "Pdf Files|*.pdf";

            if(saveFile.ShowDialog() == DialogResult.OK)
            {
                //creamos archivos en memoria con fileStream       ruta,        crear archivo
                using (FileStream stream = new FileStream(saveFile.FileName, FileMode.Create))
                {
                    iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4,25,25,25,25);

                    PdfWriter write = PdfWriter.GetInstance(pdfDoc, stream);
                    pdfDoc.Open();

                    //insertamos el logo
                    bool obtenido = true;
                    byte[] byteImage = new CN_Negocio().ObtenerLogo(out obtenido);

                    if(obtenido)
                    {
                        iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(byteImage);
                        img.ScaleToFit(60, 60); //60 px, 60 px
                        //alineamos la imagen sobre el texto
                        img.Alignment = iTextSharp.text.Image.UNDERLYING;
                        //le pasamos la posicion x,y donde estara pegada la imagen
                        img.SetAbsolutePosition(pdfDoc.Left, pdfDoc.GetTop(51));
                        pdfDoc.Add(img);
                    }

                    using (StringReader sr = new StringReader(Texto_HTML))
                    {
                        XMLWorkerHelper.GetInstance().ParseXHtml(write, pdfDoc, sr);
                    }

                    pdfDoc.Close();
                    stream.Close();
                    MessageBox.Show("Documento Generado", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
            }

        }
    }
}
