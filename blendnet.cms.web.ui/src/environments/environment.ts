export const environment = {
  production: false,
  allowedMaxSelection: 3,
  maxFileUploadSize: 1000000,
  fileAllowedType: 'application/json',
  // contentProviderApiUrl: 'https://localhost:44338/api/v1/ContentProvider',
  // contentApiUrl: 'https://localhost:44338/api/v1/Content',
  // userApiUrl: 'https://localhost:44397/api/v1/Identity',
  contentProviderApiUrl: 'http://13.71.0.226/cmsapi/api/v1/ContentProvider',
  contentApiUrl: 'http://13.71.0.226/cmsapi/api/v1/Content',
  userApiUrl: 'http://13.71.0.226/user/api/v1/Identity',
  dashUrlPrefix: "https://ampdemo.azureedge.net/?url=",
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
  ]
 };