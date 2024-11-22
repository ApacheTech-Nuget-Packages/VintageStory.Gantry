@echo off
REM ========================================================
REM This script updates the Git repository in the current
REM directory, synchronises changes, triggers GitHub workflows,
REM and copies the ".github" folder into the repository.
REM ========================================================

REM Store the current directory for later use.
SET CurrentDirectory=%cd%

REM Echo feedback about the current repository being processed.
ECHO Processing repository: %CurrentDirectory%

REM Copy the ".github" folder from the root directory to the repository.
ECHO Copying .github folder into the current repository...
xcopy "..\.github" ".github" /E /I /Y

REM Echo feedback about deleting the Git index.
ECHO Removing Git index in the current repository...
del ".git\index" /f

REM Refresh the Git index and add all changes.
ECHO Refreshing Git index...
git update-index --really-refresh

REM Add all changes.
ECHO Staging changes...
git add .

REM Commit the changes with a standard message.
ECHO Committing changes...
git commit -m "Synchronising changes in the current repository"

REM Push the committed changes to the remote repository.
ECHO Pushing changes to remote repository...
git push

REM Trigger the specified GitHub workflow (if applicable).
ECHO Triggering GitHub workflow...
gh workflow run "Manage Labels"

REM Pause to keep the console open after the script finishes.
PAUSE