export const environment = {
    production: false,
    allowedMaxSelection: 5,
    maxFileUploadSize: 1000000,
    fileAllowedType: 'application/json',
    channelName: "CMSPortal",
    contentProviderApiUrl: "http://20.204.71.102/cmsapi/api/v1/ContentProvider",
    contentApiUrl: "http://20.204.71.102/cmsapi/api/v1/Content",
    userApiUrl: "http://20.204.71.102/userapi/api/v1/User",
    incentiveApiUrl: "http://20.204.71.102/incentiveapi/api/v1/Incentive",
    dashUrlPrefix: "https://ampdemo.azureedge.net/?url=",
    kaizalaApi0: "https://api-preprod.kaiza.la/v1/",
    kaizalaApi1: "https://api-preprod.kaiza.la/v1/",
    kaizalaApi2: "https://api-preprod.kaiza.la/v1/",
    kaizalaSignUpSignIn: "LoginWithPhoneForPartners",
    kaizalaVerifyOTP: "VerifyPhonePinForPartnerLogin",
    kaizalaGetUserRoles : "ValidatePartnerAccessToken",
    kaizalaAppNameParam : "applicationName",
    kaizalaAppName : "com.microsoft.mobile.polymer.mishtu",
  
    widewineTokenPrefix: "&widevine=true&token=Bearer%3D",
      filters : [
      {
        "filterName" : "Language",
        "filterValues" : [
          "English", "Hindi", "Marathi", "Gujarati", "Kannada", "Tamil", "Malayalam", "Telugu"]
      },
      {
        "filterName" : "Region",
        "filterValues" : [
          "Central_India", "West_India", "North_India", "South_India"]
      },
      // {
      //   "filterName" : "Device Types",
      //   "filterValues" : [
      //     "MAP_100", "MAP_500", "MAP_200"]
      // },
      {
        "filterName" : "Content Types",
        "filterValues" : [
          "Test"]
      },
    ],
    countryCodes: [
      {value: '+91', viewValue: 'India (+91)'}
    ],
    roles : {
      "SuperAdmin" : "SuperAdmin",
      "ContentAdmin": "ContentAdmin",
      "User" : "User",
      "Retailer": "Retailer"
    }
   };
  
  