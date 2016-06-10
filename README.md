# unipay-sdk-nuget
An example of how to deploy a COM library with C# wrapper.
## unipay_sdk.nuspec
Manifest file containing required information to build the package
## Install.ps1
PowerShell script to force UniPay_SDK.dll to be copied to the output directory each time the project is built.
## To create the package, simple run ```nuget pack unipay_sdk.nuspec```