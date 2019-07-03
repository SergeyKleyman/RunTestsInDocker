@rem ============================================================================
@rem === 
@rem === main / begin
@rem === 

IF "%1"=="" (
   ECHO ERROR: This batch file should not be called directly by user - exiting...
   PAUSE
   GOTO exit_this_script
)


SET dotnetRunCmdLine=dotnet run --
SET dotnetRunCmdLine=%dotnetRunCmdLine% --docker_container=%tests_Docker_container%
SET dotnetRunCmdLine=%dotnetRunCmdLine% --solution_directory=%workspaces_root_dir%\%workspace_name%
SET dotnetRunCmdLine=%dotnetRunCmdLine% --shared_with_docker_directory=%original_batch_dir%\shared_with_Docker
SET dotnetRunCmdLine=%dotnetRunCmdLine% --env_vars_file=%original_batch_dir%\zz_tests_in_Docker_-_env_vars.properties
SET dotnetRunCmdLine=%dotnetRunCmdLine% --dotnet_test_verbosity=%tests_Docker_dotnet_test_verbosity%

IF NOT "%tests_Docker_run_only_project%"=="" (
    SET dotnetRunCmdLine=%dotnetRunCmdLine% --limit_to_test_project=%tests_Docker_run_only_project%
)

IF NOT "%tests_Docker_run_only_filter%"=="" (
    SET dotnetRunCmdLine=%dotnetRunCmdLine% --limit_to_test_filter=%tests_Docker_run_only_filter%
)

PUSHD "%util_dir%\Utlz.RunTestsInDocker"
%dotnetRunCmdLine%
SET last_exit_code=%ERRORLEVEL%
POPD
IF NOT "%last_exit_code%" == "0" GOTO exit_this_script

GOTO exit_this_script

@rem === 
@rem === main / end
@rem === 
@rem ============================================================================

@rem ============================================================================
GOTO exit_this_script

@rem ============================================================================
:exit_this_script
SET pause_on_exit=true
EXIT /B %last_exit_code%

