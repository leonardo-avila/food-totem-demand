Feature: Get orders
As a user
I want to get orders
So that I can see what I have ordered

Scenario: Get orders
Given there is orders
When I get orders
Then I should get orders

Scenario: Get orders without orders
Given there is no orders
When I get orders
Then I should receive no content

Scenario: Get order by id
Given there is an order
When I get order by id
Then I should get the specific order

Scenario: Get order by invalid id
Given there is an order
When I get order by invalid id
Then I should receive not found domain exception

Scenario: Get queued orders
Given there is orders
When I get queued orders
Then I should get orders

Scenario: Get queued orders without orders
Given there is no orders
When I get queued orders
Then I should receive no content

Scenario: Get ongoing orders
Given there is orders
When I get ongoing orders
Then I should get orders

Scenario: Get ongoing orders without orders
Given there is no orders
When I get ongoing orders
Then I should receive no content