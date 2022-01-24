export let regex = {
    alphaNumeric: /^[a-zA-Z0-9]*$/,
    alphaNumericLimitedSplChar: /^[a-zA-Z0-9_@./#&+,()=:!\"-]+( [a-zA-Z0-9_@./#&+,()=:!\"-]+)*$/,
    numeric: /^-?(0|[1-9]\d*)?$/,
    url: /^[a-zA-Z0-9_@./#&+,()-=:?]*$/
}