using BackEnd.Controllers.Data;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Services
{
    public class ImpulseExpirationBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ImpulseExpirationBackgroundService> _logger;

        public ImpulseExpirationBackgroundService(IServiceProvider serviceProvider, ILogger<ImpulseExpirationBackgroundService> logger)
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

                    // Obtém todos os Impulses que já expiraram
                    var expiredImpulses = await context.Impulses
                        .Where(i => i.ExpireDate < DateTime.Now)
                        .ToListAsync(stoppingToken);

                    foreach (var impulse in expiredImpulses)
                    {
                        // Exclui o Impulse
                        context.Impulses.Remove(impulse);

                        // Encontra a Opportunity associada ao Impulse
                        var opportunity = await context.Opportunities
                            .FirstOrDefaultAsync(o => o.OpportunityId == impulse.OpportunityId, stoppingToken);

                        if (opportunity != null)
                        {
                            // Verifica se existem outros impulsos para esta Opportunity
                            var hasActiveImpulses = await context.Impulses
                                .AnyAsync(i => i.OpportunityId == opportunity.OpportunityId && i.ExpireDate > DateTime.Now, stoppingToken);

                            // Se não houver mais impulsos ativos, desativa a Opportunity
                            if (!hasActiveImpulses)
                            {
                                opportunity.IsImpulsed = false;
                            }
                        }
                    }

                    await context.SaveChangesAsync(stoppingToken); // Salva as alterações no banco de dados
                    _logger.LogInformation("Impulses expirados verificados e atualizados.");
                }
            }
        }
    }
}
