using System;
using System.Linq;
using System.Drawing;

namespace Harvester_image_ui
{
    class HarvesterImageConverter
    {
        public static void ConvertImage(string bmFileLocation, string paletteLocation, string saveLocation)
        {
            var imageBytes = System.IO.File.ReadAllBytes(bmFileLocation);
            var palleteBytes = System.IO.File.ReadAllBytes(paletteLocation);

            int currentByteLocation = 0;

            var width = BitConverter.ToUInt32(imageBytes.Take(4).ToArray(), 0);
            currentByteLocation += 4;

            var height = BitConverter.ToUInt32(imageBytes.Skip(currentByteLocation).Take(4).ToArray(), 0);
            currentByteLocation += 4;

            //skip over the pointer to paletter file\
            currentByteLocation += 4;

            using (Bitmap bm1 = new Bitmap((int)width, (int)height))
            {
                //basically go row by row and fill in the colors
                for (int row = 0; row < height; row++)
                {
                    //go column by column
                    for (int column = 0; column < width; column++)
                    {
                        Color color;
                        int r = 0;
                        int g = 0;
                        int b = 0;

                        //get current pallete location
                        int palettelocation = (int)imageBytes[currentByteLocation];
                        //pallete is in 3s. Multiply it and then go to that location
                        palettelocation *= 3;
                        r = (int)palleteBytes[palettelocation];
                        g = (int)palleteBytes[palettelocation + 1];
                        b = (int)palleteBytes[palettelocation + 2];


                        color = Color.FromArgb(r, g, b);
                        bm1.SetPixel(column, row, color);
                        currentByteLocation++;
                    }
                }

                bm1.Save(saveLocation);
            }

            Console.WriteLine(imageBytes);
        }
    }
}
