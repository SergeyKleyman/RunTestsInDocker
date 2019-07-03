IF "%1"=="" (
   ECHO ERROR: This batch file should not be called directly by user - exiting...
   PAUSE
   GOTO exit_this_script
)

CALL %util_dir%\z_generate_timestamp.bat dummyParam
SET timestamp=%ret_val%

SET "Manager_component_important_log_files=IMPORTANT.log"
SET "Manager_component_important_log_files=%Manager_component_important_log_files% ERRORS.log"
SET "Manager_component_important_log_files=%Manager_component_important_log_files% active_components.log"
SET "Manager_component_important_log_files=%Manager_component_important_log_files% backend_comm.log"
SET "Manager_component_important_log_files=%Manager_component_important_log_files% KM_communication.log"
SET "Manager_component_important_log_files=%Manager_component_important_log_files% config.log"
SET "Manager_component_important_log_files=%Manager_component_important_log_files% file_mgmt.log"
SET "Manager_component_important_log_files=%Manager_component_important_log_files% gRPC_service.log"
SET "Manager_component_important_log_files=%Manager_component_important_log_files% HTTP_client_Apache.log"
SET "Manager_component_important_log_files=%Manager_component_important_log_files% HTTP_client_Ktor.log"
SET "Manager_component_important_log_files=%Manager_component_important_log_files% logging_subsystem.log"
SET "Manager_component_important_log_files=%Manager_component_important_log_files% starts.log"
SET "Manager_component_important_log_files=%Manager_component_important_log_files% workers_log_records.log"
SET "Manager_component_important_log_files=%Manager_component_important_log_files% workers_mgmt.log"
SET "Manager_component_important_log_files=%Manager_component_important_log_files% workers_std_ERR.log"
SET "Manager_component_important_log_files=%Manager_component_important_log_files% workers_std_out.log"

SET "Manager_component_DETAILED_log_files=active_components_detailed.log"
SET "Manager_component_DETAILED_log_files=%Manager_component_DETAILED_log_files% backend_comm_detailed.log"
SET "Manager_component_DETAILED_log_files=%Manager_component_DETAILED_log_files% KM_communication_detailed.log"
SET "Manager_component_DETAILED_log_files=%Manager_component_DETAILED_log_files% config_detailed.log"
SET "Manager_component_DETAILED_log_files=%Manager_component_DETAILED_log_files% coroutines_state.log"
SET "Manager_component_DETAILED_log_files=%Manager_component_DETAILED_log_files% everything.log"
SET "Manager_component_DETAILED_log_files=%Manager_component_DETAILED_log_files% file_mgmt_detailed.log"
SET "Manager_component_DETAILED_log_files=%Manager_component_DETAILED_log_files% gRPC_service_detailed.log"
SET "Manager_component_DETAILED_log_files=%Manager_component_DETAILED_log_files% HTTP_client_Apache_detailed.log"
SET "Manager_component_DETAILED_log_files=%Manager_component_DETAILED_log_files% HTTP_client_Ktor_detailed.log"
SET "Manager_component_DETAILED_log_files=%Manager_component_DETAILED_log_files% starts_detailed.log"
SET "Manager_component_DETAILED_log_files=%Manager_component_DETAILED_log_files% workers_log_records_detailed.log"
SET "Manager_component_DETAILED_log_files=%Manager_component_DETAILED_log_files% workers_mgmt_detailed.log"

SET "Worker_component_log_files=agent.log"
SET "Worker_component_log_files=%Worker_component_log_files% cache.log"
SET "Worker_component_log_files=%Worker_component_log_files% memory.log"
SET "Worker_component_log_files=%Worker_component_log_files% schedule_manager.log"
SET "Worker_component_log_files=%Worker_component_log_files% startup.log"
SET "Worker_component_log_files=%Worker_component_log_files% sync_locks.log"


rem ###
rem ### www.baremetalsoft.com/baretailpro/usage.php?app=BareTailPro&ver=2.50aR&date=2006-11-02
rem ###
rem ### --window-state 0 | 1 | 2
rem ###
rem ###     Specifies the window state at startup:
rem ###     0
rem ###         Normal state (neither minimised or maximised)
rem ###     1
rem ###         Minimised
rem ###     2
rem ###         Maximised

SET BareTailPro_exe_cmd_line_prefix=START "Executing '%BareTailPro_exe%'..." /max /B %BareTailPro_exe% --window-state 2

PUSHD %workspaces_root_dir%\%workspace_name%\Manager\%Manager_component_WORK_subdir%\%Manager_component_LOGs_subdir%
%BareTailPro_exe_cmd_line_prefix% %Manager_component_important_log_files%
PAUSE
%BareTailPro_exe_cmd_line_prefix% %Manager_component_DETAILED_log_files%
POPD

PUSHD %workspaces_root_dir%\%workspace_name%\Worker\%Worker_component_WORK_subdir%\%Worker_component_LOGs_subdir%
PAUSE
%BareTailPro_exe_cmd_line_prefix% %Worker_component_log_files%
POPD


rem ============================================================================
GOTO exit_this_script

rem ============================================================================
:exit_this_script

