<#
.SYNOPSIS
    Generates a solution with a large number of MSTest projects, classes, and tests.

.DESCRIPTION
    Creates an MSTest solution using `dotnet new` templates, then generates additional
    test classes and test methods to produce a solution with a configurable number of
    projects, classes per project, and tests per class.

.PARAMETER OutputPath
    The directory where the solution will be created. Defaults to a "LargeTestSolution"
    folder next to the script directory.

.PARAMETER Projects
    Number of test projects to create. Default: 10.

.PARAMETER ClassesPerProject
    Number of test classes per project. Default: 100.

.PARAMETER TestsPerClass
    Number of test methods per class. Default: 20.

.PARAMETER Framework
    Target framework. Default: net10.0.

.PARAMETER UseMTP
    If set, configures projects to use Microsoft.Testing.Platform (MTP) runner
    instead of VSTest.

.EXAMPLE
    .\Generate-LargeTestSolution.ps1
    # Creates 10 projects x 100 classes x 20 tests = 20,000 tests

.EXAMPLE
    .\Generate-LargeTestSolution.ps1 -Projects 5 -ClassesPerProject 50 -TestsPerClass 10
    # Creates 5 projects x 50 classes x 10 tests = 2,500 tests

.EXAMPLE
    .\Generate-LargeTestSolution.ps1 -OutputPath "C:\temp\BigTests" -UseMTP
    # Creates in a custom location with MTP runner enabled
#>

[CmdletBinding()]
param(
    [Parameter()]
    [string]$OutputPath,

    [Parameter()]
    [int]$Projects = 10,

    [Parameter()]
    [int]$ClassesPerProject = 100,

    [Parameter()]
    [int]$TestsPerClass = 20,

    [Parameter()]
    [string]$Framework = "net10.0",

    [Parameter()]
    [switch]$UseMTP
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# Default output path to src folder next to script directory
if (-not $OutputPath) {
    $OutputPath = Join-Path (Split-Path $PSScriptRoot -Parent) "src" "LargeTestSolution"
}

$totalTests = $Projects * $ClassesPerProject * $TestsPerClass
Write-Host "Generating solution with $Projects projects, $ClassesPerProject classes/project, $TestsPerClass tests/class"
Write-Host "Total tests: $totalTests"
Write-Host "Output: $OutputPath"
Write-Host ""

# Clean and create output directory
if (Test-Path $OutputPath) {
    Write-Host "Removing existing directory: $OutputPath"
    Remove-Item -Recurse -Force $OutputPath
}
New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null

Push-Location $OutputPath
try {
    # Create solution
    Write-Host "Creating solution..."
    dotnet new sln -n LargeTestSolution --output . | Out-Null

    # If MTP, create global.json with test runner setting
    if ($UseMTP) {
        $globalJson = @{
            sdk = @{ version = "10.0.102" }
            test = @{ runner = "Microsoft.Testing.Platform" }
        } | ConvertTo-Json -Depth 3
        Set-Content -Path (Join-Path $OutputPath "global.json") -Value $globalJson -Encoding UTF8
        Write-Host "Created global.json with MTP runner configuration"
    }

    for ($p = 1; $p -le $Projects; $p++) {
        $projectName = "TestProject$p"
        $projectDir = Join-Path $OutputPath $projectName
        $paddedP = $p.ToString().PadLeft($Projects.ToString().Length, '0')

        Write-Host "Creating project $p/$Projects`: $projectName"

        # Use dotnet new to scaffold the MSTest project
        dotnet new mstest -n $projectName --output $projectDir --framework $Framework --force | Out-Null

        # Add project to solution
        dotnet sln add (Join-Path $projectDir "$projectName.csproj") | Out-Null

        # Configure csproj for MTP if needed
        $csprojPath = Join-Path $projectDir "$projectName.csproj"
        if ($UseMTP) {
            $csprojContent = Get-Content $csprojPath -Raw
            # Add MTP runner properties
            $csprojContent = $csprojContent -replace '(<PropertyGroup>)', @'
<PropertyGroup>
    <EnableMSTestRunner>true</EnableMSTestRunner>
    <OutputType>Exe</OutputType>
'@ -replace '<PropertyGroup>\s*<PropertyGroup>', '<PropertyGroup>'
            Set-Content -Path $csprojPath -Value $csprojContent -Encoding UTF8
        }

        # Remove the default test file created by the template
        $defaultTestFile = Join-Path $projectDir "Test1.cs"
        if (Test-Path $defaultTestFile) {
            Remove-Item $defaultTestFile
        }
        # Also remove any other default test files the template may create
        Get-ChildItem -Path $projectDir -Filter "*.cs" | Where-Object {
            $_.Name -ne "GlobalUsings.cs" -and $_.Name -ne "Usings.cs"
        } | Remove-Item -ErrorAction SilentlyContinue

        # Create assembly-level settings for parallel execution
        $settingsContent = @"
using Microsoft.VisualStudio.TestTools.UnitTesting;

[assembly: Parallelize(Scope = ExecutionScope.MethodLevel)]
"@
        Set-Content -Path (Join-Path $projectDir "TestSettings.cs") -Value $settingsContent -Encoding UTF8

        # Generate test classes
        for ($c = 1; $c -le $ClassesPerProject; $c++) {
            $paddedC = $c.ToString().PadLeft($ClassesPerProject.ToString().Length, '0')
            $className = "TestClass_${paddedP}_${paddedC}"
            $fileName = "$className.cs"
            $filePath = Join-Path $projectDir $fileName

            $sb = [System.Text.StringBuilder]::new(4096)
            [void]$sb.AppendLine("namespace $projectName")
            [void]$sb.AppendLine("{")
            [void]$sb.AppendLine("    [TestClass]")
            [void]$sb.AppendLine("    public sealed class $className")
            [void]$sb.AppendLine("    {")

            for ($t = 1; $t -le $TestsPerClass; $t++) {
                $paddedT = $t.ToString().PadLeft($TestsPerClass.ToString().Length, '0')
                $testName = "Test_${paddedC}_${paddedT}"

                if ($t -gt 1) {
                    [void]$sb.AppendLine()
                }
                [void]$sb.AppendLine("        [TestMethod]")
                [void]$sb.AppendLine("        public void $testName()")
                [void]$sb.AppendLine("        {")
                [void]$sb.AppendLine("        }")
            }

            [void]$sb.AppendLine("    }")
            [void]$sb.AppendLine("}")

            Set-Content -Path $filePath -Value $sb.ToString() -Encoding UTF8
        }

        Write-Host "  Created $ClassesPerProject classes with $TestsPerClass tests each"
    }

    # Detect the solution file (may be .sln or .slnx depending on SDK version)
    $slnFile = Get-ChildItem -Path $OutputPath -Filter "LargeTestSolution.sln*" | Select-Object -First 1
    $slnPath = $slnFile.FullName

    Write-Host ""
    Write-Host "Solution generated successfully!"
    Write-Host "  Location: $OutputPath"
    Write-Host "  Solution: $($slnFile.Name)"
    Write-Host "  Projects: $Projects"
    Write-Host "  Classes:  $($Projects * $ClassesPerProject)"
    Write-Host "  Tests:    $totalTests"
    Write-Host ""
    Write-Host "To build:  dotnet build `"$slnPath`""
    Write-Host "To test:   dotnet test `"$slnPath`""
}
catch {
    Write-Error "An error occurred: $_"
    throw
}
finally {
    Pop-Location
}
