using Microsoft.Win32;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace GeneradorBackend
{
    public partial class MainWindow : Window
    {
        private SqlSchemaReader? _schemaReader;
        private List<TableInfo>? _tables;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadTables_Click(object sender, RoutedEventArgs e)
        {
            // ABRIR UNA VENTANA PARA SELECCIONAR LA CADENA DE CONEXIÓN A LA BASE DE DATOS
            var connectionStringWindow = new ConnectionStringGenerator();
            connectionStringWindow.ShowDialog();

            // Asignar la cadena de conexion creada a una variable
            var connectionString = connectionStringWindow.GeneratedConnectionString;
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                MessageBox.Show("Introduce una cadena de conexión válida.");
                return;
            }
            ConnectionStringTextBox.Text = connectionString;
            ReadTables(connectionString);

        }

        private void SelectSolutionPath_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Archivos de solución (*.sln)|*.sln",
                Title = "Selecciona un archivo de solución"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                // Aquí tienes la ruta seleccionada
                string solutionPath = openFileDialog.FileName;
                // Por ejemplo, puedes mostrarla en un TextBox:
                SolutionPathTextBox.Text = solutionPath;
            }
        }

        private void TablesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_schemaReader == null || TablesComboBox.SelectedItem is not TableInfo table)
                return;

            var columns = _schemaReader.GetColumns(table.Name);
            var generator = new ModelGenerator();
            var modelCode = generator.GenerateModel(table.Name, columns);
            ModelTextBox.Text = modelCode;
        }

        private void ReadTables(string connectionString)
        {
            _schemaReader = new SqlSchemaReader(connectionString);
            _tables = _schemaReader.GetTables();
            TablesComboBox.ItemsSource = _tables;
            TablesComboBox.DisplayMemberPath = "Name";
            TablesComboBox.SelectedIndex = 0;
        }
        //Todo: verificar tipo de archivo
        //private bool CheckPath(string projectPath)
        //{

        //}
    }
}
