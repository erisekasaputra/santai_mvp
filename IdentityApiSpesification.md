# Identity.API - Version 1.0

# SEND OTP
### `POST /api/v1/Auth/otp`
- **Tags:** `Auth`
- **Request Body:**
    - **Content:**
        - `application/json`: `SendOtpRequest`
        - `text/json`: `SendOtpRequest`
        - `application/*+json`: `SendOtpRequest`
- **Responses:** 
    ### 1. Bad Request
    - **Condition:** When the OTP provider type is not recognized.
    - **Response:** 
        - **Status Code:** `400 BadRequest`
        - **Message:** `"Unknown otp provider type '{request.OtpProviderType}'"`
        - **Status:** `BadRequest`

    ### 2. Unauthorized (Request OTP not found or invalid)
    - **Condition:** When the OTP request is not found or the OTP request token is invalid.
    - **Response:**
        - **Status Code:** `401 Unauthorized`

    ### 3. Unauthorized (User not found)
    - **Condition:** When the user associated with the phone number or email is not found.
    - **Response:**
        - **Status Code:** `401 Unauthorized`

    ### 4. Internal Server Error (Missing Phone Number)
    - **Condition:** When the user’s phone number is missing or invalid.
    - **Response:**
        - **Status Code:** `500 InternalServerError`
        - **Message:** `"Phone number is missing or invalid for user ID: {Id}"`

    ### 5. Internal Server Error (Missing User Claims)
    - **Condition:** When the user's claims are missing or invalid.
    - **Response:**
        - **Status Code:** `500 InternalServerError`
        - **Message:** `"User claims are missing or invalid for user ID: {Id}"`

    ### 6. Internal Server Error (Missing User Roles)
    - **Condition:** When the user's roles are missing or invalid.
    - **Response:**
        - **Status Code:** `500 InternalServerError`
        - **Message:** `"User roles are missing or invalid for user ID: {Id}"`

    ### 7. No Content
    - **Condition:** When the user’s phone number is already confirmed.
    - **Response:**
        - **Status Code:** `204 NoContent`

    ### 8. Accepted (Phone Number Verification)
    - **Condition:** When the phone number verification OTP is generated and sent successfully.
    - **Response:**
        - **Status Code:** `202 Accepted`
        - **Body:**
            ```json
            {
                "User": {
                    "Sub": "{user.Id}",
                    "Username": "{user.UserName}",
                    "PhoneNumber": "{user.PhoneNumber}",
                    "Email": "{user.Email}",
                    "UserType": "{user.UserType}",
                    "BusinessCode": "{user.BusinessCode}"
                },
                "Next": {
                    "Link": "{Url.Action(_verifyPhoneActionName, _controllerName)}",
                    "Action": "{_verifyPhoneActionName}",
                    "Method": "{_actionMethodService.GetHttpMethodByActionName(_verifyPhoneActionName, _controllerName)}"
                }
            }
            ```

    ### 9. Accepted (Forgot Password)
    - **Condition:** When the OTP for forgot password is generated and sent successfully.
    - **Response:**
        - **Status Code:** `202 Accepted`
        - **Body:**
            ```json
            {
                "User": {
                    "Sub": "{user.Id}",
                    "Username": "{user.UserName}",
                    "PhoneNumber": "{user.PhoneNumber}",
                    "Email": "{user.Email}",
                    "UserType": "{user.UserType}",
                    "BusinessCode": "{user.BusinessCode}"
                },
                "Next": {
                    "Link": "{Url.Action(_resetPasswordActionName, _controllerName)}",
                    "Action": "{_resetPasswordActionName}",
                    "Method": "{_actionMethodService.GetHttpMethodByActionName(_resetPasswordActionName, _controllerName)}"
                },
                "RemainingTime": "{remainingTime}"
            }
            ```

    ### 10. OK (Login Verification)
    - **Condition:** When the OTP for login verification is generated and sent successfully.
    - **Response:**
        - **Status Code:** `200 OK`
        - **Body:**
            ```json
            {
                "User": {
                    "Sub": "{user.Id}",
                    "Username": "{user.UserName}",
                    "PhoneNumber": "{user.PhoneNumber}",
                    "Email": "{user.Email}",
                    "UserType": "{user.UserType}",
                    "BusinessCode": "{user.BusinessCode}"
                },
                "Next": {
                    "Link": "{Url.Action(_verifyLoginActionName, _controllerName)}",
                    "Action": "{_verifyLoginActionName}",
                    "Method": "{_actionMethodService.GetHttpMethodByActionName(_verifyLoginActionName, _controllerName)}"
                },
                "RemainingTime": "{remainingTime}"
            }
            ```

    ### 11. Internal Server Error (OTP Request Not Found)
    - **Condition:** When the OTP request type is not recognized.
    - **Response:**
        - **Status Code:** `500 InternalServerError`
        - **Message:** `"Otp request for {otpRequestFor} not found"`

    ### 12. Internal Server Error (Database Exception)
    - **Condition:** When a database update error occurs.
    - **Response:**
        - **Status Code:** `500 InternalServerError`
        - **Message:** `"{errors}"`

    ### 13. Internal Server Error (General Exception)
    - **Condition:** When any other exception occurs.
    - **Response:**
        - **Status Code:** `500 InternalServerError`
        - **Message:** `"InternalServerError"`

  



