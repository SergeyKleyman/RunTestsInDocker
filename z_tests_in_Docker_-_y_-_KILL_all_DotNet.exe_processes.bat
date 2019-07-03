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
ECHO Listing all dotnet.exe processes in Docker container %tests_Docker_container%...
ECHO.
docker exec %tests_Docker_container% tasklist /fi "IMAGENAME eq dotnet.exe"
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
ECHO KiLLiNG ALL Docker containers...
ECHO.
docker exec %tests_Docker_container% taskkill /IM "dotnet.exe" /F
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
ECHO Listing all dotnet.exe processes in Docker container %tests_Docker_container%...
ECHO.
docker exec %tests_Docker_container% tasklist /fi "IMAGENAME eq dotnet.exe"
ECHO.
ECHO #
ECHO ###
ECHO ######
ECHO ###########################################################

PAUSE

