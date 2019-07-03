IF "%1"=="" (
   ECHO ERROR: This batch file should not be called directly by user - exiting...
   PAUSE
   GOTO exit_this_script
)


SET pause_on_exit=false

SET "junction_util_exe=C:\UTIL\Junction\junction.exe"

SET "workspaces_root_dir=C:\Git\Elastic\Agent.NET"

SET "rar_exe=%ProgramFiles%\WinRAR\RAR.exe"
rem --- SET rar_exclude_not_source=-xout -x*.class
SET rar_exclude_not_source=-x*\out -x*\build -x*\Installer -xlogs -x*\.gradle -x*\.idea -x*.bin -x*.lock -x*.class

SET "SevenZip_exe=%ProgramFiles%\7-Zip\7z.exe"
rem ### SET SevenZip_exclude_not_source=-xr!Worker\src\MSXMLLite -xr!Worker\src\Doc -x!Worker\src\BMCTEAAgentProject\BMCTEAAgent.vsd -xr!out -xr!build -xr!_BUILD_OUT -xr!Debug -xr!Release -xr!BinDebug -xr!BinRelease -xr!DebugUT -xr!ReleaseUT -xr!.vs -xr!3rd_party -xr!Installer -x!LOGs -x!WORK -x!_GENERATED_src -x!.gradle -x!.idea -x!*.bin -x!*.lib -x!*.exe -x!*.dll -x!*.obj -x!*.lock -x!*.class -x!*.mmdb -x!*.dat -x!*.jpg -x!*.gif -x!*.jar -x!*.png -x!*.ltz -x!*.tlh -x!*.tli
SET SevenZip_exclude_not_source=-xr!.git -xr!.github -xr!.vs -xr!.idea -xr!packages -xr!bin -xr!obj -xr!lib -x!*.ico -x!*.svg -x!*.png -x!*.log -x!*.db -xr!BenchmarkDotNet.Artifacts -xr!sample\AspNetFullFrameworkSampleApp\Scripts -xr!sample\AspNetFullFrameworkSampleApp\Content

SET backup_secondary_location=D:\Elastic_Dev_BACKUP\Agent.NET_-_my_fork

SET "BareTailPro_exe=C:\UTIL\BareTailPro\baretailpro.exe"

SET workspace_source_subdirs=.

SET tests_Docker_container=tests_-_%workspace_name%

rem ###
rem ### Sets the verbosity level of the command. Allowed values are:
rem ###         quiet
rem ###         minimal
rem ###         normal
rem ###         detailed
rem ###         diagnostic
rem ###

IF "%tests_Docker_dotnet_test_verbosity%"=="" (
    SET tests_Docker_dotnet_test_verbosity=normal
)

rem ### SET number_of_seconds_to_wait_between_test_iterations=10

