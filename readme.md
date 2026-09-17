# Vehicle Service Booking

Completed tech-check implementation for a vehicle service booking API.

## What was completed

- Built the API with ASP.NET Core Minimal APIs and SQLite.
- Added EF Core models, migrations, repositories, and seed service types.
- Added appointment creation, updating, cancellation, validation, and duplicate-slot protection.
- Added DTOs so database entities are not exposed directly through the API.
- Added NUnit/Moq tests covering the required booking rules and cancellation behavior.

## Run it

Requires the .NET 10 SDK.

```bash
dotnet restore
dotnet build
dotnet test
dotnet run --project ./VehicleServiceBooking.API
```

Swagger is available at `/swagger` while running in Development. The SQLite
database is created automatically and seeded with three service types.

## Endpoints

- `GET /service-types`
- `GET /service-types/{id}`
- `POST /appointments`
- `PUT /appointments/{id}`
- `DELETE /appointments/{id}`

Appointments must be booked on weekdays, on the hour, between 08:00 and 17:00.
VINs are normalized to uppercase and must be exactly 17 characters.
