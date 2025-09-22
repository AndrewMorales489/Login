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
using System.Reflection;

namespace Login
{
    public partial class Menu : Form
    {
        private readonly string connectionString = TryGetConnectionStringFromConfig("MiConexion")
                                                   ?? @"Data Source=Andrew;Initial Catalog=SistemaProductos;Integrated Security=True";

        public Menu()
        {
            InitializeComponent();
            this.Load += Menu_Load;
        }

        private static string TryGetConnectionStringFromConfig(string name)
        {
            try
            {
                var asm = AppDomain.CurrentDomain.GetAssemblies()
                           .FirstOrDefault(a => string.Equals(a.GetName().Name, "System.Configuration", StringComparison.OrdinalIgnoreCase));
                if (asm == null)
                {
                    // Intentar cargar el ensamblado del GAC/framework
                    asm = Assembly.Load("System.Configuration");
                }
                if (asm == null) return null;

                var cmType = asm.GetType("System.Configuration.ConfigurationManager");
                if (cmType == null) return null;

                var connStringsProp = cmType.GetProperty("ConnectionStrings", BindingFlags.Public | BindingFlags.Static);
                if (connStringsProp == null) return null;

                var connStringsObj = connStringsProp.GetValue(null);
                if (connStringsObj == null) return null;

                // Obtener el indexador Item[string] del tipo ConnectionStringSettingsCollection
                var itemProp = connStringsObj.GetType().GetProperty("Item", new[] { typeof(string) });
                if (itemProp == null) return null;

                var css = itemProp.GetValue(connStringsObj, new object[] { name });
                if (css == null) return null;

                var csProp = css.GetType().GetProperty("ConnectionString", BindingFlags.Public | BindingFlags.Instance);
                if (csProp == null) return null;

                return csProp.GetValue(css)?.ToString();
            }
            catch
            {
                // Si algo falla, devolvemos null y el código usará el fallback.
                return null;
            }
        }

        private void Menu_Load(object sender, EventArgs e)
        {
            // Mejora visual del DataGridView
            dgvProductos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvProductos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProductos.ReadOnly = true;

            CargarProductos(); // ← Aquí se llama el método que carga los datos
        }

        private void CargarProductos()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT IdProducto, Codigo, Nombre, Existencia, Estado, Proveedor FROM Productos";

                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();

                try
                {
                    conn.Open();
                    adapter.Fill(dt);
                    dgvProductos.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar productos: " + ex.Message);
                }
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvProductos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Crear instancia del formulario Productos
            using (var productosForm = new Productos())
            {
                var resultado = productosForm.ShowDialog();

                if (resultado == DialogResult.OK)
                {
                    CargarProductos(); // Recargar productos después de cerrar el formulario
                }
            }
        }

        private void dgvProductos_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dgvProductos.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecciona un producto para editar.");
                return;
            }

            DataRowView filaSeleccionada = dgvProductos.SelectedRows[0].DataBoundItem as DataRowView;
            if (filaSeleccionada == null)
            {
                MessageBox.Show("No se pudo obtener la fila seleccionada.");
                return;
            }

            // Suponiendo que tienes un formulario llamado Productos con un método CargarDatosParaEditar
            using (var productosForm = new Productos())
            {
                productosForm.CargarDatosParaEditar(filaSeleccionada.Row);

                var resultado = productosForm.ShowDialog();
                if (resultado == DialogResult.OK)
                {
                    CargarProductos(); // ← Recarga el DataGridView
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string nombre = txt_nombreproducto.Text;
            string codigo = txt_codigo.Text;
            string estado = cmb_estado.SelectedItem?.ToString();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                StringBuilder query = new StringBuilder("SELECT IdProducto, Codigo, Nombre, Existencia, Estado, Proveedor FROM Productos WHERE 1=1");

                if (!string.IsNullOrEmpty(codigo))
                    query.Append(" AND Codigo LIKE @codigo");

                if (!string.IsNullOrEmpty(nombre))
                    query.Append(" AND Nombre LIKE @nombre");

                if (!string.IsNullOrEmpty(estado) && estado != "Todos")
                    query.Append(" AND Estado = @estado");

                SqlCommand cmd = new SqlCommand(query.ToString(), conn);

                if (!string.IsNullOrEmpty(codigo))
                    cmd.Parameters.AddWithValue("@codigo", "%" + codigo + "%");

                if (!string.IsNullOrEmpty(nombre))
                    cmd.Parameters.AddWithValue("@nombre", "%" + nombre + "%");

                if (!string.IsNullOrEmpty(estado) && estado != "Todos")
                    cmd.Parameters.AddWithValue("@estado", estado);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                try
                {
                    conn.Open();
                    adapter.Fill(dt);
                    dgvProductos.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al buscar productos: " + ex.Message);
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (dgvProductos.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecciona un producto para eliminar.");
                return;
            }

            DataRowView filaSeleccionada = dgvProductos.SelectedRows[0].DataBoundItem as DataRowView;
            if (filaSeleccionada == null)
            {
                MessageBox.Show("No se pudo obtener la fila seleccionada.");
                return;
            }

            string nombreProducto = filaSeleccionada["Nombre"].ToString();
            int idProducto = Convert.ToInt32(filaSeleccionada["IdProducto"]);

            DialogResult confirmacion = MessageBox.Show(
                $"¿Estás seguro de que deseas eliminar el producto \"{nombreProducto}\"?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirmacion == DialogResult.Yes)
            {
                EliminarProducto(idProducto);
                CargarProductos(); // ← Recarga el DataGridView
            }
        }

        // Agrega este método dentro de la clase Menu
        private void EliminarProducto(int idProducto)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Productos WHERE IdProducto = @idProducto";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idProducto", idProducto);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar el producto: " + ex.Message);


                }
            }
        }
    }
}
