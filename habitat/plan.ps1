$pkg_name="CustomActionFastMsi"
$pkg_origin="chef"
$pkg_version="0.2.0"
$pkg_maintainer="The Habitat Maintainers <humans@habitat.sh>"
$pkg_license=@("Apache-2.0")
$pkg_build_deps=@(
  "core/visual-build-tools-2019",
  "core/wix",
  "core/dotnet-45-dev-pack"
)
$pkg_bin_dirs=@("bin")

function Invoke-Build {
  Copy-Item $PLAN_CONTEXT/../* $HAB_CACHE_SRC_PATH/$pkg_dirname -recurse -force
  Copy-Item "$(Get-HabPackagePath wix)\bin\Microsoft.Deployment.WindowsInstaller.dll" "$HAB_CACHE_SRC_PATH\$pkg_dirname\CustomActionFastMsi\ext"
  MSBuild $HAB_CACHE_SRC_PATH/$pkg_dirname/CustomActionFastMsi/CustomActionFastMsi.csproj /t:Build /p:Configuration=Release /p:Platform=x86
  if($LASTEXITCODE -ne 0) {
    Write-Error "dotnet build failed!"
  }
}

function Invoke-Install {
  $fileList = ""
  @(
    "Microsoft.Deployment.WindowsInstaller.dll",
    "ext\7z.dll",
    "ext\7z.exe",
    "ext\License.txt",
    "CustomAction.config"
  ) | ForEach-Object {
    $fileList += "$HAB_CACHE_SRC_PATH\$pkg_dirname\CustomActionFastMsi\bin\Release\$_;"
  }
  ."$(Get-HabPackagePath wix)\bin\sdk\MakeSfxCA.exe" "$pkg_prefix/bin/CustomActionFastMsi.CA.dll" "$(Get-HabPackagePath wix)\bin\sdk\x86\sfxca.dll" "$HAB_CACHE_SRC_PATH\$pkg_dirname\CustomActionFastMsi\bin\Release\CustomActionFastMsi.dll" $fileList
}
