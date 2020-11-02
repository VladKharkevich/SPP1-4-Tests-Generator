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
    public class Conveyor
    {
        private List<string> fFilenames;
        private string fFolder;
        private int fMaxDegreeOfParallelism;

        public Conveyor(List<string> filenames, string folder, int maxDegreeOfParallelism)
        {
            fFilenames = filenames;
            fFolder = folder;
            fMaxDegreeOfParallelism = maxDegreeOfParallelism;
        }
        public async Task Start()
        {
            Reader fileReader = new Reader();
            Writer fileWriter = new Writer(fFolder);
            var reader = new TransformBlock<string, string>(filename => fileReader.ReadFromFile(filename), 
                                                            new ExecutionDataflowBlockOptions{
                                                                MaxDegreeOfParallelism = fMaxDegreeOfParallelism
                                                            });
            var transformer = new TransformManyBlock<string, string>(data => GeneratorTestClasses.Start(data),
                                                            new ExecutionDataflowBlockOptions
                                                            {
                                                                MaxDegreeOfParallelism = fMaxDegreeOfParallelism
                                                            });
            var writer = new ActionBlock<string>(text => fileWriter.WritetoFile(text),
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
    }
}
