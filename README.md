# SensorApp
A short code that reads the CPU usage, available memory, and GPU 3D usage in your device.

1.) Go down to the link below and download the newest version of .NET SDK
https://dotnet.microsoft.com/download
2.) After download, open Command Prompt and enter "dotnet --version"
If you see a version number, you’re good.
3.) Copy the code and paste it in your compiler. I used Visual Studio
Please make sure you have the C# Dev Kit extension installed

Creating a new project
1.) Open command prompt and enter:
dotnet new console -n SensorApp
cd SensorApp
code .
2.) Open the VS Code Terminal and run:
dotnet run
3.) To stop the program, press Ctrl + C

ERROR ISSUES
1.) Any errors that occur may be related to .NET templates: PerformanceCounter isn’t included by default (it lives in a separate NuGet package), and it’s Windows-only. If this is the case go into the VS code terminal and enter:
dotnet add package System.Diagnostics.PerformanceCounter
Then run "dotnet run" in the terminal again.
