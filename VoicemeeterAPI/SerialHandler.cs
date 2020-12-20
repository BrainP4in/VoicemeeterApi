using System;
using System.IO.Ports;
using System.Linq;
using System.Management;

namespace VoicemeeterAPI
{
    enum Topic
    {
        MOVE,
        SET
    }

    enum Part
    {
        FADER
    }

    class Message
    {
        public Topic topic;
        public Part part;
        public int channel;
        public float value;
        public bool active;
    }

    class SerialHandler
    {
        private SerialPort serial;
        public EventHandler<Message> onMessage;

        public void disconect()
        {
            serial.Close();
        }

        public void connect()
        {
            string port = selectCom();

            serial = new SerialPort(port);
            serial.BaudRate = 9600;
            serial.Parity = Parity.None;
            serial.StopBits = StopBits.One;
            serial.DataBits = 8;
            serial.Handshake = Handshake.None;
            serial.RtsEnable = true;

            serial.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            serial.Open();
        }

        // Move physical fader to val (0-1024)
        public void setFader(int channel, int value)
        {
            Console.WriteLine("setFader physical ch: " + channel + " val: " + value);

            if (serial == null) return;
            byte[] command = { (byte)Topic.SET, (byte)Part.FADER, 0, BitConverter.GetBytes(value)[0], BitConverter.GetBytes(value)[1]};
            serial.Write(command, 0, command.Length);
        }

        private static string selectCom()
        {
            string port = "COM1";
            Console.WriteLine("The following serial ports were found:");
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Caption like '%(COM%'"))
            {
                var portnames = SerialPort.GetPortNames();
                var ports = searcher.Get().Cast<ManagementBaseObject>().ToList().Select(p => p["Caption"].ToString());

                var portList = portnames.Select(n => n + " - " + ports.FirstOrDefault(s => s.Contains(n))).ToList();
                foreach (var i in portList)
                {
                    Console.WriteLine(i);
                }
                Console.Write("I want to connect to: ");
                Console.WriteLine("COM");
                string input = Console.ReadLine();
                try
                {
                    int result = Int32.Parse(input);
                    port = $"COM{result}";
                    Console.WriteLine(result);
                }
                catch (FormatException)
                {
                    throw (new Exception($"Unable to parse '{input}'"));
                }
            }
            return port;
        }

        // parses incomming serial messages and invokes onMessage
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            string[] content;
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();
            Console.WriteLine("MSG: " + indata);

            string[] messagePart = indata.Split("\r\n");

            foreach (var part in messagePart)
            {
                if (part == "") return;

                try
                {
                    content = part.Split("    ");

                    Message message = new Message();
                    message.topic = (Topic)Enum.Parse(typeof(Topic), content[0], true);
                    message.part = (Part)Enum.Parse(typeof(Part), content[1], true);
                    message.channel = Int32.Parse(content[2]);
                    message.value = Int32.Parse(content[3]);

                    //onMessage.Invoke(null, message);

                }
                catch (Exception)
                {}

            }
        }
    }
}
