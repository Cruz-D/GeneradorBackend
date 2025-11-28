using GeneradorBackend.Services;
using Microsoft.Win32;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace GeneradorBackend
{
    public partial class MainWindow : Window
    {
        private SqlSchemaService? _schemaReader;
        private List<TableInfo>? _tables;
        private readonly ProjectReaderService _projectReader;

        public MainWindow()
        {
            InitializeComponent();
            _projectReader = new ProjectReaderService();
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
                Filter = "Archivos de proyecto C# (*.csproj)|*.csproj",
                Title = "Selecciona el archivo .csproj de la solución"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var solutionPath = openFileDialog.FileName;

                if (checkSolutionPath(solutionPath))
                {
                    SolutionPathTextBox.Text = solutionPath;
                    var directoryTree = _projectReader.ReadDirectoryTree(Path.GetDirectoryName(solutionPath)!);
                    ModelTextBox_carpetas.Text = directoryTree;
                }
            }
        }


        private void TablesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_schemaReader == null || TablesComboBox.SelectedItem is not TableInfo table)
                return;

            var columns = _schemaReader.GetColumns(table.Name);
            var generator = new ModelGenerator();
            var modelCode = generator.GenerateModel(table.Name, columns);
            ModelTextBox_codigo.Text = modelCode;
        }

        private void ReadTables(string connectionString)
        {
            _schemaReader = new SqlSchemaService(connectionString);
            _tables = _schemaReader.GetTables();
            TablesComboBox.ItemsSource = _tables;
            TablesComboBox.DisplayMemberPath = "Name";
            TablesComboBox.SelectedIndex = 0;
        }

        private void ModelTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private bool checkSolutionPath(string path)
        {
            //comprobar que la ruta tiene un archivo con extension .sln
            if (!path.EndsWith(".csproj"))
            {
                MessageBox.Show("La ruta seleccionada no es un archivo de solución válido.");
            }

            return true;
        }

    }
}
