using Microsoft.AspNetCore.Mvc.ApiExplorer;
using PeopleRegistration.Infrastructure.Data;
using PeopleRegistration.Infrastructure.IoC;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddPeopleRegistrationInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
    options.AddPolicy("Default", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod())
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();

var app = builder.Build();
await app.Services.GetRequiredService<IServiceProvider>().SeedAsync();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
    app.UseSwaggerUI(c =>
    {
        foreach (var desc in provider.ApiVersionDescriptions)
            c.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json", desc.GroupName.ToUpperInvariant());
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