using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZXing;
using ZXing.QrCode;
using ZXing.QrCode.Internal;

namespace QR;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private BitmapSource qrBitmap;

    public MainWindow()
    {
        InitializeComponent();
        btnSave.IsEnabled = false;
    }

    private void btnGenerate_Click(object sender, RoutedEventArgs e)
    {
        string content = txtQRContent.Text;

        if (string.IsNullOrWhiteSpace(content))
        {
            MessageBox.Show("Por favor ingresa un texto para generar el código QR.", "Texto Vacío", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            // Obtener el tamaño seleccionado
            int size = 500; // Valor predeterminado
            if (cmbSize.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag != null)
            {
                if (int.TryParse(selectedItem.Tag.ToString(), out int selectedSize))
                {
                    size = selectedSize;
                }
            }

            var writer = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Height = size,
                    Width = size,
                    Margin = 1,
                    ErrorCorrection = ErrorCorrectionLevel.H
                }
            };

            var pixelData = writer.Write(content);

            // Convertir a BitmapSource para mostrar en WPF
            qrBitmap = BitmapSource.Create(
                pixelData.Width,
                pixelData.Height,
                96,
                96,
                PixelFormats.Bgr32,
                null,
                pixelData.Pixels,
                pixelData.Width * 4);

            imgQRCode.Source = qrBitmap;
            btnSave.IsEnabled = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al generar el código QR: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
        if (qrBitmap == null)
        {
            MessageBox.Show("Por favor genera un código QR primero.", "QR no generado", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        // Crear un nombre de archivo con la fecha actual
        string fechaActual = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string nombreArchivo = $"CodigoQR_{fechaActual}";

        SaveFileDialog saveDialog = new SaveFileDialog
        {
            Filter = "Imagen PNG (*.png)|*.png",
            Title = "Guardar Código QR",
            FileName = nombreArchivo
        };

        if (saveDialog.ShowDialog() == true)
        {
            try
            {
                // Obtener el tamaño seleccionado
                int size = 500; // Valor predeterminado
                if (cmbSize.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag != null)
                {
                    if (int.TryParse(selectedItem.Tag.ToString(), out int selectedSize))
                    {
                        size = selectedSize;
                    }
                }

                // Si queremos usar el mismo tamaño que vemos en pantalla
                if (qrBitmap.PixelWidth != size)
                {
                    // Regenerar el QR con el tamaño seleccionado para guardar
                    string content = txtQRContent.Text;

                    var writer = new BarcodeWriterPixelData
                    {
                        Format = BarcodeFormat.QR_CODE,
                        Options = new QrCodeEncodingOptions
                        {
                            Height = size,
                            Width = size,
                            Margin = 1,
                            ErrorCorrection = ErrorCorrectionLevel.H
                        }
                    };

                    var pixelData = writer.Write(content);
                    var highResQrBitmap = BitmapSource.Create(
                        pixelData.Width,
                        pixelData.Height,
                        96,
                        96,
                        PixelFormats.Bgr32,
                        null,
                        pixelData.Pixels,
                        pixelData.Width * 4);

                    using (FileStream stream = new FileStream(saveDialog.FileName, FileMode.Create))
                    {
                        PngBitmapEncoder encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(highResQrBitmap));
                        encoder.Save(stream);
                    }
                }
                else
                {
                    // Usar el bitmap actual si ya tiene el tamaño deseado
                    using (FileStream stream = new FileStream(saveDialog.FileName, FileMode.Create))
                    {
                        PngBitmapEncoder encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(qrBitmap));
                        encoder.Save(stream);
                    }
                }

                MessageBox.Show($"Código QR guardado correctamente en tamaño {size}x{size}.", "Guardado Exitoso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar el código QR: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void btnClear_Click(object sender, RoutedEventArgs e)
    {
        txtQRContent.Text = string.Empty;
        imgQRCode.Source = null;
        qrBitmap = null;
        btnSave.IsEnabled = false;
    }
}
