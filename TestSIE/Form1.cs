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

namespace TestSIE
{
    public partial class Form1 : Form
    {

        private SqlDataAdapter _dataAdapter;
        private DataTable _dataTable;

        private const string connectionString = "Data Source=(localdb)\\MSSQLLocalDB; Initial Catalog=TestSIE; Integrated Security=True;";

        public Form1()
        {
            InitializeComponent();
            Data.CellEndEdit += Data_CellEndEdit;
        }

        


        private void button1_Click(object sender, EventArgs e)
        {
            FormularioUsuario.Show();
            FormularioUsuario.BringToFront();
            FormularioVehiculo.Hide();
            string sqlQuery = "SELECT ID, NOMBRE, APELLIDO FROM USUARIOS1"; 
            LoadDataToDataGridView(sqlQuery);
            Data.Visible = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FormularioVehiculo.Show();
            FormularioVehiculo.BringToFront();
            FormularioUsuario.Hide();
            string sqlQuery = "SELECT ID, MARCA, MODELO, VIN FROM VEHICULOS"; 
            LoadDataToDataGridView(sqlQuery);
            Data.Visible = true;
        }


        private void FormularioUsuario_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true; 
            this.Hide();    
        }


        private void LoadDataToDataGridView(string query)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    _dataAdapter = new SqlDataAdapter(query, connection);
                    SqlCommandBuilder commandBuilder = new SqlCommandBuilder(_dataAdapter);
                    _dataTable = new DataTable();

                    _dataAdapter.Fill(_dataTable);
                    Data.DataSource = _dataTable;
                    Data.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar los datos: " + ex.Message);
                }
            }
        }


        private void GuardarUsuario_Click(object sender, EventArgs e)
        {
            if (FormularioUsuario.Visible)
            {
                InsertarUsuario();
            }
            else if (FormularioVehiculo.Visible)
            {
                InsertarVehiculo();
            }
            else
            {
                MessageBox.Show("No hay un formulario activo para guardar datos.");
            }
        }

        private bool ExisteUsuario(string nombre, string apellido)
        {
            string query = "SELECT COUNT(ID) FROM USUARIOS1 WHERE NOMBRE = @NOMBRE AND APELLIDO = @APELLIDO";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NOMBRE", nombre);
                    command.Parameters.AddWithValue("@APELLIDO", apellido);

                    try
                    {
                        connection.Open();
                        int count = (int)command.ExecuteScalar();
                        return count > 0; 
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al verificar usuario: " + ex.Message, "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return true; 
                    }
                }
            }
        }


        private void InsertarUsuario()
        {
            string NOMBRE = textBox1.Text.Trim();
            string APELLIDO = textBox2.Text.Trim();
            string ID = textBox3.Text;


            if (ExisteUsuario(NOMBRE, APELLIDO))
            {
                MessageBox.Show("Ya existe una persona registrada con ese Nombre y Apellido.", "Error de Unicidad", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = "INSERT INTO USUARIOS1 (ID, NOMBRE, APELLIDO) " +
                            "VALUES (@ID, @NOMBRE, @APELLIDO)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    command.Parameters.AddWithValue("@ID", ID);
                    command.Parameters.AddWithValue("@NOMBRE", NOMBRE);
                    command.Parameters.AddWithValue("@APELLIDO", APELLIDO);

                    try
                    {
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Usuario agregado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            textBox1.Clear();
                            textBox2.Clear();
                            textBox3.Clear();
                            RecargarDatosDespuesDeInsertar("USUARIOS1");
                        }
                    }
                    catch (SqlException ex)
                    {
 
                        MessageBox.Show("Error al guardar el usuario: " + ex.Message, "Error de Base de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {

                        MessageBox.Show("Ocurrió un error inesperado: " + ex.Message);
                    }
                }
            }
        }


        private bool ExisteVIN(string vin)
        {
            string query = "SELECT COUNT(VIN) FROM VEHICULOS WHERE VIN = @VIN";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@VIN", vin);
                    try
                    {
                        connection.Open();
                        int count = (int)command.ExecuteScalar();
                        return count > 0;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al verificar VIN: " + ex.Message, "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return true;
                    }
                }
            }
        }

        private void InsertarVehiculo()
        {
            
            string propietarioID = textBox4.Text.Trim(); 
            string marca = textBox6.Text.Trim();
            string modelo = textBox5.Text.Trim();
            string vin = textBox7.Text.Trim();

            if (ExisteVIN(vin))
            {
                MessageBox.Show("Ya existe un vehículo registrado con ese VIN.", "Error de Unicidad", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; 
            }
            if (string.IsNullOrWhiteSpace(propietarioID) || !ExisteUsuarioPorID(propietarioID))
            {
                MessageBox.Show("El ID del Propietario no es válido o no existe en la base de datos.", "Error de Propietario", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; 
            }
            string query = "INSERT INTO VEHICULOS (MARCA, MODELO, VIN, ID_PROPIETARIO) " +
                           "VALUES (@Marca, @Modelo, @VIN, @ID_PROPIETARIO)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    command.Parameters.AddWithValue("@Marca", marca);
                    command.Parameters.AddWithValue("@Modelo", modelo);
                    command.Parameters.AddWithValue("@VIN", vin);
                    command.Parameters.AddWithValue("@ID_PROPIETARIO", propietarioID);

                    try
                    {
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Vehículo agregado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            textBox4.Clear();
                            textBox5.Clear();
                            textBox6.Clear();
                            textBox7.Clear();
                            RecargarDatosDespuesDeInsertar("VEHICULOS");
                        }
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show("Error al guardar el vehículo: " + ex.Message, "Error de Base de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ocurrió un error inesperado: " + ex.Message);
                    }
                }
            }
        }

        private bool ExisteUsuarioPorID(string id)
        {
            string query = "SELECT COUNT(ID) FROM USUARIOS1 WHERE ID = @ID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", id);

                    try
                    {
                        connection.Open();
                        int count = (int)command.ExecuteScalar();
                        return count > 0;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
        }


        private void Data_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Asegúrate de tener un DataAdapter y DataTable válidos
            if (_dataAdapter == null || _dataTable == null)
            {
                return;
            }

            try
            {
                Data.EndEdit();

                _dataTable.EndLoadData();

                int rowsAffected = _dataAdapter.Update(_dataTable);

                if (rowsAffected > 0)
                {
                    _dataTable.AcceptChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar los cambios: " + ex.Message, "Error de Base de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (_dataTable.Rows.Count > e.RowIndex)
                {
                    _dataTable.Rows[e.RowIndex].RejectChanges();
                }
            }
        }

        private void RecargarDatosDespuesDeInsertar(string tabla)
        {
            string query = "";
            if (tabla == "USUARIOS1")
            {
                query = "SELECT ID, Nombre, Apellido FROM USUARIOS1";
            }
            else if (tabla == "VEHICULOS")
            {
                query = "SELECT ID, Marca, Modelo, VIN FROM VEHICULOS";
            }
            else
            {
                return;
            }

            LoadDataToDataGridView(query);
        }










        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void FormularioUsuario_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

       
        private void FormularioVehiculo_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Data_CellContentClick(object sender, DataGridViewCellEventArgs e)
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

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
         
        }

        private void textBox7_TextChanged(object sender, EventArgs e)   
        {
           
        }
    }
}
