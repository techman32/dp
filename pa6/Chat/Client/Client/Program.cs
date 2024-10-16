﻿namespace Client;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

internal class Program
{
    public static void StartClient(string address, int port, string message)
        {
            try
            {
                // Разрешение сетевых имён
                IPAddress ipAddress = address == "localhost" ? IPAddress.Loopback : IPAddress.Parse(address);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // CREATE
                Socket sender = new Socket(
                    ipAddress.AddressFamily,
                    SocketType.Stream,
                    ProtocolType.Tcp);

                try
                {
                    // CONNECT
                    sender.Connect(remoteEP);
                    Console.WriteLine("Удалённый адрес подключения сокета: {0}",
                        sender.RemoteEndPoint.ToString());
                    // SEND

                    int bytesSent = sender.Send(Encoding.UTF8.GetBytes(message));

                    // RECEIVE
                    byte[] buf = new byte[1024];
                    int bytesRec = sender.Receive(buf);
                    var history = JsonSerializer.Deserialize<List<string>>(Encoding.UTF8.GetString(buf, 0, bytesRec));

                    Console.WriteLine("Ответ: {0}\n", sender.RemoteEndPoint.ToString());
                    
                    foreach (var msg in history)
                    {
                        Console.WriteLine(msg);
                    }

                    // RELEASE
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }

            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
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
            if (args.Length != 3)
            {
                Console.WriteLine("Usage: dotnet run <URL> <PORT> <Message>");
            }
            else if (args[2].Length == 0)
            {
                Console.WriteLine("Message can't be empty");
            }
            else
            {
                StartClient(args[0], Int32.Parse(args[1]), args[2]);
            }
        }
}
