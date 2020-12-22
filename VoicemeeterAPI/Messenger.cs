using System;
using System.Collections.Generic;
using System.Text;

namespace VoicemeeterAPI
{
    class Messenger
    {
        SerialHandler serialHandler = new SerialHandler();
        VoiceMeeterHandler voiceMeeterHandler = new VoiceMeeterHandler();

        public Messenger()
        {
            serialHandler.onMessage += onMessageReceived;
            voiceMeeterHandler.onSetFader += movePhysicalFader;
            

            voiceMeeterHandler.connect();
            serialHandler.connect();

            char key;
            Random random = new Random();
            Console.WriteLine("Press the q key to quit...");

            while (true)
            {
                key = Console.ReadKey().KeyChar;
                if(key == 'q')
                {
                    break;
                }
                if (key == 's')
                {
                    var rand = random.Next(0, 1024);

                    voiceMeeterHandler.setFader(0, rand);

                }

            }

            // console.WriteLine();
            // Console.ReadKey();

        }


        public void movePhysicalFader(object sender, int value)
        {
            serialHandler.setFader(0, value);
        }


        public void onMessageReceived(object sender, Message message)
        {
            switch (message.topic)
            {
                case Topic.MOVE:
                    if(message.part == Part.FADER)
                    {
                        voiceMeeterHandler.setFader(message.channel, message.value);
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }

        }
    }
}
