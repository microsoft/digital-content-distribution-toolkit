export const environment = {
    production: false,
    allowedMaxSelection: 5,
    maxFileUploadSize: 1000000,
    fileAllowedType: 'application/json',
    channelName: "CMSPortal",
    // contentProviderApiUrl: '/ContentProvider',
    contentProviderApiUrl: "http://52.172.254.78/cmsapi/api/v1/ContentProvider",
    // contentApiUrl: '/Content',
    contentApiUrl: "http://52.172.254.78/cmsapi/api/v1/Content",
    // userApiUrl: '/User',
    userApiUrl: "http://52.172.254.78/userapi/api/v1/User",
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
          "English", "Hindi", "Marathi", "Gujarati", "Kannada", "Tamil", "Malayalam", "Telugu"]
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
    ],
    roles : {
      "SuperAdmin" : "SuperAdmin",
      "ContentAdmin": "ContentAdmin",
      "User" : "User"
    }
   };
  
  