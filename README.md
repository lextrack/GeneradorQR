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
- 300 MB de espacio en disco

## Uso
1. Ingrese el texto o URL que desea convertir en el campo de texto
2. Seleccione el tamaño deseado para el código QR
3. Haga clic en "Generar QR"
4. Para guardar la imagen, haga clic en "Guardar QR" y seleccione la ubicación donde desea guardarla
5. Para limpiar el contenido y empezar de nuevo, haga clic en "Limpiar"

### Flujo de Datos en el Patrón MVVM

1. **Usuario → Vista**: El usuario interactúa con la interfaz (escribe texto, hace clic en botones).
2. **Vista → ViewModel**: Los controles de la Vista están enlazados a propiedades y comandos del ViewModel mediante Data Binding.
3. **ViewModel → Modelo**: El ViewModel solicita al Modelo realizar operaciones de negocio (generar QR, guardar archivos).
4. **Modelo → ViewModel**: El Modelo devuelve los resultados al ViewModel.
5. **ViewModel → Vista**: El ViewModel actualiza sus propiedades, notifica a la Vista mediante INotifyPropertyChanged, y la Vista se actualiza automáticamente.
