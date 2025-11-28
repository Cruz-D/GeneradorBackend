using Microsoft.Win32;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.IO;

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
            // Abrir un cuadro de diálogo para seleccionar la ruta de la solución
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
                    // Mostrar la estructura de carpetas en el TextBox
                    var directoryTree = GetDirectoryTree(Path.GetDirectoryName(solutionPath)!);

                    ModelTextBox_carpetas.Text = directoryTree;
                }
            }

        }

        private static readonly string[] CarpetasIgnoradas = new[]
        {
            "bin", "obj", ".vs", ".git", ".idea", ".vscode", "node_modules","Properties"
        };

        private static readonly string[] ExtensionesIgnoradas = new[]
        {
            ".user", ".suo", ".db", ".log", ".gitignore", ".gitattributes", ".copilot", ".dockerignore", ".http", ".Development.json", ".json", ".sln"
        };

        private string GetDirectoryTree(string rootPath, string indent = "")
        {
            var sb = new System.Text.StringBuilder(); //Constructor de cadenas eficiente
            var dirInfo = new DirectoryInfo(rootPath); //DirectoryInfo proporciona propiedades y métodos para trabajar con directorios

            // Ignorar carpetas ocultas, de sistema o irrelevantes
            if ((dirInfo.Attributes & FileAttributes.Hidden) != 0 || //Attributes devuelve los atributos del archivo o directorio
                (dirInfo.Attributes & FileAttributes.System) != 0 || 
                CarpetasIgnoradas.Contains(dirInfo.Name.ToLower())) //Se mira si el nombre de la carpeta está en la lista de carpetas ignoradas, sino se procesa
                return string.Empty; // se retorna cadena vacía

            sb.AppendLine($"{indent}{dirInfo.Name}/"); //Agregar el nombre del directorio al StringBuilder

            // Archivos relevantes en el directorio actual
            foreach (var file in dirInfo.GetFiles()) //Recorrer carpeta usando la propiedad GetFiles()
            {
                // Ignorar archivos ocultos, de sistema o irrelevantes
                if ((file.Attributes & FileAttributes.Hidden) != 0 ||
                    (file.Attributes & FileAttributes.System) != 0 ||
                    ExtensionesIgnoradas.Any(ext => file.Name.ToLower().EndsWith(ext)))
                    continue;

                sb.AppendLine($"{indent}  {file.Name}");
            }

            // Subdirectorios
            foreach (var dir in dirInfo.GetDirectories())
            {
                var subTree = GetDirectoryTree(dir.FullName, indent + "  "); //Llamada recursiva para obtener la estructura de subdirectorios
                if (!string.IsNullOrWhiteSpace(subTree))
                    sb.Append(subTree);
            }

            return sb.ToString(); // retorna la estructura de directorios como cadena
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
            _schemaReader = new SqlSchemaReader(connectionString);
            _tables = _schemaReader.GetTables();
            TablesComboBox.ItemsSource = _tables;
            TablesComboBox.DisplayMemberPath = "Name";
            TablesComboBox.SelectedIndex = 0;
        }

        private void ModelTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
