using Autofac;

namespace Todo.Wpf
{
    public static class IoC
    {
        public static void Init()
        {
            ContainerBuilder builder = new ContainerBuilder();
            Engine.IoCRegistrar.Build(builder);
        }
    }
}
