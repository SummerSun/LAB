namespace Recognition
{
    using Microsoft.Azure.WebJobs;
    using Microsoft.ServiceBus.Messaging;

    public class Function
    {
        // service bus queue to queue
        public static void ServiceBusMessageToQueue([ServiceBusTrigger("bus")] string start , [Queue("feedpub")] out string feedpubMessage, [Queue("")] out string pointsMessage)
        {
            // check message type of ActivityMessage to enqueue to "feedpub" and "points"
            // check message type of AchievementRecalculateMessage to enqueue "points"
            // check message should go directly into database, write it to database

            feedpubMessage = start;
            pointsMessage = start;
        }

        public static void FeedPubQueueToDatabase([QueueTrigger("feedpub")] string message)
        {
            // get activity id, create an activity
            // call activity service to undo or to write an activity
        }

        public static void PointsQueueToDatabase([QueueTrigger("points")] string message)
        {
            // call undoPointLedgerProcessor or pointLedgeProcess to process message
        }
    }
}
