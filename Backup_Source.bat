IF "%1"=="" (
   ECHO ERROR: This batch file should not be called directly by user - exiting...
   PAUSE
   GOTO exit_this_script
)

CALL %util_dir%\z_generate_timestamp.bat dummyParam
SET timestamp=%ret_val%

PUSHD %workspaces_root_dir%\%workspace_name%

rem ### "%rar_exe%" a -m5 -mdg %rar_exclude_not_source% -s -r %original_batch_dir%\BACKUPs\%timestamp%__%workspace_name%_RAR %workspace_source_subdirs%
rem ### IF %ERRORLEVEL% NEQ 0 GOTO exit_this_script

SET result_archive=%original_batch_dir%\BACKUPs\%timestamp%_-_xyz.7z

"%SevenZip_exe%" a -r "%result_archive%" %workspace_source_subdirs% %SevenZip_exclude_not_source%
IF %ERRORLEVEL% NEQ 0 GOTO exit_this_script

COPY "%result_archive%" "%backup_secondary_location%"
IF %ERRORLEVEL% NEQ 0 GOTO exit_this_script

POPD


rem ============================================================================
GOTO exit_this_script

rem ============================================================================
:exit_this_script

