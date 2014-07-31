using System;
using System.Linq;
using System.Speech.Synthesis;
using System.Threading;
using iTunesLib;
using Timer = System.Timers.Timer;

namespace Locker
{
    public class Logic
    {
        static bool armed;
        static Timer resetTimer;
        static iTunesApp itunes;
        static SpeechSynthesizer speaker;

        public Logic()
        {
            armed = true;

            resetTimer = new Timer();
            itunes = new iTunesApp();
            speaker = new SpeechSynthesizer();

            resetTimer.Interval = TimeSpan.FromMinutes(30).TotalMilliseconds;
            resetTimer.Elapsed += (o, e) =>
                                      {
                                          armed = true;
                                          resetTimer.Stop();
                                      };
        }

        public string Process(int active, int passive)
        {
            if (armed)
            {
                if(active > 0)
                {
                    armed = false;
                    resetTimer.Start();
                    Speak("Welcome back Knowledge.");

                    if(active == 2)
                    {
                        Stop(4);
                        Speak("{0} visitor. My name is Bob. Welcome to our house", Greet());
                    }

                    Play();
                }
            }

            Console.ForegroundColor = Color(active);

            return string.Format("{0:h:mm:ss} (ACTIVE) = {1}, (PASSIVE) = {2}", DateTime.Now, active, passive);
        }
        
        public ConsoleColor Color(int active)
        {
            return active > 0 ? ConsoleColor.Red : ConsoleColor.White;
        }

        static string Greet()
        {
            var greeting = "";
            var hour = DateTime.Now.Hour;

            if (hour >= 5 && hour < 12)
            {
                greeting = "Good morning";
            }

            if (hour >= 12 && hour < 18)
            {
                greeting = "Good afternoon";
            }

            if (hour >= 18 && hour < 24)
            {
                greeting = "Good evening";
            }

            return greeting;
        }

        static void Play()
        {
            var relax = FindPlayList("Surf Roots");
            if (relax != null)
            {
                Stop(2);
                relax.PlayFirstTrack();
            }
        }
        
        static void Stop(int seconds)
        {
            Thread.Sleep(seconds * 1000);
        }

        static IITPlaylist FindPlayList(string name)
        {
            return itunes.LibrarySource.Playlists.Cast<IITPlaylist>().FirstOrDefault(i => i.Name == name);
        }

        static void Speak(string text, params object[] args)
        {
            speaker.Speak(string.Format(text, args));
        }
    }
}