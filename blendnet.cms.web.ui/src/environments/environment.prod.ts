export const environment = {
  production: true,
  allowedMaxSelection: 3,
  maxFileUploadSize: 1000000,
  fileAllowedType: 'application/json',
  contentProviderApiUrl: '/ContentProvider',
  contentApiUrl: '/Content',
  userApiUrl: '/Identity',
  dashUrlPrefix: "https://ampdemo.azureedge.net/?url=",
  kaizalaApi0: "/Kaizala0",
  kaizalaApi1: "/Kaizala2",
  kaizalaApi2: "/Kaizala2",
  appName : "com.microsoft.mobile.mishtu",
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