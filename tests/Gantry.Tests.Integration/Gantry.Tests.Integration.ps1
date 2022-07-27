param ([string]$ModInfoPath, [string]$SourceFiles, [string]$ArchiveName)
$ModVersion = (Get-Content $ModInfoPath | ConvertFrom-Json).version
$AchivePath = $ArchiveName + '_v' + $ModVersion + '.zip'
Compress-Archive -Path $SourceFiles -DestinationPath $AchivePath -Force