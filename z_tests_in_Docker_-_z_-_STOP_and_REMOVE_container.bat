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
ECHO Before stopping Docker container %tests_Docker_container%
ECHO.
docker ps
ECHO.
ECHO #
ECHO ###
ECHO ######
ECHO ###########################################################

ECHO ###########################################################
ECHO ######
ECHO ###
ECHO #
ECHO.
ECHO Stopping Docker container %tests_Docker_container% ...
ECHO.
docker stop %tests_Docker_container%
ECHO.
ECHO #
ECHO ###
ECHO ######
ECHO ###########################################################


ECHO ###########################################################
ECHO ######
ECHO ###
ECHO #
ECHO.
ECHO After stopping Docker container %tests_Docker_container%
ECHO.
docker ps
ECHO.
ECHO #
ECHO ###
ECHO ######
ECHO ###########################################################

PAUSE

