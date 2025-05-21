# Fitbot

## Requirements

To run this software, ensure the following dependencies are installed:

- .NET 9  
- Docker  
- Node.js  

---

## Setup Instructions

### 1. Install Web App Dependencies

Navigate to the web app directory:

```bash
cd SW6-project/fitnesswebapp
npm install
```


### 2. Run the Program
Use Aspire to run the backend and SmartDisplay app:

```bash
dotnet run --project .\FitnessApp.AppHost\
```
After launching, the console will provide a link to the Aspire dashboard, which includes access to the different server routes.

### 3. Run the Android App
To run the Android app, you’ll need an emulator or a physical device connected via ADB.

Steps:
Download and install Android Studio.

Set up the Android SDK and required dependencies.

Navigate to the fitnessapp directo  ry and run:

```bash
dotnet build
dotnet run
```
Alternatively, open the solution in Visual Studio and run the project directly. This method simplifies emulator setup and debugging.