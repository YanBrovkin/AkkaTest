using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Akka.Actor;
using Akka.Persistence;
using ConsoleApp.Commands;

namespace ConsoleApp.Actors
{
    public class UserActor : ReceivePersistentActor
    {
        private int msgsSinceLastSnapshot = 0;
        private List<string> messages = new List<string>();
        public override string PersistenceId { get; }

        public UserActor()
        {
            this.PersistenceId = Context.Parent.Path.Name + "-" + Self.Path.Name;

            // recover
            Recover<string>(str => messages.Add(str)); // from the journal
            Recover<SnapshotOffer>(offer => {
                var messagesSnapshot = offer.Snapshot as List<string>;
                if (messagesSnapshot != null) 
                    messages = messages.Concat(messagesSnapshot).ToList();
            });
            // commands
            Command<string>(str => Persist(str, s => 
            {
                Console.WriteLine($"Incoming {Self.Path.Name}: {str}");
                messages.Add(str); //add msg to in-memory event store after persisting
                if (++msgsSinceLastSnapshot % 100 == 0)
                {
                    //time to save a snapshot
                    SaveSnapshot(messages);
                }
            }));
            Command<SaveSnapshotSuccess>(success => {
                // soft-delete the journal up until the sequence # at
                // which the snapshot was taken
                DeleteMessages(success.Metadata.SequenceNr);
            });
            Command<SaveSnapshotFailure>(failure => {
                // handle snapshot save failure...
                Console.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} [Error][{Self.Path.Name}]: error saving snapshot - {failure.Cause.Message}");
            });
            Command<GetMessages>(get => Sender.Tell(messages.AsEnumerable()));
        }

        public static Props PropsFor(string tickerSymbol)
        {
            return Props.Create(() => new UserActor());
        }

    }
}
