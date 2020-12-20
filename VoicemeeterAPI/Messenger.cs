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

            Console.WriteLine("Press any key to continue...");
            Console.WriteLine();
            Console.ReadKey();
            serialHandler.disconect();

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
