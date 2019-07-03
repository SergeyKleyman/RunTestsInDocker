IF "%1"=="" (
   ECHO ERROR: This batch file should not be called directly by user - exiting...
   PAUSE
   GOTO exit_this_script
)

SETLOCAL enabledelayedexpansion

SET last_exit_code=0

SET "link_src=%workspaces_root_dir%\%workspace_name%\Manager\WORK\config"
SET "link_dst=%workspaces_root_dir%\%workspace_name%\Manager\src\main\resources\WORK_default_content\config"

"%junction_util_exe%" -d "%link_src%"
SET last_exit_code=%ERRORLEVEL%
IF NOT "%last_exit_code%" == "0" (
	GOTO :exit_this_script
)

"C:\UTIL\Junction\junction64.exe" "%link_src%" "%link_dst%"
SET last_exit_code=%ERRORLEVEL%
IF NOT "%last_exit_code%" == "0" (
	GOTO :exit_this_script
)

GOTO :exit_this_script
rem #########################
rem ### main - END
rem ############################################################################


rem ############################################################################
rem ### :exit_this_script
rem #########################
:exit_this_script

