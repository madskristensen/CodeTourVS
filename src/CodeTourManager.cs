using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CodeTourVS
{
    public class CodeTourManager
    {
        public List<CodeTour> Tours { get; private set; } = new List<CodeTour>();

        public static Step CurrentStep { get; set; }

        public static async Task<CodeTourManager> FromFolderAsync(string toursFolder, CancellationToken cancellationToken = default)
        {
            var manager = new CodeTourManager();

            if (Directory.Exists(toursFolder))
            {
                foreach (var file in Directory.EnumerateFiles(toursFolder, $"*{Constants.TourFileExtension}"))
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    CodeTour tour = await manager.LoadTourAsync(file);
                    manager.Tours.Add(tour);
                }
            }

            return manager;
        }

        public static Task<CodeTourManager> FromSourceFileAsync(string sourceFile, CancellationToken cancellationToken = default)
        {
            var parent = new DirectoryInfo(Path.GetDirectoryName(sourceFile));

            while (parent != null)
            {
                foreach (DirectoryInfo dir in parent.GetDirectories($"*{Constants.ToursDirName}", SearchOption.TopDirectoryOnly))
                {
                    if (dir.Name == Constants.ToursDirName)
                    {
                        return FromFolderAsync(dir.FullName, cancellationToken);
                    }
                }

                parent = parent.Parent;
            }

            return null;
        }

        public CodeTour GetTour(string tourFile)
        {
            return Tours.FirstOrDefault(t => t.File.Equals(tourFile, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<Step> GetStepsContainingFile(string sourceFile)
        {
            var list = new List<Step>();

            foreach (CodeTour tour in Tours)
            {
                list.AddRange(tour.GetStepsFromFile(sourceFile));
            }

            return list;
        }

        private async Task<CodeTour> LoadTourAsync(string fileName)
        {
            var list = new List<Step>();

            try
            {
                using (var reader = new StreamReader(fileName))
                {
                    var json = await reader.ReadToEndAsync();
                    CodeTour tour = JsonConvert.DeserializeObject<CodeTour>(json);
                    tour.File = fileName;

                    foreach (Step step in tour.Steps)
                    {
                        step.Tour = tour;
                        step.Title = step.Title ?? step.File ?? step.Directory;
                        list.Add(step);
                    }

                    return tour;
                }
            }
            catch (Exception ex)
            {
                Trace.Fail(ex.ToString());
                return null;
            }
        }
    }
}
