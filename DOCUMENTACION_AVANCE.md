# Documentación — Estado actual de la solución "GeneradorBackend"

## Resumen rápido
Aplicación WPF (.NET 9) para generar código de modelos a partir del esquema de una base de datos SQL Server. Interfaz principal en `Views/MainWindow.xaml` con su code-behind en `Views/MainWindow.xaml.cs`.

## Archivos y componentes clave
- `Views/MainWindow.xaml` — Ventana principal (moved a `Views/`). Debe tener `Build Action: Page`.
- `Views/MainWindow.xaml.cs` — Lógica de UI:
  - Selección de `.csproj` (ruta de solución).
  - Entrada de cadena de conexión a DB mediante `ConnectionStringGenerator`.
  - Lectura de tablas y columnas y generación de código de modelo.
- `Views/ConnectionStringGenerator.xaml` / `.xaml.cs` — Ventana modal para construir la cadena de conexión. Expone `GeneratedConnectionString` al aceptar.
- `SqlSchemaReader.cs` — Lector de esquema (usa `Microsoft.Data.SqlClient`):
  - `GetTables()` devuelve `List<TableInfo>`
  - `GetColumns(tableName)` devuelve `List<ColumnInfo>`
- Modelos simples: `TableInfo`, `ColumnInfo`.

## Estructura de carpetas agregada
Se añadieron carpetas comunes para organizar el proyecto y archivos placeholder `.gitkeep` para que Git las rastree:
- `Views/`, `ViewModels/`, `Services/`, `Models/`, `Repositories/`, `Helpers/`, `Resources/`, `Controls/`

En el proyecto se incluyeron patrones `None` para que estas carpetas sean visibles en el Explorador de soluciones.

## Cambios en `GeneradorBackend.csproj`
- Se añadió `<LangVersion>latest</LangVersion>`.
- Se excluyen todos los archivos con extensión `.template` de compilación y empaquetado:
  - `Compile Remove="**\*.template"` etc.
- Los `.template` se mantienen en el repositorio como `None` con `<Pack>false</Pack>` y `<CopyToOutputDirectory>Never</CopyToOutputDirectory>`.

> Nota: esto evita que archivos `.template` se compilen o se incluyan en paquetes/recursos.

## Comportamiento implementado
- Abrir diálogo para seleccionar `.csproj` y mostrar árbol de carpetas (ignora `bin`, `obj`, `.git`, etc.).
- Generar cadena de conexión (integrated security / usuario+password).
- Listar tablas de la base de datos en `TablesComboBox` y mostrar código generado en `ModelTextBox_codigo` al seleccionar tabla.

## Requisitos y dependencias
- Target framework: `.NET 9` (proyecto `GeneradorBackend.csproj`).
- Paquetes: `Microsoft.Data.SqlClient` (asegurar NuGet restore).
- Revisar si `System.Data.SqlClient` es necesaria; puede ser redundante con `Microsoft.Data.SqlClient`.
- Visual Studio 2022/2023 actualizado con soporte para .NET 9.

## Verificaciones necesarias antes de ejecutar
1. `App.xaml` — validar `StartupUri` apunta a `Views/MainWindow.xaml` si se movió el XAML.
2. `Views/MainWindow.xaml` — `Build Action` = `Page` y `Copy to Output` no necesario para XAML.
3. Restaurar paquetes NuGet y compilar (`dotnet restore` y `dotnet build`).
4. Comprobar que las carpetas nuevas aparecen en el Explorador de soluciones; si no, limpiar y recargar solución.
5. Permisos/credenciales para la base de datos y cadena de conexión válida.

## Riesgos y pendientes
- Añadir validación robusta en `checkSolutionPath` (actualmente siempre retorna `true`).
- Manejar excepciones de conexión en `SqlSchemaReader` y mostrar errores adecuados al usuario.
- Soportar esquemas con prefijos (catalog/schema) y tablas con nombres especiales.
- Añadir tests unitarios para `SqlSchemaReader` (mocks de `SqlConnection`).
- Documentar ejemplos de cadenas de conexión esperadas.
- Revisar la dependencia doble `System.Data.SqlClient` vs `Microsoft.Data.SqlClient` y eliminar la duplicada si procede.

## Siguientes mejoras sugeridas (rápido)
- Añadir plantillas base MVVM:
  - `Views/MainWindow.xaml` + `ViewModels/MainWindowViewModel.cs` con INotifyPropertyChanged.
  - `Services/IDbSchemaService` y `Services/SqlSchemaReader` implementando la interfaz.
- Registrar servicios en un contenedor DI (por ejemplo `Microsoft.Extensions.DependencyInjection`).
- Añadir un comando para exportar modelos generados a una carpeta/paquete del proyecto seleccionado.

## Cómo ejecutar localmente
1. Abrir la solución en Visual Studio.
2. Restaurar paquetes NuGet.
3. Verificar `App.xaml` y propiedades XAML.
4. Compilar y ejecutar (F5). Al iniciar, usar "Cargar Tablas" para introducir la cadena de conexión y listar tablas.

---
Documento actualizado para reflejar la estructura actual del proyecto y los cambios recientes en el `.csproj`.
