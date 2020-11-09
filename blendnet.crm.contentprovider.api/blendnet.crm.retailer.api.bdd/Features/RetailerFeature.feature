Feature: RetailerFeature
	Test all the CRUD operations for Retailer

@retailer
Scenario Outline: Create Retailer
  Given hub is "<present>" in the given data to create
  When I submit the request to create
  Then create response should recieve created
  And I should delete the created record
  Examples: 
    | present    |
    | true       |
    | false      |


@retailer
Scenario Outline: Read Retailer
  Given hub is "<present>" in the given data to create
  When I submit the request to create
  And I submit the request to read content
  Then read content response should recieve success with created id
  And I should delete the created record
  Examples: 
    | present    |
    | true       |
    | false      |

@retailer
Scenario Outline: Update Retailer
  Given hub is "<present>" in the given data to create
  When I submit the request to create
  And I submit the request to update content name
  And I submit the request to read updated content for updation
  Then update content response should receive nocontent and updated name value
  And I should delete the created record
  Examples: 
    | present    |
    | true       |
    | false      |

@retailer
Scenario Outline: Activate Retailer
  Given hub is "<present>" in the given data to create
  When I submit the request to create
  And I submit the request to update content activation
  And I submit the request to read updated content for activation
  Then update content response should receive nocontent and updated activated value
  And I should delete the created record
  Examples: 
    | present    |
    | true       |
    | false      |

@retailer
Scenario Outline: DeActivate Retailer
  Given hub is "<present>" in the given data to create
  When I submit the request to create
  And I submit the request to update content deactivation
  And I submit the request to read updated content for deactivation
  Then update content response should receive nocontent and updated deactivated value
  And I should delete the created record
  Examples: 
    | present    |
    | true       |
    | false      |

@retailer
Scenario Outline: Delete Retailer
  Given hub is "<present>" in the given data to create
  When I submit the request to create
  And I submit the request to delete
  And I submit the request to read content
  Then read content response should recieve notfound
  Examples: 
    | present    |
    | true       |
    | false      |