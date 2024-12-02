using Castle.DynamicProxy;
using SampleProject.Domain.Applications;
using SampleProject.Domain.Applications.Adapter;
using SampleProject.Domain.Applications.Behavior;
using SampleProject.Domain.Filters.OptimisticLock;
using SampleProject.Domain.Interfaces.Application;
using SampleProject.Domain.Interfaces.Repository;
using SampleProject.Domain.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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
    cfg.RegisterServicesFromAssemblyContaining(typeof(BaseApplication));
    //Assembly.Load(new AssemblyName("Apollo.MerchantCenter.Domain"));
    //cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);

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
builder.Services.AddScoped<IOrderAggRepository, OrderAggV2Repository>();

//builder.Services.IntersectBy(typeof(SelectInterceptor));
// 註冊攔截器
builder.Services.AddScoped<IInterceptor, SelectInterceptor>();
//註冊例外處理Interceptor
//builder.Services.Regi(typeof(ExceptionHandleInterceptor));

// Action Filter
builder.Services.AddMvc(options =>
{
    //options.Filters.Add<SelectAttribute>(); // 全域註冊
    //options.Filters.Add(new SelectFilterAttribute()); // 全域註冊
    //options.Filters.Add(typeof(SelectFilterAttribute)); // 全域註冊
});

#endregion

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