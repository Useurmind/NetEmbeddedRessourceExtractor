using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NetEmbeddedRessourceExtractor
{
    class Program
    {
        static void Main(string[] args)
        {
            var sourceFolder = args[0];
            var targetFolder = args.Length < 2 ? Directory.GetCurrentDirectory() : args[1];

            if (args.Length < 1)
            {
                Console.WriteLine($"Usage: NetEmbeddedRessourceExtractor <sourceFolder> [<targetFolder>]");
                return;
            }

            if (!Directory.Exists(sourceFolder))
            {
                Console.WriteLine($"ERROR: Source folder {sourceFolder} does not exist");
                return;
            }

            if (!Directory.Exists(targetFolder))
            {
                Console.WriteLine($"Creating target folder {targetFolder}");
                Directory.CreateDirectory(targetFolder);
            }

            Console.WriteLine($"Extracting ressources from all dlls in {sourceFolder} to {targetFolder}");

            var sourceFiles = Directory.GetFiles(sourceFolder, "*.dll");
            foreach (var netAssemblyFile in sourceFiles)
            {
                var assembly = Assembly.LoadFrom(netAssemblyFile);
                foreach (var ressourceName in assembly.GetManifestResourceNames())
                {
                    Console.WriteLine($"Found ressource {ressourceName} in assembly {assembly.FullName}");

                    try
                    {
                        var fileStream = File.Create(Path.Combine(targetFolder, ressourceName));
                        assembly.GetManifestResourceStream(ressourceName)
                                .CopyTo(fileStream);
                        fileStream.Close();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"ERROR: Could not load ressource {ressourceName} from assembly {assembly.FullName}: {e.Message}");
                    }
                }
            }
        }
    }
}
