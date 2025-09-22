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
    public partial class Registro : Form
    {
        private readonly string connectionString;

        public Registro()
        {
            InitializeComponent();
            connectionString = GetConnectionString();

            // Asocia los eventos KeyDown
            txt_nombre.KeyDown += txt_nombre_KeyDown;
            txt_apellido.KeyDown += txt_apellido_KeyDown;
            txt_usuario.KeyDown += txt_usuario_KeyDown;
            txt_contraseña.KeyDown += txt_contraseña_KeyDown;
            txt_correo.KeyDown += txt_correo_KeyDown;
            txt_telefono.KeyDown += txt_telefono_KeyDown;
        }

        private static string GetConnectionString()
        {
            try
            {
                
                var cmType = Type.GetType("System.Configuration.ConfigurationManager, System.Configuration");
                if (cmType != null)
                {
                    var connStringsProp = cmType.GetProperty("ConnectionStrings", BindingFlags.Static | BindingFlags.Public);
                    var connStringsObj = connStringsProp?.GetValue(null, null);
                    if (connStringsObj != null)
                    {
                        // Obtener el indexador .Item["MiConexion"]     
                        var itemProp = connStringsObj.GetType().GetProperty("Item", new[] { typeof(string) });
                        var entryObj = itemProp?.GetValue(connStringsObj, new object[] { "MiConexion" });
                        if (entryObj != null)
                        {
                            var connStringProp = entryObj.GetType().GetProperty("ConnectionString");
                            var value = connStringProp?.GetValue(entryObj) as string;
                            if (!string.IsNullOrEmpty(value))
                                return value;
                        }
                    }
                }
            }
            catch
            {
                
            }

            
            var env = Environment.GetEnvironmentVariable("MiConexion");
            if (!string.IsNullOrEmpty(env))
                return env;

            // Fallback 2: cadena por defecto (modificar según sea necesario)
            return @"Data Source=Andrew;Initial Catalog=SistemaProductos;Integrated Security=True;";
        }

        private void LimpiarCampos()
        {
            txt_nombre.Text = "";
            txt_apellido.Text = "";
            txt_usuario.Text = "";
            txt_contraseña.Text = "";
            txt_correo.Text = "";   
            txt_telefono.Text = ""; 
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_guardar_Click(object sender, EventArgs e)
        {
            string nombre = txt_nombre.Text;
            string apellido = txt_apellido.Text;
            string usuario = txt_usuario.Text;
            string contraseña = txt_contraseña.Text;
            string correo = txt_correo.Text;
            string telefono = txt_telefono.Text;

            if (nombre == "" || apellido == "" || usuario == "" || contraseña == "" || correo == "" || telefono == "")
            {
                MessageBox.Show("Por favor completa todos los campos.");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
        INSERT INTO Usuarios (NombreUsuario, Contraseña, Nombres, Apellido, Correo, Telefono)
        VALUES (@usuario, @contraseña, @nombres, @apellido, @correo, @telefono)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@usuario", usuario);
                cmd.Parameters.AddWithValue("@contraseña", contraseña);
                cmd.Parameters.AddWithValue("@nombres", nombre);
                cmd.Parameters.AddWithValue("@apellido", apellido);
                cmd.Parameters.AddWithValue("@correo", correo);
                cmd.Parameters.AddWithValue("@telefono", telefono);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("✅ Usuario registrado correctamente");
                     // Opcional: limpia los campos después
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2627) // Violación de restricción UNIQUE
                        MessageBox.Show("❌ El usuario o correo ya está registrado.");
                    else
                        MessageBox.Show("Error al registrar: " + ex.Message);
                }
            }
        }

        private void btn_borrar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
            MessageBox.Show("Campos limpiados Correctamente.");
        }

        // Corrigiendo los métodos de eventos de teclado y eliminando el bloque anidado incorrecto
        private void txt_usuario_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txt_contraseña.Focus();
                e.SuppressKeyPress = true;
            }
        }

        private void txt_contraseña_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txt_correo.Focus();
                e.SuppressKeyPress = true;
            }
        }
            
        private void txt_correo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txt_telefono.Focus();
                e.SuppressKeyPress = true;
            }
        }

        private void txt_telefono_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btn_guardar.Focus(); // O btn_guardar.PerformClick();
                e.SuppressKeyPress = true;
            }
        }

        private void txt_nombre_T(object sender, EventArgs e)
        {
            // Puedes dejarlo vacío o agregar lógica según sea necesario
        }

        private void txt_nombre_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txt_apellido.Focus();
                e.SuppressKeyPress = true;
            }
        }

        private void txt_apellido_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txt_usuario.Focus();
                e.SuppressKeyPress = true;
            }
        }
    }
}
