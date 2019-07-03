IF "%1"=="" (
   ECHO ERROR: This batch file should not be called directly by user - exiting...
   PAUSE
   GOTO exit_this_script
)

ECHO ###########################################################
ECHO ######
ECHO ###
ECHO #
ECHO.
ECHO Listing Docker containers...
ECHO.
docker ps
ECHO.
ECHO #
ECHO ###
ECHO ######
ECHO ###########################################################

PAUSE

