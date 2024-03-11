﻿using CapaEntidad;
using CapaNegocio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapaPresentacion
{
    public partial class Frm_Negocio : Form
    {
        public Frm_Negocio()
        {
            InitializeComponent();
        }

        //convertimos un array de byte en una imagen
        public Image byteToImage(byte[] imageBytes)
        {
            //nos permitira guardar imagenes
            MemoryStream ms = new MemoryStream();
            //recibe 3 parametros: el array de bytes, el indice de donde va a empezar a contar el array y el ultimo
            //es el total de bytes que tiene nuestra imagen
            ms.Write(imageBytes, 0, imageBytes.Length);
            //creamos la imagen. con bitmap hacemos la conversion de memorystream a image
            Image image = new Bitmap(ms);


            return image;
        }


        private void Frm_Negocio_Load(object sender, EventArgs e)
        {
            bool obtenido = true;
            byte[] byteimage = new CN_Negocio().ObtenerLogo(out obtenido);

            if (obtenido)
            {
                picLogo.Image = byteToImage(byteimage);
            }
            Negocio datos = new CN_Negocio().ObtenerDatos();
            txtNombre.Text = datos.Nombre;
            txtRUC.Text = datos.RUC;
            txtDireccion.Text = datos.Direccion;

        }

        private void btnSubir_Click(object sender, EventArgs e)
        {
            string mensaje = string.Empty;

            OpenFileDialog openfiledialog = new OpenFileDialog();
            openfiledialog.FileName = "Files|*.jpg;*.jpeg;*.png";

            if(openfiledialog.ShowDialog() == DialogResult.OK)
            {
                // convertimos la imagen en un array de byte
                byte[] byteimage = File.ReadAllBytes(openfiledialog.FileName);
                bool respuesta = new CN_Negocio().ActualizarLogo(byteimage, out mensaje);

                if (respuesta)
                {
                    picLogo.Image = byteToImage(byteimage);
                }
                else
                {
                    MessageBox.Show(mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void btnGuardarCambios_Click(object sender, EventArgs e)
        {
            string mensaje = string.Empty;

            Negocio obj = new Negocio()
            {
                Nombre = txtNombre.Text,
                RUC = txtRUC.Text,
                Direccion = txtDireccion.Text,
            };
            bool respuesta = new CN_Negocio().GuardarDatos(obj, out mensaje);

            if (respuesta)
            {
                MessageBox.Show("los cambios fueron guardados", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show("no se pudo guardar los cambios", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

        }
    }
}
