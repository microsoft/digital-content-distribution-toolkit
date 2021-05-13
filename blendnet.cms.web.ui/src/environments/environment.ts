export const environment = {
  production: false,
  allowedMaxSelection: 3,
  maxFileUploadSize: 1000000,
  fileAllowedType: 'application/json',
  // contentProviderApiUrl: '/ContentProvider',
  contentProviderApiUrl: "http://13.71.0.226/cmsapi/api/v1/ContentProvider",
  // contentApiUrl: '/Content',
  contentApiUrl: "http://13.71.0.226/cmsapi/api/v1/Content",
  // userApiUrl: '/Identity',
  userApiUrl: "http://13.71.0.226/user/api/v1/Identity",
  dashUrlPrefix: "https://ampdemo.azureedge.net/?url=",
  // kaizalaApi0: "/Kaizala0",
  // kaizalaApi1: "/Kaizala1",
  // kaizalaApi2: "/Kaizala2",
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
        "English", "Hindi", "Marathi", "Gujarati", "Kannada", "Tamil", "Malayalam", "Telugu","Klingon"]
    },
    {
      "filterName" : "Region",
      "filterValues" : [
        "Central India", "West India", "North India", "South India"]
    },
    // {
    //   "filterName" : "Device Types",
    //   "filterValues" : [
    //     "MAP 100", "MAP 500", "MAP 200"]
    // },
    {
      "filterName" : "Content Types",
      "filterValues" : [
        "Test"]
    },
  ],
  countryCodes: [
    {value: '+91', viewValue: 'India (+91)'}
  ]
 };

//  { 0, 0 }, { 1, 1 }, { 2, 2 }, { 3, 1 }, { 4, 0 }, { 5, 1 }, { 6, 0 }, { 7, 2 }, { 8, 2 }, { 9, 2 }