using Autofac;

namespace Sharp.Redux.Playground.Wpf
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