# RESET PASSWORD
### `POST /api/v1/Auth/reset-password`
- **Tags:** `Auth`
- **Request Body:**
    - **Content:**
        - `application/json`: `PasswordResetRequest`
        - `text/json`: `PasswordResetRequest`
        - `application/*+json`: `PasswordResetRequest`
- **Responses:**
    ### 1. Bad Request (Validation Errors)
    - **Condition:** When the request fails validation.
    - **Response:**
        - **Status Code:** `400 BadRequest`
        - **Message:** Validation errors list
        - **Errors:**
            ```json
            [
                {
                    "PropertyName": "Property",
                    "ErrorMessage": "Error description"
                }
            ]
            ```

    ### 2. Not Found (User Not Found)
    - **Condition:** When the user with the given identity (phone number or email) is not found.
    - **Response:**
        - **Status Code:** `404 NotFound`
        - **Message:**
            ```json
            {
                "Status": "NotFound",
                "Message": "User not found"
            }
            ```

    ### 3. Internal Server Error (Missing Phone Number)
    - **Condition:** When the user’s phone number is missing or invalid.
    - **Response:**
        - **Status Code:** `500 InternalServerError`
        - **Message:** `"Phone number is missing or invalid for user ID: {Id}"`

    ### 4. Bad Request (Invalid OTP)
    - **Condition:** When the provided OTP or phone number is invalid.
    - **Response:**
        - **Status Code:** `400 BadRequest`
        - **Message:**
            ```json
            {
                "Status": "BadRequest",
                "Message": "An error has occurred",
                "Errors": [
                    {
                        "Field": "Credential",
                        "Message": "OTP or phone number is invalid"
                    }
                ]
            }
            ```

    ### 5. Bad Request (Password Validation Error)
    - **Condition:** When the new password fails validation.
    - **Response:**
        - **Status Code:** `400 BadRequest`
        - **Message:**
            ```json
            {
                "Status": "BadRequest",
                "Message": "Password validation error",
                "Errors": [
                    {
                        "Code": "Error Code",
                        "Description": "Error description"
                    }
                ]
            }
            ```

    ### 6. Ok (Password Reset Success)
    - **Condition:** When the password is successfully reset.
    - **Response:**
        - **Status Code:** `200 Ok`

    ### 7. Bad Request (Update Failed)
    - **Condition:** When updating the user fails after setting the new password.
    - **Response:**
        - **Status Code:** `400 BadRequest`
        - **Message:**
            ```json
            {
                "Status": "BadRequest",
                "Message": "Validation failed",
                "Errors": [
                    {
                        "Code": "Error Code",
                        "Description": "Description: Error description"
                    }
                ]
            }
            ```

    ### 8. Internal Server Error (General Exception)
    - **Condition:** When any other exception occurs.
    - **Response:**
        - **Status Code:** `500 InternalServerError`
        - **Message:** `"InternalServerError"`
        - **Details:** Log error information

