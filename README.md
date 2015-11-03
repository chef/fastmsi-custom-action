Custom Action for Fast MSI
============================

This repository contains a WIX/MSI Custom Action that supports the Chef and
ChefDK Omnibus 'FastMSI' packager for Windows.

The FastMSI packager speeds up installs by creating a zip archive that is
simply unarchived to lay down the destination folder structure on install.

Acknowledgement
---------------

This custom action depends on the SharpZipLip library to do the unzip.

Please see https://icsharpcode.github.io/SharpZipLib/ for more info.

Usage
-----
### Build

The root directory contains a Visual Studio 2015 solution file that
can be used for building the custom action.

The project targets .Net Framework 2.0 in order to be compatible with
legacy Windows.

The WIX toolset must be installed on the machine in order to build. The
recommended version is 3.10 or greater.

The build will produce a ```CustomActionFastMsi.CA.dll``` which will need
to be packaged into the MSI by the WIX tools (see below).

### WIX Integration

The following code snippet illustrates show to use the code from within
a WIX source file:

First, define the custom action and properties it requires:

```shell
<SetProperty Id="FastUnzip"
             Value="FASTZIPDIR=[INSTALLLOCATION];FASTZIPAPPNAME=yourapp"
             Sequence="execute"
             Before="FastUnzip" />

 <CustomAction Id="FastUnzip"
               BinaryKey="CustomActionFastMsiDLL"
               DllEntry="FastUnzip"
               Execute="deferred"
               Return="check"
               Impersonate="no" />

 <Binary Id="CustomActionFastMsiDLL"
         SourceFile="CustomActionFastMsi.CA.dll" />
```

Then, add the custom action into the install sequence:

```shell
<InstallExecuteSequence>
  <Custom Action="FastUnzip" After="InstallFiles">NOT Installed</Custom>
</InstallExecuteSequence>
```

Use the WIX tools (heat/candle/light) to build your MSI as usual. Make
sure that the generated custom action dll is in your staging folder so that
it can be included into your MSI package.

### Troubleshooting

1. Use the Orca tool to check your generated MSI and make sure the custom
action is present in the MSI.

2. Use verbose logging to check for errors, etc:
```shell
msiexec /i package.msi /l*v install.log
```

License
-------
```text
Copyright 2015 Chef Software, Inc.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
```
