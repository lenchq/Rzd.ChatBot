// using System.Drawing;
// using System.Drawing.Imaging;
// using ImageMagick;
// using QRCoder;
// using ZXing;
//
// namespace Rzd.ChatBot.Extensions;
//
// public static class QrGenerator
// {
//     public static Stream GenerateQr(string contents)
//     {
//         // var qr = QRCoder.QRCodeGenerator.GenerateQrCode(contents, QRCodeGenerator.ECCLevel.H);
//         var qr = new ZXing.QrCode.QRCodeWriter().encode(contents, BarcodeFormat.QR_CODE, 256, 256);
//         Bitmap bmp = new Bitmap(256, 256);
//         for (int i = 0; i < qr.Height; i++)
//         {
//             for (int j = 0; j < qr.Width; j++)
//             {
//                 bmp.SetPixel(i,j,qr[i,j] ? Color.Black : Color.White);
//             }
//         }
//         var ms = new MemoryStream();
//         bmp.Save(ms, ImageFormat.Jpeg);
//         return ms;
//     }
// }