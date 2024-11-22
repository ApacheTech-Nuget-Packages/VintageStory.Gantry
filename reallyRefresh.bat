REM Store the current directory for later use.
SET CurrentDirectory=%cd%

REM Echo feedback about the current directory being processed.
ECHO Processing repository: %%~nxG

REM Echo feedback about deleting the Git index.
ECHO Removing Git index in %%~nxG...
del ".git\index" /f

REM Refresh the Git index and add all changes.
ECHO Refreshing Git index...
git update-index --really-refresh

REM Add all changes.
ECHO Staging changes...
git add .