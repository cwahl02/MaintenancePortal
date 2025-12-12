# Getting Started with MaintenancePortal

[← Back to README](README.md) | [Next: Features & Roadmap →](FEATURES.md)

---


## Prerequisites

### Required
- [`.NET 9 SDK`](https://dotnet.microsoft.com/en-us/download/dotnet/9.0) 
- [`Microsoft SQL Server`](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

### Optional but Recommended
- [`Visual Studio 2022`](https://visualstudio.microsoft.com/)
- [`SQL Server Management Studio (SSMS)`](https://learn.microsoft.com/en-us/ssms/install/install)

---

## Installation

### Clone or Download the Repository

Click the button to download the ZIP of the repository or clone it via Git:

[![Download zip](https://custom-icon-badges.demolab.com/badge/-Download-blue?style=for-the-badge&logo=download&logoColor=white "Download zip")](https://github.com/cwahl02/MaintenancePortal/archive/refs/heads/master.zip)

Alternatively, use Git to clone the repo:

```bash
git clone https://github.com/cwahl02/MaintenancePortal.git
```

---

## Build and Run

You can build and run the **MaintenancePortal** application either via **Visual Studio** or the **.NET CLI**.

### Option 1: Using Visual Studio

1. Open the solution file: `MaintenancePortal.sln`
2. Press **F5** to build and run the application
3. The application will open in your default browser

### Option 2: Using .NET CLI

Open a **Command Prompt**, **PowerShell**, or **Bash** window and navigate to the project folder containing `MaintenancePortal.csproj`:
- Navigate to project folder containing the project file: [`MaintenancePortal.csproj`](../master/MaintenancePortal/MaintenancePortal.csproj)
```bash
  cd MaintenancePortal
```

### 1. Restore dependencies
Make sure you have all required dependencies:
```bash
dotnet restore
```
### 2. Build
Run the following command to apply migrations and create the database:
```bash
dotnet build
```
### 3. Run
```bash
  dotnet run
```
---

[← Back to README](README.md) | [Next: Features & Roadmap →](FEATURES.md)
