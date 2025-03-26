using Microsoft.Win32;
using QR.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows;

namespace QR.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly QrCodeModel _qrCodeModel;

        private string _qrContent;
        private BitmapSource _qrImage;
        private int _selectedSize;
        private string _statusMessage;
        private bool _canSave;

        public MainViewModel()
        {
            _qrCodeModel = new QrCodeModel();

            QrContent = "";
            SelectedSize = 700;
            StatusMessage = "Listo para generar códigos QR";
            CanSave = false;
            
            AvailableSizes = new List<int> { 200, 300, 400, 500, 600, 700, 800, 1000 };

            GenerateQrCommand = new RelayCommand(GenerateQr, CanGenerateQr);
            SaveQrCommand = new RelayCommand(SaveQr, CanSaveQr);
            ClearQrCommand = new RelayCommand(ClearQr);
        }

        public string QrContent
        {
            get => _qrContent;
            set
            {
                if (_qrContent != value)
                {
                    _qrContent = value;
                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public BitmapSource QrImage
        {
            get => _qrImage;
            set
            {
                if (_qrImage != value)
                {
                    _qrImage = value;
                    OnPropertyChanged();
                    CanSave = value != null;
                }
            }
        }

        public int SelectedSize
        {
            get => _selectedSize;
            set
            {
                if (_selectedSize != value)
                {
                    _selectedSize = value;
                    OnPropertyChanged();
                }
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool CanSave
        {
            get => _canSave;
            set
            {
                if (_canSave != value)
                {
                    _canSave = value;
                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public List<int> AvailableSizes { get; }

        public ICommand GenerateQrCommand { get; }
        public ICommand SaveQrCommand { get; }
        public ICommand ClearQrCommand { get; }

        private bool CanGenerateQr(object parameter)
        {
            return !string.IsNullOrWhiteSpace(QrContent);
        }

        private void GenerateQr(object parameter)
        {
            try
            {
                QrImage = _qrCodeModel.GenerateQrCode(QrContent, SelectedSize);
                StatusMessage = $"Código QR generado con éxito (Tamaño: {SelectedSize}x{SelectedSize})";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error al generar el código QR: {ex.Message}";
                MessageBox.Show($"Error al generar el código QR: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanSaveQr(object parameter)
        {
            return CanSave;
        }

        private void SaveQr(object parameter)
        {
            try
            {
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
                    BitmapSource qrToSave = QrImage;
                    if (QrImage.PixelWidth != SelectedSize)
                    {
                        qrToSave = _qrCodeModel.GenerateQrCode(QrContent, SelectedSize);
                    }

                    _qrCodeModel.SaveQrCodeToFile(saveDialog.FileName, qrToSave);
                    StatusMessage = $"Código QR guardado exitosamente en {saveDialog.FileName}";
                    MessageBox.Show($"Código QR guardado correctamente en tamaño {SelectedSize}x{SelectedSize}.",
                                    "Guardado Exitoso",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error al guardar el código QR: {ex.Message}";
                MessageBox.Show($"Error al guardar el código QR: {ex.Message}",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private void ClearQr(object parameter)
        {
            QrContent = string.Empty;
            QrImage = null;
            StatusMessage = "Se ha limpiado el contenido";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
