using GojiApi.Data;
using GojiApi.Delegate;
using GojiApi.Delegate;
using GojiApi.Endpoints;
using GojiApi.Model.Project;
using GojiApi.Model.TaskItem;
using GojiApi.Model.User;
using GojiApi.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton<SqliteConnectionFactory>();
builder.Services.AddScoped<IUser, UserRepository>();
builder.Services.AddScoped<IProject, ProjectRepository>();
builder.Services.AddScoped<ITaskItem, TaskItemRepository>();
builder.Services.AddScoped<IUserDelegate, UserDelegate>();
builder.Services.AddScoped<IProjectDelegate, ProjectDelegate>();
builder.Services.AddScoped<ITaskDelegate, TaskDelegate>();


var app = builder.Build();

DbInitializer.Initialize(app.Services.GetRequiredService<SqliteConnectionFactory>());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "GojiApi v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

//app.MapControllers();

app.MapProjectsEndpoints();
app.MapTasksEndpoints();
app.MapUsersEndpoints();


app.Run();
