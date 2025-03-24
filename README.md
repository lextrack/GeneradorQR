# Generador de Códigos QR

## Descripción
Esta aplicación es un generador de códigos QR desarrollado con WPF y .NET 9. Permite a los usuarios crear códigos QR personalizados a partir de texto o URLs, ajustar el tamaño del código generado y guardar la imagen resultante en formato PNG.


## Características
- Generación de códigos QR a partir de texto o URLs
- Selección de diferentes tamaños para el código QR (200px - 1000px)
- Previsualización en tiempo real del código generado
- Guardado de la imagen en formato PNG con nombre automático basado en fecha y hora
- Interfaz intuitiva y fácil de usar

## Requisitos del Sistema
- Windows 7 o superior
- .NET 9 Runtime
- 200 MB de espacio en disco

## Uso
1. Ingrese el texto o URL que desea convertir en el campo de texto
2. Seleccione el tamaño deseado para el código QR
3. Haga clic en "Generar QR"
4. Para guardar la imagen, haga clic en "Guardar QR" y seleccione la ubicación donde desea guardarla
5. Para limpiar el contenido y empezar de nuevo, haga clic en "Limpiar"

## Estructura del Proyecto y Patrón MVVM

El proyecto sigue el patrón de diseño MVVM (Model-View-ViewModel), que separa la lógica de negocio de la interfaz de usuario, facilitando el mantenimiento y las pruebas del código.

### Estructura de Carpetas
```
QR/
├── Models/
│   └── QrCodeModel.cs
├── ViewModels/
│   ├── MainViewModel.cs
│   └── RelayCommand.cs
├── Views/
│   ├── MainWindow.xaml
│   └── MainWindow.xaml.cs
└── App.xaml
```

### Componentes Principales

#### Model (QrCodeModel.cs)
El modelo contiene la lógica de negocio para generar y guardar códigos QR.

```csharp
public class QrCodeModel
{
    public BitmapSource GenerateQrCode(string content, int size, int margin = 1)
    {
        // Lógica para generar el código QR utilizando la biblioteca ZXing
    }

    public void SaveQrCodeToFile(string filePath, BitmapSource qrBitmap)
    {
        // Lógica para guardar el código QR como imagen PNG
    }
}
```

El modelo se encarga exclusivamente de:
- Generar el código QR a partir del texto proporcionado
- Configurar las opciones del código QR (tamaño, margen, nivel de corrección de errores)
- Convertir el resultado en un formato que pueda mostrarse en la interfaz
- Guardar la imagen en un archivo

#### ViewModel (MainViewModel.cs)
El ViewModel actúa como intermediario entre la Vista y el Modelo, gestionando:
- El estado de la aplicación (contenido del QR, imagen generada, tamaño seleccionado)
- Comandos para las acciones del usuario (generar, guardar, limpiar)
- Notificaciones de cambios a la Vista mediante la implementación de INotifyPropertyChanged
- Validación de entrada y manejo de errores

```csharp
public class MainViewModel : INotifyPropertyChanged
{
    private readonly QrCodeModel _qrCodeModel;
    
    // Propiedades para bindear con la interfaz
    public string QrContent { get; set; }
    public BitmapSource QrImage { get; set; }
    public int SelectedSize { get; set; }
    
    // Comandos
    public ICommand GenerateQrCommand { get; }
    public ICommand SaveQrCommand { get; }
    public ICommand ClearQrCommand { get; }
    
    // Implementación de INotifyPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;
}
```

#### View (MainWindow.xaml)
La Vista es responsable únicamente de la representación visual y la interacción con el usuario:
- Muestra la interfaz gráfica
- Enlaza controles a propiedades y comandos del ViewModel mediante Data Binding
- No contiene lógica de negocio, solo configuración de la interfaz

```xml
<Grid>
    <!-- Controles de entrada -->
    <TextBox Text="{Binding QrContent, UpdateSourceTrigger=PropertyChanged}" />
    <ComboBox ItemsSource="{Binding AvailableSizes}" SelectedItem="{Binding SelectedSize}" />
    
    <!-- Comandos -->
    <Button Content="Generar QR" Command="{Binding GenerateQrCommand}" />
    
    <!-- Visualización -->
    <Image Source="{Binding QrImage}" />
</Grid>
```

#### Command (RelayCommand.cs)
Implementa la interfaz ICommand para encapsular acciones y sus condiciones de ejecución:

```csharp
public class RelayCommand : ICommand
{
    private readonly Action<object> _execute;
    private readonly Predicate<object> _canExecute;
    
    // Implementación de ICommand
    public bool CanExecute(object parameter) { ... }
    public void Execute(object parameter) { ... }
    public event EventHandler CanExecuteChanged;
}
```

### Flujo de Datos en el Patrón MVVM

1. **Usuario → Vista**: El usuario interactúa con la interfaz (escribe texto, hace clic en botones).
2. **Vista → ViewModel**: Los controles de la Vista están enlazados a propiedades y comandos del ViewModel mediante Data Binding.
3. **ViewModel → Modelo**: El ViewModel solicita al Modelo realizar operaciones de negocio (generar QR, guardar archivos).
4. **Modelo → ViewModel**: El Modelo devuelve los resultados al ViewModel.
5. **ViewModel → Vista**: El ViewModel actualiza sus propiedades, notifica a la Vista mediante INotifyPropertyChanged, y la Vista se actualiza automáticamente.

### ¿Por qué el patrón MVVM?

1. **Separación de responsabilidades**: Cada componente tiene un propósito específico, lo que facilita el mantenimiento y la comprensión del código.
2. **Testabilidad**: El ViewModel y el Modelo pueden probarse de forma independiente de la interfaz de usuario.
3. **Reusabilidad**: Los componentes pueden reutilizarse en diferentes partes de la aplicación o en otros proyectos.
4. **Escalabilidad**: Facilita la adición de nuevas características sin modificar el código existente.
5. **Mantenimiento**: Los cambios en la interfaz de usuario no afectan a la lógica de negocio y viceversa.

## Bibliotecas Utilizadas
- ZXing.Net: Para la generación de códigos QR
- Windows Presentation Foundation (WPF): Para la interfaz gráfica

## Contribuciones
Las contribuciones son bienvenidas. Para contribuir:
1. Haga un fork del repositorio
2. Cree una rama para su función (`git checkout -b feature/nueva-funcion`)
3. Haga commit de sus cambios (`git commit -m 'Añadir nueva función'`)
4. Haga push a la rama (`git push origin feature/nueva-funcion`)
5. Abra un Pull Request