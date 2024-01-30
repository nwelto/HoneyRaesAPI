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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/servicetickets", () =>
{
    foreach (var ticket in serviceTickets)
    {
        ticket.Customer = customers.FirstOrDefault(c => c.Id == ticket.CustomerId);
        ticket.Employee = employees.FirstOrDefault(e => e.Id == ticket.EmployeeId);
    }
    return serviceTickets;
});



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

app.MapDelete("/servicetickets/{id}", (int id) =>
{
    var serviceTicket = serviceTickets.FirstOrDefault(st => st.Id == id);
    if (serviceTicket != null)
    {
        serviceTickets.Remove(serviceTicket);
    }
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

    // Explanations for updating a service ticket.
app.MapPut("/servicetickets/{id}", (int id, ServiceTicket serviceTicket) =>
{
    // Find the service ticket in the list that matches the given ID.
    ServiceTicket ticketToUpdate = serviceTickets.FirstOrDefault(st => st.Id == id);

    // Get the index of the ticket to update in the list.
    int ticketIndex = serviceTickets.IndexOf(ticketToUpdate);

    // Check if the service ticket was found. If not, return a 'NotFound' result.
    if (ticketToUpdate == null)
    {
        return Results.NotFound();
    }

    // Ensure the ID in the URL matches the ID in the request body. If not, return a 'BadRequest'.
    if (id != serviceTicket.Id)
    {
        return Results.BadRequest();
    }

    // Replace the old service ticket in the list with the updated one from the request.
    serviceTickets[ticketIndex] = serviceTicket;

    // Return an 'Ok' result indicating the update was successful.
    return Results.Ok();
});

app.MapPost("/servicetickets/{id}/complete", (int id) =>
{
    ServiceTicket ticketToComplete = serviceTickets.FirstOrDefault(st => st.Id == id);

    if (ticketToComplete == null)
    {
        return Results.NotFound();
    }

    ticketToComplete.DateCompleted = DateTime.Today;

    return Results.Ok(ticketToComplete);
});

app.MapGet("/servicetickets/emergencies", () =>
{
    return serviceTickets.Where(ticket => ticket.Emergency).ToList();
});
app.MapGet("/servicetickets/unassigned", () =>
{
    return serviceTickets.Where(ticket => ticket.EmployeeId == null || ticket.EmployeeId == 0).ToList();
});

app.MapGet("/customers/inactive", () =>
{
    var oneYearAgo = DateTime.Today.AddYears(-1);

    var inactiveCustomers = customers.Where(customer =>
        !serviceTickets.Any(ticket => ticket.CustomerId == customer.Id &&
                                      ticket.DateCompleted > oneYearAgo)).ToList();

    return inactiveCustomers;
});

app.MapGet("/employees/available", () => 
{

    var openTickets = serviceTickets.Where(t => t.DateCompleted == null);

    var assignedEmployeeIds = openTickets.Select(t => t.EmployeeId).ToList();

    var availableEmployees = employees.Where(e => !assignedEmployeeIds.Contains(e.Id));

    return availableEmployees;

});
app.MapGet("/employees/{id}/customers", (int id) =>
{
    var ticketsForEmployee = serviceTickets.Where(st => st.EmployeeId == id).ToList();

    var customerIds = ticketsForEmployee.Select(st => st.CustomerId).Distinct();

    var customersForEmployee = customers.Where(c => customerIds.Contains(c.Id)).ToList();

    return customersForEmployee;
});

app.MapGet("/employee/eotm", () =>
{
    DateTime lastMonth = DateTime.Now.AddMonths(-1);
    Employee employeeOfMonth = employees.OrderByDescending(e => serviceTickets.Count(st => st.EmployeeId == e.Id && st.DateCompleted.HasValue && st.DateCompleted.Value.Month == lastMonth.Month && st.DateCompleted.Value.Year == lastMonth.Year)).FirstOrDefault();

    return Results.Ok(employeeOfMonth);
});

app.MapGet("/servicetickets/review", () =>
{
    List<ServiceTicket> completedTickets = serviceTickets.Where(st => st.DateCompleted.HasValue).OrderBy(st => st.DateCompleted).ToList();

    foreach (var ticket in completedTickets)
    {
        ticket.Customer = customers.FirstOrDefault(c => c.Id == ticket.CustomerId);
        ticket.Employee = employees.FirstOrDefault(e => e.Id == ticket.EmployeeId);
    }

    return Results.Ok(completedTickets);
});




app.Run();

