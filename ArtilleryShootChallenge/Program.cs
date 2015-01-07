using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ArtilleryShootChallenge
{
    static class Program
    {
        static void Main(string[] args)
        {
            var targets = ImportTargets("../../input.txt");
            PrintTargets(targets);

            double angle = 22.5;
            double time = 0;

            // Order the targets by distance
            targets = targets.OrderBy(s => s.Distance).ToList();
            PrintTargets(targets);

            double avgPoints = targets.Sum(s => s.PointValue) / targets.Count;
            double density = (targets.Max(s => s.Distance) - targets.Min(s => s.Distance)) / targets.Count;

           
            Console.WriteLine("Initial Range: " + Range(angle));
            Console.WriteLine("Average Points per Target: " + avgPoints);
            Console.WriteLine("Average Distance b/w Targets: " + density);
            Console.WriteLine("Appx possible movement in 1s between " + Range(angle - 10) + " and " + Range(angle + 10));


            // The cannon is already loaded to start, and will take 1s to reload. 
            // We only have 10s total, so if we fire immediately we get 11 shots total, otherwise can only fire 10
            // But, movement is not really an issue, within one second we can go from range 700 to range 422 or from 700 to 906
            
            // Call importtargets each time to get a fresh target list, as the attack methods modify the list
            BasicAttack(ImportTargets("../../input.txt"));
            Console.WriteLine("Press any key to continue to cluster attack...");
            Console.ReadKey();
            ClusterAttack(ImportTargets("../../input.txt"));
        }

        static void BasicAttack(List<Target> targets)
        {
            double angle = 22.5;
            double time = 0;

            Console.WriteLine("-------------------------Basic Attack-------------------------------------------");
            // Nieve attempt: fire first, then fire at the highest point value targets we can reach each shot
            int score = 0;
            while (time <= 10)
            {
                // fire!
                Console.WriteLine(time + "s: Firing at " + angle + "degrees! Direct hit at position " + Range(angle) + "m!");
                var toprange = Range(angle) + 5;
                var bottomrange = Range(angle) - 5;
                List<Target> hits = targets.Where(s => (s.Distance <= toprange) && (s.Distance >= bottomrange)).ToList();
                // add the points to our score, and remove the targets from the field
                foreach (var hit in hits)
                {
                    Console.WriteLine("Hit target at " + hit.Distance + "m! +" + hit.PointValue + " points!");
                    score += hit.PointValue;
                    targets.Remove(hit);
                }

                // Time to pick a new target!
                var maxrange = Math.Max(Range(Math.Max(angle - 10, 10)), Range(Math.Min(angle + 10, 80)));
                var minrange = Math.Min(Range(Math.Max(angle - 10, 10)), Range(Math.Min(angle + 10, 80)));
                // pick the highest point value target in range
                var newTarget = targets.Where(s => (s.Distance <= maxrange) && (s.Distance >= minrange)).OrderByDescending(s => s.PointValue).First();
                // Now adjust the angle, using this equation: angle (radians) = (1/2)*arcsin(10*range/100^2)
                angle = 0.5 * Math.Asin((10 * (double)newTarget.Distance) / (100 * 100));
                // angle (degrees) = angle (radians) * (180.0 / Math.PI)
                angle = angle * (180.0 / Math.PI);

                // We made sure that the angle adjustment was less than 10 degrees --> less than one second
                // Therefore no need to calculate, just add 1 second for combined adjustment and reloading
                time = time + 1.00;

                // ^--- Fire again!
            }
            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine("Final score: " + score);
        }

        static List<Target> ImportTargets(string filePath)
        {
            var file = File.OpenText(filePath);
            var targets = new List<Target>();
            while (!file.EndOfStream)
            {
                string[] line = file.ReadLine().Split(',');
                var target = new Target { 
                    TargetNumber = int.Parse(line[0]), Distance = int.Parse(line[1]), PointValue = int.Parse(line[2]) 
                };
                targets.Add(target);
            }
            return targets;
        }

        static void PrintTargets(List<Target> targets)
        {
            foreach (var target in targets)
            {
                Console.WriteLine(target.ToString());
            }
        }

        static double toRadians(this double degrees)
        {
            return degrees * (Math.PI / 180);
        }

        /// <summary>
        /// Calculates the range of the projectile at the current point
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        static double Range(double angle)
        {
            // The vertical velocity vv = 100 * sin(alpha) and the horizontal velocity vh is 100 * cos(alpha). 
            // The range is 2 * vh * vv / 10. (10 being the acceleration due to gravity = 10 m/s^2).
            // range = 2000 * sin(x) * cos(x)
            return 2000 * Math.Sin(angle.toRadians()) * Math.Cos(angle.toRadians());
        }

        /// <summary>
        /// Calculates the derivative of the range equation at the current angle
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        static double dRange(double angle)
        {
            // range = 2000 * sin(x) * cos(x)
            // d(range)/dx = 2000 * cos(2x)
            return 2000 * Math.Cos(2 * angle.toRadians());
        }

        static void ClusterAttack(List<Target> targets)
        {
            double time = 0;
            double angle = 22.5;
            int score = 0;
            Console.WriteLine("-----------------------------Cluster Attack-------------------------------------");

            List<TargetCluster> clusters = new List<TargetCluster>();
            // Low resolution, will force choice b/w [.1...9..9.][.9..9...1.] even though [9..9..9..9] would be the ideal cluster
            for (var i = targets.Min(s => s.Distance) + 5; i < targets.Max(s => s.Distance); i = i + 10)
            {
                var targetsInCluster = targets.Where(s => (s.Distance < i + 5) && (s.Distance >= i - 5)).ToList();
                clusters.Add(new TargetCluster { DistanceAtCenter = i, TotalValue = targetsInCluster.Sum(s => s.PointValue) });
            }

            while (time <= 10)
            {
                // fire!
                Console.WriteLine(time + "s: Firing at " + angle + "degrees! Direct hit at position " + Range(angle) + "m!");
                var toprange = Range(angle) + 5;
                var bottomrange = Range(angle) - 5;
                List<Target> hits = targets.Where(s => (s.Distance <= toprange) && (s.Distance >= bottomrange)).ToList();
                // add the points to our score, and remove the targets from the field
                foreach (var hit in hits)
                {
                    Console.WriteLine("Hit target at " + hit.Distance + "m! +" + hit.PointValue + " points!");
                    score += hit.PointValue;
                    targets.Remove(hit);
                }

                // Time to pick a new target!
                var maxrange = Math.Max(Range(Math.Max(angle - 10, 10)), Range(Math.Min(angle + 10, 80)));
                var minrange = Math.Min(Range(Math.Max(angle - 10, 10)), Range(Math.Min(angle + 10, 80)));
                // pick the highest point value target in range
                var newTarget = clusters.Where(s => (s.DistanceAtCenter <= maxrange) && (s.DistanceAtCenter >= minrange)).OrderByDescending(s => s.TotalValue).First();
                // Now adjust the angle, using this equation: angle (radians) = (1/2)*arcsin(10*range/100^2)
                angle = 0.5 * Math.Asin((10 * (double)newTarget.DistanceAtCenter) / (100 * 100));
                // angle (degrees) = angle (radians) * (180.0 / Math.PI)
                angle = angle * (180.0 / Math.PI);

                // Remove the cluster so it isnt re-used
                clusters.Remove(newTarget);

                // We made sure that the angle adjustment was less than 10 degrees --> less than one second
                // Therefore no need to calculate, just add 1 second for combined adjustment and reloading
                time = time + 1.00;

                // ^--- Fire again!
            }
            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine("Final score: " + score);
            Console.ReadKey();
        }
    }
}
