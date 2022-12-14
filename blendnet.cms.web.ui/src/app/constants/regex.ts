// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

export let regex = {
    alphaNumeric: /^[a-zA-Z0-9]*$/,
    alphaNumericLimitedSplChar: /^[a-zA-Z0-9_@./#&+,()=:!\"-]+( [a-zA-Z0-9_@./#&+,()=:!\"-]+)*$/,
    numeric: /^-?(0|[1-9]\d*)?$/,
    url: /^[a-zA-Z0-9_@./#&+,()-=:?]*$/,
    float: /^(?:\d*\.\d{1,2}|\d+)$/,
    alpha:  /^[a-zA-Z]*$/,
}