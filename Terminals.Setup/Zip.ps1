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

Remove-Item "D:\Kohl\Projects\Code\Terminals\Terminals\bin\x86\Distribution Release\Out\RAdmin Viewer 3" -Recurse -Force
Remove-Item "D:\Kohl\Projects\Code\Terminals\Terminals\bin\x86\Distribution Release\Out\putty.exe" -Force
Remove-Item "D:\Kohl\Projects\Code\Terminals\Terminals\bin\x86\Distribution Release\Out\QDir.exe" -Force


ZipFiles Terminals.zip "D:\Kohl\Projects\Code\Terminals\Terminals\bin\x86\Distribution Release\Out"

$strPath = 'D:\Kohl\Projects\Code\Terminals\Terminals\bin\x86\Distribution Release\Out\Terminals.exe'
$Assembly = [Reflection.Assembly]::Loadfile($strPath)
$Assemblyversion =  $Assembly.GetName().version

$string = ("Terminals_"+$Assemblyversion.Major+"."+$Assemblyversion.Minor+"."+$Assemblyversion.Build+"."+$Assemblyversion.Revision);

mv Terminals.zip ($string + ".zip") -Force
mv Terminals.exe ($string + ".exe") -Force