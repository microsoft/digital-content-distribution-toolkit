# Overview
This flow describes the Content Provider onboarding along with the Content Administrator association.

# Flow

![hign-level-flow.png](hign-level-flow.png)
1. Content Provider User will self -register on ADb2C tenant
2. The details of the user(email id, userid/objectid) will be provided to the Super Administrator using an email.
3. The CSA will in turn create a Content Provider with the administrators provided in the email associated with the CP.
4. The details of the Content Administrator would be sought from the User Management Service API. These details in turn would be sent to the CRM API and a copy of the user details would be stored int he CRM database.    
5. After the Content Provider is created and the Content Administrator is successfully associated, a domain event is raised - ContentProviderCreatedEvent. 
6. After the Content Provider is created and the Content Administrators are changed successfully, a domain event is raised - ContentProviderUpdateEvent. 


# Assumption
1. The Content Super Admin (CSA) will be provisioned using the backend.
2. The Content Provider Users would be OK to self-register on ADB2C. Else, these identities and the associated passwords would be created by the Content Super Admin.
3. There would be data replication between the AD B2c and the CRM application. This is to reduce the coupling between the service and improve the performance and availability.
4. The Content Administrator profile cannot be changed from the CRM application. The changes need to be made by the individual users by logging in to AD B2C.
5. Only association/disassociation of the users to the Content Provider is permitted int he CRM application. This in turn would result in changes to the ADb2C groups namely - Content Administrator Group.