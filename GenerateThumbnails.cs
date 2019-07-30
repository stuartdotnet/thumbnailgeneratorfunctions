using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

namespace ThumbnailGenerator
{
	public static class GenerateThumbnails
	{
		[FunctionName("GenerateThumbnails")]
		public static void Run([BlobTrigger("galleryupload/{name}", Connection = "AzureWebJobStorage")]Stream blob, string name, ILogger log,
			[Blob("gallery/1920/{name}", FileAccess.Write, Connection="AzureWebJobStorage")] Stream outputBlob,
			[Blob("gallery/670/{name}", FileAccess.Write, Connection = "AzureWebJobStorage")] Stream outputBlob2,
			[Blob("gallery/292/{name}", FileAccess.Write, Connection = "AzureWebJobStorage")] Stream outputBlob3)
		{
			IImageDecoder decoder = new JpegDecoder();
			using (Image<Rgba32> image = Image.Load(blob, decoder))
			{
				image.Mutate(x => x
						.Resize(new ResizeOptions
						{
							Mode = ResizeMode.Max,
							Size = GetSize(image, 1920)
			}).BackgroundColor(new Rgba32(0, 0, 0)));

				image.SaveAsJpeg(outputBlob);
			}

			using (Image<Rgba32> image = Image.Load(blob, decoder))
			{
				image.Mutate(x => x
						.Resize(new ResizeOptions
						{
							Mode = ResizeMode.Max,
							Size = GetSize(image, 640)
						}).BackgroundColor(new Rgba32(0, 0, 0)));

				image.SaveAsJpeg(outputBlob2);
			}

			using (Image<Rgba32> image = Image.Load(blob, decoder))
			{
				image.Mutate(x => x
						.Resize(new ResizeOptions
						{
							Mode = ResizeMode.Max,
							Size = GetSize(image, 320)
						}).BackgroundColor(new Rgba32(0, 0, 0)));

				image.SaveAsJpeg(outputBlob3);
			}

			log.LogInformation($"C# Blob trigger function Processed blob\n {name} \n Size: {blob.Length} Bytes");
		}

		private static Size GetSize(Image<Rgba32> originalImage, int thumbnailSize)
		{
			var size = new Size();

			if (originalImage.Width > originalImage.Height)
			{
				size.Width = thumbnailSize;
				size.Height = thumbnailSize * originalImage.Height / originalImage.Width;
			}
			else
			{
				size.Height = thumbnailSize;
				size.Width = thumbnailSize * originalImage.Width / originalImage.Height;
			}

			return size;
		}

		//public static void ConvertImageToThumbnailJPG(Stream input, Stream output, int thumbnailsize = 80)
		//{
		//	input.Position = 0;
		//	int width;
		//	int height;
		//	var originalImage = Bitmap.FromStream(input);

		//	if (originalImage.Width > originalImage.Height)
		//	{
		//		width = thumbnailsize;
		//		height = thumbnailsize * originalImage.Height / originalImage.Width;
		//	}
		//	else
		//	{
		//		height = thumbnailsize;
		//		width = thumbnailsize * originalImage.Width / originalImage.Height;
		//	}

		//	Bitmap thumbnailImage = null;
		//	try
		//	{
		//		thumbnailImage = new Bitmap(width, height);

		//		using (Graphics graphics = Graphics.FromImage(thumbnailImage))
		//		{
		//			graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
		//			graphics.SmoothingMode = SmoothingMode.AntiAlias;
		//			graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
		//			graphics.DrawImage(originalImage, 0, 0, width, height);
		//		}

		//		thumbnailImage.Save(output, ImageFormat.Jpeg);
		//	}
		//	finally
		//	{
		//		if (thumbnailImage != null)
		//		{
		//			thumbnailImage.Dispose();
		//		}
		//	}
		//}
	}
}
