Every application should run 
	dotnet user-secrets init
	dotnet user-secrets set "Google:ClientSecret" "Client secret from google"
	dotnet user-secrets set "KMS:SecretKey" "Secret key for key management service for data encryption and jwt generator"