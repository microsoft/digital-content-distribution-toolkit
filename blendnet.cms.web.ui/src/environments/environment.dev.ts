// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

export const environment = {
  production: false,
  allowedMaxSelection: 10,
  maxFileUploadSize: 1000000,
  fileAllowedType: 'application/json',
  channelName: "CMSPortal",
  baseUrl: "https://blendnet-dev.kaiza.la",
  contentProviderApiUrl: "/cmsapi/api/v1/ContentProvider",
  browrseContent: "/cmsapi/api/v1/BrowseContent",
  contentApiUrl: "/cmsapi/api/v1/Content",
  userApiUrl: "/userapi/api/v1/User",
  userBasicApiUrl: "/userapi/api/v1/UserBasic",
  userOnboardingApiUrl: "/userapi/api/v1/UserOnboarding",
  incentiveApiUrl: "/incentiveapi/api/v1/Incentive",
  incentiveBrowseApiUrl: "/incentiveapi/api/v1/IncentiveBrowse",
  omsApiUrl: "/omsapi/api/v1",
  retailerDashboardUrl: "/incentiveapi/api/v1/",
  retailerApiUrl: "/retailerapi/api/v1",
  notificationApiUrl: "/notificationapi/api/v1/Notification",
  deviceUrl: "/deviceapi/api/v1/Device",
  deviceContentUrl: "/deviceapi/api/v1/DeviceContent",
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
  baseHref:'portal',
  cdnBaseUrl: "https://stcontentcdndev.blob.core.windows.net/",
  cpLogoPictorialImg: "-cdn/logos/pictorialmark_square.png",
  cpLogoWaterMarkImg: "-cdn/logos/water_mark.png",
  defaultCplogoImg: "/assets/images/cp-default-logo/cp-default-logo.png",
  noOrdersImg: "/assets/images/retailer/retailer-empty-order.png",
  whitelistedUserApiUrl: "/userapi/api/v1/WhitelistedUser/create",
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
    {
      "filterName" : "Content Type",
      "filterValues" : [
        "Test", "Development"]
    },
  ],
  countryCodes: [
    {value: '+91', viewValue: 'India (+91)'}
  ],
  roles : {
    "SuperAdmin" : "SuperAdmin",
    "ContentAdmin": "ContentAdmin",
    "User" : "User",
    "Retailer": "Retailer",
    "HubDeviceManagement": "HubDeviceManagement"
  },
  featureName: {
    "Home": "",
    "ContentProviders": "",
    "SASKey": "",
    "Unprocessed": "",
    "Processed": "",
    "Broadcast": "",
    "Subscriptions": "",
    "Incentives": "",
    "Devices": "",
    "Notifications": "",
    "Export": "",
    "Delete": "",
    "RetailerDashboard": "",
    "Dashboard": "webRetailerDashboardEnabled",
    "Order": "webOrderCompletionEnabled"
  },
  genres :  ["Drama", "Family", "Reality", "Crime", "Romance", "Action", "Thriller", "Fantasy", "Mythology"],
  peopleType: ["Director", "Actor","Singer", "MusicDirector", "Other"]
 };