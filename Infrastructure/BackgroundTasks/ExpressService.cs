using Domain.DTOs.Email;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class ExpressService : BackgroundService
    {
        private readonly ILogger<ExpressService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1);

        public ExpressService(ILogger<ExpressService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("ExpressService запущен.");

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();
                        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                        var now = DateTime.UtcNow;
                        var expiredProducts = await dbContext.Products
                            .Include(p => p.User)
                            .Where(p => p.PremiumOrTopExpiryDate != null && p.PremiumOrTopExpiryDate < now)
                            .ToListAsync(stoppingToken);

                        foreach (var product in expiredProducts)
                        {
                            product.IsTop = false;
                            product.IsPremium = false;
                            product.PremiumOrTopExpiryDate = null;

                            var emailDto = new EmailDto
                            {
                                To = product.User?.Email,
                                Subject = "Срок продвижения истёк",
                                Body = $"Срок действия продвижения продукта {product.Name} истёк."
                            };

                            await emailService.SendEmailAsync(emailDto);
                        }

                        if (expiredProducts.Any())
                        {
                            await dbContext.SaveChangesAsync(stoppingToken);
                            _logger.LogInformation($"Обновлено {expiredProducts.Count} продуктов.");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Ошибка при выполнении фоновой проверки.");
                    }

                    await Task.Delay(_checkInterval, stoppingToken);
                }

                _logger.LogInformation("ExpressService завершён.");
            }
        }
    }
    
