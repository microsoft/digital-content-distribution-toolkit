
/**
 * Enter here the user flows and custom policies for your B2C application,
 * To learn more about user flows, visit https://docs.microsoft.com/en-us/azure/active-directory-b2c/user-flow-overview
 * To learn more about custom policies, visit https://docs.microsoft.com/en-us/azure/active-directory-b2c/custom-policy-overview
 */
export const b2cPolicies = {
    names: {
        signUpSignIn: "b2c_1a_signuporsigninwithphone",
        forgotPassword: "b2c_1_reset",
        editProfile: "b2c_1_edit_profile"
    },
    authorities: {
        signUpSignIn: {
            authority: "https://mishtudev.b2clogin.com/mishtudev.onmicrosoft.com/b2c_1a_signuporsigninwithphone",
        },
        forgotPassword: {
            authority: "https://mishtudev.b2clogin.com/mishtudev.onmicrosoft.com/b2c_1_reset",
        },
        editProfile: {
            authority: "https://mishtudev.b2clogin.com/mishtudev.onmicrosoft.com/b2c_1_edit_profile"
        }
    },
    authorityDomain: "mishtudev.b2clogin.com"
};

/**
 * Enter here the coordinates of your Web API and scopes for access token request
 * The current application coordinates were pre-registered in a B2C tenant.
 */
export const apiConfig: {scopes: string[]; uri: string} = {
    scopes: ['https://mishtudev.onmicrosoft.com/68fb9dd5-8d1b-4d00-9eaf-d781db510c7f/user.impersonation'],
    uri: 'https://mishtudev.onmicrosoft.com/68fb9dd5-8d1b-4d00-9eaf-d781db510c7f'
};

export const roles = {
    "SuperAdmin" : "SuperAdmin",
    "ContentAdmin": "ContentAdmin",
    "User" : "User"
}
