using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileBrowserNP.Helpers
{
    public static class GetYieldText
    {
        static BinaryReader _br;

        static StreamReader _sr;

        public static readonly string[] Digits = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f" };
        public static IEnumerator<string> GetHexLinesFromFile(string path)
        {
            try
            {
                _br = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None, 32));

                StringBuilder sb = new StringBuilder();
                
                byte[] inbuff = new byte[0];
                int b = 0;
                int count = 0;
                while ((inbuff = _br.ReadBytes(16)).Length > 0)
                {

                    sb.Append($@"{count.ToString("X8")}   ");
                    count += 16;

                    for (b = 0; b < inbuff.Length - 1; b++)
                        sb.Append(Digits[(inbuff[b] / 16) % 16] + Digits[inbuff[b] % 16] + " ");

                    sb.Append(Digits[(inbuff[b] / 16) % 16] + Digits[inbuff[b] % 16]);
                    
                    yield return sb.ToString();
                    sb.Clear();
                }
            }
            finally
            {
                CloseBinaryReader();
            }
        }

        public static IEnumerator<string> GetLinesFromFile(string path)
        {
            try
            {
                _sr = new StreamReader(path, Encoding.UTF8, true);

                StringBuilder sb = new StringBuilder();
                string? line = string.Empty;
                while ((line = _sr.ReadLine()) != null)
                    sb.AppendLine(line);

                yield return sb.ToString();
                sb.Clear();
            }
            finally
            {
                CloseStreamReader();
            }
        }

        public static void CloseBinaryReader() // вызывается из HexViewModel при переключении на другой файл
        {
            if(_br != null)
                _br.Close();
        }
        public static void CloseStreamReader() // вызывается из TextViewModel при переключении на другой файл
        {
            if (_sr != null)
                _sr.Close();
        }
    }
}
