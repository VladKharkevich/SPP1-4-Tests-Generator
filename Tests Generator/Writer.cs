using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Tests_Generator
{
    public class Writer
    {
        private string fFolder;
        public Writer(string folder)
        {
            fFolder = folder;
        }
        public async Task WritetoFile(string text)
        {
            string filename = ".cs";
            int index = text.IndexOf("public class");
            if (index != -1)
            {
                index += 13;
                string temp = "";
                while (index < text.Length && text[index] != '\r')
                {
                    temp += text[index];
                    index++;
                }
                filename = temp + filename;
            }
            using (StreamWriter sw = new StreamWriter(fFolder + "/" + filename))
            {
                await sw.WriteAsync(text);
            }
        }
    }
}