# SIGNIN GOOGLE
### `POST /api/v1/Auth/signin-google`
- **Tags:** `Auth`
- **Request Body:**
    - **Content:**
        - `application/json`: `GoogleSignInRequest`
        - `text/json`: `GoogleSignInRequest`
        - `application/*+json`: `GoogleSignInRequest`
- **Responses:**
    ### 1. Bad Request (Invalid Google ID Token)
    - **Condition:** When the provided Google ID token is invalid.
    - **Response:**
        - **Status Code:** `400 BadRequest`
        - **Message:**
            ```json
            {
                "Status": "BadRequest",
                "Message": "Google ID token is invalid"
            }
            ```

    ### 2. Not Found (User Not Found)
    - **Condition:** When the user with the provided Google ID token does not exist in the system.
    - **Response:**
        - **Status Code:** `404 NotFound`
        - **Body:**
            ```json
            {
                "GoogleIdToken": "{GoogleIdToken}",
                "Next": {
                    "Link": "{Url.Action(_createIdentityActionName, _controllerName)}",
                    "Action": "{_createIdentityActionName}",
                    "Method": "{_actionMethodService.GetHttpMethodByActionName(_createIdentityActionName, _controllerName)}",
                    "AllowedUserTypes": "{GetUserTypeConfiguration.AllowedUserRegisterBySelf}"
                }
            }
            ```

    ### 3. Internal Server Error (Missing or Invalid Claims)
    - **Condition:** When the user's claims are missing or invalid.
    - **Response:**
        - **Status Code:** `500 InternalServerError`
        - **Message:**
            ```json
            {
                "Status": "InternalServerError",
                "Message": "User claims are missing or invalid for user ID: {Id}"
            }
            ```

    ### 4. Internal Server Error (Missing or Invalid Roles)
    - **Condition:** When the user's roles are missing or invalid.
    - **Response:**
        - **Status Code:** `500 InternalServerError`
        - **Message:**
            ```json
            {
                "Status": "InternalServerError",
                "Message": "User roles are missing or invalid for user ID: {Id}"
            }
            ```

    ### 5. Accepted (Phone Number Verification Required)
    - **Condition:** When the user's phone number is not confirmed and OTP needs to be sent for verification.
    - **Response:**
        - **Status Code:** `202 Accepted`
        - **Body:**
            ```json
            {
                "User": {
                    "Sub": "{user.Id}",
                    "Username": "{user.UserName}",
                    "PhoneNumber": "{user.PhoneNumber}",
                    "Email": "{user.Email}",
                    "UserType": "{user.UserType}",
                    "BusinessCode": "{user.BusinessCode}"
                },
                "Next": {
                    "Link": "{Url.Action(_sendOtpActionName, _controllerName)}",
                    "Action": "{_sendOtpActionName}",
                    "Method": "{_actionMethodService.GetHttpMethodByActionName(_sendOtpActionName, _controllerName)}",
                    "OtpRequestToken": "{otpRequestToken}",
                    "OtpRequestId": "{requestId}",
                    "OtpProviderTypes": "{AllowedOtpProviderType.GetAll}"
                }
            }
            ```

    ### 6. Accepted (Login Verification Required)
    - **Condition:** When the user needs to verify the login via OTP.
    - **Response:**
        - **Status Code:** `202 Accepted`
        - **Body:**
            ```json
            {
                "User": {
                    "Sub": "{user.Id}",
                    "Username": "{user.UserName}",
                    "PhoneNumber": "{user.PhoneNumber}",
                    "Email": "{user.Email}",
                    "UserType": "{user.UserType}",
                    "BusinessCode": "{user.BusinessCode}"
                },
                "Next": {
                    "Link": "{Url.Action(_sendOtpActionName, _controllerName)}",
                    "Action": "{_sendOtpActionName}",
                    "Method": "{_actionMethodService.GetHttpMethodByActionName(_sendOtpActionName, _controllerName)}",
                    "OtpRequestToken": "{newOtpRequestToken}",
                    "OtpRequestId": "{newRequestId}",
                    "OtpProviderTypes": "{AllowedOtpProviderType.GetAll}"
                }
            }
            ```

    ### 7. Internal Server Error (Database Update Exception)
    - **Condition:** When a database update error occurs.
    - **Response:**
        - **Status Code:** `500 InternalServerError`
        - **Message:** `"Database update exception occurred, error: {errors}"`

    ### 8. Internal Server Error (General Exception)
    - **Condition:** When any other exception occurs.
    - **Response:**
        - **Status Code:** `500 InternalServerError`
        - **Message:** `"InternalServerError"`
        - **Details:** Log error information



