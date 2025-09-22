using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Login
{


    public partial class Productos : Form
    {
        public Productos()
        {
            InitializeComponent();

            // Asocia los eventos KeyDown para los TextBox
            txt_Codigo.KeyDown += txt_Codigo_KeyDown;
            txt_Nombre.KeyDown += txt_Nombre_KeyDown;
            txt_Existencia.KeyDown += txt_Existencia_KeyDown;
            cmb_Estado.KeyDown += cmb_Estado_KeyDown;
            txt_Proveedor.KeyDown += txt_Proveedor_KeyDown;
        }
        public int? IdProductoEditar { get; set; } = null;

        private void Productos_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
        private readonly string connectionString = @"Data Source=Andrew;Initial Catalog=SistemaProductos;Integrated Security=True";

        private void btn_Guardar_Click(object sender, EventArgs e)
        {
            string query;
            // Obtener los valores de los controles del formulario
            string codigo = txt_Codigo.Text.Trim();
            string nombre = txt_Nombre.Text.Trim();
            int existencia = 0;
            int.TryParse(txt_Existencia.Text.Trim(), out existencia);
            string estado = cmb_Estado.SelectedItem?.ToString() ?? "";
            string proveedor = txt_Proveedor.Text.Trim();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd;

                if (IdProductoEditar == null)
                {
                    // INSERTAR nuevo producto
                    query = @"
        INSERT INTO Productos (Codigo, Nombre, Existencia, Estado, Proveedor)
        VALUES (@codigo, @nombre, @existencia, @estado, @proveedor)";
                    cmd = new SqlCommand(query, conn);
                }
                else
                {
                    // ACTUALIZAR producto existente
                    query = @"
        UPDATE Productos
        SET Codigo = @codigo,
            Nombre = @nombre,
            Existencia = @existencia,
            Estado = @estado,
            Proveedor = @proveedor
        WHERE IdProducto = @id";
                    cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", IdProductoEditar.Value);
                }

                // Parámetros comunes
                cmd.Parameters.AddWithValue("@codigo", codigo);
                cmd.Parameters.AddWithValue("@nombre", nombre);
                cmd.Parameters.AddWithValue("@existencia", existencia);
                cmd.Parameters.AddWithValue("@estado", estado);
                cmd.Parameters.AddWithValue("@proveedor", proveedor);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("✅ Producto guardado correctamente");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al guardar: " + ex.Message);
                }
            }
        }

        // Agrega este método público a la clase Productos
        public void CargarDatosParaEditar(DataRow row)
        {
            if (row == null) return;

            // Asumiendo que los nombres de las columnas son los mismos que en la consulta SQL
            txt_Codigo.Text = row["Codigo"]?.ToString();
            txt_Nombre.Text = row["Nombre"]?.ToString();
            txt_Existencia.Text = row["Existencia"]?.ToString();
            cmb_Estado.SelectedItem = row["Estado"]?.ToString();
            txt_Proveedor.Text = row["Proveedor"]?.ToString();


        }
        private void LimpiarCampos()
        {
            txt_Codigo.Text = "";
            txt_Nombre.Text = "";
            txt_Existencia.Text = "";
            cmb_Estado.SelectedIndex = -1;
            txt_Proveedor.Text = "";
            txt_Codigo.Focus();
        }
        private void btn_Borrar_Click(object sender, EventArgs e)
        {
            DialogResult confirmacion = MessageBox.Show(
        "¿Deseas borrar todos los datos ingresados?",
        "Confirmar limpieza",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question
    );

            if (confirmacion == DialogResult.Yes)
            {
                LimpiarCampos();
            }
        }

        private void btn_salir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Métodos para manejar el Enter y pasar el foco al siguiente control

        private void txt_Codigo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txt_Nombre.Focus();
                e.SuppressKeyPress = true;
            }
        }

        private void txt_Nombre_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txt_Existencia.Focus();
                e.SuppressKeyPress = true;
            }
        }

        private void txt_Existencia_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                cmb_Estado.Focus();
                e.SuppressKeyPress = true;
            }
        }

        private void cmb_Estado_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txt_Proveedor.Focus();
                e.SuppressKeyPress = true;
            }
        }

        private void txt_Proveedor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btn_Guardar.Focus(); // O btn_Guardar.PerformClick();
                e.SuppressKeyPress = true;
            }
        }
    }
}

