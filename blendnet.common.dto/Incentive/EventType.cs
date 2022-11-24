// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace blendnet.common.dto.Incentive
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum EventType
    {
        CONSUMER_INCOME_FIRST_SIGNIN,
        CONSUMER_INCOME_APP_ONCE_OPEN,
        CONSUMER_INCOME_ORDER_COMPLETED,
        CONSUMER_INCOME_STREAMED_CONTENT_ONCE_PER_CONTENTPROVIDER,
        CONSUMER_INCOME_ONBOARDING_RATING_SUBMITTED,
        CONSUMER_INCOME_DOWNLOAD_MEDIA_COMPLETED,
        CONSUMER_EXPENSE_SUBSCRIPTION_REDEEM,
        RETAILER_INCOME_ORDER_COMPLETED,
        RETAILER_INCOME_REFERRAL_COMPLETED,
        RETAILER_INCOME_DOWNLOAD_MEDIA_COMPLETED,
    }
}
