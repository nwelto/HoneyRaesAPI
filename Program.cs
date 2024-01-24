using HoneyRaesAPI.Models;
List<Customer> customers = new List<Customer> {
    new Customer { Id = 1, Name = "John Doe", Address = "123 Elm St" },
    new Customer { Id = 2, Name = "Jane Smith", Address = "456 Oak St" },
    new Customer { Id = 3, Name = "Sarah Johnson", Address = "789 Pine St" }
};
List<Employee> employees = new List<Employee> {
    new Employee { Id = 1, Name = "Alice Brown", Specialty = "Plumbing" },
    new Employee { Id = 2, Name = "Bob White", Specialty = "Electrical" }
};
List<ServiceTicket> serviceTickets = new List<ServiceTicket> {
    new ServiceTicket { Id = 1, CustomerId = 1, EmployeeId = 1, Description = "Leaky faucet", Emergency = false, DateCompleted = DateTime.Now.AddDays(-2) },
    new ServiceTicket { Id = 2, CustomerId = 1, EmployeeId = 2, Description = "Broken light fixture", Emergency = true },
    new ServiceTicket { Id = 3, CustomerId = 2, EmployeeId = 1, Description = "Clogged drain", Emergency = false, DateCompleted = DateTime.Now.AddDays(-1) },
    new ServiceTicket { Id = 4, CustomerId = 3, Description = "Malfunctioning thermostat", Emergency = true },
    new ServiceTicket { Id = 5, CustomerId = 2, Description = "General maintenance", Emergency = false }
};
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/servicetickets/{id}", (int id) =>
{
    ServiceTicket serviceTicket = serviceTickets.FirstOrDefault(st => st.Id == id);
    if (serviceTicket == null)
    {
        return Results.NotFound();
    }
    serviceTicket.Employee = employees.FirstOrDefault(e => e.Id == serviceTicket.EmployeeId);
    serviceTicket.Customer = customers.FirstOrDefault(c => c.Id == serviceTicket.CustomerId);

    return Results.Ok(serviceTicket);
});

app.MapPost("/servicetickets", (ServiceTicket serviceTicket) =>
{
    serviceTicket.Id = serviceTickets.Max(st => st.Id) + 1;
    serviceTickets.Add(serviceTicket);
    return serviceTicket;
});

app.MapGet("/employees/{id}", (int id) =>
{
    Employee employee = employees.FirstOrDefault(e => e.Id == id);
    if (employee == null)
    {
        return Results.NotFound();
    }
    employee.ServiceTickets = serviceTickets.Where(st => st.EmployeeId == id).ToList();

    return Results.Ok(employee);
});

app.MapGet("/customers/{id}", (int id) =>
{
    Customer customer = customers.FirstOrDefault(e => e.Id == id);
    if (customer == null)
    {
        return Results.NotFound();
    }
    customer.ServiceTickets = serviceTickets.Where(st => st.CustomerId == id).ToList();

    return Results.Ok(customer);
});

app.Run();

