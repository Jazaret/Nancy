# Nancy

API written in DotNetCore 2.0 and Nancy 2.0 that manages accounts, topics & subscriptions.

Repository uses DynamoDB SQL api and a Redis cache. 

# How to run
From the solution path execute the following commands:

dotnet restore
dotnet build
dotnet run --project .\NancyApplication\NancyApplication.csproj