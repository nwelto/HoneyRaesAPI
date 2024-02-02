using HoneyRaesAPI.Models;


List<Customer> customers = new List<Customer>
{
    new Customer
    {
        Id = 1,
        Name = "John Doe",
        Address = "123 Elm St"
    },
    new Customer
    {
        Id = 2,
        Name = "Jane Smith",
        Address = "456 Oak St"
    },
    new Customer
    {
        Id = 3,
        Name = "Sarah Johnson",
        Address = "789 Pine St"
    },
    new Customer
    {
    Id = 4,
    Name = "Emily White",
    Address = "1010 Maple Lane"
    }
};

List<Employee> employees = new List<Employee>
{
    new Employee
    {
        Id = 1,
        Name = "Alice Brown",
        Specialty = "Plumbing"
    },
    new Employee
    {
        Id = 2,
        Name = "Bob White",
        Specialty = "Electrical"
    }
};

List<ServiceTicket> serviceTickets = new List<ServiceTicket>
{
    new ServiceTicket
    {
        Id = 1,
        CustomerId = 1,
        EmployeeId = 1,
        Description = "Leaky faucet",
        Emergency = false,
        DateCompleted = DateTime.Now.AddMonths(-6) 
    },
    new ServiceTicket
    {
        Id = 2,
        CustomerId = 1,
        EmployeeId = 2,
        Description = "Broken light fixture",
        Emergency = true,
        DateCompleted = null
        
    },
    new ServiceTicket
    {
        Id = 3,
        CustomerId = 2,
        EmployeeId = 1,
        Description = "Clogged drain",
        Emergency = false,
        DateCompleted = DateTime.Now.AddYears(-2) 
    },
    new ServiceTicket
    {
        Id = 4,
        CustomerId = 3,
        EmployeeId = 2,
        Description = "Malfunctioning thermostat",
        Emergency = true,
        DateCompleted = DateTime.Now.AddDays(-10) 
    },
    new ServiceTicket
    {
        Id = 5,
        CustomerId = 3,
        EmployeeId = 2,
        Description = "AC maintenance",
        Emergency = false,
        DateCompleted = DateTime.Now.AddYears(-1).AddDays(-1) 
    },
    new ServiceTicket
    {
        Id = 6,
        CustomerId = 2,
        EmployeeId = 1,
        Description = "Pipe repair",
        Emergency = false,
        DateCompleted = DateTime.Now.AddDays(-20) 
    },
        new ServiceTicket
    {
        Id = 7,
        CustomerId = 4,
        EmployeeId = 1,
        Description = "Old Completed Task",
        Emergency = false,
        DateCompleted = DateTime.Now.AddYears(-2) 
    },
            new ServiceTicket
    {
        Id = 8,
        CustomerId = 2,
        EmployeeId = 1,
        Description = "Furnace inspection",
        Emergency = false,
        DateCompleted = DateTime.Now.AddDays(-1) 
    },
    new ServiceTicket
    {
        Id = 9,
        CustomerId = 3,
        EmployeeId = 2,
        Description = "Window leak repair",
        Emergency = true,
        DateCompleted = DateTime.Now.AddDays(-15) 
    },
    new ServiceTicket
    {
        Id = 10,
        CustomerId = 1,
        EmployeeId = 1,
        Description = "Garage door malfunction",
        Emergency = false,
        DateCompleted = DateTime.Now.AddDays(-7) 
    },
    new ServiceTicket
    {
        Id = 11,
        CustomerId = 4,
        EmployeeId = 2,
        Description = "Gutter cleaning",
        Emergency = false,
        DateCompleted = DateTime.Now.AddDays(-20) 
    },
    new ServiceTicket
    {
        Id = 12,
        CustomerId = 2,
        EmployeeId = 1,
        Description = "Heating system maintenance",
        Emergency = false,
        DateCompleted = DateTime.Now.AddDays(-25) 
    }
};


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/servicetickets", () => Results.Ok(serviceTickets));

app.MapGet("/api/servicetickets/{id}", (int id) =>
{
    ServiceTicket serviceTicket = serviceTickets.FirstOrDefault(st => st.Id == id);
    if (serviceTicket == null)
    {
        return Results.NotFound();
    }

    serviceTicket.Employee = employees
        .Where(e => e.Id == serviceTicket.EmployeeId)
        .Select(e => new Employee { Id = e.Id, Name = e.Name, Specialty = e.Specialty })
        .FirstOrDefault();

    serviceTicket.Customer = customers
        .Where(c => c.Id == serviceTicket.CustomerId)
        .Select(c => new Customer { Id = c.Id, Name = c.Name, Address = c.Address })
        .FirstOrDefault();

    return Results.Ok(serviceTicket);
});


app.MapPost("/api/servicetickets", (ServiceTicket serviceTicket) =>
{
    serviceTicket.Id = serviceTickets.Any() ? serviceTickets.Max(st => st.Id) + 1 : 1;
    serviceTickets.Add(serviceTicket);
    return Results.Created($"/api/servicetickets/{serviceTicket.Id}", serviceTicket);
});

app.MapDelete("/api/servicetickets/{id}", (int id) =>
{
    var serviceTicket = serviceTickets.FirstOrDefault(st => st.Id == id);
    if (serviceTicket == null) return Results.NotFound();

    serviceTickets.Remove(serviceTicket);
    return Results.Ok(new { message = "Service ticket deleted successfully." });
});

app.MapPut("/api/servicetickets/{id}", (int id, ServiceTicket updatedTicket) =>
{
    var ticketToUpdate = serviceTickets.FirstOrDefault(st => st.Id == id);
    if (ticketToUpdate == null) return Results.NotFound();
    if (id != updatedTicket.Id) return Results.BadRequest(new { message = "Ticket ID mismatch." });


    ticketToUpdate.Description = updatedTicket.Description;
    ticketToUpdate.Emergency = updatedTicket.Emergency;
 

    return Results.Ok(ticketToUpdate);
});

app.MapGet("/api/employees", () => Results.Ok(employees));

app.MapGet("/api/employees/{id}", (int id) =>
{
    var employee = employees.FirstOrDefault(e => e.Id == id);
    if (employee == null) return Results.NotFound();
    employee.ServiceTickets = serviceTickets.Where(st => st.EmployeeId == id).ToList();

    return Results.Ok(employee);
});

app.MapGet("/api/customers", () => Results.Ok(customers));

app.MapGet("/api/customers/{id}", (int id) =>
{
    var customer = customers.FirstOrDefault(c => c.Id == id);
    if (customer == null) return Results.NotFound();
    customer.ServiceTickets = serviceTickets.Where(st => st.CustomerId == id).ToList();

    return Results.Ok(customer);
});



app.Run();

