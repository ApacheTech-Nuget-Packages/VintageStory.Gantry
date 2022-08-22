@echo off
for /D %%G in ("%cd%\..\src\*") DO (
	@echo %%~nxG
	dotnet build "%cd%\..\src\%%~nxG\%%~nxG.csproj"
	dotnet pack "%cd%\..\src\%%~nxG\%%~nxG.csproj" -o "%cd%\..\.publish" --include-symbols --include-source --no-build --no-restore
)