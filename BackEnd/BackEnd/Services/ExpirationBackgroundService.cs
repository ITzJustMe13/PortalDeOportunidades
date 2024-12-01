using BackEnd.Controllers.Data;
using BackEnd.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Services
{
    public class ExpirationBackgroundService: BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ExpirationBackgroundService> _logger;

        public ExpirationBackgroundService(IServiceProvider serviceProvider, ILogger<ExpirationBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken); // Intervalo de 1 hora

                // Aqui criamos um escopo para acessar os serviços do DI
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var dateValidationService = scope.ServiceProvider.GetRequiredService<DateValidationService>();

                    // Obtemos todas as entidades do tipo IExpirable
                    var expirableEntities = await context.Set<IExpirable>().ToListAsync();

                    // Iteramos e validamos cada entidade
                    foreach (var entity in expirableEntities)
                    {
                        dateValidationService.ValidateAndUpdateDate(entity);
                    }

                    await context.SaveChangesAsync(stoppingToken); // Salva as alterações no banco de dados
                    _logger.LogInformation("Entidades expiradas verificadas e atualizadas.");
                }
            }
        }
    }
}
