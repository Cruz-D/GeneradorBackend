using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GeneradorBackend
{
    /// <summary>
    /// Lógica de interacción para ConnectionStringGenerator.xaml
    /// </summary>
    public partial class ConnectionStringGenerator : Window
    {
        public ConnectionStringGenerator()
        {
            InitializeComponent();

        }
        // Propiedad para almacenar la cadena de conexión generada
        public string? GeneratedConnectionString { get; private set; }

        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            // Aquí puedes agregar la lógica para generar la cadena de conexión
            // basada en los parámetros ingresados por el usuario.
            string server = ServerTextBox.Text;
            string database = DatabaseTextBox.Text;
            string userId = UserTextBox.Text;
            string password = PasswordTextBox.Password;
            bool integratedSecurity = IntegratedSecurityCheckBox.IsChecked == true;
            bool encrypt = EncryptCheckBox.IsChecked == true;
            bool trustServerCertificate = TrustServerCertificateCheckBox.IsChecked == true;
            string connectionString;

            //Si cadena de conexion no tiene usuario y password 
            if (userId.Contains(string.Empty)||password.Contains(string.Empty))
            {
                connectionString = $"Data Source={server};Initial Catalog={database};Integrated Security=True;Encrypt={encrypt};Trust Server Certificate={trustServerCertificate}";
                
            }
            else //si hay usuario y contraseña
            {

                connectionString = $"Data Source={server};Initial Catalog={database};User ID={userId};Password={password};Encrypt={encrypt};Trust Server Certificate={trustServerCertificate}";
            }

            ConnectionStringTextBox.Text = connectionString;
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            //pasar cadena de conexion a la ventana principal
            GeneratedConnectionString = ConnectionStringTextBox.Text;
            this.Close();
        }
    }
}
