using GeneratorTestClassesLib;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows;

namespace Tests_Generator
{
    public class Сonveyor
    {
        private List<string> fFilenames;
        private string fFolder;
        private int fMaxDegreeOfParallelism;

        public Сonveyor(List<string> filenames, string folder, int maxDegreeOfParallelism)
        {
            fFilenames = filenames;
            fFolder = folder;
            fMaxDegreeOfParallelism = maxDegreeOfParallelism;
        }
        public async Task Start()
        {
            var reader = new TransformBlock<string, string>(filename => ReadFromFile(filename), 
                                                            new ExecutionDataflowBlockOptions{
                                                                MaxDegreeOfParallelism = fMaxDegreeOfParallelism
                                                            });
            var transformer = new TransformManyBlock<string, string>(data => GeneratorTestClasses.Start(data),
                                                            new ExecutionDataflowBlockOptions
                                                            {
                                                                MaxDegreeOfParallelism = fMaxDegreeOfParallelism
                                                            });
            var writer = new ActionBlock<string>(text => WritetoFile(text),
                                                            new ExecutionDataflowBlockOptions
                                                            {
                                                                MaxDegreeOfParallelism = fMaxDegreeOfParallelism
                                                            });

            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };
            reader.LinkTo(transformer, linkOptions);
            transformer.LinkTo(writer, linkOptions);


            foreach (string filename in fFilenames)
            {
                await reader.SendAsync(filename);
            }
            reader.Complete();      
            writer.Completion.Wait();
        }

        private async Task WritetoFile(string text)
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
            using (StreamWriter sw  = new StreamWriter(fFolder + "/" + filename))
            {   
               await sw.WriteAsync(text);
            }
        }

        private async Task<string> ReadFromFile(string filename)
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
