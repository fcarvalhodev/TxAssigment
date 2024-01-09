# .NET Back-End API for TxAssignment
This project is a .NET Back-End API developed with C#, .NET 6.0, Redis, and Docker. It is designed to be easily set up and run in a development environment using either Visual Studio or Docker.

## System Requirements
To run this project, you will need:
* .NET SDK 6.0
* .NET Runtime 6.0
* Docker Desktop (for running the application in a Docker container)
* Visual Studio Community 2022 (optional, for running the application using Visual Studio)
## Running the Project
### Using Visual Studio
* Open the solution file in Visual Studio Community 2022.
* In the Solution Explorer, right-click on the TxAssigmentAPI project in the first layer of the solution.
* Choose "Set as StartUp Project".
* Press the "Play" button in the middle-top of the Visual Studio toolbar. You can choose to run the project either through IIS  Express or Docker (if you have Docker Desktop installed).

### Using Docker
* You can also run this project using the provided Dockerfile:
* Navigate to the directory containing the Dockerfile using PowerShell or your preferred terminal.
* Build the Docker image by running: docker build -t txassignmentapi .
* Once the build completes, run the container: docker run -p 8080:80 txassignmentapi
* The API will be available at http://localhost:8080.


This setup allows you to run the application without requiring Visual Studio, making it suitable for various environments including development, testing, and production.

## Wait? You don't wanna download anything ?

Awesome, here is the API URL that is hosted on an Azure free instance, just click on [this link](https://txassigmentapi20231210213613.azurewebsites.net/swagger/index.html).

## Author
This project was developed by Fábio de Paula Carvalho. For more information, connect with him on [LinkedIn](https://www.linkedin.com/in/fcarvalhodev/ ).

## Learn More
For detailed information about this project, including its architecture, features, and how to contribute, please visit our GitHub [Wiki](https://github.com/fcarvalhodev/TxAssigment/wiki).

