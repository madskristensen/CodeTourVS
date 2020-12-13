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
    public class TourManager
    {
        public List<Tour> Tours { get; private set; } = new List<Tour>();

        public static async Task<TourManager> FromFolderAsync(string toursFolder, CancellationToken cancellationToken)
        {
            var manager = new TourManager();

            if (Directory.Exists(toursFolder))
            {
                foreach (var file in Directory.EnumerateFiles(toursFolder, $"*{Constants.TourFileExtension}"))
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    Tour tour = await manager.LoadTourAsync(file);
                    manager.Tours.Add(tour);
                }
            }

            return manager;
        }

        public Tour GetTour(string fileName)
        {
            return Tours.FirstOrDefault(t => t.File.Equals(fileName, StringComparison.OrdinalIgnoreCase));
        }

        private async Task<Tour> LoadTourAsync(string fileName)
        {
            var list = new List<Step>();

            try
            {
                using (var reader = new StreamReader(fileName))
                {
                    var json = await reader.ReadToEndAsync();
                    Tour tour = JsonConvert.DeserializeObject<Tour>(json);
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
