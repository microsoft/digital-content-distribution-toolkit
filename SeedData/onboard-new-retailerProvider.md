# Adding Service Accounts data

## Add new SuperAdmin

1. Create service account in Kaizala service
   - get Kaizala ID and phone number
1. Assign SuperAdmin role to above account in Kaizala service
1. Create a user object with above Kaizala ID and phone number

## Add new Retailer Provider

1. Create service account in Kaizala service
   - get Kaizala ID and phone number
1. Assign RetailerManagement role to above account in Kaizala service
1. Create a user object with above Kaizala ID and phone number
   - get User.ID
1. Using super admin token, call RMS - `api/v1/RetailerProvider/create`

   | parameter | comment |
   | - | - |
   | `name` | Retailer Provider's name |
   | `partnerCode` | 4-character partner code
   | `userId` | User.ID |

## Seed data

### Dev

| Container | file path | Contents |
| - | - | - |
| User | [User-dev.json](User-dev.json) | SuperAdmin, TSTP |
| RetailerProvider | [RetailerProvider-dev.json](RetailerProvider-dev.json) | TSTP |

### Stage

| Container | file path | Contents |
| - | - | - |
| User |  | SuperAdmin, TSTP |

### Prod

| Container | file path | Contents |
| - | - | - |
| User | | SuperAdmin |
