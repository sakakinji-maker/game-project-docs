param([string]$OutputFolder='Temp/AttackFramesCompile')
$ErrorActionPreference='Stop'
$project=Join-Path $PSScriptRoot '../Sunfall'
$unity='C:/Program Files/Unity/Hub/Editor/6000.3.27f1/Editor/Data'
Push-Location $project
try {
 $output=$OutputFolder
 New-Item -ItemType Directory -Force -Path $output | Out-Null
 $bee='Library/Bee/artifacts/1900b0aE.dag'
 $log=[System.Collections.Generic.List[string]]::new()
 foreach($assembly in @('Core','Runtime','Editor','Tests')) {
  $name='Sunfall.'+$assembly
  $rsp=Get-Content ($bee+'/'+$name+'.rsp') -Raw
  $rsp=$rsp.Replace($bee+'/'+$name+'.dll',$output+'/'+$name+'.dll')
  $rsp=$rsp.Replace($bee+'/'+$name+'.ref.dll',$output+'/'+$name+'.ref.dll')
  $rsp=[regex]::Replace($rsp,'Library/Bee/artifacts/1900b0aE\.dag/(Sunfall\.[A-Za-z]+\.ref\.dll)',($output+'/$1'))
  foreach($source in Get-ChildItem ('Assets/Sunfall/'+$(if($assembly -eq 'Tests'){'Tests/EditMode'}else{$assembly})) -Filter '*.cs' -Recurse) {
   $relative=[IO.Path]::GetRelativePath((Get-Location).Path,$source.FullName).Replace('\','/')
   if(-not $rsp.Contains($relative)){$rsp+="`n"+'"'+$relative+'"'}
  }
  $response=$output+'/'+$name+'.rsp'
  [IO.File]::WriteAllText((Join-Path (Get-Location) $response),$rsp)
  $result=& ($unity+'/NetCoreRuntime/dotnet.exe') ($unity+'/DotNetSdkRoslyn/csc.dll') ('@'+$response) 2>&1
  $exit=$LASTEXITCODE
  $log.Add($name+' exit='+$exit)
  foreach($line in $result){$log.Add([string]$line)}
  $log | Set-Content ($output+'/compile.log')
  Write-Output ($name+' exit='+$exit)
  if($exit -ne 0){$result | Write-Output;throw 'Compilation failed'}
 }
 Write-Output 'Compile only: no Unity Play mode or tests executed.'
} finally {Pop-Location}
