using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Tests_Generator
{
    public class Reader
    {
        public async Task<string> ReadFromFile(string filename)
        {
            string result = "";
            using (StreamReader sr = new StreamReader(filename))
            {
                result = await sr.ReadToEndAsync();
            }
            return result;
        }
    }
}
