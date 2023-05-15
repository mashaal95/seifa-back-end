# Project Setup

## SQL Database Initialization

### 1. Pull the SQL Server Docker image:
```powershell
docker pull mcr.microsoft.com/mssql/server:2022-latest
```
### 2. Initialize the SQL Server container:
```powershell
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=7Times=10!" -p 1433:1433 --name sql1 --hostname sql1 -d mcr.microsoft.com/mssql/server:2022-latest
```

### 3. Access the container's shell:
```powershell
docker exec -it sql1 "bash"
```
### 4. Connect to SQL Server:
```powershell
/opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "7Times=10!"
```
## Data Import

### 1. Copy CSV files to the Docker container:
```powershell
docker cp C:\Users\Mashaal\Documents\SEIFA_2011.csv sql1:/home
docker cp C:\Users\Mashaal\Documents\SEIFA_2016.csv sql1:/home
```

### 2. Initialize the database:
```sql
create database TestDb
```

### 3. Create tables for SEIFA scores:
```sql
CREATE TABLE [dbo].[seifa2011](
    [LocalGovtAreas] [varchar](max) NULL,
    [Locations] [varchar](max) NULL,
    [RelativeDisadvantage] [int] NULL,
    [RelativeAdvantage] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

CREATE TABLE [dbo].[seifa2016](
    [LGA_Code] [int] NULL,
    [LGA_Name] [varchar](max) NULL,
    [IndexOfRelativeSocioEconomicDisadvantageScore] [int] NULL,
    [IndexOfRelativeSocioEconomicDisadvantageDecile] [int] NULL,
    [IndexOfRelativeSocioEconomicAdvantageDisadvantageScore] [int] NULL,
    [IndexOfRelativeSocioEconomicAdvantageDisadvantageDecile] [int] NULL,
    [IndexOfRelativeEconomicScore] [int] NULL,
    [IndexOfRelativeEconomicDecile] [int] NULL,
    [IndexOfEducationAndOccupationScore] [int] NULL,
    [IndexOfEducationAndOccupationDecile] [int] NULL,
    [UsualResidentPopulation] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
```
### Bulk insert data from CSV files:
``` sql
bulk insert seifa2011 from '/home/SEIFA_2011.csv' with (FORMAT = 'CSV', FIRSTROW = 2)
bulk insert seifa2016 from '/home/SEIFA_2016.csv' with (FORMAT = 'CSV', FIRSTROW = 2)
```

## Web API Setup

### Create a new .NET Core 7 Web API project:
```powershell
dotnet new webapi -o SeifaAPI
```

### Install the required NuGet packages:

```powershell
dotnet add package Microsoft.EntityFrameworkCore.InMemory
dotnet add package Microsoft.EntityFrameworkCore.Design  
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```
### Scaffold the models based on the existing database tables:
```powershell
    dotnet ef dbcontext scaffold "Data Source=localhost;Initial Catalog=TestDB;User id=sa;Password=7Times=10!;TrustServerCertificate=Yes" Microsoft.EntityFrameworkCore.SqlServer
```
### Modify the database schema
- Add a SeifaID column to the seifa2011 table for the 2011 dataset.
- Make the LGA_Code column a primary key for the seifa2016 table.
- The reason why this is done, is to allow the dotnet libraries to scaffold the controllers for the next part

### Scaffold the sample controllers:
``` powershell
    dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design -v 7.0.0
    dotnet add package Microsoft.EntityFrameworkCore.Design -v 7.0.0
    dotnet add package Microsoft.EntityFrameworkCore.SqlServer -v 7.0.0
    dotnet tool uninstall -g dotnet-aspnet-codegenerator
    dotnet tool install -g dotnet-aspnet-codegenerator
    dotnet-aspnet-codegenerator controller -name Seifa2016Controller -async -api -m Seifa2016 -dc TestDbContext -outDir Controllers
    dotnet-aspnet-codegenerator controller -name Seifa2011Controller -async -api -m Seifa2011 -dc TestDbContext -outDir Controllers
```

### Additional Notes

Ensure that the Docker based SQL Server container is running and accessible before running the application.
Adjust the file paths in the docker cp commands based on your local file locations.
Modify the connection string in the dotnet ef dbcontext scaffold command to match your SQL Server credentials.
