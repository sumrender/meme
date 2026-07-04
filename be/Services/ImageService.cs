using SkiaSharp;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.Collections.Concurrent;
using System.Text.Json;

namespace Backend.Services
{
    public class ImageService
    {
        private readonly Cloudinary _cloudinary;

        public ImageService(IConfiguration config)
        {
            var cloudName = config.GetValue<string>("CLOUDINARY_CLOUD_NAME");
            var apiKey = config.GetValue<string>("CLOUDINARY_API_KEY");
            var apiSecret = config.GetValue<string>("CLOUDINARY_API_SECRET");

            if (string.IsNullOrEmpty(cloudName) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret))
            {
                throw new ArgumentException("Cloudinary configuration (CLOUDINARY_CLOUD_NAME, CLOUDINARY_API_KEY, CLOUDINARY_API_SECRET) is not fully set in appsettings or environment variables.");
            }

            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
        }

        public byte[] LoadImage(string imagePath)
        {
            if (!File.Exists(imagePath))
            {
                throw new FileNotFoundException("The requested image could not be found.", imagePath);
            }

            return File.ReadAllBytes(imagePath);
        }

        public byte[] AddCaptionToImage(string imagePath, string caption, int fontSize = 24, string colorHex = "#FFFFFF")
        {
            if (!File.Exists(imagePath))
            {
                throw new FileNotFoundException("The requested image could not be found.", imagePath);
            }

            using var inputStream = File.OpenRead(imagePath);
            using var originalImage = SKBitmap.Decode(inputStream);

            var imageWidth = originalImage.Width;
            var imageHeight = originalImage.Height;

            using var surface = SKSurface.Create(new SKImageInfo(imageWidth, imageHeight));
            var canvas = surface.Canvas;

            canvas.DrawBitmap(originalImage, 0, 0);

            var font = new SKFont(SKTypeface.Default, fontSize);
            var paint = new SKPaint
            {
                Color = SKColor.Parse(colorHex),
                IsAntialias = true,
            };

            // Split the caption into lines if necessary
            var words = caption.Split(' ');
            var lines = new List<string>();
            var currentLine = "";

            foreach (var word in words)
            {
                var testLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
                if (font.MeasureText(testLine, paint) > imageWidth - 20) // 20px padding
                {
                    lines.Add(currentLine);
                    currentLine = word;
                }
                else
                {
                    currentLine = testLine;
                }
            }

            if (!string.IsNullOrEmpty(currentLine))
            {
                lines.Add(currentLine);
            }

            // Calculate initial Y position for multi-line text
            var totalTextHeight = lines.Count * fontSize + (lines.Count - 1) * 5; // 5px line spacing
            var yPosition = imageHeight - totalTextHeight - 10; // 10px padding from bottom

            foreach (var line in lines)
            {
                var textWidth = font.MeasureText(line, paint);
                var xPosition = (imageWidth - textWidth) / 2; // Center the text horizontally

                canvas.DrawText(line, xPosition, yPosition, SKTextAlign.Left, font, paint);
                yPosition += fontSize + 5; // Move to the next line
            }

            using var outputStream = new MemoryStream();
            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);

            data.SaveTo(outputStream);
            return outputStream.ToArray();
        }

        public async Task<string[]> UploadImagesAsync(List<byte[]> images)
        {
            var uploadTasks = images.Select(async imageBytes =>
            {
                using var memoryStream = new MemoryStream(imageBytes);

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription("image.png", memoryStream),
                    Folder = "dev_generated_memes"
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                return uploadResult.SecureUrl.ToString();
            });

            return await Task.WhenAll(uploadTasks);
        }

        private async Task<string> UploadImageAsync(string imagePath)
        {
            using var memoryStream = new MemoryStream(File.ReadAllBytes(imagePath));

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(Path.GetFileName(imagePath), memoryStream),
                Folder = "meme_templates"
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl.ToString();
        }

        public async Task UploadImagesFromFolderAsync(string folderName)
        {
            if (!Directory.Exists(folderName))
            {
                throw new DirectoryNotFoundException($"The folder '{folderName}' does not exist.");
            }

            var imageFiles = Directory.GetFiles(folderName, "*.png")
                .Concat(Directory.GetFiles(folderName, "*.jpg"))
                .Concat(Directory.GetFiles(folderName, "*.jpeg"));

            if (!imageFiles.Any())
            {
                throw new ArgumentException("The folder does not contain any images.");
            }

            var uploadTasks = new ConcurrentDictionary<string, Task<string>>();

            foreach (var imagePath in imageFiles)
            {
                var fileName = Path.GetFileNameWithoutExtension(imagePath);
                uploadTasks[fileName] = UploadImageAsync(imagePath);
            }

            await Task.WhenAll(uploadTasks.Values);

            var result = uploadTasks.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Result
            );

            var memesJsonPath = Path.Combine(folderName, "memes.json");
            await File.WriteAllTextAsync(memesJsonPath, JsonSerializer.Serialize(result));
        }
    }
}
