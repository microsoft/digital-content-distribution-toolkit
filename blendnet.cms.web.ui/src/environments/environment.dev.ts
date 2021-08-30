export const environment = {
    production: false,
    allowedMaxSelection: 3,
    maxFileUploadSize: 1000000,
    fileAllowedType: 'application/json',
    channelName: "CMSPortal",
    contentProviderApiUrl: "https://blendnet-dev.kaiza.la/cmsapi/api/v1/ContentProvider",
    browrseContent: "https://blendnet-dev.kaiza.la/cmsapi/api/v1/BrowseContent",
    contentApiUrl: "https://blendnet-dev.kaiza.la/cmsapi/api/v1/Content",
    userApiUrl: "https://blendnet-dev.kaiza.la/userapi/api/v1/User",
    createUserApiUrl: "https://blendnet-dev.kaiza.la/userapi/api/v1/UserBasic",
    incentiveApiUrl: "https://blendnet-dev.kaiza.la/incentiveapi/api/v1/Incentive",
    retailerApiUrl: "https://blendnet-dev.kaiza.la/retailerapi/api/v1",
    retailerDashboardUrl: "https://blendnet-dev.kaiza.la/incentiveapi/api/v1/",
    notificationApiUrl: "https://blendnet-dev.kaiza.la/notificationapi/api/v1/Notification",
    dashUrlPrefix: "https://ampdemo.azureedge.net/?url=",
    kaizalaApi0: "https://api-alpha.kaiza.la/v1/",
    kaizalaApi1: "https://api-alpha1.kaiza.la/v1/",
    kaizalaApi2: "https://api-alpha2.kaiza.la/v1/",
    kaizalaSignUpSignIn: "LoginWithPhoneForPartners",
    kaizalaVerifyOTP: "VerifyPhonePinForPartnerLogin",
    kaizalaGetUserRoles : "ValidatePartnerAccessToken",
    kaizalaAppNameParam : "applicationName",
    kaizalaAppName : "com.microsoft.mobile.polymer.mishtu",
    baseHref:'portal',
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
  
  //  { 0, 0 }, { 1, 1 }, { 2, 2 }, { 3, 1 }, { 4, 0 }, { 5, 1 }, { 6, 0 }, { 7, 2 }, { 8, 2 }, { 9, 2 }