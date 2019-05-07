using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Evento.Infrastructure.Services
{
    public class DataInitializer : IDataInitializer
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly IUserService _userService;
        private readonly IEventService _eventService;
        public DataInitializer(IUserService userService, IEventService eventService)
        {
            _eventService = eventService;
            _userService = userService;
        }
        public async Task SeedAsync()
        {
            logger.Info("Initializing data...");
            var tasks = new List<Task>();
            tasks.Add(_userService.RegisterAsync(Guid.NewGuid(), "user@email.com", "default_user", "secret"));
            tasks.Add(_userService.RegisterAsync(Guid.NewGuid(), "admin@email.com", "admin", "secret", "admin"));
            logger.Info("Created users: user, admin");
            for(var i = 1; i<10; i++)
            {
                var evetnId = Guid.NewGuid();
                var name = $"Event {i}";
                var description = $"{name} description.";
                var startDate = DateTime.UtcNow.AddHours(3);
                var endDate = startDate.AddHours(2);
                tasks.Add(_eventService.CreateAsync(evetnId, name, description, startDate, endDate));
                tasks.Add(_eventService.AddTicketsAsync(evetnId, 1000, 100));
                logger.Info($"Created event: {name}.");
            }
            await Task.WhenAll(tasks);
            logger.Info("Data was initialized.");
        }
    }
}
