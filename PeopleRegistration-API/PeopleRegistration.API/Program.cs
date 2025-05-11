using Microsoft.AspNetCore.Mvc.ApiExplorer;
using PeopleRegistration.Infrastructure.Data;
using PeopleRegistration.Infrastructure.IoC;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
    options.AddPolicy("Default", policy =>
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
));


builder.Services
       .AddPeopleRegistrationInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    await context.Database.EnsureCreatedAsync();
    await services.SeedAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
    app.UseSwaggerUI(c =>
    {
        foreach (var desc in provider.ApiVersionDescriptions)
            c.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json",
                              desc.GroupName.ToUpperInvariant());
        c.RoutePrefix = "swagger";
    });
}

app.UseRouting();
app.UseCors("Default");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();