# SIGNIN STAFF
### `POST /api/v1/Auth/signin-staff`
- **Tags:** `Auth`
- **Request Body:**
    - **Content:**
        - `application/json`: `LoginStaffRequest`
        - `text/json`: `LoginStaffRequest`
        - `application/*+json`: `LoginStaffRequest`
- **Responses:**
    - `200`: OK

# SIGNIN USER
### `POST /api/v1/Auth/signin-user`
- **Tags:** `Auth`
- **Request Body:**
    - **Content:**
        - `application/json`: `LoginUserRequest`
        - `text/json`: `LoginUserRequest`
        - `application/*+json`: `LoginUserRequest`
- **Responses:**
    - `200`: OK

    
REGISTER NEW USER
### `POST /api/v1/Auth/register`
- **Tags:** `Auth`
- **Request Body:**
    - **Content:**
        - `application/json`: `RegisterUserRequest`
        - `text/json`: `RegisterUserRequest`
        - `application/*+json`: `RegisterUserRequest`
- **Responses:**
    - `200`: OK


# REFRESH TOKEN
### `POST /api/v1/Auth/refresh-token`
- **Tags:** `Auth`
- **Request Body:**
    - **Content:**
        - `application/json`: `RefreshTokenRequest`
        - `text/json`: `RefreshTokenRequest`
        - `application/*+json`: `RefreshTokenRequest`
- **Responses:**
    - `200`: OK


# FORGOT PASSWORD
### `POST /api/v1/Auth/forgot-password`
- **Tags:** `Auth`
- **Request Body:**
    - **Content:**
        - `application/json`: `PasswordForgotRequest`
        - `text/json`: `PasswordForgotRequest`
        - `application/*+json`: `PasswordForgotRequest`
- **Responses:**
    - `200`: OK

# VERIFY TOKEN
### `PATCH /api/v1/Auth/verify-token`
- **Tags:** `Auth`
- **Request Body:**
    - **Content:**
        - `application/json`: `RefreshTokenRequest`
        - `text/json`: `RefreshTokenRequest`
        - `application/*+json`: `RefreshTokenRequest`
- **Responses:**
    - `200`: OK

# VERIFY LOGIN
### `PATCH /api/v1/Auth/verify-login`
- **Tags:** `Auth`
- **Request Body:**
    - **Content:**
        - `application/json`: `VerifyLoginRequest`
        - `text/json`: `VerifyLoginRequest`
        - `application/*+json`: `VerifyLoginRequest`
- **Responses:**
    - `200`: OK
# VERIFY PHONE NUMBER
### `PATCH /api/v1/Auth/verify-phone-number`
- **Tags:** `Auth`
- **Request Body:**
    - **Content:**
        - `application/json`: `VerifyPhoneNumberRequest`
        - `text/json`: `VerifyPhoneNumberRequest`
        - `application/*+json`: `VerifyPhoneNumberRequest`
- **Responses:**
    - `200`: OK

