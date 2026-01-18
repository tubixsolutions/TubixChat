using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using TubixChat.BizLogicLayer.Services;
using TubixChat.DataLayer.Context;
using TubixChat.DataLayer.Repositories;
using TubixChat.Desktop.Services;
using TubixChat.Desktop.ViewModels;
using TubixChat.Desktop.Views;

namespace TubixChat.Desktop
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            var loginViewModel = ServiceProvider.GetRequiredService<LoginViewModel>();
            var loginWindow = new LoginWindow(loginViewModel);
            loginWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            //var connectionString = "Host=localhost;Port=5432;Database=tubix_chat;Username=postgres;Password=0327";
            var connectionString = "Host=ep-muddy-bonus-ahhdtk0v-pooler.c-3.us-east-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_TaqjWLwH5Y2t;SSL Mode=Require;Trust Server Certificate=true;";

            services.AddDbContext<EfCoreContext>(options => options.UseNpgsql(connectionString));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IChatService, ChatService>();

            // SignalR Service - Singleton
            services.AddSingleton<ISignalRService, SignalRService>();

            // API Service
            services.AddSingleton<IApiService, ApiService>();

            // ViewModels
            services.AddTransient<LoginViewModel>();
            services.AddTransient<RegisterViewModel>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            var signalRService = ServiceProvider.GetService<ISignalRService>();
            signalRService?.DisconnectAsync().Wait();

            base.OnExit(e);
        }
    }
}
