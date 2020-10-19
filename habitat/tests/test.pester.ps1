param (
    [Parameter()]
    [string]$PackageIdentifier = $(throw "Fully qualified package identifier must be given as a parameter.")
)

Describe "CustomActionFastMsi" {
    Push-Location $PSScriptRoot
    hab pkg install core/wix
    Copy-Item "$(hab pkg path $env:HAB_ORIGIN/CustomActionFastMsi)\bin\CustomActionFastMsi.CA.dll" .
    hab pkg exec core/wix candle.exe -nologo -arch x64 "project-files.wxs" "Product.wxs"
    hab pkg exec core/wix light.exe -nologo project-files.wixobj Product.wixobj -out test.msi
    Pop-Location

    It "expands zip" {
        Start-Process msiexec -Wait -ArgumentList "/qn /i $PSScriptRoot\test.msi"
        Test-Path "C:\SetupProject1\test\modules\chef\chef.psm1" | Should -Be $true
    }

    AfterAll {
        Remove-Item $PSScriptRoot\CustomActionFastMsi.CA.dll
        Remove-Item $PSScriptRoot\test.msi
        Remove-Item $PSScriptRoot\*.wixobj
        Remove-Item $PSScriptRoot\*.wixpdb
        Remove-Item C:\SetupProject1 -Recurse -Force
    }
}
