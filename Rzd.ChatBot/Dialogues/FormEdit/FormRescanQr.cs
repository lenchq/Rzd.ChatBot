using System.Text.RegularExpressions;
using ImageMagick;
using Microsoft.Extensions.Options;
using OpenCvSharp;
using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Attributes;
using Rzd.ChatBot.Types.Enums;
using Rzd.ChatBot.Types.Options;

namespace Rzd.ChatBot.Dialogues;

public class FormRescanQr : PhotoOrActionDialogue
{
    public override bool SupportsPhotoData => true;

    private QrOptions _qrOptions = null!;
    private readonly QRCodeDetector _qrDetector = new QRCodeDetector();
    private readonly Regex _trainDataRegex = new(@"^(\d\d\d[А-Я])(\d\d?\d?)$");
    
    private Mat _image = null!;
    private string _qrContent = string.Empty;
    
    public override State State => State.FormRescanQr;
    
    public override IEnumerable<BotAction> Options { get; set; }

    public FormRescanQr() : base("scanQr")
    {
        Options = new BotAction[] {GoBack};
    }

    [OptionIndex(0)]
    private ValueTask<State> GoBack(Context ctx) => new(State.MyFormEdited);

    protected override void Initialized()
    {
        _qrOptions = this.GetService<IOptions<QrOptions>>().Value;
    }
    public override ValueTask<State> ProceedInput(Context ctx, Photo photo)
    {
        var dataMatch = _trainDataRegex.Match(_qrContent);
        if (dataMatch.Success)
        {
            ctx.UserForm.TrainNumber = dataMatch.Groups[1].Value;
            ctx.UserForm.Seat = int.Parse(dataMatch.Groups[2].Value);
        }

        return ValueTask.FromResult(State.FormEditConfirmQr);
    }
    protected override bool ValidatePhoto(Context ctx, Photo photo)
    {
        ProcessImage((PhotoData) photo);
        if (_qrDetector.Detect(_image, out var qrPoints) &&
            _qrDetector.Decode(_image, qrPoints) is { } qrEncryptedData &&
            !string.IsNullOrEmpty(qrEncryptedData))
        {
            try
            {
                _qrContent = DecryptContent(qrEncryptedData);
            }
            catch(Exception ex)
            {
                if (Logger.IsEnabled(LogLevel.Trace))
                    Logger.LogTrace(ex, "Something went wrong when decrypting qr data");
                return false;
            }
        }
        else
        {
            if (Logger.IsEnabled(LogLevel.Trace))
                Logger.LogTrace("Qr not found");
            return false;
        }

        if (!_trainDataRegex.IsMatch(_qrContent))
        {
            if (Logger.IsEnabled(LogLevel.Trace))
                Logger.LogTrace("Unexpected qr data, {QrContent}", _qrContent);
            return false;
        }

        return true;
    }

    private string DecryptContent(string qrContent)
    {
        // get data cut after = ( https://t.me/rzdchatbot?start=code_here )
        var data = qrContent[(qrContent.IndexOf('=') + 1)..];
        return Crypto.AES.Decrypt(data, _qrOptions.SecretKey);
    }

    private void ProcessImage(PhotoData photoData)
    {
        var magickImage = new MagickImage(photoData.FileData);
        ReadOnlyMemory<byte> imageBytes = magickImage.ToByteArray(MagickFormat.Bmp);
        
        _image = Cv2.ImDecode(imageBytes.Span, ImreadModes.Grayscale)
            .GaussianBlur(new Size(3, 3), 0)
            .Threshold(0, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);
    }
}