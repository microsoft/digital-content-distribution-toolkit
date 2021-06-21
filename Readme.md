## Clone the repository 
 Use `git clone https://binemsr@dev.azure.com/binemsr/bine-crm/_git/bine-crm` to clone the respository to your local machine 

## Backend Setup
- Download Visual Studio
-  Open the file "blendnet.crm.contentprovider.api.sln" using Visual Studio
- In Solution Explorer right click on one of the api services (E.g. blendnet.user.api) 
    - Select Properties->Debug
    - Set "ASPNETCORE_ENVIRONMENT" value to "Development" in environment variables
        
- Add secrets.json files to each service. Secrets files are available at
[secrets json](https://microsoft.sharepoint.com/teams/BlendNet9/Shared%20Documents/Forms/AllItems.aspx?id=%2Fteams%2FBlendNet9%2FShared%20Documents%2FDevelopers%2FLocal%2DVs&p=true&ct=1623695129064&or=OWA%2DNT&cid=af3ccf17%2D54ef%2Dbad8%2Dfbd6%2D7d1719dd5426&originalPath=aHR0cHM6Ly9taWNyb3NvZnQuc2hhcmVwb2ludC5jb20vOmY6L3QvQmxlbmROZXQ5L0VsU1lzZmkxSEt4RXJnV2FEOEdaU0lzQmZuLUpHb0NKMHYxcmZkQ2RkVE40V3c%5FcnRpbWU9bTBNTXkyRXYyVWc)

- To run a service locally : 
    - Right Click on the service in Solution Explorer
    - Select Debug->Start New Instances