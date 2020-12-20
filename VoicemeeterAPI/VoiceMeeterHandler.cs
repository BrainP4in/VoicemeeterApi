using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace VoicemeeterAPI
{
    class VoiceMeeterHandler
    {
        float[] lastValue = { -1, -1, -1, -1};
        Timer timer;
        Timer cooldown = new Timer(1000);
        public EventHandler<int> onSetFader;


        internal void connect()
        {
            VoiceMeeter.Remote.Initialize(Voicemeeter.RunVoicemeeterParam.VoicemeeterPotato);
            timer = new Timer(1000);
            timer.Elapsed += new ElapsedEventHandler(getFader);
            cooldown.AutoReset = false;
            cooldown.Elapsed += new ElapsedEventHandler(getFaderCooldown);
            timer.Enabled = true;
        }

        internal void disconect()
        {
            Voicemeeter.RemoteWrapper.Logout();
        }

        // Move Voicemeeter fader to value
        public void setFader(int channel, float value)
        {
            //timer.Enabled = false;

            Console.WriteLine("setFader voicemeeter ch: " + channel + " val: " + value);

            value = (float) decimal.Round( (decimal) Utils.Remap(value, 3f, 1021f, -60f, 12f),1);

            VoiceMeeter.Remote.SetParameter($"Strip[{channel}].Gain", value);
            lastValue[channel] = value;

            //cooldown.Stop();
            //cooldown.Start();
        }

        public void getFader(object source, ElapsedEventArgs e)
        {


            int channel = 0;

            if (Voicemeeter.RemoteWrapper.IsParametersDirty() != 1) return; // lastValue[channel] == -1 || 

            float currentVal = VoiceMeeter.Remote.GetParameter($"Strip[{channel}].Gain");

            // Voicemeeter fader was moved
            if (Math.Abs(currentVal - lastValue[channel]) > 1)
            {
                Console.WriteLine("getFader DIFFERENT");
                lastValue[channel] = currentVal;

                int value = Convert.ToInt32(Utils.Remap(currentVal, -60f, 12f, 3f, 1021f));

                onSetFader.Invoke(null, value);
            }
        }

        public void getFaderCooldown(object source, ElapsedEventArgs e)
        {
            Console.WriteLine("COOLDOWN END");

            timer.Enabled = true;
        }



    }
}
