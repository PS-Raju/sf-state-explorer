namespace TestStatefulService.Service
{
    public interface IReliableQueueService
    {
        void PopulateReliableQueue(object cancellationToken);
    }
}