IF "%1"=="" (
   ECHO ERROR: This batch file should not be called directly by user - exiting...
   PAUSE
   GOTO exit_this_script
)

SET time_zero_padded=%time: =0%

SET "ret_val=%date%_%time_zero_padded:~0,2%-%time_zero_padded:~3,2%-%time_zero_padded:~6,2%"

