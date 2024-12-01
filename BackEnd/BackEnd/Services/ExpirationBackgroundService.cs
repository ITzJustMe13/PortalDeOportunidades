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
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken); // Intervalo de 1 dia

                // Aqui criamos um escopo para acessar os serviços do DI
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var dateValidationService = scope.ServiceProvider.GetRequiredService<DateValidationService>();

                    // Busca os Impulses expirados
                    var expiredImpulses = await context.Impulses
                        .Where(i => i.ExpireDate < DateTime.Now)
                        .ToListAsync(stoppingToken);

                    foreach (var impulse in expiredImpulses)
                    {
                        context.Impulses.Remove(impulse);

                        // Encontra a Opportunity associada ao Impulse
                        var opportunity = await context.Opportunities
                            .FirstOrDefaultAsync(o => o.OpportunityId == impulse.OpportunityId, stoppingToken);

                        if (opportunity != null)
                        {
                            var hasActiveImpulses = await context.Impulses
                                .AnyAsync(i => i.OpportunityId == opportunity.OpportunityId && i.ExpireDate > DateTime.Now, stoppingToken);

                            if (!hasActiveImpulses)
                            {
                                opportunity.IsImpulsed = false;
                            }
                        }
                    }

                    await context.SaveChangesAsync(stoppingToken);
                    _logger.LogInformation("Impulses expirados verificados e atualizados.");
                }
            }
        }
    }
}
