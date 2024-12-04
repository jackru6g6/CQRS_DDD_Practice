using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using SampleProject.Domain.Applications;
using SampleProject.Domain.Applications.Adapter;
using SampleProject.Domain.Applications.Behavior;
using SampleProject.Domain.Filters.OptimisticLock;
using SampleProject.Domain.Interfaces.Application;
using SampleProject.Domain.Interfaces.Repository;
using SampleProject.Domain.Repositories;
using System.Reflection;

public static class Program
{
    private static readonly Assembly _domainAssembly = Assembly.Load(new AssemblyName("SampleProject.Domain"));

    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // 註冊內建服務 Add services to the container.
        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        #region 客製化設定

        // MediatR 配置
        //builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
        builder.Services.AddMediatR(cfg =>
        {
            // 適配器
            //cfg.RegisterServicesFromAssemblyContaining(typeof(BaseApplication));
            //Assembly.Load(new AssemblyName("Apollo.MerchantCenter.Domain"));
            cfg.RegisterServicesFromAssembly(_domainAssembly);

            // 驗證器
            cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));

            // 推送非同步(擇一)
            //cfg.NotificationPublisher = new TaskWhenAllPublisher();

            // 推送同步(擇一，預設)
            //cfg.NotificationPublisher = new ForeachAwaitPublisher();

            // 客制化
            cfg.NotificationPublisher = new OptimisticLockExceptionRertyAdapterHandler();
        });

        // DI Container
        builder.Services.AddScoped<IOrderApplication, OrderApplication>();
        
        // Action Filter
        builder.Services.AddMvc(options =>
        {
            // 全域註冊 Controller Action Filter
            //options.Filters.Add<SelectAttribute>(); 
        });

        // 方法一
        // 註冊攔截器
        //builder.Services.AddScoped<OptimisticLockInterceptor>();
        //builder.Services.AddProxiedScoped<IOrderAggRepository, OrderAggV2Repository>();

        // 方法二
        #region Autofac

        builder.Services.AddScoped<IOrderAggRepository, OrderAggV2Repository>();
        // 使用 Autofac 替換內建 DI 容器
        builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

        // 使用 Autofac 的 ContainerBuilder 來擴展 DI，註冊autofac這個容器
        builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
        {
            // 註冊攔截器
            containerBuilder.RegisterType<OptimisticLockInterceptor>(); 

            // RegisterAssemblyTypes => 註冊所有集合
            // Where(t => t.Name.EndsWith("Service")) => 找出所有Service結尾的檔案
            // AsImplementedInterfaces => 找到Service後註冊到其所繼承的介面 
            containerBuilder.RegisterAssemblyTypes(_domainAssembly) // 使用 Autofac 的 RegisterAssemblyTypes 方法，將剛剛載入的組件中的所有類型批量註冊到 Autofac 的 DI 容器中
                            .Where(t => t.Name.EndsWith("Repository"))
                            .AsImplementedInterfaces() // 將這些類型的服務註冊為它們實現的介面，例如，類型 OrderService : IOrderService 會被註冊為 IOrderService，而不是它本身的具體類型
                            .EnableInterfaceInterceptors() // 啟用對註冊類型的介面攔截功能
                            .InterceptedBy(typeof(OptimisticLockInterceptor)); // 指定攔截器類型 OptimisticLockInterceptor，當攔截生效時執行該攔截器邏輯

            //containerBuilder.RegisterType<LoggingInterceptor>();
            //containerBuilder.RegisterType<MyService>()
            //    .As<IMyService>()
            //    .EnableInterfaceInterceptors()
            //    .InterceptedBy(typeof(LoggingInterceptor));
        });

        #endregion

        #endregion

        // 構建 WebApplication
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }

    public static IServiceCollection AddProxiedScoped<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
    {
        services.AddScoped<TImplementation>();
        services.AddScoped<TService>(provider =>
        {
            var implementation = provider.GetRequiredService<TImplementation>();
            var proxyGenerator = new ProxyGenerator();
            var cacheInterceptor = provider.GetRequiredService<OptimisticLockInterceptor>();

            return proxyGenerator.CreateInterfaceProxyWithTarget<TService>(implementation, cacheInterceptor);
        });

        return services;
    }
}