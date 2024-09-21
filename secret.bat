@echo off
REM Initialize user secrets
dotnet user-secrets init

REM Set the secret key
dotnet user-secrets set "KMS:SecretKey" "tP56kzHlfp+IUY/nhdOFbOsRjOSEbTVv6sexU/JL17I="

REM Set the secret key jwt
dotnet user-secrets set "Jwt:SecretKey" "tP56kzHlfp+IUY/nhdOFbOsRjOSEbTVv6sexU/JL17I="

REM Set the secret key senangpay
dotnet user-secrets set "SenangPay:SecretKey" "6936-735"

REM Set the google client secret 
dotnet user-secrets set "Google:ClientSecret" "get-from-google-sso-generator" 

REM Set the aws client secret 
dotnet user-secrets set "Storage:AWS_ACCESS_KEY_ID" "AKIAYS2NP3RU6DEGPJUM" 

REM Set the aws client secret 
dotnet user-secrets set "Storage:AWS_SECRET_ACCESS_KEY" "6mIoP493YVrGwjUOYeZ/8WstUKRU2eUKi1rD9Vbl" 

REM Set the aws client secret 
dotnet user-secrets set "Storage:AWS_DEFAULT_REGION" "ap-southeast-1" 
  

echo User secrets initialized and secret key set.
pause