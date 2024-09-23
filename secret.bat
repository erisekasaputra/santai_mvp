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

REM Set the aws client
dotnet user-secrets set "AWS:AccessID" "AKIAYS2NP3RU6DEGPJUM"  
dotnet user-secrets set "AWS:SecretKey" "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"  
dotnet user-secrets set "AWS:Region" "ap-southeast-1"  

dotnet user-secrets set "AWS:RDS:Username" "santaitechnology"  
dotnet user-secrets set "AWS:RDS:Password" "_SANTAItechnology2024" 
dotnet user-secrets set "AWS:RabbitMQ:Broker" "ProductionBackendBroker" 
dotnet user-secrets set "AWS:RabbitMQ:Username" "santaitechnology" 
dotnet user-secrets set "AWS:RabbitMQ:Password" "_SANTAItechnology2024" 
dotnet user-secrets set "AWS:Sns:Topic" "NotificationTopic" 
 
echo User secrets initialized and secret key set.
pause