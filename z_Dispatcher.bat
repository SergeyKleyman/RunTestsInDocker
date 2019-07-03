IF "%1"=="" (
   ECHO ERROR: This batch file should not be called directly by user - exiting...
   PAUSE
   GOTO exit_this_script
)

SET original_batch_dir=%this_batch_dir%
SET original_batch_file_name=%this_batch_file_name%

CALL %util_dir%\z_parse_original_batch_dir_filename.bat dummyParam
IF %ERRORLEVEL% NEQ 0 GOTO exit_this_script

IF EXIST %original_batch_dir%\z_customizations_pre_default.bat (
   CALL %original_batch_dir%\z_customizations_pre_default.bat dummyParam
   IF %ERRORLEVEL% NEQ 0 GOTO exit_this_script
)

CALL %util_dir%\z_default_options.bat dummyParam
IF %ERRORLEVEL% NEQ 0 GOTO exit_this_script

IF EXIST %original_batch_dir%\z_customizations_post_default.bat (
   CALL %original_batch_dir%\z_customizations_post_default.bat dummyParam
   IF %ERRORLEVEL% NEQ 0 GOTO exit_this_script
)

CALL %util_dir%\%original_batch_file_name% dummyParam
IF %ERRORLEVEL% NEQ 0 GOTO exit_this_script

rem ============================================================================
GOTO exit_this_script

rem ============================================================================
:check_for_error_on_exit

IF %ERRORLEVEL% NEQ 0 (
	ECHO ****************************************
	ECHO ******
	ECHO ***
	ECHO.
	ECHO Error code: %ERRORLEVEL%
	ECHO.
	ECHO ***
	ECHO ******
	ECHO ****************************************
	PAUSE
	GOTO:EOF
) ELSE (
   IF /I "%pause_on_exit%"=="true" (
      PAUSE
      GOTO:EOF
   )
)

GOTO:EOF

rem ============================================================================
:exit_this_script
CALL :check_for_error_on_exit

