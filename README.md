# eps-task

# Design Decision
* This app was created in .Net 8 framework with Web Socket and EF core code first database.
* Web Socket was used as chosen Protocol as it is widely supported.
* Some exception handling was not properly implemented due to time constrain.
* Disclaimer: This solution could use much need improvement, but due to time contraint. I can't implement and test it as necessary. 

# Prerequisites:
* Latest Visual Studio Installation
* MSSQL SERVER Express or Higher
* Open the project in Visual Studio.
* Postman(acting as client).

# Generating the database table:
Step 1: Update the "DefaultConnection" string in EPS.Server/appsetting.json file and replaced it with your own SQLSERVER connection string.
Step 2: Open Package Manager Console in Visual Studio Tools and run "Update-Database" command. This should generate the tables within your DB.

# Running the EPS Server:
* Install Required Packages via Nuget Package Manager
* Click "http" button in your Visual Studio to run the EPS Server.

# Generating Code:
* Open Postman and create new websocket request as per image below:
<img width="853" height="414" alt="image" src="https://github.com/user-attachments/assets/71743016-1470-4834-9022-cfc83dfb97a3" />

# Activating Code:
* With the newly generated code, use it in Postman to activate Discount Code:
<img width="846" height="452" alt="image" src="https://github.com/user-attachments/assets/342266e0-85fc-4043-925e-2b25c3db882e" />



