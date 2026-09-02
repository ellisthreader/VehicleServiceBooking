# Vehicle Service Booking System — Pre-Interview Task

## Overview

You have been provided with a solution scaffold for a vehicle service booking
system. The folder structure, project references, and database connection have
been pre-configured for you.

Your task is to implement the business logic, data access, API endpoints, and
tests across the existing layers. 

If anything is unclear, you may make reasonable assumptions.

---

## What Has Been Provided

- Project structure across three layers
- `DbContext` with connection string
- `Program.cs` with DI registration and middleware
- `~Module.cs` files to register DI on individual layers

---

## Your Task

### 1. Data Layer

Implement the following models with their requirements as listed below:

**`Service Type`**
- `Id` — Unique id for the type
- `Name` — Name of the type
- `Description` — Description of the type
- `Duration` — Duration in minutes

**`Appointment`**
- `Id` — Unique id for the appointment
- `Customer Name` — Name of the customer
- `Email` — Email contact for the customer
- `Phone` — Phone contact for the customer
- `Vehicle VIN` — VIN of the vehicle for service
- `Service Type ID` — Relational Id for the service type of the booking
- `Scheduled Date` - Date Time of the booking
  
Implement the repository or repositories:

**`ServiceTypeRepository`**
- `GetAllAsync()` — return all service types
- `GetByIdAsync(id)` — return a single service type, or `null` if not found

**`AppointmentRepository`**
- `GetAllAsync(page, pageSize)` — return a paginated list of appointments, including the related `ServiceType`
- `GetByIdAsync(id)` — return a single appointment including its related `ServiceType`, or `null` if not found
- `IsSlotTakenAsync(serviceTypeId, scheduledDateTime)` — check if an active appointment already exists for this service type at the given date and time
- `CreateAsync(appointment)` — create and return the new appointment
- `UpdateAsync(appointment)` — update and return the appointment
- `DeleteAsync(id)` — remove the appointment if found

or

**`Generic Repository`**
- `GetAllAsync(page, pageSize, includes, predicate?)` — return a paginated list of entities and relational data from the table, `predicate` optional
- `GetAsync(predicate, includes)` — retrieve singular entity and relational data from table
- `AnyAsync(predicate)` — check to see if entity exists using predicate
- `AddAsync(predicate)` — insert entity into table
- `UpdateAsync(entity)` — update entity in table
- `DeleteAsync(entity)` — delete entity from table


Once your models and repositories are implemented, run your initial migration:

```bash
dotnet ef migrations add InitialCreate --project ./VehicleServiceBooking.Data --startup-project ./VehicleServiceBooking.API
```

The application will apply pending Migrations and seed data using the Data Seeder, if you want to apply it sooner you can use the command below:

```bash
dotnet ef database update --project ./VehicleServiceBooking.Data --startup-project ./VehicleServiceBooking.API
```

You may have to seed your own data using the provided Data Seeder class. 

---

### 2. Application Layer

Implement Domain Transfer Objects for the Data Models:
- `Service Type Dto`
- `Appointment Dto`


Implement the following service classes:

**Whilst implementing your services, you can use libraries like AutoMapper or other alternatives to help make Mapping more consistent and configurable. You may also implement your own mapping. **

**`ServiceTypeService`**
- `GetAllAsync()` — retrieve all service types and map to `ServiceTypeDto`
- `GetByIdAsync(id)` — retrieve by ID and map to `ServiceTypeDto`

**`AppointmentService`**

- `CreateAsync(CreateAppointmentRequest request)` must enforce the following
  business rules before persisting:
  
  | Rule | Detail |
  |---|---|
  | VIN length | Must be exactly 17 characters |
  | Business hours | Must be Monday to Friday, 08:00–17:00 |
  | Hour slots | Must be on an exact hour (e.g. 09:00, not 09:30) |
  | No double-booking | Slot must not already be taken for the same service type |

- `UpdateAsync(int id, UpdateAppointmentRequest request)` must apply the same
validation rules as `CreateAsync`.

- `CancelAsync(int id)` — delegate to the repository and return the result.

> You can use private helper methods for your validation logic or other altnernatives as you see fit. 

---

### 3. API Layer

The endpoint empty classes have been templated, you will need to make the endpoints below:

- `ServiceTypeEndpoints.cs` — create a `GET` endpoint with exception handling which catches `401 Not Found`

- `AppointmentEndpoints.cs` — create both `POST` and `PUT` endpoints to allow Appointments to be made and updated, both endpoints should check for validation failes and return a `400 Bad Request` response with a descriptive message.

---

### 4. Tests

Implement the following test cases:

**Either NUnit or Moq can be used**

| Test | Description |
|---|---|
| `CreateAsync_ShouldThrow_WhenVINIsNot17Characters` | Invalid VIN should throw |
| `CreateAsync_ShouldThrow_WhenSlotIsOutsideBusinessHours` | 7pm slot should throw |
| `CreateAsync_ShouldThrow_WhenSlotIsOnAWeekend` | Saturday slot should throw |
| `CreateAsync_ShouldThrow_WhenSlotIsAlreadyTaken` | Taken slot should throw |
| `CreateAsync_ShouldReturnAppointmentDto_WhenRequestIsValid` | Happy path |
| `CancelAsync_ShouldReturnFalse_WhenAppointmentDoesNotExist` | Missing ID |
| `CancelAsync_ShouldReturnTrue_WhenAppointmentExists` | Happy path |

Run the tests with:

```bash
dotnet test
```

---

## Running the Application

The `appsettings.Development.json` has been setup with a default SQLite connection
string you can change this if you wish, then run using your IDE of choice or:

```bash
cd VehicleServiceBooking.API
dotnet run
```

Swagger UI will be available at:
- https://localhost:7212/swagger/index.html or
- http://localhost:5274/swagger/index.html
