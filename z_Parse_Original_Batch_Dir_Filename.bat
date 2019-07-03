IF "%1"=="" (
   ECHO ERROR: This batch file should not be called directly by user - exiting...
   PAUSE
   GOTO exit_this_script
)


rem workspace_name: main, main_TEMP, Altair
CALL :extract_last_in_path %original_batch_dir%
SET workspace_name=%ret_val%

rem ============================================================================
GOTO exit_this_script


rem ============================================================================
:extract_last_in_path

SET ret_val=%~nx1

GOTO:EOF


rem ============================================================================
:exit_this_script

