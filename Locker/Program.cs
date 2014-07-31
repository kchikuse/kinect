using System;
using System.Timers;
using Microsoft.Kinect;

namespace Locker
{
    class Program
    {
        static Timer timer;
        static Logic logic;
        static KinectSensor sensor;

        static void Main()
        {
            Init();

            try
            {
                logic = new Logic();
                sensor = KinectSensor.KinectSensors[0];

                sensor.Start();
                sensor.SkeletonStream.Enable();
                sensor.SkeletonStream.EnableTrackingInNearRange = true;

                timer = new Timer();
                timer.Interval = 500;
                timer.Elapsed += Timer_Elapsed;
                timer.Enabled = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.ReadLine();
        }

        static void Init()
        {
            Console.Title = "BOB v3";
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Clear();
        }
        
        static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var passive = 0;
            var active = 0;

            foreach(var k in KinectSensor.KinectSensors)
            {
                using(var frame = k.SkeletonStream.OpenNextFrame(50))
                {
                    if(frame != null)
                    {
                        var s = new Skeleton[frame.SkeletonArrayLength];
                        frame.CopySkeletonDataTo(s);
                        foreach (var skeleton in s)
                        {
                            if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                                active++;

                            if (skeleton.TrackingState == SkeletonTrackingState.PositionOnly)
                                passive++;
                        }
                    }
                }

                Console.WriteLine(logic.Process(active, passive));
            }
        }
    }
}