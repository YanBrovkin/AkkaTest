using System;
using Topshelf;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(windowsService =>
            {
                windowsService.Service<ChatService>(s =>
                {
                    s.ConstructUsing(service => new ChatService());
                    s.WhenStarted(service => service.Start());
                    s.WhenStopped(service => service.Stop());
                });

                windowsService.RunAsLocalSystem();
                windowsService.StartAutomatically();

                windowsService.SetDescription("TopshelfDotNetCoreExample");
                windowsService.SetDisplayName("TopshelfDotNetCoreExample");
                windowsService.SetServiceName("TopshelfDotNetCoreExample");
            });
        }
    }
}
