# Nancy [![Build Status](https://travis-ci.org/Jazaret/Nancy.svg?branch=master)](https://travis-ci.org/Jazaret/Nancy)

API written in DotNetCore 2.0 and Nancy 2.0 that manages accounts, topics & subscriptions.

## How to run application
From the solution path run `make runapp` or use the powershell script `run`

## Details

* Repository uses globally Geo-Redundant Session Consistancy Cosmos DB with SQL API
* Subscription document collection uses AccountID as partition key
* Search topics by news is cached by Redis using StackExchange & Azure Redis Cache
* Updates use ETag match for optimistic concurrency
* Application uses depenency inection for services/repositories/cache
* Unit Tests written in xunit with moq
* Build/Test run on commits with travis-ci
* Restful API returns with Hateoas navigation links

## Endpoints

* (There are curl examples to run in [the test commands file here](NancyApplication/testcommands.txt))

### Topic Routes

* GET /Topics/ - Returns list of all topics
* GET /Topics/Search?q=<searchword> - Returns list of topics that contains the search word. The search is cached in Redis
  
### Account Routes 
* POST /Account/Add - Adds account to repository.  Body must contain Query string of name="AccountNameHere"&pwd="PasswordHere"
* PUT /Account/{accountId}/Update - Updates account on repository.  Body must contain Query string of name="AccountNameHere"&pwd="PasswordHere"

### Subscription Routes
* POST /Subscriptions/{accountId}/Subscribe/{topicId} - Account subscribes to topic
* PUT /Subscriptions/{accountId}/Confirm/{confirmationToken} - Subscription is confirmed with AccountId & Confirmation Token
* DELETE /Subscriptions/{accountId}/Subscription/{subscriptionId} - Subscription is deleted with AccountId & SubscriptionId
