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
ECHO Before starting Docker container %tests_Docker_container%
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
ECHO Starting Docker container %tests_Docker_container% ...
ECHO.
docker run -d -P --rm --name %tests_Docker_container% --mount type=bind,source="%original_batch_dir%\shared_with_Docker",target="C:/shared_with_Docker" iis
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
ECHO After starting Docker container %tests_Docker_container%
ECHO.
docker ps
ECHO.
ECHO #
ECHO ###
ECHO ######
ECHO ###########################################################

PAUSE

