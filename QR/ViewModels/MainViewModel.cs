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
using System.Windows.Threading;

namespace QR.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly QrCodeModel _qrCodeModel;
        private DispatcherTimer _debounceTimer;

        private string _qrContent;
        private BitmapSource _qrImage;
        private int _selectedSize;
        private string _statusMessage;
        private bool _canSave;
        private bool _isGenerating;

        public MainViewModel()
        {
            _qrCodeModel = new QrCodeModel();

            _debounceTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            _debounceTimer.Tick += async (s, e) =>
            {
                _debounceTimer.Stop();
                if (!string.IsNullOrWhiteSpace(QrContent))
                {
                    await GenerateQrAsync();
                }
            };

            QrContent = "";
            SelectedSize = 700;
            StatusMessage = "Ingresa texto para generar un código QR";
            CanSave = false;
            IsGenerating = false;

            AvailableSizes = new List<int> { 200, 300, 400, 500, 600, 700, 800, 1000 };

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

                    _debounceTimer.Stop();
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        IsGenerating = true;
                        StatusMessage = "Generando código QR...";
                        _debounceTimer.Start();
                    }
                    else
                    {
                        QrImage = null;
                        IsGenerating = false;
                        StatusMessage = "Ingresa texto para generar un código QR";
                    }
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

                    if (!string.IsNullOrWhiteSpace(QrContent))
                    {
                        _debounceTimer.Stop();
                        IsGenerating = true;
                        StatusMessage = "Actualizando tamaño del código QR...";
                        _debounceTimer.Start();
                    }
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

        public bool IsGenerating
        {
            get => _isGenerating;
            set
            {
                if (_isGenerating != value)
                {
                    _isGenerating = value;
                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public List<int> AvailableSizes { get; }

        public ICommand SaveQrCommand { get; }
        public ICommand ClearQrCommand { get; }

        private async Task GenerateQrAsync()
        {
            try
            {
                IsGenerating = true;

                // Simular un pequeño delay para mejor feedback visual
                await Task.Delay(100);

                QrImage = _qrCodeModel.GenerateQrCode(QrContent, SelectedSize);

                IsGenerating = false;
                StatusMessage = $"Código QR generado ({SelectedSize}x{SelectedSize}px) - {QrContent.Length} caracteres";
            }
            catch (Exception ex)
            {
                IsGenerating = false;
                StatusMessage = $"Error: {ex.Message}";
            }
        }

        private bool CanSaveQr(object parameter)
        {
            return CanSave && !IsGenerating;
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
                    StatusMessage = $"Guardado exitosamente: {System.IO.Path.GetFileName(saveDialog.FileName)}";
                    MessageBox.Show($"Código QR guardado correctamente en tamaño {SelectedSize}x{SelectedSize}px.",
                                    "Guardado Exitoso",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error al guardar: {ex.Message}";
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
            IsGenerating = false;
            StatusMessage = "Contenido limpiado";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}