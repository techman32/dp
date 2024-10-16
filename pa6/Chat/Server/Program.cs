﻿namespace Server;

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

internal class Program
{ 
    private static List<string> _history = new List<string>();
    public static void StartListening(int port)
    {
        // Разрешение сетевых имён

        // Привязываем сокет ко всем интерфейсам на текущей машинe
        IPAddress ipAddress = IPAddress.Any;

        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

        // CREATE
        Socket listener = new Socket(
            ipAddress.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp);

        try
        {
            // BIND
            listener.Bind(localEndPoint);

            // LISTEN
            listener.Listen(10);

            while (true)
            {
                Console.WriteLine("Ожидание соединения клиента...");
                // ACCEPT
                Socket handler = listener.Accept();
                
                Console.WriteLine("Получение данных...");
                byte[] buf = new byte[1024];

                // RECEIVE
                int bytesRec = handler.Receive(buf);
                string data = Encoding.UTF8.GetString(buf, 0, bytesRec);

                _history.Add(data);
                Console.WriteLine("Полученный текст: {0}", data);

                // Отправляем текст обратно клиенту
                var jsonString = JsonSerializer.Serialize(_history);
                byte[] msg = Encoding.UTF8.GetBytes(jsonString);

                // SEND
                handler.Send(msg);

                // RELEASE
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }

        }            
        catch (ArgumentOutOfRangeException e)
        {
            Console.WriteLine(e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
            
    }
    static void Main(string[] args)
    {
        Console.WriteLine("Запуск сервера...");
        StartListening(Int32.Parse(args[0]));
        
        Console.WriteLine("\nНажмите ENTER чтобы выйти...");
        Console.Read();
    }
}