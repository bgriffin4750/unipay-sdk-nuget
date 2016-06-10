# unipay-sdk-nuget
An example of how to deploy a COM library with C# wrapper as a nuget package. By following the steps below, you will deploy a 
C# wrapper with the associated COM library to a sample project.
## To use this project
1. Open UniPaySdkNuget.sln and build the solution 
2. From a command prompt, navigate to root directory of this solution
3. Run ```nuget pack unipay_sdk.nuspec```
4. Copy UniPay.1.0.0.nupkg to your nuget package directory. Note the name 'UniPay.1.0.0.nupkg' will 
vary depending on what you have in the id and version tags in your nuspec file.
5. Right click the CardReaderSample project in VisualStudio and selec 'Manage NuGet Packages'
6. Select the package from your local directory and install it.
7. Build CardReaderSample.
At this point, you will have added your wrapper along with the COM library to CardReaderSample. By building CardReaderSample,
you will find both libraries: UniPaySdkWrapper.dll and UniPay_SDK.dll in the output directory. If you want to see this connect to your idtech card reader, uncomment the code found in CardReaderSample\Program.cs
