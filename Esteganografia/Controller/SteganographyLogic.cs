using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;

namespace Steganography.Controller
{
   public class SteganographyLogic
    {

        public int Capacity(byte[] bytes)
        {
            int Size = 0;
            Size = bytes.Length - bytes[10];
            return Size / 8;
        }

        public bool CanBeHidden(byte[] bytes, string Message)
        {
            bool CanBeHidden_ = false;
            if (Message.Length > Capacity(bytes))
                CanBeHidden_ = false;
            else
                CanBeHidden_ = true;
            return CanBeHidden_;
        }

        public string RevealText(string source, bool NoShowGarbage )
        {
            string Message = "";
            /* JPG
            * https://docs.fileformat.com/image/jpeg/
            * 
            * BMP
            * https://en.wikipedia.org/wiki/BMP_file_format
            */
            byte[] bytes = File.ReadAllBytes(source); // JPG starts from byte 255(0xFF) and BMP from 10 (0xA)
            int BytePointer = source.Contains(".bmp")?bytes[10]: bytes[255];

            while (BytePointer < bytes.Length - 2)
            {
                int WhileCounter = 0;
                string ByteWord = "";
                while (WhileCounter < 8)
                {

                    string currentByte = Convert.ToString(bytes[BytePointer], 2).PadLeft(8, '0');
                    ByteWord += currentByte[7];
                    BytePointer += 1;
                    WhileCounter += 1;
                }

                if (NoShowGarbage)
                {
                    if (Convert.ToChar(Convert.ToInt32(ByteWord, 2)) == '~')
                    {
                     
                        return Message;
                    }

                }

                Message += Convert.ToChar(Convert.ToInt32(ByteWord, 2));

            }

            return Message;
        }

        public bool HideText(string source, string dest, string message )
        {


            byte[] bytes = File.ReadAllBytes(source);

            if (!CanBeHidden(bytes, message))
            {
                throw new Exception("Text is too large");
            }
            /* JPG
             * https://docs.fileformat.com/image/jpeg/
             * 
             * BMP
             * https://en.wikipedia.org/wiki/BMP_file_format
             */
            int BytePointer = source.Contains(".bmp") ? bytes[10] : bytes[255];// JPG starts from byte 255(0xFF) and BMP from 10 (0xA)
            string CurrentChar = "";
            int WhileCounter = 0;
            for (int j = 0; j < message.Length; j++)
            {
                int AsciiValue = message[j];
                CurrentChar = Convert.ToString(AsciiValue, 2).PadLeft(8, '0');

                WhileCounter = 0;
                while (WhileCounter < 8)
                {
                    string currentByte = Convert.ToString(bytes[BytePointer], 2).PadLeft(8, '0');
                    string newByte = (currentByte.Substring(0, 7) + CurrentChar[WhileCounter]).PadLeft(8, '0');
                    bytes[BytePointer] = (byte)Convert.ToInt32(newByte, 2);
                    BytePointer += 1;
                    WhileCounter += 1;
                }              
            }

            CurrentChar = Convert.ToString('~', 2).PadLeft(8, '0');
            WhileCounter = 0;
            while (WhileCounter < 8)
            {
                string currentByte = Convert.ToString(bytes[BytePointer], 2).PadLeft(8, '0');
                string newByte = (currentByte.Substring(0, 7) + CurrentChar[WhileCounter]).PadLeft(8, '0');
                bytes[BytePointer] = (byte)Convert.ToInt32(newByte, 2);
                BytePointer += 1;
                WhileCounter += 1;
            }

            System.IO.FileStream _FileStream = new System.IO.FileStream(dest, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            _FileStream.Write(bytes, 0, bytes.Length);
            _FileStream.Close();
            return true;
        }
    }
}
