$ErrorActionPreference='Stop'
Add-Type -AssemblyName System.Drawing
$refs=@([AppContext]::GetData('TRUSTED_PLATFORM_ASSEMBLIES').Split([IO.Path]::PathSeparator))+[System.Drawing.Bitmap].Assembly.Location
Add-Type -ReferencedAssemblies $refs -TypeDefinition @'
using System;
using System.Drawing;
public static class AttackArtBounds {
 public static int[] Measure(string path,int x0,int y0,int x1,int y1) {
  using(var b=new Bitmap(path)) {
   int l=x1,t=y1,r=-1,d=-1,edge=0,empty=0;
   for(int y=y0;y<y1;y++) for(int x=x0;x<x1;x++) {
    int a=b.GetPixel(x,y).A;
    if(a==0)empty++;
    if(a<=16)continue;
    l=Math.Min(l,x);t=Math.Min(t,y);r=Math.Max(r,x);d=Math.Max(d,y);
    if(x==x0||x==x1-1||y==y0||y==y1-1)edge++;
   }
   return new[]{l,t,r-l+1,d-t+1,edge,empty,b.Width,b.Height};
  }
 }
}
'@
$manifest=Get-Content (Join-Path $PSScriptRoot 'attack-art-layout.json') -Raw | ConvertFrom-Json
$result=@()
foreach($art in $manifest) {
 $dest=Join-Path $PSScriptRoot ('../Sunfall/Assets/Sunfall/Resources/'+$art.resource+'.png')
 Copy-Item -LiteralPath $art.path -Destination $dest
 $frames=@()
 for($i=0;$i -lt 8;$i++) {
  $row=[int][Math]::Floor($i/2)
  $left=if($i%2 -eq 0){0}else{$art.split[$row]}
  $right=if($i%2 -eq 0){$art.split[$row]}else{1024}
  $ys=if($i%2 -eq 0 -and $art.leftRows){$art.leftRows}else{$art.rows}
  $b=[AttackArtBounds]::Measure([string]$art.path,$left,$ys[$row],$right,$ys[$row+1])
  $frames+=,@{bounds=@($b[0],$b[1],$b[2],$b[3]);edgePixels=$b[4];transparentPixels=$b[5];pivot=$art.pivots[$i]}
 }
 $result+=@{name=$art.name;resource=$art.resource;scale=$art.scale;frames=$frames}
}
$result | ConvertTo-Json -Depth 8 | Set-Content (Join-Path $PSScriptRoot 'attack-art-bounds.json')
foreach($a in $result) { Write-Output ($a.name+': '+(($a.frames | ForEach-Object {($_.bounds -join ',')+' edge='+$_.edgePixels}) -join ' / ')) }
