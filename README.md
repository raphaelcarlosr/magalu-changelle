# Features
[x] Migrations
Layer architecture
Clean Architecture
S.O.L.I.D. principles
Clean Code
Domain Validations
Domain Notifications
Domain Driven Design
CQS
Repository Pattern
Notification Pattern
Mapper by Extension Methods

# Contrib

## Tests
```powershell
dotnet test ./tests
```
## Run
```powershell
dotnet run -p ./api
```
## Publish
```powershell
dotnet publish ./api -o ./package -c Release
```
## Add migration

Open your Package Manager Console
Change the default project to Aurora.Infra.Data
Run command "Add-Migration [NAME OF YOUR MIGRATION]"
Run command "Update-Database"