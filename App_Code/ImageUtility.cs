using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

public class ImageUtility
{
	private static byte[] noImageStub = null;

	private ImageUtility()
	{
	}

	public static byte[] GetNoImageStub()
	{
		if (noImageStub != null)
			return noImageStub;

		Image img = DrawText("No image", new Font(FontFamily.GenericSansSerif, 20), Color.DarkGray, Color.LightGray);
		ImageConverter converter = new ImageConverter();
		noImageStub = (byte[])converter.ConvertTo(img, typeof(byte[]));

		return noImageStub;
	}

	private static Image DrawText(String text, Font font, Color textColor, Color backColor)
	{
		Image img = new Bitmap(1, 1);
		Graphics drawing = Graphics.FromImage(img);

		SizeF textSize = drawing.MeasureString(text, font);

		img.Dispose();
		drawing.Dispose();

		int width = (int)textSize.Width + 40;
		int height = width;

		img = new Bitmap(width, width);

		drawing = Graphics.FromImage(img);
		drawing.Clear(backColor);

		Brush textBrush = new SolidBrush(textColor);

		drawing.DrawString(text, font, textBrush, Convert.ToInt32((width - (int)textSize.Width) / 2), Convert.ToInt32((width - (int)textSize.Height) / 2));
		drawing.Save();

		textBrush.Dispose();
		drawing.Dispose();

		return img;
	}
}