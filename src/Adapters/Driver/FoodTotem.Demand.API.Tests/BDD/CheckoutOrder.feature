Feature: Checkout order
As a user
I want to checkout my order
So that I can get my food

Scenario: Checkout order
Given I have an order
When I checkout
Then I should successfully queue my order

Scenario: Checkout order with invalid order should return domain error
Given I have an invalid order
When I checkout
Then I should get a domain error

Scenario: Checkout order with internal error should return internal error
Given I have an order
And there is an internal error
When I checkout
Then I should get an internal error