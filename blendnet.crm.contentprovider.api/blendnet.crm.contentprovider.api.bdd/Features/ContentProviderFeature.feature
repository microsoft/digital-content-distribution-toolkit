Feature: ContentProviderFeature
	In order to avoid silly mistakes
	As a math idiot
	I want to be told the sum of two numbers

Scenario: Create Content Provider
	Given the data to create content provider
	When I submit the request to create
	Then response should recieve created

@mytag
Scenario: Get Content Provider
	Given the created content provider
	When I submit the request to read
	Then response should recieve success