using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Akka.Configuration;
using ConsoleApp.Actors;
using ConsoleApp.Domain;

namespace ConsoleApp
{
    public class ChatService
    {
        public void Start()
        {

            var config = ConfigurationFactory.ParseString(@"
akka.persistence{
          journal {
            plugin = ""akka.persistence.journal.sql-server""
            sql-server {
                class = ""Akka.Persistence.SqlServer.Journal.SqlServerJournal, Akka.Persistence.SqlServer""
                plugin-dispatcher = ""akka.actor.default-dispatcher""
                table-name = EventJournal
                schema-name = dbo
                auto-initialize = on
                connection-string = ""Data Source=Y-BROVKIN-M\\SQL2016;Initial Catalog=akkatest;Integrated Security=True""
            }
          }
          publish-plugin-commands = on
          snapshot-store {
            plugin = ""akka.persistence.snapshot-store.sql-server""
            sql-server {
                class = ""Akka.Persistence.SqlServer.Snapshot.SqlServerSnapshotStore, Akka.Persistence.SqlServer""
                plugin-dispatcher = ""akka.actor.default-dispatcher""
                table-name = SnapshotStore
                schema-name = dbo
                auto-initialize = on
                connection-string = ""Data Source=Y-BROVKIN-M\\SQL2016;Initial Catalog=akkatest;Integrated Security=True""
            }
          }
        }");

            using (var system = ActorSystem.Create("MyServer", config))
            {
                var userActor = system.ActorOf(Props.Create(() => new UserActor()), "UserActor1");
                system.EventStream.Subscribe(userActor, typeof(string));

                for (var i = 0; i <= 200; i++)
                {
                    var msg = $"Message{i}";
                    system.EventStream.Publish(msg);
                }
                Console.ReadLine();
            }
        }

        public void Stop()
        {

        }
    }
}