# SIGN OUT
### `POST /api/v1/Auth/signout`
- **Tags:** `Auth`
- **Request Body:**
    - **Content:**
        - `application/json`: `RefreshTokenRequest`
        - `text/json`: `RefreshTokenRequest`
        - `application/*+json`: `RefreshTokenRequest`
- **Responses:**
    - `200`: OK

## Components

### Schemas

#### `GoogleSignInRequest`
- **Required:**
    - `googleIdToken`
- **Type:** `object`
- **Properties:**
    - `googleIdToken`: `string` (nullable)
- **Additional Properties:** `false`

#### `LoginStaffRequest`
- **Required:**
    - `businessCode`
    - `password`
    - `phoneNumber`
    - `regionCode`
- **Type:** `object`
- **Properties:**
    - `businessCode`: `string` (nullable)
    - `phoneNumber`: `string` (nullable)
    - `password`: `string` (nullable)
    - `regionCode`: `string` (nullable)
    - `returnUrl`: `string` (nullable)
- **Additional Properties:** `false`

#### `LoginUserRequest`
- **Required:**
    - `password`
    - `phoneNumber`
    - `regionCode`
- **Type:** `object`
- **Properties:**
    - `phoneNumber`: `string` (nullable)
    - `password`: `string` (nullable)
    - `regionCode`: `string` (nullable)
    - `returnUrl`: `string` (nullable)
- **Additional Properties:** `false`

#### `OtpProviderType`
- **Enum Values:**
    - `Sms`
    - `Email`
- **Type:** `string`

#### `PasswordForgotRequest`
- **Required:**
    - `identity`
- **Type:** `object`
- **Properties:**
    - `identity`: `string` (nullable)
- **Additional Properties:** `false`

#### `PasswordResetRequest`
- **Required:**
    - `identity`
    - `newPassword`
    - `otpCode`
- **Type:** `object`
- **Properties:**
    - `identity`: `string` (nullable)
    - `otpCode`: `string` (nullable)
    - `newPassword`: `string` (nullable)
- **Additional Properties:** `false`

#### `RefreshTokenRequest`
- **Required:**
    - `accessToken`
    - `refreshToken`
- **Type:** `object`
- **Properties:**
    - `accessToken`: `string` (nullable)
    - `refreshToken`: `string` (nullable)
    - `deviceId`: `string` (nullable)
- **Additional Properties:** `false`

#### `RegisterUserRequest`
- **Required:**
    - `password`
    - `phoneNumber`
    - `regionCode`
    - `userType`
- **Type:** `object`
- **Properties:**
    - `phoneNumber`: `string` (nullable)
    - `password`: `string` (nullable)
    - `regionCode`: `string` (nullable)
    - `userType`: `UserType`
    - `googleIdToken`: `string` (nullable)
    - `returnUrl`: `string` (nullable)
- **Additional Properties:** `false`

#### `SendOtpRequest`
- **Required:**
    - `otpProviderType`
    - `otpRequestId`
    - `otpRequestToken`
- **Type:** `object`
- **Properties:**
    - `otpRequestId`: `string` (`uuid`)
    - `otpRequestToken`: `string` (nullable)
    - `otpProviderType`: `OtpProviderType`
- **Additional Properties:** `false`

#### `UserType`
- **Enum Values:**
    - `Administrator`
    - `RegularUser`
    - `BusinessUser`
    - `MechanicUser`
    - `StaffUser`
    - `ServiceToService`
- **Type:** `string`

#### `VerifyLoginRequest`
- **Required:**
    - `deviceId`
    - `phoneNumber`
    - `token`
- **Type:** `object`
- **Properties:**
    - `token`: `string` (nullable)
    - `phoneNumber`: `string` (nullable)
    - `deviceId`: `string` (nullable)
- **Additional Properties:** `false`

#### `VerifyPhoneNumberRequest`
- **Required:**
    - `phoneNumber`
    - `token`
- **Type:** `object`
- **Properties:**
    - `phoneNumber`: `string` (nullable)
    - `token`: `string` (nullable)
- **Additional Properties:** `false`
