function ZipFiles( $zipfilename, $sourcedir )
{
    [Reflection.Assembly]::LoadWithPartialName( "System.IO.Compression.FileSystem" )
    $compressionLevel = [System.IO.Compression.CompressionLevel]::Optimal
    [System.IO.Compression.ZipFile]::CreateFromDirectory( $sourcedir, $zipfilename, $compressionLevel, $false )
}

if ([System.IO.File]::Exists("Terminals.zip"))
{
    [System.IO.File]::Delete("Terminals.zip")
}

Remove-Item "C:\KOHL\Terminals\Git\Terminals\Terminals\bin\x86\Distribution Release\Out\RAdmin Viewer 3" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item "C:\KOHL\Terminals\Git\Terminals\Terminals\bin\x86\Distribution Release\Out\putty.exe" -Force -ErrorAction SilentlyContinue
Remove-Item "C:\KOHL\Terminals\Git\Terminals\Terminals\bin\x86\Distribution Release\Out\QDir.exe" -Force -ErrorAction SilentlyContinue

ZipFiles Terminals.zip "C:\KOHL\Terminals\Git\Terminals\Terminals\bin\x86\Distribution Release\Out"

$strPath = 'C:\KOHL\Terminals\Git\Terminals\Terminals\bin\x86\Distribution Release\Terminals.exe'
$Assembly = [Reflection.Assembly]::Loadfile($strPath)
$Assemblyversion =  $Assembly.GetName().version

$string = ($Assemblyversion.Major.ToString()+"."+$Assemblyversion.Minor.ToString()+"."+$Assemblyversion.Build.ToString()+"."+$Assemblyversion.Revision.ToString());

Remove-Item -Force -Recurse (".\"+$string+"\") -ErrorAction SilentlyContinue
mkdir $string -Force -ErrorAction SilentlyContinue | Out-Null

Move-Item -Force Output\Setup_*.exe .\$string
Remove-Item -Force -Recurse Output -ErrorAction SilentlyContinue
Move-Item Terminals.zip .\$string\Terminals_$string.zip