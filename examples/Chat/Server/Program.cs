﻿using System;
using System.Collections.Generic;
using chat.messages;
using Proto;
using Proto.Remote;

class Program
{
    static void Main(string[] args)
    {
        var context = new RootContext();
        Serialization.RegisterFileDescriptor(ChatReflection.Descriptor);
        Remote.Start("127.0.0.1", 8000);
        
        var clients = new HashSet<PID>();
        var props = Actor.FromFunc(ctx =>
        {
            switch (ctx.Message)
            {
                case Connect connect:
                    Console.WriteLine($"Client {connect.Sender} connected");
                    clients.Add(connect.Sender);
                    ctx.Send(connect.Sender, new Connected { Message = "Welcome!"});
                    break;
                case SayRequest sayRequest:
                    foreach (var client in clients)
                    {
                        ctx.Send(client, new SayResponse
                        {
                            UserName = sayRequest.UserName,
                            Message = sayRequest.Message
                        });     
                    }
                    break;
                case NickRequest nickRequest:
                    foreach (var client in clients)
                    {
                        ctx.Send(client, new NickResponse
                        {
                            OldUserName = nickRequest.OldUserName,
                            NewUserName = nickRequest.NewUserName
                        });
                    }
                    break;
            }
            return Actor.Done;
        });
        context.SpawnNamed(props, "chatserver");
        Console.ReadLine();
    }
}