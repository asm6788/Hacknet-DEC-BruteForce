using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hacknet_DEC_BruteForce
{
    class Program
    {
        private static ushort GetPassCodeFromString(string code) => ((ushort)code.GetHashCode());
        public static string[] robustNewlineDelim = new string[] { "\r\n", "\n" };
        private static string[] HeaderSplitDelimiters = new string[] { "::" };

        static void Main(string[] args)
        {
            List<ushort> KeyLIst = new List<ushort>();
            string[] temp = null;
            string code = @""; //DEC CODE
            string[] gen = null;
            for (ushort i =0; i != ushort.MaxValue;i++)
            {
                gen = DecryptString(code, i);
                if (gen[2] != "" && gen[2] != null)
                {
                    KeyLIst.Add(i);
                    try
                    {
                        Console.WriteLine(i);
                        Console.WriteLine("Header: " +gen[0]);
                        Console.WriteLine("Signed by: " + gen[1]);
                        Console.WriteLine(Encoding.UTF8.GetString(DecryptString(code, i)[2].Split('-').Select(b => Convert.ToByte(b, 16)).ToArray()));
                    }
                    catch
                    {
                        Console.WriteLine(i + ": " + gen[2]);
                    }
                }
                temp = gen;
            }
            Console.WriteLine("End");
            foreach(ushort key in KeyLIst)
            {
                Console.Write(key+",");
            }
            Console.WriteLine("Number of collisions:" + KeyLIst.Count);
            Console.Read();
        }
        private static string[] DecryptString(string data, ushort pass)
        {
            string[] ret = new string[5];
            ushort passcode = pass;
            ushort emptypasscode = GetPassCodeFromString("");

            string[] split = data.Split(robustNewlineDelim, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length < 2)
            {
                return new string[1] { pass.ToString() };
            }
            string[] headersSplit = split[0].Split(HeaderSplitDelimiters, StringSplitOptions.None);
            if (headersSplit.Length < 4) throw new FormatException("Tried to decrypt an invalid valid DEC ENC file, aborting.");

            string headerMsg = Decrypt(headersSplit[1], emptypasscode);
            string sign = Decrypt(headersSplit[2], emptypasscode);
            string check = Decrypt(headersSplit[3], passcode);
            string fileExtension = null;
            if (headersSplit.Length > 4) fileExtension = Decrypt(headersSplit[4], emptypasscode);
            string message;
            string passValid = "true";

            if (check == "ENCODED")
            {
                message = Decrypt(split[1], passcode);
            }
            else
            {
                headerMsg = null;
                sign = null;
                message = null;
                passValid = "false";
            }

            ret[0] = headerMsg;
            ret[1] = sign;
            ret[2] = message;
            ret[3] = fileExtension;
            ret[4] = passValid;
            return ret;
        }
        public static char[] spaceDelim = new char[] { ' ' };
        private static string Decrypt(string data, ushort passcode)
        {
            StringBuilder builder = new StringBuilder();
            string[] strArray = data.Split(spaceDelim, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < strArray.Length; i++)
            {
                int num2 = Convert.ToInt32(strArray[i]);
                int num3 = 0x7fff;
                int num4 = (num2 - num3) - passcode;
                num4 /= 0x71e;
                builder.Append((char)num4);
            }
            return builder.ToString().Trim();
        }

    }
}
