export const environment = {
  production: true,
  allowedMaxSelection: 3,
  maxFileUploadSize: 1000000,
  fileAllowedType: 'application/json',
  contentProviderApiUrl: 'https://localhost:44338/api/v1/ContentProvider',
  contentApiUrl: 'https://localhost:44338/api/v1/Content',
  userApiUrl: 'https://localhost:44397/api/v1/Identity',
  dashUrlPrefix: 'https://ampdemo.azureedge.net/?url=',
  widewineTokenPrefix: '&widevine=true&token=Bearer%3D',
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
    {
      "filterName" : "Device Types",
      "filterValues" : [
        "MAP 100", "MAP 500", "MAP 200"]
    },
  ]
};
