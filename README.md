# ATM
ATM provides money transfer and withdrawal functions

# Swoop Core API

The Swoop Core API (also known as the Swoop Classic API) is an ASP.NET Web API project with associated Windows service applications that expose the core functionality of the Swoop web app.

## Development environment setup
> It is strongly recommended that Windows is used if at all possible as there are major shortcomings with both MacOS and Linux when it comes to debugging .NET Framework applications.

1. Install the required software.
    ----------------------------------------------------------------
    #### Windows 
    ##### Required
    * [Visual Studio](https://visualstudio.microsoft.com/)
	* [Ubuntu](https://www.microsoft.com/en-gb/p/ubuntu/9nblggh4msv6) Download via Microsoft Store.
		When you download and install it will ask to set for username and password.
	* [Docker](https://www.docker.com/) 
		If you get WSL2 installation incomplete error message: [Download the Linux kernel update package and run it] (https://docs.microsoft.com/en-gb/windows/wsl/install-win10#step-4---download-the-linux-kernel-update-package)
		Alternatively manual setup info here:[WSL2](https://docs.microsoft.com/en-us/windows/wsl/install-win10)
	
	* Make sure IIS installed (Core API runs on IIS at https://localhost/hawk)
	
    ##### Optional
    * [SQL Server Management Studio](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)
    * [Robo3T](https://robomongo.org/)
    ----------------------------------------------------------------
    #### MacOS 
    ##### Required
    * [Visual Studio](https://visualstudio.microsoft.com/vs/mac/) or [Visual Studio Code](https://code.visualstudio.com/)
    * [Docker](https://www.docker.com/)
    ##### Optional
    * [Azure Data Studio](https://docs.microsoft.com/en-us/sql/azure-data-studio/download-azure-data-studio)
    * [Robo3T](https://robomongo.org/)
    ----------------------------------------------------------------
    #### Linux 
    ##### Required
    * [Visual Studio Code](https://code.visualstudio.com/)
    * [Docker](https://www.docker.com/)
    ##### Optional
    * [Azure Data Studio](https://docs.microsoft.com/en-us/sql/azure-data-studio/download-azure-data-studio)
    * [Robo3T](https://robomongo.org/)
    ----------------------------------------------------------------
2. Clone the GitHub repository.
    ```powershell
    git clone https://github.com/swoop-ltd/swoop-api.git
    ```
3. Run the Powershell script in the root of the repository to create and run the Docker containers (n.b. this has not been tested on MacOS or Linux and almost certainly won't work on those operating systems).
    ```powershell
    & '.\Run dependencies.ps1'
    ```
	
	Script will pull images for mssql2019, mongodb and rabbitMq then build and publish swoop database.
	Images will be running in docker as:
		* swoop-rabbitmq  at http://localhost:15672/
		* swoop-mongodb   at localhost port 27017
		* swoop-sqlserver at localhost port 1433 
		
		Checkout container environment variables for user credentials setup.
	
## Troubleshooting
 * 	Run dependencies.ps1 Errors
	If you get error message stating "Run dependencies.ps1 is not digitally signed. You cannot run this script on the current system."
	Set policy temporarly or permenantly e.g.  Set-ExecutionPolicy unrestricted

	If you get error below:
	--------------------------------------- Build SQL project ---------------------------------------
	Could not execute because the application was not found or a compatible .NET SDK is not installed.

	You need to install .net sdk. E.g. [dotnet-sdk-3.1.411-win-x64] (https://download.visualstudio.microsoft.com/download/pr/842e20e5-8cd4-4fe1-bdc5-5d27a45552dd/5660663ac2e8747101d040c7764a79c2/dotnet-sdk-3.1.117-win-x64.exe)
	
 * Run Swoop.Core API solution as Admin in VS.
   	* Before you run solution make sure IIS installed and enable WCF http Activation.
	* Restart VS in admin mode and Load project, got to web tab and create virtual directory.
	
	Hawk site is created under Default Web Site on IIS and points to path: local path e.g. C:\Projects\swoop-ltd\swoop-api\src\Swoop.Core.Api
	If above doesn’t work create website manually on IIS.
	Create app pool, make sure configure Https binding at Default site configured.
   
 * Build Error
	If you get this  error: Invalid option '7.3' for /langversion. Use '/langversion:?' 
	change the project framework to .net4.6 and then back to 4.8.

 * Run Swoop.Databse Solution
     If you get below error:
		Your project does not reference ".NETFramework,Version=v4.8" framework.
		Add a reference to ".NETFramework,Version=v4.8" in the "TargetFrameworks" property of your project file and then re-run NuGet restore.
	 Then do the following steps:
		- Clean project
		- Delete bin and obj folders
		- Builds again.
