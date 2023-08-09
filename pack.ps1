dotnet build -c Release 
dotnet pack .\EntityDynamicAttributes\ -c Release -o ..\_publish
dotnet pack .\EntityDynamicAttributes.WebApi\ -c Release -o ..\_publish
