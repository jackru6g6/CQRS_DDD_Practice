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

#region �Ȼs�Ƴ]�w

// MediatR �t�m
//builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddMediatR(cfg =>
{
    // �A�t��
    cfg.RegisterServicesFromAssemblyContaining(typeof(BaseApplication));
    //Assembly.Load(new AssemblyName("Apollo.MerchantCenter.Domain"));
    //cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);

    // ���Ҿ�
    cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));

    // ���e�D�P�B(�ܤ@)
    //cfg.NotificationPublisher = new TaskWhenAllPublisher();

    // ���e�P�B(�ܤ@�A�w�])
    //cfg.NotificationPublisher = new ForeachAwaitPublisher();

    // �Ȩ��
    cfg.NotificationPublisher = new OptimisticLockExceptionRertyAdapterHandler();
});

// DI Container
builder.Services.AddScoped<IOrderApplication, OrderApplication>();
builder.Services.AddScoped<IOrderAggRepository, OrderAggV2Repository>();

//builder.Services.IntersectBy(typeof(SelectInterceptor));
// ���U�d�I��
builder.Services.AddScoped<IInterceptor, SelectInterceptor>();
//���U�ҥ~�B�zInterceptor
//builder.Services.Regi(typeof(ExceptionHandleInterceptor));

// Action Filter
builder.Services.AddMvc(options =>
{
    //options.Filters.Add<SelectAttribute>(); // ������U
    //options.Filters.Add(new SelectFilterAttribute()); // ������U
    //options.Filters.Add(typeof(SelectFilterAttribute)); // ������U
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