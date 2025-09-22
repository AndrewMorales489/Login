using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Xaml;
namespace Login
{
    public partial class Form1 : Form
    {
        private readonly string connectionString = "server=Andrew;DataBase=SistemaProductos;Trusted_Connection=true";

        public Form1()
        {
            InitializeComponent();
            txt_Usuario.KeyDown += new KeyEventHandler(txt_Usuario_KeyDown);
            txt_Contraseña.KeyDown += new KeyEventHandler(txt_Contraseña_KeyDown);
        }



        private void ProbarConexion()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    MessageBox.Show("Conexión exitosa a la base de datos.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al conectar a la base de datos: " + ex.Message);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txt_Contraseña_TextChanged(object sender, EventArgs e)
        {

        }



        private void horafecha_Tick(object sender, EventArgs e)
        {
            lblHora.Text = DateTime.Now.ToLongTimeString();
            lblFecha.Text = DateTime.Now.ToLongDateString();
        }

        private void btn_Registrar_Click(object sender, EventArgs e)
        {
            Registro registro = new Registro();
            registro.Show();

        }

        private void btn_Iniciar_Click(object sender, EventArgs e)
        {
            string usuario = txt_Usuario.Text;
            string contraseña = txt_Contraseña.Text;

            if (usuario == "" || contraseña == "")
            {
                MessageBox.Show("Por favor ingresa usuario y contraseña.");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Usuarios WHERE NombreUsuario = @usuario AND Contraseña = @contraseña";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@usuario", usuario);
                cmd.Parameters.AddWithValue("@contraseña", contraseña);

                try
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            MessageBox.Show("✅ Inicio de sesión exitoso");
                            Menu menu = new Menu();
                            this.Hide();
                            menu.ShowDialog(); // modal: vuelve aquí cuando se cierre Menu
                            this.Show();       // opcional: mostrar de nuevo el login
                        }
                        else
                        {
                            MessageBox.Show("❌ Usuario o contraseña incorrectos");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al conectar: " + ex.Message);
                }
            }
        }

        private void txt_Usuario_TextChanged(object sender, EventArgs e)
        {
            // Aquí puedes agregar lógica si es necesario cuando cambia el texto del usuario
        }

        private void txt_Usuario_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txt_Contraseña.Focus();
                e.SuppressKeyPress = true;
            }
        }

        private void txt_Contraseña_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btn_Iniciar.PerformClick(); // Ejecuta el botón de login
                e.SuppressKeyPress = true;
            }
        }


    }
    }
