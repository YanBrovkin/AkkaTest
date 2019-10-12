using System;
using Akka.Actor;
using Akka.Cluster;
using Akka.Cluster.Sharding;
using Akka.Configuration;
using Akka.Persistence;
using ConsoleApp.Actors;
using ConsoleApp.Config;
using ConsoleApp.Domain;

namespace ConsoleApp
{
    public class ChatService
    {
        public void Start()
        {

            //var config = HoconLoader.ParseConfig("akka.hocon");
            var config = HoconLoader.ParseConfig("persistence.hocon")
                .WithFallback(ClusterSharding.DefaultConfig())
                .WithFallback(Persistence.DefaultConfig());
            var actorSystem = ActorSystem.Create("testSystem", config);
            var sharding = ClusterSharding.Get(actorSystem);


            var shardRegion = sharding.Start("priceAggregator",
                s => Props.Create(() => new UserActor()),
                ClusterShardingSettings.Create(actorSystem),
                new MessageExtractor());
            //Cluster.Get(system).RegisterOnMemberUp(() =>
            //{
            //    var sharding = ClusterSharding.Get(system);

            //    var shardRegion = sharding.Start("userActor", s => UserActor.PropsFor(s), ClusterShardingSettings.Create(system),
            //        new MessageExtractor());
            //});


            //var userActor = system.ActorOf(Props.Create(() => new UserActor()), "UserActor1");
            //system.EventStream.Subscribe(userActor, typeof(string));

            for (var i = 0; i <= 200; i++)
            {
                var msg = $"Message{i}";
                shardRegion.Tell(msg);
                //system.EventStream.Publish(msg);
            }
            Console.ReadLine();
        }

        public void Stop()
        {

        }
    }
}
