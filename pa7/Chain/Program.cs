namespace pa7;

using System.Net;
using System.Net.Sockets;


internal class Program
{
    private static Socket _receiver;
    private static Socket _sender;

    private static void Start(int listeningPort, string nextHost, int nextPort, bool isInit)
    {
        try
        {
            var prevIpAddress = IPAddress.Any;
            var nextIpAddress = (nextHost == "localhost") ? IPAddress.Loopback : IPAddress.Parse(nextHost);

            var prevEp = new IPEndPoint(prevIpAddress, listeningPort);
            var nextEp = new IPEndPoint(nextIpAddress, nextPort);

            _sender = new Socket(nextIpAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _receiver = new Socket(prevIpAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                _receiver.Bind(prevEp);
                _receiver.Listen(6);

                Connect(nextEp);

                var number = Console.ReadLine();
                var x = Convert.ToInt32(number);
                
                var listenerHandler = _receiver.Accept();

                if (isInit)
                    WorkAsInitiator(listenerHandler, x);
                else
                    WorkAsProcess(listenerHandler, x);

                listenerHandler.Shutdown(SocketShutdown.Both);
                listenerHandler.Close();

                _sender.Shutdown(SocketShutdown.Both);
                _sender.Close();
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane);
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private static void WorkAsInitiator(Socket listenerHandler, int x)
    {
        _sender.Send(BitConverter.GetBytes(x));

        var buf = new byte[1024];
        listenerHandler.Receive(buf);
        var y = BitConverter.ToInt32(buf);

        x = y;

        _sender.Send(BitConverter.GetBytes(x));

        buf = new byte[1024];
        listenerHandler.Receive(buf);
        x = BitConverter.ToInt32(buf);

        Console.Write(x);
    }

    private static void WorkAsProcess(Socket listenerHandler, int x)
    {
        var buf = new byte[1024];
        listenerHandler.Receive(buf);
        var y = BitConverter.ToInt32(buf);

        _sender.Send(BitConverter.GetBytes(Math.Max(x, y)));

        buf = new byte[1024];
        listenerHandler.Receive(buf);
        x = BitConverter.ToInt32(buf);

        _sender.Send(BitConverter.GetBytes(x));

        Console.Write($"The biggest number is {x}");
    }

    private static void Connect(IPEndPoint remoteEp)	
    {
        while (true)
            try
            {
                _sender.Connect(remoteEp);
                break;
            }
            catch
            {
                Thread.Sleep(1000);
            }
    }
    
    static void Main(string[] args)
    {
        var listeningPort = Int32.Parse(args[0]);
        var nextHost = args[1];
        var nextPort = Int32.Parse(args[2]);
        var isInit = false;
        
        if (args.Length >= 4)
        {
            isInit = bool.Parse(args[3]);
        }

        Console.WriteLine($"The process on {listeningPort} is awaiting...");
        Start(listeningPort, nextHost, nextPort, isInit);
        Console.ReadKey();
    }
}