export const environment = {
  production: false,
  allowedMaxSelection: 3,
  maxFileUploadSize: 1000000,
  fileAllowedType: 'application/json',
  channelName: "CMSPortal",
  baseUrl: "https://blendnet-dev.kaiza.la",
  contentProviderApiUrl: "/cmsapi/api/v1/ContentProvider",
  browrseContent: "/cmsapi/api/v1/BrowseContent",
  contentApiUrl: "/cmsapi/api/v1/Content",
  userApiUrl: "/userapi/api/v1/User",
  omsApiUrl: "/omsapi/api/v1",
  createUserApiUrl: "/userapi/api/v1/UserBasic",
  incentiveApiUrl: "/incentiveapi/api/v1/Incentive",
  retailerDashboardUrl: "/incentiveapi/api/v1/",
  retailerApiUrl: "/retailerapi/api/v1",
  notificationApiUrl: "/notificationapi/api/v1/Notification",
  deviceUrl: "/deviceapi/api/v1/Device",
  retailerUrl: "/retailerapi/api/v1/Retailer",
  retailerProviderUrl: "/retailerapi/api/v1/RetailerProvider",
  dashUrlPrefix: "https://ampdemo.azureedge.net/?url=",
  kaizalaApi0: "https://api-alpha.kaiza.la/v1/",
  kaizalaApi1: "https://api-alpha1.kaiza.la/v1/",
  kaizalaApi2: "https://api-alpha2.kaiza.la/v1/",
  kaizalaSignUpSignIn: "LoginWithPhoneForPartners",
  kaizalaVerifyOTP: "VerifyPhonePinForPartnerLogin",
  kaizalaGetUserRoles : "ValidatePartnerAccessToken",
  kaizalaAppNameParam : "applicationName",
  kaizalaAppName : "com.microsoft.mobile.polymer.mishtu",
  baseHref:'',
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