Feature: Update order
As a user
I want to update my order status
So that I can rollout my order

Scenario: Update an order status
Given I have a order
When I update the order status
Then the order status is updated

Scenario: Update an order status with invalid status
Given I have a order
When I update the order status with invalid status
Then I receive a invalid status domain exception

Scenario: Update an order status with invalid order
Given I have a order
When I update the order status with invalid order
Then I receive a invalid order domain exception

Scenario: Update an order with internal error
Given I have a order
And there is an update internal error
When I update the order status
Then I receive a internal error for update