# file_watcher
Monitor a given file location for create, change, rename and delete file events

This program monitors a given file location for create, change, rename and delete file events and
then exits with a specific OS exit code once triggered.

## Configuration

* The default path that is monitored is the `UserProfile` directory, defined by `pathToFolder`
* Paths (or partial paths, file names) can be ignored by placing them in the `ignore_list`
* The default OS exit code is defined by `success_exit_code`

In the source code, change these as needed:
* success_exit_code
* ignore_list
* pathToFolder

## Compilation

This program can be compiled on **any** Windows 10 system by running this command:
* `C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe /nologo /debug:full /out:.\file_watcher.exe /target:exe file_watcher.cs`

## Run

* To quit the program, press `Ctrl-C` or `Ctrl-Break`
