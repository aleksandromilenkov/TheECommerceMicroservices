ECommerce Microservices

This project is an E-Commerce Microservices Architecture built using .NET, with a focus on Clean Architecture principles. It consists of multiple microservices that handle different business functionalities, along with an API Gateway for routing and security. Also there are unit tests for the controllers, services and repositories of each microservice implemented with xUnit.  


Technologies Used

## .NET 8 (C#)  

## ASP.NET Web API  

## Entity Framework Core (SQL Server)  

## Ocelot API Gateway  

## JWT Authentication (Custom implementation)  

## Ocelot CacheManager  

## Polly package for retry pipeline

## Serilog Logging  

## Auto-Mapper  

xUnit  

I have created 4 microservices:

1. API Gateway Microservice - which is using ocelot to limit the calls to the API and register the allowed routes. Deciding which routes are requiring authentication and which aren't. Also I'm caching the products and orders.
  
2. Authentication Microservice - For manipulating with the application's users. Retreiving, registering, loging, updating and deleting users.
 
3. Orders Microservice - For manipulating with the orders - Here I'm using synchrouns communication with the Product and Authentication Microservice for getting the Order Details and User's orders.  Also there are other basic CRUD operations for the Order entity.

4. Product Microservice - For manipulating with the products.

5. Shared Library - this is Class Library project (not microservice) which is referenced in every microservice because here i have generic methods for connecting to the database, declaring JWT authentication scheme, logging with Serilog, generic interface for the repositories, Middlewares for checking if the Request contain's the "App-Gateway" header which is specified in the API Gateway.

I'm using Microsoft SQL Server database, just one database for all the microservices.  
There are also some limitations for example to create, update and delete the products you must have a role of admin.  
