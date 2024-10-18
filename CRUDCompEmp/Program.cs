
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "yourdomain.com",
            ValidAudience = "yourdomain.com",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("vainitaOMGclavelargaysegura_a234243423423awda"))
        };
    });

builder.Services.AddAuthorization();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();


var companies = new List<Company>{
    new Company{Id = 1, Name= "Alan company"},
    new Company{Id = 2, Name= "Jose company"}
};
var employees = new List<Employee>{
    new Employee{Id=1, Name="Alan Alarcon", CompanyId=1},
    new Employee{Id=2, Name="Joel Peña", CompanyId=1}
};


int companyIdCounter = 1;
int employeeIdCounter = 1;


app.MapGet("/companies", () =>
{
    return Results.Ok(companies);
});

app.MapGet("/companies/{id}", (int id) =>
{
    var company = companies.FirstOrDefault(c => c.Id == id);
    return company is not null ? Results.Ok(company) : Results.NotFound();
});

app.MapPost("/companies", (Company company) =>
{
    company.Id = companyIdCounter++;
    companies.Add(company);
    return Results.Created($"/companies/{company.Id}", company);
});

app.MapPut("/companies/{id}", (int id, Company updatedCompany) =>
{
    var company = companies.FirstOrDefault(c => c.Id == id);
    if (company is null) return Results.NotFound();

    company.Name = updatedCompany.Name;
    return Results.Ok(company);
});

app.MapDelete("/companies/{id}", (int id) =>
{
    var company = companies.FirstOrDefault(c => c.Id == id);
    if (company is null) return Results.NotFound();

    var hasEmployees = employees.Any(e => e.CompanyId == id);
    if (hasEmployees)
        return Results.BadRequest("Cannot delete company because it has assigned employees.");

    companies.Remove(company);
    return Results.NoContent();
});


app.MapGet("/employees", () => employees);

app.MapGet("/employees/{id}", (int id) =>
{
    var employee = employees.FirstOrDefault(e => e.Id == id);
    return employee is not null ? Results.Ok(employee) : Results.NotFound();
});

app.MapPost("/employees", (Employee employee) =>
{
    employee.Id = employeeIdCounter++;
    employees.Add(employee);
    return Results.Created($"/employees/{employee.Id}", employee);
});

app.MapPut("/employees/{id}", (int id, Employee updatedEmployee) =>
{
    var employee = employees.FirstOrDefault(e => e.Id == id);
    if (employee is null) return Results.NotFound();

    employee.Name = updatedEmployee.Name;
    employee.CompanyId = updatedEmployee.CompanyId;
    return Results.Ok(employee);
});

app.MapDelete("/employees/{id}", (int id) =>
{
    var employee = employees.FirstOrDefault(e => e.Id == id);
    if (employee is null) return Results.NotFound();

    employees.Remove(employee);
    return Results.NoContent();
});

app.MapDelete("/companies/{id}/with-employees", (int id) =>
{
    var company = companies.FirstOrDefault(c => c.Id == id);
    if (company is null) return Results.NotFound();

    employees.RemoveAll(e => e.CompanyId == id);
    companies.Remove(company);

    return Results.NoContent();
});

app.MapGet("/employee/{id}/withCompany", (int id) =>
{
    var employee = employees.FirstOrDefault(e => e.Id == id);
    if (employee is null) return Results.NotFound();
    var company = companies.FirstOrDefault(c => c.Id == employee.CompanyId);
    if (company is null) return Results.NotFound();

    var employeeCompany = new
    {
        employeeId = employee.Id,
        employeeName = employee.Name,
        employeeCompany = company.Name
    };

    return Results.Ok(employeeCompany);



});

app.Run();

public class Company
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class Employee
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int CompanyId { get; set; }
}


