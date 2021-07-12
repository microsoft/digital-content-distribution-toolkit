export const environment = {
  production: true,
  allowedMaxSelection: 5,
  maxFileUploadSize: 1000000,
  fileAllowedType: 'application/json',
  channelName: "CMSPortal",
  contentProviderApiUrl: "/cmsapi/api/v1/ContentProvider",
  contentApiUrl: "/cmsapi/api/v1/Content",
  userApiUrl: "/userapi/api/v1/User",
  incentiveApiUrl: "/incentiveapi/api/v1/Incentive",
  retailerApiUrl: "/retailerapi/api/v1",
  dashUrlPrefix: "https://ampdemo.azureedge.net/?url=",
  kaizalaApi0: "https://api-alpha.kaiza.la/v1/",
  kaizalaApi1: "https://api-alpha1.kaiza.la/v1/",
  kaizalaApi2: "https://api-alpha2.kaiza.la/v1/",
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
    // {
    //   "filterName" : "Content Types",
    //   "filterValues" : [
    //     "Test"]
    // },
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

