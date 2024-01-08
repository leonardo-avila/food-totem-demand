Feature: Delete order
As a user
I want to delete an order
So that I can remove orders that I do not want anymore

Scenario: Delete order
Given I have a order
When I delete the order
Then the order is deleted

Scenario: Delete order that does not exist
Given I do not have an order
When I delete the order
Then I get an domain exception

Scenario: Delete order with internal error
Given I have a order
And there is a internal error
When I delete the order
Then I get an internal error
