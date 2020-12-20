using System;

namespace VoicemeeterAPI
{

    class Program
    {

        static void Main(string[] args)
        {
            new Messenger();
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
        }

        static void OnProcessExit(object sender, EventArgs e)
        {
            Voicemeeter.RemoteWrapper.Logout();
        }

    }
}


