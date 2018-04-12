namespace Sharp.Redux.Shared.Models
{
    public readonly struct StepsFilter
    {
        public int? MaxCount { get; }
        public StepsFilter(int? maxCount)
        {
            MaxCount = maxCount;
        }
    }
}
