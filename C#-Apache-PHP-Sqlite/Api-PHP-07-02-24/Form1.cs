using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Api_Flask_07_02_24
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Insertar datos en el servidor
                string postDataUrl = "http://127.0.0.1/dashboard/php-2022/0-Data-Analytics/26-CNET-Apache-PHP-Sqlite/";
                var newData = new
                {
                    id = textBox5.Text,
                    nombre = textBox1.Text,
                    direccion = textBox2.Text,
                    telefono = textBox3.Text,
                    email = textBox4.Text
                };
                string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(newData);

                using (HttpClient httpClient = new HttpClient())
                {
                    StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = httpClient.PostAsync(postDataUrl, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Datos insertados correctamente");
                    }
                    else
                    {
                        MessageBox.Show("Error al insertar datos. Código de estado: " + response.StatusCode);
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show("Error al insertar datos en el servidor: " + ex.Message);
            }

            try
            {
                // Obtener datos del servidor y actualizar el DataGridView
                string getDataUrl = "http://127.0.0.1/dashboard/php-2022/0-Data-Analytics/26-CNET-Apache-PHP-Sqlite/";

                using (HttpClient httpClient = new HttpClient())
                {
                    HttpResponseMessage getDataResponse = httpClient.GetAsync(getDataUrl).Result;

                    if (getDataResponse.IsSuccessStatusCode)
                    {
                        string responseData = getDataResponse.Content.ReadAsStringAsync().Result;
                        Newtonsoft.Json.Linq.JObject result = Newtonsoft.Json.Linq.JObject.Parse(responseData);
                        List<Newtonsoft.Json.Linq.JToken> dataList = result["datos"].ToObject<List<Newtonsoft.Json.Linq.JToken>>();

                        DataTable dataTable = new DataTable();
                        foreach (string columnName in new[] { "id", "nombre", "direccion", "telefono", "email" })
                        {
                            dataTable.Columns.Add(columnName);
                        }

                        foreach (Newtonsoft.Json.Linq.JToken dataItem in dataList)
                        {
                            dataTable.Rows.Add(dataItem["id"].ToString(), dataItem["nombre"].ToString(), dataItem["direccion"].ToString(), dataItem["telefono"].ToString(), dataItem["email"].ToString());
                        }

                        dataGridView1.DataSource = dataTable;

                        foreach (DataGridViewColumn column in dataGridView1.Columns)
                        {
                            column.HeaderText = column.Name;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No se encontraron datos para actualizar.");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show("Error al procesar la búsqueda: " + ex.Message);
            }

            // Limpiar los campos de entrada y deshabilitar los controles después de la operación
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";

            textBox1.Enabled = false;
            textBox2.Enabled = false;
            textBox3.Enabled = false;
            textBox4.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = true;
            textBox5.Visible = true;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Deshabilitar controles
            textBox1.Enabled = false;
            textBox2.Enabled = false;
            textBox3.Enabled = false;
            textBox4.Enabled = false;
            button1.Enabled = false;
            textBox5.Visible = false;

            // Realizar la solicitud GET para obtener los datos más recientes
            string getDataUrl = "http://127.0.0.1/dashboard/php-2022/0-Data-Analytics/26-CNET-Apache-PHP-Sqlite/";

            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage getDataResponse = httpClient.GetAsync(getDataUrl).Result;

                if (getDataResponse.IsSuccessStatusCode)
                {
                    string responseData = getDataResponse.Content.ReadAsStringAsync().Result;

                    if (string.IsNullOrWhiteSpace(responseData))
                    {
                        MessageBox.Show("La respuesta está vacía.");
                    }
                    else
                    {
                        Newtonsoft.Json.Linq.JObject result = Newtonsoft.Json.Linq.JObject.Parse(responseData);
                        List<Newtonsoft.Json.Linq.JToken> dataList = null;

                        try
                        {
                            dataList = result["datos"].ToObject<List<Newtonsoft.Json.Linq.JToken>>();

                            if (dataList.Count > 0)
                            {
                                // Limpiar el DataGridView
                                dataGridView1.Rows.Clear();
                                dataGridView1.Columns.Clear();

                                // Crear DataTable y agregar columnas
                                DataTable dataTable = new DataTable();
                                foreach (string columnName in new string[] { "id", "nombre", "direccion", "telefono", "email" })
                                {
                                    dataTable.Columns.Add(columnName);
                                }

                                // Agregar filas al DataTable
                                foreach (Newtonsoft.Json.Linq.JToken dataItem in dataList)
                                {
                                    dataTable.Rows.Add(dataItem["id"].ToString(), dataItem["nombre"].ToString(), dataItem["direccion"].ToString(), dataItem["telefono"].ToString(), dataItem["email"].ToString());
                                }

                                // Actualizar el DataSource del DataGridView
                                dataGridView1.DataSource = dataTable;

                                // Mostrar los encabezados de las columnas
                                foreach (DataGridViewColumn column in dataGridView1.Columns)
                                {
                                    column.HeaderText = column.Name;
                                }
                            }
                            else
                            {
                                MessageBox.Show("No se encontraron datos para actualizar.");
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error al analizar los datos JSON: " + ex.Message);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Error al obtener los datos para actualizar. Código de estado: " + getDataResponse.StatusCode);
                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Realizar la solicitud GET para obtener los datos más recientes
            string getDataUrl = "http://127.0.0.1/dashboard/php-2022/0-Data-Analytics/26-CNET-Apache-PHP-Sqlite/";

            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage getDataResponse = httpClient.GetAsync(getDataUrl).Result; // Síncrono

                if (getDataResponse.IsSuccessStatusCode)
                {
                    string responseData = getDataResponse.Content.ReadAsStringAsync().Result;
                    Newtonsoft.Json.Linq.JObject result = Newtonsoft.Json.Linq.JObject.Parse(responseData);
                    List<Newtonsoft.Json.Linq.JToken> dataList = result["datos"].ToObject<List<Newtonsoft.Json.Linq.JToken>>();

                    // Agregar nombres de las columnas
                    string[] columnNames = { "id", "nombre", "direccion", "telefono", "email" };

                    // Extraer el último ID
                    if (dataList.Any())
                    {
                        string lastIdString = dataList.Last()["id"].ToString();
                        // string lastIdString = dataList.Last()[0].ToString(); // Suponiendo que el ID es la primera columna
                        if (int.TryParse(lastIdString, out int lastId))
                        {
                            // Sumarle 1 al último ID
                            int siguienteId = lastId + 1;

                            // Escribir el siguiente ID en textBox5
                            textBox5.Text = siguienteId.ToString();
                        }
                        else
                        {
                            // Manejar el caso en el que el último ID no sea un número válido
                            textBox5.Text = "1"; // Por ejemplo, podrías establecer el valor predeterminado a 1
                        }
                    }
                    else
                    {
                        // Manejar el caso en el que no haya datos en la lista
                        textBox5.Text = "1"; // Por ejemplo, podrías establecer el valor predeterminado a 1
                    }
                }
                else
                {
                    // Manejar el caso en que la solicitud no sea exitosa
                    textBox5.Text = "1"; // Por ejemplo, podrías establecer el valor predeterminado a 1
                }
            }

            // Habilitar campos y botones después de la solicitud
            textBox1.Enabled = true;
            textBox2.Enabled = true;
            textBox3.Enabled = true;
            textBox4.Enabled = true;
            textBox1.Focus();
            button1.Enabled = true;
            button2.Enabled = false;

            //////////////////////////////////////////////////////////////
        }
        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string nombreBuscado = textBox6.Text.Trim(); // Obtener el texto ingresado en textBox6 para la búsqueda por nombre
                string getDataUrl = "http://127.0.0.1/dashboard/php-2022/0-Data-Analytics/26-CNET-Apache-PHP-Sqlite/";

                using (HttpClient httpClient = new HttpClient())
                {
                    HttpResponseMessage getDataResponse = httpClient.GetAsync(getDataUrl).Result;

                    if (getDataResponse.IsSuccessStatusCode)
                    {
                        string responseData = getDataResponse.Content.ReadAsStringAsync().Result;
                        Newtonsoft.Json.Linq.JObject result = Newtonsoft.Json.Linq.JObject.Parse(responseData);
                        List<Newtonsoft.Json.Linq.JToken> dataList = result["datos"].ToObject<List<Newtonsoft.Json.Linq.JToken>>();

                        // Filtrar los datos por nombre en tiempo real mientras el usuario escribe
                        List<Newtonsoft.Json.Linq.JToken> resultadosBusqueda = dataList
                            .Where(d => d["nombre"].ToString().IndexOf(nombreBuscado, StringComparison.OrdinalIgnoreCase) >= 0)
                            .ToList();

                        // Crear DataTable y agregar columnas con nombres específicos
                        DataTable dataTable = new DataTable();
                        dataTable.Columns.Add("Id");
                        dataTable.Columns.Add("Nombre");
                        dataTable.Columns.Add("Dirección");
                        dataTable.Columns.Add("Teléfono");
                        dataTable.Columns.Add("Email");

                        // Agregar filas al DataTable
                        foreach (Newtonsoft.Json.Linq.JToken dataItem in resultadosBusqueda)
                        {
                            dataTable.Rows.Add(dataItem["id"].ToString(), dataItem["nombre"].ToString(), dataItem["direccion"].ToString(), dataItem["telefono"].ToString(), dataItem["email"].ToString());
                        }

                        // Actualizar el DataSource del DataGridView en el hilo de la interfaz de usuario
                        dataGridView1.Invoke((Action)(() =>
                        {
                            dataGridView1.DataSource = dataTable;
                        }));
                    }
                    else
                    {
                        MessageBox.Show("Error al obtener los datos. Código de estado: " + getDataResponse.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al procesar la búsqueda: " + ex.Message);
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                int dato_id;
                if (!int.TryParse(textBox7.Text.Trim(), out dato_id))
                {
                    MessageBox.Show("Ingrese un ID válido para el registro a borrar.");
                    return;
                }

                string urlBorrar = $"http://127.0.0.1/dashboard/php-2022/0-Data-Analytics/26-CNET-Apache-PHP-Sqlite/?id={dato_id}";

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = client.DeleteAsync(urlBorrar).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Registro con ID {dato_id} borrado exitosamente.");
                        // Actualizar el DataGridView u otra lógica necesaria después de borrar el registro...
                    }
                    else
                    {
                        MessageBox.Show("Error al borrar el registro. Código de estado: " + response.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al borrar el registro: " + ex.Message);
            }

            // Obtener datos del servidor y actualizar el DataGridView
            try
            {
                string getDataUrl = "http://127.0.0.1/dashboard/php-2022/0-Data-Analytics/26-CNET-Apache-PHP-Sqlite/";

                using (HttpClient httpClient = new HttpClient())
                {
                    HttpResponseMessage getDataResponse = httpClient.GetAsync(getDataUrl).Result;

                    if (getDataResponse.IsSuccessStatusCode)
                    {
                        string responseData = getDataResponse.Content.ReadAsStringAsync().Result;
                        Newtonsoft.Json.Linq.JObject result = Newtonsoft.Json.Linq.JObject.Parse(responseData);
                        List<Newtonsoft.Json.Linq.JToken> dataList = result["datos"].ToObject<List<Newtonsoft.Json.Linq.JToken>>();

                        DataTable dataTable = new DataTable();
                        foreach (string columnName in new[] { "id", "nombre", "direccion", "telefono", "email" })
                        {
                            dataTable.Columns.Add(columnName);
                        }

                        foreach (Newtonsoft.Json.Linq.JToken dataItem in dataList)
                        {
                            dataTable.Rows.Add(dataItem["id"].ToString(), dataItem["nombre"].ToString(), dataItem["direccion"].ToString(), dataItem["telefono"].ToString(), dataItem["email"].ToString());
                        }

                        dataGridView1.DataSource = dataTable;

                        foreach (DataGridViewColumn column in dataGridView1.Columns)
                        {
                            column.HeaderText = column.Name;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No se encontraron datos para actualizar.");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show("Error al procesar la búsqueda: " + ex.Message);
            }

            textBox7.Text = ""; 


        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        




    }
    }

       
  

