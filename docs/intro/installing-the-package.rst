.. _refInstalling:

Installing
==========

The latest package is available on `NuGet`_. 

.. _NuGet: https://www.nuget.org/packages/Stravaig.Gedcom

+------------------------------------------------------------------------------------------------------------+---------------------------+
| Badge                                                                                                      | Notes                     |
+------------------------------------------------------------------------------------------------------------+---------------------------+
| .. image:: https://img.shields.io/nuget/v/Stravaig.Gedcom?color=004880&label=nuget%20stable&logo=nuget     | Latest stable             |
+------------------------------------------------------------------------------------------------------------+---------------------------+
| .. image:: https://img.shields.io/nuget/vpre/Stravaig.Gedcom?color=ffffff&label=nuget%20latest&logo=nuget) | Latest, including preview |
+------------------------------------------------------------------------------------------------------------+---------------------------+

Installing from a PowerShell prompt
-----------------------------------

You can install the package into your project from a PowerShell prompt. Navigate to the folder your project file is in and type: ::

    Install-Package Stravaig.Gedcom

If you want to install a specific version add ``-Version <version>``

Installing using the .NET CLI
-----------------------------

You can install the package into your project with the .NET CLI command. Navigate to the folder your project file is in and type:

::

    dotnet add package Stravaig.Gedcom

If you want to add a specific version add ``--version <version>`` to the end of the command. If you want the lastest prerelease version you can add ``--prerelease`` instead.