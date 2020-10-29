Feature: ContentProviderFeature
	Test all the CRUD operations for Content Provider

@contentProvider
Scenario Outline: Create Content Provider
  Given admin is "<present>" in the given data to create
  When I submit the request to create
  Then create response should recieve created
  And I should delete the created record
  Examples: 
    | present    |
    | true       |
    | false      |

@contentProvider
Scenario Outline: Read Content Provider
  Given admin is "<present>" in the given data to create
  When I submit the request to create
  And I submit the request to read content
  Then read content response should recieve success with created id
  And I should delete the created record
  Examples: 
    | present    |
    | true       |
    | false      |

@contentProvider
Scenario Outline: Update Content Provider
  Given admin is "<present>" in the given data to create
  When I submit the request to create
  And I submit the request to update content name
  And I submit the request to read updated content for updation
  Then update content response should receive nocontent and updated name value
  And I should delete the created record
  Examples: 
    | present    |
    | true       |
    | false      |

@contentProvider
Scenario Outline: Activate Content Provider
  Given admin is "<present>" in the given data to create
  When I submit the request to create
  And I submit the request to update content activation
  And I submit the request to read updated content for activation
  Then update content response should receive nocontent and updated activated value
  And I should delete the created record
  Examples: 
    | present    |
    | true       |
    | false      |

@contentProvider
Scenario Outline: DeActivate Content Provider
  Given admin is "<present>" in the given data to create
  When I submit the request to create
  And I submit the request to update content deactivation
  And I submit the request to read updated content for deactivation
  Then update content response should receive nocontent and updated deactivated value
  And I should delete the created record
  Examples: 
    | present    |
    | true       |
    | false      |