﻿using System.IO;

namespace S4GFXLibrary.FileReader
{
    public class DilFileReader : FileReaderBase
    {
        int[] offsetTable;

        public int ReverseLookupOffset(int gilIndex)
        {
            int offset = gilIndex * 4 + HeaderSize;

            int lastGood = 0;

            for (int i = 0; i < offsetTable.Length; i++)
            {
                if (offsetTable[i] == 0)
                    continue;

                if (offsetTable[i] > offset)
                {
                    //Console.WriteLine($"DIL index:{gilIndex} --> dil:{lastGood} at offset:{offset}");
                    return lastGood;
                }

                lastGood = i;
            }

            //Console.WriteLine($"Unable to find offset gilIndex: {gilIndex}");
            return lastGood;
        }

        public void FakeLookupOffset(int length, JilFileReader jil)
        {
            offsetTable = new int[length];

            for (int i = 0; i < offsetTable.Length; i++)
            {
                offsetTable[i] = i * 4 + HeaderSize;
            }
        }

        public DilFileReader(BinaryReader reader)
        {
            ReadResource(reader);

            int imageCount = GetImageCount(reader);
            //Console.WriteLine($"Image count: {imageCount}");

            offsetTable = new int[imageCount];

            for (int i = 0; i < imageCount; i++)
            {
                offsetTable[i] = reader.ReadInt32();
            }
        }
        override public byte[] GetData()
        {
            byte[] data = new byte[offsetTable.Length * 4 + HeaderSize];

            using (BinaryWriter writer = new BinaryWriter(new MemoryStream(data)))
            {
                writer.Write(GetHeaderData());
                //magic = startReader.ReadInt32();
                //flag1 = startReader.ReadInt32();
                //flag2 = startReader.ReadInt32();
                //flag3 = startReader.ReadInt32();
                //flag4 = startReader.ReadInt32();

                writer.Seek(HeaderSize, SeekOrigin.Begin);

                for (int i = 0; i < offsetTable.Length; i++)
                {
                    writer.Write(offsetTable[i]);
                }
            }

            return data;
        }
    }
}
