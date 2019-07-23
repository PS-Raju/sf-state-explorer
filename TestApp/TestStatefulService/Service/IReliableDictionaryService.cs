namespace TestStatefulService.Service
{
    public interface IReliableDictionaryService
    {
        void PopulateReliableDictionary(object cancellationToken);
    }
}