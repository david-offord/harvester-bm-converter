using System;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Collections.Generic;

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
                        //get current pallete location
                        byte paletteByte = imageBytes[currentByteLocation];

                        Color color = GetPaletteColoring(paletteByte, ref palleteBytes);
                        bm1.SetPixel(column, row, color);
                        currentByteLocation++;
                    }
                }

                bm1.Save(saveLocation);
            }

        }


        public static void ConvertAnimatedImage(string abmFile, string palFile, string saveFile)
        {
            var imageBytes = System.IO.File.ReadAllBytes(abmFile);

            //sometimes there are just empty files
            if (imageBytes.Length == 0)
                return;

            var palleteBytes = System.IO.File.ReadAllBytes(palFile);
            var abmTmpFiles = Path.GetFileName(abmFile) + new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            if (Directory.Exists("./tmp") == false)
                Directory.CreateDirectory("./tmp");

            List<Color> palColors = new List<Color>();
            for (int i = 0; i < palleteBytes.Length; i += 3)
            {
                palColors.Add(Color.FromArgb(255, palleteBytes[i], palleteBytes[i + 1], palleteBytes[i + 2]));
            }

            int currentByteLocation = 0;//0x00

            uint cellCount = BitConverter.ToUInt32(imageBytes.Take(4).ToArray(), 0);
            currentByteLocation += 4;// 0x04

            //I won't need this. But this is at this locaiton
            uint largestFrameByteCount = BitConverter.ToUInt32(imageBytes.Skip(currentByteLocation).Take(4).ToArray(), 0);
            currentByteLocation += 4;// 0x08
            int cell = 0;

            //for every cell in the .abm file
            for (cell = 0; cell < cellCount; cell++)
            {
                //I won't need this. But this is at this locaiton
                uint cellPaddingX = BitConverter.ToUInt32(imageBytes.Skip(currentByteLocation).Take(4).ToArray(), 0);
                currentByteLocation += 4;// 0x0C
                //I won't need this. But this is at this locaiton
                uint cellPaddingY = BitConverter.ToUInt32(imageBytes.Skip(currentByteLocation).Take(4).ToArray(), 0);
                currentByteLocation += 4;// 0x10

                //width and height of cell is found in header
                uint cellWidth = BitConverter.ToUInt32(imageBytes.Skip(currentByteLocation).Take(4).ToArray(), 0);
                currentByteLocation += 4;// 0x14
                uint cellHeight = BitConverter.ToUInt32(imageBytes.Skip(currentByteLocation).Take(4).ToArray(), 0);
                currentByteLocation += 4;// 0x18

                //check if the animation is compressed. 1 = yes, RLE. 0 = no
                byte compressed = imageBytes.Skip(currentByteLocation).FirstOrDefault();
                currentByteLocation += 1;//0x19

                //payload length is the payload while still compressed
                uint payloadLength = BitConverter.ToUInt32(imageBytes.Skip(currentByteLocation).Take(4).ToArray(), 0);
                currentByteLocation += 4;//0x1D

                //ignore this completely
                uint palettePointer = BitConverter.ToUInt32(imageBytes.Skip(currentByteLocation).Take(4).ToArray(), 0);
                currentByteLocation += 4;//0x22


                //if its compressed, its a different process
                if (compressed == 1)
                {
                    //take the payload.
                    var cellBytes = imageBytes.Skip(currentByteLocation).Take((int)payloadLength).ToArray();
                    currentByteLocation += (int)payloadLength;//skip bbytes already read

                    //this is an array of uncompressed bytes
                    List<byte> newCellBytes = new List<byte>();

                    //number of bytes read for this specific cell
                    int cellBytesRead = 0;

                    //for every byte in the payload
                    while (cellBytesRead < cellBytes.Length)
                    {
                        byte controlByte = cellBytes[cellBytesRead];//get first byte
                        cellBytesRead++;

                        bool repetition = IsBitSet(controlByte, 7);//check if the most significant bit is set

                        //if the first byte is 1, the remaining bytes follow that previous one
                        if (repetition)
                        {
                            int numOfRepeats = (byte)(controlByte ^ (1 << 7));//flip the first byte from 1 to 0. the remaining number gives us how many bytes to repeat
                            byte repeatingByte = cellBytes[cellBytesRead];//read the repeating byte
                            cellBytesRead++;

                            //for each repeat, add that to the new cell
                            for (int i = 0; i < numOfRepeats; i++)
                            {
                                newCellBytes.Add(repeatingByte);
                            }
                        }
                        else
                        {
                            int numOfLiteralReads = (int)(controlByte);//if it isn't a repition, we are supposed to rad the next x bytes literally

                            for (int i = 0; i < numOfLiteralReads; i++)
                            {
                                newCellBytes.Add(cellBytes[cellBytesRead]);
                                cellBytesRead++;
                            }

                        }

                    }

                    //now that the payload is decompressed, create the image
                    using (Bitmap bm1 = new Bitmap((int)cellWidth, (int)cellHeight))
                    {
                        //do the same process of going byte by byte to determine color based on the palette file
                        int newCellLocation = 0;
                        //basically go row by row and fill in the colors
                        for (int row = 0; row < cellHeight; row++)
                        {
                            //go column by column
                            for (int column = 0; column < cellWidth; column++)
                            {
                                //get current pallete location
                                byte paletteByte = newCellBytes[newCellLocation];

                                bm1.SetPixel(column, row, palColors[paletteByte]);
                                newCellLocation++;
                            }
                        }

                        //delete any existing files
                        if (File.Exists(Path.Combine("./tmp", abmTmpFiles + cell + ".png")))
                            File.Delete(Path.Combine("./tmp", abmTmpFiles + cell + ".png"));

                        bm1.Save(Path.Combine("./tmp", abmTmpFiles + cell + ".png"), ImageFormat.Png);//, ImageFormat.Png);

                    }

                }
                else
                {
                    //just get the image in the normal way with no RLE
                    using (Bitmap bm1 = new Bitmap((int)cellWidth, (int)cellHeight))
                    {
                        //for every row, get the column color color
                        for (int row = 0; row < cellHeight; row++)
                        {
                            //for every column, get the color
                            for (int col = 0; col < cellWidth; col++)
                            {
                                //get current pallete location
                                byte paletteByte = imageBytes[currentByteLocation];

                                bm1.SetPixel(col, row, palColors[paletteByte]);
                                currentByteLocation++;
                            }
                        }

                        bm1.Save(Path.Combine("./tmp", abmTmpFiles + cell + ".png"), ImageFormat.Png);//, ImageFormat.Png);
                    }
                }

            }

            using (GifWriter gw = new GifWriter(saveFile))
            {
                for (int i = 0; i < cellCount; i++)
                {
                    using (Image img = Image.FromFile(Path.Combine("./tmp", abmTmpFiles + i + ".png")))
                    {
                        // Add first image and set the animation delay to 100ms
                        gw.WriteFrame(img, 100);
                    }
                    File.Delete(Path.Combine("./tmp", abmTmpFiles + i + ".png"));//delete the tmp file
                }
            }

        }


        private static Color GetPaletteColoring(byte location, ref byte[] palleteBytes)
        {
            Color color;
            int r = 0;
            int g = 0;
            int b = 0;

            //pallete is in 3s. Multiply it and then go to that location
            location *= 3;

            r = (int)palleteBytes[location];
            g = (int)palleteBytes[location + 1];
            b = (int)palleteBytes[location + 2];
            color = Color.FromArgb(r, g, b);

            return color;
        }

        static bool IsBitSet(byte b, int pos)
        {
            return (b & (1 << pos)) != 0;
        }

    }
}
