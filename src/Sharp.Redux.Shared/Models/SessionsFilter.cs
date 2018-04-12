namespace Sharp.Redux.Shared.Models
{
    public readonly struct SessionsFilter
    {
        public int? MaxCount { get; }
        public SessionsFilter(int? maxCount)
        {
            MaxCount = maxCount;
        }
    }
}
