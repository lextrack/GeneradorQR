using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using ZXing.QrCode.Internal;
using ZXing.QrCode;
using ZXing;

namespace QR.Models
{
    public class QrCodeModel
    {
        public BitmapSource GenerateQrCode(string content, int size, int margin = 1)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentException("El contenido del código QR no puede estar vacío.");
            }

            var writer = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Height = size,
                    Width = size,
                    Margin = margin,
                    ErrorCorrection = ErrorCorrectionLevel.H
                }
            };

            var pixelData = writer.Write(content);

            return BitmapSource.Create(
                pixelData.Width,
                pixelData.Height,
                96,
                96,
                PixelFormats.Bgr32,
                null,
                pixelData.Pixels,
                pixelData.Width * 4);
        }

        public void SaveQrCodeToFile(string filePath, BitmapSource qrBitmap)
        {
            if (qrBitmap == null)
            {
                throw new ArgumentNullException(nameof(qrBitmap), "No hay un código QR para guardar.");
            }

            using (System.IO.FileStream stream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(qrBitmap));
                encoder.Save(stream);
            }
        }
    }
}
