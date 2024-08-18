@echo off
REM Initialize user secrets
dotnet user-secrets init

REM Set the secret key
dotnet user-secrets set "KMS:SecretKey" "tP56kzHlfp+IUY/nhdOFbOsRjOSEbTVv6sexU/JL17I="

REM Set the secret key jwt
dotnet user-secrets set "Jwt:SecretKey" "tP56kzHlfp+IUY/nhdOFbOsRjOSEbTVv6sexU/JL17I="


REM Set the google client secret 
dotnet user-secrets set "Google:ClientSecret" "get-from-google-sso-generator"

echo User secrets initialized and secret key set.
pause