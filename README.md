# Prerequisities
- installed latest version of SCIA Engineer (19.1 patch 2)
- installed MS Visual Studio https://visualstudio.microsoft.com/cs/vs/ Community edition is sufficient. You can also use Visual Studio Code https://code.visualstudio.com/

# To start new VB.NET development project in MS Visual Studio to produce app that will use the Scia OpenAPI:
- create empty VB.NET console app project using .NET 4.6.1
- Add reference to SCIA.OpenAPI.dll located in Scia Engineer install folder, edit properties of reference and set Copy Local = False
- Create new / use configuration for x86 / x64 as needed according to SCIA Engineer Architecture

- Write your application that use the SCIA.OpenAPI functions
- Don't forget to use "using" statement for environment object creation OR call the Environment's Dispose() method when you finish your work with SCIA OpenAPI
- write method for resolving of assemblies - see sample 
```vb.net
Private Function ResolveAssemblies(sender As Object, e As System.ResolveEventArgs) As Reflection.Assembly
        ' Function which is needed for coorect load of SEn assemblies, see the line "AddHandler AppDomain.CurrentDomain.AssemblyResolve, AddressOf ResolveAssemblies"
        Dim dllName = e.Name.Substring(0, e.Name.IndexOf(",")) + ".dll"
        Dim dllFullPath = Path.Combine(GetSEnPath(), dllName)
        If Not System.IO.File.Exists(dllFullPath) Then
            dllFullPath = Path.Combine(GetSEnPath(), "OpenAPI_dll", dllName)
            If Not System.IO.File.Exists(dllFullPath) Then
                Return Nothing
            End If
        End If
        Return Reflection.Assembly.LoadFrom(dllFullPath)
End Function
```
- write method for deliting temp folder
```vb.net
If System.IO.Directory.Exists(GetSEnTempPath()) Then
	System.IO.Directory.Delete(GetSEnTempPath(), True)
End If
```
- Write your application that use the SCIA.OpenAPI functions
- Methods using OpenAPI have to run in single thread appartment - use STAThread

# To use this example application:
- clone this GIT repo
  - start command line
  - go to desired directory
  - write command "git clone <url_to_this_exmaple_app_repo_that_can_be_found_on_github_project_page>"
- open project in MS Visual Studio
- check correctness of path to referenced DLL located in your Scia Engineer install folder: SCIA.OpenAPI.dll (e.g. edit the .vbproj)
- check correctness of paths in code poinitng to Scia Engineer install directory and template file
- select appropriate configuration (x86/x64) as needed according to SCIA Engineer Architecture
- build
- copy following DLLs (from Scia Engineer install directory) to your application output directory: SCIA.OpenAPI.dll, ESAAtl80Extern.dll
- run your application

# Troubleshooting:
* if you get following exception, just register esa libraries
	* run cmd AS ADMINISTRATOR
	* navigate to Scia Engineer directory
	* run "EP_regsvr32 esa.exe"
```
Unhandled Exception: System.Reflection.TargetInvocationException: Exception has been thrown by the target of an invocation. ---> System.AccessViolationException: Attempted to read or write protected memory. This is often an indication that other memory is corrupt.
```
