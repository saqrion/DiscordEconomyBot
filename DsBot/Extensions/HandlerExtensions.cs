using Discord.Interactions;
using DsBot.Commands;
using DsBot.Handlers;
using System.Reflection;

namespace DsBot.Services
{
    public static class HandlerExtensions
    {
        public static IServiceCollection AddHandlers (this IServiceCollection services, Assembly? assembly = null)
        {
            assembly ??= Assembly.GetExecutingAssembly ();
            var handlerType = typeof(IHandler);

            var handlers = assembly.GetTypes()
                .Where(t => handlerType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);

            foreach (var handler in handlers)
            {
                services.AddScoped(handlerType, handler);
            }
            
            return services;
        }

        public static IServiceCollection AddBotCommands (this IServiceCollection services, Assembly? assembly = null)
        {
            assembly ??= Assembly.GetExecutingAssembly ();
            var commandType = typeof(InteractionModuleBase);

            var commands = assembly.GetTypes()
                .Where(c => commandType.IsAssignableFrom(c) && c.IsClass && !c.IsAbstract);

            foreach (var command in commands)
            {
                services.AddScoped(commandType, command);
            }

            return services;
        }

        public static async Task InitializeHandlersAsync(this IServiceProvider provider)
        {
            var handlers = provider.GetServices<IHandler>();
            foreach (var handler in handlers)
            {
                await handler.InitializeAsync();
            }
        }
    }
}
