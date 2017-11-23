namespace Sharp.Redux.Playground.Engine.Models
{
    public abstract class Entity: IKeyedItem<int>
    {
        public int Id { get; }
        int IKeyedItem<int>.Key => Id;

        public Entity(int id)
        {
            Id = id;
        }
    }
}
